using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddOData(opt =>
{
    opt.Select().Filter().Expand().SetMaxTop(1000).Count().OrderBy(); 
    opt.AddRouteComponents("masterdata", GetEdmModel());
}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => opt.DocInclusionPredicate((name, api) => api.HttpMethod != null));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Store>("Stores");
    builder.EnableLowerCamelCase();

    return builder.GetEdmModel();
}

public class Store : Entity
{
    public string ExternalId { get; set; } = string.Empty;
    public string? Name { get; set; }
}

public abstract class Entity
{
    public int Id { get; set; } = 0;
}