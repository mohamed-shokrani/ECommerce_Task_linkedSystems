using Api.Helper;
using AutoMapper;
using Core.Constants;
using Core.Dto;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[Route("api/products")]
[ApiController]
//[Authorize(AuthenticationSchemes = "Bearer", Roles = RolesConstants.User)]

public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper ;

    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("{id}")]
    [Authorize (AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult> GetSingle(int id)
    {
        var product = await _unitOfWork.Products.GetById(id);
        var productDto = _mapper.Map<ProductDto>(product);
        return product is not null ? Ok(productDto) 
                                   : NotFound("Sorry we could not find the product you are looking for");

    }
    [HttpGet]
    public async Task<ActionResult> GetAllProducts([FromQuery] PaginationParams paginationParams)
    {
        var products = _unitOfWork.Products.GetAllQueryable();

        var paginatedProducts = await ApplyPagination<Product>.CreateAsync(products, paginationParams.PageNumber, paginationParams.PageSize);

        var productDtos = _mapper.Map<List<ProductDto>>(paginatedProducts);

        var paginationResponse = new Pagination<ProductDto>(
                                            paginationParams.PageNumber,
                                            paginationParams.PageSize,
                                            products.Count(),
                                            productDtos);                                                            

        return Ok(paginationResponse);
    }
    [HttpPost]
    public  ActionResult Add(ProductDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
       var product = _mapper.Map<Product>(productDto);
        product.UpdatedBy = "sss";
        product.CreatedDate = DateTime.Now;
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Update(int id,[FromBody] ProductUpdateDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var product =await _unitOfWork.Products.GetById(id);
        product.PhotoUrl = productDto.PhotoUrl;
        product.Price = productDto.Price;
        product.Description = productDto.Description;
        product.Name  = productDto.Name;
        product.UpdatedBy = "sss";
        product.UpdatedDate = DateTime.Now;
        var result=  await _unitOfWork.Products.Update(id,product);

        return result ? NoContent(): NotFound("The element with the specified ID was not found.");
        
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id )
    {
       var result=await  _unitOfWork.Products.DeleteAsync(x=>x.Id ==id);
        return result > 0 ? NoContent()
                          : NotFound("The element with the specified ID was not found.");
    }

}
