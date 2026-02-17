CREATE PROCEDURE dbo.sp_Producto_Insertar
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

    INSERT INTO dbo.Producto
    (
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
    )
    VALUES
    (
        @SKU,
        @Nombre,
        @CategoriaProductoId,
        @UnidadMedidaId,
        @Costo,
        @Precio,
        @Stock,
        @StockMinimo,
        1,
        GETDATE()
    );

    SELECT CAST(SCOPE_IDENTITY() AS int) AS ProductoId;
END