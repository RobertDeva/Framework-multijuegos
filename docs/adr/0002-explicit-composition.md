# ADR 0002: Composición explícita antes de plugins dinámicos

- Estado: Aceptado para la fase inicial
- Fecha: 2026-11-26

## Contexto
El framework necesita combinar capacidades opcionales, pero no hay todavía dos clientes reales que exijan descubrimiento dinámico, carga de assemblies o extensiones de terceros.

## Decisión
La composición inicial se realizará mediante referencias explícitas y configuración validada. Los módulos declararán dependencias y contratos. No se implementará un sistema de plugins basado en reflexión ni un contenedor DI propio hasta que exista una necesidad demostrable.

## Consecuencias
La arquitectura será más simple y verificable. Si posteriormente se requiere carga dinámica, se definirá un contrato de versión, aislamiento, fallos de carga y compatibilidad antes de implementarla.
