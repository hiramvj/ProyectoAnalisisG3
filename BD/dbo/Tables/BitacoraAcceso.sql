CREATE TABLE [dbo].[BitacoraAcceso] (
    [BitacoraId]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [UsuarioId]   INT            NULL,
    [FechaEvento] DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [Evento]      NVARCHAR (50)  NOT NULL,
    [IP]          NVARCHAR (45)  NULL,
    [UserAgent]   NVARCHAR (200) NULL,
    [Detalle]     NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([BitacoraId] ASC),
    CONSTRAINT [FK_Bitacora_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId])
);

