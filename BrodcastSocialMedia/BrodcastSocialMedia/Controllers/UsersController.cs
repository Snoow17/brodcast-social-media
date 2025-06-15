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

        public UsersController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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

        public async Task<IActionResult> ShowUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var broadcasts = await _dbContext.Broadcasts
                .Where(b => b.UserId == id)
                .Include(b => b.Likes)
                .OrderByDescending(b => b.Published)
                .Select(b => new BroadcastViewModel
                {
                    Id = b.Id,
                    Message = b.Message,
                    ImageUrl = b.ImageUrl,  // <-- Include image here
                    Published = b.Published,
                    LikeCount = b.Likes.Count,
                    UserId = b.UserId,
                })
                .ToListAsync();

            var currentUserId = _userManager.GetUserId(User);
            var isListening = await _dbContext.UserListenings
                .AnyAsync(l => l.ListenerId == currentUserId && l.TargetId == id);

            var viewModel = new UsersShowUserViewModel
            {
                User = user,
                Broadcasts = broadcasts,
                IsListening = isListening,
                CurrentUserId = currentUserId
            };

            return View(viewModel);
        }

        [HttpGet("/Users/Explore")]
        public async Task<IActionResult> Explore()
        {
            var user = await _userManager.GetUserAsync(User);  // <-- get user here
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must register and log in to use this feature.";
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = user.Id;

            var listeningToIds = await _dbContext.UserListenings
               .Where(l => l.ListenerId == currentUserId)
               .Select(l => l.TargetId)
               .ToListAsync();

            listeningToIds ??= new List<string>();

            var users = await _dbContext.Users
                .Include(u => u.Broadcasts)
                .Where(u => u.Id != currentUserId && !listeningToIds.Contains(u.Id))
                .OrderByDescending(u => u.Broadcasts.Count)
                .Take(10)
                .ToListAsync();

            var viewModel = new ExploreViewModel
            {
                Users = users.Select(u => new ExploreUserViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    BroadcastCount = u.Broadcasts.Count,
                    ProfileImageUrl = u.ProfileImageUrl ?? "/images/default-profile.png",
                    IsListening = false
                }).ToList()
            };

            return View("Explore", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleListen([FromBody] UsersListenToUserViewModel viewModel)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(currentUserId) || currentUserId == viewModel.UserId)
                return BadRequest("Invalid user.");

            var listening = await _dbContext.UserListenings
                .FirstOrDefaultAsync(l => l.ListenerId == currentUserId && l.TargetId == viewModel.UserId);

            bool isListening;

            if (listening != null)
            {
                _dbContext.UserListenings.Remove(listening);
                isListening = false;
            }
            else
            {
                _dbContext.UserListenings.Add(new UserListening
                {
                    ListenerId = currentUserId,
                    TargetId = viewModel.UserId
                });
                isListening = true;
            }

            await _dbContext.SaveChangesAsync();

            return Json(new { isListening });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserModal(string id)
        {
            var user = await _dbContext.Users
                .Include(u => u.Broadcasts)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var isListening = await _dbContext.UserListenings
                .AnyAsync(l => l.ListenerId == currentUserId && l.TargetId == id);

            var model = new UsersShowUserViewModel
            {
                User = user,
                Broadcasts = user.Broadcasts
                    .OrderByDescending(b => b.Published)
                    .Select(b => new BroadcastViewModel
                    {
                        Id = b.Id,
                        Message = b.Message,
                        ImageUrl = b.ImageUrl,
                        Published = b.Published
                    })
                    .ToList(),
                CurrentUserId = currentUserId,
                IsListening = isListening
            };

            return PartialView("_UserModalPartial", model);
        }
    }
}
