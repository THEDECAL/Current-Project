using EasyBilling.Models.Pocos;
using EasyBilling.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyBilling.Data
{
    public class BillingDbContext : IdentityDbContext<IdentityUser>
    {
        //public static readonly ILoggerFactory loggerFactory
        //    = LoggerFactory.Create(b => b.AddFilter((category, level) =>
        //                                    category == DbLoggerCategory.Database.Command.Name
        //                                    && level == LogLevel.Information)
        //                                .AddConsole());

        public DbSet<ControllerName> ControllersNames { get; set; }
        public DbSet<DeviceState> DeviceStates { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AccessRight> AccessRights { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }

        public BillingDbContext(DbContextOptions<BillingDbContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseLoggerFactory(loggerFactory)
        //        .UseSqlServer("Data Source=GEEK-NB;Initial Catalog=EasyBillingDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //    //base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);
        }
    }
}