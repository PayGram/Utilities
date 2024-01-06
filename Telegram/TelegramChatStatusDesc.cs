using Telegram.Bot.Types;

namespace Utilities.Telegram.Extentions
{
	/// <summary>
	/// Represents the status of a telegram chat with a specific user
	/// </summary>
	public class TelegramChatStatusDesc
	{
		/// <summary>
		/// The user id this TelegramChatStatus belongs to
		/// </summary>
		public long UserTid => ChatId?.Identifier ?? 0;
		/// <summary>
		/// The most recent message id sent by the bot
		/// </summary>
		public int LastMessageId { get; set; }
		/// <summary>
		/// The message id that the user has just sent
		/// </summary>
		public int AnswerToMessageId { get; set; }

		public ChatId ChatId { get; internal set; }
		/// <summary>
		/// Gets or sets whether the last message sent by the bot should be deleted
		/// </summary>
		public bool DeleteMessage { get; set; }
		public int DeleteAlsoMessageId { get; internal set; }

		public TelegramChatStatusDesc(ChatId chatId)
		{
			this.ChatId = chatId;
		}
		public TelegramChatStatusDesc()
		{

		}
		public override string ToString()
		{
			//			return $"{ChatId}:{UserTid}:{LastMessageId}:{AnswerToMessageId}:{DeleteMessage}";
			return $"{ChatId}:{LastMessageId}:{AnswerToMessageId}:{DeleteMessage}";
		}
	}
}