CREATE PROCEDURE dbo.sp_Producto_ListarPorEstado
    @Activo bit
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
    WHERE Activo = @Activo;
END