using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazorEnterprise.Client
{
    public static class ScreensProvider
    {
        public static IEnumerable<Type> Screens { get; set; }
        static ScreensProvider()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var classes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ComponentBase))
            &&x.Namespace.Contains("BlazorEnterprise.Client.Pages"));
            Screens = classes;
        }
    }
}
