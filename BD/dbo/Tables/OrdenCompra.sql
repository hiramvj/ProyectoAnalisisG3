CREATE TABLE [dbo].[OrdenCompra] (
    [OrdenCompraId] INT            IDENTITY (1, 1) NOT NULL,
    [NumeroOrden]   NVARCHAR (30)  NOT NULL,
    [ProveedorId]   INT            NOT NULL,
    [BodegaId]      INT            NOT NULL,
    [Estado]        NVARCHAR (20)  DEFAULT ('CREADA') NOT NULL,
    [FechaOrden]    DATE           DEFAULT (CONVERT([date],getdate())) NOT NULL,
    [FechaEsperada] DATE           NULL,
    [Observaciones] NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([OrdenCompraId] ASC),
    CONSTRAINT [CK_OC_Estado] CHECK ([Estado]='CANCELADA' OR [Estado]='RECIBIDA' OR [Estado]='APROBADA' OR [Estado]='CREADA'),
    CONSTRAINT [FK_OC_Bodega] FOREIGN KEY ([BodegaId]) REFERENCES [dbo].[Bodega] ([BodegaId]),
    CONSTRAINT [FK_OC_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    UNIQUE NONCLUSTERED ([NumeroOrden] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_OrdenCompra
ON dbo.OrdenCompra
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.OrdenCompra', @accion, @cnt);
END