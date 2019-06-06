using System.Diagnostics.CodeAnalysis;
using Bot.Persistence.Domain;
using Bot.Persistence.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bot.Persistence.EntityFrameWork.Configurations
{

    /// <summary>
    /// This class contains the configurations for the <see cref="Request"/> table. 
    /// </summary>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class RequestsConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("requests");
            builder.HasKey(x => x.Id);
            builder.HasAlternateKey(x => x.TimeStamp);

            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(x => x.ServerId).HasColumnName("serverid");
            builder.Property(x => x.UserId).HasColumnName("userid").IsRequired();
            builder.Property(x => x.Command).HasColumnName("command").IsRequired();
            builder.Property(x => x.ErrorMessage).HasColumnName("errormessage").IsRequired(false);
            builder.Property(x => x.IsSuccessFull).HasColumnName("issuccessfull").IsRequired();
            builder.Property(x => x.RunTime).HasColumnName("runtime").IsRequired();
            builder.Property(x => x.TimeStamp).HasColumnName("timestamp").IsRequired();
        }
    }
}
