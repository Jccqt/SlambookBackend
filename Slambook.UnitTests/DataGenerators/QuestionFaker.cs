using Bogus;
using SlambookBackend.Models;

namespace Slambook.UnitTests.DataGenerators
{
    public class QuestionFaker : Faker<Questions>
    {
        public QuestionFaker()
        {
            UseSeed(1337);

            RuleFor(q => q.Id, f => f.IndexFaker + 1);
            RuleFor(q => q.SlambookId, f => f.Random.Int(1, 100));
            RuleFor(q => q.QuestionText, f => f.Lorem.Sentence(6));
            RuleFor(q => q.Answers, _ => new List<Answers>());
        }
    }
}
