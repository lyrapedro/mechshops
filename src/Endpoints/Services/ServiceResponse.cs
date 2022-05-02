namespace MechShops.Endpoints.Services;

public record ServiceResponse(int Id, string Name, int WorkUnits, DateTime ModifiedAt, DateTime CreatedAt);