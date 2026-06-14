using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ExpenseTrackerAPI.DTO;

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
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories() // IEnumerable - list of collection of categories
        {
            var userId = GetUserId();
            var categories = await _context.Categories.Where(c => c.UserId == userId).ToListAsync();

            var response = categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                UserId = c.UserId
            });
            return Ok(response);

        }

        // GET api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {

            var category = await _context.Categories.FindAsync(id);

            if (category == null || category.UserId != GetUserId())
            {
                return NotFound(); 
            }

            var response = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                UserId = category.UserId
            };

            return Ok(response);
        }


        // POST api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryDto input)
        {
            // for post we are setting the correct User Id - but also checks if the real user is in the header.
            var newCategory = new Category
            {
                Name = input.Name,
                UserId = GetUserId()
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync(); // save chanegs to actual DB

            var response = new CategoryResponseDto 
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                UserId = newCategory.UserId
            };

            return CreatedAtAction(nameof(GetCategory), new { id = response.Id}, response);
        }

        // PUT api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto categoryIn)
        {
      
            // Get the actual Category from the DB and find its true ID and see if header check
            // Header is only real thing we use to compare 

            //just create a var from DB and do some check
            var category = await _context.Categories.FindAsync(id);

            if (category == null || category.UserId != GetUserId())
            {
                return NotFound();
            }

            //checks are done lets create/update the category
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