CREATE TABLE [dbo].[Producto] (
    [ProductoId]          INT             IDENTITY (1, 1) NOT NULL,
    [SKU]                 NVARCHAR (40)   NOT NULL,
    [Nombre]              NVARCHAR (150)  NOT NULL,
    [CategoriaProductoId] INT             NULL,
    [UnidadMedidaId]      INT             NOT NULL,
    [Costo]               DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Precio]              DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [StockMinimo]         DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [Activo]              BIT             DEFAULT ((1)) NOT NULL,
    [FechaCreacion]       DATETIME2 (7)   DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ProductoId] ASC),
    CONSTRAINT [FK_Producto_Categoria] FOREIGN KEY ([CategoriaProductoId]) REFERENCES [dbo].[CategoriaProducto] ([CategoriaProductoId]),
    CONSTRAINT [FK_Producto_Unidad] FOREIGN KEY ([UnidadMedidaId]) REFERENCES [dbo].[UnidadMedida] ([UnidadMedidaId]),
    UNIQUE NONCLUSTERED ([SKU] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Producto_Nombre]
    ON [dbo].[Producto]([Nombre] ASC);


GO
CREATE   TRIGGER dbo.tr_AUD_Producto
ON dbo.Producto
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

    INSERT INTO dbo.Auditoria(Tabla, Accion, CantidadFilas) VALUES (N'dbo.Producto', @accion, @cnt);
END