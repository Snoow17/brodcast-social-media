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

        [Route("/Users/Details/{id}")]
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

        [HttpGet]
        [Route("/Users/Explore")]
        public async Task<IActionResult> Explore()
        {
            var currentUser = await _dbContext.Users
                .Include(u => u.ListeningTo)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var listeningToIds = currentUser.ListeningTo.Select(u => u.Id).ToHashSet();

            var users = await _dbContext.Users
                .Include(u => u.Broadcasts)
                .Take(2)
                .OrderByDescending(u => u.Broadcasts.Count)
                .Select(u => new ExploreUserViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    BroadcastCount = u.Broadcasts.Count,
                    ProfileImageUrl = "/images/default-profile.png"
                })
                .ToListAsync();

            var viewModel = new ExploreViewModel
            {
                Users = users
            };

            Console.WriteLine($"Explore returning {users.Count} users");

            return View("Explore", viewModel);
        }





        [HttpGet]
        public async Task<IActionResult> GetUserModal(string id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Broadcasts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UsersShowUserViewModel
            {
                User = user,
                Broadcasts = user.Broadcasts
                    .Where(b => b.Published != null)
                    .OrderByDescending(b => b.Published)
                    .ToList()
            };

            return PartialView("_UserModalPartial", model);
        }


    }
}
