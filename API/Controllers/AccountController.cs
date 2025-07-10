using System;
using API.DTOs;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Although Identity system provides default route, creating custom routes here for more tailored solutions.
// inherits features of BaseApiController like [Route("api/[controller]")].
// the base route becomes api/account by automatically removing Controller from AccountController and changing it to lowercase.
// takes signInManager of User from Dependency Injection to manipulate Identity System.
public class AccountController(SignInManager<User> signInManager) : BaseApiController
{
    // Anyone can register.
    [AllowAnonymous]
    // account/register
    [HttpPost("register")]
    // RegisterUser method returns ActionResult after successfully registering a user.
    // It takes RegisterDto as parameter only.
    public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
    {
        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            // customise /register route to add DisplayName. 
            // Because the default /register route by .Net Identity only requires email and password.
            DisplayName = registerDto.DisplayName
        };

        // create new user in the Identity system. Identity system will check password complexity, duplicate email etc.
        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        // return Task<ActionResult> Ok if successful.
        if (result.Succeeded) return Ok();

        foreach (var error in result.Errors)
        {
            // for each error, adds error code and description to Model State which will be returned by ValidationProblem.
            ModelState.AddModelError(error.Code, error.Description);
        }

        // returns Identity and normal validation errors.
        return ValidationProblem();
    }

    // unauthenticated user can access this endpoint because client side needs to access to user-info to check 
    // if username or email exists before registration.
    [AllowAnonymous]
    [HttpGet("user-info")]
    // GetUserInfo method returns ActionResult.
    public async Task<ActionResult> GetUserInfo()
    {
        // if user is not authenticated, return no content.
        if (User.Identity?.IsAuthenticated == false) return NoContent();

        //  Get user from Identity system.
        var user = await signInManager.UserManager.GetUserAsync(User);

        if (user == null) return Unauthorized();

        return Ok(new
        {
            user.DisplayName,
            user.Email,
            user.Id,
            user.ImageUrl,
        });
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        // Identity system signs user out and removes cookie.
        await signInManager.SignOutAsync();

        // signals Request is Ok but nothing to send back to the client.
        return NoContent();
    }
}
