using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CloudinaryController(ICloudinaryService cloudinaryService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [Route(CloudinarySignatureRoute.UploadSignature)]
    public IActionResult GenerateSignature([FromQuery] string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return BadRequest("The 'category' request parameter is required.");

        var sanitizedCategory = Regex.Replace(category.ToLower(), "[^a-z0-9-]", string.Empty);
        if (string.IsNullOrWhiteSpace(sanitizedCategory))
            return BadRequest("The 'category' request parameter contains invalid characters.");

        CloudinarySignatureResponse signatureResponse = cloudinaryService.GenerateUploadSignature(category.ToLower());
        return Ok(signatureResponse);
    }
}
