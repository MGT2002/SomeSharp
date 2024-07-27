namespace Task1.Models
{
    internal record class MortgageCredit : Credit
    {
        public string AddressOfObject { get; set; }
        public decimal Square { get; set; }
    }
}
