﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficTimetable
{
    public class ClientDataContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=ec2-46-51-184-229.eu-west-1.compute.amazonaws.com;Port=5432;Database=d1acbcpbs7vprr;Username=pckoawqkmnqabb;Password=0573b23c5c6d1d2c26d198862455c2576417f1fcac1a42a0c17188b1a7269bc5;SslMode=Require;Trust Server Certificate=true");
        }
    }
}