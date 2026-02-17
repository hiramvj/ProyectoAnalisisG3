CREATE TABLE [dbo].[Cotizacion] (
    [CotizacionId]     INT             IDENTITY (1, 1) NOT NULL,
    [NumeroCotizacion] NVARCHAR (30)   NOT NULL,
    [ClienteId]        INT             NOT NULL,
    [UsuarioId]        INT             NULL,
    [ListaPrecioId]    INT             NULL,
    [Estado]           NVARCHAR (20)   DEFAULT ('BORRADOR') NOT NULL,
    [FechaEmision]     DATE            DEFAULT (CONVERT([date],getdate())) NOT NULL,
    [FechaValidez]     DATE            NULL,
    [Subtotal]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Impuesto]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Total]            DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Observaciones]    NVARCHAR (250)  NULL,
    PRIMARY KEY CLUSTERED ([CotizacionId] ASC),
    CONSTRAINT [CK_Cot_Estado] CHECK ([Estado]='ANULADA' OR [Estado]='EXPIRADA' OR [Estado]='ACEPTADA' OR [Estado]='ENVIADA' OR [Estado]='BORRADOR'),
    CONSTRAINT [FK_Cot_Cliente] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([ClienteId]),
    CONSTRAINT [FK_Cot_Lista] FOREIGN KEY ([ListaPrecioId]) REFERENCES [dbo].[ListaPrecio] ([ListaPrecioId]),
    CONSTRAINT [FK_Cot_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId]),
    UNIQUE NONCLUSTERED ([NumeroCotizacion] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Cotizacion_Estado]
    ON [dbo].[Cotizacion]([Estado] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Cotizacion_Cliente_Fecha]
    ON [dbo].[Cotizacion]([ClienteId] ASC, [FechaEmision] DESC);

