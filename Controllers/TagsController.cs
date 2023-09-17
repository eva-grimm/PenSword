using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PenSword.Models;
using PenSword.Services.Interfaces;

namespace PenSword.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            return View(await _tagService.GetAllTagsAsync());
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Tag? tag = await _tagService.GetSingleTagAsync(id);
            if (tag == null) return NotFound();

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Tag? tag = await _tagService.GetSingleTagAsync(id);
            if (tag == null) return NotFound();

            bool success = await _tagService.DeleteTagAsync(tag);
            if (!success) return BadRequest();

            return RedirectToAction(nameof(Index));
        } 
    }
}