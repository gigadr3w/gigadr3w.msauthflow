
using gigadr3w.msauthflow.autenticator.api.Filters;
using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using gigadr3w.msauthflow.common.Configurations;
using gigadr3w.msauthflow.common.Loggers;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.dataaccess.mysql;
using gigadr3w.msauthflow.dataaccess.mysql.Contexes;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace gigadr3w.msauthflow.autenticator.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Read configurations
            MySqlDbContextConfiguration mysqlConfiguration = builder.Configuration.GetSection(nameof(MySqlDbContextConfiguration)).Get<MySqlDbContextConfiguration>();
            JwtTokenConfiguration jwtConfirutation = builder.Configuration.GetSection(nameof(JwtTokenConfiguration)).Get<JwtTokenConfiguration>();
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

            //local memory cache for request throttling, analysis etc.
            builder.Services.AddMemoryCache();

            // Authentication service
            builder.Services.AddScoped<ILoginService, LoginService>();

            //JwtToken generator service
            builder.Services.AddScoped<IJwtTokenService>(instance => new JwtTokenService(jwtConfirutation));

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                // Add exception filter
                options.Filters.Add<ExceptionHandlerFilter>();
            });

            // Add Swagger
            builder.Services.AddSwaggerGen(configuration =>
            {
                configuration.SwaggerDoc("v1", new OpenApiInfo { Title = "API Authorizer", Version = "v1" });
                configuration.EnableAnnotations();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            //ModelBinding reads content before action filter. 
            //Here, to restore the buffer 
            //NOTICE I  - before the controller/endpoint mapping.
            //NOTICE II - here there is only one endpoint... remember that this operation will be applied on all requests
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            app.MapControllers();

            app.Run();
        }
    }
}