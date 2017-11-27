using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;
using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using Company.API.Filter;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Company.API
{
    /// <summary>
    /// Classe Startup.
    /// </summary>
    public class Startup
    {

        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        /// <summary>
        /// Construtor da StartupBase.
        /// </summary>
        /// <param name="env">Configurações do ambiente.</param>
        /// <param name="loggerFactory">Fábrica de log.</param>
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        /// <summary>
        /// Configuração da aplicação.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Configuração dos serviços.
        /// </summary>
        /// <param name="services">Coleção de serviços.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                var settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    DefaultValueHandling = DefaultValueHandling.Include,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Include,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                config.InputFormatters.Add(new JsonInputFormatter(_logger, settings, ArrayPool<char>.Shared,
                    new DefaultObjectPoolProvider()));
                config.InputFormatters.Add(new JsonPatchInputFormatter(_logger, settings, ArrayPool<char>.Shared,
                    new DefaultObjectPoolProvider()));

                config.OutputFormatters.Add(new JsonOutputFormatter(settings, ArrayPool<char>.Shared));
                config.OutputFormatters.Add(new HttpNoContentOutputFormatter());

                config.ReturnHttpNotAcceptable = true;
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc(GetVersion(), GetSwashbuckleApiInfo());
                a.DocumentFilter<LowercaseDocumentFilter>();
            });

            services.ConfigureSwaggerGen(config =>
            {
                config.DescribeAllEnumsAsStrings();
                config.DescribeStringEnumsInCamelCase();
                config.IncludeXmlComments(GetAPIXmlCommentsPath());
                config.IncludeXmlComments(GetApplicationXmlCommentsPath());

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug, true);
            loggerFactory.AddConsole(LogLevel.Critical, true);
            loggerFactory.AddConsole(LogLevel.Information, true);
            loggerFactory.AddConsole(LogLevel.Warning, true);

            app.UseCors(a =>
            {
                a.AllowAnyOrigin();
                a.AllowAnyHeader();
                a.AllowAnyMethod();
            });

            app.UseResponseCompression();

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("pt-BR"),
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
            };

            app.UseRequestLocalization(options);
            app.UseStaticFiles();
            //app.UseMvc();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "sciensa API v1");
            });
        }

        private string GetVersion()
        {
            string version = PlatformServices.Default.Application.ApplicationVersion;

            version = "v" + version.Substring(0, version.IndexOf('.'));

            return version;
        }

        private Info GetSwashbuckleApiInfo()
        {
            var applicationName = PlatformServices.Default.Application.ApplicationName;

            if (applicationName.ToLower().EndsWith(".api"))
            {
                applicationName = applicationName.Remove(applicationName.Length - 4, 4);
            }

            return new Info
            {
                Title = "Accounts",
                Version = GetVersion(),
                Contact = new Contact
                {
                    Email = "developer@sciensa.com.br",
                    Name = "Sciensa Labs",
                    Url = "http://sciensalabs.com/"
                }
            };
        }

        private string GetAPIXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            var path = Path.Combine(app.ApplicationBasePath, $"{app.ApplicationName}.xml");

            return path;
        }

        private string GetApplicationXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            var path = Path.Combine(app.ApplicationBasePath,
                $"{app.ApplicationName.Replace(".api", ".application")}.xml");

            return path;
        }
    }
}
