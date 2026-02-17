CREATE TABLE [dbo].[EntregaEvidencia] (
    [EvidenciaId]   BIGINT          IDENTITY (1, 1) NOT NULL,
    [EntregaId]     INT             NOT NULL,
    [Tipo]          NVARCHAR (10)   NOT NULL,
    [UrlArchivo]    NVARCHAR (300)  NOT NULL,
    [RecibidoPor]   NVARCHAR (150)  NULL,
    [Latitud]       DECIMAL (10, 7) NULL,
    [Longitud]      DECIMAL (10, 7) NULL,
    [FechaRegistro] DATETIME2 (7)   DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([EvidenciaId] ASC),
    CONSTRAINT [CK_Ev_Tipo] CHECK ([Tipo]='FOTO' OR [Tipo]='FIRMA'),
    CONSTRAINT [FK_Ev_Entrega] FOREIGN KEY ([EntregaId]) REFERENCES [dbo].[Entrega] ([EntregaId])
);

