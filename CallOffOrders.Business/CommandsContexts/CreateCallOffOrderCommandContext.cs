using Cmas.BusinessLayers.CallOffOrders.Entities;
using Cmas.Infrastructure.Domain.Commands;

namespace Cmas.BusinessLayers.CallOffOrders.CommandsContexts
{
    public class CreateCallOffOrderCommandContext : ICommandContext
    {
        public string Id;
        public CallOffOrder CallOffOrder;
    }
}
