CREATE TABLE [dbo].[Vehiculo] (
    [VehiculoId]       INT             IDENTITY (1, 1) NOT NULL,
    [Placa]            NVARCHAR (15)   NOT NULL,
    [Tipo]             NVARCHAR (40)   NULL,
    [CapacidadPeso]    DECIMAL (18, 2) NULL,
    [CapacidadVolumen] DECIMAL (18, 2) NULL,
    [Activo]           BIT             DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([VehiculoId] ASC),
    UNIQUE NONCLUSTERED ([Placa] ASC)
);

