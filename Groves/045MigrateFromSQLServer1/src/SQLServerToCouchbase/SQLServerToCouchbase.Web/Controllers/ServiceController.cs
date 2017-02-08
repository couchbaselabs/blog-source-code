using System.Web.Http;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using SQLServerToCouchbase.Core.Shopping;

namespace SQLServerToCouchbase.Web.Controllers
{
    // this controller is in the same project as the web application
    // but it is meant to illustrate a separate service tier
    public class ServiceController : ApiController
    {
        private readonly IBucket _bucket;

        public ServiceController()
        {
            // this WebAPI controller is only used for Couchbase
            // the SQL server implementation uses a stored procedure
            _bucket = ClusterHelper.GetBucket("sqltocb");
        }

        // tag::service[]
        [HttpGet]
        [Route("api/searchByName/{searchString}")]
        public IHttpActionResult SearchByName(string searchString)
        {
            var n1ql = @"SELECT META(c).id, c.`user`
                FROM `sqltocb` c
                WHERE c.type = 'ShoppingCart'
                AND c.`user` LIKE $searchString";
            var query = QueryRequest.Create(n1ql);
            query.AddNamedParameter("searchString", "%" + searchString + "%");
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var results = _bucket.Query<ShoppingCart>(query).Rows;

            return Json(results);
        }
        // end::service[]
    }
}