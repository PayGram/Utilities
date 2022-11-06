using CurrenciesLib;

namespace Utilities.Decimal.Extensions
{
	public static class DecimalExtensions
	{
		public static string ToString(this decimal amount, Currencies curr, bool appendCurrency=true)
		{
			const string formatPhpLikeFiat = "#,##0.##"; // decimals are very little, almost non significant
			const string formatUsdLikeFiat = "#,##0.00";
			const string formatVndLikeFiat = "#,##0";
			const string formatBtcLikeCrypto = "#,##0.##########";
			string format;
			switch(curr)
			{
				case Currencies.PHP:
				case Currencies.PHPT:
					format = formatPhpLikeFiat;
					break;

				case Currencies.GBP:
				case Currencies.AED:
				case Currencies.CNY:
				case Currencies.EUR:
				case Currencies.USD:
				case Currencies.USDT:
					format = formatUsdLikeFiat;
					break;

				case Currencies.BTC:
				case Currencies.ETH:
				case Currencies.LTC:
				case Currencies.XMR:
				case Currencies.XRP:
					format = formatBtcLikeCrypto;
					break;

				case Currencies.VND:
					format = formatVndLikeFiat;
					break;

				default:
					format = formatUsdLikeFiat;
					break;
			}
			var amountStr = amount.ToString(format);
			if (appendCurrency)
				amountStr += " " + curr.ToString();
			return amountStr;
		}
	}
}
