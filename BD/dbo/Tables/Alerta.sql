CREATE TABLE [dbo].[Alerta] (
    [AlertaId]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [Tipo]           NVARCHAR (30)  NOT NULL,
    [ReferenciaTipo] NVARCHAR (30)  NULL,
    [ReferenciaId]   BIGINT         NULL,
    [Mensaje]        NVARCHAR (250) NOT NULL,
    [Nivel]          NVARCHAR (10)  DEFAULT ('INFO') NOT NULL,
    [Estado]         NVARCHAR (15)  DEFAULT ('ACTIVA') NOT NULL,
    [FechaCreacion]  DATETIME2 (7)  DEFAULT (sysdatetime()) NOT NULL,
    [FechaAtendida]  DATETIME2 (7)  NULL,
    [UsuarioId]      INT            NULL,
    PRIMARY KEY CLUSTERED ([AlertaId] ASC),
    CONSTRAINT [CK_Alerta_Estado] CHECK ([Estado]='CERRADA' OR [Estado]='ATENDIDA' OR [Estado]='ACTIVA'),
    CONSTRAINT [CK_Alerta_Nivel] CHECK ([Nivel]='CRIT' OR [Nivel]='WARN' OR [Nivel]='INFO'),
    CONSTRAINT [FK_Alerta_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([UsuarioId])
);

