using FirstApii.Data.DAL;
using FirstApii.Dtos.CategoryDtos;
using FirstApii.Dtos.ProductDtos;
using FirstApii.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApii.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var category = _appDbContext.Categories
                 .Where(c => !c.IsDeleted)
                 .FirstOrDefault(c => c.Id == id);
         
            if (category == null) return StatusCode(StatusCodes.Status404NotFound);
            CategoryReturnDto categoryReturnDto = new();
            categoryReturnDto.Name = category.Name;
            categoryReturnDto.Desc = category.Desc;
            categoryReturnDto.ImageUrl= "https://localhost:7110/img/" + category.ImageUrl;
            categoryReturnDto.UpdateDate = category.UpdateDate;
            categoryReturnDto.CreateDate = category.CreateDate;
            return Ok(category);
        }
        [HttpGet]
        public IActionResult GetAll(int page, string search)
        {
            var query = _appDbContext.Categories
                .Where(c => !c.IsDeleted);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c=> c.Name.Contains(search));

            }

            CategoryListDto categoryListDto = new();
            categoryListDto.TotalCount = query.Count();
            categoryListDto.CurrentPage = page;

            categoryListDto.Items = query.Skip((page - 1) * 2)
                .Take(2)
                .Select(c=> new CategoryListItemDto
                {

                    Name= c.Name,
                    Desc= c.Desc,
                    ImageUrl= "https://localhost:7110/img/" + c.ImageUrl,
            CreateDate = c.CreateDate,
                    UpdateDate= c.UpdateDate,


                }).ToList();

            List<CategoryListItemDto> listItemDtos = new();


            return StatusCode(200, categoryListDto);
           
        }
        [HttpPost]
        public IActionResult AddCategory(CategoryCreateDto categoryCreateDto)
        {
            Category newCategory = new()
            {
                Name = categoryCreateDto.Name,
                Desc= categoryCreateDto.Desc,
                ImageUrl=""
            
            };

            _appDbContext.Categories.Add(newCategory);
            _appDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, newCategory);
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

    }
}