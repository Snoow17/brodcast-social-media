using BrodcastSocialMedia.Models;
using BrodcastSocialMedia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BrodcastSocialMedia.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
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
        public async Task<IActionResult> ProfileImage(ProfileIndexViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = "/uploads/" + uniqueFileName;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
