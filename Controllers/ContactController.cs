using Microsoft.AspNetCore.Mvc;
using CookBook_Dynamic_Final.Data;
using CookBook_Dynamic_Final.Models;

namespace CookBook_Dynamic_Final.Controllers
{
    public class ContactController : Controller
    {
        private readonly CookBookDbContext _context;

        public ContactController(CookBookDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thank you for contacting us!";
                return RedirectToAction("Index", "Home");
            }
            return View("Index", contact);
        }
    }
}
