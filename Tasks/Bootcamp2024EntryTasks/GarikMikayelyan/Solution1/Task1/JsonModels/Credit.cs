using Task1.Data;

namespace Task1.JsonModels
{
    internal class Credit
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public int CountOfMonth { get; set; }
        public decimal Percent { get; set; }
        public Borrower Borrower { get; set; }
        public Models.Bank Bank { get; set; }
        public string CarModel { get; set; }
        public string CarBrand { get; set; }
        public string VIN { get; set; }
        public string AddressOfObject { get; set; }
        public decimal Square { get; set; }
        public string UniversityName { get; set; }
        public string UniversityAddress { get; set; }
        public CreditType Type { get; set; }
    }
}
