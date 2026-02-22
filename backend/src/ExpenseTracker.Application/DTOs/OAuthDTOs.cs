namespace ExpenseTracker.Application.DTOs;

public record GoogleLoginRequest(string Email, string Name, string GoogleId);
public record PhoneLoginRequest(string PhoneNumber, string VerificationCode);
public record SendPhoneCodeRequest(string PhoneNumber);
public record PhoneCodeResponse(string Message);
