namespace Oficina300.Endpoints.Employees;

public record EmployeeRequest(string Cpf, string Password, string Name, string ShopId);
