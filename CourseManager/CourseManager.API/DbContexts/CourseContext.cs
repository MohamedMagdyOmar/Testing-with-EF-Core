using CourseManager.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManager.API.DbContexts
{
    public class CourseContext : DbContext
    {
        // we need to initialize it from unit test with an "In-Memory" Database
        // so we added below below constructor, to initialize "In-Memory" Database
        public DbSet<Course> Courses { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public CourseContext()
        { }

        // 1- this is prefered options in ASP.Net Core App. it provides these options when registering the context in the "ConfigureServices" Method in the "StartUp" Class
        // 3- through this constructor, we can setup our "DbContext" from the "Startup" class, when we are actually running the application.

        // 3- also from unit test, you can setup your "DbContext", by initializing it and passing through an options object from unit test directly
        // 5- there is another way instead of adding below constructor, you can use "OnConfiguring" method that you used in EFCore Course, like below commented method.
        public CourseContext(DbContextOptions<CourseContext> options): base(options)
        {

        }

        // 6- this method will be triggered every time we initialize "DbContext", but if we are writing unit test, we use "optionsBuilder.UseInMemoryDatabase", not 
        // "optionsBuilder.UseSqlServer", so we need to put condition, if it is configured do not configure it again.
        // in this course we will use "Constructor approach" above, so we are commenting below lines
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server = (localdb)\\mssqllocaldb; Database = CourseManagerDB; Trusted_Connection = True;");
            }
        }
        */
    }
}
