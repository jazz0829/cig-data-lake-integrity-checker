using System;
using System.Linq;
using DataIntegrityCheckerTests.DataAccess;
using Xunit.Runners;

namespace DataIntegrityCheckerTests.ConsoleApp
{
	public class ConsoleView
	{
		// We use consoleLock because messages can arrive in parallel, so we want to make sure we get
		// consistent console output.
		private readonly object consoleLock = new object();

		public void ShowInitializationStartMessage()
		{
			Console.WriteLine("===================================================");
			Console.WriteLine("Initializing tests run...");
		}

		public void ShowTransformationsMightBeStillRunningWarning()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Note that our transformations are executing every morning. Due to that some data could be still incomplete.");
			Console.ResetColor();
		}

		public void ShowValidationErrors(TestsToExecuteInitializationDetails details)
		{
			if (details.NotFoundConfigFiles.Any())
			{
				showWarning("Not found configuration files:");
				var files = string.Join('\n', details.NotFoundConfigFiles);
				Console.WriteLine(files);
			}

			if (details.InvalidFormatConfigFiles.Any())
			{
				showWarning("These files have incorrect yaml format:");
				var files = string.Join('\n', details.InvalidFormatConfigFiles);
				Console.WriteLine(files);
			}

			if (details.TestsWithMissedScope.Any())
			{
				showWarning("Tests with not specified scope:");
				var files = string.Join('\n', details.TestsWithMissedScope);
				Console.WriteLine(files);
			}

			if (details.TestsWithUnknownScope.Any())
			{
				showWarning("Tests with unknown scope:");
				var files = string.Join('\n', details.TestsWithUnknownScope);
				Console.WriteLine(files);
			}
		}

		private void showWarning(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[Warning] ");
			Console.ResetColor();
			Console.WriteLine(message);
		}

		public void ShowNoConfigurationFilesError()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[Error] ");
			Console.ResetColor();
			Console.WriteLine("No provided configuration files can be used for this tests run.");
		}

		public void ShowInitializationCompletedMessage(
			TestsToExecuteInitializationDetails details)
		{
			Console.WriteLine("Tests are initialized from following configuration files:");
			var files = string.Join('\n', details.ValidConfigFiles);
			Console.WriteLine(files);
		}

		public void ShowDiscoveringMessage()
		{
			Console.WriteLine("Discovering tests...");
		}

		public void OnDiscoveryComplete(DiscoveryCompleteInfo info)
		{
			lock (consoleLock)
			{
				Console.WriteLine($"Executing {info.TestCasesToRun} tests...");
			}
		}

		public void OnExecutionComplete(ExecutionCompleteInfo info)
		{
			lock (consoleLock)
			{
				Console.WriteLine(
					$"\nFinished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s" +
					$"\nPassed: {info.TotalTests - info.TestsSkipped - info.TestsFailed}" +
					$"\nSkipped: {info.TestsSkipped}" +
					$"\nFailed: {info.TestsFailed}");
			}
		}
		public void OnTestStarting(TestStartingInfo info)
		{
			lock (consoleLock)
			{
				Console.Write("[Test is Pending...]");
			}
		}

		private void clearCurrentConsoleLine()
		{
			var currentLineCursor = Console.CursorTop;
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, currentLineCursor);
		}

		public void OnTestSkipped(TestSkippedInfo info)
		{
			lock (consoleLock)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
				Console.ResetColor();
			}
		}

		public void OnTestPassed(TestPassedInfo info)
		{
			lock (consoleLock)
			{
				clearCurrentConsoleLine();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("[Passed {0:0.000}s] ", Math.Round(info.ExecutionTime, 3));
				Console.ResetColor();
				Console.WriteLine(info.TestDisplayName);
			}
		}
		public void OnTestFailed(TestFailedInfo info)
		{
			lock (consoleLock)
			{
				clearCurrentConsoleLine();
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("[Failed {0:0.000}s] ", Math.Round(info.ExecutionTime, 3));
				Console.ResetColor();
				// While we didn't figure out how to provide message with failing Approval test, read comparison message from xunit output
				var exceptionMessage = !string.IsNullOrEmpty(info.Output) ? info.Output : info.ExceptionMessage;
				Console.WriteLine("{0}\n{1}", info.TestDisplayName, exceptionMessage);
				// If needed, stacktrace can be also added to the message
			}
		}

		public void ShowTestRunIsCompletedMessage()
		{
			Console.WriteLine("\nTest run is completed.");
#if DEBUG
			Console.WriteLine("Please press enter to exit...");
			Console.ReadLine(); 
#endif
		}
	}
}