using System;
using System.Collections.Generic;
using CampusNetGraph.Models;
using CampusNetGraph.Views;

namespace CampusNetGraph.Controllers
{
    // Capa Controller encargada de secuenciar y orquestar todos los casos de uso de CampusNet
    public class GraphController
    {
        private readonly Graph grafo;
        private readonly ConsolaView vista;

        public GraphController()
        {
            grafo = new Graph();
            vista = new ConsolaView();
        }

        public void IniciarSimulacion()
        {
            vista.MostrarBannerInicial();

            ConstruirGrafoInicial();
            EjecutarRecorridos();
            EjecutarConsultasSociales();
            EjecutarOperacionesCrud();
            MostrarAnalisisFinal();
        }

        private void RegistrarUsuario(string id, string nombre, string rol)
        {
            bool exito = grafo.AgregarUsuario(id, nombre, rol, out string detalle);
            vista.MostrarResultadoOperacion($"InsertarUsuario({id})", exito, detalle);
        }

        private void RegistrarRelacion(string idOrigen, string idDestino)
        {
            bool exito = grafo.AgregarRelacion(idOrigen, idDestino, out string detalle);
            vista.MostrarResultadoOperacion($"Seguir({idOrigen} -> {idDestino})", exito, detalle);
        }

        private void MostrarEstructuraActual(string titulo)
        {
            var detalleAdyacencia = grafo.ObtenerDetalleListaAdyacencia();
            vista.ImprimirListaAdyacencia(
                grafo.ContarVertices(),
                grafo.ContarAristas(),
                detalleAdyacencia,
                titulo);
        }

        // ==================== 1. CONSTRUCCIÓN DEL GRAFO ====================
        private void ConstruirGrafoInicial()
        {
            vista.MostrarEncabezadoSeccion("1. CONSTRUCCIÓN DEL GRAFO DIRIGIDO (Mínimo 12 usuarios y 18 aristas)");

            vista.MostrarSubtitulo("1.1. Registro de 12 usuarios académicos con roles variados (+ prueba de duplicado):");
            RegistrarUsuario("U01", "Dra. Elena Restrepo", "Profesor");
            RegistrarUsuario("U02", "Dr. Carlos Mejía", "Profesor");
            RegistrarUsuario("U03", "Sofía Valencia", "Estudiante");
            RegistrarUsuario("U04", "Mateo Giraldo", "Estudiante");
            RegistrarUsuario("U05", "Ing. Laura Patiño", "Egresado");
            RegistrarUsuario("U06", "Andrés Castaño", "Estudiante");
            RegistrarUsuario("U07", "Dra. Diana Londoño", "Profesor");
            RegistrarUsuario("U08", "Camila Orozco", "Egresado");
            RegistrarUsuario("U09", "Julián Ríos", "Estudiante");
            RegistrarUsuario("U10", "Ing. Felipe Salazar", "Egresado");
            RegistrarUsuario("U11", "Valentina Duque", "Estudiante");  // Sin seguidores (grado de entrada 0)
            RegistrarUsuario("U12", "Sebastián Henao", "Egresado");    // Sin seguidores (grado de entrada 0)

            // Prueba de control de duplicados en vértices (case-insensitive)
            RegistrarUsuario("u01", "Usuario Duplicado", "Estudiante");

            vista.MostrarSubtitulo("1.2. Registro de 24 relaciones dirigidas de seguimiento (+ pruebas de validación):");

            // U01 sigue a 5 usuarios (Grado de salida = 5 >= 4)
            RegistrarRelacion("U01", "U02");
            RegistrarRelacion("U01", "U03");
            RegistrarRelacion("U01", "U04");
            RegistrarRelacion("U01", "U05");
            RegistrarRelacion("U01", "U07");

            // U02 sigue a 5 usuarios (Grado de salida = 5 >= 4)
            RegistrarRelacion("U02", "U03");
            RegistrarRelacion("U02", "U05");
            RegistrarRelacion("U02", "U06");
            RegistrarRelacion("U02", "U08");
            RegistrarRelacion("U02", "U10");

            // U03 sigue a 4 usuarios (Grado de salida = 4 >= 4) y cierra el Ciclo 1: U01 -> U02 -> U03 -> U01
            RegistrarRelacion("U03", "U01");
            RegistrarRelacion("U03", "U04");
            RegistrarRelacion("U03", "U07");
            RegistrarRelacion("U03", "U09");

            // Relaciones intermedias y Ciclo 2: U05 -> U06 -> U08 -> U05
            RegistrarRelacion("U04", "U07");
            RegistrarRelacion("U05", "U06");
            RegistrarRelacion("U06", "U08");
            RegistrarRelacion("U07", "U09");
            RegistrarRelacion("U08", "U05");
            RegistrarRelacion("U08", "U10");
            RegistrarRelacion("U09", "U10");

            // Relaciones salientes desde nodos sin seguidores (U11 y U12 conservan grado de entrada = 0)
            RegistrarRelacion("U11", "U01");
            RegistrarRelacion("U11", "U07");
            RegistrarRelacion("U12", "U04");

            // Pruebas de control de duplicados de aristas y autorreferencia
            RegistrarRelacion("U01", "U02");
            RegistrarRelacion("U03", "U03");

            MostrarEstructuraActual("ESTRUCTURA INICIAL DEL GRAFO (LISTA DE ADYACENCIA)");
        }

        // ==================== 2. RECORRIDOS BFS Y DFS ====================
        private void EjecutarRecorridos()
        {
            vista.MostrarEncabezadoSeccion("2. RECORRIDOS EN ANCHURA (3 BFS) Y EN PROFUNDIDAD (1 DFS COMPLETO)");

            vista.MostrarSubtitulo("2.1. Ejecución de BFS desde 3 usuarios distintos:");
            EjecutarBFSDesde("U01"); // Nodo altamente conectado en componente principal
            EjecutarBFSDesde("U05"); // Nodo en subcomunidad de egresados/estudiantes
            EjecutarBFSDesde("U12"); // Nodo fuente sin seguidores

            vista.MostrarSubtitulo("2.2. Ejecución de DFS completo con detección de ciclos dirigidos:");
            grafo.RecorridoDFSCompleto(out List<string> ordenDescubrimiento, out List<string> ciclosDetectados);
            vista.MostrarResultadoDFS(ordenDescubrimiento, ciclosDetectados);
        }

        private void EjecutarBFSDesde(string idInicio)
        {
            Vertex? usuario = grafo.ObtenerUsuario(idInicio);
            bool exito = grafo.RecorridoBFS(idInicio, out List<string> ordenVisita, out int alcanzados, out string detalle);
            vista.MostrarResultadoBFS(idInicio, usuario, exito, ordenVisita, alcanzados, detalle);
        }

        // ==================== 3. CONSULTAS SOCIALES ====================
        private void EjecutarConsultasSociales()
        {
            vista.MostrarEncabezadoSeccion("3. CONSULTAS SOCIALES Y MÉTRICAS DE INFLUENCIA / ALCANZABILIDAD");

            vista.MostrarSubtitulo("3.1. Tabla general de grados de entrada (seguidores) y salida (seguidos):");
            vista.MostrarTablaGrados(grafo.ObtenerTablaConectividad());

            vista.MostrarSubtitulo("3.2. Usuarios sin seguidores (Grado de entrada = 0):");
            vista.MostrarUsuariosSinSeguidores(grafo.ObtenerUsuariosSinSeguidores());

            vista.MostrarSubtitulo("3.3. Usuarios más influyentes (Mayor grado de entrada):");
            var influyentes = grafo.ObtenerDetalleUsuariosMasInfluyentes(out int maxGradoEntrada);
            vista.MostrarUsuariosInfluyentes(influyentes, maxGradoEntrada);

            vista.MostrarSubtitulo("3.4. Usuarios más activos (Mayor grado de salida y verificación >= 4):");
            List<Vertex> activos = grafo.ObtenerUsuariosMasActivos(out int maxGradoSalida);
            var conCuatroOMas = grafo.ObtenerUsuariosConSalidaMinima(4);
            vista.MostrarUsuariosMasActivos(activos, maxGradoSalida, conCuatroOMas);

            vista.MostrarSubtitulo("3.5. Verificación de alcanzabilidad dirigida (A -> B) mediante BFS:");
            ProbarAlcanzabilidad("U01", "U10"); // Alcanzable en varios saltos
            ProbarAlcanzabilidad("U11", "U06"); // Alcanzable desde nodo sin seguidores
            ProbarAlcanzabilidad("U05", "U01"); // No alcanzable (sin camino de retorno hacia U01)
            ProbarAlcanzabilidad("U10", "U03"); // No alcanzable (U10 tiene grado de salida 0)
        }

        private void ProbarAlcanzabilidad(string idOrigen, string idDestino)
        {
            Vertex? uOrigen = grafo.ObtenerUsuario(idOrigen);
            Vertex? uDestino = grafo.ObtenerUsuario(idDestino);
            bool alcanzable = grafo.VerificarAlcanzabilidadBFS(idOrigen, idDestino, out List<string> camino, out int saltos);
            vista.MostrarResultadoAlcanzabilidad(idOrigen, idDestino, uOrigen, uDestino, alcanzable, camino, saltos);
        }

        // ==================== 4. OPERACIONES CRUD ====================
        private void EjecutarOperacionesCrud()
        {
            vista.MostrarEncabezadoSeccion("4. OPERACIONES CRUD SOBRE EL GRAFO (Con impresión tras cada cambio)");

            // 4.1 Agregar nuevos usuarios y nuevas relaciones dirigidas
            vista.MostrarSubtitulo("4.1. Agregar nuevos usuarios ('U13', 'U14') y registrar relaciones dirigidas:");
            RegistrarUsuario("U13", "Natalia Gómez", "Estudiante");
            RegistrarUsuario("U14", "Dr. Mauricio Pardo", "Profesor");
            RegistrarRelacion("U13", "U01");
            RegistrarRelacion("U04", "U14");
            RegistrarRelacion("U14", "U10");
            MostrarEstructuraActual("GRAFO TRAS AGREGAR USUARIOS (U13, U14) Y SUS RELACIONES");

            // 4.2 Actualizar nombre y rol de un usuario existente
            vista.MostrarSubtitulo("4.2. Actualizar nombre y rol de usuario existente ('U03' pasa a Egresado):");
            bool act = grafo.ActualizarUsuario("U03", "Ing. Sofía Valencia", "Egresado", out string detAct);
            vista.MostrarResultadoOperacion("ActualizarUsuario(U03)", act, detAct);
            MostrarEstructuraActual("GRAFO TRAS ACTUALIZAR DATOS DE U03");

            // 4.3 Eliminar una relación dirigida específica
            vista.MostrarSubtitulo("4.3. Eliminar relación dirigida ('U02' deja de seguir a 'U08'):");
            bool elimRel = grafo.EliminarRelacion("U02", "U08", out string detElimRel);
            vista.MostrarResultadoOperacion("EliminarRelacion", elimRel, detElimRel);
            MostrarEstructuraActual("GRAFO TRAS ELIMINAR RELACIÓN U02 -> U08");

            // 4.4 Eliminar un usuario completo del grafo y limpiar sus referencias entrantes y salientes
            vista.MostrarSubtitulo("4.4. Eliminar usuario ('U14') y limpiar sus aristas entrantes y salientes:");
            bool elimUsu = grafo.EliminarUsuario("U14", out string detElimUsu);
            vista.MostrarResultadoOperacion("EliminarUsuario(U14)", elimUsu, detElimUsu);
            MostrarEstructuraActual("GRAFO FINAL TRAS ELIMINAR USUARIO U14");
        }

        // ==================== 5. ANÁLISIS ESTRUCTURAL FINAL ====================
        private void MostrarAnalisisFinal()
        {
            List<Vertex> influyentes = grafo.ObtenerUsuariosMasInfluyentes(out int maxEntrada);
            List<Vertex> activos = grafo.ObtenerUsuariosMasActivos(out int maxSalida);
            List<Vertex> sinSeguidores = grafo.ObtenerUsuariosSinSeguidores();
            List<Vertex> sinSeguidos = grafo.ObtenerUsuariosSinSeguidos();
            grafo.RecorridoDFSCompleto(out _, out List<string> ciclosFinales);

            vista.MostrarMetricasYAnalisisFinal(
                grafo.ContarVertices(),
                grafo.ContarAristas(),
                grafo.CalcularDensidad(),
                grafo.CalcularGradoPromedio(),
                influyentes,
                maxEntrada,
                activos,
                maxSalida,
                sinSeguidores,
                sinSeguidos,
                ciclosFinales);
        }
    }
}
