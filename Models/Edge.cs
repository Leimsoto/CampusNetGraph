using System;

namespace CampusNetGraph.Models
{
    // Representa una relación dirigida de seguimiento (Origen sigue a Destino: A -> B)
    public class Edge
    {
        public string IdOrigen { get; set; }
        public string IdDestino { get; set; }

        public Edge(string idOrigen, string idDestino)
        {
            IdOrigen = idOrigen.Trim().ToUpperInvariant();
            IdDestino = idDestino.Trim().ToUpperInvariant();
        }

        public override string ToString()
        {
            return $"{IdOrigen} -> {IdDestino}";
        }
    }
}
