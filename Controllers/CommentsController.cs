using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PenSword.Models;
using PenSword.Enums;
using PenSword.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PenSword.Controllers
{
    public class CommentsController : Controller
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly ICommentService _commentService;
        private readonly IRoleService _roleService;

        public CommentsController(UserManager<BlogUser> userManager,
            ICommentService commentService,
            IRoleService roleService)
        {
            _userManager = userManager;
            _commentService = commentService;
            _roleService = roleService;
        }

        // POST: Comments/Create
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Body,BlogPostId")] Comment comment, string? slug)
        {
            ModelState.Remove("AuthorId");

            if (!ModelState.IsValid)
            {
                if (slug != null) return RedirectToAction("Details", "BlogPosts", new { slug, swalMessage = "Error: Cannot make a blank comment." });
                else return RedirectToAction("Index", "BlogPosts", new { swalMessage = "Error: Cannot make a blank comment." });
            }
            else
            {
                comment.AuthorId = _userManager.GetUserId(User);
                comment.Created = DateTime.Now;

                // remove excess space around comment due to editor
                comment.Body = Regex.Replace(comment.Body!, @"<[^>]*>", string.Empty);

                bool success = await _commentService.AddCommentAsync(comment);
                if (!success) return BadRequest();
                if (slug != null) return RedirectToAction("Details", "BlogPosts", new { slug, swalMessage = "Error: Cannot make a blank comment." });
                else return RedirectToAction("Index", "BlogPosts", new { swalMessage = "Error: Cannot make a blank comment." });
            }
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Comment? comment = await _commentService.GetCommentAsync(id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            Comment? comment = await _commentService.GetCommentAsync(id);
            if (comment == null) return NotFound();
            string? currentUserId = _userManager.GetUserId(User);

            // only allow edits if an admin, a moderator, or the comment's author
            bool allowedToEdit = (await _roleService.IsUserInRoleAsync(currentUserId, nameof(Roles.Admin))
                || await _roleService.IsUserInRoleAsync(currentUserId, nameof(Roles.Moderator))
                || comment.AuthorId == currentUserId);
            if (!allowedToEdit) return Unauthorized();

            bool validUpdate = await TryUpdateModelAsync(
                comment,
                string.Empty,
                c => c.Body,
                c => c.UpdateReason);

            if (!validUpdate) return View(comment);

            try
            {
                comment.Updated = DateTime.Now;
                bool success = await _commentService.UpdateCommentAsync(comment);
                if (!success) return BadRequest();

                string? blogPostSlug = comment.BlogPost!.Slug;
                if (blogPostSlug != null)
                    return RedirectToAction("Details", "BlogPosts", new { slug = blogPostSlug });
                else
                    return RedirectToAction("Index", "BlogPosts");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // GET: Comments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Comment? comment = await _commentService.GetCommentAsync(id);
            if (comment == null) return NotFound();
            string? currentUserId = _userManager.GetUserId(User);

            // only allow delete if an admin, a moderator, or the comment's author
            bool allowedToDelete = (await _roleService.IsUserInRoleAsync(currentUserId, nameof(Roles.Admin))
                || await _roleService.IsUserInRoleAsync(currentUserId, nameof(Roles.Moderator))
                || comment.AuthorId == currentUserId);
            if (!allowedToDelete) return Unauthorized();

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete"),ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Comment? comment = await _commentService.GetCommentAsync(id);
            if (comment == null) return NotFound();
            string? currentUserId = _userManager.GetUserId(User);

            // only allow delete if an admin, a moderator, or the comment's author
            bool allowedToDelete = (await _roleService.IsUserInRoleAsync(currentUserId, nameof(Roles.Admin))
                || await _roleService.IsUserInRoleAsync(currentUserId, nameof(Roles.Moderator))
                || comment.AuthorId == currentUserId);
            if (!allowedToDelete) return Unauthorized();

            try
            {
                bool success = await _commentService.DeleteCommentAsync(comment);
                if (!success) return BadRequest();

                string? blogPostSlug = comment.BlogPost!.Slug;
                if (blogPostSlug != null)
                    return RedirectToAction("Details", "BlogPosts", new { slug = blogPostSlug });
                else
                    return RedirectToAction("Index", "BlogPosts");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
