using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace DDT.Core.WPF.Utilities;

public class ThemeInfo
{
    public string? Name { get; set; }
    public string? Uri { get; set; }
}

public static class ThemeHelper
{
    public static List<ThemeInfo> ThemeInfos
        => new List<ThemeInfo>()
        {
            new ThemeInfo { Name = "Dark", Uri = "Dark" },
            new ThemeInfo { Name = "Light", Uri = "Light" },
        };

    public static List<ThemeInfo> GetAvailableThemes() => ThemeInfos;

    public static void ChangeTheme(ResourceDictionary ThemeDictionary, string theme)
    {
        if (theme == null)
            return;

        try
        {
            ThemeDictionary.MergedDictionaries.Clear();
            var themeUri = new Uri($"pack://application:,,,/DDT.Core.WPF;component/Themes/{theme}.xaml", UriKind.Absolute);
            ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = themeUri });
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            MessageBox.Show($"Failed to apply theme '{theme}': {ex.Message}", "Theme Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
