using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Test.Models
{
    public class PostsCommentsContext : DbContext
    {
        public PostsCommentsContext()
            : base("ConnectionToTest")
        {
        }

        PostsContext postContext = new PostsContext();

        public DbSet<PostsComments> posts_comments { get; set; }

        public PostsComments getPostCommentById(int id)
        {
            IEnumerable<PostsComments> _posts_comments = posts_comments;
            foreach (var p in _posts_comments)
            {
                if (p.id == id) return p;
            }
            return null;
        }

        //Возвращает все комментарии к текущему посту
        public IEnumerable<PostsComments> getPostsCommentsByPostId(int post_id)
        {
            List<PostsComments> postCommentsList = new List<PostsComments>();
            int count = 0;
            foreach (var p in posts_comments)
            {
                if (p.post_id == post_id)
                {
                    postCommentsList.Add(p);
                    count++;
                }
            }
            if (count == 0) return null;
            return postCommentsList;
        }

        //Возвращает все комментарии пользователя
        public IEnumerable<PostsComments> getPostsCommentsByUserId(int user_id)
        {
            List<PostsComments> postCommentsList = new List<PostsComments>();
            int count = 0;
            foreach (var p in posts_comments)
            {
                if (p.user_id == user_id)
                {
                    postCommentsList.Add(p);
                    count++;
                }
            }
            if (count == 0) return null;
            return postCommentsList;
        }

        //Возвращает комментарии пользователя к текущему посту
        public IEnumerable<PostsComments> getPostCommentsByUserId(int post_id, int user_id)
        {
            List<PostsComments> userPostCommentsList = new List<PostsComments>();
            IEnumerable<PostsComments> _postsComments = getPostsCommentsByPostId(post_id);
            int count = 0;
            foreach (var p in _postsComments)
            {
                if (p.user_id == user_id)
                {
                    userPostCommentsList.Add(p);
                    count++;
                }
            }
            if (count == 0) return null;
            return userPostCommentsList;
        }

        public bool dellComment(int id)
        {
            PostsCommentsContext _db = new PostsCommentsContext();
            var comment = _db.posts_comments.Where(p => p.id == id).FirstOrDefault();
            if (comment != null)
            {
                _db.posts_comments.Remove(comment);
                _db.SaveChanges();
            }
            return false;
        }

        public PostsComments AddPostComment(PostsComments newPostComment)
        {
            if (newPostComment.post_id != null && newPostComment.post_id != 0 && newPostComment.text.Trim() != "")
            {
                PostsCommentsContext _db = new PostsCommentsContext();
                _db.posts_comments.Add(newPostComment);
                _db.SaveChanges();
                var postCommentId = _db.posts_comments.Select(id => id.id).Max();
                newPostComment.id = postCommentId;
                return newPostComment;
            }
            return null;
        }
    }

    public class PostsCommentsModel
    {
        [Required]
        [Display(Name = "post_id")]
        public int post_id { get; set; }

        [Required]
        [Display(Name = "user_id")]
        public int user_id { get; set; }

        [Display(Name = "text")]
        public string text { get; set; }
    }

    public class PostsComments
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        // ID 
        public int id { get; set; }
        // id поста
        public int post_id { get; set; }
        // id пользователя
        public int user_id { get; set; }
        // текст
        public string text { get; set; }
    }
}