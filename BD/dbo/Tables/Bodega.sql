CREATE TABLE [dbo].[Bodega] (
    [BodegaId]  INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]    NVARCHAR (80)  NOT NULL,
    [Direccion] NVARCHAR (250) NULL,
    [Activo]    BIT            DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([BodegaId] ASC),
    UNIQUE NONCLUSTERED ([Nombre] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Bodega
ON dbo.Bodega
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Bodega', @accion, @cnt);
END