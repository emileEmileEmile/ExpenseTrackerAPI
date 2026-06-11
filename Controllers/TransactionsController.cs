using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerAPI.Data;
using ExpenseTrackerAPI.Models;

namespace ExpenseTrackerAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class TransactionsController : ControllerBase 
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }


        // api/transactions/5
        [HttpGet("{id}")] // get a singular Transaction from the plural transactions table
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction); 

            await _context.SaveChangesAsync(); //Insert into DB and auto assign ID

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction); // Return Route to caller
        }


        // api/transactions/5
       [HttpPut("{id}")]
       public async Task<IActionResult> UpdateTransaction(int id, Transaction transaction)
       {

            if (id != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
       }

       [HttpDelete("{id}")]
       public async Task<IActionResult> DeleteTransaction(int id)
       {

            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();

       }







        

    }


}