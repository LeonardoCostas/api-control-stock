const glow = document.querySelector(".cursor-glow");

window.addEventListener("pointermove", (event) => {
    if (!glow) return;
    glow.style.transform = `translate(${event.clientX - 140}px, ${event.clientY - 140}px)`;
});

const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if (entry.isIntersecting) {
            entry.target.classList.add("is-visible");
        }
    });
}, { threshold: 0.16 });

document.querySelectorAll(".reveal").forEach((element) => observer.observe(element));

const counters = document.querySelectorAll("[data-count]");
const counterObserver = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if (!entry.isIntersecting) return;

        const element = entry.target;
        const target = Number(element.dataset.count || 0);
        const suffix = element.dataset.suffix || "";
        const duration = 1100;
        const start = performance.now();

        const tick = (time) => {
            const progress = Math.min((time - start) / duration, 1);
            const eased = 1 - Math.pow(1 - progress, 3);
            element.textContent = `${Math.round(target * eased)}${suffix}`;
            if (progress < 1) requestAnimationFrame(tick);
        };

        requestAnimationFrame(tick);
        counterObserver.unobserve(element);
    });
}, { threshold: 0.5 });

counters.forEach((counter) => counterObserver.observe(counter));

const publicDemoProducts = [
    { codigo: "CAD-116-SHI", nombre: "Cadena Shimano 116 eslabones", tipoProducto: "Transmision", marca: "Shimano", precioMayorista: 8200, imagenUrl: "https://images.unsplash.com/photo-1637289031856-9625abbba63f?auto=format&fit=crop&w=600&q=80" },
    { codigo: "PUN-ERG-NEG", nombre: "Punos ergonomicos negros", tipoProducto: "Accesorios", marca: "Zamponi", precioMayorista: 2100, imagenUrl: "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80" },
    { codigo: "CAR-750-TRA", nombre: "Caramanola transparente 750 ml", tipoProducto: "Accesorios", marca: "Zamponi", precioMayorista: 1800, imagenUrl: "https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=600&q=80" },
    { codigo: "CUB-29-KEN", nombre: "Cubierta MTB 29 x 2.10", tipoProducto: "Ruedas", marca: "Kenda", precioMayorista: 14500, imagenUrl: "https://images.unsplash.com/photo-1529422643029-d4585747aaf2?auto=format&fit=crop&w=600&q=80" }
];

let publicProductsData = [...publicDemoProducts];

function normalizePublicProduct(product) {
    return {
        codigo: product.codigo ?? product.Codigo,
        nombre: product.nombre ?? product.Nombre,
        tipoProducto: product.tipoProducto ?? product.TipoProducto,
        marca: product.marca ?? product.Marca,
        precioMayorista: product.precioMayorista ?? product.PrecioMayorista ?? 0,
        imagenUrl: product.imagenUrl ?? product.ImagenUrl ?? "assets/zamponi.jpeg"
    };
}

function renderPublicProducts(products) {
    const container = document.querySelector("#publicProducts");
    if (!container) return;

    container.innerHTML = products.slice(0, 8).map((product) => {
        const message = encodeURIComponent(`Hola Zamponi, queria consultar por ${product.codigo} - ${product.nombre}`);
        return `
            <article class="public-product reveal is-visible">
                <img src="${product.imagenUrl || "assets/zamponi.jpeg"}" alt="">
                <div>
                    <code>${product.codigo}</code>
                    <h3>${product.nombre}</h3>
                    <p>${product.tipoProducto} · ${product.marca} · ${formatPublicMoney(product.precioMayorista)}</p>
                    <a href="https://wa.me/541143033174?text=${message}" target="_blank" rel="noreferrer">Consultar</a>
                </div>
            </article>
        `;
    }).join("");
}

function formatPublicMoney(value) {
    return new Intl.NumberFormat("es-AR", { style: "currency", currency: "ARS", maximumFractionDigits: 0 }).format(Number(value || 0));
}

async function loadPublicCatalog() {
    try {
        const response = await fetch("/api/productos");
        if (!response.ok) throw new Error("API offline");
        const products = await response.json();
        publicProductsData = products.map(normalizePublicProduct);
    } catch {
        publicProductsData = [...publicDemoProducts];
    }

    renderPublicProducts(publicProductsData);
}

document.querySelector("#publicCatalogSearch")?.addEventListener("submit", async (event) => {
    event.preventDefault();
    const text = document.querySelector("#publicCatalogInput").value.trim().toUpperCase();
    if (!text) {
        renderPublicProducts(publicProductsData);
        return;
    }

    try {
        const response = await fetch(`/api/productos/buscar?texto=${encodeURIComponent(text)}`);
        if (!response.ok) throw new Error("API offline");
        const results = await response.json();
        renderPublicProducts(results.map(normalizePublicProduct));
    } catch {
        renderPublicProducts(publicProductsData.filter((product) =>
            product.codigo.toUpperCase().includes(text) ||
            product.nombre.toUpperCase().includes(text) ||
            product.tipoProducto.toUpperCase().includes(text) ||
            product.marca.toUpperCase().includes(text)));
    }
});

loadPublicCatalog();

document.querySelectorAll(".product-card, .shop-feature, .brand-stage, .contact-card").forEach((card) => {
    card.addEventListener("pointermove", (event) => {
        const rect = card.getBoundingClientRect();
        const x = (event.clientX - rect.left) / rect.width - 0.5;
        const y = (event.clientY - rect.top) / rect.height - 0.5;
        card.style.setProperty("--tilt-x", `${(-y * 5).toFixed(2)}deg`);
        card.style.setProperty("--tilt-y", `${(x * 5).toFixed(2)}deg`);
    });

    card.addEventListener("pointerleave", () => {
        card.style.setProperty("--tilt-x", "0deg");
        card.style.setProperty("--tilt-y", "0deg");
    });
});
