CREATE PROCEDURE dbo.sp_Producto_ObtenerPorId
    @ProductoId int
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ProductoId,
        SKU,
        Nombre,
        CategoriaProductoId,
        UnidadMedidaId,
        Costo,
        Precio,
        Stock,
        StockMinimo,
        Activo,
        FechaCreacion
    FROM dbo.Producto
    WHERE ProductoId = @ProductoId;
END