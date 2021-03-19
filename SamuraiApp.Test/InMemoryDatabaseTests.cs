using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Data.Database;
using SamuraiApp.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.Test
{
    [TestClass]
    public class InMemoryDatabaseTests
    {
        [TestMethod]
        public void CanInsertSamuraiIntoDb()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("CanInsertSamurai");
            using (var ctx = new SamuraiContext(optionsBuilder.Options))
            {
                // Don't care about properties. No non-nullable properties
                var samurai = new Samurai();
                ctx.Samurais.Add(samurai);
                Assert.AreEqual(EntityState.Added, ctx.Entry(samurai).State);
            }
        }

        [TestMethod]
        public void CanAddSamuraisByName()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("CanAddSamuraisByName");
            var ctx = new SamuraiContext(optionsBuilder.Options);

            var businessLogic = new BusinessDataLogic(ctx);

            var names = new string[] { "John", "Harry", "James", "Michael" };

            var result = businessLogic.AddSamuraisByName(names);

            Assert.AreEqual(names.Length, result);
        }

        [TestMethod]
        public void CanInsertSingleMethod()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase("CanInsertSingleMethod");
            using (var ctx = new SamuraiContext(optionsBuilder.Options))
            {
                var businessLogic = new BusinessDataLogic(ctx);
                var samurai = new Samurai { Name = "Tommy" };
                var result = businessLogic.InsertNewSamurai(samurai);
            }
            // Use different context to same Db instance
            using (var ctx2 = new SamuraiContext(optionsBuilder.Options))
            {
                Assert.AreEqual(1, ctx2.Samurais.Count());
            }
        }

        [TestMethod]
        public void CanInsertSamuraiwithQuotes()
        {

            var samuraiGraph = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote> {
          new Quote { Text = "Watch out for my sharp sword!" },
          new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" }
        }
            };
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("SamuraiWithQuotes");
            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                var result = bizlogic.InsertNewSamurai(samuraiGraph);
            };
            using (var context2 = new SamuraiContext(builder.Options))
            {
                var samuraiWithQuotes = context2.Samurais.Include(s => s.Quotes).FirstOrDefaultAsync().Result;
                Assert.AreEqual(2, samuraiWithQuotes.Quotes.Count);
            }
        }

        [TestMethod, TestCategory("SamuraiWithQuotes")]
        public void CanGetSamuraiwithQuotes()
        {
            int samuraiId;
            var builder = new DbContextOptionsBuilder();
            builder.UseInMemoryDatabase("SamuraiWithQuotes");
            using (var seedcontext = new SamuraiContext(builder.Options))
            {
                var samuraiGraph = new Samurai
                {
                    Name = "Kyuzo",
                    Quotes = new List<Quote> {
                        new Quote { Text = "Watch out for my sharp sword!" },
                        new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" }
                    }
                };
                seedcontext.Samurais.Add(samuraiGraph);
                seedcontext.SaveChanges();
                samuraiId = samuraiGraph.Id;
            }
            using (var context = new SamuraiContext(builder.Options))
            {
                var bizlogic = new BusinessDataLogic(context);
                var result = bizlogic.GetSamuraiWithQuotes(samuraiId);
                Assert.AreEqual(2, result.Quotes.Count);
            };
        }
    }
}