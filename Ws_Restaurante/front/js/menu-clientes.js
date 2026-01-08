const API_PLATOS = "http://cafesanjuanr.runasp.net/api/platos";

let platosGlobal = [];

// ============================
// Cargar platos al iniciar
// ============================
document.addEventListener("DOMContentLoaded", cargarMenu);

function cargarMenu() {
    fetch(API_PLATOS)
        .then(r => r.json())
        .then(data => {
            platosGlobal = data;
            renderMenu(data);
        })
        .catch(err => console.log("Error cargando menú:", err));
}

// ============================
// Renderizar menú
// ============================
function renderMenu(lista) {
    const cont = document.getElementById("lista-menu");
    cont.innerHTML = "";

    lista.forEach(p => {
        const nombre = p.Nombre || p.Plato;

        cont.innerHTML += `
            <div class="menu-card" onclick="abrirModalPlato(${p.IdPlato})">

                <img src="${p.ImagenURL || 'img/placeholder.jpg'}">

                <h3>${nombre}</h3>

                <div class="menu-info"><b>Categoría:</b> ${p.Categoria}</div>
                <div class="menu-info"><b>Tipo:</b> ${p.TipoComida}</div>
                <div class="menu-info"><b>Precio:</b> $${p.Precio}</div>

                <p class="menu-desc">${p.Descripcion}</p>
            </div>
        `;
    });
}

// ============================
// Filtros
// ============================
document.getElementById("f-cat").onchange = filtrar;
document.getElementById("f-tipo").onchange = filtrar;
document.getElementById("f-texto").onkeyup = filtrar;

function filtrar() {
    const cat = document.getElementById("f-cat").value.toLowerCase();
    const tipo = document.getElementById("f-tipo").value.toLowerCase();
    const texto = document.getElementById("f-texto").value.toLowerCase();

    let lista = platosGlobal;

    if (cat) lista = lista.filter(p => (p.Categoria || "").toLowerCase() === cat);
    if (tipo) lista = lista.filter(p => (p.TipoComida || "").toLowerCase().includes(tipo));
    if (texto) {
        lista = lista.filter(p =>
            (p.Nombre || p.Plato || "").toLowerCase().includes(texto) ||
            (p.Descripcion || "").toLowerCase().includes(texto)
        );
    }

    renderMenu(lista);
}

// ============================
// MODAL – Abrir
// ============================
function abrirModalPlato(idPlato) {

    const p = platosGlobal.find(x => x.IdPlato == idPlato);
    if (!p) return;

    const nombre = p.Nombre || p.Plato;

    document.getElementById("md-imagen").src = p.ImagenURL || "img/placeholder.jpg";
    document.getElementById("md-nombre").innerText = nombre;
    document.getElementById("md-categoria").innerText = p.Categoria || "Sin categoría";
    document.getElementById("md-tipo").innerText = p.TipoComida || "Sin tipo";
    document.getElementById("md-precio").innerText = p.Precio;
    document.getElementById("md-descripcion").innerText = p.Descripcion || "Sin descripción";

    document.getElementById("modal-plato").style.display = "flex";
}

// ============================
// MODAL – Cerrar
// ============================
function cerrarModalPlato() {
    document.getElementById("modal-plato").style.display = "none";
}
