using System.Globalization;

namespace MechShops.Endpoints.Shops;

public class ShopHelper
{
	public static bool IsCnpj(string cnpj)
	{
		int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
		int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
		int soma;
		int resto;
		string digito;
		string tempCnpj;
		cnpj = cnpj.Trim();
		cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
		if (cnpj.Length != 14)
			return false;
		tempCnpj = cnpj.Substring(0, 12);
		soma = 0;
		for (int i = 0; i < 12; i++)
			soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
		resto = (soma % 11);
		if (resto < 2)
			resto = 0;
		else
			resto = 11 - resto;
		digito = resto.ToString();
		tempCnpj = tempCnpj + digito;
		soma = 0;
		for (int i = 0; i < 13; i++)
			soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
		resto = (soma % 11);
		if (resto < 2)
			resto = 0;
		else
			resto = 11 - resto;
		digito = digito + resto.ToString();
		return cnpj.EndsWith(digito);
	}

	public static string GetOnlyNumbers(string cnpj)
    {
		cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

		return cnpj;
	}

	public static DateTime? DateParse(string date)
    {
		DateTime validDate;
		CultureInfo provider = new CultureInfo("pt-BR");
		return DateTime.TryParse(date, provider, DateTimeStyles.None, out validDate) ? validDate : null;
	}
}
