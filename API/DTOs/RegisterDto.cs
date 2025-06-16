using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    // DisplayName is required but set a default empty string to standardise the error message.
    [Required]
    public string DisplayName { get; set; } = "";

    [Required]
    // Email Address Validator.
    [EmailAddress]
    public string Email { get; set; } = "";

    // no need to set DataAnnotations for password because Identity will enforce password complexity.
    public string Password { get; set; } = "";

}
