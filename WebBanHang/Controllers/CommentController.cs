using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.DataAccess;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Create(int productId, string content, int? parentCommentId, int rating)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            var comment = new Comment
            {
                ProductId = productId,
                UserId = _userManager.GetUserId(User),
                UserName = _userManager.GetUserName(User),
                Content = content,
                Rating = rating,
                CommentDate = DateTime.UtcNow,
                ParentCommentId = parentCommentId,
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Display", "Product", new {id = productId});
        }
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            if(comment.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Display", "Product", new { id = comment.ProductId });
        }
        public async Task<double> CalculateAverageRating (int productId)
        {
            var ratings = await _context.Comments
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToListAsync();
            double averageRating = ratings.Any() ? ratings.Average() : 0;

            return averageRating;
        }
    }
}
