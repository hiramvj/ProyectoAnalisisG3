CREATE TABLE [dbo].[PasswordResetToken] (
    [ResetId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [UsuarioId]   INT            NOT NULL,
    [Token]       NVARCHAR (120) NOT NULL,
    [ExpiraEn]    DATETIME2 (7)  NOT NULL,
    [Usado]       BIT            DEFAULT ((0)) NOT NULL,
    [FechaCreado] DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ResetId] ASC),
    CONSTRAINT [FK_Reset_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId]),
    UNIQUE NONCLUSTERED ([Token] ASC)
);

