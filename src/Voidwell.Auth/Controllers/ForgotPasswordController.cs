using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Auth.UserManagement.Models;
using Voidwell.Auth.UserManagement.Services.Abstractions;

namespace Voidwell.Auth.Controllers;

[Route("forgotpassword")]
[SecurityHeaders]
public class ForgotPasswordController : Controller
{
    private readonly ISecurityQuestionService _securityQuestionService;
    private readonly IUserService _userService;

    public ForgotPasswordController(ISecurityQuestionService securityQuestionService, IUserService userService)
    {
        _securityQuestionService = securityQuestionService;
        _userService = userService;
    }

    [HttpPost("start")]
    public async Task<ActionResult> PostResetStart([FromBody] ResetPasswordStart model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var questions = await _securityQuestionService.GetSecurityQuestionsByEmail(model.Email);
        if (questions == null)
        {
            return NotFound("No user was found with this email.");
        }

        var questionsWithoutAnswers = questions.Select(a => a.Question);

        return Ok(questionsWithoutAnswers);
    }

    [HttpPost("questions")]
    public async Task<ActionResult> PostResetQuestions([FromBody] ResetPasswordQuestions model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var questions = await _securityQuestionService.GetSecurityQuestionsByEmail(model.Email);

        foreach (var storedQuestion in questions)
        {
            var match = model.Questions.Any(a => a.Question == storedQuestion.Question && a.Answer.ToLower() == storedQuestion.Answer.ToLower());
            if (!match)
            {
                return BadRequest("One or more answers were invalid.");
            }
        }

        var user = await _userService.GetUserByEmail(model.Email);
        var resetToken = _userService.GetPasswordResetToken(user.Id);

        return Ok(resetToken);
    }

    [HttpPost("reset")]
    public async Task<ActionResult> PostReset([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userService.GetUserByEmail(request.Email);
        await _userService.ResetPassword(user.Id, request.Token, request.NewPassword);

        return Ok();
    }
}
