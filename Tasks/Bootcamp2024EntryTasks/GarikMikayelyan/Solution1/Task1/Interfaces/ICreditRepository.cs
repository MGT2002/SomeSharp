using Task1.Models;

namespace Task1.Interfaces
{
    internal interface ICreditRepository
    {
        List<Credit> GetCreditModels();
    }
}
