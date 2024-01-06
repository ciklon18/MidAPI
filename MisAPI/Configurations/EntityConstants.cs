namespace MisAPI.Configurations;

public static class EntityConstants
{
    public const string PhoneNumberRegex = @"^((\+7)|8)[0-9]{10}$";

    public const string IncorrectPhoneNumberError =
        "Invalid phone number format. It must be +7xxxxxxxxxx or 8xxxxxxxxxx";

    public const string ShortPasswordError = "Too short password. Password must be 6 or more characters.";
    public const string ShortOrLongEmailError = "Email must be at least 5 characters and no more than 100.";
    public const string IncorrectDateError = "Invalid date. it can contain only numbers and -. And date can't be in future.";

    public const string FullNameRegex = @"^[a-zA-ZА-Яа-я\s]*$";
    public const string WrongSymbolInFullNameError = "Name can only contain letters and spaces.";
    public const string WrongPatientNameError = "Patient name must be 2 and more characters.";
    public const string IncorrectEmailError = "Invalid email format. It must be user@example.com";
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    
    
    public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
    public const string PasswordRegex = @"^(?=.*[A-Za-z])(?=.*\d).{4,}$";
    public const string ImageUrlRegex = "^(https?|ftp|file)://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]$";
    
    

    public const string IncorrectPasswordError =
        "Password must be 6 or more characters. Also it must contains letters and digits";

}