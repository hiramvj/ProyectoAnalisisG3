CREATE TABLE [dbo].[Factura] (
    [FacturaId]        INT             IDENTITY (1, 1) NOT NULL,
    [NumeroFactura]    NVARCHAR (30)   NOT NULL,
    [PedidoVentaId]    INT             NOT NULL,
    [FechaEmision]     DATE            DEFAULT (CONVERT([date],getdate())) NOT NULL,
    [Subtotal]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Impuesto]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Total]            DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Estado]           NVARCHAR (20)   DEFAULT ('EMITIDA') NOT NULL,
    [FechaVencimiento] DATE            NULL,
    PRIMARY KEY CLUSTERED ([FacturaId] ASC),
    CONSTRAINT [CK_Fac_Estado] CHECK ([Estado]='ANULADA' OR [Estado]='EMITIDA'),
    CONSTRAINT [FK_Fac_PV] FOREIGN KEY ([PedidoVentaId]) REFERENCES [dbo].[PedidoVenta] ([PedidoVentaId]),
    UNIQUE NONCLUSTERED ([NumeroFactura] ASC),
    UNIQUE NONCLUSTERED ([PedidoVentaId] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Factura
ON dbo.Factura
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Factura', @accion, @cnt);
END