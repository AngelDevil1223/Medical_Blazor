namespace BlazorEnterprise.Client
{
    public static class ActiveScreen
    {
        public static string Screen { get; set; } = "Screen Name";
        public static Dictionary<string, string> History { get; set; } = new Dictionary<string, string>();
        public static string ApplicationName { get; set; } = "Blazor Enterprise";
    }
}
