using DataAccessLayer;
using DomainTables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DetailsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DetailsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Detail>>> GetDetails()
    {
        return await _context.Details.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Detail>> GetDetail(int id)
    {
        var detail = await _context.Details.FindAsync(id);
        if (detail == null)
        {
            return NotFound();
        }
        return detail;
    }

    [HttpPost]
    public async Task<ActionResult<Detail>> PostDetail(Detail detail)
    {
        _context.Details.Add(detail);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDetail), new { id = detail.CodeDetail }, detail);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetail(int id, Detail detail)
    {
        if (id != detail.CodeDetail)
        {
            return BadRequest();
        }
        _context.Entry(detail).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Details.Any(e => e.CodeDetail == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetail(int id)
    {
        var detail = await _context.Details.FindAsync(id);
        if (detail == null)
        {
            return NotFound();
        }
        _context.Details.Remove(detail);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
