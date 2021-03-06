﻿using System;

namespace Cmas.BusinessLayers.CallOffOrders.Entities
{
   public class Rate
    {
        public string Id;

        public String Name;

        public bool IsRate;

        public string ParentId;

        /// <summary>
        /// Ставка
        /// </summary>
        public double Amount;

        /// <summary>
        /// Ед. изм.
        /// </summary>
        public RateUnit Unit;
    }
}
