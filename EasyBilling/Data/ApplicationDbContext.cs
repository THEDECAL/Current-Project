using System;
using System.Collections.Generic;
using System.Text;
using EasyBilling.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyBilling.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CashOutlay> CashOutlays { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}