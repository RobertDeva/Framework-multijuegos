# Inventario de migración: Game-Logic-motor

Este documento resume la migración. Para el detalle archivo por archivo —responsabilidad, dependencias/acoplamientos, pruebas, clasificación y destino— consulta el [inventario completo](source-inventory-detailed.md).

**Fuente:** `RobertDeva/Game-Logic-motor`, rama `main`  
**HEAD verificado:** `788b4a5517a3c55dbc9780826ae0692f65af1949`  
**Árbol Git:** `f07f2d001065c0f1becc6219e3ca7cc5a0012cf1`  
**Destino:** `RobertDeva/Framework-multijuegos`

La reinspección confirmó que el HEAD de origen coincide con la auditoría anterior. El árbol contiene 129 archivos versionados: 56 archivos C# de producción, 31 de pruebas, 31 Markdown, 5 proyectos .NET y 6 archivos de configuración/solución.

## Fundamentos extraídos al Core

| Origen | Destino | Estado |
|---|---|---|
| EntityId, Result<T>, GameError | Core/Primitives | Extraídos y probados |
| SimulationDuration, SimulationInstant | Core/Time | Extraídos y probados |
| IGameClock, ManualGameClock | Core/Time | Extraídos y probados |
| IRandomSource, SeededRandomSource | Core/Randomness | Adaptados y probados |
| DomainEntity, EntityLifecycleState | Core/Entities | Extraídos como EntityBase/EntityLifecycleState |
| ActionId, ActionDefinition | Core/Actions | Extraídos; describen y validan, no ejecutan |
| IGameCondition, ConditionEvaluation, ConditionFailure, ConditionSet | Core/Conditions | Extraídos y probados |

## Módulos y Runtime

| Capacidad | Destino | Estado |
|---|---|---|
| ResourceId, ResourcePool | Modules.Resources | Extraídos como estado acotado/inmutable; sin política de gasto/regeneración |
| Ejecución de acciones y postacciones | Runtime.Actions | Implementada con profundidad-first, detección de ciclos, límites y outputs ordenados |
| Despacho de eventos | Runtime | Pendiente |
| Aplicación y ciclo de vida de efectos | Runtime | Pendiente |
| Estadísticas | Modules.Statistics | Pendiente; falta evaluador efectivo y lifecycle |
| Combate | Modules.Combat | Pendiente; separar parámetros configurables de reglas ME |

## Exclusiones de Mythical Exodus

Mantener fuera del Core: PlayerCharacter, PlayerClass, PlayerClassDefaults, CharacterAttribute, CharacterAttributes, MuertebenditaExecutionResolver, NemesisProgression, PlayerResurrectionState, ResurrectionState y las reglas concretas de energías, postura, ejecuciones, jefes y balance.

## Estado de la migración

- El repositorio fuente permanece intacto.
- El inventario detallado registra los 129 archivos versionados.
- La CI inicial del destino fue exitosa.
- La CI de fundamentos, Resources y Runtime fue exitosa.
- El último commit con código es `4e41a68977db414efdd9b9fe15cf9a2999c5c6bb`; 42 pruebas aprobadas, cero warnings y cero errores.
- HEAD actual de la rama, `7fb3c6fab5ce640e5e8440699d376c07c93700ce`, solo actualiza documentación y su CI de push/PR también fue exitosa.
- PR #1 continúa abierto como borrador; no se ha integrado en `main`.

El runtime actual no debe describirse como un sistema completo de eventos/efectos hasta implementar el despacho de eventos y la aplicación/lifecycle de efectos.
