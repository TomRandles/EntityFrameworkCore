using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data.Database;
using SamuraiApp.Domain;
using System.Diagnostics;

namespace SamuraiApp.Test
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void CanInsertSamuraiIntoDb()
        {
            using (var ctx = new SamuraiContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                // Don't care about properties. No non-nullable properties
                var samurai = new Samurai();
                ctx.Samurais.Add(samurai);
                Debug.WriteLine($"Before saving samurai {samurai.Id}");
                ctx.SaveChanges();
                Debug.WriteLine($"After saving samurai {samurai.Id}");

                // EF Core should generate a PK Id and apply it to the entity Id
                Assert.AreNotEqual(0, samurai.Id);
            }
        }
    }
}
