using Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Api.ActionFilters;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        // Check if the user has a claim of type "permission"
        // with the value equal to the required permission string.
        var hasPermission = context.User.HasClaim(claim =>
            claim.Type == ClaimsConstants.Permission && claim.Value == requirement.Permission
        );

        if(hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
