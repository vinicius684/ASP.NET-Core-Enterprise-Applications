using Microsoft.EntityFrameworkCore;
using NSE.Clientes.API.Models;
using NSE.Core.Data;



namespace NSE.Clientes.API.Data
{
    public sealed class ClientesContext : DbContext, IUnitOfWork
    {

        public ClientesContext(DbContextOptions<ClientesContext> options)
            : base(options)
        {
            //Nossa forma de arquitetura não depende desses comportamentos de tracking, só iria atrapalhar na vdd
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;//quando você faz uma consulta ao banco de dados, o Entity Framework não irá acompanhar se esses dados são modificados
            ChangeTracker.AutoDetectChangesEnabled = false;//Normalmente, o Entity Framework verifica automaticamente se há alterações nos dados a serem salvos. Com essa configuração, ele não faz essa verificação constantemente
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<ValidationResult>();
            //modelBuilder.Ignore<Event>();

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes() //onde houver relacionamento, vou desligar o delete em cascata
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesContext).Assembly);
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;
            //if (sucesso) await _mediatorHandler.PublicarEventos(this);

            return sucesso;
        }
    }

    //public static class MediatorExtension
    //{
    //    public static async Task PublicarEventos<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
    //    {
    //        var domainEntities = ctx.ChangeTracker
    //            .Entries<Entity>()
    //            .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

    //        var domainEvents = domainEntities
    //            .SelectMany(x => x.Entity.Notificacoes)
    //            .ToList();

    //        domainEntities.ToList()
    //            .ForEach(entity => entity.Entity.LimparEventos());

    //        var tasks = domainEvents
    //            .Select(async (domainEvent) => {
    //                await mediator.PublicarEvento(domainEvent);
    //            });

    //        await Task.WhenAll(tasks);
    //    }
    //}
}