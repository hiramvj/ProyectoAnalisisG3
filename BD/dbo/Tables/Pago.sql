CREATE TABLE [dbo].[Pago] (
    [PagoId]           INT             IDENTITY (1, 1) NOT NULL,
    [FacturaId]        INT             NOT NULL,
    [MetodoPagoId]     INT             NOT NULL,
    [CuentaBancariaId] INT             NULL,
    [Monto]            DECIMAL (18, 2) NOT NULL,
    [FechaPago]        DATETIME2 (7)   DEFAULT (sysdatetime()) NOT NULL,
    [Referencia]       NVARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([PagoId] ASC),
    CONSTRAINT [FK_Pago_Cuenta] FOREIGN KEY ([CuentaBancariaId]) REFERENCES [dbo].[CuentaBancaria] ([CuentaBancariaId]),
    CONSTRAINT [FK_Pago_Factura] FOREIGN KEY ([FacturaId]) REFERENCES [dbo].[Factura] ([FacturaId]),
    CONSTRAINT [FK_Pago_Metodo] FOREIGN KEY ([MetodoPagoId]) REFERENCES [dbo].[MetodoPago] ([MetodoPagoId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Pago_Factura]
    ON [dbo].[Pago]([FacturaId] ASC, [FechaPago] DESC);


GO
CREATE   TRIGGER dbo.tr_AUD_Pago
ON dbo.Pago
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Pago', @accion, @cnt);
END