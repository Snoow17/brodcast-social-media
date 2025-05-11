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
                    Broadcasts = new List<Broadcast>()
                });
            }

            var dbUser = await _dbContext.Users.Where(u => u.Id == user.Id).FirstOrDefaultAsync();

            var broadcasts = await _dbContext.Users.Where(u => u.Id == user.Id)
                .SelectMany(u => u.ListeningTo)
                .SelectMany(u => u.Broadcasts)
                .Include(b => b.User)
                .OrderByDescending(b => b.Published)
                .ToListAsync();

            var viewModel = new HomeIndexViewModel()
            {
                Broadcasts = broadcasts
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
            var user = await _userManager.GetUserAsync(User);

            string? imagePath = null;

            if (viewModel.Image != null && viewModel.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

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


    }
}
