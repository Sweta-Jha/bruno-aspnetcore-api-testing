using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Api.Models;

/// <summary>A product in the catalog.</summary>
public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, 10000)]
    public int Stock { get; set; }
}
