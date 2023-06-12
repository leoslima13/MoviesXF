using System;
using System.Linq;
using Xamarin.Forms;

namespace Movies.Helpers
{
    public static class Colors
    {
        private static readonly Lazy<ResourceDictionary> _colors = new Lazy<ResourceDictionary>(() =>
        {
            return Application.Current.Resources.MergedDictionaries.Single(md => md.Source.OriginalString.Contains("Styles/Colors.xaml"));
        });

        public static Color PrimaryColor => (Color)_colors.Value["PrimaryColor"];
        public static Color WhiteColor => (Color)_colors.Value["WhiteColor"];
        public static Color AccentColor => (Color)_colors.Value["AccentColor"];
        public static Color BlackColor => (Color)_colors.Value["BlackColor"];
    }
}