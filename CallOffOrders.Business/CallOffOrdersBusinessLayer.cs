using System;
using System.Collections.Generic;
using System.Linq;
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

        public CallOffOrdersBusinessLayer(ICommandBuilder commandBuilder, IQueryBuilder queryBuilder)
        {
            _commandBuilder = commandBuilder;
            _queryBuilder = queryBuilder;
        }

        public async Task<CallOffOrder> GetCallOffOrder(string id)
        {
            return await _queryBuilder.For<Task<CallOffOrder>>().With(new FindById(id));
        }

        public async Task<string> DeleteCallOffOrder(string id)
        {
            var context = new DeleteCallOffOrderCommandContext
            {
                Id = id
            };

            context = await _commandBuilder.Execute(context);

            return context.Id;
        }

        public async Task<IEnumerable<CallOffOrder>> GetCallOffOrders()
        {
            return await _queryBuilder.For<Task<IEnumerable<CallOffOrder>>>().With(new AllEntities());
        }

        public async Task<IEnumerable<CallOffOrder>> GetCallOffOrders(string contractId)
        {
            return await _queryBuilder.For<Task<IEnumerable<CallOffOrder>>>().With(new FindByContractId(contractId));
        }

        public async Task<string> CreateCallOffOrder(CallOffOrder form)
        {
            form.UpdatedAt = DateTime.UtcNow;
            form.CreatedAt = DateTime.UtcNow;
            form.Id = null;

            var context = new CreateCallOffOrderCommandContext {Form = form};

            context = await _commandBuilder.Execute(context);

            return context.Id;
        }

        /// <summary>
        /// Обновить наряд заказ
        /// </summary>
        /// <param name="callOffOrderId">ID наряд заказа</param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<string> UpdateCallOffOrder(string callOffOrderId, CallOffOrder order)
        {
            order.Id = callOffOrderId;

            order.UpdatedAt = DateTime.UtcNow;

            var context = new UpdateCallOffOrderCommandContext
            {
                Form = order
            };

            context = await _commandBuilder.Execute(context);

            return context.Form.Id;
        }

        public async Task<Rate> AddRate(string callOffOrderId, Rate rate)
        {
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

        public async Task DeleteRate(string callOffOrderId, string rateId)
        {
            CallOffOrder callOffOrder = await _queryBuilder.For<Task<CallOffOrder>>()
                .With(new FindById(callOffOrderId));

            var rateForRemove = callOffOrder.Rates.Where(r => r.Id == rateId).SingleOrDefault();

            if (rateForRemove == null)
                throw new ArgumentException(String.Format("Rate with id {0} not found", rateId));

            var removesCount = callOffOrder.Rates.Remove(rateForRemove);

            await UpdateCallOffOrder(callOffOrderId, callOffOrder);
        }

        public async Task UpdateRate(string callOffOrderId, Rate rate)
        {
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
                throw new ArgumentException(String.Format("Rate with id {0} not found", rate.Id));

            await UpdateCallOffOrder(callOffOrderId, callOffOrder);
        }
    }
}