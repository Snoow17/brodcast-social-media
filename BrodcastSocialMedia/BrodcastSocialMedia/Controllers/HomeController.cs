using System.Diagnostics;
using System.Threading.Tasks;
using BrodcastSocialMedia.Data;
using BrodcastSocialMedia.Models;
using BrodcastSocialMedia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrodcastSocialMedia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View(new HomeIndexViewModel
                {
                    Broadcasts = new List<BroadcastViewModel>()
                });
            }

            // Get IDs of users the current user is listening to
            var listenedUserIds = await _dbContext.UserListenings
                .Where(ul => ul.ListenerId == user.Id)
                .Select(ul => ul.TargetId)
                .ToListAsync();

            // Also include current user's own ID
            listenedUserIds.Add(user.Id);

            // Fetch broadcasts from listened users + self
            var broadcasts = await _dbContext.Broadcasts
                .Where(b => listenedUserIds.Contains(b.UserId))
                .Include(b => b.User)
                .Include(b => b.Likes)
                .OrderByDescending(b => b.Published)
                .ToListAsync();

            var viewModel = new HomeIndexViewModel
            {
                Broadcasts = broadcasts.Select(b => new BroadcastViewModel
                {
                    Id = b.Id,
                    Message = b.Message,
                    ImageUrl = b.ImageUrl,
                    Published = b.Published,
                    UserName = b.User.Name,
                    ProfileImageUrl = b.User.ProfileImageUrl,
                    LikeCount = b.Likes.Count,
                    IsLikedByCurrentUser = b.Likes.Any(l => l.UserId == user.Id),
                    IsOwnedByCurrentUser = b.UserId == user.Id,
                    UserId = b.UserId
                }).ToList()
            };

            return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> Broadcast(HomeBroadcastViewModel viewModel)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You must be logged in to post a broadcast.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var broadcasts = await _dbContext.UserListenings
                    .Where(ul => ul.ListenerId == user.Id)
                    .Select(ul => ul.Target)
                    .SelectMany(u => u.Broadcasts)
                    .Include(b => b.User)
                    .Include(b => b.Likes)
                    .OrderByDescending(b => b.Published)
                    .ToListAsync();

                var model = new HomeIndexViewModel
                {
                    Broadcasts = broadcasts.Select(b => new BroadcastViewModel
                    {
                        Id = b.Id,
                        Message = b.Message,
                        ImageUrl = b.ImageUrl,
                        Published = b.Published,
                        UserName = b.User.Name,
                        LikeCount = b.Likes.Count,
                        IsLikedByCurrentUser = b.Likes.Any(l => l.UserId == user.Id)
                    }).ToList()
                };

                ViewData["Error"] = "Message cannot be empty.";
                return View("Index", model);
            }

            string? imagePath = null;

            if (viewModel.Image != null && viewModel.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(viewModel.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.Image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + uniqueFileName;
            }

            var broadcast = new Broadcast
            {
                Message = viewModel.Message,
                User = user,
                ImageUrl = imagePath,
                Published = DateTime.UtcNow
            };

            _dbContext.Broadcasts.Add(broadcast);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBroadcast(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var broadcast = await _dbContext.Broadcasts.FindAsync(id);

            if (broadcast == null)
                return NotFound();

            if (broadcast.UserId != user.Id)
                return Forbid(); // Prevent deleting others' posts

            _dbContext.Broadcasts.Remove(broadcast);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike([FromBody] int broadcastId)
        {
            var user = await _userManager.GetUserAsync(User);

            var like = await _dbContext.BroadcastLikes
                .FirstOrDefaultAsync(bl => bl.BroadcastId == broadcastId && bl.UserId == user.Id);

            bool isLiked;

            if (like != null)
            {
                _dbContext.BroadcastLikes.Remove(like);
                isLiked = false;
            }
            else
            {
                _dbContext.BroadcastLikes.Add(new BroadcastLike
                {
                    BroadcastId = broadcastId,
                    UserId = user.Id,
                    LikedAt = DateTime.UtcNow
                });
                isLiked = true;
            }

            await _dbContext.SaveChangesAsync();

            var likeCount = await _dbContext.BroadcastLikes
                .CountAsync(bl => bl.BroadcastId == broadcastId);

            return Json(new { likeCount, isLiked });
        }

        [HttpGet]
        public async Task<IActionResult> GetLikers(int broadcastId)
        {
            var likers = await _dbContext.BroadcastLikes
                .Where(bl => bl.BroadcastId == broadcastId)
                .Include(bl => bl.User)
                .Select(bl => new {
                    name = bl.User.Name,
                    profileImageUrl = string.IsNullOrEmpty(bl.User.ProfileImageUrl)
                        ? "/uploads/default-profile.png"
                        : bl.User.ProfileImageUrl
                })
                .ToListAsync();

            return Json(likers);
        }

        [HttpGet]
        public async Task<IActionResult> TopBroadcasts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "You must register and log in to use this feature.";
                return RedirectToAction("Index");
            }

            var userId = user.Id;
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var topBroadcasts = await _dbContext.Broadcasts
                .Include(b => b.User)
                .Include(b => b.Likes)
                .Where(b => b.Likes.Any(l => l.LikedAt >= sevenDaysAgo))
                .Select(b => new
                {
                    Broadcast = b,
                    LikeCount = b.Likes.Count(l => l.LikedAt >= sevenDaysAgo)
                })
                .OrderByDescending(x => x.LikeCount)
                .Take(10)
                .ToListAsync();

            var viewModel = new HomeIndexViewModel
            {
                Broadcasts = topBroadcasts.Select(x => new BroadcastViewModel
                {
                    Id = x.Broadcast.Id,
                    Message = x.Broadcast.Message,
                    ImageUrl = x.Broadcast.ImageUrl,
                    Published = x.Broadcast.Published,
                    UserName = x.Broadcast.User.Name,
                    LikeCount = x.LikeCount,
                    IsLikedByCurrentUser = x.Broadcast.Likes.Any(l => l.UserId == userId),
                    UserId = x.Broadcast.UserId
                }).ToList()
            };

            return View("TopBroadcasts", viewModel);
        }
    }
}
