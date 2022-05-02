namespace Oficina300.Endpoints.Shops;

public record ShopRequest(string Cnpj, string Password, int WorkLoad);
public record ShopRequestPut(int WorkLoad);