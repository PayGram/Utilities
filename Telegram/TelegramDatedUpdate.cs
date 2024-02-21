using Telegram.Bot.Types;

public class TelegramDatedUpdate : Update
{
	public DateTime ReceivedUtc { get; set; }
	public TelegramDatedUpdate(Update update)
	{
		Id = update.Id;
		Message = update.Message;
		EditedMessage = update.EditedMessage;
		ChannelPost = update.ChannelPost;
		EditedChannelPost = update.EditedChannelPost;
		InlineQuery = update.InlineQuery;
		ChosenInlineResult = update.ChosenInlineResult;
		CallbackQuery = update.CallbackQuery;
		CallbackQuery = update.CallbackQuery;
		ShippingQuery = update.ShippingQuery;
		PreCheckoutQuery = update.PreCheckoutQuery;
		Poll = update.Poll;
		PollAnswer = update.PollAnswer;
		MyChatMember = update.MyChatMember;
		ChatMember = update.ChatMember;
		ChatJoinRequest = update.ChatJoinRequest;
	}
}
