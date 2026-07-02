# Control de Stock Zamponi

MVP de backend y pagina de presentacion para Zamponi, una empresa mayorista de bicicletas, partes y repuestos con operacion de depositos, entrada de mercaderia por camion y reparto diario.

## Que incluye

- API REST con ASP.NET Core.
- Persistencia con Entity Framework Core y SQL Server LocalDB.
- CRUD de productos, marcas, tipos de producto y almacenes.
- Entradas de stock por deposito.
- Salidas de stock para reparto, ventas o preparacion de pedidos.
- Transferencias entre depositos.
- Reportes de resumen y productos con stock bajo.
- Pagina web inicial servida desde `wwwroot`.
- Panel administrador en `wwwroot/admin.html`.
- Buscador operativo por codigo, nombre, marca o categoria.
- Datos demo para mostrar productos mayoristas sin cargar una base completa.
- Swagger para probar la API.

## Endpoints principales

- `GET /api/productos`
- `POST /api/productos`
- `POST /api/movimientos-stock/entrada`
- `POST /api/movimientos-stock/salida`
- `POST /api/movimientos-stock/transferencia`
- `GET /api/reportes/resumen`
- `GET /api/reportes/stock-bajo?minimo=5`
- `GET /api/productos/buscar?texto=CAD-116`
- `POST /api/datos-demo/seed`

## Ejemplo de entrada de stock

```json
{
  "codigoProducto": "CAD-116",
  "almacenId": 1,
  "cantidad": 20,
  "referencia": "Camion proveedor Shimano",
  "observacion": "Ingreso a deposito principal"
}
```

## Ejemplo de transferencia

```json
{
  "codigoProducto": "CAD-116",
  "almacenOrigenId": 1,
  "almacenDestinoId": 2,
  "cantidad": 6,
  "transporte": "Reparto diario",
  "observacion": "Reposicion para mostrador"
}
```

## Como ejecutar

1. Clonar el repositorio.
2. Revisar el connection string en `appsettings.json`.
3. Crear o actualizar la base con Entity Framework.
4. Ejecutar el proyecto.
5. Abrir `/swagger` para probar la API o `/` para ver la pagina de presentacion.
6. Abrir `/admin.html` para usar el panel administrador.

```bash
dotnet ef database update
dotnet run
```

## Proximos pasos sugeridos

- Login con roles para administracion, deposito y reparto.
- Historial persistente de movimientos con usuario, fecha y comprobante.
- Imagen por producto y carga por codigo de barras.
- Pedidos de clientes mayoristas.
- Dashboard con ventas, faltantes y reposicion sugerida.
