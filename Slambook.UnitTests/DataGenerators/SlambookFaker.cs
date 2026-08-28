using Bogus;
using SlambookBackend.Models;

namespace Slambook.UnitTests.DataGenerators
{
    public class SlambookFaker : Faker<Slambooks>
    {
        public SlambookFaker()
        {
            UseSeed(1337);

            RuleFor(s => s.Id, f => f.IndexFaker + 1);
            RuleFor(s => s.CreatorId, f => f.Random.Int(1, 100));
            RuleFor(s => s.Title, f => f.Lorem.Sentence(3));
            RuleFor(s => s.Description, f => f.Lorem.Paragraph());
            RuleFor(s => s.CreatedDate, f => DateOnly.FromDateTime(f.Date.Past(2)));
            RuleFor(s => s.Questions, _ => new List<Questions>());
        }
    }
}
