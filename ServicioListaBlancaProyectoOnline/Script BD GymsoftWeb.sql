IF NOT EXISTS(SELECT * FROM tblConfiguracion WHERE bitIngressWhiteList = 1)
BEGIN

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Invoice' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_Invoice --TERMINADO - PROBADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Courtesy' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_Courtesy -- TERMINADO - 
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Visit' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_Visit -- TERMINADO
	END
	
	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Visitors' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_Visitors -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Fingerprint' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_Fingerprint -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_specialClients' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_specialClients -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_clients' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_clients -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_employee' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_employee -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_plans' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_plans -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_BlackList' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_BlackList -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_suspendedInvoices' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_suspendedInvoices --TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_suspendedCourtesies' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_suspendedCourtesies --TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_plans_adicionales' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_plans_adicionales --TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_credits' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_credits -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_subgroup' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_subgroup --TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_subgroup_adicionales' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_subgroup_adicionales -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_contracts' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_contracts -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_reserve' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_reserve -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_appointment' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_appointment -- TERMINADO
	END

	IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_clientCards' and type in (N'TR'))
	BEGIN
		DROP TRIGGER trgWhiteList_clientCards -- TERMINADO
	END

END
GO

--BORRADOS ADICIONALES
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'tgrInsertarDetalle' and type in (N'TR'))
BEGIN
	DROP TRIGGER tgrInsertarDetalle
END
GO

--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------INICIO-----------------------------------------------------------------------------------
----------------------------------------------------------------------FUNCIONES VISTAS -----------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
if exists (select * from sys.objects where name = 'vwSuspendedInvoices' and type in (N'V'))
begin
    drop view vwSuspendedInvoices
end
go

create view vwSuspendedInvoices
as
    --FACTURAS CONGELADAS
    select cf.num_fac_con as 'invoiceId', cf.con_fkdia_codigo as 'dian', cf.con_sucursal as 'branchId', cf.cdgimnasio as 'gymId' 
    from gim_con_fac cf
    where cf.des_con = 0 and 
          convert(varchar(10),getdate(),111) between convert(varchar(10),cf.fec_ini_con,111) and 
                                                     convert(varchar(10),cf.fec_ter_con,111)
go

if exists (select * from sys.objects where name = 'vwSuspendedCourtesies' and type in (N'V'))
begin
    drop view vwSuspendedCourtesies
end
go

create view vwSuspendedCourtesies
as
    --CORTESÍAS CONGELADAS
    select cfe.num_fac_con as 'invoiceId', cfe.con_intfkSucursal as 'branchId', cfe.cdgimnasio as 'gymId' 
    from gim_con_fac_esp cfe
    where cfe.des_con = 0 and 
          convert(varchar(10),getdate(),111) between convert(varchar(10),cfe.fec_ini_con,111) and 
                                                     convert(varchar(10),cfe.fec_ter_con,111)
go

if exists (select * from sys.objects where name = 'vwEmployeesWithPlan' and type in (N'V'))
begin
    drop view vwEmployeesWithPlan
end
go

create view vwEmployeesWithPlan
as
    select emp.emp_identifi as 'id', 
            isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName', 
            pu.plusu_fecha_vcto as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = emp.cdgimnasio and eu.enusu_identifi = pu.plusu_identifi_cliente order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Empleado' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') as 'restrictions',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', emp.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(20)) as 'personState', isnull(emp.emp_sin_huella,0) as 'withoutFingerprint',
            --HUELLA DEL EMPLEADO		
            case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else null end as 'fingerprint',
			case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else null end as 'strDatoFoto',

            cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
            cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            cast(0 as int) as 'subgroupId', isnull(convert(varchar(50),emp.emp_strcodtarjeta),'') as 'cardId'
    from gim_empleados emp inner join gim_planes_usuario pu on (emp.cdgimnasio = pu.cdgimnasio and
                                                                emp.emp_identifi = pu.plusu_identifi_cliente)
                           inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and
                                                         pu.cdgimnasio = pla.cdgimnasio)
                           inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and
                                                            pu.cdgimnasio = su.cdgimnasio)
    where convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111) and
          pu.plusu_avisado = 0 and
          pu.plusu_est_anulada = 0 and
          pu.plusu_codigo_plan != 999 and
          emp.emp_estado = 1 and pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo)
    
    union

    select emp.emp_identifi as 'id', 
            isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName', 
            pu.plusu_fecha_vcto as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = emp.cdgimnasio and eu.enusu_identifi = pu.plusu_identifi_cliente order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Empleado' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') as 'restrictions',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', emp.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(20)) as 'personState', isnull(emp.emp_sin_huella,0) as 'withoutFingerprint',
            --HUELLA DEL EMPLEADO		
            case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else null end as 'fingerprint',
			case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else null end as 'strDatoFoto',

            cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
            cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            cast(0 as int) as 'subgroupId', isnull(convert(varchar(50),emp.emp_strcodtarjeta),'') as 'cardId'
    from gim_empleados emp inner join gim_planes_usuario_especiales pu on (emp.cdgimnasio = pu.cdgimnasio and
                                                                           emp.emp_identifi = pu.plusu_identifi_cliente)
                           inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and
                                                         pu.cdgimnasio = pla.cdgimnasio)
                           inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and
                                                            pu.cdgimnasio = su.cdgimnasio)
    where convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111) and
          pu.plusu_avisado = 0 and
          pu.plusu_est_anulada = 0 and
          pu.plusu_codigo_plan != 999 and
          emp.emp_estado = 1 and pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal)
go

if exists (select * from sys.objects where name = 'vwEmployeesWithoutPlan' and type in (N'V'))
begin
    drop view vwEmployeesWithoutPlan
end
go

create view vwEmployeesWithoutPlan
as
    select emp.emp_identifi as 'id', 
                isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'name',
                cast(0 as int) as 'planId', cast('' as varchar(100)) as 'planName',
                null as 'expirationDate',
                --ÚLTIMA ENTRADA
                null as 'lastEntry',
                --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = emp.cdgimnasio and eu.enusu_identifi = emp_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
                cast('' as varchar(50)) as 'planType', cast('Empleado' as varchar(100)) as 'typePerson',
                --TIQUETES DISPONIBLES
                cast(0 as int) as 'availableEntries',
                --HORARIOS INGRESO PLAN, POR DÍA.
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) + '| ' +
                cast(' ' as varchar(max)) as 'restrictions',
                su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', emp.cdgimnasio as 'gymId',
                cast('Pendiente' as varchar(20)) as 'personState', isnull(emp.emp_sin_huella,0) as 'withoutFingerprint',
                --HUELLA DEL EMPLEADO		
                case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
                case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else null end as 'fingerprint',
				case when (emp.emp_sin_huella is null or emp.emp_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = emp.emp_identifi and hu.cdgimnasio = emp.cdgimnasio) else null end as 'strDatoFoto',

                cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
                cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName',  
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath',
            cast(0 as int) as 'invoiceId', cast(0 as int) as 'dianId', cast('' as varchar(50)) as 'documentType',
            cast(0 as int) as 'subgroupId', isnull(convert(varchar(50),emp.emp_strcodtarjeta),'') as 'cardId' 
    from gim_empleados emp inner join gim_sucursales su on (emp.emp_sucursal = su.suc_intpkIdentificacion and
                                                            emp.cdgimnasio = su.cdgimnasio)
    where emp_estado = 1
go

if exists (select * from sys.objects where name = 'vwSpecialClientWithCourtesy' and type in (N'V'))
begin
    drop view vwSpecialClientWithCourtesy
end
go

create view vwSpecialClientWithCourtesy
as
    select dbo.fFloatAVarchar(cli.cli_identifi) as 'id', 
            isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',
            pue.plusu_fecha_vcto as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Prospecto' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pue.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pue.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') as 'restrictions',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', case when (cli.cli_EntryFingerprint = 1) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
            --HUELLA DEL EMPLEADO		
            case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
			case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
            cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pue.plusu_numero_fact as 'invoiceId', pue.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            cast(0 as int) as 'subgroupId', cast('' as varchar(50)) as 'cardId'
    from gim_clientes_especiales cli inner join gim_planes_usuario_especiales pue on (dbo.fFloatAVarchar(cli.cli_identifi) = pue.plusu_identifi_cliente and
                                                                                      cli.cdgimnasio = pue.cdgimnasio)
                                     inner join gim_sucursales su on (pue.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                     inner join gim_planes pla on (pue.plusu_codigo_plan = pla.pla_codigo and pue.cdgimnasio = pla.cdgimnasio)
    where pue.plusu_avisado = 0 and convert(varchar(10),pue.plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pue.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pue.plusu_est_anulada = 0 and pue.plusu_codigo_plan != 999
go

if exists (select * from sys.objects where name = 'vwSpecialClient' and type in (N'V'))
begin
    drop view vwSpecialClient
end
go

create view vwSpecialClient
as
    select dbo.fFloatAVarchar(cli.cli_identifi) as 'id', 
            isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
            cast(0 as int) as 'planId', cast('' as varchar(max)) as 'planName',null as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            cast('' as varchar(10)) as 'planType', cast('Prospecto' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when ((cli.cli_cortesia = 1 and cli.cli_entro_cortesia = 0) and cli.cli_entrar_conocer = 1) then cast(2 as int)
                 when (cli.cli_cortesia = 1 and cli.cli_entro_cortesia = 0) then cast(1 as int)
                 else cast(1 as int) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) as 'restrictions',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', case when (cli.cli_EntryFingerprint = 1) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
            --HUELLA DEL EMPLEADO		
            case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
			case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
            cast(0 as bit) as 'updateFingerprint', cli.cli_entrar_conocer as 'know', cli.cli_cortesia as 'courtesy',
            cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath',
            cast(0 as int) as 'invoiceId', cast(0 as int) as 'dianId', cast('' as varchar(50)) as 'documentType',
            cast(0 as int) as 'subgroupId', cast('' as varchar(50)) as 'cardId'
    from gim_clientes_especiales cli inner join gim_sucursales su on (cli.cli_intfkSucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
    where (cli.cli_cortesia = 1 and cli.cli_entro_cortesia = 0) or cli.cli_entrar_conocer = 1
GO

if exists (select * from sys.objects where name = 'vwVisitors' and type in (N'V'))
begin
    drop view vwVisitors
end
go

create view vwVisitors
as
    select vis.vis_strVisitorId as 'id', 
            isnull(vis.vis_strName,'') + ' ' + isnull(vis.vis_strFirstLastName,'') + ' ' + isnull(vis.vis_strSecondLastName,'') as 'name',
            cast(0 as int) as 'planId', cast('' as varchar(max)) as 'planName',null as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            cast('' as varchar(10)) as 'planType', cast('Visitante' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            cast(1 as int) as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) + '| ' +
            cast(' ' as varchar(max)) as 'restrictions',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', vis.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', case when (vis.vis_EntryFingerprint = 0) then vis.vis_EntryFingerprint else cast(0 as bit) end as 'withoutFingerprint',
            --HUELLA DEL EMPLEADO		
            case when (vis.vis_EntryFingerprint = 1) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = vis.vis_strVisitorId and hu.cdgimnasio = vis.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (vis.vis_EntryFingerprint = 1) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = vis.vis_strVisitorId and hu.cdgimnasio = vis.cdgimnasio) else null end as 'fingerprint',
            case when (vis.vis_EntryFingerprint = 1) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = vis.vis_strVisitorId and hu.cdgimnasio = vis.cdgimnasio) else null end as 'strDatoFoto',
			cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
            cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath',
            cast(0 as int) as 'invoiceId', cast(0 as int) as 'dianId', cast('' as varchar(50)) as 'documentType',
            cast(0 as int) as 'subgroupId', cast('' as varchar(50)) as 'cardId'
    from Visitors vis inner join gim_sucursales su on (vis.vis_intBranch = su.suc_intpkIdentificacion and vis.cdgimnasio = su.cdgimnasio)
                      inner join Visit v on (vis.cdgimnasio = v.cdgimnasio and vis.vis_strVisitorId = v.VisitorId and vis.vis_intBranch = v.Branch)
    where (select count(*) from gim_entradas_usuarios with(index (IX_ENUSU_WhiteList)) where cdgimnasio = vis.cdgimnasio and enusu_identifi = v.VisitorId and enusu_VisitId = v.Id and enusu_fecha_entrada = v.DateVisit) = 0
          and convert(varchar(10),v.DateVisit,111) = convert(varchar(10),getdate(),111)
go
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------FIN------------------------------------------------------------------------------------
----------------------------------------------------------------------FUNCIONES VISTAS -----------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------INICIO--------------------------------------------------------------------------------
----------------------------------------------------------------------FUNCIONES LISTA BLANCA ----------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
if exists (select * from sys.objects where name = 'fnAdditionalRestrictions' and type in (N'TF'))
begin
    drop function fnAdditionalRestrictions
end
go

create function fnAdditionalRestrictions(@gymId int, @bitConsultaInfoCita bit, @bitBloqueoCitaNoCumplidaMSW bit, @bitBloqueoClienteNoApto bit, @bitBloqueoNoDisentimento bit,
                                         @bitBloqueoNoAutorizacionMenor bit, @fullAge int, @daysWithoutEntries int, @daysWithoutMedicalAppointment int)
returns @table table
(
    id varchar(15)
)
as
begin
    --Capturamos los datos de las citas médicas de los clientes
    if (@bitConsultaInfoCita = 1)
    begin
        if (@bitBloqueoCitaNoCumplidaMSW = 1)
        begin
            insert @table
            select *
            from fnClientsWithLockByAppointment(@gymId, @daysWithoutEntries, @daysWithoutMedicalAppointment)
        end
    end
    

    if (@bitBloqueoClienteNoApto = 1)
    begin
        insert @table
        select cli.cli_identifi
        from gim_clientes cli
        where cli.cdgimnasio = @gymId and cli.cli_estado = 1
              and cli.cli_Apto = 0
    end
    
    if (@bitBloqueoNoDisentimento = 1)
    begin
        insert @table
        select cli.cli_identifi
        from gim_clientes cli
        where cli.cdgimnasio = @gymId and cli.cli_estado = 1
              and cli.cli_imgdisentimiento is null
    end
    
    if (@bitBloqueoNoAutorizacionMenor = 1)
    begin
        insert @table
        select cli.cli_identifi
        from gim_clientes cli
        where cli.cdgimnasio = @gymId and cli.cli_estado = 1
              and (cli.cli_bitAutorizacionM is null or cli.cli_bitAutorizacionM = 0)
              and DATEDIFF(HOUR, CONVERT(datetime, CONVERT(VARCHAR(10), cli.cli_fecha_nacimien,111),111), GETDATE())/(8766) < @fullAge
    end
    return;
end
go

if exists (select * from sys.objects where name = 'fnClientsWithLockByAppointment' and type in (N'TF'))
begin
    drop function fnClientsWithLockByAppointment
end
go

create function fnClientsWithLockByAppointment(@gymId int, @daysWithoutEntries int, @daysWithoutMedicalAppointment int)
returns @table table
(
    id varchar(15)
)
as
begin
    insert @table
    select cli.cli_identifi
    from gim_clientes cli inner join tblCitas c  on (cli.cdgimnasio = c.intEmpresa 
                                                     and cli.cli_identifi = c.strIdentificacionPaciente)
    where cli.cli_altoRiesgo = 0 and c.bitAtendida = 0 and c.bitCancelada = 0 
          and cli.cdgimnasio = @gymId and cli.cli_estado = 1
          and c.bitActivo = 1 and cli.cli_imgdisentimiento is null
          and convert(varchar(10),c.datFechaCita,111) < convert(varchar(10),getdate(),111)
    union
    select cli.cli_identifi
    from gim_clientes cli left join tblCitas c  on (cli.cdgimnasio = c.intEmpresa 
                                                    and cli.cli_identifi = c.strIdentificacionPaciente)
    where cli.cli_altoRiesgo = 1 and cli.cli_estado = 1 and cli.cdgimnasio = @gymId
    union
    select cli.cli_identifi
    from gim_clientes cli inner join tblCitas c  on (cli.cdgimnasio = c.intEmpresa 
                                                     and cli.cli_identifi = c.strIdentificacionPaciente)
    where cli.cli_altoRiesgo = 1 and cli.cli_estado = 1 and cli.cdgimnasio = @gymId
          and c.bitAtendida = 0 and c.bitCancelada = 0 and c.bitActivo = 1
          and (select count(*) 
               from gim_entradas_usuarios eu
               where eu.enusu_identifi = cli.cli_identifi
                     and eu.cdgimnasio = cli.cdgimnasio
                     and convert(varchar(10),eu.enusu_fecha_entrada,111) > convert(varchar(10),c.datFechaCita,111)) > @daysWithoutEntries
    union
    select cli.cli_identifi
    from gim_clientes cli inner join tblCitas c  on (cli.cdgimnasio = c.intEmpresa 
                                                     and cli.cli_identifi = c.strIdentificacionPaciente)
    where cli.cli_altoRiesgo = 1 and cli.cli_estado = 1 and cli.cdgimnasio = @gymId
          and c.bitAtendida = 0 and c.bitCancelada = 0 and c.bitActivo = 1
          and datediff(day,c.datFechaCita,getdate()) > @daysWithoutMedicalAppointment
    return;
end
go


if exists (select * from sys.objects where name = 'fnClientsWithVigentPlan' and type in (N'TF'))
begin
    drop function fnClientsWithVigentPlan
end
go

CREATE FUNCTION [dbo].[fnClientsWithVigentPlan](@gymId int, @branchId int)
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
	updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    
    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
           cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                               cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo)

    union

    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
		   cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli 
	inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                          cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal)
    return;
end
go

if exists (select * from sys.objects where name = 'fnClientsWithVigentPlanAndNotReserve' and type in (N'TF'))
begin
    drop function fnClientsWithVigentPlanAndNotReserve
end
go

CREATE function [dbo].[fnClientsWithVigentPlanAndNotReserve](@gymId int, @branchId int)
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    
    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
		   cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                               cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                          left join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo)
          and res.IdentificacionCliente is null

    union

    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
		   cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                          cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                          left join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal)
          and res.IdentificacionCliente is null
    return;
end

go

if exists (select * from sys.objects where name = 'fnClientsWithVigentPlanAndNotReserveWithSignedContract' and type in (N'TF'))
begin
    drop function fnClientsWithVigentPlanAndNotReserveWithSignedContract
end
go

CREATE function [dbo].[fnClientsWithVigentPlanAndNotReserveWithSignedContract](@gymId int, @branchId int)
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    
    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
           cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                               cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                          inner join gim_detalle_contrato dc on (dc.dtcont_doc_cliente = pu.plusu_identifi_cliente and dc.cdgimnasio = pu.cdgimnasio and dc.dtcont_numero_plan = pu.plusu_numero_fact and dc.dtcont_fkdia_codigo = pu.plusu_fkdia_codigo)
                          inner join gim_contrato c on (dc.dtcont_FKcontrato = c.cont_codigo and dc.cdgimnasio = c.cdgimnasio)
                          left join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
    where pu.plusu_avisado = 0 and 
	convert(VARCHAR(10),DATEADD(DAY, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and c.int_fkTipoContrato = 1 and 
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo)
          and res.IdentificacionCliente is null

    union

    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
		   cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                          cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                          inner join gim_detalle_contrato dc on (dc.dtcont_doc_cliente = pu.plusu_identifi_cliente and dc.cdgimnasio = pu.cdgimnasio and dc.dtcont_numero_plan = pu.plusu_numero_fact and dc.dtcont_fkdia_codigo = pu.plusu_fkdia_codigo)
                          inner join gim_contrato c on (dc.dtcont_FKcontrato = c.cont_codigo and dc.cdgimnasio = c.cdgimnasio)
                          left join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
    where pu.plusu_avisado = 0 
	and CONVERT(VARCHAR(10),DATEADD(DAY, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and c.int_fkTipoContrato = 1 and 
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal)
          and res.IdentificacionCliente is null
    return;
end

go

if exists (select * from sys.objects where name = 'fnClientsWithVigentPlanAndReserve' and type in (N'TF'))
begin
    drop function fnClientsWithVigentPlanAndReserve
end
go
CREATE function [dbo].[fnClientsWithVigentPlanAndReserve](@gymId int, @branchId int, @bitImprimirHoraReserva bit, @intMinutosAntesReserva int, @intMinutosDespuesReserva int)
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    
    select cli.cli_identifi as 'id', 
            isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',
			dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', 
			case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
            --HUELLA DEL CLIENTE		
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
			cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
            --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
            --CÓDIGO DE GRUPO
            isnull((select grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
            cast(1 as bit) as 'isRestrictionClass', 
            --HORARIO DE INGRESO SEGÚN RESERVA
            case -- en caso de que no tenga minutos antes y despues para la reserva toma el rango completo de la clase 
			when @intMinutosAntesReserva = 0 and @intMinutosDespuesReserva = 0 then
			convert(varchar(10),hcla.hora,114) + '-' + convert(varchar(10),hcla.hora_fin,114)
			else convert(varchar(10),dateadd(minute,-@intMinutosAntesReserva,res.fecha_clase),114) + '-' + convert(varchar(10),dateadd(minute,@intMinutosDespuesReserva,res.fecha_clase),114) end  as 'classSchedule',
            res.fecha_clase as 'dateClass', res.cdreserva as 'reserveId', cla.nombre as 'className', isnull(res.megas_utilizadas,0) as 'utilizedMegas', isnull(res.tiq_utilizados,0) as 'utilizedTickets', 
            isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'employeeName', 
            res.intensidad as 'classIntensity', res.estado as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                cli.cdgimnasio = pu.cdgimnasio)
                            inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                            inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                            inner join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
                            inner join gim_clases cla on (res.cdclase = cla.cdclase and res.cdgimnasio = cla.cdgimnasio)
                            inner join gim_horarios_clase hcla on (res.cdhorario_clase = hcla.cdhorario_clase and res.cdgimnasio = hcla.cdgimnasio and cla.cdclase = hcla.cdclase)
                            inner join gim_empleados emp on (hcla.profesor = emp.cdempleado and hcla.cdgimnasio = emp.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo) and
          res.estado != 'Anulada' and res.estado != 'Asistio' and convert(varchar(10),res.fecha_clase,111) = convert(varchar(10),getdate(),111)


    union

    select cli.cli_identifi as 'id', 
            isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',
			dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', 
			case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
            --HUELLA DEL CLIENTE		
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
			cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
            --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
            --CÓDIGO DE GRUPO
            isnull((select grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
            cast(1 as bit) as 'isRestrictionClass', 
            --HORARIO DE INGRESO SEGÚN RESERVA
			case -- en caso de que no tenga minutos antes y despues para la reserva toma el rango completo de la clase 
			when @intMinutosAntesReserva = 0 and @intMinutosDespuesReserva = 0 then
			convert(varchar(10),hcla.hora,114) + '-' + convert(varchar(10),hcla.hora_fin,114)
			else convert(varchar(10),dateadd(minute,-@intMinutosAntesReserva,res.fecha_clase),114) + '-' + convert(varchar(10),dateadd(minute,@intMinutosDespuesReserva,res.fecha_clase),114) end  as 'classSchedule',
            res.fecha_clase as 'dateClass', res.cdreserva as 'reserveId', cla.nombre as 'className', isnull(res.megas_utilizadas,0) as 'utilizedMegas', isnull(res.tiq_utilizados,0) as 'utilizedTickets', 
            isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'employeeName', 
            res.intensidad as 'classIntensity', res.estado as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                            cli.cdgimnasio = pu.cdgimnasio)
                            inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                            inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                            inner join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
                            inner join gim_clases cla on (res.cdclase = cla.cdclase and res.cdgimnasio = cla.cdgimnasio)
                            inner join gim_horarios_clase hcla on (res.cdhorario_clase = hcla.cdhorario_clase and res.cdgimnasio = hcla.cdgimnasio and cla.cdclase = hcla.cdclase)
                            inner join gim_empleados emp on (hcla.profesor = emp.cdempleado and hcla.cdgimnasio = emp.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal) and
          res.estado != 'Anulada' and res.estado != 'Asistio' and convert(varchar(10),res.fecha_clase,111) = convert(varchar(10),getdate(),111)
    return;
end

go

if exists (select * from sys.objects where name = 'fnClientsWithVigentPlanAndReserveWithSignedContract' and type in (N'TF'))
begin
    drop function fnClientsWithVigentPlanAndReserveWithSignedContract
end
go

CREATE function [dbo].[fnClientsWithVigentPlanAndReserveWithSignedContract](@gymId int, @branchId int, @bitImprimirHoraReserva bit, @intMinutosAntesReserva int, @intMinutosDespuesReserva int)
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    
    select cli.cli_identifi as 'id', 
            isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', 
			case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
            --HUELLA DEL CLIENTE		
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
			cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
            --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
            --CÓDIGO DE GRUPO
            isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
            cast(1 as bit) as 'isRestrictionClass', 
            --HORARIO DE INGRESO SEGÚN RESERVA
            convert(varchar(10),dateadd(minute,-@intMinutosAntesReserva,res.fecha_clase),114) + '-' + convert(varchar(10),dateadd(minute,@intMinutosDespuesReserva,res.fecha_clase),114) as 'classSchedule',
            res.fecha_clase as 'dateClass', res.cdreserva as 'reserveId', cla.nombre as 'className', isnull(res.megas_utilizadas,0) as 'utilizedMegas', isnull(res.tiq_utilizados,0) as 'utilizedTickets', 
            isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'employeeName', 
            res.intensidad as 'classIntensity', res.estado as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                cli.cdgimnasio = pu.cdgimnasio)
                            inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                            inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                            inner join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
                            inner join gim_clases cla on (res.cdclase = cla.cdclase and res.cdgimnasio = cla.cdgimnasio)
                            inner join gim_horarios_clase hcla on (res.cdhorario_clase = hcla.cdhorario_clase and res.cdgimnasio = hcla.cdgimnasio and cla.cdclase = hcla.cdclase)
                            inner join gim_empleados emp on (hcla.profesor = emp.cdempleado and hcla.cdgimnasio = emp.cdgimnasio)
                            inner join gim_detalle_contrato dc on (dc.dtcont_doc_cliente = pu.plusu_identifi_cliente and dc.cdgimnasio = pu.cdgimnasio and dc.dtcont_numero_plan = pu.plusu_numero_fact and dc.dtcont_fkdia_codigo = pu.plusu_fkdia_codigo)
                            inner join gim_contrato c on (dc.dtcont_FKcontrato = c.cont_codigo and dc.cdgimnasio = c.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and c.int_fkTipoContrato = 1 and 
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo) and
          res.estado != 'Anulada' and res.estado != 'Asistio' and convert(varchar(10),res.fecha_clase,111) = convert(varchar(10),getdate(),111)


    union

    select cli.cli_identifi as 'id', 
            isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
            pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
            --ÚLTIMA ENTRADA
            null as 'lastEntry',
            --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
            pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
            --TIQUETES DISPONIBLES
            case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
            --HORARIOS INGRESO PLAN, POR DÍA.
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
            isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
            isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
            su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
            cast('Pendiente' as varchar(30)) as 'personState', 
			case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
            --HUELLA DEL CLIENTE		
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
            case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
			cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
            --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
            --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
            case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
            --CÓDIGO DE GRUPO
            isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
            cast(1 as bit) as 'isRestrictionClass', 
            --HORARIO DE INGRESO SEGÚN RESERVA
            convert(varchar(10),dateadd(minute,-@intMinutosAntesReserva,res.fecha_clase),114) + '-' + convert(varchar(10),dateadd(minute,@intMinutosDespuesReserva,res.fecha_clase),114) as 'classSchedule',
            res.fecha_clase as 'dateClass', res.cdreserva as 'reserveId', cla.nombre as 'className', isnull(res.megas_utilizadas,0) as 'utilizedMegas', isnull(res.tiq_utilizados,0) as 'utilizedTickets', 
            isnull(emp.emp_nombre,'') + ' ' + isnull(emp.emp_primer_apellido,'') + ' ' + isnull(emp.emp_segundo_apellido,'') as 'employeeName', 
            res.intensidad as 'classIntensity', res.estado as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                            cli.cdgimnasio = pu.cdgimnasio)
                            inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                            inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                            inner join gim_reservas res on (cli.cli_identifi = res.IdentificacionCliente and cli.cdgimnasio = res.cdgimnasio and pu.plusu_sucursal = res.cdsucursal)
                            inner join gim_clases cla on (res.cdclase = cla.cdclase and res.cdgimnasio = cla.cdgimnasio)
                            inner join gim_horarios_clase hcla on (res.cdhorario_clase = hcla.cdhorario_clase and res.cdgimnasio = hcla.cdgimnasio and cla.cdclase = hcla.cdclase)
                            inner join gim_empleados emp on (hcla.profesor = emp.cdempleado and hcla.cdgimnasio = emp.cdgimnasio)
                            inner join gim_detalle_contrato dc on (dc.dtcont_doc_cliente = pu.plusu_identifi_cliente and dc.cdgimnasio = pu.cdgimnasio and dc.dtcont_numero_plan = pu.plusu_numero_fact and dc.dtcont_fkdia_codigo = pu.plusu_fkdia_codigo)
                            inner join gim_contrato c on (dc.dtcont_FKcontrato = c.cont_codigo and dc.cdgimnasio = c.cdgimnasio)
    where pu.plusu_avisado = 0 and 
	convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) <= convert(varchar(10),getdate(),111) and c.int_fkTipoContrato = 1 and 
          pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal) and
          res.estado != 'Anulada' and res.estado != 'Asistio' and convert(varchar(10),res.fecha_clase,111) = convert(varchar(10),getdate(),111)
    return;
end

go

if exists (select * from sys.objects where name = 'fnClientsWithVigentPlanWithSignedContract' and type in (N'TF'))
begin
    drop function fnClientsWithVigentPlanWithSignedContract
end
go

CREATE function [dbo].[fnClientsWithVigentPlanWithSignedContract](@gymId int, @branchId int)
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    
    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
		   cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                               cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                          left join gim_detalle_contrato dc on (dc.dtcont_doc_cliente = pu.plusu_identifi_cliente and dc.cdgimnasio = pu.cdgimnasio and dc.dtcont_numero_plan = pu.plusu_numero_fact and dc.dtcont_fkdia_codigo = pu.plusu_fkdia_codigo)
                          left join gim_contrato c on (dc.dtcont_FKcontrato = c.cont_codigo and dc.cdgimnasio = c.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
		 -- and c.int_fkTipoContrato = 1 
		  and pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedInvoices where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal and dian = pu.plusu_fkdia_codigo)

    union

    select cli.cli_identifi as 'id', 
           isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
           pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
           --ÚLTIMA ENTRADA
           null as 'lastEntry',
           --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
           pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
           --TIQUETES DISPONIBLES
           case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
           --HORARIOS INGRESO PLAN, POR DÍA.
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
           isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
           isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
           su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
           cast('Pendiente' as varchar(30)) as 'personState', 
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
           --HUELLA DEL CLIENTE		
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
           case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
		   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
           cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
           --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1 grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
           --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
           case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1 grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
           --CÓDIGO DE GRUPO
           isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp on (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
           cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
            null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
            cast('' as varchar(200)) as 'employeeName', 
            cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
            pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
            isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                          cli.cdgimnasio = pu.cdgimnasio)
                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                          left join gim_detalle_contrato dc on (dc.dtcont_doc_cliente = pu.plusu_identifi_cliente and dc.cdgimnasio = pu.cdgimnasio and dc.dtcont_numero_plan = pu.plusu_numero_fact and dc.dtcont_fkdia_codigo = pu.plusu_fkdia_codigo)
                          left join gim_contrato c on (dc.dtcont_FKcontrato = c.cont_codigo and dc.cdgimnasio = c.cdgimnasio)
    where pu.plusu_avisado = 0 
	and convert(varchar(10),dateadd(day, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto),111) >= convert(varchar(10),getdate(),111) and
          convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
		  --and c.int_fkTipoContrato = 1 
		  and pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId and pu.plusu_sucursal = @branchId and
          pu.plusu_numero_fact not in (select invoiceId from vwSuspendedCourtesies where gymId = pu.cdgimnasio and branchId = pu.plusu_sucursal)
    return;
end

go

if exists (select * from sys.objects where name = 'fnIncludeClients' and type in (N'TF'))
begin
    drop function fnIncludeClients
end
go

create function fnIncludeClients(@gymId int, @branchId int, @clientId varchar(15))
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    --Parámetro para validar si los empleados ingresan sin plan.
    declare @IngresoEmpSinPlan bit
    --Parámetro para validar si hay bloqueo por firma de contrato "general"
    declare @bitValideContrato bit
    --Parámetro para validar si hay bloqueo por firma de contrato "por factura"
    declare @bitValideContPorFactura bit
    --Parámetro para validar si hay bloqueo por no asistir a una cita en MSW
    declare @bitBloqueoCitaNoCumplidaMSW bit
    --Parámetro para validar si hay bloqueo por ser no APTO en MSW.
    declare @bitBloqueoClienteNoApto bit
    --Parámetro para validar si hay bloqueo si no ha hecho disentimiento presencial.
    declare @bitBloqueoNoDisentimento bit
    --Parámetro para validar si hay bloqueo si no tiene Autorización menor de edad.
    declare @bitBloqueoNoAutorizacionMenor bit
    --Parámetro para validar MSW.
    declare @bitConsultaInfoCita bit
    --Parámetro para saber si se debe imprimir la hora de la reserva en el ticket.
    declare @bitImprimirHoraReserva bit
    declare @bitCambiarEstadoTiqueteClase bit
    --Parámetro que indica si se controla el acceso según las reservas de clases Web del cliente.
    declare @bitAccesoPorReservaWeb bit=0
    --Parámetro que indica si se validará el plan del cliente y la reserva web a la vez. Ambos escenarios dan/niegan acceso.
    declare @bitValidarPlanYReservaWeb bit=0
    --Parámetro para determinar el tiempo en minutos en que el cliente puede entrar antes de su próxima clase web reservada.
    declare @intMinutosAntesReserva int
    --Parámetro para determinar el tiempo en minutos en que el cliente puede entrar despues de su actual clase web reservada.
    declare @intMinutosDespuesReserva int
    declare @daysWithoutMedicalAppointment int
    declare @daysWithoutEntries int
    declare @fullAge int

    --Capturamos el parámetro para saber si los empleados ingresan sin plan.
    set @IngresoEmpSinPlan = isnull((select top 1 isnull(bitIngresoEmpSinPlan,1) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),1)
    --Capturamos el parámetro para saber si se valida la firma del contrato.
    set @bitValideContrato = isnull((select top 1 isnull(bitValideContrato,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si se valida la firma del contrato por factura.
    set @bitValideContPorFactura = isnull((select top 1 isnull(bitValideContratoPorFactura,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si los clientes ingresan siempre y cuando tengan plan y reserva de clase
    set @bitAccesoPorReservaWeb = isnull((select top 1 isnull(bitAccesoPorReservaWeb,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si los clientes ingresan ya sea por plan o por cortesía
    set @bitValidarPlanYReservaWeb = isnull((select top 1 isnull(bitValidarPlanYReservaWeb,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    set @intMinutosAntesReserva = isnull((select top 1 isnull(intMinutosAntesReserva,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    set @intMinutosDespuesReserva = isnull((select top 1 isnull(intMinutosDespuesReserva,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si se van a validar los datos de cita médica
    set @bitConsultaInfoCita = isnull((select top 1 isnull(bitConsultaInfoCita,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si el cliente no ha cumplido con una cita médica
    set @bitBloqueoCitaNoCumplidaMSW = isnull((select top 1 isnull(bitBloqueoCitaNoCumplidaMSW,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si el cliente no es apto para entrenar
    set @bitBloqueoClienteNoApto = isnull((select top 1 isnull(bitBloqueoClienteNoApto,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si el cliente no ha cumplido con el disentimiento presencial
    set @bitBloqueoNoDisentimento = isnull((select top 1 isnull(bitBloqueoNoDisentimento,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro para saber si el cliente es menor de edad
    set @bitBloqueoNoAutorizacionMenor = isnull((select top 1 isnull(bitBloqueoNoAutorizacionMenor,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro que determina cuantos días despues de pedir la cita puede asistir el cliente
    set @daysWithoutMedicalAppointment = isnull((select top 1 isnull(intdiassincita_bloqueoing,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro que determina cuantas entradas puede tener un cliente sin asistir a una cita
    set @daysWithoutEntries = isnull((select top 1 isnull(intentradas_sincita_bloqueoing,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
    --Capturamos el parámetro que determina cual es la mayoría de edad para el gimnasio
    set @fullAge = isnull((select top 1 isnull(intAnosMayoriaEdad,0) from tblConfiguracion where cdgimnasio = @gymId),0)
    
    --Captura de clientes para lista blanca
    if (@bitValideContrato = 1)
    begin
        if (@bitValideContPorFactura = 1)
        begin
            --Capturamos los clientes que tengan plan vigente y hayan firmado el contrato sobre la factura actual
            if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 0)
            begin
                insert @table
                select *
                from fnClientsWithVigentPlanWithSignedContract(@gymId,@branchId) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId
            end
            else if (@bitAccesoPorReservaWeb = 1 and @bitValidarPlanYReservaWeb = 0)
            begin
                insert @table
                select *
                from fnClientsWithVigentPlanAndReserveWithSignedContract(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId
            end
            else if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 1)
            begin
                insert @table
                select *
                from fnClientsWithVigentPlanAndNotReserveWithSignedContract(@gymId,@branchId) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId

                union

                select *
                from fnClientsWithVigentPlanAndReserveWithSignedContract(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId
            end
        end
        else
        begin
            --Capturamos los clientes que tengan plan vigente y hayan firmado el contrato en el gimnasio
            if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 0)
            begin
                insert @table
                select *
                from fnClientsWithVigentPlan(@gymId,@branchId) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1) and
                      cwvp.id in (SELECT dc.dtcont_doc_cliente
                                  FROM gim_detalle_contrato dc INNER JOIN gim_contrato c ON dc.dtcont_FKcontrato = c.cont_codigo 
                                                                  AND c.int_fkTipoContrato=1 and dc.cdgimnasio = c.cdgimnasio
                                  WHERE dc.cdgimnasio = @gymId)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId
            end
            else if (@bitAccesoPorReservaWeb = 1 and @bitValidarPlanYReservaWeb = 0)
            begin
                insert @table
                select *
                from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1) and
                      cwvp.id in (SELECT dc.dtcont_doc_cliente
                                  FROM gim_detalle_contrato dc INNER JOIN gim_contrato c ON dc.dtcont_FKcontrato = c.cont_codigo 
                                                                  AND c.int_fkTipoContrato=1 and dc.cdgimnasio = c.cdgimnasio
                                  WHERE dc.cdgimnasio = @gymId)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId
            end
            else if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 1)
            begin
                insert @table
                select *
                from fnClientsWithVigentPlanAndNotReserve(@gymId,@branchId) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1) and
                      cwvp.id in (SELECT dc.dtcont_doc_cliente
                                  FROM gim_detalle_contrato dc INNER JOIN gim_contrato c ON dc.dtcont_FKcontrato = c.cont_codigo 
                                                                  AND c.int_fkTipoContrato=1 and dc.cdgimnasio = c.cdgimnasio
                                  WHERE dc.cdgimnasio = @gymId)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId

                union

                select *
                from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                where cwvp.id not in (select cre_identifi 
                                      FROM dbo.gim_creditos 
                                      WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                            AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                      cwvp.id not in (select id 
                                      from gim_listanegra
                                      where cdgimnasio = @gymId and listneg_bitEstado = 1) and
                      cwvp.id in (SELECT dc.dtcont_doc_cliente
                                  FROM gim_detalle_contrato dc INNER JOIN gim_contrato c ON dc.dtcont_FKcontrato = c.cont_codigo 
                                                                  AND c.int_fkTipoContrato=1 and dc.cdgimnasio = c.cdgimnasio
                                  WHERE dc.cdgimnasio = @gymId)
                      and cwvp.id not in (select *
                                          from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                        @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                        @daysWithoutMedicalAppointment))
                      and cwvp.id = @clientId
            end
        end
    end
    else
    begin
        --Capturamos los clientes que tengan plan vigente
        if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 0)
        begin
            insert @table
            select *
            from fnClientsWithVigentPlan(@gymId,@branchId) as cwvp
            where cwvp.id not in (select cre_identifi 
                                  FROM dbo.gim_creditos 
                                  WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                        AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                  cwvp.id not in (select id 
                                  from gim_listanegra
                                  where cdgimnasio = @gymId and listneg_bitEstado = 1)
                  and cwvp.id not in (select *
                                      from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                    @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                    @daysWithoutMedicalAppointment))
                  and cwvp.id = @clientId
        end
        else if (@bitAccesoPorReservaWeb = 1 and @bitValidarPlanYReservaWeb = 0)
        begin
            insert @table
            select *
            from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
            where cwvp.id not in (select cre_identifi 
                                  FROM dbo.gim_creditos 
                                  WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                        AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                  cwvp.id not in (select id 
                                  from gim_listanegra
                                  where cdgimnasio = @gymId and listneg_bitEstado = 1)
                  and cwvp.id not in (select *
                                      from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                    @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                    @daysWithoutMedicalAppointment))
                  and cwvp.id = @clientId
        end
        else if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 1)
        begin
            insert @table
            select *
            from fnClientsWithVigentPlanAndNotReserve(@gymId,@branchId) as cwvp
            where cwvp.id not in (select cre_identifi 
                                  FROM dbo.gim_creditos 
                                  WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                        AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                  cwvp.id not in (select id 
                                  from gim_listanegra
                                  where cdgimnasio = @gymId and listneg_bitEstado = 1)
                  and cwvp.id not in (select *
                                      from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                    @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                    @daysWithoutMedicalAppointment))
                  and cwvp.id = @clientId

            union

            select *
            from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
            where cwvp.id not in (select cre_identifi 
                                  FROM dbo.gim_creditos 
                                  WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) 
                                        AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)) and
                  cwvp.id not in (select id 
                                  from gim_listanegra
                                  where cdgimnasio = @gymId and listneg_bitEstado = 1)
                  and cwvp.id not in (select *
                                      from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                                    @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                                    @daysWithoutMedicalAppointment))
                  and cwvp.id = @clientId
        end
    end

    return;
end
go

if exists (select * from sys.objects where name = 'fnIncludeProspects' and type in (N'TF'))
begin
    drop function fnIncludeProspects
end
go

create function fnIncludeProspects(@gymId int, @branchId int, @clientId varchar(15))
returns @table table
(
    id varchar(15),
    name varchar(max),
    planId int,
    planName varchar(max),
    expirationDate datetime,
    lastEntry datetime,
    planType varchar(10),
    typePerson varchar(100),
    availableEntries int,
    restrictions varchar(max),
    branchId int,
    branchName varchar(max),
    gymId int,
    personState varchar(100),
    withoutFingerprint bit,
    fingerprintId int,
    fingerprint binary(2000),
	strDatoFoto varchar(max),
    updateFingerprint bit,
    know bit,
    courtesy bit,
    groupEntriesControl bit,
    groupEntriesQuantity int,
    groupId int,
    isRestrictionClass bit,
    classSchedule varchar(max),
    dateClass datetime,
    reserveId int,
    className varchar(200),
    utilizedMegas int,
    utilizedTickets int,
    employeeName varchar(200),
    classIntensity varchar(200),
    classState varchar(100),
    photoPath varchar(max),
    invoiceId int,
    dianId int,
    documentType varchar(50),
    subgroupId int,
    cardId varchar(50)
)
as
begin
    insert @table
    --Capturamos los visitantes (prospectos) que serán guardados en la lista blanca.
    select id collate Modern_Spanish_CI_AS,name collate Modern_Spanish_CI_AS, planId,planName collate Modern_Spanish_CI_AS, expirationDate,lastEntry,
           planType collate Modern_Spanish_CI_AS,typePerson collate Modern_Spanish_CI_AS,
           availableEntries,restrictions collate Modern_Spanish_CI_AS, branchId, 
           branchName collate Modern_Spanish_CI_AS,gymId,personState collate Modern_Spanish_CI_AS,
           withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,courtesy,groupEntriesControl,groupEntriesQuantity,
           groupId,isRestrictionClass,classSchedule collate Modern_Spanish_CI_AS,dateClass,
           reserveId,className collate Modern_Spanish_CI_AS,utilizedMegas,
           utilizedTickets,employeeName collate Modern_Spanish_CI_AS,classIntensity collate Modern_Spanish_CI_AS,
           classState collate Modern_Spanish_CI_AS,photoPath collate Modern_Spanish_CI_AS,invoiceId,dianId,
           documentType collate Modern_Spanish_CI_AS, subgroupId, cardId collate Modern_Spanish_CI_AS
    from vwSpecialClient
    where gymId = @gymId and branchId = @branchId
          and id not in (select id 
                         from gim_listanegra
                         where cdgimnasio = @gymId and listneg_bitEstado = 1)
          and id not in (select cli_identifi from gim_clientes where cdgimnasio = @gymId)
          and id = @clientId
    union
    --Capturamos los visitantes (prospectos) que serán guardados en la lista blanca (con cortesía vigente).
    select id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
           branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
           courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
           classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
           employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
           subgroupId,cardId
    from vwSpecialClientWithCourtesy
    where gymId = @gymId and branchId = @branchId
          and id not in (select id 
                         from gim_listanegra
                         where cdgimnasio = @gymId and listneg_bitEstado = 1)
          and id not in (select cli_identifi from gim_clientes where cdgimnasio = @gymId)
          and id = @clientId

    return;
end
go


--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------FIN--------------------------------------------------------------------------------
----------------------------------------------------------------------FUNCIONES LISTA BLANCA -----------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------INICIO--------------------------------------------------------------------------------
-----------------------------------------------------------------------SP'S LISTA BLANCA ---------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGetPlanesByIdUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[spGetPlanesByIdUser] AS' 
END
GO

ALTER PROCEDURE [dbo].[spGetPlanesByIdUser]
	@idEmpresa INT = NULL,
	@idCliente VARCHAR(MAX) = NULL
AS
BEGIN

	SELECT		DISTINCT gim_planes.pla_codigo,
				gim_planes.pla_descripc,
				gim_planes_usuario.cdplusu,
				gim_planes_usuario.plusu_numero_fact,
				convert(varchar,gim_planes_usuario.plusu_fecha_inicio,103) as plusu_fecha_inicio,
				convert(varchar,gim_planes_usuario.plusu_fecha_vcto,103) as plusu_fecha_vcto
	FROM		gim_planes_usuario
	INNER JOIN	gim_planes_usuario_Detalle ON gim_planes_usuario_Detalle.Numero_de_factura = gim_planes_usuario.plusu_numero_fact
	AND			gim_planes_usuario_Detalle.cdgimnasio = gim_planes_usuario.cdgimnasio
	INNER JOIN	gim_planes ON gim_planes.pla_codigo = gim_planes_usuario_Detalle.Codigo_producto
	AND			gim_planes.cdgimnasio = gim_planes_usuario.cdgimnasio
	WHERE		gim_planes_usuario.plusu_identifi_cliente = @idCliente--'50008'
	AND			gim_planes_usuario.cdgimnasio = @idEmpresa--'136'
	AND			CAST(gim_planes_usuario.plusu_fecha_vcto AS DATE) >= CAST(GETDATE() AS DATE)

END
GO

if exists (select * from sys.objects where name = 'spGenerateWhiteList' and type in (N'P'))
begin
    drop procedure spGenerateWhiteList
end
go

CREATE procedure [dbo].[spGenerateWhiteList]
as
begin
	print('Actualiza con base a gim_planes_usuario ')
	select distinct cli_identifi,gim_clientes.cdgimnasio,cli_dias_gracia,cli_modificado_TG into #clientes from gim_planes_usuario
	inner join gim_clientes
	on plusu_identifi_cliente = cli_identifi
	where plusu_avisado =0 and plusu_est_anulada =0

    UPDATE c 
    SET c.cli_modificado_TG =1 
    FROM gim_planes_usuario f INNER JOIN gim_clientes c ON (c.cli_identifi = f.plusu_identifi_cliente AND c.cdgimnasio = f.cdgimnasio)
    WHERE f.plusu_avisado=0 AND convert(varchar(10),(dateadd(day, c.cli_dias_gracia, f.plusu_fecha_vcto)),111) <= convert(varchar(10),dateadd(day, -1, getdate()),111)

	print('Actualiza con base a gim_planes_usuario_especiales ')
    UPDATE c SET c.cli_modificado_TG =1 
    FROM gim_planes_usuario_especiales co INNER JOIN gim_clientes c ON (c.cli_identifi = co.plusu_identifi_cliente and c.cdgimnasio = co.cdgimnasio)
    WHERE co.plusu_avisado=0 AND  convert(varchar(10),(dateadd(day, c.cli_dias_gracia, co.plusu_fecha_vcto)),111) <= convert(varchar(10),dateadd(day, -1, getdate()),111)

	print('Actualiza con base a gim_planes_usuario ')
    UPDATE f 
    SET f.plusu_avisado = 1, f.plusu_modificado = 1, f.bits_replica = '111111111111111111111111111111'  
    FROM gim_planes_usuario f INNER JOIN gim_clientes c ON (c.cli_identifi = f.plusu_identifi_cliente and c.cdgimnasio = f.cdgimnasio)
    WHERE f.plusu_avisado=0	AND convert(varchar(10),(dateadd(day, c.cli_dias_gracia, f.plusu_fecha_vcto)),111) <= convert(varchar(10),dateadd(day, -1, getdate()),111)

	print('Actualiza con base a gim_planes_usuario_especiales ')
    UPDATE co 
    SET co.plusu_avisado = 1, co.plusu_modificado = 1, co.bits_replica = '111111111111111111111111111111'   
    FROM gim_planes_usuario_especiales co INNER JOIN gim_clientes c ON (c.cli_identifi = co.plusu_identifi_cliente and c.cdgimnasio = co.cdgimnasio)
    WHERE co.plusu_avisado=0 AND  convert(varchar(10),(dateadd(day, c.cli_dias_gracia, co.plusu_fecha_vcto)),111) <= convert(varchar(10),dateadd(day, -1, getdate()),111)

	print('Actualiza con base a gim_con_fac ')
    UPDATE gim_con_fac 
    SET des_con = 1, con_modificado = 1, bits_replica = '111111111111111111111111111111'   
    WHERE des_con = 0 AND fec_ter_con <= getdate()

	print('Actualiza con base a gim_con_fac_esp ')
    UPDATE gim_con_fac_esp 
    SET des_con = 1, con_esp_modificado = 1, bits_replica = '111111111111111111111111111111'  
    WHERE des_con = 0 AND fec_ter_con <= getdate()

	print('Actualiza con base a gim_planes_usuario ')
    UPDATE gim_planes_usuario 
    SET plusu_avisado = 1, plusu_modificado = 1, gim_planes_usuario.bits_replica = '111111111111111111111111111111'   
    FROM gim_planes INNER JOIN gim_planes_usuario ON (gim_planes.pla_codigo = plusu_codigo_plan and gim_planes.cdgimnasio = gim_planes_usuario.cdgimnasio)
    WHERE plusu_avisado=0 AND gim_planes.pla_tipo='T' AND plusu_tiq_disponible<=0;

	print('Actualiza con base a gim_planes_usuario_especiales ')
    UPDATE gim_planes_usuario_especiales 
    SET plusu_avisado = 1, plusu_modificado = 1, gim_planes_usuario_especiales.bits_replica = '111111111111111111111111111111'  
    FROM gim_planes INNER JOIN gim_planes_usuario_especiales ON (gim_planes.pla_codigo = plusu_codigo_plan and gim_planes.cdgimnasio = gim_planes_usuario_especiales.cdgimnasio)
    WHERE plusu_avisado=0 AND gim_planes.pla_tipo='T' AND plusu_tiq_disponible<=0;

	print('Actualiza con base a gim_clientes ')
    UPDATE gim_clientes 
    SET cli_bitAutorizacionM=0, cli_modificado = 1, bits_replica = '111111111111111111111111111111' 
    from gim_clientes
    WHERE cli_bitAutorizacionM=1
            AND (DATEDIFF(YY, cli_dtmFechaAutorizacionM, GETDATE()) - CASE WHEN DATEADD(YY, DATEDIFF(YY, cli_dtmFechaAutorizacionM, GETDATE()), cli_dtmFechaAutorizacionM) > GETDATE() THEN 1 ELSE 0 END) >= 1

	print('Truncamos la tabla lista blanca WhiteList')
    --Truncamos la tabla lista blanca
    truncate table WhiteList

    declare @gymId int
    declare @branchId int
    declare @auxGym int = 0
    declare @auxBranch int = 0
    declare @quantityBranch int

	print('Se busca la cantidad de gym que cumplen con la configuracion bitIngressWhiteList en true')
    declare @quantityGym int = (select count(*)
                                from tblConfiguracion
                                where cdgimnasio is not null and cdgimnasio != 0 and bitIngressWhiteList = 1)
print('Se crea un cursor')
    declare cursor_gym cursor for
        select cdgimnasio
        from tblConfiguracion
        where cdgimnasio is not null and cdgimnasio != 0 and bitIngressWhiteList = 1
    open cursor_gym

    while @auxGym < @quantityGym
    begin
        fetch next from cursor_gym
        into @gymId
        set @quantityBranch = (select count(*)
                               from gim_sucursales
                               where cdgimnasio = @gymId and suc_bitActiva = 1 and suc_intpkIdentificacion is not null and suc_intpkIdentificacion != 0)
        set @auxBranch = 0

        declare cursor_branch cursor for
            select suc_intpkIdentificacion
            from gim_sucursales
            where cdgimnasio = @gymId and suc_bitActiva = 1 and suc_intpkIdentificacion is not null and suc_intpkIdentificacion != 0
        open cursor_branch

        while @auxBranch < @quantityBranch
        begin
            fetch next from cursor_branch
            into @branchId
        
            /*Inicia SP*/
			print('Se declara variables')
            --Parámetro para validar si los empleados ingresan sin plan.
            declare @IngresoEmpSinPlan bit
            --Parámetro para validar si hay bloqueo por firma de contrato "general"
            declare @bitValideContrato bit
            --Parámetro para validar si hay bloqueo por firma de contrato "por factura"
            declare @bitValideContPorFactura bit
            --Parámetro para validar si hay bloqueo por no asistir a una cita en MSW
            declare @bitBloqueoCitaNoCumplidaMSW bit
            --Parámetro para validar si hay bloqueo por ser no APTO en MSW.
            declare @bitBloqueoClienteNoApto bit
            --Parámetro para validar si hay bloqueo si no ha hecho disentimiento presencial.
            declare @bitBloqueoNoDisentimento bit
            --Parámetro para validar si hay bloqueo si no tiene Autorización menor de edad.
            declare @bitBloqueoNoAutorizacionMenor bit
            --Parámetro para validar MSW.
            declare @bitConsultaInfoCita bit
            --Parámetro para saber si se debe imprimir la hora de la reserva en el ticket.
            declare @bitImprimirHoraReserva bit
            declare @bitCambiarEstadoTiqueteClase bit
            --Parámetro que indica si se controla el acceso según las reservas de clases Web del cliente.
            declare @bitAccesoPorReservaWeb bit=0
            --Parámetro que indica si se validará el plan del cliente y la reserva web a la vez. Ambos escenarios dan/niegan acceso.
            declare @bitValidarPlanYReservaWeb bit=0
            --Parámetro para determinar el tiempo en minutos en que el cliente puede entrar antes de su próxima clase web reservada.
            declare @intMinutosAntesReserva int
            --Parámetro para determinar el tiempo en minutos en que el cliente puede entrar despues de su actual clase web reservada.
            declare @intMinutosDespuesReserva int
            declare @daysWithoutMedicalAppointment int
            declare @daysWithoutEntries int
            declare @fullAge int

			print('Actualizamos el parámetro que indica que la lista blanca local se debe depurar. gim_configuracion_ingreso')
			--Actualizamos el parámetro que indica que la lista blanca local se debe depurar.
			update gim_configuracion_ingreso
			set bitResetLocalWhiteList = 1
			where cdgimnasio = @gymId and intfkSucursal = @branchId

			print('Creamos tablas temporales para clientes, empleados y restricciones adicionales (Relacionadas con MSW y parámetros del cliente "Apto, Alto Riesgo", Disentimiento)')
            --Creamos tablas temporales para clientes, empleados y restricciones adicionales (Relacionadas con MSW y parámetros del cliente "Apto, Alto Riesgo", Disentimiento)			
            if exists (select * from tempdb.dbo.sysobjects where name like '%#ClientsWithVigentPlan%')
            begin
                drop table #ClientsWithVigentPlan
            end

            create table #ClientsWithVigentPlan(
                id varchar(15),
                name varchar(max),
                planId int,
                planName varchar(max),
                expirationDate datetime,
                lastEntry datetime,
                planType varchar(10),
                typePerson varchar(100),
                availableEntries int,
                restrictions varchar(max),
                branchId int,
                branchName varchar(max),
                gymId int,
                personState varchar(100),
                withoutFingerprint bit,
                fingerprintId int,
                fingerprint binary(2000),
				strDatoFoto varchar(max),
                updateFingerprint bit,
                know bit,
                courtesy bit,
                groupEntriesControl bit,
                groupEntriesQuantity int,
                groupId int,
                isRestrictionClass bit,
                classSchedule varchar(max),
                dateClass datetime,
                reserveId int,
                className varchar(200),
                utilizedMegas int,
                utilizedTickets int,
                employeeName varchar(200),
                classIntensity varchar(200),
                classState varchar(100),
                photoPath varchar(max),
                invoiceId int,
                dianId int,
                documentType varchar(50),
                subgroupId int,
                cardId varchar(50)
            )

            if exists (select * from tempdb.dbo.sysobjects where name like '%#employees%')
            begin
                drop table #employees
            end

            create table #employees(
                id varchar(15),
                name varchar(max),
                planId int,
                planName varchar(max),
                expirationDate datetime,
                lastEntry datetime,
                planType varchar(10),
                typePerson varchar(100),
                availableEntries int,
                restrictions varchar(max),
                branchId int,
                branchName varchar(max),
                gymId int,
                personState varchar(100),
                withoutFingerprint bit,
                fingerprintId int,
                fingerprint binary(2000),
				strDatoFoto varchar(max),
                updateFingerprint bit,
                know bit,
                courtesy bit,
                groupEntriesControl bit,
                groupEntriesQuantity int,
                groupId int,
                isRestrictionClass bit,
                classSchedule varchar(max),
                dateClass datetime,
                reserveId int,
                className varchar(200),
                utilizedMegas int,
                utilizedTickets int,
                employeeName varchar(200),
                classIntensity varchar(200),
                classState varchar(100),
                photoPath varchar(max),
                invoiceId int,
                dianId int,
                documentType varchar(50),
                subgroupId int,
                cardId varchar(50)
            )

			print('Depuramos la lista blanca de la sucursal que se va a validar en el momento.')
            --Depuramos la lista blanca de la sucursal que se va a validar en el momento.
            --delete from WhiteList where gymId = @gymId and branchId = @branchId

            --Capturamos el parámetro para saber si los empleados ingresan sin plan.
            set @IngresoEmpSinPlan = isnull((select top 1 isnull(bitIngresoEmpSinPlan,1) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),1)
            --Capturamos el parámetro para saber si se valida la firma del contrato.
            set @bitValideContrato = isnull((select top 1 isnull(bitValideContrato,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            
			--Se agrega por el BM
			set @bitValideContrato=0;

			--Capturamos el parámetro para saber si se valida la firma del contrato por factura.
            set @bitValideContPorFactura = isnull((select top 1 isnull(bitValideContratoPorFactura,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si los clientes ingresan siempre y cuando tengan plan y reserva de clase
            set @bitAccesoPorReservaWeb = isnull((select top 1 isnull(bitAccesoPorReservaWeb,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si los clientes ingresan ya sea por plan o por cortesía
            set @bitValidarPlanYReservaWeb = isnull((select top 1 isnull(bitValidarPlanYReservaWeb,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            set @intMinutosAntesReserva = isnull((select top 1 isnull(intMinutosAntesReserva,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            set @intMinutosDespuesReserva = isnull((select top 1 isnull(intMinutosDespuesReserva,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si se van a validar los datos de cita médica
            set @bitConsultaInfoCita = isnull((select top 1 isnull(bitConsultaInfoCita,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si el cliente no ha cumplido con una cita médica
            set @bitBloqueoCitaNoCumplidaMSW = isnull((select top 1 isnull(bitBloqueoCitaNoCumplidaMSW,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si el cliente no es apto para entrenar
            set @bitBloqueoClienteNoApto = isnull((select top 1 isnull(bitBloqueoClienteNoApto,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si el cliente no ha cumplido con el disentimiento presencial
            set @bitBloqueoNoDisentimento = isnull((select top 1 isnull(bitBloqueoNoDisentimento,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro para saber si el cliente es menor de edad
            set @bitBloqueoNoAutorizacionMenor = isnull((select top 1 isnull(bitBloqueoNoAutorizacionMenor,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro que determina cuantos días despues de pedir la cita puede asistir el cliente
            set @daysWithoutMedicalAppointment = isnull((select top 1 isnull(intdiassincita_bloqueoing,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro que determina cuantas entradas puede tener un cliente sin asistir a una cita
            set @daysWithoutEntries = isnull((select top 1 isnull(intentradas_sincita_bloqueoing,0) from gim_configuracion_ingreso where cdgimnasio = @gymId and intfkSucursal = @branchId),0)
            --Capturamos el parámetro que determina cual es la mayoría de edad para el gimnasio
            set @fullAge = isnull((select top 1 isnull(intAnosMayoriaEdad,0) from tblConfiguracion where cdgimnasio = @gymId),0)

			print('Consultamos la lista negra del gimnasio')
            --Consultamos la lista negra del gimnasio
            select listneg_floatId as id
            into #blackList
            from gim_listanegra
            where cdgimnasio = @gymId and listneg_bitEstado = 1

			print('Captura los créditos pendientes')
            --Captura los créditos pendientes
            SELECT cre.cre_numero, dbo.fFloatAVarchar(cre.cre_identifi) as cre_identifi, cre.cre_factura, cre.cre_fecha, cre.cre_fecharealpago, cre.cre_valor, 
                   cre.cre_valor_abono, cre.cre_pagado, cre.cre_anulado, cre.cre_sucursal, cre.cre_modificado, cre.cre_sede, cre.cdgimnasio
            into #pendingCredits
            FROM dbo.gim_creditos cre 
            WHERE (convert(varchar(10),cre_fecha,111) < convert(varchar(10),getdate(),111)) AND (cre_anulado = 0) AND (cre_pagado = 0) and (cdgimnasio = @gymId)

			print('Capturamos los contratos firmados en el gimnasio')
            --Capturamos los contratos firmados en el gimnasio
            if (@bitValideContrato = 1)
            begin
                SELECT dc.* 
                into #signedContracts
                FROM gim_detalle_contrato dc INNER JOIN gim_contrato c ON dc.dtcont_FKcontrato = c.cont_codigo 
                                                                          AND c.int_fkTipoContrato=1 and dc.cdgimnasio = c.cdgimnasio
                WHERE dc.cdgimnasio = @gymId
            end

			print('Captura de clientes para lista blanca')
            --Captura de clientes para lista blanca
            if (@bitValideContrato = 1)
            begin
                if (@bitValideContPorFactura = 1)
                begin
				print('Capturamos los clientes que tengan plan vigente y hayan firmado el contrato sobre la factura actual')
                    --Capturamos los clientes que tengan plan vigente y hayan firmado el contrato sobre la factura actual
                    if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 0)
                    begin
                        insert into #ClientsWithVigentPlan
                        select *
                        from fnClientsWithVigentPlanWithSignedContract(@gymId,@branchId) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList)
                    end
                    else if (@bitAccesoPorReservaWeb = 1 and @bitValidarPlanYReservaWeb = 0)
                    begin
                        insert into #ClientsWithVigentPlan
                        select *
                        from fnClientsWithVigentPlanAndReserveWithSignedContract(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList)
                    end
                    else if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 1)
                    begin
                        insert into #ClientsWithVigentPlan
                        select *
                        from fnClientsWithVigentPlanAndNotReserveWithSignedContract(@gymId,@branchId) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList)

                        union

                        select *
                        from fnClientsWithVigentPlanAndReserveWithSignedContract(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList)
                    end
                end
                else
                begin
				print('Capturamos los clientes que tengan plan vigente y hayan firmado el contrato en el gimnasio')
                    --Capturamos los clientes que tengan plan vigente y hayan firmado el contrato en el gimnasio
                    if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 0)
                    begin
                        insert into #ClientsWithVigentPlan
                        select *
                        from fnClientsWithVigentPlan(@gymId,@branchId) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList) and
                              cwvp.id in (select dtcont_doc_cliente from #signedContracts where cdgimnasio = @gymId)
                    end
                    else if (@bitAccesoPorReservaWeb = 1 and @bitValidarPlanYReservaWeb = 0)
                    begin
                        insert into #ClientsWithVigentPlan
                        select *
                        from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList) and
                              cwvp.id in (select dtcont_doc_cliente from #signedContracts where cdgimnasio = @gymId)
                    end
                    else if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 1)
                    begin
                        insert into #ClientsWithVigentPlan
                        select *
                        from fnClientsWithVigentPlanAndNotReserve(@gymId,@branchId) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList) and
                              cwvp.id in (select dtcont_doc_cliente from #signedContracts where cdgimnasio = @gymId)

                        union

                        select *
                        from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                        where cwvp.id not in (select cre_identifi from #pendingCredits) and
                              cwvp.id not in (select id from #blackList) and
                              cwvp.id in (select dtcont_doc_cliente from #signedContracts where cdgimnasio = @gymId)
                    end
                end
            end
            else 
            begin
			print('Capturamos los clientes que tengan plan vigente')
                --Capturamos los clientes que tengan plan vigente
                if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 0)
                begin
                    insert into #ClientsWithVigentPlan
                    select *
                    from fnClientsWithVigentPlan(@gymId,@branchId) as cwvp
                    where cwvp.id not in (select cre_identifi from #pendingCredits) and
                          cwvp.id not in (select id from #blackList)
                end
                else if (@bitAccesoPorReservaWeb = 1 and @bitValidarPlanYReservaWeb = 0)
                begin
                    insert into #ClientsWithVigentPlan
                    select *
                    from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                    where cwvp.id not in (select cre_identifi from #pendingCredits) and
                          cwvp.id not in (select id from #blackList)
                end
                else if (@bitAccesoPorReservaWeb = 0 and @bitValidarPlanYReservaWeb = 1)
                begin
                    insert into #ClientsWithVigentPlan
                    select *
                    from fnClientsWithVigentPlanAndNotReserve(@gymId,@branchId) as cwvp
                    where cwvp.id not in (select cre_identifi from #pendingCredits) and
                          cwvp.id not in (select id from #blackList)

                    union

                    select *
                    from fnClientsWithVigentPlanAndReserve(@gymId,@branchId,@bitImprimirHoraReserva,@intMinutosAntesReserva,@intMinutosDespuesReserva) as cwvp
                    where cwvp.id not in (select cre_identifi from #pendingCredits) and
                          cwvp.id not in (select id from #blackList)
                end
            end

            select *
            into #clients
            from #ClientsWithVigentPlan
            where id not in (select id collate Modern_Spanish_CI_AS
                             from fnAdditionalRestrictions(@gymId, @bitConsultaInfoCita, @bitBloqueoCitaNoCumplidaMSW, @bitBloqueoClienteNoApto, 
                                                           @bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor, @fullAge, @daysWithoutEntries, 
                                                           @daysWithoutMedicalAppointment))
print('Capturamos los empleados que serán guardados en la lista blanca')
            --Capturamos los empleados que serán guardados en la lista blanca.
            if (@IngresoEmpSinPlan = 1)
            begin
                insert into #employees
                select *
                from vwEmployeesWithoutPlan 
                where gymId = @gymId
                      and branchId = @branchId
                      and id not in (select id from #blackList)	
            end
            else
            begin	
                insert into #employees
                select * from vwEmployeesWithPlan where gymId = @gymId and branchId = @branchId and id not in (select id from #blackList)
            end

			print('Capturamos los visitantes (prospectos) que serán guardados en la lista blanca')
            --Capturamos los visitantes (prospectos) que serán guardados en la lista blanca.
            select *
            into #SpecialClients
            from vwSpecialClient
            where gymId = @gymId and branchId = @branchId
                  and id not in (select id from #blackList)
                  and id not in (select cli_identifi from gim_clientes where cdgimnasio = @gymId)

		print('Capturamos los visitantes (prospectos) que serán guardados en la lista blanca (con cortesía vigente).')
            --Capturamos los visitantes (prospectos) que serán guardados en la lista blanca (con cortesía vigente).
            select *
            into #SpecialClientWithCourtesy
            from vwSpecialClientWithCourtesy
            where gymId = @gymId and branchId = @branchId
                  and id not in (select id from #blackList)
                  and id not in (select cli_identifi from gim_clientes where cdgimnasio = @gymId)

		   print('Capturamos los visitantes (visitas) que serán guardados en la lista blanca')
            --Capturamos los visitantes (visitas) que serán guardados en la lista blanca
            select *
            into #Visitors
            from vwVisitors
            where gymId = @gymId and branchId = @branchId
                  and id not in (select id from #blackList)
                
            select id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                   branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                   courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                   classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                   employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,subgroupId,cardId
            into #tmpWhiteList
            from #clients 
            union
            select id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                   branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                   courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                   classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                   employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,subgroupId,cardId 
            from #employees 
            union
            select id collate Modern_Spanish_CI_AS,name collate Modern_Spanish_CI_AS,planId,planName collate Modern_Spanish_CI_AS,expirationDate,lastEntry,
                   planType collate Modern_Spanish_CI_AS,typePerson collate Modern_Spanish_CI_AS,
                   availableEntries,restrictions collate Modern_Spanish_CI_AS, branchId, 
                   branchName collate Modern_Spanish_CI_AS,gymId,personState collate Modern_Spanish_CI_AS,
                   withoutFingerprint,fingerprintId,fingerprint,strDatoFoto collate Modern_Spanish_CI_AS,
				   updateFingerprint,know,courtesy,groupEntriesControl,groupEntriesQuantity,
                   groupId,isRestrictionClass,classSchedule collate Modern_Spanish_CI_AS,dateClass,
                   reserveId,className collate Modern_Spanish_CI_AS,utilizedMegas,
                   utilizedTickets,employeeName collate Modern_Spanish_CI_AS,classIntensity collate Modern_Spanish_CI_AS,
                   classState collate Modern_Spanish_CI_AS,photoPath collate Modern_Spanish_CI_AS,invoiceId,dianId,
                   documentType  collate Modern_Spanish_CI_AS,subgroupId,cardId collate Modern_Spanish_CI_AS
            from #SpecialClients
            union
            select id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                   branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                   courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                   classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                   employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,subgroupId,cardId
            from #SpecialClientWithCourtesy
            union
            select id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                   branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                   courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                   classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                   employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,subgroupId,cardId
            from #Visitors

			print('Hacemos la inserción en la lista blanca de los datos obtenidos en la sucursal actual WhiteList')
            --Hacemos la inserción en la lista blanca de los datos obtenidos en la sucursal actual
            insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                  subgroupId,cardId)
            select id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                  subgroupId,cardId
            from #tmpWhiteList

			print('Actualizamos el parámetro que indica que la lista blanca local se debe depurar. gim_configuracion_ingreso')
            --Actualizamos el parámetro que indica que la lista blanca local se debe depurar.
            update gim_configuracion_ingreso
            set bitResetLocalWhiteList = 1
            where cdgimnasio = @gymId and intfkSucursal = @branchId

            drop table #blackList
            drop table #pendingCredits
            if exists (select * from tempdb.dbo.sysobjects where name like '%#signedContracts%')
            begin
                drop table #signedContracts
            end
            drop table #ClientsWithVigentPlan
            drop table #clients
            drop table #employees
            drop table #SpecialClients
            drop table #SpecialClientWithCourtesy
            drop table #Visitors
            drop table #tmpWhiteList

            /*Finaliza SP*/

            set @auxBranch = @auxBranch + 1
        end

        close cursor_branch
        deallocate cursor_branch

        set @auxGym = @auxGym + 1
    end

    close cursor_gym
    deallocate cursor_gym
end

go

if exists (select * from sys.objects where name = 'spUpdatePlanRestrictions_WhiteList' and type in (N'P'))
begin
    drop procedure spUpdatePlanRestrictions_WhiteList
end
go

create procedure spUpdatePlanRestrictions_WhiteList
@gymId int = null,
@planId int = null
as
begin
    declare @id varchar(15)
    declare @invoiceId int
    declare @documentType varchar(50)
    declare @personType varchar(50)
    declare @branchId int
    declare @dianId int
    declare @IngresoEmpSinPlan bit
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from WhiteList
                                    where planId = @planId and gymId = @gymId),0)

    declare cursor_updateRestrictions cursor for
        select id,invoiceId,documentType,typePerson,branchId,dianId
        from WhiteList
        where planId = @planId and gymId = @gymId
    open cursor_updateRestrictions

    while @aux < @quantity
    begin
        fetch next from cursor_updateRestrictions
        into @id,@invoiceId,@documentType,@personType,@branchId,@dianId

        if (@personType = 'Empleado')
        begin
            set @IngresoEmpSinPlan = isnull((select top 1 isnull(bitIngresoEmpSinPlan,1) from gim_configuracion_ingreso where cdgimnasio = @gymId),1)

            if (@IngresoEmpSinPlan = 0)
            begin
                if (@documentType = 'Factura')
                begin
                    update WhiteList
                    set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ')
                                        from gim_empleados emp inner join gim_planes_usuario pu on (emp.cdgimnasio = pu.cdgimnasio and
                                                                                                    emp.emp_identifi = pu.plusu_identifi_cliente)
                                                               inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and
                                                                                             pu.cdgimnasio = pla.cdgimnasio)
                                                               inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and
                                                                                                pu.cdgimnasio = su.cdgimnasio)
                                        where pu.plusu_sucursal = @branchId and pu.plusu_fkdia_codigo = @dianId and pu.plusu_numero_fact = @invoiceId 
                                              and pu.plusu_identifi_cliente = @id and pu.cdgimnasio = @gymId), personState = 'Pendiente'
                    where id = @id and invoiceId = @invoiceId and documentType = @documentType and typePerson = 'Empleado'
                end
                else if (@documentType = 'Cortesía')
                begin
                    update WhiteList
                    set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +		
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ') + '| ' +
                                                     isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional)),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = emp.cdgimnasio)),' ')
                                        from gim_empleados emp inner join gim_planes_usuario_especiales pu on (emp.cdgimnasio = pu.cdgimnasio and
                                                                                                               emp.emp_identifi = pu.plusu_identifi_cliente)
                                                               inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and
                                                                                             pu.cdgimnasio = pla.cdgimnasio)
                                                               inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and
                                                                                                pu.cdgimnasio = su.cdgimnasio)
                                        where pu.plusu_sucursal = @branchId and pu.plusu_numero_fact = @invoiceId 
                                              and pu.plusu_identifi_cliente = @id and pu.cdgimnasio = @gymId), personState = 'Pendiente'
                    where id = @id and invoiceId = @invoiceId and documentType = @documentType and typePerson = 'Empleado'
                end
            end
        end
        else if (@personType = 'Cliente')
        begin
            if (@documentType = 'Factura')
            begin
                update WhiteList
                set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ')
                                    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                               cli.cdgimnasio = pu.cdgimnasio)
                                                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                                    where pu.plusu_sucursal = @branchId and pu.plusu_fkdia_codigo = @dianId and pu.plusu_numero_fact = @invoiceId 
                                          and pu.plusu_identifi_cliente = @id and pu.cdgimnasio = @gymId), personState = 'Pendiente'
                where id = @id and invoiceId = @invoiceId and documentType = @documentType and typePerson = 'Cliente'
            end
            else if (@documentType = 'Cortesía')
            begin
                update WhiteList
                set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ')
                                    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                                          cli.cdgimnasio = pu.cdgimnasio)
                                                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                                    where pu.plusu_sucursal = @branchId and pu.plusu_numero_fact = @invoiceId 
                                          and pu.plusu_identifi_cliente = @id and pu.cdgimnasio = @gymId), personState = 'Pendiente'
                where id = @id and invoiceId = @invoiceId and documentType = @documentType and typePerson = 'Cliente'
            end
        end
        else if (@personType = 'Prospecto')
        begin
            update WhiteList
            set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                                             isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ')
                                from gim_clientes_especiales cli inner join gim_planes_usuario_especiales pue on (dbo.fFloatAVarchar(cli.cli_identifi) = pue.plusu_identifi_cliente and
                                                                                                                    cli.cdgimnasio = pue.cdgimnasio)
                                                                    inner join gim_sucursales su on (pue.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                                    inner join gim_planes pla on (pue.plusu_codigo_plan = pla.pla_codigo and pue.cdgimnasio = pla.cdgimnasio)
                                where pue.plusu_sucursal = @branchId and pue.plusu_numero_fact = @invoiceId 
                                      and pue.plusu_identifi_cliente = @id and pue.cdgimnasio = @gymId), personState = 'Pendiente'
            where id = @id and invoiceId = @invoiceId and documentType = @documentType and typePerson = 'Prospecto'
        end

        set @aux = @aux + 1
    end

    close cursor_updateRestrictions
    deallocate cursor_updateRestrictions
end
go

if exists (select * from sys.objects where name = 'spUpdateSubgroupRestrictions_WhiteList' and type in (N'P'))
begin
    drop procedure spUpdateSubgroupRestrictions_WhiteList
end
go

create procedure spUpdateSubgroupRestrictions_WhiteList
@gymId int = null,
@subgroupId int = null
as
begin
    declare @id varchar(15)
    declare @documentType varchar(50)
    declare @personType varchar(50)
    declare @branchId int
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from WhiteList
                                    where subgroupId = @subgroupId and gymId = @gymId),0)

    if (@quantity > 0)
    begin
        declare cursor_updateRestrictionsSubgroup cursor for
            select id,documentType,typePerson,branchId
            from WhiteList
            where subgroupId = @subgroupId and gymId = @gymId
        open cursor_updateRestrictionsSubgroup

        while @aux < @quantity
        begin
            fetch next from cursor_updateRestrictionsSubgroup
            into @id,@documentType,@personType,@branchId

            if (@documentType = 'Factura')
            begin
                update WhiteList
                set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ')
                                    from gim_clientes cli inner join gim_planes_usuario pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                               cli.cdgimnasio = pu.cdgimnasio)
                                                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                                    where pu.plusu_sucursal = @branchId and pu.plusu_identifi_cliente = @id and pu.cdgimnasio = @gymId and cli.cli_intcodigo_subgrupo = @subgroupId)
                where id = @id and documentType = @documentType and typePerson = @personType and branchId = @branchId
            end
            else if (@documentType = 'Cortesía')
            begin
                update WhiteList
                set restrictions = (select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla on (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                 isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ')
                                    from gim_clientes cli inner join gim_planes_usuario_especiales pu on (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                                          cli.cdgimnasio = pu.cdgimnasio)
                                                          inner join gim_sucursales su on (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                          inner join gim_planes pla on (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                                    where pu.plusu_sucursal = @branchId and pu.plusu_identifi_cliente = @id and pu.cdgimnasio = @gymId and cli.cli_intcodigo_subgrupo = @subgroupId)
                where id = @id and documentType = @documentType and typePerson = @personType and branchId = @branchId
            end
			set @aux=@aux+1
        end
		close cursor_updateRestrictionsSubgroup
        deallocate cursor_updateRestrictionsSubgroup
    end
end
go

if exists (select * from sys.objects where name = 'spWhiteList' and type in (N'P'))
begin
    drop procedure spWhiteList
end
go

CREATE procedure [dbo].[spWhiteList]
@gymId int = null,
@branchId int = null
as
begin
    select *,(select PK_ClientCardId as clientCardId ,cli_identifi as clientId ,cardCode,  state from ClientCards where cdgimnasio = @gymId and cli_identifi = WhiteList.id for json path) as tblCardId
    from WhiteList
	where personState != 'Enviado' and gymId = @gymId and branchId = @branchId
end

go

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_GetMisReservasTiquetes]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SP_GetMisReservasTiquetes] AS' 
END
GO
ALTER PROCEDURE [dbo].[SP_GetMisReservasTiquetes] 
@cdusuario varchar(max) = NULL,
@cdgimnasio varchar(max) = NULL,
@intsucursal varchar(max) = NULL,
@resultadoApiReserva varchar(max) = NULL
AS
BEGIN

  SET NOCOUNT ON;

  DECLARE @intMinutosAntesReserva int = ISNULL((SELECT TOP 1
    ISNULL(intMinutosAntesReserva, 0)
  FROM gim_configuracion_ingreso
  WHERE cdgimnasio = @cdgimnasio
  AND intfkSucursal = @intsucursal)
  , 0)
  DECLARE @intMinutosDespuesReserva int = ISNULL((SELECT TOP 1
    ISNULL(intMinutosDespuesReserva, 0)
  FROM gim_configuracion_ingreso
  WHERE cdgimnasio = @cdgimnasio
  AND intfkSucursal = @intsucursal)
  , 0)

  IF (@resultadoApiReserva = '0')
  BEGIN

    SELECT
      reserva.IdentificacionCliente,
      cliente.cli_nombres,
      cliente.cli_primer_apellido,
      cliente.cli_segundo_apellido,
      clases.nombre AS clases_nombre,
      reserva.cdreserva,
      reserva.fecha_clase,
      reserva.estado,
      reserva.tiq_utilizados,
      reserva.cdgimnasio,
      reserva.cdplusu,
      reserva.tipo_Corte_Fact,
      reserva.tiq_vigentes_al_reservar,
      reserva.megas_utilizadas,
      reserva.ubi_reserva AS ubicacion,
      '' AS profesor,
      clases.cdclase,
      FORMAT(fecha_clase, 'dd/MM/yyyy') AS fecha_clase_impresion,
      FORMAT(fecha_clase, 'hh:mm:ss') AS hora_clase_impresion,
	  convert(varchar(8),horariosClase.hora,108) as horaDesde,
	  convert(varchar(8),horariosClase.hora_fin,108) as horaHasta
    FROM dbo.gim_reservas reserva
    JOIN gim_clientes cliente  ON reserva.IdentificacionCliente = cliente.cli_identifi
    JOIN gim_clases clases  ON reserva.cdclase = clases.cdclase
    JOIN gim_horarios_clase horariosClase  ON reserva.cdhorario_clase = horariosClase.cdhorario_clase
    WHERE cliente.cli_identifi = @cdusuario
    AND cliente.cdgimnasio = @cdgimnasio
    AND reserva.cdgimnasio = @cdgimnasio
    AND convert(varchar(8),GETDATE(),108) BETWEEN DATEADD(MINUTE, -@intMinutosAntesReserva, CONVERT(varchar(5), horariosClase.hora,108)) AND DATEADD(MINUTE, @intMinutosDespuesReserva, CONVERT(varchar(5), horariosClase.hora_fin,108))
	and convert(varchar(10),reserva.fecha_clase,120) between  CONVERT(varchar(10), horariosClase.fecha_inicio,120) AND  CONVERT(varchar(10), horariosClase.fecha_fin,120)
    AND reserva.cdsucursal = @intsucursal
    AND (UPPER(reserva.estado) = 'ACTIVA'
    OR UPPER(reserva.estado) = 'ACTIVO')
    ORDER BY reserva.cdreserva DESC
  END
  ELSE
  IF (@resultadoApiReserva != 'false')
  BEGIN
    SELECT
      reserva.IdentificacionCliente,
      cliente.cli_nombres,
      cliente.cli_primer_apellido,
      cliente.cli_segundo_apellido,
      clases.nombre AS clases_nombre,
      reserva.cdreserva,
      reserva.fecha_clase,
      reserva.estado,
      reserva.tiq_utilizados,
      reserva.cdgimnasio,
      reserva.cdplusu,
      reserva.tipo_Corte_Fact,
      reserva.tiq_vigentes_al_reservar,
      reserva.megas_utilizadas,
      reserva.ubi_reserva AS ubicacion,
      '' AS profesor,
      clases.cdclase,
      FORMAT(fecha_clase, 'dd/MM/yyyy') AS fecha_clase_impresion,
      FORMAT(fecha_clase, 'hh:mm:ss') AS hora_clase_impresion,
	  convert(varchar(8),horariosClase.hora,108) as horaDesde,
	  convert(varchar(8),horariosClase.hora_fin,108) as horaHasta  
    FROM dbo.gim_reservas reserva
	JOIN gim_clientes cliente ON reserva.IdentificacionCliente = cliente.cli_identifi
    JOIN gim_clases clases ON reserva.cdclase = clases.cdclase
    JOIN gim_horarios_clase horariosClase ON reserva.cdhorario_clase = horariosClase.cdhorario_clase
    WHERE cliente.cli_identifi = @cdusuario
    AND cliente.cdgimnasio = @cdgimnasio
    AND reserva.cdgimnasio = @cdgimnasio
	 AND convert(varchar(8),GETDATE(),108) BETWEEN DATEADD(MINUTE, -@intMinutosAntesReserva, CONVERT(varchar(5), horariosClase.hora,108)) AND DATEADD(MINUTE, @intMinutosDespuesReserva, CONVERT(varchar(5), horariosClase.hora_fin,108))
	and convert(varchar(10),reserva.fecha_clase,120) between  CONVERT(varchar(10), horariosClase.fecha_inicio,120) AND  CONVERT(varchar(10), horariosClase.fecha_fin,120)
     AND reserva.cdsucursal = @intsucursal
    AND reserva.cdreserva = @resultadoApiReserva
    ORDER BY reserva.cdreserva DESC
  END
END
GO

--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------FIN-----------------------------------------------------------------------------------
-----------------------------------------------------------------------SP'S LISTA BLANCA ---------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------


--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------INICIO--------------------------------------------------------------------------------
-----------------------------------------------------------------------TRIGGERS LISTA BLANCA -----------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Invoice' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_Invoice
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_Invoice] ON [dbo].[gim_planes_usuario]
after insert, update
as
begin
	--Incidente 0005670 MToro
	declare @bitAccesoPorReservaWeb bit = 0
	declare @bitValidarPlanYReservaWeb bit = 0
	--FIN Incidente 0005670 MToro
	declare @applyWhiteList bit
	declare @id varchar(15)
	declare @name varchar(max)
	declare @planId int
	declare @planName varchar(max)
	declare @expirationDate datetime
	declare @lastEntry datetime
	declare @planType varchar(10)
	declare @typePerson varchar(100)
	declare @availableEntries int
	declare @restrictions varchar(max)
	declare @branchId int
	declare @branchName varchar(max)
	declare @gymId int
	declare @personState varchar(100)
	declare @withoutFingerprint bit
	declare @fingerprintId int
	declare @fingerprint binary(2000)
	declare @strDatoFoto varchar(max)
	declare @updateFingerprint bit
	declare @know bit
	declare @courtesy bit
	declare @groupEntriesControl bit
	declare @groupEntriesQuantity int
	declare @groupId int
	declare @isRestrictionClass bit
	declare @classSchedule varchar(max)
	declare @dateClass datetime
	declare @reserveId int
	declare @className varchar(200)
	declare @utilizedMegas int
	declare @utilizedTickets int
	declare @employeeName varchar(200)
	declare @classIntensity varchar(200)
	declare @classState varchar(100)
	declare @photoPath varchar(max)
	declare @invoiceId int
	declare @dianId int
	declare @documentType varchar(50)
	declare @subgroupId int
	declare @cardId varchar(50)
	declare @anulledState bit
	declare @initialDate smalldatetime
	declare @finalDate smalldatetime
	declare @informedState bit
	declare @classAvailableTickets int
	declare @aux int = 0
	declare @isDeleted bit = 0
	declare @quantity int = isnull((select count(*) 
									from inserted
									where plusu_codigo_plan != '999'),0)

	set @applyWhiteList = isnull((select top 1 bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select top 1 cdgimnasio from inserted)),0)

	
	--Incidente 0005670
	set @bitAccesoPorReservaWeb = isnull(
	(
	select top 1 isnull(bitAccesoPorReservaWeb,0) from gim_configuracion_ingreso 
	where (cdgimnasio = (select top 1 cdgimnasio from inserted) and intfkSucursal = (select top 1 plusu_sucursal from inserted))
	),0)

	set @bitValidarPlanYReservaWeb = isnull(
	(
	select top 1 isnull(bitValidarPlanYReservaWeb,0) from gim_configuracion_ingreso 
	where (cdgimnasio = (select top 1 cdgimnasio from inserted) and intfkSucursal = (select top 1 plusu_sucursal from inserted))
	),0)
	--FIN Incidente 0005670 MToro


	if (@applyWhiteList = 1 and (@bitAccesoPorReservaWeb = 0 or @bitValidarPlanYReservaWeb = 1)) -- and agregado por MToro (Incidente 0005670)
	begin
		declare cursor_invoices cursor for
			select cli.cli_identifi as 'id', 
					isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
					pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',
					DATEADD(DAY, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
					--ÚLTIMA ENTRADA
					null as 'lastEntry',
					--(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
					pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
					--TIQUETES DISPONIBLES
					case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
					--HORARIOS INGRESO PLAN, POR DÍA.
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
					isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
					isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
					su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
					cast('Pendiente' as varchar(30)) as 'personState', 
					case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
					--HUELLA DEL CLIENTE		
					case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
					case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
					case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
					cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
					--CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
					case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1  grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
					--CANTIDAD DE ENTRADAS GRUPO FAMILIAR
					case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1  grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
					--CÓDIGO DE GRUPO
					isnull((select top 1  grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
					cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
					null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
					cast('' as varchar(200)) as 'employeeName', 
					cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
					pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
					pu.plusu_avisado, pu.plusu_fecha_inicio, pu.plusu_fecha_vcto, pu.plusu_est_anulada, pu.plusu_intcantidadtotaltiquetes,
					isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
			from gim_clientes cli inner join inserted pu ON (cli.cli_identifi = pu.plusu_identifi_cliente and
																		cli.cdgimnasio = pu.cdgimnasio)
									inner join gim_sucursales su ON (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
									inner join gim_planes pla ON (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
			where	pu.plusu_codigo_plan != 999
		open cursor_invoices

		while @aux < @quantity
		begin
			fetch next from cursor_invoices
			into @id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
					@branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
					@courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
					@classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
					@employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
					@informedState,@initialDate,@finalDate,@anulledState,@classAvailableTickets,@subgroupId,@cardId

			if not exists (select * from WhiteList where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura')
			begin
				if ((@informedState != 1) and (convert(varchar(10),@finalDate,111) >= convert(varchar(10),getdate(),111)) and
					(convert(varchar(10),@initialDate,111) <= convert(varchar(10),getdate(),111)) and (@anulledState != 1))
				begin
				
					if ((@planType = 'M') or (@planType = 'D') or (@planType = 'T' and @availableEntries > 0))
					begin
						insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
												branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
												courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
												classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
												employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
												subgroupId,cardId)
						select @id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
								@branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
								@courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
								@classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
								@employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
								@subgroupId,@cardId
					end
				end
			end
			else
			begin
				--EL REGISTRO DEL CLIENTE EXISTE EN LA LISTA BLANCA
				--Si anularon o avisaron una factura, esta se debe eliminar de la lista blanca
				
				if (@anulledState = 1 or @informedState = 1)
				begin
					update WhiteList
					set personState = 'Eliminar'
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'

					set @isDeleted = 1
				end
				--Si quitaron el avisado a una factura que estaba para eliminar, esta se vuelve a ingresar como pendiente
				else if (@informedState != 1 and @anulledState != 1 and (select top 1  personState from WhiteList where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura') != 'Pendiente' and @isDeleted = 0)
				begin
					update WhiteList
					set personState = 'Pendiente', expirationDate = @expirationDate, availableEntries = @availableEntries
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'
				end

				--Validamos que el plan sea tipo "Tiquetera" y en caso de no tener tiquetes no se debe permitir el ingreso al cliente, en caso contrario se debe habilitar el ingreso nuevamente.
				if (@planType = 'T' and @availableEntries <= 0)
				begin
					update WhiteList
					set personState = 'Eliminar'
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'

					set @isDeleted = 1
				end
				else if (@planType = 'T' and @availableEntries > 0 and @isDeleted = 0)
				begin
					update WhiteList
					set personState = 'Pendiente', expirationDate = @expirationDate, availableEntries = @availableEntries
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'
				end

				--Si cambiaron las fechas y la factura se venció, esta se debe eliminar de lista blanca
				if ((convert(varchar(10),@initialDate,111) < convert(varchar(10),getdate(),111)) and
					(convert(varchar(10),@finalDate,111) < convert(varchar(10),getdate(),111)))
				begin
					update WhiteList
					set personState = 'Eliminar'
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'

					set @isDeleted = 1
				end
				--Se cambiaron las fechas y la factura aún no ha iniciado la vigencia, esta se debe eliminar de lista blanca
				else if ((convert(varchar(10),@initialDate,111) > convert(varchar(10),getdate(),111)) and
							(convert(varchar(10),@finalDate,111) > convert(varchar(10),getdate(),111)))
				begin
					update WhiteList
					set personState = 'Eliminar'
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'

					set @isDeleted = 1
				end
				--Se cambiaron las fechas, la factura entra en vigencia y esta estaba eliminada, se vuelve a ingresar a la lista blanca
				else if ((convert(varchar(10),@initialDate,111) <= convert(varchar(10),getdate(),111)) and
							(convert(varchar(10),@finalDate,111) >= convert(varchar(10),getdate(),111)) and
							(select  top 1 personState from WhiteList where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura') = 'Eliminar' and
							@anulledState != 1 and @isDeleted = 0)
				begin
					update WhiteList
					set personState = 'Pendiente', expirationDate = @expirationDate, availableEntries = @availableEntries
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'
				end
				else
				begin
					update WhiteList
					set expirationDate = @expirationDate, availableEntries = @availableEntries
					where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'
				end
			end
            
			set @aux = @aux + 1
		end

		close cursor_invoices
		deallocate cursor_invoices
	end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Courtesy' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_Courtesy
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_Courtesy] ON [dbo].[gim_planes_usuario_especiales]
after insert, update
as
begin
	--Incidente 0005670
    declare @bitAccesoPorReservaWeb bit = 0
    declare @bitValidarPlanYReservaWeb bit = 0
	--FIN Incidente 0005670
    declare @applyWhiteList bit
    declare @id varchar(15)
    declare @name varchar(max)
    declare @planId int
    declare @planName varchar(max)
    declare @expirationDate datetime
    declare @lastEntry datetime
    declare @planType varchar(10)
    declare @typePerson varchar(100)
    declare @availableEntries int
    declare @restrictions varchar(max)
    declare @branchId int
    declare @branchName varchar(max)
    declare @gymId int
    declare @personState varchar(100)
    declare @withoutFingerprint bit
    declare @fingerprintId int
    declare @fingerprint binary(2000)
	declare @strDatoFoto varchar(max)
    declare @updateFingerprint bit
    declare @know bit
    declare @courtesy bit
    declare @groupEntriesControl bit
    declare @groupEntriesQuantity int
    declare @groupId int
    declare @isRestrictionClass bit
    declare @classSchedule varchar(max)
    declare @dateClass datetime
    declare @reserveId int
    declare @className varchar(200)
    declare @utilizedMegas int
    declare @utilizedTickets int
    declare @employeeName varchar(200)
    declare @classIntensity varchar(200)
    declare @classState varchar(100)
    declare @photoPath varchar(max)
    declare @invoiceId int
    declare @dianId int
    declare @documentType varchar(50)
    declare @subgroupId int
    declare @cardId varchar(50)
    declare @anulledState bit
    declare @initialDate smalldatetime
    declare @finalDate smalldatetime
    declare @informedState bit
    declare @classAvailableTickets int
    declare @aux int = 0
    declare @isDeleted bit = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted pu
                                    where pu.plusu_codigo_plan != '999'),0)

    set @applyWhiteList = isnull((select top 1 bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select top 1 cdgimnasio from inserted)),0)


		--Incidente 0005670
    set @bitAccesoPorReservaWeb = isnull(
	(
	select isnull(bitAccesoPorReservaWeb,0) from gim_configuracion_ingreso 
	where (cdgimnasio = (select top 1 cdgimnasio from inserted) and intfkSucursal = (select top 1 plusu_sucursal from inserted))
	),0)

	set @bitValidarPlanYReservaWeb = isnull(
	(
	select isnull(bitValidarPlanYReservaWeb,0) from gim_configuracion_ingreso 
	where (cdgimnasio = (select top 1 cdgimnasio from inserted) and intfkSucursal = (select top 1 plusu_sucursal from inserted))
	),0)
	--FIN Incidente 0005670 MToro


    if (@applyWhiteList = 1 and (@bitAccesoPorReservaWeb = 0 or @bitValidarPlanYReservaWeb = 1)) -- and agregado por MToro (Incidente 0005670)
    begin
        declare cursor_courtesies cursor for
            select cli.cli_identifi as 'id', 
                   isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
                   pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',
				   DATEADD(DAY, isnull(cli.cli_dias_gracia,0), pu.plusu_fecha_vcto) as 'expirationDate',
                   --ÚLTIMA ENTRADA
                   null as 'lastEntry',
                   --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
                   pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
                   --TIQUETES DISPONIBLES
                   case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
                   --HORARIOS INGRESO PLAN, POR DÍA.
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                   isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                   isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictionPlan',
                   su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
                   cast('Pendiente' as varchar(30)) as 'personState', 
				   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerPrint',
                   --HUELLA DEL CLIENTE		
                   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerPrintId',
                   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerPrint',
                   case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
				   cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                   --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
                   case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select top 1  grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
                   --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
                   case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select top 1  grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
                   --CÓDIGO DE GRUPO
                   isnull((select top 1 grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
                   cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
                    null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
                    cast('' as varchar(200)) as 'employeeName', 
                    cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
                    pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
                    pu.plusu_avisado, pu.plusu_fecha_inicio, pu.plusu_fecha_vcto, pu.plusu_est_anulada, pu.plusu_intcantidadtotaltiquetes,
                    isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
            from gim_clientes cli inner join inserted pu ON (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                  cli.cdgimnasio = pu.cdgimnasio)
                                  inner join gim_sucursales su ON (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                  inner join gim_planes pla ON (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
            where pu.plusu_codigo_plan != 999
            union
            select dbo.fFloatAVarchar(cli.cli_identifi) as 'id', 
                    isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
                    pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',pue.plusu_fecha_vcto as 'expirationDate',
                    --ÚLTIMA ENTRADA
                    null as 'lastEntry',
                    --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
                    pla.pla_tipo as 'planType', cast('Prospecto' as varchar(100)) as 'typePerson',
                    --TIQUETES DISPONIBLES
                    case when (pla.pla_tipo = 'T') then pue.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pue.plusu_fecha_vcto,111)) end as 'availableEntries',
                    --HORARIOS INGRESO PLAN, POR DÍA.
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') as 'restrictions',
                    su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
                    cast('Pendiente' as varchar(30)) as 'personState', case when (cli.cli_EntryFingerprint = 1) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
                    --HUELLA DEL EMPLEADO		
                    case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
                    case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
                    case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
					cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                    cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
                   cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
                    null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
                    cast('' as varchar(200)) as 'employeeName', 
                    cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
                    pue.plusu_numero_fact as 'invoiceId', pue.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
                    pue.plusu_avisado, pue.plusu_fecha_inicio, pue.plusu_fecha_vcto, pue.plusu_est_anulada, pue.plusu_intcantidadtotaltiquetes,
                    cast(0 as int) as 'subgroupId', cast('' as varchar(50)) as 'cardId'
            from gim_clientes_especiales cli inner join inserted pue ON (dbo.fFloatAVarchar(cli.cli_identifi) = pue.plusu_identifi_cliente and
                                                                                              cli.cdgimnasio = pue.cdgimnasio)
                                             inner join gim_sucursales su ON (pue.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                             inner join gim_planes pla ON (pue.plusu_codigo_plan = pla.pla_codigo and pue.cdgimnasio = pla.cdgimnasio)
            where pue.plusu_codigo_plan != 999
        open cursor_courtesies

        while @aux < @quantity
        begin
            fetch next from cursor_courtesies
            into @id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
                 @branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
                 @courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
                 @classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
                 @employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
                 @informedState,@initialDate,@finalDate,@anulledState,@classAvailableTickets,@subgroupId,@cardId

            if not exists (select * from WhiteList where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía')
            begin
                if ((@informedState != 1) and (convert(varchar(10),@finalDate,111) >= convert(varchar(10),getdate(),111)) and
                   (convert(varchar(10),@initialDate,111) <= convert(varchar(10),getdate(),111)) and (@anulledState != 1))
                begin
                    if ((@planType = 'M') or (@planType = 'D') or (@planType = 'T' and @availableEntries > 0))
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,fingerprintId,withoutFingerprint,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select @id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
                               @branchId,@branchName,@gymId,@personState,@fingerprintId,@withoutFingerprint,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
                               @courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
                               @classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
                               @employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
                               @subgroupId,@cardId
                    end
                end
            end
            else
            begin
                --EL REGISTRO DEL CLIENTE EXISTE EN LA LISTA BLANCA
                --Si anularon o avisaron una factura, esta se debe eliminar de la lista blanca
                if (@anulledState = 1 or @informedState = 1)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía'

                    set @isDeleted = 1
                end
                --Si quitaron el avisado a una factura que estaba para eliminar, esta se vuelve a ingresar como pendiente
                else if (@informedState != 1 and @anulledState != 1 and (select top 1  personState from WhiteList where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía') != 'Pendiente' and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente', expirationDate = @expirationDate, availableEntries = @availableEntries
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía'
                end

                --Validamos que el plan sea tipo "Tiquetera" y en caso de no tener tiquetes no se debe permitir el ingreso al cliente, en caso contrario se debe habilitar el ingreso nuevamente.
                if (@planType = 'T' and @availableEntries <= 0)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'

                    set @isDeleted = 1
                end
                else if (@planType = 'T' and @availableEntries > 0 and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente', expirationDate = @expirationDate, availableEntries = @availableEntries
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Factura'
                end

                --Si cambiaron las fechas y la factura se venció, esta se debe eliminar de lista blanca
                if ((convert(varchar(10),@initialDate,111) < convert(varchar(10),getdate(),111)) and
                    (convert(varchar(10),@finalDate,111) < convert(varchar(10),getdate(),111)))
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía'

                    set @isDeleted = 1
                end
                --Se cambiaron las fechas y la factura aún no ha iniciado la vigencia, esta se debe eliminar de lista blanca
                else if ((convert(varchar(10),@initialDate,111) > convert(varchar(10),getdate(),111)) and
                         (convert(varchar(10),@finalDate,111) > convert(varchar(10),getdate(),111)))
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía'

                    set @isDeleted = 1
                end
                --Se cambiaron las fechas, la factura entra en vigencia y esta estaba eliminada, se vuelve a ingresar a la lista blanca
                else if ((convert(varchar(10),@initialDate,111) <= convert(varchar(10),getdate(),111)) and
                         (convert(varchar(10),@finalDate,111) >= convert(varchar(10),getdate(),111)) and
                         (select top 1 personState from WhiteList where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía') = 'Eliminar' and
                         @anulledState != 1 and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente', expirationDate = @expirationDate, availableEntries = @availableEntries
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía'
                end
				else
				begin
					update WhiteList
                    set expirationDate = @expirationDate, availableEntries = @availableEntries
                    where id = @id and gymId = @gymId and invoiceId = @invoiceId and dianId = @dianId and branchId = @branchId and documentType = 'Cortesía'
                end
            end

            set @aux = @aux + 1
        end

        close cursor_courtesies
        deallocate cursor_courtesies
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Visit' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_Visit
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_Visit] ON Visit
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @id varchar(15)
    declare @name varchar(max)
    declare @planId int
    declare @planName varchar(max)
    declare @expirationDate datetime
    declare @lastEntry datetime
    declare @planType varchar(10)
    declare @typePerson varchar(100)
    declare @availableEntries int
    declare @restrictions varchar(max)
    declare @branchId int
    declare @branchName varchar(max)
    declare @gymId int
    declare @personState varchar(100)
    declare @withoutFingerprint bit
    declare @fingerprintId int
    declare @fingerprint binary(2000)
	declare @strDatoFoto varchar(max)
    declare @updateFingerprint bit
    declare @know bit
    declare @courtesy bit
    declare @groupEntriesControl bit
    declare @groupEntriesQuantity int
    declare @groupId int
    declare @isRestrictionClass bit
    declare @classSchedule varchar(max)
    declare @dateClass datetime
    declare @reserveId int
    declare @className varchar(200)
    declare @utilizedMegas int
    declare @utilizedTickets int
    declare @employeeName varchar(200)
    declare @classIntensity varchar(200)
    declare @classState varchar(100)
    declare @photoPath varchar(max)
    declare @invoiceId int
    declare @dianId int
    declare @documentType varchar(50)
    declare @subgroupId int
    declare @cardId varchar(50)
    declare @DateFinalizeVisit datetime
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted 
                                    where convert(varchar(10),DateVisit,111) = convert(varchar(10),getdate(),111)),0)

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_visit cursor for
            select vis.vis_strVisitorId as 'id', 
                    isnull(vis.vis_strName,'') + ' ' + isnull(vis.vis_strFirstLastName,'') + ' ' + isnull(vis.vis_strSecondLastName,'') as 'name',
                    cast(0 as int) as 'planId', cast('' as varchar(max)) as 'planName',null as 'expirationDate',
                    --ÚLTIMA ENTRADA
                    null as 'lastEntry',
                    cast('' as varchar(10)) as 'planType', cast('Visitante' as varchar(100)) as 'typePerson',
                    --TIQUETES DISPONIBLES
                    cast(1 as int) as 'availableEntries',
                    --HORARIOS INGRESO PLAN, POR DÍA.
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) + '| ' +
                    cast(' ' as varchar(max)) as 'restrictions',
                    su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', vis.cdgimnasio as 'gymId',
                    cast('Pendiente' as varchar(30)) as 'personState', case when (vis.vis_EntryFingerprint = 0 or vis.vis_EntryFingerprint is null) then cast(1 as bit) else cast(0 as bit) end as 'withoutFingerprint',
                    --HUELLA DEL EMPLEADO		
                    case when (vis.vis_EntryFingerprint = 1) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = vis.vis_strVisitorId and hu.cdgimnasio = vis.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
                    case when (vis.vis_EntryFingerprint = 1) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = vis.vis_strVisitorId and hu.cdgimnasio = vis.cdgimnasio) else null end as 'fingerprint',
                    case when (vis.vis_EntryFingerprint = 1) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = vis.vis_strVisitorId and hu.cdgimnasio = vis.cdgimnasio) else null end as 'strDatoFoto',
					cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                    cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
                    cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
                    null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
                    cast('' as varchar(200)) as 'employeeName', 
                    cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath',
                    cast(0 as int) as 'invoiceId', cast(0 as int) as 'dianId', cast('' as varchar(50)) as 'documentType',v.DateFinalizeVisit,
                    cast(0 as int) as 'subgroupId', cast('' as varchar(50)) as 'cardId'
            from Visitors vis inner join gim_sucursales su ON (vis.vis_intBranch = su.suc_intpkIdentificacion and vis.cdgimnasio = su.cdgimnasio)
                              inner join inserted v ON (vis.cdgimnasio = v.cdgimnasio and vis.vis_strVisitorId = v.VisitorId and vis.vis_intBranch = v.Branch)
            where convert(varchar(10),v.DateVisit,111) = convert(varchar(10),getdate(),111)
        open cursor_visit

        while @aux < @quantity
        begin
            fetch next from cursor_visit
            into @id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
                 @branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
                 @courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
                 @classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
                 @employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
                 @DateFinalizeVisit,@subgroupId,@cardId

            if not exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Visitante')
            begin
                insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                      branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                      courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                      classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                      employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                      subgroupId,cardId)
                select @id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
                       @branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
                       @courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
                       @classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
                       @employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
                       @subgroupId,@cardId
            end
            else
            begin
                if (@DateFinalizeVisit is not null)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and typePerson = 'Visitante'
                end 
            end

            set @aux = @aux + 1
        end

        close cursor_visit
        deallocate cursor_visit
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Visitors' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_Visitors
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_Visitors] ON Visitors
after update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(50)
    declare @entryFingerprint bit
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_visitors cursor for
            select cdgimnasio, vis_strVisitorId, vis_EntryFingerprint from inserted
        open cursor_visitors

        while @aux < @quantity
        begin
            fetch next from cursor_visitors
            into @gymId,@id,@entryFingerprint

            if exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Visitante')
            begin
                if (@entryFingerprint = 1 and (select withoutFingerprint from WhiteList where id = @id and gymId = @gymId and typePerson = 'Visitante') = 1)
                begin
                    update WhiteList
                    set withoutFingerprint = 0, fingerprint = (select top 1 hue_dato from gim_huellas where hue_identifi = @id and cdgimnasio = @gymId), personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Visitante'
                end
                else if (@entryFingerprint = 0 and (select withoutFingerprint from WhiteList where id = @id and gymId = @gymId and typePerson = 'Visitante') = 0)
                begin
                    update WhiteList
                    set withoutFingerprint = 1, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Visitante'
                end
            end
            
            set @aux = @aux + 1
        end

        close cursor_visitors
        deallocate cursor_visitors
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_Fingerprint' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_Fingerprint
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_Fingerprint] ON gim_huellas
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(15)
	declare @fingerprintId int
    declare @fingerprint binary(2000)
	declare @strDatoFoto varchar(max)
    declare @quantity int = isnull((select count(*) from inserted),0)
    declare @aux int = 0

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select top 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_fingerprint cursor for
            select cdgimnasio, hue_identifi, hue_dato, hue_id, strDatoFoto from inserted
        open cursor_fingerprint

        while @aux < @quantity
        begin
            fetch next from cursor_fingerprint
            into @gymId, @id, @fingerprint, @fingerprintId,@strDatoFoto

            if exists (select * from WhiteList where id = @id and gymId = @gymId)
            begin
                update WhiteList
                set fingerprint = @fingerprint, fingerprintId = @fingerprintId,strDatoFoto=@strDatoFoto, personState = 'Pendiente'
                where id = @id and gymId = @gymId
            end

            set @aux = @aux + 1
        end

        close cursor_fingerprint
        deallocate cursor_fingerprint
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_suspendedInvoices' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_suspendedInvoices
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_suspendedInvoices] ON gim_con_fac
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @invoiceId int
    declare @initialDate datetime
    declare @finalDate datetime
    declare @noSuspend bit
    declare @branchId int
    declare @dianId int
    declare @gymId int
    declare @aux int = 0
    declare @isDeleted bit = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    declare @informedState bit
    declare @finalDateInvoice datetime
    declare @initialDateInvoice datetime
    declare @anulledState bit

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select top 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_suspendedInvoices cursor for
            select num_fac_con,
			fec_ini_con,
			fec_ter_con,
			des_con, con_sucursal, con_fkdia_codigo, cdgimnasio
            from inserted
        open cursor_suspendedInvoices

        while @aux < @quantity
        begin
            fetch next from cursor_suspendedInvoices
            into @invoiceId,@initialDate,@finalDate,@noSuspend,@branchId,@dianId,@gymId

            --Esta condición se ejecuta cuando se está insertando la congelación (congelando)
            if (@noSuspend != 1)
            begin
                if exists (select * from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId and documentType = 'Factura')
                begin
                    if (convert(varchar(10),getdate(),111) between convert(varchar(10),@initialDate,111) and convert(varchar(10),@finalDate,111))
                    begin
                        update WhiteList
                        set personState = 'Eliminar'
                        where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId

                        set @isDeleted = 1
                    end
                end
            end
            --Esta condición se ejecuta cuando se está actualizando la congelación (descongelando)
            else
            begin
                --Esta condición se ejecuta cuando el registro al ser descongelado y aplicar para entrar, no se encuentra en la lista blanca
                if not exists (select * from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId and documentType = 'Factura')
                begin
                    select * 
                    into #tmpWhiteListInvoiceBySuspend
                    from gim_planes_usuario 
                    where cdgimnasio = @gymId and plusu_numero_fact = @invoiceId and plusu_sucursal = @branchId and plusu_fkdia_codigo = @dianId
                    
                    set @informedState = (select plusu_avisado from #tmpWhiteListInvoiceBySuspend)
                    set @anulledState = (select plusu_est_anulada from #tmpWhiteListInvoiceBySuspend)
                    set @initialDateInvoice = (select plusu_fecha_inicio from #tmpWhiteListInvoiceBySuspend)
                    set @finalDateInvoice = (select plusu_fecha_vcto from #tmpWhiteListInvoiceBySuspend)

                    if ((@informedState != 1) and (convert(varchar(10),@finalDateInvoice,111) >= convert(varchar(10),getdate(),111)) and
                       (convert(varchar(10),@initialDateInvoice,111) <= convert(varchar(10),getdate(),111)) and (@anulledState != 1))
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select cli.cli_identifi as 'id', 
                                isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
                                pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',pu.plusu_fecha_vcto as 'expirationDate',
                                --ÚLTIMA ENTRADA
                                null as 'lastEntry',
                                --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
                                pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
                                --TIQUETES DISPONIBLES
                                case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
                                --HORARIOS INGRESO PLAN, POR DÍA.
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
                                su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
                                cast('Pendiente' as varchar(30)) as 'personState', 
								case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
                                --HUELLA DEL CLIENTE		
                                case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
                                case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
                                case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
								cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                                --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
                                case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
                                --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
                                case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
                                --CÓDIGO DE GRUPO
                                isnull((select grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
                                cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
                                null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
                                cast('' as varchar(200)) as 'employeeName', 
                                cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
                                pu.plusu_numero_fact as 'invoiceId', pu.plusu_fkdia_codigo as 'dianId', cast('Factura' as varchar(50)) as 'documentType',
                                isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId' 
                        from gim_clientes cli inner join #tmpWhiteListInvoiceBySuspend pu ON (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                              cli.cdgimnasio = pu.cdgimnasio)
                                                inner join gim_sucursales su ON (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                inner join gim_planes pla ON (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                    end

                    drop table #tmpWhiteListInvoiceBySuspend
                end
                --Esta condición se ejecuta cuando el registro al ser descongelado y aplicar para entrar, se encuentra en la lista blanca con estado "eliminado"
                else if (@isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId and documentType = 'Factura'
                end
            end

            set @aux = @aux + 1
        end

        close cursor_suspendedInvoices
        deallocate cursor_suspendedInvoices
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_suspendedCourtesies' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_suspendedCourtesies
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_suspendedCourtesies] ON gim_con_fac_esp
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @invoiceId int
    declare @initialDate datetime
    declare @finalDate datetime
    declare @noSuspend bit
    declare @branchId int
    declare @gymId int
    declare @aux int = 0
    declare @isDeleted bit = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    declare @informedState bit
    declare @finalDateInvoice datetime
    declare @initialDateInvoice datetime
    declare @anulledState bit
    declare @clientId varchar(15)
    declare @isClient bit

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select top 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_suspendedCourtesies cursor for
            select num_fac_con, fec_ini_con, fec_ter_con, des_con, con_intfkSucursal, cdgimnasio
            from inserted
        open cursor_suspendedCourtesies

        while @aux < @quantity
        begin
            fetch next from cursor_suspendedCourtesies
            into @invoiceId,@initialDate,@finalDate,@noSuspend,@branchId,@gymId

            --Esta condición se ejecuta cuando se está insertando la congelación (congelando)
            if (@noSuspend != 1)
            begin
                if exists (select * from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and documentType = 'Cortesía')
                begin
                    if (convert(varchar(10),getdate(),111) between convert(varchar(10),@initialDate,111) and convert(varchar(10),@finalDate,111))
                    begin
                        update WhiteList
                        set personState = 'Eliminar'
                        where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and documentType = 'Cortesía'

                        set @isDeleted = 1
                    end
                end
            end
            --Esta condición se ejecuta cuando se está actualizando la congelación (descongelando)
            else
            begin
                if not exists (select * from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and documentType = 'Cortesía')
                begin
                    select * 
                    into #tmpWhiteListCourtesyBySuspend
                    from gim_planes_usuario_especiales 
                    where cdgimnasio = @gymId and plusu_numero_fact = @invoiceId and plusu_sucursal = @branchId
                    
                    set @informedState = (select plusu_avisado from #tmpWhiteListCourtesyBySuspend)
                    set @anulledState = (select plusu_est_anulada from #tmpWhiteListCourtesyBySuspend)
                    set @initialDateInvoice = (select plusu_fecha_inicio from #tmpWhiteListCourtesyBySuspend)
                    set @finalDateInvoice = (select plusu_fecha_vcto from #tmpWhiteListCourtesyBySuspend)
                    set @clientId = (select plusu_identifi_cliente from #tmpWhiteListCourtesyBySuspend)

                    if exists (select cli_identifi from gim_clientes where cdgimnasio = @gymId)
                    begin
                        set @isClient = 1
                    end
                    else if exists (select cli_identifi from gim_clientes_especiales where cdgimnasio = @gymId)
                    begin
                        set @isClient = 0
                    end

                    if ((@informedState != 1) and (convert(varchar(10),@finalDateInvoice,111) >= convert(varchar(10),getdate(),111)) and
                       (convert(varchar(10),@initialDateInvoice,111) <= convert(varchar(10),getdate(),111)) and (@anulledState != 1))
                    begin
                        if (@isClient = 1)
                        begin
                            insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                                  subgroupId,cardId)
                            select cli.cli_identifi as 'id', 
                                    isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
                                    pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',pu.plusu_fecha_vcto as 'expirationDate',
                                    --ÚLTIMA ENTRADA
                                    null as 'lastEntry',
                                    --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
                                    pla.pla_tipo as 'planType', cast('Cliente' as varchar(100)) as 'typePerson',
                                    --TIQUETES DISPONIBLES
                                    case when (pla.pla_tipo = 'T') then pu.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pu.plusu_fecha_vcto,111)) end as 'availableEntries',
                                    --HORARIOS INGRESO PLAN, POR DÍA.
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                    isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') as 'restrictions',
                                    su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
                                    cast('Pendiente' as varchar(30)) as 'personState', 
									case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
                                    --HUELLA DEL CLIENTE		
                                    case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_id from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
                                    case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.hue_dato from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
                                    case when (cli.cli_sin_huella is null or cli.cli_sin_huella = 0) then (select top 1 hu.strDatoFoto from gim_huellas hu with (index(IX_HUE_WhiteList)) where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
									cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                                    --CONTROL DE ENTRADAS DE ASOCIADOS DE GRUPO FAMILIAR
                                    case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as bit) else isnull((select grpm.gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesControl', 
                                    --CANTIDAD DE ENTRADAS GRUPO FAMILIAR
                                    case when (cli.cli_GrupoFamiliar is null or cli.cli_GrupoFamiliar = 0) then cast(0 as int) else isnull((select grpm.gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) end as 'groupEntriesQuantity', 
                                    --CÓDIGO DE GRUPO
                                    isnull((select grp.gim_gf_IDgrupo from gim_grupoFamiliar_Maestro grpm inner join gim_grupoFamiliar grp ON (grpm.gim_gf_pk_IDgrupo = grp.gim_gf_IDgrupo and grpm.cdgimnasio = grp.cdgimnasio) where grp.gim_gf_IDCliente = cli.cli_identifi and grp.cdgimnasio = cli.cdgimnasio),0) as 'groupId',
                                    cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
                                    null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
                                    cast('' as varchar(200)) as 'employeeName', 
                                    cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
                                    pu.plusu_numero_fact as 'invoiceId', cast(0 as int) as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
                                    isnull(cli.cli_intcodigo_subgrupo,0) as 'subgroupId', isnull(cli.cli_strcodtarjeta,'') as 'cardId'
                            from gim_clientes cli inner join #tmpWhiteListCourtesyBySuspend pu ON (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                                  cli.cdgimnasio = pu.cdgimnasio)
                                                    inner join gim_sucursales su ON (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                    inner join gim_planes pla ON (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                        end
                        else if (@isClient = 0)
                        begin
                            insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                                  subgroupId,cardId)
                            select dbo.fFloatAVarchar(cli.cli_identifi) as 'id', 
                                    isnull(cli.cli_nombres,'') + ' ' + isnull(cli.cli_primer_apellido,'') + ' ' + isnull(cli.cli_segundo_apellido,'') as 'name',
                                    pla.pla_codigo as 'planId', pla.pla_descripc as 'planName',pue.plusu_fecha_vcto as 'expirationDate',
                                    --ÚLTIMA ENTRADA
                                    null as 'lastEntry',
                                    --(select top 1 eu.enusu_fecha_entrada from gim_entradas_usuarios eu where eu.cdgimnasio = cli.cdgimnasio and eu.enusu_identifi = cli.cli_identifi order by eu.enusu_fecha_entrada desc) as 'lastEntry',
                                    pla.pla_tipo as 'planType', cast('Prospecto' as varchar(100)) as 'typePerson',
                                    --TIQUETES DISPONIBLES
                                    case when (pla.pla_tipo = 'T') then pue.plusu_tiq_disponible else datediff(day, convert(varchar(10),getdate(),111), convert(varchar(10),pue.plusu_fecha_vcto,111)) end as 'availableEntries',
                                    --HORARIOS INGRESO PLAN, POR DÍA.
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +		
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') + '| ' +
                                    isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pue.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') as 'restrictions',
                                    su.suc_intpkIdentificacion as 'branchId', su.suc_strNombre as 'branchName', cli.cdgimnasio as 'gymId',
                                    cast('Pendiente' as varchar(30)) as 'personState', case when (cli.cli_EntryFingerprint = 1) then cast(0 as bit) else cast(1 as bit) end as 'withoutFingerprint',
                                    --HUELLA DEL EMPLEADO		
                                    case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_id from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else cast(0 as int) end as 'fingerprintId',
                                    case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.hue_dato from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'fingerprint',
                                    case when (cli.cli_EntryFingerprint = 1) then (select top 1 hu.strDatoFoto from gim_huellas hu where hu.hue_identifi = cli.cli_identifi and hu.cdgimnasio = cli.cdgimnasio) else null end as 'strDatoFoto',
									cast(0 as bit) as 'updateFingerprint', cast(0 as bit) as 'know', cast(0 as bit) as 'courtesy',
                                    cast(0 as bit) as 'groupEntriesControl', cast(0 as int) as 'groupEntriesQuantity', cast(0 as int) as 'groupId',
                                    cast(0 as bit) as 'isRestrictionClass', cast('' as varchar(max)) as 'classSchedule',
                                    null as 'dateClass', cast(0 as int) as 'reserveId', cast('' as varchar(200)) as 'className', cast(0 as int) as 'utilizedMegas', cast(0 as int) as 'utilizedTickets', 
                                    cast('' as varchar(200)) as 'employeeName', 
                                    cast('' as varchar(200)) as 'classIntensity', cast('' as varchar(100)) as 'classState', cast('' as varchar(max)) as 'photoPath', 
                                    pue.plusu_numero_fact as 'invoiceId', pue.plusu_fkdia_codigo as 'dianId', cast('Cortesía' as varchar(50)) as 'documentType',
                                    cast(0 as int) as 'subgroupId', cast('' as varchar(50)) as 'cardId'
                            from gim_clientes_especiales cli inner join #tmpWhiteListCourtesyBySuspend pue ON (dbo.fFloatAVarchar(cli.cli_identifi) = pue.plusu_identifi_cliente and
                                                                                                              cli.cdgimnasio = pue.cdgimnasio)
                                                             inner join gim_sucursales su ON (pue.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                             inner join gim_planes pla ON (pue.plusu_codigo_plan = pla.pla_codigo and pue.cdgimnasio = pla.cdgimnasio)
                            where pue.plusu_avisado = 0 and convert(varchar(10),pue.plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111) and
                                  convert(varchar(10),pue.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
                                  pue.plusu_est_anulada = 0 and pue.plusu_codigo_plan != 999
                        end
                    end

                    drop table #tmpWhiteListCourtesyBySuspend
                end
                else if (@isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId
                end
            end

            set @aux = @aux + 1
        end

        close cursor_suspendedCourtesies
        deallocate cursor_suspendedCourtesies
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_BlackList' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_BlackList
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_BlackList] ON gim_listanegra
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(15)
    declare @state bit
    declare @isClient bit
    declare @isEmployee bit
    declare @isProspect bit
    declare @IngresoEmpSinPlan bit
    declare @branchId int
    declare @fullAge int
    declare @quantity int = isnull((select count(*) from inserted),0)
    declare @aux int = 0
    declare @isDeleted bit = 0

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select top 1 cdgimnasio from inserted)),0)
    --Validaciones Generales
    set @IngresoEmpSinPlan = isnull((select top 1 isnull(bitIngresoEmpSinPlan,1) from gim_configuracion_ingreso where cdgimnasio = (select top 1 cdgimnasio from inserted)),1)

    if (@applyWhiteList = 1)
    begin
        declare cursor_blackList cursor for
            select cdgimnasio, dbo.fFloatAVarchar(listneg_floatId), listneg_bitEstado from inserted
        open cursor_blackList 

        while @aux < @quantity
        begin
            fetch next from cursor_blackList
            into @gymId, @id, @state

            if exists (select * from WhiteList where id = @id and gymId = @gymId)
            begin
                if (@state = 1)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId

                    set @isDeleted = 1
                end
                else if (@state = 0 and (select personState from WhiteList where id = @id and gymId = @gymId) = 'Eliminar' and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where id = @id and gymId = @gymId
                end
            end
            else
            begin
                if (@state = 0)
                begin
                    set @isEmployee = case when (select count(*) from gim_empleados where cdgimnasio = @gymId and emp_identifi = @id) > 0 then cast(1 as bit) else cast(0 as bit) end
                    set @isClient = case when (select count(*) from gim_clientes where cli_identifi = @id and cdgimnasio = @gymId) > 0 then cast(1 as bit) else cast(0 as bit) end
                    set @isProspect = case when (select count(*) from gim_clientes_especiales where cli_identifi = @id and cdgimnasio = @gymId) > 0 then cast(1 as bit) else cast(0 as bit) end
                    
                    if (@isEmployee = 1)
                    begin
                        if (@IngresoEmpSinPlan = 1)
                        begin
                            insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                                  subgroupId,cardId)
                            select * from vwEmployeesWithoutPlan where gymId = @gymId and id = @id
                        end
                        else					
                        begin
                            insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                                  subgroupId,cardId)
                            select * from vwEmployeesWithPlan where gymId = @gymId and id = @id
                        end
                    end
                    else if (@isClient = 1)
                    begin
                        if exists (select count(*) from gim_planes_usuario where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                        begin
                            set @branchId = isnull((select top 1 plusu_sucursal 
                                                    from gim_planes_usuario 
                                                    where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                          and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                          and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                          and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                                    order by plusu_fecha_vcto desc),0)
                        end
                        else if exists (select * from gim_planes_usuario_especiales where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                        begin
                            set @branchId = isnull((select top 1 plusu_sucursal 
                                                    from gim_planes_usuario_especiales 
                                                    where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                          and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                          and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                          and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                                    order by plusu_fecha_vcto desc),0)
                        end

                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
                    end
                    else if (@isProspect = 1)
                    begin
                        if exists (select * from gim_planes_usuario_especiales where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                        begin
                            set @branchId = isnull((select top 1 plusu_sucursal 
                                                    from gim_planes_usuario_especiales 
                                                    where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                          and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                          and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                          and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                                    order by plusu_fecha_vcto desc),0)
                        end
                        else
                        begin
                            set @branchId = isnull((select cli_intfkSucursal 
                                                    from gim_clientes_especiales),0)
                        end

                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeProspects(@gymId,@branchId,@id)
                    end
                end	
            end

            set @aux = @aux + 1
        end

        close cursor_blackList
        deallocate cursor_blackList
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_clients' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_clients
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_clients] ON gim_clientes
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(50)
    declare @whitoutFingerprint bit
    declare @familiarGroup bit
    declare @subgroupId int
    declare @competent bit
    declare @AutorizacionM bit
    declare @dateBorn datetime
    declare @consentimiento bit
    declare @aux int = 0
    declare @isDeleted bit = 0
    declare @bitBloqueoClienteNoApto bit
    declare @bitBloqueoNoAutorizacionMenor bit
    declare @bitBloqueoNoDisentimento bit
    declare @fullAge int
    declare @cardId varchar(20)
    declare @branchId int
    declare @state bit
    declare @giftDays int
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_clients cursor for
            select cdgimnasio, cli_identifi, cli_sin_huella, cli_GrupoFamiliar,cli_intcodigo_subgrupo,cli_Apto,cli_bitAutorizacionM,
                   cli_fecha_nacimien, case when cli_disentimientoVirtual is null then cast(0 as bit) else cast(1 as bit) end,
                   cli_estado,cli_strcodtarjeta,cli_dias_gracia
            from inserted
        open cursor_clients

        while @aux < @quantity
        begin
            fetch next from cursor_clients
            into @gymId,@id,@whitoutFingerprint,@familiarGroup,@subgroupId,@competent,@AutorizacionM,@dateBorn,
                 @consentimiento,@state,@cardId,@giftDays

			
            --Cuando el cliente existe en la lista blanca
            if exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente')
            begin
                --Actualizamos de acuerdo al estado del cliente
                if (@state = 0)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'

                    set @isDeleted = 1
                end
                else if (@state = 1 and (select top 1 personState from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 'Eliminar' and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'
                end

                --Actualizamos ingreso sin huella
                if (@whitoutFingerprint = 1 and @isDeleted = 0)
                begin
                    update WhiteList
                    set withoutFingerprint = 1, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'
                end
                else if (@whitoutFingerprint = 0 and @isDeleted = 0)
                begin
                    update WhiteList
                    set withoutFingerprint = 0, fingerprint = (select top 1 hue_dato from gim_huellas where hue_identifi = @id and cdgimnasio = @gymId), personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'
                end

                --Actualizamos información del grupo familiar
                if (@familiarGroup = 1 and (select top 1 cli_GrupoFamiliar from inserted where cdgimnasio = @gymId and cli_identifi = @id) = 0 and @isDeleted = 0)
                begin
                    update WhiteList
                    set groupId = (select top 1 gim_gf_IDgrupo from gim_grupoFamiliar where gim_gf_IDCliente = @id and cdgimnasio = @gymId and gim_gf_estado = 1),
                        groupEntriesControl = isnull((select top 1 gim_gf_bitControlIngreso from gim_grupoFamiliar_Maestro where cdgimnasio = @gymId and gim_gf_pk_IDgrupo = (select gim_gf_IDgrupo from gim_grupoFamiliar where gim_gf_IDCliente = @id and cdgimnasio = @gymId and gim_gf_estado = 1)),0),
                        groupEntriesQuantity = isnull((select top 1 gim_gf_intNumlIngresos from gim_grupoFamiliar_Maestro where cdgimnasio = @gymId and gim_gf_pk_IDgrupo = (select gim_gf_IDgrupo from gim_grupoFamiliar where gim_gf_IDCliente = @id and cdgimnasio = @gymId and gim_gf_estado = 1)),0),
                        personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'
                end
                else if (@familiarGroup = 0 and (select top 1 cli_GrupoFamiliar from inserted where cdgimnasio = @gymId and cli_identifi = @id) = 1 and @isDeleted = 0)
                begin
                    update WhiteList
                    set groupId = 0, groupEntriesControl = 0, groupEntriesQuantity = 0, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'
                end

                --Actualizamos las restricciones
                update WhiteList
                set restrictions = isnull((select top 1 isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_lunes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_lunes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_martes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_martes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_miercoles_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_miercoles_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_jueves_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_jueves_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_viernes_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_viernes_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_sabado_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_sabado_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +		
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_domingo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_domingo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ') + '| ' +
                                                        isnull((select distinct(select distinct (select (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_desde_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),pa.pla_hasta_hora_adicional) + ':' + convert(varchar(10),pa.pla_min_hasta_adicional))),114)) + '; ' from gim_planes_adicionales pa where pa.pla_codigo_plan = pap.pla_codigo_plan and pa.pla_festivo_adicional = 1 and pa.cdgimnasio = pap.cdgimnasio for xml path('')) from gim_planes_adicionales pap inner join gim_planes pla ON (pap.pla_codigo_plan = pla.pla_codigo and pap.cdgimnasio = pla.cdgimnasio) where pap.pla_codigo_plan = pu.plusu_codigo_plan and pap.cdgimnasio = cli.cdgimnasio)),' ') +
                                                        isnull((select distinct(select distinct (select 'SG' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_desde_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_desde_adicional))),114)) + '-' + (convert(varchar(10),convert(datetime,(convert(varchar(10),sga.subgru_adi_hasta_hora_adicional) + ':' + convert(varchar(10),sga.subgru_adi_min_hasta_adicional))),114)) + '; ' from gim_subgru_adicionales sga where sga.subgru_adi_codigo_subgru = sgap.subgru_adi_codigo_subgru and sga.subgru_adi_festivo_adicional = 1 and sga.cdgimnasio = sgap.cdgimnasio for xml path('')) from gim_subgru_adicionales sgap where sgap.cdgimnasio = cli.cdgimnasio and sgap.subgru_adi_codigo_subgru = cli.cli_intcodigo_subgrupo)),' ')
                                           from inserted cli inner join gim_planes_usuario pu ON (cli.cli_identifi = pu.plusu_identifi_cliente and
                                                                                                  cli.cdgimnasio = pu.cdgimnasio)
                                                             inner join gim_sucursales su ON (pu.plusu_sucursal = su.suc_intpkIdentificacion and cli.cdgimnasio = su.cdgimnasio)
                                                             inner join gim_planes pla ON (pu.plusu_codigo_plan = pla.pla_codigo and pu.cdgimnasio = pla.cdgimnasio)
                                           where pu.plusu_avisado = 0 and convert(varchar(10),pu.plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111) and
                                                 convert(varchar(10),pu.plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and
                                                 pu.plusu_est_anulada = 0 and pu.plusu_codigo_plan != 999 and pu.cdgimnasio = @gymId),'')
                where id = @id and gymId = @gymId and typePerson = 'Cliente'

                --Actualizamos el cliente de acuerdo al estado de alto riesgo
                if (@competent = 0)
                begin
                    set @bitBloqueoClienteNoApto = isnull((select isnull(bitBloqueoClienteNoApto,0) 
                                                           from gim_configuracion_ingreso 
                                                           where cdgimnasio = @gymId and intfkSucursal = (select top 1 branchId 
                                                                                                          from WhiteList 
                                                                                                          where id = @id and gymId = @gymId and typePerson = 'Cliente')),0)
                    if (@bitBloqueoClienteNoApto = 1)
                    begin
                        update WhiteList
                        set personState = 'Eliminar'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'

                        set @isDeleted = 1
                    end
                end
                else if (@competent = 1 and (select top 1 personState from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 'Eliminar')
                begin
                    set @bitBloqueoClienteNoApto = isnull((select isnull(bitBloqueoClienteNoApto,0) 
                                                           from gim_configuracion_ingreso 
                                                           where cdgimnasio = @gymId and intfkSucursal = (select top 1 branchId 
                                                                                                          from WhiteList 
                                                                                                          where id = @id and gymId = @gymId and typePerson = 'Cliente')),0)
                    
                    if (@bitBloqueoClienteNoApto = 1 and @isDeleted = 0)
                    begin
                        update WhiteList
                        set personState = 'Pendiente'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'
                    end
                end

                --Actualizamos el cliente de acuerdo a si se autoriza el ingreso a menor de edad y este lo es
                set @fullAge = isnull((select isnull(intAnosMayoriaEdad,0) from tblConfiguracion where cdgimnasio = @gymId),0)
                if (@AutorizacionM = 0 and DATEDIFF(HOUR, CONVERT(datetime, CONVERT(VARCHAR(10), @dateBorn,111),111), GETDATE())/(8766) < @fullAge)
                begin
                    set @bitBloqueoNoAutorizacionMenor = isnull((select isnull(bitBloqueoNoAutorizacionMenor,0) 
                                                                 from gim_configuracion_ingreso 
                                                                 where cdgimnasio = @gymId and intfkSucursal = (select top 1 branchId 
                                                                                                                from WhiteList 
                                                                                                                where id = @id and gymId = @gymId and typePerson = 'Cliente')),0)
                    if (@bitBloqueoNoAutorizacionMenor = 1)
                    begin
                        update WhiteList
                        set personState = 'Eliminar'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'

                        set @isDeleted = 1
                    end
                end
                else if (@AutorizacionM = 1 and (select top 1 personState from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 'Eliminar')
                begin
                    set @bitBloqueoNoAutorizacionMenor = isnull((select isnull(bitBloqueoNoAutorizacionMenor,0) 
                                                                 from gim_configuracion_ingreso 
                                                                 where cdgimnasio = @gymId and intfkSucursal = (select top 1 branchId 
                                                                                                                from WhiteList 
                                                                                                                where id = @id and gymId = @gymId and typePerson = 'Cliente')),0)
                    if (@bitBloqueoNoAutorizacionMenor = 1 and @isDeleted = 0)
                    begin
                        update WhiteList
                        set personState = 'Pendiente'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'
                    end
                end

                --Actualizamos el cliente de acuerdo a si tiene consentimiento informado
                if (@consentimiento = 0)
                begin
                    set @bitBloqueoNoDisentimento = isnull((select isnull(bitBloqueoNoDisentimento,0) 
                                                            from gim_configuracion_ingreso 
                                                            where cdgimnasio = @gymId and intfkSucursal = (select top 1 branchId 
                                                                                                           from WhiteList 
                                                                                                           where id = @id and gymId = @gymId and typePerson = 'Cliente')),0)
                    if (@bitBloqueoNoDisentimento = 1)
                    begin
                        update WhiteList
                        set personState = 'Eliminar'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'

                        set @isDeleted = 1
                    end
                end
                else if (@consentimiento = 1 and (select top 1 personState from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 'Eliminar')
                begin
                    set @bitBloqueoNoDisentimento = isnull((select isnull(bitBloqueoNoDisentimento,0) 
                                                            from gim_configuracion_ingreso 
                                                            where cdgimnasio = @gymId and intfkSucursal = (select top 1 branchId 
                                                                                                           from WhiteList 
                                                                                                           where id = @id and gymId = @gymId and typePerson = 'Cliente')),0)
                    
                    if (@bitBloqueoNoDisentimento = 1 and @isDeleted = 0)
                    begin
                        update WhiteList
                        set personState = 'Pendiente'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'
                    end
                end

                --Actualizamos el código de la tarjeta
				print(@isDeleted)
                if (@cardId != (select top 1 cardId from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') and @isDeleted = 0)
                begin
                    update WhiteList
                    set cardId = @cardId, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Cliente'
                end

                --Actualizamos el registro de acuerdo a los días de gracia
                if (@giftDays <= 0 or @giftDays is null)
                begin
                    declare @invoice int = isnull((select top 1 invoiceId from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente'),0)
                    declare @dian int = isnull((select top 1 dianId from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente'),0)
                    declare @branch int = isnull((select top 1 branchId from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente'),0)

                    if (@invoice > 0 and @dian > 0 and @branch > 0)
                    begin
                        if (convert(varchar(10),(select dateadd(day,@giftDays,plusu_fecha_vcto) from gim_planes_usuario where cdgimnasio = @gymId and plusu_sucursal = @branch and plusu_numero_fact = @invoice and plusu_fkdia_codigo = @dian),111) < convert(varchar(10),getdate(),111))  
                        begin
                            update WhiteList
                            set personState = 'Eliminar'
                            where id = @id and gymId = @gymId and typePerson = 'Cliente' and invoiceId = @invoice and dianId = @dian and branchId = @branch

                            set @isDeleted = 1
                        end
                    end
                end
            end
            --Cuando el cliente no existe en la lista blanca
            else
            begin
                if exists (select * from gim_planes_usuario where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                begin
                    set @branchId = isnull((select top 1 plusu_sucursal 
                                            from gim_planes_usuario 
                                            where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                  and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                  and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                  and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                            order by plusu_fecha_vcto desc),0)

                    set @fullAge = isnull((select isnull(intAnosMayoriaEdad,0) from tblConfiguracion where cdgimnasio = @gymId),0)

                    --Actualizamos de acuerdo al estado del cliente
                    if ((@state = 1) or (@competent = 1) or (@consentimiento = 1) or (@AutorizacionM = 1 and DATEDIFF(HOUR, CONVERT(datetime, CONVERT(VARCHAR(10), @dateBorn,111),111), GETDATE())/(8766) < @fullAge))
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
                    end

                    --Actualizamos el registro de acuerdo a los días de gracia
                    if (@giftDays > 0)
                    begin
                        if convert(varchar(10),(select dateadd(day,@giftDays,plusu_fecha_vcto) from gim_planes_usuario where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)),111) >= convert(varchar(10),getdate(),111)
                        begin
                            insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                                  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                                  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                                  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                                  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                                  subgroupId,cardId)
                            select *
                            from fnIncludeClients(@gymId,@branchId,@id)
                        end
                    end
                end
                else if exists (select * from gim_planes_usuario_especiales where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                begin
                    set @branchId = isnull((select top 1 plusu_sucursal 
                                            from gim_planes_usuario_especiales 
                                            where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                  and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                  and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                  and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                            order by plusu_fecha_vcto desc),0)

                    set @fullAge = isnull((select isnull(intAnosMayoriaEdad,0) from tblConfiguracion where cdgimnasio = @gymId),0)

                    --Actualizamos de acuerdo al estado del cliente
                    if ((@state = 1) or (@competent = 1) or (@consentimiento = 1) or (@AutorizacionM = 1 and DATEDIFF(HOUR, CONVERT(datetime, CONVERT(VARCHAR(10), @dateBorn,111),111), GETDATE())/(8766) < @fullAge))
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
                    end
                end
            end

            set @aux = @aux + 1
        end

        close cursor_clients
        deallocate cursor_clients
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_employee' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_employee
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_employee] ON gim_empleados
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(50)
    declare @state bit
    declare @withoutFingerprint bit
    declare @cardId varchar(20)
    declare @aux int = 0
    declare @isDeleted bit = 0
    declare @IngresoEmpSinPlan bit
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)
    set @IngresoEmpSinPlan = isnull((select top 1 isnull(bitIngresoEmpSinPlan,1) from gim_configuracion_ingreso where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),1)

    if (@applyWhiteList = 1)
    begin
        declare cursor_employee cursor for
            select cdgimnasio,emp_identifi,emp_estado,emp_sin_huella,emp_strcodtarjeta
            from inserted
        open cursor_employee

        while @aux < @quantity
        begin
            fetch next from cursor_employee
            into @gymId,@id,@state,@withoutFingerprint,@cardId

            --Cuando el cliente existe en la lista blanca
            if exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Empleado')
            begin
                --Actualizamos de acuerdo al estado del empleado
                if (@state = 0)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where id = @id and gymId = @gymId and typePerson = 'Empleado'

                    set @isDeleted = 1
                end
                else if (@state = 1 and (select top 1 personState from WhiteList where id = @id and gymId = @gymId and typePerson = 'Empleado') = 'Eliminar' and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Empleado'
                end

                --Actualizamos ingreso sin huella
                if (@withoutFingerprint = 1 and @isDeleted = 0)
                begin
                    update WhiteList
                    set withoutFingerprint = 1, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Empleado'
                end
                else if (@withoutFingerprint = 0 and @isDeleted = 0)
                begin
                    update WhiteList
                    set withoutFingerprint = 0, fingerprint = (select top 1 hue_dato from gim_huellas where hue_identifi = @id and cdgimnasio = @gymId), personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Empleado'
                end

                --Actualizamos el código de la tarjeta
                if (@cardId != (select top 1 cardId from WhiteList where id = @id and gymId = @gymId and typePerson = 'Empleado') and @isDeleted = 0)
                begin
                    update WhiteList
                    set cardId = @cardId, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Empleado'
                end
            end
            else
            begin
                if (@state = 1)
                begin
                    if (@IngresoEmpSinPlan = 1)
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select * from vwEmployeesWithoutPlan where gymId = @gymId and id = @id
                    end
                    else					
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select * from vwEmployeesWithPlan where gymId = @gymId and id = @id
                    end
                end
            end

            set @aux = @aux + 1
        end

        close cursor_employee
        deallocate cursor_employee
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_plans' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_plans
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_plans] ON gim_planes
after update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @planId int
    declare @additionals bit
    declare @state bit
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

	if update(pla_adicionales)
	begin
    if (@applyWhiteList = 1)
    begin
        declare cursor_plans cursor for
            select cdgimnasio, pla_codigo,pla_adicionales, case when pla_desactivado = 1 then cast(0 as bit) else cast(1 as bit) end
            from inserted
        open cursor_plans

        while @aux < @quantity
        begin
            fetch next from cursor_plans
            into @gymId,@planId,@additionals,@state

				if (@state = 1)
            begin
                if exists (select * from WhiteList where planId = @planId and gymId = @gymId)
                begin
                    exec spUpdatePlanRestrictions_WhiteList @gymId,@planId
                end
            end

            set @aux = @aux + 1
        end

        close cursor_plans
        deallocate cursor_plans
    end
end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_plans_adicionales' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_plans_adicionales
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_plans_adicionales] ON gim_planes_adicionales
after update, insert
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @planId int
    declare @additionals bit
    declare @state bit
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

	if (@applyWhiteList = 1)
    begin
        declare cursor_plans cursor for
            select inserted.cdgimnasio, pla_codigo,pla_adicionales, case when pla_desactivado = 1 then cast(0 as bit) else cast(1 as bit) end
            from inserted
			inner join gim_planes as planes
			ON planes.pla_codigo = inserted.pla_codigo_plan and inserted.cdgimnasio = planes.cdgimnasio
        open cursor_plans

        while @aux < @quantity
        begin
            fetch next from cursor_plans
            into @gymId,@planId,@additionals,@state

				if (@state = 1)
            begin
                if exists (select * from WhiteList where planId = @planId and gymId = @gymId)
                begin
                    exec spUpdatePlanRestrictions_WhiteList @gymId,@planId
                end
            end

            set @aux = @aux + 1
        end

        close cursor_plans
        deallocate cursor_plans
    end
    

end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_credits' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_credits
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_credits] ON gim_creditos
after update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @creditId int
    declare @invoiceId int
    declare @branchId int
    declare @dianId int
    declare @date datetime
    declare @paid bit
    declare @id int
    declare @aux int = 0
    declare @isDeleted bit = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_credits cursor for
            select cdgimnasio,cre_numero,cre_factura,cre_sucursal,cre_fkdia_codigo,cre_fecha,cre_pagado
            from inserted
        open cursor_credits

        while @aux < @quantity
        begin
            fetch next from cursor_credits
            into @gymId,@creditId,@invoiceId,@branchId,@dianId,@date,@paid

            if (select count(*) from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId) > 0
            begin
                if (convert(varchar(10),@date,111) < convert(varchar(10),getdate(),111) and @paid = 0)
                begin
                    update WhiteList
                    set personState = 'Eliminar'
                    where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId

                    set @isDeleted = 1
                end
                else if (convert(varchar(10),@date,111) >= convert(varchar(10),getdate(),111) and @paid = 0 and (select personState from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId) = 'Eliminar' and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId
                end
                else if (@paid = 1 and (select personState from WhiteList where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId) = 'Eliminar' and @isDeleted = 0)
                begin
                    update WhiteList
                    set personState = 'Pendiente'
                    where gymId = @gymId and invoiceId = @invoiceId and branchId = @branchId and dianId = @dianId
                end
            end
            else
            begin
                if ((convert(varchar(10),@date,111) >= convert(varchar(10),getdate(),111) and @paid = 0) or (@paid = 1))
                begin
                    if exists (select * from gim_planes_usuario where plusu_sucursal = @branchId and plusu_numero_fact = @invoiceId and plusu_codigo_plan != 999 and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                    begin
                        set @id = isnull((select plusu_identifi_cliente 
                                          from gim_planes_usuario 
                                          where plusu_codigo_plan != 999 and cdgimnasio = @gymId 
                                                and plusu_numero_fact = @invoiceId and plusu_fkdia_codigo = @dianId
                                                and plusu_sucursal = @branchId
                                                and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)),0)
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
                    end
                end
            end

            set @aux = @aux + 1
        end

        close cursor_credits
        deallocate cursor_credits
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_subgroup' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_subgroup
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_subgroup] ON gim_subgrupos
after update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @subgroupId int
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_subgroup cursor for
            select cdgimnasio, subg_intcodigo
            from inserted
        open cursor_subgroup

        while @aux < @quantity
        begin
            fetch next from cursor_subgroup
            into @gymId,@subgroupId

            if (select count(*) from gim_subgru_adicionales where subgru_adi_codigo_subgru = @subgroupId and cdgimnasio = @gymId) > 0
            begin
                if exists (select * from WhiteList where subgroupId = @subgroupId and gymId = @gymId)
                begin
                    exec spUpdateSubgroupRestrictions_WhiteList @gymId,@subgroupId
                end
            end

            set @aux = @aux + 1
        end

        close cursor_subgroup
        deallocate cursor_subgroup
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_subgroup_adicionales' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_subgroup_adicionales
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_subgroup_adicionales] ON gim_subgru_adicionales
after update, insert
as
begin

    declare @applyWhiteList bit
    declare @gymId int
    declare @subgroupId int
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_subgroup cursor for
            select inserted.cdgimnasio, gim_sub.subg_intcodigo
            from inserted
			inner join gim_subgrupos as gim_sub
			ON gim_sub.cdgimnasio = inserted.cdgimnasio and gim_sub.subg_intcodigo = inserted.subgru_adi_codigo_subgru
        open cursor_subgroup

        while @aux < @quantity
        begin
            fetch next from cursor_subgroup
            into @gymId,@subgroupId

			select top 1 @subgroupId=subgru_adi_codigo_subgru,
			@gymId= cdgimnasio 
			from inserted
			

            if (select count(*) from gim_subgru_adicionales where subgru_adi_codigo_subgru = @subgroupId and cdgimnasio = @gymId) > 0
            begin

                if exists (select * from WhiteList where subgroupId = @subgroupId and gymId = @gymId)
                begin
                    exec spUpdateSubgroupRestrictions_WhiteList @gymId,@subgroupId
                end

            end

            set @aux = @aux + 1
        end

        close cursor_subgroup
        deallocate cursor_subgroup
    end

end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_contracts' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_contracts
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_contracts] ON gim_detalle_contrato
after insert
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @invoiceId int
    declare @branchId int
    declare @id int
    declare @bitValideContrato bit
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)
    set @bitValideContrato = isnull((select top 1 isnull(bitValideContrato,0) from gim_configuracion_ingreso where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        if (@bitValideContrato = 1)
        begin
            declare cursor_contracts cursor for
                select cdgimnasio,dbo.fFloatAVarchar(dtcont_doc_cliente),dtcont_numero_plan,dtcont_sucursal_plan
                from inserted
            open cursor_contracts

            while @aux < @quantity
            begin
                fetch next from cursor_contracts
                into @gymId,@id,@invoiceId,@branchId

                if not exists (select * from WhiteList where gymId = @gymId and invoiceId = @invoiceId and id = @id and branchId = @branchId)
                begin
                    if exists (select * from gim_planes_usuario where plusu_sucursal = @branchId and plusu_identifi_cliente = @id and plusu_numero_fact = @invoiceId and plusu_codigo_plan != 999 and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                    begin
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
                    end
                end

                set @aux = @aux + 1
            end

            close cursor_contracts
            deallocate cursor_contracts
        end
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_reserve' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_reserve
END
GO

create trigger [dbo].[trgWhiteList_reserve] on [dbo].[gim_reservas]
after insert,update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(50)
    declare @aux int = 0
    declare @state varchar(50)
    declare @branchId int
    declare @classId int
    declare @date datetime
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_reserve cursor for
            select cdgimnasio, IdentificacionCliente,estado,cdclase,fecha_clase
            from inserted
        open cursor_reserve

        while @aux < @quantity
        begin
            fetch next from cursor_reserve
            into @gymId,@id,@state,@classId,@date

            --Cuando el cliente existe en la lista blanca
            if exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente')
            begin
                if (@state != 'Anulada' and convert(varchar(10),@date,111) = convert(varchar(10),getdate(),111))
                begin
                    if exists (select * from gim_planes_usuario where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                    begin
                        set @branchId = isnull((select top 1 plusu_sucursal 
                                                from gim_planes_usuario 
                                                where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                      and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                      and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                      and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                                order by plusu_fecha_vcto desc),0)
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
						where not exists (select id from WhiteList 
										  where WhiteList.reserveId = fnIncludeClients.reserveId
										  and WhiteList.dateClass = fnIncludeClients.dateClass
										  and WhiteList.classSchedule = fnIncludeClients.classSchedule)
                    end
                end
            end
            else if not exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente')
			begin
			 if (@state != 'Anulada' and convert(varchar(10),@date,111) = convert(varchar(10),getdate(),111))
                begin
                    if exists (select * from gim_planes_usuario where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                    begin
                        set @branchId = isnull((select top 1 plusu_sucursal 
                                                from gim_planes_usuario 
                                                where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                      and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                      and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                      and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                                order by plusu_fecha_vcto desc),0)
                        insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                              branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                              courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                              classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                              employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                              subgroupId,cardId)
                        select *
                        from fnIncludeClients(@gymId,@branchId,@id)
                    end
                end
			end
			else
            begin
                if (@state = 'Anulada' and convert(varchar(10),@date,111) = convert(varchar(10),getdate(),111))
                begin
                    if ((select top 1 isRestrictionClass from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 1)
                    begin
                        update WhiteList
                        set personState = 'Eliminar', isRestrictionClass = 'false'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'
                    end
                end
				if ((@state = 'Activa') and convert(varchar(10),@date,111) = convert(varchar(10),getdate(),111))
                begin
                    if ((select top 1 isRestrictionClass from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 1)
                    begin
                        update WhiteList
                        set personState = 'Pendiente' , isRestrictionClass = 'true'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'
                    end
                end
				if ((@state = 'Asistido') and convert(varchar(10),@date,111) = convert(varchar(10),getdate(),111))
                begin
                    if ((select top 1 isRestrictionClass from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente') = 1)
                    begin
                        update WhiteList
                        set personState = 'Pendiente', isRestrictionClass = 'false'
                        where id = @id and gymId = @gymId and typePerson = 'Cliente'
                    end
                end
            end

            set @aux = @aux + 1
        end

        close cursor_reserve
        deallocate cursor_reserve
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_appointment' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_appointment
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_appointment] ON tblCitas
after insert,update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(50)
    declare @aux int = 0
    declare @attended bit
    declare @branchId int
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)

    set @applyWhiteList = isnull((select top 1 isnull(bitIngressWhiteList,0) from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_appointment cursor for
            select intEmpresa, strIdentificacionPaciente,bitAtendida
            from inserted
        open cursor_appointment

        while @aux < @quantity
        begin
            fetch next from cursor_appointment
            into @gymId,@id,@attended
            --Cuando el cliente existe en la lista blanca
            if not exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Cliente')
            begin
                if exists (select * from gim_planes_usuario where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId and plusu_est_anulada != 1 and plusu_avisado != 1 and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111))
                begin
                    set @branchId = isnull((select top 1 plusu_sucursal 
                                            from gim_planes_usuario 
                                            where plusu_codigo_plan != 999 and plusu_identifi_cliente = @id and cdgimnasio = @gymId 
                                                    and plusu_est_anulada != 1 and plusu_avisado != 1 
                                                    and convert(varchar(10),plusu_fecha_inicio,111) <= convert(varchar(10),getdate(),111) 
                                                    and convert(varchar(10),plusu_fecha_vcto,111) >= convert(varchar(10),getdate(),111)
                                            order by plusu_fecha_vcto desc),0)
                    insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                            branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                            courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                            classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                            employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                            subgroupId,cardId)
                    select *
                    from fnIncludeClients(@gymId,@branchId,@id)
                end
            end

            set @aux = @aux + 1
        end

        close cursor_appointment
        deallocate cursor_appointment
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_specialClients' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_specialClients
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_specialClients] ON gim_clientes_especiales
after insert, update
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @id varchar(50)
    declare @know bit
    declare @courtesy bit
    declare @entryFingerprint bit
    declare @branchId int
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)

    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

    if (@applyWhiteList = 1)
    begin
        declare cursor_specialClients cursor for
            select cdgimnasio, dbo.fFloatAVarchar(cli_identifi), cli_entrar_conocer,cli_cortesia,
                   cli_EntryFingerprint, cli_intfkSucursal 
            from inserted
        open cursor_specialClients

        while @aux < @quantity
        begin
            fetch next from cursor_specialClients
            into @gymId,@id,@know,@courtesy,@entryFingerprint,@branchId

            if exists (select * from WhiteList where id = @id and gymId = @gymId and typePerson = 'Prospecto')
            begin
                if (@entryFingerprint = 1 and (select withoutFingerprint from WhiteList where id = @id and gymId = @gymId and typePerson = 'Prospecto') = 1)
                begin
                    update WhiteList
                    set withoutFingerprint = 0, fingerprint = (select top 1 hue_dato from gim_huellas where hue_identifi = @id and cdgimnasio = @gymId), personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Prospecto'
                end
                else if (@entryFingerprint = 0 and (select withoutFingerprint from WhiteList where id = @id and gymId = @gymId and typePerson = 'Prospecto') = 0)
                begin
                    update WhiteList
                    set withoutFingerprint = 1, personState = 'Pendiente'
                    where id = @id and gymId = @gymId and typePerson = 'Prospecto'
                end
            end
            else
            begin
                if (@know = 1 or @courtesy = 1)
                begin
                    insert into WhiteList(id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
                                          branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
                                          courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
                                          classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
                                          employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
                                          subgroupId,cardId)
                    select *
                    from fnIncludeProspects(@gymId,@branchId,@id)
                end
            end

            set @aux = @aux + 1
        end

        close cursor_specialClients
        deallocate cursor_specialClients
    end
end
GO

IF EXISTS (SELECT * FROM sys.objects WHERE name = 'trgWhiteList_clientCards' and type in (N'TR'))
BEGIN
	DROP TRIGGER trgWhiteList_clientCards
END
GO

CREATE TRIGGER [dbo].[trgWhiteList_clientCards] ON ClientCards
after update, insert
as
begin
    declare @applyWhiteList bit
    declare @gymId int
    declare @clientId varchar(50)
    declare @aux int = 0
    declare @quantity int = isnull((select count(*) 
                                    from inserted),0)
    set @applyWhiteList = isnull((select bitIngressWhiteList from tblConfiguracion where cdgimnasio = (select TOP 1 cdgimnasio from inserted)),0)

	if (@applyWhiteList = 1)
    begin
        declare trgWhiteList_clientCards cursor for
            select cdgimnasio, cli_identifi
            from inserted
        open trgWhiteList_clientCards

		declare @tmpUsersCards as table (
			userId varchar(50),
			gymId int
		)

        while @aux < @quantity
        begin
            fetch next from trgWhiteList_clientCards
            into @gymId,@clientId

			if not exists (select top 1 1 from @tmpUsersCards where userId = @clientId)
			begin
				insert into @tmpUsersCards values (@clientId,@gymId)
			end

            set @aux = @aux + 1
        end

		update WhiteList
		set personState = 'Pendiente'
		from WhiteList wl inner join @tmpUsersCards tmp ON (wl.gymId = tmp.gymId and wl.id = tmp.userId)
		where wl.personState != 'Eliminar' and wl.personState != 'Pendiente' and wl.typePerson = 'Cliente'
			  and wl.id in (select id from dbo.fnClientsWithVigentPlan(wl.gymId, wl.branchId))

        close trgWhiteList_clientCards
        deallocate trgWhiteList_clientCards
    end
end
GO

--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------FIN--------------------------------------------------------------------------------
-----------------------------------------------------------------------TRIGGERS LISTA BLANCA -----------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------------------------

















