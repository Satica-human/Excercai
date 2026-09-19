using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentManager
{
    public class StudentDAO
    {
        private readonly List<Student> _students = new List<Student>();

        /// <summary>
        /// Thêm sinh viên mới. Ném exception nếu Id đã tồn tại.
        /// </summary>
        public void Add(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (string.IsNullOrWhiteSpace(student.Id))
                throw new ArgumentException("Student Id cannot be null or empty.");

            if (_students.Any(s => s.Id == student.Id))
                throw new InvalidOperationException($"Student with Id '{student.Id}' already exists.");

            _students.Add(student);
        }

        /// <summary>
        /// Cập nhật thông tin sinh viên theo Id. Ném exception nếu không tìm thấy.
        /// </summary>
        public void Edit(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            var existing = _students.FirstOrDefault(s => s.Id == student.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Student with Id '{student.Id}' not found.");

            existing.Name = student.Name;
            existing.DateOfBirth = student.DateOfBirth;
            existing.Email = student.Email;
            existing.Address = student.Address;
            existing.GPA = student.GPA;
        }

        /// <summary>
        /// Xóa sinh viên theo Id. Ném exception nếu không tìm thấy.
        /// </summary>
        public void Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty.");

            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
                throw new KeyNotFoundException($"Student with Id '{id}' not found.");

            _students.Remove(student);
        }

        /// <summary>
        /// Lấy danh sách toàn bộ sinh viên.
        /// </summary>
        public List<Student> GetAll()
        {
            return _students.ToList();
        }

        /// <summary>
        /// Tìm sinh viên theo Id. Trả về null nếu không tìm thấy.
        /// </summary>
        public Student? GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty.");

            return _students.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Tìm sinh viên theo tên (chứa keyword, không phân biệt hoa/thường).
        /// </summary>
        public List<Student> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.");

            return _students
                .Where(s => s.Name != null && s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Sắp xếp danh sách sinh viên theo tên (A-Z).
        /// </summary>
        public List<Student> SortByName()
        {
            return _students.OrderBy(s => s.Name).ToList();
        }

        /// <summary>
        /// Sắp xếp danh sách sinh viên theo GPA giảm dần.
        /// </summary>
        public List<Student> SortByGPADescending()
        {
            return _students.OrderByDescending(s => s.GPA).ToList();
        }

        /// <summary>
        /// Lấy danh sách sinh viên có GPA >= threshold.
        /// </summary>
        public List<Student> GetByMinGPA(double minGPA)
        {
            return _students.Where(s => s.GPA >= minGPA).ToList();
        }

        /// <summary>
        /// Đếm tổng số sinh viên.
        /// </summary>
        public int Count()
        {
            return _students.Count;
        }
    }
}
