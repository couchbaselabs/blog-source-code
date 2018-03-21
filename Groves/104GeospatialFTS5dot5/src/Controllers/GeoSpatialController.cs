using System;
using System.Collections.Generic;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Search;
using Couchbase.Search.Queries.Geo;
using Microsoft.AspNetCore.Mvc;

namespace GeospatialSearch.Controllers
{
    public class BoxSearch
    {
        public double LatitudeTopLeft { get; set; }
        public double LongitudeTopLeft { get; set; }
        public double LatitudeBottomRight { get; set; }
        public double LongitudeBottomRight { get; set; }
    }

    public class GeoSearchResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string InfoWindow { get; set; }
    }

    public class PointSearch
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Distance { get; set; }
        // miles is being assumed as the unit
        public string DistanceWithUnits => Distance + "mi";
    }

    public class GeoSpatialController : Controller
    {
        private readonly IBucket _bucket;

        public GeoSpatialController(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("travel-sample");
        }

        [Route("api/Point")]
        [HttpPost]
        public IActionResult Point([FromBody] PointSearch point)
        {
            var query = new GeoDistanceQuery();
            query.Latitude(point.Latitude);
            query.Longitude(point.Longitude);
            query.Distance(point.DistanceWithUnits);
            var searchParams = new SearchParams().Limit(10).Timeout(TimeSpan.FromMilliseconds(10000));
            var searchQuery = new SearchQuery
            {
                Query = query,
                Index = "mygeoindex",
                SearchParams = searchParams
            };
            var results = _bucket.Query(searchQuery);

            var list = new List<GeoSearchResult>();
            foreach (var hit in results.Hits)
            {
                list.Add(new GeoSearchResult
                {

                });
            }
//            list.Add(new GeoSearchResult
//            {
//                Latitude = 37.754582,
//                Longitude = -122.446418,
//                InfoWindow = "Something something " + Guid.NewGuid()
//            });
            return Ok(list);
        }

        [Route("api/Box")]
        [HttpPost]
        public IActionResult Box([FromBody] BoxSearch box)
        {
            var query = new GeoBoundingBoxQuery();
            query.TopLeft(box.LongitudeTopLeft, box.LatitudeTopLeft); // -2.235143, 53.482358);
            query.BottomRight(box.LongitudeBottomRight, box.LatitudeBottomRight); // 28.955043, 40.991862);
            var searchParams = new SearchParams().Limit(10).Timeout(TimeSpan.FromMilliseconds(10000));
            var searchQuery = new SearchQuery
            {
                Query = query,
                Index = "mygeoindex",
                SearchParams = searchParams
            };
            var results = _bucket.Query(searchQuery);

            var list = new List<GeoSearchResult>();
            foreach (var hit in results.Hits)
            {
                list.Add(new GeoSearchResult
                {

                });
            }
//            list.Add(new GeoSearchResult
//            {
//                Latitude = 37.754582,
//                Longitude = -122.446418,
//                InfoWindow = "Something something " + Guid.NewGuid()
//            });
            return Ok(list);
        }
    }
}
