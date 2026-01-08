const API = "http://cafesanjuanr.runasp.net/api/reservas";
const API_MESAS = "http://cafesanjuanr.runasp.net/api/mesas";

let reservasGlobal = [];
let reservaEditando = null;

// ================================
// VALIDAR ADMIN
// ================================
document.addEventListener("DOMContentLoaded", () => {
    const usuario = JSON.parse(localStorage.getItem("usuario"));
    if (!usuario || usuario.Rol !== "ADMIN") {
        alert("Acceso denegado.");
        window.location.href = "index.html";
        return;
    }
    cargarReservas();
});

// ================================
// Cargar todas las reservas
// ================================
function cargarReservas() {
    fetch(API)
        .then(r => r.json())
        .then(reservas => {
            reservasGlobal = reservas;
            mostrarReservas(reservas);
        });
}

// ================================
// Mostrar tarjetas
// ================================
function mostrarReservas(lista) {
    const cont = document.getElementById("contenedor-reservas");
    cont.innerHTML = "";
    if (!lista.length) {
        cont.innerHTML = "<p>No hay reservas registradas.</p>";
        return;
    }
    lista.forEach(r => {
        cont.innerHTML += `
        <div class="tarjeta">
            <p><b>Fecha:</b> ${r.Fecha.split("T")[0]}</p>
            <p><b>Hora:</b> ${r.Hora}</p>
            <p><b>Personas:</b> ${r.NumeroPersonas}</p>
            <p><b>Mesa:</b> ${r.IdMesa}</p>
            <p><b>Estado:</b> ${r.Estado}</p>
            <p><b>Cliente (ID):</b> ${r.IdUsuario}</p>
            <div style="margin-top:15px;">
                <button class="btn btn-confirmar" onclick="actualizarEstado(${r.IdReserva}, 'CONFIRMADA')">Confirmar</button>
                <button class="btn btn-completar" onclick="actualizarEstado(${r.IdReserva}, 'COMPLETADA')">Completar</button>
                <button class="btn btn-cancelar" onclick="actualizarEstado(${r.IdReserva}, 'CANCELADA')">Cancelar</button>
            </div>
        </div>`;
    });
}

// ================================
// FILTROS
// ================================
function aplicarFiltros() {
    let id = document.getElementById("filtro-id").value.trim();
    let cliente = document.getElementById("filtro-cliente").value.trim();
    let estado = document.getElementById("filtro-estado").value;
    let lista = reservasGlobal;

    if (id) lista = lista.filter(r => r.IdReserva == id);
    if (cliente) lista = lista.filter(r => r.IdUsuario == cliente);
    if (estado) lista = lista.filter(r => r.Estado == estado);

    mostrarReservas(lista);
}

// ================================
// MODAL
// ================================
function abrirModalAdmin(idReserva) {
    reservaEditando = reservasGlobal.find(r => r.IdReserva === idReserva);

    const fechaISO = reservaEditando.Fecha.split("T")[0];
    document.getElementById("adm-fecha").value = fechaISO;

    document.getElementById("adm-personas").value = reservaEditando.NumeroPersonas;
    document.getElementById("adm-mesa").value = reservaEditando.IdMesa;
    document.getElementById("adm-obs").value = reservaEditando.Observaciones || "";

    const horas = [
        "08:00 AM", "09:00 AM", "10:00 AM", "11:00 AM",
        "12:00 PM", "01:00 PM", "02:00 PM", "03:00 PM",
        "04:00 PM", "05:00 PM", "06:00 PM", "07:00 PM",
        "08:00 PM", "09:00 PM", "10:00 PM"
    ];

    const sel = document.getElementById("adm-hora");
    sel.innerHTML = "";

    const hora12 = formato12(reservaEditando.Hora);
    horas.forEach(h => {
        const op = document.createElement("option");
        op.value = h;
        op.textContent = h;
        if (h === hora12) op.selected = true;
        sel.appendChild(op);
    });

    document.getElementById("modal-admin").style.display = "flex";
}

function cerrarModalAdmin() {
    document.getElementById("modal-admin").style.display = "none";
}

// ================================
// GUARDAR CAMBIOS (ADMIN EDITAR)
// ================================
function guardarCambiosAdmin() {
    const horaAMPM = document.getElementById("adm-hora").value;

    const data = {
        IdReserva: reservaEditando.IdReserva,
        Fecha: document.getElementById("adm-fecha").value,
        Hora: horaAMPM,
        NumeroPersonas: parseInt(document.getElementById("adm-personas").value),
        IdMesa: parseInt(document.getElementById("adm-mesa").value),
        Observaciones: document.getElementById("adm-obs").value,
        IdUsuario: reservaEditando.IdUsuario,
        Estado: reservaEditando.Estado
    };

    fetch(`${API}/editar`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
        .then(r => {
            if (!r.ok) throw new Error("Error API");
            return r.json();
        })
        .then(() => {
            alert("Reserva actualizada correctamente.");
            cerrarModalAdmin();
            cargarReservas();
        })
        .catch(err => console.error("Error:", err));
}

// ================================
// CAMBIAR ESTADO RESERVA + MESA
// ================================
function actualizarEstado(idReserva, estado) {
    fetch(`${API}/estado`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ IdReserva: idReserva, Estado: estado })
    })
        .then(r => r.json())
        .then(() => {
            const reserva = reservasGlobal.find(r => r.IdReserva === idReserva);

            if (!reserva) {
                alert("Error: no se encontró la reserva para actualizar la mesa");
                return;
            }

            // CONFIRMADA → mesa ocupada
            if (estado === "CONFIRMADA") {
                actualizarEstadoMesa(reserva.IdMesa, "Ocupada");
            }

            // COMPLETADA → mesa disponible
            if (estado === "COMPLETADA") {
                actualizarEstadoMesa(reserva.IdMesa, "Disponible");
            }

            // CANCELADA → mesa disponible
            if (estado === "CANCELADA") {
                actualizarEstadoMesa(reserva.IdMesa, "Disponible");
            }

            alert("Estado actualizado.");
            cargarReservas();
        });
}

// ================================
// CAMBIAR ESTADO MESA (API)
// ================================
function actualizarEstadoMesa(idMesa, nuevoEstado) {
    return fetch(`${API_MESAS}/${idMesa}/estado`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Estado: nuevoEstado })
    })
        .then(r => r.json())
        .then(() => console.log(`Mesa ${idMesa} actualizada → ${nuevoEstado}`))
        .catch(err => console.error("Error actualizando mesa:", err));
}

// ================================
// CONVERSIÓN DE HORAS
// ================================
function formato12(hora24) {
    let [h, m] = hora24.split(":");
    h = parseInt(h);
    const ampm = h >= 12 ? "PM" : "AM";
    if (h > 12) h -= 12;
    if (h === 0) h = 12;
    return `${h.toString().padStart(2, "0")}:${m} ${ampm}`;
}

function formato24(hora12) {
    let [hora, minutos] = hora12.split(":");
    let sufijo = minutos.split(" ")[1];
    minutos = minutos.split(" ")[0];
    hora = parseInt(hora);
    if (sufijo === "PM" && hora !== 12) hora += 12;
    if (sufijo === "AM" && hora === 12) hora = 0;
    return `${hora.toString().padStart(2, "0")}:${minutos}:00`;
}
