﻿// <auto-generated />
using System;
using Catfish.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catfish.Core.Migrations
{
    [DbContext(typeof(CatfishDbContext))]
    partial class CatfishDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Catfish.Core.Models.Relationship", b =>
                {
                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<int>("ObjctId")
                        .HasColumnType("int");

                    b.Property<string>("Predicate")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubjectId", "ObjctId");

                    b.HasIndex("ObjctId");

                    b.ToTable("Catfish_Relationships");
                });

            modelBuilder.Entity("Catfish.Core.Models.XmlModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("xml");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Catfish_XmlModels");

                    b.HasDiscriminator<string>("Discriminator").HasValue("XmlModel");
                });

            modelBuilder.Entity("Catfish.Core.Models.Aggregation2", b =>
                {
                    b.HasBaseType("Catfish.Core.Models.XmlModel");

                    b.ToTable("Catfish_XmlModels");

                    b.HasDiscriminator().HasValue("Aggregation2");
                });

            modelBuilder.Entity("Catfish.Core.Models.Collection2", b =>
                {
                    b.HasBaseType("Catfish.Core.Models.Aggregation2");

                    b.ToTable("Catfish_XmlModels");

                    b.HasDiscriminator().HasValue("Collection2");
                });

            modelBuilder.Entity("Catfish.Core.Models.Item2", b =>
                {
                    b.HasBaseType("Catfish.Core.Models.Aggregation2");

                    b.ToTable("Catfish_XmlModels");

                    b.HasDiscriminator().HasValue("Item2");
                });

            modelBuilder.Entity("Catfish.Core.Models.Relationship", b =>
                {
                    b.HasOne("Catfish.Core.Models.Aggregation2", "Objct")
                        .WithMany("ObjectRelationships")
                        .HasForeignKey("ObjctId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Catfish.Core.Models.Aggregation2", "Subject")
                        .WithMany("SubjectRelationships")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
