using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballManager.Controllers
{
    [Authorize]
    public class DrillController : Controller
    {
        private readonly IDrillService _drillService;

        public DrillController(IDrillService drillService)
        {
            _drillService = drillService;
        }

        // GET /Drill
        public async Task<IActionResult> Index(string? category, string? search)
        {
            var drills = await _drillService.GetAllAsync(category, search);
            ViewBag.Category = category;
            ViewBag.Search   = search;
            return View(drills);
        }

        // GET /Drill/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var drill = await _drillService.GetDetailAsync(id);
            if (drill == null) return NotFound();
            return View(drill);
        }

        // GET /Drill/Create
        [Authorize(Roles = "Admin,Coach")]
        public IActionResult Create()
        {
            return View(new DrillFormViewModel());
        }

        // POST /Drill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Create(DrillFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var id = await _drillService.CreateAsync(model, userId);

            TempData["Success"] = $"Đã tạo bài tập \"{model.Name}\"";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET /Drill/Edit/5
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _drillService.GetFormAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST /Drill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(DrillFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _drillService.UpdateAsync(model);
            TempData["Success"] = "Đã cập nhật bài tập";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // POST /Drill/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _drillService.ToggleActiveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET /Drill/SelectList?category=Physical  — AJAX endpoint dùng trong Training form
        public async Task<IActionResult> SelectList(string? category)
        {
            var list = await _drillService.GetSelectListAsync(category);
            return Json(list);
        }
    }
}
