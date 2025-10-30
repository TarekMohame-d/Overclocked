using System.Text.RegularExpressions;
using Api.Routing;
using Application.Abstraction.Services;
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
            return BadRequest("The 'category' query parameter is required.");

        var sanitizedCategory = Regex.Replace(category.ToLower(), @"[^a-z0-9-]", "");
        if (string.IsNullOrWhiteSpace(sanitizedCategory))
            return BadRequest("The 'category' query parameter contains invalid characters.");

        var signatureResponse = _cloudinaryService.GenerateUploadSignature(category.ToLower());
        return Ok(signatureResponse);
    }
}
