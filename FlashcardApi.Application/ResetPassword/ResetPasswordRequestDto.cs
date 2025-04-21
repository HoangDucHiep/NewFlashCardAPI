namespace FlashcardApi.Application.ResetPassword;

public class ResetPasswordRequestDto
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}