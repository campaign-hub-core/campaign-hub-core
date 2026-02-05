using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignHub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdAccounts_Customers_CustomerId",
                table: "AdAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_AdAccounts_AdAccountId",
                table: "Campaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_MetricCampaigns_Campaigns_CampaignId",
                table: "MetricCampaigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricCampaigns",
                table: "MetricCampaigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campaigns",
                table: "Campaigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdAccounts",
                table: "AdAccounts");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Organizations",
                newName: "Organization");

            migrationBuilder.RenameTable(
                name: "MetricCampaigns",
                newName: "Metric");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customer");

            migrationBuilder.RenameTable(
                name: "Campaigns",
                newName: "Campaign");

            migrationBuilder.RenameTable(
                name: "AdAccounts",
                newName: "AdAccount");

            migrationBuilder.RenameIndex(
                name: "IX_MetricCampaigns_CampaignId_ReferencePeriod",
                table: "Metric",
                newName: "IX_Metric_CampaignId_ReferencePeriod");

            migrationBuilder.RenameIndex(
                name: "IX_Campaigns_AdAccountId",
                table: "Campaign",
                newName: "IX_Campaign_AdAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_AdAccounts_CustomerId",
                table: "AdAccount",
                newName: "IX_AdAccount_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Metric",
                table: "Metric",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customer",
                table: "Customer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campaign",
                table: "Campaign",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdAccount",
                table: "AdAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdAccount_Customer_CustomerId",
                table: "AdAccount",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_AdAccount_AdAccountId",
                table: "Campaign",
                column: "AdAccountId",
                principalTable: "AdAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Metric_Campaign_CampaignId",
                table: "Metric",
                column: "CampaignId",
                principalTable: "Campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdAccount_Customer_CustomerId",
                table: "AdAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_AdAccount_AdAccountId",
                table: "Campaign");

            migrationBuilder.DropForeignKey(
                name: "FK_Metric_Campaign_CampaignId",
                table: "Metric");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Metric",
                table: "Metric");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customer",
                table: "Customer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campaign",
                table: "Campaign");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdAccount",
                table: "AdAccount");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "Organizations");

            migrationBuilder.RenameTable(
                name: "Metric",
                newName: "MetricCampaigns");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "Campaign",
                newName: "Campaigns");

            migrationBuilder.RenameTable(
                name: "AdAccount",
                newName: "AdAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_Metric_CampaignId_ReferencePeriod",
                table: "MetricCampaigns",
                newName: "IX_MetricCampaigns_CampaignId_ReferencePeriod");

            migrationBuilder.RenameIndex(
                name: "IX_Campaign_AdAccountId",
                table: "Campaigns",
                newName: "IX_Campaigns_AdAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_AdAccount_CustomerId",
                table: "AdAccounts",
                newName: "IX_AdAccounts_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricCampaigns",
                table: "MetricCampaigns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campaigns",
                table: "Campaigns",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdAccounts",
                table: "AdAccounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdAccounts_Customers_CustomerId",
                table: "AdAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_AdAccounts_AdAccountId",
                table: "Campaigns",
                column: "AdAccountId",
                principalTable: "AdAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetricCampaigns_Campaigns_CampaignId",
                table: "MetricCampaigns",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
