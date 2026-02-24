CREATE PROCEDURE dbo.sp_Cliente_Actualizar
    @ClienteId INT,
    @NombreCompleto NVARCHAR(150),
    @Identificacion NVARCHAR(30) = NULL,
    @Correo NVARCHAR(120) = NULL,
    @Telefono NVARCHAR(20) = NULL,
    @Direccion NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Cliente
    SET NombreCompleto = @NombreCompleto,
        Identificacion = @Identificacion,
        Correo = @Correo,
        Telefono = @Telefono,
        Direccion = @Direccion
    WHERE ClienteId = @ClienteId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
CREATE PROCEDURE dbo.sp_Cliente_CambiarEstado
    @ClienteId INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Cliente
    SET Activo = @Activo
    WHERE ClienteId = @ClienteId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
CREATE PROCEDURE dbo.sp_Cliente_Insertar
    @NombreCompleto NVARCHAR(150),
    @Identificacion NVARCHAR(30) = NULL,
    @Correo NVARCHAR(120) = NULL,
    @Telefono NVARCHAR(20) = NULL,
    @Direccion NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Cliente
    (NombreCompleto, Identificacion, Correo, Telefono, Direccion, Activo, FechaCreacion)
    VALUES
    (@NombreCompleto, @Identificacion, @Correo, @Telefono, @Direccion, 1, SYSDATETIME());

    SELECT SCOPE_IDENTITY() AS ClienteId;
END
CREATE PROCEDURE dbo.sp_Cliente_ListarPorEstado
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ClienteId,
        NombreCompleto,
        Identificacion,
        Correo,
        Telefono,
        Direccion,
        Activo,
        FechaCreacion
    FROM dbo.Cliente
    WHERE Activo = @Activo
    ORDER BY NombreCompleto;
END
CREATE PROCEDURE dbo.sp_Cliente_ObtenerPorId
    @ClienteId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ClienteId,
        NombreCompleto,
        Identificacion,
        Correo,
        Telefono,
        Direccion,
        Activo,
        FechaCreacion
    FROM dbo.Cliente
    WHERE ClienteId = @ClienteId;
END
﻿CREATE   PROCEDURE dbo.sp_Factura_CrearDesdePedido
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
END﻿CREATE   PROCEDURE dbo.sp_PedidoVenta_Crear
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
END﻿CREATE   PROCEDURE dbo.sp_PedidoVentaDetalle_Agregar
    @PedidoVentaId INT,
    @ProductoId INT,
    @Cantidad INT,
    @PrecioUnitario DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.PedidoVentaDetalle(PedidoVentaId, ProductoId, Cantidad, PrecioUnitario)
    VALUES(@PedidoVentaId, @ProductoId, @Cantidad, @PrecioUnitario);
END﻿CREATE PROCEDURE dbo.sp_Producto_Actualizar
    @ProductoId int,
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

    UPDATE dbo.Producto
    SET
        SKU = @SKU,
        Nombre = @Nombre,
        CategoriaProductoId = @CategoriaProductoId,
        UnidadMedidaId = @UnidadMedidaId,
        Costo = @Costo,
        Precio = @Precio,
        Stock = @Stock,
        StockMinimo = @StockMinimo
    WHERE ProductoId = @ProductoId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END﻿CREATE   PROCEDURE dbo.sp_Producto_CambiarEstado
    @ProductoId INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Producto
    SET Activo = @Activo
    WHERE ProductoId = @ProductoId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END﻿CREATE PROCEDURE dbo.sp_Producto_Insertar
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
END﻿CREATE PROCEDURE dbo.sp_Producto_ListarPorEstado
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
END﻿CREATE PROCEDURE dbo.sp_Producto_ObtenerPorId
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