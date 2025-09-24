using Api.Routing;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class CloudinarySignature : ControllerBase
{
    private readonly ICloudinaryService _cloudinaryService;

    public CloudinarySignature(ICloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    [Route(CloudinarySignatureRoute.Generate)]
    public IActionResult GenerateSignature([FromQuery] string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return BadRequest("The 'category' query parameter is required.");
        }

        var signatureResponse = _cloudinaryService.GenerateUploadSignature(category);
        return Ok(signatureResponse);
    }
}
