CREATE TABLE [FujiUser] (
  [ID]                  INT           NOT NULL IDENTITY(1, 1) PRIMARY KEY,
  [ASPNetIdentityId]    NVARCHAR(450) NOT NULL,
  [FirstName]           NVARCHAR(50)  NOT NULL,
  [LastName]            NVARCHAR(50)  NOT NULL
);

CREATE TABLE [Apple] (
  [ID]              INT NOT NULL IDENTITY(1, 1) PRIMARY KEY,
  [VarietyName]     NVARCHAR(50) NOT NULL,
  [ScientificName]  NVARCHAR(100)
);

CREATE TABLE [ApplesConsumed] (
  [ID]          INT      NOT NULL IDENTITY(1, 1) PRIMARY KEY,
  [FujiUserID]  INT      NOT NULL,
  [AppleID]     INT      NOT NULL,
  [Count]       INT      NOT NULL,
  [ConsumedAt]  DATETIME NOT NULL
);

ALTER TABLE [ApplesConsumed] ADD CONSTRAINT [ApplesConsumed_fk_FujiUser] FOREIGN KEY ([FujiUserID]) REFERENCES [FujiUser] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE [ApplesConsumed] ADD CONSTRAINT [ApplesConsumed_fk_Apple] FOREIGN KEY ([AppleID]) REFERENCES [Apple] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION;