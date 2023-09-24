using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PenSword.Models;
using PenSword.Enums;
using PenSword.Services.Interfaces;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PenSword.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailService;
        private readonly IRoleService _roleService;

        public HomeController(ILogger<HomeController> logger,
            UserManager<BlogUser> userManager,
            IUserService userService,
            IConfiguration configuration,
            IEmailSender emailService,
            IRoleService roleService)
        {
            _logger = logger;
            _userManager = userManager;
            _userService = userService;
            _configuration = configuration;
            _emailService = emailService;
            _roleService = roleService;
        }

        // GET: ShowAuthorDetails
        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> ShowAuthorProfile(string? authorId)
        {
            if (string.IsNullOrEmpty(authorId)) return NotFound();

            BlogUser? author = await _userService.GetUserByIdAsync(authorId);
            if (author == null) return NotFound();

            return View(author);
        }

        // GET: EditAuthorProfile/5
        [Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> EditAuthorProfile(string? authorId)
        {
            if (string.IsNullOrEmpty(authorId)) return NotFound();

            BlogUser? author = await _userService.GetUserByIdAsync(authorId);
            if (author == null) return NotFound();

            return View(author);
        }

        // POST: EditAuthorProfile
        [HttpPost,ValidateAntiForgeryToken,Authorize(Policy = nameof(Policies.AdAuth))]
        public async Task<IActionResult> EditAuthorProfile()
        {
            string? authorId = _userManager.GetUserId(User);

            BlogUser? author = await _userService.GetUserByIdAsync(authorId);
            if (author == null || author.Id != _userManager.GetUserId(User)) return NotFound();

            bool validUpdate = await TryUpdateModelAsync(author,
                string.Empty,
                a => a.JobTitle,
                a => a.Company,
                a => a.Byline,
                a => a.Bio,
                a => a.LinkedIn,
                a => a.GitHub,
                a => a.Twitter,
                a => a.Facebook,
                a => a.Instagram,
                a => a.Website);

            if (validUpdate)
            {
                try
                {
                    await _userService.UpdateUser(author);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_userService.UserExists(author.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index", "BlogPosts", new { authorId });
            }
            return View(nameof(EditAuthorProfile), new { authorId });
        }

        // GET: Contact Us
        public async Task<IActionResult> ContactUs(string? swalMessage = null)
        {
            ViewBag.SwalMessage = swalMessage;

            string? blogUserId = _userManager.GetUserId(User);

            BlogUser? model = await _userService.GetUserByIdAsync(blogUserId)
                ?? new BlogUser();

            return View(model);
        }

        // POST: Contact Us
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs([Bind("FirstName,LastName,Email")] BlogUser blogUser, string message)
        {
            string? swalMessage = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    string? adminEmail = _configuration["AdminLoginEmail"] ?? Environment.GetEnvironmentVariable("AdminLoginEmail");
                    await _emailService.SendEmailAsync(adminEmail!, $"Contact Us Message From - {blogUser.FullName}", message!);
                    swalMessage = "Email sent successfully!";
                    return RedirectToAction("Index", "BlogPosts", new { swalMessage });
                }
                catch (Exception)
                {
                    swalMessage = "Error: Unable to send email.";
                    return RedirectToAction(nameof(ContactUs), new { swalMessage });
                }
            }
            swalMessage = "Error: You must fill all fields.";
            return RedirectToAction(nameof(ContactUs), new { swalMessage });
        }

        // GET: Author Application
        public async Task<IActionResult> AuthorApplication (string? swalMessage = null)
        {
            ViewBag.SwalMessage = swalMessage;

            string? blogUserId = _userManager.GetUserId(User);

            BlogUser? model = await _userService.GetUserByIdAsync(blogUserId)
                ?? new BlogUser();

            return View(model);
        }

        // POST: Author Application
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthorApplication([Bind("FirstName,LastName,Email")] BlogUser blogUser, string message)
        {
            string? swalMessage = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    string? adminEmail = _configuration["AdminLoginEmail"] ?? Environment.GetEnvironmentVariable("AdminLoginEmail");
                    await _emailService.SendEmailAsync(adminEmail!, $"Author Application From - {blogUser.FullName}", message!);
                    swalMessage = "Email sent successfully!";
                    return RedirectToAction("Index", "BlogPosts", new { swalMessage });
                }
                catch (Exception)
                {
                    swalMessage = "Error: Unable to send email.";
                    return RedirectToAction(nameof(ContactUs), new { swalMessage });
                }
            }
            swalMessage = "Error: You must fill all fields.";
            return RedirectToAction(nameof(ContactUs), new { swalMessage });
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> ManageUsers(string? incomingMessage = null)
        {
            ViewBag.SwalMessage = incomingMessage;
            IEnumerable<BlogUser> model = await _userService.GetAllUsersAsync();
            return View(model);
        }

        [HttpGet, Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> ManageUserRoles(string? userId)
        {
            BlogUser? currentUser = await _userManager.GetUserAsync(User);
            BlogUser? model = await _userService.GetUserByIdAsync(userId);
            if (model == null) return NotFound();

            if (currentUser == model) return BadRequest();

            IEnumerable<string> currentRoles = await _roleService.GetUserRolesAsync(model.Id);
            ViewBag.Roles = new MultiSelectList(await _roleService.GetRolesAsync(), "Name", "Name", currentRoles);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> ManageUserRolesConfirmed(string userId, string? selected)
        {
            string? swalMessage = string.Empty;
            BlogUser? currentUser = await _userManager.GetUserAsync(User);
            BlogUser? userToEdit = await _userService.GetUserByIdAsync(userId);
            if (currentUser == userToEdit) return BadRequest();

            IEnumerable<string>? currentRoles = await _roleService.GetUserRolesAsync(userToEdit.Id);

            if (string.IsNullOrWhiteSpace(selected)) return BadRequest();

            bool success = await _roleService.RemoveUserFromRolesAsync(userToEdit.Id, currentRoles);
            if (!success)
            {
                swalMessage = "There was a problem removing the user's existing role.";
                return RedirectToAction(nameof(ManageUsers), new { incomingMessage = swalMessage });
            }
            else if (success && selected.Equals("None"))
            {
                swalMessage = "Success: User role removed.";
                return RedirectToAction(nameof(ManageUsers), new { incomingMessage = swalMessage });
            }

            success = await _roleService.AddUserToRoleAsync(userToEdit.Id, selected);
            if (!success)
            {
                swalMessage = "The user's existing role was removed, but there was a problem adding the selected role.";
                return RedirectToAction(nameof(ManageUsers), new { incomingMessage = swalMessage });
            }

            swalMessage = "Success: User role changed.";
            return RedirectToAction(nameof(ManageUsers), new { incomingMessage = swalMessage });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}