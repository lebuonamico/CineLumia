# Resumen del Proyecto: Cine-Lumia

Este documento describe la estructura y los componentes principales del proyecto Cine-Lumia, una aplicación web desarrollada con ASP.NET Core MVC. El objetivo de la aplicación es gestionar la venta de entradas de cine y productos consumibles (snacks) de la cadena de cines "Lumia".

## Arquitectura General

El proyecto sigue el patrón de diseño **Modelo-Vista-Controlador (MVC)**, que separa la lógica de negocio, la interfaz de usuario y la interacción del usuario en tres componentes interconectados.

- **Modelos (Models & Entities):** Representan los datos y la lógica de negocio. Se dividen en dos conceptos principales:
  - **Entidades (Entities):** Clases que mapean directamente a las tablas de la base de datos (Ej: `Pelicula`, `Cine`, `Usuario`).
  - **ViewModels:** Clases diseñadas para transportar los datos que las Vistas necesitan mostrar, combinando información de una o más entidades.
- **Vistas (Views):** Son los archivos `.cshtml` que componen la interfaz de usuario. Muestran los datos proporcionados por los controladores y envían las acciones del usuario de vuelta a estos.
- **Controladores (Controllers):** Actúan como intermediarios. Reciben las peticiones del usuario desde las Vistas, interactúan con los Modelos para consultar o modificar datos y, finalmente, devuelven una Vista con los datos necesarios.

---

## Descripción de Componentes

### 1. Entidades (`/Entities`)

Contiene las clases principales que definen la estructura de la base de datos. Cada clase es una tabla y sus propiedades son las columnas.

- **Pelicula.cs:** Define una película con propiedades como título, sinopsis, duración, URL del póster, etc.
- **Cine.cs:** Representa una sucursal del cine (nombre, ubicación).
- **Sala.cs:** Define una sala dentro de un cine (capacidad, número de sala).
- **Proyeccion.cs (Función):** Representa una función específica: una película que se proyecta en una sala, en una fecha y hora determinadas.
- **Asiento.cs:** Modela un asiento individual en una sala.
- **Entrada.cs:** Representa una entrada de cine comprada para una función y un asiento.
- **Consumible.cs:** Define los productos de snack (nombre, precio, imagen).
- **Espectador.cs:** Modela al usuario/cliente que realiza compras.
- **Relaciones:** Clases como `PeliculaGenero` o `CineConsumible` gestionan las relaciones "muchos a muchos" entre entidades.

### 2. Modelos (`/Models`)

Además de las entidades, esta carpeta contiene clases clave para la lógica y la presentación.

- **CineDbContext.cs:** Es el corazón de la interacción con la base de datos. Utiliza **Entity Framework Core** para actuar como un puente (ORM) entre el código C# y la base de datos SQL. Aquí se definen los `DbSet` que representan cada tabla.
- **CineSeeder.cs:** Una clase de utilidad para poblar la base de datos con datos iniciales (ej: crear cines, películas o un usuario administrador por defecto).
- **ViewModels (`/Models/ViewModel`):** Clases específicas para las Vistas. Por ejemplo:
  - `LoginViewModel.cs`: Contiene solo los campos necesarios para el formulario de login (Email, Password).
  - `ResumenCompraViewModel.cs`: Agrupa toda la información necesaria para mostrar el resumen de una compra (entradas, asientos, consumibles, total).

### 3. Controladores (`/Controllers`)

Gestionan el flujo de la aplicación y la lógica de negocio.

- **HomeController.cs:** Controlador principal. Gestiona la página de inicio, mostrando el banner de películas y la cartelera principal.
- **AccountController.cs:** Administra todo lo relacionado con las cuentas de usuario: registro (`Register`), inicio de sesión (`Login`), cierre de sesión, gestión de perfil (`Profile`) y cambio de contraseña.
- **PeliculasController.cs:** Muestra el catálogo de películas.
- **FuncionesController.cs:** Permite a los usuarios ver las funciones (horarios) disponibles para una película en un cine específico.
- **VentaEntradasController.cs:** Inicia el proceso de compra de entradas para una función seleccionada.
- **AsientosController.cs:** Muestra el mapa de asientos de una sala para que el usuario seleccione dónde sentarse.
- **ConsumiblesController.cs:** Gestiona la "tienda" de snacks, permitiendo al usuario añadirlos a su carrito.
- **ResumenController.cs:** Muestra la vista de resumen de la compra antes de proceder al pago.
- **PagoController.cs:** Simula el proceso de pago, procesando la tarjeta y finalizando la compra.
- **ComprasRealizadasController.cs:** Permite a los usuarios ver su historial de compras.

### 4. Vistas (`/Views`)

Son los archivos `.cshtml` (Razor) que generan el HTML que ve el usuario. La estructura de carpetas coincide con la de los controladores.

- **`/Views/Shared`:** Contiene las vistas y componentes reutilizables en toda la aplicación.
  - **_Layout.cshtml:** Es la plantilla principal del sitio. Define la estructura común (menú de navegación, footer) donde se renderizan las demás vistas.
- **`/Views/Home/Index.cshtml`:** La página de inicio.
- **`/Views/Account/Login.cshtml`:** El formulario para iniciar sesión.
- **`/Views/Asientos/Index.cshtml`:** La interfaz visual para seleccionar los asientos.
- **`/Views/ResumenCompra/Index.cshtml`:** La página que muestra el detalle final del carrito antes de pagar.
- El resto de las vistas corresponden a las acciones de sus respectivos controladores, mostrando formularios, listados o detalles.

### 5. Archivos de Configuración y Arranque

- **Program.cs:** Es el punto de entrada de la aplicación. Aquí se configura el servidor web, se registran los servicios (como `CineDbContext`), se configura la autenticación y se define el enrutamiento MVC.
- **appsettings.json:** Almacena las cadenas de conexión a la base de datos y otras configuraciones de la aplicación.

### 6. Contenido Estático (`/wwwroot`)

Carpeta que contiene todos los archivos estáticos que se envían directamente al navegador del cliente.

- **`/css`:** Hojas de estilo (CSS) para dar diseño a la aplicación.
- **`/js`:** Archivos de JavaScript para añadir interactividad en el lado del cliente.
- **`/images` y `/img`:** Contiene las imágenes utilizadas en el sitio, como pósters de películas, fotos de snacks y logos.
- **`/lib`:** Bibliotecas de terceros para el frontend, como Bootstrap y jQuery.

---
## Flujo de Usuario Típico (Compra de Entrada)

1.  **Inicio:** El usuario llega a `Home/Index` y ve la cartelera.
2.  **Selección de Función:** Elige una película y ve los horarios (`Funciones/Index`).
3.  **Inicio de Compra:** Selecciona un horario, lo que lo lleva a `VentaEntradas/Index`.
4.  **Selección de Asientos:** Es redirigido a `Asientos/Index` para elegir sus butacas.
5.  **Añadir Snacks (Opcional):** El sistema lo lleva a `Consumibles/Index` para añadir snacks a su orden.
6.  **Resumen de Compra:** Se le presenta `ResumenCompra/Index` con el detalle de su pedido y el total a pagar.
7.  **Pago:** Procede a `Pago/Index`, ingresa los datos de su tarjeta y confirma la compra.
8.  **Confirmación:** La compra se registra en la base de datos y se le muestra una confirmación. El usuario puede ver esta compra en `ComprasRealizadas/Index`.
