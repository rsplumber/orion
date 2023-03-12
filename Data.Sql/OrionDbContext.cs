using Core.Files;
using Core.Replications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Core.Files.File;

namespace Data.Sql;

public class OrionDbContext : DbContext
{
    public OrionDbContext(DbContextOptions<OrionDbContext> options) : base(options)
    {
    }


    public DbSet<File> Files { get; set; }

    public DbSet<FileLocation> FileLocations { get; set; }

    public DbSet<Replication> Replications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new FileEntityTypeConfiguration());
        builder.ApplyConfiguration(new FileLocationEntityTypeConfiguration());
        builder.ApplyConfiguration(new ReplicationEntityTypeConfiguration());
        base.OnModelCreating(builder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
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

            builder.HasMany(file => file.Locations)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    private class FileLocationEntityTypeConfiguration : IEntityTypeConfiguration<FileLocation>
    {
        public void Configure(EntityTypeBuilder<FileLocation> builder)
        {
            builder.ToTable("file_locations")
                .HasKey(location => location.Id);

            builder.Property(location => location.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(location => location.Link)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("location");

            builder.Property(location => location.Path)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("path");

            builder.Property(location => location.Provider)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("provider");
            builder.HasIndex(location => location.Provider);
        }
    }

    private class ReplicationEntityTypeConfiguration : IEntityTypeConfiguration<Replication>
    {
        public void Configure(EntityTypeBuilder<Replication> builder)
        {
            builder.Property(replication => replication.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(replication => replication.FileId)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("file_id");

            builder.Property(replication => replication.Provider)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("provider_name");

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