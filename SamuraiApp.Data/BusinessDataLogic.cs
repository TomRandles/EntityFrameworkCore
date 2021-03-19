using SamuraiApp.Data.Database;
using SamuraiApp.Domain;

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
            var dbResult = _ctx.SaveChanges();
            return dbResult;
        }
    }
}