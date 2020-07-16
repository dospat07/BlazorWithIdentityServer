using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

using System.Security.Claims;
using System.Threading.Tasks;

namespace is4inmem
{
    public class ADProfileService : IProfileService
    {
      

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            UserPrincipal user = FindUser(context.Subject.GetDisplayName());
            if (user != null)
            {
                var claims = new List<Claim>()
                {
                new Claim(JwtClaimTypes.Name, user.Name),
                new Claim(JwtClaimTypes.GivenName, user.Name),
                new Claim(JwtClaimTypes.FamilyName,user.Name),

                };

                context.IssuedClaims = claims;
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {

            UserPrincipal user  = FindUser(context.Subject.Identity.Name);
            if(user == null)
            {
                return Task.CompletedTask;
            }
            // To be active, user must be enabled and not locked out

            var isLocked = user.IsAccountLockedOut();

            context.IsActive = (bool)(user.Enabled & !isLocked);

            return Task.CompletedTask;
        }


        private static UserPrincipal FindUser(string userName)
        {
            if (userName == null) return null;
            var principalContext = new PrincipalContext(ContextType.Machine);

            var userPrincipal = new UserPrincipal(principalContext) { SamAccountName = userName };
            var searcher = new PrincipalSearcher(userPrincipal);
            return searcher.FindOne() as UserPrincipal;

        }
    }
}