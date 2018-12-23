using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TrafficTimetable.Domain;

namespace TrafficTimetable
{
    public class ClientDataContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ClientTag>().HasKey(table => new {
                table.ClientId,
                table.TagName
            });
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Stop> Stops { get; set; }

        public DbSet<ClientTag> ClientTags { get; set; }

        public DbSet<ClientState> ClientStates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=ec2-46-137-99-175.eu-west-1.compute.amazonaws.com;Port=5432;Database=d7mrml4jq1k4ln;Username=yqjapbavfngqnj;Password=0e023237c26b52c5b9eccf5b8fa87e9d1d0e268a1caa3b730908b4d19113e80e; SslMode=Require;Trust Server Certificate=true");
        }
    }
}
