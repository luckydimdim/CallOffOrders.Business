using Cmas.BusinessLayers.CallOffOrders.Entities;
using Cmas.Infrastructure.Domain.Commands;

namespace Cmas.BusinessLayers.CallOffOrders.CommandsContexts
{
    public class UpdateCallOffOrderCommandContext : ICommandContext
    {
        public CallOffOrder Form;
    }
}
