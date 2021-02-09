using System;
using System.Linq;
using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace CursoEFCore.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CursoEFCore;Trusted_Connection=True;MultipleActiveResultSets=true;"
            ,p=>p.EnableRetryOnFailure(
                maxRetryCount: 2,maxRetryDelay: TimeSpan.FromSeconds(5),errorNumbersToAdd: null)
            );//p=>p.EnableRetryOnFailure() tenta se conecar ate 2 vezes caso haja erro
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //importar um a um 
            // modelBuilder.ApplyConfiguration(new ClienteConfiguration());

            //busca todas class que implementa IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            MapearPropriedadesEsquicidas(modelBuilder);
        }

        private void MapearPropriedadesEsquicidas(ModelBuilder modelBuilder){
            foreach(var entity in modelBuilder.Model.GetEntityTypes()){
                var properties = entity.GetProperties().Where(predicate=>predicate.ClrType == typeof(string));
                foreach(var prop in properties){
                    if(string.IsNullOrEmpty(prop.GetColumnType()) && !prop.GetMaxLength().HasValue){
                        //prop.SetMaxLength(100);
                        prop.SetColumnType("VARCHAR(100)");
                    }
                }
            }
        }
    }
}