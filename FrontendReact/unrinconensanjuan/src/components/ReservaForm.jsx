import { useState } from "react";
import "../styles/style.css";

export default function ReservaForm() {
  const [step, setStep] = useState(1);
  const [zona, setZona] = useState("");
  const [personas, setPersonas] = useState(null);
  const [formData, setFormData] = useState({
    fecha: "",
    hora: "",
    nombre: "",
    correo: "",
    telefono: "",
    comentarios: "",
  });

  const handleNext = () => setStep(step + 1);
  const handlePrev = () => setStep(step - 1);

  // 🕒 Lista de horas fijas (AM/PM)
  const horasDisponibles = [
    "08:00 AM", "09:00 AM", "10:00 AM", "11:00 AM",
    "12:00 PM", "01:00 PM", "02:00 PM", "03:00 PM",
    "04:00 PM", "05:00 PM", "06:00 PM", "07:00 PM",
    "08:00 PM", "09:00 PM", "10:00 PM"
  ];

  // 🧠 Filtra las horas pasadas si la fecha seleccionada es hoy
  const getHorasValidas = () => {
    if (!formData.fecha) return horasDisponibles;

    const hoy = new Date();
    const fechaSeleccionada = new Date(formData.fecha + "T00:00:00");

    // Si la fecha no es hoy, mostrar todas las horas
    if (fechaSeleccionada.toDateString() !== hoy.toDateString()) {
      return horasDisponibles;
    }

    const horaActual = hoy.getHours();

    // Convertir las horas de la lista a formato 24h para comparar
    return horasDisponibles.filter((hora) => {
      let [time, meridiem] = hora.split(" ");
      let [h] = time.split(":");
      let hour = parseInt(h);

      if (meridiem === "PM" && hour !== 12) hour += 12;
      if (meridiem === "AM" && hour === 12) hour = 0;

      // Solo mostrar horas iguales o mayores a la hora actual + 1
      return hour > horaActual;
    });
  };

  const handleInput = (e) => {
    const { name, value } = e.target;

    // ✅ Solo números en teléfono
    if (name === "telefono" && /[^0-9]/.test(value)) return;

    // ✅ Validar fecha (solo actual o futura)
    if (name === "fecha") {
      const hoy = new Date();
      const seleccionada = new Date(value);
      hoy.setHours(0, 0, 0, 0);
      if (seleccionada < hoy) {
        alert("⚠️ La fecha no puede ser anterior a la actual.");
        return;
      }

      // Reiniciar hora si cambia la fecha
      setFormData({ ...formData, fecha: value, hora: "" });
      return;
    }

    setFormData({ ...formData, [name]: value });
  };

  const validarCorreo = (correo) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo);

  const handleSubmit = (e) => {
    e.preventDefault();

    // ⚙️ Validaciones finales
    if (!formData.fecha) return alert("⚠️ Por favor selecciona una fecha.");
    if (!formData.hora) return alert("⚠️ Selecciona una hora disponible.");
    if (!formData.nombre.trim()) return alert("⚠️ Ingresa tu nombre.");
    if (!validarCorreo(formData.correo)) return alert("⚠️ Ingresa un correo electrónico válido.");
    if (formData.telefono.length < 7) return alert("⚠️ El número de teléfono debe tener al menos 7 dígitos.");

    alert(`✅ Reserva registrada:
Zona: ${zona}
Personas: ${personas}
Fecha: ${formData.fecha}
Hora: ${formData.hora}`);
  };

  return (
    <section className="reserva-form fondo-caribeno">
      {/* === PASO 1: Zona === */}
      {step === 1 && (
        <div className="reserva-step fade-in">
          <h2>Selecciona tu zona favorita</h2>
          <div className="zona-opciones">
            <button onClick={() => { setZona("Interior"); handleNext(); }}>Interior</button>
            <button onClick={() => { setZona("Exterior"); handleNext(); }}>Exterior</button>
          </div>
        </div>
      )}

      {/* === PASO 2: Personas === */}
      {step === 2 && (
        <div className="reserva-step fade-in">
          <h2>Elige la cantidad de personas</h2>
          <div className="personas-grid">
            {[2, 3, 4, 5, 6].map((num) => (
              <button key={num} onClick={() => { setPersonas(num); handleNext(); }}>
                {num}
              </button>
            ))}
          </div>
          <button className="volver-btn" onClick={handlePrev}>← Volver</button>
        </div>
      )}

      {/* === PASO 3: Datos === */}
      {step === 3 && (
        <div className="reserva-step fade-in">
          <h2>Detalles de tu reserva</h2>
          <form className="reserva-formulario" onSubmit={handleSubmit}>
            <label>
              Fecha:
              <input
                type="date"
                name="fecha"
                value={formData.fecha}
                onChange={handleInput}
                min={new Date().toISOString().split("T")[0]}
                required
              />
            </label>

            <label>
              Hora:
              <select
                name="hora"
                value={formData.hora}
                onChange={handleInput}
                required
                disabled={!formData.fecha}
              >
                <option value="">-- Selecciona una hora --</option>
                {getHorasValidas().length > 0 ? (
                  getHorasValidas().map((hora) => (
                    <option key={hora} value={hora}>{hora}</option>
                  ))
                ) : (
                  <option disabled>⏰ No hay horarios disponibles hoy</option>
                )}
              </select>
            </label>

            <label>
              Nombre completo:
              <input
                type="text"
                name="nombre"
                value={formData.nombre}
                onChange={handleInput}
                required
              />
            </label>

            <label>
              Correo electrónico:
              <input
                type="email"
                name="correo"
                value={formData.correo}
                onChange={handleInput}
                required
              />
            </label>

            <label>
              Teléfono:
              <input
                type="tel"
                name="telefono"
                value={formData.telefono}
                onChange={handleInput}
                required
              />
            </label>

            <label>
              Comentarios:
              <textarea
                name="comentarios"
                value={formData.comentarios}
                onChange={handleInput}
                placeholder="Ej. Celebración, mesa especial, etc."
              />
            </label>

            <button type="submit" className="btn-verde">Ver disponibilidad</button>
          </form>

          <button className="volver-btn" onClick={handlePrev}>← Cambiar personas</button>
        </div>
      )}
    </section>
  );
}
