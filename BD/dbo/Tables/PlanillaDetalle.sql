CREATE TABLE [dbo].[PlanillaDetalle] (
    [PlanillaDetalleId] INT             IDENTITY (1, 1) NOT NULL,
    [PlanillaId]        INT             NOT NULL,
    [EmpleadoId]        INT             NOT NULL,
    [SalarioBruto]      DECIMAL (18, 2) NOT NULL,
    [Deducciones]       DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [SalarioNeto]       AS              ([SalarioBruto]-[Deducciones]) PERSISTED,
    PRIMARY KEY CLUSTERED ([PlanillaDetalleId] ASC),
    CONSTRAINT [FK_PD_Empleado] FOREIGN KEY ([EmpleadoId]) REFERENCES [dbo].[Empleado] ([EmpleadoId]),
    CONSTRAINT [FK_PD_Planilla] FOREIGN KEY ([PlanillaId]) REFERENCES [dbo].[Planilla] ([PlanillaId])
);


GO
CREATE   TRIGGER dbo.tr_AUD_PlanillaDetalle
ON dbo.PlanillaDetalle
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.PlanillaDetalle', @accion, @cnt);
END