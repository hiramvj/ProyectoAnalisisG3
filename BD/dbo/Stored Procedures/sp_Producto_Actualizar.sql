CREATE PROCEDURE dbo.sp_Producto_Actualizar
    @ProductoId int,
    @SKU nvarchar(40),
    @Nombre nvarchar(150),
    @CategoriaProductoId int = NULL,
    @UnidadMedidaId int,
    @Costo decimal(18,2),
    @Precio decimal(18,2),
    @Stock decimal(18,2),
    @StockMinimo decimal(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Producto
    SET
        SKU = @SKU,
        Nombre = @Nombre,
        CategoriaProductoId = @CategoriaProductoId,
        UnidadMedidaId = @UnidadMedidaId,
        Costo = @Costo,
        Precio = @Precio,
        Stock = @Stock,
        StockMinimo = @StockMinimo
    WHERE ProductoId = @ProductoId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END