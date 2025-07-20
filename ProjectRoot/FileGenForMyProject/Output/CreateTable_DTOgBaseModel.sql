CREATE TABLE [DTOg].[BaseModel] (
    [Id] BIGINT NOT NULL,  -- Id of table
	[Name] NVARCHAR(MAX) NULL,  -- Name of table
	[Description] NVARCHAR(MAX) NULL, 
	[Type] NVARCHAR(MAX) NULL, 
	[CreatedDate] Date NOT NULL,  -- Crated date which should alwasy be DateOnly -> Date
	[CreatedUserId] BIGINT NOT NULL,  -- Foregin key to user
);

ALTER TABLE [DTOg].[BaseModel] ADD CONSTRAINT [PK_DTOg_BaseModel] PRIMARY KEY ([Id]);

ALTER TABLE [DTOg].[BaseModel]
ADD CONSTRAINT [FK_DTOg_BaseModel_CreatedUserId]
FOREIGN KEY ([CreatedUserId])
REFERENCES [ASD].[User]([Id]);