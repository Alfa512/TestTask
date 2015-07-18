using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Test.Models
{
    public class PostImageContext : DbContext
    {
        public PostImageContext()
            : base("ConnectionToTest")
        {
        }

        PostImage image = new PostImage();
        ImagesContext imagesContext = new ImagesContext();

        public DbSet<PostImage> post_image { get; set; }

        public PostImage getPostImageById(int id)
        {
            IEnumerable<PostImage> _post_image = post_image;
            foreach (var p in _post_image)
            {
                if (p.id == id) return p;
            }
            return null;
        }

        public IEnumerable<PostImage> getPostImageByPostId(int post_id)
        {
            List<PostImage> postList = new List<PostImage>();
            IEnumerable<PostImage> _currUserPosts = null;
            foreach (var cuf in post_image)
            {
                if (cuf.post_id == post_id) postList.Add(cuf);
            }
            _currUserPosts = postList;
            return _currUserPosts;
        }

        public Image getImageByPostId(int post_id)
        {
            Image image = new Image();
            foreach (var pi in post_image)
            {
                if (pi.post_id == post_id)
                {
                    image = imagesContext.getImageById(pi.image_id);
                    return image;
                }
            }
            return null;
        }

        public PostImage AddPostImage(int post_id, int image_id) // Возвращает только что добавленную запись
        {
            PostImage postImage = new PostImage();
            if (post_id != null && image_id != null)
            {
                PostImageContext postImageContext = new PostImageContext();
                postImage.post_id = post_id;
                postImage.image_id = image_id;

                postImageContext.post_image.Add(postImage);
                postImageContext.SaveChanges();
                var imId = postImageContext.post_image.Select(id => id.id).Max();
                postImage.id = imId;
                return postImage;
            }
            else return null;
        }
    }

    public class PostImageModel
    {
        [Required]
        [Display(Name = "post_id")]
        public int post_id { get; set; }

        [Required]
        [Display(Name = "image_id")]
        public int image_id { get; set; }
    }



    public class PostImage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // id пользователя
        public int post_id { get; set; }
        // 
        public int image_id { get; set; }
    }
}