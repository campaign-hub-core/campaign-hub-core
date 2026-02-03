namespace CampaignHub.Domain.Entities;

public class MetricCampaign : Entity
{
    public string CampaingId { get; set; }
    public DateTime ReferencePeriod { get; set; }
    public decimal Expenses { get; set; }
    public int Leads { get; set; }
    public string? Sales { get; set; }
    public string? Revenue { get; set; }

    protected MetricCampaign() { }
    public MetricCampaign(string campaingId, DateTime referencePeriod, decimal expenses, int leads, string? sales, string? revenue)
    {
        CampaingId = campaingId;
        ReferencePeriod = NormalizePeriod(referencePeriod);
        Expenses = expenses;
        Leads = leads;
        Sales = sales;
        Revenue = revenue;
    }

    private void UpdateExpenses(decimal newExpenses)
    {
        Expenses = newExpenses;
    }

    private static DateTime NormalizePeriod(DateTime date)
    { 
        return new DateTime(date.Year, date.Month, 1);
    }
}
