CREATE TABLE [dbo].[NotaAjuste] (
    [NotaId]       INT             IDENTITY (1, 1) NOT NULL,
    [Tipo]         NVARCHAR (10)   NOT NULL,
    [NumeroNota]   NVARCHAR (30)   NOT NULL,
    [FacturaId]    INT             NOT NULL,
    [DevolucionId] INT             NULL,
    [UsuarioId]    INT             NULL,
    [FechaEmision] DATE            DEFAULT (CONVERT([date],getdate())) NOT NULL,
    [Motivo]       NVARCHAR (200)  NOT NULL,
    [Subtotal]     DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Impuesto]     DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Total]        DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Estado]       NVARCHAR (20)   DEFAULT ('EMITIDA') NOT NULL,
    PRIMARY KEY CLUSTERED ([NotaId] ASC),
    CONSTRAINT [CK_Nota_Estado] CHECK ([Estado]='ANULADA' OR [Estado]='EMITIDA'),
    CONSTRAINT [CK_Nota_Tipo] CHECK ([Tipo]='DEBITO' OR [Tipo]='CREDITO'),
    CONSTRAINT [FK_Nota_Dev] FOREIGN KEY ([DevolucionId]) REFERENCES [dbo].[DevolucionVenta] ([DevolucionId]),
    CONSTRAINT [FK_Nota_Fac] FOREIGN KEY ([FacturaId]) REFERENCES [dbo].[Factura] ([FacturaId]),
    CONSTRAINT [FK_Nota_Usr] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId]),
    UNIQUE NONCLUSTERED ([NumeroNota] ASC)
);

