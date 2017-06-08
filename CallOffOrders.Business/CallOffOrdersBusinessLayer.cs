using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cmas.BusinessLayers.CallOffOrders.CommandsContexts;
using Cmas.BusinessLayers.CallOffOrders.Criteria;
using Cmas.BusinessLayers.CallOffOrders.Entities;
using Cmas.Infrastructure.Domain.Commands;
using Cmas.Infrastructure.Domain.Queries;
using Cmas.Infrastructure.Domain.Criteria;

namespace Cmas.BusinessLayers.CallOffOrders
{
    public class CallOffOrdersBusinessLayer
    {
        private readonly ICommandBuilder _commandBuilder;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public CallOffOrdersBusinessLayer(IServiceProvider serviceProvider, ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
            _commandBuilder = (ICommandBuilder) serviceProvider.GetService(typeof(ICommandBuilder));
            _queryBuilder = (IQueryBuilder) serviceProvider.GetService(typeof(IQueryBuilder));
        }

        /// <summary>
        /// Получить наряд заказ
        /// </summary>
        public async Task<CallOffOrder> GetCallOffOrder(string id)
        {
            return await _queryBuilder.For<Task<CallOffOrder>>().With(new FindById(id));
        }

        /// <summary>
        /// Получить все наряд заказы
        /// </summary>
        public async Task<IEnumerable<CallOffOrder>> GetCallOffOrders()
        {
            return await _queryBuilder.For<Task<IEnumerable<CallOffOrder>>>().With(new AllEntities());
        }

        /// <summary>
        /// Получить наряд заказы по договору
        /// </summary>
        public async Task<IEnumerable<CallOffOrder>> GetCallOffOrders(string contractId)
        {
            if (string.IsNullOrEmpty(contractId))
                throw new ArgumentException("contractId");

            return await _queryBuilder.For<Task<IEnumerable<CallOffOrder>>>().With(new FindByContractId(contractId));
        }

        /// <summary>
        /// Создать наряд заказ
        /// </summary>
        /// <param name="contractId">ID договора</param>
        /// <param name="templateSysName">Системное имя шаблона</param>
        /// <param name="currencySysName">Валюта наряд заказа</param>
        /// <returns></returns>
        public async Task<string> CreateCallOffOrder(CallOffOrder сallOffOrder)
        {
            сallOffOrder.UpdatedAt = DateTime.UtcNow;
            сallOffOrder.CreatedAt = DateTime.UtcNow;
            сallOffOrder.Id = null;

            var context = new CreateCallOffOrderCommandContext {CallOffOrder = сallOffOrder };

            context = await _commandBuilder.Execute(context);

            return context.Id;
        }

        /// <summary>
        /// Удалить наряд заказ
        /// </summary>
        public async Task<string> DeleteCallOffOrder(string id)
        {
            var context = new DeleteCallOffOrderCommandContext
            {
                Id = id
            };

            context = await _commandBuilder.Execute(context);

            return context.Id;
        }

        /// <summary>
        /// Обновить наряд заказ
        /// </summary>
        public async Task<string> UpdateCallOffOrder(string callOffOrderId, CallOffOrder order)
        {
            if (string.IsNullOrEmpty(callOffOrderId))
                throw new ArgumentException("callOffOrderId");

            order.Id = callOffOrderId;

            order.UpdatedAt = DateTime.UtcNow;

            var context = new UpdateCallOffOrderCommandContext
            {
                CallOffOrder = order
            };

            context = await _commandBuilder.Execute(context);

            return context.CallOffOrder.Id;
        }

        /// <summary>
        /// Добавить ставку/группу
        /// </summary>
        public async Task<Rate> AddRate(string callOffOrderId, Rate rate)
        {
            if (string.IsNullOrEmpty(callOffOrderId))
                throw new ArgumentException("callOffOrderId");

            CallOffOrder callOffOrder = await _queryBuilder.For<Task<CallOffOrder>>()
                .With(new FindById(callOffOrderId));

            if (string.IsNullOrEmpty(rate.Id))
            {
                rate.Id = Guid.NewGuid().ToString();
            }

            if (rate.IsRate && !string.IsNullOrEmpty(rate.ParentId))
            {
                int groupIndex = -1;
                int lastRateInGroupIndex = -1;


                // вставляем ставку в определенную группу
                for (int i = 0; i < callOffOrder.Rates.Count; i++)
                {
                    if (callOffOrder.Rates[i].Id == rate.ParentId)
                    {
                        groupIndex = i;
                    }
                    else if (callOffOrder.Rates[i].ParentId == rate.ParentId)
                    {
                        lastRateInGroupIndex = i;
                    }
                }

                if (lastRateInGroupIndex >= 0)
                {
                    callOffOrder.Rates.Insert(lastRateInGroupIndex + 1, rate);
                }
                else if (groupIndex >= 0)
                {
                    callOffOrder.Rates.Insert(groupIndex + 1, rate);
                }
                else
                {
                    throw new ArgumentException("Cannot find group with id = " + rate.ParentId);
                }
            }
            else
            {
                // вставляем в конец списка
                callOffOrder.Rates.Add(rate);
            }

            await UpdateCallOffOrder(callOffOrderId, callOffOrder);

            return rate;
        }

        /// <summary>
        /// Удалить ставку/группу
        /// </summary>
        public async Task DeleteRate(string callOffOrderId, string rateId)
        {
            if (string.IsNullOrEmpty(callOffOrderId))
                throw new ArgumentException("callOffOrderId");

            if (string.IsNullOrEmpty(rateId))
                throw new ArgumentException("rateId");

            CallOffOrder callOffOrder = await _queryBuilder.For<Task<CallOffOrder>>()
                .With(new FindById(callOffOrderId));

            var rateForRemove = callOffOrder.Rates.Where(r => r.Id == rateId).SingleOrDefault();

            if (rateForRemove == null)
                throw new Exception($"Rate with id {rateId} not found");

            var removesCount = callOffOrder.Rates.Remove(rateForRemove);

            await UpdateCallOffOrder(callOffOrderId, callOffOrder);
        }

        /// <summary>
        /// Обновить ставку/группу
        /// </summary>
        public async Task UpdateRate(string callOffOrderId, Rate rate)
        {
            if (string.IsNullOrEmpty(callOffOrderId))
                throw new ArgumentException("callOffOrderId");

            CallOffOrder callOffOrder = await _queryBuilder.For<Task<CallOffOrder>>()
                .With(new FindById(callOffOrderId));

            bool found = false;
            for (int i = 0; i < callOffOrder.Rates.Count; i++)
            {
                if (callOffOrder.Rates[i].Id == rate.Id)
                {
                    callOffOrder.Rates[i] = rate;
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new Exception($"Rate with id {rate.Id} not found");

            await UpdateCallOffOrder(callOffOrderId, callOffOrder);
        }

        /// <summary>
        /// Получить название ед. измерения ставки
        /// </summary>
        public static string GetRateUnitName(RateUnit rateUnit)
        {
            switch (rateUnit)
            {
                case RateUnit.Hour:
                    return "Час";
                case RateUnit.Day:
                    return "День";
                case RateUnit.Month:
                    return "Месяц";
                default:
                    return "???";
            }
        }
    }
}