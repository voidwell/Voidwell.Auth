using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.Services;
using System.Collections.Generic;

namespace Voidwell.VoidwellAuth.Client.Controllers
{
    [Route("account/questions")]
    public class SecurityQuestionsController : Controller
    {
        private readonly ISecurityQuestionService _securityQuestionService;

        public SecurityQuestionsController(ISecurityQuestionService securityQuestionService)
        {
            _securityQuestionService = securityQuestionService;
        }

        [HttpGet]
        public IEnumerable<string> GetSecurityQuestions()
        {
            return _securityQuestionService.GetSecurityQuestionsList();
        }
    }
}
