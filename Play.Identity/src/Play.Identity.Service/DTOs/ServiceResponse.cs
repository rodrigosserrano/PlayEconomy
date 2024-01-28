namespace Play.Identity.Service.DTOs;

public class ServiceResponse
{
    public record class LoginResponse(string Token, string Message);
}