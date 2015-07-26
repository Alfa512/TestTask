using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Test.Models
{
    public interface IUserImageContext
    {
        UserImage GetUserImageById(int id);
        IEnumerable<UserImage> GetUserImageByUserId(int user_id);
        Image GetImageByUserId(int user_id);
        UserImage AddUserImage(int user_id, int image_id); // Возвращает только что добавленную запись
    }

    public class UserImageContext : DbContext, IUserImageContext
    {
        public UserImageContext()
            : base("ConnectionToTest")
        {
        }

        UserImage image = new UserImage();
        IImagesContext imagesContext = new ImagesContext();

        public DbSet<UserImage> UserImages { get; set; }

        public UserImage GetUserImageById(int id)
        {
            if (UserImages.Local.Count != 0)
            {
                foreach (var p in UserImages)
                {
                    if (p.id == id) return p;
                }
            }
            return null;
        }

        public IEnumerable<UserImage> GetUserImageByUserId(int user_id)
        {
            List<UserImage> userList = new List<UserImage>();
            IEnumerable<UserImage> _currUserImages = null;
            if (UserImages.Count() != 0)
            {
                foreach (var cuf in UserImages)
                {
                    if (cuf.user_id == user_id) userList.Add(cuf);
                }
                _currUserImages = userList;
                return _currUserImages;
            }
            else return null;
        }

        public Image GetImageByUserId(int user_id)
        {
            Image image = new Image();
            if (UserImages.Count() != 0)
            {
                foreach (var ui in UserImages)
                {
                    if (ui.user_id == user_id)
                    {
                        image = imagesContext.GetImageById(ui.image_id);
                        return image;
                    }
                }
            }
            return null;
        }

        public UserImage AddUserImage(int user_id, int image_id) // Возвращает только что добавленную запись
        {
            UserImage userImage = new UserImage();
            if (user_id != null && image_id != null)
            {
                UserImageContext userImageContext = new UserImageContext();
                userImage.user_id = user_id;
                userImage.image_id = image_id;

                userImageContext.UserImages.Add(userImage);
                userImageContext.SaveChanges();
                var imId = userImageContext.UserImages.Select(id => id.id).Max();
                userImage.id = imId;
                return userImage;
            }
            else return null;
        }
    }

    public class UserImageModel
    {
        [Required]
        [Display(Name = "user_id")]
        public int user_id { get; set; }

        [Required]
        [Display(Name = "image_id")]
        public int image_id { get; set; }
    }



    public class UserImage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // id пользователя
        public int user_id { get; set; }
        // 
        public int image_id { get; set; }
    }
}