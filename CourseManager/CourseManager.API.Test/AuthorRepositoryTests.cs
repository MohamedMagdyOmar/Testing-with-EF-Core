using CourseManager.API.DbContexts;
using CourseManager.API.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace CourseManager.API.Test
{
    public class AuthorRepositoryTests
    {
        [Fact]
        // MethodNameUnderTest_State_ExpectedResult
        public void GetAuthors_PageSizeIsThree_ReturnsThreeAuthors()
        {
            // Arrange

            var options = new DbContextOptionsBuilder<CourseContext>().UseInMemoryDatabase("CourseDatabaseForTesting").Options;

            using (var context = new CourseContext(options))
            {
                context.Countries.Add(new Entities.Country()
                {
                    Id = "BE",
                    Description = "Belgium"
                });

                context.Countries.Add(new Entities.Country()
                {
                    Id = "US",
                    Description = "United States Of America"
                });

                context.Authors.Add(new Entities.Author()
                { FirstName = "Kevin", LastName = "Dockx", CountryId = "BE" });

                context.Authors.Add(new Entities.Author()
                { FirstName = "Gill", LastName = "Cleeren", CountryId = "BE" });

                context.Authors.Add(new Entities.Author()
                { FirstName = "Julie", LastName = "Lerman", CountryId = "US" });

                context.Authors.Add(new Entities.Author()
                { FirstName = "Shawn", LastName = "Wildermuth", CountryId = "BE" });

                context.Authors.Add(new Entities.Author()
                { FirstName = "Deborah", LastName = "Kurata", CountryId = "US" });

                context.SaveChanges();

                var authorRepo = new AuthorRepository(context);

                // Act
                var authors = authorRepo.GetAuthors(1, 3);

                //Assert
                Assert.Equal(3, authors.Count());


            }
        }
    }
}
