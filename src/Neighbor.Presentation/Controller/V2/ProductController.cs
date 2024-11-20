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
        var userId = Guid.Parse("D966AF08-7609-41FE-9866-357E556CA9E5");
        var result = await Sender.Send(new Command.CreateProductCommand(productRequestDTO.Name, productRequestDTO.Description, productRequestDTO.Value, productRequestDTO.Price, productRequestDTO.Policies, productRequestDTO.CategoryId, userId, productRequestDTO.ProductImages, new InsuranceDTO.InsuranceRequestDTO()
        {
            Name = productRequestDTO.InsuranceName,
            Description = productRequestDTO.InsuranceDescription,
            IssueDate = productRequestDTO.IssueDate,
            ExpirationDate = productRequestDTO.ExpirationDate,
            InsuranceImages = productRequestDTO.InsuranceImages
        }, productRequestDTO.Surcharges));
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

    //[HttpGet("get_product_by_id", Name = "GetProductById")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> GetProductById([FromBody] Command.RegisterCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}

    //[HttpPut("confirm_product", Name = "ConfirmProduct")]
    //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    //[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    //public async Task<IActionResult> ConfirmProduct([FromBody] Command.RegisterCommand commands)
    //{
    //    var result = await Sender.Send(commands);
    //    if (result.IsFailure)
    //        return HandlerFailure(result);

    //    return Ok(result);
    //}
}