CREATE TABLE [dbo].[ClienteListaPrecio] (
    [ClienteId]     INT NOT NULL,
    [ListaPrecioId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ClienteId] ASC),
    CONSTRAINT [FK_CLP_Cliente] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Cliente] ([ClienteId]),
    CONSTRAINT [FK_CLP_Lista] FOREIGN KEY ([ListaPrecioId]) REFERENCES [dbo].[ListaPrecio] ([ListaPrecioId])
);

