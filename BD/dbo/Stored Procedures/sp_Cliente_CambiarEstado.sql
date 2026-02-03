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
