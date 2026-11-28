# Runtime: despacho determinista de eventos

## Responsabilidad

`EventDispatchPipeline<TContext,TEvent,TEffect>` consume un lote explícito de eventos y coordina sus handlers registrados. No depende de un bus global, un motor gráfico ni un modelo de eventos concreto.

Cada handler implementa:

- `CanHandle(context, event)`: filtro determinista para decidir si participa.
- `Handle(context, event)`: devuelve nuevos eventos e intenciones de efecto, o un error.

Los handlers se evalúan en el orden en que fueron registrados. Todos reciben la oportunidad de filtrar cada evento; solo se ejecuta `Handle` cuando `CanHandle` devuelve `true`. Se permiten registros repetidos si el juego necesita varios suscriptores con la misma implementación.

## Orden de entrega

1. El pipeline copia el lote inicial.
2. Procesa los eventos mediante una cola FIFO.
3. Para cada evento, evalúa los handlers en orden de registro.
4. Los eventos nuevos se añaden al final de la cola.
5. Los efectos se acumulan en el orden de handler y de emisión.

Por ello, todos los eventos iniciales pendientes se procesan antes de los eventos derivados que se hayan añadido después a la cola. La secuencia es determinista para el mismo lote, contexto, registro y handlers deterministas.

## Límites y errores

- Máximo predeterminado: 1.024 eventos programados por cascada (lote inicial + derivados).
- Máximo predeterminado: 10.000 evaluaciones de handlers.
- Máximo predeterminado: 10.000 intenciones de efecto.
- Techos duros configurables: 10.000 eventos, 100.000 evaluaciones y 100.000 intenciones de efecto.
- Se informan fallos de filtro, handler, límite de eventos y límite de invocaciones con códigos estables.
- `DispatchedEvents` conserva la traza hasta el punto del fallo.
- Si cualquier handler falla, `FollowUpEvents` y `Effects` quedan vacíos en el reporte.

Los límites se comprueban antes de ampliar la cola y antes de acumular outputs, por lo que también acotan el tamaño de la cascada pendiente. Los resultados de handler limitan además sus snapshots individuales. No se deduplican eventos: dos eventos iguales pueden ser legítimos.

## Pureza y garantías

Los handlers y sus filtros deben ser deterministas y no mutar el contexto, el evento recibido ni estado externo. Todos reciben el mismo contexto explícito de la llamada; el dispatcher no aplica cambios ni crea un contexto nuevo entre eventos. Deben devolver intenciones en vez de aplicar cambios directamente. Si una cascada necesita observar efectos intermedios, esa semántica debe definirse en la etapa de aplicación/orquestación de efectos.

El reporte solo publica los outputs planificados cuando toda la cascada termina correctamente. Esto es una garantía de planificación, **no un rollback transaccional**: el Runtime no puede revertir mutaciones externas que un handler haga incumpliendo el contrato. La aplicación posterior de efectos todavía debe definir sus propias garantías y límites transaccionales.

## Composición con acciones

Una composición inicial puede ejecutar un árbol con `ActionExecutionPipeline`, comprobar `ActionExecutionReport.IsSuccess` y, si tiene éxito, pasar `ActionExecutionReport.Events` a `EventDispatchPipeline`. El dispatcher devuelve efectos planificados y eventos derivados; no aplica efectos.

## Fuera de alcance

Todavía no se implementan:

- aplicación de efectos;
- duración, stacking, expiración y retirada por fuente de efectos persistentes;
- persistencia, replay o event sourcing;
- suscripciones dinámicas con altas/bajas durante la simulación;
- concurrencia, prioridades o semántica de orden entre hilos;
- rollback de estado del juego.

Estas capacidades requieren contratos y pruebas propios antes de considerarse parte del Runtime.
