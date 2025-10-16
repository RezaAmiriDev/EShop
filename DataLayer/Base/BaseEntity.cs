

namespace DataLayer.Base
{
    public interface IEntity
    {
    }

    public abstract class BaseEntity<Tkey> : IEntity
    {
        public Tkey? Id { get; set; }
        public DateTime DateOfOperation { get; set; } = DateTime.UtcNow;
    }

    public abstract class BaseEntity : BaseEntity<Guid>
    {
    }
}

