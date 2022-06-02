
using Microsoft.EntityFrameworkCore;
using MyDemoApp.Models;
using Microsoft.Extensions.Configuration;

using System.IO;
namespace MyDemoApp.Data
{
    public class DemoAppContext:DbContext
    {
        public DemoAppContext() { }

        public DemoAppContext(DbContextOptions<DemoAppContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
           
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(config.GetValue<string>("ConnectionStrings:DefaultConnection").ToString());
            }

            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
    }
}
