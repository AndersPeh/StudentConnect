using Domain;

namespace Application.Interfaces;

// this interface defines a contract for a service to get information of the currently logged in user.
// any class that implements IUserAccessor must have these methods defined inside it.
public interface IUserAccessor
{
    // a method that must return the string ID of the current User.
    string GetUserId();

    // an async method that must returns a full User object of the current User without navigation properties.
    Task<User> GetUserAsync();

    // a method that returns User object with Photos navigation property.
    Task<User> GetUserWithPhotosAsync();
}
