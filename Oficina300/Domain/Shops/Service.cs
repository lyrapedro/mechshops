using Flunt.Validations;

namespace Oficina300.Domain.Shops;

public class Service : Entity
{
    public string ShopId { get; set; }
    public string Name { get; set; }
    public int WorkUnits { get; set; }

    public Service()
    {
        Validate();
    }

    public Service(string name, int workUnits, string shopId)
    {
        Guid validGuid;
        ShopId = shopId;
        Name = name;
        WorkUnits = workUnits;
        ModifiedAt = DateTime.Now;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Service>()
            .IsNotNullOrEmpty(Name, "Name");
        AddNotifications(contract);
    }

    public void EditInfo(string name, int workUnits)
    {
        Name = name;
        WorkUnits = workUnits;
        ModifiedAt = DateTime.Now;

        Validate();
    }
}
