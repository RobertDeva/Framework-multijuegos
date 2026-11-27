# Módulo de recursos

## Alcance actual

`FrameworkMultijuegos.Modules.Resources` contiene dos tipos:

- `ResourceId`: identificador de recurso definido por el juego.
- `ResourcePool`: estado inmutable con cantidad actual y máxima.

El pool garantiza:

- identificador válido;
- máximo no negativo;
- cantidad actual entre cero y el máximo;
- actualizaciones mediante nuevas instancias validadas.

## Lo que no hace

No gasta ni restaura automáticamente, no regenera con el tiempo, no define costes de acciones, deuda, prioridad, overflow, recarga al morir ni persistencia. Esas políticas dependen del juego y no deben asumirse universales.

## Dependencias

El módulo no depende de Core porque su modelo actual no necesita ningún tipo de Core. No se añadirá una referencia solo para imponer uniformidad. Si una futura política requiere identidad de entidades, tiempo o resultados del framework, la dependencia se incorporará de forma explícita y se probará.

## Origen y pruebas

Adaptado de `Game-Logic-motor/src/MythicalExodus.Domain/Resources`. Las pruebas de origen se reescribieron en `FrameworkMultijuegos.Modules.Resources.Tests`; el repositorio fuente permanece intacto.
