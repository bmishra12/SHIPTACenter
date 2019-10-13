using System;

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UmbracoShipTac.Code
{

    public class Encryptor
    {

        //public static void Main()
        //{
        //    System.Console.WriteLine("Hello, World!");

        //    /*byte[] test = Encoding.ASCII.GetBytes("password");
        //    System.Console.Write("Test: ");
        //    foreach (byte b in test)
        //    {
        //        System.Console.Write("{0:x} ", b);
        //    }
        //    System.Console.Write("\n");*/

        //    string encrypted = Encryptor.StringCipher.Encrypt("HELLO WORLD", "123medicarepass");
        //    System.Console.WriteLine("Decrypted text is: " + Encryptor.StringCipher.Decrypt(encrypted, "123medicarepass"));
        //}

        public static class StringCipher
        {
            // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
            // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
            // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.

            // If we set blocksize to 256, the string must be 32 characters to satisfy this.
            private static byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2tu89geji340t89u2");


            // This constant is used to determine the keysize of the encryption algorithm.
            private const int keysize = 256;
            // We define a byte array for the salt since the Rfc2898 method requires a non-null salt parameter
            private static byte[] salt = { 0x78, 0x57, 0x8e, 0x5a, 0x5d, 0x63, 0xcb, 0x06 };

            public static string Encrypt(string plainText, string passPhrase)
            {
                /*System.Console.WriteLine(passPhrase);
                System.Console.Write("initVector: ");
                foreach (byte b in initVectorBytes)
                {
                    System.Console.Write("{0:x} ", b);
                }
                System.Console.Write("\n\n");*/
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                /*System.Console.Write("Plain Text: ");
                foreach (byte b in plainTextBytes)
                {
                    System.Console.Write("{0:x} ", b);
                }
                System.Console.Write("\n\n");*/
                using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, salt))
                {
                    //System.Console.WriteLine(password);
                    // Default iterations count is 1000.
                    //System.Console.WriteLine("Iterations: " + password.IterationCount);
                    byte[] keyBytes = password.GetBytes(keysize / 8);
                    //System.Console.WriteLine("keyBytes length is: " + keyBytes.Length);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        // We set BlockSize to 256
                        symmetricKey.BlockSize = 256;
                        // PaddingMode is set to 'Zeros' - if there isn't enough data to fill an entire block, the block is padded with null bytes
                        symmetricKey.Padding = PaddingMode.Zeros;
                        /*System.Console.Write("Key: ");
                        foreach (byte b in keyBytes)
                        {
                            System.Console.Write("{0:x} ", b);
                        }
                        System.Console.Write("\n\n");*/
                        symmetricKey.Mode = CipherMode.CBC;
                        using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    byte[] cipherTextBytes = memoryStream.ToArray();
                                    /*System.Console.Write("Cipher Text bytes: ");
                                    foreach (byte b in cipherTextBytes)
                                    {
                                        System.Console.Write("{0:x} ", b);
                                    }
                                    System.Console.Write("\n\n");
                                    System.Console.WriteLine(Convert.ToBase64String(cipherTextBytes));*/
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }

            public static string Decrypt(string cipherText, string passPhrase)
            {
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, salt))
                {
                    byte[] keyBytes = password.GetBytes(keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        // We have to set BlockSize and Padding Mode for this to decrypt properly.
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Padding = PaddingMode.Zeros;
                        symmetricKey.Mode = CipherMode.CBC;
                        using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}