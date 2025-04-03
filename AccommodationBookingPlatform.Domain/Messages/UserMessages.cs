namespace Domain.Messages;


public static class UserMessages
{ 
    public const string NotAuthenticated = "User authentication is required.";
    public const string NotGuest = "The authenticated user is not recognized as a guest.";
    public const string NotFound = "No user found with the provided ID.";
    public const string WithEmailExists = "A user with this email already exists.";
    public const string CredentialsNotValid = "Invalid credentials provided.";
    public const string InvalidRole = "The specified role is not valid.";
  
}