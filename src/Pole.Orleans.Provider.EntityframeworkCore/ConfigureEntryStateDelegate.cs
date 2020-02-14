using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public delegate void ConfigureEntryStateDelegate<TGrainState>(EntityEntry<TGrainState> entry)
        where TGrainState : class;
}
