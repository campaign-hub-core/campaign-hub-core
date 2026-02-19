using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignHub.Infra.Migrations
{
    /// <inheritdoc />
    public partial class addname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Campaign",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "AdAccount",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "AdAccount",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdSet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CampaignId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DailyBudget = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdSet_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ad",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AdSetId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ad_AdSet_AdSetId",
                        column: x => x.AdSetId,
                        principalTable: "AdSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_ExternalId",
                table: "Campaign",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdAccount_ExternalId",
                table: "AdAccount",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ad_AdSetId",
                table: "Ad",
                column: "AdSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Ad_ExternalId",
                table: "Ad",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AdSet_CampaignId",
                table: "AdSet",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_AdSet_ExternalId",
                table: "AdSet",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ad");

            migrationBuilder.DropTable(
                name: "AdSet");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_ExternalId",
                table: "Campaign");

            migrationBuilder.DropIndex(
                name: "IX_AdAccount_ExternalId",
                table: "AdAccount");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "AdAccount");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "AdAccount");
        }
    }
}
