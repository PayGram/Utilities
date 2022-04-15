using Telegram.Bot.Types;

namespace Utilities.Telegram
{
	public class TGCallBack
	{
		public const char PARAMS_SEP = '_';
		public string[] Parameters { get; set; }
		public CallbackQuery CallbackQuery { get; private set; }
		public long FromId => CallbackQuery?.From?.Id ?? 0;

		public string FirstParameter => Parameters == null || Parameters.Length == 0 ? null : Parameters[0];
		public bool HasParameters => Parameters != null && Parameters.Length > 0;
		public int ParametersCount => !HasParameters ? 0 : Parameters.Length;

		public string ParameterAt(int idx) => idx >= ParametersCount ? null : Parameters[idx];

		public string GetData()
		{
			return GetData(Parameters);
		}

		public static string GetData(params string[] Parameters)
		{
			if (Parameters == null) return null;

			return string.Join($"{PARAMS_SEP}", Parameters);
		}

		public static TGCallBack Parse(string s)
		{
			if (string.IsNullOrWhiteSpace(s)) return null;
			return new TGCallBack() { Parameters = s.Split(PARAMS_SEP) };
		}

		public static TGCallBack Parse(CallbackQuery q)
		{
			if (q == null) return null;
			string data = q.Data;
			if (string.IsNullOrWhiteSpace(data)) return null;

			var tgcallback = new TGCallBack()
			{
				Parameters = data.Split(PARAMS_SEP),
				CallbackQuery = q
			};

			return tgcallback;
		}
	}
}
