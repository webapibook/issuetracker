using System;
using System.Text;

namespace WebApiBook.IssueTrackerApi.Infrastructure
{
    public class BasicCredentials
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public BasicCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public static BasicCredentials TryParse(string credentials)
        {
            string pair;
            try
            {
                pair = Encoding.ASCII.GetString(Convert.FromBase64String(credentials));
            }
            catch (FormatException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            var ix = pair.IndexOf(':');
            return ix == -1 ? null : new BasicCredentials(pair.Substring(0, ix), pair.Substring(ix + 1));
        }

        public string ToCredentialString()
        {
            return Convert.ToBase64String(
                Encoding.ASCII.GetBytes(Username + ':' + Password));
        }
    }
}