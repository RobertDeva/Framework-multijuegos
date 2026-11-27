# Inventario técnico detallado — Game-Logic-motor

**HEAD de origen verificado:** `788b4a5517a3c55dbc9780826ae0692f65af1949` (`main`)  
**Árbol Git:** `f07f2d001065c0f1becc6219e3ca7cc5a0012cf1`  
**Destino:** `RobertDeva/Framework-multijuegos`  
**Revisión:** 2026-11-27

Este documento detalla los 129 archivos versionados del origen. Los 56 archivos C# de producción y los 31 de pruebas se enumeran individualmente. También se inventarían los 31 Markdown, cinco proyectos .NET y seis archivos de configuración.

Clasificación: **Core** = mecanismo independiente de juego; **Módulo** = capacidad opcional que requiere contratos/políticas; **ME** = regla o semántica exclusiva de Mythical Exodus; **Host/Infra** = punto de entrada/adaptador; **Referencia** = prueba o documento que debe conservarse en el origen.

## A. Producción — 56 archivos C#

| Archivo | Responsabilidad | Dependencias y acoplamiento | Prueba en origen | Clasificación | Destino |
|---|---|---|---|---|---|
| `Application/Combat/INemesisProgressionRepository.cs` | Puerto de persistencia de progresión Nemesis | Domain.Combat, Domain.Primitives | JsonNemesisProgressionRepositoryTests | ME | Mantener en cliente ME |
| `Application/Combat/NemesisFinalExecutionCoordinator.cs` | Coordina remate final y recuperación de Nemesis | Domain.Combat, Domain.Primitives | NemesisFinalExecutionCoordinatorTests | ME | Mantener en cliente ME |
| `Application/Combat/NemesisWorldPopulationCoordinator.cs` | Decide población/presencia de Nemesis | Domain.Combat | NemesisWorldPopulationCoordinatorTests | ME | Mantener; no convertir la política Nemesis en población universal |
| `Application/Combat/NemesisWorldPresence.cs` | Registro en memoria de presencia de actores Nemesis | Domain.Primitives; contrato y registro propios | NemesisWorldPresenceRegistryTests | Módulo candidato con acoplamiento ME | No extraer aún; estudiar un contrato neutral de presencia con un segundo cliente |
| `Application/Combat/PlayerDefeatCoordinator.cs` | Orquesta derrota del jugador y consecuencias | Domain.Combat, Domain.Primitives | PlayerDefeatCoordinatorTests | ME | Mantener en cliente ME |
| `Application/Combat/PlayerDefeatOutcome.cs` | Describe resultado y crédito de derrota | Domain.Combat, Domain.Primitives | PlayerDefeatCoordinatorTests | ME | Mantener en cliente ME |
| `Application/Combat/PlayerDefeatProcessingResult.cs` | Resultado de procesar derrota | Domain.Combat | PlayerDefeatCoordinatorTests | ME | Mantener en cliente ME |
| `Application/Time/IGameClock.cs` | Contrato de reloj y reloj manual | Domain.Primitives.SimulationInstant/Duration | Sin prueba directa del reloj en origen | Core | Extraído a Core/Time; añadir cobertura específica del reloj |
| `Domain/Abstractions/IRandomSource.cs` | Contrato de aleatoriedad inyectable | Sin dependencia de juego | Sin prueba directa | Core | Extraído a Core/Randomness |
| `Domain/Actions/ActionDefinition.cs` | Define acción y valida precondiciones; no ejecuta | Domain.Conditions | ActionDefinitionTests | Core | Extraído a Core/Actions; el ejecutor queda en Runtime |
| `Domain/Actions/ActionId.cs` | Identificador estable de acción | Sin dependencia externa | ActionDefinitionTests | Core | Extraído a Core/Actions |
| `Domain/Characters/CharacterAttribute.cs` | Enum de atributos del personaje ME | CharacterAttributes, PlayerCharacter y fórmulas | CharacterAttributesTests, PlayerCharacterTests | ME | Excluir del Core |
| `Domain/Characters/CharacterAttributes.cs` | Ocho atributos y límites del sistema ME | CharacterAttribute | CharacterAttributesTests, PlayerCharacterTests | ME | Mantener en cliente; no universalizar el conjunto de atributos |
| `Domain/Characters/PlayerCharacter.cs` | Estado, clase y mejora del personaje jugador | DomainEntity, PlayerClassDefaults, CharacterAttributes, Result/GameError | PlayerCharacterTests | ME | Mantener en cliente ME |
| `Domain/Characters/PlayerClass.cs` | Clases de personaje ME | PlayerClassDefaults, PlayerCharacter | PlayerCharacterTests | ME | Mantener en cliente ME |
| `Domain/Characters/PlayerClassDefaults.cs` | Valores iniciales por clase | CharacterAttribute, CharacterAttributes, PlayerClass | PlayerCharacterTests | ME | Mantener en cliente ME |
| `Domain/Combat/AdaptiveEnemyCombat.cs` | Memoria, perfil, reglas y adaptación de patrones enemigos | Tipos internos del mismo modelo de combate | AdaptiveEnemyCombatTests | Módulo candidato con política ME | No extraer todavía; separar política y parámetros primero |
| `Domain/Combat/BossPhaseState.cs` | Estado y transiciones de fases de jefe | Domain.Primitives | BossPhaseStateTests | Módulo candidato | Posible módulo BossPhases; mantener reglas actuales en ME |
| `Domain/Combat/CombatActionContext.cs` | Contexto para validar acción de combate | Tipos del módulo de combate | CombatActionValidatorTests | Módulo | Extraer junto a un contrato neutral de combate |
| `Domain/Combat/CombatActionRequest.cs` | Solicitud de acción de combate | Domain.Actions, Domain.Primitives | CombatActionValidatorTests | Módulo | Separar petición común de campos propios de ME |
| `Domain/Combat/CombatActionValidator.cs` | Valida acciones de combate y condiciones | Domain.Actions, Domain.Conditions, Domain.Primitives | CombatActionValidatorTests | Módulo | No copiar hasta parametrizar reglas |
| `Domain/Combat/CombatImpactResult.cs` | Resultado de impacto | Domain.Primitives | CombatResourceImpactApplierTests | Módulo | Definir contrato común de impacto antes de extraer |
| `Domain/Combat/CombatResourceImpact.cs` | Describe impacto sobre recurso | Domain.Primitives | CombatResourceImpactApplierTests | Módulo | Candidato después de definir módulo de recursos |
| `Domain/Combat/CombatResourceImpactApplier.cs` | Aplica impactos a recursos de combatiente | Domain.Primitives y CombatantResourceState | CombatResourceImpactApplierTests | Módulo | Extraer junto a políticas de recursos/combate |
| `Domain/Combat/CombatantResourceState.cs` | Estado de recursos del combatiente | Domain.Primitives, Domain.Resources | CombatantResourceStateTests | Módulo con semántica de combate | No llevar al Core; revisar con ResourcePool |
| `Domain/Combat/CombatantSnapshot.cs` | Proyección inmutable de combatiente | Domain.Entities, Domain.Primitives | AdaptiveEnemyCombatTests, BossPhaseStateTests y validación de combate | Módulo | Rediseñar como contrato del módulo si se demuestra reutilizable |
| `Domain/Combat/DefensiveDamageCalculator.cs` | Mitigación, resistencias y cálculo defensivo | Domain.Primitives, Domain.Characters, Domain.Statistics | DefensiveDamageCalculatorTests | ME/módulo candidato | No copiar fórmulas; separar algoritmo de parámetros y validar con otro juego |
| `Domain/Combat/EnergyAffinityMatrix.cs` | Afinidades entre energías concretas de ME | Domain.Primitives y enum EnergyType propio | EnergyAffinityResolverTests | ME/patrón candidato | Mantener energías fuera; una matriz configurable sería otro componente |
| `Domain/Combat/MuertebenditaExecutionResolver.cs` | Reglas de ejecución y resistencia Muertebendita | Domain.Primitives | MuertebenditaExecutionResolverTests | ME | Excluir del framework |
| `Domain/Combat/NemesisProgression.cs` | Rangos, victorias, estados y progresión Nemesis | Domain.Primitives | NemesisProgressionTests | ME | Excluir del framework |
| `Domain/Combat/OffensiveDamageCalculator.cs` | Daño ofensivo y escalado por atributos | Domain.Characters, Domain.Primitives, Domain.Statistics | OffensiveDamageCalculatorTests | ME/módulo candidato | No copiar fórmulas ni atributos ME |
| `Domain/Combat/PerfectDeflect.cs` | Ventana temporal y consecuencias de desvío perfecto | Domain.Primitives | PerfectDeflectResolverTests | ME/módulo candidato | Mantener en cliente; estudiar módulo de timing defensivo más adelante |
| `Domain/Combat/PlayerResurrectionState.cs` | Oportunidades, decisiones y debuff de resurrección | Domain.Primitives, Domain.Statistics | PlayerResurrectionStateTests | ME | Excluir del framework |
| `Domain/Combat/PosturePressureAndRecovery.cs` | Presión, curvas y recuperación de postura | Domain.Primitives | PosturePressureAndRecoveryTests | ME/módulo candidato | No universalizar postura; evaluar recurso de presión parametrizable |
| `Domain/Combat/ResurrectionState.cs` | Estado de resurrección/recuperación de Nemesis | Domain.Primitives | ResurrectionStateTests | ME | Excluir del framework |
| `Domain/Combat/VitalityTrack.cs` | Seguimiento de vitalidad y vulnerabilidad | Domain.Primitives, Domain.Resources | VitalityTrackTests | ME/módulo candidato | Mantener en cliente hasta separar reglas de vitalidad |
| `Domain/Conditions/ConditionEvaluation.cs` | Resultado inmutable de evaluación | Sin dependencia externa | ConditionSetTests, ActionDefinitionTests | Core | Extraído a Core/Conditions |
| `Domain/Conditions/ConditionFailure.cs` | Fallo de condición con código estable | Domain.Primitives.GameError | ConditionSetTests, ActionDefinitionTests | Core | Extraído a Core/Conditions |
| `Domain/Conditions/ConditionSet.cs` | Evalúa condiciones en orden y acumula fallos | IGameCondition, ConditionEvaluation | ConditionSetTests | Core | Extraído a Core/Conditions |
| `Domain/Conditions/IGameCondition.cs` | Predicado sobre contexto explícito | ConditionEvaluation | ConditionSetTests, ActionDefinitionTests | Core | Extraído a Core/Conditions |
| `Domain/Entities/DomainEntity.cs` | Identidad, igualdad y retirada Active/Removed | Domain.Primitives.EntityId | DomainEntityTests, PlayerCharacterTests | Core, con nombre adaptado | Extraído como EntityBase; no modela muerte ni derrota |
| `Domain/Entities/EntityLifecycleState.cs` | Estados técnicos Active/Removed | Sin dependencia externa | DomainEntityTests | Core | Extraído a Core/Entities |
| `Domain/Primitives/EntityId.cs` | Identidad estable no vacía | Guid | EntityIdTests | Core | Extraído a Core/Primitives |
| `Domain/Primitives/Result.cs` | Resultado tipado y GameError | Sin dependencia externa | ResultTests | Core | Extraído a Core/Primitives |
| `Domain/Primitives/SimulationDuration.cs` | Duración lógica no negativa | Sin dependencia externa | SimulationTimeTests | Core | Extraído a Core/Time |
| `Domain/Primitives/SimulationInstant.cs` | Instante lógico y tiempo transcurrido | SimulationDuration | SimulationTimeTests | Core | Extraído a Core/Time |
| `Domain/Resources/ResourceId.cs` | Identificador de recurso | Sin dependencia externa | ResourcePoolTests | Módulo | Extraído a Modules.Resources/Resources/ResourceId.cs |
| `Domain/Resources/ResourcePool.cs` | Cantidad actual y máxima, actualizaciones inmutables | ResourceId | ResourcePoolTests, CombatantResourceStateTests | Módulo | Extraído a Modules.Resources; las políticas de coste/regeneración siguen fuera |
| `Domain/Statistics/AttributeScalingCurve.cs` | Curvas de escalado vinculadas a atributos ME | Domain.Characters.CharacterAttribute | StatModelTests y pruebas de daño | ME/módulo candidato | No extraer en su forma actual; eliminar dependencia del enum ME |
| `Domain/Statistics/StatId.cs` | Identificador de estadística | Sin dependencia externa | StatModelTests | Módulo | Candidato a Module.Statistics |
| `Domain/Statistics/StatModifier.cs` | Modificador con fuente, objetivo, operación y valor | Domain.Primitives | StatModelTests | Módulo | Extraer como dato; falta lifecycle de modificadores |
| `Domain/Statistics/StatModifierOperation.cs` | Operaciones de modificación | Sin dependencia externa | StatModelTests | Módulo | Candidato; definir orden y stacking |
| `Domain/Statistics/StatValue.cs` | Valor base de estadística | Sin dependencia externa | StatModelTests | Módulo | Candidato a Module.Statistics |
| `Headless/Program.cs` | Entrada de consola mínima | Sin lógica significativa | Sin prueba | Host/prototipo | No migrar; no constituye un runtime general |
| `Infrastructure/Persistence/JsonNemesisProgressionRepository.cs` | Persistencia JSON de progresión Nemesis | System.Text.Json, Application.Combat, Domain.Combat, Domain.Primitives | JsonNemesisProgressionRepositoryTests | Infra ME | Mantener en cliente; referencia para futuros adaptadores |
| `Infrastructure/Randomness/SeededRandomSource.cs` | Generador determinista SplitMix64 | Domain.Abstractions.IRandomSource | Sin prueba directa en origen | Core | Adaptado a Core/Randomness; pruebas deterministas en destino |

## B. Pruebas de origen — 31 archivos C#

| Archivo | Cobertura | Tratamiento |
|---|---|---|
| `Actions/ActionDefinitionTests.cs` | ID y precondiciones | Adaptado en Core.Tests/Actions |
| `Characters/CharacterAttributesTests.cs` | Límites y suma de atributos ME | Mantener en cliente ME |
| `Characters/PlayerCharacterTests.cs` | Clases, atributos y progresión de jugador | Mantener en cliente ME |
| `Combat/AdaptiveEnemyCombatTests.cs` | Memoria/adaptación enemiga | Mantener como referencia de módulo |
| `Combat/BossPhaseStateTests.cs` | Transiciones de jefe | Mantener como referencia de módulo |
| `Combat/CombatActionValidatorTests.cs` | Validación de acciones de combate | Mantener en cliente hasta extraer módulo |
| `Combat/CombatResourceImpactApplierTests.cs` | Aplicación de impactos | Mantener como referencia de módulo |
| `Combat/CombatantResourceStateTests.cs` | Estado de recursos de combatiente | Mantener como referencia de módulo |
| `Combat/DefensiveDamageCalculatorTests.cs` | Fórmulas defensivas ME | Mantener; no copiar al Core |
| `Combat/EnergyAffinityResolverTests.cs` | Afinidades energéticas ME | Mantener; no copiar el enum |
| `Combat/JsonNemesisProgressionRepositoryTests.cs` | Persistencia de Nemesis | Mantener en cliente/adaptador ME |
| `Combat/MuertebenditaExecutionResolverTests.cs` | Muertebendita | Mantener en cliente ME |
| `Combat/NemesisFinalExecutionCoordinatorTests.cs` | Remate/recuperación Nemesis | Mantener en cliente ME |
| `Combat/NemesisProgressionTests.cs` | Rangos y progresión Nemesis | Mantener en cliente ME |
| `Combat/NemesisWorldPopulationCoordinatorTests.cs` | Población Nemesis | Mantener en cliente ME |
| `Combat/NemesisWorldPresenceRegistryTests.cs` | Registro de presencia | Mantener; posible inspiración para contrato futuro |
| `Combat/OffensiveDamageCalculatorTests.cs` | Fórmulas ofensivas ME | Mantener; no copiar al Core |
| `Combat/PerfectDeflectResolverTests.cs` | Desvío perfecto | Mantener en cliente ME |
| `Combat/PlayerDefeatCoordinatorTests.cs` | Derrota y consecuencias | Mantener en cliente ME |
| `Combat/PlayerResurrectionStateTests.cs` | Resurrección del jugador | Mantener en cliente ME |
| `Combat/PosturePressureAndRecoveryTests.cs` | Presión y recuperación de postura | Mantener en cliente ME |
| `Combat/ResurrectionStateTests.cs` | Recuperación Nemesis | Mantener en cliente ME |
| `Combat/VitalityTrackTests.cs` | Vitalidad y vulnerabilidad | Mantener en cliente ME |
| `Conditions/ConditionSetTests.cs` | Orden, acumulación y snapshot de fallos | Adaptado en Core.Tests/Conditions |
| `Entities/DomainEntityTests.cs` | Identidad, igualdad y retirada | Adaptado en Core.Tests/Entities |
| `Primitives/EntityIdTests.cs` | Identidad no vacía | Adaptado en Core.Tests/Primitives |
| `Primitives/ResultTests.cs` | Result y GameError | Adaptado en Core.Tests/Primitives |
| `Primitives/SimulationTimeTests.cs` | Instantes y duraciones | Adaptado en Core.Tests/Time |
| `Resources/ResourcePoolTests.cs` | Capacidad, límites y actualizaciones de recursos | Adaptado en Modules.Resources.Tests |
| `Statistics/StatModelTests.cs` | Identificadores, valores y modificadores | Mantener como referencia de módulo |
| `Usings.cs` | Import global de xUnit | No migrar; pertenece a cada proyecto de pruebas |

## C. Documentación de origen — 31 archivos Markdown

| Archivo | Tema | Clasificación | Tratamiento |
|---|---|---|---|
| `README.md` | Descripción del motor ME | ME | Mantener en origen |
| `docs/actions-and-conditions-foundation.md` | Contratos de acciones/condiciones | Mixto | Referencia para contratos Core |
| `docs/adaptive-enemy-combat.md` | Adaptación enemiga | ME/módulo | Mantener; posible módulo futuro |
| `docs/architecture-audit.md` | Auditoría del motor | Migración | Referencia histórica; este inventario es la matriz vigente |
| `docs/architecture.md` | Arquitectura ME | ME | Mantener; arquitectura neutral en destino |
| `docs/boss-phase-transitions.md` | Fases de jefes | ME | Mantener |
| `docs/boss-reward-catalog-simulation.md` | Catálogo/simulación de recompensas | Balance ME | Mantener |
| `docs/combat-action-validation.md` | Validación de acciones de combate | Mixto | Referencia de módulo |
| `docs/combat-impact-application.md` | Aplicación de impacto | Mixto | Referencia de módulo |
| `docs/combat-mathematical-model.md` | Fórmulas y balance | ME | No convertir en regla genérica |
| `docs/combat-resource-state-foundation.md` | Recursos en combate | Mixto | Referencia de módulo |
| `docs/defensive-damage-calculation.md` | Daño defensivo | ME | Mantener |
| `docs/domain-foundation.md` | Fundamentos de dominio | Mixto | Reutilizar ideas, no semántica ME |
| `docs/energy-affinity-system.md` | Afinidades energéticas | ME | Mantener |
| `docs/full-progression-economic-simulation.md` | Economía/progresión | ME | Mantener |
| `docs/gdd-baseline.md` | GDD funcional | Canon ME | No copiar al framework |
| `docs/muertebendita-execution.md` | Muertebendita | ME | Mantener |
| `docs/nemesis-final-execution.md` | Remate Nemesis | ME | Mantener |
| `docs/nemesis-progression-policy.md` | Política Nemesis | ME | Mantener |
| `docs/nemesis-rank-evolution.md` | Rangos Nemesis | ME | Mantener |
| `docs/nemesis-system.md` | Sistema Nemesis | ME | Mantener |
| `docs/new-game-plus-design.md` | Nuevo juego+ | ME | Mantener |
| `docs/offensive-damage-calculation.md` | Daño ofensivo | ME | Mantener |
| `docs/perfect-deflects.md` | Desvío perfecto | ME | Mantener |
| `docs/player-character-foundation.md` | Personaje jugador | ME | Mantener |
| `docs/player-resurrection-system.md` | Resurrección jugador | ME | Mantener |
| `docs/post-gdd-expansion-candidates.md` | Candidatos de expansión | Propuestas ME | Mantener como propuestas; no promover a canon |
| `docs/posture-pressure-and-recovery.md` | Postura | ME | Mantener |
| `docs/progression-curve-rebalance.md` | Balance de progresión | ME | Mantener |
| `docs/resource-model-foundation.md` | Modelo de recursos | Mixto | Referencia para Module.Resources |
| `docs/statistics-and-modifiers-foundation.md` | Estadísticas/modificadores | Mixto | Referencia para Module.Statistics |

## D. Proyectos y configuración — 11 archivos

| Archivo | Función y dependencias | Tratamiento |
|---|---|---|
| `MythicalExodus.sln` | Solución de cuatro proyectos de producción y uno de pruebas | No copiar; solución neutral en destino |
| `Directory.Build.props` | .NET 8, C# 12, nullable, warnings-as-errors, análisis y determinismo | Criterios adaptados a la solución destino |
| `global.json` | SDK .NET 8.0.100, roll-forward latestFeature | Referencia; mantener el pin de destino y verificar CI |
| `.editorconfig` | Formato y estilo | Revisar y adaptar |
| `.gitignore` | Exclusiones IDE/build | Adaptado en destino |
| `.github/workflows/dotnet.yml` | Restore/build/test del origen | Referencia; CI propia en destino |
| `src/MythicalExodus.Application/MythicalExodus.Application.csproj` | Capa Application depende de Domain | No copiar; proyectos neutrales propios |
| `src/MythicalExodus.Domain/MythicalExodus.Domain.csproj` | Dominio ME | No copiar; Core neutral propio |
| `src/MythicalExodus.Headless/MythicalExodus.Headless.csproj` | Host de consola | No copiar; host general posterior |
| `src/MythicalExodus.Infrastructure/MythicalExodus.Infrastructure.csproj` | Adaptadores de ME | No copiar; infraestructura por cliente |
| `tests/MythicalExodus.Domain.Tests/MythicalExodus.Domain.Tests.csproj` | xUnit, Test SDK, referencia a Domain | No copiar; Core.Tests independiente |

## E. Matriz origen → destino y estado

| Origen | Destino neutral | Estado |
|---|---|---|
| Domain/Primitives/EntityId.cs | Core/Primitives/EntityId.cs | Extraído y probado |
| Domain/Primitives/Result.cs | Core/Primitives/Result.cs | Extraído y probado |
| Domain/Primitives/SimulationDuration.cs | Core/Time/SimulationDuration.cs | Extraído y probado |
| Domain/Primitives/SimulationInstant.cs | Core/Time/SimulationInstant.cs | Extraído y probado |
| Application/Time/IGameClock.cs | Core/Time/IGameClock.cs, ManualGameClock.cs | Extraído; falta prueba específica de reloj manual |
| Domain/Abstractions/IRandomSource.cs | Core/Randomness/IRandomSource.cs | Extraído |
| Infrastructure/Randomness/SeededRandomSource.cs | Core/Randomness/SeededRandomSource.cs | Adaptado y probado; no criptográfico |
| Domain/Entities/DomainEntity.cs | Core/Entities/EntityBase.cs | Extraído y probado |
| Domain/Entities/EntityLifecycleState.cs | Core/Entities/EntityLifecycleState.cs | Extraído y probado |
| Domain/Actions/ActionId.cs | Core/Actions/ActionId.cs | Extraído y probado |
| Domain/Actions/ActionDefinition.cs | Core/Actions/ActionDefinition.cs | Extraído y probado; no ejecuta |
| Domain/Conditions/*.cs | Core/Conditions/*.cs | Extraídos y probados |
| Definición de acción + contratos de Runtime | Runtime/Actions/ActionExecutionPipeline | Ejecuta árboles de postacciones y devuelve eventos/efectos ordenados; no los despacha ni aplica |
| Domain/Resources/ResourceId.cs, ResourcePool.cs | Modules.Resources | Extraídos como estado acotado e inmutable; las políticas siguen siendo responsabilidad del juego |
| Domain/Statistics/* | Module.Statistics | Pendiente; falta evaluador efectivo y lifecycle |
| Domain/Combat/* | Module.Combat | Pendiente; no copiar fórmulas/reglas ME |
| Application/Combat/* | Cliente ME / posible módulo de presencia | Específico o candidato sin extraer |
| Infrastructure/Persistence/* | Adaptador del cliente ME | Específico |
| Headless/Program.cs | Host futuro | No migrado; no es runtime |

## F. Módulo compartido incorporado: Resources

`ResourceId` y `ResourcePool` se han extraído a `FrameworkMultijuegos.Modules.Resources`, con proyecto y pruebas independientes. El módulo no referencia Core porque estos dos tipos no necesitan sus contratos. No implementa gasto, regeneración, costes, deuda, recarga ni overflow. Esa exclusión evita convertir políticas de un juego en comportamiento universal.

## G. Primera capacidad de Runtime incorporada

`FrameworkMultijuegos.Runtime` contiene un pipeline de ejecución de acciones registrado explícitamente. Ejecuta postacciones múltiples y anidadas en profundidad primero, conserva el orden de eventos/intenciones de efecto, detecta ciclos en la ruta de ancestros y limita profundidad e invocaciones. Los resultados se publican solo si todo el árbol tiene éxito. Los handlers deben ser deterministas y no mutar estado externo.

El pipeline todavía no despacha eventos ni aplica efectos. La entrega de eventos, el ciclo de vida de efectos persistentes, stacking, expiración y retirada son trabajo pendiente; no se declaran implementados.

## H. Límites y riesgos

1. El repositorio fuente permanece intacto.
2. El repositorio fuente no declara una licencia en los metadatos de GitHub. La migración es entre repositorios del mismo propietario/autor identificado y no incorpora código de terceros. Antes de publicar paquetes o aceptar contribuciones externas, el propietario debe elegir y añadir una licencia explícita.
3. Atributos, clases de personaje, energías, Muertebendita, Nemesis, resurrección, postura, fórmulas y balance permanecen fuera del Core.
4. `ActionDefinition` solo describe y valida precondiciones; no ejecuta.
5. `EntityBase.Remove()` solo retira una entidad de la simulación; no significa muerte/destrucción.
6. El origen no contiene un pipeline general de eventos/efectos/postacciones. En destino existe un primer pipeline de acciones/postacciones; el despacho de eventos y la aplicación/lifecycle de efectos siguen pendientes.
7. No se añade ECS, carga dinámica de plugins, event sourcing, scripting ni editor sin casos de uso verificables.

## I. Reinspección y CI

- HEAD origen: `788b4a5517a3c55dbc9780826ae0692f65af1949`; coincide con auditoría anterior.
- CI origen, ejecución `45895955807`: restore/build/test exitosos.
- CI inicial destino, ejecución `45918025870`: restore/build/test exitosos.
- PR destino: `#1`, rama `foundation/architecture-and-initial-extraction`, draft, sin merge.
- Commit de ciclo de vida/acciones/condiciones `e73cea1e5c37b9f994645c9c32d5bdf997c9bc39`: CI exitosa.
- Commit de Resources `0819da1ad7e40b3d22af2992818c509f3566ed3f`: CI exitosa.
- Commit de Runtime `4e41a68977db414efdd9b9fe15cf9a2999c5c6bb`: CI exitosa; 42 pruebas aprobadas, cero warnings y cero errores.
- HEAD actual de la rama de trabajo: `4e41a68977db414efdd9b9fe15cf9a2999c5c6bb`.

Actualizar este inventario si cambia el HEAD de origen o se incorpora una extracción nueva.