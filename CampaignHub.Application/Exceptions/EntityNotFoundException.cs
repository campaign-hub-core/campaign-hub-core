namespace CampaignHub.Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public string EntityName { get; }
    public string Id { get; }

    public EntityNotFoundException(string entityName, string id)
        : base($"{entityName} with id '{id}' was not found.")
    {
        EntityName = entityName;
        Id = id;
    }
}
