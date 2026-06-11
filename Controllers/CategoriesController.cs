using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

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
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);

        }

        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(); 
            }
            return Ok(category);
        }


        // POST api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category newCategory)
        {
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync(); // save chanegs to actual DB

            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id}, newCategory);
        }

        // PUT api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id) // ID/ Category mismatch 
            {
                return BadRequest("ID mismatch between URL and body");
            }

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent(); //standard for successful update 204
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}