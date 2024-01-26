using gigadr3w.msauthflow.authenticator.iterator.Configurations;
using gigadr3w.msauthflow.authenticator.iterator.Filters;
using gigadr3w.msauthflow.authenticator.iterator.Handlers;
using gigadr3w.msauthflow.authenticator.iterator.Services;
using gigadr3w.msauthflow.backoffice.iterator.Services;
using gigadr3w.msauthflow.common.Configurations;
using gigadr3w.msauthflow.common.Loggers;
using gigadr3w.msauthflow.dataaccess.Interfaces;
using gigadr3w.msauthflow.dataaccess.mysql;
using gigadr3w.msauthflow.dataaccess.mysql.Configuration;
using gigadr3w.msauthflow.dataaccess.mysql.Contexes;
using gigadr3w.msauthflow.sharedcache.Interfaces;
using gigadr3w.msauthflow.sharedcache.redis;
using gigadr3w.msauthflow.sharedcache.redis.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace gigadr3w.msauthflow.backoffice.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Read configurations

            MySqlConfiguration mySqlDbContextConfiguration = builder.Configuration.GetSection(nameof(MySqlConfiguration)).Get<MySqlConfiguration>();
            LoggingConfiguration loggingConfiguration = builder.Configuration.GetSection("Logging:LogLevel").Get<LoggingConfiguration>();

            ConsoleLoggerProvider consoleLoggerProvider = new(loggingConfiguration);

            // Add configurations to the container

            builder.Services.Configure<JwtTokenConfiguration>(builder.Configuration.GetSection(nameof(JwtTokenConfiguration)));

            builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection(nameof(RedisConfiguration)));

            // Add services to the container.

            // Adding specific mysql-dbcontext
            builder.Services.AddDbContext<DataContext>(options =>
            {
                //specify domain
                options.UseMySql(mySqlDbContextConfiguration.ConnectionString,
                    ServerVersion.AutoDetect(mySqlDbContextConfiguration.ConnectionString));

                //add my own logger provider
                options.UseLoggerFactory(
                    LoggerFactory.Create(builder => builder.AddProvider(consoleLoggerProvider))
                );
            });

            //Add logging provider
            builder.Services.AddLogging(builder => builder.AddProvider(consoleLoggerProvider));

            //Adding generic data access mysql implementation service
            builder.Services.AddScoped(typeof(IDataAccess<>), typeof(MySQLDataAccess<>));

            //Adding shared cache client service
            builder.Services.AddScoped<ISharedCache, RedisSharedCache>();

            //Adding Jwt token service (used by JwtAuthenticatorHandler)
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            //Adding Item Service
            builder.Services.AddScoped<IItemService, ItemService>();

            //Add my own authentication filter handler
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtTokenConfiguration.DEFAULT_SCHEMA;
                options.DefaultChallengeScheme = JwtTokenConfiguration.DEFAULT_SCHEMA;
            })
            .AddScheme<AuthenticationSchemeOptions, JwtAuthenticatorHandler>(JwtTokenConfiguration.DEFAULT_SCHEMA, options => { });

            builder.Services.AddControllers();

            // Add swagger
            builder.Services.AddSwaggerGen(configuration =>
            {
                configuration.SwaggerDoc("v1", new OpenApiInfo { Title = "API Backoffice", Version = "v1" });
                configuration.EnableAnnotations();
                configuration.OperationFilter<SwaggerAuthenticationFilter>();   //Add authentication filter handled by JwtAuthenticatorHandler
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}