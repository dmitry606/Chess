using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Chess.MvcClient.Infrastructure
{
    public static class LoggerExtensions
    {
		public static void AddFileDestination(this ILoggerFactory factory, IConfigurationSection config)
		{
			var logPath = config["LogPath"];
			var minLevel = config["LogLevel"] ?? config.GetSection("LogLevel")["Default"];

			AddFileDestination(factory, logPath, (LogLevel)Enum.Parse(typeof(LogLevel), minLevel));
		}

		public static void AddFileDestination(this ILoggerFactory factory, string fullName, LogLevel minLevel)
		{
			if (null == fullName)
				throw new ArgumentNullException(nameof(fullName));

			Truncate(fullName);
			factory.AddProvider(new FileLoggerProvider(fullName, minLevel));
		}

		private static void Truncate(string fullName)
		{
			if (!File.Exists(fullName))
				return;

			using (new FileStream(fullName, FileMode.Truncate))
			{ }
		}
    }

	public class FileLoggerProvider : ILoggerProvider
	{
		private StreamWriter _stream;

		public LogLevel Level { get; }
		public string FullName { get; }


		public FileLoggerProvider(string fullName, LogLevel minLevel)
		{
			FullName = fullName;
			Level = minLevel;

			_stream = new StreamWriter(fullName);
		}

		public ILogger CreateLogger(string categoryName)
		{
			return new StreamLogger(_stream, categoryName, Level);
		}

		public void Dispose()
		{
			_stream.Dispose();
		}
	}

	public class StreamLogger : ILogger
	{
		private static readonly string _loglevelPadding = ": ";
		private static readonly string _messagePadding;

		public string Name { get; }
		public LogLevel Level { get; }
		public bool WriteCategory { get; set; }

		private StreamWriter _output;

		static StreamLogger()
		{
			var logLevelString = GetLogLevelString(LogLevel.Information);
			_messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
		}

		public StreamLogger(StreamWriter output, string name, LogLevel level, bool writeCategory = false)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (output == null)
			{
				throw new ArgumentNullException(nameof(output));
			}

			Name = name;
			Level = level;
			_output = output;
			WriteCategory = writeCategory;
		}

		public IDisposable BeginScopeImpl(object state)
		{
			return NullDisposable.Instance;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logLevel >= Level;
		}

		public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}
			string message;
			
			if (formatter != null)
			{
				message = formatter(state, exception);
			}
			else
			{
				message = LogFormatter.Formatter(state, exception);
			}

			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			WriteMessage(logLevel, eventId, message);
		}

		private void WriteMessage(LogLevel logLevel, int eventId, string message)
		{
			//var s = GetLogLevelString(logLevel) + _loglevelPadding + (WriteCategory? Name : string.Empty) + _messagePadding + message;
			// check if the message has any new line characters in it and provide the padding if necessary
			message = message.Replace(Environment.NewLine, Environment.NewLine + _messagePadding);

			// Example:
			// INFO: ConsoleApp.Program[10]
			//       Request received
			_output.WriteLine(GetLogLevelString(logLevel) + _loglevelPadding + Name + $"[{eventId}]");
			_output.WriteLine(_messagePadding + message);
			_output.Flush();
		}


		private static string GetLogLevelString(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Verbose:
					return "TRCE";
				case LogLevel.Debug:
					return "DBUG";
				case LogLevel.Information:
					return "INFO";
				case LogLevel.Warning:
					return "WARN";
				case LogLevel.Error:
					return "FAIL";
				case LogLevel.Critical:
					return "CRIT";
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel));
			}
		}

		private class NullDisposable : IDisposable
		{
			public static readonly NullDisposable Instance = new NullDisposable();

			public void Dispose()
			{
				// intentionally does nothing
			}
		}
	}
}

