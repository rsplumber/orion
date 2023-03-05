using Core.FileLocations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Core.Files.File;

namespace Data.Sql;

public class ObjectStorageDbContext : DbContext
{
    public ObjectStorageDbContext(DbContextOptions<ObjectStorageDbContext> options) : base(options)
    {
    }


    public DbSet<File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new FileEntityTypeConfiguration());
        builder.ApplyConfiguration(new FileLocationEntityTypeConfiguration());
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

            builder.Property(location => location.Location)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("location");

            builder.HasIndex(location => location.Location);

            builder.Property(location => location.Provider)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("provider");


            builder.Property(location => location.FileId)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("file_id");

            builder.HasIndex(location => location.FileId);
        }
    }
}