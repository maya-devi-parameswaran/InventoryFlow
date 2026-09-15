using InventoryFlow.Domain.Common;
using InventoryFlow.Domain.Exceptions;

namespace InventoryFlow.Domain.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Location { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private Warehouse() { }

    public static Warehouse Create(string name, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Warehouse name is required.");
        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Warehouse location is required.");

        return new Warehouse { Name = name.Trim(), Location = location.Trim() };
    }
}