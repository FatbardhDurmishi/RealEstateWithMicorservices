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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231123165818_SqlServer'
)
BEGIN
    CREATE TABLE [PropertyTypes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_PropertyTypes] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231123165818_SqlServer'
)
BEGIN
    CREATE TABLE [Properties] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [BedRooms] int NULL,
        [BathRooms] int NULL,
        [Area] decimal(18,2) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [State] nvarchar(50) NOT NULL,
        [City] nvarchar(50) NOT NULL,
        [StreetAddress] nvarchar(50) NOT NULL,
        [CoverImageUrl] nvarchar(max) NOT NULL,
        [TransactionType] nvarchar(max) NOT NULL,
        [UserId] nvarchar(450) NULL,
        [PropertyTypeId] int NULL,
        CONSTRAINT [PK_Properties] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Properties_PropertyTypes_PropertyTypeId] FOREIGN KEY ([PropertyTypeId]) REFERENCES [PropertyTypes] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231123165818_SqlServer'
)
BEGIN
    CREATE TABLE [PropertyImages] (
        [Id] int NOT NULL IDENTITY,
        [ImageUrl] nvarchar(max) NOT NULL,
        [PropertyId] int NULL,
        CONSTRAINT [PK_PropertyImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PropertyImages_Properties_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231123165818_SqlServer'
)
BEGIN
    CREATE INDEX [IX_Properties_PropertyTypeId] ON [Properties] ([PropertyTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231123165818_SqlServer'
)
BEGIN
    CREATE INDEX [IX_PropertyImages_PropertyId] ON [PropertyImages] ([PropertyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20231123165818_SqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231123165818_SqlServer', N'8.0.1');
END;
GO

COMMIT;
GO

