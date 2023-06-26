﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Core.Files;
using Data.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Sql.Migrations
{
    [DbContext(typeof(OrionDbContext))]
    partial class OrionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
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

                    b.Property<List<FileLocation>>("Locations")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("locations");

                    b.Property<Dictionary<string, string>>("Metas")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("metas");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("files", (string)null);
                });

            modelBuilder.Entity("Core.Providers.Provider", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date_utc");

                    b.Property<Dictionary<string, string>>("Metas")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("metas");

                    b.Property<bool>("Primary")
                        .HasColumnType("boolean")
                        .HasColumnName("primary");

                    b.Property<bool>("Replication")
                        .HasColumnType("boolean")
                        .HasColumnName("replication");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("Name");

                    b.ToTable("providers", (string)null);
                });

            modelBuilder.Entity("Core.Providers.Replication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedDateUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date_utc");

                    b.Property<Guid>("FileId")
                        .HasColumnType("uuid")
                        .HasColumnName("file_id");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("provider");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("Provider");

                    b.ToTable("replications", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
