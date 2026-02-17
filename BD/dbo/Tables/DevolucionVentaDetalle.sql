CREATE TABLE [dbo].[DevolucionVentaDetalle] (
    [DevolucionDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [DevolucionId]        INT             NOT NULL,
    [ProductoId]          INT             NOT NULL,
    [Cantidad]            DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([DevolucionDetalleId] ASC),
    CONSTRAINT [CK_DVD_Cantidad] CHECK ([Cantidad]>(0)),
    CONSTRAINT [FK_DVD_Dev] FOREIGN KEY ([DevolucionId]) REFERENCES [dbo].[DevolucionVenta] ([DevolucionId]),
    CONSTRAINT [FK_DVD_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

