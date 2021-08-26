using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using DataIntegrityCheckerTests.ConsoleApp;
using Xunit;
using Xunit.Abstractions;
using Xunit.Runners;

namespace DataIntegrityCheckerTests
{
	class Program
	{
		// Use an event to know when we're done
		private static ManualResetEvent finished = new ManualResetEvent(false);

		// Start out assuming success; set this to 1 if we get a failed test
		private static int result = 0;

		private static readonly ConsoleView view = new ConsoleView();
		private static SortedSet<string> initializedScopes;

		static int Main(string[] args)
		{
			view.ShowInitializationStartMessage();
			var initializationDetails = Services.InitRunConfiguration(args);
			view.ShowValidationErrors(initializationDetails);
			if (initializationDetails.ValidConfigFiles.Count == 0)
			{
				result = 2;
				view.ShowNoConfigurationFilesError();
				view.ShowTestRunIsCompletedMessage();
				return result;
			}
			view.ShowInitializationCompletedMessage(initializationDetails);
			if (DateTime.UtcNow.Hour < 12)
			{
				view.ShowTransformationsMightBeStillRunningWarning();
			}
			initializedScopes =
				new SortedSet<string>(initializationDetails.TestsToExecute.SelectMany(t => t.scope));
			
			var assembly = Assembly.GetExecutingAssembly().Location;
			using (var runner = AssemblyRunner.WithoutAppDomain(assembly))
			{
				runner.OnDiscoveryComplete = view.OnDiscoveryComplete;
				runner.OnExecutionComplete = info =>
				{
					view.OnExecutionComplete(info);
					finished.Set();
				};

				runner.OnTestStarting = view.OnTestStarting;
				// So far skipped tests are filtered out by testCaseFilter
				runner.OnTestSkipped = view.OnTestSkipped;
				runner.OnTestPassed = view.OnTestPassed;
				runner.OnTestFailed = info =>
				{
					view.OnTestFailed(info);
					result = 1;
				};

				view.ShowDiscoveringMessage();
				runner.TestCaseFilter = testCaseFilter;
				runner.Start(methodDisplay: TestMethodDisplay.Method, parallel: true);

				finished.WaitOne();
				finished.Dispose();

				view.ShowTestRunIsCompletedMessage();
				return result;
			}
		}

		private static bool testCaseFilter(ITestCase testCase)
		{
			if (!string.IsNullOrEmpty(testCase.SkipReason))
			{
				return false;
			}
			if (!testCase.Traits.ContainsKey("Scope") ||
				!Consts.KnownScopes.Contains(testCase.Traits["Scope"].First()) ||
				!initializedScopes.Contains(testCase.Traits["Scope"].First()))
			{
				return false;
			}
			return true;
		}
	}
}