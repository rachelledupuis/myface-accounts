using MyFace.Data;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


namespace MyFace.Helpers

{
    public class PasswordHelper

    {
    public static byte[] GetRandomSalt()
    {
        // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
        byte[] salt = new byte[128 / 8];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetNonZeroBytes(salt);
        }
        return salt;
    }
    public static HashedPasswordSalt GetHashedPassword(string password, byte[] salt)
    {
        string saltString = Convert.ToBase64String(salt);
        string newPassword = password;
        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: newPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

            return new HashedPasswordSalt (hashed, saltString);
    }
    }

}

