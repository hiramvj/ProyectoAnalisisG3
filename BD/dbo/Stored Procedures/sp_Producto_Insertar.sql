CREATE   PROCEDURE dbo.sp_Producto_Insertar
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

    INSERT INTO dbo.Producto
    (SKU, Nombre, CategoriaProductoId, UnidadMedidaId, Costo, Precio, StockMinimo, Activo, FechaCreacion)
    VALUES
    (@SKU, @Nombre, @CategoriaProductoId, @UnidadMedidaId, @Costo, @Precio, @StockMinimo, 1, SYSDATETIME());

    SELECT SCOPE_IDENTITY() AS ProductoId;
END