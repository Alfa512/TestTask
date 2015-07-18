using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Test.Models
{
    public class PostsContext : DbContext
    {
        public PostsContext()
            : base("ConnectionToTest")
        {
        }

        Post post = new Post();

        public DbSet<Post> posts { get; set; }

        public IEnumerable<Post> getAllPosts()
        {
            IEnumerable<Post> _posts = posts;
            return _posts;
        }

        public Post getPostById(int id)
        {
            IEnumerable<Post> _posts = posts;
            foreach (var p in _posts)
            {
                if (p.id == id) return p;
            }
            return null;
        }

        public IEnumerable<Post> getPostsByUserId(int id_user)
        {
            List<Post> postList = new List<Post>();
            IEnumerable<Post> _currUserPosts;
            int count = 0;
            foreach (var p in posts)
            {
                if (p.id_user == id_user)
                {
                    postList.Add(p);
                    count++;
                }
            }
            if (count == 0) return null;
            _currUserPosts = postList;
            return _currUserPosts;
        }

        public List<Post> getPostsByUploading(int startFrom, int category)
        {
            PostsContext _db = new PostsContext();
            List<Post> postList = new List<Post>();
            IEnumerable<Post> _currUserPosts;
            Post post = new Post();

            if (category != 0)
            {
                var tposts = _db.Database.SqlQuery(typeof(Post), "SELECT * FROM posts WHERE category_id = " + category + " ORDER BY id DESC OFFSET " + startFrom + " ROWS FETCH NEXT " + Config.pageItems + " ROWS ONLY ");
                return tposts.OfType<Post>().ToList();
            }
            else
            {
                var tposts = _db.Database.SqlQuery(typeof(Post), "SELECT * FROM posts ORDER BY id DESC OFFSET " + startFrom + " ROWS FETCH NEXT " + Config.pageItems + " ROWS ONLY ");
                return tposts.OfType<Post>().ToList(); 
            }

            return _currUserPosts.ToList();

        }

        public Post AddPost(Post newPost)
        {
            if (newPost.id_user != null && newPost.id_user != 0 && newPost.title.Trim() != "")
            {
                PostsContext _db = new PostsContext();
                _db.posts.Add(newPost);
                _db.SaveChanges();
                var postId = _db.posts.Select(id => id.id).Max();
                newPost.id = postId;
                return newPost;
            }
            return null;
        }

        public Post EditPost(Post newPost)
        {
            if (newPost.id_user != null && newPost.id_user != 0 && newPost.title.Trim() != "")
            {
                PostsContext _db = new PostsContext();
                _db.Entry(newPost).State = EntityState.Modified;
                _db.SaveChanges();
                return newPost;
            }
            return null;
        }

        public bool dellPost(int id)
        {
            PostsContext _db = new PostsContext();
            var comment = _db.posts.Where(p => p.id == id).FirstOrDefault();
            if (comment != null)
            {
                _db.posts.Remove(comment);
                _db.SaveChanges();
            }
            return false;
        }
    }

    public class PostModel
    {
        [Required]
        [Display(Name = "id_user")]
        //public string Email { get; set; }
        public int id_user { get; set; }

        [Required]
        [Display(Name = "title")]
        public string title { get; set; }

        [Display(Name = "text")]
        public string text { get; set; }

        [Display(Name = "category_id")]
        public int category_id { get; set; }
    }

    public class Post
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // id пользователя
        public int id_user { get; set; }
        // 
        public string title { get; set; }
        // текст
        public string text { get; set; }

        public int category_id { get; set; }
    }

}