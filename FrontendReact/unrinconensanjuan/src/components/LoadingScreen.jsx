import React, { useEffect, useState } from "react";
import "../styles/style.css";
import logo from "../assets/logo-rincon.png"; // ✅ Logo de Café San Juan

export default function LoadingScreen({ onFinish }) {
  const [visible, setVisible] = useState(true);

  useEffect(() => {
    // Oculta la pantalla de carga después de 3 segundos
    const timer = setTimeout(() => {
      setVisible(false);
      if (onFinish) onFinish();
    }, 3000);

    return () => clearTimeout(timer);
  }, [onFinish]);

  if (!visible) return null;

  return (
    <div className="loading-container">
      <div className="logo-wrapper">
        <img
          src={logo}
          alt="Café San Juan"
          className="logo-animado"
        />
      </div>
      <p className="loading-text">Preparando tu experiencia...</p>
    </div>
  );
}
