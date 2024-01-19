using Humanizer;

namespace SharedKernel.Libraries
{
    public static class LanguageHelper
    {
        public static string Singularize(string input)
        {
            return input.Singularize();
        }

        public static string Pluralize(string input)
        {
            return input.Pluralize();
        }

        public static string Normalize(string input)
        {
            return input.Normalize();
        }

        public static string Pascalize(string input)
        {
            return input.Pascalize();
        }

        public static string Titleize(string input)
        {
            return input.Titleize();
        }

        public static string Humanize(string input)
        {
            return input.Humanize();
        }
    }
}
