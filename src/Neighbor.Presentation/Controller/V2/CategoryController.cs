using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.Services.Categories;
using Neighbor.Presentation.Abstractions;
using static Neighbor.Contract.Services.Categories.Filter;

namespace Neighbor.Presentation.Controller.V2;

public class CategoryController : ApiController
{
    public CategoryController(ISender sender) : base(sender)
    { }

    [HttpPost("get_all_categories", Name = "GetAllCategories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetAllCategories([FromQuery] CategoryFilter filterParams,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string[] selectedColumns = null)
    {
        var result = await Sender.Send(new Query.GetAllCategoriesQuery(pageIndex, pageSize, filterParams, selectedColumns));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}