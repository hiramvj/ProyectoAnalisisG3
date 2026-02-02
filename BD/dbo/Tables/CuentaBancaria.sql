CREATE TABLE [dbo].[CuentaBancaria] (
    [CuentaBancariaId] INT           IDENTITY (1, 1) NOT NULL,
    [NombreBanco]      NVARCHAR (80) NOT NULL,
    [NumeroCuenta]     NVARCHAR (40) NOT NULL,
    [Moneda]           NVARCHAR (10) DEFAULT ('CRC') NOT NULL,
    [Activa]           BIT           DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([CuentaBancariaId] ASC),
    UNIQUE NONCLUSTERED ([NumeroCuenta] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_CuentaBancaria
ON dbo.CuentaBancaria
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.CuentaBancaria', @accion, @cnt);
END