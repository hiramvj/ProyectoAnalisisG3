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
