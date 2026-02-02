CREATE TABLE [dbo].[CotizacionDetalle] (
    [CotizacionDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [CotizacionId]        INT             NOT NULL,
    [ProductoId]          INT             NOT NULL,
    [Cantidad]            DECIMAL (18, 2) NOT NULL,
    [PrecioUnitario]      DECIMAL (18, 2) NOT NULL,
    [DescuentoMonto]      DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [TotalLinea]          AS              ([Cantidad]*[PrecioUnitario]-[DescuentoMonto]) PERSISTED,
    PRIMARY KEY CLUSTERED ([CotizacionDetalleId] ASC),
    CONSTRAINT [CK_CD_Cantidad] CHECK ([Cantidad]>(0)),
    CONSTRAINT [CK_CD_Desc] CHECK ([DescuentoMonto]>=(0)),
    CONSTRAINT [FK_CD_Cot] FOREIGN KEY ([CotizacionId]) REFERENCES [dbo].[Cotizacion] ([CotizacionId]),
    CONSTRAINT [FK_CD_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

