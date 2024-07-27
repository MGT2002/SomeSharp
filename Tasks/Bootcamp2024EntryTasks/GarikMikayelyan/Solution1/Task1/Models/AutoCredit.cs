namespace Task1.Models
{
    internal record class AutoCredit : Credit
    {
        public string CarModel { get; set; }
        public string CarBrand { get; set; }
        public string VIN { get; set; }
    }
}
