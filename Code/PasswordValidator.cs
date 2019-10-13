using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace UmbracoShipTac.Code
{
    public static class PasswordValidator
    {


        /// <summary>
        /// Take all the words in the password string and split them 
        /// Check if the length of the element greater than 3
        /// and check if the contain any word
        /// </summary>
        //public static bool DoesTextContainsWord(string pwdText)
        //{
        //    List<string> words = WordCache.WordList;

        //    string[] w = Regex.Split(pwdText, @"[^A-Za-z]+");
        //    foreach (string p in w)
        //    {
        //        if (p.Length > 3)
        //        {
        //            foreach (string x in words)
        //            {
        //                if (pwdText.Contains(x))
        //                {
        //                    return true;
        //                }
        //            }

        //        }
        //        return false;
        //    }
        //    return false;

        //}

        public static bool DoesTextContainsFirstLastName(string pwdText, string firstName, string lastName)
        {

            if (pwdText.Contains(firstName))
            {
                return true;
            }

            if (pwdText.Contains(lastName))
            {
                return true;
            }



            return false;
        }

        public static bool DoesContainFourConsecutive(string input)
        {

            string[] w = Regex.Split(input, @"[^A-Za-z1-9]+");
            foreach (string token in w)
            {
                if (token.Length > 3)
                {


                    // Convert the string into a byte[].
                    byte[] asciiBytes = Encoding.ASCII.GetBytes(token);
                    int[] ints = asciiBytes.Select(x => (int)x).ToArray();
                    int count = 1;

                    for (int i = 0; i < ints.Length - 1; i++)
                    {
                        if (ints[i] + 1 == ints[i + 1])
                            count = count + 1;
                        else
                            count = 1; //reset the counter

                        if (count > 3)
                            return true;
                    }

                }
            }

            return false;
        }


        /// <summary>
        /// Take all the words in the email string and separate them.
        /// Check if password contains email greater than 3
        /// </summary>
        public static bool DoesPassWordContainsEmail(string email, string pwd)
        {


            string[] w = Regex.Split(email.Split('@')[0], @"[^A-Za-z]+");
            foreach (string p in w)
            {
                if (p.Length > 3)
                {
                    if (pwd.Contains(p))
                        return true;
                }
            }
            return false;
        }
    }
}