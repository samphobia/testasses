using System;

namespace API.Entities
{
    public class AppTeacher
    {
        public int  Id { get; set; }
        // National ID number - required field
        public string NationalID { get; set; }

        // Title - required and can be either [Mr, Mrs, Miss, Dr, Prof]
        public string Title { get; set; }

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
                if (CalculateAge(value) < 21)
                {
                    throw new ArgumentException("Age must not be less than 21.");
                }
                _dateOfBirth = value;
            }
        }

        // Teacher Number - required
        public string TeacherNumber { get; set; }

        // Salary - optional
        public decimal? Salary { get; set; }

        public AppTeacher(string nationalID, string title, string name, string surname, DateTime dateOfBirth, string teacherNumber, decimal? salary = null)
        {
            NationalID = nationalID;
            Title = title;
            Name = name;
            Surname = surname;
            DateOfBirth = dateOfBirth;
            TeacherNumber = teacherNumber;
            Salary = salary;
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