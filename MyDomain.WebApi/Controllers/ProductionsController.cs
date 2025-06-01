using DataAccessLayer;
using DomainTables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class ProductionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Production>>> GetProductions()
    {
        return await _context.Productions
            .Include(p => p.Detail)
            .Include(p => p.Operation)
            .ToListAsync();
    }

    [HttpGet("{codeDetail}/{operationNumber}/{codeOperation}")]
    public async Task<ActionResult<Production>> GetProduction(short codeDetail, short operationNumber, short codeOperation)
    {
        var production = await _context.Productions
            .Include(p => p.Detail)
            .Include(p => p.Operation)
            .FirstOrDefaultAsync(p =>
                p.CodeDetail == codeDetail &&
                p.OperationNumber == operationNumber &&
                p.CodeOperation == codeOperation);

        if (production == null)
        {
            return NotFound();
        }

        return production;
    }

    [HttpPost]
    public async Task<ActionResult<Production>> PostProduction(Production production)
    {
        _context.Productions.Add(production);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduction),
            new
            {
                codeDetail = production.CodeDetail,
                operationNumber = production.OperationNumber,
                codeOperation = production.CodeOperation
            },
            production);
    }

    [HttpPut("{codeDetail}/{operationNumber}/{codeOperation}")]
    public async Task<IActionResult> PutProduction(short codeDetail, short operationNumber, short codeOperation, Production production)
    {
        if (codeDetail != production.CodeDetail ||
            operationNumber != production.OperationNumber ||
            codeOperation != production.CodeOperation)
        {
            return BadRequest("Composite key mismatch.");
        }

        _context.Entry(production).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            var exists = await _context.Productions.AnyAsync(p =>
                p.CodeDetail == codeDetail &&
                p.OperationNumber == operationNumber &&
                p.CodeOperation == codeOperation);
            if (!exists)
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{codeDetail}/{operationNumber}/{codeOperation}")]
    public async Task<IActionResult> DeleteProduction(short codeDetail, short operationNumber, short codeOperation)
    {
        var production = await _context.Productions.FirstOrDefaultAsync(p =>
            p.CodeDetail == codeDetail &&
            p.OperationNumber == operationNumber &&
            p.CodeOperation == codeOperation);

        if (production == null)
        {
            return NotFound();
        }

        _context.Productions.Remove(production);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
