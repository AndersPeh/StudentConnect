using System;
using Microsoft.AspNetCore.Identity;

namespace Domain;

// inherits from IdentityUser which uses a string as primary key. IdentityUser generates salted and hashed representation of the password.
public class User : IdentityUser
{
    // make these properties optional.
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }

    public string? ImageUrl { get; set; }


}
