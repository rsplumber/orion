﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Data.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Sql.Migrations
{
    [DbContext(typeof(ObjectStorageDbContext))]
    partial class ObjectStorageDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Core.Files.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date_utc");

                    b.Property<Dictionary<string, string>>("Metas")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("metas");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("Core.Files.FileLocation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid?>("FileId")
                        .HasColumnType("uuid");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("filename");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("location");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("provider");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("Provider");

                    b.ToTable("file_locations", (string)null);
                });

            modelBuilder.Entity("Core.Files.FileLocation", b =>
                {
                    b.HasOne("Core.Files.File", null)
                        .WithMany("Locations")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Core.Files.File", b =>
                {
                    b.Navigation("Locations");
                });
#pragma warning restore 612, 618
        }
    }
}
