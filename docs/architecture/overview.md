# Arquitectura objetivo

## Propósito

Framework Multijuegos proporciona mecanismos reutilizables para construir juegos con reglas, universos, géneros y escalas diferentes. No define el contenido ni las reglas de ningún juego concreto.

## Capas

1. **Core**: tipos fundamentales, invariantes pequeñas y deterministas; no depende de motores gráficos, almacenamiento ni aplicaciones.
2. **Runtime**: orquestación de acciones y postacciones. La primera versión ejecuta árboles acotados y devuelve eventos e intenciones de efecto ordenados; el despacho de eventos y la aplicación/lifecycle de efectos se incorporarán en etapas posteriores.
3. **Módulos compartidos**: capacidades opcionales y componibles (combate, estadísticas, recursos, inventario, IA, economía, misiones, progresión y narrativa). Ningún juego debe cargar módulos que no necesite.
4. **Adaptadores/hosts**: integración con persistencia, presentación, motores de juego y procesos Headless.
5. **Proyectos de juego**: contienen reglas, contenido, balance, narrativa, identidad y composición de módulos específicos.

## Reglas de dependencia

- Core no conoce juegos clientes ni motores.
- Los módulos dependen de Core y declaran sus contratos.
- Los juegos deciden qué módulos componen su experiencia.
- Infrastructure implementa puertos definidos por capas internas; no impone políticas de dominio.
- Las dependencias se dirigen hacia abstracciones estables.

## Composición

La primera versión usará composición explícita por código y configuración validada. No se añadirá carga dinámica de plugins, reflexión generalizada ni un contenedor DI propio hasta que exista un caso real que lo requiera.

## Límites

No se presupone ECS, event sourcing, multijugador distribuido, scripting, editor visual ni simulación geográfica universal. Son decisiones futuras sujetas a requisitos y prototipos.
