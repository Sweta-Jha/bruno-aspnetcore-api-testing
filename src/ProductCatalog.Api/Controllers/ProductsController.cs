using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Models;

namespace ProductCatalog.Api.Controllers;

/// <summary>Create, read, update and delete products.</summary>
/// <remarks>Products are kept in memory to keep the demo simple, so they reset when the API restarts.</remarks>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products =
    [
        new() { Id = 1, Name = "Mechanical Keyboard", Price = 89.00m, Stock = 25 },
        new() { Id = 2, Name = "27-inch Monitor", Price = 329.00m, Stock = 10 },
        new() { Id = 3, Name = "USB-C Dock", Price = 149.00m, Stock = 40 }
    ];

    private static readonly Lock _lock = new();

    /// <summary>Gets all products.</summary>
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        lock (_lock)
        {
            return Ok(_products.ToList());
        }
    }

    /// <summary>Gets one product by id.</summary>
    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product is null ? NotFound() : Ok(product);
        }
    }

    /// <summary>Creates a new product.</summary>
    [HttpPost]
    public ActionResult<Product> Create(Product product)
    {
        lock (_lock)
        {
            product.Id = _products.Count == 0 ? 1 : _products.Max(p => p.Id) + 1;
            _products.Add(product);
        }

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Updates an existing product.</summary>
    [HttpPut("{id:int}")]
    public ActionResult<Product> Update(int id, Product product)
    {
        lock (_lock)
        {
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing is null)
            {
                return NotFound();
            }

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            return Ok(existing);
        }
    }

    /// <summary>Deletes a product.</summary>
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        lock (_lock)
        {
            var removed = _products.RemoveAll(p => p.Id == id);
            return removed == 0 ? NotFound() : NoContent();
        }
    }
}
