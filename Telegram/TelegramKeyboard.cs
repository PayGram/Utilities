using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using Utilities.Telegram;

namespace Utilities.Telegram
{
	public class TelegramKeyboard
	{
		List<KeyValuePair<string, string>> keyboard = new List<KeyValuePair<string, string>>();
		public InlineKeyboardMarkup InlineKeyboard => TelegramUtilities.MakeKeyboard(keyboard, ButtonsPerRow);
		public ReplyKeyboardMarkup ReplyKeyboard => TelegramUtilities.MakeReplyKeyboard(keyboard, ButtonsPerRow, Resize, Selective);
		public int ButtonsPerRow { get; set; }
		/// <summary>
		/// If set to true, the reply keyboard is resized.
		/// Only applies to ReplyKeyboard
		/// Default = true
		/// </summary>
		public bool Resize { get; private set; }
		/// <summary>
		/// If set to true, the reply keyboard is sent only to the selected users.
		/// Only applies to ReplyKeyboard.
		/// Default = false
		/// </summary>
		public bool Selective { get; private set; }

		public TelegramKeyboard(int btnsPerRow)
		{
			ButtonsPerRow = btnsPerRow;
			Resize = true;
			Selective = false;
		}
		public TelegramKeyboard(int btnsPerRow, bool resize)
		{
			ButtonsPerRow = btnsPerRow;
			Resize = resize;
		}
		public TelegramKeyboard(int btnsPerRow, bool resize, bool selective)
		{
			ButtonsPerRow = btnsPerRow;
			Resize = resize;
			Selective = selective;
		}
		public void Add(string callbackData, string text)
			=> keyboard.Add(new KeyValuePair<string, string>(callbackData, text));
		public void AddCallback(string text, params string[] callBackData)
			=> keyboard.Add(new KeyValuePair<string, string>(TGCallBack.GetData(callBackData), text));
		public void AddSwitchInlineQuery(string text, params string[] callbackData)
			=> keyboard.Add(new KeyValuePair<string, string>($"{TelegramUtilities.SwitchInlineQuery_TOKEN}{TGCallBack.GetData(callbackData)}", text));
		public void AddSwitchInlineQueryCurrentChat(string text, params string[] callbackData)
			=> keyboard.Add(new KeyValuePair<string, string>($"{TelegramUtilities.SwitchInlineQueryCurrentChat_TOKEN}{TGCallBack.GetData(callbackData)}", text));
	}
}
