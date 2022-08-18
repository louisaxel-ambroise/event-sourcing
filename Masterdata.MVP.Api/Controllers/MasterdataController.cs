using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Masterdata.MVP.Api.Controllers;

public abstract class MasterdataController<T> : ODataController
    where T : Entity
{
    public static List<T> Entities { get; protected set; } = new ();

    [EnableQuery]
    public IQueryable<T> Get()
    {
        return Entities.AsQueryable();
    }

    [EnableQuery]
    public SingleResult<T> Get(int key) => new(Entities.Where(x => x.Id == key).AsQueryable());

    public IActionResult Patch([FromODataUri] int key, [FromBody] Delta<T> delta)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = Entities.SingleOrDefault(x => x.Id == key);

        if (entity == null) return NotFound();

        delta.Patch(entity);

        return Updated(entity);
    }

    public IActionResult Put([FromODataUri] int key, [FromBody] T update)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (key != update.Id) return BadRequest();

        return Updated(update);
    }

    public ActionResult Delete([FromODataUri] int key)
    {
        var entity = Entities.SingleOrDefault(x => x.Id == key);

        if (entity == null) return NotFound();

        return NoContent();
    }
}
