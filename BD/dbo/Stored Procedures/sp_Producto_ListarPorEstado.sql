CREATE   PROCEDURE dbo.sp_Producto_ListarPorEstado
    @Activo BIT
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
        StockMinimo,
        Activo,
        FechaCreacion
    FROM dbo.Producto
    WHERE Activo = @Activo
    ORDER BY Nombre;
END