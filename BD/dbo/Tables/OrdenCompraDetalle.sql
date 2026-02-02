CREATE TABLE [dbo].[OrdenCompraDetalle] (
    [OrdenCompraDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [OrdenCompraId]        INT             NOT NULL,
    [ProductoId]           INT             NOT NULL,
    [Cantidad]             DECIMAL (18, 2) NOT NULL,
    [CostoUnitario]        DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([OrdenCompraDetalleId] ASC),
    CONSTRAINT [CK_OCD_Cantidad] CHECK ([Cantidad]>(0)),
    CONSTRAINT [FK_OCD_OC] FOREIGN KEY ([OrdenCompraId]) REFERENCES [dbo].[OrdenCompra] ([OrdenCompraId]),
    CONSTRAINT [FK_OCD_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);


GO
CREATE NONCLUSTERED INDEX [IX_OCD_OC]
    ON [dbo].[OrdenCompraDetalle]([OrdenCompraId] ASC);


GO
CREATE   TRIGGER dbo.tr_AUD_OrdenCompraDetalle
ON dbo.OrdenCompraDetalle
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.OrdenCompraDetalle', @accion, @cnt);
END