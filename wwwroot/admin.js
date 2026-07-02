const demoProducts = [
    { id: 1, codigo: "CAD-116-SHI", nombre: "Cadena Shimano 116 eslabones", stock: 42, almacenId: 1, marcaId: 1, tipoProductoId: 1, almacen: "Deposito Barracas", marca: "Shimano", tipoProducto: "Transmision" },
    { id: 2, codigo: "PUN-ERG-NEG", nombre: "Punos ergonomicos negros", stock: 96, almacenId: 1, marcaId: 2, tipoProductoId: 4, almacen: "Deposito Barracas", marca: "Zamponi", tipoProducto: "Accesorios" },
    { id: 3, codigo: "CAR-750-TRA", nombre: "Caramanola transparente 750 ml", stock: 130, almacenId: 1, marcaId: 2, tipoProductoId: 4, almacen: "Deposito Barracas", marca: "Zamponi", tipoProducto: "Accesorios" },
    { id: 4, codigo: "BOT-ALU-LAT", nombre: "Porta botella aluminio lateral", stock: 54, almacenId: 2, marcaId: 2, tipoProductoId: 4, almacen: "Deposito Reparto", marca: "Zamponi", tipoProducto: "Accesorios" },
    { id: 5, codigo: "CAM-26-VAL", nombre: "Camara 26 valvula auto", stock: 8, almacenId: 2, marcaId: 2, tipoProductoId: 3, almacen: "Deposito Reparto", marca: "Zamponi", tipoProducto: "Ruedas" },
    { id: 6, codigo: "PAS-DIS-ORG", nombre: "Pastillas de freno organicas", stock: 6, almacenId: 2, marcaId: 4, tipoProductoId: 2, almacen: "Deposito Reparto", marca: "Promax", tipoProducto: "Frenos" }
];

const demoLookups = {
    almacenes: [{ id: 1, nombre: "Deposito Barracas" }, { id: 2, nombre: "Deposito Reparto" }],
    marcas: [{ id: 1, name: "Shimano" }, { id: 2, name: "Zamponi" }, { id: 3, name: "Kenda" }, { id: 4, name: "Promax" }],
    tipos: [{ id: 1, nombre: "Transmision" }, { id: 2, nombre: "Frenos" }, { id: 3, nombre: "Ruedas" }, { id: 4, nombre: "Accesorios" }, { id: 5, nombre: "Seguridad" }]
};

const state = {
    apiOnline: true,
    products: [],
    almacenes: [],
    marcas: [],
    tipos: [],
    activities: JSON.parse(localStorage.getItem("zamponiActivities") || "[]")
};

const $ = (selector) => document.querySelector(selector);
const $$ = (selector) => [...document.querySelectorAll(selector)];

async function api(path, options = {}) {
    const response = await fetch(path, {
        headers: { "Content-Type": "application/json" },
        ...options
    });

    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `Error ${response.status}`);
    }

    const contentType = response.headers.get("content-type") || "";
    return contentType.includes("application/json") ? response.json() : response.text();
}

async function loadData() {
    try {
        const [products, almacenes, marcas, tipos] = await Promise.all([
            api("/api/productos"),
            api("/api/almacenes"),
            api("/api/marcas"),
            api("/api/tipoproductos")
        ]);

        state.apiOnline = true;
        state.products = normalizeProducts(products);
        state.almacenes = almacenes;
        state.marcas = marcas;
        state.tipos = tipos;
    } catch {
        state.apiOnline = false;
        state.products = [...demoProducts];
        state.almacenes = [...demoLookups.almacenes];
        state.marcas = [...demoLookups.marcas];
        state.tipos = [...demoLookups.tipos];
        addActivity("Modo demo", "La API no esta disponible. El panel muestra datos de ejemplo.");
    }

    renderAll();
}

function normalizeProducts(products) {
    return products.map((product) => ({
        ...product,
        codigo: product.codigo ?? product.Codigo,
        nombre: product.nombre ?? product.Nombre,
        stock: product.stock ?? product.Stock,
        almacen: product.almacen ?? product.Almacen,
        marca: product.marca ?? product.Marca,
        tipoProducto: product.tipoProducto ?? product.TipoProducto,
        almacenId: product.almacenId ?? product.AlmacenId,
        marcaId: product.marcaId ?? product.MarcaId,
        tipoProductoId: product.tipoProductoId ?? product.TipoProductoId
    }));
}

function renderAll() {
    fillSelects();
    renderDashboard();
    renderProducts();
    renderActivities();
}

function fillSelects() {
    $$("select[name='almacenId']").forEach((select) => {
        select.innerHTML = state.almacenes.map((item) => `<option value="${item.id}">${item.nombre}</option>`).join("");
    });

    $$("select[name='marcaId']").forEach((select) => {
        select.innerHTML = state.marcas.map((item) => `<option value="${item.id}">${item.name}</option>`).join("");
    });

    $$("select[name='tipoProductoId']").forEach((select) => {
        select.innerHTML = state.tipos.map((item) => `<option value="${item.id}">${item.nombre}</option>`).join("");
    });

    $("#categoryFilter").innerHTML = `<option value="">Todas las categorias</option>${state.tipos.map((item) => `<option value="${item.nombre}">${item.nombre}</option>`).join("")}`;
    $("#warehouseFilter").innerHTML = `<option value="">Todos los depositos</option>${state.almacenes.map((item) => `<option value="${item.nombre}">${item.nombre}</option>`).join("")}`;
}

function renderDashboard() {
    const totalUnits = state.products.reduce((sum, product) => sum + Number(product.stock || 0), 0);
    const lowStock = state.products.filter((product) => Number(product.stock || 0) <= 10).length;
    const warehouses = new Set(state.products.map((product) => product.almacen)).size || state.almacenes.length;

    $("#totalProducts").textContent = state.products.length;
    $("#totalUnits").textContent = totalUnits;
    $("#lowStock").textContent = lowStock;
    $("#warehouses").textContent = warehouses;
}

function renderProducts() {
    const category = $("#categoryFilter").value;
    const warehouse = $("#warehouseFilter").value;
    const filtered = state.products.filter((product) => {
        const categoryOk = !category || product.tipoProducto === category;
        const warehouseOk = !warehouse || product.almacen === warehouse;
        return categoryOk && warehouseOk;
    });

    $("#productsTable").innerHTML = filtered.map((product) => `
        <tr>
            <td><code>${product.codigo}</code></td>
            <td>${product.nombre}</td>
            <td>${product.tipoProducto}</td>
            <td>${product.marca}</td>
            <td>${product.almacen}</td>
            <td><span class="stock-badge ${Number(product.stock) <= 10 ? "low" : ""}">${product.stock}</span></td>
        </tr>
    `).join("");
}

function renderActivities() {
    const activities = state.activities.slice(0, 8);
    $("#activityList").innerHTML = activities.length
        ? activities.map((item) => `<div class="activity-item"><strong>${item.title}</strong>${item.detail}</div>`).join("")
        : `<div class="activity-item"><strong>Sin actividad todavia</strong>Los movimientos y altas van a aparecer aca.</div>`;
}

function addActivity(title, detail) {
    state.activities.unshift({ title, detail, at: new Date().toISOString() });
    state.activities = state.activities.slice(0, 20);
    localStorage.setItem("zamponiActivities", JSON.stringify(state.activities));
}

function renderSearchResult(results) {
    if (!results.length) {
        $("#searchResult").innerHTML = `<p>No encontre productos con ese codigo o texto.</p>`;
        return;
    }

    $("#searchResult").innerHTML = results.slice(0, 5).map((product) => `
        <div class="activity-item">
            <strong><code>${product.codigo}</code> · ${product.nombre}</strong>
            ${product.tipoProducto} · ${product.marca} · ${product.almacen} · Stock: ${product.stock}
        </div>
    `).join("");
}

$("#searchForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const text = $("#searchInput").value.trim().toUpperCase();
    if (!text) return;

    try {
        const results = state.apiOnline
            ? normalizeProducts(await api(`/api/productos/buscar?texto=${encodeURIComponent(text)}`))
            : state.products.filter((product) =>
                product.codigo.toUpperCase().includes(text) ||
                product.nombre.toUpperCase().includes(text) ||
                product.tipoProducto.toUpperCase().includes(text));
        renderSearchResult(results);
    } catch (error) {
        $("#searchResult").innerHTML = `<p>${error.message}</p>`;
    }
});

$("#productForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = Object.fromEntries(new FormData(event.currentTarget));
    const payload = {
        codigo: data.codigo.trim().toUpperCase(),
        nombre: data.nombre.trim(),
        stock: Number(data.stock),
        almacenId: Number(data.almacenId),
        marcaId: Number(data.marcaId),
        tipoProductoId: Number(data.tipoProductoId)
    };

    try {
        if (state.apiOnline) {
            await api("/api/productos", { method: "POST", body: JSON.stringify(payload) });
            await loadData();
        } else {
            const almacen = state.almacenes.find((item) => item.id === payload.almacenId);
            const marca = state.marcas.find((item) => item.id === payload.marcaId);
            const tipo = state.tipos.find((item) => item.id === payload.tipoProductoId);
            state.products.unshift({ id: Date.now(), ...payload, almacen: almacen.nombre, marca: marca.name, tipoProducto: tipo.nombre });
            renderAll();
        }

        addActivity("Producto cargado", `${payload.codigo} · ${payload.nombre}`);
        event.currentTarget.reset();
        renderActivities();
    } catch (error) {
        alert(error.message);
    }
});

$("#movementForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = Object.fromEntries(new FormData(event.currentTarget));
    const payload = {
        codigoProducto: data.codigoProducto.trim().toUpperCase(),
        almacenId: Number(data.almacenId),
        cantidad: Number(data.cantidad),
        referencia: data.referencia,
        observacion: data.observacion
    };

    try {
        if (state.apiOnline) {
            await api(`/api/movimientos-stock/${data.tipo}`, { method: "POST", body: JSON.stringify(payload) });
            await loadData();
        } else {
            const product = state.products.find((item) => item.codigo.toUpperCase() === payload.codigoProducto && item.almacenId === payload.almacenId);
            if (!product) throw new Error("No se encontro el producto en el deposito indicado.");
            product.stock += data.tipo === "entrada" ? payload.cantidad : -payload.cantidad;
            renderAll();
        }

        addActivity(`Movimiento: ${data.tipo}`, `${payload.codigoProducto} · ${payload.cantidad} unidades`);
        event.currentTarget.reset();
        renderActivities();
    } catch (error) {
        alert(error.message);
    }
});

$("#seedDemoButton").addEventListener("click", async () => {
    try {
        if (state.apiOnline) {
            await api("/api/datos-demo/seed", { method: "POST", body: "{}" });
            await loadData();
        } else {
            state.products = [...demoProducts];
            renderAll();
        }

        addActivity("Datos demo", "Productos mayoristas de ejemplo cargados.");
        renderActivities();
    } catch (error) {
        alert(error.message);
    }
});

$("#categoryFilter").addEventListener("change", renderProducts);
$("#warehouseFilter").addEventListener("change", renderProducts);

loadData();
