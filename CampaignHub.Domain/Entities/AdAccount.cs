using System.Diagnostics.CodeAnalysis;
using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities
{
    public class AdAccount : Entity
    {
        public required string CustomerId { get; set; }
        public decimal MonthlyBudget { get; set; }
        public required string Goal { get; set; }
        public AdPlatformEnum AdPlatform { get; set; }
        public string? ExternalId { get; set; }
        public DateTime? LastSyncedAt { get; set; }

        private readonly List<Campaign> _campaigns = new();
        public IReadOnlyCollection<Campaign> Campaigns => _campaigns;

        protected AdAccount() { }

        [SetsRequiredMembers]
        public AdAccount(string customerId, decimal monthlyBudget, string goal, AdPlatformEnum adPlatform)
        {
            CustomerId = customerId;
            MonthlyBudget = monthlyBudget;
            Goal = goal;
            AdPlatform = adPlatform;
        }

        public void Update(decimal monthlyBudget, string goal)
        {
            MonthlyBudget = monthlyBudget;
            Goal = goal;
        }

        public void UpdateMonthlyBudget(decimal newBudget)
        {
            MonthlyBudget = newBudget;
        }

        public void SetExternalId(string externalId)
        {
            ExternalId = externalId;
        }

        public void MarkSynced()
        {
            LastSyncedAt = DateTime.UtcNow;
        }
    }
}
