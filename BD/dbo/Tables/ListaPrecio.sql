CREATE TABLE [dbo].[ListaPrecio] (
    [ListaPrecioId] INT           IDENTITY (1, 1) NOT NULL,
    [Nombre]        NVARCHAR (80) NOT NULL,
    [Activa]        BIT           DEFAULT ((1)) NOT NULL,
    [FechaCreacion] DATETIME2 (7) DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([ListaPrecioId] ASC),
    UNIQUE NONCLUSTERED ([Nombre] ASC)
);

