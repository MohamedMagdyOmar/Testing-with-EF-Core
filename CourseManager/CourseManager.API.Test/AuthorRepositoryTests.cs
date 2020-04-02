using CourseManager.API.DbContexts;
using CourseManager.API.Entities;
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

            var options = new DbContextOptionsBuilder<CourseContext>().UseInMemoryDatabase($"CourseDatabaseForTesting {Guid.NewGuid()}").Options;

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
            }

            using (var context = new CourseContext(options))
            {
                var authorRepo = new AuthorRepository(context);

                // Act
                var authors = authorRepo.GetAuthors(1, 3);

                //Assert
                Assert.Equal(3, authors.Count());
            }

        }

        [Fact]

        public void GetAuthor_EmptyGuid_ThrowsArgumentException()
        {
            // Arrange

            var options = new DbContextOptionsBuilder<CourseContext>().UseInMemoryDatabase($"CourseDatabaseForTesting {Guid.NewGuid()}").Options;

            using (var context = new CourseContext(options))
            {
                var authorRepo = new AuthorRepository(context);

                // Act
                Assert.Throws<ArgumentException>(() => authorRepo.GetAuthor(Guid.Empty));
            }
        }

        [Fact]

        public void AddAuthor_AuthorWithoutCountryId_AuthorHasBEAsCountryId()
        {
            // Arrange

            var options = new DbContextOptionsBuilder<CourseContext>().UseInMemoryDatabase($"CourseDatabaseForTesting {Guid.NewGuid()}").Options;


            using (var context = new CourseContext(options))
            {
                context.Countries.Add(new Entities.Country()
                {
                    Id = "BE",
                    Description = "Belgium"
                });

                context.SaveChanges();
            }

            using (var context = new CourseContext(options))
            {
                var authorRepo = new AuthorRepository(context);

                var authorToAdd = new Author()
                {
                    FirstName = "Kevin",
                    LastName = "Dockx",
                    Id = Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b")
                };

                authorRepo.AddAuthor(authorToAdd);
                authorRepo.SaveChanges();
            }

            using (var context = new CourseContext(options))
            {
                var authorRepo = new AuthorRepository(context);

                var addedAuthor = authorRepo.GetAuthor(Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b"));

                Assert.Equal("BE", addedAuthor.CountryId);
            }     
        }
    }
}
