CREATE TABLE [dbo].[RutaEntrega] (
    [RutaId]          INT            IDENTITY (1, 1) NOT NULL,
    [CodigoRuta]      NVARCHAR (30)  NOT NULL,
    [FechaProgramada] DATE           NULL,
    [Estado]          NVARCHAR (20)  DEFAULT ('PLANIFICADA') NOT NULL,
    [TransportistaId] INT            NULL,
    [VehiculoId]      INT            NULL,
    [Observaciones]   NVARCHAR (250) NULL,
    [FechaCreacion]   DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([RutaId] ASC),
    CONSTRAINT [CK_Ruta_Estado] CHECK ([Estado]='CANCELADA' OR [Estado]='FINALIZADA' OR [Estado]='EN_RUTA' OR [Estado]='PLANIFICADA'),
    CONSTRAINT [FK_Ruta_Trans] FOREIGN KEY ([TransportistaId]) REFERENCES [dbo].[Transportista] ([TransportistaId]),
    CONSTRAINT [FK_Ruta_Veh] FOREIGN KEY ([VehiculoId]) REFERENCES [dbo].[Vehiculo] ([VehiculoId]),
    UNIQUE NONCLUSTERED ([CodigoRuta] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Ruta_Estado_Fecha]
    ON [dbo].[RutaEntrega]([Estado] ASC, [FechaProgramada] ASC);

