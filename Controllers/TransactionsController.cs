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
    public class TransactionsController : ControllerBase 
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }



        // api/transactions 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var userId = GetUserId();

            var transactions = await _context.Transactions.Where(u => u.UserId == userId).ToListAsync();
            return Ok(transactions);
        }


        // api/transactions/5
        [HttpGet("{id}")] // get a singular Transaction from the plural transactions table
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null || transaction.UserId != GetUserId())
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            //if (transaction.CategoryId != )
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == transaction.CategoryId && c.UserId == GetUserId());

            if (category == null)
            {
                return BadRequest("Invalid Category");
            }

            transaction.UserId = GetUserId();
            _context.Transactions.Add(transaction); 

            await _context.SaveChangesAsync(); //Insert into DB and auto assign ID

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction); // Return Route to caller
        }


        // api/transactions/5
       [HttpPut("{id}")]
       public async Task<IActionResult> UpdateTransaction(int id, Transaction transactionIn)
       {
            // silly check that front end guy doesnt send wrong id
            if (id != transactionIn.Id)
            {
                return BadRequest();
            }

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null || transaction.UserId != GetUserId())
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == transactionIn.CategoryId && c.UserId == GetUserId());

            if (category == null)
            {
                return BadRequest("Invalid category");
            }

            // We fetch the actual record from the DB and use the parsed body info to update it 
            transaction.Amount = transactionIn.Amount;
            transaction.TransactionType = transactionIn.TransactionType;
            transaction.Date = transactionIn.Date;
            transaction.CategoryId = transactionIn.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
       }

       [HttpDelete("{id}")]
       public async Task<IActionResult> DeleteTransaction(int id)
       {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null || transaction.UserId != GetUserId())
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
       }

       private int GetUserId()
       {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
       }







        

    }


}