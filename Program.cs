using CampusNetGraph.Controllers;

namespace CampusNetGraph
{
    class Program
    {
        static void Main(string[] args)
        {
            GraphController controlador = new GraphController();
            controlador.IniciarSimulacion();
        }
    }
}
