# Couchbase with Windows and .NET - Part 2 - ASP.NET MVC

*This blog post is part 3 of a series*:

- [Part 1 covered how to install and setup Couchbase on Windows](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-1)
- [Part 2 covered some Couchbase lingo that you'll need to know](#)

Are you ready to write some code? In this blog post, we're going to start a new ASP.NET MVC project, add the Couchbase SDK to it with NuGet, and get the infrastructure in place to start using Couchbase.

I just started in Visual Studio with a File->New, and selected ASP.NET Web Application, then selected "MVC". I'm going to assume you have some familiarity with ASP.NET MVC, but if anything looks out of the ordinary to you, please leave a comment, [ping me on Twitter](http://twitter.com/mgroves), or email me (matthew.groves AT couchbase DOT com) with your questions.

## Installing the Couchbase client library ##

The first thing we'll need to do is add the Couchbase .NET client. You can do this with the NuGet UI by right-clicking on "References", clicking "Manage NuGet Packages", clicking "Browse", and then searching for "CouchbaseNetClient". (If you want to, you can search for "Linq2Couchbase" instead. Installing that will also cause CouchbaseNetClient to be installed, but I won't actually be using any Linq2Couchbase until later blog posts).

![NuGet UI for installing CouchbaseNetClient](https://dl.dropboxusercontent.com/u/224582/blogpost4/NuGetUI_001.png)

If you prefer the NuGet command line, then open up the Package Manager Console, and type ```Install-Package CouchbaseNetClient```.

![NuGet Package Manager Console for installing CouchbaseNetClient](https://dl.dropboxusercontent.com/u/224582/blogpost4/NuGetPackageManagerConsole_002.png)

## Getting the ASP.NET app to talk to a Couchbase cluster

Now let's setup the ASP.NET app to be able to connect to Couchbase. The first thing we need to do is locate the Couchbase Cluster. The best place to do this is in the Global.asax.cs when the application starts. At a minimum, we need to specify one node in the cluster, and give that to the ```ClusterHelper```. This only needs to be done once in ```Application_Start```. When the application ends, it's a good idea to close the ```ClusterHelper``` in order to clean up and dispose of resources that aren't needed.

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var config = new ClientConfiguration();
            config.Servers = new List<Uri>
            {
                new Uri("http://localhost:8091")
            };
            config.UseSsl = false;
            ClusterHelper.Initialize(config);
        }

        protected void Application_End()
        {
            ClusterHelper.Close();
        }
    }

Some notes:

- This code assumes that you are running a Couchbase node on your local machine (localhost). If that's not true, then substitute localhost. For instance, I have a Couchbase node running on a different machine in my office, so I would substitute ```new Uri("http://192.168.1.5")```.
- I have UseSsl set to false, because I don't have a certificate running on my Couchbase node. If you are accessing Couchbase over the internet, you can use SSL to prevent your data traffic from being sent in the clear.

## Setting up an IoC container

Once the ClusterHelper is initialized, we can use it to access buckets.

There are many ways you can proceed to wire up dependencies in your app, but I like to use an IoC container. There are many IoC tools available for .NET, but my favorite is [StructureMap](http://structuremap.github.io/). There's another NuGet package that integrates StructureMap with MVC for you. After installing this, MVC Controller objects will be instantiated via StructureMap. Install, with NuGet (UI or console), ```StructureMap.MVC5```.

![Installing StructureMap.MVC5 with NuGet](https://dl.dropboxusercontent.com/u/224582/blogpost4/NuGetPackageManagerConsole2_002b.png)

It will add StructureMap to your project, as well as several other files. One of them is DefaultRegistry.cs, which sets up StructureMap to use default conventions.

What we'll need to do with Couchbase, is to modify that registry so that StructureMap can give us an instance of IBucket. An IBucket, then, is used to interact with a Couchbase bucket (get documents, add documents, update documents, and so on). Here's is how to setup an IBucket registration:

     public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
            // this next 'For' is what I've added for Couchbase
            For<IBucket>().Singleton().Use<IBucket>("Get a Couchbase Bucket",
                x => ClusterHelper.GetBucket("hello-couchbase", "password!"));
        }

        #endregion
    }

In this example:

- I'm using the ClusterHelper to get a specific bucket (which I called 'hello-couchbase', but you can call whatever you want). Make sure that this bucket exists in Couchbase (you can use 'default' or one of the example buckets if you set one up in [part 1 of this blog series](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-1)).
- Putting a password on a bucket is not required, but it's a good idea.
- The IBucket instance is a singleton, because there is no reason to have multiple instances of it.

## Using the IBucket in a controller

Just to show that this works, go ahead and add IBucket to a constructor of a controller, say HomeController. StructureMap has already been setup to instantiate controllers, and we already told it how to instantiate an IBucket. (In the long run, you will probably not want to use IBucket directly in the controller; more on that in future blog posts).

    public class HomeController : Controller
    {
        private readonly IBucket _bucket;

        public HomeController(IBucket bucket)
        {
            _bucket = bucket;
        }
    }

Next, add a document to your bucket, directly in Couchbase Console. Make note of the key you give it.

![Specifying a key for a new document in Couchbase](https://dl.dropboxusercontent.com/u/224582/blogpost4/CouchbaseCreateDocument_003.png)

![Creating a document in Couchbase](https://dl.dropboxusercontent.com/u/224582/blogpost4/CouchbaseCreateDocument_004.png)

Now, add an action to HomeController. This is a throwaway action just for demonstration purposes. It's the simplest thing that can be done: it will get the document based on the key, and write the document values in the response.

        public ActionResult Index()
        {
            var doc = _bucket.Get<dynamic>("foo::123");
            return Content("Name: " + doc.Value.name + ", Address: " + doc.Value.address);
        }

```doc.Value``` is of type ```dynamic```, so make sure that the fields you use (in my case, name and address) match up to the JSON document you put into the bucket. Run your MVC site in a browser, and you should see something like this:

![Outputting the document values to a browser](https://dl.dropboxusercontent.com/u/224582/blogpost4/CouchbaseAspNetHelloWorld_005.png)

Congratulations, you've successfully written an ASP.NET site that uses Couchbase. That wasn't so hard, was it?

## Conclusion

You can view the complete source code for [this example on Github](#).

I've shown you the very basics of connecting to and use Couchbase in ASP.NET MVC. But we can do a lot better. In the next blog post, I'm going to show you how to use Linq2Couchbase to build an entity, a repository, and how to use this repository to make a website with actual functionality. As always, if you need help with anything, please leave a comment, [ping me on Twitter](http://twitter.com/mgroves), or email me (matthew.groves AT couchbase DOT com).