using System;

namespace CampusNetGraph.Models
{
    // Representa un usuario académico (vértice) dentro de la red social CampusNet
    public class Vertex
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }

        public Vertex(string id, string nombre, string rol)
        {
            Id = id.Trim().ToUpperInvariant();
            Nombre = nombre.Trim();
            Rol = rol.Trim();
        }

        public string ObtenerEtiqueta()
        {
            return $"{Id} ({Nombre} - {Rol})";
        }

        public override string ToString()
        {
            return $"[{Id}] {Nombre} ({Rol})";
        }
    }
}
