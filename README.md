# Sistema de Generación de Tickets y Turnos para Pasajeros

## Descripción
Este proyecto es una aplicación diseñada para una empresa de autobuses. Permite a los pasajeros generar y obtener un ticket de bus con un código QR, y a los conductores registrar esos tickets usando un escáner QR para validar el acceso al bus.

La aplicación se compone de dos partes principales:
1. **Generación de tickets y turnos para pasajeros**: Los pasajeros pueden ingresar sus datos y obtener un ticket con un código QR único.
2. **Registro de tickets por parte de los conductores**: Los conductores utilizan un escáner de código QR para verificar y registrar la validez de los tickets a bordo.

## Funcionalidades

### Para los Pasajeros:
- Ingresar información personal (nombre, destino, hora, etc.).
- Generación de un ticket con un código QR único.
- Visualización del código QR para mostrar al conductor.

### Para los Conductores:
- Escanear el código QR del ticket utilizando un escáner.
- Validar la validez del ticket y marcarlo como registrado.
- Registrar el acceso de los pasajeros a bordo.

## Tecnologías Utilizadas

- **Frontend**: 
  - HTML, CSS, JavaScript (vanilla)
  - Bootstrap 5.3 (para diseño responsivo)
  - SweetAlert2 (para mensajes emergentes)
  
- **Backend**:
  - ASP.NET Core con C# (para la lógica del servidor)
  
- **Base de Datos**:
  - SQL Server (o cualquier sistema de base de datos relacional)
  
- **Generación y Escaneo de QR**:
  - Librería de generación de QR: [jsQR](https://github.com/cozmo/jsQR) o similar
  - API de escaneo de QR integrada para conductores.

## Instalación

### Clonar el repositorio

```bash
git clone https://github.com/tu_usuario/proyecto-buses.git
cd proyecto-buses
