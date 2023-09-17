using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PenSword.Models;
using PenSword.Services.Interfaces;

namespace PenSword.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IImageService _imageService;
        private readonly ICategoryService _categoryService;

        public CategoriesController(IBlogService blogService,
            IImageService imageService,
            ICategoryService categoryService)
        {
            _blogService = blogService;
            _imageService = imageService;
            _categoryService = categoryService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _categoryService.GetCategoriesAsync());
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost,ValidateAntiForgeryToken]
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

                await _categoryService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            Category? category = await _categoryService.GetSingleCategoryAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost,ValidateAntiForgeryToken]
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

                await _categoryService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            Category? category = await _categoryService.GetSingleCategoryAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // Categories/DeleteConfirmed/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Category? category = await _categoryService.GetSingleCategoryAsync(id);
            if (category != null)  await _categoryService.DeleteCategoryAsync(category);
            
            return RedirectToAction(nameof(Index));
        }
    }
}