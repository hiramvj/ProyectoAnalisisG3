CREATE   PROCEDURE dbo.sp_PedidoVentaDetalle_Agregar
    @PedidoVentaId INT,
    @ProductoId INT,
    @Cantidad INT,
    @PrecioUnitario DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PedidoVentaDetalle(PedidoVentaId, ProductoId, Cantidad, PrecioUnitario)
    VALUES(@PedidoVentaId, @ProductoId, @Cantidad, @PrecioUnitario);
END