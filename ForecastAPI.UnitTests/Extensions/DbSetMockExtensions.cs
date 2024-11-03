using Microsoft.EntityFrameworkCore;
using Moq;

namespace ForecastAPI.UnitTests.Extensions;

public static class DbSetMockExtensions
{
    public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
    {
        var mockDbSet = new Mock<DbSet<T>>();

        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());

        mockDbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => ((List<T>)source).Add(s));
        mockDbSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>((s) => ((List<T>)source).Remove(s));

        return mockDbSet;
    }
}
