CREATE TABLE [dbo].[Cliente] (
    [ClienteId]      INT            IDENTITY (1, 1) NOT NULL,
    [NombreCompleto] NVARCHAR (150) NOT NULL,
    [Identificacion] NVARCHAR (30)  NULL,
    [Correo]         NVARCHAR (120) NULL,
    [Telefono]       NVARCHAR (20)  NULL,
    [Direccion]      NVARCHAR (250) NULL,
    [Activo]         BIT            DEFAULT ((1)) NOT NULL,
    [FechaCreacion]  DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ClienteId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Cliente_Nombre]
    ON [dbo].[Cliente]([NombreCompleto] ASC);


GO
CREATE   TRIGGER dbo.tr_AUD_Cliente
ON dbo.Cliente
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Cliente', @accion, @cnt);
END