CREATE TABLE [dbo].[DevolucionVenta] (
    [DevolucionId]  INT            IDENTITY (1, 1) NOT NULL,
    [PedidoVentaId] INT            NOT NULL,
    [FacturaId]     INT            NULL,
    [UsuarioId]     INT            NULL,
    [Fecha]         DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [Motivo]        NVARCHAR (200) NOT NULL,
    [Estado]        NVARCHAR (20)  DEFAULT ('REGISTRADA') NOT NULL,
    PRIMARY KEY CLUSTERED ([DevolucionId] ASC),
    CONSTRAINT [CK_Dev_Estado] CHECK ([Estado]='ANULADA' OR [Estado]='APROBADA' OR [Estado]='REGISTRADA'),
    CONSTRAINT [FK_Dev_Fac] FOREIGN KEY ([FacturaId]) REFERENCES [dbo].[Factura] ([FacturaId]),
    CONSTRAINT [FK_Dev_PV] FOREIGN KEY ([PedidoVentaId]) REFERENCES [dbo].[PedidoVenta] ([PedidoVentaId]),
    CONSTRAINT [FK_Dev_Usr] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId])
);

