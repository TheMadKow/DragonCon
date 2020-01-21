using System.Collections.Generic;
using System.Threading.Tasks;
using DragonCon.Modeling.Models.Identities;

namespace DragonCon.Logical.Identities
{
    public static class IdentityResults
    {
        public class UpdateParticipantResult
        {
            public bool IsSuccess { get; set; }
            public bool IsEmailChange { get; set; }
            public string Details { get; set; } = string.Empty;


            public static UpdateParticipantResult Fail(string details = null) => new UpdateParticipantResult
            {
                IsSuccess = false,
                Details = details
            };

            public static UpdateParticipantResult Success(bool longTerm, bool emailChange) => new UpdateParticipantResult
            {
                IsSuccess = true,
                IsEmailChange = emailChange
            };
        }

        public class AddParticipantResult
        {
            public bool IsSuccess { get; set; }
            public bool IsLongTerm { get; set; }
            public string InitialPassword { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;

            public static AddParticipantResult Fail(string details = null) => new AddParticipantResult
            {
                IsSuccess = false,
                Details = details
            };

            public static AddParticipantResult Success(bool longTerm, string initialPassword) => new AddParticipantResult
            {
                IsSuccess = true,
                IsLongTerm = longTerm,
                InitialPassword = initialPassword
            };

        }
        

        public class SignInResult
        {
            public bool IsSuccess { get; set; }
            public string Details { get; set; } = string.Empty;

            public static SignInResult Fail(string details = null) => new SignInResult
            {
                IsSuccess = false,
                Details = details
            };

            public static SignInResult Success(string details = null) => new SignInResult
            {
                IsSuccess = true,
                Details = details
            };

        }

        public class PasswordResults
        {
            public bool IsSuccess { get; set; }
            public string Token { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;

            public static PasswordResults Fail(string details = null) => new PasswordResults
            {
                IsSuccess = false,
                Details = details
            };

            public static PasswordResults Success(string Token = null) => new PasswordResults
            {
                IsSuccess = true,
                Token = Token
            };
        }
    }

    public interface IIdentityFacade
    {
        // Getters
        Task<LongTermParticipant> GetParticipantByUsernameAsync(string username);
        Task<LongTermParticipant> GetParticipantByIdAsync(string id);

        // Participant
        Task<IdentityResults.AddParticipantResult> AddNewParticipant(IParticipant user, string password = "");
        Task<IdentityResults.UpdateParticipantResult> UpdateParticipant(IParticipant user);
        Task<IdentityResults.UpdateParticipantResult> UpdateLongTermRoles(LongTermParticipant user, IEnumerable<SystemRoles> roles);
        Task<IEnumerable<SystemRoles>> GetRolesForLongTerm(LongTermParticipant user);


        // Passwords
        Task<IdentityResults.PasswordResults> GeneratePasswordResetTokenAsync(LongTermParticipant user);
        Task<IdentityResults.PasswordResults> ChangePasswordAsync(LongTermParticipant user, string currentPassword, string newPassword);
        Task<IdentityResults.PasswordResults> SetPasswordAsync(LongTermParticipant user, string newPassword);
        Task<IdentityResults.PasswordResults> ResetPasswordAsync(LongTermParticipant user, string token, string newPassword);

        // Login & Logout
        Task<IdentityResults.SignInResult> LoginAsync(string username, string password, bool rememberMe);
        Task LogoutAsync();
    }
}
