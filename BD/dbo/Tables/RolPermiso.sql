CREATE TABLE [dbo].[RolPermiso] (
    [RolId]     INT NOT NULL,
    [PermisoId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([RolId] ASC, [PermisoId] ASC),
    CONSTRAINT [FK_RP_Permiso] FOREIGN KEY ([PermisoId]) REFERENCES [dbo].[Permiso] ([PermisoId]),
    CONSTRAINT [FK_RP_Rol] FOREIGN KEY ([RolId]) REFERENCES [dbo].[Rol] ([RolId])
);

