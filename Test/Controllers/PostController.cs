using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;
using System.IO;
using System.Web.Helpers;
using System.Net;

namespace Test.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddPost() //Обработка данных для добавления поста
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");
            PostsContext postsContext = new PostsContext();
            PostImageContext postImageContext = new PostImageContext();
            UsersContext usersContext = new UsersContext();
            ImagesContext imageContext = new ImagesContext();
            CategoriesContext categoriesContext = new CategoriesContext();

            User currentUser = new User();
            currentUser = usersContext.getUserByLogin((string)Session["login"]);

            Post newPost = new Post();
            newPost.id_user = currentUser.id;
            newPost.title = Request.Form["posttitle"];
            newPost.text = Request.Form["posttext"];
            //newPost.category_id = Convert.ToInt32(Request.Form["menu-val"]);

            if (Request.Form["menu-val"] == "newCat")
            {
                Categories cat = new Categories();
                cat.value = Request.Form["catText"];
                newPost.category_id = categoriesContext.createCategory(cat).id;
            }
            else if(Convert.ToInt32(Request.Form["menu-val"]) == -1)
            {
                if (categoriesContext.getCategoryByValue("Общая") == null)
                {
                    Categories cat = new Categories();
                    cat.value = "Общая";
                    newPost.category_id = categoriesContext.createCategory(cat).id;
                }
                else
                {
                    Categories cat = new Categories();
                    cat = categoriesContext.getCategoryByValue("Общая");
                    newPost.category_id = cat.id;
                }
                
            }
            else
            {
                newPost.category_id = Convert.ToInt32(Request.Form["menu-val"]);
            }

            newPost = postsContext.AddPost(newPost);

            var file = Request.Files["post_image"];

            if (file.ContentLength != 0)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "images/posts/";
                string filename = newPost.id.ToString() + Path.GetExtension(file.FileName);
                if (filename != null) file.SaveAs(path + filename);
                Image image = new Image();
                image.image_path = filename;
                image = imageContext.AddImage(image.image_path);
                postImageContext.AddPostImage(newPost.id, image.id);
            }
            return RedirectToAction("userProfile", "Account");
        }

        [AllowAnonymous]
        public ActionResult EditPost() //Загрузка формы изменения поста
        {
            PostsContext postsContext = new PostsContext();
            PostImageContext postImageContext = new PostImageContext();
            UsersContext usersContext = new UsersContext();
            Post post = new Post();
            string url = "~/Post/PostPage?post=";
            post = postsContext.getPostById(Convert.ToInt32(Request.QueryString["post"]));
            ViewBag.post = post;
            if (post.id_user != usersContext.getUserByLogin(Session["login"].ToString()).id) return Redirect(url + post.id);

            CategoriesContext catContext = new CategoriesContext();
            List<Categories> categories = new List<Categories>();
            categories = catContext.getAllCategories().ToList();
            ViewBag.categories = categories;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditPostF() //Обработка данных для изменения поста
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");
            PostsContext postsContext = new PostsContext();
            PostImageContext postImageContext = new PostImageContext();
            UsersContext usersContext = new UsersContext();
            ImagesContext imageContext = new ImagesContext();
            string url = "~/Post/PostPage?post=";

            User currentUser = new User();
            currentUser = usersContext.getUserByLogin((string)Session["login"]);

            Post newPost = new Post();
            newPost.id = Convert.ToInt32(Request.Form["postId"]);
            newPost.id_user = currentUser.id;
            newPost.title = Request.Form["posttitle"];
            newPost.text = Request.Form["posttext"];
            newPost.category_id = Convert.ToInt32(Request.Form["menu-val"]);


            newPost = postsContext.EditPost(newPost);

            var file = Request.Files["post_image"];

            if (file.ContentLength != 0)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "images/posts/";
                string filename = newPost.id.ToString() + Path.GetExtension(file.FileName);
                if (filename != null) file.SaveAs(path + filename);
                if(postImageContext.getImageByPostId(newPost.id) == null)
                {
                    Image image = new Image();
                    image.image_path = filename;
                    image = imageContext.AddImage(image.image_path);
                    postImageContext.AddPostImage(newPost.id, image.id);
                }
                else
                {
                    Image image = new Image();
                    image = postImageContext.getImageByPostId(newPost.id);
                    image.image_path = filename;
                    image = imageContext.EditImage(image);
                }
            }
            return Redirect(url + newPost.id);
        }

        [AllowAnonymous]
        public ActionResult PostPage() //Загрузка данных новостной страницы
        {
            if (Request.QueryString["post"] == null) RedirectToAction("Index", "Home");
            CategoriesContext catContext = new CategoriesContext();
            PostsContext postsContext = new PostsContext();
            PostsCommentsContext postsCommentsContext = new PostsCommentsContext();
            UsersContext usersContext = new UsersContext();
            User currentUser = new User();
            if (Session != null && Session["isAuth"] != null && (bool)Session["isAuth"] != false)
                currentUser = usersContext.getUserByLogin(Session["login"].ToString());
            else
                currentUser = null;
            ViewBag.currentUser = currentUser;
            ViewBag.usersContext = usersContext;
            Post post = new Post();
            if (Request.QueryString["post"] == null) return RedirectToAction("Index", "Home");
            post = postsContext.getPostById(Convert.ToInt32(Request.QueryString["post"]));
            if (post == null) return RedirectToAction("Index", "Home");
            User postUser = usersContext.getUserById(post.id_user);
            ViewBag.postUser = postUser;
            ViewBag.categories = catContext.getAllCategories().ToList();
            ViewBag.catContext = catContext;
            
            
            PostImageContext postImageContext = new PostImageContext();
            if (postImageContext.getImageByPostId(post.id) != null)
                ViewBag.postImage = postImageContext.getImageByPostId(post.id).image_path;
            ViewBag.post = post;
            IEnumerable<PostsComments> postComments = postsCommentsContext.getPostsCommentsByPostId(post.id);

            int page = 1;
            if (Request.QueryString["page"] != null)
                page = Convert.ToInt32(Request.QueryString["page"]);

            if (postComments != null)
            {
                List<PostsComments> _postsComments = new List<PostsComments>();
                _postsComments = postComments.ToList();
                List<PostsComments> currComments = new List<PostsComments>(Config.pageItems);
                int pagination = GetPagination(_postsComments.Count);
                int start = 0, end = 0;

                start = (page - 1) * Config.pageItems;
                if (_postsComments.Count == 1) end = 0;
                else
                    if (_postsComments.Count < Config.pageItems) end = _postsComments.Count - 1;
                    else 
                        if (page == pagination && _postsComments.Count % Config.pageItems > 0)
                            end = _postsComments.Count - 1;
                        else
                            end = page * Config.pageItems - 1;

                for (int i = start; i <= end; i++)
                {
                    currComments.Add(_postsComments[i]);
                }
                ViewBag.currentPage = page;
                ViewBag.pagination = pagination;
                ViewBag.postComments = currComments;
            }
            else
            {
                page = 0;
            }
            return View();
        }

        public int GetPagination(int count) //Формирование пагинации
        {
            List<string> pagination = new List<string>();
            int pages = 0;
            int max;
            if (count % Config.pageItems > 0)
                max = count / Config.pageItems + 1;
            else max = count / Config.pageItems;
            for (int i = 0; i < max; i++)
            {
                pages++;
            }
            return pages;
        }

        struct HTPosts //Структура данных постов для подгрузки
        {
            public int id;
            public int user_id;
            public int currUser_id;
            public string title;
            public string text;
            public string image_name;
            public string category;
            public string userName;
            public string userLastName;
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult getPosts() //Формирование списа постов для скрипта подгрузки
        {
            PostsContext postsContext = new PostsContext();
            PostImageContext postImageContext = new PostImageContext();
            UsersContext usersContext = new UsersContext();
            CategoriesContext catContext = new CategoriesContext();
            IEnumerable<Post> posts = postsContext.getAllPosts();
            ViewBag.usersContext = usersContext;
            ViewBag.catContext = catContext;
            ViewBag.posts = posts.ToList();
            User currUser = new User();
            if (Session != null && Session["isAuth"] != null && (bool)Session["isAuth"] != false)
                currUser = usersContext.getUserByLogin(Session["login"].ToString());
            else currUser = null;

            List<Post> sortedPosts = new List<Post>();

            int category = Convert.ToInt32(Request.Form["category"]);
            int startFrom = Convert.ToInt32(Request.Form["startFrom"]);
            sortedPosts = postsContext.getPostsByUploading(startFrom, category);

            List<HTPosts> htPosts = new List<HTPosts>();

            foreach (var p in sortedPosts)
            {
                HTPosts hp;
                hp.id = p.id;
                hp.user_id = p.id_user;
                if (currUser == null)
                    hp.currUser_id = 0;
                else hp.currUser_id = currUser.id;

                hp.title = p.title;
                hp.text = p.text;
                hp.category = catContext.getCategoryById(p.category_id).value;
                if (postImageContext.getImageByPostId(p.id) != null)
                    hp.image_name = postImageContext.getImageByPostId(p.id).image_path;
                else
                    hp.image_name = "";
                hp.userName = usersContext.getUserById(p.id_user).name;
                hp.userLastName = usersContext.getUserById(p.id_user).last_name;

                htPosts.Add(hp);
            }

            JsonResult data = Json(htPosts);

            return data;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddComment() //Обработка данных с формы добавления комментария
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");

            PostsContext postsContext = new PostsContext();
            PostsCommentsContext postsCommentsContext = new PostsCommentsContext();
            UsersContext usersContext = new UsersContext();
            PostsComments comment = new PostsComments();
            int postId = Convert.ToInt32(Request.Form["postId"]);
            if (postId == 0) return RedirectToAction("Index", "Home");
            comment.post_id = postId;
            comment.user_id = Convert.ToInt32(Request.Form["userId"]);
            comment.text = Request.Form["commentText"];
            string url = Request.Form["postURL"];
            if (Request.Form["commentText"] == null)
                if (url != null)
                    return Redirect(url);
                else
                    return RedirectToAction("Index", "Home");

            comment = postsCommentsContext.AddPostComment(comment);

            return Redirect(url);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult dellComment() //Удаление комментария
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");

            PostsContext postsContext = new PostsContext();
            PostsCommentsContext postsCommentsContext = new PostsCommentsContext();
            UsersContext usersContext = new UsersContext();
            PostsComments comment = new PostsComments();
            comment = postsCommentsContext.getPostCommentById(Convert.ToInt32(Request.Form["commentId"]));
            //if (comment == null || postsContext.getPostById(comment.post_id).id_user != usersContext.getUserByLogin(Session["login"].ToString()).id) return RedirectToAction("Index", "Home");
            if (comment == null) return RedirectToAction("Index", "Home");
            postsCommentsContext.dellComment(comment.id);
            string url = "~/Post/PostPage?post=" + comment.post_id;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult dellPost() //Удаление поста
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");

            PostsContext postsContext = new PostsContext();
            PostsCommentsContext postsCommentsContext = new PostsCommentsContext();
            UsersContext usersContext = new UsersContext();
            Post post = new Post();
            post = postsContext.getPostById(Convert.ToInt32(Request.Form["postId"]));
            if (post == null || post.id_user != usersContext.getUserByLogin(Session["login"].ToString()).id) return RedirectToAction("Index", "Home");
            postsContext.dellPost(post.id);
            string url = "~/Post/PostPage?post=" + post.id;
            return RedirectToAction("Index", "Home");
        }
    }
}