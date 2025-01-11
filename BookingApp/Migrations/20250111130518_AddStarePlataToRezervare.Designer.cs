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
    [Migration("20250111130518_AddStarePlataToRezervare")]
    partial class AddStarePlataToRezervare
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
                        },
                        new
                        {
                            HotelId = 3,
                            Address = "Sibu",
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

            modelBuilder.Entity("BookingApp.Models.Review", b =>
                {
                    b.Property<int>("ReviewId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReviewId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<double>("Rating")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ReviewId");

                    b.HasIndex("HotelId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");

                    b.HasData(
                        new
                        {
                            ReviewId = 1,
                            Description = "A fost bine",
                            HotelId = 1,
                            Rating = 4.9000000000000004,
                            UserId = 1
                        },
                        new
                        {
                            ReviewId = 2,
                            Description = "Mancare excelenta",
                            HotelId = 1,
                            Rating = 5.0,
                            UserId = 2
                        },
                        new
                        {
                            ReviewId = 3,
                            Description = "Priveliste la mare",
                            HotelId = 1,
                            Rating = 5.0,
                            UserId = 3
                        },
                        new
                        {
                            ReviewId = 4,
                            Description = "Cazare tarzie",
                            HotelId = 1,
                            Rating = 3.5,
                            UserId = 3
                        },
                        new
                        {
                            ReviewId = 5,
                            Description = "Muste in camera",
                            HotelId = 2,
                            Rating = 2.0,
                            UserId = 2
                        });
                });

            modelBuilder.Entity("BookingApp.Models.Rezervare", b =>
                {
                    b.Property<int>("RezervareId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RezervareId"));

                    b.Property<DateTime>("CheckIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOut")
                        .HasColumnType("datetime2");

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Neplatita");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("RezervareId");

                    b.HasIndex("PretCameraId");

                    b.HasIndex("UserId");

                    b.ToTable("Rezervari");
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
                            TipCameraId = 1,
                            CapacitatePersoane = 1,
                            HotelId = 1,
                            Name = "Single",
                            NrCamereDisponibile = 10,
                            NrCamereOcupate = 10,
                            NrTotalCamere = 20
                        },
                        new
                        {
                            TipCameraId = 2,
                            CapacitatePersoane = 2,
                            HotelId = 2,
                            Name = "Double",
                            NrCamereDisponibile = 30,
                            NrCamereOcupate = 10,
                            NrTotalCamere = 40
                        },
                        new
                        {
                            TipCameraId = 3,
                            CapacitatePersoane = 4,
                            HotelId = 1,
                            Name = "Apartament",
                            NrCamereDisponibile = 1,
                            NrCamereOcupate = 9,
                            NrTotalCamere = 10
                        },
                        new
                        {
                            TipCameraId = 4,
                            CapacitatePersoane = 2,
                            HotelId = 2,
                            Name = "SeaView",
                            NrCamereDisponibile = 2,
                            NrCamereOcupate = 13,
                            NrTotalCamere = 15
                        },
                        new
                        {
                            TipCameraId = 5,
                            CapacitatePersoane = 1,
                            HotelId = 1,
                            Name = "Single",
                            NrCamereDisponibile = 10,
                            NrCamereOcupate = 10,
                            NrTotalCamere = 20
                        },
                        new
                        {
                            TipCameraId = 6,
                            CapacitatePersoane = 1,
                            HotelId = 2,
                            Name = "Single",
                            NrCamereDisponibile = 5,
                            NrCamereOcupate = 5,
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
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("user")
                        .UseCollation("SQL_Latin1_General_CP1_CI_AS");

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
                            Password = "parola1",
                            PhoneNumber = "0775695878",
                            Rol = "user",
                            UserName = "Mihai",
                            Varsta = 30
                        },
                        new
                        {
                            UserId = 2,
                            Email = "nicu@gmail.com",
                            Password = "parola2",
                            PhoneNumber = "0770605078",
                            Rol = "user",
                            UserName = "Nicu",
                            Varsta = 20
                        },
                        new
                        {
                            UserId = 3,
                            Email = "alex@gmail.com",
                            Password = "parola3",
                            PhoneNumber = "0765665668",
                            Rol = "user",
                            UserName = "Alex",
                            Varsta = 32
                        });
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

            modelBuilder.Entity("BookingApp.Models.Review", b =>
                {
                    b.HasOne("BookingApp.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingApp.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BookingApp.Models.Rezervare", b =>
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

            modelBuilder.Entity("BookingApp.Models.TipCamera", b =>
                {
                    b.HasOne("BookingApp.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });
#pragma warning restore 612, 618
        }
    }
}
