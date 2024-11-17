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
}
