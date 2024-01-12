namespace GithubFreshdeskUserExample.Utility
{
    public static class Extensions
    {
        public const string DateTimeISOStringFormat = "yyyy-MM-dd";

        public static string ToSnakeCaseString(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
