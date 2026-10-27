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
	/// - the mvc settings: PayGram.Web uses the AddNewtonsoftJson defaults (camelCase) plus <see cref="WithUtcDates"/>,
	///   PayGramTgSrv configures them in Startup (ReferenceLoopHandling.Ignore + FlexibleEnumConverter: integer enums out,
	///   names or numbers in, plus <see cref="WithUtcDates"/>);
	/// - PayGram.Public, the public sdk, keeps its own copy of <see cref="Storage"/> (PublicJson) to stay dependency free.
	/// The bytes each profile produces are pinned by the golden tests (PayGram.Tests/Golden, TelegramBotServer.Tests/Golden).
	/// </summary>
	public static class JsonDefaults
	{
		/// <summary>
		/// The dates of every json the platform answers with (the api of PayGram.Web, the api of PayGramTgSrv, the callbacks):
		/// a DateTime is written in UTC with "Z". The database columns hold UTC (the code writes DateTime.UtcNow and
		/// GETUTCDATE() only) and EF reads them back with Kind Unspecified: such a value is taken as UTC as it is (same clock,
		/// "Z" added), a Local one is converted to UTC, a Utc one is written as before.
		/// Reading with these settings, a date without zone (the answers stored before this rule, replayed by the unique key)
		/// is taken as UTC, never shifted; a date with an offset is converted to UTC. DateTimeOffset values keep their offset.
		/// The stored formats (<see cref="Storage"/>, <see cref="StorageCompact"/>) are not changed: their bytes are what is
		/// in the database and in the Telegram buttons, and their readers take both forms.
		/// </summary>
		public static JsonSerializerSettings WithUtcDates(this JsonSerializerSettings settings)
		{
			settings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
			return settings;
		}

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
		/// included), dates in UTC with "Z" (<see cref="WithUtcDates"/>: the callbacks always carried UTC dates, the bytes are
		/// the same). The X-PayGram-Signature hmac covers exactly these bytes.
		/// </summary>
		public static JsonSerializerSettings CallbackWire => new JsonSerializerSettings()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		}.WithUtcDates();

		/// <summary>
		/// Requests to and responses from third-party providers expecting camelCase names (CryptoDepositsApi).
		/// </summary>
		public static JsonSerializerSettings ProviderCamel => new()
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};
	}
}
