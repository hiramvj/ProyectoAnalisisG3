CREATE TABLE [dbo].[Auditoria] (
    [AuditoriaId]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [FechaEvento]   DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [Tabla]         NVARCHAR (128) NOT NULL,
    [Accion]        NVARCHAR (10)  NOT NULL,
    [CantidadFilas] INT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([AuditoriaId] ASC)
);

