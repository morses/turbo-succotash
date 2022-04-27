using Fuji.DAL.Abstract;
using Fuji.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fuji.DAL.Concrete
{
    public class FujiUserRepository : Repository<FujiUser>, IFujiUserRepository
    {
        public FujiUserRepository(FujiDbContext ctx) : base(ctx)
        {

        }

        public virtual bool Exists(FujiUser fu)
        {
            return _dbSet.Any(x => x.AspnetIdentityId == fu.AspnetIdentityId && x.FirstName == fu.FirstName && x.LastName == fu.LastName);
        }

        public virtual FujiUser? GetFujiUserByIdentityId(string identityID)
        {
            return _dbSet.Where(u => u.AspnetIdentityId == identityID).FirstOrDefault();
        }

        public virtual async Task EatAsync(FujiUser user, int appleId, DateTime timestamp)
        {
            // validate inputs?  Here or pass it off to the calling function?

            // Now we have a verified Apple and a verified User.  Let that user eat that apple!
            ApplesConsumed appleCore = new ApplesConsumed
            {
                AppleId = appleId,
                FujiUser = user,
                ConsumedAt = timestamp,
                Count = 1
            };
            _context.Add(appleCore);
            await _context.SaveChangesAsync();
            return;
        }

        public virtual Dictionary<Apple, int> GetCountOfSpecificApplesEaten(IEnumerable<Apple> appleList, FujiUser fu)
        {
            if (fu == null)
            {
                throw new ArgumentNullException();
            }

            // We can't trust that the FujiUser passed in has had it's navigation properties loaded or that it is
            // even backed by EF, so we look it up here and make sure it has what we need
            FujiUser? foundUser = _dbSet.Include("ApplesConsumeds").Where(u => u.Id == fu.Id).FirstOrDefault();

            Dictionary<Apple, int> output = new Dictionary<Apple, int>();

            if (foundUser == null || appleList == null)
            {
                return output;
            }
            // Could have done a GroupBy here.  This is slower (more calls to the db) but easier to write.  If performance
            // is an issue then move it to the SQL Server in a single query, see Data/Scripts/apples_consumed.sql
            // for an explanation and example
            foreach (Apple a in appleList)
            {
                int count = foundUser.ApplesConsumeds.Where(ac => ac.AppleId == a.Id).Select(ac => ac.Count).Sum();
                output.Add(a, count);
            }

            return output;
        }
    }
}

