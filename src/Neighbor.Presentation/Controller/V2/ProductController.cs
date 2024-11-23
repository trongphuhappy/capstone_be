using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Services.Products;
using Neighbor.Presentation.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;
using static Neighbor.Contract.Services.Products.Filter;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class ProductController : ApiController
{
    public ProductController(ISender sender) : base(sender)
    { }

    //[Authorize]
    [HttpPost("create_product", Name = "CreateProduct")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> CreateProduct([FromForm] ProductDTO.ProductRequestDTO productRequestDTO)
    {
        //var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var userId = Guid.Parse("8FD64A3D-C603-4663-9C37-CC3C3562D55F");
        var result = await Sender.Send(new Command.CreateProductCommand(productRequestDTO.Name, productRequestDTO.Description, productRequestDTO.Value, productRequestDTO.Price, productRequestDTO.MaximumRentDays, productRequestDTO.Policies, productRequestDTO.CategoryId, userId, productRequestDTO.ProductImages, new InsuranceDTO.InsuranceRequestDTO()
        {
            Name = productRequestDTO.InsuranceName,
            IssueDate = productRequestDTO.IssueDate,
            ExpirationDate = productRequestDTO.ExpirationDate,
            InsuranceImages = productRequestDTO.InsuranceImages
        }, new SurchargeDTO.SurchargeRequestDTO()
        {
           SurchargeId = productRequestDTO.SurchargeId,
           Price = productRequestDTO.SurchargePrice,
        }));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpGet("get_all_products", Name = "GetAllProducts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilter filterParams,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string[] selectedColumns = null)
    {
        var result = await Sender.Send(new Query.GetAllProductsQuery(pageIndex, pageSize, filterParams, selectedColumns));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    //[HttpGet("get_all_products_by_user", Name = "GetAllProductsByUser")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> GetAllProductsByUser([FromBody] Command.RegisterCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}

    //[HttpGet("get_all_products_by_admin", Name = "GetAllProductsByAdmin")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> GetAllProductsByAdmin([FromBody] Command.RegisterCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}

    //[HttpGet("get_all_products_by_lessor", Name = "GetAllProductsByLessor")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> GetAllProductsByLessor([FromBody] Command.RegisterCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}

    [HttpGet("get_product_by_id", Name = "GetProductById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetProductById([FromQuery] Guid Id)
    {
        var result = await Sender.Send(new Query.GetProductByIdQuery(Id));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    [HttpPut("confirm_product", Name = "ConfirmProduct")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> ConfirmProduct([FromBody] Command.ConfirmProductCommand commands)
    {
        var result = await Sender.Send(commands);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}