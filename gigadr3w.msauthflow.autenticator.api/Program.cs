
using gigadr3w.msauthflow.authenticator.iterator.Services;
using gigadr3w.msauthflow.common.Configurations;
using gigadr3w.msauthflow.common.Loggers;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.dataaccess.mysql;
using gigadr3w.msauthflow.dataaccess.mysql.Contexes;
using gigadr3w.msauthflow.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace gigadr3w.msauthflow.autenticator.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Read configurations
            MySqlDbContextConfiguration mysqlConfiguration = builder.Configuration.GetSection(nameof(MySqlDbContextConfiguration)).Get<MySqlDbContextConfiguration>();
            LoggingConfiguration loggingConfiguration = builder.Configuration.GetSection("Logging:LogLevel").Get<LoggingConfiguration>();

            ConsoleLoggerProvider consoleLoggerProvider = new(loggingConfiguration);

            // Adding specific mysql-dbcontext
            builder.Services.AddDbContext<DataContext>(options =>
            {
                //specify domain
                options.UseMySql(mysqlConfiguration.ConnectionString, 
                    ServerVersion.AutoDetect(mysqlConfiguration.ConnectionString));

                //add my own logger provider
                options.UseLoggerFactory(
                    LoggerFactory.Create(builder => builder.AddProvider(consoleLoggerProvider))
                );
            });

            //add my own logger provider
            builder.Services.AddLogging(builder => builder.AddProvider(consoleLoggerProvider));

            // Adding generic service for dataaccess (it uses repository pattern)
            builder.Services.AddScoped(typeof(IDataAccess<>), typeof(MySQLDataAccess<>));

            // Authentication service
            builder.Services.AddScoped<IAuthenticatorService, AuthenticatorService>();

            // Add services to the container.
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}