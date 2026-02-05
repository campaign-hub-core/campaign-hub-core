using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities
{
    public class AdAccount : Entity
    {
        public required string CustomerId { get; set; }
        public decimal MonthlyBudget { get; set; }
        public required string Goal { get; set; }
        public AdPlatformEnum AdPlatform { get; set; }

        private readonly List<Campaign> _campaigns = new();
        public IReadOnlyCollection<Campaign> Campaigns => _campaigns;

        protected AdAccount() { }

        public AdAccount(string customerId, decimal monthlyBudget, string goal, AdPlatformEnum adPlatform)
        {
            CustomerId = customerId;
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
