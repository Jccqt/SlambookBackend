using Bogus;
using SlambookBackend.Models;

namespace Slambook.UnitTests.DataGenerators
{
    public class AnswerFaker : Faker<Answers>
    {
        public AnswerFaker()
        {
            UseSeed(1337);

            RuleFor(a => a.Id, f => f.IndexFaker + 1);
            RuleFor(a => a.QuestionId, f => f.Random.Int(1, 100));
            RuleFor(a => a.ResponderId, f => f.Random.Int(1, 100));
            RuleFor(a => a.AnswerText, f => f.Lorem.Sentence());
            RuleFor(a => a.Status, f => f.PickRandom(0, 1));
        }
    }
}
