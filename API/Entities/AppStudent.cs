using System;

namespace API.Entities
{
    public class AppStudent
    {
        public int  Id { get; set; }
        // National ID number - required field
        public string NationalID { get; set; }

        // Name - required
        public string Name { get; set; }

        // Surname - required
        public string Surname { get; set; }

        // Date of Birth - required - their age may not be less than 21
        private DateTime _dateOfBirth;

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                if (CalculateAge(value) > 22)
                {
                    throw new ArgumentException("Age must not be more than 22.");
                }
                _dateOfBirth = value;
            }
        }

        // Teacher Number - required
        public string StudentNumber { get; set; }

        public AppStudent(string nationalID, string name, string surname, DateTime dateOfBirth, string studentNumber)
        {
            NationalID = nationalID;
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            StudentNumber = studentNumber;
        }

        // Helper method to calculate age based on the date of birth
        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}