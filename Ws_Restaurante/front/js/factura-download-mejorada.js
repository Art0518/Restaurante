// ============================================================
// FUNCIÓN MEJORADA PARA DESCARGAR FACTURA CON LOGO
// ============================================================
// Reemplazar la función descargarFactura() en carrito.js con esta versión mejorada

function descargarFactura() {
    if (!facturaActual || !facturaActual.IdFactura) {
  showAlert('No hay factura para descargar', 'warning');
        return;
    }

    const ventanaImpresion = window.open('', '_blank', 'width=800,height=600');

  const contenidoFactura = `
    <!DOCTYPE html>
    <html lang="es">
    <head>
    <meta charset="UTF-8">
        <title>Factura #${facturaActual.IdFactura} - Café San Juan</title>
 <style>
    body { 
      font-family: Arial, sans-serif; 
     margin: 20px; 
     color: #333;
  }
     .header { 
    text-align: center; 
              border-bottom: 3px solid #5a3e2b; 
        padding-bottom: 20px; 
      margin-bottom: 30px;
     }
            .logo {
             max-width: 120px;
            height: auto;
  margin-bottom: 15px;
   }
            .empresa-nombre {
  font-size: 28px;
    color: #5a3e2b;
             font-weight: bold;
  margin: 10px 0;
         }
            .factura-titulo {
  font-size: 20px;
       color: #666;
      margin: 5px 0;
            }
      .fecha {
    color: #888;
        font-size: 14px;
            }
       .info-section { 
           margin: 20px 0; 
        display: flex;
      justify-content: space-between;
            }
       .info-box {
       flex: 1;
       padding: 15px;
          background-color: #f9f9f9;
         border-radius: 5px;
    margin: 0 10px;
     }
            .info-box h3 {
     color: #5a3e2b;
                border-bottom: 2px solid #5a3e2b;
       padding-bottom: 8px;
   margin-top: 0;
            }
       .info-box p {
      margin: 8px 0;
     line-height: 1.6;
}
    .table { 
        width: 100%; 
                border-collapse: collapse; 
              margin: 20px 0; 
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
  .table th, .table td { 
  border: 1px solid #ddd; 
       padding: 12px; 
    text-align: left; 
     }
  .table th { 
           background-color: #5a3e2b; 
    color: white;
                font-weight: bold;
            }
  .table tbody tr:nth-child(even) {
           background-color: #f9f9f9;
            }
     .table tbody tr:hover {
     background-color: #f0f0f0;
      }
 .totals { 
   text-align: right; 
  margin-top: 30px; 
          padding: 20px;
         background-color: #f9f9f9;
    border-radius: 5px;
            }
            .total-line { 
          margin: 8px 0; 
     font-size: 16px;
         }
    .final-total { 
            font-weight: bold; 
          font-size: 1.4em; 
      color: #5a3e2b;
         margin-top: 15px;
             padding-top: 15px;
     border-top: 2px solid #5a3e2b;
            }
        .footer {
        text-align: center;
  margin-top: 40px;
                padding-top: 20px;
             border-top: 1px solid #ddd;
          color: #888;
        font-size: 12px;
            }
            @media print {
         body { margin: 0; }
         .no-print { display: none; }
  }
        </style>
    </head>
    <body>
        <div class="header">
 <img src="img/logo-rincon.png" alt="Café San Juan Logo" class="logo">
            <div class="empresa-nombre">Café San Juan</div>
   <div class="factura-titulo">FACTURA #${facturaActual.IdFactura}</div>
    <div class="fecha">Fecha: ${new Date().toLocaleDateString('es-ES', { 
    day: '2-digit', 
        month: '2-digit', 
       year: 'numeric' 
  })}</div>
        </div>
     
     <div class="info-section">
            <div class="info-box">
                <h3>Cliente</h3>
    <p><strong>Nombre:</strong> ${usuario?.Nombre || 'Cliente'}</p>
         <p><strong>Teléfono:</strong> ${usuario?.Telefono || 'No especificado'}</p>
                <p><strong>Email:</strong> ${usuario?.Email || 'No especificado'}</p>
          </div>
            <div class="info-box">
       <h3>Factura</h3>
    <p><strong>Número:</strong> ${facturaActual.IdFactura}</p>
    <p><strong>Estado:</strong> ${document.getElementById('factura-estado').textContent}</p>
                <p><strong>Método de Pago:</strong> ${document.getElementById('factura-metodo-pago').textContent}</p>
            </div>
        </div>
        
        <table class="table">
          <thead>
            <tr>
              <th>Descripción</th>
     <th style="width: 100px; text-align: center;">Cantidad</th>
        <th style="width: 120px; text-align: right;">Precio Unit.</th>
        <th style="width: 120px; text-align: right;">Subtotal</th>
        </tr>
   </thead>
    <tbody>
    ${Array.from(document.getElementById('factura-detalles').rows).map(row =>
        `<tr>${Array.from(row.cells).map((cell, index) => {
   const align = index === 0 ? 'left' : index === 1 ? 'center' : 'right';
     return `<td style="text-align: ${align};">${cell.textContent}</td>`;
            }).join('')}</tr>`
     ).join('')}
            </tbody>
     </table>
  
        <div class="totals">
     <div class="total-line">Subtotal: ${document.getElementById('factura-subtotal').textContent}</div>
            ${document.getElementById('factura-descuento-row').style.display !== 'none' ? 
     `<div class="total-line" style="color: #28a745;">Descuento: ${document.getElementById('factura-descuento').textContent}</div>` : ''}
     <div class="total-line">IVA (11.5%): ${document.getElementById('factura-iva').textContent}</div>
 <div class="final-total">Total: ${document.getElementById('factura-total').textContent}</div>
    </div>
        
  <div class="footer">
 <p><strong>Café San Juan</strong> - Sabores del Caribe y tradición puertorriqueña</p>
   <p>Gracias por su preferencia</p>
</div>
        
 <script>
     window.onload = function() { 
      // Esperar a que la imagen se cargue antes de imprimir
             setTimeout(function() {
          window.print(); 
      }, 500);
   }
     </script>
    </body>
    </html>
    `;

  ventanaImpresion.document.write(contenidoFactura);
    ventanaImpresion.document.close();
}
