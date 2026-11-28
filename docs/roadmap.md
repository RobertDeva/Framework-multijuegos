# Hoja de ruta

## Fase 0 — Base verificable (completada)
- Reinspección de origen y destino.
- Inventario resumido y detallado del repositorio fuente.
- Solución .NET 8, Core inicial, pruebas y CI.
- Restore, build y test verificados en GitHub Actions.

## Fase 1 — Fundamentos (completada y validada)
- [x] EntityId, Result<T>, GameError.
- [x] SimulationDuration, SimulationInstant, IGameClock y ManualGameClock.
- [x] IRandomSource y generador determinista.
- [x] Ciclo de vida técnico Active/Removed mediante EntityBase.
- [x] ActionId, ActionDefinition y evaluación de condiciones.
- [x] Pruebas unitarias de invariantes y errores.
- CI exitosa en commit `e73cea1e5c37b9f994645c9c32d5bdf997c9bc39`.

## Fase 2 — Runtime (primera vertical slice implementada y validada)
- [x] Registro explícito de definiciones y handlers.
- [x] Ejecución de múltiples postacciones anidadas en profundidad primero y orden determinista.
- [x] Detección de ciclos por ActionId en la ruta de ancestros.
- [x] Límites configurables de profundidad e invocaciones con máximos duros.
- [x] Acumulación ordenada de eventos e intenciones de efecto; se publican solo si todo el árbol tiene éxito.
- [x] Fallos de precondición/handler y excepciones convertidos a errores estables.
- [x] Despacho explícito de eventos con filtros/handlers registrados, orden determinista, cola FIFO y límites de cascada.
- [ ] Aplicación y modelo/ciclo de vida de efectos instantáneos/persistentes: origen, objetivo, duración, stacking, expiración y retirada.
- [ ] Pruebas de integración de acciones + eventos + efectos con un módulo/juego real.
- CI exitosa en commit `4e41a68977db414efdd9b9fe15cf9a2999c5c6bb`.

## Fase 3 — Módulos compartidos
- [x] Primer módulo opcional Resources: ResourceId y ResourcePool como estado acotado/inmutable.
- [ ] Estadísticas genéricas: falta evaluador efectivo, lifecycle y políticas de stacking.
- [ ] Combate configurable como módulo opcional.
- [ ] Pruebas con reglas distintas de Mythical Exodus.
- CI exitosa para el módulo Resources en commit `0819da1ad7e40b3d22af2992818c509f3566ed3f`.

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
