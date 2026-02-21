# Citas Médicas - Guía de Ejecución

## Arquitectura

```
Capa Cliente (MVC) → Capa Servidor (BFF) → API.CitasMedicas (Microservicio)
    :5053               :5201                  :5112
```

| Capa | Puerto | Descripción |
|------|--------|-------------|
| API.CitasMedicas | 5112 | Microservicio de citas (acceso a BD) |
| Capa Servidor | 5201 | Backend For Frontend (centraliza APIs) |
| Capa Cliente | 5053 | Frontend MVC (vistas de usuario) |

---

## Ejecutar Todo (Orden Recomendado)

Abrir 3 terminales y ejecutar en este orden:

### Terminal 1 - Microservicio
```bash
cd "API.CitasMedicas"
dotnet run
```

### Terminal 2 - BFF (Backend)
```bash
cd "Capa Servidor"
dotnet run
```

### Terminal 3 - Frontend
```bash
cd "Capa Cliente"
dotnet run
```

### URLs disponibles:
- **Frontend:** http://localhost:5053
- **Citas:** http://localhost:5053/Appointments
- **BFF Swagger:** http://localhost:5201/swagger
- **API Swagger:** http://localhost:5112/swagger

---

## Ejecutar por Separado

### Solo API.CitasMedicas (Microservicio)

```bash
cd "API.CitasMedicas"
dotnet run
```
- Puerto: `http://localhost:5112`
- Swagger: `http://localhost:5112/swagger`
- Acceso directo a base de datos
- Contiene la lógica de negocio de citas

### Solo Capa Servidor (BFF)

```bash
cd "Capa Servidor"
dotnet run
```
- Puerto: `http://localhost:5201`
- Swagger: `http://localhost:5201/swagger`
- **Requiere:** API.CitasMedicas corriendo
- Centraliza llamadas a microservicios

### Solo Capa Cliente (Frontend MVC)

```bash
cd "Capa Cliente"
dotnet run
```
- Puerto: `http://localhost:5053`
- **Requiere:** Capa Servidor corriendo
- Interfaz de usuario para gestionar citas

---

## Compilar Todo

```bash
dotnet build MVC.CitasMedicas.slnx
```

## Compilar por Separado

```bash
# Microservicio
cd "API.CitasMedicas"
dotnet build

# BFF
cd "Capa Servidor"
dotnet build

# Frontend
cd "Capa Cliente"
dotnet build
```

---

## Swagger

Swagger es una librería para documentar APIs (Application Programming Interface).

- **BFF:** http://localhost:5201/swagger
- **Microservicio:** http://localhost:5112/swagger

---

## Configuración de Conexiones

| Archivo | Configuración |
|---------|---------------|
| `API.CitasMedicas/appsettings.json` | ConnectionString a SQL Server |
| `Capa Servidor/appsettings.json` | URL del microservicio (5112) |
| `Capa Cliente/appsettings.json` | URL del BFF (5201) |

---

## Script PowerShell para Ejecutar Todo

Guarda esto como `start-all.ps1`:

```powershell
# Iniciar todos los servicios
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'API.CitasMedicas'; dotnet run"
Start-Sleep -Seconds 3
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'Capa Servidor'; dotnet run"
Start-Sleep -Seconds 3
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'Capa Cliente'; dotnet run"
Start-Sleep -Seconds 3
Start-Process "http://localhost:5053/Appointments"
```
