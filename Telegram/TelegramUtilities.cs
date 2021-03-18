using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
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
		/// Makes a keyboard where values and labels are each 2 elements of the passed array
		/// the values will be added as inline query if switchInline is set
		/// </summary>
		/// <param name="valuesLabels">Value|Label|Value|Label and so on. the length of this collection must be even</param>
		/// <param name="perRow">How many buttons on one row, pass 0 to auto detect</param>
		/// <param name="switchInline">0: will not switch inline, 1: inline same chat, 2: inline private chat</param>
		/// <returns>The InlineKeyboardMarkup to send to telegram</returns>
		public static InlineKeyboardMarkup MakeKeyboard(int perRow, params string[] valuesLabels)
		{
			if (valuesLabels == null) return null;
			perRow = perRow == 0 ? int.MaxValue : perRow;
			int k = 0;
			List<List<InlineKeyboardButton>> rows = new List<List<InlineKeyboardButton>>();
			List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
			var array = valuesLabels.ToArray();
			if (array.Length % 2 != 0)
				throw new InvalidParameterException("valuesLabel must contain an even number of values");
			for (int i = 0; i < array.Length;)
			{
				if (k == perRow)
				{
					rows.Add(row);
					row = new List<InlineKeyboardButton>();
					k = 0;
				}
				string value = array[i++];
				bool isUrl = Uri.TryCreate(value, UriKind.Absolute, out Uri url);
				if (isUrl)
					row.Add(new InlineKeyboardButton() { Url = value, Text = array[i++] });
				else if (value == INLINEKEYBOARD_PAY_BUTTON_VALUE)
					row.Add(new InlineKeyboardButton() { Pay = true, Text = array[i++] });
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
					row.Add(new InlineKeyboardButton() { SwitchInlineQueryCurrentChat = query, Text = array[i++] });
				}
				else if (value.StartsWith(SwitchInlineQuery_TOKEN) && value.Length >= SwitchInlineQuery_TOKEN.Length)
				{
					string query = value.Length == SwitchInlineQuery_TOKEN.Length ? "" : value.Substring(SwitchInlineQuery_TOKEN.Length);
					row.Add(new InlineKeyboardButton() { SwitchInlineQuery = query, Text = array[i++] });
				}
				else
				{
					row.Add(new InlineKeyboardButton() { CallbackData = value, Text = array[i++] });
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
				row.Add(new KeyboardButton() { Text = labels[i++] });
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
	}
}
