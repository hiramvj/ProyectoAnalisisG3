CREATE TABLE [dbo].[IncidenciaEntrega] (
    [IncidenciaId]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [EntregaId]     INT            NOT NULL,
    [Tipo]          NVARCHAR (20)  NOT NULL,
    [Descripcion]   NVARCHAR (250) NOT NULL,
    [FechaRegistro] DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([IncidenciaId] ASC),
    CONSTRAINT [CK_Inc_Tipo] CHECK ([Tipo]='DEVOLUCION' OR [Tipo]='RECHAZO' OR [Tipo]='FALTANTE' OR [Tipo]='DAÑO' OR [Tipo]='RETRASO'),
    CONSTRAINT [FK_Inc_Entrega] FOREIGN KEY ([EntregaId]) REFERENCES [dbo].[Entrega] ([EntregaId])
);

