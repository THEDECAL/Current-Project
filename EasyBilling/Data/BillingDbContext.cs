using EasyBilling.Models.Pocos;
using EasyBilling.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyBilling.Data
{
    public class BillingDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<ControllerName> ControllersNames { get; set; }
        public DbSet<DeviceState> DeviceStates { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AccessRight> AccessRights { get; set; }
        public DbSet<CashOutlay> CashOutlays { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }

        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
            //this.Database.EnsureDeleted();
            //this.Database.EnsureCreated();
        }
    }
}