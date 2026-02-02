CREATE TABLE [dbo].[Asistencia] (
    [AsistenciaId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [EmpleadoId]   INT      NOT NULL,
    [Fecha]        DATE     NOT NULL,
    [HoraEntrada]  TIME (7) NULL,
    [HoraSalida]   TIME (7) NULL,
    PRIMARY KEY CLUSTERED ([AsistenciaId] ASC),
    CONSTRAINT [FK_Asistencia_Empleado] FOREIGN KEY ([EmpleadoId]) REFERENCES [dbo].[Empleado] ([EmpleadoId]),
    CONSTRAINT [UQ_Asistencia] UNIQUE NONCLUSTERED ([EmpleadoId] ASC, [Fecha] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Asistencia
ON dbo.Asistencia
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Asistencia', @accion, @cnt);
END