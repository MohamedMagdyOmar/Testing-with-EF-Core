using CourseManager.API.DbContexts;
using CourseManager.API.Entities;
using CourseManager.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CourseManager.API.Test
{
    public class AuthorRepositoryTests
    {
        private readonly ITestOutputHelper _output;

        public AuthorRepositoryTests(ITestOutputHelper output)
        {
            _output = output;
        }
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
            //var options = new DbContextOptionsBuilder<CourseContext>().UseInMemoryDatabase($"CourseDatabaseForTesting {Guid.NewGuid()}").Options;

            // now we are going to use SQL Lite
            // DataSource = ":memory:" -> to ensure database is created in memory
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<CourseContext>().UseSqlite(connection).Options;


            using (var context = new CourseContext(options))
            {
                // to open connection to sqlite database
                context.Database.OpenConnection();

                // to make sure that Db created
                context.Database.EnsureCreated();

                var authorRepo = new AuthorRepository(context);

                // Act
                Assert.Throws<ArgumentException>(() => authorRepo.GetAuthor(Guid.Empty));
            }
        }

        [Fact]

        public void AddAuthor_AuthorWithoutCountryId_AuthorHasBEAsCountryId()
        {
            // Arrange
            //var logs = new List<string>();

            // we commented this line because we are going to use Sqlite instead of ImMemory Provider
            // var options = new DbContextOptionsBuilder<CourseContext>().UseInMemoryDatabase($"CourseDatabaseForTesting {Guid.NewGuid()}").Options;
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            var options = new DbContextOptionsBuilder<CourseContext>().UseLoggerFactory(new LoggerFactory(
                new[] {new LogToActionLoggerProvider((log) => {
                    //logs.Add(log);
                    //Debug.WriteLine(log);
                    // this will makes you see the actual SQL Server Queries that has been executed
                    _output.WriteLine(log);
                })}
                )).UseSqlite(connection).Options;

            using (var context = new CourseContext(options))
            {
                // note that we opened only the connection for one time, and we did not open it again in the other opened context.
                // this is because the connection is reused across our contexts, so it remains open untill it goes out of the scope, which means untill the test is done
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // note that below add is not required when you are using "InMemory Provider" because in this provider it doesnot depend on relational features,
                // but if you are using Sql Lite, below line should exist because it acts as foreign key for Author Table, so it will throws exception if below line is missing.
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
