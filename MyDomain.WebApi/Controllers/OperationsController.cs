using DataAccessLayer;
using DomainTables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public OperationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Operation>>> GetOperations()
    {
        return await _context.Operations.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Operation>> GetOperation(int id)
    {
        var operation = await _context.Operations.FindAsync(id);
        if (operation == null)
        {
            return NotFound();
        }
        return operation;
    }

    [HttpPost]
    public async Task<ActionResult<Operation>> PostOperation(Operation operation)
    {
        _context.Operations.Add(operation);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOperation), new { id = operation.CodeOperation }, operation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutOperation(int id, Operation operation)
    {
        if (id != operation.CodeOperation)
        {
            return BadRequest();
        }

        _context.Entry(operation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Operations.Any(e => e.CodeOperation == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOperation(int id)
    {
        var operation = await _context.Operations.FindAsync(id);
        if (operation == null)
        {
            return NotFound();
        }

        _context.Operations.Remove(operation);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
