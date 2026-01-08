import { useState } from "react";
import "../styles/style.css";

const AuthModal = ({ isOpen, closeModal }) => {
  const [isLogin, setIsLogin] = useState(true);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    password: "",
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const toggleForm = () => setIsLogin(!isLogin);

  const handleSubmit = (e) => {
    e.preventDefault();
    alert(isLogin ? "Iniciar sesión" : "Registrarse");
    closeModal();
  };

  return (
    isOpen && (
      <div className="auth-modal-overlay">
        <div className="auth-modal">
          {/* ✅ X visible directamente dentro del botón */}
          <button type="button" className="close-btn" onClick={closeModal}>
            ✕
          </button>

          <h2 className="titulo-auth">
            {isLogin ? "Inicia sesión" : "Regístrate"}
          </h2>

          <form onSubmit={handleSubmit}>
            {isLogin ? (
              <>
                <label>
                  Correo electrónico:
                  <input
                    type="email"
                    name="email"
                    placeholder="Ingresa tu correo"
                    value={formData.email}
                    onChange={handleInputChange}
                    required
                  />
                </label>
                <label>
                  Contraseña:
                  <input
                    type="password"
                    name="password"
                    placeholder="Ingresa tu contraseña"
                    value={formData.password}
                    onChange={handleInputChange}
                    required
                  />
                </label>
              </>
            ) : (
              <>
                <label>
                  Nombre completo:
                  <input
                    type="text"
                    name="name"
                    placeholder="Ingresa tu nombre"
                    value={formData.name}
                    onChange={handleInputChange}
                    required
                  />
                </label>
                <label>
                  Correo electrónico:
                  <input
                    type="email"
                    name="email"
                    placeholder="Ingresa tu correo"
                    value={formData.email}
                    onChange={handleInputChange}
                    required
                  />
                </label>
                <label>
                  Teléfono:
                  <input
                    type="tel"
                    name="phone"
                    placeholder="Ingresa tu teléfono"
                    value={formData.phone}
                    onChange={handleInputChange}
                    required
                  />
                </label>
                <label>
                  Contraseña:
                  <input
                    type="password"
                    name="password"
                    placeholder="Ingresa tu contraseña"
                    value={formData.password}
                    onChange={handleInputChange}
                    required
                  />
                </label>
              </>
            )}

            <button type="submit" className="btn-verde">
              {isLogin ? "Iniciar sesión" : "Registrarse"}
            </button>
          </form>

          <p>
            {isLogin ? (
              <>
                ¿No tienes cuenta?{" "}
                <a href="#" onClick={toggleForm}>
                  Regístrate
                </a>
              </>
            ) : (
              <>
                ¿Ya tienes cuenta?{" "}
                <a href="#" onClick={toggleForm}>
                  Inicia sesión
                </a>
              </>
            )}
          </p>
        </div>
      </div>
    )
  );
};

export default AuthModal;
