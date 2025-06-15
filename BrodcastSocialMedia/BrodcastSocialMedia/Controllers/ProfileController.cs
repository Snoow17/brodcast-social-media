using BrodcastSocialMedia.Models;
using BrodcastSocialMedia.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrodcastSocialMedia.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _env = env;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var viewModel = new ProfileIndexViewModel()
            {
                Name = user.Name ?? "",
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProfileIndexViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);

            var usersWithSameName = _userManager.Users
                .Where(u => u.Name == viewModel.Name && u.Id != user.Id)
                .ToList();

            if (usersWithSameName.Any())
            {
                ModelState.AddModelError("Name", "That username is already taken.");
                return View("Index", viewModel);
            }

            user.Name = viewModel.Name;

            await _userManager.UpdateAsync(user);

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult ProfileImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileImage(ProfileIndexViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = "/uploads/" + uniqueFileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }
    }
}
