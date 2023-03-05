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

            builder.Property(file => file.Locations)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("locations")
                .HasColumnType("jsonb");
            //todo locations to table not dict 

            builder.Property(file => file.CreatedDateUtc)
                .UsePropertyAccessMode(PropertyAccessMode.Property)
                .HasColumnName("created_date_utc");
        }
    }
}