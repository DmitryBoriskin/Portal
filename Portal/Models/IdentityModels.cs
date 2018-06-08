using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Validation;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System;
using System.Globalization;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    #region aspnet.identity.entytyframework base entities
    //base
    //public class IdentityUserClaim<TKey>
    //{
    //    //public IdentityUserClaim();
    //    public virtual string ClaimType { get; set; }
    //    public virtual string ClaimValue { get; set; }
    //    public virtual int Id { get; set; }
    //    public virtual TKey UserId { get; set; }
    //}

    //public class IdentityUser<TKey, TLogin, TRole, TClaim> : IUser<TKey>
    //    where TLogin : IdentityUserLogin<TKey>
    //    where TRole : IdentityUserRole<TKey>
    //    where TClaim : IdentityUserClaim<TKey>
    //{
    //    public virtual int AccessFailedCount { get; set; }
    //    public virtual ICollection<TClaim> Claims { get; }
    //    public virtual string Email { get; set; }
    //    public virtual bool EmailConfirmed { get; set; }
    //    public virtual TKey Id { get; set; }
    //    public virtual bool LockoutEnabled { get; set; }
    //    public virtual DateTime? LockoutEndDateUtc { get; set; }
    //    public virtual ICollection<TLogin> Logins { get; }
    //    public virtual string PasswordHash { get; set; }
    //    public virtual string PhoneNumber { get; set; }
    //    public virtual bool PhoneNumberConfirmed { get; set; }
    //    public virtual ICollection<TRole> Roles { get; }
    //    public virtual string SecurityStamp { get; set; }
    //    public virtual bool TwoFactorEnabled { get; set; }
    //    public virtual string UserName { get; set; }
    //}
    #endregion

    [Table("AspNetRoleClaims", Schema ="core")]
    public class ApplicationRoleClaim
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        //[ForeignKey("RoleId")]
        //public virtual ICollection<ApplicationRole> Role { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }

        //public ApplicationRole(string name, string description) : base(name)
        //{
        //    this.Description = description;
        //}
        //public virtual string Description { get; set; }

        public virtual ICollection<ApplicationRoleClaim> Claims { get; set; }
    }

    [Table("AspNetUserProfiles", Schema = "core")]
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public bool Disabled { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegDate { get; set; }

        public string FullName
        {
            get
            {
                return $"{this.Surname} {this.Name} {this.Patronymic}";
            }
        }

        public virtual ApplicationUser User { get; set; }
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Id in Guid format
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// siteId in Guid format
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Доп инфо
        /// </summary>
        public virtual UserProfile UserInfo { get; set; }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        //public ICollection<ApplicationRole> UserRoles { get; set; }

        /// <summary>
        /// Инициализация UserIdentity
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            ApplicationDbContext _dbContext = new ApplicationDbContext();

            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FullName", UserInfo.FullName));
            userIdentity.AddClaim(new Claim("SiteId", SiteId.ToString()));
            userIdentity.AddClaim(new Claim("Email", Email));
            //userIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth, UserInfo.BirthDate.ToString()));

            //Названия ролей пользователя ( string[]{"Admin","User"})
            var userRoles = userIdentity.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value).ToList();

            //Добавляем пользователю все Claims группы
            if (userRoles.Any())
            {
                var uRoles = _dbContext.Roles
                    .Where(r => userRoles.Any(ur => ur == r.Name))
                    .Select(r => r.Id)
                    .ToList();

                //Находим Claims для каждой из ролей
                var roleClaims = _dbContext.RoleClaims
                    //.Where(x => userRoles.Any(role => role == x.Role.Name)).ToList()
                    .Where(x => uRoles.Any(role => role == x.RoleId))
                    .ToList();

                //var roleClaims = t.Select(x => new Claim(x.ClaimType, x.ClaimValue));

                //Применяем к пользователю
                if (roleClaims.Any())
                    foreach (var claim in roleClaims)
                    {
                        //Права на сайты, которые есть у роли
                        if (claim.ClaimType.ToLower() == "_siteidentity")
                        {
                            userIdentity.AddClaim(new Claim(ClaimTypes.Role, claim.ClaimValue));
                        }
                        else
                            userIdentity.AddClaim(new Claim(claim.ClaimType, claim.ClaimValue));
                    }
            }

            return userIdentity;

        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("dbConnection", throwIfV1Schema: false) { }

        public DbSet<UserProfile> UserInfo { get; set; }
        public DbSet<ApplicationRoleClaim> RoleClaims { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            base.OnModelCreating(modelBuilder);

            #region  base.OnModelCreating
            //-------------------------------------------------
            //var user = modelBuilder.Entity<ApplicationUser>()
            //    .ToTable("AspNetUsers");
            //user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            //user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            //user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            //user.Property(u => u.UserName)
            //    .IsRequired()
            //    .HasMaxLength(256)
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));

            //// CONSIDER: u.Email is Required if set on options?
            //user.Property(u => u.Email).HasMaxLength(256);

            //modelBuilder.Entity<TUserRole>()
            //    .HasKey(r => new { r.UserId, r.RoleId })
            //    .ToTable("AspNetUserRoles");

            //modelBuilder.Entity<Login>()
            //    .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
            //    .ToTable("AspNetUserLogins");

            //modelBuilder.Entity<TUserClaim>()
            //    .ToTable("AspNetUserClaims");

            //var role = modelBuilder.Entity<TRole>()
            //    .ToTable("AspNetRoles");
            //role.Property(r => r.Name)
            //    .IsRequired()
            //    .HasMaxLength(256)
            //    .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
            //role.HasMany(r => r.Users).WithRequired().HasForeignKey(ur => ur.RoleId);
            //-------------------------------------------------
            #endregion

            // If you are letting EntityFrameowrk to create the database, 
            // it will by default create the __MigrationHisotry table in the dbo schema
            // Use HasDefaultSchema to specify alternative (i.e public) schema
            modelBuilder.HasDefaultSchema("core");

            var user = modelBuilder.Entity<ApplicationUser>()
                .ToTable("AspNetUsers");
            user.HasMany(u => u.Roles).WithRequired().HasForeignKey(ur => ur.UserId);
            user.HasMany(u => u.Claims).WithRequired().HasForeignKey(uc => uc.UserId);
            user.HasMany(u => u.Logins).WithRequired().HasForeignKey(ul => ul.UserId);
            user.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = true }));
            user.HasRequired(e => e.UserInfo)
                .WithRequiredPrincipal(c => c.User)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ApplicationRoleClaim>()
               .ToTable("AspNetRoleClaims");

            var role = modelBuilder.Entity<ApplicationRole>()
                .ToTable("AspNetRoles");
            role.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("RoleNameIndex") { IsUnique = true }));
            role.HasMany(u => u.Claims)
                .WithRequired()
                .HasForeignKey(uc => uc.RoleId);

            //var userNameProp = modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers").Property(p => p.UserName);
        }


        //protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        //{
        //    if (entityEntry != null && entityEntry.State == EntityState.Added)
        //    {
        //        var errors = new List<DbValidationError>();
        //        var user = entityEntry.Entity as ApplicationUser;
        //        //check for uniqueness of user name and email
        //        if (user != null)
        //        {
        //            if (Users.Any(u => String.Equals(u.UserName, user.UserName) && u.SiteId == user.SiteId))
        //            {
        //                //errors.Add(new DbValidationError("User", String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateUserName, user.UserName)));
        //                errors.Add(new DbValidationError("User", String.Format("Пользователь с именем {0} уже существует", user.UserName)));
        //            }
        //            if (RequireUniqueEmail && Users.Any(u => String.Equals(u.Email, user.Email) && u.SiteId == user.SiteId))
        //            {
        //                //errors.Add(new DbValidationError("User", String.Format(CultureInfo.CurrentCulture, IdentityResources.DuplicateEmail, user.Email)));
        //                errors.Add(new DbValidationError("User", String.Format("Пользователь с email {0} уже существует", user.UserName)));
        //            }
        //        }
        //        else
        //        {
        //            //var role = entityEntry.Entity as TRole;
        //            ////check for uniqueness of role name
        //            //if (role != null && Roles.Any(r => String.Equals(r.Name, role.Name)))
        //            //{
        //            //    errors.Add(new DbValidationError("Role", String.Format(CultureInfo.CurrentCulture, IdentityResources.RoleAlreadyExists, role.Name)));
        //            //}
        //        }
        //        if (errors.Any())
        //        {
        //            return new DbEntityValidationResult(entityEntry, errors);
        //        }
        //    }
        //    return base.ValidateEntity(entityEntry, items);
        //}
    }

}