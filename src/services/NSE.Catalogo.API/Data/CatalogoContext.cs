using Microsoft.EntityFrameworkCore;
using NSE.Catalogo.API.Models;
using NSE.Core.Data;

namespace NSE.Catalogo.API.Data
{
    public class CatalogoContext : DbContext, IUnitOfWork
    { 
        public CatalogoContext(DbContextOptions<CatalogoContext> options) : base(options) { }

        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //override necessário para o mappings funcionar
        {
            foreach(var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoContext).Assembly);
        }

        public async Task<bool> Commit() //método da interface IUnitOfWork - no momento que eu der um commit, o saveChangesAsync tras um int como resposta que é o numero de linhas afetadas, se for acima de 0 quer dizer que meu commit deu certo, senão deu errado
        {
            return await base.SaveChangesAsync() > 0;
        }
    }
}
