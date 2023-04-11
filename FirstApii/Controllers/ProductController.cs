using FirstApii.Data.DAL;
using FirstApii.Dtos.ProductDtos;
using FirstApii.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FirstApii.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult GetAll(int page,string search)
        {
            var query = _appDbContext.Products
                .Where(p => !p.IsDelete);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search));

            }

            ProductListDto productListDto = new();
            productListDto.TotalCount = query.Count();
            productListDto.CurrentPage= page;
               
            productListDto.Items =query.Skip((page-1)*2)
                .Take(2)
                .Select(p => new ProductListItemDto
            {

                Name = p.Name,
                CostPrice = p.CostPrice,
                SalePrice = p.SalePrice,
                CreateDate = p.CreateDate,
                UpdateDate = p.UpdateDate


            }).ToList();

            List<ProductListItemDto> listItemDtos = new();


            return StatusCode(200, productListDto);
            //return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            Product product = _appDbContext.Products
                 .Where(p => !p.IsDelete)
                 .FirstOrDefault(p => p.Id == id);
            if (product == null) return StatusCode(StatusCodes.Status404NotFound);
            ProductReturnDto productReturnDto = new()
            {
                Name = product.Name,
                SalePrice = product.SalePrice,
                CostPrice = product.CostPrice,
                CreateDate = product.CreateDate,
                UpdateDate = product.UpdateDate,

            };

            return Ok(productReturnDto);
            //return StatusCode(200, products[0]);
        }
        [HttpPost]
        public IActionResult AddProduct(ProductCreateDto productCreateDto)
        {
            Product newProduct = new()
            {
                Name = productCreateDto.Name,
                SalePrice = productCreateDto.SalePrice,
                CostPrice = productCreateDto.CostPrice,
                IsActive = productCreateDto.IsActive,
                IsDelete= productCreateDto.IsDelete,
            };

            _appDbContext.Products.Add(newProduct);
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, newProduct);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _appDbContext.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            _appDbContext.Products.Remove(product);
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);

        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, ProductUpdateDto productUpdate)
        {
            var existProduct = _appDbContext.Products.FirstOrDefault(p => p.Id == id);
            if (existProduct == null) return NotFound();
            existProduct.Name = productUpdate.Name;
            existProduct.SalePrice = productUpdate.SalePrice;
            existProduct.CostPrice = productUpdate.CostPrice;
            existProduct.IsActive = productUpdate.IsActive;
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }
        [HttpPatch]
        public IActionResult ChangeStatus(int id, bool IsActive)
        {
            var existProduct = _appDbContext.Products.FirstOrDefault(p => p.Id == id);
            if (existProduct == null) return NotFound();
            existProduct.IsActive = IsActive;
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status204NoContent);
        }

    }
}
