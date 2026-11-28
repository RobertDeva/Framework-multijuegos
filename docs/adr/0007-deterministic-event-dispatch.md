# ADR 0007: despacho explícito y determinista de eventos

- Estado: aceptado para la primera vertical slice del Runtime.
- Fecha: 2026-11-28.

## Contexto

El pipeline de acciones puede producir eventos e intenciones de efecto ordenados, pero no existía una etapa para entregar esos eventos a lógica reactiva. Un bus global o una infraestructura de suscripciones dinámicas añadiría ciclo de vida, concurrencia y estado ambiental antes de tener casos de uso que lo justifiquen.

## Decisión

Introducir `EventDispatchPipeline<TContext,TEvent,TEffect>` dentro de Runtime con:

1. lote inicial explícito;
2. registro explícito e inmutable de handlers;
3. filtro `CanHandle` y handler `Handle` por contrato;
4. evaluación en orden de registro;
5. cola FIFO para eventos derivados;
6. límites configurables de eventos programados, evaluaciones e intenciones de efecto;
7. outputs de eventos/efectos publicados solo si toda la cascada termina con éxito.

Los handlers son planificadores puros: no mutan el contexto ni estado externo. El pipeline conserva una traza de eventos procesados al fallar, pero descarta los outputs planificados.

## Consecuencias

- La secuencia es reproducible si el contexto y los handlers cumplen el contrato determinista.
- El Runtime no necesita conocer tipos de eventos de un juego.
- Los juegos pueden definir filtros y handlers específicos sin introducir sus reglas en Core.
- No se promete rollback de mutaciones externas; el contrato puro es una obligación del handler, no una transacción impuesta por el framework.
- La aplicación/lifecycle de efectos sigue siendo una fase separada y pendiente.
- No se incorporan todavía bus global, concurrencia, persistencia ni event sourcing.

## Alternativas descartadas por ahora

- Bus global/ambiental: oculta dependencias y complica pruebas y orden.
- Suscripciones mutables durante el dispatch: requieren reglas de ciclo de vida y concurrencia no justificadas todavía.
- Ejecutar efectos directamente desde handlers: mezcla planificación y mutación, y hace imposible descartar outputs de forma segura ante un fallo posterior.
