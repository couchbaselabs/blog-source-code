using AspNetCoreTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Couchbase;
using Couchbase.Query;
using Couchbase.Extensions.DependencyInjection;

namespace AspNetCoreTutorial.Controllers;

// tag::controllerstart[]
public class GiftsController : Controller
{
    private readonly IBucketProvider _bucketProvider;

    public GiftsController(IBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }
// end::controllerstart[]

    // // tag::getall[]
    // [HttpGet]
    // [Route("api/getall")]
    // public async Task<IActionResult> GetAll()
    // {
    //     var bucket = await _bucketProvider.GetBucketAsync("demo");
    //     var cluster = bucket.Cluster;
    //
    //     var result = await cluster.QueryAsync<WishlistItem>(
    //         "SELECT META(w).id, w.* FROM demo._default.wishlist w;",
    //         options => options.ScanConsistency(QueryScanConsistency.RequestPlus));
    //
    //     return Ok(result);
    // }
    // // end::getall[]
    //
    // [HttpGet]
    // [Route("api/get/{id}")]
    // public async Task<IActionResult> Get(string id)
    // {
    //     var bucket = await _bucketProvider.GetBucketAsync("demo");
    //     var collection = await bucket.CollectionAsync("wishlist");
    //
    //     var item = await collection.GetAsync(id);
    //
    //     return Ok(item.ContentAs<WishlistItem>());
    // }
    //
    // [HttpPost]
    // [Route("api/edit")]
    // public async Task<IActionResult> CreateOrEdit(WishlistItem item)
    // {
    //     var bucket = await _bucketProvider.GetBucketAsync("demo");
    //     var collection = await bucket.CollectionAsync("wishlist");
    //
    //     if (!item.Id.HasValue)
    //         item.Id = Guid.NewGuid();
    //
    //     await collection.UpsertAsync(item.Id.ToString(), new
    //     {
    //         Name = item.Name
    //     });
    //
    //     return Ok(new { success = true});
    // }
    //
    // [HttpDelete]
    // [Route("api/delete")]
    // public async Task<IActionResult> Delete(Guid id)
    // {
    //     var bucket = await _bucketProvider.GetBucketAsync("demo");
    //     var collection = await bucket.CollectionAsync("wishlist");
    //
    //     await collection.RemoveAsync(id.ToString());
    //
    //     return Ok(new { success = true });
    // }
}