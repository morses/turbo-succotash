using Fuji.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fuji.DAL.Abstract
{
    public interface IAppleRepository : IRepository<Apple>
    {
        int GetTotalConsumed(IQueryable<Apple> apples);
    }
}
