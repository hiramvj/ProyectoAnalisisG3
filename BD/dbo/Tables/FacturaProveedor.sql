CREATE TABLE [dbo].[FacturaProveedor] (
    [FacturaProveedorId] INT             IDENTITY (1, 1) NOT NULL,
    [ProveedorId]        INT             NOT NULL,
    [OrdenCompraId]      INT             NULL,
    [NumeroDocumento]    NVARCHAR (40)   NOT NULL,
    [FechaEmision]       DATE            NOT NULL,
    [FechaVencimiento]   DATE            NULL,
    [Subtotal]           DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Impuesto]           DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Total]              DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Estado]             NVARCHAR (20)   DEFAULT ('PENDIENTE') NOT NULL,
    [Observaciones]      NVARCHAR (250)  NULL,
    PRIMARY KEY CLUSTERED ([FacturaProveedorId] ASC),
    CONSTRAINT [CK_FacProv_Estado] CHECK ([Estado]='ANULADA' OR [Estado]='PAGADA' OR [Estado]='PARCIAL' OR [Estado]='PENDIENTE'),
    CONSTRAINT [FK_FacProv_OC] FOREIGN KEY ([OrdenCompraId]) REFERENCES [dbo].[OrdenCompra] ([OrdenCompraId]),
    CONSTRAINT [FK_FacProv_Prov] FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Proveedor] ([ProveedorId]),
    CONSTRAINT [UQ_FacProv] UNIQUE NONCLUSTERED ([ProveedorId] ASC, [NumeroDocumento] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_FacProv_Estado_Vence]
    ON [dbo].[FacturaProveedor]([Estado] ASC, [FechaVencimiento] ASC);

