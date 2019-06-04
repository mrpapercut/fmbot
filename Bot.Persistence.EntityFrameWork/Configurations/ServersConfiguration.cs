using Bot.Persistence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bot.Persistence.EntityFrameWork.Configurations
{

    /// <summary>
    /// This class contains the configurations for the <see cref="Server"/> table. 
    /// </summary>
    public class ServersConfiguration : IEntityTypeConfiguration<Server>
    {


        public void Configure(EntityTypeBuilder<Server> builder)
        {
            builder.ToTable("servers");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id").IsRequired();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired();
            builder.Property(x => x.JoinDate).HasColumnName("joindate").IsRequired();
            builder.Property(x => x.Active).HasColumnName("active").IsRequired();
            builder.Property(x => x.TotalMembers).HasColumnName("totalmembers").IsRequired();
            builder.Property(x => x.Prefix).HasColumnName("prefix").IsRequired(false);
        }
    }
}

