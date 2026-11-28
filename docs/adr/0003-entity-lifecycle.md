# ADR 0003: ciclo de vida técnico mínimo de entidades

- Estado: aceptado para la primera versión del Core
- Fecha: 2026-11-27

## Contexto

El motor de referencia define `DomainEntity` con identidad estable y dos estados técnicos: `Active` y `Removed`. La retirada es idempotente. La documentación del propio motor separa expresamente la participación en la simulación de estados de gameplay como muerte, aturdimiento o derrota.

## Decisión

Extraer el mecanismo como `EntityBase` y `EntityLifecycleState` dentro de `FrameworkMultijuegos.Core.Entities`. La entidad mantiene identidad inmutable; `Remove()` cambia una sola vez de Active a Removed y devuelve si produjo una transición. La igualdad entre entidades se basa en `EntityId`.

## Consecuencias

- El Core no define muerte, destrucción, derrota, respawn, pooling ni persistencia.
- Los juegos pueden componer otra estrategia de ciclo de vida si Active/Removed no les basta.
- La herencia de `EntityBase` es una comodidad, no una obligación del framework.
- Cualquier transición adicional debe justificarse con un cliente real y pruebas.

## Validación

Pruebas para estado inicial, retirada idempotente, identidad/equivalencia y rechazo de identificador vacío en `EntityBaseTests`.
