CREATE TABLE [dbo].[ListaPrecioDetalle] (
    [ListaPrecioDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [ListaPrecioId]        INT             NOT NULL,
    [ProductoId]           INT             NOT NULL,
    [Precio]               DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([ListaPrecioDetalleId] ASC),
    CONSTRAINT [CK_LPD_Precio] CHECK ([Precio]>=(0)),
    CONSTRAINT [FK_LPD_Lista] FOREIGN KEY ([ListaPrecioId]) REFERENCES [dbo].[ListaPrecio] ([ListaPrecioId]),
    CONSTRAINT [FK_LPD_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId]),
    CONSTRAINT [UQ_LPD] UNIQUE NONCLUSTERED ([ListaPrecioId] ASC, [ProductoId] ASC)
);

