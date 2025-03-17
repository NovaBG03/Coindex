using Coindex.Core.Domain.Interfaces;

namespace Coindex.Core.Domain.Entities;

public abstract class BaseEntity : ITimestampEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
