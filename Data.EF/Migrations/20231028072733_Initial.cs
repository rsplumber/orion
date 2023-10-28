using System;
using System.Collections.Generic;
using Core.Files;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "buckets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buckets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "providers",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    primary = table.Column<bool>(type: "boolean", nullable: false),
                    replication = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    metas = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_providers", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "replications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_replications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    bucket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metas = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    locations = table.Column<List<FileLocation>>(type: "jsonb", nullable: false),
                    created_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.id);
                    table.ForeignKey(
                        name: "FK_files_buckets_bucket_id",
                        column: x => x.bucket_id,
                        principalTable: "buckets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_buckets_name",
                table: "buckets",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_files_bucket_id",
                table: "files",
                column: "bucket_id");

            migrationBuilder.CreateIndex(
                name: "IX_replications_file_id",
                table: "replications",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_replications_provider",
                table: "replications",
                column: "provider",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "providers");

            migrationBuilder.DropTable(
                name: "replications");

            migrationBuilder.DropTable(
                name: "buckets");
        }
    }
}
