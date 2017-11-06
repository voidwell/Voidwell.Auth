using System.Threading.Tasks;
using Voidwell.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Voidwell.Auth.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly ISecurityQuestionService _securityQuestionService;

        public RegistrationService(SignInManager<ApplicationUser> signInManager, IUserService userService, ISecurityQuestionService securityQuestionService)
        {
            _signInManager = signInManager;
            _userService = userService;
            _securityQuestionService = securityQuestionService;
        }

        public async Task<ApplicationUser> RegisterNewUserAsync(RegistrationForm registration)
        {
            _securityQuestionService.ValidateSecurityQuestions(registration.SecurityQuestions);

            var user = await _userService.CreateUser(registration.Username, registration.Email, registration.Password);

            await _securityQuestionService.CreateSecurityQuestions(user.Id, registration.SecurityQuestions);

            await _signInManager.SignInAsync(user, true);

            return user;
        }
    }
}
