using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public class EntityWapper<TEntity> where TEntity : class
    {
        public TEntity Entity { get; set; }
        public bool ToAdd { get; set; }
        public bool IsAdded { get; set; }
    }
}
