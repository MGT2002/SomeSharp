public class IncomeTaxCalculator
{
    // --- Constants for 2025-2026 Tax Year ---

    // UK-wide constants
    private const decimal StandardPersonalAllowance = 12570m;
    private const decimal PersonalAllowanceTaperThreshold = 100000m;

    // England, Wales, Northern Ireland Bands (Taxable Income Limits)
    private static readonly TaxBand[] UkBands = new TaxBand[]
    {
        // Basic Rate: £12,571 to £50,270 means the band covers income from £0 up to £37,700 after PA
        new TaxBand(0m, 37700m, 0.20m),       // Basic Rate (50270 - 12570 = 37700)
        // Higher Rate: £50,271 to £125,140 means the band covers income from £37,701 up to £112,570 after PA
        new TaxBand(37700m, 112570m, 0.40m),  // Higher Rate (125140 - 12570 = 112570)
        // Additional Rate: Over £125,140 means the band covers income over £112,570 after PA
        new TaxBand(112570m, decimal.MaxValue, 0.45m) // Additional Rate
    };

    // Scottish Bands (Taxable Income Limits)
    private static readonly TaxBand[] ScottishBands = new TaxBand[]
    {
        // Starter Rate: £12,571 to £14,876 -> £0 to £2,306 taxable
        new TaxBand(0m, 2306m, 0.19m),        // Starter Rate (14876 - 12570 = 2306)
        // Basic Rate: £14,877 to £26,561 -> £2,307 to £13,991 taxable
        new TaxBand(2306m, 13991m, 0.20m),    // Basic Rate (26561 - 12570 = 13991)
        // Intermediate Rate: £26,562 to £43,662 -> £13,992 to £31,092 taxable
        new TaxBand(13991m, 31092m, 0.21m),   // Intermediate Rate (43662 - 12570 = 31092)
         // Higher Rate: £43,663 to £75,000 -> £31,093 to £62,430 taxable
        new TaxBand(31092m, 62430m, 0.42m),   // Higher Rate (75000 - 12570 = 62430)
         // Advanced Rate: £75,001 to £125,140 -> £62,431 to £112,570 taxable
        new TaxBand(62430m, 112570m, 0.45m),  // Advanced Rate (125140 - 12570 = 112570)
        // Top Rate: Over £125,140 -> Over £112,570 taxable
        new TaxBand(112570m, decimal.MaxValue, 0.48m) // Top Rate
    };

    // --- Calculation Methods ---

    public decimal CalculatePersonalAllowance(decimal grossIncome)
    {
        if (grossIncome <= PersonalAllowanceTaperThreshold)
        {
            return StandardPersonalAllowance;
        }
        else
        {
            decimal reduction = Math.Floor((grossIncome - PersonalAllowanceTaperThreshold) / 2m);
            decimal adjustedAllowance = StandardPersonalAllowance - reduction;
            return Math.Max(0m, adjustedAllowance); // Allowance cannot be negative
        }
    }

    public decimal CalculateTax(decimal grossIncome, bool isScottishTaxpayer)
    {
        if (grossIncome <= 0)
        {
            return 0m;
        }

        decimal personalAllowance = CalculatePersonalAllowance(grossIncome);
        decimal taxableIncome = grossIncome - personalAllowance;

        // If taxable income is zero or less, no tax is due
        if (taxableIncome <= 0)
        {
            return 0m;
        }

        TaxBand[] bandsToUse = isScottishTaxpayer ? ScottishBands : UkBands;
        decimal totalTax = 0m;
        decimal incomeProcessed = 0m;

        foreach (var band in bandsToUse)
        {
            // Calculate the upper limit of this band relative to the start (0)
            decimal bandUpperLimit = band.UpperLimit;
            decimal bandLowerLimit = band.LowerLimit;

            // Amount of income that falls into *this* specific band
            decimal incomeInThisBand = 0m;

            if (taxableIncome > bandLowerLimit)
            {
                // Calculate how much of the taxable income falls into this band
                // Max taxable income for this band is bandUpperLimit
                // Min taxable income for this band is bandLowerLimit + 0.01 (conceptually)
                incomeInThisBand = Math.Min(taxableIncome - bandLowerLimit, bandUpperLimit - bandLowerLimit);
            }


            if (incomeInThisBand > 0)
            {
                totalTax += incomeInThisBand * band.Rate;
            }

            // If all taxable income has been processed, stop
            if (taxableIncome <= bandUpperLimit)
            {
                break;
            }
        }

        return totalTax;
    }

    // Helper structure for tax bands
    private struct TaxBand
    {
        public decimal LowerLimit { get; } // Lower bound of taxable income for this band (exclusive)
        public decimal UpperLimit { get; } // Upper bound of taxable income for this band (inclusive)
        public decimal Rate { get; }       // Tax rate for this band

        public TaxBand(decimal lowerLimit, decimal upperLimit, decimal rate)
        {
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            Rate = rate;
        }
    }

    // --- Example Usage ---
    public static void Main(string[] args)
    {
        IncomeTaxCalculator calculator = new IncomeTaxCalculator();

        Console.WriteLine("UK Income Tax Calculator 2025-2026");
        Console.WriteLine("---------------------------------");

        Console.Write("Enter Gross Annual Income (£): ");
        if (decimal.TryParse(Console.ReadLine(), out decimal income))
        {
            Console.Write("Are you a Scottish taxpayer? (yes/no): ");
            string scottishResponse = Console.ReadLine()?.Trim().ToLower();
            bool isScottish = scottishResponse == "yes" || scottishResponse == "y";

            decimal allowance = calculator.CalculatePersonalAllowance(income);
            decimal taxDue = calculator.CalculateTax(income, isScottish);
            decimal netIncome = income - taxDue;

            Console.WriteLine($"\n--- Results ---");
            Console.WriteLine($"Gross Income:       £{income:N2}");
            Console.WriteLine($"Personal Allowance: £{allowance:N2}");
            Console.WriteLine($"Taxable Income:     £{Math.Max(0, income - allowance):N2}");
            Console.WriteLine($"Region:             {(isScottish ? "Scotland" : "England/Wales/NI")}");
            Console.WriteLine($"Total Income Tax:   £{taxDue:N2}");
            Console.WriteLine($"Net Income (after tax): £{netIncome:N2}");

            Console.WriteLine("\nDisclaimer: This calculation is an estimate based on 2025-2026 rates.");
            Console.WriteLine("It does not include National Insurance, student loans, pensions, etc.");
            Console.WriteLine("Consult a professional for financial advice.");
        }
        else
        {
            Console.WriteLine("Invalid income entered.");
        }

        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }
}