CREATE TABLE [dbo].[Permiso] (
    [PermisoId]   INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]      NVARCHAR (80)  NOT NULL,
    [Descripcion] NVARCHAR (200) NULL,
    [Activo]      BIT            DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([PermisoId] ASC),
    UNIQUE NONCLUSTERED ([Codigo] ASC)
);

