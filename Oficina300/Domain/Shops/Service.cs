using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Service : Entity
{
    public int ShopId { get; set; }
    public Shop Shop { get; set; }
    public string Name { get; set; }
    public int WorkUnits { get; set; }


    public Service(string name, int workUnits, int shopId, string createdBy)
    {
        Guid validGuid;
        ShopId = shopId;
        Name = name;
        WorkUnits = workUnits;
        ModifiedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        ModifiedBy = Guid.TryParse(createdBy, out validGuid) ? validGuid : Guid.Empty;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Service>()
            .IsNotNullOrEmpty(Name, "Name");
        AddNotifications(contract);
    }

    public void EditInfo(string name, int workUnits, string modifiedBy)
    {
        Guid validGuid;
        Name = name;
        WorkUnits = workUnits;
        ModifiedBy = Guid.TryParse(modifiedBy, out validGuid) ? validGuid : Guid.Empty;
        ModifiedAt = DateTime.UtcNow;

        Validate();
    }
}
