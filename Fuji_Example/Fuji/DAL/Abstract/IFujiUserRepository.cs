using Fuji.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fuji.DAL.Abstract
{
    public interface IFujiUserRepository : IRepository<FujiUser>
    {
        FujiUser? GetFujiUserByIdentityId(string identityID);
        bool Exists(FujiUser fu);

        Task EatAsync(FujiUser user, int appleId, DateTime timestamp);

        Dictionary<Apple, int> GetCountOfSpecificApplesEaten(IEnumerable<Apple> appleList, FujiUser fu);
    }
}
