using FoodSlot2.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using FoodSlot2.Areas.API.DTOs;

namespace FoodSlot.Areas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoreSearchController : ControllerBase
    {
        private readonly IAPIResultService _apiResultService;

        public StoreSearchController(
            IAPIResultService apiResultService)
        {
            _apiResultService = apiResultService;
        }

        [HttpPost("Nearby")]
        public async Task<IActionResult> Nearby(
            [FromBody] NearbySearchRequestDTO request)
        {
            var result =
                await _apiResultService
                    .NearbySearchAsync(request);

            return Ok(result);
        }
    }
}