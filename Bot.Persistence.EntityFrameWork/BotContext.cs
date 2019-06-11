using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Bot.Domain.Persistence;
using Bot.Persistence.Domain;
using Bot.Persistence.EntityFrameWork.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Bot.Persistence.EntityFrameWork
{

    public class BotContext : DbContext
    {

        /// <summary>Table containing all the server.</summary>
        public DbSet<Server> Servers { get; set; }

        /// <summary>Table containing all the users.</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Table containing all the requests.</summary>
        public DbSet<Request> Requests { get; set; }


        /// <inheritdoc/>
        /// <example>
        /// Migrations:
        /// Follow these steps in the Package manager console.
        /// 1. Move with `cd path` to the correct folder.
        /// 2. Use: `dotnet ef migrations add InitialCreate`.
        /// 3. Use: `dotnet ef database update`.
        /// Note: The connection string needs to be hardcoded to use a migration.
        /// </example>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=password;Database=fmbot;Command Timeout=15;Timeout=30");
        }


        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RequestsConfiguration());
            modelBuilder.ApplyConfiguration(new ServersConfiguration());
            modelBuilder.ApplyConfiguration(new UsersConfiguration());

            // get all composite keys (entity decorated by more than 1 [Key] attribute
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes()
                .Where(t =>
                    t.ClrType.GetProperties()
                        .Count(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute))) > 1))
            {
                // get the keys in the appropriate order
                string[] orderedKeys = entity.ClrType
                    .GetProperties()
                    .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)))
                    .OrderBy(p =>
                        p.CustomAttributes.Single(x => x.AttributeType == typeof(ColumnAttribute))?
                            .NamedArguments?.Single(y => y.MemberName == nameof(ColumnAttribute.Order))
                            .TypedValue.Value ?? 0)
                    .Select(x => x.Name)
                    .ToArray();

                // apply the keys to the model builder
                modelBuilder.Entity(entity.ClrType).HasKey(orderedKeys);
            }
        }
    }
}
