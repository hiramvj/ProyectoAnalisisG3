CREATE   PROCEDURE dbo.sp_Producto_Actualizar
    @ProductoId INT,
    @SKU NVARCHAR(40),
    @Nombre NVARCHAR(150),
    @CategoriaProductoId INT = NULL,
    @UnidadMedidaId INT,
    @Costo DECIMAL(18,2),
    @Precio DECIMAL(18,2),
    @StockMinimo DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Producto
    SET SKU=@SKU,
        Nombre=@Nombre,
        CategoriaProductoId=@CategoriaProductoId,
        UnidadMedidaId=@UnidadMedidaId,
        Costo=@Costo,
        Precio=@Precio,
        StockMinimo=@StockMinimo
    WHERE ProductoId=@ProductoId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END