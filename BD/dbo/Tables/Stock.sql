CREATE TABLE [dbo].[Stock] (
    [BodegaId]   INT             NOT NULL,
    [ProductoId] INT             NOT NULL,
    [Cantidad]   DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([BodegaId] ASC, [ProductoId] ASC),
    CONSTRAINT [CK_Stock_NoNegativo] CHECK ([Cantidad]>=(0)),
    CONSTRAINT [FK_Stock_Bodega] FOREIGN KEY ([BodegaId]) REFERENCES [dbo].[Bodega] ([BodegaId]),
    CONSTRAINT [FK_Stock_Producto] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);


GO
CREATE   TRIGGER dbo.tr_AUD_Stock
ON dbo.Stock
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Stock', @accion, @cnt);
END