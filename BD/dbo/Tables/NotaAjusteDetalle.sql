CREATE TABLE [dbo].[NotaAjusteDetalle] (
    [NotaDetalleId]  INT             IDENTITY (1, 1) NOT NULL,
    [NotaId]         INT             NOT NULL,
    [ProductoId]     INT             NOT NULL,
    [Cantidad]       DECIMAL (18, 2) NOT NULL,
    [PrecioUnitario] DECIMAL (18, 2) NOT NULL,
    [TotalLinea]     AS              ([Cantidad]*[PrecioUnitario]) PERSISTED,
    PRIMARY KEY CLUSTERED ([NotaDetalleId] ASC),
    CONSTRAINT [CK_ND_Cantidad] CHECK ([Cantidad]>(0)),
    CONSTRAINT [FK_ND_Nota] FOREIGN KEY ([NotaId]) REFERENCES [dbo].[NotaAjuste] ([NotaId]),
    CONSTRAINT [FK_ND_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

