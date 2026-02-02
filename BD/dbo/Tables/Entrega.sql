CREATE TABLE [dbo].[Entrega] (
    [EntregaId]       INT            IDENTITY (1, 1) NOT NULL,
    [PedidoVentaId]   INT            NOT NULL,
    [Estado]          NVARCHAR (20)  DEFAULT ('PENDIENTE') NOT NULL,
    [Direccion]       NVARCHAR (250) NOT NULL,
    [FechaProgramada] DATE           NULL,
    [FechaEntregada]  DATE           NULL,
    [Transportista]   NVARCHAR (120) NULL,
    [Notas]           NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([EntregaId] ASC),
    CONSTRAINT [CK_Ent_Estado] CHECK ([Estado]='CANCELADA' OR [Estado]='ENTREGADA' OR [Estado]='EN_RUTA' OR [Estado]='PENDIENTE'),
    CONSTRAINT [FK_Ent_PV] FOREIGN KEY ([PedidoVentaId]) REFERENCES [dbo].[PedidoVenta] ([PedidoVentaId]),
    UNIQUE NONCLUSTERED ([PedidoVentaId] ASC)
);


GO
CREATE   TRIGGER dbo.tr_AUD_Entrega
ON dbo.Entrega
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Entrega', @accion, @cnt);
END