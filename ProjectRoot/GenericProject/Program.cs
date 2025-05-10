// --- Previous using statements and TaxRates2025_2026 class remain the same ---
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

// --- PayFrequency enum, PayeInput, PayeOutput, ParsedTaxCode record/enum remain the same ---
public enum PayFrequency { Weekly, Monthly, Fortnightly, FourWeekly }

public class PayeInput
{
    public string TaxCode { get; set; } = "1257L";
    public decimal GrossPayForPeriod { get; set; } = 0M;
    public PayFrequency Frequency { get; set; } = PayFrequency.Monthly;
    public int PeriodNumber { get; set; } = 1;
    public bool UseWeek1Month1Basis { get; set; } = false;
    public decimal PreviousGrossPayYtd { get; set; } = 0M;
    public decimal PreviousTaxPaidYtd { get; set; } = 0M;
}

public class PayeOutput
{
    public decimal TaxablePayForPeriod { get; set; } // Approx for display
    public decimal TaxDueForPeriod { get; set; }
    public decimal AnnualAllowanceUsed { get; set; }
    public string CalculationBasisUsed { get; set; } = "";
    public List<string> Notes { get; set; } = new List<string>();
    // New fields matching image
    public decimal CumulativeFreePayYtd { get; set; } // Our calculated cumulative free pay
    public decimal CumulativeTaxablePayYtd { get; set; }
    public decimal TotalTaxDueYtd { get; set; }
    // Regulatory Limit might be added if K Code logic implemented fully
}

// --- Tax Code Parsing Logic (ParseTaxCode, TaxCodeType, TaxRegion, ParsedTaxCode record) remains the same ---
#region Tax Code Parsing Logic
public enum TaxCodeType { StandardAllowance, AddToPay_K, RateBased }
public enum TaxRegion { UK, Scotland, Wales } // UK = England & NI

record ParsedTaxCode(
    string OriginalCode,
    string BasicCode, // e.g., 1257L, K497, BR, 0T
    TaxCodeType CodeType,
    TaxRegion TaxRegion,
    decimal AnnualAllowance, // For L/M/N/T codes = allowance; For K codes = addition to pay; For RateBased = 0
    bool IsCumulativeByDefault
);

ParsedTaxCode ParseTaxCode(string taxCode)
{
    if (string.IsNullOrWhiteSpace(taxCode)) taxCode = "1257L"; // Default if empty
    taxCode = taxCode.Trim().ToUpperInvariant();

    TaxRegion region = TaxRegion.UK;
    if (taxCode.StartsWith("S")) { region = TaxRegion.Scotland; taxCode = taxCode.Substring(1); }
    else if (taxCode.StartsWith("C")) { region = TaxRegion.Wales; taxCode = taxCode.Substring(1); }

    bool useW1M1 = taxCode.EndsWith(" W1") || taxCode.EndsWith(" M1") || taxCode.EndsWith("X");
    if (useW1M1) taxCode = Regex.Replace(taxCode, @"\s+(W1|M1|X)$", ""); // Remove suffix for parsing basic code

    // Non-numerical / Rate Based Codes
    string[] rateCodes = { "BR", "D0", "D1", "0T", "NT" };
    if (rateCodes.Contains(taxCode))
    {
        return new ParsedTaxCode(taxCode, taxCode, TaxCodeType.RateBased, region, 0, false); // Rate codes are typically non-cumulative unless specified otherwise by HMRC P9
    }

    // K Codes
    if (taxCode.StartsWith("K"))
    {
        if (int.TryParse(taxCode.Substring(1), out int kNumber) && kNumber >= 0)
        {
            decimal additionToPay = kNumber * 10M;
            return new ParsedTaxCode(taxCode, taxCode, TaxCodeType.AddToPay_K, region, additionToPay, !useW1M1); // Cumulative unless W1/M1 override
        }
    }

    // Standard Allowance Codes (L, M, N, T suffixes usually)
    var match = Regex.Match(taxCode, @"^(\d+)([LTMN]?)$");
    if (match.Success && int.TryParse(match.Groups[1].Value, out int number) && number >= 0)
    {
        decimal allowance = number * 10M;
        // Note: M and N codes imply Marriage Allowance adjustments, not fully implemented here.
        // T code implies HMRC specific adjustments, treated as standard allowance here.
        return new ParsedTaxCode(taxCode, taxCode, TaxCodeType.StandardAllowance, region, allowance, !useW1M1); // Cumulative unless W1/M1 override
    }

    // Fallback / Emergency - often treated as 0T or basic 1257L W1/M1
    Console.Error.WriteLine($"Warning: Could not parse tax code '{taxCode}'. Defaulting to 0T logic.");
    return new ParsedTaxCode(taxCode, "0T", TaxCodeType.RateBased, region, 0, false);
}
#endregion

public class HmrcPayeCalculator
{
    // --- Helper Methods (GetPeriodsInYear, ApplyTaxRate, CalculateTaxOnAmount - slightly modified rounding) ---
    #region Helper Methods
    private int GetPeriodsInYear(PayFrequency frequency) => frequency switch
    {
        PayFrequency.Weekly => 52,
        PayFrequency.Monthly => 12,
        PayFrequency.Fortnightly => 26,
        PayFrequency.FourWeekly => 13,
        _ => throw new ArgumentOutOfRangeException(nameof(frequency)),
    };

    // Calculates tax on a given taxable amount using the specified annual bands.
    // Returns raw calculated tax before period deduction / final rounding
    private decimal CalculateTaxOnAmount(decimal taxableAmount, List<TaxRates2025_2026.TaxBand> bands)
    {
        if (taxableAmount <= 0) return 0M;
        decimal totalTax = 0M;
        decimal previousBandThreshold = 0M;
        foreach (var band in bands)
        {
            if (taxableAmount <= previousBandThreshold) break;
            decimal incomeInThisBand = Math.Min(taxableAmount, band.UpperThreshold) - previousBandThreshold;
            // Use higher precision during calculation
            totalTax += incomeInThisBand * band.Rate;
            previousBandThreshold = band.UpperThreshold;
        }
        // DO NOT round here yet - round the final period tax later
        return totalTax;
    }

    // Simplified rate application for BR, D0, D1 codes
    private decimal ApplyTaxRate(decimal taxableAmount, decimal rate, List<TaxRates2025_2026.TaxBand> bands)
    {
        if (taxableAmount <= 0) return 0M;
        // DO NOT round here yet
        return taxableAmount * rate;
    }
    #endregion

    // --- Main Calculation Method ---
    public PayeOutput Calculate(PayeInput input)
    {
        var output = new PayeOutput();
        if (input.GrossPayForPeriod < 0) input.GrossPayForPeriod = 0;

        var parsedCode = ParseTaxCode(input.TaxCode);
        output.AnnualAllowanceUsed = parsedCode.AnnualAllowance;

        bool isCumulative = !input.UseWeek1Month1Basis && parsedCode.IsCumulativeByDefault;
        output.CalculationBasisUsed = isCumulative ? "Cumulative" : "Week 1 / Month 1";

        List<TaxRates2025_2026.TaxBand> bands = parsedCode.TaxRegion switch
        {
            TaxRegion.Scotland => TaxRates2025_2026.ScottishBands,
            TaxRegion.Wales => TaxRates2025_2026.WelshBands,
            _ => TaxRates2025_2026.UkBands,
        };

        int periodsInYear = GetPeriodsInYear(input.Frequency);
        decimal taxDuePeriod = 0M;
        decimal taxablePayYtd = 0M; // Cumulative taxable pay YTD
        decimal freePayYtd = 0M; // Cumulative free pay YTD

        // --- Calculation Logic ---

        if (parsedCode.CodeType == TaxCodeType.RateBased)
        {
            output.CalculationBasisUsed = "N/A (Rate Based Code)";
            output.TaxablePayForPeriod = input.GrossPayForPeriod; // Whole amount taxable
            taxablePayYtd = input.PreviousGrossPayYtd + input.GrossPayForPeriod; // Track YTD for context
            output.CumulativeTaxablePayYtd = taxablePayYtd;
            output.CumulativeFreePayYtd = 0; // No allowance

            decimal rawTax = 0M;
            switch (parsedCode.BasicCode)
            {
                case "BR":
                    rawTax = ApplyTaxRate(output.TaxablePayForPeriod, bands[0].Rate, bands);
                    output.Notes.Add($"Tax calculated at Basic Rate ({bands[0].Rate:P0}).");
                    break;
                case "D0":
                    rawTax = ApplyTaxRate(output.TaxablePayForPeriod, bands.Count > 1 ? bands[1].Rate : bands[0].Rate, bands);
                    output.Notes.Add($"Tax calculated at First Higher Rate ({(bands.Count > 1 ? bands[1].Rate : bands[0].Rate):P0}).");
                    break;
                case "D1":
                    rawTax = ApplyTaxRate(output.TaxablePayForPeriod, bands.Count > 2 ? bands[2].Rate : (bands.Count > 1 ? bands[1].Rate : bands[0].Rate), bands);
                    output.Notes.Add($"Tax calculated at Second Higher/Add Rate ({(bands.Count > 2 ? bands[2].Rate : (bands.Count > 1 ? bands[1].Rate : bands[0].Rate)):P0}).");
                    break;
                case "0T":
                    rawTax = CalculateTaxOnAmount(output.TaxablePayForPeriod, bands); // Tax using bands, 0 allowance
                    output.Notes.Add($"Tax calculated using standard bands but £0 Personal Allowance.");
                    break;
                case "NT":
                    rawTax = 0M;
                    output.Notes.Add("NT code: No Income Tax deducted.");
                    break;
            }
            // For non-cumulative rate codes, period tax is just the calculated tax
            // Applying HMRC rounding down (Floor to 2 decimal places)
            taxDuePeriod = Math.Floor(rawTax * 100) / 100M;
            // YTD tax would need previous periods if cumulative was forced, but typically isn't for these codes
            output.TotalTaxDueYtd = input.PreviousTaxPaidYtd + taxDuePeriod;
        }
        else if (parsedCode.CodeType == TaxCodeType.AddToPay_K)
        {
            isCumulative = !input.UseWeek1Month1Basis; // K codes can be cumulative or W1/M1
            output.CalculationBasisUsed = isCumulative ? "Cumulative" : "Week 1 / Month 1";

            decimal additionToPayAnnual = parsedCode.AnnualAllowance;
            decimal cumulativeGrossYtd = input.PreviousGrossPayYtd + input.GrossPayForPeriod;

            if (isCumulative)
            {
                decimal cumulativeAdditionToPay = Math.Round((additionToPayAnnual / periodsInYear) * input.PeriodNumber, 2); // Standard rounding for this intermediate value? Assume yes.
                decimal cumulativeAdjustedPay = cumulativeGrossYtd + cumulativeAdditionToPay;
                taxablePayYtd = Math.Max(0M, cumulativeAdjustedPay);
                freePayYtd = 0; // K code means no free pay

                decimal totalTaxDueYtdRaw = CalculateTaxOnAmount(taxablePayYtd, bands);
                decimal taxDuePeriodRaw = totalTaxDueYtdRaw - input.PreviousTaxPaidYtd;

                // Apply 50% overriding limit FOR THE PERIOD
                decimal maxTaxThisPeriod = Math.Floor(input.GrossPayForPeriod * 0.50M * 100) / 100M; // Round down limit
                if (taxDuePeriodRaw > maxTaxThisPeriod)
                {
                    output.Notes.Add($"K Code 50% limit applied. Tax capped at £{maxTaxThisPeriod:N2} (was £{Math.Floor(taxDuePeriodRaw * 100) / 100M:N2}). Remainder may be collected later.");
                    taxDuePeriod = maxTaxThisPeriod;
                }
                else
                {
                    // Apply HMRC rounding down to the period tax
                    taxDuePeriod = Math.Floor(taxDuePeriodRaw * 100) / 100M;
                }
                output.TotalTaxDueYtd = input.PreviousTaxPaidYtd + taxDuePeriod; // YTD tax reflects the actual tax taken this period
                output.Notes.Add($"K Code {parsedCode.BasicCode}: Added cumulative £{cumulativeAdditionToPay:N2} to pay YTD.");
            }
            else // Week 1 / Month 1 for K code
            {
                decimal additionToPayPeriod = Math.Round(additionToPayAnnual / periodsInYear, 2);
                decimal adjustedPayPeriod = input.GrossPayForPeriod + additionToPayPeriod;
                output.TaxablePayForPeriod = Math.Max(0M, adjustedPayPeriod); // Period taxable pay
                taxablePayYtd = output.TaxablePayForPeriod; // For W1M1, YTD taxable doesn't accumulate properly
                freePayYtd = 0; // K code means no free pay

                decimal taxDuePeriodRaw = CalculateTaxOnAmount(output.TaxablePayForPeriod, bands);

                // Apply 50% overriding limit FOR THE PERIOD
                decimal maxTaxThisPeriod = Math.Floor(input.GrossPayForPeriod * 0.50M * 100) / 100M;
                if (taxDuePeriodRaw > maxTaxThisPeriod)
                {
                    output.Notes.Add($"K Code 50% limit applied. Tax capped at £{maxTaxThisPeriod:N2} (was £{Math.Floor(taxDuePeriodRaw * 100) / 100M:N2}).");
                    taxDuePeriod = maxTaxThisPeriod;
                }
                else
                {
                    // Apply HMRC rounding down
                    taxDuePeriod = Math.Floor(taxDuePeriodRaw * 100) / 100M;
                }
                output.TotalTaxDueYtd = taxDuePeriod; // For W1M1 YTD tax = period tax
                output.Notes.Add($"K Code {parsedCode.BasicCode}: Added £{additionToPayPeriod:N2} to period pay (W1/M1 calc).");
            }
        }
        else // Standard Allowance Code
        {
            decimal annualAllowance = parsedCode.AnnualAllowance;
            // Add taper warning logic here if needed (as before)

            if (isCumulative)
            {
                decimal cumulativeGrossYtd = input.PreviousGrossPayYtd + input.GrossPayForPeriod;

                // Calculate cumulative free pay - *** This is where the discrepancy likely lies ***
                // Using simple pro-rata. HMRC might use tables/specific rounding.
                freePayYtd = Math.Round((annualAllowance / periodsInYear) * input.PeriodNumber, 2); // Standard rounding for now
                output.Notes.Add($"Simulated Cumulative Free Pay (Pay Adjustment): £{freePayYtd:N2}. (Note: May differ slightly from official HMRC value due to rounding rules).");

                taxablePayYtd = Math.Max(0M, cumulativeGrossYtd - freePayYtd);

                decimal totalTaxDueYtdRaw = CalculateTaxOnAmount(taxablePayYtd, bands);
                decimal taxDuePeriodRaw = totalTaxDueYtdRaw - input.PreviousTaxPaidYtd;

                // Apply HMRC rounding down (Floor)
                taxDuePeriod = Math.Floor(taxDuePeriodRaw * 100) / 100M;
                // Ensure tax isn't negative (refunds are handled by lower positive tax)
                taxDuePeriod = Math.Max(0M, taxDuePeriod);

                output.TotalTaxDueYtd = input.PreviousTaxPaidYtd + taxDuePeriod; // Actual total tax paid reflects period deduction

                // Approximate taxable pay for *this* period for display
                decimal prevFreePay = input.PeriodNumber > 1 ? Math.Round((annualAllowance / periodsInYear) * (input.PeriodNumber - 1), 2) : 0M;
                decimal prevTaxablePay = Math.Max(0M, input.PreviousGrossPayYtd - prevFreePay);
                output.TaxablePayForPeriod = taxablePayYtd - prevTaxablePay;


            }
            else // Week 1 / Month 1 for Standard Code
            {
                freePayYtd = Math.Round(annualAllowance / periodsInYear, 2); // Free pay for this period ONLY
                output.TaxablePayForPeriod = Math.Max(0M, input.GrossPayForPeriod - freePayYtd);
                taxablePayYtd = output.TaxablePayForPeriod; // Doesn't accumulate

                decimal taxDuePeriodRaw = CalculateTaxOnAmount(output.TaxablePayForPeriod, bands);

                // Apply HMRC rounding down (Floor)
                taxDuePeriod = Math.Floor(taxDuePeriodRaw * 100) / 100M;
                taxDuePeriod = Math.Max(0M, taxDuePeriod); // Ensure non-negative

                output.TotalTaxDueYtd = taxDuePeriod; // Only this period's tax matters for YTD on W1M1
            }
        }

        output.TaxDueForPeriod = taxDuePeriod;
        output.CumulativeFreePayYtd = freePayYtd;
        output.CumulativeTaxablePayYtd = taxablePayYtd;

        return output;
    }
}


// --- Main Program Class (Example Usage) ---
public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CultureInfo ukCulture = new CultureInfo("en-GB");
        HmrcPayeCalculator calculator = new HmrcPayeCalculator();

        Console.WriteLine("HMRC PAYE Tax Calculator Simulation (2025-2026) - Income Tax Only");
        Console.WriteLine("==================================================================");
        Console.WriteLine("Disclaimer: Simulates common scenarios for educational purposes.");
        Console.WriteLine("Does NOT include NI, Student Loans, exact HMRC rounding/free pay calcs, or all tax codes/rules.");
        Console.WriteLine("!!! DO NOT USE FOR ACTUAL PAYROLL !!!\n");

        // --- Replicate Image Scenario ---
        Console.WriteLine("--- Replicating Image Scenario ---");
        PayeInput imageInput = new PayeInput
        {
            TaxCode = "1257L",
            Frequency = PayFrequency.Weekly,
            GrossPayForPeriod = 1000M,
            PeriodNumber = 4,
            UseWeek1Month1Basis = false,
            PreviousGrossPayYtd = 0M, // Assuming this is first pay in period 4? Or total before week 4 pay? Let's assume total BEFORE week 4 pay.
            PreviousTaxPaidYtd = 0M   // Assume £0 tax paid before week 4 if previous pay was 0.
        };
        // If previous pay means Weeks 1, 2, 3 pay:
        // imageInput.PreviousGrossPayYtd = 3000M; // e.g. if paid £1000 weeks 1, 2, 3
        // imageInput.PreviousTaxPaidYtd = ?; // Need tax from previous periods

        // Let's assume the £1000 is the *first* payment, happening *in* week 4 (unusual, but fits previous pay=0)
        Console.WriteLine("Assuming £1000 is first payment, occurring in Week 4 (Previous Pay=0, Previous Tax=0)");
        PayeOutput imageResult = calculator.Calculate(imageInput);
        PrintResult(imageInput, imageResult, ukCulture);

        Console.WriteLine("\n--- Replicating Image Scenario (Assuming £1000 paid each week for 4 weeks) ---");
        PayeInput wk1Input = new PayeInput { TaxCode = "1257L", Frequency = PayFrequency.Weekly, GrossPayForPeriod = 1000M, PeriodNumber = 1, UseWeek1Month1Basis = false, PreviousGrossPayYtd = 0, PreviousTaxPaidYtd = 0 };
        PayeOutput wk1Result = calculator.Calculate(wk1Input);
        PayeInput wk2Input = new PayeInput { TaxCode = "1257L", Frequency = PayFrequency.Weekly, GrossPayForPeriod = 1000M, PeriodNumber = 2, UseWeek1Month1Basis = false, PreviousGrossPayYtd = wk1Result.CumulativeGrossPayYtd, PreviousTaxPaidYtd = wk1Result.TotalTaxDueYtd };
        PayeOutput wk2Result = calculator.Calculate(wk2Input);
        PayeInput wk3Input = new PayeInput { TaxCode = "1257L", Frequency = PayFrequency.Weekly, GrossPayForPeriod = 1000M, PeriodNumber = 3, UseWeek1Month1Basis = false, PreviousGrossPayYtd = wk2Result.CumulativeGrossPayYtd, PreviousTaxPaidYtd = wk2Result.TotalTaxDueYtd };
        PayeOutput wk3Result = calculator.Calculate(wk3Input);
        PayeInput wk4Input = new PayeInput { TaxCode = "1257L", Frequency = PayFrequency.Weekly, GrossPayForPeriod = 1000M, PeriodNumber = 4, UseWeek1Month1Basis = false, PreviousGrossPayYtd = wk3Result.CumulativeGrossPayYtd, PreviousTaxPaidYtd = wk3Result.TotalTaxDueYtd };
        PayeOutput wk4Result = calculator.Calculate(wk4Input);
        Console.WriteLine("Result for Week 4:");
        PrintResult(wk4Input, wk4Result, ukCulture);


        // --- Interactive Input ---
        Console.WriteLine("\n--- Interactive Calculation ---");
        try
        {
            PayeInput currentInput = new PayeInput();

            Console.WriteLine("Enter details for the pay period:");
            Console.Write(" > Tax Code (e.g., 1257L, S1257L, BR, 0T, K497, 100L W1): ");
            currentInput.TaxCode = Console.ReadLine() ?? "1257L";
            Console.Write(" > Pay Frequency (Weekly/Monthly/Fortnightly/FourWeekly): ");
            currentInput.Frequency = (PayFrequency)Enum.Parse(typeof(PayFrequency), Console.ReadLine() ?? "Monthly", true);
            Console.Write($" > Gross Pay for this {currentInput.Frequency}: £");
            currentInput.GrossPayForPeriod = decimal.Parse(Console.ReadLine() ?? "0", ukCulture);
            Console.Write(" > Pay Period Number (e.g., Week 1-52, Month 1-12): ");
            currentInput.PeriodNumber = int.Parse(Console.ReadLine() ?? "1");

            bool codeImpliesW1M1 = currentInput.TaxCode.ToUpper().EndsWith(" W1") || currentInput.TaxCode.ToUpper().EndsWith(" M1") || currentInput.TaxCode.ToUpper().EndsWith("X");
            var parsedCode = new HmrcPayeCalculator().ParseTaxCode(currentInput.TaxCode); // Need instance or make static

            if (codeImpliesW1M1)
            {
                currentInput.UseWeek1Month1Basis = true;
                Console.WriteLine(" > Calculation Basis: Week 1/Month 1 (Implied by Tax Code Suffix)");
            }
            else if (!parsedCode.IsCumulativeByDefault)
            {
                currentInput.UseWeek1Month1Basis = true; // Default for BR, D0, D1, 0T, NT unless overridden by HMRC
                Console.WriteLine(" > Calculation Basis: Week 1/Month 1 (Default for this type of code)");
            }
            else
            {
                Console.Write(" > Use Week 1 / Month 1 basis? (yes/no - default no): ");
                string basisInput = Console.ReadLine()?.Trim().ToLower() ?? "no";
                currentInput.UseWeek1Month1Basis = (basisInput == "yes" || basisInput == "y");
            }

            if (!currentInput.UseWeek1Month1Basis && parsedCode.IsCumulativeByDefault)
            { // Only ask for YTD if cumulative standard/K code
                Console.Write($" > Previous Gross Pay Year-to-Date (before this period): £");
                currentInput.PreviousGrossPayYtd = decimal.Parse(Console.ReadLine() ?? "0", ukCulture);
                Console.Write($" > Previous Tax Paid Year-to-Date (before this period): £");
                currentInput.PreviousTaxPaidYtd = decimal.Parse(Console.ReadLine() ?? "0", ukCulture);
            }

            PayeOutput result = calculator.Calculate(currentInput);
            PrintResult(currentInput, result, ukCulture);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}. Please check your inputs.");
        }

        Console.WriteLine("\nPress Enter to exit.");
        Console.ReadLine();
    }

    // Helper to print results consistently
    static void PrintResult(PayeInput input, PayeOutput result, CultureInfo culture)
    {
        Console.WriteLine("\n--- Calculation Result ---");
        Console.WriteLine($" Input:");
        Console.WriteLine($"   Tax Code:             {input.TaxCode}");
        Console.WriteLine($"   Frequency:            {input.Frequency}");
        Console.WriteLine($"   Period Number:        {input.PeriodNumber}");
        Console.WriteLine($"   Gross Pay This Per:   {input.GrossPayForPeriod.ToString("C", culture)}");
        Console.WriteLine($"   Basis Specified:      {(input.UseWeek1Month1Basis ? "Week 1/Month 1" : "Cumulative")}");
        if (!input.UseWeek1Month1Basis && result.CalculationBasisUsed == "Cumulative")
        {
            Console.WriteLine($"   Previous Gross YTD:   {input.PreviousGrossPayYtd.ToString("C", culture)}");
            Console.WriteLine($"   Previous Tax YTD:     {input.PreviousTaxPaidYtd.ToString("C", culture)}");
        }
        Console.WriteLine($" Output:");
        Console.WriteLine($"   Basis Used:           {result.CalculationBasisUsed}");
        Console.WriteLine($"   Derived Annual Allow: {result.AnnualAllowanceUsed.ToString("C0", culture)}");
        Console.WriteLine($"   Cum. Free Pay YTD:    {result.CumulativeFreePayYtd.ToString("C", culture)}");
        Console.WriteLine($"   Cum. Taxable Pay YTD: {result.CumulativeTaxablePayYtd.ToString("C", culture)}");
        Console.WriteLine($"   Total Tax Due YTD:    {result.TotalTaxDueYtd.ToString("C", culture)}");
        Console.WriteLine($"   Taxable Pay This Per: {result.TaxablePayForPeriod.ToString("C", culture)} (approx.)");
        Console.WriteLine($"   Income Tax This Per:  {result.TaxDueForPeriod.ToString("C", culture)}");
        if (result.Notes.Any())
        {
            Console.WriteLine("\n Notes:");
            foreach (string note in result.Notes) { Console.WriteLine($"   - {note}"); }
        }
        Console.WriteLine("--------------------------");
    }

    // Need instance or make static
    // Add ParseTaxCode method here or make HmrcPayeCalculator instance accessible
    // For simplicity in Program.Main, let's duplicate the parsing logic or make it static/utility
    // Duplicating the nested record/enums and ParseTaxCode method from HmrcPayeCalculator here...
    #region Duplicated Tax Code Parsing Logic for Program.Main
    private enum TaxCodeType { StandardAllowance, AddToPay_K, RateBased }
    private enum TaxRegion { UK, Scotland, Wales }

    private record ParsedTaxCode(
        string OriginalCode,
        string BasicCode,
        TaxCodeType CodeType,
        TaxRegion TaxRegion,
        decimal AnnualAllowance,
        bool IsCumulativeByDefault
    );

    // Make static for use in Main
    public static ParsedTaxCode ParseTaxCode(string taxCode)
    {
        if (string.IsNullOrWhiteSpace(taxCode)) taxCode = "1257L";
        taxCode = taxCode.Trim().ToUpperInvariant();

        TaxRegion region = TaxRegion.UK;
        if (taxCode.StartsWith("S")) { region = TaxRegion.Scotland; taxCode = taxCode.Substring(1); }
        else if (taxCode.StartsWith("C")) { region = TaxRegion.Wales; taxCode = taxCode.Substring(1); }

        bool useW1M1 = taxCode.EndsWith(" W1") || taxCode.EndsWith(" M1") || taxCode.EndsWith("X");
        if (useW1M1) taxCode = Regex.Replace(taxCode, @"\s+(W1|M1|X)$", "");

        string[] rateCodes = { "BR", "D0", "D1", "0T", "NT" };
        if (rateCodes.Contains(taxCode))
        {
            return new ParsedTaxCode(taxCode, taxCode, TaxCodeType.RateBased, region, 0, false);
        }

        if (taxCode.StartsWith("K"))
        {
            if (int.TryParse(taxCode.Substring(1), out int kNumber) && kNumber >= 0)
            {
                decimal additionToPay = kNumber * 10M;
                return new ParsedTaxCode(taxCode, taxCode, TaxCodeType.AddToPay_K, region, additionToPay, !useW1M1);
            }
        }

        var match = Regex.Match(taxCode, @"^(\d+)([LTMN]?)$");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int number) && number >= 0)
        {
            decimal allowance = number * 10M;
            return new ParsedTaxCode(taxCode, taxCode, TaxCodeType.StandardAllowance, region, allowance, !useW1M1);
        }

        Console.Error.WriteLine($"Warning: Could not parse tax code '{taxCode}'. Defaulting to 0T logic.");
        return new ParsedTaxCode(taxCode, "0T", TaxCodeType.RateBased, region, 0, false);
    }
    #endregion


} // End of Program class