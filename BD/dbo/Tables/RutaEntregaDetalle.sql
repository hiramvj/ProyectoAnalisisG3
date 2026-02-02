CREATE TABLE [dbo].[RutaEntregaDetalle] (
    [RutaDetalleId] INT           IDENTITY (1, 1) NOT NULL,
    [RutaId]        INT           NOT NULL,
    [PedidoVentaId] INT           NOT NULL,
    [OrdenParada]   INT           NOT NULL,
    [EstadoParada]  NVARCHAR (20) DEFAULT ('PENDIENTE') NOT NULL,
    PRIMARY KEY CLUSTERED ([RutaDetalleId] ASC),
    CONSTRAINT [CK_Parada_Estado] CHECK ([EstadoParada]='PARCIAL' OR [EstadoParada]='RECHAZADA' OR [EstadoParada]='ENTREGADA' OR [EstadoParada]='PENDIENTE'),
    CONSTRAINT [CK_RDet_Orden] CHECK ([OrdenParada]>(0)),
    CONSTRAINT [FK_RDet_PV] FOREIGN KEY ([PedidoVentaId]) REFERENCES [dbo].[PedidoVenta] ([PedidoVentaId]),
    CONSTRAINT [FK_RDet_Ruta] FOREIGN KEY ([RutaId]) REFERENCES [dbo].[RutaEntrega] ([RutaId]),
    CONSTRAINT [UQ_Ruta_Pedido] UNIQUE NONCLUSTERED ([RutaId] ASC, [PedidoVentaId] ASC)
);

