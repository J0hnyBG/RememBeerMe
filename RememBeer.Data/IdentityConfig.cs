﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

using RememBeer.Data.DbContexts;
using RememBeer.Data.DbContexts.Contracts;

namespace RememBeer.Data
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public interface IApplicationUserManager : IDisposable
    {
        Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType);

        Task<IdentityResult> CreateAsync(ApplicationUser user);

        Task<IdentityResult> UpdateAsync(ApplicationUser user);

        Task<IdentityResult> DeleteAsync(ApplicationUser user);

        Task<ApplicationUser> FindByIdAsync(string userId);

        Task<ApplicationUser> FindByNameAsync(string userName);

        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);

        Task<ApplicationUser> FindAsync(string userName, string password);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        Task<bool> HasPasswordAsync(string userId);

        Task<IdentityResult> AddPasswordAsync(string userId, string password);

        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task<IdentityResult> RemovePasswordAsync(string userId);

        Task<string> GetSecurityStampAsync(string userId);

        Task<IdentityResult> UpdateSecurityStampAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(string userId);

        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);

        Task<ApplicationUser> FindAsync(UserLoginInfo login);

        Task<IdentityResult> RemoveLoginAsync(string userId, UserLoginInfo login);

        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Task<IList<UserLoginInfo>> GetLoginsAsync(string userId);

        Task<IdentityResult> AddClaimAsync(string userId, Claim claim);

        Task<IdentityResult> RemoveClaimAsync(string userId, Claim claim);

        Task<IList<Claim>> GetClaimsAsync(string userId);

        Task<IdentityResult> AddToRoleAsync(string userId, string role);

        Task<IdentityResult> AddToRolesAsync(string userId, params string[] roles);

        Task<IdentityResult> RemoveFromRolesAsync(string userId, params string[] roles);

        Task<IdentityResult> RemoveFromRoleAsync(string userId, string role);

        Task<IList<string>> GetRolesAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<string> GetEmailAsync(string userId);

        Task<IdentityResult> SetEmailAsync(string userId, string email);

        Task<ApplicationUser> FindByEmailAsync(string email);

        Task<string> GenerateEmailConfirmationTokenAsync(string userId);

        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        Task<bool> IsEmailConfirmedAsync(string userId);

        Task<string> GetPhoneNumberAsync(string userId);

        Task<IdentityResult> SetPhoneNumberAsync(string userId, string phoneNumber);

        Task<IdentityResult> ChangePhoneNumberAsync(string userId, string phoneNumber, string token);

        Task<bool> IsPhoneNumberConfirmedAsync(string userId);

        Task<string> GenerateChangePhoneNumberTokenAsync(string userId, string phoneNumber);

        Task<bool> VerifyChangePhoneNumberTokenAsync(string userId, string token, string phoneNumber);

        Task<bool> VerifyUserTokenAsync(string userId, string purpose, string token);

        Task<string> GenerateUserTokenAsync(string purpose, string userId);

        void RegisterTwoFactorProvider(string twoFactorProvider, IUserTokenProvider<ApplicationUser, string> provider);

        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);

        Task<bool> VerifyTwoFactorTokenAsync(string userId, string twoFactorProvider, string token);

        Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider);

        Task<IdentityResult> NotifyTwoFactorTokenAsync(string userId, string twoFactorProvider, string token);

        Task<bool> GetTwoFactorEnabledAsync(string userId);

        Task<IdentityResult> SetTwoFactorEnabledAsync(string userId, bool enabled);

        Task SendEmailAsync(string userId, string subject, string body);

        Task SendSmsAsync(string userId, string message);

        Task<bool> IsLockedOutAsync(string userId);

        Task<IdentityResult> SetLockoutEnabledAsync(string userId, bool enabled);

        Task<bool> GetLockoutEnabledAsync(string userId);

        Task<DateTimeOffset> GetLockoutEndDateAsync(string userId);

        Task<IdentityResult> SetLockoutEndDateAsync(string userId, DateTimeOffset lockoutEnd);

        Task<IdentityResult> AccessFailedAsync(string userId);

        Task<IdentityResult> ResetAccessFailedCountAsync(string userId);

        Task<int> GetAccessFailedCountAsync(string userId);

        IPasswordHasher PasswordHasher { get; set; }

        IIdentityValidator<ApplicationUser> UserValidator { get; set; }

        IIdentityValidator<string> PasswordValidator { get; set; }

        IClaimsIdentityFactory<ApplicationUser, string> ClaimsIdentityFactory { get; set; }

        IIdentityMessageService EmailService { get; set; }

        IIdentityMessageService SmsService { get; set; }

        IUserTokenProvider<ApplicationUser, string> UserTokenProvider { get; set; }

        bool UserLockoutEnabledByDefault { get; set; }

        int MaxFailedAccessAttemptsBeforeLockout { get; set; }

        TimeSpan DefaultAccountLockoutTimeSpan { get; set; }

        bool SupportsUserTwoFactor { get; }

        bool SupportsUserPassword { get; }

        bool SupportsUserSecurityStamp { get; }

        bool SupportsUserRole { get; }

        bool SupportsUserLogin { get; }

        bool SupportsUserEmail { get; }

        bool SupportsUserPhoneNumber { get; }

        bool SupportsUserClaim { get; }

        bool SupportsUserLockout { get; }

        bool SupportsQueryableUsers { get; }

        IQueryable<ApplicationUser> Users { get; }

        IDictionary<string, IUserTokenProvider<ApplicationUser, string>> TwoFactorProviders { get; }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<RememBeerMeDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)this.UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
