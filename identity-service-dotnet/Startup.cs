// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Serverless;
using SInnovations.Azure.TableStorageRepository;

namespace IdentityServer
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            var builder = services.AddIdentityServer(options => {
                    options.IssuerUri = "http://localhost:7075";
                    options.PublicOrigin = "http://localhost:7075";
                })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            var certNameKey = Configuration["KeyVault:CertName"];
            var certificateString = Configuration[certNameKey];
            var pfxBytes = Convert.FromBase64String(certificateString);
            var cert = new X509Certificate2(pfxBytes, (string)null, X509KeyStorageFlags.MachineKeySet);
            
            builder.AddSigningCredential(cert);
            
            services.AddSingleton<IEntityTypeConfigurationsContainer, EntityTypeConfigurationsContainer>();

            services.AddSingleton(((c) =>
            {
                var account = CloudStorageAccount.Parse(Configuration["AzureWebJobsStorage"]);
                account.CreateCloudTableClient().GetTableReference(PersistedGrantContext.TABLENAME).CreateIfNotExistsAsync().Wait();
                
                return account;
            }));

            services.AddTransient<PersistedGrantContext>()
               .AddTransient<IPersistedGrantStore, TableStoragePersistedGrantStore>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(
                    System.Environment.GetEnvironmentVariable("HOST_FUNCTION_CONTENT_PATH"), "wwwroot"))
            });

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}