using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Data.Database;
using SamuraiApp.Domain;

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
    }
}