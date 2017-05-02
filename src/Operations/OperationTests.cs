using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Operations.Linq;

namespace Operations
{
    public class OperationTests
    {
        public static IOperation<Family> BuildFamily =>
            from yuni in People.Someone("Sergey", isHappy: true)
            from sergey in People.Someone("Yuni", isHappy: true)
            from couple in People.MakeCouple(yuni, sergey)
            let happyCouple = couple.Happy()
            from ourKid in People.Someone("No name yet", isHappy: true)
            from family in People.MakeFamily(couple: happyCouple, kids: new List<Person> { ourKid })
            let happyKids = from kid in family.Kids where kid.IsHappy select kid
            where happyCouple.IsHappy
            where family.Kids.Any()
            where yuni.IsHappy && sergey.IsHappy && ourKid.IsHappy
            select family;

        public static async Task TestBuildFamily()
        {
            Console.WriteLine("Building a family builder...");
            var familyBuilder = OperationTests.BuildFamily;
            Console.WriteLine("Building the family...");
            var family = await familyBuilder.ExecuteAsync();
            if (!family.Succeeded)
            {
                Console.WriteLine("Family is not succeeded");
            }
            Console.WriteLine(family.Result);
        }

        public class Person
        {
            public string Name { get; }
            public bool IsHappy { get; }

            public Person(string name, bool isHappy)
            {
                Console.WriteLine($"Getting a person {name}...");
                Name = name;
                IsHappy = isHappy;
            }

            public override string ToString() => Name;
        }

        public class Couple
        {
            public Person Wife { get; }
            public Person Husband { get; }
            public bool IsHappy { get; private set; }

            public Couple(Person wife, Person husband)
            {
                Wife = wife;
                Husband = husband;
                Console.WriteLine($"Creating a couple {this}...");
            }

            public Couple Happy()
            {
                Console.WriteLine($"Making the couple {this} happy...");
                IsHappy = true;
                return this;
            }

            public override string ToString() => $"<{Wife} + {Husband}>";
        }

        public class Family
        {
            public IEnumerable<Person> Kids { get; set; }
            public Person Wife { get; }
            public Person Husband { get; }
            public bool IsHappy { get; private set; }

            public Family(Person wife, Person husband, IEnumerable<Person> kids)
            {
                Wife = wife;
                Husband = husband;
                Kids = kids ?? new List<Person>();
                Console.WriteLine($"Creating a family {this}...");
            }

            public Family Happy()
            {
                Console.WriteLine($"Making the family {this} happy...");
                IsHappy = true;
                return this;
            }

            public override string ToString() =>
                $"<{Wife} + {Husband} + kids: " +
                String.Join("+", Kids.Select(kid => kid.Name)) + ">";
        }

        public static class People
        {
            public static IOperation<Person> Someone(string name, bool isHappy)
                => Operation.Return(() => new Person(name, isHappy));

            public static IOperation<Couple> MakeCouple(Person wife, Person husband)
                => Operation.Return(() => new Couple(wife, husband));

            public static IOperation<Family> MakeFamily(Couple couple, IEnumerable<Person> kids)
                => Operation.Return(() => new Family(couple.Wife, couple.Husband, kids));
        }
    }
}