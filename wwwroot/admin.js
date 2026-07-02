const demoProducts = [
    { id: 1, codigo: "CAD-116-SHI", nombre: "Cadena Shimano 116 eslabones", stock: 42, stockMinimo: 12, precioMayorista: 8200, imagenUrl: "https://images.unsplash.com/photo-1637289031856-9625abbba63f?auto=format&fit=crop&w=600&q=80", activo: true, almacenId: 1, marcaId: 1, tipoProductoId: 1, almacen: "Deposito Barracas", marca: "Shimano", tipoProducto: "Transmision" },
    { id: 2, codigo: "PUN-ERG-NEG", nombre: "Punos ergonomicos negros", stock: 96, stockMinimo: 24, precioMayorista: 2100, imagenUrl: "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80", activo: true, almacenId: 1, marcaId: 2, tipoProductoId: 4, almacen: "Deposito Barracas", marca: "Zamponi", tipoProducto: "Accesorios" },
    { id: 3, codigo: "CAR-750-TRA", nombre: "Caramanola transparente 750 ml", stock: 130, stockMinimo: 30, precioMayorista: 1800, imagenUrl: "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80", activo: true, almacenId: 1, marcaId: 2, tipoProductoId: 4, almacen: "Deposito Barracas", marca: "Zamponi", tipoProducto: "Accesorios" },
    { id: 4, codigo: "BOT-ALU-LAT", nombre: "Porta botella aluminio lateral", stock: 54, stockMinimo: 20, precioMayorista: 2600, imagenUrl: "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80", activo: true, almacenId: 2, marcaId: 2, tipoProductoId: 4, almacen: "Deposito Reparto", marca: "Zamponi", tipoProducto: "Accesorios" },
    { id: 5, codigo: "CAM-26-VAL", nombre: "Camara 26 valvula auto", stock: 8, stockMinimo: 15, precioMayorista: 2300, imagenUrl: "https://images.unsplash.com/photo-1529422643029-d4585747aaf2?auto=format&fit=crop&w=600&q=80", activo: true, almacenId: 2, marcaId: 2, tipoProductoId: 3, almacen: "Deposito Reparto", marca: "Zamponi", tipoProducto: "Ruedas" },
    { id: 6, codigo: "PAS-DIS-ORG", nombre: "Pastillas de freno organicas", stock: 6, stockMinimo: 10, precioMayorista: 3100, imagenUrl: "https://images.unsplash.com/photo-1507035895480-2b3156c31fc8?auto=format&fit=crop&w=600&q=80", activo: true, almacenId: 2, marcaId: 4, tipoProductoId: 2, almacen: "Deposito Reparto", marca: "Promax", tipoProducto: "Frenos" }
];

const demoLookups = {
    almacenes: [{ id: 1, nombre: "Deposito Barracas" }, { id: 2, nombre: "Deposito Reparto" }],
    marcas: [{ id: 1, name: "Shimano" }, { id: 2, name: "Zamponi" }, { id: 3, name: "Kenda" }, { id: 4, name: "Promax" }],
    tipos: [{ id: 1, nombre: "Transmision" }, { id: 2, nombre: "Frenos" }, { id: 3, nombre: "Ruedas" }, { id: 4, nombre: "Accesorios" }, { id: 5, nombre: "Seguridad" }]
};

const state = {
    apiOnline: true,
    token: localStorage.getItem("zamponiAdminToken") || "",
    products: [],
    almacenes: [],
    marcas: [],
    tipos: [],
    activities: JSON.parse(localStorage.getItem("zamponiActivities") || "[]")
};

const $ = (selector) => document.querySelector(selector);
const $$ = (selector) => [...document.querySelectorAll(selector)];

async function api(path, options = {}) {
    const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
    if (state.token) headers.Authorization = `Bearer ${state.token}`;

    const response = await fetch(path, {
        ...options,
        headers
    });

    if (response.status === 401) {
        showLogin("La sesion expiro o falta iniciar sesion.");
        throw new Error("No autorizado.");
    }

    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `Error ${response.status}`);
    }

    const contentType = response.headers.get("content-type") || "";
    return contentType.includes("application/json") ? response.json() : response.text();
}

async function loadData() {
    if (!state.token && location.protocol !== "file:") {
        showLogin();
        return;
    }

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

function showLogin(message = "Demo: admin / zamponi2026") {
    $("#loginMessage").textContent = message;
    $("#loginScreen").classList.add("is-visible");
    document.body.classList.add("locked");
}

function hideLogin() {
    $("#loginScreen").classList.remove("is-visible");
    document.body.classList.remove("locked");
}

function normalizeProducts(products) {
    return products.map((product) => ({
        ...product,
        codigo: product.codigo ?? product.Codigo,
        nombre: product.nombre ?? product.Nombre,
        stock: product.stock ?? product.Stock,
        precioMayorista: product.precioMayorista ?? product.PrecioMayorista ?? 0,
        stockMinimo: product.stockMinimo ?? product.StockMinimo ?? 5,
        imagenUrl: product.imagenUrl ?? product.ImagenUrl ?? "",
        activo: product.activo ?? product.Activo ?? true,
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
    const activeProducts = state.products.filter((product) => product.activo !== false);
    const lowStock = activeProducts.filter((product) => Number(product.stock || 0) <= Number(product.stockMinimo || 0)).length;
    const warehouses = new Set(state.products.map((product) => product.almacen)).size || state.almacenes.length;

    $("#totalProducts").textContent = activeProducts.length;
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
        return product.activo !== false && categoryOk && warehouseOk;
    });

    $("#productsTable").innerHTML = filtered.map((product) => `
        <tr>
            <td><code>${product.codigo}</code></td>
            <td>
                <div class="product-cell">
                    <img class="product-thumb" src="${product.imagenUrl || "assets/zamponi.jpeg"}" alt="">
                    <span>${product.nombre}</span>
                </div>
            </td>
            <td>${product.tipoProducto}</td>
            <td>${product.marca}</td>
            <td>${product.almacen}</td>
            <td>${formatMoney(product.precioMayorista)}</td>
            <td>${product.stockMinimo}</td>
            <td><span class="stock-badge ${Number(product.stock) <= Number(product.stockMinimo) ? "low" : ""}">${product.stock}</span></td>
            <td>
                <div class="row-actions">
                    <button class="icon-button" type="button" data-action="edit" data-id="${product.id}">Editar</button>
                    <button class="icon-button danger" type="button" data-action="deactivate" data-id="${product.id}">Desactivar</button>
                </div>
            </td>
        </tr>
    `).join("");
}

function formatMoney(value) {
    return new Intl.NumberFormat("es-AR", { style: "currency", currency: "ARS", maximumFractionDigits: 0 }).format(Number(value || 0));
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
            ${product.tipoProducto} · ${product.marca} · ${product.almacen} · ${formatMoney(product.precioMayorista)} · Stock: ${product.stock}
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
        stockMinimo: Number(data.stockMinimo),
        precioMayorista: Number(data.precioMayorista),
        imagenUrl: data.imagenUrl.trim(),
        activo: data.activo === "true",
        almacenId: Number(data.almacenId),
        marcaId: Number(data.marcaId),
        tipoProductoId: Number(data.tipoProductoId)
    };
    const id = Number(data.id);

    try {
        if (state.apiOnline) {
            await api(id ? `/api/productos/${id}` : "/api/productos", { method: id ? "PUT" : "POST", body: JSON.stringify(payload) });
            await loadData();
        } else {
            const almacen = state.almacenes.find((item) => item.id === payload.almacenId);
            const marca = state.marcas.find((item) => item.id === payload.marcaId);
            const tipo = state.tipos.find((item) => item.id === payload.tipoProductoId);
            const updatedProduct = { id: id || Date.now(), ...payload, almacen: almacen.nombre, marca: marca.name, tipoProducto: tipo.nombre };
            if (id) {
                state.products = state.products.map((product) => product.id === id ? updatedProduct : product);
            } else {
                state.products.unshift(updatedProduct);
            }
            renderAll();
        }

        addActivity(id ? "Producto editado" : "Producto cargado", `${payload.codigo} · ${payload.nombre}`);
        resetProductForm();
        renderActivities();
    } catch (error) {
        alert(error.message);
    }
});

$("#productsTable").addEventListener("click", async (event) => {
    const button = event.target.closest("button[data-action]");
    if (!button) return;

    const id = Number(button.dataset.id);
    const product = state.products.find((item) => item.id === id);
    if (!product) return;

    if (button.dataset.action === "edit") {
        fillProductForm(product);
        document.querySelector("#productForm").scrollIntoView({ behavior: "smooth", block: "center" });
        return;
    }

    if (!confirm(`Desactivar ${product.codigo}?`)) return;

    try {
        if (state.apiOnline) {
            await api(`/api/productos/${id}/desactivar`, { method: "PATCH" });
            await loadData();
        } else {
            product.activo = false;
            renderAll();
        }

        addActivity("Producto desactivado", `${product.codigo} · ${product.nombre}`);
        renderActivities();
    } catch (error) {
        alert(error.message);
    }
});

$("#cancelEditButton").addEventListener("click", resetProductForm);

function fillProductForm(product) {
    const form = $("#productForm");
    form.elements.id.value = product.id;
    form.elements.codigo.value = product.codigo;
    form.elements.nombre.value = product.nombre;
    form.elements.stock.value = product.stock;
    form.elements.stockMinimo.value = product.stockMinimo;
    form.elements.precioMayorista.value = product.precioMayorista;
    form.elements.imagenUrl.value = product.imagenUrl || "";
    form.elements.almacenId.value = product.almacenId;
    form.elements.marcaId.value = product.marcaId;
    form.elements.tipoProductoId.value = product.tipoProductoId;
    form.elements.activo.value = String(product.activo !== false);
    $("#saveProductButton").textContent = "Actualizar producto";
    $("#cancelEditButton").hidden = false;
}

function resetProductForm() {
    $("#productForm").reset();
    $("#productForm").elements.id.value = "";
    $("#productForm").elements.stock.value = 0;
    $("#productForm").elements.stockMinimo.value = 5;
    $("#productForm").elements.precioMayorista.value = 0;
    $("#saveProductButton").textContent = "Guardar producto";
    $("#cancelEditButton").hidden = true;
}

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

$("#loginForm").addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = Object.fromEntries(new FormData(event.currentTarget));

    try {
        const result = await api("/api/auth/login", {
            method: "POST",
            body: JSON.stringify({ username: data.username, password: data.password })
        });

        state.token = result.token ?? result.Token;
        localStorage.setItem("zamponiAdminToken", state.token);
        hideLogin();
        await loadData();
        addActivity("Sesion iniciada", `Usuario ${result.username ?? result.Username}`);
        renderActivities();
    } catch (error) {
        if (location.protocol === "file:") {
            state.token = "demo";
            localStorage.setItem("zamponiAdminToken", state.token);
            hideLogin();
            await loadData();
            return;
        }

        $("#loginMessage").textContent = error.message;
    }
});

$("#logoutButton").addEventListener("click", () => {
    state.token = "";
    localStorage.removeItem("zamponiAdminToken");
    showLogin("Sesion cerrada.");
});

$("#categoryFilter").addEventListener("change", renderProducts);
$("#warehouseFilter").addEventListener("change", renderProducts);

if (state.token || location.protocol === "file:") {
    loadData();
} else {
    showLogin();
}
