CREATE TABLE [dbo].[CategoriaProducto] (
    [CategoriaProductoId] INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]              NVARCHAR (80) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoriaProductoId] ASC),
    UNIQUE NONCLUSTERED ([Nombre] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_CategoriaProducto
ON dbo.CategoriaProducto
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.CategoriaProducto', @accion, @cnt);
END