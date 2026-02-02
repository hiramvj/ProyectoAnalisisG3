CREATE TABLE [dbo].[Usuario] (
    [UsuarioId]      INT            IDENTITY (1, 1) NOT NULL,
    [RolId]          INT            NOT NULL,
    [NombreCompleto] NVARCHAR (120) NOT NULL,
    [Correo]         NVARCHAR (120) NOT NULL,
    [Telefono]       NVARCHAR (20)  NULL,
    [HashPassword]   NVARCHAR (255) NOT NULL,
    [Activo]         BIT            DEFAULT ((1)) NOT NULL,
    [FechaCreacion]  DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([UsuarioId] ASC),
    CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([RolId]),
    UNIQUE NONCLUSTERED ([Correo] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Usuario
ON dbo.Usuario
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @accion NVARCHAR(10) =
        CASE WHEN EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted) THEN 'UPDATE'
             WHEN EXISTS(SELECT 1 FROM inserted) THEN 'INSERT'
             ELSE 'DELETE' END;

    DECLARE @cnt INT =
        CASE WHEN @accion IN ('INSERT','UPDATE') THEN (SELECT COUNT(*) FROM inserted)
             ELSE (SELECT COUNT(*) FROM deleted) END;

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas)
    VALUES (N'dbo.Usuario', @accion, @cnt);
END