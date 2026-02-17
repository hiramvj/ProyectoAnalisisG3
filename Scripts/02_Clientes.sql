USE [DistribuidoraTachiDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Clientes](
        [ClienteId] [int] IDENTITY(1,1) NOT NULL,
        [Identificacion] [nvarchar](30) NULL,
        [NombreCompleto] [nvarchar](150) NOT NULL,
        [Correo] [nvarchar](120) NULL,
        [Telefono] [nvarchar](20) NULL,
        [Direccion] [nvarchar](250) NULL,
        [Activo] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
     CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
    (
        [ClienteId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

-- SP LISTAR
CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_ListarPorEstado]
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Clientes WHERE Activo = @Activo
END
GO

-- SP OBTENER POR ID
CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_ObtenerPorId]
    @ClienteId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Clientes WHERE ClienteId = @ClienteId
END
GO

-- SP INSERTAR
CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_Insertar]
    @Identificacion NVARCHAR(30) = NULL,
    @NombreCompleto NVARCHAR(150),
    @Correo NVARCHAR(120) = NULL,
    @Telefono NVARCHAR(20) = NULL,
    @Direccion NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Clientes (Identificacion, NombreCompleto, Correo, Telefono, Direccion, Activo, FechaCreacion)
    VALUES (@Identificacion, @NombreCompleto, @Correo, @Telefono, @Direccion, 1, GETDATE())

    SELECT CAST(SCOPE_IDENTITY() as int)
END
GO

-- SP ACTUALIZAR
CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_Actualizar]
    @ClienteId INT,
    @Identificacion NVARCHAR(30) = NULL,
    @NombreCompleto NVARCHAR(150),
    @Correo NVARCHAR(120) = NULL,
    @Telefono NVARCHAR(20) = NULL,
    @Direccion NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Clientes
    SET Identificacion = @Identificacion,
        NombreCompleto = @NombreCompleto,
        Correo = @Correo,
        Telefono = @Telefono,
        Direccion = @Direccion
    WHERE ClienteId = @ClienteId

    SELECT 1
END
GO

-- SP CAMBIAR ESTADO
CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_CambiarEstado]
    @ClienteId INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Clientes
    SET Activo = @Activo
    WHERE ClienteId = @ClienteId

    SELECT 1
END
GO
