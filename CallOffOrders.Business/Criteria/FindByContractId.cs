using Cmas.Infrastructure.Domain.Criteria;

namespace Cmas.BusinessLayers.CallOffOrders.Criteria
{
    public class FindByContractId : ICriterion
    {
        public FindByContractId(string ContractId = null)
        {
            this.ContractId = ContractId;
        }

        public string ContractId { get; set; }
    }
}
