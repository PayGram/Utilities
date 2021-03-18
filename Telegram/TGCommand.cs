using System;
using Utilities.String.Extentions;

namespace Utilities.Telegram
{
	/// <summary>
	/// Parses and makes a command received/to send on Telegram
	/// </summary>
	public class TGCommand
	{
		public const char PARAMS_SEP = '_';
		/// <summary>
		/// The name of the bot which is receiving commands
		/// </summary>
		public string BotName { get; set; }
		/// <summary>
		/// The command to send or receive
		/// </summary>
		public string Command { get; set; }
		/// <summary>
		/// The parameters of the command to send/received
		/// </summary>
		public string[] Parameters { get; set; }
		/// <summary>
		/// The original text sent by telegram
		/// </summary>
		public string ParseText { get; set; }
		/// <summary>
		/// Makes a link that can be parsed back to TGCommand
		/// If it is a start or startgroup command, it can be opened on Telegram
		/// Parameters will be Base64-encoded
		/// </summary>
		/// <returns>a link that can be parsed back to TGCommand</returns>
		public string MakeBotLink()
		{
			return TGCommand.MakeBotLink(BotName, Command, Parameters);
		}
		public string FirstParameter => Parameters == null || Parameters.Length == 0 ? null : Parameters[0];

		public bool HasParameters => Parameters != null && Parameters.Length > 0;
		public int ParametersCount => !HasParameters ? 0 : Parameters.Length;
		/// <summary>
		/// Returns the parameter at the specified index or null if the parameter is outside the Parameters lenght
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		public string ParameterAt(int idx) => idx >= ParametersCount ? null : Parameters[idx];

		/// <summary>
		/// Makes a link that can be parsed back to TGCommand
		/// If it is a start or startgroup command, it can be opened on Telegram
		/// Parameters will be Base64-encoded
		/// </summary>
		/// <param name="BotName">the telegram bot username, should not be null, or the link will not work</param>
		/// <param name="Command">start or startgroup</param>
		/// <param name="Parameters">The parameters that will be base64 encoded and passed to the bot</param>
		/// <returns>a link that can be parsed back to TGCommand</returns>
		public static string MakeBotLink(string BotName, string Command, params string[] Parameters)
		{
			string prms = Parameters == null || Parameters.Length == 0 ? null : string.Join($"{PARAMS_SEP}", Parameters);
			prms = prms == null ? null : "=" + prms.Base64Encode();
			return $"https://t.me/{BotName}?{Command}{prms}";
		}

		/// <summary>
		/// Returns a string representation, non base64-encoded of this TGCommand
		/// </summary>
		public override string ToString()
		{
			string prms = Parameters == null ? null : " " + string.Join(" ", Parameters);
			return $"{Command}{prms}";
		}

		/// <summary>
		/// Parses and returns a TGCommand.
		/// If botName is specified and it is not found on the command, null will be returned.
		/// If botName is not specified, any command will be parsed and returned.
		/// The parameters after start can be either base64-encoded or plain text
		/// </summary>
		/// <param name="text">The text to look for a command</param>
		/// <param name="botName">If botName is specified, only commands containing this name will be returned. If it is null, all commands will be returned</param>
		/// <returns>A TGCommand that represents the passed text</returns>
		public static TGCommand Parse(string text, string botName = null)
		{
			if (string.IsNullOrWhiteSpace(text)) return null;

			if (text.StartsWith("/") == false) //not a command
				return null;

			// examples of commands:
			// /start@botname p1_p2_p3
			// /start p1_p2_p3

			string[] commandParts = text.Split(' ');
			if (commandParts.Length == 0 || commandParts.Length > 2)
				return null; // invalid telegram command

			// /start@botname  OR  /start
			string commandPart = commandParts[0];
			if (string.IsNullOrEmpty(botName) == false && commandPart.EndsWith($"@{botName}", StringComparison.OrdinalIgnoreCase) == false)
				return null; // comand not for us

			// p1_p2_p3
			string prmsPart = commandParts.Length == 2 ? commandParts[1] : null;
			prmsPart = prmsPart.Base64Decode() ?? prmsPart; // is it base64-encoded
			string[] prms = prmsPart?.Split(PARAMS_SEP);


			int iofAt = text.IndexOf("@");
			string command;
			if (iofAt == -1)
				command = commandPart.Substring(1); //drop the /
			else
				command = commandPart.Substring(1, iofAt - 1);

			return new TGCommand()
			{
				BotName = botName,
				Command = command,
				Parameters = prms ?? new string[] { },
				ParseText = text
			};
		}

		/// <summary>
		/// Makes a command that can be sent into a chat
		/// </summary>
		/// <param name="adminBotName">eventually the name of the bot, or null</param>
		/// <param name="cmd">the command to pass to the bot</param>
		/// <param name="parm">the parameters that will be base64 encoded</param>
		/// <returns></returns>
		public static string MakeCommand(string adminBotName, string cmd, params string[] parm)
		{
			if (string.IsNullOrWhiteSpace(cmd)) return null;
			string prms = parm == null || parm.Length == 0 ? null : string.Join($"{PARAMS_SEP}", parm);
			prms = prms == null ? null : prms.Base64Encode();
			cmd = adminBotName != null ? $"{cmd}@{adminBotName}" : cmd;
			return $"/{cmd} {prms}";
		}
	}
}
