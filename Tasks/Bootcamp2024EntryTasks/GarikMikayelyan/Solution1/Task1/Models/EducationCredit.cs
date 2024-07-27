namespace Task1.Models
{
    internal record class EducationCredit : Credit
    {
        public string UniversityName { get; set; }
        public string UniversityAddress { get; set; }
    }
}
