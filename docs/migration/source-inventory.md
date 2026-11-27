# Inventario de migración: Game-Logic-motor

**Fuente inspeccionada:** `RobertDeva/Game-Logic-motor`  
**Rama:** `main`  
**Commit verificado:** `788b4a5517a3c55dbc9780826ae0692f65af1949`  
**Destino:** `RobertDeva/Framework-multijuegos`

El inventario se basa en el árbol recursivo completo del commit fuente: 129 archivos versionados, incluidos código, pruebas y documentación. La tabla agrupa archivos por responsabilidad; las rutas concretas de cada grupo se enumeran para que la migración pueda verificarse. La clasificación es una propuesta técnica, no una decisión de que cada archivo deba copiarse.

## Fundamentos candidatos al Core

| Archivos/rutas de origen | Destino | Tratamiento |
|---|---|---|
| `Domain/Primitives/EntityId.cs` | Core/Primitives | Extraído como `EntityId`; verificar serialización y compatibilidad |
| `Domain/Primitives/Result.cs` | Core/Primitives | Extraídos `Result<T>` y `GameError`; evaluar variantes async y composición solo con casos de uso |
| `Domain/Primitives/SimulationDuration.cs`, `SimulationInstant.cs` | Core/Time | Extraídos con ticks lógicos; unidad de tick debe ser documentada por cada juego |
| `Application/Time/IGameClock.cs` | Core/Time | Extraída interfaz; `ManualGameClock` también movido al Core por ser determinista y no depender de infraestructura |
| `Domain/Abstractions/IRandomSource.cs` | Core/Randomness | Extraída interfaz |
| `Infrastructure/Randomness/SeededRandomSource.cs` | Core/Randomness | Extraído generador determinista; no es criptográfico |

## Candidatos que requieren adaptación antes de extraer

| Archivos/rutas | Destino posible | Riesgo/condición |
|---|---|---|
| `Domain/Entities/DomainEntity.cs`, `EntityLifecycleState.cs` | Core/Entities | El estado Active/Removed es genérico, pero la base mutable y la semántica de retirada deben contrastarse con identidad, persistencia y lifecycle de otros juegos |
| `Domain/Actions/ActionId.cs`, `ActionDefinition.cs` | Core/Actions | Contrato de definición y precondiciones genérico; no ejecuta efectos ni muta estado |
| `Domain/Conditions/IGameCondition.cs`, `ConditionEvaluation.cs`, `ConditionFailure.cs`, `ConditionSet.cs` | Core/Conditions | Evaluación pura y determinista; confirmar estrategia de errores y composición |
| `Domain/Resources/ResourceId.cs`, `ResourcePool.cs` | Module.Resources | Mecanismo reutilizable, pero políticas de regeneración, gasto, deuda y límites no están generalizadas |
| `Domain/Statistics/StatId.cs`, `StatValue.cs`, `StatModifier.cs`, `StatModifierOperation.cs`, `AttributeScalingCurve.cs` | Module.Statistics | Extraer modelo configurable; falta evaluador general de valor efectivo y política de orden/acumulación |

## Módulos de gameplay candidatos, no Core

- **Combate:** `AdaptiveEnemyCombat.cs`, `BossPhaseState.cs`, `CombatActionContext.cs`, `CombatActionRequest.cs`, `CombatActionValidator.cs`, `CombatImpactResult.cs`, `CombatResourceImpact.cs`, `CombatResourceImpactApplier.cs`, `CombatantResourceState.cs`, `CombatantSnapshot.cs`, `DefensiveDamageCalculator.cs`, `EnergyAffinityMatrix.cs`, `OffensiveDamageCalculator.cs`, `PerfectDeflect.cs`, `PosturePressureAndRecovery.cs`, `VitalityTrack.cs`. Requieren separar reglas configurables de fórmulas y semántica de Mythical Exodus.
- **Application/combate:** `INemesisProgressionRepository.cs`, `NemesisFinalExecutionCoordinator.cs`, `NemesisWorldPopulationCoordinator.cs`, `NemesisWorldPresence.cs`, `PlayerDefeatCoordinator.cs`, `PlayerDefeatOutcome.cs`, `PlayerDefeatProcessingResult.cs`. Mantener específicos de ME; el registro de presencia de actores podría inspirar una abstracción distinta tras validar otro uso.
- **Infrastructure:** `JsonNemesisProgressionRepository.cs` específico de ME; `SeededRandomSource.cs` extraído a Core.
- **Headless:** `Program.cs` es un host mínimo, no un runtime general.

## Exclusiones específicas de Mythical Exodus

Mantener fuera del Core: `CharacterAttribute.cs`, `CharacterAttributes.cs`, `PlayerCharacter.cs`, `PlayerClass.cs`, `PlayerClassDefaults.cs`, `MuertebenditaExecutionResolver.cs`, `NemesisProgression.cs`, `PlayerResurrectionState.cs`, `ResurrectionState.cs`, además de reglas concretas de energías, postura, ejecuciones, jefes y balance. Algunos algoritmos podrían convertirse en módulos parametrizables, pero solo tras especificación y pruebas separadas.

## Pruebas y documentación de origen

- Las pruebas del proyecto `MythicalExodus.Domain.Tests` deben conservarse como evidencia de comportamiento del cliente, no copiarse sin adaptar al framework.
- Los archivos `docs/*` contienen tanto decisiones técnicas como diseño y balance de ME. La auditoría y documentos de fundamentos sirven como referencia; GDD, progresión, Nemesis, resurrección y fórmulas específicas permanecen en el proyecto fuente.
- `MythicalExodus.sln`, `Directory.Build.props`, `global.json`, `.editorconfig`, `.gitignore` y CI son referencias de configuración, no deben copiarse a ciegas.

## Siguiente revisión

Antes de extraer entidades, acciones y condiciones, revisar sus pruebas y todas sus referencias. Antes de extraer módulos, definir contratos de configuración y políticas independientes de ME. Mantener una matriz de trazabilidad por archivo y commit.
