using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Caching
{
    public interface IDistributedRedisCache : IBaseCaching
    {
        IDistributedCache GetOriginalCaching();
    }
}
