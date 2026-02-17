CREATE TABLE [dbo].[PedidoVentaDetalle] (
    [PedidoVentaDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [PedidoVentaId]        INT             NOT NULL,
    [ProductoId]           INT             NOT NULL,
    [Cantidad]             DECIMAL (18, 2) NOT NULL,
    [PrecioUnitario]       DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([PedidoVentaDetalleId] ASC),
    CONSTRAINT [CK_PVD_Cantidad] CHECK ([Cantidad]>(0)),
    CONSTRAINT [FK_PVD_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId]),
    CONSTRAINT [FK_PVD_PV] FOREIGN KEY ([PedidoVentaId]) REFERENCES [dbo].[PedidoVenta] ([PedidoVentaId])
);


GO
CREATE NONCLUSTERED INDEX [IX_PVD_PV]
    ON [dbo].[PedidoVentaDetalle]([PedidoVentaId] ASC);


GO
CREATE   TRIGGER dbo.tr_AUD_PedidoVentaDetalle
ON dbo.PedidoVentaDetalle
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @accion NVARCHAR(10) =
        CASE WHEN EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted) THEN 'UPDATE'
             WHEN EXISTS(SELECT 1 FROM inserted) THEN 'INSERT' ELSE 'DELETE' END;
    DECLARE @cnt INT =
        CASE WHEN @accion IN ('INSERT','UPDATE') THEN (SELECT COUNT(*) FROM inserted)
             ELSE (SELECT COUNT(*) FROM deleted) END;

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.PedidoVentaDetalle', @accion, @cnt);
END