# ADR 0005: ResourcePool como módulo opcional

- Estado: aceptado para el primer módulo compartido
- Fecha: 2026-11-27

## Contexto

El modelo de recursos de Mythical Exodus contiene un identificador estable y un estado inmutable con valor actual y máximo. El modelo valida sus límites, pero no define políticas de gasto, regeneración, coste, deuda, recarga ni desbordamiento.

## Decisión

Extraer `ResourceId` y `ResourcePool` como módulo opcional `FrameworkMultijuegos.Modules.Resources`, fuera del Core. Mantener su contrato deliberadamente pequeño: identidad, valor actual, máximo y operaciones de copia validadas (`WithCurrent`, `WithMaximum`).

El módulo no introduce reglas automáticas de consumo o regeneración. Tampoco fuerza una dependencia artificial del Core mientras no use sus tipos.

## Consecuencias

- Los juegos pueden usar el modelo sin incorporar el módulo al Core.
- Cada juego define costes, regeneración, recargas, prioridades y reglas de desbordamiento.
- Los tipos son estado inmutable, no un sistema completo de economía ni de recursos.
- Las futuras políticas se añadirán en otra capa solo cuando exista un caso de uso concreto.

## Validación

Las pruebas cubren identificador, límites, estado vacío, inmutabilidad de actualizaciones y rechazo de máximos inválidos.
