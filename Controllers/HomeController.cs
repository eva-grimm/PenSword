using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;
using PenSword.Models;
using PenSword.Models.ViewModels;
using System.Diagnostics;

namespace PenSword.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailService;

        public HomeController(ILogger<HomeController> logger,
            UserManager<BlogUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            IEmailSender emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Contact Me
        public async Task<IActionResult> ContactMe(string? swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;

            string? blogUserId = _userManager.GetUserId(User);

            // Adjust what shows if user isn't logged in
            if (blogUserId == null) return View(new ContactMeViewModel { });

            BlogUser? blogUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == blogUserId);

            // Adjust what shows if user isn't logged in
            if (blogUser == null) return View(new ContactMeViewModel { });

            EmailData emailData = new()
            {
                EmailAddress = blogUser.Email,
                FirstName = blogUser.FirstName,
                LastName = blogUser.LastName,
            };

            ContactMeViewModel viewModel = new()
            {
                BlogUser = blogUser,
                EmailData = emailData,
            };

            return View(viewModel);
        }

        // POST: Contact Me
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe([Bind("FirstName,LastName,Email")] BlogUser blogUser, string message)
        {
            string? swalMessage = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    string? adminEmail = _configuration["AdminLoginEmail"] ?? Environment.GetEnvironmentVariable("AdminLoginEmail");
                    await _emailService.SendEmailAsync(adminEmail!, $"Contact Me Message From - {blogUser.FullName}", message!);
                    swalMessage = "Email sent successfully!";
                    return RedirectToAction("Index", "BlogPosts", new { swalMessage });
                }
                catch (Exception)
                {
                    swalMessage = "Error: Unable to send email.";
                    return RedirectToAction(nameof(ContactMe), new { swalMessage });
                    throw;
                }
            }
            swalMessage = "Error: You must fill all fields.";
            return RedirectToAction(nameof(ContactMe), new { swalMessage });
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