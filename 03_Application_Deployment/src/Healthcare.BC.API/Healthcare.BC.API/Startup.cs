using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Healthcare.BC.Application.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCors();
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Info { Title = " Healthcare Eiligibility Service", Version = "v1" });
            });


            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<HealthcareBC.ESC.DocProof, HealthcareBC.ProofDocSvc.DocProof>();
            //    cfg.CreateMap<List<HealthcareBC.ESC.ContractTransaction>, List<HealthcareBC.Tracker.ContractTransaction>>();
            //    cfg.CreateMap<HealthcareBC.ESC.ContractTransaction, HealthcareBC.Tracker.ContractTransaction>();
            //    cfg.CreateMap<List<HealthcareBC.Tracker.ContractTransaction>, List<HealthcareBC.ESC.ContractTransaction>>();
            //});
            //Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());

            //services.AddAutoMapper();
            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", " Healthcare Eiligibility Service");

            });

            app.UseMvc();
        }
    }
}
