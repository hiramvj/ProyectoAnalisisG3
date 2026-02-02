CREATE TABLE [dbo].[Planilla] (
    [PlanillaId]    INT           IDENTITY (1, 1) NOT NULL,
    [PeriodoInicio] DATE          NOT NULL,
    [PeriodoFin]    DATE          NOT NULL,
    [FechaPago]     DATE          NULL,
    [Estado]        NVARCHAR (20) DEFAULT ('GENERADA') NOT NULL,
    PRIMARY KEY CLUSTERED ([PlanillaId] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Planilla
ON dbo.Planilla
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Planilla', @accion, @cnt);
END