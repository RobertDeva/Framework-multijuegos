# Runtime: ejecución de acciones

## Componentes

- `ActionInvocation<TContext>`: ID de acción y contexto explícito.
- `ActionRegistration<TContext,TEvent,TEffect>`: une una definición Core con un handler.
- `IActionHandler<TContext,TEvent,TEffect>`: planifica outputs a partir del contexto.
- `ActionHandlerResult`: postacciones, eventos e intenciones de efecto, o un error.
- `ActionExecutionPipeline`: valida precondiciones y recorre el árbol de acciones.
- `ActionExecutionLimits`: límites de profundidad e invocaciones.
- `ActionExecutionReport`: traza de intentos, outputs o fallo.

## Orden y anidamiento

El pipeline ejecuta profundidad primero. Primero acumula los eventos/efectos del handler padre y después recorre sus postacciones en el orden declarado. Cada postacción puede devolver varias postacciones propias, sin una cantidad fija de niveles, siempre dentro de los límites configurados.

Un mismo ActionId puede aparecer en dos ramas hermanas. Se considera ciclo si vuelve a aparecer en la ruta activa de ancestros.

## Límites de seguridad

- Profundidad de la raíz: 0.
- Máxima profundidad configurable: 64.
- Máximo de invocaciones configurable: 10 000.
- Una acción no registrada falla.
- Las precondiciones se validan antes de llamar al handler.
- Excepciones de condición/handler se convierten en errores estables sin exponer mensajes internos.
- Si cualquier nodo falla, los eventos y efectos acumulados no se publican en el reporte.

## Contrato de pureza

Los handlers deben ser deterministas y no mutar el contexto ni estado externo. Deben describir los cambios deseados como intenciones de efecto. El pipeline no puede revertir mutaciones externas hechas indebidamente por un handler.

## Composición con despacho de eventos

`ActionExecutionPipeline` no despacha por sí mismo los eventos que produce. Si `ActionExecutionReport.IsSuccess` es `true`, el consumidor puede pasar `ActionExecutionReport.Events` a `EventDispatchPipeline<TContext,TEvent,TEffect>`. Este segundo pipeline procesa el lote en FIFO, evalúa los handlers en orden de registro y devuelve eventos derivados e intenciones de efecto. Consulta [despacho de eventos](event-dispatch.md) para sus límites y garantías.

## Qué no está implementado todavía

Runtime todavía no aplica efectos ni gestiona su ciclo de vida. Faltan el contrato de aplicación, duración, stacking, expiración, retirada por fuente y las garantías transaccionales de esa etapa. Tampoco hay pruebas de integración de la cadena completa con un juego cliente. Los pipelines de acciones y eventos solo publican outputs planificados si su propia ejecución completa tiene éxito; no pueden revertir mutaciones externas realizadas por handlers que incumplan el contrato de pureza.
