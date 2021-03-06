﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() //Загрузка главной страницы
        {
            IPostsContext postsContext = new PostsContext();
            IUsersContext usersContext = new UsersContext();
            ICategoriesContext catContext = new CategoriesContext();
            IEnumerable<Post> posts = postsContext.GetAllPosts();
            List<Categories> categories = new List<Categories>();
            if (catContext.GetAllCategories() != null)
            {
                categories = catContext.GetAllCategories().ToList();
                ViewBag.categories = categories;
            }
            ViewBag.usersContext = usersContext;
            ViewBag.catContext = catContext;
            User currUser = new User();
            if (Session != null && Session["isAuth"] != null && (bool)Session["isAuth"] != false)
                currUser = usersContext.GetUserByLogin(Session["login"].ToString());
            else currUser = null;
            ViewBag.currUser = currUser;

            IPostImageContext pic = new PostImageContext();
            List<Image> postImageList = new List<Image>();

            if (posts != null)
            {
                foreach (var p in posts)
                {
                    postImageList.Add(pic.GetImageByPostId(p.id));
                }
            }
            if (postImageList != null)
                ViewBag.postImageList = postImageList;

            return View();
        }

        [AllowAnonymous]
        public ActionResult SearchResult(string words) //Формирование результатов поиска
        {
            if (words != null && words != "")
            {
                Session["search"] = words;
            }
            else
            {
                if (Session["search"] == null)
                    if (Request.Form["search"] != null)
                        words = Request.Form["search"].ToString();
                    else words = "";
                else
                    words = Session["search"].ToString();
            }
            if (words == null) words = "";
            List<Post> foundPosts = SearchPosts(words);

            IPostsContext postsContext = new PostsContext();
            IUsersContext usersContext = new UsersContext();
            ICategoriesContext catContext = new CategoriesContext();
            ViewBag.usersContext = usersContext;
            ViewBag.catContext = catContext;
            ViewBag.posts = foundPosts;
            User currUser = new User();
            if (Session != null && Session["isAuth"] != null && (bool)Session["isAuth"] != false)
                currUser = usersContext.GetUserByLogin(Session["login"].ToString());
            else currUser = null;
            ViewBag.currUser = currUser;

            IPostImageContext pic = new PostImageContext();
            List<Image> postImageList = new List<Image>();

            if (foundPosts != null)
            {
                foreach (var p in foundPosts)
                {
                    postImageList.Add(pic.GetImageByPostId(p.id));
                }
            }
            if (postImageList != null)
                ViewBag.postImageList = postImageList;

            ViewBag.words = words;
            ViewBag.foundPosts = foundPosts;
            return View();
        }

        public List<Post> SearchPosts(string words) //Поиск среди статей по ключевым словам
        {
            string __words = words.Trim();
            string[] _words = __words.Split(' ', ',', '.');
            List<Post> foundPosts = new List<Post>();
            IPostsContext postsContext = new PostsContext();
            IEnumerable<Post> posts = postsContext.GetAllPosts();
            foreach (var u in posts)
            {
                foreach (var w in _words)
                    if (u.title.ToLower().Contains(w.ToLower()) || u.text.Contains(w)) foundPosts.Add(u);
            }
            return foundPosts;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Neutrinosoft. Тестовое задание.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Ждём ваших предложений!";

            return View();
        }
    }
}