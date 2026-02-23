CREATE   PROCEDURE dbo.sp_Factura_CrearDesdePedido
    @PedidoVentaId INT,
    @FacturaId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1) Validar que exista el pedido
        IF NOT EXISTS (SELECT 1 FROM dbo.PedidoVenta WHERE PedidoVentaId = @PedidoVentaId)
        BEGIN
            THROW 50001, 'El PedidoVentaId no existe.', 1;
        END

        -- 2) Validar que tenga detalle
        IF NOT EXISTS (SELECT 1 FROM dbo.PedidoVentaDetalle WHERE PedidoVentaId = @PedidoVentaId)
        BEGIN
            THROW 50002, 'El pedido no tiene productos en el detalle.', 1;
        END

        -- 3) Generar correlativo de factura
        DECLARE @NumeroFactura INT =
            ISNULL((SELECT MAX(NumeroFactura) FROM dbo.Factura), 0) + 1;

        -- 4) Calcular totales desde el detalle del pedido
        DECLARE @Subtotal DECIMAL(18,2) =
        (
            SELECT ISNULL(SUM(Cantidad * PrecioUnitario), 0)
            FROM dbo.PedidoVentaDetalle
            WHERE PedidoVentaId = @PedidoVentaId
        );

        DECLARE @Impuesto DECIMAL(18,2) = ROUND(@Subtotal * 0.13, 2);
        DECLARE @Total DECIMAL(18,2) = @Subtotal + @Impuesto;

        -- 5) Insertar encabezado de factura
        INSERT INTO dbo.Factura
            (NumeroFactura, PedidoVentaId, FechaEmision, Subtotal, Impuesto, Total, Estado)
        VALUES
            (@NumeroFactura, @PedidoVentaId, GETDATE(), @Subtotal, @Impuesto, @Total, 'Emitida');

        SET @FacturaId = SCOPE_IDENTITY();

        -- 6) Insertar detalle de factura (NO incluir TotalLinea porque es computed)
        INSERT INTO dbo.FacturaDetalle
            (FacturaId, ProductoId, Cantidad, PrecioUnitario)
        SELECT
            @FacturaId,
            d.ProductoId,
            d.Cantidad,
            d.PrecioUnitario
        FROM dbo.PedidoVentaDetalle d
        WHERE d.PedidoVentaId = @PedidoVentaId;

        -- 7) Validar stock antes de rebajar (evita stock negativo)
        IF EXISTS
        (
            SELECT 1
            FROM dbo.PedidoVentaDetalle d
            INNER JOIN dbo.Producto p ON p.ProductoId = d.ProductoId
            WHERE d.PedidoVentaId = @PedidoVentaId
              AND p.Stock < d.Cantidad
        )
        BEGIN
            THROW 50003, 'Stock insuficiente para uno o más productos.', 1;
        END

        -- 8) Rebajar stock
        UPDATE p
        SET p.Stock = p.Stock - d.Cantidad
        FROM dbo.Producto p
        INNER JOIN dbo.PedidoVentaDetalle d ON d.ProductoId = p.ProductoId
        WHERE d.PedidoVentaId = @PedidoVentaId;

        -- 9) Cambiar estado del pedido (✅ debe ser uno permitido por CK_PV_Estado)
        UPDATE dbo.PedidoVenta
        SET Estado = 'ENTREGADA'
        WHERE PedidoVentaId = @PedidoVentaId;

        COMMIT;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END