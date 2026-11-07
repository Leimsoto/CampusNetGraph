# Taller Integrador - Grafos Dirigidos (MVC en C#)
## Caso de Estudio: CampusNet - Red Social Académica

Proyecto de consola en C# (.NET 8.0) implementado bajo el patrón arquitectónico Modelo-Vista-Controlador (MVC). Modela una red social dirigida de seguimiento entre usuarios académicos (**CampusNet**), donde cada vértice representa un usuario (`Estudiante`, `Profesor` o `Egresado`) y cada arista dirigida ($A \to B$) representa que el usuario $A$ sigue al usuario $B$. Incluye lista de adyacencia, recorridos en anchura (BFS) y profundidad (DFS) con detección de ciclos dirigidos, consultas de influencia/conectividad y operaciones CRUD completas.

## Integrantes y Responsabilidades

| Integrante | Responsabilidades |
| :--- | :--- |
| Germán Alberto Colorado Moscoso | Implementación de la capa Model (`Vertex.cs`, `Edge.cs`, `Graph.cs`), estructura de lista de adyacencia (`Dictionary<string, List<string>>`), operaciones CRUD, recorridos BFS/DFS con detección de ciclos y cálculo de grados de entrada/salida. |
| Leimar Yesid Soto Olaya | Implementación de la capa View (`ConsolaView.cs`), capa Controller (`GraphController.cs`, `Program.cs`), presentación de listas de adyacencia, métricas sociales, pruebas de alcanzabilidad y redacción del `README.md`. |

## Estructura del Proyecto (Arquitectura MVC)

- **Models/**:
  - `Vertex.cs`: Representa cada usuario de la red académica (`Id`, `Nombre`, `Rol`: Estudiante, Profesor o Egresado).
  - `Edge.cs`: Representa una relación dirigida de seguimiento (`IdOrigen -> IdDestino`).
  - `Graph.cs`: Contiene la lista de adyacencia (`Dictionary<string, List<string>>`), validaciones de duplicados (vértices y aristas), algoritmos `RecorridoBFS`, `RecorridoDFSCompleto` (con detección de aristas de retroceso para identificar ciclos), consultas de grados de entrada/salida, alcanzabilidad BFS y operaciones CRUD sin referencias a `Console`.
- **Views/**:
  - `ConsolaView.cs`: Responsable exclusivamente de la presentación en consola a partir de los datos ya procesados por el controlador (impresión de la lista de adyacencia, trazabilidad de recorridos BFS/DFS, tablas de influencia/actividad, rutas de alcanzabilidad y métricas estructurales).
- **Controllers/**:
  - `GraphController.cs`: Orquesta en orden los casos de uso exigidos por la guía (construcción del grafo, 3 BFS, 1 DFS completo, consultas sociales, operaciones CRUD e informe de métricas finales).
- **Program.cs**: Punto de entrada que instancia e invoca únicamente al controlador (`GraphController`), sin lógica de negocio en `Main`.

## Requisitos e Instrucciones para Ejecutar

1. Tener instalado el SDK de .NET 8.0 (o superior).
2. Clonar el repositorio y ubicarse en la carpeta del proyecto:
   ```bash
   git clone https://github.com/Leimsoto/CampusNetGraph.git
   cd CampusNetGraph
   ```
3. Compilar y ejecutar directamente el proyecto:
   ```bash
   dotnet run
   ```

## Evidencia de Ejecución en Consola

```text
==========================================================================
     CAMPUSNET - RED SOCIAL ACADÉMICA DE GRAFOS DIRIGIDOS (MVC EN C#)     
==========================================================================

--------------------------------------------------------------------------
  1. CONSTRUCCIÓN DEL GRAFO DIRIGIDO (Mínimo 12 usuarios y 18 aristas)
--------------------------------------------------------------------------

  [ESTRUCTURA INICIAL DEL GRAFO (LISTA DE ADYACENCIA)]
  Total usuarios (|V|): 12 | Total relaciones dirigidas (|E|): 24
  ------------------------------------------------------------------------
  [U01] Dra. Elena Restrepo    (Profesor  ) [Ent:2 | Sal:5] -> U02 (Dr. Carlos Mejía), U03 (Sofía Valencia), U04 (Mateo Giraldo), U05 (Ing. Laura Patiño), U07 (Dra. Diana Londoño)
  [U02] Dr. Carlos Mejía       (Profesor  ) [Ent:1 | Sal:5] -> U03 (Sofía Valencia), U05 (Ing. Laura Patiño), U06 (Andrés Castaño), U08 (Camila Orozco), U10 (Ing. Felipe Salazar)
  [U03] Sofía Valencia         (Estudiante) [Ent:2 | Sal:4] -> U01 (Dra. Elena Restrepo), U04 (Mateo Giraldo), U07 (Dra. Diana Londoño), U09 (Julián Ríos)
  [U04] Mateo Giraldo          (Estudiante) [Ent:3 | Sal:1] -> U07 (Dra. Diana Londoño)
  [U05] Ing. Laura Patiño      (Egresado  ) [Ent:3 | Sal:1] -> U06 (Andrés Castaño)
  [U06] Andrés Castaño         (Estudiante) [Ent:2 | Sal:1] -> U08 (Camila Orozco)
  [U07] Dra. Diana Londoño     (Profesor  ) [Ent:4 | Sal:1] -> U09 (Julián Ríos)
  [U08] Camila Orozco          (Egresado  ) [Ent:2 | Sal:2] -> U05 (Ing. Laura Patiño), U10 (Ing. Felipe Salazar)
  [U09] Julián Ríos            (Estudiante) [Ent:2 | Sal:1] -> U10 (Ing. Felipe Salazar)
  [U10] Ing. Felipe Salazar    (Egresado  ) [Ent:3 | Sal:0] -> (Sin seguidos)
  [U11] Valentina Duque        (Estudiante) [Ent:0 | Sal:2] -> U01 (Dra. Elena Restrepo), U07 (Dra. Diana Londoño)
  [U12] Sebastián Henao        (Egresado  ) [Ent:0 | Sal:1] -> U04 (Mateo Giraldo)

--------------------------------------------------------------------------
  2. RECORRIDOS EN ANCHURA (3 BFS) Y EN PROFUNDIDAD (1 DFS COMPLETO)
--------------------------------------------------------------------------

  * Recorrido BFS desde [U01 - Dra. Elena Restrepo (Profesor)]:
    - Orden de visita:             U01 (Dra. Elena Restrepo) -> U02 (Dr. Carlos Mejía) -> U03 (Sofía Valencia) -> U04 (Mateo Giraldo) -> U05 (Ing. Laura Patiño) -> U07 (Dra. Diana Londoño) -> U06 (Andrés Castaño) -> U08 (Camila Orozco) -> U10 (Ing. Felipe Salazar) -> U09 (Julián Ríos)
    - Cantidad vértices alcanzados: 10

  * Recorrido BFS desde [U05 - Ing. Laura Patiño (Egresado)]:
    - Orden de visita:             U05 (Ing. Laura Patiño) -> U06 (Andrés Castaño) -> U08 (Camila Orozco) -> U10 (Ing. Felipe Salazar)
    - Cantidad vértices alcanzados: 4

  * Recorrido BFS desde [U12 - Sebastián Henao (Egresado)]:
    - Orden de visita:             U12 (Sebastián Henao) -> U04 (Mateo Giraldo) -> U07 (Dra. Diana Londoño) -> U09 (Julián Ríos) -> U10 (Ing. Felipe Salazar)
    - Cantidad vértices alcanzados: 5

  * Recorrido DFS Completo sobre el grafo:
    - Total descubiertos (12):
      U01 (Dra. Elena Restrepo) -> U02 (Dr. Carlos Mejía) -> U03 (Sofía Valencia) -> U04 (Mateo Giraldo) -> U07 (Dra. Diana Londoño) -> U09 (Julián Ríos) -> U10 (Ing. Felipe Salazar) -> U05 (Ing. Laura Patiño) -> U06 (Andrés Castaño) -> U08 (Camila Orozco) -> U11 (Valentina Duque) -> U12 (Sebastián Henao)

    - Identificación de ciclos dirigidos (2 detectado(s)):
      [Ciclo #1]: U01 -> U02 -> U03 -> U01
      [Ciclo #2]: U05 -> U06 -> U08 -> U05

--------------------------------------------------------------------------
  3. CONSULTAS SOCIALES Y MÉTRICAS DE INFLUENCIA / ALCANZABILIDAD
--------------------------------------------------------------------------

  * Usuarios sin seguidores (Grado de entrada = 0):
    - [U11] Valentina Duque (Estudiante) -> 0 seguidores
    - [U12] Sebastián Henao (Egresado) -> 0 seguidores

  * Usuarios más influyentes (Mayor grado de entrada = 4 seguidores):
    - [U07] Dra. Diana Londoño (Profesor) | Seguido por: U01, U03, U04, U11

  * Usuarios más activos (Mayor grado de salida = 5 seguidos):
    - [U01] Dra. Elena Restrepo (Profesor) -> Sigue a 5 usuarios
    - [U02] Dr. Carlos Mejía (Profesor) -> Sigue a 5 usuarios

  * Usuarios con grado de salida >= 4 (3 usuarios):
    - [U01] Dra. Elena Restrepo (Profesor) -> Grado de salida: 5
    - [U02] Dr. Carlos Mejía (Profesor) -> Grado de salida: 5
    - [U03] Sofía Valencia (Estudiante) -> Grado de salida: 4

  * Verificación de alcanzabilidad dirigida (A -> B) mediante BFS:
    [ALCANZABLE]    U01 (Dra. Elena Restrepo) -> U10 (Ing. Felipe Salazar) | Saltos: 2 | Ruta BFS: U01 (Dra. Elena Restrepo) -> U02 (Dr. Carlos Mejía) -> U10 (Ing. Felipe Salazar)
    [ALCANZABLE]    U11 (Valentina Duque) -> U06 (Andrés Castaño) | Saltos: 3 | Ruta BFS: U11 (Valentina Duque) -> U01 (Dra. Elena Restrepo) -> U02 (Dr. Carlos Mejía) -> U06 (Andrés Castaño)
    [NO ALCANZABLE] U05 (Ing. Laura Patiño) -/-> U01 (Dra. Elena Restrepo) | No existe camino dirigido entre ambos usuarios.
    [NO ALCANZABLE] U10 (Ing. Felipe Salazar) -/-> U03 (Sofía Valencia) | No existe camino dirigido entre ambos usuarios.

--------------------------------------------------------------------------
  4. OPERACIONES CRUD SOBRE EL GRAFO (Con impresión tras cada cambio)
--------------------------------------------------------------------------

-> 4.1. Agregar nuevos usuarios ('U13', 'U14') y registrar relaciones dirigidas:
  [OK]        InsertarUsuario(U13)   | Usuario 'Natalia Gómez' (Estudiante) registrado con ID 'U13'.
  [OK]        InsertarUsuario(U14)   | Usuario 'Dr. Mauricio Pardo' (Profesor) registrado con ID 'U14'.
  [OK]        Seguir(U13 -> U01)     | Relación registrada: U13 (Natalia Gómez) sigue a U01 (Dra. Elena Restrepo).
  [OK]        Seguir(U04 -> U14)     | Relación registrada: U04 (Mateo Giraldo) sigue a U14 (Dr. Mauricio Pardo).
  [OK]        Seguir(U14 -> U10)     | Relación registrada: U14 (Dr. Mauricio Pardo) sigue a U10 (Ing. Felipe Salazar).

-> 4.2. Actualizar nombre y rol de usuario existente ('U03' pasa a Egresado):
  [OK]        ActualizarUsuario(U03) | Usuario 'U03' actualizado: 'Sofía Valencia (Estudiante)' -> 'Ing. Sofía Valencia (Egresado)'.

-> 4.3. Eliminar relación dirigida ('U02' deja de seguir a 'U08'):
  [OK]        EliminarRelacion       | Relación eliminada: U02 dejó de seguir a U08.

-> 4.4. Eliminar usuario ('U14') y limpiar sus aristas entrantes y salientes:
  [OK]        EliminarUsuario(U14)   | Usuario 'U14' (Dr. Mauricio Pardo) eliminado junto con 1 relación(es) saliente(s) y 1 entrante(s).

  [GRAFO FINAL TRAS ELIMINAR USUARIO U14]
  Total usuarios (|V|): 13 | Total relaciones dirigidas (|E|): 24
  ------------------------------------------------------------------------
  [U01] Dra. Elena Restrepo    (Profesor  ) [Ent:3 | Sal:5] -> U02 (Dr. Carlos Mejía), U03 (Ing. Sofía Valencia), U04 (Mateo Giraldo), U05 (Ing. Laura Patiño), U07 (Dra. Diana Londoño)
  [U02] Dr. Carlos Mejía       (Profesor  ) [Ent:1 | Sal:4] -> U03 (Ing. Sofía Valencia), U05 (Ing. Laura Patiño), U06 (Andrés Castaño), U10 (Ing. Felipe Salazar)
  [U03] Ing. Sofía Valencia    (Egresado  ) [Ent:2 | Sal:4] -> U01 (Dra. Elena Restrepo), U04 (Mateo Giraldo), U07 (Dra. Diana Londoño), U09 (Julián Ríos)
  [U04] Mateo Giraldo          (Estudiante) [Ent:3 | Sal:1] -> U07 (Dra. Diana Londoño)
  [U05] Ing. Laura Patiño      (Egresado  ) [Ent:3 | Sal:1] -> U06 (Andrés Castaño)
  [U06] Andrés Castaño         (Estudiante) [Ent:2 | Sal:1] -> U08 (Camila Orozco)
  [U07] Dra. Diana Londoño     (Profesor  ) [Ent:4 | Sal:1] -> U09 (Julián Ríos)
  [U08] Camila Orozco          (Egresado  ) [Ent:1 | Sal:2] -> U05 (Ing. Laura Patiño), U10 (Ing. Felipe Salazar)
  [U09] Julián Ríos            (Estudiante) [Ent:2 | Sal:1] -> U10 (Ing. Felipe Salazar)
  [U10] Ing. Felipe Salazar    (Egresado  ) [Ent:3 | Sal:0] -> (Sin seguidos)
  [U11] Valentina Duque        (Estudiante) [Ent:0 | Sal:2] -> U01 (Dra. Elena Restrepo), U07 (Dra. Diana Londoño)
  [U12] Sebastián Henao        (Egresado  ) [Ent:0 | Sal:1] -> U04 (Mateo Giraldo)
  [U13] Natalia Gómez          (Estudiante) [Ent:0 | Sal:1] -> U01 (Dra. Elena Restrepo)

==========================================================================
              MÉTRICAS DE CONECTIVIDAD Y ANÁLISIS ESTRUCTURAL             
==========================================================================
  * Total de usuarios (vértices |V|):       13
  * Total de seguimientos (aristas |E|):    24
  * Densidad del grafo dirigido:            0,1538 (15,38%)
  * Grado promedio (entrada/salida):        1,85 relaciones/usuario
  * Mayor grado de entrada (influencia):    4 (U07)
  * Mayor grado de salida (actividad):      5 (U01)
  * Nodos fuente (grado entrada = 0):       3 (U11, U12, U13)
  * Nodos sumidero (grado salida = 0):      1 (U10)
  * Ciclos dirigidos activos:               2
    - U01 -> U02 -> U03 -> U01
    - U05 -> U06 -> U08 -> U05
==========================================================================
```

## Análisis y Justificación de Resultados

1. **Asimetría del grafo dirigido**: El grado de entrada ($\text{deg}^-(v)$) mide la **influencia** académica (cuántos usuarios siguen a $v$), destacando a `U07 (Dra. Diana Londoño)` con 4 seguidores. En contraste, el grado de salida ($\text{deg}^+(v)$) mide la **actividad** en la red, liderada por `U01 (Dra. Elena Restrepo)` con 5 seguidos.
2. **Alcance en BFS y nodos fuente/sumidero**: Desde `U01` el recorrido en anchura alcanza 10 vértices porque forma parte del núcleo fuertemente conectado (`U01 -> U02 -> U03 -> U01`), sin alcanzar únicamente a `U11` y `U12` por tener grado de entrada 0 (nodos fuente). En cambio, desde `U05` solo se alcanzan 4 vértices (`U05, U06, U08, U10`) ya que esa subcomunidad no posee aristas salientes de regreso hacia el núcleo principal, y `U10` actúa como nodo sumidero ($\text{deg}^+(U10) = 0$).
3. **Detección de ciclos con DFS**: Mediante el seguimiento de estados en la pila de recursión (aristas de retroceso / *back edges*), el recorrido DFS identifica los dos circuitos cerrados de retroalimentación académica: `U01 -> U02 -> U03 -> U01` y `U05 -> U06 -> U08 -> U05`.
