using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class User : Entity
{
    public int RoleId { get; private set; }
    public RoleType RoleType
    {
        get => (RoleType)RoleId;
        set => RoleId = (int)value;
    }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public required string PasswordHash { get; set; }
    public required string Phone { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Role? Role { get; set; }
    public ICollection<Address>? Addresses { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public EmailConfirmationCode? EmailConfirmationCode { get; set; }
    public Cart? Cart { get; set; }
    public Wishlist? Wishlist { get; set; }
    public ICollection<Review> Reviews { get; set; } = [];

    // Employee navigation property
    public ICollection<EmployeeActivityLog> ActivityLogs { get; set; } = [];
    public ICollection<ReviewReply> ReviewReplies { get; set; } = [];
}
