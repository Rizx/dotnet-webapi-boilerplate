using DN.WebApi.Application.Common.Interfaces;
using Xunit;

namespace Infrastructure.Test.Caching;

public abstract class CacheServiceTests<TCacheService>
    where TCacheService : ICacheService
{
    private record TestRecord(Guid id, string stringValue, DateTime dateTimeValue);

    private static string _testKey = "testkey";
    private static string _testValue = "testvalue";

    protected abstract TCacheService CreateCacheService();

    [Fact]
    public void GettingANonExistingValueReturnsNull()
    {
        var sut = CreateCacheService();
        string? test = sut.Get<string>(_testKey);
        Assert.Null(test);
    }

    [Fact]
    public void GettingAnExistingValueReturnsThatValue()
    {
        var sut = CreateCacheService();
        sut.Set(_testKey, _testValue);
        string? actual = sut.Get<string>(_testKey);
        Assert.Equal(_testValue, actual);
    }

    [Fact]
    public async Task GettingAnExpiredValueReturnsNull()
    {
        var sut = CreateCacheService();
        sut.Set(_testKey, _testValue, TimeSpan.FromMilliseconds(200));
        string? actual = sut.Get<string>(_testKey);
        Assert.Equal(_testValue, actual);
        await Task.Delay(200);
        actual = sut.Get<string>(_testKey);
        Assert.Null(actual);
    }

    [Fact]
    public void GettingAnExistingObjectReturnsThatObject()
    {
        var expected = new TestRecord(Guid.NewGuid(), _testValue, DateTime.UtcNow);
        var sut = CreateCacheService();
        sut.Set(_testKey, expected);
        var actual = sut.Get<TestRecord>(_testKey);
        Assert.Equal(expected, actual);
    }
}