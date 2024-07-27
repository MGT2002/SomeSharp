using Newtonsoft.Json;
using System.Globalization;
using Task1.Data;
using Task1.Interfaces;
using Task1.Models;
using Json = Task1.JsonModels;

namespace Task1.Repositories
{
    internal class CreditRepository : ICreditRepository
    {
        private string path;

        /// <param name="path">The specific json file fullname</param>
        public CreditRepository(string path)
        {
            this.path = path;
        }

        public List<Credit> GetCreditModels()
        {
            var jsonCredits = GetJsonCreditModels();
            List<Credit> res = new List<Credit>(jsonCredits.Count);

            foreach (var jsonCredit in jsonCredits)
            {
                Credit credit;
                if (jsonCredit.Type == CreditType.Auto)
                {
                    credit = new AutoCredit()
                    {
                        CreditType = CreditType.Auto,
                        CarModel = jsonCredit.CarModel,
                        CarBrand = jsonCredit.CarBrand,
                        VIN = jsonCredit.VIN,
                    };
                }
                else if (jsonCredit.Type == CreditType.Mortgage)
                {
                    credit = new MortgageCredit()
                    {
                        CreditType = CreditType.Mortgage,
                        AddressOfObject = jsonCredit.AddressOfObject,
                        Square = jsonCredit.Square,
                    };
                }
                else
                {
                    credit = new EducationCredit()
                    {
                        CreditType = CreditType.Education,
                        UniversityAddress = jsonCredit.UniversityAddress,
                        UniversityName = jsonCredit.UniversityName,
                    };
                }

                FillCreditByJsonCredit(jsonCredit, credit);
                res.Add(credit);
            }

            return res;
        }

        private void FillCreditByJsonCredit(Json.Credit jsonCredit, Credit credit)
        {
            credit.ID = jsonCredit.ID;
            credit.Amount = jsonCredit.Amount;
            credit.CountOfMonth = jsonCredit.CountOfMonth;
            credit.Percent = jsonCredit.Percent;
            credit.Borrower = new Borrower()
            {
                Id = jsonCredit.Borrower.Id,
                FirstName = jsonCredit.Borrower.FirstName,
                LastName = jsonCredit.Borrower.LastName,
                DateOfBirth = DateTime.ParseExact(jsonCredit.Borrower.DateOfBirth,
                    "dd/MM/yyyy", CultureInfo.InvariantCulture),
                PassportNumber = jsonCredit.Borrower.PassportNumber,
            };
            credit.Bank = new Bank()
            {
                Id = jsonCredit.Bank.Id,
                Name = jsonCredit.Bank.Name,
                Address = jsonCredit.Bank.Address,
            };
        }

        private List<Json.Credit> GetJsonCreditModels()
        {
            var res = JsonConvert.DeserializeObject<List<Json.Credit>>
                (File.ReadAllText(path))!;

            foreach (var credit in res)
            {
                if (!string.IsNullOrWhiteSpace(credit.CarModel))
                    credit.Type = CreditType.Auto;
                else if (!string.IsNullOrWhiteSpace(credit.AddressOfObject))
                    credit.Type = CreditType.Mortgage;
                else
                    credit.Type = CreditType.Education;
            }

            return res;
        }
    }
}
