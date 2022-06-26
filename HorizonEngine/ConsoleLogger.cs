using System;
using System.Collections.Generic;
using System.Text;

namespace HorizonEngine
{
	public static class ConsoleLogger
	{
		public enum MessageType
		{
			Info,
			Alert,
			Error,
			Critical,
			Highlight
		}

		public static void PrintMessage(MessageType _MessageType, string _Message)
		{
			switch(_MessageType)
			{
				case MessageType.Info:
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "[INFO]  " + _Message);
					break;

				case MessageType.Alert:
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "[ALERT] " + _Message);
					break;

				case MessageType.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "[ERROR] " + _Message);
				break;

				case MessageType.Critical:
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine("[" + DateTime.Now.ToString() + "]" + "[CRIT]  " + _Message);
				break;

				case MessageType.Highlight:
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine(_Message);
				break;

				default:
					Console.ForegroundColor = ConsoleColor.Magenta;
					Console.WriteLine("Program tried to send an unknown message type!\nMessage: " + _Message);
					break;
			}

			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}
