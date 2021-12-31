using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Services;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		readonly string FrontendOrigin = "_FrontendOrigin";

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddCors(options =>
			{
				options.AddPolicy(
					name: FrontendOrigin,
					builder => builder
						.WithOrigins("http://localhost:8081")
						.AllowAnyHeader()
						.AllowAnyMethod()
				// .AllowCredentials()
				);
			});
			services.AddControllers();
			services.AddCouchbase(Configuration.GetSection("Couchbase"));
			services.AddCouchbaseBucket<INamedBucketProvider>("TodoAppBucket");
			services.AddSingleton<ICouchbaseService, CouchbaseService>();
			services.AddSingleton<IUserService, UserService>();
			services.AddSingleton<IAuthTokenService, AuthTokenService>();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors(FrontendOrigin);

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			appLifetime.ApplicationStopped.Register(() =>
			{
				app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
			});
		}
	}
}
