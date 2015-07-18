using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Test.Models
{
    public class ImagesContext : DbContext
    {
        public ImagesContext()
            : base("ConnectionToTest")
        {
        }


        public DbSet<Image> images { get; set; }

        public Image getImageById(int id)
        {
            if (images.Count() != 0)
            foreach (var p in images)
            {
                if (p.id == id) return p;
            }
            return null;
        }
        public Image AddImage(string image_path) // Возвращает только что добавленную запись
        {
            Image image = new Image();
            if (image_path != null && image_path.Trim() != "")
            {
                ImagesContext _db = new ImagesContext();
                image.image_path = image_path;

                _db.images.Add(image);
                _db.SaveChanges();
                var imId = _db.images.Select(id => id.id).Max();
                image.id = imId;
                return image;
            }
            else return null;
        }

        public Image EditImage(Image image)
        {
            if (image != null && image.id != 0 && image.image_path.Trim() != "")
            {
                ImagesContext _db = new ImagesContext();
                _db.Entry(image).State = EntityState.Modified;
                _db.SaveChanges();
                return image;
            }
            return null;
        }

    }

    public class ImageModel
    {
        [Required]
        [Display(Name = "image_path")]
        public string image_path { get; set; }

    }



    public class Image
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // путь
        public string image_path { get; set; }

    }
}