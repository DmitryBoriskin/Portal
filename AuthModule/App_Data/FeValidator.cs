using AuthModule.Auth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AuthModule
{
    public class FeValidator : UserValidator<ApplicationUser>
    {
        public FeValidator(UserManager<ApplicationUser, string> manager) 
            : base(manager)
        {
        }

        //List<string> _allowedEmailDomains = new List<string> { "outlook.com", "hotmail.com", "gmail.com", "yahoo.com" };

        //public FeValidator(ApplicationUserManager appUserManager)
        //    : base(appUserManager)
        //{
        //}

        public override async Task<IdentityResult> ValidateAsync(ApplicationUser user)
        {
            
            IdentityResult result = await base.ValidateAsync(user);
            //var errors = new List<IdentityResult>();
            //var emailDomain = user.Email.Split('@')[1];
            //if (!_allowedEmailDomains.Contains(emailDomain.ToLower()))
            //{
            //    var errors = result.Errors.ToList();
            //    errors.Add(String.Format("Email domain '{0}' is not allowed", emailDomain));
            //    result = new IdentityResult(errors);
            //}

            return result;
        }
    }
}