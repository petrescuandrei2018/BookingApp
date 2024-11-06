﻿// <auto-generated />
using System;
using BookingApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookingApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241103220603_HotelTipCamera")]
    partial class HotelTipCamera
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookingApp.Models.Hotel", b =>
                {
                    b.Property<int>("HotelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HotelId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HotelId");

                    b.ToTable("Hotels");

                    b.HasData(
                        new
                        {
                            HotelId = 1,
                            Address = "Brasov",
                            Name = "Hotel1"
                        },
                        new
                        {
                            HotelId = 2,
                            Address = "Constanta",
                            Name = "Hotel2"
                        });
                });

            modelBuilder.Entity("BookingApp.Models.HotelTipCamera", b =>
                {
                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<int>("TipCameraId")
                        .HasColumnType("int");

                    b.HasKey("HotelId", "TipCameraId");

                    b.HasIndex("TipCameraId");

                    b.ToTable("HotelSiTipCamere");
                });

            modelBuilder.Entity("BookingApp.Models.TipCamera", b =>
                {
                    b.Property<int>("TipCameraId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TipCameraId"));

                    b.Property<int>("CapacitatePersoane")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NrCamereDisponibile")
                        .HasColumnType("int");

                    b.Property<int>("NrCamereOcupate")
                        .HasColumnType("int");

                    b.Property<int>("NrTotalCamere")
                        .HasColumnType("int");

                    b.HasKey("TipCameraId");

                    b.ToTable("TipCamere");

                    b.HasData(
                        new
                        {
                            TipCameraId = 1,
                            CapacitatePersoane = 1,
                            Name = "Single",
                            NrCamereDisponibile = 10,
                            NrCamereOcupate = 10,
                            NrTotalCamere = 20
                        },
                        new
                        {
                            TipCameraId = 2,
                            CapacitatePersoane = 2,
                            Name = "Double",
                            NrCamereDisponibile = 30,
                            NrCamereOcupate = 10,
                            NrTotalCamere = 40
                        });
                });

            modelBuilder.Entity("BookingApp.Models.HotelTipCamera", b =>
                {
                    b.HasOne("BookingApp.Models.Hotel", "Hotel")
                        .WithMany("HotelSiTipCamere")
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingApp.Models.TipCamera", "TipCamera")
                        .WithMany("HotelSiTipCamere")
                        .HasForeignKey("TipCameraId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");

                    b.Navigation("TipCamera");
                });

            modelBuilder.Entity("BookingApp.Models.Hotel", b =>
                {
                    b.Navigation("HotelSiTipCamere");
                });

            modelBuilder.Entity("BookingApp.Models.TipCamera", b =>
                {
                    b.Navigation("HotelSiTipCamere");
                });
#pragma warning restore 612, 618
        }
    }
}
