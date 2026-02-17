CREATE TABLE [dbo].[ReservaInventario] (
    [ReservaId]     BIGINT          IDENTITY (1, 1) NOT NULL,
    [BodegaId]      INT             NOT NULL,
    [ProductoId]    INT             NOT NULL,
    [FuenteTipo]    NVARCHAR (20)   NOT NULL,
    [FuenteId]      INT             NOT NULL,
    [Cantidad]      DECIMAL (18, 2) NOT NULL,
    [Estado]        NVARCHAR (20)   DEFAULT ('ACTIVA') NOT NULL,
    [FechaCreacion] DATETIME2 (7)   DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ReservaId] ASC),
    CONSTRAINT [CK_Res_Cantidad] CHECK ([Cantidad]>(0)),
    CONSTRAINT [CK_Res_Estado] CHECK ([Estado]='CONSUMIDA' OR [Estado]='LIBERADA' OR [Estado]='ACTIVA'),
    CONSTRAINT [CK_Res_Tipo] CHECK ([FuenteTipo]='PEDIDO' OR [FuenteTipo]='COTIZACION'),
    CONSTRAINT [FK_Res_Bodega] FOREIGN KEY ([BodegaId]) REFERENCES [dbo].[Bodega] ([BodegaId]),
    CONSTRAINT [FK_Res_Prod] FOREIGN KEY ([ProductoId]) REFERENCES [dbo].[Producto] ([ProductoId])
);

