CREATE   PROCEDURE dbo.sp_PedidoVenta_Crear
    @ClienteId INT,
    @Observaciones VARCHAR(250) = NULL,
    @MetodoPagoId INT = NULL,
    @PedidoVentaId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NumeroPedido INT = ISNULL((SELECT MAX(NumeroPedido) FROM dbo.PedidoVenta), 0) + 1;

    INSERT INTO dbo.PedidoVenta(NumeroPedido, ClienteId, Estado, FechaPedido, Observaciones, MetodoPagoId)
    VALUES(@NumeroPedido, @ClienteId, 'Borrador', GETDATE(), @Observaciones, @MetodoPagoId);

    SET @PedidoVentaId = SCOPE_IDENTITY();
END