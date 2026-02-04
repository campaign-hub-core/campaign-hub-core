namespace CampaignHub.Domain.Entities
{
    public class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
