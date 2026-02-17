CREATE TABLE [dbo].[Contrato] (
    [ContratoId]  INT             IDENTITY (1, 1) NOT NULL,
    [EmpleadoId]  INT             NOT NULL,
    [Tipo]        NVARCHAR (40)   NOT NULL,
    [SalarioBase] DECIMAL (18, 2) NOT NULL,
    [FechaInicio] DATE            NOT NULL,
    [FechaFin]    DATE            NULL,
    PRIMARY KEY CLUSTERED ([ContratoId] ASC),
    CONSTRAINT [FK_Contrato_Empleado] FOREIGN KEY ([EmpleadoId]) REFERENCES [dbo].[Empleado] ([EmpleadoId])
);


GO
CREATE   TRIGGER dbo.tr_AUD_Contrato
ON dbo.Contrato
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Contrato', @accion, @cnt);
END