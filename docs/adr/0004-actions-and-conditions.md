# ADR 0004: definiciones de acciones y evaluación de condiciones

- Estado: aceptado para la primera versión del Core
- Fecha: 2026-11-27

## Contexto

El motor de referencia ya contiene identificadores de acción, definiciones con precondiciones y un conjunto de condiciones puras sobre un contexto explícito. No existe un pipeline general que ejecute acciones, produzca eventos o aplique efectos.

## Decisión

Extraer `ActionId`, `ActionDefinition<TContext>`, `IGameCondition<TContext>`, `ConditionSet<TContext>`, `ConditionEvaluation` y `ConditionFailure` a Core. Las condiciones se evalúan en orden de declaración, se evalúan todas (sin cortocircuito) y sus fallos se devuelven como snapshot de solo lectura.

Una definición de acción describe identidad y precondiciones; no ejecuta lógica ni muta estado. La ejecución se implementará posteriormente en Runtime mediante handlers explícitos.

## Consecuencias

- No se confunden definición, validación y ejecución.
- Los predicados reciben contexto explícito y deben ser deterministas y sin efectos laterales.
- La política de error de cada juego puede usar códigos estables sin introducir conceptos de gameplay en Core.
- Eventos, efectos y postacciones anidadas permanecen pendientes y no se infieren de estos contratos.

## Validación

Pruebas para ID válido, validación, evaluación vacía, orden de evaluación, acumulación de fallos, snapshot inmutable, entradas nulas y resultado nulo de una condición.
