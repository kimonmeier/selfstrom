using Microsoft.AspNetCore.Identity;
using SelfStrom.Server.Data.Entities;
using System.Text;

namespace SelfStrom.Server.Helper;

public static class HashHelper
{
    public static string Hash(string toHash)
    {
        return BCrypt.Net.BCrypt.HashPassword(toHash, 13, true);
    }

    public static bool Verify(string hashed, string unhashed)
    {
        return BCrypt.Net.BCrypt.Verify(unhashed, hashed, true);
    }
}
