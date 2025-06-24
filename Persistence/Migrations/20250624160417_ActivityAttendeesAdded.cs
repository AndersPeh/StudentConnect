using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// EF Core runs migration code to bring database schema in sync with C# code (ActivityAttendee object)
namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ActivityAttendeesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityAttendee",
                columns: table => new
                // Migration doesnt create navigation properties, they are not stored as columns in the database,
                // they are used by EF Core for navigate between related entities.
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ActivityId = table.Column<string>(type: "TEXT", nullable: false),
                    IsHost = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateJoined = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityAttendee", x => new { x.ActivityId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ActivityAttendee_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        // Let's say if an Activity is deleted, associated ActivityAttendee will be deleted.
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityAttendee_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityAttendee_UserId",
                table: "ActivityAttendee",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityAttendee");
        }
    }
}
