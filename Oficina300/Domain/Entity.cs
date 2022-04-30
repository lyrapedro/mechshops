using Flunt.Notifications;

namespace Oficina300.Domain;

public abstract class Entity : Notifiable<Notification>
{
    public int Id { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
