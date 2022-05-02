namespace MechShops.Endpoints.Demands;

public record DemandRequest(int Id, DateTime Date, int ShopId);