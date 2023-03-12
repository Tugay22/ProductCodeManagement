using Microsoft.AspNetCore.Mvc;
using ProductCodeManagement.Services.Abstract;

namespace ProductCodeManagement.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class ProductCodeController : Controller
    {
        private readonly IProductCodeService _productCodeService;

        public ProductCodeController(IProductCodeService productCodeService)
        {
            this._productCodeService = productCodeService;
        }

        [HttpPost("GenerateCode")]
        public IActionResult GenerateCode(int count)
        {
            List<string> result = _productCodeService.GenerateCode(count);
            return Ok(result);
        }

        [HttpGet("CheckCode/{Code}")]
        public IActionResult CheckCode(string code)
        {
            string result = _productCodeService.CheckCode(code);
            return Ok(result);
        }
    }
}
