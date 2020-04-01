using CourseManager.API.DbContexts;
using CourseManager.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseManager.API
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
            //services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddScoped<AuthorRepository>();

            services.AddDbContext<CourseContext>(options =>
            {
                // 2- behind the scenes, when the database is first accessed, these options will passed to the constructor you created in "CourseContext".
                // if you set breakpoint here and in the constructor in "CourseContext", you will fins that below line will call the constructor in "CourseContext"
                // and will pass to it the below "ConnectionString".
                // then this constructor will call "Main" function in "Program.cs"
                //options.UseSqlServer(@"Server = (localdb)\\mssqllocaldb; Database = CourseManagerDB; Trusted_Connection = True;");

                //4- we call "UseInMemoryDatabase" instead of above "UseSqlServer" for unit testing,
                options.UseInMemoryDatabase("CourseManagerInMemoryDB");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) 
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
