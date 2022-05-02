namespace MechShops.Endpoints.Demands;

public record DemandResponse(int Id, int ShopId, Guid CreatedBy, DateTime ModifiedAt, DateTime CreatedAt);