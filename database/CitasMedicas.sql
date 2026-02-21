-- =============================================================
-- Script de Base de Datos: Sistema de Gestión de Citas Médicas (con datos)
-- Motor: Microsoft SQL Server (T-SQL)
-- Fecha de generación: 2026-02-03
-- Autor: M365 Copilot
-- =============================================================

IF DB_ID('CitasMedicas') IS NULL
BEGIN
    CREATE DATABASE CitasMedicas;
END
GO

USE CitasMedicas;
GO

-- Eliminar restricciones si existen
IF OBJECT_ID('dbo.Cita','U') IS NOT NULL BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Cita_Paciente') ALTER TABLE dbo.Cita DROP CONSTRAINT FK_Cita_Paciente;
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Cita_Doctor') ALTER TABLE dbo.Cita DROP CONSTRAINT FK_Cita_Doctor;
END
IF OBJECT_ID('dbo.Doctor','U') IS NOT NULL BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name='FK_Doctor_Especialidad') ALTER TABLE dbo.Doctor DROP CONSTRAINT FK_Doctor_Especialidad;
END
GO

-- Drops ordenados
IF OBJECT_ID('dbo.Cita','U') IS NOT NULL DROP TABLE dbo.Cita;
IF OBJECT_ID('dbo.Doctor','U') IS NOT NULL DROP TABLE dbo.Doctor;
IF OBJECT_ID('dbo.Paciente','U') IS NOT NULL DROP TABLE dbo.Paciente;
IF OBJECT_ID('dbo.Especialidad','U') IS NOT NULL DROP TABLE dbo.Especialidad;
GO

-- Tablas
CREATE TABLE dbo.Especialidad (
  EspecialidadId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Especialidad PRIMARY KEY,
  Nombre NVARCHAR(100) NOT NULL CONSTRAINT UQ_Especialidad UNIQUE,
  Activo BIT NOT NULL CONSTRAINT DF_Especialidad_Activo DEFAULT(1),
  FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Especialidad_Fecha DEFAULT (SYSUTCDATETIME())
);
GO

CREATE TABLE dbo.Doctor (
  DoctorId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Doctor PRIMARY KEY,
  Nombre NVARCHAR(150) NOT NULL,
  EspecialidadId INT NOT NULL,
  Telefono NVARCHAR(30) NULL,
  Email NVARCHAR(150) NULL,
  Activo BIT NOT NULL CONSTRAINT DF_Doctor_Activo DEFAULT(1),
  FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Doctor_Fecha DEFAULT (SYSUTCDATETIME())
);
GO

CREATE TABLE dbo.Paciente (
  PacienteId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Paciente PRIMARY KEY,
  Nombre NVARCHAR(150) NOT NULL,
  Edad INT NOT NULL CONSTRAINT CK_Paciente_Edad CHECK (Edad BETWEEN 0 AND 120),
  Telefono NVARCHAR(30) NULL,
  Email NVARCHAR(150) NULL,
  Activo BIT NOT NULL CONSTRAINT DF_Paciente_Activo DEFAULT(1),
  FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Paciente_Fecha DEFAULT (SYSUTCDATETIME())
);
GO

CREATE TABLE dbo.Cita (
  CitaId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cita PRIMARY KEY,
  PacienteId INT NOT NULL,
  DoctorId INT NOT NULL,
  FechaCita DATE NOT NULL,
  HoraCita TIME(0) NOT NULL,
  Motivo NVARCHAR(500) NULL,
  Estado TINYINT NOT NULL CONSTRAINT DF_Cita_Estado DEFAULT(1) CONSTRAINT CK_Cita_Estado CHECK (Estado IN (1,2,3)),
  FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_Cita_Fecha DEFAULT (SYSUTCDATETIME())
);
GO

-- FK & Índices
ALTER TABLE dbo.Doctor ADD CONSTRAINT FK_Doctor_Especialidad FOREIGN KEY (EspecialidadId) REFERENCES dbo.Especialidad(EspecialidadId);
ALTER TABLE dbo.Cita ADD CONSTRAINT FK_Cita_Paciente FOREIGN KEY (PacienteId) REFERENCES dbo.Paciente(PacienteId);
ALTER TABLE dbo.Cita ADD CONSTRAINT FK_Cita_Doctor FOREIGN KEY (DoctorId) REFERENCES dbo.Doctor(DoctorId);
GO

CREATE INDEX IX_Doctor_EspecialidadId ON dbo.Doctor(EspecialidadId);
CREATE INDEX IX_Cita_FechaCita ON dbo.Cita(FechaCita);
CREATE INDEX IX_Cita_PacienteId ON dbo.Cita(PacienteId);
CREATE INDEX IX_Cita_DoctorId ON dbo.Cita(DoctorId);
GO

-- =====================
-- Datos Semilla
-- =====================
INSERT INTO dbo.Especialidad (Nombre) VALUES
(N'Cardiología'),
(N'Pediatría'),
(N'Medicina General'),
(N'Dermatología'),
(N'Ginecología'),
(N'Oftalmología'),
(N'Traumatología'),
(N'Neurología'),
(N'Psiquiatría'),
(N'Endocrinología');
GO
INSERT INTO dbo.Doctor (Nombre, EspecialidadId, Telefono, Email) VALUES
(N'Dr. Álvaro Rojas', 1, N'+506 2221-1001', N'dr..alvaro.rojas@clinicacr.com'),
(N'Dra. Sofía Martínez', 2, N'+506 2222-1002', N'dra..sofia.martinez@clinicacr.com'),
(N'Dr. Ricardo Jiménez', 3, N'+506 2223-1003', N'dr..ricardo.jimenez@clinicacr.com'),
(N'Dra. Valeria Campos', 4, N'+506 2224-1004', N'dra..valeria.campos@clinicacr.com'),
(N'Dr. Esteban Mora', 5, N'+506 2225-1005', N'dr..esteban.mora@clinicacr.com'),
(N'Dra. Irene Solís', 6, N'+506 2226-1006', N'dra..irene.solis@clinicacr.com'),
(N'Dr. Marcelo Díaz', 7, N'+506 2227-1007', N'dr..marcelo.diaz@clinicacr.com'),
(N'Dra. Natalia Chaves', 8, N'+506 2228-1008', N'dra..natalia.chaves@clinicacr.com'),
(N'Dr. Andrés Quesada', 9, N'+506 2229-1009', N'dr..andres.quesada@clinicacr.com'),
(N'Dra. Paula Herrera', 10, N'+506 2230-1010', N'dra..paula.herrera@clinicacr.com');
GO
INSERT INTO dbo.Paciente (Nombre, Edad, Telefono, Email) VALUES
(N'Juan Pérez', 82, N'+506 8881-1001', N'juan.perez@email.com'),
(N'Laura Gómez', 15, N'+506 8882-1002', N'laura.gomez@email.com'),
(N'Carlos Rodríguez', 4, N'+506 8883-1003', N'carlos.rodriguez@email.com'),
(N'María Fernández', 36, N'+506 8884-1004', N'maria.fernandez@email.com'),
(N'José Ramírez', 32, N'+506 8885-1005', N'jose.ramirez@email.com'),
(N'Ana Morales', 29, N'+506 8886-1006', N'ana.morales@email.com'),
(N'Diego Castro', 18, N'+506 8887-1007', N'diego.castro@email.com'),
(N'Sofía González', 14, N'+506 8888-1008', N'sofia.gonzalez@email.com'),
(N'Luis Herrera', 87, N'+506 8889-1009', N'luis.herrera@email.com'),
(N'Gabriela Vargas', 70, N'+506 8890-1010', N'gabriela.vargas@email.com'),
(N'Andrés López', 12, N'+506 8891-1011', N'andres.lopez@email.com'),
(N'Daniela Jiménez', 76, N'+506 8892-1012', N'daniela.jimenez@email.com'),
(N'Fernando Salazar', 55, N'+506 8893-1013', N'fernando.salazar@email.com'),
(N'Camila Navarro', 5, N'+506 8894-1014', N'camila.navarro@email.com'),
(N'Pablo Torres', 4, N'+506 8895-1015', N'pablo.torres@email.com'),
(N'Mariana León', 12, N'+506 8896-1016', N'mariana.leon@email.com'),
(N'Kevin Mena', 28, N'+506 8897-1017', N'kevin.mena@email.com'),
(N'Isabella Rojas', 30, N'+506 8898-1018', N'isabella.rojas@email.com'),
(N'Ricardo Solís', 65, N'+506 8899-1019', N'ricardo.solis@email.com'),
(N'Valeria Sánchez', 78, N'+506 88100-1020', N'valeria.sanchez@email.com'),
(N'Javier Arias', 4, N'+506 88101-1021', N'javier.arias@email.com'),
(N'Paula Méndez', 72, N'+506 88102-1022', N'paula.mendez@email.com'),
(N'Héctor Brenes', 26, N'+506 88103-1023', N'hector.brenes@email.com'),
(N'Nicole Alpízar', 84, N'+506 88104-1024', N'nicole.alpizar@email.com'),
(N'Jorge Chacón', 90, N'+506 88105-1025', N'jorge.chacon@email.com'),
(N'Carolina Pérez', 70, N'+506 88106-1026', N'carolina.perez@email.com'),
(N'Martin Zúñiga', 54, N'+506 88107-1027', N'martin.zuniga@email.com'),
(N'Alejandra Alfaro', 29, N'+506 88108-1028', N'alejandra.alfaro@email.com'),
(N'Sergio Barboza', 58, N'+506 88109-1029', N'sergio.barboza@email.com'),
(N'Karina Soto', 76, N'+506 88110-1030', N'karina.soto@email.com'),
(N'Allan Cordero', 36, N'+506 88111-1031', N'allan.cordero@email.com'),
(N'Melissa Aguilar', 1, N'+506 88112-1032', N'melissa.aguilar@email.com'),
(N'Esteban Quesada', 21, N'+506 88113-1033', N'esteban.quesada@email.com'),
(N'Priscilla Mora', 90, N'+506 88114-1034', N'priscilla.mora@email.com'),
(N'Randy Jiménez', 55, N'+506 88115-1035', N'randy.jimenez@email.com'),
(N'Katherine Solano', 44, N'+506 88116-1036', N'katherine.solano@email.com'),
(N'Gustavo Paniagua', 36, N'+506 88117-1037', N'gustavo.paniagua@email.com'),
(N'Fabiola Coto', 20, N'+506 88118-1038', N'fabiola.coto@email.com'),
(N'Henry Muñoz', 28, N'+506 88119-1039', N'henry.munoz@email.com'),
(N'Roxana Rojas', 44, N'+506 88120-1040', N'roxana.rojas@email.com'),
(N'Oscar Campos', 14, N'+506 88121-1041', N'oscar.campos@email.com'),
(N'Natalia Vega', 12, N'+506 88122-1042', N'natalia.vega@email.com'),
(N'Victor Alfaro', 49, N'+506 88123-1043', N'victor.alfaro@email.com'),
(N'Daniela Monge', 13, N'+506 88124-1044', N'daniela.monge@email.com'),
(N'César Hidalgo', 46, N'+506 88125-1045', N'cesar.hidalgo@email.com'),
(N'Lucía Mora', 45, N'+506 88126-1046', N'lucia.mora@email.com'),
(N'Tomás Araya', 78, N'+506 88127-1047', N'tomas.araya@email.com'),
(N'Andrea Porras', 34, N'+506 88128-1048', N'andrea.porras@email.com'),
(N'Michael León', 6, N'+506 88129-1049', N'michael.leon@email.com'),
(N'María José Castro', 59, N'+506 88130-1050', N'maria.jose.castro@email.com');
GO
INSERT INTO dbo.Cita (PacienteId, DoctorId, FechaCita, HoraCita, Motivo, Estado) VALUES
(35, 2, '2026-02-27', '08:30', N'Consulta pediátrica', 1),
(41, 10, '2026-02-26', '13:30', N'Dolor de espalda', 1),
(3, 4, '2026-02-21', '08:30', N'Chequeo de la vista', 1),
(7, 7, '2026-02-20', '11:30', N'Revisión de tratamiento', 1),
(11, 6, '2026-02-25', '09:30', N'Revisión de tratamiento', 1),
(45, 2, '2026-03-13', '14:00', N'Dolor de cabeza', 3),
(47, 4, '2026-02-13', '11:30', N'Lectura de exámenes', 1),
(41, 9, '2026-02-17', '14:00', N'Referencia a especialista', 1),
(15, 1, '2026-02-23', '11:00', N'Consulta de seguimiento', 1),
(14, 10, '2026-03-20', '10:30', N'Dolor de espalda', 2),
(26, 8, '2026-02-12', '10:00', N'Dolor de cabeza', 1),
(48, 9, '2026-03-09', '10:00', N'Control de presión', 3),
(28, 10, '2026-02-28', '10:30', N'Dolor de espalda', 1),
(33, 8, '2026-02-08', '15:00', N'Chequeo general', 1),
(10, 3, '2026-03-18', '11:00', N'Evaluación preoperatoria', 1),
(25, 7, '2026-03-13', '11:30', N'Consulta pediátrica', 1),
(36, 1, '2026-03-18', '14:30', N'Control anual', 3),
(49, 5, '2026-03-16', '10:30', N'Control anual', 1),
(28, 3, '2026-03-04', '08:00', N'Control de presión', 1),
(33, 3, '2026-03-07', '16:00', N'Control anual', 1),
(41, 9, '2026-03-13', '09:30', N'Dolor de cabeza', 1),
(49, 3, '2026-03-09', '15:00', N'Consulta pediátrica', 1),
(39, 6, '2026-03-06', '08:00', N'Control anual', 1),
(20, 4, '2026-02-06', '09:30', N'Evaluación preoperatoria', 1),
(6, 8, '2026-02-07', '15:00', N'Consulta pediátrica', 1),
(9, 8, '2026-03-10', '09:00', N'Consulta de seguimiento', 3),
(39, 7, '2026-02-16', '16:00', N'Consulta pediátrica', 1),
(46, 5, '2026-02-28', '14:00', N'Revisión de tratamiento', 1),
(29, 9, '2026-03-03', '08:30', N'Dolor de espalda', 1),
(5, 6, '2026-02-04', '13:30', N'Consulta pediátrica', 1),
(38, 4, '2026-02-03', '08:30', N'Control de presión', 1),
(15, 2, '2026-02-05', '15:30', N'Referencia a especialista', 1),
(33, 4, '2026-02-20', '14:00', N'Vacunación', 1),
(35, 3, '2026-03-11', '13:30', N'Vacunación', 1),
(31, 7, '2026-02-15', '08:30', N'Control anual', 2),
(23, 7, '2026-03-01', '11:30', N'Chequeo de la vista', 1),
(44, 2, '2026-02-06', '11:00', N'Control de presión', 1),
(7, 4, '2026-02-15', '09:30', N'Consulta pediátrica', 2),
(9, 7, '2026-02-14', '10:00', N'Vacunación', 1),
(5, 8, '2026-03-10', '08:30', N'Chequeo general', 3),
(1, 2, '2026-02-18', '09:00', N'Lectura de exámenes', 2),
(31, 4, '2026-02-28', '16:00', N'Chequeo general', 1),
(25, 1, '2026-02-27', '10:00', N'Alergias', 2),
(19, 7, '2026-03-19', '14:30', N'Alergias', 3),
(43, 8, '2026-02-12', '09:30', N'Consulta de seguimiento', 1),
(4, 10, '2026-03-09', '08:00', N'Control de presión', 1),
(4, 1, '2026-03-12', '11:30', N'Consulta pediátrica', 3),
(11, 1, '2026-03-07', '08:30', N'Chequeo de la vista', 1),
(5, 10, '2026-02-07', '14:00', N'Chequeo de la vista', 1),
(26, 2, '2026-03-11', '09:30', N'Evaluación preoperatoria', 3),
(3, 10, '2026-02-08', '11:00', N'Revisión de tratamiento', 3),
(37, 9, '2026-02-23', '16:00', N'Consulta de seguimiento', 1),
(43, 6, '2026-02-18', '10:00', N'Lectura de exámenes', 1),
(43, 5, '2026-03-04', '10:30', N'Alergias', 1),
(1, 8, '2026-03-14', '13:30', N'Control anual', 1),
(35, 4, '2026-03-07', '10:00', N'Dolor de cabeza', 1),
(5, 4, '2026-02-26', '10:00', N'Dolor de cabeza', 2),
(35, 5, '2026-03-14', '15:00', N'Revisión de tratamiento', 3),
(1, 9, '2026-02-22', '16:00', N'Revisión de tratamiento', 1),
(9, 5, '2026-02-10', '16:00', N'Control anual', 3),
(10, 5, '2026-02-21', '13:30', N'Dolor de espalda', 1),
(14, 5, '2026-03-07', '11:30', N'Consulta de seguimiento', 1),
(6, 7, '2026-02-20', '08:00', N'Chequeo general', 1),
(50, 3, '2026-03-15', '10:00', N'Dolor de cabeza', 2),
(36, 7, '2026-03-10', '08:00', N'Control anual', 1),
(45, 3, '2026-03-09', '08:00', N'Chequeo de la vista', 1),
(38, 9, '2026-02-12', '11:00', N'Dolor de cabeza', 1),
(20, 6, '2026-02-05', '16:00', N'Referencia a especialista', 1),
(44, 4, '2026-03-17', '08:30', N'Referencia a especialista', 3),
(27, 10, '2026-02-12', '16:00', N'Dolor de espalda', 1),
(12, 7, '2026-02-04', '09:00', N'Control de presión', 1),
(27, 4, '2026-02-20', '09:00', N'Alergias', 1),
(25, 1, '2026-03-05', '09:30', N'Dolor de espalda', 2),
(23, 5, '2026-02-17', '09:30', N'Chequeo general', 1),
(26, 6, '2026-02-20', '15:30', N'Control anual', 1),
(23, 9, '2026-02-28', '14:00', N'Chequeo de la vista', 3),
(22, 1, '2026-02-10', '16:00', N'Consulta de seguimiento', 1),
(38, 5, '2026-02-05', '08:30', N'Evaluación preoperatoria', 2),
(23, 6, '2026-03-02', '13:30', N'Consulta pediátrica', 1),
(25, 10, '2026-02-15', '10:00', N'Chequeo general', 2),
(1, 9, '2026-03-09', '14:00', N'Control de presión', 1),
(24, 7, '2026-02-07', '14:00', N'Referencia a especialista', 3),
(21, 2, '2026-02-22', '13:00', N'Consulta de seguimiento', 2),
(21, 7, '2026-03-19', '10:00', N'Consulta pediátrica', 1),
(13, 7, '2026-03-17', '11:00', N'Revisión de tratamiento', 1),
(40, 10, '2026-02-22', '11:00', N'Consulta pediátrica', 1),
(20, 5, '2026-02-16', '11:00', N'Alergias', 3),
(39, 6, '2026-03-04', '11:30', N'Vacunación', 1),
(33, 8, '2026-02-13', '14:00', N'Control anual', 1),
(33, 10, '2026-02-24', '08:30', N'Chequeo de la vista', 1),
(44, 5, '2026-02-17', '15:00', N'Dolor de espalda', 1),
(2, 1, '2026-02-18', '11:30', N'Evaluación preoperatoria', 1),
(30, 7, '2026-03-15', '13:30', N'Dolor de espalda', 2),
(32, 7, '2026-02-18', '09:00', N'Revisión de tratamiento', 1),
(49, 2, '2026-03-02', '09:30', N'Dolor de cabeza', 3),
(30, 1, '2026-03-10', '09:30', N'Chequeo de la vista', 1),
(30, 3, '2026-03-04', '14:00', N'Consulta pediátrica', 3),
(39, 6, '2026-03-03', '13:30', N'Chequeo de la vista', 3),
(28, 9, '2026-03-03', '16:00', N'Dolor de cabeza', 2),
(29, 5, '2026-02-18', '15:30', N'Revisión de tratamiento', 1),
(50, 9, '2026-03-06', '14:00', N'Dolor de espalda', 1),
(29, 2, '2026-03-20', '10:00', N'Dolor de espalda', 1),
(22, 6, '2026-03-09', '08:30', N'Dolor de cabeza', 1),
(15, 7, '2026-03-19', '09:00', N'Control de presión', 1),
(5, 7, '2026-03-01', '10:30', N'Consulta pediátrica', 2),
(27, 1, '2026-02-16', '15:30', N'Lectura de exámenes', 2),
(50, 10, '2026-03-19', '08:00', N'Chequeo de la vista', 3),
(25, 8, '2026-02-03', '10:30', N'Consulta de seguimiento', 2),
(27, 9, '2026-03-09', '15:00', N'Evaluación preoperatoria', 1),
(32, 4, '2026-02-20', '11:00', N'Vacunación', 1),
(25, 6, '2026-03-17', '14:00', N'Alergias', 2),
(47, 3, '2026-03-04', '16:00', N'Dolor de cabeza', 3),
(35, 1, '2026-02-28', '13:30', N'Evaluación preoperatoria', 1),
(6, 7, '2026-02-11', '15:30', N'Vacunación', 1),
(4, 5, '2026-02-27', '10:30', N'Dolor de espalda', 2),
(21, 6, '2026-02-27', '10:00', N'Alergias', 2),
(17, 2, '2026-03-05', '08:00', N'Control de presión', 3),
(4, 6, '2026-02-17', '14:00', N'Control anual', 1),
(49, 1, '2026-02-18', '09:30', N'Chequeo de la vista', 1),
(40, 3, '2026-02-18', '09:00', N'Vacunación', 1);
GO

IF OBJECT_ID('dbo.vw_CitasDetalle','V') IS NOT NULL DROP VIEW dbo.vw_CitasDetalle;
GO
CREATE VIEW dbo.vw_CitasDetalle AS
SELECT c.CitaId,
       p.PacienteId, p.Nombre AS Paciente,
       d.DoctorId, d.Nombre AS Doctor,
       e.EspecialidadId, e.Nombre AS Especialidad,
       c.FechaCita, c.HoraCita, c.Motivo, c.Estado
FROM dbo.Cita c
JOIN dbo.Paciente p ON p.PacienteId = c.PacienteId
JOIN dbo.Doctor d   ON d.DoctorId   = c.DoctorId
JOIN dbo.Especialidad e ON e.EspecialidadId = d.EspecialidadId;
GO

-- Consultas rápidas de validación
-- SELECT TOP 10 * FROM dbo.Paciente;
-- SELECT TOP 10 * FROM dbo.Doctor;
-- SELECT COUNT(*) AS TotalCitas FROM dbo.Cita;
-- SELECT TOP 10 * FROM dbo.vw_CitasDetalle ORDER BY FechaCita, HoraCita;

-- Fin del script