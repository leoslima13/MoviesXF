using Prism.Navigation;

namespace Movies.Extensions
{
    public static class NavigationParametersExtensions
    {
        public static INavigationParameters ToNavigationParameter(this object value)
        {
            return new NavigationParameters() {{value.GetType().FullName, value}};
        }

        public static INavigationParameters SetValue<T>(this INavigationParameters navigationParameters, T value)
        {
            navigationParameters.Add(typeof(T).FullName, value);
            return navigationParameters;
        }

        public static T GetValue<T>(this INavigationParameters navigationParameters)
        {
            var key = typeof(T).FullName;
            if (!navigationParameters.ContainsKey(key))
                return default(T);

            return (T)navigationParameters[key];
        }
    }
}