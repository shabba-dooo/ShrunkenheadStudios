using System.Web;
using System.Web.Security;

namespace ShrunkenHeadStudios.Providers
{
    public class AuthProvider : IAuthProvider
    {
        //Return True if the user is already logged-in.
        public bool IsLoggedIn
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }

        // Authenticate an user and set cookie if user is valid.
        public bool Login(string username, string password)
        {
            bool result = FormsAuthentication.Authenticate(username, password);

            if (result)
                FormsAuthentication.SetAuthCookie(username, false);

            return result;
        }

        // Logout the user.
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}