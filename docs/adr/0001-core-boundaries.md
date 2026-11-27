# ADR 0001: Límites del Core

- Estado: Aceptado para la fase inicial
- Fecha: 2026-11-26

## Contexto
El primer motor cliente es Mythical Exodus, pero el framework debe servir juegos con reglas y universos distintos. El Core no puede heredar vocabulario ni reglas del primer cliente.

## Decisión
El Core contiene únicamente mecanismos que puedan expresarse sin mencionar un juego. Combate, recursos y estadísticas serán módulos separados. Las reglas, datos, contenido, balance, clases, energías, progresión y narrativa pertenecen a cada juego.

## Consecuencias
- La extracción será selectiva y trazable.
- Los tipos genéricos se prueban sin referencias a Mythical Exodus.
- Los módulos son opt-in.
- No se introduce carga dinámica de plugins en la primera fase.
- Una abstracción solo se promueve a Core cuando existe evidencia de uso independiente.
