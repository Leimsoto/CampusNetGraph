using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CampusNetGraph.Models;

namespace CampusNetGraph.Views
{
    // Capa View encargada exclusivamente de la presentación en consola (recibe datos ya procesados por el Controller)
    public class ConsolaView
    {
        public ConsolaView()
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        public void MostrarBannerInicial()
        {
            Console.WriteLine("==========================================================================");
            Console.WriteLine("     CAMPUSNET - RED SOCIAL ACADÉMICA DE GRAFOS DIRIGIDOS (MVC EN C#)     ");
            Console.WriteLine("==========================================================================");
        }

        public void MostrarEncabezadoSeccion(string titulo)
        {
            Console.WriteLine("\n--------------------------------------------------------------------------");
            Console.WriteLine($"  {titulo}");
            Console.WriteLine("--------------------------------------------------------------------------");
        }

        public void MostrarSubtitulo(string texto)
        {
            Console.WriteLine($"\n-> {texto}");
        }

        public void MostrarResultadoOperacion(string operacion, bool exito, string detalle)
        {
            string estado = exito ? "[OK]" : "[RECHAZADO]";
            Console.WriteLine($"  {estado,-11} {operacion,-22} | {detalle}");
        }

        // Impresión de la lista de adyacencias del grafo dirigido a partir de los datos entregados por el Controller
        public void ImprimirListaAdyacencia(
            int totalVertices,
            int totalAristas,
            List<(Vertex Usuario, int GradoEntrada, int GradoSalida, List<string> DestinosEtiquetados)> filasAdyacencia,
            string titulo)
        {
            Console.WriteLine($"\n  [{titulo}]");

            if (totalVertices == 0)
            {
                Console.WriteLine("  (Grafo vacío)");
                return;
            }

            Console.WriteLine($"  Total usuarios (|V|): {totalVertices} | Total relaciones dirigidas (|E|): {totalAristas}");
            Console.WriteLine("  ------------------------------------------------------------------------");

            foreach (var fila in filasAdyacencia)
            {
                Vertex u = fila.Usuario;
                string cabeceraUsuario = $"  [{u.Id}] {u.Nombre,-22} ({u.Rol,-10}) [Ent:{fila.GradoEntrada} | Sal:{fila.GradoSalida}]";

                if (fila.DestinosEtiquetados.Count == 0)
                {
                    Console.WriteLine($"{cabeceraUsuario} -> (Sin seguidos)");
                }
                else
                {
                    Console.WriteLine($"{cabeceraUsuario} -> {string.Join(", ", fila.DestinosEtiquetados)}");
                }
            }
        }

        public void MostrarResultadoBFS(string idInicio, Vertex? usuarioInicio, bool exito, List<string> ordenVisita, int cantidadAlcanzados, string detalle)
        {
            string etiqueta = usuarioInicio != null ? $"{usuarioInicio.Id} - {usuarioInicio.Nombre} ({usuarioInicio.Rol})" : idInicio;
            Console.WriteLine($"\n  * Recorrido BFS desde [{etiqueta}]:");

            if (!exito)
            {
                Console.WriteLine($"    [ERROR] {detalle}");
                return;
            }

            Console.WriteLine($"    - Orden de visita:             {string.Join(" -> ", ordenVisita)}");
            Console.WriteLine($"    - Cantidad vértices alcanzados: {cantidadAlcanzados}");
        }

        public void MostrarResultadoDFS(List<string> ordenDescubrimiento, List<string> ciclosDetectados)
        {
            Console.WriteLine("\n  * Recorrido DFS Completo sobre el grafo:");
            if (ordenDescubrimiento.Count == 0)
            {
                Console.WriteLine("    (Grafo vacío)");
                return;
            }

            Console.WriteLine($"    - Total descubiertos ({ordenDescubrimiento.Count}):");
            Console.WriteLine($"      {string.Join(" -> ", ordenDescubrimiento)}");

            Console.WriteLine($"\n    - Identificación de ciclos dirigidos ({ciclosDetectados.Count} detectado(s)):");
            if (ciclosDetectados.Count == 0)
            {
                Console.WriteLine("      No se detectaron ciclos dirigidos en la estructura.");
            }
            else
            {
                for (int i = 0; i < ciclosDetectados.Count; i++)
                {
                    Console.WriteLine($"      [Ciclo #{i + 1}]: {ciclosDetectados[i]}");
                }
            }
        }

        public void MostrarUsuariosSinSeguidores(List<Vertex> usuariosSinSeguidores)
        {
            Console.WriteLine("\n  * Usuarios sin seguidores (Grado de entrada = 0):");
            if (usuariosSinSeguidores.Count == 0)
            {
                Console.WriteLine("    Todos los usuarios tienen al menos un seguidor.");
                return;
            }

            foreach (Vertex u in usuariosSinSeguidores)
            {
                Console.WriteLine($"    - [{u.Id}] {u.Nombre} ({u.Rol}) -> 0 seguidores");
            }
        }

        public void MostrarUsuariosInfluyentes(List<(Vertex Usuario, List<string> Seguidores)> influyentes, int maxGradoEntrada)
        {
            Console.WriteLine($"\n  * Usuarios más influyentes (Mayor grado de entrada = {maxGradoEntrada} seguidores):");
            if (influyentes.Count == 0)
            {
                Console.WriteLine("    (Sin usuarios registrados)");
                return;
            }

            foreach (var item in influyentes)
            {
                Console.WriteLine($"    - [{item.Usuario.Id}] {item.Usuario.Nombre} ({item.Usuario.Rol}) | Seguido por: {string.Join(", ", item.Seguidores)}");
            }
        }

        public void MostrarUsuariosMasActivos(List<Vertex> masActivos, int maxGradoSalida, List<(Vertex Usuario, int GradoSalida)> usuariosConCuatroOMas)
        {
            Console.WriteLine($"\n  * Usuarios más activos (Mayor grado de salida = {maxGradoSalida} seguidos):");
            foreach (Vertex u in masActivos)
            {
                Console.WriteLine($"    - [{u.Id}] {u.Nombre} ({u.Rol}) -> Sigue a {maxGradoSalida} usuarios");
            }

            Console.WriteLine($"\n  * Usuarios con grado de salida >= 4 ({usuariosConCuatroOMas.Count} usuarios):");
            foreach (var item in usuariosConCuatroOMas)
            {
                Console.WriteLine($"    - [{item.Usuario.Id}] {item.Usuario.Nombre} ({item.Usuario.Rol}) -> Grado de salida: {item.GradoSalida}");
            }
        }

        public void MostrarTablaGrados(List<(Vertex Usuario, int GradoEntrada, int GradoSalida)> tabla)
        {
            Console.WriteLine("\n  * Resumen de grados de conectividad por usuario (ordenado por influencia):");
            Console.WriteLine("    ID    | Nombre                 | Rol        | Grado Entrada | Grado Salida");
            Console.WriteLine("    ------+------------------------+------------+---------------+-------------");
            foreach (var fila in tabla)
            {
                Console.WriteLine($"    {fila.Usuario.Id,-5} | {fila.Usuario.Nombre,-22} | {fila.Usuario.Rol,-10} | {fila.GradoEntrada,13} | {fila.GradoSalida,12}");
            }
        }

        public void MostrarResultadoAlcanzabilidad(string idOrigen, string idDestino, Vertex? uOrigen, Vertex? uDestino, bool alcanzable, List<string> camino, int saltos)
        {
            string etiqOrigen = uOrigen != null ? $"{uOrigen.Id} ({uOrigen.Nombre})" : idOrigen;
            string etiqDestino = uDestino != null ? $"{uDestino.Id} ({uDestino.Nombre})" : idDestino;

            if (alcanzable)
            {
                Console.WriteLine($"  [ALCANZABLE]    {etiqOrigen} -> {etiqDestino} | Saltos: {saltos} | Ruta BFS: {string.Join(" -> ", camino)}");
            }
            else
            {
                Console.WriteLine($"  [NO ALCANZABLE] {etiqOrigen} -/-> {etiqDestino} | No existe camino dirigido entre ambos usuarios.");
            }
        }

        public void MostrarMetricasYAnalisisFinal(
            int totalVertices,
            int totalAristas,
            double densidad,
            double gradoPromedio,
            List<Vertex> influyentes,
            int maxEntrada,
            List<Vertex> activos,
            int maxSalida,
            List<Vertex> sinSeguidores,
            List<Vertex> sinSeguidos,
            List<string> ciclosFinales)
        {
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("              MÉTRICAS DE CONECTIVIDAD Y ANÁLISIS ESTRUCTURAL             ");
            Console.WriteLine("==========================================================================");
            Console.WriteLine($"  * Total de usuarios (vértices |V|):       {totalVertices}");
            Console.WriteLine($"  * Total de seguimientos (aristas |E|):    {totalAristas}");
            Console.WriteLine($"  * Densidad del grafo dirigido:            {densidad:F4} ({densidad * 100:F2}%)");
            Console.WriteLine($"  * Grado promedio (entrada/salida):        {gradoPromedio:F2} relaciones/usuario");
            Console.WriteLine($"  * Mayor grado de entrada (influencia):    {maxEntrada} ({string.Join(", ", influyentes.Select(u => u.Id))})");
            Console.WriteLine($"  * Mayor grado de salida (actividad):      {maxSalida} ({string.Join(", ", activos.Select(u => u.Id))})");
            Console.WriteLine($"  * Nodos fuente (grado entrada = 0):       {sinSeguidores.Count} ({string.Join(", ", sinSeguidores.Select(u => u.Id))})");
            Console.WriteLine($"  * Nodos sumidero (grado salida = 0):      {sinSeguidos.Count} ({string.Join(", ", sinSeguidos.Select(u => u.Id))})");
            Console.WriteLine($"  * Ciclos dirigidos activos:               {ciclosFinales.Count}");
            foreach (string ciclo in ciclosFinales)
            {
                Console.WriteLine($"    - {ciclo}");
            }

            Console.WriteLine("--------------------------------------------------------------------------");
            Console.WriteLine("  JUSTIFICACIÓN Y ANÁLISIS DE LA ESTRUCTURA RESULTANTE:");
            Console.WriteLine("  1. Asimetría y dirección: El modelo dirigido refleja relaciones reales de");
            Console.WriteLine("     seguimiento académico donde seguir a un docente o colega no implica");
            Console.WriteLine("     reciprocidad automática, diferenciando liderazgo de actividad.");
            Console.WriteLine("  2. Comunidades y retroalimentación: Los ciclos dirigidos detectados con DFS");
            Console.WriteLine("     evidencian circuitos cerrados de difusión de información académica.");
            Console.WriteLine("  3. Roles topológicos: Los nodos con grado de entrada 0 actúan como fuentes");
            Console.WriteLine("     (consumen contenido pero no tienen seguidores), mientras que los nodos");
            Console.WriteLine("     con grado de salida 0 actúan como sumideros que limitan el alcance BFS.");
            Console.WriteLine("==========================================================================");
        }
    }
}
