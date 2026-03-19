using Abstracciones.Modelos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProyectoTachi.Servicios
{
    public class FacturaPdfDocument : IDocument
    {
        private readonly PedidoVentaDetalleDto _model;
        private readonly string _logoPath;

        public FacturaPdfDocument(PedidoVentaDetalleDto model, string logoPath)
        {
            _model = model;
            _logoPath = logoPath;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);

                page.Header().Element(x => ComposeHeader(x));
                page.Content().Element(x => ComposeContent(x));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("TACHI Distribuidora - Generado el ");
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                });
            });
        }

        private void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.Row(row =>
            {
                row.ConstantItem(120).Height(60).Element(x =>
                {
                    if (File.Exists(_logoPath))
                    {
                        x.Image(File.ReadAllBytes(_logoPath));
                    }
                    else
                    {
                        x.AlignMiddle().Text("TACHI").Bold().FontSize(24);
                    }
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text("FACTURA / PEDIDO").Bold().FontSize(18);
                    col.Item().AlignRight().Text($"Pedido #{_model.NumeroPedido}").SemiBold();
                    col.Item().AlignRight().Text($"Fecha: {_model.FechaPedido:dd/MM/yyyy}");
                });
            });
        }

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(12);

                col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(info =>
                {
                    info.Spacing(4);
                    info.Item().Text("TACHI Distribuidora").Bold().FontSize(14);
                    info.Item().Text("Documento de venta / facturación");
                    info.Item().Text($"Cliente: {_model.ClienteNombre}");
                    info.Item().Text($"Método de pago: {_model.MetodoPagoNombre}");
                    info.Item().Text($"Estado: {_model.Estado}");

                    if (!string.IsNullOrWhiteSpace(_model.Observaciones))
                        info.Item().Text($"Observaciones: {_model.Observaciones}");
                });

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Producto").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Cant.").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Precio").Bold();
                        header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                    });

                    foreach (var linea in _model.Lineas)
                    {
                        table.Cell().Element(CellStyle).Text(linea.ProductoNombre);
                        table.Cell().Element(CellStyle).AlignRight().Text(linea.Cantidad.ToString("N2"));
                        table.Cell().Element(CellStyle).AlignRight().Text($"₡ {linea.PrecioUnitario:N2}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"₡ {linea.TotalLinea:N2}");
                    }
                });

                col.Item().AlignRight().Width(220).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(totals =>
                {
                    totals.Spacing(4);

                    totals.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Subtotal:").SemiBold();
                        r.RelativeItem().AlignRight().Text($"₡ {_model.Subtotal:N2}");
                    });

                    totals.Item().Row(r =>
                    {
                        r.RelativeItem().Text("IVA (13%):").SemiBold();
                        r.RelativeItem().AlignRight().Text($"₡ {_model.IVA:N2}");
                    });

                    totals.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text("Total:").Bold().FontSize(14);
                        r.RelativeItem().AlignRight().Text($"₡ {_model.Total:N2}").Bold().FontSize(14);
                    });
                });
            });
        }

        private static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(6)
                .PaddingHorizontal(4);
        }
    }
}