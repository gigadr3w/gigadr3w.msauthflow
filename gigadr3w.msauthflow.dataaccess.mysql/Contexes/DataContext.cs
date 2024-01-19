using gigadr3w.msauthflow.common.Extensions;
using gigadr3w.msauthflow.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace gigadr3w.msauthflow.dataaccess.mysql.Contexes
{
    public class DataContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Role> Roles { get; set; }

        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder ()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Using Development MySQL Connection String");
                    configurationBuilder.AddJsonFile("appsettings.Development.json");
                }
                IConfiguration configuration = configurationBuilder.Build();

                string connectionString = configuration.GetConnectionString("Default")
                    ?? throw new ArgumentException("MySQL connection missing in action!");

                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                //to give more information in case of exception
                optionsBuilder.EnableSensitiveDataLogging();
            }

            base.OnConfiguring(optionsBuilder);
            //TODO - includere?
            //optionsBuilder.UseMemoryCache();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API : map entities, don't use arguments mapping!

            //tables

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Role>()
                .Property(r => r.EnabledService)
                .IsRequired()
                .HasMaxLength(150);
            modelBuilder.Entity<Role>()
                .Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(150);

            //records

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { Id = 1, Name = "BackofficeRead", EnabledService = "Backoffice", Description = "Read operations from backoffice service" },
                    new Role { Id = 2, Name = "BackofficeWrite", EnabledService = "Backoffice", Description = "Both add and update operations from backoffice service" },
                    new Role { Id = 3, Name = "BackofficeDelete", EnabledService = "Backoffice", Description = "Delete operations from backoffice service" },
                    new Role { Id = 4, Name = "ReportingRead", EnabledService = "Reporting", Description = "Read operations from reporting service" }
                );

            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    Id = 1,
                    Email = "m.rossi@msauthflow.com",
                    Password = "_Password01".Hash256(),
                    Name = "Mario Rossi"
                });

            //joins

            modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(u => u.Users)
            .UsingEntity(jt => jt.ToTable("UserRoles")
                        .HasData(
                            new { UsersId = 1, RolesId = 1 },
                            new { UsersId = 1, RolesId = 2 },
                            new { UsersId = 1, RolesId = 3 },
                            new { UsersId = 1, RolesId = 4 }
                        ));

            base.OnModelCreating(modelBuilder);
        }
    }
}
