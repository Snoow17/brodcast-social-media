using BrodcastSocialMedia.Data;
using BrodcastSocialMedia.Models;
using BrodcastSocialMedia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrodcastSocialMedia.Controllers
{
    public class UsersController : Controller
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public UsersController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index(UsersIndexViewModel viewModel)
        {
            if(viewModel.Search != null)
            {
                var users = await _dbContext.Users.Where(u => u.Name.Contains(viewModel.Search))
                                .ToListAsync();
                viewModel.Result = users;
            }
            


            return View(viewModel);
        }

        [Route("/Users/{id}")]
        public async Task<IActionResult> ShowUser(string id)
        {
            var broadcasts = await _dbContext.Broadcasts.Where(b => b.User.Id == id)
                .OrderByDescending(b => b.Published)
                .ToListAsync();
            var user = await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            var viewModel = new UsersShowUserViewModel()
            {
                Broadcasts = broadcasts,
                User = user
            };

            return View(viewModel);
        }

        [HttpPost, Route("/Users/Listen")]
        public async Task<IActionResult> ListenToUser(UsersListenToUserViewModel viewModel)
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            var userToListenTo = await _dbContext.Users.Where(u => u.Id == viewModel.UserId)
                .FirstOrDefaultAsync();

            loggedInUser.ListeningTo.Add(userToListenTo);

            await _userManager.UpdateAsync(loggedInUser);
            await _dbContext.SaveChangesAsync();

            return Redirect("/");
        }

        [HttpPost, Route("/Users/Unlisten")]
        public async Task<IActionResult> UnlistenToUser(UsersListenToUserViewModel viewModel)
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            var userToUnlisten = await _dbContext.Users
                .Where(u => u.Id == viewModel.UserId)
                .FirstOrDefaultAsync();

            if (loggedInUser.ListeningTo.Contains(userToUnlisten))
            {
                loggedInUser.ListeningTo.Remove(userToUnlisten);
                await _userManager.UpdateAsync(loggedInUser);
                await _dbContext.SaveChangesAsync();
            }

            return Redirect("/");
        }

        
    }
}
