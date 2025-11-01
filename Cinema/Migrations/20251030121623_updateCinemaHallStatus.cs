using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Migrations
{
    /// <inheritdoc />
    public partial class updateCinemaHallStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaHalls_CinemaBranches_BranchId",
                table: "CinemaHalls");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CinemaHalls",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "HallType",
                table: "CinemaHalls",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "CinemaHalls",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaHalls_CinemaBranches_BranchId",
                table: "CinemaHalls",
                column: "BranchId",
                principalTable: "CinemaBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaHalls_CinemaBranches_BranchId",
                table: "CinemaHalls");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "CinemaHalls",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HallType",
                table: "CinemaHalls",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "CinemaHalls",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaHalls_CinemaBranches_BranchId",
                table: "CinemaHalls",
                column: "BranchId",
                principalTable: "CinemaBranches",
                principalColumn: "Id");
        }
    }
}
