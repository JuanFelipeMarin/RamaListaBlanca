----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------CREACION DE TABLAS ------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------

if not exists (select * from sys.objects where name = 'tblAction' and type in (N'U'))
begin
	create table tblAction(
		id int not null primary key identity(1,1),
		ipAddress varchar(100) not null,
		strAction varchar(100) not null,
		actionDate datetime not null,
		used bit not null,
		actionState bit not null,
		modifiedDate datetime null,
		cdgimnasio int null,
		intIdSucursales int null
	)
end
go

if not exists (select * from sys.objects where name = 'tblActionParameters' and type in (N'U'))
begin
	create table tblActionParameters(
		id int not null primary key identity(1,1),
		actionId int not null,
		parameterName varchar(100) not null,
		parameterValue varchar(max) not null,
		dateParameter datetime not null
		constraint FK_tblAction foreign key (actionId) references tblAction(id)
	)
end
go
if not exists (select * from sys.objects where name = 'tblReplicaHuellas' and type in (N'U'))
begin
	create table tblReplicaHuellas(
		intPkId int not null identity(1,1) primary key,
		intIdHuella_Tarjeta int not null,
		idPersona varchar(50) not null,
		ipAddress varchar(100) not null,
		idEmpresa int not null,
		dtmfechaReplica datetime not null,
		bitDelete bit default 0,
		bitHuella bit default 0,
		bitTarje_Pin bit default 0
	)
end
go
if not exists (select * from sys.objects where name = 'tblReplicatedMessages' and type in (N'U'))
begin
	CREATE TABLE [dbo].[tblReplicatedMessages](
	[intPkId] [int] IDENTITY(1,1) NOT NULL,
	[messageId] [int] NOT NULL,
	[ipAddress] [varchar](100) NOT NULL,
	[replicationDate] [datetime] NOT NULL
)
end
go

if not exists (select * from sys.objects where name = 'tblPlanesUsuarioEliminar' and type in (N'U'))
begin
CREATE TABLE tblPlanesUsuarioEliminar (
				cdgimnasio INT NOT NULL,
				plusu_numero_fact INT NOT NULL,
				plusu_identifi_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
				plusu_codigo_plan INT,
				plusu_tiq_disponible INT,
				plusu_fecha_vcto DATETIME,
				plusu_est_anulada BIT,
				plusu_avisado BIT,
				plusu_sucursal INT NOT NULL,
				plusu_fkdia_codigo INT NOT NULL,
				strTipoRegistro VARCHAR(15) NOT NULL,
				pla_codigo INT NOT NULL,
				pla_descripc VARCHAR(50),
				pla_tipo VARCHAR(1),
				bit_Eliminar bit default (0),
				INDEX IX_PlusuIdentifiCliente (plusu_identifi_cliente, cdgimnasio)
			);
end
go
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------FIN CREACION DE TABLAS ------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------CREACION DE INDICES CAMPOS  ------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_gim_listanegra')
BEGIN
    CREATE NONCLUSTERED INDEX IX_gim_listanegra 
    ON gim_listanegra (listneg_floatId, listneg_bitEstado, cdgimnasio);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_planes_usuario_especiales_ListaBlanca')
BEGIN
    CREATE NONCLUSTERED INDEX IX_planes_usuario_especiales_ListaBlanca
    ON gim_planes_usuario_especiales (cdgimnasio, plusu_identifi_cliente);
END
GO
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------FIN CREACION DE INDICES CAMPOS ------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------CREACION DE FUNCIONES ---------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
CREATE OR ALTER FUNCTION [dbo].[fnObtenerRestriccionesWL]
(
    @idGimnasio INT,  
    @intIdSubgrupo INT,
	@strIdCodigoPlan VARCHAR(MAX)
)
RETURNS @tblRestricciones TABLE
(
	LUNES VARCHAR(MAX),
	MARTES VARCHAR(MAX),
	MIERCOLES VARCHAR(MAX),
	JUEVES VARCHAR(MAX),
	VIERNES VARCHAR(MAX),
	SABADO VARCHAR(MAX),
	DOMINGO VARCHAR(MAX),
	FESTIVO VARCHAR(MAX)
)
AS
BEGIN
	
	DECLARE @tmpTabla table (
		LUNES VARCHAR(MAX),
		MARTES VARCHAR(MAX),
		MIERCOLES VARCHAR(MAX),
		JUEVES VARCHAR(MAX),
		VIERNES VARCHAR(MAX),
		SABADO VARCHAR(MAX),
		DOMINGO VARCHAR(MAX),
		FESTIVO VARCHAR(MAX)
	);

	IF @strIdCodigoPlan <> 0
	BEGIN

		INSERT INTO	@tmpTabla (LUNES, MARTES, MIERCOLES, JUEVES, VIERNES, SABADO, DOMINGO, FESTIVO)
		SELECT		DISTINCT
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_lunes_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS LUNES,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_martes_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS MARTES,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_miercoles_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS MIERCOLES,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_jueves_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS JUEVES,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_viernes_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS VIERNES,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_sabado_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS SABADO,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_domingo_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS DOMINGO,
					STRING_AGG(CASE WHEN gim_planes_adicionales.pla_festivo_adicional = 1 THEN CONCAT((convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_desde_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),gim_planes_adicionales.pla_hasta_hora_adicional) + ':' + convert(varchar(10),gim_planes_adicionales.pla_min_hasta_adicional))),114))) ELSE NULL END, ';') AS FESTIVO
		FROM		gim_planes_adicionales
		WHERE		gim_planes_adicionales.pla_codigo_plan = @strIdCodigoPlan
		AND			gim_planes_adicionales.cdgimnasio = @idGimnasio

	END
	
	IF @intIdSubgrupo <> 0
	BEGIN

		INSERT INTO	@tmpTabla (LUNES, MARTES, MIERCOLES, JUEVES, VIERNES, SABADO, DOMINGO, FESTIVO)
		SELECT		DISTINCT
					STRING_AGG(CASE WHEN subgru_adi_lunes_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS LUNES,
					STRING_AGG(CASE WHEN subgru_adi_martes_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS MARTES,
					STRING_AGG(CASE WHEN subgru_adi_miercoles_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS MIERCOLES,
					STRING_AGG(CASE WHEN subgru_adi_jueves_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS JUEVES,
					STRING_AGG(CASE WHEN subgru_adi_viernes_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS VIERNES,
					STRING_AGG(CASE WHEN subgru_adi_sabado_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS SABADO,
					STRING_AGG(CASE WHEN subgru_adi_domingo_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS DOMINGO,
					STRING_AGG(CASE WHEN subgru_adi_festivo_adicional = 1 THEN CONCAT('SG', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_desde_adicional))),114)), '-', (convert(varchar(10),convert(datetime,(convert(varchar(10),subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),subgru_adi_min_hasta_adicional))),114))) ELSE NULL END, ';') AS FESTIVO
		FROM		gim_subgru_adicionales
		WHERE		gim_subgru_adicionales.subgru_adi_codigo_subgru = @intIdSubgrupo
		AND			gim_subgru_adicionales.cdgimnasio = @idGimnasio

	END

	INSERT @tblRestricciones
	SELECT	REPLACE(STRING_AGG(LUNES, ';'), ' ', '') AS LUNES,
			REPLACE(STRING_AGG(MARTES, ';'), ' ', '') AS MARTES,
			REPLACE(STRING_AGG(MIERCOLES, ';'), ' ', '') AS MIERCOLES,
			REPLACE(STRING_AGG(JUEVES, ';'), ' ', '') AS JUEVES,
			REPLACE(STRING_AGG(VIERNES, ';'), ' ', '') AS VIERNES,
			REPLACE(STRING_AGG(SABADO, ';'), ' ', '') AS SABADO,
			REPLACE(STRING_AGG(DOMINGO, ';'), ' ', '') AS DOMINGO,
			REPLACE(STRING_AGG(FESTIVO, ';'), ' ', '') AS FESTIVO
	FROM	@tmpTabla

	RETURN;

END
GO

----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------FIN CREACION DE FUNCIONES -----------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------CREACION DE PROCEDIMIENTOS ----------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spReplicaHuellas' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spReplicaHuellas
END
GO
CREATE PROCEDURE [dbo].[spReplicaHuellas]
@action varchar(50) = '',
@intIdHuella_Tarjeta varchar(max) = '',
@idPersona varchar(max) = '',
@ipAddress varchar(max) = '',
@idEmpresa int = 0,
@bitDelete bit = 0,
@bitHuella bit = 0,
@bitTarje_Pin bit = 0

as
begin
if (@action = 'obtenerHuellasReplicas')
	begin

		select *
		from gim_huellas fin
		inner join WhiteList whiteList on fin.hue_identifi = whiteList.id
		where fin.hue_id not in (select intIdHuella_Tarjeta 
								 from tblReplicaHuellas rf
								 where rf.ipAddress = @ipAddress
								 and rf.bitDelete = 0
								 and bitHuella = 1)
		

	end

IF(@action = 'insertarHuellasReplicas')
BEGIN

insert into tblReplicaHuellas (intIdHuella_Tarjeta,idPersona,ipAddress,idEmpresa,dtmfechaReplica,bitDelete,bitHuella,bitTarje_Pin)
values (@intIdHuella_Tarjeta,@idPersona,@ipAddress,@idEmpresa,getdate(),@bitDelete,@bitHuella,@bitTarje_Pin)
END 

IF(@action = 'EliminarHuellasReplicas')
BEGIN

----pARTE 1 LISTAR HUELLAS DE PERSONAS QUE HAYAN SIDO REPLICADAS Y YA NO EXISTAN EN LISTA BLANCA 
select tblReplicaHuellas.*
from gim_huellas 
left join WhiteList on WhiteList.id = gim_huellas.hue_identifi
inner join tblReplicaHuellas on gim_huellas.hue_id = tblReplicaHuellas.intIdHuella_Tarjeta
	and gim_huellas.hue_identifi = tblReplicaHuellas.idPersona
where WhiteList.id is null and tblReplicaHuellas.bitDelete = 0
and tblReplicaHuellas.ipAddress = @ipAddress
UNION 
----PARTE DOS PERSONAS QUE FUERON MARCADAS COMO ELIMINAR POR DURANTE ALGUNA VALIDACION 
select tblReplicaHuellas.*
from gim_huellas 
left join WhiteList on WhiteList.id = gim_huellas.hue_identifi
inner join tblReplicaHuellas on gim_huellas.hue_id = tblReplicaHuellas.intIdHuella_Tarjeta
	and gim_huellas.hue_identifi = tblReplicaHuellas.idPersona
where WhiteList.id is NOT null and tblReplicaHuellas.bitDelete = 0
and tblReplicaHuellas.ipAddress = @ipAddress
AND personState = 'Eliminar'



END 

IF(@action = 'cambioEstadoEliminarHuellasReplicas')
BEGIN
	UPDATE tblReplicaHuellas
	SET bitDelete = 1
	WHERE idPersona = @idPersona
	AND intIdHuella_Tarjeta = @intIdHuella_Tarjeta
	and ipAddress = @ipAddress

END 

IF(@action = 'EliminarReplicaPersona')
BEGIN

delete from tblReplicaHuellas 
where intIdHuella_Tarjeta = @intIdHuella_Tarjeta and idPersona = @idPersona


END 


end

GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spGenerateWhiteListNuevo' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spGenerateWhiteListNuevo
END
GO
CREATE  PROCEDURE [dbo].[spGenerateWhiteListNuevo]
AS
BEGIN

	UPDATE	gim_con_fac 
    SET		des_con = 1,
			con_modificado = 1,
			bits_replica = '111111111111111111111111111111'   
    WHERE	des_con = 0
	AND		fec_ter_con <= GETDATE()

    UPDATE	gim_con_fac_esp 
    SET		des_con = 1,
			con_esp_modificado = 1,
			bits_replica = '111111111111111111111111111111'  
    WHERE	des_con = 0
	AND		fec_ter_con <= GETDATE()

	UPDATE		gim_planes_usuario 
    SET			gim_planes_usuario.plusu_avisado = 1,
				gim_planes_usuario.plusu_modificado = 1,
				gim_planes_usuario.bits_replica = '111111111111111111111111111111'
    FROM		gim_planes_usuario
	INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario.plusu_codigo_plan
	AND			gim_planes.cdgimnasio = gim_planes_usuario.cdgimnasio
	LEFT JOIN	gim_clientes ON gim_clientes.cli_identifi = gim_planes_usuario.plusu_identifi_cliente
	AND			gim_clientes.cdgimnasio = gim_planes_usuario.cdgimnasio
    WHERE		plusu_avisado = 0
	AND			((gim_planes.pla_tipo = 'T' AND plusu_tiq_disponible <= 0) OR (CONVERT(VARCHAR(10), (DATEADD(DAY, gim_clientes.cli_dias_gracia, gim_planes_usuario.plusu_fecha_vcto)), 111) <= CONVERT(VARCHAR(10),DATEADD(DAY, -1, GETDATE()),111)))

	UPDATE		gim_planes_usuario_especiales 
    SET			gim_planes_usuario_especiales.plusu_avisado = 1,
				gim_planes_usuario_especiales.plusu_modificado = 1,
				gim_planes_usuario_especiales.bits_replica = '111111111111111111111111111111'
    FROM		gim_planes_usuario_especiales
	INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario_especiales.plusu_codigo_plan
	AND			gim_planes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
	LEFT JOIN	gim_clientes ON gim_clientes.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente
	AND			gim_clientes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
    WHERE		plusu_avisado = 0
	AND			((gim_planes.pla_tipo = 'T' AND plusu_tiq_disponible <= 0) OR (CONVERT(VARCHAR(10), (DATEADD(DAY, gim_clientes.cli_dias_gracia, gim_planes_usuario_especiales.plusu_fecha_vcto)), 111) <= CONVERT(VARCHAR(10),DATEADD(DAY, -1, GETDATE()),111)))


	UPDATE	gim_clientes
    SET		cli_bitAutorizacionM = 0,
			cli_modificado = 1,
			bits_replica = '111111111111111111111111111111' 
    WHERE	cli_bitAutorizacionM=1
	AND		(DATEDIFF(YY, cli_dtmFechaAutorizacionM, GETDATE()) - CASE WHEN DATEADD(YY, DATEDIFF(YY, cli_dtmFechaAutorizacionM, GETDATE()), cli_dtmFechaAutorizacionM) > GETDATE() THEN 1 ELSE 0 END) >= 1

	TRUNCATE TABLE WhiteList
	truncate table tblPlanesUsuarioEliminar

	DECLARE @intCantidadGimnasios INT = 0;
	DECLARE @intRegistroGimnasioActual INT = 0;
	DECLARE @intIdGimnasio INT = 0;
	DECLARE @intMayoriaEdad INT = 0;
	DECLARE @intCantidadSucursales INT = 0;
	DECLARE @intRegistroSucursalActual INT = 0;
	DECLARE @intIdSucursal INT = 0;

	DECLARE @bitValidarContrato BIT = 0;
	DECLARE @bitValidarContratoEntrenamiento BIT = 0;
	DECLARE @bitValidarReglamentoDeUso BIT = 0;
	DECLARE @bitValidarInfoCita BIT = 0;
	DECLARE @bitValidarCitaNoCumplida BIT = 0;
	DECLARE @bitValidarClienteNoApto BIT = 0;
	DECLARE @bitValidarNoDisentimento BIT = 0;
	DECLARE @bitValidarAutorizacionMenor BIT = 0;
	DECLARE @intDiasGraciaCitaMedicaIngreso INT = 0;
	DECLARE @intEntradasCitaMedicaIngreso INT = 0;
	DECLARE @bitValidarConsentimientoDatosBiometricos BIT = 0;
	DECLARE @bitValidarConsentimientoInformado BIT = 0;
	DECLARE @bitValidarCarnetCodiv19 BIT = 0;
	DECLARE @bitValidarIngresoSinPlanEmpleado BIT = 0;
	DECLARE @bitValideContPorFactura BIT = 0;
	
	DECLARE @bitValidaReservaWeb BIT = 0;
	DECLARE @bitValidarPlanYReservaWeb BIT = 0;
	DECLARE @bitValidarDatosVitales BIT = 0;

	DECLARE @intMinutosAntesReserva INT = 0;
	DECLARE @intMinutosDespuesReserva INT = 0;

	SET @intCantidadGimnasios = (SELECT COUNT(*) FROM tblConfiguracion WHERE ISNULL(cdgimnasio, 0) != 0 AND bitIngressWhiteList = 1)
	
	-- CURSOR PARA LOS GIMNASIOS CREADOS EN LA BASE DE DATOS
	DECLARE cursorGimnasios CURSOR FOR
		SELECT	DISTINCT cdgimnasio, ISNULL(intAnosMayoriaEdad,0) AS intMayoriaEdad
		FROM	tblConfiguracion	
		WHERE	ISNULL(cdgimnasio, 0) != 0 AND bitIngressWhiteList = 1
    OPEN cursorGimnasios

		CREATE TABLE #tmpClientes(
			[cli_identifi] [varchar](max)  NULL,
			[cli_nombres] [varchar](max) ,
			[cli_primer_apellido] [varchar](max) NULL,
			[cli_segundo_apellido] [varchar](max) NULL,
			[cli_dias_gracia] [int] NULL,
			[cdgimnasio] [int]  NULL,
			[cli_sucursal] [int] null,
			[cli_intcodigo_subgrupo] [int] NULL,
			[cli_sin_huella] [bit]  NULL,
			[cli_GrupoFamiliar] [bit] NULL,
			[cli_strcodtarjeta] [varchar](max) NULL,
			[hue_id] [int] NULL,
			[hue_dato] [binary](3000) NULL,
			[strDatoFoto] [varchar](max) NULL,
			[bitCarnetdevacunaciondeCOVID19] [bit] NULL,
			[IMGCarnetdevacunaciondeCOVID19] [image] NULL,
			[cli_estado] [bit] NULL,
			[listneg_floatId] [float] NULL,
			[cli_Apto] [bit] NULL,
			[cli_altoRiesgo] [bit] NULL,
			[cli_imgdisentimiento] [image] NULL,
			[cli_bitAutorizacionM] [bit] NULL,
			[cli_fecha_nacimien] [datetime] NULL,
			[cli_nombre_Allamar] [varchar](max) NULL,
			[cli_telefono_emergencia] [varchar](max) NULL,
			[cli_rh] [varchar](50) NULL,
			[cli_cod_como_supo] [int] NULL,
			[cre_fecha] [datetime] NULL,
			[cre_anulado] [bit] NULL,
			[cre_pagado] [bit] NULL,
			[cre_identifi] [float] NULL,
			[bitError] [bit] NULL,
			[mensaje] [varchar](max) NULL
		)



	-- CICLO PARA RECORRER LOS REGISTROS DE LOS GIMNASIOS EN LA BASE DE DATOS
	WHILE @intRegistroGimnasioActual < @intCantidadGimnasios
	BEGIN
		
		-- INSERTO EN LAS VARIABLES LOS DATOS QUE CAPTURE CON EL CURSOR
		FETCH NEXT FROM cursorGimnasios
		INTO @intIdGimnasio, @intMayoriaEdad

		----- contrato 
		SELECT	TOP(1)
				@bitValidarContrato = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 1 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarContratoEntrenamiento = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 2 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarConsentimientoInformado = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 3 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValideContPorFactura = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 4 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarConsentimientoDatosBiometricos = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 5 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarReglamentoDeUso = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 6 AND cdgimnasio = @intIdGimnasio;

	
		CREATE TABLE #tmpProspecto(
			[cli_identifi] [float] NOT NULL,
			[cdgimnasio] [int] NOT NULL,
			[cli_nombres] [varchar](50) NOT NULL,
			[cli_primer_apellido] [varchar](50) NOT NULL,
			[cli_segundo_apellido] [varchar](50) NULL,
			[cli_cortesia] [bit] NOT NULL,
			[cli_entro_cortesia] [bit] NOT NULL,
			[cli_entrar_conocer] [bit] NOT NULL,
			[cli_EntryFingerprint] [bit] NULL,
			[cli_intfkSucursal] [int] NULL,
			[hue_id] [int] NULL,
			[hue_dato] [binary](3000) NULL,
			[strDatoFoto] [varchar](max) NULL,
			[bitCarnetdevacunaciondeCOVID19] [bit] NULL,
			[IMGCarnetdevacunaciondeCOVID19] [image] NULL,
			[listneg_floatId] [float] NULL,
			[bitError] [bit] NULL,
			[mensaje] [varchar](max) NULL
		) 

		CREATE TABLE #tmpVisitante(
			[vis_strVisitorId] [varchar](50) NOT NULL,
			[cdgimnasio] [int] NOT NULL,
			[vis_strName] [varchar](max) NOT NULL,
			[vis_strFirstLastName] [varchar](max) NOT NULL,
			[vis_strSecondLastName] [varchar](max) NULL,
			[bitCortesia] [bit] NOT NULL,
			[vis_EntryFingerprint] [bit] NULL,
			[vis_intBranch] [int] NULL,
			[hue_id] [int] NULL,
			[hue_dato] [binary](3000) NULL,
			[strDatoFoto] [varchar](max) NULL,
			[bitCarnetCOVID19] [bit] NOT NULL,
			[imgCarnetCOVID19] [image] NULL,
			[listneg_floatId] [float] NOT NULL,
			[bitError] [bit] NULL,
			[mensaje] [varchar](max) NULL
		) 

		CREATE TABLE #tmpEmpleados(
			[emp_identifi] [varchar](15) NOT NULL,
			[cdgimnasio] [int] NOT NULL,
			[emp_nombre] [varchar](50) NULL,
			[emp_primer_apellido] [varchar](50) NULL,
			[emp_segundo_apellido] [varchar](50) NULL,
			[emp_sin_huella] [bit] NULL,
			[emp_strcodtarjeta] [varchar](20) NULL,
			[emp_sucursal] [int] NULL,
			[hue_id] [int] NULL,
			[hue_dato] [binary](3000) NULL,
			[strDatoFoto] [varchar](max) NULL,
			[bitCarnetdevacunaciondeCOVID19] [bit] NULL,
			[IMGCarnetdevacunaciondeCOVID19] [image] NULL,
			[emp_estado] [bit] NULL,
			[listneg_floatId] [float] NULL,
			[bitError] [bit] NULL,
			[mensaje] [varchar](max) NULL
		) 


		CREATE TABLE #tmpDatosContratos (
			dtcont_FKcontrato INT NOT NULL,
			dtcont_doc_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
		)

		INSERT INTO	#tmpDatosContratos
		SELECT		DISTINCT
					gim_detalle_contrato.dtcont_FKcontrato,
					gim_detalle_contrato.dtcont_doc_cliente
		FROM		gim_detalle_contrato
		WHERE		gim_detalle_contrato.cdgimnasio = @intIdGimnasio

		SET @intCantidadSucursales = (SELECT COUNT(*) FROM gim_sucursales WHERE cdgimnasio = @intIdGimnasio AND suc_bitActiva = 1 AND ISNULL(suc_intpkIdentificacion, 0) != 0)

		DECLARE cursorSucursales CURSOR FOR
			SELECT	DISTINCT suc_intpkIdentificacion
			FROM	gim_sucursales
			WHERE	cdgimnasio = @intIdGimnasio
			AND		suc_bitActiva = 1
			AND		ISNULL(suc_intpkIdentificacion, 0) != 0
		OPEN cursorSucursales

		WHILE @intRegistroSucursalActual < @intCantidadSucursales
		BEGIN

		FETCH NEXT FROM cursorSucursales
			INTO @intIdSucursal

			PRINT('COMIENZA LA CONSULTA DE PARAMETROS DE INGRESO - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			SELECT	TOP(1)
					@bitValidarInfoCita = ISNULL(bitConsultaInfoCita,0),
					@bitValidarCitaNoCumplida = ISNULL(bitBloqueoCitaNoCumplidaMSW, 0),
					@bitValidarClienteNoApto = ISNULL(bitBloqueoClienteNoApto,0),
					@bitValidarNoDisentimento = ISNULL(bitBloqueoNoDisentimento,0),
					@bitValidarAutorizacionMenor = ISNULL(bitBloqueoNoAutorizacionMenor,0),
					@intDiasGraciaCitaMedicaIngreso = ISNULL(intdiassincita_bloqueoing,0),
					@intEntradasCitaMedicaIngreso = ISNULL(intentradas_sincita_bloqueoing,0),
					@bitValidarCarnetCodiv19 = ISNULL(bitCargadecarnetdevacunaciondeCOVID19, 0),
					@bitValidarIngresoSinPlanEmpleado = ISNULL(bitIngresoEmpSinPlan, 0),
					@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
					@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0),
					@intMinutosAntesReserva = ISNULL(gim_configuracion_ingreso.intMinutosAntesReserva, 0),
					@intMinutosDespuesReserva = ISNULL(gim_configuracion_ingreso.intMinutosDespuesReserva, 0),
					@bitValidarDatosVitales = ISNULL(gim_configuracion_ingreso.bitDatosVirtualesUsuario, 0)
			FROM	gim_configuracion_ingreso
			WHERE	cdgimnasio = @intIdGimnasio
			AND		intfkSucursal = @intIdSucursal

			PRINT('FINALIZA LA CONSULTA DE PARAMETROS DE INGRESO - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			PRINT('COMIENZAN LAS VALIDACIONES PARA CLIENTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			insert into #tmpClientes
			SELECT		gim_clientes.cli_identifi COLLATE Modern_Spanish_CI_AS,
						gim_clientes.cli_nombres,
						gim_clientes.cli_primer_apellido,
						gim_clientes.cli_segundo_apellido,
						gim_clientes.cli_dias_gracia,
						gim_clientes.cdgimnasio,
						gim_clientes.cli_sucursal,
						gim_clientes.cli_intcodigo_subgrupo,
						gim_clientes.cli_sin_huella,
						gim_clientes.cli_GrupoFamiliar,
						gim_clientes.cli_strcodtarjeta,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_id end as hue_id,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_dato end as hue_dato,

						gim_huellas.strDatoFoto
						,bitCarnetdevacunaciondeCOVID19
						,IMGCarnetdevacunaciondeCOVID19
						,cli_estado
						,gim_listanegra.listneg_floatId
						,isnull(gim_clientes.cli_Apto ,0) as cli_Apto
						,gim_clientes.cli_altoRiesgo
						,gim_clientes.cli_imgdisentimiento
						,isnull(gim_clientes.cli_bitAutorizacionM,0) as cli_bitAutorizacionM
						,gim_clientes.cli_fecha_nacimien
						,gim_clientes.cli_nombre_Allamar
						,gim_clientes.cli_telefono_emergencia
						,gim_clientes.cli_rh
						,gim_clientes.cli_cod_como_supo
						,gim_creditos.cre_fecha
						,gim_creditos.cre_anulado
						,gim_creditos.cre_pagado
						,gim_creditos.cre_identifi
						,cast(0 as bit) as bitError
						, CAST('' AS VARCHAR(MAX)) as mensaje
			FROM		gim_clientes
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_clientes.cli_identifi
			AND			gim_huellas.cdgimnasio = gim_clientes.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON listneg_floatId = gim_clientes.cli_identifi
			AND			gim_listanegra.listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = gim_clientes.cdgimnasio
			LEFT JOIN	gim_creditos ON gim_creditos.cre_identifi = gim_clientes.cli_identifi
			AND			gim_creditos.cdgimnasio = gim_clientes.cdgimnasio
			WHERE		gim_clientes.cdgimnasio = @intIdGimnasio
			AND			gim_clientes.cli_sucursal = @intIdSucursal

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI EL CLIENTE ESTA INACTIVA'
			where bitError = 0 
			and cli_estado = 0

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE SEA APTO'
			where bitError = 0
			and (@bitValidarClienteNoApto = 1 AND ISNULL(cli_Apto, 0) = 1) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI EL CLIENTE TIENE CARNET DE VACUNACION COVID-19'
			where bitError = 0
			and(@bitValidarCarnetCodiv19 = 1 AND bitCarnetdevacunaciondeCOVID19 = 0 AND IMGCarnetdevacunaciondeCOVID19 IS NULL) 
		
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR INGRESO CLIENTE CON HUELLA'
			where  bitError = 0
			and (cli_sin_huella = 0 AND (ISNULL(hue_id, 0) != 0 OR ISNULL(cli_strcodtarjeta, '') != '')) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA'
			where  bitError = 0 and listneg_floatId IS not NULL
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO SEA DE ALTO RIESGO'
			where bitError = 0 and ISNULL(cli_altoRiesgo, 0) = 1
			 
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR BLOQUEO POR NO DISENTIMIENTO'
			where bitError = 0 and (@bitValidarNoDisentimento = 1 AND NOT cli_imgdisentimiento IS NULL) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR BLOQUEO POR NO AUTORIZACION MENOR DE EDAD'
			where bitError = 0 
			and (@bitValidarAutorizacionMenor = 1 AND cli_bitAutorizacionM = 1 AND cli_fecha_nacimien > DATEADD(YEAR, -@intMayoriaEdad, GETDATE())) 
		
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE TENGA CONTRATO FIRMADO PARA EL GIMNASIO'
			where bitError = 0 and (@bitValidarContrato = 1  AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE	#tmpDatosContratos.dtcont_FKcontrato = 1  AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS )) 
			 
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE TENGA CONTRATO FIRMADO PARA EL ENTRENAMIENTO'
			where bitError = 0 and (@bitValidarContratoEntrenamiento = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE	#tmpDatosContratos.dtcont_FKcontrato = 2 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE CONSENTIMIENTO INFORMADO'
			where bitError = 0 and (@bitValidarConsentimientoInformado = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 3 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE CONSENTIMIENTO DE DATOS BIOMETRICOS'
			where bitError = 0 and (@bitValidarConsentimientoDatosBiometricos = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 5 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE FIRMADO EL REGLAMENTO DE USO'
			where bitError = 0 and (@bitValidarReglamentoDeUso = 1 AND EXISTS(SELECT TOP(1) 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 6 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE DATOS VITALES REGISTRADOS'
			where  bitError = 0 AND
						(
						@bitValidarDatosVitales = 1	
						AND( ISNULL(cli_nombre_Allamar, '') = ''
						OR ISNULL(cli_telefono_emergencia, '') = ''
						OR ISNULL(cli_rh, '') = ''
						OR ISNULL(cli_cod_como_supo, 0) = 0)
					)
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO TENGA CREDITOS PENDIENTES'
			where bitError = 0 AND (cre_fecha > GETDATE() AND cre_anulado = 0 AND cre_pagado = 0)

			---------------------------------------------------------------------------------------------------------
			---------------------------------------------------------------------------------------------------------
			----------------------------------------------------------------------------------------------------------

			INSERT INTO #tmpProspecto
			SELECT	gim_clientes_especiales.cli_identifi,
						gim_clientes_especiales.cdgimnasio,
						gim_clientes_especiales.cli_nombres,
						gim_clientes_especiales.cli_primer_apellido,
						gim_clientes_especiales.cli_segundo_apellido,
						gim_clientes_especiales.cli_cortesia,
						gim_clientes_especiales.cli_entro_cortesia,
						gim_clientes_especiales.cli_entrar_conocer,
						gim_clientes_especiales.cli_EntryFingerprint,
						gim_clientes_especiales.cli_intfkSucursal,
						gim_huellas.hue_id,
						gim_huellas.hue_dato,
						gim_huellas.strDatoFoto
						,gim_clientes_especiales.bitCarnetdevacunaciondeCOVID19
						,gim_clientes_especiales.IMGCarnetdevacunaciondeCOVID19
						,gim_listanegra.listneg_floatId
						,cast(0 as bit) as bitError
						, cast('' as varchar(max)) as mensaje
			
			FROM		gim_clientes_especiales
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_clientes_especiales.cli_identifi
			AND			gim_huellas.cdgimnasio = gim_clientes_especiales.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON gim_listanegra.listneg_floatId = gim_clientes_especiales.cli_identifi 
			AND			gim_listanegra.listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = gim_clientes_especiales.cdgimnasio
			WHERE		gim_clientes_especiales.cdgimnasio = @intIdGimnasio
			AND			gim_clientes_especiales.cli_intfkSucursal = @intIdGimnasio


			UPDATE #tmpProspecto
			set bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA PROSPECTO' 
			WHERE bitError = 0  AND (cli_EntryFingerprint = 1 AND hue_id IS NOT NULL) 

			UPDATE #tmpProspecto
			set bitError = 1 , mensaje = 'VALIDAR SI EL PROSPECTO TIENE CARNET DE VACUNACION COVID-19)' 
			WHERE bitError = 0  AND (@bitValidarCarnetCodiv19 = 1 AND bitCarnetdevacunaciondeCOVID19 = 0 AND IMGCarnetdevacunaciondeCOVID19 IS NULL) 

			UPDATE #tmpProspecto
			set bitError = 1 , mensaje = 'VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA' 
			where  bitError = 0 and listneg_floatId IS NOT NULL

			-----------------------------------------------------------------------------------------------------------------------------
			----------------------------------------------------------------------------------------------------------------------------

			INSERT INTO #tmpVisitante
			SELECT		Visitors.vis_strVisitorId,
						Visitors.cdgimnasio,
						Visitors.vis_strName,
						Visitors.vis_strFirstLastName,
						Visitors.vis_strSecondLastName,
						Visitors.bitCortesia,
						Visitors.vis_EntryFingerprint,
						Visitors.vis_intBranch,
						gim_huellas.hue_id,
						gim_huellas.hue_dato,
						gim_huellas.strDatoFoto
						,Visitors.bitCarnetCOVID19
						,isnull(Visitors.imgCarnetCOVID19,'') as imgCarnetCOVID19
						,isnull(gim_listanegra.listneg_floatId,0) as listneg_floatId
						,cast(0 as bit) as bitError
						, cast('' as varchar(max)) as mensaje
			
			FROM		Visitors
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = Visitors.vis_strVisitorId
			AND			gim_huellas.cdgimnasio = Visitors.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON listneg_floatId = Visitors.vis_strVisitorId
			AND			listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = Visitors.cdgimnasio
			WHERE		Visitors.cdgimnasio = @intIdGimnasio
			AND			Visitors.vis_intBranch = @intIdSucursal


						UPDATE #tmpVisitante
						SET bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA VISITANTE'
						WHERE bitError = 0  AND (vis_EntryFingerprint = 1 AND hue_id IS NOT NULL)

						UPDATE #tmpVisitante
						SET bitError = 1 , mensaje = 'VALIDAR SI EL VISITANTE TIENE CARNET DE VACUNACION COVID-19'
						WHERE bitError = 0  AND ((@bitValidarCarnetCodiv19 = 1 AND ISNULL(bitCarnetCOVID19, 0) = 0 AND NOT imgCarnetCOVID19 IS NULL) OR @bitValidarCarnetCodiv19 = 0)
										
						UPDATE #tmpVisitante
						SET bitError = 1 , mensaje = 'VALIDAR QUE EL VISITANTE NO ESTE EN LA LISTA NEGRA'
						WHERE bitError = 0  and isnull(listneg_floatId,0) <> 0

						-----------------------------------------------------------------------------------------
						-----------------------------------------------------------------------------------------

						INSERT INTO #tmpEmpleados
						SELECT	gim_empleados.emp_identifi,
						gim_empleados.cdgimnasio,
						gim_empleados.emp_nombre,
						gim_empleados.emp_primer_apellido,
						gim_empleados.emp_segundo_apellido,
						gim_empleados.emp_sin_huella,
						gim_empleados.emp_strcodtarjeta,
						gim_empleados.emp_sucursal,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_id end as hue_id,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_dato end as hue_dato,
						
						--gim_huellas.hue_id,
						--gim_huellas.hue_dato,
						gim_huellas.strDatoFoto
						,bitCarnetdevacunaciondeCOVID19
						,IMGCarnetdevacunaciondeCOVID19
						,emp_estado
						,listneg_floatId
						,cast(0 as bit) as bitError
						, cast('' as varchar(max)) as mensaje
			FROM		gim_empleados
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_empleados.emp_identifi
			AND			gim_huellas.cdgimnasio = gim_empleados.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON listneg_floatId = gim_empleados.emp_identifi
			AND			listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = gim_empleados.cdgimnasio
			WHERE		gim_empleados.cdgimnasio = @intIdGimnasio
			AND			gim_empleados.emp_sucursal = @intIdGimnasio
			
			
			UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA EMPLEADO'
			WHERE bitError = 0 AND (emp_sin_huella = 0 AND hue_id IS NOT NULL) 
			
			UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR SI EL EMPLEADO TIENE CARNET DE VACUNACION COVID-19'
			WHERE bitError = 0 AND (@bitValidarCarnetCodiv19 = 1 AND ISNULL(bitCarnetdevacunaciondeCOVID19, 0) = 0 )--AND NOT IMGCarnetdevacunaciondeCOVID19 IS NULL)

			UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR SI EL EMPLEADO ESTA ACTIVA'
			WHERE bitError = 0 AND ISNULL(emp_estado, 0)  = 0
			
						UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR QUE EL EMPLEADO NO ESTE EN LA LISTA NEGRA'
			WHERE bitError = 0 AND isnull(listneg_floatId,0) <> 0

			CREATE TABLE #tmpCortesiasCongeladas (
				invoiceId INT
			)

			INSERT INTO #tmpCortesiasCongeladas (invoiceId)
			SELECT gim_con_fac_esp.num_fac_con
			FROM gim_con_fac_esp
			WHERE gim_con_fac_esp.des_con = 0
				AND CONVERT(varchar(10), GETDATE(), 111) BETWEEN CONVERT(varchar(10), gim_con_fac_esp.fec_ini_con, 111) AND CONVERT(varchar(10), gim_con_fac_esp.fec_ter_con, 111)
				AND gim_con_fac_esp.cdgimnasio = @intIdGimnasio
				AND gim_con_fac_esp.con_intfkSucursal = @intIdSucursal

			CREATE TABLE #tmpPlanesUsuario (
				cdgimnasio INT NOT NULL,
				plusu_numero_fact INT NOT NULL,
				plusu_identifi_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
				plusu_codigo_plan INT,
				plusu_tiq_disponible INT,
				plusu_fecha_vcto DATETIME,
				plusu_est_anulada BIT,
				plusu_avisado BIT,
				plusu_sucursal INT NOT NULL,
				plusu_fkdia_codigo INT NOT NULL,
				strTipoRegistro VARCHAR(15) NOT NULL,
				pla_codigo INT NOT NULL,
				pla_descripc VARCHAR(50),
				pla_tipo VARCHAR(1),
				INDEX IX_PlusuIdentifiCliente (plusu_identifi_cliente, cdgimnasio)
			);

				
			INSERT INTO	#tmpPlanesUsuario
			SELECT		DISTINCT gim_planes_usuario.cdgimnasio,
						gim_planes_usuario.plusu_numero_fact,
						gim_planes_usuario.plusu_identifi_cliente,
						gim_planes_usuario.plusu_codigo_plan,
						gim_planes_usuario.plusu_tiq_disponible,
						gim_planes_usuario.plusu_fecha_vcto,
						gim_planes_usuario.plusu_est_anulada,
						gim_planes_usuario.plusu_avisado,
						gim_planes_usuario.plusu_sucursal,
						gim_planes_usuario.plusu_fkdia_codigo,
						'Factura',
						gim_planes.pla_codigo,
						gim_planes.pla_descripc,
						gim_planes.pla_tipo
			FROM		gim_planes_usuario WITH(INDEX(IX_gim_planes_usuario_cdgimnasio_plusu_identifi_cliente))
			LEFT JOIN	#tmpClientes ON #tmpClientes.cli_identifi = gim_planes_usuario.plusu_identifi_cliente collate Modern_Spanish_CI_AS
			AND			#tmpClientes.cdgimnasio = gim_planes_usuario.cdgimnasio
			INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario.plusu_codigo_plan
			AND			gim_planes.cdgimnasio = gim_planes_usuario.cdgimnasio
			WHERE		gim_planes_usuario.plusu_avisado = 0
			AND			gim_planes_usuario.plusu_est_anulada = 0
			AND			gim_planes_usuario.plusu_codigo_plan != 999
			AND			gim_planes_usuario.plusu_sucursal = @intIdSucursal
			AND			gim_planes_usuario.cdgimnasio = @intIdGimnasio
			AND			CONVERT(VARCHAR(10), gim_planes_usuario.plusu_fecha_inicio, 111) collate SQL_Latin1_General_CP1_CI_AS <= CONVERT(VARCHAR(10), GETDATE(),111) 
	

			INSERT INTO	#tmpPlanesUsuario
			SELECT		DISTINCT
						gim_planes_usuario_especiales.cdgimnasio,
						gim_planes_usuario_especiales.plusu_numero_fact,
						gim_planes_usuario_especiales.plusu_identifi_cliente,
						gim_planes_usuario_especiales.plusu_codigo_plan,
						gim_planes_usuario_especiales.plusu_tiq_disponible,
						gim_planes_usuario_especiales.plusu_fecha_vcto,
						gim_planes_usuario_especiales.plusu_est_anulada,
						gim_planes_usuario_especiales.plusu_avisado,
						gim_planes_usuario_especiales.plusu_sucursal,
						gim_planes_usuario_especiales.plusu_fkdia_codigo,
						'Cortes�a',
						gim_planes.pla_codigo,
						gim_planes.pla_descripc,
						gim_planes.pla_tipo
			FROM		gim_planes_usuario_especiales WITH (INDEX(IX_planes_usuario_especiales_ListaBlanca))
			LEFT JOIN #tmpClientes ON #tmpClientes.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente collate Modern_Spanish_CI_AS
			AND #tmpClientes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	#tmpProspecto ON #tmpProspecto.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente AND #tmpProspecto.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	#tmpEmpleados ON #tmpEmpleados.emp_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente  collate Modern_Spanish_CI_AS
			AND #tmpEmpleados.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario_especiales.plusu_codigo_plan AND gim_planes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			WHERE		gim_planes_usuario_especiales.plusu_avisado = 0
			AND			gim_planes_usuario_especiales.plusu_est_anulada = 0
			AND			gim_planes_usuario_especiales.plusu_codigo_plan != 999
			AND			gim_planes_usuario_especiales.plusu_sucursal = @intIdSucursal
			AND			gim_planes_usuario_especiales.cdgimnasio = @intIdGimnasio
			AND			CONVERT(VARCHAR(10), gim_planes_usuario_especiales.plusu_fecha_inicio, 111) <= CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			(gim_planes_usuario_especiales.plusu_numero_fact IS NULL OR gim_planes_usuario_especiales.plusu_numero_fact NOT IN (SELECT invoiceId FROM #tmpCortesiasCongeladas))
	
	
			INSERT INTO	tblPlanesUsuarioEliminar
			SELECT		DISTINCT gim_planes_usuario.cdgimnasio,
						gim_planes_usuario.plusu_numero_fact,
						gim_planes_usuario.plusu_identifi_cliente,
						gim_planes_usuario.plusu_codigo_plan,
						gim_planes_usuario.plusu_tiq_disponible,
						gim_planes_usuario.plusu_fecha_vcto,
						gim_planes_usuario.plusu_est_anulada,
						gim_planes_usuario.plusu_avisado,
						gim_planes_usuario.plusu_sucursal,
						gim_planes_usuario.plusu_fkdia_codigo,
						'Factura',
						gim_planes.pla_codigo,
						gim_planes.pla_descripc,
						gim_planes.pla_tipo
						,case when tblReplicaHuellas.bitDelete = 0 then 1 
						else 0 end 
			FROM		gim_planes_usuario WITH(INDEX(IX_gim_planes_usuario_cdgimnasio_plusu_identifi_cliente))
			LEFT JOIN	#tmpClientes ON #tmpClientes.cli_identifi = gim_planes_usuario.plusu_identifi_cliente collate Modern_Spanish_CI_AS
			AND			#tmpClientes.cdgimnasio = gim_planes_usuario.cdgimnasio
			INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario.plusu_codigo_plan
			left join   tblReplicaHuellas ON gim_planes_usuario.plusu_identifi_cliente collate Modern_Spanish_CI_AS = tblReplicaHuellas.idPersona collate Modern_Spanish_CI_AS and tblReplicaHuellas.bitDelete = 0
			AND			gim_planes.cdgimnasio = gim_planes_usuario.cdgimnasio
			WHERE		gim_planes_usuario.plusu_avisado = 0
			AND			gim_planes_usuario.plusu_est_anulada = 0
			AND			gim_planes_usuario.plusu_codigo_plan != 999
			AND			gim_planes_usuario.plusu_sucursal = @intIdSucursal
			AND			gim_planes_usuario.cdgimnasio = @intIdGimnasio
			AND			CONVERT(VARCHAR(10), gim_planes_usuario.plusu_fecha_vcto, 111) collate SQL_Latin1_General_CP1_CI_AS <= CONVERT(VARCHAR(10), GETDATE(),111) 
			and			(CONVERT(VARCHAR(10), gim_planes_usuario.plusu_fecha_vcto, 111) collate SQL_Latin1_General_CP1_CI_AS <= CONVERT(VARCHAR(10), GETDATE(),111)  
						 AND gim_planes_usuario.plusu_est_anulada = 0
						 AND gim_planes_usuario.plusu_avisado = 0)
	
		INSERT INTO	tblPlanesUsuarioEliminar
			SELECT		DISTINCT
						gim_planes_usuario_especiales.cdgimnasio,
						gim_planes_usuario_especiales.plusu_numero_fact,
						gim_planes_usuario_especiales.plusu_identifi_cliente,
						gim_planes_usuario_especiales.plusu_codigo_plan,
						gim_planes_usuario_especiales.plusu_tiq_disponible,
						gim_planes_usuario_especiales.plusu_fecha_vcto,
						gim_planes_usuario_especiales.plusu_est_anulada,
						gim_planes_usuario_especiales.plusu_avisado,
						gim_planes_usuario_especiales.plusu_sucursal,
						gim_planes_usuario_especiales.plusu_fkdia_codigo,
						'Cortes�a',
						gim_planes.pla_codigo,
						gim_planes.pla_descripc,
						gim_planes.pla_tipo
						,case when tblReplicaHuellas.bitDelete = 0 then 1 
						else 0 end 
			FROM		gim_planes_usuario_especiales WITH (INDEX(IX_planes_usuario_especiales_ListaBlanca))
			LEFT JOIN #tmpClientes ON #tmpClientes.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente collate Modern_Spanish_CI_AS
			AND #tmpClientes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	#tmpProspecto ON #tmpProspecto.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente AND #tmpProspecto.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	#tmpEmpleados ON #tmpEmpleados.emp_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente  collate Modern_Spanish_CI_AS
			AND #tmpEmpleados.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario_especiales.plusu_codigo_plan AND gim_planes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			left join   tblReplicaHuellas ON gim_planes_usuario_especiales.plusu_identifi_cliente collate Modern_Spanish_CI_AS = tblReplicaHuellas.idPersona collate Modern_Spanish_CI_AS and tblReplicaHuellas.bitDelete = 0
			WHERE		gim_planes_usuario_especiales.plusu_codigo_plan != 999
			AND			gim_planes_usuario_especiales.plusu_sucursal = @intIdSucursal
			AND			gim_planes_usuario_especiales.cdgimnasio = @intIdGimnasio
			AND			CONVERT(VARCHAR(10), gim_planes_usuario_especiales.plusu_fecha_vcto, 111) < CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			(gim_planes_usuario_especiales.plusu_numero_fact IS NULL OR gim_planes_usuario_especiales.plusu_numero_fact NOT IN (SELECT invoiceId FROM #tmpCortesiasCongeladas))
			and			(CONVERT(VARCHAR(10), gim_planes_usuario_especiales.plusu_fecha_vcto, 111) collate SQL_Latin1_General_CP1_CI_AS <= CONVERT(VARCHAR(10), GETDATE(),111)  
						 AND gim_planes_usuario_especiales.plusu_est_anulada = 0
						 AND gim_planes_usuario_especiales.plusu_avisado = 0)
			

			PRINT('INICIA EL PROCESO DE INSERCION PARA CLIENTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			IF @bitValidarPlanYReservaWeb = 1 OR (@bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 0)
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
							branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
							courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
							classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
							employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
							subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT ISNULL(dbo.fFloatAVarchar(clientes.cli_identifi), '0')  AS 'id' ,
							ISNULL(clientes.cli_nombres,'') + ' ' + ISNULL(clientes.cli_primer_apellido,'') + ' ' + ISNULL(clientes.cli_segundo_apellido,'') AS 'name',
							planesUsuarios.pla_codigo AS 'planId',
							planesUsuarios.pla_descripc AS 'planName',
							ISNULL(DATEADD(DAY, ISNULL(clientes.cli_dias_gracia,0), planesUsuarios.plusu_fecha_vcto), NULL) AS 'expirationDate',
							NULL AS 'lastEntry',
							planesUsuarios.pla_tipo AS 'planType',
							CAST('Cliente' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	datediff(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM	[dbo].[fnObtenerRestriccionesWL] (clientes.cdgimnasio,clientes.cli_intcodigo_subgrupo,planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							clientes.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN CAST(0 AS BIT)
								ELSE	CAST(1 AS BIT)
							END AS 'withoutFingerprint',	
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_id)
								ELSE	CAST(0 AS BIT)
							END AS 'fingerprintId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_dato)
								ELSE	NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_bitControlIngreso,0)
							END AS 'groupEntriesControl',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_intNumlIngresos,0)
							END AS 'groupEntriesQuantity',
							ISNULL(gim_grupoFamiliar.gim_gf_IDgrupo,0) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							ISNULL(clientes.cli_intcodigo_subgrupo,0) as 'subgroupId',
							ISNULL(clientes.cli_strcodtarjeta,'') as 'cardId',
							CASE	
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,clientes.bitError
							,clientes.mensaje
				FROM		#tmpClientes clientes
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				AND			gim_grupoFamiliar.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = clientes.cdgimnasio
				WHERE		CONVERT(VARCHAR(10), DATEADD(DAY, ISNULL(clientes.cli_dias_gracia, 0), planesUsuarios.plusu_fecha_vcto), 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			((@bitValideContPorFactura = 1 AND EXISTS(SELECT 1 FROM gim_detalle_contrato WHERE gim_detalle_contrato.cdgimnasio = planesUsuarios.cdgimnasio AND gim_detalle_contrato.dtcont_doc_cliente = clientes.cli_identifi AND gim_detalle_contrato.dtcont_numero_plan = planesUsuarios.plusu_numero_fact AND gim_detalle_contrato.dtcont_FKcontrato = 4)) OR @bitValideContPorFactura = 0)
				AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

		END

			IF @bitValidaReservaWeb = 1 OR @bitValidarPlanYReservaWeb = 1
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
								branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
								courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
								classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
								employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
								subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT ISNULL(dbo.fFloatAVarchar(clientes.cli_identifi), '0') AS 'id',
							ISNULL(clientes.cli_nombres,'') + ' ' + ISNULL(clientes.cli_primer_apellido,'') + ' ' + ISNULL(clientes.cli_segundo_apellido,'') AS 'name',
							planesUsuarios.pla_codigo AS 'planId',
							planesUsuarios.pla_descripc AS 'planName',
							ISNULL(DATEADD(DAY, ISNULL(clientes.cli_dias_gracia,0), planesUsuarios.plusu_fecha_vcto), NULL) AS 'expirationDate',
							NULL AS 'lastEntry',
							planesUsuarios.pla_tipo AS 'planType',
							CAST('Cliente' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	datediff(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	
									ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM
									[dbo].[fnObtenerRestriccionesWL] (clientes.cdgimnasio,clientes.cli_intcodigo_subgrupo,planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							clientes.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN CAST(0 AS BIT)
								ELSE	CAST(1 AS BIT)
							END AS 'withoutFingerprint',	
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_id)
								ELSE	CAST(0 AS BIT)
							END AS 'fingerprintId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_dato)
								ELSE	NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_bitControlIngreso,0)
							END AS 'groupEntriesControl',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_intNumlIngresos,0)
							END AS 'groupEntriesQuantity',
							ISNULL(gim_grupoFamiliar.gim_gf_IDgrupo,0) AS 'groupId',
							CAST(1 AS BIT) AS 'isRestrictionClass',
							CONVERT(VARCHAR(10), DATEADD(MINUTE, -@intMinutosAntesReserva, gim_reservas.fecha_clase), 114) + '-' + CONVERT(VARCHAR(10), DATEADD(MINUTE, @intMinutosDespuesReserva, gim_reservas.fecha_clase),114) AS 'classSchedule',
							gim_reservas.fecha_clase AS 'dateClass',
							gim_reservas.cdreserva AS 'reserveId',
							gim_clases.nombre AS 'className',
							ISNULL(gim_reservas.megas_utilizadas,0) AS 'utilizedMegas',
							ISNULL(gim_reservas.tiq_utilizados,0) AS 'utilizedTickets', 
							ISNULL(gim_empleados.emp_nombre,'') + ' ' + ISNULL(gim_empleados.emp_primer_apellido,'') + ' ' + ISNULL(gim_empleados.emp_segundo_apellido,'') AS 'employeeName', 
							gim_reservas.intensidad AS 'classIntensity',
							gim_reservas.estado AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							ISNULL(clientes.cli_intcodigo_subgrupo,0) AS 'subgroupId',
							ISNULL(clientes.cli_strcodtarjeta,'') AS 'cardId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,clientes.bitError
							,clientes.mensaje
				FROM		#tmpClientes clientes
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				AND			gim_grupoFamiliar.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = clientes.cdgimnasio
				INNER JOIN	gim_reservas ON clientes.cli_identifi = gim_reservas.IdentificacionCliente COLLATE Modern_Spanish_CI_AS
				AND			clientes.cdgimnasio = gim_reservas.cdgimnasio
				AND			gim_reservas.cdsucursal = planesUsuarios.plusu_sucursal
				INNER JOIN	gim_clases ON gim_reservas.cdclase = gim_clases.cdclase
				AND			gim_reservas.cdgimnasio = gim_clases.cdgimnasio
				INNER JOIN	gim_horarios_clase ON gim_horarios_clase.cdhorario_clase = gim_reservas.cdhorario_clase
				AND			gim_horarios_clase.cdgimnasio = gim_reservas.cdgimnasio
				AND			gim_horarios_clase.cdclase = gim_clases.cdclase
				INNER JOIN	gim_empleados on gim_empleados.cdempleado = gim_horarios_clase.profesor
				AND			gim_empleados.cdgimnasio = gim_horarios_clase.cdgimnasio
				WHERE		gim_reservas.estado != 'Anulada'
				AND			gim_reservas.estado != 'Asistio'
				AND			CONVERT(VARCHAR(10), DATEADD(DAY, ISNULL(clientes.cli_dias_gracia, 0), planesUsuarios.plusu_fecha_vcto), 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			CONVERT(VARCHAR(10), gim_reservas.fecha_clase, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			((@bitValideContPorFactura = 1 AND EXISTS(SELECT 1 FROM gim_detalle_contrato WHERE gim_detalle_contrato.cdgimnasio = planesUsuarios.cdgimnasio AND gim_detalle_contrato.dtcont_doc_cliente = clientes.cli_identifi AND gim_detalle_contrato.dtcont_numero_plan = planesUsuarios.plusu_numero_fact AND gim_detalle_contrato.dtcont_FKcontrato = 4)) OR @bitValideContPorFactura = 0)
				AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			END

			PRINT('FINALIZA EL PROCESO DE INSERCION PARA CLIENTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))


			PRINT('SE INICIA EL PROCESO DE INSERCION PARA PROSPECTOS - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
						branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
						courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
						classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
						employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
						subgroupId,cardId,strDatoFoto,bitError,strMensaje)
			SELECT		DISTINCT dbo.fFloatAVarchar(prospecto.cli_identifi) AS 'id',
						ISNULL(prospecto.cli_nombres,'') + ' ' + ISNULL(prospecto.cli_primer_apellido,'') + ' ' + ISNULL(prospecto.cli_segundo_apellido,'') AS 'name',
						CAST(0 AS INT) AS 'planId',
						CAST('' AS VARCHAR(MAX)) AS 'planName',
						NULL AS 'expirationDate',
						NULL AS 'lastEntry',
						CAST('' AS VARCHAR(MAX)) AS 'planType',
						CAST('Prospecto' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN	((prospecto.cli_cortesia = 1 AND prospecto.cli_entro_cortesia = 0) AND prospecto.cli_entrar_conocer = 1) THEN CAST(2 AS INT)
							WHEN	(prospecto.cli_cortesia = 1 AND prospecto.cli_entro_cortesia = 0) THEN CAST(1 AS INT)
							ELSE	CAST(1 AS INT)
						END AS 'availableEntries',
						CAST('|||||||' AS VARCHAR(MAX)) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						prospecto.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(prospecto.cli_entrar_conocer AS BIT) AS 'know',
						CAST(prospecto.cli_cortesia AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						CAST(0 AS INT) AS 'invoiceId',
						CAST(0 AS INT) AS 'dianId',
						CAST('Cortes�a' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN prospecto.cli_EntryFingerprint = 1 THEN prospecto.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,prospecto.bitError
						,prospecto.mensaje
			FROM		#tmpProspecto prospecto
			INNER JOIN	gim_sucursales sucursal ON prospecto.cli_intfkSucursal = sucursal.suc_intpkIdentificacion 
			AND			sucursal.cdgimnasio = prospecto.cdgimnasio
			WHERE		((ISNULL(prospecto.cli_cortesia, 0) = 1 AND ISNULL(prospecto.cli_entro_cortesia, 0) = 0) OR ISNULL(prospecto.cli_entrar_conocer, 0) = 1) 
			AND			sucursal.suc_intpkIdentificacion = @intIdSucursal
			

			INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
						branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
						courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
						classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
						employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
						subgroupId,cardId,strDatoFoto,bitError,strMensaje)
			SELECT		DISTINCT dbo.fFloatAVarchar(prospecto.cli_identifi) AS 'id',
						ISNULL(prospecto.cli_nombres,'') + ' ' + ISNULL(prospecto.cli_primer_apellido,'') + ' ' + ISNULL(prospecto.cli_segundo_apellido,'') AS 'name',
						planesUsuarios.pla_codigo AS 'planId',
						planesUsuarios.pla_descripc AS 'planName',
						planesUsuarios.plusu_fecha_vcto AS 'expirationDate',
						NULL AS 'lastEntry',
						planesUsuarios.pla_tipo AS 'planType',
						CAST('Prospecto' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN (planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
							ELSE DATEDIFF(DAY, CONVERT(VARCHAR(10), GETDATE(), 111), CONVERT(VARCHAR(10), planesUsuarios.plusu_fecha_vcto, 111))
						END AS 'availableEntries',
						(
							SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
							FROM [dbo].[fnObtenerRestriccionesWL](prospecto.cdgimnasio, 0, planesUsuarios.plusu_codigo_plan)
						) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						prospecto.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(0 AS BIT) AS 'know',
						CAST(0 AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						planesUsuarios.plusu_numero_fact AS 'invoiceId',
						planesUsuarios.plusu_fkdia_codigo AS 'dianId',
						CAST('Cortes�a' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN prospecto.cli_EntryFingerprint = 1 THEN prospecto.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,prospecto.bitError
						,prospecto.mensaje
			FROM		#tmpProspecto prospecto
			INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = prospecto.cli_identifi
			INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
			AND			sucursal.cdgimnasio = prospecto.cdgimnasio
			WHERE		CONVERT(VARCHAR(10), planesUsuarios.plusu_fecha_vcto, 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			PRINT('FINALIZA EL PROCESO DE INSERCION PARA PROSPECTOS - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))


			PRINT('SE INICIA EL PROCESO DE INSERCION PARA VISITANTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
						branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
						courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
						classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
						employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
						subgroupId,cardId,strDatoFoto,bitError,strMensaje)
			SELECT		visitante.vis_strVisitorId AS 'id',
						ISNULL(visitante.vis_strName, '') + ' ' + ISNULL(visitante.vis_strFirstLastName, '') + ' ' + ISNULL(visitante.vis_strSecondLastName, '') AS 'name',
						CAST(0 AS INT) AS 'planId',
						CAST('' AS VARCHAR(MAX)) AS 'planName',
						NULL AS 'expirationDate',
						NULL AS 'lastEntry',
						CAST('' AS VARCHAR(10)) AS 'planType',
						CAST('Visitante' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN visitante.bitCortesia = 1 THEN CAST(2 AS INT)
							ELSE CAST(1 AS INT)
						END AS 'availableEntries',
						CAST(' ||||||| ' AS VARCHAR(MAX)) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						visitante.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN visitante.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN visitante.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(0 AS BIT) AS 'know',
						CAST(0 AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						CAST(visitas.Id AS INT) AS 'invoiceId',
						CAST(0 AS INT) AS 'dianId',
						CAST('' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN visitante.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,visitante.bitError
						,visitante.mensaje
			FROM		#tmpVisitante visitante
			INNER JOIN	Visit visitas ON visitas.VisitorId = visitante.vis_strVisitorId COLLATE Modern_Spanish_CI_AS
			INNER JOIN	gim_sucursales sucursal ON visitante.vis_intBranch = sucursal.suc_intpkIdentificacion
			AND			visitante.cdgimnasio = sucursal.cdgimnasio
			WHERE		(SELECT COUNT(*) FROM gim_entradas_usuarios WHERE gim_entradas_usuarios.enusu_VisitId = visitas.Id) < CAST(ISNULL(visitante.bitCortesia, 0) AS INT) + 1
			AND			CONVERT(VARCHAR(10), visitas.DateVisit, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			PRINT('FINALIZA EL PROCESO DE INSERCION PARA VISITANTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))


			PRINT('SE INICIA EL PROCESO DE INSERCION PARA EMPLEADOS - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			IF @bitValidarIngresoSinPlanEmpleado = 1
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
							branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
							courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
							classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
							employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
							subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT dbo.fFloatAVarchar(emp.emp_identifi) AS 'id',
							ISNULL(emp.emp_nombre, '') + ' ' + ISNULL(emp.emp_primer_apellido, '') + ' ' + ISNULL(emp.emp_segundo_apellido, '') AS 'name',
							CAST(0 AS INT) AS 'planId',
							CAST('' AS VARCHAR(100)) AS 'planName',
							NULL AS 'expirationDate',
							NULL AS 'lastEntry',
							CAST('' AS VARCHAR(50)) AS 'planType',
							CAST('Empleado' AS VARCHAR(100)) AS 'typePerson',
							CAST(0 AS INT) AS 'availableEntries',
							CAST('|||||||' AS VARCHAR(MAX)) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							emp.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(20)) AS 'personState',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN emp.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN emp.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS INT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath',
							CAST(0 AS INT) AS 'invoiceId',
							CAST(0 AS INT) AS 'dianId',
							CAST('' AS VARCHAR(50)) AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							ISNULL(CONVERT(VARCHAR(50), emp.emp_strcodtarjeta), '') AS 'cardId',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN emp.strDatoFoto
								ELSE NULL
							END AS 'strDatoFoto'
							,emp.bitError
							,emp.mensaje
				FROM		#tmpEmpleados emp
				INNER JOIN	gim_sucursales sucursal ON emp.emp_sucursal = sucursal.suc_intpkIdentificacion
				AND			emp.cdgimnasio = sucursal.cdgimnasio
				WHERE		sucursal.suc_intpkIdentificacion = @intIdSucursal

				
			END
			ELSE 
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
							branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
							courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
							classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
							employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
							subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT dbo.fFloatAVarchar(empleados.emp_identifi) AS 'id',
							ISNULL(empleados.emp_nombre, '') + ' ' + ISNULL(empleados.emp_primer_apellido, '') + ' ' + ISNULL(empleados.emp_segundo_apellido, '') AS 'name',
							planesUsuarios.pla_codigo AS 'planId',
							planesUsuarios.pla_descripc AS 'planName',
							planesUsuarios.plusu_fecha_vcto AS 'expirationDate',
							NULL AS 'lastEntry',
							planesUsuarios.pla_tipo AS 'planType',
							CAST('Empleado' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	DATEDIFF(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM	[dbo].[fnObtenerRestriccionesWL] (empleados.cdgimnasio, 0, planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							empleados.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN empleados.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN empleados.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS BIT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							ISNULL(CONVERT(VARCHAR(50), empleados.emp_strcodtarjeta), '') AS 'cardId',
							CASE	
								WHEN	(empleados.emp_sin_huella = 0) THEN (empleados.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,empleados.bitError
							,empleados.mensaje
				FROM		#tmpEmpleados empleados
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = empleados.emp_identifi COLLATE Modern_Spanish_CI_AS
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = empleados.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = empleados.emp_identifi COLLATE Modern_Spanish_CI_AS
				AND			gim_grupoFamiliar.cdgimnasio = empleados.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = empleados.cdgimnasio
				WHERE		CONVERT(VARCHAR(10), planesUsuarios.plusu_fecha_vcto, 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			END

			
			DROP TABLE #tmpPlanesUsuario
			DROP TABLE #tmpCortesiasCongeladas
			--DROP TABLE #tmpClientes
			--DROP TABLE #tmpProspecto
			--DROP TABLE #tmpVisitante
			--DROP TABLE #tmpEmpleados

			UPDATE	gim_configuracion_ingreso
			SET		bitResetLocalWhiteList = 1
			WHERE	cdgimnasio = @intIdGimnasio
			AND		intfkSucursal = @intIdSucursal

			SET @intRegistroSucursalActual += 1;

		END

		CLOSE cursorSucursales
		DEALLOCATE cursorSucursales

		DROP TABLE #tmpDatosContratos


		SET @intRegistroGimnasioActual += 1;

	END

	CLOSE cursorGimnasios
	DEALLOCATE cursorGimnasios

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spGenerateWhiteListNuevo' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spWhiteListClienteNuevo
END
GO
CREATE   PROCEDURE [dbo].[spWhiteListClienteNuevo]
    @intIdGimnasio INT,  
    @intIdSucursal INT,
	@strIdPersona VARCHAR(MAX)
AS
BEGIN

	DECLARE @intCantidadGimnasios INT = 0;
	DECLARE @intRegistroGimnasioActual INT = 0;
	DECLARE @intMayoriaEdad INT = 0;
	DECLARE @intCantidadSucursales INT = 0;
	DECLARE @intRegistroSucursalActual INT = 0;

	DECLARE @bitValidarContrato BIT = 0;
	DECLARE @bitValidarContratoEntrenamiento BIT = 0;
	DECLARE @bitValidarReglamentoDeUso BIT = 0;
	DECLARE @bitValidarInfoCita BIT = 0;
	DECLARE @bitValidarCitaNoCumplida BIT = 0;
	DECLARE @bitValidarClienteNoApto BIT = 0;
	DECLARE @bitValidarNoDisentimento BIT = 0;
	DECLARE @bitValidarAutorizacionMenor BIT = 0;
	DECLARE @intDiasGraciaCitaMedicaIngreso INT = 0;
	DECLARE @intEntradasCitaMedicaIngreso INT = 0;
	DECLARE @bitValidarConsentimientoDatosBiometricos BIT = 0;
	DECLARE @bitValidarConsentimientoInformado BIT = 0;
	DECLARE @bitValidarCarnetCodiv19 BIT = 0;
	DECLARE @bitValidarIngresoSinPlanEmpleado BIT = 0;
	DECLARE @bitValideContPorFactura BIT = 0;
	
	DECLARE @bitValidaReservaWeb BIT = 0;
	DECLARE @bitValidarPlanYReservaWeb BIT = 0;
	DECLARE @bitValidarDatosVitales BIT = 0;

	DECLARE @intMinutosAntesReserva INT = 0;
	DECLARE @intMinutosDespuesReserva INT = 0;

	SELECT	TOP(1) 
			@intMayoriaEdad = ISNULL(intAnosMayoriaEdad,0)
	FROM	tblConfiguracion
	WHERE	bitIngressWhiteList = 1
	AND		cdgimnasio = @intIdGimnasio

		----- contrato 
		SELECT	TOP(1)
				@bitValidarContrato = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 1 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarContratoEntrenamiento = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 2 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarConsentimientoInformado = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 3 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValideContPorFactura = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 4 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarConsentimientoDatosBiometricos = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 5 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarReglamentoDeUso = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 6 AND cdgimnasio = @intIdGimnasio;

		CREATE TABLE #tmpDatosContratos (
			dtcont_FKcontrato INT NOT NULL,
			dtcont_doc_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
		)

		INSERT INTO	#tmpDatosContratos
		SELECT		DISTINCT
					gim_detalle_contrato.dtcont_FKcontrato,
					gim_detalle_contrato.dtcont_doc_cliente
		FROM		gim_detalle_contrato
		WHERE		gim_detalle_contrato.cdgimnasio = @intIdGimnasio

	
			PRINT('COMIENZA LA CONSULTA DE PARAMETROS DE INGRESO - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			SELECT	TOP(1)
					@bitValidarInfoCita = ISNULL(bitConsultaInfoCita,0),
					@bitValidarCitaNoCumplida = ISNULL(bitBloqueoCitaNoCumplidaMSW, 0),
					@bitValidarClienteNoApto = ISNULL(bitBloqueoClienteNoApto,0),
					@bitValidarNoDisentimento = ISNULL(bitBloqueoNoDisentimento,0),
					@bitValidarAutorizacionMenor = ISNULL(bitBloqueoNoAutorizacionMenor,0),
					@intDiasGraciaCitaMedicaIngreso = ISNULL(intdiassincita_bloqueoing,0),
					@intEntradasCitaMedicaIngreso = ISNULL(intentradas_sincita_bloqueoing,0),
					@bitValidarCarnetCodiv19 = ISNULL(bitCargadecarnetdevacunaciondeCOVID19, 0),
					@bitValidarIngresoSinPlanEmpleado = ISNULL(bitIngresoEmpSinPlan, 0),
					@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
					@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0),
					@intMinutosAntesReserva = ISNULL(gim_configuracion_ingreso.intMinutosAntesReserva, 0),
					@intMinutosDespuesReserva = ISNULL(gim_configuracion_ingreso.intMinutosDespuesReserva, 0),
					@bitValidarDatosVitales = ISNULL(gim_configuracion_ingreso.bitDatosVirtualesUsuario, 0)
			FROM	gim_configuracion_ingreso
			WHERE	cdgimnasio = @intIdGimnasio
			AND		intfkSucursal = @intIdSucursal

			PRINT('FINALIZA LA CONSULTA DE PARAMETROS DE INGRESO - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			PRINT('COMIENZAN LAS VALIDACIONES PARA CLIENTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			SELECT		gim_clientes.cli_identifi,
						gim_clientes.cli_nombres,
						gim_clientes.cli_primer_apellido,
						gim_clientes.cli_segundo_apellido,
						gim_clientes.cli_dias_gracia,
						gim_clientes.cdgimnasio,
						gim_clientes.cli_sucursal,
						gim_clientes.cli_intcodigo_subgrupo,
						gim_clientes.cli_sin_huella,
						gim_clientes.cli_GrupoFamiliar,
						gim_clientes.cli_strcodtarjeta,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_id end as hue_id,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_dato end as hue_dato,

						gim_huellas.strDatoFoto
						,bitCarnetdevacunaciondeCOVID19
						,IMGCarnetdevacunaciondeCOVID19
						,cli_estado
						,gim_listanegra.listneg_floatId
						,isnull(gim_clientes.cli_Apto ,0) as cli_Apto
						,gim_clientes.cli_altoRiesgo
						,gim_clientes.cli_imgdisentimiento
						,isnull(gim_clientes.cli_bitAutorizacionM,0) as cli_bitAutorizacionM
						,gim_clientes.cli_fecha_nacimien
						,gim_clientes.cli_nombre_Allamar
						,gim_clientes.cli_telefono_emergencia
						,gim_clientes.cli_rh
						,gim_clientes.cli_cod_como_supo
						,gim_creditos.cre_fecha
						,gim_creditos.cre_anulado
						,gim_creditos.cre_pagado
						,gim_creditos.cre_identifi
						,cast(0 as bit) as bitError
						, CAST('' AS VARCHAR(MAX)) as mensaje
			into  #tmpClientes
			FROM		gim_clientes
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_clientes.cli_identifi
			AND			gim_huellas.cdgimnasio = gim_clientes.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON listneg_floatId = gim_clientes.cli_identifi
			AND			gim_listanegra.listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = gim_clientes.cdgimnasio
			LEFT JOIN	gim_creditos ON gim_creditos.cre_identifi = gim_clientes.cli_identifi
			AND			gim_creditos.cdgimnasio = gim_clientes.cdgimnasio
			WHERE		gim_clientes.cdgimnasio = @intIdGimnasio
			AND			gim_clientes.cli_sucursal = @intIdSucursal
			AND			gim_clientes.cli_identifi = @strIdPersona

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI EL CLIENTE ESTA INACTIVA'
			where bitError = 0 
			and cli_estado = 0

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE SEA APTO'
			where bitError = 0
			and (@bitValidarClienteNoApto = 1 AND ISNULL(cli_Apto, 0) = 1) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI EL CLIENTE TIENE CARNET DE VACUNACION COVID-19'
			where bitError = 0
			and(@bitValidarCarnetCodiv19 = 1 AND bitCarnetdevacunaciondeCOVID19 = 0 AND IMGCarnetdevacunaciondeCOVID19 IS NULL) 
		
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR INGRESO CLIENTE CON HUELLA'
			where  bitError = 0
			and (cli_sin_huella = 0 AND (ISNULL(hue_id, 0) != 0 OR ISNULL(cli_strcodtarjeta, '') != '')) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA'
			where  bitError = 0 and listneg_floatId IS not NULL
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO SEA DE ALTO RIESGO'
			where bitError = 0 and ISNULL(cli_altoRiesgo, 0) = 1
			 
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR BLOQUEO POR NO DISENTIMIENTO'
			where bitError = 0 and (@bitValidarNoDisentimento = 1 AND NOT cli_imgdisentimiento IS NULL) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR BLOQUEO POR NO AUTORIZACION MENOR DE EDAD'
			where bitError = 0 
			and (@bitValidarAutorizacionMenor = 1 AND cli_bitAutorizacionM = 1 AND cli_fecha_nacimien > DATEADD(YEAR, -@intMayoriaEdad, GETDATE())) 
		
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE TENGA CONTRATO FIRMADO PARA EL GIMNASIO'
			where bitError = 0 and (@bitValidarContrato = 1  AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE	#tmpDatosContratos.dtcont_FKcontrato = 1  AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS )) 
			 
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE TENGA CONTRATO FIRMADO PARA EL ENTRENAMIENTO'
			where bitError = 0 and (@bitValidarContratoEntrenamiento = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE	#tmpDatosContratos.dtcont_FKcontrato = 2 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE CONSENTIMIENTO INFORMADO'
			where bitError = 0 and (@bitValidarConsentimientoInformado = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 3 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 

			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE CONSENTIMIENTO DE DATOS BIOMETRICOS'
			where bitError = 0 and (@bitValidarConsentimientoDatosBiometricos = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 5 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE FIRMADO EL REGLAMENTO DE USO'
			where bitError = 0 and (@bitValidarReglamentoDeUso = 1 AND EXISTS(SELECT TOP(1) 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 6 AND #tmpDatosContratos.dtcont_doc_cliente = cli_identifi collate Modern_Spanish_CI_AS)) 
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR SI LA PERSONA TIENE DATOS VITALES REGISTRADOS'
			where  bitError = 0 AND
						(
						@bitValidarDatosVitales = 1	
						AND( ISNULL(cli_nombre_Allamar, '') = ''
						OR ISNULL(cli_telefono_emergencia, '') = ''
						OR ISNULL(cli_rh, '') = ''
						OR ISNULL(cli_cod_como_supo, 0) = 0)
					)
			
			update #tmpClientes
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO TENGA CREDITOS PENDIENTES'
			where bitError = 0 AND (cre_fecha > GETDATE() AND cre_anulado = 0 AND cre_pagado = 0)

			---------------------------------------------------------------------------------------------------------
			---------------------------------------------------------------------------------------------------------
			----------------------------------------------------------------------------------------------------------

			SELECT	gim_clientes_especiales.cli_identifi,
						gim_clientes_especiales.cdgimnasio,
						gim_clientes_especiales.cli_nombres,
						gim_clientes_especiales.cli_primer_apellido,
						gim_clientes_especiales.cli_segundo_apellido,
						gim_clientes_especiales.cli_cortesia,
						gim_clientes_especiales.cli_entro_cortesia,
						gim_clientes_especiales.cli_entrar_conocer,
						gim_clientes_especiales.cli_EntryFingerprint,
						gim_clientes_especiales.cli_intfkSucursal,
						gim_huellas.hue_id,
						gim_huellas.hue_dato,
						gim_huellas.strDatoFoto
						,gim_clientes_especiales.bitCarnetdevacunaciondeCOVID19
						,gim_clientes_especiales.IMGCarnetdevacunaciondeCOVID19
						,gim_listanegra.listneg_floatId
						,cast(0 as bit) as bitError
						, cast('' as varchar(max)) as mensaje
			INTO #tmpProspecto
			FROM		gim_clientes_especiales
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_clientes_especiales.cli_identifi
			AND			gim_huellas.cdgimnasio = gim_clientes_especiales.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON gim_listanegra.listneg_floatId = gim_clientes_especiales.cli_identifi 
			AND			gim_listanegra.listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = gim_clientes_especiales.cdgimnasio
			WHERE		gim_clientes_especiales.cdgimnasio = @intIdGimnasio
			AND			gim_clientes_especiales.cli_intfkSucursal = @intIdGimnasio
			AND			gim_clientes_especiales.cli_identifi = @strIdPersona


			UPDATE #tmpProspecto
			set bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA PROSPECTO' 
			WHERE bitError = 0  AND (cli_EntryFingerprint = 1 AND hue_id IS NOT NULL) 

			UPDATE #tmpProspecto
			set bitError = 1 , mensaje = 'VALIDAR SI EL PROSPECTO TIENE CARNET DE VACUNACION COVID-19)' 
			WHERE bitError = 0  AND (@bitValidarCarnetCodiv19 = 1 AND bitCarnetdevacunaciondeCOVID19 = 0 AND IMGCarnetdevacunaciondeCOVID19 IS NULL) 

			UPDATE #tmpProspecto
			set bitError = 1 , mensaje = 'VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA' 
			where  bitError = 0 and listneg_floatId IS NOT NULL

			-----------------------------------------------------------------------------------------------------------------------------
			----------------------------------------------------------------------------------------------------------------------------

			SELECT		Visitors.vis_strVisitorId,
						Visitors.cdgimnasio,
						Visitors.vis_strName,
						Visitors.vis_strFirstLastName,
						Visitors.vis_strSecondLastName,
						Visitors.bitCortesia,
						Visitors.vis_EntryFingerprint,
						Visitors.vis_intBranch,
						gim_huellas.hue_id,
						gim_huellas.hue_dato,
						gim_huellas.strDatoFoto
						,Visitors.bitCarnetCOVID19
						,isnull(Visitors.imgCarnetCOVID19,'') as imgCarnetCOVID19
						,isnull(gim_listanegra.listneg_floatId,0) as listneg_floatId
						,cast(0 as bit) as bitError
						, cast('' as varchar(max)) as mensaje
			INTO #tmpVisitante
			FROM		Visitors
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = Visitors.vis_strVisitorId
			AND			gim_huellas.cdgimnasio = Visitors.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON listneg_floatId = Visitors.vis_strVisitorId
			AND			listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = Visitors.cdgimnasio
			WHERE		Visitors.cdgimnasio = @intIdGimnasio
			AND			Visitors.vis_intBranch = @intIdSucursal
			and			Visitors.vis_strVisitorId = @strIdPersona
						UPDATE #tmpVisitante
						SET bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA VISITANTE'
						WHERE bitError = 0  AND (vis_EntryFingerprint = 1 AND hue_id IS NOT NULL)

						UPDATE #tmpVisitante
						SET bitError = 1 , mensaje = 'VALIDAR SI EL VISITANTE TIENE CARNET DE VACUNACION COVID-19'
						WHERE bitError = 0  AND ((@bitValidarCarnetCodiv19 = 1 AND ISNULL(bitCarnetCOVID19, 0) = 0 AND NOT imgCarnetCOVID19 IS NULL) OR @bitValidarCarnetCodiv19 = 0)
										
						UPDATE #tmpVisitante
						SET bitError = 1 , mensaje = 'VALIDAR QUE EL VISITANTE NO ESTE EN LA LISTA NEGRA'
						WHERE bitError = 0  and isnull(listneg_floatId,0) <> 0

						-----------------------------------------------------------------------------------------
						-----------------------------------------------------------------------------------------

						SELECT	gim_empleados.emp_identifi,
						gim_empleados.cdgimnasio,
						gim_empleados.emp_nombre,
						gim_empleados.emp_primer_apellido,
						gim_empleados.emp_segundo_apellido,
						gim_empleados.emp_sin_huella,
						gim_empleados.emp_strcodtarjeta,
						gim_empleados.emp_sucursal,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_id end as hue_id,
						case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_dato end as hue_dato,
						gim_huellas.strDatoFoto
						,bitCarnetdevacunaciondeCOVID19
						,IMGCarnetdevacunaciondeCOVID19
						,emp_estado
						,listneg_floatId
						,cast(0 as bit) as bitError
						, cast('' as varchar(max)) as mensaje
			INTO #tmpEmpleados
			FROM		gim_empleados
			LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_empleados.emp_identifi
			AND			gim_huellas.cdgimnasio = gim_empleados.cdgimnasio
			LEFT JOIN	gim_listanegra	WITH (INDEX(IX_gim_listanegra))	ON listneg_floatId = gim_empleados.emp_identifi
			AND			listneg_bitEstado = 1
			AND			gim_listanegra.cdgimnasio = gim_empleados.cdgimnasio
			WHERE		gim_empleados.cdgimnasio = @intIdGimnasio
			AND			gim_empleados.emp_sucursal = @intIdGimnasio
			and			gim_empleados.emp_identifi =  @strIdPersona
			
			
			UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA EMPLEADO'
			WHERE bitError = 0 AND (emp_sin_huella = 0 AND hue_id IS NOT NULL) 
			
			UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR SI EL EMPLEADO TIENE CARNET DE VACUNACION COVID-19'
			WHERE bitError = 0 AND (@bitValidarCarnetCodiv19 = 1 AND ISNULL(bitCarnetdevacunaciondeCOVID19, 0) = 0 )--AND NOT IMGCarnetdevacunaciondeCOVID19 IS NULL)

			UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR SI EL EMPLEADO ESTA ACTIVA'
			WHERE bitError = 0 AND ISNULL(emp_estado, 0)  = 0
			
						UPDATE #tmpEmpleados
			SET bitError = 1 , mensaje = 'VALIDAR QUE EL EMPLEADO NO ESTE EN LA LISTA NEGRA'
			WHERE bitError = 0 AND isnull(listneg_floatId,0) <> 0

			CREATE TABLE #tmpCortesiasCongeladas (
				invoiceId INT
			)

			INSERT INTO #tmpCortesiasCongeladas (invoiceId)
			SELECT gim_con_fac_esp.num_fac_con
			FROM gim_con_fac_esp
			WHERE gim_con_fac_esp.des_con = 0
				AND CONVERT(varchar(10), GETDATE(), 111) BETWEEN CONVERT(varchar(10), gim_con_fac_esp.fec_ini_con, 111) AND CONVERT(varchar(10), gim_con_fac_esp.fec_ter_con, 111)
				AND gim_con_fac_esp.cdgimnasio = @intIdGimnasio
				AND gim_con_fac_esp.con_intfkSucursal = @intIdSucursal

			CREATE TABLE #tmpPlanesUsuario (
				cdgimnasio INT NOT NULL,
				plusu_numero_fact INT NOT NULL,
				plusu_identifi_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
				plusu_codigo_plan INT,
				plusu_tiq_disponible INT,
				plusu_fecha_vcto DATETIME,
				plusu_est_anulada BIT,
				plusu_avisado BIT,
				plusu_sucursal INT NOT NULL,
				plusu_fkdia_codigo INT NOT NULL,
				strTipoRegistro VARCHAR(15) NOT NULL,
				pla_codigo INT NOT NULL,
				pla_descripc VARCHAR(50),
				pla_tipo VARCHAR(1),
				INDEX IX_PlusuIdentifiCliente (plusu_identifi_cliente, cdgimnasio)
			);

			INSERT INTO	#tmpPlanesUsuario
			SELECT		DISTINCT gim_planes_usuario.cdgimnasio,
						gim_planes_usuario.plusu_numero_fact,
						gim_planes_usuario.plusu_identifi_cliente,
						gim_planes_usuario.plusu_codigo_plan,
						gim_planes_usuario.plusu_tiq_disponible,
						gim_planes_usuario.plusu_fecha_vcto,
						gim_planes_usuario.plusu_est_anulada,
						gim_planes_usuario.plusu_avisado,
						gim_planes_usuario.plusu_sucursal,
						gim_planes_usuario.plusu_fkdia_codigo,
						'Factura',
						gim_planes.pla_codigo,
						gim_planes.pla_descripc,
						gim_planes.pla_tipo
			FROM		gim_planes_usuario WITH(INDEX(IX_gim_planes_usuario_cdgimnasio_plusu_identifi_cliente))
			LEFT JOIN	#tmpClientes ON #tmpClientes.cli_identifi = gim_planes_usuario.plusu_identifi_cliente collate Modern_Spanish_CI_AS
			AND			#tmpClientes.cdgimnasio = gim_planes_usuario.cdgimnasio
			INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario.plusu_codigo_plan
			AND			gim_planes.cdgimnasio = gim_planes_usuario.cdgimnasio
			LEFT JOIN	WhiteList ON WhiteList.invoiceId = gim_planes_usuario.plusu_numero_fact AND WhiteList.id = gim_planes_usuario.plusu_identifi_cliente
			WHERE		gim_planes_usuario.plusu_avisado = 0
			AND			gim_planes_usuario.plusu_est_anulada = 0
			AND			gim_planes_usuario.plusu_codigo_plan != 999
			AND			gim_planes_usuario.plusu_sucursal = @intIdSucursal
			AND			gim_planes_usuario.cdgimnasio = @intIdGimnasio
			AND			CONVERT(VARCHAR(10), gim_planes_usuario.plusu_fecha_inicio, 111) collate SQL_Latin1_General_CP1_CI_AS <= CONVERT(VARCHAR(10), GETDATE(),111) 
			AND			WhiteList.intPkId IS NULL	

			INSERT INTO	#tmpPlanesUsuario
			SELECT		DISTINCT
						gim_planes_usuario_especiales.cdgimnasio,
						gim_planes_usuario_especiales.plusu_numero_fact,
						gim_planes_usuario_especiales.plusu_identifi_cliente,
						gim_planes_usuario_especiales.plusu_codigo_plan,
						gim_planes_usuario_especiales.plusu_tiq_disponible,
						gim_planes_usuario_especiales.plusu_fecha_vcto,
						gim_planes_usuario_especiales.plusu_est_anulada,
						gim_planes_usuario_especiales.plusu_avisado,
						gim_planes_usuario_especiales.plusu_sucursal,
						gim_planes_usuario_especiales.plusu_fkdia_codigo,
						'Cortes�a',
						gim_planes.pla_codigo,
						gim_planes.pla_descripc,
						gim_planes.pla_tipo
			FROM		gim_planes_usuario_especiales WITH (INDEX(IX_planes_usuario_especiales_ListaBlanca))
			LEFT JOIN #tmpClientes ON #tmpClientes.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente collate Modern_Spanish_CI_AS
			AND #tmpClientes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	#tmpProspecto ON #tmpProspecto.cli_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente AND #tmpProspecto.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	#tmpEmpleados ON #tmpEmpleados.emp_identifi = gim_planes_usuario_especiales.plusu_identifi_cliente  collate Modern_Spanish_CI_AS
			AND #tmpEmpleados.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario_especiales.plusu_codigo_plan AND gim_planes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio
			LEFT JOIN	WhiteList ON WhiteList.invoiceId = gim_planes_usuario_especiales.plusu_numero_fact AND WhiteList.id = gim_planes_usuario_especiales.plusu_identifi_cliente
			WHERE		gim_planes_usuario_especiales.plusu_avisado = 0
			AND			gim_planes_usuario_especiales.plusu_est_anulada = 0
			AND			gim_planes_usuario_especiales.plusu_codigo_plan != 999
			AND			gim_planes_usuario_especiales.plusu_sucursal = @intIdSucursal
			AND			gim_planes_usuario_especiales.cdgimnasio = @intIdGimnasio
			AND			CONVERT(VARCHAR(10), gim_planes_usuario_especiales.plusu_fecha_inicio, 111) <= CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			(gim_planes_usuario_especiales.plusu_numero_fact IS NULL OR gim_planes_usuario_especiales.plusu_numero_fact NOT IN (SELECT invoiceId FROM #tmpCortesiasCongeladas))
			AND			WhiteList.intPkId IS NULL

			PRINT('INICIA EL PROCESO DE INSERCION PARA CLIENTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			IF @bitValidarPlanYReservaWeb = 1 OR (@bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 0)
			BEGIN

				--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
				--			branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
				--			courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
				--			classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
				--			employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
				--			subgroupId,cardId,strDatoFoto)
				SELECT		DISTINCT ISNULL(dbo.fFloatAVarchar(clientes.cli_identifi), '0')  AS 'id' ,
							ISNULL(clientes.cli_nombres,'') + ' ' + ISNULL(clientes.cli_primer_apellido,'') + ' ' + ISNULL(clientes.cli_segundo_apellido,'') AS 'name',
							planesUsuarios.pla_codigo AS 'planId',
							planesUsuarios.pla_descripc AS 'planName',
							ISNULL(DATEADD(DAY, ISNULL(clientes.cli_dias_gracia,0), planesUsuarios.plusu_fecha_vcto), NULL) AS 'expirationDate',
							NULL AS 'lastEntry',
							planesUsuarios.pla_tipo AS 'planType',
							CAST('Cliente' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	datediff(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM	[dbo].[fnObtenerRestriccionesWL] (clientes.cdgimnasio,clientes.cli_intcodigo_subgrupo,planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							clientes.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN CAST(0 AS BIT)
								ELSE	CAST(1 AS BIT)
							END AS 'withoutFingerprint',	
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_id)
								ELSE	CAST(0 AS BIT)
							END AS 'fingerprintId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_dato)
								ELSE	NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_bitControlIngreso,0)
							END AS 'groupEntriesControl',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_intNumlIngresos,0)
							END AS 'groupEntriesQuantity',
							ISNULL(gim_grupoFamiliar.gim_gf_IDgrupo,0) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							ISNULL(clientes.cli_intcodigo_subgrupo,0) as 'subgroupId',
							ISNULL(clientes.cli_strcodtarjeta,'') as 'cardId',
							CASE	
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,clientes.bitError
							,clientes.mensaje
				FROM		#tmpClientes clientes
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				AND			gim_grupoFamiliar.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = clientes.cdgimnasio
				WHERE		CONVERT(VARCHAR(10), DATEADD(DAY, ISNULL(clientes.cli_dias_gracia, 0), planesUsuarios.plusu_fecha_vcto), 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			((@bitValideContPorFactura = 1 AND EXISTS(SELECT 1 FROM gim_detalle_contrato WHERE gim_detalle_contrato.cdgimnasio = planesUsuarios.cdgimnasio AND gim_detalle_contrato.dtcont_doc_cliente = clientes.cli_identifi AND gim_detalle_contrato.dtcont_numero_plan = planesUsuarios.plusu_numero_fact AND gim_detalle_contrato.dtcont_FKcontrato = 4)) OR @bitValideContPorFactura = 0)
				AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

		END

			IF @bitValidaReservaWeb = 1 OR @bitValidarPlanYReservaWeb = 1
			BEGIN

				--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
				--				branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
				--				courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
				--				classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
				--				employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
				--				subgroupId,cardId,strDatoFoto)
				SELECT		DISTINCT ISNULL(dbo.fFloatAVarchar(clientes.cli_identifi), '0') AS 'id',
							ISNULL(clientes.cli_nombres,'') + ' ' + ISNULL(clientes.cli_primer_apellido,'') + ' ' + ISNULL(clientes.cli_segundo_apellido,'') AS 'name',
							planesUsuarios.pla_codigo AS 'planId',
							planesUsuarios.pla_descripc AS 'planName',
							ISNULL(DATEADD(DAY, ISNULL(clientes.cli_dias_gracia,0), planesUsuarios.plusu_fecha_vcto), NULL) AS 'expirationDate',
							NULL AS 'lastEntry',
							planesUsuarios.pla_tipo AS 'planType',
							CAST('Cliente' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	datediff(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	
									ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM
									[dbo].[fnObtenerRestriccionesWL] (clientes.cdgimnasio,clientes.cli_intcodigo_subgrupo,planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							clientes.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN CAST(0 AS BIT)
								ELSE	CAST(1 AS BIT)
							END AS 'withoutFingerprint',	
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_id)
								ELSE	CAST(0 AS BIT)
							END AS 'fingerprintId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.hue_dato)
								ELSE	NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_bitControlIngreso,0)
							END AS 'groupEntriesControl',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_intNumlIngresos,0)
							END AS 'groupEntriesQuantity',
							ISNULL(gim_grupoFamiliar.gim_gf_IDgrupo,0) AS 'groupId',
							CAST(1 AS BIT) AS 'isRestrictionClass',
							CONVERT(VARCHAR(10), DATEADD(MINUTE, -@intMinutosAntesReserva, gim_reservas.fecha_clase), 114) + '-' + CONVERT(VARCHAR(10), DATEADD(MINUTE, @intMinutosDespuesReserva, gim_reservas.fecha_clase),114) AS 'classSchedule',
							gim_reservas.fecha_clase AS 'dateClass',
							gim_reservas.cdreserva AS 'reserveId',
							gim_clases.nombre AS 'className',
							ISNULL(gim_reservas.megas_utilizadas,0) AS 'utilizedMegas',
							ISNULL(gim_reservas.tiq_utilizados,0) AS 'utilizedTickets', 
							ISNULL(gim_empleados.emp_nombre,'') + ' ' + ISNULL(gim_empleados.emp_primer_apellido,'') + ' ' + ISNULL(gim_empleados.emp_segundo_apellido,'') AS 'employeeName', 
							gim_reservas.intensidad AS 'classIntensity',
							gim_reservas.estado AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							ISNULL(clientes.cli_intcodigo_subgrupo,0) AS 'subgroupId',
							ISNULL(clientes.cli_strcodtarjeta,'') AS 'cardId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (clientes.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,clientes.bitError
							,clientes.mensaje
				FROM		#tmpClientes clientes
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = clientes.cli_identifi COLLATE Modern_Spanish_CI_AS
				AND			gim_grupoFamiliar.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = clientes.cdgimnasio
				INNER JOIN	gim_reservas ON clientes.cli_identifi = gim_reservas.IdentificacionCliente COLLATE Modern_Spanish_CI_AS
				AND			clientes.cdgimnasio = gim_reservas.cdgimnasio
				AND			gim_reservas.cdsucursal = planesUsuarios.plusu_sucursal
				INNER JOIN	gim_clases ON gim_reservas.cdclase = gim_clases.cdclase
				AND			gim_reservas.cdgimnasio = gim_clases.cdgimnasio
				INNER JOIN	gim_horarios_clase ON gim_horarios_clase.cdhorario_clase = gim_reservas.cdhorario_clase
				AND			gim_horarios_clase.cdgimnasio = gim_reservas.cdgimnasio
				AND			gim_horarios_clase.cdclase = gim_clases.cdclase
				INNER JOIN	gim_empleados on gim_empleados.cdempleado = gim_horarios_clase.profesor
				AND			gim_empleados.cdgimnasio = gim_horarios_clase.cdgimnasio
				WHERE		gim_reservas.estado != 'Anulada'
				AND			gim_reservas.estado != 'Asistio'
				AND			CONVERT(VARCHAR(10), DATEADD(DAY, ISNULL(clientes.cli_dias_gracia, 0), planesUsuarios.plusu_fecha_vcto), 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			CONVERT(VARCHAR(10), gim_reservas.fecha_clase, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			((@bitValideContPorFactura = 1 AND EXISTS(SELECT 1 FROM gim_detalle_contrato WHERE gim_detalle_contrato.cdgimnasio = planesUsuarios.cdgimnasio AND gim_detalle_contrato.dtcont_doc_cliente = clientes.cli_identifi AND gim_detalle_contrato.dtcont_numero_plan = planesUsuarios.plusu_numero_fact AND gim_detalle_contrato.dtcont_FKcontrato = 4)) OR @bitValideContPorFactura = 0)
				AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			END

			PRINT('FINALIZA EL PROCESO DE INSERCION PARA CLIENTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))


			PRINT('SE INICIA EL PROCESO DE INSERCION PARA PROSPECTOS - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
			--			branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
			--			courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
			--			classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
			--			employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
			--			subgroupId,cardId,strDatoFoto)
			SELECT		DISTINCT dbo.fFloatAVarchar(prospecto.cli_identifi) AS 'id',
						ISNULL(prospecto.cli_nombres,'') + ' ' + ISNULL(prospecto.cli_primer_apellido,'') + ' ' + ISNULL(prospecto.cli_segundo_apellido,'') AS 'name',
						CAST(0 AS INT) AS 'planId',
						CAST('' AS VARCHAR(MAX)) AS 'planName',
						NULL AS 'expirationDate',
						NULL AS 'lastEntry',
						CAST('' AS VARCHAR(MAX)) AS 'planType',
						CAST('Prospecto' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN	((prospecto.cli_cortesia = 1 AND prospecto.cli_entro_cortesia = 0) AND prospecto.cli_entrar_conocer = 1) THEN CAST(2 AS INT)
							WHEN	(prospecto.cli_cortesia = 1 AND prospecto.cli_entro_cortesia = 0) THEN CAST(1 AS INT)
							ELSE	CAST(1 AS INT)
						END AS 'availableEntries',
						CAST('|||||||' AS VARCHAR(MAX)) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						prospecto.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(prospecto.cli_entrar_conocer AS BIT) AS 'know',
						CAST(prospecto.cli_cortesia AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						CAST(0 AS INT) AS 'invoiceId',
						CAST(0 AS INT) AS 'dianId',
						CAST('Cortes�a' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN prospecto.cli_EntryFingerprint = 1 THEN prospecto.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,prospecto.bitError
						,prospecto.mensaje
			FROM		#tmpProspecto prospecto
			INNER JOIN	gim_sucursales sucursal ON prospecto.cli_intfkSucursal = sucursal.suc_intpkIdentificacion 
			AND			sucursal.cdgimnasio = prospecto.cdgimnasio
			WHERE		((ISNULL(prospecto.cli_cortesia, 0) = 1 AND ISNULL(prospecto.cli_entro_cortesia, 0) = 0) OR ISNULL(prospecto.cli_entrar_conocer, 0) = 1) 
			AND			sucursal.suc_intpkIdentificacion = @intIdSucursal
			

			--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
			--			branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
			--			courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
			--			classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
			--			employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
			--			subgroupId,cardId,strDatoFoto)
			SELECT		DISTINCT dbo.fFloatAVarchar(prospecto.cli_identifi) AS 'id',
						ISNULL(prospecto.cli_nombres,'') + ' ' + ISNULL(prospecto.cli_primer_apellido,'') + ' ' + ISNULL(prospecto.cli_segundo_apellido,'') AS 'name',
						planesUsuarios.pla_codigo AS 'planId',
						planesUsuarios.pla_descripc AS 'planName',
						planesUsuarios.plusu_fecha_vcto AS 'expirationDate',
						NULL AS 'lastEntry',
						planesUsuarios.pla_tipo AS 'planType',
						CAST('Prospecto' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN (planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
							ELSE DATEDIFF(DAY, CONVERT(VARCHAR(10), GETDATE(), 111), CONVERT(VARCHAR(10), planesUsuarios.plusu_fecha_vcto, 111))
						END AS 'availableEntries',
						(
							SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
							FROM [dbo].[fnObtenerRestriccionesWL](prospecto.cdgimnasio, 0, planesUsuarios.plusu_codigo_plan)
						) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						prospecto.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN (prospecto.cli_EntryFingerprint = 1) THEN prospecto.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(0 AS BIT) AS 'know',
						CAST(0 AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						planesUsuarios.plusu_numero_fact AS 'invoiceId',
						planesUsuarios.plusu_fkdia_codigo AS 'dianId',
						CAST('Cortes�a' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN prospecto.cli_EntryFingerprint = 1 THEN prospecto.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,prospecto.bitError
						,prospecto.mensaje
			FROM		#tmpProspecto prospecto
			INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = prospecto.cli_identifi
			INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
			AND			sucursal.cdgimnasio = prospecto.cdgimnasio
			WHERE		CONVERT(VARCHAR(10), planesUsuarios.plusu_fecha_vcto, 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			PRINT('FINALIZA EL PROCESO DE INSERCION PARA PROSPECTOS - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))


			PRINT('SE INICIA EL PROCESO DE INSERCION PARA VISITANTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
			--			branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
			--			courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
			--			classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
			--			employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
			--			subgroupId,cardId,strDatoFoto)
			SELECT		visitante.vis_strVisitorId AS 'id',
						ISNULL(visitante.vis_strName, '') + ' ' + ISNULL(visitante.vis_strFirstLastName, '') + ' ' + ISNULL(visitante.vis_strSecondLastName, '') AS 'name',
						CAST(0 AS INT) AS 'planId',
						CAST('' AS VARCHAR(MAX)) AS 'planName',
						NULL AS 'expirationDate',
						NULL AS 'lastEntry',
						CAST('' AS VARCHAR(10)) AS 'planType',
						CAST('Visitante' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN visitante.bitCortesia = 1 THEN CAST(2 AS INT)
							ELSE CAST(1 AS INT)
						END AS 'availableEntries',
						CAST(' ||||||| ' AS VARCHAR(MAX)) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						visitante.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN visitante.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN visitante.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(0 AS BIT) AS 'know',
						CAST(0 AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						CAST(visitas.Id AS INT) AS 'invoiceId',
						CAST(0 AS INT) AS 'dianId',
						CAST('' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN visitante.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,visitante.bitError
						,visitante.mensaje
			FROM		#tmpVisitante visitante
			INNER JOIN	Visit visitas ON visitas.VisitorId = visitante.vis_strVisitorId COLLATE Modern_Spanish_CI_AS
			INNER JOIN	gim_sucursales sucursal ON visitante.vis_intBranch = sucursal.suc_intpkIdentificacion
			AND			visitante.cdgimnasio = sucursal.cdgimnasio
			WHERE		(SELECT COUNT(*) FROM gim_entradas_usuarios WHERE gim_entradas_usuarios.enusu_VisitId = visitas.Id) < CAST(ISNULL(visitante.bitCortesia, 0) AS INT) + 1
			AND			CONVERT(VARCHAR(10), visitas.DateVisit, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)
			AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			PRINT('FINALIZA EL PROCESO DE INSERCION PARA VISITANTES - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))


			PRINT('SE INICIA EL PROCESO DE INSERCION PARA EMPLEADOS - FECHA: ' + CONVERT(VARCHAR(MAX), GETDATE(), 120))

			IF @bitValidarIngresoSinPlanEmpleado = 1
			BEGIN

				--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
				--			branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
				--			courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
				--			classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
				--			employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
				--			subgroupId,cardId,strDatoFoto)
				SELECT		DISTINCT dbo.fFloatAVarchar(emp.emp_identifi) AS 'id',
							ISNULL(emp.emp_nombre, '') + ' ' + ISNULL(emp.emp_primer_apellido, '') + ' ' + ISNULL(emp.emp_segundo_apellido, '') AS 'name',
							CAST(0 AS INT) AS 'planId',
							CAST('' AS VARCHAR(100)) AS 'planName',
							NULL AS 'expirationDate',
							NULL AS 'lastEntry',
							CAST('' AS VARCHAR(50)) AS 'planType',
							CAST('Empleado' AS VARCHAR(100)) AS 'typePerson',
							CAST(0 AS INT) AS 'availableEntries',
							CAST('|||||||' AS VARCHAR(MAX)) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							emp.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(20)) AS 'personState',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN emp.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN emp.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS INT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath',
							CAST(0 AS INT) AS 'invoiceId',
							CAST(0 AS INT) AS 'dianId',
							CAST('' AS VARCHAR(50)) AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							ISNULL(CONVERT(VARCHAR(50), emp.emp_strcodtarjeta), '') AS 'cardId',
							CASE
								WHEN ISNULL(emp.emp_sin_huella, 0) = 0 THEN emp.strDatoFoto
								ELSE NULL
							END AS 'strDatoFoto'
							,emp.bitError
							,emp.mensaje
				FROM		#tmpEmpleados emp
				INNER JOIN	gim_sucursales sucursal ON emp.emp_sucursal = sucursal.suc_intpkIdentificacion
				AND			emp.cdgimnasio = sucursal.cdgimnasio
				WHERE		sucursal.suc_intpkIdentificacion = @intIdSucursal

				
			END
			ELSE 
			BEGIN

				--INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
				--			branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
				--			courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
				--			classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
				--			employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
				--			subgroupId,cardId,strDatoFoto)
				SELECT		DISTINCT dbo.fFloatAVarchar(empleados.emp_identifi) AS 'id',
							ISNULL(empleados.emp_nombre, '') + ' ' + ISNULL(empleados.emp_primer_apellido, '') + ' ' + ISNULL(empleados.emp_segundo_apellido, '') AS 'name',
							planesUsuarios.pla_codigo AS 'planId',
							planesUsuarios.pla_descripc AS 'planName',
							planesUsuarios.plusu_fecha_vcto AS 'expirationDate',
							NULL AS 'lastEntry',
							planesUsuarios.pla_tipo AS 'planType',
							CAST('Empleado' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planesUsuarios.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	DATEDIFF(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM	[dbo].[fnObtenerRestriccionesWL] (empleados.cdgimnasio, 0, planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							empleados.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN empleados.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN empleados.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS BIT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							ISNULL(CONVERT(VARCHAR(50), empleados.emp_strcodtarjeta), '') AS 'cardId',
							CASE	
								WHEN	(empleados.emp_sin_huella = 0) THEN (empleados.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,empleados.bitError
							,empleados.mensaje
				FROM		#tmpEmpleados empleados
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = empleados.emp_identifi COLLATE Modern_Spanish_CI_AS
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = empleados.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = empleados.emp_identifi COLLATE Modern_Spanish_CI_AS
				AND			gim_grupoFamiliar.cdgimnasio = empleados.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = empleados.cdgimnasio
				WHERE		CONVERT(VARCHAR(10), planesUsuarios.plusu_fecha_vcto, 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			sucursal.suc_intpkIdentificacion = @intIdSucursal

			END

			
			DROP TABLE #tmpPlanesUsuario
			DROP TABLE #tmpClientes
			DROP TABLE #tmpProspecto
			DROP TABLE #tmpVisitante
			DROP TABLE #tmpEmpleados

			UPDATE	gim_configuracion_ingreso
			SET		bitResetLocalWhiteList = 1
			WHERE	cdgimnasio = @intIdGimnasio
			AND		intfkSucursal = @intIdSucursal


		DROP TABLE #tmpDatosContratos
	END

GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spValidarPersonaListaBlancaNuevo' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spValidarPersonaListaBlancaNuevo
END
GO
CREATE PROCEDURE [dbo].[spValidarPersonaListaBlancaNuevo]
(
	@idGimnasio INT,
	@intIdSucursal INT,
	@strIdPersona VARCHAR(MAX),
	@strTipoPersona VARCHAR(MAX)
)
AS
BEGIN

	DECLARE @procesoCompletado BIT = 0;
	DECLARE @bitValidarContrato BIT = 0;
	DECLARE @bitValidarInfoCita BIT = 0;
	DECLARE @bitValidarContratoEntrenamiento BIT = 0;
	DECLARE @bitValidarReglamentoDeUso BIT = 0;
	DECLARE @bitValidarCitaNoCumplida BIT = 0;
	DECLARE @bitValidarClienteNoApto BIT = 0;
	DECLARE @bitValidarNoDisentimento BIT = 0;
	DECLARE @bitValidarAutorizacionMenor BIT = 0;
	DECLARE @intDiasGraciaCitaMedicaIngreso INT = 0;
	DECLARE @intEntradasCitaMedicaIngreso INT = 0;
	DECLARE @bitValidarConsentimientoDatosBiometricos BIT = 0;
	DECLARE @bitValidarConsentimientoInformado BIT = 0;
	DECLARE @bitValidarCarnetCodiv19 BIT = 0;
	DECLARE @bitValidarIngresoSinPlanEmpleado BIT = 0;
	DECLARE @bitValidarDatosVitales BIT = 0;

	SELECT	TOP(1)
			@bitValidarContrato = ISNULL(bitEstado, 0)
	FROM	tblConfiguracion_FirmaContratosAcceso
	WHERE	intFkIdTipoContrato = 1 AND cdgimnasio = @idGimnasio;

	SELECT	TOP(1)
			@bitValidarContratoEntrenamiento = ISNULL(bitEstado, 0)
	FROM	tblConfiguracion_FirmaContratosAcceso
	WHERE	intFkIdTipoContrato = 2 AND cdgimnasio = @idGimnasio;

	SELECT	TOP(1)
			@bitValidarConsentimientoInformado = ISNULL(bitEstado, 0)
	FROM	tblConfiguracion_FirmaContratosAcceso
	WHERE	intFkIdTipoContrato = 3 AND cdgimnasio = @idGimnasio;

	SELECT	TOP(1)
			@bitValidarConsentimientoDatosBiometricos = ISNULL(bitEstado, 0)
	FROM	tblConfiguracion_FirmaContratosAcceso
	WHERE	intFkIdTipoContrato = 5 AND cdgimnasio = @idGimnasio;

	SELECT	TOP(1)
			@bitValidarReglamentoDeUso = ISNULL(bitEstado, 0)
	FROM	tblConfiguracion_FirmaContratosAcceso
	WHERE	intFkIdTipoContrato = 6 AND cdgimnasio = @idGimnasio;

	SELECT	TOP(1)
			@bitValidarInfoCita = ISNULL(bitConsultaInfoCita,0),
			@bitValidarCitaNoCumplida = ISNULL(bitBloqueoCitaNoCumplidaMSW, 0),
			@bitValidarClienteNoApto = ISNULL(bitBloqueoClienteNoApto,0),
			@bitValidarNoDisentimento = ISNULL(bitBloqueoNoDisentimento,0),
			@bitValidarAutorizacionMenor = ISNULL(bitBloqueoNoAutorizacionMenor,0),
			@intDiasGraciaCitaMedicaIngreso = ISNULL(intdiassincita_bloqueoing,0),
			@intEntradasCitaMedicaIngreso = ISNULL(intentradas_sincita_bloqueoing,0),
			@bitValidarCarnetCodiv19 = ISNULL(bitCargadecarnetdevacunaciondeCOVID19, 0),
			@bitValidarIngresoSinPlanEmpleado = ISNULL(bitIngresoEmpSinPlan, 0),
			@bitValidarDatosVitales = ISNULL(bitDatosVirtualesUsuario, 0)
	FROM	gim_configuracion_ingreso
	WHERE	cdgimnasio = @idGimnasio
	AND		intfkSucursal = @intIdSucursal

	IF @strTipoPersona = 'CLIENTE'
	BEGIN

		CREATE TABLE #tmpDatosCliente (
			strIdPersona VARCHAR(MAX) NOT NULL,
			bitPersonaNoApta BIT NOT NULL,
			bitPersonaAltoRiesgo BIT NOT NULL,
			imgDisentimiento IMAGE NULL,
			bitAutorizacionMenor BIT,
			dtmFechaNacimiento DATETIME null,
			bitIngresoSinHuella BIT,
			bitCarnetCovid BIT,
			imgCarnetCovid IMAGE,
			strTarjetaCliente VARCHAR(MAX) NOT NULL,
			cdgimnasio INT NOT NULL,
			cli_estado bit 
			,cli_nombre_Allamar varchar(max) NULL
			,cli_telefono_emergencia varchar(max) NULL
			,cli_rh varchar(50) NULL
			,cli_cod_como_supo int NULL
			,hue_id BIT NULL
			,bitError bit null default(0)
			,mensaje varchar(max) null
		);

		DECLARE @intMayoriaEdad INT = 0;

		SELECT	TOP(1)
				@intMayoriaEdad = ISNULL(intAnosMayoriaEdad,0)
		FROM	tblConfiguracion
		WHERE	cdgimnasio = @idGimnasio


		INSERT INTO #tmpDatosCliente
		SELECT		cli_identifi,
					ISNULL(cli_Apto, 0),
					ISNULL(cli_altoRiesgo, 0),
					cli_imgdisentimiento,
					ISNULL(cli_bitAutorizacionM, 0),
					cli_fecha_nacimien,
					ISNULL(cli_sin_huella, 0),
					ISNULL(bitCarnetdevacunaciondeCOVID19, 0),
					IMGCarnetdevacunaciondeCOVID19,
					ISNULL(gim_clientes.cli_strcodtarjeta, ''),
					gim_clientes.cdgimnasio,
					cli_estado
					,ISNULL(cli_nombre_Allamar,'') AS cli_nombre_Allamar
					,ISNULL(cli_telefono_emergencia,'') AS cli_telefono_emergencia
					,ISNULL(cli_rh ,'') AS cli_rh
					,ISNULL(cli_cod_como_supo,0) AS cli_cod_como_supo
					,case
							when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi having count(hue_id) > 2 ) > 2then null else gim_huellas.hue_id end as hue_id
					,0 as bitError
					,'' as mensaje
		FROM		gim_clientes
		LEFT JOIN	gim_huellas		WITH (INDEX(IX_HUE_WhiteList))	ON gim_huellas.hue_identifi = gim_clientes.cli_identifi
			AND			gim_huellas.cdgimnasio = gim_clientes.cdgimnasio
		WHERE		cli_identifi = @strIdPersona

		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR, CLIENTE NO ESTA ACTIVO'
		where bitError = 0 and cli_estado = 0

		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR QUE EL CLIENTE SEA APTO'
		where bitError = 0 AND ((@bitValidarClienteNoApto = 1 AND ISNULL(bitPersonaNoApta, 0) = 1) OR @bitValidarClienteNoApto = 0)

		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR QUE EL CLIENTE NO SEA DE ALTO RIESGO'
		where bitError = 0 AND ISNULL(bitPersonaAltoRiesgo, 0) = CAST(1 AS BIT)

		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR BLOQUEO POR NO DISENTIMIENTO'
		where bitError = 0 AND (@bitValidarNoDisentimento = 1 AND imgdisentimiento IS NULL) 

		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR SI LA PERSONA TIENE CARNET DE VACUNACION COVID-19'
		where bitError = 0 AND (@bitValidarCarnetCodiv19 = 1 AND bitCarnetCovid = 0 AND imgCarnetCovid IS NULL)
		
		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR BLOQUEO POR NO AUTORIZACION MENOR DE EDAD'
		where bitError = 0 AND (@bitValidarAutorizacionMenor = 1 AND ISNULL(bitAutorizacionMenor, 1) = 1 AND dtmFechaNacimiento > DATEADD(YEAR, -@intMayoriaEdad, GETDATE())) 

		 
		update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR SI LA PERSONA TIENE DATOS VITALES REGISTRADOS'
		where bitError = 0  
		AND	(
				@bitValidarDatosVitales = 1
				AND( cli_nombre_Allamar = ''
				OR cli_telefono_emergencia = ''
				OR cli_rh = ''
				OR cli_cod_como_supo = 0)
			)
						
		
			update #tmpDatosCliente
		set bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA'
		where bitError = 0 
		AND (bitIngresoSinHuella = 0 AND (ISNULL(hue_id, 0) = 0 OR ISNULL(strTarjetaCliente, '') = '')) 

			SELECT		dtcont_FKcontrato, gim_detalle_contrato.dtcont_doc_cliente
		INTO		#tmpContratos
		FROM		gim_detalle_contrato
		WHERE		gim_detalle_contrato.cdgimnasio = @idGimnasio
		AND			gim_detalle_contrato.dtcont_doc_cliente = @strIdPersona

			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDACION (VALIDAR QUE EL CLIENTE TENGA CONTRATO FIRMADO PARA EL GIMNASIO)'
			where bitError = 0 and (@bitValidarContrato = 1  AND EXISTS(SELECT 1 FROM #tmpContratos WHERE	#tmpContratos.dtcont_FKcontrato = 1  AND #tmpContratos.dtcont_doc_cliente = strIdPersona collate Modern_Spanish_CI_AS )) 
			 
			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDACION (VALIDAR QUE EL CLIENTE TENGA CONTRATO FIRMADO PARA EL ENTRENAMIENTO)'
			where bitError = 0 and (@bitValidarContratoEntrenamiento = 1 AND EXISTS(SELECT 1 FROM #tmpContratos WHERE	#tmpContratos.dtcont_FKcontrato = 2 AND #tmpContratos.dtcont_doc_cliente = strIdPersona collate Modern_Spanish_CI_AS)) 

			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDACION (VALIDAR SI LA PERSONA TIENE CONSENTIMIENTO INFORMADO)'
			where bitError = 0 and (@bitValidarConsentimientoInformado = 1 AND EXISTS(SELECT 1 FROM #tmpContratos WHERE #tmpContratos.dtcont_FKcontrato = 3 AND #tmpContratos.dtcont_doc_cliente = strIdPersona collate Modern_Spanish_CI_AS)) 

			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDACION (VALIDAR SI LA PERSONA TIENE CONSENTIMIENTO DE DATOS BIOMETRICOS)'
			where bitError = 0 and (@bitValidarConsentimientoDatosBiometricos = 1 AND EXISTS(SELECT 1 FROM #tmpContratos WHERE #tmpContratos.dtcont_FKcontrato = 5 AND #tmpContratos.dtcont_doc_cliente = strIdPersona collate Modern_Spanish_CI_AS)) 
			
			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDACION (VALIDAR SI LA PERSONA TIENE FIRMADO EL REGLAMENTO DE USO)'
			where bitError = 0 and (@bitValidarReglamentoDeUso = 1 AND EXISTS(SELECT TOP(1) 1 FROM #tmpContratos WHERE #tmpContratos.dtcont_FKcontrato = 6 AND #tmpContratos.dtcont_doc_cliente = strIdPersona collate Modern_Spanish_CI_AS)) 
			
  		--VALIDACION NO.11 (VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA)
		IF EXISTS(SELECT 1 FROM	gim_listanegra WHERE listneg_floatId = @strIdPersona AND listneg_bitEstado = 1 AND cdgimnasio = @idGimnasio)
		BEGIN

			PRINT('FALLO VALIDACION N0.11 - VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA')
			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA'
			where bitError = 0 
		END

		PRINT('PASO VALIDACION N0.11 - VALIDAR QUE EL CLIENTE NO ESTE EN LA LISTA NEGRA')

		--VALIDACION NO.12 (VALIDAR QUE EL CLIENTE NO TENGA CREDITOS PENDIENTES)
		IF EXISTS(SELECT 1	FROM gim_creditos WHERE gim_creditos.cre_fecha < GETDATE() AND gim_creditos.cre_anulado = 0 AND gim_creditos.cre_pagado = 0 AND cdgimnasio = @idGimnasio AND gim_creditos.cre_identifi = @strIdPersona)
		BEGIN
			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'VALIDAR QUE EL CLIENTE NO TENGA CREDITOS PENDIENTES'
			where bitError = 0 

		END
		PRINT('PASO VALIDACION N0.12 - VALIDAR QUE EL CLIENTE NO TENGA CREDITOS PENDIENTES')

		--VALIDACION N0.13 (VALIDAR BLOQUEO POR INCUMPLIMIENTO DE CITA MEDICA)
		IF @bitValidarCitaNoCumplida = 1 AND @bitValidarInfoCita = 1
		BEGIN

			DECLARE @bitClienteBloqueado BIT = 0;

			SELECT	@bitClienteBloqueado = CASE WHEN EXISTS (
												SELECT		1
												FROM		#tmpDatosCliente cli
												INNER JOIN	tblCitas c ON cli.cdgimnasio = c.intEmpresa 
												AND			cli.strIdPersona = c.strIdentificacionPaciente
												WHERE		cli.imgDisentimiento IS NULL
												AND			((
																	cli.bitPersonaAltoRiesgo = 0
																	AND c.bitAtendida = 0
																	AND c.bitCancelada = 0
																	AND c.bitActivo = 1
																	AND CONVERT(VARCHAR(10), c.datFechaCita, 111) < CONVERT(VARCHAR(10), GETDATE(), 111)
															)
															OR
															(
																cli.bitPersonaAltoRiesgo = 1
																AND 
																(
																	(
																		c.bitAtendida = 0
																		AND c.bitCancelada = 0
																		AND c.bitActivo = 1
																		AND (
																			SELECT COUNT(*) 
																			FROM gim_entradas_usuarios eu 
																			WHERE eu.enusu_identifi = cli.strIdPersona 
																			AND eu.cdgimnasio = cli.cdgimnasio 
																			AND CONVERT(VARCHAR(10), eu.enusu_fecha_entrada, 111) > CONVERT(VARCHAR(10), c.datFechaCita, 111)) > @intEntradasCitaMedicaIngreso
																	)
																	OR
																	(
																		DATEDIFF(day, c.datFechaCita, GETDATE()) > @intDiasGraciaCitaMedicaIngreso
																	)
																)
															))
											) THEN 1 ELSE 0 END

			IF @bitClienteBloqueado = 1
			BEGIN
				
			update #tmpDatosCliente
			set bitError = 1 ,  mensaje = 'BLOQUEO POR INCUMPLIMIENTO DE CITA MEDICA'
			where bitError = 0 

			END

		END
		PRINT('PASO VALIDACION N0.13 - BLOQUEO POR INCUMPLIMIENTO DE CITA MEDICA')

			
		select bitError,mensaje, 'CLIENTE' AS TipoPersona from #tmpDatosCliente

	END
	ELSE IF @strTipoPersona = 'PROSPECTO'
	BEGIN

		CREATE TABLE #tmpDatosProspecto (
			strIdPersona VARCHAR(MAX) NOT NULL,
			bitCarnetCovid BIT,
			imgCarnetCovid IMAGE,
			bitIngresoConHuella BIT,
			bitCortesia BIT,
			cdgimnasio INT NOT NULL
			,bitError bit null default(0)
			,mensaje varchar(max) null
		);

		INSERT INTO #tmpDatosProspecto
		SELECT		cli_identifi,
					ISNULL(bitCarnetdevacunaciondeCOVID19, 0),
					IMGCarnetdevacunaciondeCOVID19,
					ISNULL(cli_EntryFingerprint, 0),
					ISNULL(cli_cortesia, 0),
					cdgimnasio
					,0 as bitError
					,'' as mensaje
		FROM		gim_clientes_especiales
		WHERE		cli_identifi = @strIdPersona

		-- VALIDACION N0.1 (VALIDAR SI EL PROSPECTO TIENE CARNET DE VACUNACION COVID-19)
		update #tmpDatosProspecto
		set bitError = 1 , mensaje = 'VALIDAR SI LA PERSONA TIENE CARNET DE VACUNACION COVID-19'
		where bitError = 0 AND (@bitValidarCarnetCodiv19 = 1 AND bitCarnetCovid = 0 AND imgCarnetCovid IS NULL)

		--VALIDACION N0.2 (VALIDAR SI LA PERSONA TIENE HUELLA, EN CASO DE QUE TENGA QUE INGRESAR CON HUELLA)
		IF EXISTS(SELECT 1 FROM #tmpDatosProspecto WHERE bitIngresoConHuella = 1)
		BEGIN

			IF NOT EXISTS(SELECT 1 FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @idGimnasio)
			BEGIN
				
			update #tmpDatosProspecto
			set bitError = 1 , mensaje = 'VALIDAR SI LA PERSONA TIENE HUELLA, EN CASO DE QUE TENGA QUE INGRESAR CON HUELLA'
			where bitError = 0

			END

		END
		PRINT('PASO VALIDACION N0.2 - VALIDAR SI LA PERSONA TIENE HUELLA, EN CASO DE QUE TENGA QUE INGRESAR CON HUELLA')

		--VALIDACION NO.3 (VALIDAR LA CANTIDAD DE ENTRADAS VALIDAS)
		SELECT		*
		INTO		#tmpEntradasProspecto
		FROM		gim_entradas_usuarios
		WHERE		gim_entradas_usuarios.enusu_identifi = @strIdPersona
		AND			gim_entradas_usuarios.cdgimnasio = @idGimnasio
		AND			CONVERT(VARCHAR(10), gim_entradas_usuarios.enusu_fecha_entrada, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)

		IF EXISTS(SELECT 1 FROM	#tmpDatosProspecto WHERE bitCortesia = 1)
		BEGIN
		
			IF  (SELECT COUNT(*) FROM #tmpEntradasProspecto) >= 2
			BEGIN
					
			update #tmpDatosProspecto
			set bitError = 1 , mensaje = 'VALIDAR CANTIDAD ENTRADAS VALIDAS'
			where bitError = 0

			END

		END
		ELSE
		BEGIN

			IF  (SELECT COUNT(*) FROM #tmpEntradasProspecto) >= 1
			BEGIN
				
			update #tmpDatosProspecto
			set bitError = 1 , mensaje = 'VALIDAR CANTIDAD ENTRADAS VALIDAS'
			where bitError = 0
							
			END

		END
		PRINT('PASO VALIDACION N0.3 - VALIDAR CANTIDAD ENTRADAS VALIDAS')

		--VALIDACION NO.4 (VALIDAR QUE EL PROSPECTO NO ESTE EN LA LISTA NEGRA)
		IF EXISTS(SELECT 1 FROM	gim_listanegra WHERE listneg_floatId = @strIdPersona AND listneg_bitEstado = 1 AND cdgimnasio = @idGimnasio)
		BEGIN

			update #tmpDatosProspecto
			set bitError = 1 , mensaje = 'VALIDAR QUE EL PROSPECTO NO ESTE EN LA LISTA NEGRA'
			where bitError = 0

		END
		PRINT('PASO VALIDACION N0.4 - VALIDAR QUE EL PROSPECTO NO ESTE EN LA LISTA NEGRA')

		select bitError,mensaje, 'PROSPECTO' AS TipoPersona from #tmpDatosProspecto
	END
	ELSE IF @strTipoPersona = 'VISITANTE'
	BEGIN

		CREATE TABLE #tmpDatosVisitante (
			strIdPersona VARCHAR(MAX) NOT NULL,
			bitIngresoConHuella BIT,
			bitCarnetCOVID19 BIT,
			imgCarnetCOVID19 IMAGE,
			cdgimnasio INT NOT NULL
			,bitError bit null default(0)
			,mensaje varchar(max) null
		);

		INSERT INTO #tmpDatosVisitante
		SELECT		vis_strVisitorId,
					ISNULL(vis_EntryFingerprint, 0),
					ISNULL(bitCarnetCOVID19, 0),
					CASE
						WHEN bitCarnetCOVID19 = 1 THEN imgCarnetCOVID19
						ELSE NULL
					END AS imgCarnetCOVID19,
					cdgimnasio
					,0 as bitError
					,'' as mensaje
		FROM		Visitors
		WHERE		vis_strVisitorId = @strIdPersona

		-- VALIDACION N0.1 (VALIDAR SI EL VISITANTE TIENE CARNET DE VACUNACION COVID-19)
		update #tmpDatosVisitante
		set bitError = 1 , mensaje = 'VALIDAR SI LA PERSONA TIENE CARNET DE VACUNACION COVID-19'
		where bitError = 0 AND (@bitValidarCarnetCodiv19 = 1 AND bitCarnetCOVID19 = 0 AND imgCarnetCOVID19 IS NULL)


		--VALIDACION N0.2 (VALIDAR INGRESO CON HUELLA)
		IF EXISTS(SELECT 1 FROM #tmpDatosVisitante WHERE bitIngresoConHuella = 1)
		BEGIN

			IF NOT EXISTS(SELECT 1 FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @idGimnasio)
			BEGIN
			
			update #tmpDatosVisitante
			set bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA'
			where bitError = 0

			END

		END
		PRINT('PASO VALIDACION N0.2 - VALIDAR INGRESO CON HUELLA')

		--VALIDACION NO.3 (VALIDAR QUE EL VISITANTE NO ESTE EN LA LISTA NEGRA)
		IF EXISTS(SELECT 1 FROM	gim_listanegra WHERE listneg_floatId = @strIdPersona AND listneg_bitEstado = 1 AND cdgimnasio = @idGimnasio)
		BEGIN

			update #tmpDatosVisitante
			set bitError = 1 , mensaje = 'VALIDAR QUE EL VISITANTE NO ESTE EN LA LISTA NEGRA'
			where bitError = 0

		END
		PRINT('PASO VALIDACION N0.3 - VALIDAR QUE EL VISITANTE NO ESTE EN LA LISTA NEGRA')

		select bitError,mensaje, 'VISITANTE' AS TipoPersona from #tmpDatosVisitante
	END
	ELSE IF @strIdPersona = 'EMPLEADO'
	BEGIN
		CREATE TABLE #tmpDatosEmpleado (
			strIdPersona VARCHAR(MAX) NOT NULL,
			bitEstadoPersona BIT NOT NULL,
			bitIngresoConHuella BIT,
			cdgimnasio INT NOT NULL
			,emp_estado bit
			,bitCarnetCovid BIT
			,imgCarnetCovid IMAGE
			,bitError bit null default(0)
			,mensaje varchar(max) null
		);

		INSERT INTO #tmpDatosEmpleado
		SELECT		emp_identifi,
					ISNULL(emp_estado, 1),
					ISNULL(emp_sin_huella,0),
					cdgimnasio
					,emp_estado
					,bitCarnetdevacunaciondeCOVID19
					,IMGCarnetdevacunaciondeCOVID19
					,0 as bitError
					,'' as mensaje
		FROM		gim_empleados
		WHERE		gim_empleados.emp_identifi = @strIdPersona

		PRINT('FALLO VALIDACION N0.1 - VALIDAR SI EL EMPLEADO ESTA ACTIVO')
		update #tmpDatosEmpleado
		set bitError = 1 , mensaje = 'VALIDAR SI EL EMPLEADO ESTA ACTIVO'
		where bitError = 0 AND ISNULL(emp_estado, 0) = 1

		PRINT('FALLO VALIDACION N0.1 - VALIDAR SI EL EMPLEADO ESTA ACTIVO')
		update #tmpDatosEmpleado
		set bitError = 1 , mensaje = 'VALIDAR SI LA PERSONA TIENE CARNET DE VACUNACION COVID-19'
		where bitError = 0 AND (@bitValidarCarnetCodiv19 = 1 AND bitCarnetCovid = 0 AND imgCarnetCovid IS NULL)


		--VALIDACION N0.3 (VALIDAR INGRESO CON HUELLA)
		IF EXISTS(SELECT 1 FROM #tmpDatosEmpleado WHERE bitIngresoConHuella = 0)
		BEGIN

			IF NOT EXISTS(SELECT 1 FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @idGimnasio)
			BEGIN
				PRINT('FALLO VALIDACION N0.3 - VALIDAR INGRESO CON HUELLA')
				update #tmpDatosEmpleado
				set bitError = 1 , mensaje = 'VALIDAR INGRESO CON HUELLA'
				where bitError = 0 
				
			END

		END
		PRINT('PASO VALIDACION N0.3 - VALIDAR INGRESO CON HUELLA')

		--VALIDACION NO.4 (VALIDAR QUE EL EMPLEADO NO ESTE EN LA LISTA NEGRA)
		IF EXISTS(SELECT 1 FROM	gim_listanegra WHERE listneg_floatId = @strIdPersona AND listneg_bitEstado = 1 AND cdgimnasio = @idGimnasio)
		BEGIN

			PRINT('FALLO VALIDACION N0.4 - VALIDAR QUE EL EMPLEADO NO ESTE EN LA LISTA NEGRA')
			update #tmpDatosEmpleado
			set bitError = 1 , mensaje = 'VALIDAR QUE EL EMPLEADO NO ESTE EN LA LISTA NEGRA'
			where bitError = 0 
			

		END
		PRINT('PASO VALIDACION N0.4 - VALIDAR QUE EL EMPLEADO NO ESTE EN LA LISTA NEGRA')

		select bitError,mensaje, 'EMPLEADO' AS TipoPersona from #tmpDatosVisitante
	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spInsertarPersonaListaBlancaNuevo' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spInsertarPersonaListaBlancaNuevo
END
GO
CREATE PROCEDURE [dbo].[spInsertarPersonaListaBlancaNuevo]
(
	@idGimnasio INT, 
	@intIdSucursal INT,
	@strIdPersona VARCHAR(MAX),
	@strTrigger VARCHAR(MAX),
	@strTipoPersona VARCHAR(MAX),
	@bitValidarReserva BIT,
	@bitValidarPlan BIT,
	@intNumeroFactura INT,
	@bitError bit  null,
	@strMensaje VARCHAR(MAX) null
)
AS
BEGIN
    DECLARE @procesoCompletado BIT = 0;

	CREATE TABLE #tmpPlanesUsuario (
		cdgimnasio INT NOT NULL,
		plusu_numero_fact INT NOT NULL,
		plusu_identifi_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		plusu_codigo_plan INT,
		plusu_tiq_disponible INT,
		plusu_fecha_vcto DATETIME,
		plusu_est_anulada BIT,
		plusu_avisado BIT,
		plusu_sucursal INT NOT NULL,
		plusu_fkdia_codigo INT NOT NULL,
		strTipoRegistro VARCHAR(15) NOT NULL
	);

	CREATE TABLE #tmpHuellas (
		hue_id INT NOT NULL,
		hue_identifi VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
		hue_dato BINARY(3000),
		strDatoFoto VARCHAR(MAX)
	);

    IF @idGimnasio IS NOT NULL AND @intIdSucursal IS NOT NULL AND @strIdPersona IS NOT NULL
    BEGIN
		
		PRINT('TRIGGER DISPARADO DE LA TABLA = ' + @strTrigger)

		IF @strTrigger = 'gim_planes_usuario' OR @strTrigger = 'gim_huellas' OR @strTrigger = 'gim_clientes' OR @strTrigger = 'gim_con_fac' OR @strTrigger = 'gim_reservas' OR @strTrigger = 'tblCitas'
		BEGIN

			IF @strTipoPersona = 'CLIENTE'
			BEGIN
					PRINT('SE CONSULTAN CONGELACION FACTURA')

				CREATE TABLE #tmpFacturaCongeladas (
					invoiceId INT
				)

				INSERT INTO #tmpFacturaCongeladas (invoiceId)
				SELECT	gim_con_fac.num_fac_con
				FROM	gim_con_fac
				WHERE	gim_con_fac.des_con = 0
				AND		CONVERT(varchar(10), GETDATE(), 111) BETWEEN CONVERT(varchar(10), gim_con_fac.fec_ini_con, 111) AND CONVERT(varchar(10), gim_con_fac.fec_ter_con, 111)
				AND		gim_con_fac.cdgimnasio = @idGimnasio
				AND		gim_con_fac.con_sucursal = @intIdSucursal


				PRINT('SE CONSULTAN LAS FACTURAS')

				INSERT INTO	#tmpPlanesUsuario
				SELECT		gim_planes_usuario.cdgimnasio,
							gim_planes_usuario.plusu_numero_fact,
							gim_planes_usuario.plusu_identifi_cliente,
							gim_planes_usuario.plusu_codigo_plan,
							gim_planes_usuario.plusu_tiq_disponible,
							gim_planes_usuario.plusu_fecha_vcto,
							gim_planes_usuario.plusu_est_anulada,
							gim_planes_usuario.plusu_avisado,
							gim_planes_usuario.plusu_sucursal,
							gim_planes_usuario.plusu_fkdia_codigo,
							'Factura'
				FROM		gim_planes_usuario
				LEFT JOIN	WhiteList ON WhiteList.invoiceId = gim_planes_usuario.plusu_numero_fact
				AND			WhiteList.id = gim_planes_usuario.plusu_identifi_cliente
				AND			((WhiteList.typePerson = 'Cliente' AND NOT EXISTS (SELECT 1 FROM gim_reservas WHERE IdentificacionCliente = gim_planes_usuario.plusu_identifi_cliente AND WhiteList.reserveId <> gim_reservas.cdreserva)) OR WhiteList.typePerson <> 'Cliente')
				WHERE		gim_planes_usuario.plusu_identifi_cliente = @strIdPersona
				AND			WhiteList.intPkId IS NULL
				AND			gim_planes_usuario.plusu_avisado = 0
				AND			gim_planes_usuario.plusu_est_anulada = 0
				AND			gim_planes_usuario.plusu_codigo_plan != 999
				AND			gim_planes_usuario.cdgimnasio = @idGimnasio
				AND			gim_planes_usuario.plusu_sucursal = @intIdSucursal
				AND			(gim_planes_usuario.plusu_numero_fact  = @intNumeroFactura OR @intNumeroFactura = 0)
				AND			CONVERT(VARCHAR(10), gim_planes_usuario.plusu_fecha_inicio, 111) <= CONVERT(VARCHAR(10), GETDATE(),111)
				AND			gim_planes_usuario.plusu_numero_fact NOT IN (SELECT invoiceId FROM #tmpFacturaCongeladas)

			END

		END
		
		IF @strTrigger = 'gim_planes_usuario_especiales' OR  @strTrigger = 'gim_huellas' OR @strTrigger = 'gim_clientes' OR @strTrigger = 'gim_con_fac_esp' OR @strTrigger = 'gim_reservas'
		BEGIN

			IF @strTipoPersona = 'PROSPECTO' OR @strTipoPersona = 'CLIENTE' OR @strTipoPersona = 'EMPLEADO'
			BEGIN

				PRINT('SE CONSULTAN LAS CORTESIAS')

				CREATE TABLE #tmpCortesiasCongeladas (
					invoiceId INT
				)

				INSERT INTO #tmpCortesiasCongeladas (invoiceId)
				SELECT	gim_con_fac_esp.num_fac_con
				FROM	gim_con_fac_esp
				WHERE	gim_con_fac_esp.des_con = 0
				AND		CONVERT(varchar(10), GETDATE(), 111) BETWEEN CONVERT(varchar(10), gim_con_fac_esp.fec_ini_con, 111) AND CONVERT(varchar(10), gim_con_fac_esp.fec_ter_con, 111)
				AND		gim_con_fac_esp.cdgimnasio = @idGimnasio
				AND		gim_con_fac_esp.con_intfkSucursal = @intIdSucursal

				INSERT INTO	#tmpPlanesUsuario
				SELECT		gim_planes_usuario_especiales.cdgimnasio,
							gim_planes_usuario_especiales.plusu_numero_fact,
							gim_planes_usuario_especiales.plusu_identifi_cliente,
							gim_planes_usuario_especiales.plusu_codigo_plan,
							gim_planes_usuario_especiales.plusu_tiq_disponible,
							gim_planes_usuario_especiales.plusu_fecha_vcto,
							gim_planes_usuario_especiales.plusu_est_anulada,
							gim_planes_usuario_especiales.plusu_avisado,
							gim_planes_usuario_especiales.plusu_sucursal,
							gim_planes_usuario_especiales.plusu_fkdia_codigo,
							'Cortes�a'
				FROM		gim_planes_usuario_especiales
				LEFT JOIN	WhiteList ON WhiteList.invoiceId = gim_planes_usuario_especiales.plusu_numero_fact
				AND			WhiteList.id = gim_planes_usuario_especiales.plusu_identifi_cliente
				AND			((WhiteList.typePerson = 'Cliente' AND NOT EXISTS (SELECT 1 FROM gim_reservas WHERE IdentificacionCliente = gim_planes_usuario_especiales.plusu_identifi_cliente AND WhiteList.reserveId <> gim_reservas.cdreserva)) OR WhiteList.typePerson <> 'Cliente')
				WHERE		gim_planes_usuario_especiales.plusu_identifi_cliente = @strIdPersona
				AND			WhiteList.intPkId IS NULL
				AND			gim_planes_usuario_especiales.plusu_avisado = 0
				AND			gim_planes_usuario_especiales.plusu_est_anulada = 0
				AND			gim_planes_usuario_especiales.plusu_codigo_plan != 999
				AND			gim_planes_usuario_especiales.cdgimnasio = @idGimnasio
				AND			gim_planes_usuario_especiales.plusu_sucursal = @intIdSucursal
				AND			(gim_planes_usuario_especiales.plusu_numero_fact  = @intNumeroFactura OR @intNumeroFactura = 0)
				AND			CONVERT(VARCHAR(10), gim_planes_usuario_especiales.plusu_fecha_inicio, 111) <= CONVERT(VARCHAR(10), GETDATE(),111)
				AND			gim_planes_usuario_especiales.plusu_numero_fact NOT IN (SELECT invoiceId FROM #tmpCortesiasCongeladas)

			END

		END

		IF @strTipoPersona = 'CLIENTE'
		BEGIN

			PRINT('ES CLIENTE');
			SELECT	*
			INTO	#tmpClientes
			FROM	gim_clientes
			WHERE	
			--gim_clientes.cli_estado = 1 AND
					gim_clientes.cdgimnasio = @idGimnasio
			AND		gim_clientes.cli_identifi = @strIdPersona

			IF EXISTS(SELECT 1 FROM #tmpClientes WHERE cli_sin_huella = 0)
			BEGIN

				INSERT INTO	#tmpHuellas
				SELECT		TOP(1) 
							--hue_id,
							case when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi AND	h2.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then 0 else hue_id end as hue_id ,
							hue_identifi,
							--hue_dato,
							case when (select count(*) from gim_huellas h3 where h3.hue_identifi = gim_huellas.hue_identifi AND	h3.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then null else hue_dato end as hue_dato,
							strDatoFoto
				FROM		gim_huellas
				WHERE		gim_huellas.hue_identifi = @strIdPersona
				AND			gim_huellas.cdgimnasio = @idGimnasio

			END

			DECLARE @dtmUltimaEntrada DATETIME = NULL

			SELECT		TOP(1) @dtmUltimaEntrada = MAX(gim_entradas_usuarios.enusu_fecha_entrada)
			FROM		gim_entradas_usuarios
			WHERE		gim_entradas_usuarios.enusu_identifi = @strIdPersona
			AND			gim_entradas_usuarios.cdgimnasio = @idGimnasio
		
			IF @bitValidarPlan = 1
			BEGIN

				DECLARE @bitValideContPorFactura BIT = 0;

				SELECT	TOP(1)
						@bitValideContPorFactura = ISNULL(bitEstado, 0)
				FROM	tblConfiguracion_FirmaContratosAcceso
				WHERE	intFkIdTipoContrato = 4 AND cdgimnasio = @idGimnasio;

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
							branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
							courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
							classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
							employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
							subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT ISNULL(dbo.fFloatAVarchar(clientes.cli_identifi), '0') AS 'id',
							ISNULL(clientes.cli_nombres,'') + ' ' + ISNULL(clientes.cli_primer_apellido,'') + ' ' + ISNULL(clientes.cli_segundo_apellido,'') AS 'name',
							planes.pla_codigo AS 'planId',
							planes.pla_descripc AS 'planName',
							ISNULL(DATEADD(DAY, ISNULL(clientes.cli_dias_gracia,0), #tmpPlanesUsuario.plusu_fecha_vcto), NULL) AS 'expirationDate',
							@dtmUltimaEntrada AS 'lastEntry',
							planes.pla_tipo AS 'planType',
							CAST('Cliente' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planes.pla_tipo = 'T') THEN #tmpPlanesUsuario.plusu_tiq_disponible
								ELSE	datediff(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),#tmpPlanesUsuario.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM	[dbo].[fnObtenerRestriccionesWL] (clientes.cdgimnasio,clientes.cli_intcodigo_subgrupo,#tmpPlanesUsuario.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							clientes.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN CAST(0 AS BIT)
								ELSE	CAST(1 AS BIT)
							END AS 'withoutFingerprint',	
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (#tmpHuellas.hue_id)
								ELSE	CAST(0 AS BIT)
							END AS 'fingerprintId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (#tmpHuellas.hue_dato)
								ELSE	NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_bitControlIngreso,0)
							END AS 'groupEntriesControl',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_intNumlIngresos,0)
							END AS 'groupEntriesQuantity',
							ISNULL(gim_grupoFamiliar.gim_gf_IDgrupo,0) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							#tmpPlanesUsuario.plusu_numero_fact AS 'invoiceId',
							#tmpPlanesUsuario.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							ISNULL(clientes.cli_intcodigo_subgrupo,0) as 'subgroupId',
							ISNULL(clientes.cli_strcodtarjeta,'') as 'cardId',
							CASE	
								WHEN	(clientes.cli_sin_huella = 0) THEN (#tmpHuellas.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,@bitError
							,@strMensaje
				FROM		#tmpClientes clientes
				INNER JOIN	#tmpPlanesUsuario ON #tmpPlanesUsuario.plusu_identifi_cliente = clientes.cli_identifi
				INNER JOIN	gim_planes planes ON planes.pla_codigo = #tmpPlanesUsuario.plusu_codigo_plan
				AND			planes.cdgimnasio = #tmpPlanesUsuario.cdgimnasio
				INNER JOIN	gim_sucursales sucursal ON #tmpPlanesUsuario.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = clientes.cli_identifi
				AND			clientes.cli_sin_huella = 0
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = clientes.cli_identifi
				AND			gim_grupoFamiliar.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = clientes.cdgimnasio
				WHERE		CONVERT(VARCHAR(10), DATEADD(DAY, ISNULL(clientes.cli_dias_gracia, 0), #tmpPlanesUsuario.plusu_fecha_vcto), 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				--AND			((@bitValideContPorFactura = 1 AND EXISTS(SELECT 1 FROM gim_detalle_contrato WHERE gim_detalle_contrato.cdgimnasio = #tmpPlanesUsuario.cdgimnasio AND gim_detalle_contrato.dtcont_doc_cliente = clientes.cli_identifi AND gim_detalle_contrato.dtcont_numero_plan = #tmpPlanesUsuario.plusu_numero_fact AND gim_detalle_contrato.dtcont_FKcontrato = 4)) OR @bitValideContPorFactura = 0)

			END

			IF @bitValidarReserva = 1
			BEGIN

				PRINT('SE INSERTARAN LOS REGISTROS QUE TENGAN UNA RESERVA ASOCIADA')

				DECLARE @intMinutosAntesReserva INT = 0;
				DECLARE @intMinutosDespuesReserva INT = 0;

				SELECT	TOP(1)
						@intMinutosAntesReserva = ISNULL(gim_configuracion_ingreso.intMinutosAntesReserva, 0),
						@intMinutosDespuesReserva = ISNULL(gim_configuracion_ingreso.intMinutosDespuesReserva, 0)
				FROM	gim_configuracion_ingreso
				WHERE	cdgimnasio = @idGimnasio
				AND		intfkSucursal = @intIdSucursal

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
						branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
						courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
						classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
						employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
						subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT ISNULL(dbo.fFloatAVarchar(clientes.cli_identifi), '0') AS 'id',
							ISNULL(clientes.cli_nombres,'') + ' ' + ISNULL(clientes.cli_primer_apellido,'') + ' ' + ISNULL(clientes.cli_segundo_apellido,'') AS 'name',
							planes.pla_codigo AS 'planId',
							planes.pla_descripc AS 'planName',
							ISNULL(DATEADD(DAY, ISNULL(clientes.cli_dias_gracia,0), #tmpPlanesUsuario.plusu_fecha_vcto), NULL) AS 'expirationDate',
							@dtmUltimaEntrada AS 'lastEntry',
							planes.pla_tipo AS 'planType',
							CAST('Cliente' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planes.pla_tipo = 'T') THEN #tmpPlanesUsuario.plusu_tiq_disponible
								ELSE	datediff(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),#tmpPlanesUsuario.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	
									ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM
									[dbo].[fnObtenerRestriccionesWL] (clientes.cdgimnasio,clientes.cli_intcodigo_subgrupo,#tmpPlanesUsuario.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							clientes.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN CAST(0 AS BIT)
								ELSE	CAST(1 AS BIT)
							END AS 'withoutFingerprint',	
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (#tmpHuellas.hue_id)
								ELSE	CAST(0 AS BIT)
							END AS 'fingerprintId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (#tmpHuellas.hue_dato)
								ELSE	NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_bitControlIngreso,0)
							END AS 'groupEntriesControl',
							CASE
								WHEN	ISNULL(clientes.cli_GrupoFamiliar,0) = 0 THEN CAST(0 AS BIT)
								ELSE	ISNULL(gim_grupoFamiliar_Maestro.gim_gf_intNumlIngresos,0)
							END AS 'groupEntriesQuantity',
							ISNULL(gim_grupoFamiliar.gim_gf_IDgrupo,0) AS 'groupId',
							CAST(1 AS BIT) AS 'isRestrictionClass',
							CONVERT(VARCHAR(10), DATEADD(MINUTE, -@intMinutosAntesReserva, gim_reservas.fecha_clase), 114) + '-' + CONVERT(VARCHAR(10), DATEADD(MINUTE, @intMinutosDespuesReserva, gim_reservas.fecha_clase),114) AS 'classSchedule',
							gim_reservas.fecha_clase AS 'dateClass',
							gim_reservas.cdreserva AS 'reserveId',
							gim_clases.nombre AS 'className',
							ISNULL(gim_reservas.megas_utilizadas,0) AS 'utilizedMegas',
							ISNULL(gim_reservas.tiq_utilizados,0) AS 'utilizedTickets', 
							ISNULL(gim_empleados.emp_nombre,'') + ' ' + ISNULL(gim_empleados.emp_primer_apellido,'') + ' ' + ISNULL(gim_empleados.emp_segundo_apellido,'') AS 'employeeName', 
							gim_reservas.intensidad AS 'classIntensity',
							gim_reservas.estado AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							#tmpPlanesUsuario.plusu_numero_fact AS 'invoiceId',
							#tmpPlanesUsuario.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							ISNULL(clientes.cli_intcodigo_subgrupo,0) AS 'subgroupId',
							ISNULL(clientes.cli_strcodtarjeta,'') AS 'cardId',
							CASE
								WHEN	(clientes.cli_sin_huella = 0) THEN (#tmpHuellas.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,@bitError
							,@strMensaje
				FROM		#tmpClientes clientes
				INNER JOIN	#tmpPlanesUsuario ON #tmpPlanesUsuario.plusu_identifi_cliente = clientes.cli_identifi
				INNER JOIN	gim_planes planes ON planes.pla_codigo = #tmpPlanesUsuario.plusu_codigo_plan
				AND			planes.cdgimnasio = #tmpPlanesUsuario.cdgimnasio
				INNER JOIN	gim_sucursales sucursal ON #tmpPlanesUsuario.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = clientes.cli_identifi
				AND			clientes.cli_sin_huella = 0
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = clientes.cli_identifi
				AND			gim_grupoFamiliar.cdgimnasio = clientes.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = clientes.cdgimnasio
				INNER JOIN	gim_reservas ON clientes.cli_identifi = gim_reservas.IdentificacionCliente
				AND			clientes.cdgimnasio = gim_reservas.cdgimnasio
				AND			gim_reservas.cdsucursal = #tmpPlanesUsuario.plusu_sucursal
				INNER JOIN	gim_clases ON gim_reservas.cdclase = gim_clases.cdclase
				AND			gim_reservas.cdgimnasio = gim_clases.cdgimnasio
				INNER JOIN	gim_horarios_clase ON gim_horarios_clase.cdhorario_clase = gim_reservas.cdhorario_clase
				AND			gim_horarios_clase.cdgimnasio = gim_reservas.cdgimnasio
				AND			gim_horarios_clase.cdclase = gim_clases.cdclase
				INNER JOIN	gim_empleados on gim_empleados.cdempleado = gim_horarios_clase.profesor
				AND			gim_empleados.cdgimnasio = gim_horarios_clase.cdgimnasio
				WHERE		gim_reservas.estado != 'Anulada'
				AND			gim_reservas.estado != 'Asistio'
				AND			CONVERT(VARCHAR(10), DATEADD(DAY, ISNULL(clientes.cli_dias_gracia, 0), #tmpPlanesUsuario.plusu_fecha_vcto), 111) >= CONVERT(VARCHAR(10), GETDATE(), 111)
				AND			CONVERT(VARCHAR(10), gim_reservas.fecha_clase, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)

			END

			DROP TABLE #tmpClientes

		END
		ELSE IF @strTipoPersona = 'PROSPECTO'
		BEGIN
			
			PRINT('ES PROSPECTO');
			SELECT	TOP(1) *
			INTO	#tmpProspecto
			FROM	gim_clientes_especiales
			WHERE	gim_clientes_especiales.cdgimnasio = @idGimnasio
			AND		gim_clientes_especiales.cli_identifi = @strIdPersona

			IF EXISTS(SELECT 1 FROM #tmpProspecto WHERE cli_EntryFingerprint = 1)
			BEGIN

				INSERT INTO	#tmpHuellas
				SELECT		TOP(1) 
							--hue_id,
							case when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi AND	h2.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then 0 else hue_id end as hue_id ,
							hue_identifi,
							--hue_dato,
							case when (select count(*) from gim_huellas h3 where h3.hue_identifi = gim_huellas.hue_identifi AND	h3.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then null else hue_dato end as hue_dato,
							strDatoFoto
				FROM		gim_huellas
				WHERE		gim_huellas.hue_identifi = @strIdPersona
				AND			gim_huellas.cdgimnasio = @idGimnasio

			END

			DECLARE @dtmUltimaEntradaProspectos DATETIME = NULL

			SELECT		TOP(1) @dtmUltimaEntradaProspectos = MAX(gim_entradas_usuarios.enusu_fecha_entrada)
			FROM		gim_entradas_usuarios
			WHERE		gim_entradas_usuarios.enusu_identifi = @strIdPersona
			AND			gim_entradas_usuarios.cdgimnasio = @idGimnasio
			AND			CONVERT(VARCHAR(10), gim_entradas_usuarios.enusu_fecha_entrada, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)	

			IF NOT EXISTS(SELECT 1 FROM WhiteList WHERE typePerson = 'Prospecto' AND id = @strIdPersona AND gymId = @idGimnasio AND know = 1 AND courtesy = 1)
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
					branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
					courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
					classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
					employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
					subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT dbo.fFloatAVarchar(prospecto.cli_identifi) AS 'id',
							ISNULL(prospecto.cli_nombres,'') + ' ' + ISNULL(prospecto.cli_primer_apellido,'') + ' ' + ISNULL(prospecto.cli_segundo_apellido,'') AS 'name',
							CAST(0 AS INT) AS 'planId',
							CAST('' AS VARCHAR(MAX)) AS 'planName',
							NULL AS 'expirationDate',
							@dtmUltimaEntradaProspectos AS 'lastEntry',
							CAST('' AS VARCHAR(MAX)) AS 'planType',
							CAST('Prospecto' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	((prospecto.cli_cortesia = 1 AND prospecto.cli_entro_cortesia = 0) AND prospecto.cli_entrar_conocer = 1) THEN CAST(2 AS INT)
								WHEN	(prospecto.cli_cortesia = 1 AND prospecto.cli_entro_cortesia = 0) THEN CAST(1 AS INT)
								ELSE	CAST(1 AS INT)
							END AS 'availableEntries',
							CAST('|||||||' AS VARCHAR(MAX)) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							prospecto.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN (prospecto.cli_EntryFingerprint = 1) THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN (prospecto.cli_EntryFingerprint = 1) THEN #tmpHuellas.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN (prospecto.cli_EntryFingerprint = 1) THEN #tmpHuellas.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(prospecto.cli_entrar_conocer AS BIT) AS 'know',
							CAST(prospecto.cli_cortesia AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS INT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath',
							CAST(0 AS INT) AS 'invoiceId',
							CAST(0 AS INT) AS 'dianId',
							CAST('Cortes�a' AS VARCHAR(50)) AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							CAST('' AS VARCHAR(50)) AS 'cardId',
							CASE
								WHEN prospecto.cli_EntryFingerprint = 1 THEN #tmpHuellas.strDatoFoto
								ELSE NULL
							END AS 'strDatoFoto'
							,@bitError
							,@strMensaje
				FROM		#tmpProspecto prospecto
				INNER JOIN	gim_sucursales sucursal ON prospecto.cli_intfkSucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = prospecto.cdgimnasio
				LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = prospecto.cli_identifi
				AND			prospecto.cli_EntryFingerprint = 1
				WHERE		(prospecto.cli_cortesia = 1 and prospecto.cli_entro_cortesia = 0) or prospecto.cli_entrar_conocer = 1
				AND			NOT EXISTS (SELECT id FROM WhiteList WHERE id = dbo.fFloatAVarchar(prospecto.cli_identifi) AND typePerson = 'Prospecto' AND planId = 0)
			END

			IF EXISTS(SELECT 1 FROM #tmpPlanesUsuario)
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
							branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
							courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
							classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
							employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
							subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT dbo.fFloatAVarchar(prospecto.cli_identifi) AS 'id',
							ISNULL(prospecto.cli_nombres,'') + ' ' + ISNULL(prospecto.cli_primer_apellido,'') + ' ' + ISNULL(prospecto.cli_segundo_apellido,'') AS 'name',
							planes.pla_codigo AS 'planId',
							planes.pla_descripc AS 'planName',
							planesUsuario.plusu_fecha_vcto AS 'expirationDate',
							@dtmUltimaEntradaProspectos AS 'lastEntry',
							planes.pla_tipo AS 'planType',
							CAST('Prospecto' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN (planes.pla_tipo = 'T') THEN planesUsuario.plusu_tiq_disponible
								ELSE DATEDIFF(DAY, CONVERT(VARCHAR(10), GETDATE(), 111), CONVERT(VARCHAR(10), planesUsuario.plusu_fecha_vcto, 111))
							END AS 'availableEntries',
							(
								SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM [dbo].[fnObtenerRestriccionesWL](prospecto.cdgimnasio, 0, planesUsuario.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							prospecto.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN (prospecto.cli_EntryFingerprint = 1) THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN (prospecto.cli_EntryFingerprint = 1) THEN #tmpHuellas.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN (prospecto.cli_EntryFingerprint = 1) THEN #tmpHuellas.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS INT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath',
							planesUsuario.plusu_numero_fact AS 'invoiceId',
							planesUsuario.plusu_fkdia_codigo AS 'dianId',
							CAST('Cortes�a' AS VARCHAR(50)) AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							CAST('' AS VARCHAR(50)) AS 'cardId',
							CASE
								WHEN prospecto.cli_EntryFingerprint = 1 THEN #tmpHuellas.strDatoFoto
								ELSE NULL
							END AS 'strDatoFoto'
							,@bitError
							,@strMensaje
				FROM		#tmpProspecto prospecto
				INNER JOIN	#tmpPlanesUsuario planesUsuario ON planesUsuario.plusu_identifi_cliente = prospecto.cli_identifi
				INNER JOIN	gim_planes planes ON planes.pla_codigo = planesUsuario.plusu_codigo_plan
				AND			planes.cdgimnasio = planesUsuario.cdgimnasio
				INNER JOIN	gim_sucursales sucursal ON planesUsuario.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = prospecto.cdgimnasio
				LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = prospecto.cli_identifi
				AND			prospecto.cli_EntryFingerprint = 1

			END

			DROP TABLE #tmpProspecto

		END
		ELSE IF @strTipoPersona = 'VISITANTE'
		BEGIN

			PRINT('ES VISITANTE')
			SELECT	Visitors.*
			INTO	#tmpVisitante
			FROM	Visitors
			WHERE	Visitors.cdgimnasio = @idGimnasio
			AND		Visitors.vis_strVisitorId = @strIdPersona

			IF EXISTS(SELECT 1 FROM #tmpVisitante WHERE ISNULL(vis_EntryFingerprint, 0) = 1)
			BEGIN

				INSERT INTO	#tmpHuellas
				SELECT		TOP(1) 
							--hue_id,
							case when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi AND	h2.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then 0 else hue_id end as hue_id ,
							hue_identifi,
							case when (select count(*) from gim_huellas h3 where h3.hue_identifi = gim_huellas.hue_identifi AND	h3.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then null else hue_dato end as hue_dato,
							--hue_dato,
							strDatoFoto
				FROM		gim_huellas
				WHERE		gim_huellas.hue_identifi = @strIdPersona
				AND			gim_huellas.cdgimnasio = @idGimnasio

			END

			DECLARE @dtmUltimaEntradaVisitante DATETIME = NULL

			SELECT		*
			INTO		#tmpEntradasVisitante
			FROM		gim_entradas_usuarios
			WHERE		gim_entradas_usuarios.enusu_identifi = @strIdPersona
			AND			gim_entradas_usuarios.cdgimnasio = @idGimnasio
			AND			CONVERT(VARCHAR(10), gim_entradas_usuarios.enusu_fecha_entrada, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)

			SELECT		TOP(1) @dtmUltimaEntradaVisitante = ISNULL(MAX(#tmpEntradasVisitante.enusu_fecha_entrada), GETDATE())
			FROM		#tmpEntradasVisitante

			INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
						branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
						courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
						classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
						employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
						subgroupId,cardId,strDatoFoto,bitError,strMensaje)
			SELECT		DISTINCT dbo.fFloatAVarchar(visitante.vis_strVisitorId) AS 'id',
						ISNULL(visitante.vis_strName, '') + ' ' + ISNULL(visitante.vis_strFirstLastName, '') + ' ' + ISNULL(visitante.vis_strSecondLastName, '') AS 'name',
						CAST(0 AS INT) AS 'planId',
						CAST('' AS VARCHAR(MAX)) AS 'planName',
						NULL AS 'expirationDate',
						@dtmUltimaEntradaVisitante AS 'lastEntry',
						CAST('' AS VARCHAR(10)) AS 'planType',
						CAST('Visitante' AS VARCHAR(100)) AS 'typePerson',
						CASE
							WHEN visitante.bitCortesia = 1 THEN CAST(2 AS INT)
							ELSE CAST(1 AS INT)
						END AS 'availableEntries',
						CAST(' ||||||| ' AS VARCHAR(MAX)) AS 'restrictions',
						sucursal.suc_intpkIdentificacion AS 'branchId',
						sucursal.suc_strNombre AS 'branchName',
						visitante.cdgimnasio AS 'gymId',
						CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN CAST(0 AS BIT)
							ELSE CAST(1 AS BIT)
						END AS 'withoutFingerprint',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN #tmpHuellas.hue_id
							ELSE CAST(0 AS INT)
						END AS 'fingerprintId',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN #tmpHuellas.hue_dato
							ELSE NULL
						END AS 'fingerprint',
						CAST(0 AS BIT) AS 'updateFingerprint',
						CAST(0 AS BIT) AS 'know',
						CAST(0 AS BIT) AS 'courtesy',
						CAST(0 AS BIT) AS 'groupEntriesControl',
						CAST(0 AS INT) AS 'groupEntriesQuantity',
						CAST(0 AS INT) AS 'groupId',
						CAST(0 AS BIT) AS 'isRestrictionClass',
						CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
						NULL AS 'dateClass',
						CAST(0 AS INT) AS 'reserveId',
						CAST('' AS VARCHAR(200)) AS 'className',
						CAST(0 AS INT) AS 'utilizedMegas',
						CAST(0 AS INT) AS 'utilizedTickets',
						CAST('' AS VARCHAR(200)) AS 'employeeName',
						CAST('' AS VARCHAR(200)) AS 'classIntensity',
						CAST('' AS VARCHAR(100)) AS 'classState',
						CAST('' AS VARCHAR(MAX)) AS 'photoPath',
						CAST(visitas.Id AS INT) AS 'invoiceId',
						CAST(0 AS INT) AS 'dianId',
						CAST('' AS VARCHAR(50)) AS 'documentType',
						CAST(0 AS INT) AS 'subgroupId',
						CAST('' AS VARCHAR(50)) AS 'cardId',
						CASE
							WHEN visitante.vis_EntryFingerprint = 1 THEN #tmpHuellas.strDatoFoto
							ELSE NULL
						END AS 'strDatoFoto'
						,@bitError
						,@strMensaje
			FROM		#tmpVisitante visitante
			INNER JOIN	Visit visitas ON visitas.VisitorId = visitante.vis_strVisitorId
			INNER JOIN	gim_sucursales sucursal ON visitante.vis_intBranch = sucursal.suc_intpkIdentificacion
			AND			visitante.cdgimnasio = sucursal.cdgimnasio
			LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = visitante.vis_strVisitorId
			AND			visitante.vis_EntryFingerprint = 1
			WHERE CONVERT(VARCHAR(10), visitas.DateVisit, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)
			--AND (SELECT	COUNT(*)
			--			FROM	#tmpEntradasVisitante
			--			WHERE	#tmpEntradasVisitante.enusu_VisitId = visitas.Id) = 0

			DROP TABLE #tmpVisitante

		END
		ELSE IF @strTipoPersona = 'EMPLEADO'
		BEGIN

			SELECT	*
			INTO	#tmpEmpleados
			FROM	gim_empleados
			WHERE	emp_identifi = @strIdPersona
			AND		cdgimnasio = @idGimnasio

			IF EXISTS(SELECT 1 FROM #tmpEmpleados WHERE emp_sin_huella = 0)
			BEGIN

				INSERT INTO	#tmpHuellas
				SELECT		TOP(1) 
							--hue_id,
							case when (select count(*) from gim_huellas h2 where h2.hue_identifi = gim_huellas.hue_identifi AND	h2.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then 0 else hue_id end as hue_id ,
							hue_identifi,
							case when (select count(*) from gim_huellas h3 where h3.hue_identifi = gim_huellas.hue_identifi AND	h3.cdgimnasio = gim_huellas.cdgimnasio ) > 2 then null else hue_dato end as hue_dato,
							--hue_dato,
							strDatoFoto
				FROM		gim_huellas
				WHERE		gim_huellas.hue_identifi = @strIdPersona
				AND			gim_huellas.cdgimnasio = @idGimnasio

			END

			IF EXISTS(SELECT 1 FROM gim_configuracion_ingreso WHERE bitIngresoEmpSinPlan = 1 AND intfkSucursal = @intIdSucursal)
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
					branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
					courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
					classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
					employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
					subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT dbo.fFloatAVarchar(empleados.emp_identifi) AS 'id',
							ISNULL(empleados.emp_nombre, '') + ' ' + ISNULL(empleados.emp_primer_apellido, '') + ' ' + ISNULL(empleados.emp_segundo_apellido, '') AS 'name',
							CAST(0 AS INT) AS 'planId',
							CAST('' AS VARCHAR(100)) AS 'planName',
							NULL AS 'expirationDate',
							NULL AS 'lastEntry',
							CAST('' AS VARCHAR(50)) AS 'planType',
							CAST('Empleado' AS VARCHAR(100)) AS 'typePerson',
							CAST(0 AS INT) AS 'availableEntries',
							CAST('|||||||' AS VARCHAR(MAX)) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							empleados.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(20)) AS 'personState',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN #tmpHuellas.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN #tmpHuellas.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS INT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath',
							CAST(0 AS INT) AS 'invoiceId',
							CAST(0 AS INT) AS 'dianId',
							CAST('' AS VARCHAR(50)) AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							ISNULL(CONVERT(VARCHAR(50), empleados.emp_strcodtarjeta), '') AS 'cardId',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN #tmpHuellas.strDatoFoto
								ELSE NULL
							END AS 'strDatoFoto'
							,@bitError
							,@strMensaje
				FROM		#tmpEmpleados empleados
				INNER JOIN	gim_sucursales sucursal ON empleados.emp_sucursal = sucursal.suc_intpkIdentificacion
				AND			empleados.cdgimnasio = sucursal.cdgimnasio
				LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = empleados.emp_identifi
				AND			ISNULL(empleados.emp_sin_huella, 0) = 0
				WHERE		NOT EXISTS (SELECT id FROM WhiteList WHERE id = dbo.fFloatAVarchar(empleados.emp_identifi) AND typePerson = 'Empleado' AND planId = 0)

			END
			ELSE 
			BEGIN

				INSERT INTO WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
					branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,updateFingerprint,know,
					courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
					classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
					employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
					subgroupId,cardId,strDatoFoto,bitError,strMensaje)
				SELECT		DISTINCT dbo.fFloatAVarchar(empleados.emp_identifi) AS 'id',
							ISNULL(empleados.emp_nombre, '') + ' ' + ISNULL(empleados.emp_primer_apellido, '') + ' ' + ISNULL(empleados.emp_segundo_apellido, '') AS 'name',
							planes.pla_codigo AS 'planId',
							planes.pla_descripc AS 'planName',
							planesUsuarios.plusu_fecha_vcto AS 'expirationDate',
							NULL AS 'lastEntry',
							planes.pla_tipo AS 'planType',
							CAST('Empleado' AS VARCHAR(100)) AS 'typePerson',
							CASE
								WHEN	(planes.pla_tipo = 'T') THEN planesUsuarios.plusu_tiq_disponible
								ELSE	DATEDIFF(day, CONVERT(VARCHAR(10),getdate(),111), CONVERT(VARCHAR(10),planesUsuarios.plusu_fecha_vcto,111)) 
							END AS 'availableEntries',
							(
								SELECT	ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||')
								FROM	[dbo].[fnObtenerRestriccionesWL] (empleados.cdgimnasio, 0, planesUsuarios.plusu_codigo_plan)
							) AS 'restrictions',
							sucursal.suc_intpkIdentificacion AS 'branchId',
							sucursal.suc_strNombre AS 'branchName',
							empleados.cdgimnasio AS 'gymId',
							CAST('Pendiente' AS VARCHAR(30)) AS 'personState',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN CAST(0 AS BIT)
								ELSE CAST(1 AS BIT)
							END AS 'withoutFingerprint',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN #tmpHuellas.hue_id
								ELSE CAST(0 AS INT)
							END AS 'fingerprintId',
							CASE
								WHEN ISNULL(empleados.emp_sin_huella, 0) = 0 THEN #tmpHuellas.hue_dato
								ELSE NULL
							END AS 'fingerprint',
							CAST(0 AS BIT) AS 'updateFingerprint',
							CAST(0 AS BIT) AS 'know',
							CAST(0 AS BIT) AS 'courtesy',
							CAST(0 AS BIT) AS 'groupEntriesControl',
							CAST(0 AS BIT) AS 'groupEntriesQuantity',
							CAST(0 AS INT) AS 'groupId',
							CAST(0 AS BIT) AS 'isRestrictionClass',
							CAST('' AS VARCHAR(MAX)) AS 'classSchedule',
							NULL AS 'dateClass',
							CAST(0 AS INT) AS 'reserveId',
							CAST('' AS VARCHAR(200)) AS 'className',
							CAST(0 AS INT) AS 'utilizedMegas',
							CAST(0 AS INT) AS 'utilizedTickets',
							CAST('' AS VARCHAR(200)) AS 'employeeName',
							CAST('' AS VARCHAR(200)) AS 'classIntensity',
							CAST('' AS VARCHAR(100)) AS 'classState',
							CAST('' AS VARCHAR(MAX)) AS 'photoPath', 
							planesUsuarios.plusu_numero_fact AS 'invoiceId',
							planesUsuarios.plusu_fkdia_codigo AS 'dianId',
							strTipoRegistro AS 'documentType',
							CAST(0 AS INT) AS 'subgroupId',
							ISNULL(CONVERT(VARCHAR(50), empleados.emp_strcodtarjeta), '') AS 'cardId',
							CASE	
								WHEN	(empleados.emp_sin_huella = 0) THEN (#tmpHuellas.strDatoFoto)
								ELSE	NULL
							END AS 'strDatoFoto'
							,@bitError
							,@strMensaje
				FROM		#tmpEmpleados empleados
				INNER JOIN	#tmpPlanesUsuario planesUsuarios ON planesUsuarios.plusu_identifi_cliente = empleados.emp_identifi
				INNER JOIN	gim_planes planes ON planes.pla_codigo = planesUsuarios.plusu_codigo_plan
				AND			planes.cdgimnasio = planesUsuarios.cdgimnasio
				INNER JOIN	gim_sucursales sucursal ON planesUsuarios.plusu_sucursal = sucursal.suc_intpkIdentificacion
				AND			sucursal.cdgimnasio = empleados.cdgimnasio
				LEFT JOIN	#tmpHuellas ON #tmpHuellas.hue_identifi = empleados.emp_identifi
				AND			ISNULL(empleados.emp_sin_huella, 0) = 0
				LEFT JOIN	gim_grupoFamiliar ON gim_grupoFamiliar.gim_gf_IDCliente = empleados.emp_identifi
				AND			gim_grupoFamiliar.cdgimnasio = empleados.cdgimnasio
				LEFT JOIN	gim_grupoFamiliar_Maestro ON gim_grupoFamiliar_Maestro.gim_gf_pk_IDgrupo = gim_grupoFamiliar.gim_gf_IDgrupo
				AND			gim_grupoFamiliar_Maestro.cdgimnasio = empleados.cdgimnasio
				WHERE		NOT EXISTS (SELECT id FROM WhiteList WHERE id = dbo.fFloatAVarchar(empleados.emp_identifi) AND typePerson = 'Empleado' AND planId = planes.pla_codigo)

			END

		END


		SET @procesoCompletado = 1;

    END

    RETURN @procesoCompletado;
END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spActualizarCantidadTiquetesWeb' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spActualizarCantidadTiquetesWeb
END
GO
CREATE PROCEDURE [dbo].[spActualizarCantidadTiquetesWeb]
    @id INT = NULL, 
    @invoiceId INT = NULL, 
    @dianId INT = NULL, 
    @documentType VARCHAR(MAX) = NULL, 
    @availableEntries INT = NULL, 
    @gymId INT = NULL, 
    @branchId varchar(max) = NULL,
	@planId INT = NULL
AS
BEGIN

DECLARE @intMinutosDescontarTiquetes BIT = (SELECT intMinutosDescontarTiquetes FROM gim_configuracion_ingreso WHERE cdgimnasio = @gymId and intfkSucursal = @branchId)
declare @respuesta bit = 0
declare @planType varchar(max) = (select top (1) planType from WhiteList where id = @id and gymId = @gymId and branchId = @branchId)

if(@planType = 'T')
BEGIN

select top 1 *
into #tmpCantidadEntradas
from gim_entradas_usuarios
WHERE enusu_identifi = @id and enusu_intnumero = @invoiceId and enusu_plan = @planId
and enusu_fecha_entrada is not null
order by enusu_numero desc

if(DATEdiff(MINUTE,getdate(),(select enusu_fecha_entrada from #tmpCantidadEntradas)) < @intMinutosDescontarTiquetes)
begin 
	set @respuesta = 1
end 

IF(@respuesta = 1)
BEGIN

   IF (@documentType = 'Factura')
    BEGIN
        UPDATE gim_planes_usuario
        SET plusu_tiq_disponible = @availableEntries
        WHERE plusu_numero_fact = @invoiceId
            AND plusu_fkdia_codigo = @dianId
            AND cdgimnasio = @gymId
            AND plusu_sucursal = @branchId
            AND plusu_identifi_cliente = @id
    END

    IF (@documentType = 'Cortesia')
    BEGIN

		IF @invoiceId > 0
		BEGIN

			UPDATE gim_planes_usuario_especiales
			SET plusu_tiq_disponible = @availableEntries
			WHERE plusu_numero_fact = @invoiceId
				AND plusu_fkdia_codigo = @dianId
				AND cdgimnasio = @gymId
				AND plusu_sucursal = @branchId
				AND plusu_identifi_cliente = @id

		END


END 
SELECT @respuesta as respuesta

END 

 
		--ELSE
		--BEGIN

		--	IF @availableEntries > 0
		--	BEGIN

		--		UPDATE	WhiteList
		--		SET		availableEntries = @availableEntries
		--		WHERE	id = @id
		--		AND		branchId = @branchId
		--		AND		documentType = 'Cortes�a'
		--		AND		invoiceId = @invoiceId
		--		AND		dianId = @dianId
				
		--	END
		--	ELSE
		--	BEGIN

		--		UPDATE	WhiteList
		--		SET		availableEntries = @availableEntries,
		--				personState = 'Eliminar'
		--		WHERE	id = @id
		--		AND		branchId = @branchId
		--		AND		documentType = 'Cortes�a'
		--		AND		invoiceId = @invoiceId
		--		AND		dianId = @dianId

		--	END

		--END

   END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spWhiteListTipoAcceso' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spWhiteListTipoAcceso
END
GO
ALTER procedure [dbo].[spWhiteListTipoAcceso]
	@action varchar(50) = '',
	@id varchar(max) = null,
	@branchId varchar(max) = null,
	@gymId int = null,
	@option bit = null,
	@groupId int = 0

	
as
begin
 DECLARE @bitValidarPlanYReservaWeb BIT = 0
 DECLARE @bitAccesoPorReservaWeb BIT = 0
 DECLARE @strTipoPersona VARCHAR(MAX) = ''
 DECLARE @idCantidadREgistrosGrupoFamiliar INT = 0

  set @bitValidarPlanYReservaWeb = (select bitValidarPlanYReservaWeb from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId)
  set @bitAccesoPorReservaWeb = (select bitAccesoPorReservaWeb from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId)

	if (@option = 0 AND @action = 'GetListarCliente')
	begin
		select *, 'huella' as tipo
		from WhiteList
		where fingerprintId = convert(float,@id)
		and branchId in (select value from STRING_SPLIT(@branchId,','))
		union
		select *, case when withoutFingerprint = 'true' then 'tarjeta' else 'huella' end as tipo
		from WhiteList
		where id = @id
		and branchId in (select value from STRING_SPLIT(@branchId,','))
		union
		select *, 'tarjeta' as tipo
		from WhiteList
		where cardId = @id
		and branchId in (select value from STRING_SPLIT(@branchId,','))

	end

	IF(@option = 1 AND (@bitValidarPlanYReservaWeb = 1 OR (@bitValidarPlanYReservaWeb = 0  OR @bitAccesoPorReservaWeb = 0)))
	BEGIN 
	SET @action = '11111'

	

		SET @strTipoPersona = (SELECT TOP(1) typePerson FROM WhiteList WHERE id = @id)

		IF (@strTipoPersona = 'Prospecto')
		BEGIN
			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					isnull((SELECT TOP 1 enusu_fecha_entrada FROM gim_entradas_usuarios WHERE enusu_identifi = @id ORDER BY enusu_numero DESC),(lastEntry)) as lastEntry,
					--lastEntry,
					planType,
					typePerson,
					availableEntries,
					restrictions,
					branchId,
					branchName,
					gymId,
					personState,
					withoutFingerprint,
					fingerprintId,
					fingerprint,
					strDatoFoto,
					ISNULL(updateFingerprint,0) AS updateFingerprint,
					know,
					courtesy, 
					groupEntriesControl,
					groupEntriesQuantity,
					groupId,
					isRestrictionClass,
					classSchedule,
					dateClass,
					reserveId,
					className,
					utilizedMegas,
					utilizedTickets,
					employeeName,
					classIntensity,
					classState,
					photoPath,
					invoiceId,
					dianId,
					documentType,
					subgroupId,
					cardId,
					'' as tipo
			FROM	WhiteList
			WHERE	id = @id
			AND		((WhiteList.invoiceId = 0 AND WhiteList.documentType = 'Cortes�a' 
			AND (SELECT count(*) enusu_fecha_entrada FROM gim_entradas_usuarios WHERE enusu_identifi = @id) 
			< WhiteList.availableEntries) OR  WhiteList.invoiceId <> 0)

		END
		ELSE
		BEGIN
		
			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					isnull((SELECT TOP 1 enusu_fecha_entrada FROM gim_entradas_usuarios WHERE enusu_identifi = @id ORDER BY enusu_numero DESC),(lastEntry)) as lastEntry,
					--lastEntry,
					planType,
					typePerson,
					availableEntries,
					restrictions,
					branchId,
					branchName,
					gymId,
					personState,
					withoutFingerprint,
					fingerprintId,
					fingerprint,
					strDatoFoto,
					ISNULL(updateFingerprint,0) AS updateFingerprint,
					know,
					courtesy, 
					groupEntriesControl,
					groupEntriesQuantity,
					groupId,
					isRestrictionClass,
					classSchedule,
					dateClass,
					reserveId,
					className,
					utilizedMegas,
					utilizedTickets,
					employeeName,
					classIntensity,
					classState,
					photoPath,
					invoiceId,
					dianId,
					documentType,
					subgroupId,
					cardId,
					'' as tipo
			FROM	WhiteList
			WHERE	id = @id

		END
	END 
	ELSE if (@action <> 'GetUsersFamilyGroup')
	BEGIN
			SET @strTipoPersona = (SELECT TOP(1) typePerson FROM WhiteList WHERE id = @id)
		
		IF(@strTipoPersona = 'Cliente')
		BEGIN 

			SELECT	intPkId,id,
					name,
					planId,
					planName,
					expirationDate,
					CASE 
						WHEN lastEntry IS NULL THEN (SELECT TOP 1 enusu_fecha_entrada FROM gim_entradas_usuarios WHERE enusu_identifi = @id ORDER BY enusu_numero DESC)
						ELSE GETDATE()
					END AS lastEntry,
					planType,
					typePerson,
					availableEntries,
					restrictions,
					branchId,
					branchName,
					gymId,
					personState,
					withoutFingerprint,
					fingerprintId,
					fingerprint,
					strDatoFoto,
					ISNULL(updateFingerprint,0) AS updateFingerprint,
					know,
					courtesy, 
					groupEntriesControl,
					groupEntriesQuantity,
					groupId,
					isRestrictionClass,
					classSchedule,
					dateClass,
					reserveId,
					className,
					utilizedMegas,
					utilizedTickets,
					employeeName,
					classIntensity,
					classState,
					photoPath,
					invoiceId,
					dianId,
					documentType,
					subgroupId,
					cardId,
					convert(varchar(10),SUBSTRING(classSchedule,0,9),108) as 'horaInicial',
					convert(varchar(10),SUBSTRING(classSchedule,12,22),108) as 'horaFinal',
					'' as tipo
			INTO	#tmpWhiteList
			FROM	WhiteList
			WHERE	convert(varchar(10),dateClass,120) = convert(varchar(10),getdate(),120)
			AND		id = @id

			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					lastEntry,
					planType,
					typePerson,
					availableEntries,
					restrictions,
					branchId,
					branchName,
					gymId,
					personState,
					withoutFingerprint,
					fingerprintId,
					fingerprint,
					strDatoFoto,
					isnull(updateFingerprint,0)as updateFingerprint,
					know,
					courtesy,
					groupEntriesControl,
					groupEntriesQuantity,
					groupId,
					isRestrictionClass,
					classSchedule,
					dateClass,
					reserveId,
					className,
					utilizedMegas,
					utilizedTickets,
					employeeName,
					classIntensity,
					classState,
					photoPath,
					invoiceId,
					dianId,
					documentType,
					subgroupId,
					cardId,
					tipo
			FROM	#tmpWhiteList
			WHERE	LEFT(CONVERT(TIME,GETDATE(),108),8) BETWEEN LEFT(CONVERT(TIME,horaInicial,108),8) AND LEFT(CONVERT(TIME,horaFinal,108),8)
		
			DROP TABLE #tmpWhiteList
		END
		ELSE IF (@strTipoPersona = 'Prospecto')
		BEGIN

			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					lastEntry,
					planType,
					typePerson,
					availableEntries,
					restrictions,
					branchId,
					branchName,
					gymId,
					personState,
					withoutFingerprint,
					fingerprintId,
					fingerprint,
					strDatoFoto,
					ISNULL(updateFingerprint,0) AS updateFingerprint,
					know,
					courtesy, 
					groupEntriesControl,
					groupEntriesQuantity,
					groupId,
					isRestrictionClass,
					classSchedule,
					dateClass,
					reserveId,
					className,
					utilizedMegas,
					utilizedTickets,
					employeeName,
					classIntensity,
					classState,
					photoPath,
					invoiceId,
					dianId,
					documentType,
					subgroupId,
					cardId
					,'' as tipo
			FROM	WhiteList
			WHERE	id = @id
			AND		((WhiteList.invoiceId = 0 AND WhiteList.documentType = 'Cortes�a'
			AND (SELECT COUNT(*) FROM gim_entradas_usuarios WHERE enusu_identifi = @id AND planId = 0) < WhiteList.availableEntries) 
			OR  WhiteList.invoiceId <> 0)

		END
		ELSE
		BEGIN

			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					CASE 
						WHEN lastEntry IS NULL THEN (SELECT TOP 1 enusu_fecha_entrada FROM gim_entradas_usuarios WHERE enusu_identifi = @id ORDER BY enusu_numero DESC)
						ELSE GETDATE()
					END AS lastEntry,
					planType,
					typePerson,
					availableEntries,
					restrictions,
					branchId,
					branchName,
					gymId,
					personState,
					withoutFingerprint,
					fingerprintId,
					fingerprint,
					strDatoFoto,
					ISNULL(updateFingerprint,0) AS updateFingerprint,
					know,
					courtesy, 
					groupEntriesControl,
					groupEntriesQuantity,
					groupId,
					isRestrictionClass,
					classSchedule,
					dateClass,
					reserveId,
					className,
					utilizedMegas,
					utilizedTickets,
					employeeName,
					classIntensity,
					classState,
					photoPath,
					invoiceId,
					dianId,
					documentType,
					subgroupId,
					cardId
					, '' as tipo
			FROM	WhiteList
			WHERE	id = @id

		END

	END 

	if (@option = 0 AND @action = 'GetUsersFamilyGroup')
	begin
		select id
		into #tmpIdBusqueda
		from WhiteList
		where typePerson not in ('Empleado','Visitante') and groupId = @groupId

		IF((SELECT COUNT(*) FROM #tmpIdBusqueda) > 0)
		BEGIN
		SET @idCantidadREgistrosGrupoFamiliar =( SELECT count(*) 
					   from gim_entradas_usuarios
					   where  convert(varchar(10),enusu_fecha_entrada,111) = convert(varchar(10),getdate(),111)
							 and enusu_identifi in (select id from #tmpIdBusqueda))
		END 
		ELSE 
		BEGIN
		SET @idCantidadREgistrosGrupoFamiliar =  0
		END 

		SELECT @idCantidadREgistrosGrupoFamiliar AS idCantidadREgistrosGrupoFamiliar

	end

	end
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spConsultarEliminacionContratos' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spConsultarEliminacionContratos
END
GO
CREATE procedure [dbo].[spConsultarEliminacionContratos]
	@id varchar(max) = null,
	@intIdGimnasio varchar(max) = ''

	as
begin

	DECLARE @bitValidarContrato BIT = 0;
	DECLARE @bitValidarContratoEntrenamiento BIT = 0;
	DECLARE @bitValidarConsentimientoInformado BIT = 0;
	DECLARE @bitValideContPorFactura BIT = 0;
	DECLARE @bitValidarConsentimientoDatosBiometricos BIT = 0;
	DECLARE @bitValidarReglamentoDeUso BIT = 0;

		CREATE TABLE #tmpDatosContratos (
			dtcont_FKcontrato INT NOT NULL,
			dtcont_doc_cliente VARCHAR(15) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
		)

			select distinct id 
		into #tmpIdentificacionCliente
		from  WhiteList 
		where fingerprintId = convert(float,@id)
		union
		select distinct id 
		from WhiteList 
		where id = @id
		union
		select id
		from WhiteList 
		where cardId = @id
		

		INSERT INTO	#tmpDatosContratos
		SELECT		DISTINCT
					gim_detalle_contrato.dtcont_FKcontrato,
					gim_detalle_contrato.dtcont_doc_cliente
		FROM		gim_detalle_contrato
		WHERE		gim_detalle_contrato.cdgimnasio = @intIdGimnasio
		and			gim_detalle_contrato.dtcont_doc_cliente = (select top 1 id from #tmpIdentificacionCliente)

----- que contratos estan activos  
		SELECT	TOP(1)
				@bitValidarContrato = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 1 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarContratoEntrenamiento = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 2 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarConsentimientoInformado = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 3 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValideContPorFactura = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 4 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarConsentimientoDatosBiometricos = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 5 AND cdgimnasio = @intIdGimnasio;

		SELECT	TOP(1)
				@bitValidarReglamentoDeUso = ISNULL(bitEstado, 0)
		FROM	tblConfiguracion_FirmaContratosAcceso
		WHERE	intFkIdTipoContrato = 6 AND cdgimnasio = @intIdGimnasio;


	
	select *
	from WhiteList
	inner join #tmpIdentificacionCliente on WhiteList.id = #tmpIdentificacionCliente.id
	where ((@bitValidarContrato = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE	#tmpDatosContratos.dtcont_FKcontrato = 1 AND #tmpDatosContratos.dtcont_doc_cliente = WhiteList.id)) OR @bitValidarContrato = 0)
		 AND			((@bitValidarContratoEntrenamiento = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE	#tmpDatosContratos.dtcont_FKcontrato = 2 AND #tmpDatosContratos.dtcont_doc_cliente = WhiteList.id)) OR @bitValidarContratoEntrenamiento = 0)	 
		AND			((@bitValidarConsentimientoInformado = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 3 AND #tmpDatosContratos.dtcont_doc_cliente = WhiteList.id)) OR @bitValidarConsentimientoInformado = 0)
		AND			((@bitValidarConsentimientoDatosBiometricos = 1 AND EXISTS(SELECT 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 5 AND #tmpDatosContratos.dtcont_doc_cliente = WhiteList.id)) OR @bitValidarConsentimientoDatosBiometricos = 0)
		AND			((@bitValidarReglamentoDeUso = 1 AND EXISTS(SELECT TOP(1) 1 FROM #tmpDatosContratos WHERE #tmpDatosContratos.dtcont_FKcontrato = 6 AND #tmpDatosContratos.dtcont_doc_cliente = WhiteList.id)) OR @bitValidarReglamentoDeUso = 0)
		

end 
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spRespuestaIngresosVisitantes' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spRespuestaIngresosVisitantes
END
GO
CREATE procedure [dbo].[spRespuestaIngresosVisitantes]
	@id varchar(max) = null,
	@intIdGimnasio varchar(max) = ''

	as
begin

	SELECT COUNT(*) as 'cantidadEntradasPersonas',enusu_identifi
	into #tmpCantidadEntradasPersonas
	FROM gim_entradas_usuarios
	where enusu_identifi = @id
	group by enusu_identifi

	select top 1 id
	from whitelist
	where id = @id
	and( typePerson = 'Prospecto' or typePerson = 'Visitante')
	and (select cantidadEntradasPersonas from #tmpCantidadEntradasPersonas where whitelist.id = #tmpCantidadEntradasPersonas.enusu_identifi) >= availableEntries

end 
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spFingerprint' AND type in (N'P'))
BEGIN
    DROP PROCEDURE spFingerprint
END
GO
ALTER PROCEDURE [dbo].[spFingerprint]
@action varchar(50) = '',
@personId varchar(50) = '',
@cdgimnasio int = 0,
@strSucursal varchaR(max) = ''

as
begin
if(@personId = '0')
begin
set @personId = null
end 

if (@action = 'GetAllFingerprints')
	begin
		select *
		from gim_huellas
		where cdgimnasio = @cdgimnasio
		and (@personId IS NULL  OR gim_huellas.hue_identifi = @personId)
	
	end

	if (@action = 'GetFingerprintsByGym')
	begin
		select *
		from gim_huellas
		where cdgimnasio = @cdgimnasio
	
	end

end
GO
-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
----------------------------------FIN CREACION DE PROCEDIMIENTOS-------------------------------------
-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------


-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------
----------------------------------INCIO CREACION DE TRIGERS-----------------------------------------
-----------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_InvoiceNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_InvoiceNuevo
END
GO
CREATE  TRIGGER [dbo].[trgWhiteList_InvoiceNuevo] ON [dbo].[gim_planes_usuario]
AFTER INSERT, UPDATE
AS
BEGIN
	
	IF EXISTS (SELECT 1 FROM inserted WHERE plusu_codigo_plan = '999')
	BEGIN
		RETURN;
	END

	DECLARE @intIdGimnasio INT, @intIdSucursal INT, @strIdPersona VARCHAR(MAX), @intNumeroFactura INT;

	SELECT	@intIdGimnasio = cdgimnasio,
			@intIdSucursal = ISNULL(plusu_sucursal, 0),
			@strIdPersona = ISNULL(plusu_identifi_cliente, ''),
			@intNumeroFactura = ISNULL(plusu_numero_fact, 0)
	FROM	inserted;

	DECLARE @bitUsaListaBlanca BIT = ISNULL((SELECT TOP(1) bitIngressWhiteList FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio), 0);
		
	IF @bitUsaListaBlanca = 1
	BEGIN
		PRINT('USA LISTA BLANCA')

		DECLARE @bitValidaReservaWeb BIT = 0;
		DECLARE @bitValidarPlanYReservaWeb BIT = 0;
					
		SELECT	@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
				@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
		FROM	gim_configuracion_ingreso 
		WHERE	cdgimnasio = @intIdGimnasio
		AND		intfkSucursal = @intIdSucursal

		DECLARE @TablaResultado TABLE (
						bitError BIT,
						strMensaje varchar(max),
						strTipoPersona varchar(max)
					);
		declare @bitErrorTabla bit = 0
		declare @strMensajeTabla varchar(max) = ''

		IF @bitValidaReservaWeb = 0 or @bitValidarPlanYReservaWeb = 1
		BEGIN
				
			-- SE VALIDA SI LA FACTURA QUE SE ESTA CREANDO O MODIFICANDO YA EXISTE EN LISTA BLANCA
			IF NOT EXISTS(SELECT 1 FROM WhiteList WHERE id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura)
			BEGIN

				PRINT('LA FACTURA NO EXISTE EN LISTA BLANCA')

				-- SI LA FACTURA NO EXISTE EN LISTA BLANCA
				DECLARE	@return_value BIT

				insert into @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

				IF (select count(*) from @TablaResultado) = 1
				BEGIN
					set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
					set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)
						
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL REGISTRO A LISTA BLANCA')
					EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
							@idGimnasio = @intIdGimnasio,
							@intIdSucursal = @intIdSucursal,
							@strIdPersona = @strIdPersona,
							@strTrigger = 'gim_planes_usuario',
							@strTipoPersona = 'CLIENTE',
							@bitValidarReserva = 0,
							@bitValidarPlan = 1,
							@intNumeroFactura = @intNumeroFactura,
							@bitError = @bitErrorTabla,
							@strMensaje = @strMensajeTabla

					IF @return_value = 1
					BEGIN
						PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL REGISTRO A LISTA BLANCA')
					END
					ELSE
					BEGIN
						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
					END

				END
				ELSE
				BEGIN

					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

				END

			END
			ELSE 
			BEGIN

				insert into @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				PRINT('LA FACTURA YA EXISTE EN LISTA BLANCA')
					
				DECLARE @bitFacturaAnulada BIT = 0, @bitFacturaAvisada BIT = 0;
				DECLARE @strTipoPlan VARCHAR(MAX) = '', @intTiquetesDisponibles INT = 0;
				DECLARE @dtmFechaInicial DATETIME = NULL, @dtmFechaFinal DATETIME = NULL;
				DECLARE @bitRegistroEliminado BIT = 0;

				SELECT		@bitFacturaAnulada = ISNULL(plusu_est_anulada, 0),
							@bitFacturaAvisada = ISNULL(plusu_avisado, 0),
							@strTipoPlan = ISNULL(gim_planes.pla_tipo, ''),
							@intTiquetesDisponibles = ISNULL(CASE WHEN (gim_planes.pla_tipo = 'T') THEN inserted.plusu_tiq_disponible ELSE DATEDIFF(DAY, CONVERT(VARCHAR(10),GETDATE(),111), CONVERT(VARCHAR(10),inserted.plusu_fecha_vcto,111)) END, 0),
							@dtmFechaInicial = ISNULL(plusu_fecha_inicio, NULL),
							@dtmFechaFinal = ISNULL(DATEADD(DAY, ISNULL(gim_clientes.cli_dias_gracia,0), plusu_fecha_vcto), NULL)
				FROM		inserted
				LEFT JOIN	gim_planes ON gim_planes.pla_codigo = inserted.plusu_codigo_plan AND gim_planes.cdgimnasio = inserted.cdgimnasio
				INNER JOIN	gim_clientes ON gim_clientes.cli_identifi = inserted.plusu_identifi_cliente AND gim_clientes.cdgimnasio = inserted.cdgimnasio;

				IF @bitRegistroEliminado = 0
				BEGIN

					IF(@bitFacturaAnulada = 1 OR @bitFacturaAvisada = 1)
					BEGIN

						PRINT('LA FACTURA SE ANULO O SE MARCO COMO AVISADA')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'LA FACTURA SE ANULO O SE MARCO COMO AVISADA'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'
						
						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (@bitFacturaAnulada != 1 AND @bitFacturaAvisada != 1)
					BEGIN

						PRINT('LA FACTURA SE DES-ANULO O SE DES-MARCO COMO AVISADA')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'

					END

				END

				IF @bitRegistroEliminado = 0
				BEGIN

					IF (@strTipoPlan = 'T' AND @intTiquetesDisponibles <= 0)
					BEGIN

						PRINT('EL PLAN ES TIQUETERA Y LA PERSONA YA NO CUENTA CON TIQUETES')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'EL PLAN ES TIQUETERA Y LA PERSONA YA NO CUENTA CON TIQUETES'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'
						
						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (@strTipoPlan = 'T' AND @intTiquetesDisponibles > 0)
					BEGIN

						PRINT('EL PLAN ES TIQUETERA Y LA PERSONA NO TENIA TIQUETES Y SE LE AGREGARON')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'
						
					END

				END

				IF @bitRegistroEliminado = 0
				BEGIN

					IF (CONVERT(VARCHAR(10),@dtmFechaInicial,111) < CONVERT(VARCHAR(10),GETDATE(),111) AND CONVERT(VARCHAR(10),@dtmFechaFinal,111) < CONVERT(VARCHAR(10),GETDATE(),111))
					BEGIN

						PRINT('SE CAMBIARON LAS FECHAS DE LA FACTURA Y ESTA QUEDO VENCIDA')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'SE CAMBIARON LAS FECHAS DE LA FACTURA Y ESTA QUEDO VENCIDA'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (CONVERT(VARCHAR(10),@dtmFechaInicial,111) > CONVERT(VARCHAR(10),GETDATE(),111) AND CONVERT(VARCHAR(10),@dtmFechaFinal,111) > CONVERT(VARCHAR(10),GETDATE(),111))
					BEGIN
					
						PRINT('LA FACTURA AUN NO SE ENCUENTRA EN VIGENCIA')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'LA FACTURA AUN NO SE ENCUENTRA EN VIGENCIA'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (convert(varchar(10),@dtmFechaInicial,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),@dtmFechaFinal,111) >= convert(varchar(10),getdate(),111))
					BEGIN

						PRINT('SE CAMBIARON LAS FECHAS DE LA FACTURA Y ESTA SE VOLVIO VIGENTE')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'

					END
					ELSE
					BEGIN

						PRINT('SE ACTUALIZAN UNICAMENTE LAS VIGENCIAS DE LA FACTURA Y EL NUMERO DE TIQUETES')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'

					END

				END

				IF	@bitRegistroEliminado = 0
				BEGIN

				PRINT('SE CONSULTAN CONGELACION FACTURA')

				CREATE TABLE #tmpFacturaCongeladas (
					invoiceId INT,
					sucursal int,
					cdgimnasio int,
					codigoDian int
				)

				INSERT INTO #tmpFacturaCongeladas (invoiceId,sucursal,cdgimnasio,codigoDian)
				SELECT	gim_con_fac.num_fac_con,con_sucursal,cdgimnasio,con_fkdia_codigo
				FROM	gim_con_fac
				WHERE	gim_con_fac.des_con = 0
				AND		CONVERT(varchar(10), GETDATE(), 111) BETWEEN CONVERT(varchar(10), gim_con_fac.fec_ini_con, 111) AND CONVERT(varchar(10), gim_con_fac.fec_ter_con, 111)
				AND		gim_con_fac.cdgimnasio = @intIdGimnasio
				AND		gim_con_fac.con_sucursal = @intIdSucursal

				UPDATE	WhiteList
				SET		personState = 'Eliminar'
						,bitError = 1
						,strMensaje = 'SE CONSULTAN CONGELACION FACTURA'
				WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'
				and (select  count(*) from #tmpFacturaCongeladas where invoiceId = WhiteList.invoiceId and sucursal = WhiteList.branchId and codigoDian = WhiteList.dianId and cdgimnasio = WhiteList.gymId ) > 0

				SET @bitRegistroEliminado = 1;



				END 

			END
			
		END
			
	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_CourtesynNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_CourtesynNuevo
END
GO
CREATE  TRIGGER [dbo].[trgWhiteList_CourtesynNuevo] ON [dbo].[gim_planes_usuario_especiales]
AFTER INSERT, UPDATE
AS
BEGIN

	IF EXISTS (SELECT 1 FROM inserted WHERE plusu_codigo_plan = '999')
	BEGIN
		RETURN;
	END

	DECLARE @intIdGimnasio INT, @intIdSucursal INT, @strIdPersona VARCHAR(MAX), @intNumeroFactura INT;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
			@intIdSucursal = ISNULL(plusu_sucursal,0),
			@strIdPersona = ISNULL(plusu_identifi_cliente, ''),
			@intNumeroFactura = ISNULL(plusu_numero_fact, 0)
	FROM	inserted

	DECLARE @bitUsaListaBlanca BIT = ISNULL((SELECT TOP(1) bitIngressWhiteList FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio), 0);
		
	IF @bitUsaListaBlanca = 1
	BEGIN

		DECLARE @bitValidaReservaWeb BIT = 0;
		DECLARE @bitValidarPlanYReservaWeb BIT = 0;
					
		SELECT	@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
				@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
		FROM	gim_configuracion_ingreso 
		WHERE	cdgimnasio = @intIdGimnasio
		AND		intfkSucursal = @intIdSucursal

		IF @bitValidaReservaWeb = 0 or @bitValidarPlanYReservaWeb = 1
		BEGIN

			DECLARE @strTipoPersona VARCHAR(MAX) = '';

			IF EXISTS(SELECT 1 FROM gim_clientes WHERE cdgimnasio = @intIdGimnasio AND gim_clientes.cli_identifi = @strIdPersona)
			BEGIN
				
				PRINT('LA PERSONA ES UN CLIENTE')
				SET @strTipoPersona = 'CLIENTE'

			END
			ELSE IF EXISTS(SELECT 1 FROM gim_clientes_especiales WHERE cdgimnasio = @intIdGimnasio AND gim_clientes_especiales.cli_identifi = @strIdPersona)
			BEGIN

				PRINT('LA PERSONA ES UN PROSPECTO')
				SET @strTipoPersona = 'PROSPECTO'

			END
			ELSE IF EXISTS(SELECT 1 FROM gim_empleados WHERE cdgimnasio = @intIdGimnasio AND gim_empleados.emp_identifi = @strIdPersona)
			BEGIN

				PRINT('LA PERSONA ES UN EMPLEADO')
				SET @strTipoPersona = 'EMPLEADO'

			END

			DECLARE @TablaResultado TABLE (
						bitError BIT,
						strMensaje varchar(max),
						strTipoPersona varchar(max)
					);
			DECLARE @bitErrorTabla bit = 0
			DECLARE @strMensajeTabla varchar(max) = ''

			-- SE VALIDA SI LA FACTURA QUE SE ESTA CREANDO O MODIFICANDO YA EXISTE EN LISTA BLANCA
			IF NOT EXISTS(SELECT 1 FROM WhiteList WHERE id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura)
			BEGIN
				-- SI LA FACTURA NO EXISTE EN LISTA BLANCA
				DECLARE	@return_value BIT

				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = @strTipoPersona

				IF (select count(*) from @TablaResultado) = 1
				BEGIN
					SET @bitErrorTabla = (SELECT TOP 1 bitError FROM @TablaResultado)
					SET	@strMensajeTabla  = (SELECT TOP 1 strMensaje FROM @TablaResultado)

					EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
							@idGimnasio = @intIdGimnasio,
							@intIdSucursal = @intIdSucursal,
							@strIdPersona = @strIdPersona,
							@strTrigger = 'gim_planes_usuario_especiales',
							@strTipoPersona = @strTipoPersona,
							@bitValidarReserva = 0,
							@bitValidarPlan = 1,
							@intNumeroFactura = @intNumeroFactura,
							@bitError = @bitErrorTabla,
							@strMensaje = @strMensajeTabla

					IF @return_value = 1
					BEGIN
						PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL REGISTRO A LISTA BLANCA')
					END
					ELSE
					BEGIN
						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
					END

				END
				ELSE
				BEGIN

					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

				END

			END
			ELSE 
			BEGIN

				DECLARE @bitFacturaAnulada BIT = 0;
				DECLARE @bitFacturaAvisada BIT = 0;
				DECLARE @strTipoPlan VARCHAR(MAX) = '';
				DECLARE @intTiquetesDisponibles INT = 0;
				DECLARE @dtmFechaInicial DATETIME = NULL;
				DECLARE @dtmFechaFinal DATETIME = NULL;
				DECLARE @bitRegistroEliminado BIT = 0;

				PRINT('LA CORTESIA EXISTE EN LISTA BLANCA')

				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = @strTipoPersona

				IF @strTipoPersona = 'CLIENTE'
				BEGIN

					SELECT		TOP(1)
								@bitFacturaAnulada = ISNULL(plusu_est_anulada, 0),
								@bitFacturaAvisada = ISNULL(plusu_avisado, 0),
								@strTipoPlan = ISNULL(gim_planes.pla_tipo, ''),
								@intTiquetesDisponibles = ISNULL(CASE WHEN (gim_planes.pla_tipo = 'T') THEN inserted.plusu_tiq_disponible ELSE DATEDIFF(DAY, CONVERT(VARCHAR(10),GETDATE(),111), CONVERT(VARCHAR(10),inserted.plusu_fecha_vcto,111)) END, 0),
								@dtmFechaInicial = ISNULL(plusu_fecha_inicio, NULL),
								@dtmFechaFinal = ISNULL(DATEADD(DAY, ISNULL(gim_clientes.cli_dias_gracia,0), plusu_fecha_vcto), NULL)
					FROM		inserted
					LEFT JOIN	gim_planes ON gim_planes.pla_codigo = inserted.plusu_codigo_plan
					AND			gim_planes.cdgimnasio = inserted.cdgimnasio
					INNER JOIN	gim_clientes ON gim_clientes.cli_identifi = inserted.plusu_identifi_cliente
					AND			gim_clientes.cdgimnasio = inserted.cdgimnasio

				END
				ELSE IF @strTipoPersona <> 'CLIENTE'
				BEGIN

					SELECT		TOP(1)
								@bitFacturaAnulada = ISNULL(plusu_est_anulada, 0),
								@bitFacturaAvisada = ISNULL(plusu_avisado, 0),
								@strTipoPlan = ISNULL(gim_planes.pla_tipo, ''),
								@intTiquetesDisponibles = ISNULL(CASE WHEN (gim_planes.pla_tipo = 'T') THEN inserted.plusu_tiq_disponible ELSE DATEDIFF(DAY, CONVERT(VARCHAR(10),GETDATE(),111), CONVERT(VARCHAR(10),inserted.plusu_fecha_vcto,111)) END, 0),
								@dtmFechaInicial = ISNULL(plusu_fecha_inicio, NULL),
								@dtmFechaFinal = ISNULL(plusu_fecha_vcto, NULL)
					FROM		inserted
					LEFT JOIN	gim_planes ON gim_planes.pla_codigo = inserted.plusu_codigo_plan
					AND			gim_planes.cdgimnasio = inserted.cdgimnasio

				END
				ELSE 
				BEGIN

					PRINT('LA PERSONA NO SE HA PODIDO IDENTIFICAR COMO CLIENTE O COMO PROSPECTO')
					RETURN;

				END

				IF @bitRegistroEliminado = 0
				BEGIN

					IF(@bitFacturaAnulada = 1 OR @bitFacturaAvisada = 1)
					BEGIN

						PRINT('LA CORTESIA SE ANULO O SE MARCO COMO AVISADA')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'LA CORTESIA SE ANULO O SE MARCO COMO AVISADA'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (@bitFacturaAnulada != 1 AND @bitFacturaAvisada != 1)
					BEGIN

						PRINT('LA CORTESIA SE DES-ANULO O SE DES-MARCO COMO AVISADA')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'

					END

				END

				IF @bitRegistroEliminado = 0
				BEGIN

					IF (@strTipoPlan = 'T' AND @intTiquetesDisponibles <= 0)
					BEGIN

						PRINT('EL PLAN ES TIQUETERA Y LA PERSONA YA NO CUENTA CON TIQUETES')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'EL PLAN ES TIQUETERA Y LA PERSONA YA NO CUENTA CON TIQUETES'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (@strTipoPlan = 'T' AND @intTiquetesDisponibles > 0)
					BEGIN

						PRINT('EL PLAN ES TIQUETERA Y LA PERSONA NO TENIA TIQUETES Y SE LE AGREGARON')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'
						
					END

				END

				IF @bitRegistroEliminado = 0
				BEGIN
					
					IF (CONVERT(VARCHAR(10),@dtmFechaInicial,111) < CONVERT(VARCHAR(10),GETDATE(),111) AND CONVERT(VARCHAR(10),@dtmFechaFinal,111) < CONVERT(VARCHAR(10),GETDATE(),111))
					BEGIN

						PRINT('SE CAMBIARON LAS FECHAS DE LA CORTESIA Y ESTA QUEDO VENCIDA')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'SE CAMBIARON LAS FECHAS DE LA CORTESIA Y ESTA QUEDO VENCIDA'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (CONVERT(VARCHAR(10),@dtmFechaInicial,111) > CONVERT(VARCHAR(10),GETDATE(),111) AND CONVERT(VARCHAR(10),@dtmFechaFinal,111) > CONVERT(VARCHAR(10),GETDATE(),111))
					BEGIN
					
						PRINT('LA VIGENCIA DE LA CORTESIA NO HA COMENZADO')

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								,bitError = 1
								,strMensaje = 'LA VIGENCIA DE LA CORTESIA NO HA COMENZADO'
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

						SET @bitRegistroEliminado = 1;

					END
					ELSE IF (convert(varchar(10),@dtmFechaInicial,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),@dtmFechaFinal,111) >= convert(varchar(10),getdate(),111))
					BEGIN

						PRINT('SE CAMBIARON LAS FECHAS DE LA CORTESIA Y ESTA SE VOLVIO VIGENTE')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Pendiente'

					END
					ELSE
					BEGIN

						PRINT('SE ACTUALIZAN UNICAMENTE LAS VIGENCIAS DE LA CORTESIA Y EL NUMERO DE TIQUETES')

						UPDATE	WhiteList
						SET		personState = 'Pendiente',
								expirationDate = @dtmFechaFinal,
								availableEntries = @intTiquetesDisponibles
								,bitError = @bitErrorTabla
								,strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'

					END

				END

				IF	@bitRegistroEliminado = 0
				BEGIN

				PRINT('SE CONSULTAN CONGELACION COTECIA')
							
				CREATE TABLE #tmpCortesiasCongeladas (
					invoiceId INT,
					sucursal int,
					cdgimnasio int
					)

				INSERT INTO #tmpCortesiasCongeladas (invoiceId,sucursal,cdgimnasio)
				SELECT	gim_con_fac_esp.num_fac_con,gim_con_fac_esp.con_intfkSucursal,gim_con_fac_esp.cdgimnasio
				FROM	gim_con_fac_esp
				WHERE	gim_con_fac_esp.des_con = 0
				AND		CONVERT(varchar(10), GETDATE(), 111) BETWEEN CONVERT(varchar(10), gim_con_fac_esp.fec_ini_con, 111) AND CONVERT(varchar(10), gim_con_fac_esp.fec_ter_con, 111)
				AND		gim_con_fac_esp.cdgimnasio = @intIdGimnasio
				AND		gim_con_fac_esp.con_intfkSucursal = @intIdSucursal


				UPDATE	WhiteList
				SET		personState = 'Eliminar'
						,bitError = 1
						,strMensaje = 'SE CONSULTAN CONGELACION CORTECIA'
				WHERE	id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND invoiceId = @intNumeroFactura AND personState != 'Eliminar'
				and (select  count(*) from #tmpCortesiasCongeladas where invoiceId = WhiteList.invoiceId and sucursal = WhiteList.branchId and cdgimnasio = WhiteList.gymId ) > 0

				SET @bitRegistroEliminado = 1;



				END 

			END
			
		END
			
	END

END

GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_VisitNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_VisitNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_VisitNuevo] ON [dbo].[Visit]
AFTER INSERT, UPDATE
AS
BEGIN

	IF EXISTS(SELECT 1 FROM inserted)
	BEGIN

		DECLARE @intIdGimnasio INT = 0;

		SELECT	TOP(1)
				@intIdGimnasio = ISNULL(inserted.cdgimnasio,0)
		FROM	inserted

		DECLARE @bitUsaListaBlanca BIT = 0;

		SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
		
		IF @bitUsaListaBlanca = 1
		BEGIN

			DECLARE @intIdSucursal INT = 0;
			DECLARE @strIdPersona VARCHAR(MAX) = '';
			DECLARE @dtmFechaFinal DATETIME = NULL;

			
			DECLARE @TablaResultado TABLE (
						bitError BIT,
						strMensaje varchar(max),
						strTipoPersona varchar(max)
					);
			DECLARE @bitErrorTabla bit = 0
			DECLARE @strMensajeTabla varchar(max) = ''

			SELECT	TOP(1)
					@intIdSucursal = ISNULL(Branch, 0),
					@strIdPersona = ISNULL(VisitorId, ''),
					@dtmFechaFinal = ISNULL(DateFinalizeVisit, NULL)
			FROM	inserted

			-- SE CONSULTA SI YA EXISTE UN REGISTRO EN LISTA BLANCA PARA LA VISITA
			SELECT		WhiteList.*
			INTO		#tmpWhiteList
			FROM		inserted
			INNER JOIN	WhiteList ON WhiteList.id = @strIdPersona
			AND			WhiteList.gymId = @intIdGimnasio
			AND			WhiteList.typePerson = 'Visitante'

			-- SE VALIDA SI LA FACTURA QUE SE ESTA CREANDO O MODIFICANDO YA EXISTE EN LISTA BLANCA
			IF NOT EXISTS(SELECT 1 FROM #tmpWhiteList)
			BEGIN
				-- SI LA FACTURA NO EXISTE EN LISTA BLANCA
				DECLARE	@return_value BIT
				insert into @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'VISITANTE'

				IF (select count(*) from @TablaResultado) = 1
				BEGIN

					EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
							@idGimnasio = @intIdGimnasio,
							@intIdSucursal = @intIdSucursal,
							@strIdPersona = @strIdPersona,
							@strTrigger = 'Visit',
							@strTipoPersona = 'VISITANTE',
							@bitValidarReserva = 0,
							@bitValidarPlan = 0,
							@intNumeroFactura = 0,
							@bitError = @bitErrorTabla,
							@strMensaje = @strMensajeTabla

					IF @return_value = 1
					BEGIN
						PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL REGISTRO A LISTA BLANCA')
					END
					ELSE
					BEGIN
						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
					END

				END
				ELSE
				BEGIN

					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

				END

			END
			ELSE 
			BEGIN
				
				IF @dtmFechaFinal IS NOT NULL
                BEGIN

					PRINT('LA PERSONA NO CUENTA CON UNA VISITA VIGENTE.')
                    UPDATE	WhiteList
                    SET		personState = 'Eliminar'
							,bitError = 1
							,strMensaje = 'LA PERSONA NO CUENTA CON UNA VISITA VIGENTE'
                    WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Visitante'
                
				END

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_VisitorsNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_VisitorsNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_VisitorsNuevo] ON [dbo].[Visitors]
AFTER UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0)
	FROM	inserted

	DECLARE @TablaResultado TABLE (
						bitError BIT,
						strMensaje varchar(max),
						strTipoPersona varchar(max)
					);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

	DECLARE @bitUsaListaBlanca BIT = 0;

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
		
	IF @bitUsaListaBlanca = 1
	BEGIN

		DECLARE @intIdSucursal INT = 0;
		DECLARE @strIdPersona VARCHAR(MAX) = '';
		DECLARE @bitIngresoConHuella BIT = NULL;

		SELECT	TOP(1)
				@intIdSucursal = ISNULL(vis_intBranch, 0),
				@strIdPersona = ISNULL(vis_strVisitorId, ''),
				@bitIngresoConHuella = ISNULL(vis_EntryFingerprint, 0)
		FROM	inserted

		-- SE CONSULTA SI YA EXISTE UN REGISTRO EN LISTA BLANCA PARA LA ACTUAL PERSONA
		SELECT		WhiteList.*
		INTO		#tmpWhiteList
		FROM		inserted
		INNER JOIN	WhiteList ON WhiteList.id = @strIdPersona
		AND			WhiteList.gymId = @intIdGimnasio
		AND			WhiteList.typePerson = 'Visitante'

		-- SE VALIDA SI YA EXISTE EN LISTA BLANCA
		IF EXISTS(SELECT 1 FROM #tmpWhiteList)
		BEGIN

				INSERT INTO @TablaResultado
				EXEC	[dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'VISITANTE'

				SET @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				SET	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)
				
			IF @bitIngresoConHuella = 1
            BEGIN

				SELECT	TOP(1) *
				INTO	#tmpHuellaVisitante
				FROM	gim_huellas
				WHERE	gim_huellas.hue_identifi = @strIdPersona
				AND		gim_huellas.cdgimnasio = @intIdGimnasio

				IF EXISTS(SELECT 1 FROM #tmpHuellaVisitante)
				BEGIN
					
					PRINT('SE CAMBIA EL BIT PARA QUE EL VISITANTE SOLO PUEDA INGRESAR CON HUELLA.')
					UPDATE	WhiteList
					SET		withoutFingerprint = 0,
							fingerprintId = (SELECT TOP(1) #tmpHuellaVisitante.hue_id FROM #tmpHuellaVisitante),
							fingerprint = (SELECT TOP(1) #tmpHuellaVisitante.hue_dato FROM #tmpHuellaVisitante),
							personState = 'Pendiente'
							,bitError = @bitErrorTabla
							,strMensaje = @strMensajeTabla
					WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Visitante'

				END
				ELSE 
				BEGIN

					PRINT('SE MARCA EL REGISTRO PARA ELIMINAR YA QUE LA PERSONA NO CUENTA CON HUELLA.')
					UPDATE	WhiteList
					SET		personState = 'Eliminar'
							,bitError = 1
							,strMensaje = 'SE MARCA EL REGISTRO PARA ELIMINAR YA QUE LA PERSONA NO CUENTA CON HUELLA.'
					WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Visitante'

				END

                
			END
			ELSE IF @bitIngresoConHuella = 0
			BEGIN

				PRINT('SE CAMBIA EL BIT PARA QUE EL VISITANTE NO PUEDA CON HUELLA.')					
                UPDATE	WhiteList
                SET		withoutFingerprint = 1,
						fingerprintId = 0,
						fingerprint = NULL,
						personState = 'Pendiente'
						,bitError = @bitErrorTabla
						,strMensaje = @strMensajeTabla
                WHERE	id = @strIdPersona 
				AND		gymId = @intIdGimnasio
				AND		typePerson = 'Visitante'

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_FingerprintNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_FingerprintNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_FingerprintNuevo] ON [gim_huellas]
AFTER UPDATE
AS
BEGIN

	IF EXISTS(SELECT 1 FROM inserted)
	BEGIN

		DECLARE @intIdGimnasio INT = 0;

		SELECT	TOP(1)
				@intIdGimnasio = ISNULL(inserted.cdgimnasio,0)
		FROM	inserted

		DECLARE @bitUsaListaBlanca BIT = 0;
		
		SELECT	TOP(1) 
				@bitUsaListaBlanca = ISNULL(bitIngressWhiteList,0)
		FROM	tblConfiguracion 
		WHERE	cdgimnasio = @intIdGimnasio
		
		IF @bitUsaListaBlanca = 1
		BEGIN

			DECLARE @intIdSucursal INT = 0;
			DECLARE @strIdPersona VARCHAR(MAX) = '';

			SELECT	TOP(1)
					@intIdSucursal = ISNULL(intfkSucursal, 0),
					@strIdPersona = ISNULL(hue_identifi, '')
			FROM	inserted

			-- SE CONSULTA SI YA EXISTE UN REGISTRO EN LISTA BLANCA PARA LA PERSONA
			SELECT		WhiteList.*
			INTO		#tmpWhiteList
			FROM		inserted
			INNER JOIN	WhiteList ON WhiteList.id = @strIdPersona
			AND			WhiteList.gymId = @intIdGimnasio

			DECLARE @TablaResultado TABLE (
				bitError BIT,
				strMensaje varchar(max),
				strTipoPersona varchar(max)
			);
			DECLARE @bitErrorTabla bit = 0
			DECLARE @strMensajeTabla varchar(max) = ''

			-- SE VALIDA SI LA FACTURA QUE SE ESTA CREANDO O MODIFICANDO YA EXISTE EN LISTA BLANCA
			IF EXISTS(SELECT 1 FROM #tmpWhiteList)
			BEGIN
				
				IF EXISTS(SELECT 1 FROM #tmpWhiteList WHERE #tmpWhiteList.withoutFingerprint = 0)
                BEGIN

					
				INSERT INTO @TablaResultado
				EXEC	[dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

				SET @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				SET	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

					SELECT	TOP(1) *
					INTO	#tmpHuellaPersona
					FROM	inserted
					WHERE	inserted.hue_identifi = @strIdPersona
					AND		inserted.cdgimnasio = @intIdGimnasio

					PRINT('SE ACTUALIZAN LOS DATOS DE LA HUELLA DE LA PERSONA.')
                    UPDATE	WhiteList
                    SET		withoutFingerprint = 0,
							fingerprintId = (SELECT TOP(1) #tmpHuellaPersona.hue_id FROM #tmpHuellaPersona),
							fingerprint = (SELECT TOP(1) #tmpHuellaPersona.hue_dato FROM #tmpHuellaPersona),
							personState = 'Pendiente'
                    WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio
                
				END

			END
			ELSE
			BEGIN

				-- NO EXISTE EN LISTA BLANCA
				DECLARE	@return_value BIT



				IF EXISTS(SELECT 1 FROM gim_clientes WHERE cdgimnasio = @intIdGimnasio AND gim_clientes.cli_identifi = @strIdPersona)
				BEGIN
					
				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

					IF (select count(*) from @TablaResultado) = 1
					BEGIN

						DECLARE @bitValidaReservaWeb BIT = 0;
						DECLARE @bitValidarPlanYReservaWeb BIT = 0;
						DECLARE @bitValidarReserva BIT = 0;
						DECLARE @bitValidarPlan BIT = 0;
						SET @bitErrorTabla = (select top 1 bitError from @TablaResultado)
						SET	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

						SELECT	TOP(1)
								@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
								@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
						FROM	gim_configuracion_ingreso 
						WHERE	cdgimnasio = @intIdGimnasio
						AND		intfkSucursal = @intIdSucursal

						IF @bitValidarPlanYReservaWeb = 1 AND @bitValidaReservaWeb = 0
						BEGIN
						
							SET @bitValidarReserva = 1;
							SET @bitValidarPlan = 1;

						END
						ELSE IF @bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 1
						BEGIN

							SET @bitValidarReserva = 1;
							SET @bitValidarPlan = 0; 

						END

						EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
								@idGimnasio = @intIdGimnasio,
								@intIdSucursal = @intIdSucursal,
								@strIdPersona = @strIdPersona,
								@strTrigger = 'gim_huellas',
								@strTipoPersona = 'CLIENTE',
								@bitValidarReserva = @bitValidarReserva,
								@bitValidarPlan = @bitValidarPlan,
								@intNumeroFactura = 0,
								@bitError = @bitErrorTabla,
								@strMensaje = @strMensajeTabla

						IF @return_value = 1
						BEGIN
							PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
						END
						ELSE
						BEGIN
							PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
						END

					END
					ELSE
					BEGIN

						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

					END

				END
				ELSE IF EXISTS(SELECT 1 FROM gim_clientes_especiales WHERE cdgimnasio = @intIdGimnasio AND gim_clientes_especiales.cli_identifi = @strIdPersona)
				BEGIN
				
				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'PROSPECTO'

					IF (select count(*) from @TablaResultado) = 1
					BEGIN

					set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
					set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

						EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
								@idGimnasio = @intIdGimnasio,
								@intIdSucursal = @intIdSucursal,
								@strIdPersona = @strIdPersona,
								@strTrigger = 'gim_huellas',
								@strTipoPersona = 'PROSPECTO',
								@bitValidarReserva = 0,
								@bitValidarPlan = 0,
								@intNumeroFactura = 0,
								@bitError = @bitErrorTabla,
								@strMensaje = @strMensajeTabla

						IF @return_value = 1
						BEGIN
							PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL PROSPECTO A LISTA BLANCA')
						END
						ELSE
						BEGIN
							PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
						END

					END
					ELSE
					BEGIN

						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

					END

				END
				ELSE IF EXISTS(SELECT 1 FROM Visitors WHERE cdgimnasio = @intIdGimnasio AND Visitors.vis_strVisitorId = @strIdPersona)
				BEGIN
						
						INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'VISITANTE'

	
					IF (select count(*) from @TablaResultado) = 1
					BEGIN

						EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
								@idGimnasio = @intIdGimnasio,
								@intIdSucursal = @intIdSucursal,
								@strIdPersona = @strIdPersona,
								@strTrigger = 'Visit',
								@strTipoPersona = 'VISITANTE',
								@bitValidarReserva = 0,
								@bitValidarPlan = 0,
								@intNumeroFactura = 0,
								@bitError = @bitErrorTabla,
								@strMensaje = @strMensajeTabla

						IF @return_value = 1
						BEGIN
							PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL VISITANTE A LISTA BLANCA')
						END
						ELSE
						BEGIN
							PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
						END

					END
					ELSE
					BEGIN

						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

					END

				END
				ELSE IF EXISTS(SELECT 1 FROM gim_empleados WHERE cdgimnasio = @intIdGimnasio AND gim_empleados.emp_identifi = @strIdPersona)
				BEGIN

				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'EMPLEADO'

					IF (select count(*) from @TablaResultado) = 1
					BEGIN

						EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
								@idGimnasio = @intIdGimnasio,
								@intIdSucursal = @intIdSucursal,
								@strIdPersona = @strIdPersona,
								@strTrigger = 'gim_huellas',
								@strTipoPersona = 'EMPLEADO',
								@bitValidarReserva = 0,
								@bitValidarPlan = 0,
								@intNumeroFactura = 0,
								@bitError = @bitErrorTabla,
								@strMensaje = @strMensajeTabla

						IF @return_value = 1
						BEGIN
							PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL EMPLEADO A LISTA BLANCA')
						END
						ELSE
						BEGIN
							PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
						END

					END
					ELSE
					BEGIN

						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

					END

				END

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_specialClientsNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_specialClientsNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_specialClientsNuevo] ON [gim_clientes_especiales]
AFTER INSERT, UPDATE
AS
BEGIN

	PRINT('TRIGGER DISPARADO - gim_clientes_especiales')

	DECLARE @intIdGimnasio INT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0)
	FROM	inserted

	DECLARE @bitUsaListaBlanca BIT = 0;

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		PRINT('EL GIMNASIO USA LISTA BLANCA');

		DECLARE @intIdSucursal INT = 0;
		DECLARE @strIdPersona VARCHAR(MAX) = '';
		DECLARE @bitIngresoConHuella BIT = NULL;
		DECLARE @bitConocerGimnasio BIT = NULL;
		DECLARE @bitCortesia BIT = NULL;

		DECLARE @TablaResultado TABLE (
						bitError BIT,
						strMensaje varchar(max),
						strTipoPersona varchar(max)
					);
		DECLARE @bitErrorTabla bit = 0
		DECLARE @strMensajeTabla varchar(max) = ''

		SELECT	TOP(1)
				@intIdSucursal = ISNULL(cli_intfkSucursal, 0),
				@strIdPersona = ISNULL(cli_identifi, ''),
				@bitIngresoConHuella = ISNULL(cli_EntryFingerprint, 0),
				@bitConocerGimnasio = ISNULL(cli_entrar_conocer, 0),
				@bitCortesia = ISNULL(cli_cortesia, 0)
		FROM	inserted

		-- SE CONSULTA SI YA EXISTE UN REGISTRO EN LISTA BLANCA PARA LA ACTUAL PERSONA
		SELECT		WhiteList.*
		INTO		#tmpWhiteList
		FROM		inserted
		INNER JOIN	WhiteList ON WhiteList.id = @strIdPersona
		AND			WhiteList.gymId = @intIdGimnasio
		AND			WhiteList.typePerson = 'Prospecto'

		-- SE VALIDA SI YA EXISTE EN LISTA BLANCA
		IF EXISTS(SELECT 1 FROM #tmpWhiteList)
		BEGIN
				insert into @TablaResultado
				EXEC	[dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'PROSPECTO'

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

			IF @bitIngresoConHuella = 1
            BEGIN

				SELECT	TOP(1) *
				INTO	#tmpHuellaVisitante
				FROM	gim_huellas
				WHERE	gim_huellas.hue_identifi = @strIdPersona
				AND		gim_huellas.cdgimnasio = @intIdGimnasio

				IF EXISTS(SELECT 1 FROM #tmpHuellaVisitante)
				BEGIN

					PRINT('SE CAMBIA EL BIT PARA QUE EL PROSPECTO SOLO PUEDA INGRESAR CON HUELLA.')
					UPDATE	WhiteList
					SET		withoutFingerprint = 0,
							fingerprintId = (SELECT TOP(1) #tmpHuellaVisitante.hue_id FROM #tmpHuellaVisitante),
							fingerprint = (SELECT TOP(1) #tmpHuellaVisitante.hue_dato FROM #tmpHuellaVisitante),
							personState = 'Pendiente'
							, bitError = @bitErrorTabla
							, strMensaje = @strMensajeTabla
					WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Prospecto'

				END
				ELSE
				BEGIN
						
					PRINT('SE CAMBIAN LOS ESTADOS DE LOS REGISTROS PARA QUE SEAN ELIMINADOS POR QUE NO TIENE HUELLA.')					
					UPDATE	WhiteList
					SET		personState = 'Eliminar'
							, bitError = @bitErrorTabla
							, strMensaje = @strMensajeTabla
					WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Prospecto'

				END

			END
			ELSE IF @bitIngresoConHuella = 0
			BEGIN

				PRINT('SE CAMBIA EL BIT PARA QUE EL PROSPECTO PUEDA INGRESAR SIN HUELLA.')					
                UPDATE	WhiteList
                SET		withoutFingerprint = 1,
						fingerprintId = 0,
						fingerprint = NULL,
						personState = 'Pendiente'
						, bitError = @bitErrorTabla
						, strMensaje = @strMensajeTabla
                WHERE	id = @strIdPersona 
				AND		gymId = @intIdGimnasio
				AND		typePerson = 'Prospecto'

			END

		END
		ELSE IF(@bitConocerGimnasio = 1 OR @bitCortesia = 1)
		BEGIN
			
			PRINT('SE PROCEDE A REALIZAR LAS VALIDACIONES DEL REGISTRO EN LISTA BLANCA')

			DECLARE	@return_value BIT

			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'PROSPECTO'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_planes_usuario_especiales',
						@strTipoPersona = 'PROSPECTO',
						@bitValidarReserva = 0,
						@bitValidarPlan = 0,
						@intNumeroFactura = 0,
						@bitError = @bitErrorTabla,
						@strMensaje = @strMensajeTabla

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL PROSPECTO A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_clients_Nuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_clients_Nuevo
END
GO
CREATE   TRIGGER [dbo].[trgWhiteList_clients_Nuevo] ON [dbo].[gim_clientes]
AFTER UPDATE, INSERT
AS
BEGIN

	IF EXISTS(SELECT 1 FROM inserted)
	BEGIN

		DECLARE @intIdGimnasio INT = 0;
		DECLARE @intIdSucursal INT = 0;
		DECLARE @strIdPersona VARCHAR(MAX) = '';
		DECLARE	@bitEstadoPersona BIT = 0;
		DECLARE @bitIngresoConHuella BIT = 0;
		DECLARE @bitGrupoFamiliar BIT = 0;
		DECLARE @bitAltoRiesgoPersona BIT = 0;
		DECLARE @dtmFechaNacimiento DATETIME = NULL;
		DECLARE @bitAutorizacionMenorEdad BIT = 0;
		DECLARE @bitConsentimientoInformado BIT = 0;	
		DECLARE @strIdTarjeta VARCHAR(20) = 0;
		DECLARE @intDiasGracia INT = 0;

		SELECT	TOP(1)
				@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
				@intIdSucursal = ISNULL(inserted.cli_sucursal, 0),
				@strIdPersona = ISNULL(cli_identifi, ''),
				@bitEstadoPersona = ISNULL(cli_estado, ''),
				@bitIngresoConHuella = ISNULL(cli_sin_huella, 1),
				@bitGrupoFamiliar = ISNULL(cli_GrupoFamiliar, 0),
				@bitAltoRiesgoPersona = ISNULL(cli_Apto, 0),
				@dtmFechaNacimiento = ISNULL(cli_fecha_nacimien, NULL),
				@bitAutorizacionMenorEdad = ISNULL(cli_bitAutorizacionM, 0),
				@bitConsentimientoInformado = CASE WHEN cli_disentimientoVirtual IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END,
				@strIdTarjeta = ISNULL(cli_strcodtarjeta, ''),
				@intDiasGracia = ISNULL(cli_dias_gracia, 0)
		FROM	inserted

		DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
		DECLARE @bitErrorTabla bit = 0
		DECLARE @strMensajeTabla varchar(max) = ''

		DECLARE @bitUsaListaBlanca BIT = 0;

		SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
		
		IF @bitUsaListaBlanca = 1
		BEGIN

			-- SE CONSULTA SI YA EXISTE UN REGISTRO EN LISTA BLANCA PARA LA ACTUAL PERSONA
			IF EXISTS(SELECT 1 FROM	inserted INNER JOIN	WhiteList ON WhiteList.id = @strIdPersona AND WhiteList.gymId = @intIdGimnasio AND WhiteList.typePerson = 'Cliente')
			BEGIN
			
				PRINT('EL CLIENTE TIENE YA REGISTROS EN LISTA BLANCA')

				--SECCION: ESTADO DEL CLIENTE
				DECLARE @bitRegistrosEliminados BIT = 0;
				DECLARE @bitValidarListaBlanca BIT = 0;

				DECLARE	@bitEstadoPersonaOld BIT = 0;
				DECLARE @bitIngresoConHuellaOld BIT = 0;
				DECLARE @bitGrupoFamiliarOld BIT = 0;
				DECLARE @bitAltoRiesgoPersonaOld BIT = 0;
				DECLARE @dtmFechaNacimientoOld DATETIME = NULL;
				DECLARE @bitAutorizacionMenorEdadOld BIT = 0;
				DECLARE @bitConsentimientoInformadoOld BIT = 0;
				DECLARE @strIdTarjetaOld VARCHAR(20) = 0;
				DECLARE @intDiasGraciaOld INT = 0;


				SELECT	TOP(1)
						@bitEstadoPersonaOld = ISNULL(cli_estado, ''),
						@bitIngresoConHuellaOld = ISNULL(cli_sin_huella, 1),
						@bitGrupoFamiliarOld = ISNULL(cli_GrupoFamiliar, 0),
						@bitAltoRiesgoPersonaOld = ISNULL(cli_Apto, 0),
						@dtmFechaNacimientoOld = ISNULL(cli_fecha_nacimien, NULL),
						@bitAutorizacionMenorEdadOld = ISNULL(cli_bitAutorizacionM, 0),
						@bitConsentimientoInformadoOld = CASE WHEN cli_disentimientoVirtual IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END,
						@strIdTarjetaOld = ISNULL(cli_strcodtarjeta, ''),
						@intDiasGraciaOld = ISNULL(cli_dias_gracia, 0)
				FROM	deleted

				INSERT INTO @TablaResultado
				EXEC	[dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				--SECCION CODIGO TARJETA
				IF @strIdTarjeta <> @strIdTarjetaOld
				BEGIN

					UPDATE	WhiteList
                    SET		cardId = @strIdTarjeta,
							personState = 'Pendiente'
							, bitError = @bitErrorTabla
							, strMensaje = @strMensajeTabla
                    WHERE	id = @strIdPersona
					AND		gymId = @intIdGimnasio
					and		typePerson = 'Cliente'
                
				END

				--SECCION GRUPO FAMILIAR
				IF @bitGrupoFamiliar <> @bitGrupoFamiliarOld
				BEGIN

					IF	@bitGrupoFamiliar = 1
					BEGIN
					
						UPDATE		WhiteList
						SET			groupId = gf.gim_gf_IDgrupo,
									groupEntriesControl = ISNULL(gm.gim_gf_bitControlIngreso, 0),
									groupEntriesQuantity = ISNULL(gm.gim_gf_intNumlIngresos, 0),
									personState = 'Pendiente'
									, bitError = @bitErrorTabla
									, strMensaje = @strMensajeTabla
						FROM		WhiteList AS wl
						INNER JOIN	gim_grupoFamiliar AS gf ON gf.gim_gf_IDCliente = wl.id
						INNER JOIN	gim_grupoFamiliar_Maestro AS gm ON gm.gim_gf_pk_IDgrupo = gf.gim_gf_IDgrupo
						WHERE		wl.id = @strIdPersona
						AND			wl.gymId = @intIdGimnasio
						AND			wl.typePerson = 'Cliente'
						AND			gf.cdgimnasio = @intIdGimnasio
						AND			gf.gim_gf_estado = 1
						AND			gm.cdgimnasio = @intIdGimnasio;

					END
					ELSE IF @bitGrupoFamiliar = 0
					BEGIN
					
						UPDATE		WhiteList
						SET			groupId = 0,
									groupEntriesControl = 0,
									groupEntriesQuantity = 0,
									personState = 'Pendiente'
									, bitError = @bitErrorTabla
									, strMensaje = @strMensajeTabla
						WHERE		WhiteList.id = @strIdPersona
						AND			WhiteList.gymId = @intIdGimnasio
						AND			WhiteList.typePerson = 'Cliente'

					END

				END

				--SECCION ESTADO PERSONA - VALIDACION PRIMORDIAL LISTA BLANCA
				IF @bitEstadoPersonaOld <> @bitEstadoPersona AND @bitRegistrosEliminados = 0
				BEGIN

					-- SE HACE LA VALIDACION POR EL BIT @bitRegistrosEliminados, YA QUE SE PUDO BORRAR EL REGISTRO ANTERIORMENTE
					-- Y ASI SE PUEDE EVITAR EL REPROCESO DE CAMBIAR EL ESTADO A ELIMINAR A REGISTROS QUE ESTAN PARA ELIMINAR
					IF @bitEstadoPersona = 0
					BEGIN
					
						PRINT('EL CLIENTE SE INACTIVO, TODOS LOS REGISTROS DE LISTA BLANCA SE ELIMINAN')
						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						SET @bitRegistrosEliminados = 1
						
					END
					ELSE IF @bitEstadoPersona = 1
					BEGIN
						
						UPDATE	WhiteList
						SET		personState = 'Pendiente'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona 
						AND		gymId = @intIdGimnasio 
						AND		typePerson = 'Cliente'

						-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE ESTA INACTIVO
						-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
						-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
						SET @bitValidarListaBlanca = 1;

					END

				END

				--SECCION: HUELLA DEL CLIENTE - VALIDACION PRIMORDIAL LISTA BLANCA
				IF @bitIngresoConHuella <> @bitIngresoConHuellaOld AND @bitRegistrosEliminados = 0
				BEGIN

				    IF @bitIngresoConHuella = 1
					BEGIN

						PRINT('SE CAMBIA EL BIT PARA QUE EL CLIENTE PUEDA INGRESAR SIN HUELLA.')
						UPDATE	WhiteList
						SET		withoutFingerprint = 1,
								fingerprintId = 0,
								fingerprint = NULL,
								personState = 'Pendiente'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona 
						AND		gymId = @intIdGimnasio 
						AND		typePerson = 'Cliente'

						-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE TIENE INGRESO CON HUELLA Y NO TIENE HUELLA
						-- PUEDE QUE LA PERSONA ANTES NO TUVIERA HUELLA Y NO SE INSERTO A LISTA BLANCA, AHORA PODRIA INGRESAR
						-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
						-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
						SET @bitValidarListaBlanca = 1;
                
					END
					ELSE IF @bitIngresoConHuella = 0
					BEGIN

						IF EXISTS(SELECT 1 FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio)
						BEGIN

							PRINT('SE CAMBIA EL BIT PARA QUE EL CLIENTE SOLO PUEDA INGRESAR CON HUELLA.')
							UPDATE	WhiteList
							SET		withoutFingerprint = 0,
									fingerprintId = case when (select count(*) from gim_huellas  WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio ) >2 then null else (SELECT TOP(1) gim_huellas.hue_id FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio) end ,
									fingerprint = case when (select count(*) from gim_huellas  WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio ) >2 then null else (SELECT TOP(1) gim_huellas.hue_dato FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio) end ,
									personState = 'Pendiente'
									, bitError = @bitErrorTabla
								    , strMensaje = @strMensajeTabla
							WHERE	id = @strIdPersona 
							AND		gymId = @intIdGimnasio
							AND		typePerson = 'Cliente'

							-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE TIENE INGRESO CON HUELLA Y NO TIENE HUELLA
							-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
							-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
							SET @bitValidarListaBlanca = 1;

						END
						ELSE
						BEGIN

							IF(@strIdTarjeta = '')
							BEGIN

								PRINT('SE CAMBIAN LOS ESTADOS DE LOS REGISTROS PARA QUE SEAN ELIMINADOS POR QUE NO TIENE HUELLA.')					
								UPDATE	WhiteList
								SET		personState = 'Eliminar'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
								WHERE	id = @strIdPersona
								AND		gymId = @intIdGimnasio
								AND		typePerson = 'Cliente'

								SET @bitRegistrosEliminados = 1

							END


						END


					END

				END

				--SECCION CLIENTE ALTO RIESGO - VALIDACION PRIMORDIAL LISTA BLANCA
				DECLARE @bitBloqueoClienteNoApto BIT = 0;
				DECLARE @bitBloqueoNoAutorizacionMenor BIT = 0;
				DECLARE @bitBloqueoNoDisentimento BIT = 0;

				SELECT	@bitBloqueoClienteNoApto = ISNULL(bitBloqueoClienteNoApto, 0),
						@bitBloqueoNoAutorizacionMenor = ISNULL(bitBloqueoNoAutorizacionMenor,0),
						@bitBloqueoNoDisentimento = ISNULL(bitBloqueoNoDisentimento,0) 
				FROM	gim_configuracion_ingreso
				WHERE	cdgimnasio = @intIdGimnasio
				AND		intfkSucursal = @intIdSucursal

				IF @bitAltoRiesgoPersona <> @bitAltoRiesgoPersonaOld AND @bitBloqueoClienteNoApto = 1 AND @bitRegistrosEliminados = 0
				BEGIN

					-- SE HACE LA VALIDACION POR EL BIT @bitRegistrosEliminados, YA QUE SE PUDO BORRAR EL REGISTRO ANTERIORMENTE
					-- Y ASI SE PUEDE EVITAR EL REPROCESO DE CAMBIAR EL ESTADO A ELIMINAR A REGISTROS QUE ESTAN PARA ELIMINAR
					IF @bitAltoRiesgoPersona = 0
					BEGIN

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						SET @bitRegistrosEliminados = 1

					END
					ELSE IF @bitAltoRiesgoPersona = 1
					BEGIN

						UPDATE	WhiteList
						SET		personState = 'Pendiente'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona 
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE ES DE ALTO RIESGO Y ESTA ACTIVO EL BLOQUEO DE ALTO RIESGO
						-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
						-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
						SET @bitValidarListaBlanca = 1;

					END

				END

                --SECCION CLIENTE MENOR DE EDAD - VALIDACION PRIMORDIAL LISTA BLANCA
				IF @bitAutorizacionMenorEdad <> @bitAutorizacionMenorEdadOld AND @bitBloqueoNoAutorizacionMenor = 1 AND @bitRegistrosEliminados = 0
				BEGIN

					DECLARE @intMayoriaEdad INT = 0;

					SET @intMayoriaEdad = ISNULL((SELECT ISNULL(intAnosMayoriaEdad,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio),0)

					IF @bitAutorizacionMenorEdad = 0 AND @dtmFechaNacimiento > DATEADD(YEAR, -@intMayoriaEdad, GETDATE())
					BEGIN

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						SET	@bitRegistrosEliminados = 1

					END
					ELSE
					BEGIN

						UPDATE	WhiteList
						SET		personState = 'Pendiente'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE NO TIENE AUTORIZACION POR SER MENOR DE EDAD
						-- SI SE LA ACTIVABAN PUEDE INGRESAR A LISTA BLANCA
						-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
						-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
						SET @bitValidarListaBlanca = 1;

					END

				END

                --SECCION CONSENTIMIENTO INFORMADO - VALIDACION PRIMORDIAL LISTA BLANCA
				IF @bitConsentimientoInformado <> @bitConsentimientoInformadoOld AND @bitRegistrosEliminados = 0 AND @bitBloqueoNoDisentimento = 1
				BEGIN

					IF @bitConsentimientoInformado = 0
					BEGIN

						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						SET @bitRegistrosEliminados = 1

					END
					ELSE IF @bitConsentimientoInformado = 1 
					BEGIN	

						UPDATE	WhiteList
						SET		personState = 'Pendiente'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'

						-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE NO TIENE AUTORIZACION POR SER MENOR DE EDAD
						-- SI SE LA ACTIVABAN PUEDE INGRESAR A LISTA BLANCA
						-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
						-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
						SET @bitValidarListaBlanca = 1;
                
					END

				END

				--SECCION DIAS DE GRACIA
				IF @intDiasGracia <> @intDiasGraciaOld AND @intDiasGracia <= 0
				BEGIN

						UPDATE	WhiteList
						SET		personState = 'Eliminar',
								expirationDate = CONVERT(VARCHAR(10), DATEADD(DAY, -@intDiasGracia ,expirationDate), 111)
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Cliente'
						AND		CONVERT(VARCHAR(10), DATEADD(DAY, -@intDiasGracia ,expirationDate), 111) < CONVERT(VARCHAR(10), GETDATE(), 111)

				END

				IF @bitRegistrosEliminados = 1 OR (@bitRegistrosEliminados = 0 AND @bitValidarListaBlanca = 0)
				BEGIN

					RETURN;

				END

			END

			DECLARE @return_value INT = 0;

			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'CLIENTE'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_clientes',
						@strTipoPersona = 'CLIENTE',
						@bitValidarReserva = 1,
						@bitValidarPlan = 1,
						@intNumeroFactura = 0,
						@bitError = @bitErrorTabla,
						@strMensaje = @strMensajeTabla

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_employee_Nuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_employee_Nuevo
END
GO
CREATE   TRIGGER [dbo].[trgWhiteList_employee_Nuevo] ON [dbo].[gim_empleados]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @intIdSucursal INT = 0;
	DECLARE @strIdPersona VARCHAR(MAX) = '';
	DECLARE @bitEstadoEmpleado BIT = 0;
	DECLARE @bitIngresoConHuella BIT = 0;
	DECLARE @strIdTarjeta VARCHAR(MAX) = '';

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
			@intIdSucursal = ISNULL(inserted.emp_sucursal, 0),
			@strIdPersona = ISNULL(emp_identifi, ''),
			@bitEstadoEmpleado = ISNULL(emp_estado, 0),
			@bitIngresoConHuella = ISNULL(emp_sin_huella, 0),
			@strIdTarjeta = ISNULL(emp_strcodtarjeta, '')
	FROM	inserted

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

	DECLARE @bitUsaListaBlanca BIT = 0;

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
		
	IF @bitUsaListaBlanca = 1
	BEGIN

		IF EXISTS(SELECT 1 FROM WhiteList WHERE id = @strIdPersona AND gymId = @intIdGimnasio AND typePerson = 'Empleado')
		BEGIN

			DECLARE @bitRegistrosEliminados BIT = 0;
			DECLARE @bitValidarListaBlanca BIT = 0;

			DECLARE @bitEstadoEmpleadoOld BIT = 0;
			DECLARE @bitIngresoConHuellaOld BIT = 0;
			DECLARE @strIdTarjetaOld VARCHAR(MAX) = '';

			SELECT	@bitEstadoEmpleadoOld = ISNULL(emp_estado, 0),
					@bitIngresoConHuellaOld = ISNULL(emp_sin_huella, 0),
					@strIdTarjetaOld = ISNULL(emp_strcodtarjeta, '')
			FROM	deleted

			INSERT INTO @TablaResultado
			EXEC [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @intIdSucursal,
				@strIdPersona = @strIdPersona,
				@strTipoPersona = 'EMPLEADO'

			
			IF @strIdTarjeta <> @strIdTarjetaOld
			BEGIN

				PRINT('SE CAMBIA EL ESTADO DE LA TARJETA')
				UPDATE	WhiteList
                SET		cardId = @strIdTarjeta,
						personState = 'Pendiente'
						, bitError = @bitErrorTabla
						, strMensaje = @strMensajeTabla
                WHERE	id = @strIdPersona
				AND		gymId = @intIdGimnasio
				and		typePerson = 'Cliente'
                
			END

			--SECCION ESTADO DEL EMPLEADO
			IF @bitEstadoEmpleado <> @bitEstadoEmpleadoOld
			BEGIN

				IF @bitEstadoEmpleado = 0
                BEGIN

					PRINT('SE CAMBIA EL ESTADO DEL EMPLEADO A INACTIVO')
					UPDATE	WhiteList
                    SET		personState = 'Eliminar'
							, bitError = @bitErrorTabla
						, strMensaje = @strMensajeTabla
                    WHERE	id = @strIdPersona
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Empleado'

                    SET @bitRegistrosEliminados = 1

                END
                ELSE IF @bitEstadoEmpleado = 1
                BEGIN

					PRINT('SE CAMBIA EL ESTADO DEL EMPLEADO A ACTIVO')
                    UPDATE	WhiteList
                    SET		personState = 'Pendiente'
							, bitError = @bitErrorTabla
							, strMensaje = @strMensajeTabla
                    WHERE	id = @strIdPersona
					AND		gymId = @intIdGimnasio
					AND		typePerson = 'Empleado'

					-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL EMPLEADO ESTA INACTIVO
					-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
					-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
					SET @bitValidarListaBlanca = 1;

                END

			END
              
			-- SECCCION INGRESO CON HUELLA
			IF @bitIngresoConHuella <> @bitIngresoConHuellaOld AND @bitRegistrosEliminados = 0
			BEGIN

				IF @bitIngresoConHuella = 1
				BEGIN

					PRINT('SE CAMBIA EL BIT PARA QUE EL EMPLEADO PUEDA INGRESAR SIN HUELLA.')
					UPDATE	WhiteList
					SET		withoutFingerprint = 1,
							fingerprintId = 0,
							fingerprint = NULL,
							personState = 'Pendiente'
							, bitError = @bitErrorTabla
							, strMensaje = @strMensajeTabla
					WHERE	id = @strIdPersona 
					AND		gymId = @intIdGimnasio 
					AND		typePerson = 'Empleado'

					-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE TIENE INGRESO CON HUELLA Y NO TIENE HUELLA
					-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
					-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
					SET @bitValidarListaBlanca = 1;
                
				END
				ELSE IF @bitIngresoConHuella = 0
				BEGIN

					IF EXISTS(SELECT 1 FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio)
					BEGIN

						PRINT('SE CAMBIA EL BIT PARA QUE EL EMPLEADO SOLO PUEDA INGRESAR CON HUELLA.')
						UPDATE	WhiteList
						SET		withoutFingerprint = 0,
								fingerprintId = CASE WHEN (select count(*) from gim_huellas  WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio ) >2 THEN NULL ELSE(SELECT TOP(1) gim_huellas.hue_id FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio)END,
								fingerprint =	CASE WHEN (select count(*) from gim_huellas  WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio ) >2 THEN NULL ELSE (SELECT TOP(1) gim_huellas.hue_dato FROM gim_huellas WHERE gim_huellas.hue_identifi = @strIdPersona AND gim_huellas.cdgimnasio = @intIdGimnasio)END ,
								personState = 'Pendiente'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona 
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Empleado'

						-- COMO LOS REGISTROS AHORA NO SE CREAN SI EL CLIENTE TIENE INGRESO CON HUELLA Y NO TIENE HUELLA
						-- SE MARCA EL CHECK PARA QUE AL FINALIZAR TODAS LAS VALIDACIONES
						-- SE VALIDE SI LA PERSONA PUEDE ENTRAR A LISTA BLANCA O NO
						SET @bitValidarListaBlanca = 1;

					END
					ELSE
					BEGIN

						PRINT('SE CAMBIA EL ESTADO DE LOS REGISTROS A ELIMINAR POR QUE EL EMPLEADO NO TIENE HUELLA')
						UPDATE	WhiteList
						SET		personState = 'Eliminar'
								, bitError = @bitErrorTabla
								, strMensaje = @strMensajeTabla
						WHERE	id = @strIdPersona
						AND		gymId = @intIdGimnasio
						AND		typePerson = 'Empleado'

						SET @bitRegistrosEliminados = 1

					END

				END

				IF @bitRegistrosEliminados = 1 OR (@bitRegistrosEliminados = 0 AND @bitValidarListaBlanca = 0)
				BEGIN

					RETURN;
					
				END

			END

		END

		DECLARE @return_value INT = 0;

		INSERT INTO @TablaResultado
		EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @intIdSucursal,
				@strIdPersona = @strIdPersona,
				@strTipoPersona = 'EMPLEADO'

		IF (select count(*) from @TablaResultado) = 1
		BEGIN
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

			EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTrigger = 'gim_empleados',
					@strTipoPersona = 'EMPLEADO',
					@bitValidarReserva = 1,
					@bitValidarPlan = 1,
					@intNumeroFactura = 0
					,@bitError = @bitErrorTabla
					,@strMensaje = @strMensajeTabla

			IF @return_value = 1
			BEGIN
				PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
			END
			ELSE
			BEGIN
				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
			END

		END
		ELSE
		BEGIN

			PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_plans' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_plans
END
GO

CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_plans] ON [gim_planes]
AFTER UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @strCodigoPlan VARCHAR(MAX) = '';
	DECLARE @bitEstadoPlan BIT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
			@strCodigoPlan = ISNULL(inserted.pla_codigo, 0),
			@bitEstadoPlan = CASE WHEN inserted.pla_desactivado = 1 THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END
	FROM	inserted

	DECLARE @bitUsaListaBlanca BIT = 0;

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
		
	IF UPDATE(pla_adicionales)

	IF @bitUsaListaBlanca = 1 AND @bitEstadoPlan = 1
	BEGIN

		UPDATE	WhiteList
		SET		restrictions = (SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||') FROM [dbo].[fnObtenerRestriccionesWL] (@intIdGimnasio, subgroupId, planId)),
				personState = 'Pendiente'
		WHERE	WhiteList.planId = @strCodigoPlan

	END

END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_BlackListNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_BlackListNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_BlackListNuevo] ON [gim_listanegra]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @strTipoPersona varchar(max) = '';
	DECLARE @strIdPersona VARCHAR(MAX) = '';
	DECLARE @intIdSucursal INT = 0;
	DECLARE @bitEstado BIT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
			@strIdPersona = ISNULL(dbo.fFloatAVarchar(inserted.listneg_floatId), 0),
			@bitEstado = ISNULL(listneg_bitEstado, 0)
	FROM	inserted

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
	

	IF @bitUsaListaBlanca = 1
	BEGIN
	   
		

		IF EXISTS(SELECT 1 FROM WhiteList WHERE id = @strIdPersona AND gymId = @intIdGimnasio)
		BEGIN
		
			DECLARE @bitValidarListaBlanca BIT = 0;
			SET @strTipoPersona =  (SELECT DISTINCT UPPER(typePerson) FROM WhiteList WHERE id = @strIdPersona AND gymId = @intIdGimnasio)

			INSERT INTO @TablaResultado
			EXEC [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @intIdSucursal,
				@strIdPersona = @strIdPersona,
				@strTipoPersona = @strTipoPersona

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

			IF @bitEstado = 1
            BEGIN

                UPDATE	WhiteList
                SET		personState = 'Eliminar'
						, bitError = @bitErrorTabla
						, strMensaje = @strMensajeTabla
                WHERE	id = @strIdPersona
				AND		gymId = @intIdGimnasio

            END
            ELSE IF @bitEstado = 0
            BEGIN

                UPDATE	WhiteList
                SET		personState = 'Pendiente'
						, bitError = @bitErrorTabla
						, strMensaje = @strMensajeTabla
                WHERE	id = @strIdPersona
				AND		gymId = @intIdGimnasio

				SET @bitValidarListaBlanca = 1
            
			END

			IF @bitValidarListaBlanca = 0
			BEGIN

				RETURN;
			
			END

		END

		DECLARE @return_value INT = 0;
		
		IF EXISTS(SELECT 1 FROM gim_clientes WHERE cdgimnasio = @intIdGimnasio AND gim_clientes.cli_identifi = @strIdPersona)
		BEGIN
					
			DECLARE @bitValidaReservaWeb BIT = 0;
			DECLARE @bitValidarPlanYReservaWeb BIT = 0;
			DECLARE @bitValidarReserva BIT = 0;
			DECLARE @bitValidarPlan BIT = 0;
			
			SET @intIdSucursal = (SELECT TOP(1) ISNULL(cli_sucursal, 0) FROM gim_clientes WHERE cdgimnasio = @intIdGimnasio AND gim_clientes.cli_identifi = @strIdPersona)

			SELECT	TOP(1)
					@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
					@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
			FROM	gim_configuracion_ingreso 
			WHERE	cdgimnasio = @intIdGimnasio
			AND		intfkSucursal = @intIdSucursal

			IF @bitValidarPlanYReservaWeb = 1 AND @bitValidaReservaWeb = 0
			BEGIN
						
				SET @bitValidarReserva = 1;
				SET @bitValidarPlan = 1;

			END
			ELSE IF @bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 1
			BEGIN

				SET @bitValidarReserva = 1;
				SET @bitValidarPlan = 0; 

			END
			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'CLIENTE'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN
							
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_listanegra',
						@strTipoPersona = 'CLIENTE',
						@bitValidarReserva = @bitValidarReserva,
						@bitValidarPlan = @bitValidarPlan,
						@intNumeroFactura = 0,
						@bitError = @bitErrorTabla,
						@strMensaje = @strMensajeTabla

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END
		ELSE IF EXISTS(SELECT 1 FROM gim_clientes_especiales WHERE cdgimnasio = @intIdGimnasio AND gim_clientes_especiales.cli_identifi = @strIdPersona)
		BEGIN

			SET @intIdSucursal = (SELECT ISNULL(cli_intfkSucursal, 0) FROM gim_clientes_especiales WHERE cdgimnasio = @intIdGimnasio AND gim_clientes_especiales.cli_identifi = @strIdPersona)

			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'PROSPECTO'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_listanegra',
						@strTipoPersona = 'PROSPECTO',
						@bitValidarReserva = 0,
						@bitValidarPlan = 0,
						@intNumeroFactura = 0,
						@bitError = @bitErrorTabla,
						@strMensaje = @strMensajeTabla

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL PROSPECTO A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END
		ELSE IF EXISTS(SELECT 1 FROM Visitors WHERE cdgimnasio = @intIdGimnasio AND Visitors.vis_strVisitorId = @strIdPersona)
		BEGIN

			SET @intIdSucursal = (SELECT ISNULL(vis_intBranch, 0) FROM Visitors WHERE cdgimnasio = @intIdGimnasio AND Visitors.vis_strVisitorId = @strIdPersona)
			
			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'VISITANTE'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN
			
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_listanegra',
						@strTipoPersona = 'VISITANTE',
						@bitValidarReserva = 0,
						@bitValidarPlan = 0,
						@intNumeroFactura = 0
						,@bitError = @bitErrorTabla
						,@strMensaje = @strMensajeTabla

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL VISITANTE A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END
		ELSE IF EXISTS(SELECT 1 FROM gim_empleados WHERE cdgimnasio = @intIdGimnasio AND gim_empleados.emp_identifi = @strIdPersona)
		BEGIN

			SET @intIdSucursal = (SELECT ISNULL(emp_sucursal, 0) FROM gim_empleados WHERE cdgimnasio = @intIdGimnasio AND gim_empleados.emp_identifi = @strIdPersona)

			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'EMPLEADO'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_listanegra',
						@strTipoPersona = 'EMPLEADO',
						@bitValidarReserva = 0,
						@bitValidarPlan = 0,
						@intNumeroFactura = 0
						,@bitError = @bitErrorTabla
						,@strMensaje = @strMensajeTabla

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL EMPLEADO A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_suspendedInvoicesNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_suspendedInvoicesNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_suspendedInvoicesNuevo] ON [gim_con_fac]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @intNumeroFactura VARCHAR(MAX) = '';
	DECLARE @bitFacturaCongelada BIT = 0;
	DECLARE @intIdDian INT = 0;
	DECLARE @dtmFechaInicial DATETIME = NULL;
	DECLARE @dtmFechaFinal DATETIME = NULL;
	declare @con_sucursal int = 0;
	declare @id VARCHAR(MAX) = 0;
	
	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
			@intNumeroFactura = ISNULL(num_fac_con, 0),
			@bitFacturaCongelada = ISNULL(des_con, 0),
			@intIdDian = ISNULL(con_fkdia_codigo, 0),
			@dtmFechaInicial = fec_ini_con,
			@dtmFechaFinal = fec_ter_con
			,@con_sucursal = con_sucursal
	FROM	inserted

		DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		IF @bitFacturaCongelada = 0
		BEGIN

			

			PRINT('SE ESTA CONGELANDO LA FACTURA')
			IF EXISTS(SELECT * FROM WhiteList WHERE gymId = @intIdGimnasio and invoiceId = @intNumeroFactura and dianId = @intIdDian and documentType = 'Factura')
			BEGIN

			SET @id = (SELECT DISTINCT id FROM WhiteList WHERE gymId = @intIdGimnasio and invoiceId = @intNumeroFactura and dianId = @intIdDian and documentType = 'Factura')
			
			INSERT INTO @TablaResultado
			EXEC [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @con_sucursal,
				@strIdPersona = @id,
				@strTipoPersona = 'CLIENTE'

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				IF(CONVERT(VARCHAR(10), GETDATE(), 111) BETWEEN CONVERT(VARCHAR(10), @dtmFechaInicial, 111) AND CONVERT(VARCHAR(10), @dtmFechaFinal, 111))
				BEGIN

					UPDATE	WhiteList
					SET		personState = 'Eliminar'
					,bitError = 1
					,strMensaje = 'SE ESTA CONGELANDO LA FACTURA'
					WHERE	gymId = @intIdGimnasio
					AND		invoiceId = @intNumeroFactura
					AND		dianId = @intIdDian

				END

			END

		END
		ELSE
		BEGIN

			PRINT('SE ESTA DESCONGELANDO LA FACTURA')
            UPDATE	WhiteList
            SET		personState = 'Pendiente'
					,bitError = @bitErrorTabla
					,strMensaje = @strMensajeTabla
            WHERE	gymId = @intIdGimnasio
			AND		invoiceId = @intNumeroFactura
			AND		dianId = @intIdDian
			AND		documentType = 'Factura'

			DECLARE @strIdPersona VARCHAR(MAX) = 0;
			DECLARE @intIdSucursal INT = 0;

            SELECT	TOP(1)
					@strIdPersona = gim_planes_usuario.plusu_identifi_cliente,
					@intIdSucursal = gim_planes_usuario.plusu_sucursal
            FROM	gim_planes_usuario 
            WHERE	cdgimnasio = @intIdGimnasio
			and		plusu_numero_fact = @intNumeroFactura
			and		plusu_fkdia_codigo = @intIdDian

			IF EXISTS(SELECT 1 FROM gim_configuracion_ingreso WHERE bitValidarPlanYReservaWeb = 1 AND bitAccesoPorReservaWeb = 0 AND cdgimnasio = @intIdGimnasio AND intfkSucursal = @intIdSucursal)
			BEGIN

				DECLARE @return_value INT = 0;
				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

						set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
						set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				IF (select count(*) from @TablaResultado) = 1
				BEGIN

					EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
							@idGimnasio = @intIdGimnasio,
							@intIdSucursal = @intIdSucursal,
							@strIdPersona = @strIdPersona,
							@strTrigger = 'gim_con_fac',
							@strTipoPersona = 'CLIENTE',
							@bitValidarReserva = 0,
							@bitValidarPlan = 1,
							@intNumeroFactura = 0
							,@bitError = @bitErrorTabla
							,@strMensaje = @strMensajeTabla

					IF @return_value = 1
					BEGIN
						PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
					END
					ELSE
					BEGIN
						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
					END

				END
				ELSE
				BEGIN

					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

				END

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_suspendedCourtesiesNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_suspendedCourtesiesNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_suspendedCourtesiesNuevo] ON [gim_con_fac_esp]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @intNumeroFactura VARCHAR(MAX) = '';
	DECLARE @bitFacturaCongelada BIT = 0;
	DECLARE @dtmFechaInicial DATETIME = NULL;
	DECLARE @dtmFechaFinal DATETIME = NULL;
	DECLARE @con_intfkSucursal INT = 0;
	DECLARE @ID VARCHAR(MAX) ='';
	DECLARE @strTipoPersona varchar(max);

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''
	
	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio,0),
			@intNumeroFactura = ISNULL(num_fac_con, 0),
			@bitFacturaCongelada = ISNULL(des_con, 0),
			@dtmFechaInicial = fec_ini_con,
			@dtmFechaFinal = fec_ter_con,
			@con_intfkSucursal = con_intfkSucursal
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		IF @bitFacturaCongelada = 0
		BEGIN

			PRINT('SE ESTA CONGELANDO LA CORTESIA')
			IF EXISTS(SELECT * FROM WhiteList WHERE gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND documentType = 'Cortes�a')
			BEGIN

			SET @ID = (SELECT DISTINCT id FROM WhiteList WHERE gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND documentType = 'Cortes�a')
			set @strTipoPersona =(SELECT DISTINCT UPPER(typePerson) FROM WhiteList WHERE gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND documentType = 'Cortes�a')

			INSERT INTO @TablaResultado
			EXEC [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @con_intfkSucursal,
				@strIdPersona = @id,
				@strTipoPersona = @strTipoPersona

				
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				IF(CONVERT(VARCHAR(10), GETDATE(), 111) BETWEEN CONVERT(VARCHAR(10), @dtmFechaInicial, 111) AND CONVERT(VARCHAR(10), @dtmFechaFinal, 111))
				BEGIN

					UPDATE	WhiteList
					SET		personState = 'Eliminar'
					,bitError = 1
					,strMensaje = 'SE ESTA CONGELANDO LA CORTESIA'
					WHERE	gymId = @intIdGimnasio
					AND		invoiceId = @intNumeroFactura
					AND		documentType = 'Cortes�a'

				END

			END

		END
		ELSE
		BEGIN

			PRINT('SE ESTA DESCONGELANDO LA CORTESIA')
            UPDATE	WhiteList
            SET		personState = 'Pendiente'
			,bitError = @bitErrorTabla
			,strMensaje = @strMensajeTabla
            WHERE	gymId = @intIdGimnasio
			AND		invoiceId = @intNumeroFactura
			AND		documentType = 'Cortes�a'

			DECLARE @strIdPersona VARCHAR(MAX) = 0;
			DECLARE @intIdSucursal INT = 0;

            SELECT	TOP(1)
					@strIdPersona = plusu_identifi_cliente,
					@intIdSucursal = plusu_sucursal
            FROM	gim_planes_usuario_especiales 
            WHERE	cdgimnasio = @intIdGimnasio
			and		plusu_numero_fact = @intNumeroFactura

			DECLARE @return_value INT = 0;

			IF EXISTS(SELECT  1 FROM gim_clientes WHERE cli_identifi = @strIdPersona)
			BEGIN
				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'CLIENTE'

						set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
						set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				IF (select count(*) from @TablaResultado) = 1
				BEGIN

					EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
							@idGimnasio = @intIdGimnasio,
							@intIdSucursal = @intIdSucursal,
							@strIdPersona = @strIdPersona,
							@strTrigger = 'gim_con_fac_esp',
							@strTipoPersona = 'CLIENTE',
							@bitValidarReserva = 0,
							@bitValidarPlan = 1,
							@intNumeroFactura = 0
							,@bitError = @bitErrorTabla
							,@strMensaje = @strMensajeTabla

					IF @return_value = 1
					BEGIN
						PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
					END
					ELSE
					BEGIN
						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
					END

				END
				ELSE
				BEGIN

					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

				END

			END
			ELSE IF EXISTS(SELECT  1 FROM gim_clientes_especiales WHERE cli_identifi = @strIdPersona)
			BEGIN

				INSERT INTO @TablaResultado
				EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTipoPersona = 'PROSPECTO'

						set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
						set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				IF (select count(*) from @TablaResultado) = 1
				BEGIN

					EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
							@idGimnasio = @intIdGimnasio,
							@intIdSucursal = @intIdSucursal,
							@strIdPersona = @strIdPersona,
							@strTrigger = 'gim_con_fac_esp',
							@strTipoPersona = 'PROSPECTO',
							@bitValidarReserva = 0,
							@bitValidarPlan = 0,
							@intNumeroFactura = 0
							,@bitError = @bitErrorTabla
							,@strMensaje = @strMensajeTabla

					IF @return_value = 1
					BEGIN
						PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL PROSPECTO A LISTA BLANCA')
					END
					ELSE
					BEGIN
						PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
					END
				
				END
				ELSE
				BEGIN

					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

				END

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_plans_adicionales' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_plans_adicionales
END
GO

CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_plans_adicionales] ON [gim_planes_adicionales]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @strCodigoPlan VARCHAR(MAX) = '';
	DECLARE @bitEstadoPlan BIT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(inserted.cdgimnasio,0),
			@strCodigoPlan = ISNULL(inserted.pla_codigo_plan, 0),
			@bitEstadoPlan = (SELECT (CASE WHEN pla_desactivado = 1 THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END) FROM gim_planes WHERE gim_planes.pla_codigo = inserted.pla_codigo_plan AND gim_planes.cdgimnasio = inserted.cdgimnasio)
	FROM	inserted

	DECLARE @bitUsaListaBlanca BIT = 0;

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)
		
	IF @bitUsaListaBlanca = 1 AND @bitEstadoPlan = 1
	BEGIN

		UPDATE	WhiteList
		SET		restrictions = (SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||') FROM [dbo].[fnObtenerRestriccionesWL] (@intIdGimnasio, subgroupId, planId)),
				personState = 'Pendiente'
		WHERE	WhiteList.planId = @strCodigoPlan

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_creditsNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_creditsNuevo
END
GO
CREATE  TRIGGER [dbo].[trgWhiteList_creditsNuevo] ON [dbo].[gim_creditos]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @intNumeroFactura VARCHAR(MAX) = '';
	DECLARE @dtmFechaCredito DATETIME = NULL;
	DECLARE @intIdDian INT = 0;
	DECLARE @bitCreditoPago BIT = 0;
	DECLARE @cre_identifi �int = 0;
	DECLARE @cre_sucursal INT = 0;

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio,0),
			@intNumeroFactura = ISNULL(cre_factura, 0),
			@dtmFechaCredito = cre_fecha,
			@intIdDian = ISNULL(cre_fkdia_codigo, 0),
			@bitCreditoPago = ISNULL(cre_pagado, 0),
			@cre_identifi �= isnull(cre_identifi ,0)
			,@cre_sucursal = isnull(cre_sucursal,0)
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		IF EXISTS(SELECT * FROM WhiteList WHERE gymId = @intIdGimnasio AND invoiceId = @intNumeroFactura AND dianId = @intIdDian and id = @cre_identifi)
		BEGIN

		
			INSERT INTO @TablaResultado
			EXEC [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @cre_sucursal,
				@strIdPersona = @cre_identifi,
				@strTipoPersona = 'CLIENTE'

				
				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

			IF CONVERT(VARCHAR(10), @dtmFechaCredito, 111) < CONVERT(VARCHAR(10),GETDATE(), 111) AND @bitCreditoPago = 0
			BEGIN

				PRINT('EL CREDITO SE ENCUENTRA PENDIENTE DE PAGO AUN Y LA FECHA DEL MISMO YA PASO')
				UPDATE	WhiteList
				SET		personState = 'Eliminar'
						, bitError = 1
						, strMensaje = 'EL CREDITO SE ENCUENTRA PENDIENTE DE PAGO AUN Y LA FECHA DEL MISMO YA PASO'
				WHERE	gymId = @intIdGimnasio
				AND		invoiceId = @intNumeroFactura
				AND		dianId = @intIdDian
				and id = @cre_identifi

				RETURN;

			END
			ELSE IF (CONVERT(VARCHAR(10), @dtmFechaCredito, 111) >= CONVERT(VARCHAR(10),GETDATE(), 111) AND @bitCreditoPago = 0) OR @bitCreditoPago = 1
			BEGIN

				PRINT('LA FECHA DEL CREDITO AUN NO ESTA VIGENTE O EL CREDITO YA SE PAGO')
				UPDATE	WhiteList
				SET		personState = 'Pendiente'
						, bitError = @bitErrorTabla
						,strMensaje = @strMensajeTabla
				WHERE	gymId = @intIdGimnasio
				AND		invoiceId = @intNumeroFactura
				AND		dianId = @intIdDian
				and id = @cre_identifi

			END

		END

		DECLARE @strIdPersona VARCHAR(MAX) = 0;
		DECLARE @intIdSucursal INT = 0;

        SELECT	TOP(1)
				@strIdPersona = plusu_identifi_cliente,
				@intIdSucursal = plusu_sucursal
        FROM	gim_planes_usuario
        WHERE	cdgimnasio = @intIdGimnasio
		and		plusu_numero_fact = @intNumeroFactura
		and  gim_planes_usuario.plusu_fkdia_codigo =  @intIdDian

		DECLARE @return_value INT = 0;

		INSERT INTO @TablaResultado
		EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @intIdSucursal,
				@strIdPersona = @strIdPersona,
				@strTipoPersona = 'CLIENTE'

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

		IF (select count(*) from @TablaResultado) = 1
		BEGIN

			DECLARE @bitValidaReservaWeb BIT = 0;
			DECLARE @bitValidarPlanYReservaWeb BIT = 0;
			DECLARE @bitValidarReserva BIT = 0;
			DECLARE @bitValidarPlan BIT = 0;

			SELECT	TOP(1)
					@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
					@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
			FROM	gim_configuracion_ingreso 
			WHERE	cdgimnasio = @intIdGimnasio
			AND		intfkSucursal = @intIdSucursal

			IF @bitValidarPlanYReservaWeb = 1 AND @bitValidaReservaWeb = 0
			BEGIN
						
				SET @bitValidarReserva = 1;
				SET @bitValidarPlan = 1;

			END
			ELSE IF @bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 1
			BEGIN

				SET @bitValidarReserva = 1;
				SET @bitValidarPlan = 0; 

			END

			EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTrigger = 'gim_con_fac',
					@strTipoPersona = 'CLIENTE',
					@bitValidarReserva = @bitValidarReserva,
					@bitValidarPlan = @bitValidarPlan,
					@intNumeroFactura = 0
					,@bitError = @bitErrorTabla
					,@strMensaje = @strMensajeTabla

			IF @return_value = 1
			BEGIN
				PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
			END
			ELSE
			BEGIN
				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
			END

		END
		ELSE
		BEGIN

			PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_subgroup' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_subgroup
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_subgroup] ON [gim_subgrupos]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @intIdSubgrupo INT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio,0),
			@intIdSubgrupo = ISNULL(subg_intcodigo, 0)
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		IF EXISTS(SELECT 1 FROM WhiteList WHERE subgroupId = @intIdSubgrupo and gymId = @intIdGimnasio)
		BEGIN
			
			UPDATE	WhiteList
			SET		restrictions = (SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||') FROM [dbo].[fnObtenerRestriccionesWL] (@intIdGimnasio, subgroupId, planId)),
					personState = 'Pendiente'
			WHERE	WhiteList.subgroupId = @intIdSubgrupo

		END		

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_subgroup_adicionales' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_subgroup_adicionales
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_subgroup_adicionales] ON [gim_subgru_adicionales]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @intIdSubgrupo INT = 0;

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio,0),
			@intIdSubgrupo = ISNULL(subgru_adi_codigo_subgru, 0)
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		IF EXISTS(SELECT 1 FROM WhiteList WHERE subgroupId = @intIdSubgrupo and gymId = @intIdGimnasio)
		BEGIN
			
			UPDATE	WhiteList
			SET		restrictions = (SELECT ISNULL(CONCAT(LUNES, '|', MARTES, '|', MIERCOLES, '|', JUEVES, '|', VIERNES, '|', SABADO, '|', DOMINGO,'|', FESTIVO), '|||||||') FROM [dbo].[fnObtenerRestriccionesWL] (@intIdGimnasio, subgroupId, planId)),
					personState = 'Pendiente'
			WHERE	WhiteList.subgroupId = @intIdSubgrupo

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_contractsNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_contractsNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_contractsNuevo] ON [gim_detalle_contrato]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @strIdPersona VARCHAR(MAX) = NULL;
	DECLARE @intIdSucursal INT = 0;

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio,0),
			@strIdPersona = ISNULL(dtcont_doc_cliente, '')
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		DECLARE @return_value INT = 0;
		DECLARE @bitValidaReservaWeb BIT = 0;
		DECLARE @bitValidarPlanYReservaWeb BIT = 0;
		DECLARE @bitValidarReserva BIT = 0;
		DECLARE @bitValidarPlan BIT = 0;

		SELECT	TOP(1) @intIdSucursal = cli_sucursal FROM gim_clientes WHERE cli_identifi = @strIdPersona AND cdgimnasio = @intIdGimnasio

		SELECT	TOP(1)
				@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
				@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
		FROM	gim_configuracion_ingreso 
		WHERE	cdgimnasio = @intIdGimnasio
		AND		intfkSucursal = @intIdSucursal

		IF @bitValidarPlanYReservaWeb = 1 AND @bitValidaReservaWeb = 0
		BEGIN
						
			SET @bitValidarReserva = 1;
			SET @bitValidarPlan = 1;

		END
		ELSE IF @bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 1
		BEGIN

			SET @bitValidarReserva = 1;
			SET @bitValidarPlan = 0; 

		END

		INSERT INTO @TablaResultado
		EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
				@idGimnasio = @intIdGimnasio,
				@intIdSucursal = @intIdSucursal,
				@strIdPersona = @strIdPersona,
				@strTipoPersona = 'CLIENTE'


		set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
		set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

		IF (select count(*) from @TablaResultado) = 1
		BEGIN

			EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTrigger = 'gim_con_fac_esp',
					@strTipoPersona = 'CLIENTE',
					@bitValidarReserva = @bitValidarReserva,
					@bitValidarPlan = @bitValidarPlan,
					@intNumeroFactura = 0
					,@bitError = @bitErrorTabla
					,@strMensaje = @strMensajeTabla

			IF @return_value = 1
			BEGIN
				PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
			END
			ELSE
			BEGIN
				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
			END

		END
		ELSE
		BEGIN

			PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_reserveNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_reserveNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_reserveNuevo] ON [gim_reservas]
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @intIdGimnasio INT, @strIdPersona VARCHAR(MAX), @intIdSucursal INT,
            @intIdReserva INT, @strEstadoReserva VARCHAR(MAX), @dtmFechaReserva DATETIME;

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''

    SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio, 0),
			@strIdPersona = ISNULL(IdentificacionCliente, ''),
			@intIdSucursal = ISNULL(cdsucursal, 0),
			@intIdReserva = ISNULL(cdreserva, 0),
			@strEstadoReserva = estado,
			@dtmFechaReserva = fecha_clase
    FROM	inserted;

    DECLARE @bitIngressWhiteList BIT;
    SELECT TOP(1) @bitIngressWhiteList = ISNULL(bitIngressWhiteList, 0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio;

    IF @bitIngressWhiteList = 1 AND CONVERT(DATE, @dtmFechaReserva) = CONVERT(DATE, GETDATE())
    BEGIN

        DECLARE @personState VARCHAR(20), @isRestrictionClass VARCHAR(5);

        IF EXISTS (SELECT 1 FROM WhiteList WHERE id = @strIdPersona AND gymId = @intIdGimnasio AND branchId = @intIdSucursal AND reserveId = @intIdReserva)
        BEGIN
            SET @personState = CASE
                WHEN @strEstadoReserva = 'Anulada' THEN 'Eliminar'
                WHEN @strEstadoReserva = 'Activa' THEN 'Pendiente'
                WHEN @strEstadoReserva = 'Asistido' THEN 'Pendiente'
                ELSE @personState END;

            SET @isRestrictionClass = CASE
                WHEN @strEstadoReserva = 'Activa' THEN 'true'
                ELSE 'false' END;

            UPDATE	WhiteList
            SET		personState = @personState,
					isRestrictionClass = @isRestrictionClass
            WHERE	id = @strIdPersona
			AND		gymId = @intIdGimnasio
			AND		reserveId = @intIdReserva;
        END
        ELSE IF @strEstadoReserva <> 'Anulada'
        BEGIN
            DECLARE @return_value INT;

			INSERT INTO @TablaResultado
            EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'CLIENTE';

			set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
			set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

            IF (select count(*) from @TablaResultado) = 1
            BEGIN
                EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'gim_reservas',
						@strTipoPersona = 'CLIENTE',
						@bitValidarReserva = 1,
						@bitValidarPlan = 0,
						@intNumeroFactura = 0
						,@bitError = @bitErrorTabla
						,@strMensaje = @strMensajeTabla;

                IF @return_value = 1
                BEGIN
                    PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA');
                END
                ELSE
                BEGIN
                    PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA');
                END;
            END
            ELSE
            BEGIN
                PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA');
            END;
        END;
    END;
END;
go
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_appointmentNuevo' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_appointmentNuevo
END
GO
CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_appointmentNuevo] ON [tblCitas]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @strIdPersona VARCHAR(MAX) = NULL;
	DECLARE @intIdSucursal INT = 0;

	DECLARE @TablaResultado TABLE (
		bitError BIT,
		strMensaje varchar(max),
		strTipoPersona varchar(max)
		);
	DECLARE @bitErrorTabla bit = 0
	DECLARE @strMensajeTabla varchar(max) = ''
	
	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(intEmpresa, 0),
			@strIdPersona = ISNULL(strIdentificacionPaciente, '')
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		IF NOT EXISTS(SELECT 1 FROM WhiteList WHERE id = @strIdPersona and gymId = @intIdGimnasio and branchId = @intIdSucursal AND typePerson = 'Cliente')
		BEGIN

			DECLARE @return_value INT = 0;

			SELECT	TOP(1) @intIdSucursal = cli_sucursal FROM gim_clientes WHERE cli_identifi = @strIdPersona AND cdgimnasio = @intIdGimnasio

			INSERT INTO @TablaResultado
			EXEC	@return_value = [dbo].[spValidarPersonaListaBlancaNuevo]
					@idGimnasio = @intIdGimnasio,
					@intIdSucursal = @intIdSucursal,
					@strIdPersona = @strIdPersona,
					@strTipoPersona = 'CLIENTE'

			IF (select count(*) from @TablaResultado) = 1
			BEGIN

				DECLARE @bitValidaReservaWeb BIT = 0;
				DECLARE @bitValidarPlanYReservaWeb BIT = 0;
				DECLARE @bitValidarReserva BIT = 0;
				DECLARE @bitValidarPlan BIT = 0;

				set @bitErrorTabla = (select top 1 bitError from @TablaResultado)
				set	@strMensajeTabla  = (select top 1 strMensaje from @TablaResultado)

				SELECT	TOP(1)
						@bitValidaReservaWeb = ISNULL(bitAccesoPorReservaWeb, 0),
						@bitValidarPlanYReservaWeb = ISNULL(bitValidarPlanYReservaWeb, 0)
				FROM	gim_configuracion_ingreso 
				WHERE	cdgimnasio = @intIdGimnasio
				AND		intfkSucursal = @intIdSucursal

				IF @bitValidarPlanYReservaWeb = 1 AND @bitValidaReservaWeb = 0
				BEGIN
						
					SET @bitValidarReserva = 1;
					SET @bitValidarPlan = 1;

				END
				ELSE IF @bitValidarPlanYReservaWeb = 0 AND @bitValidaReservaWeb = 1
				BEGIN

					SET @bitValidarReserva = 1;
					SET @bitValidarPlan = 0; 

				END

				EXEC	@return_value = [dbo].[spInsertarPersonaListaBlancaNuevo]
						@idGimnasio = @intIdGimnasio,
						@intIdSucursal = @intIdSucursal,
						@strIdPersona = @strIdPersona,
						@strTrigger = 'tblCitas',
						@strTipoPersona = 'CLIENTE',
						@bitValidarReserva = @bitValidarReserva,
						@bitValidarPlan = @bitValidarPlan,
						@intNumeroFactura = 0
						,@bitError = @bitErrorTabla
						,@strMensaje = @strMensajeTabla;

				IF @return_value = 1
				BEGIN
					PRINT('SE HA AGREGADO DE FORMA SATISFACTORIA EL CLIENTE A LISTA BLANCA')
				END
				ELSE
				BEGIN
					PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')
				END

			END
			ELSE
			BEGIN

				PRINT('LA PERSONA NO HA CUMPLIDO CON LOS PARAMETROS NECESARIOS PARA INGRESAR A LISTA BLANCA')

			END

		END

	END

END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_clientCards' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_clientCards
END
GO

CREATE OR ALTER TRIGGER [dbo].[trgWhiteList_clientCards] ON [ClientCards]
AFTER INSERT, UPDATE
AS
BEGIN

	DECLARE @intIdGimnasio INT = 0;
	DECLARE @bitUsaListaBlanca BIT = 0;
	DECLARE @strIdPersona VARCHAR(MAX) = NULL;;
	
	SELECT	TOP(1)
			@intIdGimnasio = ISNULL(cdgimnasio, 0),
			@strIdPersona = ISNULL(cli_identifi, '')
	FROM	inserted

	SET @bitUsaListaBlanca = (SELECT TOP(1) ISNULL(bitIngressWhiteList,0) FROM tblConfiguracion WHERE cdgimnasio = @intIdGimnasio)

	IF @bitUsaListaBlanca = 1
	BEGIN

		UPDATE	WhiteList
		SET		personState = 'Pendiente'
		WHERE	personState != 'Eliminar'
		AND		personState != 'Pendiente'
		AND		typePerson = 'Cliente'
		AND		id = @strIdPersona
		AND		gymId = @intIdGimnasio

	END

END
GO

----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------INCIO CREACION DE TRIGERS------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------

----posible utilidad 
----IF EXISTS (SELECT * FROM sys.objects WHERE name = 'spActualizarCantidadTiquetesWeb' AND type in (N'P'))
----BEGIN
----    DROP PROCEDURE spActualizarCantidadTiquetesWeb
----END
----GO

----CREATE PROCEDURE [dbo].[spActualizarCantidadTiquetesWeb]
----    @id INT = NULL, 
----    @invoiceId INT = NULL, 
----    @dianId INT = NULL, 
----    @documentType VARCHAR(MAX) = NULL, 
----    @availableEntries INT = NULL, 
----    @gymId INT = NULL, 
----    @branchId INT = NULL
----AS
----BEGIN
----    IF (@documentType = 'Factura')
----    BEGIN
----        UPDATE gim_planes_usuario
----        SET plusu_tiq_disponible = @availableEntries
----        WHERE plusu_numero_fact = @invoiceId
----            AND plusu_fkdia_codigo = @dianId
----            AND cdgimnasio = @gymId
----            AND plusu_sucursal = @branchId
----            AND plusu_identifi_cliente = @id
----    END

----    IF (@documentType = 'Cortesia')
----    BEGIN

----		IF @invoiceId > 0
----		BEGIN

----			UPDATE gim_planes_usuario_especiales
----			SET plusu_tiq_disponible = @availableEntries
----			WHERE plusu_numero_fact = @invoiceId
----				AND plusu_fkdia_codigo = @dianId
----				AND cdgimnasio = @gymId
----				AND plusu_sucursal = @branchId
----				AND plusu_identifi_cliente = @id

----		END
----		ELSE
----		BEGIN

----			IF @availableEntries > 0
----			BEGIN

----				UPDATE	WhiteList
----				SET		availableEntries = @availableEntries
----				WHERE	id = @id
----				AND		branchId = @branchId
----				AND		documentType = 'Cortes�a'
----				AND		invoiceId = @invoiceId
----				AND		dianId = @dianId
				
----			END
----			ELSE
----			BEGIN

----				UPDATE	WhiteList
----				SET		availableEntries = @availableEntries,
----						personState = 'Eliminar'
----				WHERE	id = @id
----				AND		branchId = @branchId
----				AND		documentType = 'Cortes�a'
----				AND		invoiceId = @invoiceId
----				AND		dianId = @dianId

----			END

----		END

----    END

----END
----GO