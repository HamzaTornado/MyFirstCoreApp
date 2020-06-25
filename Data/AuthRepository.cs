using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOME.FINANCEMENT.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HOME.FINANCEMENT.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user==null)
            {
                return null;
            }
            if(!VerifyPasswordHach(password,user.PasswrdHash,user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHach(string password, byte[] passwrdHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwrdHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHach, passwordSalt;
            CreatePasswordHash(password, out passwordHach, out passwordSalt);

            user.PasswrdHash = passwordHach;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHach, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHach = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
           
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x=> x.Username==username))
                return true;
            
            return false;
        }
    }
}
