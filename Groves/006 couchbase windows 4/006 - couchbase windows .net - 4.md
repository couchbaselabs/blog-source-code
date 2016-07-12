# Couchbase with Windows and .NET - Part 4 - Linq2Couchbase

- [Part 1 covered how to install and setup Couchbase on Windows](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-1)
- [Part 2 covered some Couchbase lingo that you'll need to know](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-2)
- [Part 3 showed the very simplest example of using Couchbase in ASP.NET](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-3---asp.net-mvc)

In this blog post, I'm going to build on part 3 by introducing [Linq2Couchbase](https://github.com/couchbaselabs/Linq2Couchbase). I'll also be moving Couchbase out of the Controller and put it into a very basic [repository](http://www.martinfowler.com/eaaCatalog/repository.html) class. My goal of this blog post is to have you feeling comfortable with the basics of Couchbase and Linq2Couchbase, and be able to start applying it in your web application.

## Moving Couchbase out of the Controller

The Controller's job is to direct traffic: take incoming requests, hand them to a model, and then give the results to the view. To follow the [SOLID principles](http://www.butunclebob.com/ArticleS.UncleBob.PrinciplesOfOod) (specifically the Single Responsibility Principle), data access should be somewhere in a "model" and not the controller.

The first step is to refactor the existing code. We can keep the 'really simple example' from Part 3, but it should be moved to a method in another class. Here is the refactored HomeController and the new PersonRepository:

    public class HomeController : Controller
    {
        private readonly PersonRepository _personRepo;

        public HomeController(PersonRepository personRepo)
        {
            _personRepo = personRepo;
        }

        public ActionResult Index()
        {
            var person = _personRepo.GetPersonByKey("foo::123");
            return Content("Name: " + person.name + ", Address: " + person.address);
        }
    }

    public class PersonRepository
    {
        private readonly IBucket _bucket;

        public PersonRepository(IBucket bucket)
        {
            _bucket = bucket;
        }

        public dynamic GetPersonByKey(string key)
        {
            return _bucket.Get<dynamic>(key).Value;
        }
    }

Some things to note:

- HomeController no longer depends directly on Couchbase. If the Couchbase API were to change, for instance, we would only have to make changes to PersonRepository--not to HomeController.
- I did not have to explicitly tell StructureMap how to instantiate PersonRepository. It figures that PersonRepository is "self-binding". If I were to use an interface instead (like IPersonRepository), I would have to make a change to the DefaultRegistry to tell StructureMap that. If you're using a different IoC container, your situation may be different.

## Refactoring to use a Person Class

In the above example, I'm using a ```dynamic``` object. ```dynamic``` is great for some situations, but in this case, it would be a good idea to come up with a more concrete definition of what a "Person" is. I can do this with a C# class.

    public class Person
    {
        public string Name { get; set; } 
        public string Address { get; set; }
    }

I'll also update the PersonRepository to use this class.

    public Person GetPersonByKey(string key)
    {
        return _bucket.Get<Person>(key).Value;
    }

While we're at it, I'm going to take some steps to make this more of a proper MVC app. Instead of returning Content(), I'm going to make the Index action return a View, and I'm going to pass it a *list* of Person objects. I'll create an Index.cshtml file, which will delegate to a partial of _person.cshtml. I'm also going to drop in a layout that uses Bootstrap. This last part is completely gratuitous, but it will make the screenshots look a bit nicer.

New Index action:

        public ActionResult Index()
        {
            var person = _personRepo.GetPersonByKey("foo::123");
            var list = new List<Person> {person};
            return View(list);
        }

Index.cshtml:

	@model List<CouchbaseAspNetExample2.Models.Person>
	
	@{
	    ViewBag.Title = "Home : Couchbase & ASP.NET Example";
	}
	
	@if (!Model.Any())
	{
	    <p>There are no people yet.</p>
	}
	
	@foreach (var item in Model)
	{
	    @Html.Partial("_person", item)
	}

_person.cshtml:

	@model CouchbaseAspNetExample2.Models.Person
	
	<div class="panel panel-default">
	    <div class="panel-heading">
	        <h2 class="panel-title">@Model.Name</h2>
	    </div>
	    <div class="panel-body">
	        @Html.Raw(Model.Address)
	    </div>
	</div>

Now it looks a little nicer. Additionally, we'll be able to show a whole list of Person documents later in the demo.

![The Index view of Couchbase Person documents in Bootstrap](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/006pics/IndexOfCouchbaseDocumentsInBootstrap_001.png)

## Introducing Linq2Couchbase

Couchbase Server supports a query language known as [N1QL](http://www.couchbase.com/n1ql?utm_source=blogs&utm_medium=link&utm_campaign=blogs). It's a superset of SQL, and allows you to leverage your existing knowledge of SQL to construct very powerful queries over JSON documents in Couchbase. Linq2Couchbase takes this a step further and converts Linq queries into N1QL queries (much like Entity Framework converts Linq queries into SQL queries).

Linq2Couchbase is part of [Couchbase Labs](https://github.com/couchbaselabs), and is not yet part of the core, supported Couchbase .NET SDK library. However, if you're used to Entity Framework, NHibernate.Linq, or any other Linq provider, it's a great way to introduce yourself to Couchbase. For some operations, you will still need to use the core Couchbase .NET SDK, but there is a lot we can do with Linq2Couchbase.

Start by adding Linq2Couchbase with NuGet (if you haven't already).

![Install Linq2Couchbase with NuGet](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/006pics/NuGetLinq2Couchbase_002.png)

N1QL (and therefore Linq2Couchbase) depends on the [bucket being indexed](http://developer.couchbase.com/documentation/server/4.5/n1ql/n1ql-language-reference/createprimaryindex.html?utm_source=blogs&utm_medium=link&utm_campaign=blogs). Go into Couchbase Console, click the 'Query' tab, and create a primary index on the ```hello-couchbase``` bucket.

![Create a primary index on a Couchbase bucket](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/006pics/CreatePrimaryIndexOnCouchbaseBucket_004.png)

If you don't have an index, Linq2Couchbase will give you a helpful error message like "No primary index on keyspace hello-couchbase. Use CREATE PRIMARY INDEX to create one."

In order to use Linq2Couchbase most effectively, we have to start giving Couchbase documents a "type" field. This way, we can differentiate between a "person" document and a "location" document, for instance. In this example, I'm only going to have "person" documents, but it's a good idea to do this from the start. I'll just create a Type field, and set it to "Person". I'll also put an attribute on the C# class so that Linq2Couchbase understands that this class is meant for a certain type of document.

	using Couchbase.Linq.Filters;

    [DocumentTypeFilter("Person")]
    public class Person
    {
        public Person()
        {
            Type = "Person";
        }
        public string Type { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; }
    }

If you make these changes, your app will continue to work. This is because we are still retrieving the document by its key. But now let's change the Index action to try and get ALL Person documents.

    public ActionResult Index()
    {
        var list = _personRepo.GetAll();
        return View(list);
    }

We'll need to implement that new GetAll repository method:

	using System.Collections.Generic;
	using System.Linq;
	using Couchbase.Core;
	using Couchbase.Linq;
	using Couchbase.Linq.Extensions;
	using Couchbase.N1QL;

    public class PersonRepository
    {
        private readonly IBucket _bucket;
        private readonly IBucketContext _context;

        public PersonRepository(IBucket bucket, IBucketContext context)
        {
            _bucket = bucket;
            _context = context;
        }

        public List<Person> GetAll()
        {
            return _context.Query<Person>()
               .ScanConsistency(ScanConsistency.RequestPlus)
               .OrderBy(p => p.Name)
               .ToList();
        }
	}

In this example, I'm telling Couchbase to order all the results by Name. If you'd like, you can experiment with the normal Linq methods that you're used to: Where, Select, Take, Skip, and so on. 

Just ignore that ScanConsistency for now: I'll discuss it more later. But what about that IBucketContext? The IBucketContext is similar to DbContext for Entity Framework, or ISession for NHibernate. To get that IBucketContext, we'll need to update the DefaultRegistry.

    For<IBucket>().Singleton().Use<IBucket>("Get a Couchbase Bucket",
        x => ClusterHelper.GetBucket("hello-couchbase", "password!"));
    For<IBucketContext>().HttpContextScoped().Use<IBucketContext>("Get a Couchbase Bucket Context",
        x => new BucketContext(x.GetInstance<IBucket>()));

This is telling StructureMap that I want to create a new BucketContext, and I want it to be scoped to each HTTP request. If you use HttpContextScoped in StructureMap, you also have to use ```HttpContextLifecycle.DisposeAndClearAll()``` in the Application_EndRequest. If you're using a different IoC container, you will have to manage it differently.

Now, if you compile and run the web app again, it will display "There are no people yet". Hey, where did I go?! I didn't show up because the "foo::123" document doesn't have a "type" field yet. Go to Couchbase Console and add it.

![Adding a type field to a Couchbase document](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/006pics/UpdateCouchbaseDocument_003.png)

Once you do that, refresh your web page, and the person will appear again.

## A quick note about ScanConsistency

Linq2Couchbase relies on an Index in order to generate and execute queries. When you add new documents, the index must be updated. Until the index gets updates, any documents not yet indexed will not be returned by Linq2Couchbase (by default). By adding in ScanConsistency of RequestPlus ([See Couchbase documentation for the details about scan consistency](http://developer.couchbase.com/documentation/server/4.5/architecture/querying-data-with-n1ql.html?utm_source=blogs&utm_medium=link&utm_campaign=blogs)), Linq2Couchbase will effectively wait until the index is updated before executing a query and returning a response. This is a tradeoff that you will have to think about when designing your application. Which is more important: raw speed or complete accuracy?

As a simple example, let's say you are creating a content management system:

- If you are creating admin tools, then you probably value complete accuracy more than performance.
	- The admins need to know exactly what's in the data in order to manage it effectively.
	- The admin features are used infrequently compared to the public features, so some latency is acceptable.
- If you're creating a public page that lists all the content, raw speed is probably more important.
	- If a new page of content takes an extra second or two to appear to the public, that's okay.
	- The public portion of a site will be accessed very frequently, so performance is an important factor.
- This is just an example: which type of Scan Consistency you should use is up to you and your use cases.

## Conclusion

Linq2Couchbase is a powerful tool for working with Couchbase in a familiar way. It's open source, but it's not yet officially supported by Couchbase. I've made all the code for this blog post available on [Github](https://github.com/couchbaselabs/couchbase-asp-net-blog-example-2).

In the next post, I'll show you how to use Linq2Couchbase to create, update, and delete documents. We'll also look at the difference in flexibility that Couchbase can give you compared to a traditional RDBMS like SQL Server.

Got questions? Something not working how you expect? Please leave a comment, [ping me on Twitter](http://twitter.com/mgroves), or email me (matthew.groves AT couchbase DOT com) and I'll help you!