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
    [Migration("20250203121817_ActualizareRecenzii")]
    partial class ActualizareRecenzii
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
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

                    b.Property<double>("Latitudine")
                        .HasColumnType("float");

                    b.Property<double>("Longitudine")
                        .HasColumnType("float");

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
                            Latitudine = 45.653199999999998,
                            Longitudine = 25.6113,
                            Name = "Hotel1"
                        },
                        new
                        {
                            HotelId = 2,
                            Address = "Constanta",
                            Latitudine = 44.159799999999997,
                            Longitudine = 28.634799999999998,
                            Name = "Hotel2"
                        },
                        new
                        {
                            HotelId = 3,
                            Address = "Sibu",
                            Latitudine = 45.798299999999998,
                            Longitudine = 24.125599999999999,
                            Name = "Hotel3"
                        });
                });

            modelBuilder.Entity("BookingApp.Models.PretCamera", b =>
                {
                    b.Property<int>("PretCameraId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PretCameraId"));

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndPretCamera")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<float>("PretNoapte")
                        .HasColumnType("real");

                    b.Property<DateTime>("StartPretCamera")
                        .HasColumnType("datetime2");

                    b.Property<int>("TipCameraId")
                        .HasColumnType("int");

                    b.HasKey("PretCameraId");

                    b.HasIndex("TipCameraId");

                    b.ToTable("PretCamere");

                    b.HasData(
                        new
                        {
                            PretCameraId = 1,
                            EndPretCamera = new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PretNoapte = 900f,
                            StartPretCamera = new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TipCameraId = 3
                        },
                        new
                        {
                            PretCameraId = 2,
                            EndPretCamera = new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PretNoapte = 700f,
                            StartPretCamera = new DateTime(2024, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TipCameraId = 4
                        },
                        new
                        {
                            PretCameraId = 3,
                            EndPretCamera = new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PretNoapte = 500f,
                            StartPretCamera = new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TipCameraId = 5
                        },
                        new
                        {
                            PretCameraId = 4,
                            EndPretCamera = new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PretNoapte = 550f,
                            StartPretCamera = new DateTime(2024, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            TipCameraId = 6
                        });
                });

            modelBuilder.Entity("BookingApp.Models.Recenzie", b =>
                {
                    b.Property<int>("RecenzieId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecenzieId"));

                    b.Property<string>("AspecteNegative")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AspectePozitive")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataRecenziei")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Descriere")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Rating")
                        .HasColumnType("decimal(2,1)");

                    b.Property<bool>("RecomandaHotelul")
                        .HasColumnType("bit");

                    b.Property<int>("UtilizatorId")
                        .HasColumnType("int");

                    b.HasKey("RecenzieId");

                    b.HasIndex("HotelId");

                    b.HasIndex("UtilizatorId");

                    b.ToTable("Recenzii");
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

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

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

                    b.HasIndex("HotelId");

                    b.ToTable("TipCamere");

                    b.HasData(
                        new
                        {
                            TipCameraId = 3,
                            CapacitatePersoane = 4,
                            HotelId = 1,
                            Name = "Apartament",
                            NrCamereDisponibile = 4,
                            NrCamereOcupate = 6,
                            NrTotalCamere = 10
                        },
                        new
                        {
                            TipCameraId = 4,
                            CapacitatePersoane = 2,
                            HotelId = 2,
                            Name = "SeaView",
                            NrCamereDisponibile = 0,
                            NrCamereOcupate = 15,
                            NrTotalCamere = 15
                        },
                        new
                        {
                            TipCameraId = 5,
                            CapacitatePersoane = 1,
                            HotelId = 1,
                            Name = "Single",
                            NrCamereDisponibile = 4,
                            NrCamereOcupate = 16,
                            NrTotalCamere = 20
                        },
                        new
                        {
                            TipCameraId = 6,
                            CapacitatePersoane = 1,
                            HotelId = 2,
                            Name = "Single",
                            NrCamereDisponibile = 0,
                            NrCamereOcupate = 10,
                            NrTotalCamere = 10
                        });
                });

            modelBuilder.Entity("BookingApp.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rol")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("user");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Varsta")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            Email = "mihai@gmail.com",
                            Password = "$2a$11$2DoCTB2xda.BNYmvYpOteujba4JNDKiCUKZ2xyYsXItQMRkemn8aS",
                            PhoneNumber = "0775695878",
                            Rol = "admin",
                            UserName = "Mihai",
                            Varsta = 30
                        },
                        new
                        {
                            UserId = 2,
                            Email = "nicu@gmail.com",
                            Password = "$2a$11$7Ge.C6hkX3I5NowTSpJxJuqSmZeIWQm78OP5TYogzBf/eNi.HCZsm",
                            PhoneNumber = "0770605078",
                            Rol = "admin",
                            UserName = "Nicu",
                            Varsta = 20
                        },
                        new
                        {
                            UserId = 3,
                            Email = "alex@gmail.com",
                            Password = "$2a$11$g0yF8Xk1vZxOZRD1jlErE./z2ev289.o/SybsDQMbQmCe9ZC0B1/e",
                            PhoneNumber = "0765665668",
                            Rol = "user",
                            UserName = "Alex",
                            Varsta = 32
                        });
                });

            modelBuilder.Entity("Rezervare", b =>
                {
                    b.Property<int>("RezervareId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RezervareId"));

                    b.Property<DateTime>("CheckIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOut")
                        .HasColumnType("datetime2");

                    b.Property<string>("ClientSecret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PretCameraId")
                        .HasColumnType("int");

                    b.Property<string>("Stare")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StarePlata")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("SumaAchitata")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SumaRamasaDePlata")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SumaTotala")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("RezervareId");

                    b.HasIndex("PretCameraId");

                    b.HasIndex("UserId");

                    b.ToTable("Rezervari");
                });

            modelBuilder.Entity("BookingApp.Models.PretCamera", b =>
                {
                    b.HasOne("BookingApp.Models.TipCamera", "TipCamera")
                        .WithMany()
                        .HasForeignKey("TipCameraId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TipCamera");
                });

            modelBuilder.Entity("BookingApp.Models.Recenzie", b =>
                {
                    b.HasOne("BookingApp.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingApp.Models.User", "Utilizator")
                        .WithMany()
                        .HasForeignKey("UtilizatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");

                    b.Navigation("Utilizator");
                });

            modelBuilder.Entity("BookingApp.Models.TipCamera", b =>
                {
                    b.HasOne("BookingApp.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("Rezervare", b =>
                {
                    b.HasOne("BookingApp.Models.PretCamera", "PretCamera")
                        .WithMany()
                        .HasForeignKey("PretCameraId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingApp.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PretCamera");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
