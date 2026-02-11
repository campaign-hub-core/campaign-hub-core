namespace CampaignHub.Application.DTOs.Meta;

public class MetaPaginatedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public MetaPaging? Paging { get; set; }
}

public class MetaPaging
{
    public MetaCursors? Cursors { get; set; }
    public string? Next { get; set; }
}

public class MetaCursors
{
    public string? Before { get; set; }
    public string? After { get; set; }
}
