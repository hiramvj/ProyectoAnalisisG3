CREATE TABLE [dbo].[ColaDespacho] (
    [ColaId]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [PedidoVentaId] INT            NOT NULL,
    [Prioridad]     NVARCHAR (10)  DEFAULT ('NORMAL') NOT NULL,
    [Estado]        NVARCHAR (20)  DEFAULT ('EN_COLA') NOT NULL,
    [FechaEnCola]   DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [Observaciones] NVARCHAR (250) NULL,
    PRIMARY KEY CLUSTERED ([ColaId] ASC),
    CONSTRAINT [CK_Cola_Estado] CHECK ([Estado]='RETENIDO' OR [Estado]='LISTO' OR [Estado]='EN_CARGA' OR [Estado]='EN_COLA'),
    CONSTRAINT [CK_Cola_Prioridad] CHECK ([Prioridad]='BAJA' OR [Prioridad]='NORMAL' OR [Prioridad]='ALTA'),
    CONSTRAINT [FK_Cola_PV] FOREIGN KEY ([PedidoVentaId]) REFERENCES [dbo].[PedidoVenta] ([PedidoVentaId]),
    UNIQUE NONCLUSTERED ([PedidoVentaId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ColaDespacho_Estado]
    ON [dbo].[ColaDespacho]([Estado] ASC, [FechaEnCola] ASC);

