﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;
using Pole.Core.Domain;
using Pole.EventBus;
using Pole.EventBus.Transaction;
using Pole.EventBus.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    internal class GrainStorage<TContext, TGrain, TGrainState, TEntity> : IGrainStorage
        where TContext : DbContext
        where TGrain : Grain<TGrainState>
        where TGrainState : class, new()
        where TEntity : Entity
    {
        private readonly GrainStorageOptions<TContext, TGrain, TEntity> _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GrainStorage<TContext, TGrain, TGrainState, TEntity>> _logger;
        private readonly IServiceProvider _serviceProvider;

        public GrainStorage(string grainType, IServiceProvider serviceProvider)
        {
            if (grainType == null) throw new ArgumentNullException(nameof(grainType));

            _serviceProvider = serviceProvider
                               ?? throw new ArgumentNullException(nameof(serviceProvider));

            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory?.CreateLogger<GrainStorage<TContext, TGrain, TGrainState, TEntity>>()
                      ?? NullLogger<GrainStorage<TContext, TGrain, TGrainState, TEntity>>.Instance;

            _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            _options = GetOrCreateDefaultOptions(grainType);
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<TContext>())
            {
                TEntity entity = await _options.ReadStateNoTrackingAsync(context, grainReference)
                    .ConfigureAwait(false);
                _options.SetEntity(grainState, entity);

                if (entity != null && _options.CheckForETag)
                    grainState.ETag = _options.GetETagFunc(entity);
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            TEntity entity = _options.GetEntity(grainState);

            using (IServiceScope scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<TContext>())
            {
                if (GrainStorageContext<TEntity>.IsConfigured)
                {
                    EntityEntry<TEntity> entry = context.Entry(entity);
                    GrainStorageContext<TEntity>.ConfigureStateDelegate(entry);
                }
                else
                {
                    bool isPersisted = _options.IsPersistedFunc(entity);
                    if (isPersisted)
                    {
                        if (_options.IsRelatedData)
                        {
                            TEntity entityInDb = await _options.ReadStateAsync(context, grainReference)
                                    .ConfigureAwait(false);
                            Copy(entity, entityInDb);
                        }
                        else
                        {
                            context.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        context.Set<TEntity>().Add(entity);
                    }
                }

                try
                {
                    if (entity.DomainEvents!=null&&entity.DomainEvents.Count != 0)
                    {
                        using (var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>())
                        {
                            using (var dbTransactionAdapter = scope.ServiceProvider.GetRequiredService<IDbTransactionAdapter>())
                            {
                                var bus = scope.ServiceProvider.GetRequiredService<IBus>();
                                using (var transaction = await context.Database.BeginTransactionAsync())
                                {
                                    dbTransactionAdapter.DbTransaction = transaction;
                                    unitOfWork.Enlist(dbTransactionAdapter, bus);
                                    var publishTasks = entity.DomainEvents.Select(m => bus.Publish(m));
                                    await Task.WhenAll(publishTasks);
                                    await context.SaveChangesAsync().ConfigureAwait(false);

                                    if (_options.CheckForETag)
                                        grainState.ETag = _options.GetETagFunc(entity);

                                    await unitOfWork.CompeleteAsync();
                                }
                            }                            
                        };
                    }
                    else
                    {
                        await context.SaveChangesAsync().ConfigureAwait(false);

                        if (_options.CheckForETag)
                            grainState.ETag = _options.GetETagFunc(entity);
                    }
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!_options.CheckForETag)
                        throw new InconsistentStateException(e.Message, e);

                    object storedETag = e.Entries.First().OriginalValues[_options.ETagProperty];
                    throw new InconsistentStateException(e.Message,
                        _options.ConvertETagObjectToStringFunc(storedETag),
                        grainState.ETag,
                        e);
                }
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            TEntity entity = _options.GetEntity(grainState);
            using (IServiceScope scope = _scopeFactory.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<TContext>())
            {
                context.Remove(entity);
                await context.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
        }

        public static void Copy<T>(T from, T to) where T : Entity
        {
            if (ReferenceEquals(from, null))
                throw new ArgumentNullException("from");
            if (ReferenceEquals(to, null))
                throw new ArgumentNullException("to");
            Type type = from.GetType();
            PropertyInfo[] Properties = type.GetProperties();
            foreach (PropertyInfo p in Properties)
            {
                if (p.Name == "DomainEvents" || p.Name == "Id") continue;
                p.SetValue(to, p.GetValue(from));
            }
        }
        private GrainStorageOptions<TContext, TGrain, TEntity> GetOrCreateDefaultOptions(string grainType)
        {
            var options
                = _serviceProvider.GetOptionsByName<GrainStorageOptions<TContext, TGrain, TEntity>>(grainType);

            if (options.IsConfigured)
                return options;

            // Try generating a default options for the grain

            Type optionsType = typeof(GrainStoragePostConfigureOptions<,,,>)
                .MakeGenericType(
                    typeof(TContext),
                    typeof(TGrain),
                    typeof(TGrainState),
                    typeof(TEntity));

            var postConfigure = (IPostConfigureOptions<GrainStorageOptions<TContext, TGrain, TEntity>>)
                Activator.CreateInstance(optionsType, _serviceProvider);

            postConfigure.PostConfigure(grainType, options);

            _logger.LogInformation($"GrainStorageOptions is not configured for grain {grainType} " +
                                   "and default options will be used. If default configuration is not desired, " +
                                   "consider configuring options for grain using " +
                                   "using IServiceCollection.ConfigureGrainStorageOptions<TContext, TGrain, TState> extension method.");

            return options;
        }
    }
}
