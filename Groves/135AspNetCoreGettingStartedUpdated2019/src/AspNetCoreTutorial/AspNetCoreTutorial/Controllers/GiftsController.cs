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

    // tag::getall[]
    [HttpGet]
    [Route("api/getall")]
    public async Task<IActionResult> GetAll()
    {
        var bucket = await _bucketProvider.GetBucketAsync("demo");
        var cluster = bucket.Cluster;

        var result = await cluster.QueryAsync<WishlistItem>(
            "SELECT META(w).id, w.* FROM demo._default.wishlist w;"
        );

        return Ok(result);
    }
    // end::getall[]

    // tag::get[]
    [HttpGet]
    [Route("api/get/{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var bucket = await _bucketProvider.GetBucketAsync("demo");
        var collection = await bucket.CollectionAsync("wishlist");

        var item = await collection.GetAsync(id.ToString());

        return Ok(item.ContentAs<WishlistItem>());
    }
    // end::get[]

    // tag::editWithSql[]
    [HttpPost]
    [Route("api/editWithSql")]
    public async Task<IActionResult> CreateOrEditWithSql(WishlistItem item)
    {
        var bucket = await _bucketProvider.GetBucketAsync("demo");
        var cluster = bucket.Cluster;

        var id = item.Id ?? Guid.NewGuid();

        var result = await cluster.QueryAsync<WishlistItem>(
            @"UPSERT INTO demo._default.wishlist (KEY, VALUE)
                      VALUES ($id, { ""name"" : $name });",
            options => options
                .Parameter("id", id)
                .Parameter("name", item.Name)
        );

        return Ok(result);
    }
    // end::editWithSql[]

    // tag::edit[]
    [HttpPost]
    [Route("api/edit")]
    public async Task<IActionResult> CreateOrEdit(WishlistItem item)
    {
        var bucket = await _bucketProvider.GetBucketAsync("demo");
        var collection = await bucket.CollectionAsync("wishlist");

        var id = item.Id ?? Guid.NewGuid();

        await collection.UpsertAsync(id.ToString(), new
        {
            Name = item.Name
        });

        return Ok(new { success = true});
    }
    // end::edit[]

    /*

    [HttpDelete]
    [Route("api/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
    var bucket = await _bucketProvider.GetBucketAsync("demo");
    var collection = await bucket.CollectionAsync("wishlist");

    await collection.RemoveAsync(id.ToString());

    return Ok(new { success = true });
    }
    */
}