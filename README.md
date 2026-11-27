# Framework Multijuegos

Framework modular para desarrollar juegos de distintos géneros y universos a partir de un núcleo lógico reutilizable.

El desarrollo comienza con una extracción controlada de componentes genéricos desde [Game-Logic-motor](https://github.com/RobertDeva/Game-Logic-motor). Ese repositorio permanece como proyecto de referencia y no será modificado por esta migración.

## Estado

La solución .NET 8, el Core inicial, las pruebas y la CI están creados en la rama de trabajo. Se han extraído identidad, resultados/errores, tiempo lógico, reloj manual, aleatoriedad determinista, ciclo de vida técnico, definiciones de acciones y condiciones. La última extracción está pendiente de validación CI.

## Documentación

- [Arquitectura objetivo](docs/architecture/overview.md)
- [Inventario resumido](docs/migration/source-inventory.md)
- [Inventario detallado archivo por archivo](docs/migration/source-inventory-detailed.md)
- [Hoja de ruta y criterios de avance](docs/roadmap.md)
- [Decisiones arquitectónicas](docs/adr/)
