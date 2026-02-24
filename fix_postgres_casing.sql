-- In PostgreSQL, unquoted identifiers are folded to lowercase.
-- Entity Framework Core expects Exact Casing (PascalCase).
-- We need to rename all our tables to enforce the quotes so EF Core finds them exactly.

ALTER TABLE IF EXISTS modulo RENAME TO "Modulo";
ALTER TABLE IF EXISTS permiso RENAME TO "Permiso";
ALTER TABLE IF EXISTS rol RENAME TO "Rol";
ALTER TABLE IF EXISTS rolpermiso RENAME TO "RolPermiso";
ALTER TABLE IF EXISTS usuario RENAME TO "Usuario";
ALTER TABLE IF EXISTS usuariorol RENAME TO "UsuarioRol";

ALTER TABLE IF EXISTS auditoria RENAME TO "Auditoria";

ALTER TABLE IF EXISTS unidadmedida RENAME TO "UnidadMedida";
ALTER TABLE IF EXISTS categoriaproducto RENAME TO "CategoriaProducto";
ALTER TABLE IF EXISTS metodoentrega RENAME TO "MetodoEntrega";
ALTER TABLE IF EXISTS metodopago RENAME TO "MetodoPago";
ALTER TABLE IF EXISTS estadopedido RENAME TO "EstadoPedido";

ALTER TABLE IF EXISTS cliente RENAME TO "Cliente";
ALTER TABLE IF EXISTS proveedor RENAME TO "Proveedor";
ALTER TABLE IF EXISTS producto RENAME TO "Producto";

ALTER TABLE IF EXISTS compra RENAME TO "Compra";
ALTER TABLE IF EXISTS compradetalle RENAME TO "CompraDetalle";
ALTER TABLE IF EXISTS pagoncompras RENAME TO "PagoNCompras"; 

ALTER TABLE IF EXISTS pedidoventa RENAME TO "PedidoVenta";
ALTER TABLE IF EXISTS pedidoventadetalle RENAME TO "PedidoVentaDetalle";

ALTER TABLE IF EXISTS rutadistribucion RENAME TO "RutaDistribucion";
ALTER TABLE IF EXISTS vehiculo RENAME TO "Vehiculo";
ALTER TABLE IF EXISTS entrega RENAME TO "Entrega";
ALTER TABLE IF EXISTS entregadetalle RENAME TO "EntregaDetalle";

ALTER TABLE IF EXISTS factura RENAME TO "Factura";
ALTER TABLE IF EXISTS facturadetalle RENAME TO "FacturaDetalle";
ALTER TABLE IF EXISTS pago RENAME TO "Pago";
ALTER TABLE IF EXISTS notacredito RENAME TO "NotaCredito";

ALTER TABLE IF EXISTS empleado RENAME TO "Empleado";
ALTER TABLE IF EXISTS asistentereparto RENAME TO "AsistenteReparto";
ALTER TABLE IF EXISTS comision RENAME TO "Comision";
ALTER TABLE IF EXISTS nomina RENAME TO "Nomina";
