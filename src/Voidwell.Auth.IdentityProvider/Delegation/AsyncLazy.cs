using System;
using System.Threading.Tasks;

namespace Voidwell.Auth.IdentityProvider.Delegation;

internal class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(() => Task.Factory.StartNew(taskFactory).Unwrap())
    { }
}
