﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrafficTimetable;

namespace TrafficTimetable.Migrations
{
    [DbContext(typeof(ClientDataContext))]
    [Migration("20181220161608_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TrafficTimetable.Domain.Client", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.ClientState", b =>
                {
                    b.Property<string>("ClientId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BufferDirection");

                    b.Property<string>("BufferRouteName");

                    b.Property<string>("BufferStopName");

                    b.Property<string>("BufferTagName");

                    b.Property<bool>("IsAddName");

                    b.Property<bool>("IsAddRoute");

                    b.Property<bool>("IsAddStop");

                    b.Property<bool>("IsAddTag");

                    b.Property<bool>("IsChoosingDirection");

                    b.Property<bool>("IsDefault");

                    b.HasKey("ClientId");

                    b.ToTable("ClientStates");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.ClientTag", b =>
                {
                    b.Property<string>("ClientId");

                    b.Property<string>("TagName");

                    b.Property<string>("StopId");

                    b.HasKey("ClientId", "TagName");

                    b.ToTable("ClientTags");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.Stop", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<List<string>>("Routes");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Stops");
                });
#pragma warning restore 612, 618
        }
    }
}
