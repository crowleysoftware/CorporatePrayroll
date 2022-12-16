create table deduction_frequency
(
    Name  nvarchar(20) not null collate SQL_Latin1_General_CP1_CI_AS
        constraint deduction_frequency_pk
            primary key,
    Value int
)
go

create table payee
(
    ID               int identity
        constraint payee_pk
            primary key,
    FirstName        nvarchar(50) collate SQL_Latin1_General_CP1_CI_AS,
    LastName         nvarchar(50) collate SQL_Latin1_General_CP1_CI_AS,
    DateOfHire       datetime2,
    Exempt           bit,
    PayRate          money,
    StateOfResidence char(2) collate SQL_Latin1_General_CP1_CI_AS
)
go

create table deduction
(
    ID                 int identity
        constraint deduction_pk
            primary key,
    EmployeeID         int
        constraint deduction_payee_ID_fk
            references payee
            on update cascade,
    DeductionName      nvarchar(50),
    DeductionFrequency int,
    Amount             money,
    effective_date     date
)
go

create table tax_table
(
    ID         int identity
        constraint tax_table_pk
            primary key,
    State      char(2) collate SQL_Latin1_General_CP1_CI_AS,
    StartRange money,
    EndRange   money,
    TaxRate    decimal(5, 2)
)
go

create table timecard
(
    ID          int identity
        constraint timecard_pk
            primary key,
    EmployeeID  int
        constraint timecard_payee_ID_fk
            references payee
            on update cascade,
    DateOfWork  datetime2,
    HoursWorked decimal(5, 3)
)
go

