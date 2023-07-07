using System;
using System.Collections.Generic;

/// <summary>
/// Represents a student in a school.
/// </summary>
public class Student
{
    /// <summary>
    /// Gets or sets the unique identifier of the student.
    /// </summary>
    /// <value>The unique identifier of the student.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the student.
    /// </summary>
    /// <value>The full name of the student.</value>
    public string FullName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the student.
    /// </summary>
    /// <value>The date of birth of the student.</value>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the list of subjects that the student is enrolled in.
    /// </summary>
    /// <value>The list of subjects that the student is enrolled in.</value>
    public List<string> EnrolledSubjects { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Student"/> class.
    /// </summary>
    public Student()
    {
        // Default constructor
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Student"/> class with the specified identifier and full name.
    /// </summary>
    /// <param name="id">The unique identifier of the student.</param>
    /// <param name="fullName">The full name of the student.</param>
    public Student(int id, string fullName)
    {
        Id = id;
        FullName = fullName;
    }

    /// <summary>
    /// Calculates the age of the student based on the date of birth.
    /// </summary>
    /// <returns>The age of the student in years.</returns>
    /// <remarks>
    /// This method uses the current date to calculate the student's age.
    /// </remarks>
    public int CalculateAge()
    {
        DateTime currentDate = DateTime.Today;
        int age = currentDate.Year - DateOfBirth.Year;

        if (currentDate < DateOfBirth.AddYears(age))
        {
            age--;
        }

        return age;
    }

    /// <summary>
    /// Enrolls the student in the specified subject.
    /// </summary>
    /// <param name="subject">The subject to enroll the student in.</param>
    /// <exception cref="ArgumentNullException">Thrown if the subject is null.</exception>
    /// <remarks>
    /// The student can be enrolled in multiple subjects.
    /// </remarks>
    /// <example>
    /// This example demonstrates how to enroll a student in a subject.
    /// <code>
    /// Student student = new Student();
    /// string subject = "Math";
    /// student.EnrollInSubject(subject);
    /// </code>
    /// </example>
    public void EnrollInSubject(string subject)
    {
        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject), "Subject cannot be null.");
        }

        EnrolledSubjects.Add(subject);
    }
}
