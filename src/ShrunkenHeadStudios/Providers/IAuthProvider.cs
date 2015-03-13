namespace ShrunkenHeadStudios.Providers
{
    // Defines methods for login, logout.
    public interface IAuthProvider
    {
        // Return True if the user is already logged-in.
        bool IsLoggedIn { get; }

        // Authenticate an user and set cookie if user is valid.
        bool Login(string username, string password);

        // Logout the user.
        void Logout();
    }
}