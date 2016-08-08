using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;

namespace BookStoreApplication.Models
{
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }

    // Модель пользователя
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.OrderDetails = new List<OrderDetails>();
        }

        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }
    }

    // Модель роли пользователя
    public class ApplicationRole: IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
          : this()
        {
            this.Name = name;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public ApplicationDbContext()
            : base("ConnectionStringName")
        {
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class ApplicationUserStore
        : UserStore<ApplicationUser, ApplicationRole, string,
            ApplicationUserLogin, ApplicationUserRole,
            ApplicationUserClaim>, IUserStore<ApplicationUser, string>,
        IDisposable
    {
        public ApplicationUserStore()
          : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
          : base(context)
        {
        }
    }

    public class ApplicationRoleStore
    : RoleStore<ApplicationRole, string, ApplicationUserRole>,
    IQueryableRoleStore<ApplicationRole, string>,
    IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
          : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
          : base(context)
        {
        }
    }
}