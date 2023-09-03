﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RealEstate.Services.TransactionService.Data;

#nullable disable

namespace RealEstate.Services.TransactionService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230828145852_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("RealEstate.Services.TransactionService.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BuyerId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("OwnerId")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("PropertyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RentEndDate")
                        .HasColumnType("datetime");

                    b.Property<decimal?>("RentPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("RentStartDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TransactionType");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("RealEstate.Services.TransactionService.Models.TransactionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("TransactionTypes");
                });

            modelBuilder.Entity("RealEstate.Services.TransactionService.Models.Transaction", b =>
                {
                    b.HasOne("RealEstate.Services.TransactionService.Models.TransactionType", "TransactionTypeNavigation")
                        .WithMany("Transactions")
                        .HasForeignKey("TransactionType");

                    b.Navigation("TransactionTypeNavigation");
                });

            modelBuilder.Entity("RealEstate.Services.TransactionService.Models.TransactionType", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}