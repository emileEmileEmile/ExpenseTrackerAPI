using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public CategoriesController(AppDbContext context) //Constructor. Program.cs passes the DB context 
        {
            _context = context;
        }

        // GET api/categories 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories() // IEnumerable - list of collection of categories
        {
            var userId = GetUserId();
            var categories = await _context.Categories.Where(c => c.UserId == userId).ToListAsync();
            return Ok(categories);

        }

        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {

            var category = await _context.Categories.FindAsync(id);

            if (category == null || category.UserId != GetUserId())
            {
                return NotFound(); 
            }
            return Ok(category);
        }


        // POST api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category newCategory)
        {
            // for post we are setting the correct User Id - but also checks if the real user is in the header.
            newCategory.UserId = GetUserId();
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync(); // save chanegs to actual DB

            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id}, newCategory);
        }

        // PUT api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category categoryIn)
        {
            if (id != categoryIn.Id) // ID/ Category mismatch 
            {
                return BadRequest("ID mismatch between URL and body");
            }

            // Get the actual Category from the DB and find its true ID and see if header check
            // Header is only real thing we use to compare 
            var category = await _context.Categories.FindAsync(id);

            if (category == null || category.UserId != GetUserId())
            {
                return NotFound();
            }

            //ok because that is all their is to update - Name field 
            category.Name = categoryIn.Name;

            await _context.SaveChangesAsync();
            return NoContent(); //standard for successful update 204
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category == null || category.UserId != GetUserId())
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
         
    }

}