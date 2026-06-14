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
    public class TransactionsController : ControllerBase 
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }



        // api/transactions 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetTransactions()
        {
            var userId = GetUserId();

            var transactions = await _context.Transactions.Where(u => u.UserId == userId).ToListAsync();

            var response = transactions.Select(t => new TransactionResponseDto 
            {   
                Id = t.Id,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Date = t.Date,
                UserId = t.UserId,
                CategoryId = t.CategoryId
            });
            return Ok(response);
        }


        // api/transactions/5
        [HttpGet("{id}")] // get a singular Transaction from the plural transactions table
        public async Task<ActionResult<TransactionResponseDto>> GetTransaction(int id)
        {

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null || transaction.UserId != GetUserId())
            {
                return NotFound();
            }

            var response = new TransactionResponseDto 
            {   
                Id = transaction.Id,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                Date = transaction.Date,
                UserId = transaction.UserId,
                CategoryId = transaction.CategoryId
            };

            return Ok(response);
        }

        // api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> CreateTransaction(TransactionDto input)
        {
            
            //
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == input.CategoryId && c.UserId == GetUserId());

            if (category == null)
            {
                return BadRequest("Invalid Category");
            }

            var transaction = new Transaction 
            {
                Amount = input.Amount,
                TransactionType = input.TransactionType,
                Date = input.Date,
                CategoryId = input.CategoryId,
                UserId = GetUserId()
            };
            
            _context.Transactions.Add(transaction); 

            await _context.SaveChangesAsync(); //Insert into DB and auto assign ID

            var response = new TransactionResponseDto 
            {   
                Id = transaction.Id,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                Date = transaction.Date,
                UserId = transaction.UserId,
                CategoryId = transaction.CategoryId
            };

        
            return CreatedAtAction(nameof(GetTransaction), new { id = response.Id }, response); // Return Route to caller
        }


        // api/transactions/5
       [HttpPut("{id}")]
       public async Task<IActionResult> UpdateTransaction(int id, TransactionDto transactionIn)
       {

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