using API.DTOs.Product;
using API.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string keyword = null,
      [FromQuery] int? categoryId = null,
     [FromQuery] decimal? minPrice = null,
     [FromQuery] decimal? maxPrice = null)
        {
            var result = await _productService.GetPagedAsync(page, pageSize, keyword, minPrice, maxPrice, categoryId);
            return Ok(result);
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDTO dto)
        {
            var result = await _productService.CreateAsync(dto);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.SanPhamId }, result);
        }

		// PUT: api/Product/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDTO dto)
		{
			var result = await _productService.UpdateAsync(id, dto);
			if (!result)
				return NotFound("Không tìm thấy sản phẩm để cập nhật");
			return Ok("Cập nhật thành công");
		}

		// DELETE: api/Product/{id}
		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
        // GET: api/Product/name?ten=abc
        [HttpGet("name")]
        public async Task<IActionResult> SearchByName([FromQuery] string ten)
        {
            var result = await _productService.SearchByNameAsync(ten);
            return Ok(result);
        }
        // GET: api/Product/filter-by-price?min=1000&max=2000
        [HttpGet("filter-by-price")]
        public async Task<IActionResult> FilterByPrice([FromQuery] decimal min, [FromQuery] decimal max)
        {
            var result = await _productService.FilterByPriceAsync(min, max);
            return Ok(result);
        }


    }
}
