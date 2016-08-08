using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using BookStoreApplication.Models;
using System.Data.Entity;

namespace BookStoreApplication
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationUserManager
        : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser, ApplicationRole, string,
                    ApplicationUserLogin, ApplicationUserRole,
                    ApplicationUserClaim>(context.Get<ApplicationDbContext>()));
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationDbInitializer
        : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new ApplicationUserStore(context));
            var roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));

            string[] roles = new string[3] { "admin", "manager", "user" };

            foreach (string role in roles)
            {
                var roleObject = roleManager.FindByName(role);
                if (roleObject == null)
                {
                    ApplicationRole newRole = new ApplicationRole(role);
                    var roleresult = roleManager.Create(newRole);
                }
            }
            // Имя пользователя администратора
            const string name = "admin@admin";
            // Пароль администратора
            const string password = "Admin@123456";
            var user = userManager.FindByName(name);
            if (user == null)
            {
                var userObject = new ApplicationUser { UserName = name, Email = name };
                var result = userManager.Create(userObject, password);
                result = userManager.SetLockoutEnabled(userObject.Id, false);
                userManager.AddToRoles(userManager.FindByEmail(name).Id, "admin", "user", "manager");
            }
            base.Seed(context);
        }

        public ApplicationDbInitializer()
        {
        }
    }
}