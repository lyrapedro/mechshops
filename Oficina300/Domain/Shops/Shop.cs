using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Shop : Entity
{
    public string Name { get; private set; }
    public int WorkLoad { get; private set; }

    public Shop(string name, int workLoad)
    {
        Name = name;
        WorkLoad = workLoad;
        ModifiedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Shop>()
            .IsNotNullOrEmpty(Name, "Name");
        AddNotifications(contract);
    }

    public void EditInfo(string name, int workLoad, string modifiedBy)
    {
        Guid validGuid;
        Name = name;
        WorkLoad = workLoad;
        ModifiedBy = Guid.TryParse(modifiedBy, out validGuid) ? validGuid : Guid.Empty;
        ModifiedAt = DateTime.UtcNow;

        Validate();
    }
}
