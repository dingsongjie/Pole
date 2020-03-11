using Pole.Sagas.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class SagasGroupEntity
    {
        public string ServiceName { get; set; }
        public List<SagaEntity> SagaEntities { get; set; }
    }
}
