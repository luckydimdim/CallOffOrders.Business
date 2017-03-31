using System;

namespace Cmas.BusinessLayers.CallOffOrders.Entities
{
   public class Rate
    {
        public int Id;

        public String Name;

        public bool IsRate;

        public int? ParentId;

        /// <summary>
        /// Ставка
        /// </summary>
        public double Amount;

        /// <summary>
        /// Валюта
        /// </summary>
        public String Currency;

        /// <summary>
        /// Ед. изм.
        /// </summary>
        public String UnitName;
    }
}
