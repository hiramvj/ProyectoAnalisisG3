CREATE TABLE [dbo].[PedidoVenta] (
    [PedidoVentaId] INT            IDENTITY (1, 1) NOT NULL,
    [NumeroPedido]  NVARCHAR (30)  NOT NULL,
    [ClienteId]     INT            NOT NULL,
    [Estado]        NVARCHAR (20)  DEFAULT ('CREADA') NOT NULL,
    [FechaPedido]   DATE           DEFAULT (CONVERT([date],getdate())) NOT NULL,
    [Observaciones] NVARCHAR (250) NULL,
    [MetodoPagoId]  INT            NULL,
    PRIMARY KEY CLUSTERED ([PedidoVentaId] ASC),
    CONSTRAINT [CK_PV_Estado] CHECK ([Estado]='CANCELADA' OR [Estado]='ENTREGADA' OR [Estado]='CONFIRMADA' OR [Estado]='CREADA'),
    CONSTRAINT [FK_PV_Cliente] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([ClienteId]),
    CONSTRAINT [FK_PV_MetodoPago] FOREIGN KEY ([MetodoPagoId]) REFERENCES [dbo].[MetodoPago] ([MetodoPagoId]),
    UNIQUE NONCLUSTERED ([NumeroPedido] ASC)
);

