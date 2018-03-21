using System;
using System.Collections.Generic;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Search;
using Couchbase.Search.Queries.Geo;
using GeospatialSearch.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeospatialSearch.Controllers
{
    public class GeoSpatialController : Controller
    {
        private readonly IBucket _bucket;

        public GeoSpatialController(IBucketProvider bucketProvider)
        {
            _bucket = bucketProvider.GetBucket("travel-sample");
        }

        // tag::Point[]
        [Route("api/Point")]
        [HttpPost]
        public IActionResult Point([FromBody] PointSearch point)
        {
            var query = new GeoDistanceQuery();
            query.Latitude(point.Latitude);
            query.Longitude(point.Longitude);
            query.Distance(point.DistanceWithUnits);
            var searchParams = new SearchParams()
                .Limit(10)
                .Timeout(TimeSpan.FromMilliseconds(10000));
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
                // *** this part shouldn't be necessary
                // the geo and name should come with the search results
                // and not require a separate lookup
                // but there's maybe an SDK bug
                var doc = _bucket.Get<dynamic>(hit.Id).Value;
                // ****************
                list.Add(new GeoSearchResult
                {
                    Latitude = doc.geo.lat,
                    Longitude = doc.geo.lon,
                    InfoWindow = new InfoWindow
                    {
                        Content = doc.name + "<br />" +
                                  doc.city + ", " +
                                  doc.state + " " +
                                  doc.country
                    }
                });
            }
            return Ok(list);
        }
        // end::Point[]

        // tag::Box[]
        [Route("api/Box")]
        [HttpPost]
        public IActionResult Box([FromBody] BoxSearch box)
        {
            var query = new GeoBoundingBoxQuery();
            query.TopLeft(box.LongitudeTopLeft, box.LatitudeTopLeft);
            query.BottomRight(box.LongitudeBottomRight, box.LatitudeBottomRight);
            var searchParams = new SearchParams()
                .Limit(10)
                .Timeout(TimeSpan.FromMilliseconds(10000));
            var searchQuery = new SearchQuery
            {
                Query = query,
                Index = "mygeoindex",
                SearchParams = searchParams
            };
            var results = _bucket.Query(searchQuery);
            // end::Box[]

            // tag::BoxResults[]
            var list = new List<GeoSearchResult>();
            foreach (var hit in results.Hits)
            {
                // *** this part shouldn't be necessary
                // the geo and name should come with the search results
                // and not require a separate lookup
                // but there's maybe an SDK bug
                var doc = _bucket.Get<dynamic>(hit.Id).Value;
                // ****************
                list.Add(new GeoSearchResult
                {
                    Latitude = doc.geo.lat,
                    Longitude = doc.geo.lon,
                    InfoWindow = new InfoWindow
                    {
                        Content = doc.name + "<br />" +
                            doc.city + ", " +
                            doc.state + " " +
                            doc.country
                    }
                });
            }
            return Ok(list);
        }
        // end::BoxResults[]
    }
}
