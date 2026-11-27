# Hoja de ruta

## Fase 0 — Base verificable
- Reinspección de origen y destino.
- Inventario de migración y límites arquitectónicos.
- Solución, Core inicial, pruebas y CI.
- Resultado: compilación y pruebas verdes.

## Fase 1 — Fundamentos
- Revisar y migrar ciclo de vida, acciones y condiciones.
- Añadir tests de contratos, inmutabilidad y errores.
- Documentar compatibilidad y serialización.

## Fase 2 — Runtime
- Definir pipeline de comandos/acciones y eventos.
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

## Criterio de avance
Cada fase requiere compilación, pruebas automatizadas, documentación, revisión de dependencias y una decisión explícita sobre lo que se excluye. No se marca una fase como completa por tener carpetas o interfaces vacías.
