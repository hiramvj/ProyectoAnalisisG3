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
