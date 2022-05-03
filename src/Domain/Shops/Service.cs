using Flunt.Validations;

namespace MechShops.Domain.Shops;

public class Service : Entity
{
    public string ShopId { get; set; }
    public string Name { get; set; }
    public int WorkUnits { get; set; }

    public Service()
    { }

    public Service(string name, int workUnits, string shopId)
    {
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
