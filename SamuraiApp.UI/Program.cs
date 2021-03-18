using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data.Database;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SamuraiApp.UI
{
    class Program
    {
        private static SamuraiContext _context = new SamuraiContext();
        static void Main(string[] args)
        {
            _context.Database.EnsureCreated();
            AddSamurai("Jack", "Joan", "Harry", "Meghan");

            GetSamurai();
            AddVariousTypes();
            QueryFilters();
            RetrieveAndUpdateSamurai();
            RetrieveAndUpdateMultipleSamurai();
            RetrieveAndDeleteSamurai();
            QueryAggregates();
            QueryAndUpdateBattles_Disconnected();
            InsertSamuraiWithAQuote();
            InsertSamuraiMultipleQuotes();
            AddQuoteToExistingSamuraiWhileTracking();
            AddQuoteToExistingSamuraiNoTracking(1);
            Simpler_AddQuoteToExistingSamuraiNoTracking(1);
            EagerLoadSamuraiWithQuotes();
            ProjectSomeProperties();
            ProjectSomePropertiesWithQuotes();
            ExplicitLoadQuotes();
            LazyLoadingQuotes();
            FilteringWithRelatedData();
            ModifyingRelatedDataWhileTracking();
            ModifyingRelatedDataNoTracking();
            AddNewSamuraiToExistingBattle();
            ReturnBattleWithSamurais();
            ReturnAllBattlesWithSamurais();
            AddAllSamuraisToAllBattles();
            RemoveSamuraisFromABattle();
            RemoveSamuraisFromABattleExplicit();
            AddNewSamuraiWithHorse();
            AddNewHorseToExistingSamuraiUsingId();
            AddNewHorseToExistingSamuraiObject();
            AddNewHorseToDisconnectedExistingSamuraiObject();
            ReplaceAHorse();
            ReplaceAHorse2();
            GetSamuraiWithHorse();
            GetHorsesWithSamurai();
            QuerySamuraiBattleStats();
            QueryUsingRawSql();
            QueryRelatedUsingRawSql();
            QueryUsingRawSQLWithInterpolation();
            DangerQueryUsingRawSQLWithInterpolation();
            QueryUsingFromSQLRawWithStoredProcedure();
            ExecuteSomeRawSQL();

            Console.Write("Press any key");
            Console.ReadKey();
        }
        private static void AddSamurai(params string[] names)
        {
            foreach (var name in names)
            {
                var samurai = new Samurai { Name = name };
                _context.Samurais.Add(samurai);
            }
            _context.SaveChanges();
        }
        private static void AddVariousTypes()
        {
            // Batch operations example - different types
            _context.Samurais.AddRange(
                new Samurai { Name = "Shimado" },
                new Samurai { Name = "Okamoto" });
            _context.Battles.AddRange(
                new Battle { Name = "Battle of Anegawa" },
                new Battle { Name = "Battle of Nagashino" }
                );
            _context.SaveChanges();
        }

        private static void QueryFilters()
        {
            var samurais = _context.Samurais.Where(s => EF.Functions.Like(s.Name, "J%")).ToList();
        }

        private static void RetrieveAndUpdateSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            if (samurai != null)
            {
                samurai.Name += "San";
                _context.SaveChanges();
            }

        }

        private static void RetrieveAndUpdateMultipleSamurai()
        {
            var samurais = _context.Samurais.Skip(1).Take(4).ToList();
            if (samurais != null)
            {
                samurais.ForEach(s => s.Name += "San");
                _context.SaveChanges();
            }
        }

        public static void RetrieveAndDeleteSamurai()
        {
            var samurai = _context.Samurais.FirstOrDefault();
            if (samurai != null)
            {
                _context.Remove(samurai);
                _context.SaveChanges();
            }
        }
        private static void QueryAggregates()
        {
            var name = "Jane";
            var samurai = _context.Samurais.FirstOrDefault(s => s.Name == name);
        }
        private static void GetSamurai()
        {
            var samurais = _context
                .Samurais
                .TagWith("Console app: Get samurais")    // Query tags. Will not cause an SQL injection
                .ToList();
            foreach (var samurai in samurais)
            {
                Console.WriteLine($"Name: {samurai.Name}");
            }
        }
        private static void QueryAndUpdateBattles_Disconnected()
        {
            // Demonstrate disconnected context 

            List<Battle> disconnectedBattles;
            //Retrieve objects from one context
            using (var ctx = new SamuraiContext())
            {

                disconnectedBattles = ctx.Battles.ToList();
            } // ctx is disposed - out of scope

            // Update objects
            disconnectedBattles.ForEach(b =>
            {
                b.StartDate = new DateTime(1570, 01, 01);
                b.EndDate = new DateTime(1570, 12, 01);
            });
            // update and save objects using a different context
            // Has no tracking information on retrieved objects
            using (var ctx2 = new SamuraiContext())
            {
                ctx2.UpdateRange(disconnectedBattles);
                ctx2.SaveChanges();
            }
        }
        public static void InsertSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kambei Shimada",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "I have come to save you" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        public static void InsertSamuraiMultipleQuotes()
        {
            var samurai = new Samurai
            {
                Name = "Kyuzo",
                Quotes = new List<Quote>
                {
                    new Quote { Text = "Watch my sword" },
                    new Quote { Text = "Mind my sword" },
                    new Quote { Text = "I told you to mind my sword" }
                }
            };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }
        public static void AddQuoteToExistingSamuraiWhileTracking()
        {
            // Context tracking the samurai. Change tracker figures out FK requirements.
            var samurai = _context.Samurais.FirstOrDefault();

            samurai.Quotes.Add(
                    new Quote { Text = "I'm sure you're glad I saved you." }
            );
            _context.SaveChanges();
        }

        public static void AddQuoteToExistingSamuraiNoTracking(int samuraiId)
        {

            var samurai = _context.Samurais.Find(samuraiId);
            if (samurai != null)
            {
                samurai.Quotes.Add(
                    new Quote { Text = "Now that I've saved you, can you make my dinner?" }
                );

                //Save using different context - disconnected scenario
                using (var ctxNew = new SamuraiContext())
                {
                    ctxNew.Samurais.Update(samurai);
                    ctxNew.SaveChanges();
                }
            }
        }
        public static void Simpler_AddQuoteToExistingSamuraiNoTracking(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            // Better solution. Always update child's FK value (Quote.SamuraiId).  
            if (samurai != null)
            {
                samurai.Quotes.Add(
                    new Quote { Text = "Now that I've saved you, can you make my dinner?", SamuraiId = samuraiId }
                );

                //Save using different context - disconnected scenario
                using (var ctxNew = new SamuraiContext())
                {
                    ctxNew.Samurais.Update(samurai);
                    ctxNew.SaveChanges();
                }
            }
        }
        public static void EagerLoadSamuraiWithQuotes()
        {
            // Include - include related objects in query
            // EF Core: creates Left Join to retrieve data
            // var samurais = _context.Samurais.Include(s => s.Quotes).ToList();
            //Use AsSplitQuery
            var samurais = _context.Samurais.AsSplitQuery().Include(s => s.Quotes).ToList();

            // Include filtering
            var thankfulSamurais = _context.Samurais.Include(s => s.Quotes.Where(q => q.Text.Contains("Thanks"))).ToList();
        }
        public static void ProjectSomeProperties()
        {
            // Return two properties into new type. Use anonymouse type in lambda expression - new {}
            // Returned type known only to this method
            // Returns here a list of anonymous types; each type containing two properties
            var someProperties = _context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
        }
        public static void ProjectSomePropertiesWithQuotes()
        {
            // Include child quotes in projection
            var somePropertiesAndQuotes = _context.Samurais.Select(s => new { s.Id, s.Name, s.Quotes }).ToList();

            // Get just count of quotes. Creates new NumberOfQuotes property in anonymous type
            var somePropertiesAndQuotesCount = _context.Samurais.Select(s => new { s.Id, s.Name, NumberOfQuotes=s.Quotes.Count }).ToList();

            // Filter on quotes
            var somePropertiesAndHappyQuotesOnly = _context.Samurais.Select(s => new { s.Id, 
                                                                                       s.Name, 
                                                                                       HappyQuotes = s.Quotes
                                                                                                       .Where(q=>q.Text.Contains("Happy")) })
                                                                                        .ToList();
            // Project full entity objects (samurais) with filtered child objects
            // EF Core connects the HappyQuotes to the Samurai
            var samuraisAndQuotes = _context.Samurais.Select(s => new { Samurai=s,
                                                                        HappyQuotes = s.Quotes
                                                                                       .Where(q => q.Text.Contains("Happy")) 
                                                                      })
                                                     .ToList();
            //Edit name in first samurai returned in the result
            // NB: Entities recognized by the DB Context are tracked! _context.ChangeTracker.Entries(), results - first samurai entry modified! 
            var samurai = samuraisAndQuotes[0].Samurai.Name += "The happiest"; 
        }

        private static void ExplicitLoadQuotes()
        {
            // Get samurai with no horse
            var sam = _context.Samurais.Where(s => s.Horse == null).FirstOrDefault();
            if (sam != null)
            {
                // Ensure that there is a horse in the Db first
                _context.Set<Horse>().Add(new Horse { SamuraiId = sam.Id, Name = "Black Beauty" });
                _context.SaveChanges();
                // Clear change tracker
                _context.ChangeTracker.Clear();

                var samurai = _context.Samurais.Find(sam.Id);
                if (samurai != null)
                {
                    _context.Entry(samurai).Collection(s => s.Quotes).Load();
                    _context.Entry(samurai).Reference(s => s.Horse).Load();
                }
            }
        }

        private static void LazyLoadingQuotes()
        {
            var samurais = _context.Samurais.FirstOrDefault();
            if (samurais != null)
            {
                var quoteCount = samurais.Quotes.Count(); // Needs Lazy Loading to be setup first. 
            }
        }
        private static void FilteringWithRelatedData()
        {
            var samurais = _context.Samurais.Where(s => s.Quotes.Any(q => q.Text.Contains("Happy")))
                                            .ToList();   
        }
        private static void ModifyingRelatedDataWhileTracking()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            if (samurai != null)
            {
                samurai.Quotes[0].Text = "Did you hear that?";
                _context.Quotes.Remove(samurai.Quotes[2]);
                _context.SaveChanges();
            }
        }
        private static void ModifyingRelatedDataNoTracking()
        {
            var samurai = _context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 2);
            if (samurai != null)
            {
                var quote = samurai.Quotes[0];
                quote.Text += "Did you hear that again?";
                using var ctx2 = new SamuraiContext();
                // The following approach causes some strange EF Core SQL generation
                // Update samurai and all existing quotes. Not what was required! 
                //ctx2.Quotes.Update(quote);

                // Solution  - use Entry. Much more control. Will focus only on entity passed.
                // Tracker will only track this one quote - SQL generated will reflect this
                ctx2.Entry(quote).State = EntityState.Modified;
                ctx2.SaveChanges();
            }
        }

        private static void AddNewSamuraiToExistingBattle()
        {
            var battle = _context.Battles.FirstOrDefault();
            battle.Samurais.Add(new Samurai { Name = "Harry", });
            _context.SaveChanges();
        }

        private static void ReturnBattleWithSamurais()
        {
            // Don't have to know about the join table
            // EF Core - navigates through the join table, takes the results and sorts out the objects and
            // relationships
            var battle = _context.Battles.Include(s => s.Samurais).FirstOrDefault();

            // many battles with many Samurais
            var battles = _context.Battles.Include(s => s.Samurais).ToList();
        }

        private static void ReturnAllBattlesWithSamurais()
        {
            // many battles with many Samurais
            var battles = _context.Battles.Include(s => s.Samurais).ToList();
        }
        private static void AddAllSamuraisToAllBattles()
        {
            // The following code presumes there are no existing relationships that might cause a
            // PK duplication exception
            // var battles = _context.Battles.Include.ToList();

            // Include existing samurais in battles list. 
            // Note that this solution may not be optimal - too much data. 
            var battles = _context.Battles.Include(s =>s.Samurais).ToList();
            var samurais = _context.Samurais.ToList();

            foreach(var battle in battles)
            {
                // Only new samurais will now be added
                battle.Samurais.AddRange(samurais);
            }
            _context.SaveChanges();
        }
        private static void RemoveSamuraisFromABattle()
        {
            /* 
             * Need to remove fluent API for payload 
            // Need full graph of the relationship in memory
            var battleWithSamurai = _context.Battles
                .Include(b => b.Samurais.Where(s => s.Id == 12))
                .Single(s => s.BattleId == 1);
            var samurai = battleWithSamurai.Samurais[0];
            // Following removes the Samurai-Battle entry from the join table
            // Note: grabbing samurai and battle individually and deleting the samurai from tbe battle object
            // will not work - relationship not being tracked. 
            battleWithSamurai.Samurais.Remove(samurai);
            _context.SaveChanges();
            */
        }

        private static void RemoveSamuraisFromABattleExplicit()
        {
            // Retrieve the join data with the Set command: creates  a DbSet<T> entity
            // SingleOrDefault returns matching row. NB: only one row that matches!
            var bs = _context.Set<BattleSamurai>()
                             .SingleOrDefault(bs => bs.BattleId == 1 && bs.SamuraiId == 10);
            if (bs != null)
            {
                // Use dbset entity to remove, short cut approach
                _context.Remove(bs); // 
                _context.SaveChanges();
            }
        }

        private static void AddNewSamuraiWithHorse()
        {
            var samurai = new Samurai { Name = "Johnny" };
            samurai.Horse = new Horse { Name = "Red Rum" };
            _context.Samurais.Add(samurai);
            _context.SaveChanges();
        }

        private static void AddNewHorseToExistingSamuraiUsingId()
        {
            // Find a samurai without a horse
            var samurai = _context.Samurais.FirstOrDefault(s => s.Horse == null);
            if (samurai != null)
            {
                // Create new horse object; assign samurai FK
                var horse = new Horse { Name = "Red Rum", SamuraiId = samurai.Id };
                _context.Add(horse);
                _context.SaveChanges();
            }
        }
        private static void AddNewHorseToExistingSamuraiObject()
        {
            // Find a samurai without a horse
            var samurai = _context.Samurais.FirstOrDefault(s => s.Horse == null);
            if (samurai != null)
            {
                // Create new horse object; assign samurai.Horse
                samurai.Horse = new Horse { Name = "Red Rum2" };
                _context.SaveChanges();
            }
        }

        private static void AddNewHorseToDisconnectedExistingSamuraiObject()
        {
            // Find a samurai without a horse
            var samurai = _context.Samurais.AsNoTracking().FirstOrDefault(s => s.Horse == null);
            if (samurai != null)
            {
                // Create new horse object; assign samurai.Horse
                samurai.Horse = new Horse { Name = "Red Rum3" };
                var newCtx = new SamuraiContext();
                // Samurai already has an Id - exists-unchanged. 
                // Horse does not have an Id. Will be inserted into Horse table with Samurai ID as FK
                newCtx.Samurais.Attach(samurai);
                newCtx.SaveChanges();
            }
        }

        private static void ReplaceAHorse()
        {
            var samurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Id == 5);
            if (samurai != null)
            {
                // EF Core will delete the old horse entry and then add new one.
                // Constraint does not allow horse to exist without a Samurai
                samurai.Horse = new Horse { Name= "Kicking King" };
                _context.SaveChanges();
            }
        }

        private static void ReplaceAHorse2()
        {
            // Find a samurai with no horse
            var samurai = _context.Samurais.Where(s => s.Horse == null).FirstOrDefault();

            // Horse object must be in memory for this to work.
            var horse = _context.Set<Horse>().FirstOrDefault(h => h.Name == "Black Beauty");
            // Assign owner by setting the FK!
            horse.SamuraiId = samurai.Id;
            _context.SaveChanges();
        }

        private static void GetSamuraiWithHorse()
        {
            // project horse (include) return a graph
            var samurais = _context.Samurais.Include(s => s.Horse).ToList();
        }

        private static void GetHorsesWithSamurai()
        {
            // No Horse DbSet exists
            // No samurai property in Horse 
            // Not as straight forward to load horses with samurai

            //Use db context set method
            var horseOnly = _context.Set<Horse>().Find(3);

            // Tackle the challenge from the samurai perspective
            // Query for a smaurai,
            // Drill through the relationship to filter on the horse Id
            var horseWithSamurai = _context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Horse.Id == 3);

            // Get horse samurai pairs - new anonymous object 
            var horseSamuraiPairs = _context.Samurais.Where(s => s.Horse != null).Select(s => new { Samurai = s, Horse = s.Horse }).ToList();
        }
        private static void QuerySamuraiBattleStats()
        {
            // Queries new db SamuraiBattleStats view
            var battleStats = _context.SamuraiBattleStats.ToList();
        }
        private static void QueryUsingRawSql()
        {
            var samurais = _context.Samurais.FromSqlRaw("Select * from samurais").ToList();
        }
        private static void QueryRelatedUsingRawSql()
        {
            var samurais = _context.Samurais
                                   .FromSqlRaw("Select Id, Name from samurais")
                                   .Include(q => q.Quotes)
                                   .ToList();
        }
        private static void QueryUsingRawSQLWithInterpolation()
        {
            string name = "Harry";
            var samurais = _context.Samurais.FromSqlRaw($"Select * from Samurais where name = {name}").ToList();
        }

        private static void DangerQueryUsingRawSQLWithInterpolation()
        {
            // Do not use single quotes with parameter. Suseptable to SQL injection attack.
            string name = "Harry";
            var samurais = _context.Samurais.FromSqlRaw($"Select * from Samurais where name = '{name}'").ToList();
        }

        private static void QueryUsingFromSQLRawWithStoredProcedure()
        {
            var text = "Happy";
            var samurais = _context.Samurais.FromSqlRaw("EXEC dbo.SamuraisWhoSaidAWord {0}", text).ToList();

            samurais = _context.Samurais
                               .FromSqlInterpolated($"EXEC dbo.SamuraisWhoSaidAWord {text}")
                               .ToList();
        }
        private static void ExecuteSomeRawSQL()
        {
            // Find a samurai with quotes
            var samurai = _context.Samurais.FirstOrDefault(s => s.Quotes != null);
            if (samurai != null)
            {
                // Must use parameters! 
                var affected = _context.Database
                                       .ExecuteSqlRaw("EXEC dbo.DeleteQuotesForSamurai {0}", samurai.Id);
            }
        }
    }
}