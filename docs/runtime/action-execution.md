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

## Qué no está implementado todavía

El pipeline no despacha eventos, no aplica efectos y no gestiona efectos persistentes. Faltan los contratos de suscripción/entrega, el modelo de duración/stacking/expiración, la aplicación de efectos y sus pruebas de integración. No debe describirse como un sistema completo de eventos o efectos hasta que esas etapas existan.
