using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Fuji.Models;
using Fuji.DAL.Abstract;

namespace Fuji.Controllers
{
    public class ApplesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFujiUserRepository _fuRepo;
        private readonly IAppleRepository _appleRepo;

        public ApplesController(UserManager<IdentityUser> userManager, IFujiUserRepository fuRepo, IAppleRepository appleRepo)
        {
            _userManager = userManager;
            _fuRepo = fuRepo;
            _appleRepo = appleRepo;
        }

        // AJAX endpoint for eating an apple
        [HttpPost]
        [Authorize]
        // Note: can be forged as we don't have an anti forgery token.  Will need to add this, but how with
        // an ajax method?
        public async Task<JsonResult> Ate(int? id)
        {
            // verify id is actually a real apple
            if (id == null)
            {
                return Json(new { success = false, message = "id expected" });
            }
            if(!await _appleRepo.ExistsAsync((int)id))
            {
                return Json(new { success = false, message = "appleID not found" });
            }

            // and that we have a logged in user
            string? aspNetUserID = _userManager.GetUserId(User);
            if(aspNetUserID == null)
            {
                return Json(new { success = false, message = "user not logged in" });
            }
            FujiUser? fu = _fuRepo.GetFujiUserByIdentityId(aspNetUserID);
            if (fu == null)
            {
                return Json(new { success = false, message = "user not found" });
            }

            // Now we have a verified Apple and a verified User.  Let that user eat that apple!
            await _fuRepo.EatAsync(fu!, (int)id, DateTime.UtcNow);
            return Json(new { success = true, message = "user ate apple" });
        }

        // AJAX endpoint
        // GET: Apples eaten by this user; could also filter by apples eaten today or yesterday, etc.
        [Authorize]
        public JsonResult Eaten()
        {
            // Find the current user
            string? aspNetUserID = _userManager.GetUserId(User);
            if (aspNetUserID == null)
            {
                return Json(new { success = false, message = "user not logged in" });
            }
            FujiUser? fu = _fuRepo.GetFujiUserByIdentityId(aspNetUserID);
            if (fu == null)
            {
                return Json(new { success = false, message = "user not found" });
            }

            Dictionary<Apple, int> values = _fuRepo.GetCountOfSpecificApplesEaten(_appleRepo.GetAll(),fu);
            //Total and VarietyName
            var apples = values.Select(v => new { VarietyName = v.Key.VarietyName, Total = v.Value });
            return Json(new { success = true, message = "ok", apples = apples });
        }

    }
}
