# Framework Multijuegos

Framework modular para desarrollar juegos de distintos géneros y universos a partir de un núcleo lógico reutilizable.

El desarrollo comienza con una extracción controlada de componentes genéricos desde [Game-Logic-motor](https://github.com/RobertDeva/Game-Logic-motor). Ese repositorio permanece como proyecto de referencia y no será modificado por esta migración.

## Estado

La solución .NET 8 tiene Core, Runtime y un primer módulo opcional de recursos. Se han extraído identidad, resultados/errores, tiempo lógico, reloj manual, aleatoriedad determinista, ciclo de vida técnico, definiciones de acciones y condiciones. Runtime ejecuta múltiples postacciones anidadas con límites y devuelve eventos/intenciones de efecto ordenados.

La CI del commit de código `4e41a68977db414efdd9b9fe15cf9a2999c5c6bb` completó restore, build y test: **42 pruebas aprobadas, 0 warnings y 0 errores**. Las actualizaciones documentales posteriores también han pasado restore, build y test en CI. El PR sigue en borrador y no se ha integrado en `main`.

## Documentación

- [Arquitectura objetivo](docs/architecture/overview.md)
- [Inventario resumido](docs/migration/source-inventory.md)
- [Inventario detallado archivo por archivo](docs/migration/source-inventory-detailed.md)
- [Dependencias y referencias entre proyectos](docs/dependencies.md)
- [Ejecución de acciones en Runtime](docs/runtime/action-execution.md)
- [Hoja de ruta y criterios de avance](docs/roadmap.md)
- [Decisiones arquitectónicas](docs/adr/)
