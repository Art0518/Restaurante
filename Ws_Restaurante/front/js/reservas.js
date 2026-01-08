let reserva = {
    zona: null,
    personas: null,
    IdMesa: null,
    fecha: null,
    horaTexto: null,
    hora24: null,
    horasOcupadas: []
};

let mesasZona = [];

// ===========================================================
// 📅 FECHA CORREGIDA (-1 día por el navegador adelantado)
// ===========================================================
function getHoyCorregido() {
    const d = new Date();
    d.setDate(d.getDate() - 1);  // 🔥 Compensación
    return d;
}

// Obtener mañana REAL basado en HOY corregido
function getMananaReal() {
    const d = getHoyCorregido();
    d.setDate(d.getDate() + 1); // mañana real

    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, "0");
    const dia = String(d.getDate()).padStart(2, "0");

    return `${y}-${m}-${dia}`;
}

// ===========================================================
// 🟢 BLOQUEAR SI NO ESTÁ LOGUEADO
// ===========================================================
document.addEventListener("DOMContentLoaded", () => {
    const usuario = JSON.parse(localStorage.getItem("usuario"));
    if (!usuario) {
        alert("Debes iniciar sesión para hacer una reserva.");
        window.location.href = "index.html";
        return;
    }

    if (document.getElementById("nombre"))
        document.getElementById("nombre").value = usuario.Nombre;

    if (document.getElementById("correo"))
        document.getElementById("correo").value = usuario.Email;

    if (document.getElementById("telefono"))
        document.getElementById("telefono").value = usuario.Telefono || "";
});

// ===========================================================
// 🔙 FUNCIÓN VOLVER
// ===========================================================
function goBack(step) {
    if (step === 1) {
        document.getElementById("step2").style.display = "none";
        document.getElementById("step1").style.display = "flex";
    }
    if (step === 2) {
        document.getElementById("step3").style.display = "none";
        document.getElementById("step2").style.display = "flex";
    }
    if (step === 3) {
        document.getElementById("step4").style.display = "none";
        document.getElementById("step3").style.display = "flex";
    }
}

// ===========================================================
// 🟦 PASO 1 → CARGAR MESAS
// ===========================================================
function selectZona(z) {
    reserva.zona = z;

    document.getElementById("step1").style.display = "none";
    document.getElementById("step2").style.display = "flex";

    cargarMesasPorZona();
}

function cargarMesasPorZona() {
    fetch("http://cafesanjuanr.runasp.net/api/mesas")
        .then(r => r.json())
        .then(todas => {
            mesasZona = todas.filter(m => m.TipoMesa === reserva.zona);

            const cont = document.getElementById("listaMesas");
            cont.innerHTML = "";

            if (!mesasZona.length) {
                cont.innerHTML = `<p style="color:white;">No hay mesas en esta zona.</p>`;
                return;
            }

            let html = `
                <div style="
                    display:flex;
                    flex-wrap:wrap;
                    justify-content:center;
                    gap:20px;
                    margin-top:10px;
                ">
            `;

            mesasZona.forEach(m => {
                const imgSrc = m.ImagenURL?.trim() ? m.ImagenURL : "img/mesa-placeholder.jpg";

                html += `
                    <div id="mesa-${m.IdMesa}"
                        onclick="seleccionarMesa(${m.IdMesa})"
                        style="
                            width:220px;
                            background:white;
                            border-radius:15px;
                            overflow:hidden;
                            box-shadow:0 4px 12px rgba(0,0,0,0.25);
                            cursor:pointer;
                        ">

                        <img src="${imgSrc}" style="width:100%; height:140px; object-fit:cover;">

                        <div style="padding:10px;">
                            <h3 style="margin:0; color:#1a4d2e; font-size:18px;">
                                Mesa ${m.NumeroMesa}
                            </h3>
                            <p style="margin:4px 0; font-size:14px;">
                                Capacidad: <b>${m.Capacidad} personas</b>
                            </p>
                        </div>

                    </div>
                `;
            });

            html += "</div>";
            cont.innerHTML = html;
        })
        .catch(err => {
            alert("Error al cargar las mesas.");
            console.log(err);
        });
}

// ===========================================================
// 🟩 PASO 2 → SELECCIONAR MESA
// ===========================================================
function seleccionarMesa(idMesa) {
    const mesa = mesasZona.find(m => m.IdMesa === idMesa);
    if (!mesa) return alert("Error al seleccionar mesa.");

    reserva.IdMesa = idMesa;
    reserva.personas = mesa.Capacidad;

    mesasZona.forEach(m => {
        const card = document.getElementById("mesa-" + m.IdMesa);
        if (card) card.style.border = "3px solid transparent";
    });

    document.getElementById("mesa-" + idMesa).style.border = "3px solid #1a4d2e";

    document.getElementById("step2").style.display = "none";
    document.getElementById("step3").style.display = "flex";

    inicializarFechaHora();
}

// ===========================================================
// 🟧 PASO 3 → FECHA + HORAS
// ===========================================================
function inicializarFechaHora() {
    const fechaInput = document.getElementById("fechaSeleccion");

    // 🔥 FECHA MÍNIMA: mañana real (con fecha corregida)
    fechaInput.min = getMananaReal();

    fechaInput.removeEventListener("change", validarFecha);
    fechaInput.addEventListener("change", validarFecha);
}

function validarFecha() {
    const fecha = document.getElementById("fechaSeleccion").value;
    if (!fecha) return;

    reserva.fecha = fecha;

    fetch(`http://cafesanjuanr.runasp.net/api/mesas/${reserva.IdMesa}/disponibilidad?fecha=${fecha}`)
        .then(r => r.json())
        .then(data => {
            reserva.horasOcupadas = data.map(r => r.Hora);
            document.getElementById("horaSeleccion").disabled = false;
            cargarHorasDisponibles();
        })
        .catch(err => {
            console.log(err);
            alert("Error al cargar disponibilidad.");
        });
}

// ===========================================================
// 🟦 CARGAR HORAS DISPONIBLES + REGLA 24h
// ===========================================================
function cargarHorasDisponibles() {

    const select = document.getElementById("horaSeleccion");
    select.innerHTML = `<option value="">-- Selecciona una hora --</option>`;

    const HORARIO_INICIO = 8;
    const HORARIO_FIN = 22;

    const ahora = getHoyCorregido(); // 🔥 fecha corregida
    const fechaSel = new Date(reserva.fecha);

    // límite: hoy real + 24h
    const limiteMinimo = new Date(ahora.getTime() + 24 * 60 * 60 * 1000);

    for (let h = HORARIO_INICIO; h <= HORARIO_FIN; h++) {

        const horaSql = `${h.toString().padStart(2, "0")}:00:00`;

        if (reserva.horasOcupadas.includes(horaSql)) continue;

        const fechaHora = new Date(
            fechaSel.getFullYear(),
            fechaSel.getMonth(),
            fechaSel.getDate(),
            h, 0, 0
        );

        // REGLA: solo permitir > 24h reales
        if (fechaHora <= limiteMinimo) {
            continue;
        }

        const label = new Date(0, 0, 0, h).toLocaleTimeString("es-EC", {
            hour: "2-digit",
            minute: "2-digit"
        });

        const option = document.createElement("option");
        option.value = label;
        option.textContent = label;
        select.appendChild(option);
    }
}

// ===========================================================
// ⏰ Convertir hora
// ===========================================================
function convertirHoraA24(hora12) {
    let texto = hora12.toLowerCase().replace(/\s+/g, " ").trim();
    let partes = texto.split(" ");
    let [hora, minutos] = partes[0].split(":");
    let ampm = partes.slice(1).join("").replace(/\./g, "");

    hora = parseInt(hora);

    if (ampm === "pm" && hora !== 12) hora += 12;
    if (ampm === "am" && hora === 12) hora = 0;

    return `${hora.toString().padStart(2, "0")}:${minutos}:00`;
}

// ===========================================================
// 🟦 CONFIRMAR FECHA/HORA
// ===========================================================
function confirmarFechaHora() {
    const fecha = document.getElementById("fechaSeleccion").value;
    const hora12 = document.getElementById("horaSeleccion").value;

  if (!fecha || !hora12) {
     showWarning("Por favor, selecciona fecha y hora.");
        return;
    }

    reserva.fecha = fecha;
    reserva.horaFinal = convertirHoraA24(hora12);

    document.getElementById("step3").style.display = "none";
    document.getElementById("step4").style.display = "flex";

    document.getElementById("fecha").value = fecha;
document.getElementById("hora").value = hora12;
}

// ===========================================================
// 🟩 ENVIAR RESERVA
// ===========================================================
function submitReserva() {

    const usuario = JSON.parse(localStorage.getItem("usuario"));

    // Recopilar todos los datos actuales de la reserva
 const fechaSeleccionada = reserva.fecha;
    const horaSeleccionada = document.getElementById("horaSeleccion").value;
    const comentarios = document.getElementById("comentarios").value;

    console.log('Datos antes de enviar:', {
    fecha: fechaSeleccionada,
        hora: horaSeleccionada,
        personas: reserva.personas,
  mesa: reserva.IdMesa,
        comentarios: comentarios
    });

    const data = {
        IdUsuario: usuario.IdUsuario,
 IdMesa: reserva.IdMesa,
  Fecha: reserva.fecha,
  Hora: reserva.horaFinal,
        NumeroPersonas: reserva.personas,
        Observaciones: comentarios,
        Estado: "HOLD"
    };

    fetch("http://cafesanjuanr.runasp.net/api/reservas/crear", {
        method: "POST",
      headers: { "Content-Type": "application/json" },
body: JSON.stringify(data)
    })
      .then(r => r.json())
  .then((response) => {
    // Limpiar cualquier dato anterior de reserva
 localStorage.removeItem("reservaConfirmada");
   
    // Crear objeto con los datos exactos de la reserva actual
    // USAR EL MISMO ESTADO QUE SE ENVIÓ A LA BASE DE DATOS
      const reservaConfirmada = {
    IdReserva: response.IdReserva || Math.floor(Math.random() * 10000),
    Fecha: fechaSeleccionada,
      Hora: horaSeleccionada,
  NumeroPersonas: reserva.personas,
    IdMesa: reserva.IdMesa,
        Observaciones: comentarios || "Ninguno",
    Estado: "HOLD", // ✅ Usar el estado real que se envió
                FechaCreacion: new Date().toISOString(),
    TimeStamp: Date.now() // Para verificar que es la reserva más reciente
            };

  console.log('Guardando en localStorage:', reservaConfirmada);
     
    // Guardar en localStorage
       localStorage.setItem("reservaConfirmada", JSON.stringify(reservaConfirmada));
     
       // Verificar que se guardó correctamente
  const verificacion = JSON.parse(localStorage.getItem("reservaConfirmada"));
    console.log('Verificación de guardado:', verificacion);
    
   // Mostrar mensaje y redirigir cuando el usuario presione Aceptar
   showSuccess("Tu reserva ha sido creada exitosamente", "¡Reserva Confirmada!", function() {
       window.location.href = "confirmacion.html";
 });
  })
  .catch(err => {
  showError("Error al crear la reserva. Por favor, intenta nuevamente.");
   console.log(err);
        });
}

// ===========================================================
// 🧹 LIMPIAR DATOS ANTERIORES AL INICIAR NUEVA RESERVA
// ===========================================================
function limpiarDatosAnteriores() {
    // Limpiar datos de confirmación anterior para evitar confusiones
    localStorage.removeItem("reservaConfirmada");
    console.log("Datos de reserva anterior limpiados");
}

// Ejecutar al cargar la página de reservas
document.addEventListener('DOMContentLoaded', function() {
    limpiarDatosAnteriores();
});

// ===========================================================
// 🟨 PASO 1: SELECCIONAR ZONA
// ===========================================================
