using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DragonCon.Logical.Identities;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Identities;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;

namespace DragonCon.RavenDB.Identities
{
    public class RavenIdentityFacade : IIdentityFacade, IDisposable
    {
        private readonly IAsyncDocumentSession _session;
        private readonly UserManager<EmailIdentity> _userManager;
        private readonly SignInManager<EmailIdentity> _signInManager;

        public RavenIdentityFacade(
            UserManager<EmailIdentity> userManager,
            SignInManager<EmailIdentity> signInManager,
            IAsyncDocumentSession session)
        {
            _session = session;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Basic Getters
        private async Task<string> GetParticipantIdByUsername(string username)
        {
            var identity = await _userManager.FindByEmailAsync(username);
            return identity?.LongTermId ?? string.Empty;
        }

        public async Task<LongTermParticipant> GetParticipantByUsernameAsync(string username)
        {
            return await GetParticipantByIdAsync(await GetParticipantIdByUsername(username));
        }

        public async Task<LongTermParticipant> GetParticipantByIdAsync(string id)
        {
            return await _session.LoadAsync<LongTermParticipant>(id);
        }
        #endregion

        #region Translators
        private string TranslateErrors(SignInResult result)
        {
            var sb = new StringBuilder();
            if (result.IsLockedOut)
            {
                sb.AppendLine("Locked Out");
            }
            if (result.IsNotAllowed)
            {
                sb.AppendLine("Not Allowed");
            }
            if (result.RequiresTwoFactor)
            {
                sb.AppendLine("Two Factor Required");
            }
            return sb.ToString();
        }
        private string TranslateErrors(IdentityResult result)
        {
            var sb = new StringBuilder();

            foreach (var identityError in result.Errors)
            {
                sb.AppendLine($"{identityError.Code}: {identityError.Description}");
            }

            return sb.ToString();
        }
        #endregion

        #region Add Participants
        public async Task<IdentityResults.AddParticipantResult> AddNewParticipant(IParticipant user, string password = "")
        {
            if (user is ShortTermParticipant shortP)
            {
                return await AddShortTermParticipant(shortP);
            }

            if (user is LongTermParticipant longP)
            {
                return await AddLongTermParticipant(longP, password);
            }

            throw new Exception("Unknown Type");
        }

        private async Task<IdentityResults.AddParticipantResult> AddLongTermParticipant(LongTermParticipant user,
            string password = "")
        {
            if ((await HasUserWithEmail(user.Email)).IsSuccess)
                return IdentityResults.AddParticipantResult.Fail("קיים משתמש עם כתובת דואר אלקטרוני זו");

            if (password.IsEmptyString())
            {
                password = new RandomPasswordGenerator(8).Generate();
            }

            await _session.StoreAsync(user);
            var ident = new EmailIdentity()
            {
                LongTermId = user.Id,
                Email = user.Email.ToLowerInvariant(),
                UserName = user.Email.ToLowerInvariant(),
            };

            var createUserResult = await _userManager.CreateAsync(ident, password);
            if (createUserResult.Succeeded)
            {
                return IdentityResults.AddParticipantResult.Success(true, password);
            }
            else
            {
                return IdentityResults.AddParticipantResult.Fail(TranslateErrors(createUserResult));

            }

        }

        private async Task<IdentityResults.SignInResult> HasUserWithEmail(string userEmail)
        {
            var storeUser = await _userManager.FindByEmailAsync(userEmail);
            return storeUser == null ? IdentityResults.SignInResult.Fail() : IdentityResults.SignInResult.Success();
        }

        private async Task<IdentityResults.AddParticipantResult> AddShortTermParticipant(ShortTermParticipant user)
        {
            await _session.StoreAsync(user);
            await _session.SaveChangesAsync();
            return IdentityResults.AddParticipantResult.Success(false, string.Empty);
        }

        public async Task<IdentityResults.UpdateParticipantResult> UpdateParticipant(IParticipant user)
        {
            if (user is ShortTermParticipant shortP)
            {
                return await UpdateShortTermParticipant(shortP);
            }

            if (user is LongTermParticipant longP)
            {
                return await UpdateLongTermParticipant(longP);
            }

            throw new Exception("Unknown Type");
        }

        private async Task<IdentityResults.UpdateParticipantResult> UpdateShortTermParticipant(ShortTermParticipant user)
        {
            var shortTerm = await _session.LoadAsync<ShortTermParticipant>(user.Id);
            shortTerm.YearOfBirth = user.YearOfBirth;
            shortTerm.FullName = user.FullName;
            shortTerm.PhoneNumber = user.PhoneNumber;
            await _session.StoreAsync(shortTerm);
            await _session.SaveChangesAsync();
            return IdentityResults.UpdateParticipantResult.Success(false, false);
        }

        private async Task<IdentityResults.UpdateParticipantResult> UpdateLongTermParticipant(LongTermParticipant user)
        {
            var longTerm = await _session.LoadAsync<LongTermParticipant>(user.Id);

            longTerm.YearOfBirth = user.YearOfBirth;
            longTerm.FullName = user.FullName;
            longTerm.PhoneNumber = user.PhoneNumber;
            longTerm.IsAllowingPromotions = user.IsAllowingPromotions;

            if (longTerm.Email.ToLowerInvariant() != user.Email.ToLowerInvariant())
            {
                var identity = await _userManager.FindByEmailAsync(user.Email.ToLower());
                identity.Email = user.Email.ToLowerInvariant();
                await _userManager.UpdateAsync(identity);
                return IdentityResults.UpdateParticipantResult.Success(true, true);
            }
            else
            {
                await _session.StoreAsync(longTerm);
                await _session.SaveChangesAsync();
                return IdentityResults.UpdateParticipantResult.Success(true, false);
            }
        }

        public async Task<IdentityResults.UpdateParticipantResult> UpdateLongTermRoles(
            LongTermParticipant user,
            IEnumerable<SystemRoles> roles)
        {
            var identity = await _userManager.FindByEmailAsync(user.Email.ToLower());
            identity.SystemRoles.Clear();
            identity.SystemRoles.AddRange(roles);
            var result = await _userManager.UpdateAsync(identity);
            if (result.Succeeded)
            {
                return IdentityResults.UpdateParticipantResult.Success(true, false);
            }
            else
            {
                return IdentityResults.UpdateParticipantResult.Fail(TranslateErrors(result));

            }
        }

        public async Task<IEnumerable<SystemRoles>> GetRolesForLongTerm(LongTermParticipant user)
        {
            var identity = await _userManager.FindByEmailAsync(user.Email.ToLower());
            return identity.SystemRoles;

        }

        #endregion

        #region Password
        public async Task<IdentityResults.PasswordResults> GeneratePasswordResetTokenAsync(LongTermParticipant user)
        {
            var storeUser = await _userManager.FindByEmailAsync(user.Email);
            if (storeUser is null)
                return IdentityResults.PasswordResults.Fail("Missing Identity");

            var result = await _userManager.GeneratePasswordResetTokenAsync(storeUser);
            await _session.SaveChangesAsync();

            return IdentityResults.PasswordResults.Success(result);
        }

        public async Task<IdentityResults.PasswordResults> ChangePasswordAsync(LongTermParticipant user, string currentPassword,
            string newPassword)
        {
            var storeUser = await _userManager.FindByEmailAsync(user.Email);
            if (storeUser is null)
                return IdentityResults.PasswordResults.Fail("Missing Identity");

            var result = await _userManager.ChangePasswordAsync(storeUser, currentPassword, newPassword);

            if (result.Succeeded)
            {
                await _session.SaveChangesAsync();
                return IdentityResults.PasswordResults.Success();
            }
            else
                return IdentityResults.PasswordResults.Fail(TranslateErrors(result));
        }

        public async Task<IdentityResults.PasswordResults> SetPasswordAsync(LongTermParticipant user, string newPassword)
        {
            var storeUser = await _userManager.FindByEmailAsync(user.Email);
            if (storeUser is null)
                return IdentityResults.PasswordResults.Fail("Missing Identity");

            var result = await _userManager.RemovePasswordAsync(storeUser);
            if (result.Succeeded == false)
                return IdentityResults.PasswordResults.Fail(TranslateErrors(result));

            result = await _userManager.AddPasswordAsync(storeUser, newPassword);
            if (result.Succeeded)
            {
                await _session.SaveChangesAsync();
                return IdentityResults.PasswordResults.Success();
            }
            else
                return IdentityResults.PasswordResults.Fail(TranslateErrors(result));
        }

        public async Task<IdentityResults.PasswordResults> ResetPasswordAsync(LongTermParticipant user, string token, string newPassword)
        {
            var storeUser = await _userManager.FindByEmailAsync(user.Email);
            if (storeUser is null)
                return IdentityResults.PasswordResults.Fail("Missing Identity");


            var result = await _userManager.ResetPasswordAsync(storeUser, token, newPassword);
            await _session.SaveChangesAsync();
            if (result.Succeeded)
            {
                await _session.SaveChangesAsync();
                return IdentityResults.PasswordResults.Success();
            }
            else
                return IdentityResults.PasswordResults.Fail(TranslateErrors(result));
        }


        #endregion

        #region Sign-In
        public async Task<IdentityResults.SignInResult> LoginAsync(string username, string password, bool rememberMe)
        {
            var storeUser = await _userManager.FindByEmailAsync(username);
            if (storeUser is null)
                return IdentityResults.SignInResult.Fail("Missing Identity");

            var signinResult = await _signInManager.PasswordSignInAsync(storeUser, password, rememberMe, false);
            if (signinResult.Succeeded)
            {
                await _session.SaveChangesAsync();
                return IdentityResults.SignInResult.Success();
            }
            else
                return IdentityResults.SignInResult.Fail(TranslateErrors(signinResult));
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            await _session.SaveChangesAsync();
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
