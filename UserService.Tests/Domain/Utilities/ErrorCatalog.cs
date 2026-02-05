using System.Reflection;
using UserService.Domain.Common;

namespace UserService.Tests.Domain.Utilities;

internal static class ErrorCatalog
{
    public static IReadOnlyCollection<Error> GetAllErrors()
    {
        var domainAssembly = typeof(Error).Assembly;

        return domainAssembly
            .GetTypes()
            .Where(t => t.IsClass && t.IsAbstract && t.IsSealed)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.FieldType == typeof(Error))
            .Select(f => (Error?)f.GetValue(null))
            .Where(e => e is not null)
            .Cast<Error>()
            .ToList();
    }
}