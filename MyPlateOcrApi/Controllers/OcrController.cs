using Microsoft.AspNetCore.Mvc;
using MyPlateOcrApi.Services;

namespace MyPlateOcrApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OcrController : ControllerBase
    {
        private readonly OcrService _ocrService;

        public OcrController(IWebHostEnvironment env)
        {
            var tessPath = Path.Combine(env.ContentRootPath, "tessdata");
            _ocrService = new OcrService(tessPath);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image provided");

            using var stream = image.OpenReadStream();
            var resultText = _ocrService.RecognizeText(stream);

            return Ok(new { plate = resultText });
        }
    }
}
