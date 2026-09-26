using InventoryFlow.Domain.Constants;
using InventoryFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // requires ANY authenticated user (any valid JWT) — applies to every action below by default
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)] // overrides the class-level [Authorize] — only Admin can hit this specific action
    public IActionResult Create()
    {
        // Real implementation comes in Phase 4 (CQRS command) — this just proves the authorization gate works
        return Ok("If you can see this, you're an Admin.");
    }
}