using API.Data;
using API.DTOs.Product;
using API.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class InventoryController : ControllerBase
	{
		private readonly IInventoryService _inventoryService;
		private readonly ApplicationDbContext _context;

		public InventoryController(IInventoryService inventoryService, ApplicationDbContext context)
		{
			_inventoryService = inventoryService;
			_context = context;
		}

		// ✅ Nhập hàng
		[HttpPost("import-with-batch")]
		public async Task<IActionResult> ImportWithBatch([FromForm] ImportProductRequest dto)
		{
			var result = await _inventoryService.ImportWithBatchAsync(dto);
			if (!result)
				return BadRequest("Nhập hàng thất bại.");

			return Ok("Nhập hàng theo lô thành công.");
		}

		// ✅ Xuất hàng
		[HttpPost("export-with-batch")]
		public async Task<IActionResult> ExportWithBatch([FromBody] ExportProductRequest dto)
		{
			var result = await _inventoryService.ExportWithBatchAsync(dto);
			if (!result)
				return BadRequest("Xuất hàng thất bại hoặc không đủ hàng.");

			return Ok("Xuất hàng thành công.");
		}

		// ✅ Lấy tất cả lịch sử
		[HttpGet("history")]
		public async Task<ActionResult<List<InventoryHistoryDTO>>> GetHistory()
		{
			var history = await _inventoryService.GetHistoryAsync();
			return Ok(history);
		}

		// ✅ Lấy lịch sử nhập hàng
		[HttpGet("history/imports")]
		public async Task<ActionResult<List<InventoryHistoryDTO>>> GetImportHistory()
		{
			var importHistory = await _inventoryService.GetImportHistoryAsync();
			return Ok(importHistory);
		}


		// ✅ Lịch sử xuất hàng
		[HttpGet("history/exports")]
		public async Task<ActionResult<List<InventoryHistoryDTO>>> GetExportHistory()
		{
			var exportHistory = await _inventoryService.GetExportHistoryAsync();
			return Ok(exportHistory);
		}
		// ✅ Cập nhật giá bán sau khi nhập hàng
		[HttpPut("update-selling-price")]
		public async Task<IActionResult> UpdateSellingPrice([FromBody] UpdateSellingPriceRequest dto)
		{
			var success = await _inventoryService.UpdateSellingPriceAsync(dto);
			if (!success)
				return NotFound("Không tìm thấy sản phẩm.");

			return Ok("Cập nhật giá bán thành công.");
		}
	}
}
