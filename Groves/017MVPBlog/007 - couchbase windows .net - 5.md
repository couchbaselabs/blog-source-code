# Couchbase with Windows and .NET - Part 5 - ASP.NET CRUD

- [Part 1 covered how to install and setup Couchbase on Windows](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-1)
- [Part 2 covered some Couchbase lingo that you'll need to know](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-2)
- [Part 3 showed the very simplest example of using Couchbase in ASP.NET](http://blog.couchbase.com/2016/may/couchbase-with-windows-and-.net---part-3---asp.net-mvc)
- [In Part 4, I did some refactoring and introduced Linq2Couchbase](http://blog.couchbase.com/2016/may/couchbase-with-windows-.net-part-4-linq2couchbase)

This is the last blog post in this series introducing [Couchbase](http://developer.couchbase.com/?utm_source=blogs&utm_medium=link&utm_campaign=blogs) to ASP.NET developers (or vice-versa, depending on your point of view :). I'll be rounding out the sample app that I've been building with a full suite of CRUD functionality. The app already shows a list of people. After this post, you'll be able to add a new person via the web app (instead of directly in Couchbase Console), edit a person, and delete a person.

Before I start, a disclaimer. I've made some modeling *decisions* in this sample app. I've decided that keys to Person documents should be of the format "Person::{guid}", and I've decided that I will enforce the "Person::" prefix at the repository level. I've also made a decision not to use any intermediate view models or edit models in my MVC app, for the purposes of a concise demonstration. By no means do you have to make the same decisions I did! I encourage you to think through the implications for your particular use case, and please feel free to discuss the merits and trade-offs in the comments.

## Adding a new person document

In the previous blog posts, I added new documents through the Couchbase Console. Now let's make it possible via a standard HTML form on an ASP.NET page.

First, I need to make a slight change to the Person class:

    [DocumentTypeFilter("Person")]
    public class Person
    {
        public Person() { Type = "Person"; }

        [Key]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; }
    }

I added an "Id" field, and marked it with the ```[Key]``` attribute. This attribute comes from System.ComponentModel.DataAnnotations, but Linq2Couchbase interprets it to mean "use this field for the Couchbase key".

Now, let's add a very simple new action to HomeController:

    public ActionResult Add()
    {
        return View("Edit", new Person());
    }

And I'll like to that with the bootstrap navigation (which I snuck in previously, and by no means are you required to use):

    <ul class="nav navbar-nav">
        <li><a href="/">Home</a></li>
        <li>@Html.ActionLink("Add Person", "Add", "Home")</li>
    </ul>

Nothing much out of the ordinary so far. I'll create a simple Edit.cshtml with a straightforward, plain-looking form.

	@model CouchbaseAspNetExample3.Models.Person

	@{
	    ViewBag.Title = "Add : Couchbase & ASP.NET Example";
	}
	
	@using (Html.BeginForm("Save", "Home", FormMethod.Post))
	{
	    <p>
	        @Html.LabelFor(m => m.Name)
	        @Html.TextBoxFor(m => m.Name)
	    </p>
	
	    <p>
	        @Html.LabelFor(m => m.Address)
	        @Html.TextBoxFor(m => m.Address)
	    </p>
	
	    <input type="submit" value="Submit" />
	}

Since that form will be POSTing to a Save action, that needs to be created next:

    [HttpPost]
    public ActionResult Save(Person model)
    {
        _personRepo.Save(model);
        return RedirectToAction("Index");
    }

Notice that the Person type used in the parameter is the same type as before. Here is where a more complex web application would probably want to use an edit model, validation, mapping, and so on. I've omitted all of that, and I send the model straight to a new method in PersonRepository:

    public void Save(Person person)
    {
        // if there is no ID, then assume this is a "new" person
        // and assign an ID
        if (string.IsNullOrEmpty(person.Id))
            person.Id = "Person::" + Guid.NewGuid();

        _context.Save(person);
	}

This repository method will set the ID, if one isn't already set (it will be, when we cover 'Edit' later). The "Save" method on IBucketContext is from Linq2Couchbase. It will add a new document if the key doesn't exist, or update an existing document if it does. It's known as an "upsert" operation. In fact, I can do nearly the same thing without Linq2Couchbase:

    var doc = new Document<Person>
    {
        Id = "Person::" + person.Id,
        Content = person
    };
    _bucket.Upsert(doc);

## Editing an existing person document

Now, I want to be able to edit an existing person document in my ASP.NET site. First, let's add an edit link to each person, by making a change to _person.cshtml partial view.

    <h2 class="panel-title">
        @Model.Name
        @Html.ActionLink("[Edit]", "Edit", new {id = Model.Id.Replace("Person::", "")})
        @Html.ActionLink("[Delete]", "Delete", new {id = Model.Id.Replace("Person::", "")}, new { @class="deletePerson"})
    </h2>

Again, I'm using bootstrap here, which is not required. I also added a "delete" link while I was in there, which we'll get to later. One more thing to point out: when creating the routeValues, I stripped out "Person::" from the Id. If I don't do this, ASP.NET will complain about a potentially malicious HTTP request. It would probably be better to give each person a document a more friendly ["slug"](https://en.wikipedia.org/wiki/Semantic_URL#Slug) to use in the URL, and maybe to use that as the document key too. That's going to depend on you and your use case.

Now I need an Edit action in HomeController:

    public ActionResult Edit(Guid id)
    {
        var person = _personRepo.GetPersonByKey(id);
        return View("Edit", person);
    }

I'm reusing the same Edit.cshtml view, but now I need to add a hidden field to hold the document ID.

    <input type="hidden" name="Id" value="@Model.Id"/>

Voila! Now you can add and edit person documents.

![Add or Edit a Person document](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/007pics/EditCouchbaseDocument_001.png)

This may not be terribly impressive to those of you already comfortable with ASP.NET MVC. So, next, let's look at something cool that a NoSQL database like Couchbase brings to the table.

## Iterating on the data stored in the person document

I want to collect more information about a Person. Let's say I want to get a phone number, and a list of that person's favorite movies. With a relational database, that means that I would need to add *at least* two columns, and more likely, at least one other table to hold the movies, with a foreign key.

With Couchbase, there is no explicit schema. So instead, all I have to do is add a couple more properties to the Person class.

    [DocumentTypeFilter("Person")]
    public class Person
    {
        public Person() { Type = "Person"; }

        [Key]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; }
        
        public string PhoneNumber { get; set; }
        public List<string> FavoriteMovies { get; set; }
    }

I also need to add a corresponding UI. I used a bit of jQuery to allow the user to add any number of movies. I won't show the code for it here, because the implementation details aren't important. But I have made the whole [sample available on Github](https://github.com/couchbaselabs/couchbase-asp-net-blog-example-3), so you can check it out later if you'd like.

![Iteration on Person with new UI form](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/007pics/EditPersonIteration_002.png)

I also need to make changes to the _person.cshtml to (conditionally) display the extra information:

    <div class="panel-body">
        @Model.Address
        @if (!string.IsNullOrEmpty(Model.PhoneNumber))
        {
            <br />
            @Model.PhoneNumber
        }
        @if (Model.FavoriteMovies != null && Model.FavoriteMovies.Any())
        {
            <br/>
            <h4>Favorite Movies</h4>
            <ul>
                @foreach (var movie in Model.FavoriteMovies)
                {
                    <li>@movie</li>
                }
            </ul>
        }
    </div>

And here's how that would look (this time with two Person documents):

![Iteration on Person with new UI display](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/007pics/IterationDisplayUI_003.png)

I didn't have to migrate a SQL schema. I didn't have to create any sort of foreign key relationship. I didn't have to setup any OR/M mappings. I simply added a couple of new fields, and Couchbase turned it into a corresponding JSON document.

![Iteration on Person with new JSON document](https://dl.dropboxusercontent.com/u/224582/Couchbase%20Blog%20Posts/007pics/IterationCouchbaseDocument_004.png)

## Deleting a person document

I already added the "Delete" link, so I just need to create a new Controller action...

    public ActionResult Delete(Guid id)
    {
        _personRepo.Delete(id);
        return RedirectToAction("Index");
    }

...and a new repository method:

    public void Delete(Guid id)
    {
        _bucket.Remove("Person::" + id);
    }

Notice that this method is not using Linq2Couchbase. It's using the Remove method on IBucket. There is a Remove available on IBucketContext, but you need to pass it an object, and not just a key. I elected to use the IBucket, but there's nothing inherently superior about it.

## Wrapping up

Thanks for reading through this blog series. Hopefully, you're on your way to considering or even including Couchbase in your next ASP.NET project. Here are some more interesting links for you to continue your Couchbase journey:

- You might be interested in the [ASP.NET Identity Provider for Couchbase](http://blog.couchbase.com/2015/july/the-couchbase-asp.net-identity-storage-provider-part-1) ([github](https://github.com/couchbaselabs/couchbase-aspnet-identity)). If you want to store identity information in Couchbase, this is one way you could do it. At the time of this blog post, it's an early developer preview, and is missing support for social logins.
- Linq2Couchbase is a great project with a lot of features and documentation, but it's still a work in progress. If you are interested, I suggest visiting [Linq2Couchbase on Github](https://github.com/couchbaselabs/Linq2Couchbase). Ask questions on Gitter, and feel free to submit issues or pull requests.
- Right now, [Couchbase is giving away a $500 prize](http://blog.couchbase.com/2016/may/couchbase-4.5-application-contest-chance-to-win-usd500-amazon-gift-card) for a creative use of Couchbase. Iterate on this ASP.NET example? Create a plugin for your favorite IDE? Implement that missing functionality in the ASP.NET Identity Provider? You'll get some swag just for entering.

## Conclusion

I've put the [full source code for this example on Github](https://github.com/couchbaselabs/couchbase-asp-net-blog-example-3). 

What did I leave out? What's keeping you from trying Couchbase with ASP.NET today? Please leave a comment, [ping me on Twitter](http://twitter.com/mgroves), or email me (matthew.groves AT couchbase DOT com). I'd love to hear from you.