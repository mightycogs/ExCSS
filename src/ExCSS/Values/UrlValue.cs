namespace ExCSS
{
    /// <summary>
    /// CSS url() function value.
    /// </summary>
    public sealed class UrlValue : IStyleValue, IFunctionValue
    {
        public string Name => "url";
        public string Url { get; }

        public UrlValue(string url)
        {
            Url = url ?? string.Empty;
        }

        public string CssText => $"url(\"{EscapeUrl(Url)}\")";
        public StyleValueType Type => StyleValueType.Function;

        public override string ToString() => CssText;

        private static string EscapeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            return url.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
