using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NativeBuffering;

namespace App3
{
    [BufferedMessageSource]
    public partial class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Hobbies { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Occupation { get; set; }
        public string EducationLevel { get; set; }
        public string MaritalStatus { get; set; }
        public string SpouseName { get; set; }
        public int NumberOfChildren { get; set; }
        public string[] ChildrenNames { get; set; }
        public string[] LanguagesSpoken { get; set; }
        public bool HasPets { get; set; }
        public string[] PetNames { get; set; }

        public static Person Instance = new Person
        {
            Name = "Bill",
            Age = 30,
            Hobbies = new string[] { "Reading", "Writing", "Coding" },
            Address = "123 Main St.",
            PhoneNumber = "555-555-5555",
            Email = "bill@gmail.com",
            Gender = "M",
            Nationality = "China",
            Occupation = "Software Engineer",
            EducationLevel = "Bachelor's",
            MaritalStatus = "Married",
            SpouseName = "Jane",
            NumberOfChildren = 2,
            ChildrenNames = new string[] { "John", "Jill" },
            LanguagesSpoken = new string[] { "English", "Chinese" },
            HasPets = true,
            PetNames = new string[] { "Fido", "Spot" }
        };
    }
}
