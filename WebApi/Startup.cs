using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.RabbitMq;
using WebApi.RabbitMQ;
using WebApi.Redis;

namespace WebApi
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
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("default_cors", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyMethod();
                });
            });

            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMqConfiguration"));
            services.Configure<RedisConfiguration>(Configuration.GetSection("RedisConfiguration"));
            services.AddSingleton<IRedisServer, RedisServer>();
            services.AddSingleton<ICacheManager, RedisCacheManager>();
            services.AddSingleton<RabbitMqService, RabbitMqService>();

            //IHostedService
            services.AddHostedService<UserConsumerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("default_cors");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
