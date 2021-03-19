using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data.Database;
using SamuraiApp.Domain;
using System.Linq;

namespace SamuraiApp.Data
{
    public class BusinessDataLogic
    {
        private SamuraiContext _ctx;

        public BusinessDataLogic(SamuraiContext ctx)
        {
            _ctx = ctx;
        }
        public BusinessDataLogic()
        {
            _ctx = new SamuraiContext();
        }
        public int AddSamuraisByName(params string[] names)
        {
            foreach (var name in names)
            {
                _ctx.Samurais.Add(new Samurai { Name = name });
            }
            // Return save changes count - use in testing
            return _ctx.SaveChanges();
            
        }
        public int InsertNewSamurai(Samurai samurai)
        {
            _ctx.Samurais.Add(samurai);
            return _ctx.SaveChanges();
        }

        public Samurai GetSamuraiWithQuotes(int samuraiId)
        {
            var samuraiWithQuotes = _ctx.Samurais.Where(s => s.Id == samuraiId)
                                                 .Include(s => s.Quotes)
                                                 .FirstOrDefault();
            return samuraiWithQuotes;
        }
    }
}