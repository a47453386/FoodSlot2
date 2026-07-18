using FoodSlot2.Areas.API.DTOs;
using FoodSlot2.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FoodSlot.Areas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleMapsAPIController : ControllerBase
    {
        private readonly IAPIResultService _apiResultService;
        private readonly IConfiguration _configuration;

        public GoogleMapsAPIController(
            IAPIResultService apiResultService, IConfiguration configuration)
        {
            _apiResultService = apiResultService;
            _configuration = configuration;
        }

        /// Nearby Search 測試
        [HttpPost("NearbySearch")]
        public async Task<IActionResult> NearbySearch(
            [FromBody] NearbySearchRequestDTO request)
        {
            try
            {
                var result =
                    await _apiResultService
                        .NearbySearchAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        //取得APIKey
        [HttpGet("GetApiKey")]
        public IActionResult GetApiKey()
        {
            string apiKey = _configuration["GoogleApi:ApiKey"]!;
            return Ok(new { apiKey = apiKey });
        }
    }
}