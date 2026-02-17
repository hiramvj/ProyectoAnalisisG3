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
