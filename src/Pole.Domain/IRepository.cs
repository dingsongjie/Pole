using Pole.Core.DependencyInjection;
using Pole.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain
{
    public interface IRepository<T> : IRepository where T :  IAggregateRoot
    {
        void Update(T entity);
        void Delete(T entity);
        void Add(T entity);
        Task<T> Get(string id);
        IUnitOfWork UnitOfWork { get; }
    }
    public interface IRepository: IScopedDenpendency
    {

    }
}
