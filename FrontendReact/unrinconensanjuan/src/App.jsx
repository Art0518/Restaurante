import { useState, useEffect } from "react";
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";
import Home from "./pages/Home";
import ReservaForm from "./components/ReservaForm";
import Confirmacion from "./pages/Confirmacion";
import Menu from "./pages/Menu"; // ⭐ IMPORTANTE: Agregado el Menú
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoadingScreen from "./components/LoadingScreen";
import "./styles/style.css";

export default function App() {
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      setIsLoading(false);
    }, 3000);

    return () => clearTimeout(timer);
  }, []);

  return (
    <BrowserRouter>
      {isLoading ? (
        <LoadingScreen />
      ) : (
        <>
          <Navbar />

          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/reservas" element={<ReservaForm />} />
            <Route path="/confirmacion" element={<Confirmacion />} />

            {/* ⭐ NUEVA RUTA MENÚ */}
            <Route path="/menu" element={<Menu />} />
          </Routes>

          <Footer />
        </>
      )}
    </BrowserRouter>
  );
}
