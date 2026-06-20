using FootballManager.Services;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FootballManager.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ─── Quản lý tài khoản (Admin only) ───

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = await _userService.GetCreateFormAsync();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailablePlayers = (await _userService.GetCreateFormAsync()).AvailablePlayers;
                return View(model);
            }

            var (success, error) = await _userService.CreateAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                model.AvailablePlayers = (await _userService.GetCreateFormAsync()).AvailablePlayers;
                return View(model);
            }

            TempData["Success"] = $"Đã tạo tài khoản '{model.Username}'";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _userService.GetEditFormAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailablePlayers = (await _userService.GetEditFormAsync(model.Id))?.AvailablePlayers ?? new();
                return View(model);
            }

            var (success, error) = await _userService.UpdateAsync(model);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                model.AvailablePlayers = (await _userService.GetEditFormAsync(model.Id))?.AvailablePlayers ?? new();
                return View(model);
            }

            TempData["Success"] = "Đã cập nhật tài khoản";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var vm = await _userService.GetEditFormAsync(id);
            if (vm == null) return NotFound();
            return View(new ChangePasswordViewModel { UserId = id, FullName = vm.FullName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, error) = await _userService.ChangePasswordAsync(model.UserId, model.NewPassword);
            if (!success)
            {
                ModelState.AddModelError("", error!);
                return View(model);
            }

            TempData["Success"] = "Đã đổi mật khẩu thành công";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _userService.DeleteAsync(id);
            if (!success) TempData["Error"] = error;
            else TempData["Success"] = "Đã xóa tài khoản";
            return RedirectToAction(nameof(Index));
        }

        // ─── Player Portal (role Player) ───

        [Authorize(Roles = "Player")]
        public async Task<IActionResult> Portal()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var vm = await _userService.GetPortalAsync(userId);
            if (vm == null) return RedirectToAction("Index", "Dashboard");
            return View(vm);
        }
    }
}
