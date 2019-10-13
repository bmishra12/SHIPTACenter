using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Models;
using Umbraco.Core;
using System;

namespace UmbracoShipTac.Code
{
    public class Helpers
    {
        public static string gravatarURL(string emailAddress)
        {
            //Get email to lower
            var emailToHash = emailAddress.ToLower();

            // Create a new instance of the MD5CryptoServiceProvider object.  
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(emailToHash));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            var hashedEmail = sBuilder.ToString();  // Return the hexadecimal string.

            //Return the gravatar URL
            return "http://www.gravatar.com/avatar/" + hashedEmail;
        }

        public static string GetUserDetailAction(string useremail, string Action)
        {


            string paraphrase = "123medicarepass";
            //string email = System.Web.HttpContext.Current.User.Identity.Name;

            var userService = ApplicationContext.Current.Services.MemberService;
            //Get the member from their email address
            var amember = userService.GetByEmail(useremail);
            UserDetailAction uda = new UserDetailAction();

            uda.UserFirstName = amember.GetValue("firstName").ToString();
            uda.UserLastName = amember.GetValue("lastName").ToString();
            uda.UserEmail = useremail;

           string st =  amember.GetValue("state").ToString();
            if (st=="IND") st = "IN" ;
            if (st == "ORE") st = "OR";
            uda.UserState = st;

            uda.Organization = amember.GetValue("organization").ToString();
            uda.Action = Action;

            uda.UserRole = GetRoleOfUser(useremail);

            uda.UserId = amember.Id.ToString();

            string jsonString = JsonConvert.SerializeObject(uda);

            string encryptString = UmbracoShipTac.Code.Encryptor.StringCipher.Encrypt(jsonString, paraphrase);

            // string DecrptString = UmbracoShipTac.Code.Encryptor.StringCipher.Decrypt(encryptString, paraphrase);

            return encryptString;


        }


        //OLD method can be deleted when cleaning up
        public static string GetUserDetail()
        {


            string paraphrase = "123medicarepass";
            string email = System.Web.HttpContext.Current.User.Identity.Name;

            var userService = ApplicationContext.Current.Services.MemberService;
            //Get the member from their email address
            var amember = userService.GetByEmail(email);
            UserDetail ud = new UserDetail();

            ud.UserFirstName = amember.GetValue("firstName").ToString();
            ud.UserLastName = amember.GetValue("lastName").ToString();
            ud.UserEmail = email;

            string st = amember.GetValue("state").ToString();
            if (st == "IND") st = "IN";
            if (st == "ORE") st = "OR";
            ud.UserState = st;


            ud.Organization = amember.GetValue("organization").ToString();

            ud.UserRole = GetRoleOfUser(email);

            ud.UserId = amember.Id.ToString();

            string jsonString = JsonConvert.SerializeObject(ud);

            string encryptString = UmbracoShipTac.Code.Encryptor.StringCipher.Encrypt(jsonString, paraphrase);

            // string DecrptString = UmbracoShipTac.Code.Encryptor.StringCipher.Decrypt(encryptString, paraphrase);

            return encryptString;


        }

        private static string GetRoleOfUser(string email)
        {
            //get the role
            string _role = string.Empty;
            string[] roles = System.Web.Security.Roles.GetRolesForUser(email);


            if (roles == null || roles.Length == 0)
            {
                return "";
            }

            //there is a role assigned
            if (String.IsNullOrWhiteSpace(roles[0]))
            {
                return "";
            }
            else
            {

                return Utils.GetFriendlyRole(roles[0]);

            }

        }


        private class UserDetailAction
        {


            [JsonProperty("fname")]
            public string UserFirstName { get; set; }

            [JsonProperty("lname")]
            public string UserLastName { get; set; }

            [JsonProperty("email")]
            public string UserEmail { get; set; }

            [JsonProperty("state")]
            public string UserState { get; set; }


            [JsonProperty("role")]
            public string UserRole { get; set; }

            [JsonProperty("shipid")]
            public string UserId { get; set; }

            [JsonProperty("organization")]
            public string Organization { get; set; }

            [JsonProperty("action")]
            public string Action { get; set; }

        }



        private class UserDetail
        {


            [JsonProperty("fname")]
            public string UserFirstName { get; set; }

            [JsonProperty("lname")]
            public string UserLastName { get; set; }

            [JsonProperty("email")]
            public string UserEmail { get; set; }

            [JsonProperty("state")]
            public string UserState { get; set; }


            [JsonProperty("role")]
            public string UserRole { get; set; }

            [JsonProperty("shipid")]
            public string UserId { get; set; }

            [JsonProperty("organization")]
            public string Organization { get; set; }
        }





    }
}