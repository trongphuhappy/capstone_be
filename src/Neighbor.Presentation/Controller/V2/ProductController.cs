using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighbor.Contract.Abstractions.Shared;
using Neighbor.Contract.DTOs.ProductDTOs;
using Neighbor.Contract.Services.Products;
using Neighbor.Presentation.Abstractions;
using System.Security.Claims;
using static Neighbor.Contract.Services.Products.Filter;

namespace Neighbor.Presentation.Controller.V2;

[ApiVersion(2)]
public class ProductController : ApiController
{
    public ProductController(ISender sender) : base(sender)
    { }

    [Authorize]
    [HttpPost("create_product", Name = "CreateProduct")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> CreateProduct([FromForm] ProductDTO.ProductRequestDTO productRequestDTO)
    {
        var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var result = await Sender.Send(new Command.CreateProductCommand(productRequestDTO.Name, productRequestDTO.Description, productRequestDTO.Value, productRequestDTO.Price, productRequestDTO.MaximumRentDays, productRequestDTO.Policies, productRequestDTO.CategoryId, userId, productRequestDTO.ProductImages, new InsuranceDTO.InsuranceRequestDTO()
        {
            Name = productRequestDTO.InsuranceName,
            IssueDate = productRequestDTO.IssueDate,
            ExpirationDate = productRequestDTO.ExpirationDate,
            InsuranceImages = productRequestDTO.InsuranceImages
        }, productRequestDTO.ListSurcharges));
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
    public async Task<IActionResult> GetProductById([FromQuery] Query.GetProductByIdQuery Queries)
    {
        var result = await Sender.Send(Queries);
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

    //[Authorize]
    [HttpPut("add_to_wishlist", Name = "AddToWishlist")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> AddToWishlist([FromQuery] Guid ProductId)
    {
        //var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var userId = Guid.Parse("5F7659FA-43C8-4A0B-B993-D00FD9D91C43");
        var result = await Sender.Send(new Command.AddToWishlistCommand(userId, ProductId));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }

    //[Authorize]
    [HttpGet("get_all_products_from_wishlist", Name = "GetAllProductsFromWishlist")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<Success>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Result<Error>))]
    public async Task<IActionResult> GetAllProductsFromWishlist([FromQuery] ProductWishlistFilter filterParams,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string[] selectedColumns = null)
    {
        //var userId = Guid.Parse(User.FindFirstValue("UserId"));
        var userId = Guid.Parse("5F7659FA-43C8-4A0B-B993-D00FD9D91C43");
        var result = await Sender.Send(new Query.GetAllProductsInWishlistQuery(userId, pageIndex, pageSize, filterParams, selectedColumns));
        if (result.IsFailure)
            return HandlerFailure(result);

        return Ok(result);
    }
}