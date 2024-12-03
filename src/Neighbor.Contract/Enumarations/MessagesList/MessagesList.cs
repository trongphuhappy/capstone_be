namespace Neighbor.Contract.Enumarations.MessagesList;

public enum MessagesList
{
    [Message("This email has been registered", "auth_email_exists")]
    AuthEmailExistException,

    [Message("Registration failed, please register again", "auth_register_failure")]
    AuthRegisterFailure,

    [Message("Registration successful, please check email for confirmation", "auth_register_success")]
    AuthRegisterSuccess,

    [Message("Registration successful, you can now login", "auth_verify_success")]
    AuthVerifyEmailRegister,

    [Message("Your email is not registered", "auth_not_regist")]
    AuthEmailNotExsitException,

    [Message("This account was registered using another method", "auth_regis_another")]
    AuthAccountRegisteredAnotherMethod,

    [Message("Passwords do not match", "auth_password_not_match")]
    AuthPasswordNotMatchException,

    [Message("Your account has been banned", "account_banned")]
    AccountBanned,

    [Message("Logout successfully", "auth_logout_success")]
    AuthLogoutSuccess,

    [Message("Session has expired, please log in again", "auth_login_expired")]
    AuthRefreshTokenNull,

    [Message("Login Google fail, please try again", "auth_noti_09")]
    AuthLoginGoogleFail,

    [Message("All Categories: ", "category_noti_success_01")]
    CategoryGetAllCategoriesSuccess,

    [Message("Can not find any categories!", "category_noti_exception_01")]
    CategoryNotFoundAnyException,
    [Message("Can not find this category!", "category_noti_exception_02")]
    CategoryNotFoundException,
    [Message("Vehicle Product must have insurance and all fields of insurance must not be empty!", "category_noti_exception_03")]
    CategoryMissingInsuranceException,

    [Message("All Surcharges: ", "surcharge_noti_success_01")]
    SurchargeGetAllSurchargesSuccess,

    [Message("Can not find any surcharges!", "surcharge_noti_exception_01")]
    SurchargeNotFoundAnyException,
    [Message("Can not find this surcharge!", "surcharge_noti_exception_02")]
    SurchargeNotFoundException,

    [Message("This user is not a lessor!", "lessor_noti_exception_01")]
    LessorNotFoundException,

    [Message("Create Product Successfully!", "product_noti_success_01")]
    ProductCreateSuccess,

    [Message("Confirm Product Successfully!", "product_noti_success_02")]
    ProductConfirmSuccess,

    [Message("All Products: ", "product_noti_success_03")]
    ProductGetAllSuccess,

    [Message("Product Details: ", "product_noti_success_04")]
    ProductGetDetailsSuccess,

    [Message("All Products in Wishlist: ", "product_noti_success_05")]
    ProductGetAllInWishlistSuccess,

    [Message("Add Product to Wishlist Successfully.", "product_noti_success_06")]
    ProductAddToWishlistSuccess,

    [Message("Remove Product From Wishlist Successfully.", "product_noti_success_07")]
    ProductRemoveFromWishlistlistSuccess,

    [Message("Can not found this Product!", "product_noti_exception_01")]
    ProductNotFoundException,

    [Message("Reject Product must have reason!", "product_noti_exception_02")]
    ProductRejectNoReasonException,

    [Message("Can not find any Products", "product_noti_exception_03")]
    ProductNotFoundAnyException,

    [Message("This Product has already rejected", "product_noti_exception_04")]
    ProductHasAlreadyRejectedException,

    [Message("This Product has already approved", "product_noti_exception_05")]
    ProductHasAlreadyApprovedException,

    [Message("You cannot add a product that belongs to you to your wishlist!", "wishlist_noti_exception_01")]
    WishlistProductBelongsToUserException,

    [Message("Can not find this account!", "account_noti_exception_01")]
    AccountNotFoundException,

    [Message("This Account has already been banned!", "account_noti_exception_02")]
    AccountHasAlreadyBannedException,

    [Message("This Account has already been unbanned!", "account_noti_exception_03")]
    AccountHasAlreadyUnbannedException,

    [Message("Ban User Successfully!", "admin_noti_success_01")]
    AdminBanUserSuccess,

    [Message("Unban User Successfully!", "admin_noti_success_02")]
    AdminUnbanUserSuccess,

    [Message("Ban User must have a reason!", "admin_noti_exception_01")]
    AdminBanWithNoReasonException,

    [Message("Get profile successfully", "member_get_profile_success")]
    GetProfileSuccessfully,

    [Message("This email exists, please wait for another email", "account_update_email_fail")]
    AccountEmailUpdateExit,
    
    [Message("Please check email to change", "account_update_email_success")]
    AccountUpdateChangeEmail,

    [Message("Updated email successfully", "account_noti_update_email_success")]
    AccountUpdateEmailSuccess,

    [Message("Update citizen successfully", "update_citizen_successfully")]
    UpdateCitizenSuccessfully,

    [Message("All Orders: ", "order_noti_success_01")]
    OrderGetAllSuccessfully,

    [Message("Details Order: ", "order_noti_success_02")]
    OrderGetDetailsSuccessfully,

    [Message("You cannot rent a product that belongs to you!", "order_noti_exception_01")]
    OrderProductBelongsToUserException,

    [Message("You have already create an rental order for this product!", "order_noti_exception_02")]
    OrderProductAlreadyOrderedByUserException,

    [Message("This Product is rented by another user!", "order_noti_exception_03")]
    OrderProductOrderedByAnotherUserException,

    [Message("This Product is not approved by Admin!", "order_noti_exception_04")]
    OrderProductNotApprovedByAdminException,

    [Message("Can not find any Orders", "order_noti_exception_05")]
    OrderNotFoundAnyException,

    [Message("Can not find this Order", "order_noti_exception_06")]
    OrderNotFoundException,
}
