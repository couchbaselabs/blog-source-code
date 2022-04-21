using Couchbase.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// tag::addservice[]
builder.Services.AddCouchbase(x =>
{
    x.ConnectionString = "couchbases://" + "cb.abc123foo456.cloud.couchbase.com";
    x.UserName = "svc-wishlist";
    x.Password = "TOP-secret-123!";
    x.HttpIgnoreRemoteCertificateMismatch = true;
    x.KvIgnoreRemoteCertificateNameMismatch = true;
});
// end::addservice[]

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// tag::cleanup[]
app.Lifetime.ApplicationStopped.Register(() =>
{
    app.Services.GetService<ICouchbaseLifetimeService>()?.Close();
});
// end::cleanup[]
