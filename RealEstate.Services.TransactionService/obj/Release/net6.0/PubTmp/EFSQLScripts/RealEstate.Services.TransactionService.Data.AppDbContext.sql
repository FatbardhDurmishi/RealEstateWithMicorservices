IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231123123039_SqlServer')
BEGIN
    CREATE TABLE [Transactions] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime NOT NULL,
        [OwnerId] nvarchar(450) NULL,
        [BuyerId] nvarchar(450) NULL,
        [PropertyId] int NULL,
        [RentStartDate] datetime NOT NULL,
        [RentEndDate] datetime NOT NULL,
        [RentPrice] decimal(18,2) NULL,
        [TotalPrice] decimal(18,2) NULL,
        [Status] nvarchar(50) NULL,
        [TransactionType] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231123123039_SqlServer')
BEGIN
    CREATE TABLE [TransactionTypes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_TransactionTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231123123039_SqlServer')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231123123039_SqlServer', N'6.0.21');
END;
GO

COMMIT;
GO

