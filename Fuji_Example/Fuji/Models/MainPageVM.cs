using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fuji.Models
{
    // #nullable disable
    public class MainPageVM
    {
        
        /// <summary>
        /// Do we have a valid FujiUser for the current request/signed in user?
        /// </summary>
        public bool HasFujiUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Apple> AllPossibleApples { get; }
        public int TotalApplesConsumedByAllUsers { get; set; }

        public MainPageVM() 
        {
            HasFujiUser = false;
            FirstName = String.Empty;
            LastName = String.Empty;
            AllPossibleApples = new List<Apple>();
            TotalApplesConsumedByAllUsers = -1;   // leave it at 0 or this flag value of -1?
        }
    }
}
