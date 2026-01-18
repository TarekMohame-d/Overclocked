namespace Overclocked.Domain.UserAggregate.Enums;

public enum Permission
{
    // === Super Admin ===
    ManageUsers = 1,
    ManageRolePermissions,
    DeactivateUsers, // + all below

    // === Admin ===
    ManageOrders,
    ManageShipments,
    ManagePayments,
    SeeStatistics,
    CreateReports,
    ManageReviews,
    ReplyToReview,
    RefundOrder, // + all below

    // === Data Entry ===
    AddEditDelete,
}
