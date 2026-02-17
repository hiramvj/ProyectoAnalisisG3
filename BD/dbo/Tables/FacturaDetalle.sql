CREATE TABLE [dbo].[FacturaDetalle] (
    [FacturaDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [FacturaId]        INT             NOT NULL,
    [ProductoId]       INT             NOT NULL,
    [Cantidad]         DECIMAL (18, 2) NOT NULL,
    [PrecioUnitario]   DECIMAL (18, 2) NOT NULL,
    [TotalLinea]       AS              ([Cantidad]*[PrecioUnitario]) PERSISTED,
    PRIMARY KEY CLUSTERED ([FacturaDetalleId] ASC),
    CONSTRAINT [FK_FD_Fac] FOREIGN KEY ([FacturaId]) REFERENCES [dbo].[Factura] ([FacturaId]),
    CONSTRAINT [FK_FD_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);


GO
CREATE NONCLUSTERED INDEX [IX_FD_Fac]
    ON [dbo].[FacturaDetalle]([FacturaId] ASC);


GO
CREATE   TRIGGER dbo.tr_AUD_FacturaDetalle
ON dbo.FacturaDetalle
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.FacturaDetalle', @accion, @cnt);
END