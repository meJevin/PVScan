﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PVScan.Core.DAL;

namespace PVScan.Core.Migrations
{
    [DbContext(typeof(PVScanDbContext))]
    [Migration("20210807123620_BarcodeIndexAdded_GUID")]
    partial class BarcodeIndexAdded_GUID
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("PVScan.Core.Models.Barcode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Favorite")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Format")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GUID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ScanTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("GUID")
                        .IsUnique();

                    b.ToTable("Barcodes");
                });

            modelBuilder.Entity("PVScan.Core.Models.Barcode", b =>
                {
                    b.OwnsOne("PVScan.Core.Models.Coordinate", "ScanLocation", b1 =>
                        {
                            b1.Property<int>("BarcodeId")
                                .HasColumnType("INTEGER");

                            b1.Property<double?>("Latitude")
                                .HasColumnType("REAL");

                            b1.Property<double?>("Longitude")
                                .HasColumnType("REAL");

                            b1.HasKey("BarcodeId");

                            b1.ToTable("Barcodes");

                            b1.WithOwner()
                                .HasForeignKey("BarcodeId");
                        });

                    b.Navigation("ScanLocation");
                });
#pragma warning restore 612, 618
        }
    }
}
