using System.Windows;
using VsCodeProfileCommon.Services;

namespace VsCodeProfileProjectSearch;

public partial class App : Application
{
	public App()
	{
		LoggingService.Initialize("VsCodeProfileProjectSearch");
		LoggingService.Info("Application starting.");

		DispatcherUnhandledException += (_, args) =>
		{
			LoggingService.Error("Dispatcher unhandled exception.", args.Exception);
		};

		AppDomain.CurrentDomain.UnhandledException += (_, args) =>
		{
			var ex = args.ExceptionObject as Exception;
			LoggingService.Error("AppDomain unhandled exception.", ex ?? new Exception("Unknown unhandled exception object."));
		};

		TaskScheduler.UnobservedTaskException += (_, args) =>
		{
			LoggingService.Error("TaskScheduler unobserved task exception.", args.Exception);
			args.SetObserved();
		};
	}
}
