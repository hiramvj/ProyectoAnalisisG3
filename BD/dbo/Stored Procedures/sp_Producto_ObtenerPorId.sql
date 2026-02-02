CREATE   PROCEDURE dbo.sp_Producto_ObtenerPorId
    @ProductoId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ProductoId, SKU, Nombre, CategoriaProductoId, UnidadMedidaId,
        Costo, Precio, StockMinimo, Activo, FechaCreacion
    FROM dbo.Producto
    WHERE ProductoId = @ProductoId;
END