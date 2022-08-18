namespace Masterdata.MVP.Api.Controllers;

public class StoresController : MasterdataController<Store>
{
    static StoresController()
    {
        Entities = new List<Store> { new Store { Id = 1, ExternalId = "S1", Name = "Test" }, new Store { Id = 2, ExternalId = "S2", Name = "Other" } };
    }
}
