using System.Text.Json;
using Core.Files;
using Core.Replications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Core.Files.File;

namespace Data.Sql;

public class OrionDbContext : DbContext
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IgnoreReadOnlyFields = true
    };

    public OrionDbContext(DbContextOptions<OrionDbContext> options) : base(options)
    {
    }


    public DbSet<File> Files { get; set; }

    public DbSet<Replication> Replications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new FileEntityTypeConfiguration());
        builder.ApplyConfiguration(new ReplicationEntityTypeConfiguration());
        base.OnModelCreating(builder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    private class FileEntityTypeConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.ToTable("files")
                .HasKey(file => file.Id);

            builder.Property(file => file.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(file => file.Name)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("name");

            builder.HasIndex(file => file.Name);

            builder.Property(file => file.Metas)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("metas")
                .HasColumnType("jsonb");

            builder.Property(file => file.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");

            builder.Property(e => e.Locations)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("locations")
                .HasColumnType("jsonb");
            
        }
    }

    private class ReplicationEntityTypeConfiguration : IEntityTypeConfiguration<Replication>
    {
        public void Configure(EntityTypeBuilder<Replication> builder)
        {
            builder.ToTable("replications")
                .HasKey(location => location.Id);

            builder.Property(replication => replication.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(replication => replication.FileId)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("file_id");

            builder.HasIndex(replication => replication.FileId);

            builder.Property(replication => replication.Provider)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("provider");

            builder.HasIndex(replication => replication.Provider);

            builder.Property(replication => replication.Retry)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("retry");

            builder.Property(replication => replication.Status)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("status");

            builder.Property(replication => replication.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    }
}