CREATE TABLE [dbo].[MovimientoInventario] (
    [MovimientoId]    BIGINT          IDENTITY (1, 1) NOT NULL,
    [FechaMovimiento] DATETIME2 (7)   DEFAULT (sysdatetime()) NOT NULL,
    [BodegaId]        INT             NOT NULL,
    [ProductoId]      INT             NOT NULL,
    [TipoMovimiento]  NVARCHAR (10)   NOT NULL,
    [Cantidad]        DECIMAL (18, 2) NOT NULL,
    [Referencia]      NVARCHAR (50)   NULL,
    [Nota]            NVARCHAR (250)  NULL,
    PRIMARY KEY CLUSTERED ([MovimientoId] ASC),
    CONSTRAINT [CK_TipoMovimiento] CHECK ([TipoMovimiento]='AJUSTE' OR [TipoMovimiento]='SALIDA' OR [TipoMovimiento]='ENTRADA'),
    CONSTRAINT [FK_MovInv_Bodega] FOREIGN KEY ([BodegaId]) REFERENCES [dbo].[Bodega] ([BodegaId]),
    CONSTRAINT [FK_MovInv_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);


GO
CREATE NONCLUSTERED INDEX [IX_MovInv_Prod_Fecha]
    ON [dbo].[MovimientoInventario]([ProductoId] ASC, [FechaMovimiento] DESC);

