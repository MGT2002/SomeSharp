
 --- SQL Generation Logic ---
-- Create Table: Users
CREATE TABLE [Users] (
    [UserId] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [DateCreated] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED (
        [UserId]
    )
);
GO

-- Create Table: Products
CREATE TABLE [Products] (
    [ProductId] INT IDENTITY(1,1) NOT NULL,
    [ProductName] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Price] DECIMAL(10, 2) NOT NULL,
    [StockQuantity] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED (
        [ProductId]
    )
);
GO

-- Create Table: Orders
CREATE TABLE [Orders] (
    [OrderId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [TotalAmount] DECIMAL(10, 2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED (
        [OrderId]
    )
);
GO

-- Add Foreign Key for Orders.UserId to Users.UserId
ALTER TABLE [Orders]
ADD CONSTRAINT [FK_Orders_Users] FOREIGN KEY ([UserId])
REFERENCES [Users] ([UserId]);
GO


-- End of SQL generation.

