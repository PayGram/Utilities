using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.Telegram
{
	public static class TelegramUtilities
	{
		/// <summary>
		/// Value of the key so that a pay button appears
		/// </summary>
		public const string INLINEKEYBOARD_PAY_BUTTON_VALUE = "__PAY__";
		/// <summary>
		/// Value of the key so that pressing the button will do a SwitchInlineQueryCurrentChat with the text passed as value of value
		/// </summary>
		public const string SwitchInlineQueryCurrentChat_TOKEN = "__SWIQCC__";
		/// <summary>
		/// Value of the key so that pressing the button will do a SwitchInlineQuery  with the text passed as value of value
		/// </summary>
		public const string SwitchInlineQuery_TOKEN = "__SWIQ__";
		/// <summary>
		/// Value of the key so that pressing the buton will open the link following this tag in a webapp
		/// </summary>
		public const string OpenURLWithWebApp_TOKEN = "__WEBA__";
		/// <summary>
		/// Makes a keyboard where values and labels are each 2 elements of the passed array
		/// the values will be added as inline query if switchInline is set
		/// </summary>
		/// <param name="valuesLabels">Value|Label|Value|Label and so on. the length of this collection must be even</param>
		/// <param name="perRow">How many buttons on one row, pass 0 to auto detect</param>
		/// <param name="switchInline">0: will not switch inline, 1: inline same chat, 2: inline private chat</param>
		/// <returns>The InlineKeyboardMarkup to send to telegram</returns>
		public static InlineKeyboardMarkup? MakeKeyboard(int perRow, params string[] valuesLabels)
		{
			if (valuesLabels == null) return null;
			perRow = perRow == 0 ? int.MaxValue : perRow;
			int k = 0;
			List<List<InlineKeyboardButton>> rows = new List<List<InlineKeyboardButton>>();
			List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
			var array = valuesLabels.ToArray();
			if (array.Length % 2 != 0)
				throw new ArgumentException("valuesLabel must contain an even number of values");
			for (int i = 0; i < array.Length;)
			{
				if (k == perRow)
				{
					rows.Add(row);
					row = new List<InlineKeyboardButton>();
					k = 0;
				}
				string value = array[i++];
				Uri url;
				bool isUrl = Uri.TryCreate(value, UriKind.Absolute, out url);
				bool requestedWebApp = false;
				requestedWebApp = value.StartsWith(OpenURLWithWebApp_TOKEN) && Uri.TryCreate(value.Substring(OpenURLWithWebApp_TOKEN.Length), UriKind.Absolute, out url);
				isUrl = isUrl || requestedWebApp;

				if (isUrl)
				{
					InlineKeyboardButton button;
					if (requestedWebApp)
					{
						WebAppInfo wai = new() { Url = url.ToString() };
						button = InlineKeyboardButton.WithWebApp(array[i++], wai);
					}
					else
						button = new InlineKeyboardButton(array[i++]) { Url = url!.ToString() };
					row.Add(button);
				}
				else if (value == INLINEKEYBOARD_PAY_BUTTON_VALUE)
					row.Add(new InlineKeyboardButton(array[i++]) { Pay = true });
				else if (value == "" || value == null)
				{
					rows.Add(row);
					row = new List<InlineKeyboardButton>();
					k = 0;
					i++;
				}
				else if (value.StartsWith(SwitchInlineQueryCurrentChat_TOKEN) && value.Length >= SwitchInlineQueryCurrentChat_TOKEN.Length)
				{
					string query = value.Length == SwitchInlineQueryCurrentChat_TOKEN.Length ? "" : value.Substring(SwitchInlineQueryCurrentChat_TOKEN.Length);
					row.Add(new InlineKeyboardButton(array[i++]) { SwitchInlineQueryCurrentChat = query });
				}
				else if (value.StartsWith(SwitchInlineQuery_TOKEN) && value.Length >= SwitchInlineQuery_TOKEN.Length)
				{
					string query = value.Length == SwitchInlineQuery_TOKEN.Length ? "" : value.Substring(SwitchInlineQuery_TOKEN.Length);
					row.Add(new InlineKeyboardButton(array[i++]) { SwitchInlineQuery = query });
				}
				else
				{
					row.Add(new InlineKeyboardButton(array[i++]) { CallbackData = value });
				}
				k++;
			}
			if (k != 0)
				rows.Add(row);
			return new InlineKeyboardMarkup(rows);
		}

		/// <summary>
		/// Makes a keyboard where values and labels are each 2 elements of the passed array
		/// </summary>
		/// <param name="valuesLabels">Value|Label|Value|Label and so on. the length of this collection must be even</param>
		/// <param name="perRow">How many buttons on one row, pass 0 to auto detect</param>
		/// <returns>The InlineKeyboardMarkup to send to telegram</returns>
		//public static InlineKeyboardMarkup MakeKeyboard(int perRow, params string[] valuesLabels)
		//{
		//	return MakeKeyboard(perRow, 0, valuesLabels);
		//}

		public static InlineKeyboardMarkup MakeKeyboard(IEnumerable<KeyValuePair<string, string>> valuesLabels, int perRow = 0)
		{
			if (valuesLabels == null) return null;
			return MakeKeyboard(perRow, valuesLabels.SelectMany(x => new[] { x.Key, x.Value }).ToArray());
		}

		public static ReplyKeyboardMarkup MakeReplyKeyboard(bool selective, string[] labels, int perRow, bool resize = true)
		{
			if (labels == null) return null;
			perRow = perRow == 0 ? int.MaxValue : perRow;
			int k = 0;
			List<List<KeyboardButton>> rows = new List<List<KeyboardButton>>();
			List<KeyboardButton> row = new List<KeyboardButton>();
			for (int i = 0; i < labels.Length;)
			{
				if (k == perRow)
				{
					rows.Add(row);
					row = new List<KeyboardButton>();
					k = 0;
				}
				row.Add(new KeyboardButton(labels[i++]));
				k++;
			}
			if (k != 0)
				rows.Add(row);
			var keyboard = new ReplyKeyboardMarkup(rows);
			keyboard.ResizeKeyboard = resize;
			keyboard.Selective = selective;
			return keyboard;
		}

		/// <summary>
		/// Makes a keyboard where  labels are the elements of the passed array
		/// </summary>
		/// <param name="labels">Label|Label and so on.</param>
		/// <param name="perRow">How many buttons on one row, pass 0 to auto detect</param>
		/// <returns>The ReplyKeyboardMarkup to send to telegram</returns>
		public static ReplyKeyboardMarkup MakeReplyKeyboard(int perRow, bool resize, bool selective, params string[] labels)
		{
			return MakeReplyKeyboard(selective, labels, perRow, resize);
		}

		public static ReplyKeyboardMarkup MakeReplyKeyboard(IEnumerable<KeyValuePair<string, string>> valuesLabels, int perRow = 0, bool resize = true, bool selective = false)
		{
			if (valuesLabels == null) return null;
			return MakeReplyKeyboard(perRow, resize, selective, valuesLabels.SelectMany(x => new[] { x.Key, x.Value }).ToArray());
		}
		/// <summary>
		/// returns a link with the html syntax <a href="">username or firstname</a>
		/// </summary>
		/// <param name="tid"></param>
		/// <param name="username"></param>
		/// <param name="firstName"></param>
		/// <returns></returns>
		public static string? CreateLinkToUser(long tid, string? username = null, string? firstName = null)
		{
			if (tid == 0 && username == null) return null;

			StringBuilder sb = new();
			if (tid != 0)
				sb.Append($"<a href=\"tg://user?id={tid}\">");

			if (username != null)
			{
				sb.Append("@");
				sb.Append(username);
				if (firstName != null)
					sb.Append(" - ");
			}
			if (firstName != null)
				sb.Append(firstName);
			if (username == null && firstName == null)
				sb.Append("anonymous");
			if (tid != 0)
				sb.Append("</a>");
			return sb.ToString();
			//$"<a href=\"tg://user?id={TId}\">{(string.IsNullOrWhiteSpace(_user?.UsernameOrFirstName) ? (TId == 0 ? "anonymous" : TId.ToString()) : _user.UsernameOrFirstName)}</a>"
		}
		/// <summary>
		/// returns the link only
		/// </summary>
		/// <param name="tid"></param>
		/// <returns></returns>
		public static string CreateLinkToUser(long tid)
		{
			return $"tg://user?id={tid}";
		}
	}
}
