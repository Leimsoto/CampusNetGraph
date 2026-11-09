using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusNetGraph.Models
{
    // Implementa el grafo dirigido mediante lista de adyacencia (sin referencias a Console)
    public class Graph
    {
        // Estructura principal solicitada: lista de adyacencia (IdOrigen -> Lista de IdDestino)
        private readonly Dictionary<string, List<string>> listaAdyacencia;

        // Diccionario auxiliar para conservar los datos de cada vértice (Id -> Vertex)
        private readonly Dictionary<string, Vertex> vertices;

        public Graph()
        {
            listaAdyacencia = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            vertices = new Dictionary<string, Vertex>(StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyDictionary<string, List<string>> ObtenerListaAdyacencia()
        {
            return listaAdyacencia;
        }

        public IReadOnlyDictionary<string, Vertex> ObtenerVertices()
        {
            return vertices;
        }

        public Vertex? ObtenerUsuario(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            string clave = NormalizarId(id);
            return vertices.TryGetValue(clave, out Vertex? usuario) ? usuario : null;
        }

        private string NormalizarId(string id)
        {
            return id.Trim().ToUpperInvariant();
        }

        private bool EsRolValido(string rol)
        {
            string r = rol.Trim();
            return string.Equals(r, "Estudiante", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(r, "Profesor", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(r, "Egresado", StringComparison.OrdinalIgnoreCase);
        }

        private string NormalizarRol(string rol)
        {
            string r = rol.Trim().ToLowerInvariant();
            return char.ToUpperInvariant(r[0]) + r.Substring(1);
        }

        // ==================== OPERACIONES CRUD DE VÉRTICES (USUARIOS) ====================

        public bool AgregarUsuario(string id, string nombre, string rol, out string detalle)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(rol))
            {
                detalle = "Error: El ID, nombre y rol del usuario son obligatorios.";
                return false;
            }

            if (!EsRolValido(rol))
            {
                detalle = $"Error: El rol '{rol}' no es válido. Use Estudiante, Profesor o Egresado.";
                return false;
            }

            string clave = NormalizarId(id);

            if (vertices.ContainsKey(clave))
            {
                detalle = $"Rechazado: Ya existe un usuario registrado con el ID '{clave}' (duplicados prohibidos).";
                return false;
            }

            Vertex nuevoUsuario = new Vertex(clave, nombre, NormalizarRol(rol));
            vertices[clave] = nuevoUsuario;
            listaAdyacencia[clave] = new List<string>();

            detalle = $"Usuario '{nuevoUsuario.Nombre}' ({nuevoUsuario.Rol}) registrado con ID '{clave}'.";
            return true;
        }

        public bool ActualizarUsuario(string id, string nuevoNombre, string nuevoRol, out string detalle)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(nuevoNombre) || string.IsNullOrWhiteSpace(nuevoRol))
            {
                detalle = "Error: El ID, nuevo nombre y nuevo rol no pueden estar vacíos.";
                return false;
            }

            if (!EsRolValido(nuevoRol))
            {
                detalle = $"Error: El rol '{nuevoRol}' no es válido. Use Estudiante, Profesor o Egresado.";
                return false;
            }

            string clave = NormalizarId(id);

            if (!vertices.TryGetValue(clave, out Vertex? usuarioExistente))
            {
                detalle = $"Error: No se encontró ningún usuario con ID '{clave}' para actualizar.";
                return false;
            }

            string datosAnteriores = $"{usuarioExistente.Nombre} ({usuarioExistente.Rol})";
            usuarioExistente.Nombre = nuevoNombre.Trim();
            usuarioExistente.Rol = NormalizarRol(nuevoRol);

            detalle = $"Usuario '{clave}' actualizado: '{datosAnteriores}' -> '{usuarioExistente.Nombre} ({usuarioExistente.Rol})'.";
            return true;
        }

        public bool EliminarUsuario(string id, out string detalle)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                detalle = "Error: El ID del usuario a eliminar no puede estar vacío.";
                return false;
            }

            string clave = NormalizarId(id);

            if (!vertices.TryGetValue(clave, out Vertex? usuarioEliminado))
            {
                detalle = $"Error: El usuario con ID '{clave}' no existe en el grafo.";
                return false;
            }

            int salientesEliminadas = listaAdyacencia[clave].Count;
            int entrantesEliminadas = 0;

            // Eliminar todas las aristas entrantes hacia este usuario en otros vértices
            foreach (var par in listaAdyacencia)
            {
                if (!string.Equals(par.Key, clave, StringComparison.OrdinalIgnoreCase))
                {
                    int removidas = par.Value.RemoveAll(dest => string.Equals(dest, clave, StringComparison.OrdinalIgnoreCase));
                    entrantesEliminadas += removidas;
                }
            }

            listaAdyacencia.Remove(clave);
            vertices.Remove(clave);

            detalle = $"Usuario '{clave}' ({usuarioEliminado.Nombre}) eliminado junto con {salientesEliminadas} relación(es) saliente(s) y {entrantesEliminadas} entrante(s).";
            return true;
        }

        // ==================== OPERACIONES CRUD DE ARISTAS (RELACIONES) ====================

        public bool AgregarRelacion(string idOrigen, string idDestino, out string detalle)
        {
            if (string.IsNullOrWhiteSpace(idOrigen) || string.IsNullOrWhiteSpace(idDestino))
            {
                detalle = "Error: Los identificadores de origen y destino no pueden estar vacíos.";
                return false;
            }

            string origen = NormalizarId(idOrigen);
            string destino = NormalizarId(idDestino);

            if (string.Equals(origen, destino, StringComparison.OrdinalIgnoreCase))
            {
                detalle = $"Rechazado: Un usuario no puede seguirse a sí mismo ({origen} -> {destino}).";
                return false;
            }

            if (!vertices.ContainsKey(origen))
            {
                detalle = $"Error: El usuario origen '{origen}' no existe en el grafo.";
                return false;
            }

            if (!vertices.ContainsKey(destino))
            {
                detalle = $"Error: El usuario destino '{destino}' no existe en el grafo.";
                return false;
            }

            List<string> seguidos = listaAdyacencia[origen];
            if (seguidos.Any(d => string.Equals(d, destino, StringComparison.OrdinalIgnoreCase)))
            {
                detalle = $"Rechazado: La relación de seguimiento {origen} -> {destino} ya existe (aristas duplicadas prohibidas).";
                return false;
            }

            seguidos.Add(destino);
            detalle = $"Relación registrada: {origen} ({vertices[origen].Nombre}) sigue a {destino} ({vertices[destino].Nombre}).";
            return true;
        }

        public bool EliminarRelacion(string idOrigen, string idDestino, out string detalle)
        {
            if (string.IsNullOrWhiteSpace(idOrigen) || string.IsNullOrWhiteSpace(idDestino))
            {
                detalle = "Error: Los identificadores de origen y destino son obligatorios.";
                return false;
            }

            string origen = NormalizarId(idOrigen);
            string destino = NormalizarId(idDestino);

            if (!listaAdyacencia.TryGetValue(origen, out List<string>? seguidos))
            {
                detalle = $"Error: El usuario origen '{origen}' no existe en el grafo.";
                return false;
            }

            int eliminadas = seguidos.RemoveAll(d => string.Equals(d, destino, StringComparison.OrdinalIgnoreCase));
            if (eliminadas == 0)
            {
                detalle = $"Error: No existe una relación dirigida desde '{origen}' hacia '{destino}'.";
                return false;
            }

            detalle = $"Relación eliminada: {origen} dejó de seguir a {destino}.";
            return true;
        }

        public List<Edge> ObtenerTodasLasAristas()
        {
            List<Edge> aristas = new List<Edge>();
            foreach (var par in listaAdyacencia)
            {
                foreach (string destino in par.Value)
                {
                    aristas.Add(new Edge(par.Key, destino));
                }
            }
            return aristas;
        }

        // ==================== RECORRIDOS (BFS Y DFS) ====================

        // Recorrido en anchura (BFS) desde un usuario específico usando Queue<string>
        public bool RecorridoBFS(string idInicio, out List<string> ordenVisita, out int cantidadAlcanzados, out string detalle)
        {
            ordenVisita = new List<string>();
            cantidadAlcanzados = 0;

            if (string.IsNullOrWhiteSpace(idInicio))
            {
                detalle = "El ID de inicio no puede estar vacío.";
                return false;
            }

            string inicio = NormalizarId(idInicio);
            if (!vertices.ContainsKey(inicio))
            {
                detalle = $"El usuario inicial '{inicio}' no existe en el grafo.";
                return false;
            }

            HashSet<string> visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Queue<string> cola = new Queue<string>();

            visitados.Add(inicio);
            cola.Enqueue(inicio);

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();
                Vertex u = vertices[actual];
                ordenVisita.Add($"{u.Id} ({u.Nombre})");

                foreach (string vecino in listaAdyacencia[actual])
                {
                    if (!visitados.Contains(vecino))
                    {
                        visitados.Add(vecino);
                        cola.Enqueue(vecino);
                    }
                }
            }

            cantidadAlcanzados = ordenVisita.Count;
            detalle = $"BFS completado desde '{inicio}': {cantidadAlcanzados} vértice(s) alcanzado(s).";
            return true;
        }

        // Recorrido en profundidad (DFS) completo sobre todo el grafo con detección de ciclos dirigidos
        public void RecorridoDFSCompleto(out List<string> ordenDescubrimiento, out List<string> ciclosDetectados)
        {
            ordenDescubrimiento = new List<string>();
            ciclosDetectados = new List<string>();

            // Estados: 0 = No visitado (Blanco), 1 = En recursión activa (Gris), 2 = Finalizado (Negro)
            Dictionary<string, int> estado = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            List<string> pilaCamino = new List<string>();
            HashSet<string> ciclosUnicos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string id in vertices.Keys)
            {
                estado[id] = 0;
            }

            foreach (string id in vertices.Keys)
            {
                if (estado[id] == 0)
                {
                    DfsVisitar(id, estado, pilaCamino, ordenDescubrimiento, ciclosDetectados, ciclosUnicos);
                }
            }
        }

        private void DfsVisitar(
            string actual,
            Dictionary<string, int> estado,
            List<string> pilaCamino,
            List<string> ordenDescubrimiento,
            List<string> ciclosDetectados,
            HashSet<string> ciclosUnicos)
        {
            estado[actual] = 1; // En progreso en la rama actual
            pilaCamino.Add(actual);

            Vertex u = vertices[actual];
            ordenDescubrimiento.Add($"{u.Id} ({u.Nombre})");

            foreach (string vecino in listaAdyacencia[actual])
            {
                if (estado[vecino] == 0)
                {
                    DfsVisitar(vecino, estado, pilaCamino, ordenDescubrimiento, ciclosDetectados, ciclosUnicos);
                }
                else if (estado[vecino] == 1)
                {
                    // Arista de retroceso (Back Edge) encontrada -> Ciclo dirigido detectado
                    int indiceInicio = pilaCamino.FindIndex(v => string.Equals(v, vecino, StringComparison.OrdinalIgnoreCase));
                    if (indiceInicio >= 0)
                    {
                        List<string> nodosCiclo = pilaCamino.GetRange(indiceInicio, pilaCamino.Count - indiceInicio);
                        nodosCiclo.Add(vecino);
                        string representacionCiclo = string.Join(" -> ", nodosCiclo);

                        if (ciclosUnicos.Add(representacionCiclo))
                        {
                            ciclosDetectados.Add(representacionCiclo);
                        }
                    }
                }
            }

            pilaCamino.RemoveAt(pilaCamino.Count - 1);
            estado[actual] = 2; // Finalizado
        }

        // ==================== CONSULTAS SOCIALES Y MÉTRICAS DE CONECTIVIDAD ====================

        public int CalcularGradoSalida(string id)
        {
            string clave = NormalizarId(id);
            return listaAdyacencia.TryGetValue(clave, out List<string>? seguidos) ? seguidos.Count : 0;
        }

        public int CalcularGradoEntrada(string id)
        {
            string clave = NormalizarId(id);
            if (!vertices.ContainsKey(clave))
            {
                return 0;
            }

            int contador = 0;
            foreach (var par in listaAdyacencia)
            {
                if (par.Value.Any(destino => string.Equals(destino, clave, StringComparison.OrdinalIgnoreCase)))
                {
                    contador++;
                }
            }
            return contador;
        }

        public List<string> ObtenerSeguidoresDe(string id)
        {
            string clave = NormalizarId(id);
            List<string> seguidores = new List<string>();

            foreach (var par in listaAdyacencia)
            {
                if (par.Value.Any(destino => string.Equals(destino, clave, StringComparison.OrdinalIgnoreCase)))
                {
                    seguidores.Add(par.Key);
                }
            }
            return seguidores;
        }

        // Usuarios sin seguidores (grado de entrada == 0)
        public List<Vertex> ObtenerUsuariosSinSeguidores()
        {
            List<Vertex> resultado = new List<Vertex>();
            foreach (var par in vertices)
            {
                if (CalcularGradoEntrada(par.Key) == 0)
                {
                    resultado.Add(par.Value);
                }
            }
            return resultado;
        }

        // Usuarios sin seguidos (grado de salida == 0 / nodos sumidero)
        public List<Vertex> ObtenerUsuariosSinSeguidos()
        {
            List<Vertex> resultado = new List<Vertex>();
            foreach (var par in vertices)
            {
                if (CalcularGradoSalida(par.Key) == 0)
                {
                    resultado.Add(par.Value);
                }
            }
            return resultado;
        }

        // Usuarios más influyentes (aquellos que alcanzan el grado de entrada máximo)
        public List<Vertex> ObtenerUsuariosMasInfluyentes(out int maxGradoEntrada)
        {
            maxGradoEntrada = 0;
            List<Vertex> influyentes = new List<Vertex>();

            if (vertices.Count == 0)
            {
                return influyentes;
            }

            foreach (string id in vertices.Keys)
            {
                int grado = CalcularGradoEntrada(id);
                if (grado > maxGradoEntrada)
                {
                    maxGradoEntrada = grado;
                }
            }

            foreach (var par in vertices)
            {
                if (CalcularGradoEntrada(par.Key) == maxGradoEntrada)
                {
                    influyentes.Add(par.Value);
                }
            }

            return influyentes;
        }

        // Obtiene los usuarios más influyentes junto con la lista de IDs de sus seguidores
        public List<(Vertex Usuario, List<string> Seguidores)> ObtenerDetalleUsuariosMasInfluyentes(out int maxGradoEntrada)
        {
            List<Vertex> influyentes = ObtenerUsuariosMasInfluyentes(out maxGradoEntrada);
            List<(Vertex Usuario, List<string> Seguidores)> detalle = new List<(Vertex, List<string>)>();

            foreach (Vertex u in influyentes)
            {
                detalle.Add((u, ObtenerSeguidoresDe(u.Id)));
            }

            return detalle;
        }

        // Obtiene los datos estructurados de la lista de adyacencia para su presentación en la vista
        public List<(Vertex Usuario, int GradoEntrada, int GradoSalida, List<string> DestinosEtiquetados)> ObtenerDetalleListaAdyacencia()
        {
            var resultado = new List<(Vertex Usuario, int GradoEntrada, int GradoSalida, List<string> DestinosEtiquetados)>();

            foreach (var par in vertices.OrderBy(v => v.Key))
            {
                Vertex u = par.Value;
                List<string> destinos = listaAdyacencia[u.Id];
                int gradoEntrada = CalcularGradoEntrada(u.Id);
                int gradoSalida = destinos.Count;

                List<string> etiquetasDestino = destinos
                    .Select(idDest =>
                    {
                        Vertex? vDest = ObtenerUsuario(idDest);
                        return vDest != null ? $"{vDest.Id} ({vDest.Nombre})" : idDest;
                    })
                    .ToList();

                resultado.Add((u, gradoEntrada, gradoSalida, etiquetasDestino));
            }

            return resultado;
        }

        // Ranking ordenado de influencia (grado de entrada descendente)
        public List<(Vertex Usuario, int GradoEntrada, int GradoSalida)> ObtenerTablaConectividad()
        {
            List<(Vertex Usuario, int GradoEntrada, int GradoSalida)> tabla = new List<(Vertex, int, int)>();
            foreach (var par in vertices)
            {
                tabla.Add((par.Value, CalcularGradoEntrada(par.Key), CalcularGradoSalida(par.Key)));
            }
            return tabla
                .OrderByDescending(x => x.GradoEntrada)
                .ThenByDescending(x => x.GradoSalida)
                .ThenBy(x => x.Usuario.Id)
                .ToList();
        }

        // Usuarios más activos (aquellos que alcanzan el grado de salida máximo)
        public List<Vertex> ObtenerUsuariosMasActivos(out int maxGradoSalida)
        {
            maxGradoSalida = 0;
            List<Vertex> activos = new List<Vertex>();

            if (vertices.Count == 0)
            {
                return activos;
            }

            foreach (string id in vertices.Keys)
            {
                int grado = CalcularGradoSalida(id);
                if (grado > maxGradoSalida)
                {
                    maxGradoSalida = grado;
                }
            }

            foreach (var par in vertices)
            {
                if (CalcularGradoSalida(par.Key) == maxGradoSalida)
                {
                    activos.Add(par.Value);
                }
            }

            return activos;
        }

        // Usuarios con grado de salida >= umbral (para verificar requisito de >= 4 seguidos)
        public List<(Vertex Usuario, int GradoSalida)> ObtenerUsuariosConSalidaMinima(int minimo)
        {
            List<(Vertex Usuario, int GradoSalida)> resultado = new List<(Vertex, int)>();
            foreach (var par in vertices)
            {
                int salida = CalcularGradoSalida(par.Key);
                if (salida >= minimo)
                {
                    resultado.Add((par.Value, salida));
                }
            }
            return resultado.OrderByDescending(x => x.GradoSalida).ToList();
        }

        // Verificar alcanzabilidad dirigida de A hacia B mediante BFS y reconstruir el camino más corto
        public bool VerificarAlcanzabilidadBFS(string idOrigen, string idDestino, out List<string> camino, out int saltos)
        {
            camino = new List<string>();
            saltos = 0;

            if (string.IsNullOrWhiteSpace(idOrigen) || string.IsNullOrWhiteSpace(idDestino))
            {
                return false;
            }

            string origen = NormalizarId(idOrigen);
            string destino = NormalizarId(idDestino);

            if (!vertices.ContainsKey(origen) || !vertices.ContainsKey(destino))
            {
                return false;
            }

            if (string.Equals(origen, destino, StringComparison.OrdinalIgnoreCase))
            {
                camino.Add($"{origen} ({vertices[origen].Nombre})");
                saltos = 0;
                return true;
            }

            Queue<string> cola = new Queue<string>();
            HashSet<string> visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> predecesor = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            visitados.Add(origen);
            cola.Enqueue(origen);

            bool encontrado = false;

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();
                if (string.Equals(actual, destino, StringComparison.OrdinalIgnoreCase))
                {
                    encontrado = true;
                    break;
                }

                foreach (string vecino in listaAdyacencia[actual])
                {
                    if (!visitados.Contains(vecino))
                    {
                        visitados.Add(vecino);
                        predecesor[vecino] = actual;
                        cola.Enqueue(vecino);
                    }
                }
            }

            if (!encontrado)
            {
                return false;
            }

            // Reconstruir ruta desde destino hasta origen
            string paso = destino;
            while (!string.Equals(paso, origen, StringComparison.OrdinalIgnoreCase))
            {
                camino.Add($"{paso} ({vertices[paso].Nombre})");
                paso = predecesor[paso];
            }
            camino.Add($"{origen} ({vertices[origen].Nombre})");
            camino.Reverse();

            saltos = camino.Count - 1;
            return true;
        }

        // ==================== MÉTRICAS GLOBALES DEL GRAFO ====================

        public int ContarVertices()
        {
            return vertices.Count;
        }

        public int ContarAristas()
        {
            int total = 0;
            foreach (var par in listaAdyacencia)
            {
                total += par.Value.Count;
            }
            return total;
        }

        // Densidad de un grafo dirigido simple: |E| / (|V| * (|V| - 1))
        public double CalcularDensidad()
        {
            int v = ContarVertices();
            if (v <= 1)
            {
                return 0.0;
            }
            return (double)ContarAristas() / (v * (v - 1));
        }

        public double CalcularGradoPromedio()
        {
            int v = ContarVertices();
            if (v == 0)
            {
                return 0.0;
            }
            return (double)ContarAristas() / v;
        }
    }
}