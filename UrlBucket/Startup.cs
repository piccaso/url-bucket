using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using UrlBucket.Lib.Services;

namespace UrlBucket {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(o => o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

            var xmlComments = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Title = "UrlBucket API", Version = "v1",
                    License = new License {Name = "MIT", Url = "https://github.com/piccaso/url-bucket/blob/master/LICENSE"},
                    Contact = new Contact {Url = "https://github.com/piccaso/url-bucket", Name = "UrlBucket"},
                });
                foreach (var xmlComment in xmlComments) {
                    c.IncludeXmlComments(xmlComment);
                }
            });

            services.AddTransient<StorageService>();
            services.AddTransient<DownloadService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            //app.UseDeveloperExceptionPage();
            //app.UseHsts();
            //app.UseHttpsRedirection();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlBucket API");
            });
        }
    }
}
