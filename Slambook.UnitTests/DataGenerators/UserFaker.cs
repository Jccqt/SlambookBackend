using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using SlambookBackend.Models;

namespace Slambook.UnitTests.DataGenerators
{
    public class UserFaker : Faker<Users>
    {
        public UserFaker()
        {
            RuleFor(u => u.Id, f => f.IndexFaker + 1);
            RuleFor(u => u.FirstName, f => f.Name.FirstName());
            RuleFor(u => u.LastName, f => f.Name.LastName());
            RuleFor(u => u.Bio, f => f.Lorem.Sentence(1));
            RuleFor(u => u.Username, (f, u) => f.Internet.UserName(u.FirstName, u.LastName));
            RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName));
            RuleFor(u => u.Password, f => f.Internet.Password(12));
            RuleFor(u => u.Salt, f => f.Random.AlphaNumeric(16));
            RuleFor(u => u.ProfilePicture, f => f.Random.Bytes(1024));
            RuleFor(u => u.Status, f => f.PickRandom(0, 1));
            RuleFor(u => u.LoginCount, f => f.Random.Int(0, 100));
            RuleFor(u => u.Slambooks, f => new List<Slambooks>());
        }
    }
}
