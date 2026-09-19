using System;

namespace StudentManager
{
    public class Student
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public double GPA { get; set; }

        public Student() { }

        public Student(string id, string name, DateTime dateOfBirth, string email, string address, double gpa)
        {
            Id = id;
            Name = name;
            DateOfBirth = dateOfBirth;
            Email = email;
            Address = address;
            GPA = gpa;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, DOB: {DateOfBirth:dd/MM/yyyy}, Email: {Email}, Address: {Address}, GPA: {GPA:F2}";
        }
    }
}
