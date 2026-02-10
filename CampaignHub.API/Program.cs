using CampaignHub.API.Extensions;
using CampaignHub.Application;
using CampaignHub.Domain;
using CampaignHub.Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomain();
builder.Services.AddInfra(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddCampaignHubApi(builder.Configuration);

var app = builder.Build();

app.UseCampaignHubApiPipeline();

app.Run();
