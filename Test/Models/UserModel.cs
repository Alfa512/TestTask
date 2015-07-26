using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Helpers;

namespace Test.Models
{
    public interface IUsersContext
    {
        User GetUserById(int id);
        User GetUserByLogin(string login);
        User CreateUser(User _user);
        bool ValidateUser(string login, string password);
        User EditUser(User user);
    }

    public class UsersContext : DbContext, IUsersContext
    {
        public UsersContext()
            : base("ConnectionToTest")
        {
        }

        User user = new User();

        public DbSet<User> users { get; set; }

        public User GetUserById(int id)
        {
            if (users.Count() != 0)
            {
                foreach (var p in users)
                {
                    if (p.id == id) return p;
                }
            }
            return null;
        }

        public User GetUserByLogin(string login)
        {
            IEnumerable<User> _users = users;
            
            if (users.Count() != 0)
            {
                foreach (var p in users)
                {
                    if (p.login == login) return p;
                }
            }
            return null;
        }

        public User CreateUser(User _user)
        {
            User user = new User();
            if (GetUserByLogin(_user.login) == null)
            {
                UsersContext _db = new UsersContext();
                _user.password = Crypto.HashPassword(_user.password);

                _db.users.Add(_user);
                _db.SaveChanges();
                return _user;
            }
            else return null;

        }

        public bool ValidateUser(string login, string password)
        {
            bool isValid = false;

            using (UsersContext _db = new UsersContext())
            {
                try
                {
                    User user = (from u in _db.users
                                 where u.login == login
                                 select u).FirstOrDefault();

                    if (user != null && Crypto.VerifyHashedPassword(user.password, password))
                    {
                        isValid = true;
                    }
                }
                catch
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public User EditUser(User user)
        {
            UsersContext _db = new UsersContext();
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();
            return user;
        }
    }

    public class UserModel
    {
        [Required]
        [Display(Name = "id")]
        public int id { get; set; }

        [Required]
        [Display(Name = "login")]
        public string login { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string password { get; set; }

        [Display(Name = "name")]
        public string name { get; set; }

        [Display(Name = "last_name")]
        public string last_name { get; set; }

        [Display(Name = "gender")]
        public string gender { get; set; }

        [Display(Name = "e_mail")]
        public string e_mail { get; set; }
    }



    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // логин
        public string login { get; set; }
        // пароль
        public string password { get; set; }
        // имя
        public string name { get; set; }
        // фамилия
        public string last_name { get; set; }
        // пол
        public string gender { get; set; }
        // e_mail
        public string e_mail { get; set; }
    }
}