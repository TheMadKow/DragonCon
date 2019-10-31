using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Logical;
using DragonCon.Logical.Identities;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Identities
{
    public class RavenIdentityFacade : IIdentityFacade, IDisposable
    {
        private readonly IAsyncDocumentSession _session;
        private readonly UserManager<LongTermParticipant> _userManager;
        private readonly SignInManager<LongTermParticipant> _signInManager;

        public RavenIdentityFacade(
            UserManager<LongTermParticipant> userManager,
            SignInManager<LongTermParticipant> signInManager,
            IAsyncDocumentSession session)
        {
            _session = session;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region User
        public async Task<IdentityResults.Password> AddNewParticipant(IParticipant user)
        {
            if (user is ShortTermParticipant shortP)
            {
                return await AddShortTermParticipant(shortP);
            }

            if (user is LongTermParticipant longP)
            {
                return await AddLongTermParticipant(longP);
            }

            throw new Exception("Unknown Participant Type");
        }

        private async Task<IdentityResults.Password> AddLongTermParticipant(LongTermParticipant user)
        {
            if ((await HasUserWithEmail(user.Email)).IsSuccess)
                return new IdentityResults.Password()
                {
                    IsSuccess = false,
                    Errors = new[] {"User already exists"}
                };

            await _session.StoreAsync(user);
            await _session.SaveChangesAsync();

            var newPass = new RandomPasswordGenerator(minimumLengthPassword: 12).Generate();
            var res = await SetPasswordAsync(user, newPass);
            return res;
        }

        private async Task<IdentityResults.Password> AddShortTermParticipant(ShortTermParticipant user)
        {
            await _session.StoreAsync(user);
            await _session.SaveChangesAsync();
            return new IdentityResults.Password()
            {
                IsSuccess = true
            };
        }


        protected async Task<LongTermParticipant?> GetStoreUser(LongTermParticipant user)
        {
            return await GetStoreUser(user.UserName);
        }

        private async Task<LongTermParticipant?> GetStoreUser(string id)
        {
            var fetchedUser = await GetUserByUsernameAsync(id);
            if (fetchedUser is LongTermParticipant user)
                return user;
            return null;
        }

        public async Task<IParticipant> GetUserByUsernameAsync(string username)
        {
            return await _session.Query<LongTermParticipant>()
                .Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(60)))
                .FirstOrDefaultAsync(x => x.UserName == username);
        }
        #endregion

        #region Translators

        private IdentityResults.Password ToPassword(IdentityResult result)
        {
            var errors = result.Errors?.Select(x => $"{x.Code} : {x.Description}") ?? new List<string>();
            return new IdentityResults.Password
            {
                IsSuccess = result.Succeeded,
                Errors = errors.ToArray()
            };
        }

        private IdentityResults.SignIn ToSignIn(SignInResult result)
        {
            return new IdentityResults.SignIn
            {
                IsSuccess = result.Succeeded,
            };
        }
        #endregion

        #region Password
        public async Task<IdentityResults.Password> GeneratePasswordResetTokenAsync(LongTermParticipant user)
        {
            var storeUser = await GetStoreUser(user);
            if (storeUser is null)
                return new IdentityResults.Password {IsSuccess = false};

            var result = await _userManager.GeneratePasswordResetTokenAsync(storeUser);
            await _session.SaveChangesAsync();

            return new IdentityResults.Password
            {
                IsSuccess = true,
                Token = result
            };
        }

        public async Task<IdentityResults.Password> ChangePasswordAsync(LongTermParticipant user, string currentPassword,
            string newPassword)
        {
            var storeUser = await GetStoreUser(user);
            if (storeUser is null)
                return new IdentityResults.Password { IsSuccess = false };

            var result = await _userManager.ChangePasswordAsync(storeUser, currentPassword, newPassword);
            await _session.SaveChangesAsync();
            return ToPassword(result);
        }

        public async Task<IdentityResults.Password> SetPasswordAsync(LongTermParticipant user, string newPassword)
        {
            var storeUser = await GetStoreUser(user);
            if (storeUser is null)
                return new IdentityResults.Password { IsSuccess = false };

            await _session.SaveChangesAsync();
            var result = await _userManager.AddPasswordAsync(storeUser, newPassword);
            await _session.SaveChangesAsync();
            return ToPassword(result);
        }

        public async Task<IdentityResults.Password> ResetPasswordAsync(LongTermParticipant user, string token, string newPassword)
        {
            var storeUser = await GetStoreUser(user);
            if (storeUser is null)
                return new IdentityResults.Password { IsSuccess = false };

            var result = await _userManager.ResetPasswordAsync(storeUser, token, newPassword);
            await _session.SaveChangesAsync();
            return ToPassword(result);
        }


        #endregion

        #region Sign-In

        public async Task<IdentityResults.SignIn> LoginAsync(string username, string password, bool rememberMe)
        {
            var user = await GetStoreUser(username);
            if (user == null)
            {
                return IdentityResults.SignIn.Fail();
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);
            return ToSignIn(signinResult);
        }

        public async Task LogoutAsync(string username)
        {
            await _signInManager.SignOutAsync();
            await _session.SaveChangesAsync();
        }

        public async Task<IdentityResults.SignIn> HasUserWithEmail(string userEmail)
        {
            var result = await GetUserByUsernameAsync(userEmail);
            return result == null ? IdentityResults.SignIn.Fail() : IdentityResults.SignIn.Success();
        }
        #endregion

        #region Dispose
        ~RavenIdentityFacade()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _session?.Dispose();
            }
        }

        public void Dispose()
        {
            if (_session != null)
            {
                _session.SaveChangesAsync().GetAwaiter().GetResult();
                _session.Dispose();
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
