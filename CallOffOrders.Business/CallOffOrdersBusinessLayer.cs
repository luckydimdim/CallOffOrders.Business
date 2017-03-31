using System.Collections.Generic;
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
            var criteria = new FindByContractId();
            criteria.ContractId = contractId;

            return await _queryBuilder.For<Task<IEnumerable<CallOffOrder>>>().With(criteria);
        }

        public async Task<string> CreateCallOffOrder(CallOffOrder form)
        {
            var context = new CreateCallOffOrderCommandContext();
            context.Form = form;

            context = await _commandBuilder.Execute(context);

            return context.Id;
        }

        public async Task<string> UpdateCallOffOrder(string id, CallOffOrder form)
        {
            var context = new UpdateCallOffOrderCommandContext
            {
                Form = form
            };

            context = await _commandBuilder.Execute(context);

            return context.Form.Id;
        }

    }
}
