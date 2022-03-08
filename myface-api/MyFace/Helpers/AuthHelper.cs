using System;
using MyFace.Data;

namespace MyFace.Helpers

{
    public class AuthHelper
    {
        public static UserNamePassword GetUsernamePasswordFromAuth(string authHeaderString)
        {
            var authHeaderSplit = authHeaderString.Split(' ');
            var authType = authHeaderSplit[0];
            var encodedUsernamePassword = authHeaderSplit[1];

            var usernamePassword = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedUsernamePassword)
            );

            var usernamePasswordArray = usernamePassword.Split(':');

            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];
            return new UserNamePassword(username, password);
        }
    }
}