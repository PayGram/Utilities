using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Utilities.String.Json.Extentions
{
	/// <summary>
	/// The Newtonsoft.Json settings profiles of the platform, named after what they produce. Newtonsoft.Json is the only
	/// json serializer used: Telegram.Bot depends on it and every live contract and stored format is Newtonsoft-shaped.
	/// Each property returns a new instance, so a caller can customize it without affecting the others.
	/// Plain JsonConvert calls without settings (logs, Newtonsoft defaults) are intentionally left as they are.
	/// Other profiles that are not here on purpose:
	/// - the mvc settings: PayGram.Web uses the AddNewtonsoftJson defaults (camelCase), PayGramTgSrv configures them in
	///   Startup (ReferenceLoopHandling.Ignore + FlexibleEnumConverter: integer enums out, names or numbers in);
	/// - PayGram.Public, the public sdk, keeps its own copy of <see cref="Storage"/> (PublicJson) to stay dependency free.
	/// The bytes each profile produces are pinned by the golden tests (PayGram.Tests/Golden, TelegramBotServer.Tests/Golden).
	/// </summary>
	public static class JsonDefaults
	{
		/// <summary>
		/// Stored json (PgRequest request/response, DBNotification.Content, DbWithdrawInvoice.ProviderData, the requests
		/// PayGram.Client sends): PascalCase, default values left out when writing and populated when reading.
		/// Written indented by the ToJson methods.
		/// </summary>
		public static JsonSerializerSettings Storage => new()
		{
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};

		/// <summary>
		/// <see cref="Storage"/> without nulls, written on one line: what has to be short, like the Telegram callback_data
		/// (64 bytes) and the Question columns.
		/// </summary>
		public static JsonSerializerSettings StorageCompact => new()
		{
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
		};

		/// <summary>
		/// The body of the callbacks pushed to merchants and users: camelCase names, everything written (nulls and defaults
		/// included). The X-PayGram-Signature hmac covers exactly these bytes.
		/// </summary>
		public static JsonSerializerSettings CallbackWire => new()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		/// <summary>
		/// Requests to and responses from third-party providers expecting camelCase names (CryptoDepositsApi).
		/// </summary>
		public static JsonSerializerSettings ProviderCamel => new()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};
	}
}
