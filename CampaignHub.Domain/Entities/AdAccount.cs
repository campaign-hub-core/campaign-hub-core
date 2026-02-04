using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities
{
    public class AdAccount : Entity
    {
        public required string ClientId { get; set; }
        public decimal MonthlyBudget { get; set; }
        public required string Goal { get; set; }
        public AdPlatformEnum AdPlatform { get; set; }

        protected AdAccount() { }

        public AdAccount(string clientId, decimal monthlyBudget, string goal, AdPlatformEnum adPlatform)
        {
            ClientId = clientId;
            MonthlyBudget = monthlyBudget;
            Goal = goal;
            AdPlatform = adPlatform;
        }

        public void UpdateMonthlyBudget(decimal newBudget)
        {
            MonthlyBudget = newBudget;
        }
    }
}
