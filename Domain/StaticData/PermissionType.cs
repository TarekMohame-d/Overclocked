namespace Domain.StaticData;

public enum PermissionType
{
    // === Super Admin ===
    SuperAdmin = 1,
    ManageUsers,
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
    AddEditDelete
}
