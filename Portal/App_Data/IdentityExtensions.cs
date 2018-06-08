using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PgDbase.entity;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal
{
    public static class IdentityExtensions
    {
        public static string GetUserFullName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FullName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static bool HasClaim(this IIdentity identity, string claimType, string claimValue)
        {
            var claims = ((ClaimsIdentity)identity).FindAll(claimType);
            // Test for null to avoid issues during local testing
            if (claims.Any())
                foreach (var claim in claims)
                {
                    if (claim.Value.ToLower() == claimValue.ToLower())
                        return true;
                }
            return false;
        }

    }
}