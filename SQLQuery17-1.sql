
-----  Assignment 2  -----

-- 1. Create a database named db_yourfirstname

USE master;
GO

CREATE DATABASE db_YoungChoi;
GO

-- Verify
SELECT name, size AS [Number of Pages(8KB) ]
FROM sys.master_files
WHERE name = N'db_YoungChoi';
GO

-- 2. Create Customer table with at least the following columns: (1/2 mark)
--    CustomerID INT NOT NULL
--	  FirstName Nvarchar(50 ) NOT NULL
--	  LastName Nvarchar(50) NOT NULL

USE db_YoungChoi;
GO

IF OBJECT_ID('Customer') IS NOT NULL
DROP TABLE Customer;
GO

CREATE TABLE Customer 
(
  CustomerID INT NOT NULL,
  FirstName NVARCHAR(50) NOT NULL,
  LastName NVARCHAR(50) NOT NULL
);
GO

-- Populate data
INSERT INTO Customer (CustomerID, FirstName, LastName)
VALUES (1001, 'John', 'Smith'),
       (1002, 'Tom', 'Johnson'),
	   (1003, 'Rob', 'Brown'),
	   (1004, 'James', 'Miller'),
	   (1005, 'Peter', 'Tylor');
GO

-- Test cases
SELECT *
FROM Customer
GO

-- 3. Create Orders table as follows: (1/2 mark)
--    OrderID INT Not NULL
--    CustomerID INT NOT NULL
--    OrderDate datetime Not NULL

IF OBJECT_ID('Orders') IS NOT NULL
DROP TABLE Orders;
GO

CREATE TABLE Orders 
(
  OrderID INT NOT NULL,
  CustomerID INT NOT NULL,
  OrderDate DATETIME NOT NULL
);
GO

-- Populate date
INSERT INTO Orders (OrderID, CustomerID, OrderDate)
VALUES (3001, 1001, '20150801'),
       (3002, 1002, '20150802'),
	   (3003, 1003, '20150803'),
	   (3004, 1004, '20150804');
GO

-- Test cases
SELECT *
FROM Orders
GO

-- 4. Use triggers to impose the following constraints (4 marks)
--    a) A Customer with Orders cannot be deleted from Customer table.
--    b) Create a custom error and use Raiserror to notify.
--    c) If CustomerID is updated in Customers, referencing rows in Orders must be updated accordingly.
--    d) Updating and Insertion of rows in Orders table must verify that CustomerID exists in Customer table, otherwise Raiserror to notify.


IF OBJECT_ID('tr_CustomerDML', 'TR') IS NOT NULL
DROP TRIGGER tr_CustomerDML;
GO

CREATE TRIGGER tr_CustomerDML
ON Customer
AFTER INSERT, DELETE, UPDATE
AS
BEGIN
	IF @@ROWCOUNT = 0 RETURN; 
	SET NOCOUNT ON;

    DECLARE @insertedID AS INT, @deletedID AS INT;
	SELECT @insertedID = CustomerID from inserted;
	SELECT @deletedID = CustomerID from deleted;

	-- a) A Customer with Orders cannot be deleted from Customer table.
    IF (@insertedID IS NULL) 
		AND EXISTS (SELECT * FROM Orders where CustomerID = @deletedID)
	BEGIN
	-- b) Create a custom error and use Raiserror to notify.
		RAISERROR (50001, 16, 1);
		THROW 50000, 'The customer with orders cannot be deleted.', 0;
		RETURN
	END;
	-- c) If CustomerID is updated in Customers, referencing rows in Orders must be updated accordingly.
	IF  (@insertedID IS NOT NULL  AND @deletedID IS NOT NULL)
		AND (EXISTS (SELECT * FROM Orders where CustomerID = @deletedID) )
	BEGIN
		UPDATE Orders
		SET CustomerID = @insertedID
		WHERE CustomerID = @deletedID;
	END;
END;
GO

-- Test cases
delete from Customer
where CustomerID = 1001; -- customer with orderr, RAISERROR & THROW
go

delete from Customer
where CustomerID = 1005; -- customer without order
go

UPDATE Customer
SET CustomerID = '1014'
WHERE CustomerID = 1004;
GO

--  d) Updating and Insertion of rows in Orders table must verify 
--     that CustomerID exists in Customer table, otherwise Raiserror to notify.

IF OBJECT_ID('tr_OrdersDML', 'TR') IS NOT NULL
DROP TRIGGER tr_OrdersDML;
GO

CREATE TRIGGER tr_OrdersDML
ON Orders
AFTER INSERT, UPDATE
AS
BEGIN
	IF @@ROWCOUNT = 0 RETURN; 
	SET NOCOUNT ON;

	DECLARE @insertedID AS INT, @deletedID AS INT;
	SELECT @insertedID = CustomerID from inserted;
	SELECT @deletedID = CustomerID from deleted;
	IF NOT EXISTS 
	(	SELECT *
		FROM Customer 
		where CustomerID = @insertedID
	)
	BEGIN
		RAISERROR (50001, 16, 1);
		THROW 50000, 'invalid Insert or Update attempt', 0; 
	END;
END;
GO

-- Test cases --
SELECT * FROM Customer;
SELECT * FROM Orders;
GO

-- pass
INSERT INTO Orders (OrderID, CustomerID, OrderDate)
VALUES (3005, 1001, '20150806');
GO

update Orders
set OrderID = 4002
where OrderID = 3002;
go

-- fail
INSERT INTO Orders (OrderID, CustomerID, OrderDate)
VALUES (3010, 1010, '20150806');
go

-- 5. Create a function named CheckName to check 
--    that the Firstname and Lastname of a customer are not the same. (2 marks)

IF OBJECT_ID('fn_CheckName', 'fn') IS NOT NULL
DROP FUNCTION fn_CheckName;
GO

CREATE FUNCTION fn_CheckName
(
    @FirstName AS VARCHAR(50),
    @LastName AS VARCHAR(50)
)
RETURNS INT 
AS 
BEGIN
	DECLARE @NameCount as INT
    SELECT @NameCount =  COUNT(*)
	FROM Customer
	WHERE FirstName = @FirstName AND LastName = @LastName
	GROUP BY FirstName
	RETURN @NameCount
END;
GO

-- Test cases --
SELECT *
FROM Customer
GO

select dbo.fn_CheckName('John', 'Smith');
go

execute dbo.fn_CheckName 
	@FirstName = 'John', 
	@Lastname = 'Smith';
go

-- 6. Create a stored procedure called Proc_InsertCustomer 
--    that would take Firstname and Lastname and optional CustomerID 
--    as parameters and Insert into Customer table.
--    > If CustomerID is not provided, increment the last CustomerID and use that.
--    > Use the CheckName function to verify that the customer name is correct. (4 marks)

IF OBJECT_ID('Proc_InsertCustomer', 'P') IS NOT NULL
DROP PROC Proc_InsertCustomer;
GO

CREATE PROCEDURE Proc_InsertCustomer 
	@CustomerID as INT =  0,	-- default value, to make the parameter optional
	@FirstName as nVARCHAR(50), 
	@LastName as nVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	-- Check invalid or empty customer id
	IF @CustomerID < 1000 OR @CustomerID > 9999  -- check range
	BEGIN
		SELECT @CustomerID = (MAX(CustomerID) + 1) FROM Customer
		PRINT 'Invalid CustomerID - New CustomerID is allocated : ' + CONVERT(varchar, @CustomerID)
	END
	-- To prevent from inserting a same name
	DECLARE @DuplicationCheck AS INT;
	SET @DuplicationCheck = (select dbo.fn_CheckName (@FirstName,@LastName))
	IF @DuplicationCheck > 0
	BEGIN
		PRINT 'The same name exists in the Customer table: ' + @FirstName + ' ' + @LastName + ' ' 
		PRINT 'Please double check! Proc_InsertCustomer terminated.'
		PRINT 'Number of duplication: ' + CONVERT (VARCHAR, @DuplicationCheck)
		RETURN
	END
	-- Insert data to table
	INSERT INTO Customer (CustomerID, FirstName, LastName)
	VALUES ( @CustomerID, @FirstName, @LastName); 
	RETURN;
END;
GO

-- Test case
exec Proc_InsertCustomer  -- success
	@CustomerID = 1022, 
	@FirstName = 'Tim', 
	@LastName = 'Smith';
go

exec Proc_InsertCustomer -- failure
	@CustomerID = 1023, 
	@FirstName = 'John', 
	@LastName = 'Smith';
go

select * from Customer;
go

-- 7. Log all updates to Customer table to CusAudit table. 
--    > Indicate the previous and new values of data, 
--    > the date and time and the login name of the person who made the changes. (4 marks)

IF OBJECT_ID('CusAudit') IS NOT NULL
DROP TABLE CusAudit;
GO

CREATE TABLE CusAudit 
(
  PreCustomerID INT NULL, PreFirstName NVARCHAR(50) NULL, PreLastName NVARCHAR(50) NULL,
  NewCustomerID INT NULL, NewFirstName NVARCHAR(50) NULL, NewLastName NVARCHAR(50) NULL,
  ModifiedBy NVARCHAR(50) NOT NULL,
  DateModified DATETIME NOT NULL
);
GO

IF OBJECT_ID('tr_ModificationLog_Customer', 'TR') IS NOT NULL
DROP TRIGGER tr_ModificationLog_Customer;
GO

CREATE TRIGGER tr_ModificationLog_Customer
ON Customer
AFTER DELETE, INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @PreCustomerID as INT, @PreFirstName as NVARCHAR(50), @PreLastName as NVARCHAR(50);
	DECLARE @NewCustomerID as INT, @NewFirstName as NVARCHAR(50), @NewLastName as NVARCHAR(50);
	DECLARE @ModifiedBy as NVARCHAR(50); 
	DECLARE @DateModified as DATETIME;
	
	select @DateModified = SYSDATETIME();
	select @ModifiedBy = ORIGINAL_LOGIN();
	select @PreCustomerID = CustomerID, @PreFirstName = FirstName, @PreLastName = LastName from deleted;
	select @NewCustomerID = CustomerID, @NewFirstName = FirstName, @NewLastName = LastName from inserted;
	Insert INTO CusAudit
	VALUES (@PreCustomerID, @PreFirstName, @PreLastName, 
			@NewCustomerID, @NewFirstName, @NewLastName,
			@ModifiedBy,
			@DateModified);
	SELECT * FROM CusAudit; -- For debugging
END;
GO

--- test cases
update Customer
set FirstName = 'Ron'
where CustomerID = 1002;
go

INSERT INTO Customer (CustomerID, FirstName, LastName)
VALUES (1025, 'Rob', 'Smith');

delete Customer
where CustomerID = 1025;

select *
from Customer
GO

select *
from CusAudit
GO

