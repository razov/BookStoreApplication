using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BookStoreApplication.Models;

namespace BookStoreApplication.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // model - модель нового пользователя
        // roles - массив ролей пользователя
        #region Создаем нового пользователя с набором ролей roles
        [NonAction]
        public async Task<IHttpActionResult> CreateUser(RegisterBindingModel model, params string[] roles)
        {
            if (roles != null)
            {
                foreach (string role in roles)
                {
                    var roleUser = RoleManager.FindByName(role);
                    if (roleUser == null)
                    {
                        ModelState.AddModelError("roles", String.Format("Роль {0} не обнаружена в списке доступных ролей", role));
                    }
                }
            }
            else
            {
                ModelState.AddModelError("model.Roles", "В запросе необходимо указать массив ролей");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            foreach (string role in roles)
            {
                var roleUser = RoleManager.FindByName(role);
                UserManager.AddToRole(user.Id, role);
            }

            return Ok();
        }
        #endregion

        //POST api/Account/Register
        #region Регистрация пользователя
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            // Для каждого нового пользователя добавляем роль user
            return await CreateUser(model, "user");
        }
        #endregion

        //POST api/Account/
        #region Создание пользователя администратором
        [Authorize(Roles = "admin")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser(NewUserBindingModel model)
        {
            RegisterBindingModel modelParam = null;
            string[] roles = null;
            if (model != null)
            {
                modelParam = (RegisterBindingModel)model;
                roles = model.Roles;
                return await CreateUser(modelParam, roles);
            }
            else
            {
                ModelState.AddModelError("model","Данные для добавления пользователя отсутствуют");
                return BadRequest(ModelState);
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // Ошибки ModelState для отправки отсутствуют, поэтому просто возвращается пустой BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #endregion
    }
}
