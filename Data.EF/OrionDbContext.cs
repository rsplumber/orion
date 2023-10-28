using Core.Files;
using Core.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Core.Files.File;

namespace Data.EF;

public class OrionDbContext : DbContext
{
    public OrionDbContext(DbContextOptions<OrionDbContext> options) : base(options)
    {
    }


    public DbSet<Bucket> Buckets { get; set; }

    public DbSet<File> Files { get; set; }

    public DbSet<Replication> Replications { get; set; }

    public DbSet<Provider> Providers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new BucketEntityTypeConfiguration());
        builder.ApplyConfiguration(new FileEntityTypeConfiguration());
        builder.ApplyConfiguration(new ReplicationEntityTypeConfiguration());
        builder.ApplyConfiguration(new ProviderEntityTypeConfiguration());
        base.OnModelCreating(builder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    private class BucketEntityTypeConfiguration : IEntityTypeConfiguration<Bucket>
    {
        public void Configure(EntityTypeBuilder<Bucket> builder)
        {
            builder.ToTable("buckets")
                .HasKey(bucket => bucket.Id);

            builder.Property(bucket => bucket.Id)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("id");

            builder.Property(bucket => bucket.Name)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("name");

            builder.HasIndex(bucket => bucket.Name).IsUnique();

            builder.HasMany(bucket => bucket.Files)
                .WithOne(file => file.Bucket)
                .HasForeignKey("bucket_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(bucket => bucket.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
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

            builder.Property(file => file.Path)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("path");

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

            builder.HasIndex(replication => replication.Provider).IsUnique();

            builder.Property(replication => replication.Status)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("status");

            builder.Property(replication => replication.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    }


    private class ProviderEntityTypeConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.ToTable("providers")
                .HasKey(provider => provider.Name);

            builder.Property(provider => provider.Name)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("name");

            builder.Property(provider => provider.Status)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("status");

            builder.Property(provider => provider.Replication)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("replication");

            builder.Property(provider => provider.Primary)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("primary");

            builder.Property(provider => provider.Metas)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("metas")
                .HasColumnType("jsonb");

            builder.Property(provider => provider.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    }
}