--------------------------
-----  Assignment 2  -----
--------------------------

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

INSERT INTO Customer (CustomerID, FirstName, LastName)
VALUES (1001, 'John', 'Smith'),
       (1002, 'Tom', 'Johnson'),
	   (1003, 'Rob', 'Brown'),
	   (1004, 'James', 'Miller'),
	   (1005, 'Peter', 'Tylor');
GO

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

INSERT INTO Orders (OrderID, CustomerID, OrderDate)
VALUES (3001, 1001, '20150801'),
       (3002, 1002, '20150802'),
	   (3003, 1003, '20150803'),
	   (3004, 1004, '20150804');
GO

SELECT *
FROM Orders
GO

-- 4. Use triggers to impose the following constraints (4 marks)
--    a) A Customer with Orders cannot be deleted from Customer table.


IF OBJECT_ID('tr_CustomerDML', 'TR') IS NOT NULL
DROP TRIGGER tr_CustomerDML;
GO

CREATE TRIGGER tr_CustomerDML
ON Customer
AFTER DELETE
AS
BEGIN
  IF @@ROWCOUNT = 0 RETURN; 
  SET NOCOUNT ON;
  select CustomerID from deleted
  SELECT * FROM deleted;
  SELECT * FROM Customer;
    IF EXISTS (SELECT *
		FROM Orders 
		where CustomerID in (
			select CustomerID from deleted))
	BEGIN
	  THROW 50000, 'The customer with orders cannot be deleted.', 0;
	END;
END;
GO

--    b) Create a custom error and use Raiserror to notify.

IF OBJECT_ID('tr_CustomerDML', 'TR') IS NOT NULL
DROP TRIGGER tr_CustomerDML;
GO

CREATE TRIGGER tr_CustomerDML
ON Customer
AFTER DELETE
AS
BEGIN
  IF @@ROWCOUNT = 0 RETURN; 
  SET NOCOUNT ON;
  select CustomerID from deleted
  SELECT * FROM deleted;
  SELECT * FROM Customer;
    IF EXISTS (SELECT *
		FROM Orders 
		where CustomerID in (
			select CustomerID from deleted))
	BEGIN
	  RAISERROR (50001, 16, 1);
	END;
END;
GO

--    c) If CustomerID is updated in Customers, 
--       referencing rows in Orders must be updated accordingly.

IF OBJECT_ID('tr_CustomerDML', 'TR') IS NOT NULL
DROP TRIGGER tr_CustomerDML;
GO

CREATE TRIGGER tr_CustomerDML
ON Customer
AFTER UPDATE
AS
BEGIN
  IF @@ROWCOUNT = 0 RETURN; 
  SET NOCOUNT ON;
  DECLARE @insertedID AS INT, @deletedID AS INT;
  SELECT @insertedID = CustomerID from inserted;
  SELECT @deletedID = CustomerID from deleted;
  select CustomerID as inserted from inserted;
  select CustomerID as deleted from deleted;
  SELECT * FROM inserted;
  SELECT * FROM Customer;
    IF EXISTS 
	(	SELECT *
		FROM Orders 
		where CustomerID in (select CustomerID from deleted)
	)
	BEGIN
	  UPDATE Orders
	  SET CustomerID = @insertedID
	  WHERE CustomerID = @deletedID;
	  select * from Orders;
	END;
END;
GO

--    d) Updating and Insertion of rows in Orders table must verify 
--       that CustomerID exists in Customer table, otherwise Raiserror to notify.

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
	-- DECLARE @insertedID AS INT, @deletedID AS INT;
	-- SELECT @insertedID = CustomerID from inserted;
	-- SELECT @deletedID = CustomerID from deleted;
	select CustomerID as inserted from inserted;
	select CustomerID as deleted from deleted;
	SELECT * FROM inserted;
	SELECT * FROM deleted;
	select * from Customer;
	SELECT * FROM Orders;
		IF NOT EXISTS 
		(	SELECT *
			FROM Customer 
			where CustomerID in (select CustomerID from inserted)
		)
		BEGIN
			RAISERROR (50001, 16, 1);
			THROW 50000, 'invalid Insert or Update attempt', 0; 
		END;
END;
GO


-- pass
INSERT INTO Orders (OrderID, CustomerID, OrderDate)
VALUES (3005, 1001, '20150806');
go

-- fail
INSERT INTO Orders (OrderID, CustomerID, OrderDate)
VALUES (3010, 1010, '20150806');
go

delete from Orders
where CustomerID = 1001;


SELECT *
FROM Orders
GO

SELECT *
FROM Customer
GO

INSERT INTO Customer (CustomerID, FirstName, LastName)
VALUES (1001, 'John', 'Smith');


UPDATE Customer
SET CustomerID = '1010'
WHERE CustomerID = 1001;

delete from Customer
where CustomerID = 1003;

delete from Customer
where CustomerID = 1005;



select c.CustomerID, c.FirstName, c.LastName, o.OrderID, o.OrderDate 
from Customer as c
left join Orders as o
on c.CustomerID = o.CustomerID
go

select c.CustomerID, c.FirstName, c.LastName, o.OrderID, o.OrderDate 
from Customer as c
left join Orders as o
on c.CustomerID = o.CustomerID
go




SELECT *
    from Customer as c
	JOIN Orders as o
	ON c.CustomerID = o.CustomerID


	SELECT *
    from Customer as c
	left JOIN Orders as o
	ON c.CustomerID = o.CustomerID
	WHERE o.OrderID IS NULL

SELECT *
    from Customer as c
	JOIN Orders as o
	ON c.CustomerID = o.CustomerID
	WHERE o.OrderID IS not NULL and c.CustomerID = 1003


