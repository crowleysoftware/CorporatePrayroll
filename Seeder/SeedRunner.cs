using Microsoft.Data.SqlClient;
using CorporatePrayroll;
using Dapper;

PrayrollRepository repo = new PrayrollRepository();

string payeeSQL = "INSERT INTO dbo.payee " +
    "(ID, FirstName, LastName, DateOfHire, Exempt, PayRate, StateOfResidence) " +
    "VALUES(@ID, @FirstName, @LastName, @DateOfHire, @Exempt, @PayRate, @StateOfResidence)";

string deductionSQL = "INSERT INTO dbo.deduction " +
    "(EmployeeID, DeductionName, DeductionFrequency, Amount, effective_date)" +
    "VALUES(@EmployeeID, @DeductionName, @DeductionFrequency, @Amount, @effective_date)";

string taxTblSQL = "INSERT INTO dbo.tax_table (State, StartRange, EndRange, TaxRate) " +
    "VALUES(@State, @StartRange, @EndRange, @TaxRate)";

string timecardSQL = "INSERT INTO timecard (EmployeeID, DateOfWork, HoursWorked) " +
    "VALUES (@EmployeeID, @DateOfWork, @HoursWorked);";

try
{
    using (SqlConnection con = new SqlConnection("Data Source=(localdb)\\LocalDb2019;Initial Catalog=unit_test_concepts;Integrated Security=true;"))
    {
        await con.OpenAsync();

        await con.ExecuteAsync("TRUNCATE TABLE dbo.deduction;");
        await con.ExecuteAsync("TRUNCATE TABLE dbo.tax_table;");
        await con.ExecuteAsync("TRUNCATE TABLE dbo.timecard;");
        await con.ExecuteAsync("DELETE FROM dbo.payee;DBCC CHECKIDENT ('payee', RESEED, 0);");
        await con.ExecuteAsync("SET IDENTITY_INSERT dbo.payee ON;");

        //Create Payees        
        await foreach (var payee in repo.GetPayees())
        {
            await con.ExecuteAsync(payeeSQL, payee);

            DateTime dow = new DateTime(2023, 1, 29);

            //Generate timecards
            Random rand = new Random();

            while (dow > new DateTime(2022, 9, 1))
            {
                if (dow.DayOfWeek == DayOfWeek.Saturday || dow.DayOfWeek == DayOfWeek.Sunday)
                {
                    dow = dow.AddDays(-1);
                    continue;
                }

                var hours = rand.Next(4, 9);
                var partialHr = ((decimal)rand.Next(1, 99)) / 100;
                var totalTime = hours + partialHr;

                await con.ExecuteAsync(timecardSQL, new { EmployeeID = payee.ID, DateOfWork = dow.Date,  HoursWorked = totalTime });

                dow = dow.AddDays(-1);
            }
        }

        await con.ExecuteAsync("SET IDENTITY_INSERT dbo.payee OFF;");

        //Create tax tables
        await foreach (var tbl in repo.GetTaxTables())
        {
            foreach (var item in tbl.TaxRanges)
            {
                await con.ExecuteAsync(taxTblSQL, new
                {
                    tbl.State,
                    item.StartRange,
                    item.EndRange,
                    item.TaxRate
                });
            }
        }

        //Create Deductions
        await foreach (var ded in repo.GetDeductions())
        {
            await con.ExecuteAsync(deductionSQL, ded);
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine("Done!");