using UserService.Tests.Domain.Utilities;
using Xunit;

namespace UserService.Tests.Domain;

public class ErrorCatalogTests
{
    [Fact]
    public void All_Error_Codes_Are_Unique()
    {
        // Get all error definitions from the domain assembly
        var errors = ErrorCatalog.GetAllErrors();

        var duplicates = errors
            .GroupBy(e => e.Code)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        Assert.False(duplicates.Any(), $"Duplicate error codes found: {string.Join(", ", duplicates)}");
    }
}