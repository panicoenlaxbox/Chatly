# Plan: Añadir menú Spectre.Console a Chatly

## Resumen
Integrar Spectre.Console para crear un menú principal elegante al iniciar la app y comandos interactivos durante el chat (`/exit`, `/clear`, `/new`, `/configuration`). Incluye: 1) instalar paquete, 2) crear clase para manejar el menú, 3) refactorizar `Application.cs` para usar el menú, 4) implementar comandos incluyendo vista de configuración ofuscada.

## Pasos

### 1. Instalar Spectre.Console en `Chatly/Chatly.csproj`
- Agregar referencia `Spectre.Console` (última versión estable ~5.0+)

### 2. Crear nueva clase `MenuHandler.cs` en `Chatly/`
- Método `ShowMainMenu()`: selecciona modo inicial
- Método `ShowHelpCommands()`: muestra opciones `/exit`, `/clear`, `/new`, `/configuration`
- Método `ShowConfiguration(IConfiguration config)`: muestra endpoint, deployment name, API Key ofuscada
  - Ofuscar API Key: mostrar solo primeros 5 y últimos 5 caracteres + `***` en medio (ej: `sk_te***...abc12`)
  - Usar `Panel` para mostrar configuración
- Usar `SelectionPrompt` para UI elegante

### 3. Modificar `Application.cs`
- En `RunAsync()` línea ~12: añadir instancia de `MenuHandler` y llamar a `ShowMainMenu()` antes del loop
- En el chat loop (línea ~60-82): 
  - Cambiar `Console.ReadLine()` por `AnsiConsole.Prompt()` (Spectre styled)
  - Detectar comandos: `/exit`, `/clear`, `/new`, `/configuration`
  - Pasar `IConfiguration` a `MenuHandler` para `/configuration`
  - Aplicar la lógica correspondiente:
    - `/exit`: salir del loop
    - `/clear`: limpiar lista de mensajes y volver al menú
    - `/new`: reiniciar la conversación (mantener en chat)
    - `/configuration`: mostrar panel con config ofuscada

### 4. Mejorar salida de mensajes (opcional pero recomendado)
- Usar `AnsiConsole.MarkupLine()` para colorear "User:" y "Assistant:" con colores
- Usar panels de Spectre para destacar respuestas

## Verificación
- Compilar: `dotnet build`
- Ejecutar: `dotnet run`
- Verificar: 
  - Menú inicial aparece con opciones elegantes
  - `/configuration` muestra un panel con: Azure Endpoint, Deployment Name, API Key ofuscada
  - `/clear` limpia historial y vuelve al menú
  - `/new` reinicia conversación sin salir del chat
  - `/exit` sale correctamente

## Decisiones
- Comando `/configuration`: mostrar panel con los valores
- Ofuscación: `primeros5chars***...últimos5chars` (seguro pero reconocible)
- Componente sugerido: `Panel` de Spectre para encerrar la configuración
- Componente para menú: `SelectionPrompt` para UI elegante con navegación por flechas

## Estructura de archivos final
```
Chatly/
  Application.cs (modificado)
  MenuHandler.cs (nuevo)
  Program.cs (sin cambios)
  HttpFileHandler.cs (sin cambios)
  Chatly.csproj (modificado - agregar Spectre.Console)
  appsettings.json (sin cambios)
```
