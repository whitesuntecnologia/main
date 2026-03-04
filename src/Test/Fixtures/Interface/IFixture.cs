using System;

namespace Test.Fixtures
{
    public interface IFixture<T> where T : class
    {
        T GetFixture();
        Fixture<T> WithChanges(Action<T> changes);
    }
}