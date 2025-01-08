using Microsoft.AspNetCore.Mvc;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ProductsV2Controller : ControllerBase
{
    /// <summary>
    /// Gets products for API version 2.0.
    /// </summary>
    /// <returns>A message indicating this is version 2.0 of the Products API.</returns>
    [HttpGet]
    public ActionResult<string> GetProducts()
    {
        return "This is version 2.0 of the Products API.";
    }
}
