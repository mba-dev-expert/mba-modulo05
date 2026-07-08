using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelesEducacao.Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSecurityKeysParametersLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Parameters",
                table: "SecurityKeys",
                type: "nvarchar(max)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Parameters",
                table: "SecurityKeys",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
