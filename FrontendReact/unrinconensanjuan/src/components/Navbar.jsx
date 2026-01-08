import { useState } from "react";
import logo from "../assets/logo-rincon.png";
import "../styles/style.css";
import AuthModal from "./AuthModal"; // Importar el modal

export default function Navbar() {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const openModal = () => setIsModalOpen(true);
  const closeModal = () => setIsModalOpen(false);

  return (
    <header>
      <nav className="navbar">
        <img src={logo} alt="Un Rincón en San Juan" className="logo" />

        <div className="nav-text">
          <a href="/" className="nav-link">Inicio</a>
          <span className="separator">•</span>

          <a href="/reservas" className="nav-link">Reservas</a>
          <span className="separator">•</span>

          <a href="/menu" className="nav-link">Menú</a>
        </div>

        {/* 🔥 MOSTRAR ROL DE ADMIN */}
        <span className="user-name">Admin (Admin)</span>

        {/* 🔥 NUEVO BOTÓN: AÑADIR PLATO */}
        <a href="/admin/add-plato" className="btn-add-plato">
          Añadir Plato
        </a>

        {/* Botón de iniciar sesión */}
        <button className="login-btn" onClick={openModal}>
          Iniciar sesión
        </button>
      </nav>

      <AuthModal isOpen={isModalOpen} closeModal={closeModal} />
    </header>
  );
}
