# Hoja de ruta

## Fase 0 — Base verificable (completada en rama)
- Reinspección de origen y destino.
- Inventario resumido y detallado del repositorio fuente.
- Solución .NET 8, Core inicial, pruebas y CI.
- Restore, build y test verificados para el commit inicial de la rama.
- No se ha integrado en `main`.

## Fase 1 — Fundamentos (implementación en validación CI)
- [x] EntityId, Result<T>, GameError.
- [x] SimulationDuration, SimulationInstant, IGameClock y ManualGameClock.
- [x] IRandomSource y generador determinista.
- [x] Ciclo de vida técnico Active/Removed mediante EntityBase.
- [x] ActionId, ActionDefinition y evaluación de condiciones.
- [x] Pruebas unitarias para identidad, resultados, tiempo, aleatoriedad, entidades, acciones y condiciones.
- [ ] CI verde para el último commit de esta fase.
- [ ] Revisión final de contratos y documentación antes de cerrar la fase.

## Fase 2 — Runtime
- Definir pipeline de comandos/acciones y handlers.
- Definir eventos y sus límites de orden/entrega.
- Definir efectos instantáneos y persistentes: origen, objetivo, duración, stacking, expiración y retirada.
- Implementar postacciones múltiples y anidadas con límites explícitos de profundidad, número de operaciones, detección de ciclos, orden determinista y política de error.
- No introducir bus global hasta demostrar que la coordinación directa no basta.

## Fase 3 — Módulos compartidos
- Recursos y estadísticas genéricas.
- Combate configurable como módulo opcional.
- Pruebas con reglas distintas de Mythical Exodus.

## Fase 4 — Primer cliente
- Adaptador de Mythical Exodus y pruebas de paridad contra el motor de referencia.
- No reescribir el repositorio fuente durante la extracción.

## Fase 5 — Segundo cliente y herramientas
- Validar con un juego de Cielo de Cristal y, posteriormente, herramientas de creación geográfica/contenido cuando existan requisitos y datos validados.

## Criterios de aceptación por fase
1. El código compila con nullable y warnings como errores.
2. Las pruebas cubren invariantes, casos límite y errores esperados.
3. La CI ejecuta restore, build y test sobre la rama/PR.
4. Las dependencias y los límites Core/módulo/cliente están documentados.
5. El inventario y la matriz de extracción reflejan el estado real.
6. No se marca una fase completa hasta verificar todos los puntos aplicables.

Una fase puede estar implementada y seguir pendiente de validación; no se confunden ambos estados.
