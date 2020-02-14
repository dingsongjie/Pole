using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public class EntityTypeResolver : IEntityTypeResolver
    {
        public virtual Type ResolveEntityType(string grainType, IGrainState grainState)
        {
            return ResolveStateType(grainType, grainState);
        }

        public virtual Type ResolveStateType(string grainType, IGrainState grainState)
        {
            // todo: hack, the declared type of the grain state is only accessible like so
            return grainState.GetType().IsGenericType
                ? grainState.GetType().GenericTypeArguments[0]
                : grainState.State.GetType();
        }
    }
}
