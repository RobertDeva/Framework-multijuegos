# ADR 0006: ejecución acotada de acciones y postacciones

- Estado: aceptado como primera vertical slice de Runtime
- Fecha: 2026-11-27

## Contexto

Core ya define acciones y precondiciones, pero no ejecuta acciones. El motor fuente no tiene un pipeline general de eventos, efectos ni postacciones. Se necesita una primera capacidad que permita ejecutar acciones compuestas sin acoplar el framework a reglas de un juego.

## Decisión

Crear `FrameworkMultijuegos.Runtime` con un registro explícito de `ActionDefinition` + `IActionHandler` y un `ActionExecutionPipeline`.

Cada handler recibe un contexto explícito y devuelve una lista ordenada de postacciones, eventos e intenciones de efecto. El pipeline ejecuta postacciones en profundidad primero y en orden de declaración.

Protecciones:
- ciclo: un `ActionId` que reaparece en la ruta activa de ancestros se rechaza; repetir el mismo ID en ramas hermanas sí se permite;
- profundidad: raíz en profundidad cero; máximo configurable con techo duro de 64;
- invocaciones: máximo configurable con techo duro de 10 000;
- duplicados: no se permite registrar dos handlers para el mismo ID;
- fallos: precondiciones, errores de handler y excepciones se devuelven como fallos estables;
- atomicidad de salida: si cualquier nodo falla, el reporte no publica eventos ni efectos parciales.

Los handlers deben ser deterministas y no mutar contexto ni estado externo. Los cambios de estado se expresan como efectos para una fase de aplicación separada.

## Límites explícitos

Esta versión no implementa:
- despacho de eventos ni suscriptores;
- aplicación de efectos;
- duración, stacking, expiración o retirada de efectos persistentes;
- rollback de efectos externos, porque los handlers no deben producirlos;
- ejecución asíncrona o paralela.

No se introduce un bus global. El pipeline solo devuelve eventos/intenciones de efecto ordenados; el cliente decide cómo consumirlos después de que el árbol completo tenga éxito.

## Validación

Pruebas de orden depth-first, múltiples postacciones, anidamiento, repetición entre hermanos, ciclos, límites de profundidad/invocaciones, precondiciones, fallos parciales, excepciones, registros duplicados y acciones desconocidas.
