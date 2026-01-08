let reservaConfirmando = null;
let reservasUsuario = [];

// ================================
// Cargar reservas del usuario
// ================================
document.addEventListener("DOMContentLoaded", () => {
    const usuario = JSON.parse(localStorage.getItem("usuario"));

    if (!usuario) {
        alert("Debes iniciar sesión.");
        window.location.href = "index.html";
        return;
    }

    cargarReservas(usuario.IdUsuario);

    document.getElementById("filtro-estado").addEventListener("change", aplicarFiltros);
});

// ================================
// Obtener reservas
// ================================
function cargarReservas(idUsuario) {
    fetch(`http://cafesanjuanr.runasp.net/api/reservas/filtrar?idUsuario=${idUsuario}`)
        .then(r => r.json())
        .then(reservas => {
            reservasUsuario = reservas;
            mostrarReservas(reservas);
        });
}

// ================================
// Mostrar tarjetas
// ================================
function mostrarReservas(lista) {
    const cont = document.getElementById("lista-reservas");
    cont.innerHTML = "";

    if (!lista.length) {
        cont.innerHTML = "<p>No tienes reservas registradas.</p>";
        return;
    }

    lista.forEach(r => {

        let botones = `
            <button class="btn btn-cancelar" onclick="cancelarReserva(${r.IdReserva})">Cancelar</button>
        `;

        // SI ESTA EN HOLD → debe mostrar CONFIRMAR
        if (r.Estado === "HOLD") {
            botones = `
                <button class="btn btn-confirmar" onclick="abrirModalConfirmar(${r.IdReserva})">Confirmar</button>
                <button class="btn btn-cancelar" onclick="cancelarReserva(${r.IdReserva})">Cancelar</button>
            `;
        }

        // Determinar clase de tarjeta según estado
        const tarjetaClass = r.Estado === "CONFIRMADA" ? "tarjeta confirmada" : "tarjeta";

        // Mostrar método de pago si está confirmada
        const metodoPagoHtml = r.Estado === "CONFIRMADA" ? `<p><b>Método pago:</b> ${r.MetodoPago || 'N/A'}</p>` : '';

        cont.innerHTML += `
            <div class="${tarjetaClass}">
                <p><b>Fecha:</b> ${r.Fecha.split("T")[0]}</p>
                <p><b>Hora:</b> ${r.Hora}</p>
                <p><b>Personas:</b> ${r.NumeroPersonas}</p>
                <p><b>Mesa:</b> ${r.IdMesa}</p>
                <p><b>Estado:</b> ${r.Estado}</p>

                ${metodoPagoHtml}

                ${botones}
            </div>
        `;
    });
}

// ================================
// Filtros
// ================================
function aplicarFiltros() {
    const estado = document.getElementById("filtro-estado").value;

    let lista = reservasUsuario;

    if (estado !== "") {
        lista = lista.filter(r => r.Estado === estado);
    }

    mostrarReservas(lista);
}

// ================================
// CONFIRMAR RESERVA (modal)
// ================================
function abrirModalConfirmar(idReserva) {
    reservaConfirmando = idReserva;
    document.getElementById("modal-confirmar").style.display = "flex";
}

function cerrarConfirmar() {
    document.getElementById("modal-confirmar").style.display = "none";
}

// ================================
// FINALIZAR CONFIRMACIÓN
// ================================
function confirmarReservaFinal() {

    const metodo = document.getElementById("metodo-pago").value;

    const body = {
        IdReserva: reservaConfirmando,
        Estado: "CONFIRMADA",
        MetodoPago: metodo
    };

    fetch("http://cafesanjuanr.runasp.net/api/reservas/estado", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    })
        .then(r => r.json())
        .then(() => {
            alert("Reserva confirmada correctamente");
            cerrarConfirmar();
            location.reload();
        });
}

// ================================
// CANCELAR
// ================================
function cancelarReserva(idReserva) {

    if (!confirm("¿Seguro que deseas cancelar esta reserva?")) return;

    fetch("http://cafesanjuanr.runasp.net/api/reservas/estado", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ IdReserva: idReserva, Estado: "CANCELADA" })
    })
        .then(r => r.json())
        .then(() => {
            alert("Reserva cancelada");
            location.reload();
        });
}
