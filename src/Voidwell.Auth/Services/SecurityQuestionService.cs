using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Auth.Constants;
using Voidwell.Auth.Exceptions;
using Voidwell.Auth.Models;
using Voidwell.Auth.Data;

namespace Voidwell.Auth.Services
{
    public class SecurityQuestionService : ISecurityQuestionService
    {
        private readonly Func<AuthDbContext> _dbContextFactory;
        private readonly ILogger<SecurityQuestionService> _logger;

        public SecurityQuestionService(Func<AuthDbContext> dbContextFactory, ILogger<SecurityQuestionService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<SecurityQuestion>> GetSecurityQuestions(Guid userId)
        {
            var dbContext = _dbContextFactory();

            var questions = await dbContext.SecurityQuestions.Where(a => a.UserId == userId)
                .ToListAsync();

            if (questions == null)
                return null;

            return questions.Select(q => new SecurityQuestion { Question = q.Question, Answer = q.Answer });
        }

        public async Task<IEnumerable<SecurityQuestion>> CreateSecurityQuestions(Guid userId, IEnumerable<SecurityQuestion> securityQuestions)
        {
            var dbQuestions = securityQuestions.Select(q =>
            {
                return new Auth.Data.Models.SecurityQuestion
                {
                    UserId = userId,
                    Question = q.Question,
                    Answer = q.Answer
                };
            });

            var dbContext = _dbContextFactory();

            await dbContext.SecurityQuestions.AddRangeAsync(dbQuestions);
            await dbContext.SaveChangesAsync();

            return securityQuestions;
        }

        public async Task RemoveSecurityQuestions(Guid userId)
        {
            var dbContext = _dbContextFactory();

            var questions = await dbContext.SecurityQuestions.Where(q => q.UserId == userId)
                .ToListAsync();

            if (questions == null)
                return;

            dbContext.SecurityQuestions.RemoveRange(questions);
            await dbContext.SaveChangesAsync();
        }

        public IEnumerable<string> GetSecurityQuestionsList()
        {
            return SecurityQuestions.SecurityQuestionsList;
        }

        public void ValidateSecurityQuestions(IEnumerable<SecurityQuestion> questions)
        {
            var allQuestions = GetSecurityQuestionsList();

            foreach(var question in questions)
            {
                if (!questions.Contains(question))
                {
                    _logger.LogWarning($"Invalid security question \"{question}\" was used");
                    throw new InvalidSecurityQuestionException();
                }
            }
        }
    }
}
