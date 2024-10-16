using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSE.Carrinho.API.Migrations
{
    /// <inheritdoc />
    public partial class Voucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CarrinhoItens",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                table: "CarrinhoItens",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

            migrationBuilder.AddColumn<decimal>(
                name: "Desconto",
                table: "CarrinhoCliente",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentual",
                table: "CarrinhoCliente",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDesconto",
                table: "CarrinhoCliente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorDesconto",
                table: "CarrinhoCliente",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCodigo",
                table: "CarrinhoCliente",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VoucherUtilizado",
                table: "CarrinhoCliente",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "CarrinhoCliente");

            migrationBuilder.DropColumn(
                name: "Percentual",
                table: "CarrinhoCliente");

            migrationBuilder.DropColumn(
                name: "TipoDesconto",
                table: "CarrinhoCliente");

            migrationBuilder.DropColumn(
                name: "ValorDesconto",
                table: "CarrinhoCliente");

            migrationBuilder.DropColumn(
                name: "VoucherCodigo",
                table: "CarrinhoCliente");

            migrationBuilder.DropColumn(
                name: "VoucherUtilizado",
                table: "CarrinhoCliente");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "CarrinhoItens",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Imagem",
                table: "CarrinhoItens",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);
        }
    }
}
