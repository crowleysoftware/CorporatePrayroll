namespace CorporatePrayroll
{
    public class PrayrollRepository : IPayrollRepository
    {
        public async IAsyncEnumerable<Payee> GetPayees()
        {
            yield return new Payee { ID = 382, FirstName = "Dante", LastName = "Hicks", DateOfHire = new DateTime(2018, 11, 22), Exempt = false, PayRate = 22.17m, StateOfResidence = "NJ" };
            yield return new Payee { ID = 189, FirstName = "Randal", LastName = "Graves", DateOfHire = new DateTime(2021, 12, 25), Exempt = false, PayRate = 22.18m, StateOfResidence = "OH" };
            yield return new Payee { ID = 14, FirstName = "Silent", LastName = "Bob", DateOfHire = new DateTime(2022, 7, 4), Exempt = false, PayRate = 63.38m, StateOfResidence = "NY" };
            yield return new Payee { ID = 9901, FirstName = "Jay", LastName = "", DateOfHire = new DateTime(2020, 10, 31), Exempt = false, PayRate = 6.07m, StateOfResidence = "NJ" };
            yield return new Payee { ID = 438, FirstName = "Caitlin", LastName = "Bree", DateOfHire = new DateTime(2019, 1, 1), Exempt = true, PayRate = 31.10m, StateOfResidence = "CA" };
            yield return new Payee { ID = 217, FirstName = "Veronica", LastName = "Loughran", DateOfHire = new DateTime(2020, 2, 13), Exempt = true, PayRate = 29.01m, StateOfResidence = "NJ" };
        }

        public async Task<TimeCard> GetTimecardByEmployeeID(int employeeID, DateTime payrollDate)
        {
            //get random hour amount
            Random random = new Random();

            TimeCard card = new TimeCard { EmployeeID = employeeID, TimeEntries = new List<TimeEntry>() };

            for (int i = 0; i < 5; i++)
            {
                var hours = random.Next(1, 12);
                var partialHr = ((decimal)random.Next(1, 99)) / 100;
                var totalTime = hours + partialHr;

                if (employeeID == 438 || employeeID == 217)
                {
                    totalTime = 8.0m;
                }

                card.TimeEntries.Add(new TimeEntry { DateOfWork = payrollDate.AddDays(-i), HoursWorked = totalTime });
            }

            return await Task.FromResult(card);
        }

        public async IAsyncEnumerable<TimeCard> GetTimeCards()
        {
            yield return await GetTimecardByEmployeeID(382, DateTime.Now);
            yield return await GetTimecardByEmployeeID(189, DateTime.Now);
            yield return await GetTimecardByEmployeeID(14, DateTime.Now);
            yield return await GetTimecardByEmployeeID(9901, DateTime.Now);
            yield return await GetTimecardByEmployeeID(438, DateTime.Now);
            yield return await GetTimecardByEmployeeID(217, DateTime.Now);
        }

        public async IAsyncEnumerable<Deduction> GetDeductionsByEmployeeID(int employeeID)
        {
            switch (employeeID)
            {
                case 382:
                    yield return new Deduction { EmployeeID = 382, DeductionName = "Christmas Club", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 10.00m, effective_date = new DateTime(2022, 5, 23) };
                    yield return new Deduction { EmployeeID = 382, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 35.00m, effective_date = new DateTime(2022, 5, 23) };
                    break;
                case 189:
                    yield return new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1200.00m, effective_date = new DateTime(2022, 5, 23) };
                    yield return new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1300.00m, effective_date = new DateTime(2023, 1, 1) };
                    yield return new Deduction { EmployeeID = 189, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) };
                    break;
                case 14:
                    yield return new Deduction { EmployeeID = 14, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 900.00m, effective_date = new DateTime(2022, 5, 23) };
                    break;
                case 9901:
                    yield return new Deduction { EmployeeID = 9901, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 800.00m, effective_date = new DateTime(2022, 5, 23) };
                    yield return new Deduction { EmployeeID = 9901, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) };
                    yield return new Deduction { EmployeeID = 9901, DeductionName = "Garnishment", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 109.32m, effective_date = new DateTime(2022, 5, 23) };
                    break;
                default:
                    //yield return null;
                    break;
            }
        }

        public async IAsyncEnumerable<Deduction> GetDeductions()
        {
            yield return new Deduction { EmployeeID = 382, DeductionName = "Christmas Club", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 10.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 382, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 35.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1300.00m, effective_date = new DateTime(2023, 1, 1) };
            yield return new Deduction { EmployeeID = 189, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 1200.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 189, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 14, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 900.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 9901, DeductionName = "Med Flex Spending", DeductionFrequency = DeductionFrequency.Yearly, Amount = 800.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 9901, DeductionName = "401K", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 88.00m, effective_date = new DateTime(2022, 5, 23) };
            yield return new Deduction { EmployeeID = 9901, DeductionName = "Garnishment", DeductionFrequency = DeductionFrequency.PerPeriod, Amount = 109.32m, effective_date = new DateTime(2022, 5, 23) };
        }

        public async IAsyncEnumerable<TaxTable> GetTaxTables()
        {
            yield return
                new TaxTable
                {
                    State = "ME",
                    TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 500m, TaxRate = .1m },
                        new TaxRange { StartRange = 500.01m, EndRange = 1000m, TaxRate = .25m },
                        new TaxRange { StartRange = 1000.01m, EndRange = 1500m, TaxRate = .31m },
                        new TaxRange { StartRange = 1500.01m, EndRange = 1000000, TaxRate = .43m }
                        }
                };
            yield return new TaxTable
            {
                State = "NJ",
                TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 800m, TaxRate = .12m },
                        new TaxRange { StartRange = 800.01m, EndRange = 1900m, TaxRate = .19m },
                        new TaxRange { StartRange = 1900.01m, EndRange = 3100m, TaxRate = .28m },
                        new TaxRange { StartRange = 3100.01m, EndRange = 1000000, TaxRate = .37m }
                    }
            };
            yield return new TaxTable
            {
                State = "NY",
                TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 750m, TaxRate = .22m },
                        new TaxRange { StartRange = 750.01m, EndRange = 1000m, TaxRate = .28m },
                        new TaxRange { StartRange = 1000.01m, EndRange = 3100m, TaxRate = .33m },
                        new TaxRange { StartRange = 3100.01m, EndRange = 1000000, TaxRate = .40m }
                    }
            };
            yield return new TaxTable
            {
                State = "OH",
                TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 320m, TaxRate = .19m },
                        new TaxRange { StartRange = 320.01m, EndRange = 1018m, TaxRate = .23m },
                        new TaxRange { StartRange = 1018.01m, EndRange = 2900m, TaxRate = .37m },
                        new TaxRange { StartRange = 2900.01m, EndRange = 1000000, TaxRate = .42m }
                    }
            };
            yield return new TaxTable
            {
                State = "CA",
                TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 1300m, TaxRate = .21m },
                        new TaxRange { StartRange = 1300.01m, EndRange = 2300m, TaxRate = .28m },
                        new TaxRange { StartRange = 2300.01m, EndRange = 3300, TaxRate = .34m },
                        new TaxRange { StartRange = 3300.01m, EndRange = 1000000, TaxRate = .40m }
                    }
            };
        }

        public async Task<TaxTable> GetTaxTables(string stateCode)
        {
            switch (stateCode)
            {
                case "ME":
                    return await Task.FromResult(new TaxTable
                    {
                        State = "ME",
                        TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 500m, TaxRate = .1m },
                        new TaxRange { StartRange = 500.01m, EndRange = 1000m, TaxRate = .25m },
                        new TaxRange { StartRange = 1000.01m, EndRange = 1500m, TaxRate = .31m },
                        new TaxRange { StartRange = 1500.01m, EndRange = 1000000, TaxRate = .43m }
                        }
                    });
                case "NJ":
                    return await Task.FromResult(new TaxTable
                    {
                        State = "NJ",
                        TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 800m, TaxRate = .12m },
                        new TaxRange { StartRange = 800.01m, EndRange = 1900m, TaxRate = .19m },
                        new TaxRange { StartRange = 1900.01m, EndRange = 3100m, TaxRate = .28m },
                        new TaxRange { StartRange = 3100.01m, EndRange = 1000000, TaxRate = .37m }
                    }
                    });
                case "NY":
                    return await Task.FromResult(new TaxTable
                    {
                        State = "NY",
                        TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 750m, TaxRate = .22m },
                        new TaxRange { StartRange = 750.01m, EndRange = 1000m, TaxRate = .28m },
                        new TaxRange { StartRange = 1000.01m, EndRange = 3100m, TaxRate = .33m },
                        new TaxRange { StartRange = 3100.01m, EndRange = 1000000, TaxRate = .40m }
                    }
                    }
);
                case "OH":
                    return await Task.FromResult(new TaxTable
                    {
                        State = "OH",
                        TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 320m, TaxRate = .19m },
                        new TaxRange { StartRange = 320.01m, EndRange = 1018m, TaxRate = .23m },
                        new TaxRange { StartRange = 1018.01m, EndRange = 2900m, TaxRate = .37m },
                        new TaxRange { StartRange = 2900.01m, EndRange = 1000000, TaxRate = .42m }
                    }
                    });
                case "CA":
                    return await Task.FromResult(new TaxTable
                    {
                        State = "CA",
                        TaxRanges = new List<TaxRange>{
                        new TaxRange { StartRange = 0m, EndRange = 1300m, TaxRate = .21m },
                        new TaxRange { StartRange = 1300.01m, EndRange = 2300m, TaxRate = .28m },
                        new TaxRange { StartRange = 2300.01m, EndRange = 3300, TaxRate = .34m },
                        new TaxRange { StartRange = 3300.01m, EndRange = 1000000, TaxRate = .40m }
                    }
                    });

                default:
                    return null;
            }
        }
    }
}
