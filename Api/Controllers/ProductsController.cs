using Api.Helper;
using AutoMapper;
using Core.Constants;
using Core.Dto;
using Core.Interfaces;
using Core.Models;
using Infrastructure.GenericRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Api.Controllers;

[Route("api/products")]
[ApiController]
[Authorize( Policy = RolesConstants.AdministratorOrManager)]

public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper ;
    private readonly IImageService _imageService ;

    public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    [HttpGet("{id}")]
   
    public async Task<ActionResult> GetSingle(int id)
    {
        var product    = await _unitOfWork.Products.GetById(id);
        var productDto = _mapper.Map<ProductDto>(product);

        if (product is null)
          return NotFound("Sorry we could not find the product you are looking for");

        productDto.PhotoUrl = await _imageService.GetImageUrl(productDto.PhotoUrl);
        return Ok(productDto);

    }
    [HttpGet]
    public async Task<ActionResult> GetAllProducts([FromQuery] PaginationParams paginationParams)
    {
        var products = _unitOfWork.Products.GetAllQueryable();

        var paginatedProducts = await ApplyPagination<Product>.CreateAsync(products, paginationParams.PageNumber, paginationParams.PageSize);

        var productDtos = _mapper.Map<List<ProductDto>>(paginatedProducts);
        foreach (var productDto in productDtos)
        {
            productDto.PhotoUrl = await _imageService.GetImageUrl(productDto.PhotoUrl);
        }
        var paginationResponse = new Pagination<ProductDto>(
                                            paginationParams.PageNumber,
                                            paginationParams.PageSize,
                                            products.Count(),
                                            productDtos);                                                            

        return Ok(paginationResponse);
    }
    [HttpPost]
    public  async Task< ActionResult> Add([FromForm] ProductCreateDto productDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var product = _mapper.Map<Product>(productDto);
        product.CreatedBy = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var path = await _imageService.UploadImage(productDto.ImageFile);
        product.PhotoUrl = path;
        product.CreatedDate = DateTime.Now;
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.Complete();
      
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromForm] ProductUpdateDto productDto)
    {
        if (!ModelState.IsValid )
                  return BadRequest(ModelState);

        var product = await _unitOfWork.Products.GetById(id);

        if (product is null)
           return NotFound("Product not found.");
        
        await MapProduct(productDto, product);
        var result = await _unitOfWork.Products.Update(product);

        return result ? NoContent() : BadRequest("Failed to update the product."); ;

    }

   
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id )
    {
        var product = await _unitOfWork.Products.GetById(id);
        var result  = await  _unitOfWork.Products.DeleteAsync(x=>x.Id ==id);
        if (result > 0)
        {
          
            await _imageService.DeleteOlderImage(product.PhotoUrl);
            return NoContent();
        }
        return NotFound("The element with the specified ID was not found.");
    }

    private async Task MapProduct(ProductUpdateDto productDto, Product product)
    {
        product.Price = productDto.Price;
        product.Description = productDto.Description;
        product.Name = productDto.Name;

        if (productDto.ImageFile != null && productDto.ImageFile.Length > 0)
        {
            var newImageUrl = await _imageService.UploadImage(productDto.ImageFile);
            await _imageService.DeleteOlderImage(product.PhotoUrl);
            product.PhotoUrl = newImageUrl;
        }

        //product.CreatedBy = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        product.CreatedBy = User.Claims.First(c => c.Type == ClaimTypes.Role).Value;

        product.UpdatedDate = DateTime.Now;
    }


}
