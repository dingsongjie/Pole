﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
    }
}
