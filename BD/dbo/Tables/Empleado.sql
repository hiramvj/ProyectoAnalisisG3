CREATE TABLE [dbo].[Empleado] (
    [EmpleadoId]     INT            IDENTITY (1, 1) NOT NULL,
    [NombreCompleto] NVARCHAR (150) NOT NULL,
    [Identificacion] NVARCHAR (30)  NOT NULL,
    [Correo]         NVARCHAR (120) NULL,
    [Telefono]       NVARCHAR (20)  NULL,
    [Puesto]         NVARCHAR (80)  NULL,
    [Activo]         BIT            DEFAULT ((1)) NOT NULL,
    [FechaIngreso]   DATE           NULL,
    PRIMARY KEY CLUSTERED ([EmpleadoId] ASC),
    UNIQUE NONCLUSTERED ([Identificacion] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Empleado
ON dbo.Empleado
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Empleado', @accion, @cnt);
END