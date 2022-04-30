namespace Oficina300.Endpoints.Shops;

public record ShopResponse(int Id, string Name, int WorkLoad, DateTime ModifiedAt, DateTime CreatedAt);