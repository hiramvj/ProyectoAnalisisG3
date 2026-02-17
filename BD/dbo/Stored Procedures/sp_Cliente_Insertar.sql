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
