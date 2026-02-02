CREATE TABLE [dbo].[Transportista] (
    [TransportistaId] INT            IDENTITY (1, 1) NOT NULL,
    [NombreCompleto]  NVARCHAR (150) NOT NULL,
    [Identificacion]  NVARCHAR (30)  NULL,
    [Telefono]        NVARCHAR (20)  NULL,
    [Activo]          BIT            DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([TransportistaId] ASC)
);

