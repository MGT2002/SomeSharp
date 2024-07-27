using Task1.Data;

namespace Task1.Models
{
    internal abstract record class Credit
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public int CountOfMonth { get; set; }
        public decimal Percent { get; set; }
        public Borrower Borrower { get; set; }
        public Bank Bank { get; set; }
        public CreditType CreditType { get; init; }
    }
}
