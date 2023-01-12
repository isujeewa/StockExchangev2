namespace AuthServer.ViewModels
{
    public class ForgotPwdConfirmViewModel
    {
        public string Token { get; set; }

        public string NewPassword { get; set; }
    }

    public class AccountConfirmationViewModel
    {
        public string Token { get; set; }
    }

    public class VerficationViewModel
    {
        public string UserId { get; set; }
        public int? NumberOfVerificationAttempts { get; set; }
        public bool? IsVerified { get; set; }
    }
}
