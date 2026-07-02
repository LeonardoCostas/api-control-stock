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
- Login JWT para proteger el panel y las operaciones internas.
- Buscador operativo por codigo, nombre, marca o categoria.
- Edicion de productos con precio mayorista, stock minimo, imagen y estado activo/inactivo.
- Catalogo publico dinamico con busqueda comercial.
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
- `POST /api/auth/login`
- `PUT /api/productos/{id}`
- `PATCH /api/productos/{id}/desactivar`
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

Credenciales demo del panel:

```text
Usuario: admin
Contrasena: zamponi2026
```

```bash
dotnet ef database update
dotnet run
```

Flujo recomendado para probar con backend real:

1. Ejecutar `dotnet ef database update`.
2. Ejecutar `dotnet run`.
3. Entrar a `/admin.html`.
4. Iniciar sesion con las credenciales demo.
5. Usar el boton `Cargar demo` o ejecutar `POST /api/datos-demo/seed`.
6. Crear, editar, buscar por codigo y registrar movimientos.

Para probar endpoints protegidos desde Swagger:

1. Ejecutar `POST /api/auth/login`.
2. Copiar el `token`.
3. Presionar `Authorize` en Swagger.
4. Pegar el token como Bearer.

## Proximos pasos sugeridos

- Login con roles para administracion, deposito y reparto.
- Historial persistente de movimientos con usuario, fecha y comprobante.
- Imagen por producto y carga por codigo de barras.
- Pedidos de clientes mayoristas.
- Consulta directa por WhatsApp desde el catalogo publico.
- Dashboard con ventas, faltantes y reposicion sugerida.
