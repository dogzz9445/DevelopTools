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
    public List<string> Uris { get; set; }
}

public static class ThemeHelper
{
    public static Dictionary<string, ThemeInfo> ThemeInfos
        => new Dictionary<string, ThemeInfo>()
        {
            {
                "Generic",
                new ThemeInfo
                {
                    Name = "Dark",
                    Uris = new List<string>(),
                }
            },
            {
                "Dark",
                new ThemeInfo
                {
                    Name = "Dark",
                    Uris = new List<string>()
                    {
                        @"pack://application:,,,/DDT.Core.WPF;component/Themes/Dark.xaml"
                    }
                }
            },
            {
                "Light",
                new ThemeInfo
                {
                    Name = "Light",
                    Uris = new List<string>()
                    {
                        @"pack://application:,,,/DDT.Core.WPF;component/Themes/Light.xaml"
                    }
                }
            },
        };

    public static void ChangeTheme(ResourceDictionary ThemeDictionary, string theme)
    {
        if (theme == null)
            return;

        try
        {
            if (ThemeInfos.TryGetValue(theme, out var themeInfo))
            {
                ThemeDictionary.MergedDictionaries.Clear();

                foreach (var themeUri in themeInfo.Uris)
                {
                    var themeUriSource = new Uri(themeUri, UriKind.Absolute);
                    ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = themeUriSource });
                }
            }
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            MessageBox.Show($"Failed to apply theme '{theme}': {ex.Message}", "Theme Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static ThemeInfo Register(string theme, string uri)
    {
        if (!ThemeInfos.ContainsKey(theme))
            return null;

        var themeInfo = ThemeInfos[theme];
        themeInfo.Uris.Add(uri);
        return themeInfo;
    }
}
