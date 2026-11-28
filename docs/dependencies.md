# Dependencias y límites entre proyectos

## Plataforma

- Target framework: .NET 8 (`net8.0`).
- Lenguaje: C# 12.
- Nullable reference types: habilitado.
- Warnings como errores: habilitado.
- Build determinista: habilitado.
- La lógica de producción no depende de Unity, Unreal, Godot ni de otro motor gráfico.

## Grafo de proyectos

```text
FrameworkMultijuegos.Core
    ↑
FrameworkMultijuegos.Runtime
    ↑
FrameworkMultijuegos.Runtime.Tests

FrameworkMultijuegos.Modules.Resources
    ↑
FrameworkMultijuegos.Modules.Resources.Tests

FrameworkMultijuegos.Core
    ↑
FrameworkMultijuegos.Core.Tests
```

La flecha indica referencia de proyecto. Resources no referencia Core porque su modelo actual solo usa identificadores propios y valores decimales; no se añade una dependencia artificial. Runtime sí referencia Core para definiciones de acciones, condiciones e invariantes comunes.

## Dependencias externas directas

Las dependencias externas están limitadas a proyectos de pruebas:

| Paquete | Versión | Uso |
|---|---:|---|
| `Microsoft.NET.Test.Sdk` | 17.12.0 | Integración con VSTest |
| `xunit` | 2.9.2 | Framework de pruebas |
| `xunit.runner.visualstudio` | 2.8.2 | Adaptador de xUnit para VSTest; assets privados |

Los tres proyectos de pruebas usan el mismo conjunto de versiones. Los proyectos de producción solo usan bibliotecas de .NET.

## CI verificada

El workflow ejecuta:

1. `dotnet restore FrameworkMultijuegos.sln`
2. `dotnet build FrameworkMultijuegos.sln --configuration Release --no-restore`
3. `dotnet test FrameworkMultijuegos.sln --configuration Release --no-build --verbosity normal`

En el commit `4e41a68977db414efdd9b9fe15cf9a2999c5c6bb`, restore/build/test finalizaron correctamente; build registró cero warnings y cero errores. Se ejecutaron 42 pruebas: 23 Core, 9 Resources y 10 Runtime, todas aprobadas.

## Auditoría pendiente

No se ha ejecutado todavía un informe específico de vulnerabilidades/transitivas de NuGet ni se ha configurado Dependabot. La ausencia de paquetes de producción reduce la superficie actual, pero no sustituye una auditoría. Añadir un control de vulnerabilidades automatizado antes de publicar paquetes o ampliar dependencias externas.
