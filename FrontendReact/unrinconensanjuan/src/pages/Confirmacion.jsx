import "../styles/style.css";

export default function Confirmacion() {
  return (
    <main style={{ textAlign: "center" }}>
      <h2>¡Gracias por tu reserva!</h2>
      <p>
        Hemos recibido tu solicitud en <strong>Un Rincón en San Juan</strong>.
        Pronto te confirmaremos los detalles por correo.
      </p>
      <a href="/">
        <button>Volver al inicio</button>
      </a>
    </main>
  );
}
