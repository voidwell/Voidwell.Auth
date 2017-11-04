using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Voidwell.Auth.Data.Models;

namespace Voidwell.Auth.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserService _userService;
        private readonly ISecurityQuestionService _securityQuestionService;

        public RegistrationService(IUserService userService, ISecurityQuestionService securityQuestionService)
        {
            _userService = userService;
            _securityQuestionService = securityQuestionService;
        }

        public async Task<User> RegisterNewUserAsync(RegistrationForm registration)
        {
            _securityQuestionService.ValidateSecurityQuestions(registration.SecurityQuestions);

            var user = await _userService.CreateUser(registration.Username, registration.Email, registration.Password);

            await _securityQuestionService.CreateSecurityQuestions(user.Id, registration.SecurityQuestions);

            return user;
        }
    }
}
