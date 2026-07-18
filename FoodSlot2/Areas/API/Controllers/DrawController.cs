using FoodSlot2.Services.Interfaces;
using FoodSlot2.ViewModels.Slot;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodSlot.Areas.API.Controllers
{
    [Area("API")]
    [Route("API/[controller]/[action]")]
    [ApiController]
    public class DrawController : ControllerBase
    {
        private readonly IDrawService _drawService;
        public DrawController(IDrawService drawService)
        {
            _drawService = drawService;
        }

        [HttpPost]
        public async Task<IActionResult> StartDraw()
        {
            // 未登入時保持 null，Service 會自動改用預設 userID = 1
            int? userID = null;

            //判斷是否登入，有登入時，取得登入者的userID字串
            if (User.Identity?.IsAuthenticated == true)
            {
                string? userIDString =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //userIDString 有值時，將字串轉成 int
                if (!string.IsNullOrWhiteSpace(userIDString))
                {
                    userID = int.Parse(userIDString);
                }
            }

            //將DrawAsync()執行結果存到result
            VMSlotDrawResult result =
                await _drawService.DrawAsync(userID);

            return Ok(result);
        }

        //獎池列表API
        [HttpGet]
        public async Task<IActionResult> GetPrizePool()
        {
            int? userID = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                string? userIDString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrWhiteSpace(userIDString))
                {
                    userID = int.Parse(userIDString);
                }
            }

            try
            {
                // 呼叫讀取獎池的服務
                List<VMFoodSlotItem> result = await _drawService.GetFoodsPoolAsync(userID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}