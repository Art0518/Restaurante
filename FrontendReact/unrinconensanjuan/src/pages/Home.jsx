import "../styles/style.css";


export default function Home() {
  return (
    <>
      {/* === HERO === */}
      <section className="home-hero">
        <div className="home-content">
          <h1>Bienvenido a Un Rincón en San Juan</h1>
          <p>
            Vive una experiencia caribeña única entre sabores, aromas y
            tradición.
          </p>
          <a href="/reservas">
            <button>Reservar una mesa</button>
          </a>
        </div>
      </section>

      {/* === HISTORIA === */}
      <section className="section">
        <h2>Nuestra Historia</h2>
        <p>
          En el corazón del Viejo San Juan, entre calles adoquinadas y brisa del mar Caribe, nació Un Rincón en San Juan. Nuestro sueño comenzó con una simple idea: crear un espacio donde el aroma del café puertorriqueño se mezcle con la calidez de nuestra gente y la pasión por la buena comida.

          Cada taza que servimos es un homenaje a nuestra tierra, a sus tradiciones y a las manos que cultivan el mejor café del Caribe.

          Aquí, cada sabor cuenta una historia, la historia de Puerto Rico, contada desde un rincón lleno de amor, sabor y raíces.
        </p>

        {/* Imagen decorativa debajo del texto */}
        <div className="historia-imagen-container">
          <img
            src="/Fondo-historia.png"
            alt="Vista del Viejo San Juan al atardecer"
            className="historia-imagen"
          />
        </div>
      </section>

      {/* === ESPECIALIDADES === */}
      <section className="section especialidades">
        <h2>Especialidades</h2>
        <p>Descubre nuestras delicias más queridas por los visitantes.</p>

        <div className="grid-especialidades">
          {/* CAFÉS */}
          <div className="card-especialidad">
            <img
              src="https://apasionadosporelcafe.com/wp-content/uploads/2017/06/apasionados-espresso-caribe-cover.png"
              alt="Café Caribeño"
            />
            <h3>Café Caribeño</h3>
            <p>Suave mezcla puertorriqueña con notas de cacao y canela.</p>
          </div>

          <div className="card-especialidad">
            <img
              src="https://sanjuanfoodtours.com/wp-content/uploads/2023/04/san-juan-coffee-flavors-food-tours-1200x800-1-1024x683.jpg"
              alt="Espresso San Juan"
            />
            <h3>Espresso San Juan</h3>
            <p>Intenso y aromático, elaborado con granos tostados localmente.</p>
          </div>

          <div className="card-especialidad">
            <img
              src="https://www.brighteyedbaker.com/wp-content/uploads/2024/07/Dulce-de-Leche-Latte-Recipe.jpg"
              alt="Latte Borinqueño"
            />
            <h3>Latte Borinqueño</h3>
            <p>
              Latte cremoso con espuma de leche y un toque de canela y vainilla.
            </p>
          </div>

          {/* PASTELES / POSTRES */}
          <div className="card-especialidad">
            <img
              src="https://tiaclara.com/wp-content/uploads/2013/12/caribbean-guava-tart-DSC3240.jpg"
              alt="Pastel de Guayaba"
            />
            <h3>Pastel de Guayaba</h3>
            <p>Postre tradicional del Caribe con relleno de guayaba natural.</p>
          </div>

          <div className="card-especialidad">
            <img
              src="https://img.bestrecipes.com.au/IyKrHzG5/br/2017/11/1980-tropical-cheesecake-log-953767-1.jpg"
              alt="Cheesecake Tropical"
            />
            <h3>Cheesecake Tropical</h3>
            <p>Cremoso cheesecake con toques de coco y piña fresca.</p>
          </div>

          <div className="card-especialidad">
            <img
              src="https://www.recetasnestle.com.do/sites/default/files/styles/recipe_detail_desktop_new/public/srh_recipes/f4d0c7d20ba3edf480b0e92261be0320.jpg?itok=oCk0AlVG"
              alt="Flan de Coco"
            />
            <h3>Flan de Coco</h3>
            <p>
              Clásico flan casero con coco rallado y caramelo dorado al estilo
              boricua.
            </p>
          </div>
        </div>
      </section>

      {/* === CONTACTO === */}
      <section className="section">
        <h2>Contáctanos</h2>
        <div className="contact-item">
          <i className="fas fa-map-marker-alt"></i>
          <span>Calle del Sol #45, San Juan, Puerto Rico</span>
        </div>
        <div className="contact-item">
          <i className="fas fa-phone"></i>
          <span>+1 (787) 555-7890</span>
        </div>
        <div className="contact-item">
          <i className="fas fa-clock"></i>
          <span>Lunes a Domingo de 8:00 AM a 10:00 PM</span>
        </div>
      </section>
    </>
  );
}
