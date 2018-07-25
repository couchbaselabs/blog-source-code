using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace DistributedCachingExample.Controllers
{
    public class ValuesController : Controller
    {
        private readonly IDistributedCache _cache;

        // tag::ctor[]
        public ValuesController(IDistributedCache cache)
        {
            _cache = cache;
        }
        // end::ctor[]

        // tag::getNoCache[]
        [Route("api/get")]
        public string Get()
        {
            // generate a new string
            var myString = Guid.NewGuid() + " " + DateTime.Now;

            // wait 5 seconds (simulate a slow operation)
            Thread.Sleep(5000);

            // return this value
            return myString;
        }
        // end::getNoCache[]

        // tag::getWithCache[]
        [Route("api/getfast")]
        public string GetUsingCache()
        {
            // is the string already in the cache?
            var myString = _cache.GetString("CachedString1");
            if (myString == null)
            {
                // string is NOT in the cache

                // generate a new string
                myString = Guid.NewGuid() + " " + DateTime.Now;

                // wait 5 seconds (simulate a slow operation)
                Thread.Sleep(5000);

                // put the string in the cache
                _cache.SetString("CachedString1", myString);

                // cache only for 5 minutes
                // tag::cacheSliding[]
                // _cache.SetString("CachedString1", myString,
                //  new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5)});
                // end::cacheSliding[]
            }

            return myString;
        }
        // end::getWithCache[]
    }
}
