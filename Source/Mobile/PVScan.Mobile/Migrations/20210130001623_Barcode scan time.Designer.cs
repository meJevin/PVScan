﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PVScan.Mobile.DAL;

namespace PVScan.Mobile.Migrations
{
    [DbContext(typeof(PVScanMobileDbContext))]
    [Migration("20210130001623_Barcode scan time")]
    partial class Barcodescantime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11");

            modelBuilder.Entity("PVScan.Mobile.Models.Barcode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Format")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ScanTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("ServerSynced")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Barcodes");
                });

            modelBuilder.Entity("PVScan.Mobile.Models.Barcode", b =>
                {
                    b.OwnsOne("PVScan.Mobile.Models.Coordinate", "ScanLocation", b1 =>
                        {
                            b1.Property<int>("BarcodeId")
                                .HasColumnType("INTEGER");

                            b1.Property<double>("Latitude")
                                .HasColumnType("REAL");

                            b1.Property<double>("Longitude")
                                .HasColumnType("REAL");

                            b1.HasKey("BarcodeId");

                            b1.ToTable("Barcodes");

                            b1.WithOwner()
                                .HasForeignKey("BarcodeId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
