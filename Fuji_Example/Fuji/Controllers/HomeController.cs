using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Fuji.Models;
using Fuji.DAL.Abstract;

namespace Fuji.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFujiUserRepository _fuRepo;
        private readonly IAppleRepository _appleRepo;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IFujiUserRepository fuRepo, IAppleRepository appleRepo)
        {
            _logger = logger;
            _userManager = userManager;
            _fuRepo = fuRepo;
            _appleRepo = appleRepo;
        }
        
        public IActionResult Index()
        {
            // --- Assemble all info the view will need
            string? id = _userManager.GetUserId(User);

            FujiUser? fu = null;
            if(id != null)
            {
                fu = _fuRepo.GetFujiUserByIdentityId(id);
            }

            // --- Put that info into the view model
            MainPageVM vm = new MainPageVM()
            {
                HasFujiUser = fu != null,
                FirstName   = fu?.FirstName ?? String.Empty,
                LastName    = fu?.LastName ?? String.Empty,
                //AllPossibleApples   = appleList, // if we're OK with re-assigning and losing the empty list to garbage collection
                TotalApplesConsumedByAllUsers = _appleRepo.GetTotalConsumed(_appleRepo.GetAll())
            };
            // add to the empty list that we had to initialize it with to avoid nullable warnings
            vm.AllPossibleApples.AddRange(_appleRepo.GetAll().ToList());

            // --- Hand it off to the view
            return View(vm);
        }

        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
