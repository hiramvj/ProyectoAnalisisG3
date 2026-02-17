CREATE   PROCEDURE dbo.sp_Producto_CambiarEstado
    @ProductoId INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Producto
    SET Activo = @Activo
    WHERE ProductoId = @ProductoId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END