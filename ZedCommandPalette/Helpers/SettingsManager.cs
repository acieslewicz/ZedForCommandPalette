// Copyright (c) acieslewicz
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System;
using System.IO;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace ZedCommandPalette.Helpers;

public class SettingsManager : JsonSettingsManager
{
    private const string Namespace = "zedProjects";

    private static SettingsManager? _instance;

    private readonly TextSetting _dbPath = new(
        Namespaced(nameof(DbPath)),
        "Database Path",
        "Path to Zed's SQLite database. Leave default if setup in the standard location.",
        DefaultDbPath
    );

    private readonly TextSetting _zedPath = new(Namespaced(nameof(ZedPath)), "Zed Path",
        "The executable path for Zed. Leave default if installed in the standard location.", DefaultZedPath);

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(_dbPath);
        Settings.Add(_zedPath);

        LoadSettings();

        Settings.SettingsChanged += (s, async) => SaveSettings();
    }

    private static string DefaultDbPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zed", "db", "0-stable",
            "db.sqlite");

    private static string DefaultZedPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Zed", "Zed.exe");

    public string DbPath => string.IsNullOrWhiteSpace(_dbPath.Value) ? DefaultDbPath : _dbPath.Value;
    public string ZedPath => string.IsNullOrWhiteSpace(_zedPath.Value) ? DefaultZedPath : _zedPath.Value;

    internal static SettingsManager Instance
    {
        get
        {
            _instance ??= new SettingsManager();
            return _instance;
        }
    }

    private static string Namespaced(string propertyName)
    {
        return $"{Namespace}.{propertyName}";
    }

    private static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "settings.json");
    }
}