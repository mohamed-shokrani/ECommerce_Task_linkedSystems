using Api.Helper;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Dto;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[Route("api/products")]
[ApiController]
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
    public async Task<ActionResult> Add(ProductDto productDto)
    {
        
        var product = new Product
        {
            Description = productDto.Description,
            Name = productDto.Name,
            PhotoUrl = productDto.PhotoUrl,
            Price = productDto.Price,
            CreatedBy = "ssss",
            CreatedDate = DateTime.Now,
        };
        try
        {
             await _unitOfWork.Products.AddAsync(product);
           var res=await _unitOfWork.Complete();
        }
        catch (Exception e)
        {

            throw;
        }
        return Ok();
    }

}
