# Inventario de migración: Game-Logic-motor

Este documento es el resumen arquitectónico de la migración. Para el detalle solicitado archivo por archivo —responsabilidad, dependencias/acoplamientos, pruebas, clasificación y destino— consulta el [inventario completo](source-inventory-detailed.md).

**Fuente inspeccionada:** `RobertDeva/Game-Logic-motor`  
**Rama:** `main`  
**HEAD verificado:** `788b4a5517a3c55dbc9780826ae0692f65af1949`  
**Árbol Git:** `f07f2d001065c0f1becc6219e3ca7cc5a0012cf1`  
**Destino:** `RobertDeva/Framework-multijuegos`

La reinspección confirmó que el HEAD de origen coincide con la auditoría anterior. El árbol contiene 129 archivos versionados: 56 archivos C# de producción, 31 de pruebas, 31 Markdown, 5 proyectos .NET y 6 archivos de configuración/solución.

## Fundamentos candidatos al Core

| Archivos/rutas de origen | Destino | Tratamiento |
|---|---|---|
| `Domain/Primitives/EntityId.cs` | Core/Primitives | Extraído como `EntityId` |
| `Domain/Primitives/Result.cs` | Core/Primitives | Extraídos `Result<T>` y `GameError` |
| `Domain/Primitives/SimulationDuration.cs`, `SimulationInstant.cs` | Core/Time | Extraídos como ticks lógicos |
| `Application/Time/IGameClock.cs` | Core/Time | Interfaz y reloj manual extraídos |
| `Domain/Abstractions/IRandomSource.cs` | Core/Randomness | Interfaz extraída |
| `Infrastructure/Randomness/SeededRandomSource.cs` | Core/Randomness | Adaptado como generador determinista no criptográfico |
| `Domain/Entities/DomainEntity.cs`, `EntityLifecycleState.cs` | Core/Entities | Extraídos como `EntityBase` y `EntityLifecycleState`; solo ciclo técnico |
| `Domain/Actions/ActionId.cs`, `ActionDefinition.cs` | Core/Actions | Extraídos; describen y validan, no ejecutan |
| `Domain/Conditions/*.cs` | Core/Conditions | Contratos y evaluación pura extraídos |

## Candidatos que requieren adaptación

| Archivos/rutas | Destino posible | Riesgo/condición |
|---|---|---|
| `Domain/Resources/ResourceId.cs`, `ResourcePool.cs` | Modules.Resources | Extraídos como estado acotado; políticas de gasto/regeneración siguen fuera |
| `Domain/Statistics/StatId.cs`, `StatValue.cs`, `StatModifier.cs`, `StatModifierOperation.cs`, `AttributeScalingCurve.cs` | Module.Statistics | Falta evaluador general de valor efectivo y política de orden/acumulación |
| Modelos de `Domain/Combat` | Module.Combat | Separar parámetros configurables de fórmulas y semántica de Mythical Exodus |
| `Application/Combat/NemesisWorldPresence.cs` | Posible módulo de presencia de actores | No extraer hasta validar un segundo caso de uso |

## Exclusiones específicas de Mythical Exodus

Mantener fuera del Core: `PlayerCharacter`, `PlayerClass`, `PlayerClassDefaults`, `CharacterAttribute`, `CharacterAttributes`, `MuertebenditaExecutionResolver`, `NemesisProgression`, `PlayerResurrectionState`, `ResurrectionState` y las reglas concretas de energías, postura, ejecuciones, jefes y balance.

## Estado de extracción

- Los fundamentos anteriores están en la rama `foundation/architecture-and-initial-extraction`.
- El ciclo de vida, acciones y condiciones se añadieron en la fase actual; la CI del último commit está pendiente de resultado.
- Resources ya es un módulo opcional independiente. Runtime tiene un primer pipeline de acciones/postacciones; despacho de eventos y lifecycle/aplicación de efectos siguen pendientes. Estadísticas y combate permanecen pendientes.
- No se ha modificado `Game-Logic-motor`.
