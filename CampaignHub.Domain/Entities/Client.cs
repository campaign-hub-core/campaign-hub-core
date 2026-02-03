using CampaignHub.Domain.Entities.Enum;

namespace CampaignHub.Domain.Entities;

public class Client : Entity
{
    public string User { get; set; }
    public string Observation { get; set; }
    public ClientTypeEnum ClientType { get; set; }
    public ClientStatusEnum ClientStatus { get; set; }

    protected Client() { }

    public Client(string user, string observation, ClientTypeEnum clientType)
    {
        User = user;
        Observation = observation;
        ClientType = clientType;
        ClientStatus = ClientStatusEnum.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void Enable()
    {
        ClientStatus = ClientStatusEnum.Active;
    }

    public void Pause()
    {
        ClientStatus = ClientStatusEnum.Suspended;
    }

    public void Disable()
    {
        ClientStatus = ClientStatusEnum.Inactive;
    }
}
