using System.Collections.Generic;
using Core.Files;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class jsonb_changed_normal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "metas",
                table: "providers",
                type: "text",
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "metas",
                table: "files",
                type: "text",
                nullable: false,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "locations",
                table: "files",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<FileLocation>),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "metas",
                table: "providers",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "metas",
                table: "files",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<List<FileLocation>>(
                name: "locations",
                table: "files",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
