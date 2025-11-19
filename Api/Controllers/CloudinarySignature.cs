using System.Text.RegularExpressions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Services.CloudinarySignature;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class CloudinarySignature(ICloudinaryService cloudinaryService) : ControllerBase
{
    [HttpGet]
    [Route(CloudinarySignatureRoute.UploadSignature)]
    public IActionResult GenerateSignature([FromQuery] string category)
    {
        if(string.IsNullOrWhiteSpace(category))
            return BadRequest("The 'category' query parameter is required.");

        var sanitizedCategory = Regex.Replace(category.ToLower(), @"[^a-z0-9-]", "");
        if(string.IsNullOrWhiteSpace(sanitizedCategory))
        {
            return BadRequest("The 'category' query parameter contains invalid characters.");
        }

        CloudinarySignatureResponse signatureResponse = cloudinaryService.GenerateUploadSignature(category.ToLower());
        return Ok(signatureResponse);
    }
}
