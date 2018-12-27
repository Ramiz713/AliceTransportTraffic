﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrafficTimetable;

namespace TrafficTimetable.Infrastructure.Migrations
{
    [DbContext(typeof(ClientDataContext))]
    partial class ClientDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.HasIndex("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.ClientState", b =>
                {
                    b.Property<string>("ClientId");

                    b.Property<string>("BufferDirection");

                    b.Property<string>("BufferRouteName");

                    b.Property<string>("BufferStopName");

                    b.Property<string>("BufferTagName");

                    b.Property<int>("ClientStatus");

                    b.Property<string>("SessionId");

                    b.Property<bool>("WaitingToContinue");

                    b.HasKey("ClientId");

                    b.HasIndex("ClientId");

                    b.ToTable("ClientStates");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.ClientTag", b =>
                {
                    b.Property<string>("ClientId");

                    b.Property<string>("TagName");

                    b.Property<List<string>>("Routes");

                    b.Property<string>("StopId");

                    b.HasKey("ClientId", "TagName");

                    b.HasIndex("StopId");

                    b.HasIndex("ClientId", "TagName");

                    b.ToTable("ClientTags");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.Stop", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Stops");
                });

            modelBuilder.Entity("TrafficTimetable.Domain.ClientState", b =>
                {
                    b.HasOne("TrafficTimetable.Domain.Client", "Client")
                        .WithOne("State")
                        .HasForeignKey("TrafficTimetable.Domain.ClientState", "ClientId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TrafficTimetable.Domain.ClientTag", b =>
                {
                    b.HasOne("TrafficTimetable.Domain.Client", "Client")
                        .WithMany("Tags")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TrafficTimetable.Domain.Stop", "Stop")
                        .WithMany("Tags")
                        .HasForeignKey("StopId");
                });
#pragma warning restore 612, 618
        }
    }
}
