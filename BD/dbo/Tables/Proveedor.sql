CREATE TABLE [dbo].[Proveedor] (
    [ProveedorId]    INT            IDENTITY (1, 1) NOT NULL,
    [NombreLegal]    NVARCHAR (150) NOT NULL,
    [CedulaJuridica] NVARCHAR (30)  NULL,
    [Correo]         NVARCHAR (120) NULL,
    [Telefono]       NVARCHAR (20)  NULL,
    [Direccion]      NVARCHAR (250) NULL,
    [Activo]         BIT            DEFAULT ((1)) NOT NULL,
    [FechaCreacion]  DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ProveedorId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Proveedor_Nombre]
    ON [dbo].[Proveedor]([NombreLegal] ASC);


GO
CREATE   TRIGGER dbo.tr_AUD_Proveedor
ON dbo.Proveedor
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Proveedor', @accion, @cnt);
END