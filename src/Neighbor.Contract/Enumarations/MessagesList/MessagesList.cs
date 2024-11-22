﻿namespace Neighbor.Contract.Enumarations.MessagesList;

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

    [Message("Can not found this Product!", "product_noti_exception_01")]
    ProductNotFoundException,

    [Message("Reject Product must have reason!", "product_noti_exception_02")]
    ProductRejectNoReasonException,

    [Message("Can not find any Products", "product_noti_exception_03")]
    ProductNotFoundAnyException,

    [Message("Get profile successfully", "member_get_profile_success")]
    GetProfileSuccessfully,
}
