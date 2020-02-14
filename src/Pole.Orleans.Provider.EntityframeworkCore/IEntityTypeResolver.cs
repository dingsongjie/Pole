using Orleans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public interface IEntityTypeResolver
    {
        Type ResolveEntityType(string grainType, IGrainState grainState);
        Type ResolveStateType(string grainType, IGrainState grainState);
    }
}
