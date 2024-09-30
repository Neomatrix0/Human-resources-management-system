using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestionale_Personale_mvc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dipendenti_Statistiche_StatisticheId",
                table: "Dipendenti");

            migrationBuilder.AlterColumn<int>(
                name: "StatisticheId",
                table: "Dipendenti",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Dipendenti_Statistiche_StatisticheId",
                table: "Dipendenti",
                column: "StatisticheId",
                principalTable: "Statistiche",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dipendenti_Statistiche_StatisticheId",
                table: "Dipendenti");

            migrationBuilder.AlterColumn<int>(
                name: "StatisticheId",
                table: "Dipendenti",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dipendenti_Statistiche_StatisticheId",
                table: "Dipendenti",
                column: "StatisticheId",
                principalTable: "Statistiche",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
