using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.DataAccess;
using WebBanHang.Models;
using WebBanHang.Services;
using WebBanHang.Utilitys;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleService _roleService;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleService roleService)
        {
            _context = context;
            _userManager = userManager;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> EditRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _roleService.GetUserRolesAsync(user);
            var allRoles = await _roleService.GetAllRolesAsync();

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _roleService.GetUserRolesAsync(user);

            var rolesToAdd = model.SelectedRoles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(model.SelectedRoles);

            var addResult = await _roleService.AddRolesToUserAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể thêm vai trò cho người dùng.");
                return View(model);
            }

            var removeResult = await _roleService.RemoveRolesFromUserAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa vai trò của người dùng.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
