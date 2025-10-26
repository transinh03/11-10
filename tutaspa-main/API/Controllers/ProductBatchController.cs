using API.DTOs.Product;

using API.IService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductBatchController : ControllerBase
	{
		private readonly IProductBatchService _service;

		public ProductBatchController(IProductBatchService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] ProductBatchCreateDTO dto)
		{
			var success = await _service.CreateBatchAsync(dto);
			if (!success)
				return NotFound("Không tìm thấy sản phẩm để thêm lô hàng");

			return Ok("Thêm lô hàng thành công");
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _service.GetAllBatchesAsync();
			return Ok(result);
		}

		[HttpGet("product/{productId}")]
		public async Task<IActionResult> GetByProduct(int productId)
		{
			var result = await _service.GetBatchesByProductAsync(productId);
			return Ok(result);
		}
	}
}
