CREATE TABLE [dbo].[PagoProveedor] (
    [PagoProveedorId]    INT             IDENTITY (1, 1) NOT NULL,
    [FacturaProveedorId] INT             NOT NULL,
    [MetodoPagoId]       INT             NOT NULL,
    [CuentaBancariaId]   INT             NULL,
    [Monto]              DECIMAL (18, 2) NOT NULL,
    [FechaPago]          DATETIME2 (7)   DEFAULT (sysdatetime()) NOT NULL,
    [Referencia]         NVARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([PagoProveedorId] ASC),
    CONSTRAINT [CK_PP_Monto] CHECK ([Monto]>(0)),
    CONSTRAINT [FK_PP_Cuenta] FOREIGN KEY ([CuentaBancariaId]) REFERENCES [dbo].[CuentaBancaria] ([CuentaBancariaId]),
    CONSTRAINT [FK_PP_FacProv] FOREIGN KEY ([FacturaProveedorId]) REFERENCES [dbo].[FacturaProveedor] ([FacturaProveedorId]),
    CONSTRAINT [FK_PP_Metodo] FOREIGN KEY ([MetodoPagoId]) REFERENCES [dbo].[MetodoPago] ([MetodoPagoId])
);

