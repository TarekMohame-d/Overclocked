namespace Domain.StaticData;

public enum PermissionType
{
    // === Super Admin ===
    SuperAdmin = 1, // + all below

    // === Admin ===
    ManageOrders,
    ManageShipments,
    ManagePayments,
    SeeStatistics,
    CreateReports,
    ManageRolePermissions,
    ManageUserRoles,
    ManageCustomers,
    DeactivateUsers,
    ManageReviews,
    ReplyToReview,
    RefundOrder,
    DeleteInvoices, // + all below

    // === Data Entry ===
    AddEditDeleteProducts,
    AddEditDeleteBrands,
    AddEditDeleteCategories,
    AddEditDeleteTags,

    // === Manager ===
    RefundInvoices, // + all below

    // === Employee ===
    AddEditInvoices,
    ViewProducts
}
