using System;
using System.Collections.Generic;

namespace Cmas.BusinessLayers.CallOffOrders.Entities
{
    /// <summary>
    /// Наряд заказ
    /// </summary>
    public class CallOffOrder
    {
        /// <summary>
        /// Уникальный внутренний идентификатор
        /// </summary>
        public string Id;

        /// <summary>
        /// Идентификатор договора
        /// </summary>
        public string ContractId;

        /// <summary>
        /// Номер наряд заказа
        /// </summary>
        public string Number;

        /// <summary>
        /// ФИО
        /// </summary>
        public string Assignee;

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTime CreatedAt;

        /// <summary>
        /// Дата и время обновления
        /// </summary>
        public DateTime UpdatedAt;

        /// <summary>
        /// Дата начала действия наряд-заказа
        /// </summary>
        public string StartDate;

        /// <summary>
        /// Дата окончания действия наряд-заказа
        /// </summary>
        public string FinishDate;

        /// <summary>
        /// Наименование заказа (по сути - работы)
        /// </summary>
        public string Name;

        /// <summary>
        /// Должность
        /// </summary>
        public string Position;

        /// <summary>
        /// Место работы
        /// </summary>
        public string Location;

        /// <summary>
        /// Системное имя шаблона НЗ
        /// </summary>
        public string TemplateSysName;  // 'Default', 'Annotech'
        
         /// <summary>
        /// Табельный номер
        /// </summary>
        public string EmployeeNumber;

        /// <summary>
        /// Номер позиции
        /// </summary>
        public string PositionNumber;

        /// <summary>
        /// Происхождение персонала
        /// </summary>
        public string PersonnelSource;

        /// <summary>
        /// Номер PAAF
        /// </summary>
        public string Paaf;

        /// <summary>
        /// Ссылка плана мобилизации
        /// </summary>
        public string MobPlanReference;

        /// <summary>
        /// Дата мобилизации
        /// </summary>
        public string MobDate;


        /// <summary>
        /// Шаблонные данные <имя параметра, значение>
        /// </summary>
        public Dictionary<string, object> TemplateData; 

        /// <summary>
        /// Ставки
        /// </summary>
        public ICollection<Rate> Rates;

        public CallOffOrder()
        {
            Rates = new List<Rate>();
            TemplateSysName = "default";
            TemplateData = new Dictionary<string, object>();
        }

    }
}
