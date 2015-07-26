using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Test.Models;
using System.Web.Security;
using System.Web.Security.Cryptography;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Data;
using System.Data.Entity;
using System.IO;

namespace Test.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public bool IsAuthenticated() //Проверка авторизации
        {
            if (Session != null && Session["isAuth"] != null)
                if ((bool)Session["isAuth"] == true) return true;
            return false;
        }

        public string GetSessionLogin() //Получение логина авторизаванного пользователя
        {
            return (string)Session["login"];
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) //Загрузка представления авторизации
        {
            ViewBag.ReturnUrl = returnUrl;
            if (returnUrl != null) return Redirect(returnUrl);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) //Обработка запроса на авторизацию
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IUsersContext usersContext = new UsersContext();

            if (usersContext.ValidateUser(model.Login, model.Password))
            {
                Session["login"] = model.Login.ToLower();
                Session["isAuth"] = true;
                int a = Session.Timeout;
                
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
            // Если пользователь введет неправильные коды за указанное время, его учетная запись 
            // будет заблокирована на заданный период. 
            // Параметры блокирования учетных записей можно настроить в IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неправильный код.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register() //Загрузка представления формы регистрации
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model) //Обработка данных с формы регистрации
        {
            if (ModelState.IsValid)
            {
                var user = new User { e_mail = model.Email, login = model.Login, password = model.Password };
                IUsersContext usersContext = new UsersContext();
                LoginViewModel _model = new LoginViewModel();
                _model.Login = model.Login;
                _model.Password = model.Password;
                if (usersContext.CreateUser(user) != null)
                {
                    await Login(_model, null);
                    return RedirectToAction("Index", "Home");
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult UserProfile() // Загрузка представленя страницы пользователя
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");
            IUsersContext _user = new UsersContext();
            IPostsContext postsContext = new PostsContext();
            IUserImageContext userImageContext = new UserImageContext();
            ICategoriesContext catContext = new CategoriesContext();

            List<Categories> categories = new List<Categories>();
            if (catContext.GetAllCategories() != null)
            {
                categories = catContext.GetAllCategories().ToList();
                ViewBag.categories = categories;
            }
            ViewBag.catContext = catContext;

            int s = 0;
            if (Request.QueryString.Count == 0) s = 0;
            else
                s = Convert.ToInt32(Request.QueryString["user"]);
            if (s == null || s == 0) s = _user.GetUserByLogin(Convert.ToString(Session["login"])).id;
            User user = new User();
            user = _user.GetUserById(s);

            ViewBag.login = user.login;
            ViewBag.user = user;
            if (Session != null && Session["isAuth"] != null && (bool)Session["isAuth"] != false)
                ViewBag.sessionUser = _user.GetUserByLogin(Session["login"].ToString());

            IEnumerable<Post> userPosts;
            userPosts = postsContext.GetPostsByUserId(user.id);

            IPostImageContext pic = new PostImageContext();
            List<Image> postImageList = new List<Image>();
            Image userImage = new Image();
            userImage = userImageContext.GetImageByUserId(user.id);
            if (userImage != null)
                ViewBag.userImage = userImage.image_path;
            else
                ViewBag.userImage = "default.png";

            if (userPosts != null)
            {
                foreach (var p in userPosts)
                {
                    postImageList.Add(pic.GetImageByPostId(p.id));
                }
            }
            ViewBag.posts = userPosts;
            if (postImageList != null)
                ViewBag.postImageList = postImageList;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangeProfile(int id) //Загрузка страницы изменения профиля пользователя
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");

            IUsersContext _user = new UsersContext();
            User user = new User();
            user = _user.GetUserByLogin((string)Session["login"]);
            ViewBag.user = user;

            

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangeProfile() //Обработка данных с формы изменения профиля пользователя
        {
            if (Session == null || Session["isAuth"] == null || (bool)Session["isAuth"] == false) return RedirectToAction("Login", "Account");

            IUsersContext _user = new UsersContext();
            IImagesContext _image = new ImagesContext();
            IUserImageContext _userImage = new UserImageContext();
            User user = new User();
            user = _user.GetUserByLogin((string)Session["login"]);

            ViewBag.user = user;

            user.name = Request.Form["name"];
            user.last_name = Request.Form["last_name"];
            user.gender = Request.Form["gender"];
            user.e_mail = Request.Form["e_mail"];

            var file = Request.Files["image"];

            if (file.ContentLength != 0)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "images/users/";
                string filename = user.id.ToString() + Path.GetExtension(file.FileName);
                if (filename != null) file.SaveAs(path + filename);

                if (_userImage.GetImageByUserId(user.id) == null)
                {
                    Image image = new Image();
                    image.image_path = filename;
                    image = _image.AddImage(image.image_path);
                    _userImage.AddUserImage(user.id, image.id);
                }
                else
                {
                    Image image = new Image();
                    image = _userImage.GetImageByUserId(user.id);
                    image.image_path = filename;
                    image = _image.EditImage(image);
                }
            }

            /*-------------- Обращение к БД ------------------------*/

            try
            {
                _user.EditUser(user);
                return RedirectToAction("userProfile", "Account");
            }
            catch
            {
                return View();
            } 
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Не показывать, что пользователь не существует или не подтвержден
                    return View("ForgotPasswordConfirmation");
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Не показывать, что пользователь не существует
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Запрос перенаправления к внешнему поставщику входа
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Создание и отправка маркера
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет учетной записи, то ему предлагается создать ее
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Получение сведений о пользователе от внешнего поставщика входа
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session["login"] = null;
            Session["isAuth"] = false;
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}