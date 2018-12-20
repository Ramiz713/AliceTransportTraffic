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
            optionsBuilder.UseNpgsql("Host=ec2-46-51-184-229.eu-west-1.compute.amazonaws.com;Port=5432;Database=d1acbcpbs7vprr;Username=pckoawqkmnqabb;Password=0573b23c5c6d1d2c26d198862455c2576417f1fcac1a42a0c17188b1a7269bc5;SslMode=Require;Trust Server Certificate=true");
        }
    }
}
