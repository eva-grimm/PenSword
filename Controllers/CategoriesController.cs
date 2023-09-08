using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PenSword.Data;
using PenSword.Models;
using PenSword.Services.Interfaces;

namespace PenSword.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogService _blogService;
        private readonly IImageService _imageService;

        public CategoriesController(ApplicationDbContext context,
            IBlogService blogService,
            IImageService imageService)
        {
            _context = context;
            _blogService = blogService;
            _imageService = imageService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _blogService.GetCategoriesAsync());
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ImageFile")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ImageFile != null)
                {
                    // Convert file to byte array and assign it to image data
                    category.ImageData = await _imageService.ConvertFileToByteArrayAsync(category.ImageFile);
                    // Assign ImageType based on the chosen file
                    category.ImageType = category.ImageFile.ContentType;
                }

                await _blogService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Category? category = await _blogService.GetSingleCategoryAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFile,ImageData,ImageType")] Category category)
        {
            if (id != category.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (category.ImageFile != null)
                {
                    // Convert file to byte array and assign it to image data
                    category.ImageData = await _imageService.ConvertFileToByteArrayAsync(category.ImageFile);
                    // Assign ImageType based on the chosen file
                    category.ImageType = category.ImageFile.ContentType;
                }

                await _blogService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Category? category = await _blogService.GetSingleCategoryAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            Category? category = await _blogService.GetSingleCategoryAsync(id);
            if (category != null)  await _blogService.DeleteCategoryAsync(id);
            
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
