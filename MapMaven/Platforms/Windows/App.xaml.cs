﻿// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using MapMaven.Platforms.Windows;
using Microsoft.UI.Xaml;
using Squirrel;

namespace MapMaven.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    private readonly SingleInstanceDesktopApp _singleInstanceApp;
    private MauiApp _app;

    private LaunchActivatedEventArgs _launchEventArgs = null;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
	{
		InitializeComponent();

        _singleInstanceApp = new SingleInstanceDesktopApp("MAP-MAVEN");
        _singleInstanceApp.Launched += OnSingleInstanceLaunched;
    }

	protected override MauiApp CreateMauiApp()
    {
        _app = MauiProgram.CreateMauiApp();

        return _app;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        SquirrelAwareApp.HandleEvents(
            onInitialInstall: OnAppInstall,
            onAppUninstall: OnAppUninstall,
            onEveryRun: OnAppRun
        );

        _launchEventArgs = args;
        _singleInstanceApp.Launch(args.Arguments);
    }

    private void OnSingleInstanceLaunched(object? sender, SingleInstanceLaunchEventArgs e)
    {
        if (e.IsFirstLaunch)
        {
            base.OnLaunched(_launchEventArgs);

            if (e.Arguments.Contains("startupLaunch"))
                Platforms.Windows.WindowExtensions.MinimizeToTray();
        }
        else
        {
            Platforms.Windows.WindowExtensions.BringToFront();
        }
    }

    private static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
    }

    private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
    }

    private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        tools.SetProcessAppUserModelId();
    }
}

