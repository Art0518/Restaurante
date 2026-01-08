export default function Menu() {
  const platos = [
    {
      nombre: "Ensalada del Granjero",
      img: "https://images.squarespace-cdn.com/content/v1/5dc5f87bbf44c51db8038ae1/1580164265298-0F3U4I3TGOOH35LCRXMC/farmer-salad.png"
    },
    {
      nombre: "Ahi Sellado de Verano",
      img: "https://images.squarespace-cdn.com/content/v1/5dc5f87bbf44c51db8038ae1/1580164288284-KPVQ0HCL7FJ5JROWM6U6/summer-ahi.png"
    },
    {
      nombre: "Pollo Estilo Baja",
      img: "https://images.squarespace-cdn.com/content/v1/5dc5f87bbf44c51db8038ae1/1580164309189-WN55M2FGGQ4JP7R5UQ4T/baja-chicken.png"
    },
    {
      nombre: "Ensalada Flatiron",
      img: "https://images.squarespace-cdn.com/content/v1/5dc5f87bbf44c51db8038ae1/1580164349891-7SK9X6RPKEMV9KJ20JUE/flatiron.png"
    },
    {
      nombre: "Ensalada Thai con Coco",
      img: "https://images.squarespace-cdn.com/content/v1/5dc5f87bbf44c51db8038ae1/1580164368087-UC6X7SUZQ4V7N3H1B8S4/thai-coconut.png"
    },
    {
      nombre: "Crea tu Ensalada",
      img: "https://images.squarespace-cdn.com/content/v1/5dc5f87bbf44c51db8038ae1/1580164387055-2S6XW9BNGZ3N8YETVZ8P/build-your-own.png"
    }
  ];

  return (
    <section className="section menu-section">
      <h2>Nuestro Menú</h2>

      <div className="menu-grid">
        {platos.map((plato, i) => (
          <div key={i} className="menu-card">
            <img src={plato.img} alt={plato.nombre} />
            <h3>{plato.nombre}</h3>
            <button className="btn-order">Ordenar</button>
          </div>
        ))}
      </div>
    </section>
  );
}
