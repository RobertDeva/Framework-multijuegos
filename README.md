# Framework Multijuegos

Framework modular para desarrollar juegos de distintos géneros y universos a partir de un núcleo lógico reutilizable.

El desarrollo comienza con una extracción controlada de componentes genéricos desde [Game-Logic-motor](https://github.com/RobertDeva/Game-Logic-motor). Ese repositorio permanece como proyecto de referencia y no será modificado por esta migración.

## Estado

La solución .NET 8 tiene Core, Runtime y un primer módulo opcional de recursos. Se han extraído identidad, resultados/errores, tiempo lógico, reloj manual, aleatoriedad determinista, ciclo de vida técnico, definiciones de acciones y condiciones. Runtime ejecuta múltiples postacciones anidadas y despacha eventos en cascadas FIFO deterministas mediante handlers explícitos y acotados. Devuelve intenciones de efecto, pero todavía no las aplica.

La CI del commit `f5bf6f7e19a5dc518266d7d27d504bee4b45e306` completó restore, build y test: **50 pruebas aprobadas, 0 warnings y 0 errores** (Core 23, Resources 9, Runtime 18). El PR #1 sigue abierto en borrador y no se ha integrado en `main`.

## Documentación

- [Arquitectura objetivo](docs/architecture/overview.md)
- [Inventario resumido](docs/migration/source-inventory.md)
- [Inventario detallado archivo por archivo](docs/migration/source-inventory-detailed.md)
- [Dependencias y referencias entre proyectos](docs/dependencies.md)
- [Ejecución de acciones en Runtime](docs/runtime/action-execution.md)
- [Despacho de eventos en Runtime](docs/runtime/event-dispatch.md)
- [Hoja de ruta y criterios de avance](docs/roadmap.md)
- [Decisiones arquitectónicas](docs/adr/)
