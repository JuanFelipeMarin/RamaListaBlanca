--if not exists (select * from sys.databases where name = 'dbWhiteList')
--begin
--	create database dbWhiteList
--end
--go

--use dbWhiteList
go
-- Crear table para replica de tarjetas homologacion biostation
if not exists (select * from sys.objects where name = 'tblReplicatedCardPing' and type in (N'U'))
begin
	CREATE TABLE tblReplicatedCardPing
(
idPerson varchar(max),
strTarjeta varchar(max),
bitEliminado bit,
strIP varchar(max)
)
end
go

if not exists (select * from sys.objects where name = 'tblAccionesReplicaHuellas' and type in (N'U'))
begin
	CREATE TABLE tblAccionesReplicaHuellas
(
intID int identity (1,1),
strip varchar(max),
snTerminar varchar(max),
personId varchar(max),
bitInsertFingert bit default 0,
fingerprint binary(2000)
)
end
go


--OD 1689 - GETULIO VARGAS - 2020/01/27
if not exists (select * from sys.objects where name = 'tblClientCards' and type in (N'U'))
begin
	create table tblClientCards(
		id int not null primary key,
		clientId varchar(15) not null,
		cardCode varchar(20) not null,
		state bit not null
	)
end
go

if not exists (select * from sys.objects where name = 'tblHorrariosRestriccion' and type in (N'U'))
begin
	CREATE TABLE [dbo].[tblHorrariosRestriccion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[strHoraEntrada] [varchar](max) NOT NULL,
	[strHoraSalida] [varchar](max) NOT NULL)

end
go

IF((SELECT COUNT(*) FROM [tblHorrariosRestriccion]) = 0)
BEGIN
INSERT INTO tblHorrariosRestriccion (strHoraEntrada,strHoraSalida)
VALUES ('00:01:00:0','23:59:00:0')

END
GO


if not exists (select * from sys.objects where name = 'tblWhiteList' and type in (N'U'))
begin
	create table tblWhiteList(
		intPkId int not null primary key identity(1,1),
		id varchar(15) not null,
		name varchar(max) not null,
		planId int not null,
		planName varchar(max) not null,
		expirationDate datetime null,
		lastEntry datetime null,
		planType varchar(10) not null,
		typePerson varchar(100) not null,
		availableEntries int not null,
		restrictions varchar(max) not null,
		branchId int not null,
		branchName varchar(max) not null,
		gymId int not null,
		personState varchar(100) not null,
		withoutFingerprint bit not null,
		fingerprintId int null,
		fingerprint binary(2000) null,
		strDatoFoto varchar(max),
		updateFingerprint bit,
		know bit not null,
		courtesy bit not null,
		groupEntriesControl bit not null,
		groupEntriesQuantity int not null,
		groupId int not null,
		isRestrictionClass bit not null,
		classSchedule varchar(max) not null,
		dateClass datetime null,
		reserveId int not null,
		className varchar(200) not null,
		utilizedMegas int not null,
		utilizedTickets int not null,
		employeeName varchar(200) not null,
		classIntensity varchar(200) not null,
		classState varchar(100) not null,
		photoPath varchar(max) not null,
		invoiceId int not null,
		dianId int not null,
		documentType varchar(50) not null,
		subgroupId int not null,
		cardId varchar(50) not null
	)
end
go
IF not EXISTS (SELECT * FROM sys.columns WHERE name = 'strDatoFoto' and object_id = OBJECT_ID(N'[dbo].[tblWhiteList]'))
BEGIN
    alter table tblWhiteList add strDatoFoto varchar(max) null
end
go
if not exists (select * from sys.objects where name like '%type_tblWhiteList%' and type in (N'TT'))
begin
	CREATE TYPE type_tblWhiteList AS TABLE(
		intPkId int,
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
		cardId varchar(50),
		ins bit,
		upd bit,
		del bit
	)
end
go

if exists (select * from sys.objects where name = 'spWhiteList' and type in (N'P'))
begin
	drop procedure spWhiteList
end
go

CREATE procedure [dbo].[spWhiteList]
	@action varchar(50) = '',
	@intPkId int = null,
	@id varchar(max) = null,
	@name varchar(max) = '',
	@planId int = null,
	@planName varchar(max) = '',
	@expirationDate datetime = null,
	@lastEntry datetime = null,
	@planType varchar(10) = '',
	@typePerson varchar(100) = '',
	@availableEntries int = null,
	@restrictions varchar(max) = '',
	@branchId int = null,
	@branchName varchar(max) = '',
	@gymId int = null,
	@personState varchar(100) = '',
	@withoutFingerprint bit = null,
	@fingerprintId int = null,
	@fingerprint binary(2000) = null,
	@strDatoFoto varchar(max)=null,
	@updateFingerprint bit = null,
	@know bit = null,
	@courtesy bit = null,
	@groupEntriesControl bit = null,
	@groupEntriesQuantity int = null,
	@groupId int = null,
	@isRestrictionClass bit = null,
	@classSchedule varchar(max) = '',
	@dateClass datetime = null,
	@reserveId int = null,
	@className varchar(200) = '',
	@utilizedMegas int = null,
	@utilizedTickets int = null,
	@employeeName varchar(200) = '',
	@classIntensity varchar(200) = '',
	@classState varchar(100) = '',
	@photoPath varchar(max) = '',
	@invoiceId int = null,
	@dianId int = null,
	@documentType varchar(50) = '',
	@subgroupId int = null,
	@cardId varchar(50) = '',
	@tblWhiteList type_tblWhiteList readonly,
	@horaEntrada varchar(max) = '',
	@horaSalida varchar(max) = '',
	@diaSemana varchar(max) = '',
	@bitValidarReserva BIT = NULL
as
begin
	declare @strSql varchar(max)
	DECLARE @strTipoPersona AS VARCHAR(max)

	IF (@action = 'GetWhiteList')
	BEGIN

		SELECT	intPkId,
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
				isnull(updateFingerprint,0) as updateFingerprint,
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
				convert(varchar(10),SUBSTRING(classSchedule,12,22),108) as 'horaFinal'
		FROM	tblWhiteList
	
	END

	--Incidente:0005673 Mtoro
	if (@action = 'GetClientsReservesFromToday')
	begin
		select *
		from tblWhiteList
		where convert(varchar(10), dateClass, 111) = convert(varchar(10), getdate(), 111) -- todas los registros de hoy
				and id = @id 
				and reserveId > 0 -- que si sea reserva
	end
	--FIN Incidente:0005673 Mtoro


	if (@action = 'Insert')
	begin
		if (@expirationDate = cast('19000101' as datetime))
		begin
			set @expirationDate = null
		end

		if (@lastEntry = cast('19000101' as datetime))
		begin
			set @lastEntry = null
		end

		if (@dateClass = cast('19000101' as datetime))
		begin
			set @dateClass = null
		end

		insert into tblWhiteList (id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
								  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
								  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
								  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
								  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
								  subgroupId,cardId)
		values (@id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
				@branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
				@courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
				@classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
				@employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
				@subgroupId,@cardId)
	end

	if (@action = 'Update')
	begin
		update tblWhiteList
		set availableEntries = @availableEntries, restrictions = @restrictions, withoutFingerprint = @withoutFingerprint,
			fingerprintId = @fingerprintId, fingerprint = @fingerprint,strDatoFoto=@strDatoFoto, groupEntriesControl = @groupEntriesControl,
			groupEntriesQuantity = @groupEntriesQuantity, groupId = @groupId, isRestrictionClass = @isRestrictionClass,
			classSchedule = @classSchedule, dateClass = @dateClass, reserveId = @reserveId, className = @className,
			utilizedMegas = @utilizedMegas, utilizedTickets = @utilizedTickets, employeeName = @employeeName, 
			classIntensity = @classIntensity, classState = @classState, photoPath = @photoPath,
			subgroupId = @subgroupId, cardId = @cardId
		where intPkId = @intPkId
	end

	if (@action = 'Delete')
	begin
		if (@documentType = 'Factura')
		begin
			delete
			from tblWhiteList
			where id = @id and typePerson = @typePerson and planId = @planId
				  and documentType = @documentType and invoiceId = @invoiceId
		end
		else if (@documentType = 'Cortesía')
		begin
			delete
			from tblWhiteList
			where id = @id and typePerson = @typePerson and planId = @planId
				  and documentType = @documentType and courtesy = @courtesy
		end
	end

	if (@action = 'InsertTable')
	begin
		insert into tblWhiteList (id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
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
		from @tblWhiteList
	end

	IF (@action = 'GetUserById')
	BEGIN
		
		SET @strTipoPersona = (SELECT TOP(1) typePerson FROM tblWhiteList WHERE id = @id)
		
		IF(@strTipoPersona = 'Cliente')
		BEGIN 

			SELECT	intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					CASE 
						WHEN lastEntry IS NULL THEN (SELECT TOP 1 entryDate FROM tblEvent WHERE clientId = @id ORDER BY modifiedDate DESC)
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
					convert(varchar(10),SUBSTRING(classSchedule,12,22),108) as 'horaFinal'
			INTO	#tmpWhiteList
			FROM	tblWhiteList
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
					cardId
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
			FROM	tblWhiteList
			WHERE	id = @id
			AND		((tblWhiteList.invoiceId = 0 AND tblWhiteList.documentType = 'Cortesía' AND (SELECT COUNT(*) FROM tblEvent WHERE clientId = @id AND planId = 0 AND eventType = 'Entry') < tblWhiteList.availableEntries) OR  tblWhiteList.invoiceId <> 0)

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
						WHEN lastEntry IS NULL THEN (SELECT TOP 1 entryDate FROM tblEvent WHERE clientId = @id ORDER BY modifiedDate DESC)
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
			FROM	tblWhiteList
			WHERE	id = @id

		END

	END

	IF(@action = 'GetUserByIdSinReservas')
	BEGIN

		SET @strTipoPersona = (SELECT TOP(1) typePerson FROM tblWhiteList WHERE id = @id)

		IF (@strTipoPersona = 'Prospecto')
		BEGIN

			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					isnull((select top (1) entryDate from tblEvent where clientId = @id and strFirstMessage <> 'Acceso denegado'  order by entryDate desc ),(lastEntry)) as lastEntry,
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
					cardId
			FROM	tblWhiteList
			WHERE	id = @id
			AND		((tblWhiteList.invoiceId = 0 AND tblWhiteList.documentType = 'Cortesía' AND (SELECT COUNT(*) FROM tblEvent WHERE clientId = @id AND planId = 0 AND eventType = 'Entry') < tblWhiteList.availableEntries) OR  tblWhiteList.invoiceId <> 0)

		END
		ELSE
		BEGIN

			SELECT	TOP(1) intPkId,
					id,
					name,
					planId,
					planName,
					expirationDate,
					isnull((select top (1) entryDate from tblEvent where clientId = @id and strFirstMessage <> 'Acceso denegado'  order by entryDate desc ),(lastEntry)) as lastEntry,
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
					cardId
			FROM	tblWhiteList
			WHERE	id = @id

		END

	END

	IF (@action = 'getInformacionZonas')
	BEGIN

		SELECT	TOP(1)
				id,
				typePerson,
				gymId,
				branchId,
				planId,
				availableEntries,
				expirationDate,
				planName,
				name,
				reserveId
		FROM	tblWhiteList
		WHERE	id = @id

	END

	IF (@action = 'getPlanesPersona')
	BEGIN

		SELECT		invoiceId as [plusu_numero_fact],
					planName as [pla_descripc],
					expirationDate as [plusu_fecha_vcto]
		FROM		tblWhiteList
		WHERE		planId > 0
		AND			id = @id
		GROUP BY	invoiceId, planName, expirationDate
		ORDER BY	planName ASC

	END

	if (@action = 'GetUsersFamilyGroup')
	begin
		select id
		from tblWhiteList
		where typePerson not in ('Empleado','Visitante') and groupId = @groupId
	end

	if (@action = 'UpdateFingerprint')
	begin
		update tblWhiteList
		set fingerprint = @fingerprint, fingerprintId = @fingerprintId,strDatoFoto=@strDatoFoto, updateFingerprint = 1
		where id = @id
	end

	if (@action = 'GetFingerprintsToUpdate')
	begin
		select *
		from tblWhiteList
		where updateFingerprint = 1 and fingerprint is not null
	end

	if (@action = 'FingerprintUpdated')
	begin
		update tblWhiteList
		set updateFingerprint = 0
		where id = @id and fingerprintId = @fingerprintId
	end

	if (@action = 'GetClientWhiteListByCardId')
	begin
		--select *
		--from tblWhiteList
		--where cardId = @cardId and typePerson = 'Cliente'

		select *
		from tblWhiteList wl inner join tblClientCards cc on (wl.id = cc.clientId and wl.typePerson = 'Cliente')
		where cast(cc.cardCode as float) = cast(@cardId as float) and cc.state = 1
	end

	if (@action = 'GetEmployeeWhiteListByCardId')
	begin
		select *
		from tblWhiteList
		where cardId = @cardId and typePerson = 'Empleado'
	end

	if (@action = 'GetClientIdByFingerprintId')
	begin
		select *
		from tblWhiteList
		where fingerprintId = @fingerprintId
	end

	if (@action = 'GetClientIdBioentry')
	begin
		select *, 'huella' as tipo
		from tblWhiteList
		where fingerprintId = convert(float,@id)
		union
		select *, case when withoutFingerprint = 'true' then 'tarjeta' else 'huella' end as tipo
		from tblWhiteList
		where id = @id
		union
		select *, 'tarjeta' as tipo
		from tblWhiteList
		where cardId = @id

	end

	IF (@action = 'DiscountTicket')
	BEGIN
		UPDATE	tblWhiteList
		SET		availableEntries = availableEntries - 1
		WHERE	intPkId = @intPkId
		--AND		id = @id
	END

	if	(@action = 'ActualizarTiquetesWeb')
	begin

	select id , invoiceId ,dianId , documentType ,availableEntries,gymId,branchId
	from tblWhiteList
	WHERE	intPkId = @intPkId
	end 

	if (@action = 'GetFingerprintsById')
	begin
		set @strSql = 'select *
					   from tblWhiteList
					   where fingerprint is not null and (id like ''%' + @id + ''' or ' + @id + ' = ''0'')'

		exec(@strSql)
	end

	if (@action = 'InsertOrUpdateWhiteList')
	begin
		declare @quantity int = (select count(*)
								 from @tblWhiteList),
				@aux int = 0,
				@exist bit = 0,
				@ins bit = 0, @upd bit = 0,
				@del bit = 0

		declare cursor_iudWhiteList cursor for
			select *
			from @tblWhiteList
		open cursor_iudWhiteList

		while (@aux < @quantity)
		begin
			fetch next from cursor_iudWhiteList
			into @intPkId,@id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,
				 @restrictions,@branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,
				 @updateFingerprint,@know,@courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
			     @classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
			     @employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
			     @subgroupId,@cardId,@ins,@upd,@del
			set @exist = isnull((select top 1 1 from tblWhiteList where intPkId = @intPkId),0)

			if (@ins = 1)
			begin
				insert into tblWhiteList (id,name,planId,planName,expirationDate,lastEntry,planType,typePerson,availableEntries,restrictions,
										  branchId,branchName,gymId,personState,withoutFingerprint,fingerprintId,fingerprint,strDatoFoto,updateFingerprint,know,
										  courtesy,groupEntriesControl,groupEntriesQuantity,groupId,isRestrictionClass,
										  classSchedule,dateClass,reserveId,className,utilizedMegas,utilizedTickets,
										  employeeName,classIntensity,classState,photoPath,invoiceId,dianId,documentType,
										  subgroupId,cardId)
				values (@id,@name,@planId,@planName,@expirationDate,@lastEntry,@planType,@typePerson,@availableEntries,@restrictions,
					    @branchId,@branchName,@gymId,@personState,@withoutFingerprint,@fingerprintId,@fingerprint,@strDatoFoto,@updateFingerprint,@know,
					    @courtesy,@groupEntriesControl,@groupEntriesQuantity,@groupId,@isRestrictionClass,
					    @classSchedule,@dateClass,@reserveId,@className,@utilizedMegas,@utilizedTickets,
					    @employeeName,@classIntensity,@classState,@photoPath,@invoiceId,@dianId,@documentType,
					    @subgroupId,@cardId)
			end
			else if (@upd = 1)
			begin
				update tblWhiteList
				set availableEntries = @availableEntries, restrictions = @restrictions, withoutFingerprint = @withoutFingerprint,
					fingerprintId = @fingerprintId, fingerprint = @fingerprint,strDatoFoto=@strDatoFoto, groupEntriesControl = @groupEntriesControl,
					groupEntriesQuantity = @groupEntriesQuantity, groupId = @groupId, isRestrictionClass = @isRestrictionClass,
					classSchedule = @classSchedule, dateClass = @dateClass, reserveId = @reserveId, className = @className,
					utilizedMegas = @utilizedMegas, utilizedTickets = @utilizedTickets, employeeName = @employeeName, 
					classIntensity = @classIntensity, classState = @classState, photoPath = @photoPath,
					subgroupId = @subgroupId, cardId = @cardId
				where intPkId = @intPkId
			end
			else if (@del = 1)
			begin
				delete
				from tblWhiteList
				where intPkId = @intPkId
			end

			set @aux = @aux + 1
		end

		close cursor_iudWhiteList
		deallocate cursor_iudWhiteList
	end

	if (@action = 'UpdateFingerprintID')
	begin
		update tblWhiteList
		set fingerprint = @fingerprint, fingerprintId = @fingerprintId
		where id = @id
	end

	if (@action = 'ConsultarZonas')
	begin
		select *
		from tbl_Maestro_Zonas 
		where id in (SELECT CONVERT(INT, value) FROM STRING_SPLIT(@id, ','))
	end

	if (@action = 'fingerprintReplitDelete')
	begin
		--delete from tblReplicatedFingerprint where userId = @id and bitDelete = 'true'
		--delete from tblReplicatedCardPing where bitEliminado = 'true' and idPerson = @id

			delete from tblReplicatedFingerprint where userId = @id 
		delete from tblReplicatedCardPing where  idPerson = @id

	end

	if (@action = 'agregarHorariosRestriccion')
	begin

		if not exists (select * from tblHorrariosRestriccion where strHoraEntrada = @horaEntrada and strHoraSalida = @horaSalida )
		begin
			insert into tblHorrariosRestriccion (strHoraEntrada, strHoraSalida)
			values (@horaEntrada, @horaSalida)
		end
	end

	if (@action = 'ConsultarHorariosRestriccion')
	begin
		select  HR.id as id, HR.strHoraEntrada, HR.strHoraSalida
		from tblHorrariosRestriccion HR
		
	end

	if (@action = 'ConsultarHorariosRestriccionPorDiaHora')
	begin
		select  HR.id as id, HR.strHoraEntrada, HR.strHoraSalida
		from tblHorrariosRestriccion HR
		where HR.strHoraEntrada = @horaEntrada
			and HR.strHoraSalida = @horaSalida
	end

	if(@action = 'RemoverListaContratos')
	begin

	select distinct clientId 
		from tblEvent
		inner join  tblWhiteList 
		on clientId = id 
		where fingerprintId = convert(float,@id)
		and tblEvent.strSecondMessage = 'No puede ingresar, Debe firmar el contrato.'
		and convert(varchar(10),entryDate,120) = convert(varchar(10),getdate(),120)
		union
			select distinct clientId 
		from tblEvent
		inner join  tblWhiteList 
		on clientId = id 
		where id = @id
				and tblEvent.strSecondMessage = 'No puede ingresar, Debe firmar el contrato.'
		and convert(varchar(10),entryDate,120) = convert(varchar(10),getdate(),120)
		union
			select distinct clientId 
		from tblEvent
		inner join  tblWhiteList 
		on clientId = id 
		where cardId = @id
		and tblEvent.strSecondMessage = 'No puede ingresar, Debe firmar el contrato.'
		and convert(varchar(10),entryDate,120) = convert(varchar(10),getdate(),120)

	end 

	if (@action = 'obtenerIdVisitanteEliminar')
	begin
		select clientId 
		from tblEvent
		inner join  tblWhiteList
		on clientId = id 
		inner join tblTerminal 
		on tblTerminal.ipAddress = tblEvent.ipAddress
		where fingerprintId = convert(float,@id)
		and typePerson in ('Prospecto', 'Visitante')
		and Zonas = 0
		union
		select clientId 
		from tblEvent
		inner join  tblWhiteList 
		on clientId = id 
		inner join tblTerminal 
		on tblTerminal.ipAddress = tblEvent.ipAddress
		where id = @id
		and typePerson in ('Prospecto', 'Visitante')
		and Zonas = 0
		union
		select clientId 
		from tblEvent
		inner join  tblWhiteList 
		on clientId = id 
		inner join tblTerminal 
		on tblTerminal.ipAddress = tblEvent.ipAddress
		where cardId = @id
		and typePerson in ('Prospecto', 'Visitante')
		and Zonas = 0
	end

	if(@action = 'dReplicaUsuariosBioLiteRestriccion')
	begin
	select *, 0 as 'esClase','' as 'HoraInicial','' as 'HoraFinal'
		from tblWhiteList
		where typePerson = 'Cliente'
		and len(restrictions) > 27
		union
	select *, 1 as 'esClase',SUBSTRING (classSchedule,0,6) as 'HoraInicial',SUBSTRING (classSchedule,12,5) as 'HoraFinal'
		from tblWhiteList
		where typePerson = 'Cliente'
		and convert(varchar(10),dateClass,120) = convert(varchar(10),getdate(),120)
	end 

	if(@action = 'RemoverListaPersonas')
	begin
			delete 
			from tblReplicatedFingerprint 
			where fingerprintId = @id 
			and exists (select * from tblWhiteList where len(restrictions) > 27 or reserveId > 0)
		
			delete 
			from tblReplicatedCardPing 
			where strTarjeta = @id
			and exists (select * from tblWhiteList where len(restrictions) > 27 or reserveId > 0)
	end

END
GO

if not exists (select * from sys.objects where name = 'tblFingerprint' and type in (N'U'))
begin
	create table tblFingerprint(
		intPkId int not null identity(1,1) primary key,
		personId varchar(50) not null,
		fingerprintId int not null,
		fingerprint binary(2000) not null,
		strDatoFoto varchar(max) null,
		bitInsert bit not null,
		bitDelete bit not null,
		registerDate datetime not null,
		bitUsed bit not null
	)
end
go

if exists (select * from sys.objects where name = 'spFingerprint' and type in (N'P'))
begin
	drop procedure spFingerprint
end
go

IF EXISTS(SELECT * FROM sys.types WHERE name = 'type_tblFingerprint')
BEGIN

	DROP TYPE [dbo].[type_tblFingerprint]

END
GO

/****** Object:  UserDefinedTableType [dbo].[type_tblFingerprint]    Script Date: 25/01/2023 08:06:06 a. m. ******/
CREATE TYPE [dbo].[type_tblFingerprint] AS TABLE(
	[personId] [varchar](50) NULL,
	[fingerprintId] [int] NULL,
	[fingerprint] [binary](2000) NULL,
	[strDatoFoto] [varchar](max) NULL,
	[bitInsert] [bit] NULL,
	[bitDelete] [bit] NULL,
	[registerDate] [datetime] NULL,
	[intIndiceHuellaActual] [int] NULL
)
GO

IF Not Exists(SELECT * FROM syscolumns AS sc INNER JOIN sysobjects AS so ON sc.id=so.id AND sc.name='intIndiceHuellaActual' AND so.name='tblFingerprint')
BEGIN
	ALTER TABLE tblFingerprint ADD intIndiceHuellaActual nvarchar(10) NULL
END
GO

WHILE (exists (select * from tblFingerprint where intIndiceHuellaActual is null))
BEGIN
	UPDATE TOP (1) tblFingerprint
	SET intIndiceHuellaActual = (select (count(*) + 1) as cantidad from tblFingerprint where intIndiceHuellaActual is not null) 
	WHERE intIndiceHuellaActual is null
END
GO

ALTER TABLE tblFingerprint ALTER COLUMN intIndiceHuellaActual nvarchar(10) NOT NULL
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblLectoresCAMA2000]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblLectoresCAMA2000](
		[intId] [int] IDENTITY(1,1) NOT NULL,
		[strNombreEquipo] [varchar](200) NOT NULL,
		[bitActivo] [bit] NOT NULL
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblHuellasCAMA2000]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[tblHuellasCAMA2000](
		[intpkCodigo] [int] NOT NULL,
		[strpkIdentificacion] [varchar](15) NOT NULL,
		[intNumeroDedo] [int] NOT NULL,
		[binDatoHuella] [binary](2000) NOT NULL,
		[strNombreEquipo] [varchar](200) NOT NULL,
		[bitReplicado] [bit] NOT NULL,
		[dtmFechaCreacion] [datetime] NOT NULL,
		[dtmFechaReplicado] [datetime] NULL,
		[bitEliminar] [bit] NULL,
	CONSTRAINT [PK_tblHuellasCAMA2000] PRIMARY KEY CLUSTERED
	(
		[intpkCodigo] ASC
	)
	) ON [PRIMARY]
END
GO

CREATE PROCEDURE [dbo].[spFingerprint]
@action varchar(50) = '',
@personId varchar(50) = '',
@fingerprintId int = null,
@fingerprint binary(2000) = null,
@strDatoFoto varchar(max)= null,
@bitInsert bit = null,
@bitDelete bit = null,
@registerDate datetime = null,
@bitUsed bit = null,
@ipAddress varchar(100) = '',
@ids varchar(max) = '',
@tblFingerprint type_tblFingerprint readonly,
@exist bit = 0,
@card varchar(max) = '',
@bitEliminado bit = null,
@intIndiceHuellaActual INT = 0
as
begin
	declare @strSql varchar(max)

	if (@action = 'InsertTable')
	begin
		declare @quantity int = (select count(*)
								 from @tblFingerprint),
				@aux int = 0
				
		declare cursor_fingerprintsR cursor for
			select personId,fingerprintId,fingerprint,strDatoFoto,bitInsert,bitDelete,registerDate,intIndiceHuellaActual
			from @tblFingerprint
		open cursor_fingerprintsR

		while (@aux < @quantity)
		begin
			fetch next from cursor_fingerprintsR
			into @personId,@fingerprintId,@fingerprint,@strDatoFoto,@bitInsert,@bitDelete,@registerDate,@intIndiceHuellaActual
			set @exist = isnull((select top 1 1 from tblFingerprint where personId = @personId and fingerprintId = @fingerprintId),0)

			if (@exist = 1)
			begin
				--ACTUALIZAMOS EL USUARIO PARA QUE SEA ACTUALIZADO EN LAS TERMINALES
				update tblFingerprint
				set fingerprint = @fingerprint,strDatoFoto=@strDatoFoto, bitInsert = @bitInsert, bitDelete = @bitDelete
				where personId = @personId and fingerprintId = @fingerprintId
			end
			else
			begin
				insert into tblFingerprint(personId,fingerprintId,fingerprint,strDatoFoto,bitInsert,bitDelete,registerDate,bitUsed,intIndiceHuellaActual)
				values (@personId,@fingerprintId,@fingerprint,@strDatoFoto,@bitInsert,@bitDelete,getdate(),0,@intIndiceHuellaActual)
			end

			set @aux = @aux + 1
		end

		close cursor_fingerprintsR
		deallocate cursor_fingerprintsR
	end

	if (@action = 'Insert')
	begin
		set @exist = isnull((select top 1 1 from tblFingerprint where personId = @personId and fingerprintId = @fingerprintId),0)

		if (@exist = 1)
		begin
			update tblFingerprint
			set fingerprint = @fingerprint,strDatoFoto=@strDatoFoto, bitInsert = @bitInsert, bitDelete = @bitDelete, registerDate = getdate()
			where personId = @personId and fingerprintId = @fingerprintId
		end
		else
		begin
			insert into tblFingerprint(personId,fingerprintId,fingerprint,strDatoFoto,bitInsert,bitDelete,registerDate,bitUsed, intIndiceHuellaActual)
			values (@personId,@fingerprintId,@fingerprint,@strDatoFoto,@bitInsert,@bitDelete,@registerDate,@bitUsed, (select ISNULL(MAX(CONVERT(int,[intIndiceHuellaActual])),0) + 1 as Ultimo_ID from tblFingerprint))
		end

		delete from tblReplicatedFingerprint where userId = @personId and fingerprintId = @fingerprintId 
	end

	if (@action = 'GetFingerprintsByUser')
	begin
		set @strSql = 'select id, fingerprint, fingerprintId,strDatoFoto
					   from tblWhiteList
					   where fingerprint is not null and id like ''%' + @personId + '%'''
		
		exec (@strSql)
	end

	if (@action = 'GetFingerprints')
	begin
		select *
		from tblFingerprint
		where bitUsed != 1
		order by fingerprintId desc
	end

	if (@action = 'GetFingerprintsToReplicate')
	begin
		select fin.*, whiteList.typePerson, whiteList.planId, whiteList.gymId as gymIdWL, whiteList.branchId as branchIdWL
		, whiteList.restrictions, whiteList.reserveId, whiteList.cardId, whiteList.withoutFingerprint
		from tblFingerprint fin
		inner join tblWhiteList whiteList on fin.personId = whiteList.id
		where fin.fingerprintId not in (select rf.fingerprintId
										from tblReplicatedFingerprint rf
										where rf.ipAddress = @ipAddress)
			  and fin.bitInsert = 1 --and fin.bitUsed = 0
	end

	if (@action = 'GetTarjetasReplica')
	begin
		--select * 
		--from tblWhiteList 
		--where (withoutFingerprint = 'true' or (cardId != '' and cardId != ' ' and cardId != '0' and cardId is not null))
		--and Id not in (select rf.idPerson from tblReplicatedCardPing rf where rf.strIP = @ipAddress)
		--and (fingerprintId = 0 or fingerprintId is null or fingerprintId = '' or fingerprintId = ' ')
		----and cardId is not null and cardId <> '' and cardId <> '0'

		select * 
		from tblWhiteList 
		where (withoutFingerprint = 'true' or (cardId != '' and cardId != ' ' and cardId != '0' and cardId is not null))
		and Id not in (select rf.idPerson from tblReplicatedCardPing rf where rf.strIP = @ipAddress)
		--and (fingerprintId = 0 or fingerprintId is null or fingerprintId = '' or fingerprintId = ' ')
		--and cardId is not null and cardId <> '' and cardId <> '0'
	end

	if (@action = 'GetFingerprintsToDelete')
	begin
		select *
		from tblFingerprint
		where bitDelete = 1 and fingerprintId in (select fingerprintId from tblReplicatedFingerprint where ipAddress = @ipAddress)
	end

	if (@action = 'UpdateFingerprintDeleted')
	begin
		update tblFingerprint
		set bitUsed = 1
		where fingerprintId = @fingerprintId 
	end

	if (@action = 'Get')
	begin
		select *
		from tblFingerprint
		where fingerprintId = @fingerprintId
	end

	if (@action = 'UpdateUsedFingerprints')
	begin
		set @strSql = 'update tblFingerprint
					   set bitUsed = 1
					   where intPkId in (' + @ids + ')'
		exec (@strSql)
	end

	if	(@action = 'ContarEliminar')
	begin
		select tblFingerprint.*, tblWhiteList.id 
		from tblFingerprint
		left join tblWhiteList on personId = id  and tblFingerprint.fingerprintId = tblWhiteList.fingerprintId
		inner join tblReplicatedFingerprint on tblFingerprint.fingerprintId = tblReplicatedFingerprint.fingerprintId
			and tblFingerprint.personId = tblReplicatedFingerprint.userId
		where id is null and tblReplicatedFingerprint.bitDelete = 0
		and tblReplicatedFingerprint.ipAddress = @ipAddress
	end 

	if	(@action = 'CambiarEstadoContarEliminar')
	begin
		update tblReplicatedFingerprint
		set bitDelete = 1
		from tblFingerprint 
		inner join tblReplicatedFingerprint on tblFingerprint.fingerprintId = tblReplicatedFingerprint.fingerprintId
		and tblFingerprint.personId = tblReplicatedFingerprint.userId
		where personId = @personId
		and tblReplicatedFingerprint.ipAddress = @ipAddress 

		update tblReplicatedCardPing
		set bitEliminado = 1
		where idPerson = @personId
		and strIP = @ipAddress 

	end 
		if	(@action = 'insertTarjetas')
	begin
		if not exists (select * from tblReplicatedCardPing 
					   where idPerson = @personId and strTarjeta = @card and strIP = @ipAddress)
			begin
		insert into tblReplicatedCardPing (idPerson,strTarjeta,bitEliminado,strIP)
		values (@personId ,@card , @bitEliminado, @ipAddress)
		end 

	end 

			if	(@action = 'ContarTarjetasEliminar')
	begin
		select tblReplicatedCardPing.idPerson,
				case 
					when len(tblReplicatedCardPing.strTarjeta) <= 5 then 
						case when len(tblReplicatedCardPing.strTarjeta) = 0 
						then 
							tblReplicatedCardPing.idPerson
						else 
							tblReplicatedCardPing.strTarjeta+'9999'
						end
					else tblReplicatedCardPing.strTarjeta end as strTarjeta,
			tblReplicatedCardPing.bitEliminado,
				tblReplicatedCardPing.strIP, tblWhiteList.id 
		from tblReplicatedCardPing
		left join tblWhiteList on tblReplicatedCardPing.idPerson = id --and strTarjeta = iif(isnull(cardId,0) = 0, id, cardId)
		where tblWhiteList.id is null and tblReplicatedCardPing.bitEliminado = 0
		and strIp = @ipAddress
	end

	if	(@action = 'CambiarEstadoEliminacionTarjetas')
	begin

		delete from tblReplicatedCardPing 
		where idPerson = @personId
		and strIP = @ipAddress
	end 

	if (@action = 'GetAllFingerprints')
	begin
		select *
		from tblFingerprint
		where (personId like '%' + @personId + '%' or @personId = '0')
		and bitDelete = 0
	end

end

go

if not exists (select * from sys.objects where name = 'tblConfiguration' and type in (N'U'))
begin
	create table tblConfiguration(
		intPkId int not null identity (1,1) primary key,
		bitSucActiva bit NULL,
		intfkSucursal int NULL,
		strTipoIngreso varchar(50) NULL,
		intMinutosDescontarTiquetes int NULL,
		bitIngresoEmpSinPlan bit NULL,
		blnPermitirIngresoPantalla bit NULL,
		blnPermiteIngresosAdicionales bit NULL,
		intMinutosNoReingreso int NULL,
		blnLimpiarDescripcionAdicionales bit NULL,
		blnEntradaCumpleConPlan bit NULL,
		blnEntradaCumpleSinPlan bit NULL,
		intDiasGraciaClientes int NULL,
		bitBloqueoCitaNoCumplidaMSW bit NULL,
		bitBloqueoClienteNoApto bit NULL,
		bitBloqueoNoDisentimento bit NULL,
		bitBloqueoNoAutorizacionMenor bit NULL,
		bitConsultaInfoCita bit NULL,
		bitImprimirHoraReserva bit NULL,
		bitTiqueteClaseAsistido_alImprimir bit NULL,
		bitAccesoPorReservaWeb bit NULL,
		bitValidarPlanYReservaWeb bit NULL,
		bitValideContrato bit NULL,
		bitValideContratoPorFactura bit NULL,
		intMinutosAntesReserva int NULL,
		intMinutosDespuesReserva int NULL,
		intdiassincita_bloqueoing int NULL,
		intentradas_sincita_bloqueoing int NULL,
		bitIngresoTecladoHuella bit NULL,
		strClave varchar(50) NULL,
		strNivelSeguridad varchar(50) NULL,
		bitPermitirBorrarHuella bit NULL,
		bitIngresoMiniTouch bit NULL,
		bitTrabajarConDBEnOtroEquipo bit NULL,
		strRutaNombreLogo varchar(max) NULL,
		intUbicacionTeclasTeclado int NULL,
		bitBaseDatosSQLServer bit NULL,
		intTiempoParaLimpiarPantalla int NULL,
		intTipoUbicacionTeclado int NULL,
		strRutaNombreBanner varchar(max) NULL,
		bitIngresoAbreDesdeTouch bit NULL,
		intNivelSeguirdadLectorUSB int NULL,
		intTimeOutLector int NULL,
		intTiempoDuermeHiloEnviaComandoSalida int NULL,
		bitMensajeCumpleanos bit NULL,
		intTiempoActualizaIngresos int NULL,
		intTiempoValidaSiAbrePuerta int NULL,
		strRutaArchivoGymsoftNet varchar(max) NULL,
		bitComplices_CortxIngs bit NULL,
		bitComplices_DescuentoTiq bit NULL,
		strTextoMensajeCumpleanos varchar(max) NULL,
		bitSolo1HuellaxCliente bit NULL,
		bitMultiplesPlanesVig bit NULL,
		intComplices_Plan_CortxIngs int NULL,
		strTextoMensajeCortxIngs varchar(max) NULL,
		bitImagenSIBO bit NULL,
		strColorPpal varchar(50) NULL,
		strRutaGuiaMarcacion varchar(max) NULL,
		intPasoFinalAutoregistroWeb int NULL,
		bitAntipassbackEntrada bit NULL,
		bitReplicaImgsTCAM7000 bit NULL,
		intTiempoReplicaImgsTCAM7000 int NULL,
		bitAccesoDiscapacitados bit NULL,
		intTiempoEspaciadoTramasReplicaImgsTCAM7000 int NULL,
		intTiempoMaximoEnvioImgsTCAM7000 int NULL,
		intCalidadHuella int NULL,
		bitEsperarHuellaActualizar bit NULL,
		bitGenerarContratoPDFyEnviar bit NULL,
		intNumeroDiasActualizacionHuella int NULL,
		bitNoValidarHuella bit NULL,
		intTiempoReplicaHuellasTCAM7000 int NULL,
		intTiempoPingContinuoTCAM7000 int NULL,
		strTipoIdenTributaria varchar(50) NULL,
		bitNo_Validar_Entrada_En_Salida bit NULL,
		strIdentificadorUno varchar(50) NULL,
		strIdentificadorDos varchar(50) NULL,
		strPuertoCom varchar(50) NULL,
		intVelocidadPuerto int NULL,
		intTiempoPulso int NULL,
		bitBloqueoCita bit NULL,
		intDiasEntreCitaRiesgo int NULL,
		intDiasEntreCitaNoRiesgo int NULL,
		bitCambiarEstadoTiqueteClase bit NULL,
		bitFirmarContratoAlEnrolar bit null,
		intRangoHoraPrint int null,
		timeGetWhiteList int null,
		timeInsertEntries int null,
		timeGetConfiguration int null,
		timeGetClientMessages int null,
		timeGetTerminals int null,
		strPuertoComSalida varchar(50) null,
		bitConsentimientoInformado bit null,
		bitConsentimientoDatosBiometricos bit null,
		bitDatosVirtualesUsuario bit null,
		bitValidarHuellaMarcacionSDKAPI bit null,
		intTimeoutValidarHuellaMarcacionSDKAPI int null,
		bitValidarConfiguracionIngresoWeb bit null,
		SMTPServer varchar(500) null,
		strUser varchar(500) null,
		strPassword varchar(500) null,
		strPort varchar(100) null,
		timeGetPendingActions int null,
		timeRemoveFingerprints int null,
		timeInsertWhiteListOnTCAM int null default(0),
		allowWhiteListOnTCAM bit null default(0),
		timeTerminalConnections int null default(0),
		intTiempoEsperaRespuestaReplicaHuellasTCAM7000 int null default(0),
		timeWaitResponseReplicateUsers int null default(0),
		timeWaitResponseDeleteFingerprint int null default(0),
		timeWaitResponseDeleteUser int null default(0),
		timeResetDownloadEvents int null default(0),
		timeRemoveUsers int null default(0),
		timeDowndloadEvents int null default(0),
		timeHourSync int null default(0),
		quantityAppAccessControlSimultaneous int null default(0)
	)
end
go

--Incidente 0005921 Mtoro
if not exists (select * 
               from sys.columns 
               where name = 'Validación_de_huella_de_marcación_con_SDK_del_webserver' and object_id = OBJECT_ID('tblConfiguration'))
begin
    alter table tblConfiguration add Validación_de_huella_de_marcación_con_SDK_del_webserver bit null
end
go

if not exists (select * 
               from sys.columns 
               where name = 'Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver' and object_id = OBJECT_ID('tblConfiguration'))
begin
    alter table tblConfiguration add Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver int null
end
go
--FIN Incidente 0005921 Mtoro

--CREACION DE CAMPO JMARIN

if not exists (select * 
               from sys.columns 
               where name = 'bitGenerarCodigoQRdeingresoparavalidarLocalmente' and object_id = OBJECT_ID('tblConfiguration'))
begin
    alter table tblConfiguration add bitGenerarCodigoQRdeingresoparavalidarLocalmente BIT DEFAULT 0
end
go


if not exists (select * 
               from sys.columns 
               where name = 'bitLectorBiometricoSiempreEncendido' and object_id = OBJECT_ID('tblConfiguration'))
begin
    alter table tblConfiguration add bitLectorBiometricoSiempreEncendido BIT not null DEFAULT 1
end
go

if not exists (select * from sys.columns where name = 'intMinutosNoReingresoDia' and object_id = OBJECT_ID('tblConfiguration'))
begin
    ALTER TABLE tblConfiguration ADD intMinutosNoReingresoDia int default(0);
end
go


if exists (select * from sys.objects where name = 'spConfiguration' and type in (N'P'))
begin
	drop procedure spConfiguration
end
go

CREATE procedure [dbo].[spConfiguration]
@action varchar(50) = '',
@intPkId int = null,
@bitSucActiva bit = null,
@intfkSucursal int = null,
@strTipoIngreso varchar(50) = '',
@intMinutosDescontarTiquetes int = null,
@bitIngresoEmpSinPlan bit = null,
@blnPermitirIngresoPantalla bit = null,
@blnPermiteIngresosAdicionales bit = null,
@intMinutosNoReingreso int = null,
@intMinutosNoReingresoDia int = null,
@blnLimpiarDescripcionAdicionales bit = null,
@blnEntradaCumpleConPlan bit = null,
@blnEntradaCumpleSinPlan bit = null,
@intDiasGraciaClientes int = null,
@bitBloqueoCitaNoCumplidaMSW bit = null,
@bitBloqueoClienteNoApto bit = null,
@bitBloqueoNoDisentimento bit = null,
@bitBloqueoNoAutorizacionMenor bit = null,
@bitConsultaInfoCita bit = null,
@bitImprimirHoraReserva bit = null,
@bitTiqueteClaseAsistido_alImprimir bit = null,
@bitAccesoPorReservaWeb bit = null,
@bitValidarPlanYReservaWeb bit = null,
@bitValideContrato bit = null,
@bitValideContratoPorFactura bit = null,
@intMinutosAntesReserva int = null,
@intMinutosDespuesReserva int = null,
@intdiassincita_bloqueoing int = null,
@intentradas_sincita_bloqueoing int = null,
@bitIngresoTecladoHuella bit = null,
@strClave varchar(50) = '',
@strNivelSeguridad varchar(50) = '',
@bitPermitirBorrarHuella bit = null,
@bitIngresoMiniTouch bit = null,
@bitTrabajarConDBEnOtroEquipo bit = null,
@strRutaNombreLogo varchar(max) = '',
@intUbicacionTeclasTeclado int = null,
@bitBaseDatosSQLServer bit = null,
@intTiempoParaLimpiarPantalla int = null,
@intTipoUbicacionTeclado int = null,
@strRutaNombreBanner varchar(max) = '',
@bitIngresoAbreDesdeTouch bit = null,
@intNivelSeguirdadLectorUSB int = null,
@intTimeOutLector int = null,
@intTiempoDuermeHiloEnviaComandoSalida int = null,
@bitMensajeCumpleanos bit = null,
@intTiempoActualizaIngresos int = null,
@intTiempoValidaSiAbrePuerta int = null,
@strRutaArchivoGymsoftNet varchar(max) = '',
@bitComplices_CortxIngs bit = null,
@bitComplices_DescuentoTiq bit = null,
@strTextoMensajeCumpleanos varchar(max) = '',
@bitSolo1HuellaxCliente bit = null,
@bitMultiplesPlanesVig bit = null,
@intComplices_Plan_CortxIngs int = null,
@strTextoMensajeCortxIngs varchar(max) = '',
@bitImagenSIBO bit = null,
@strColorPpal varchar(50) = '',
@strRutaGuiaMarcacion varchar(max) = '',
@intPasoFinalAutoregistroWeb int = null,
@bitAntipassbackEntrada bit = null,
@bitReplicaImgsTCAM7000 bit = null,
@intTiempoReplicaImgsTCAM7000 int = null,
@bitAccesoDiscapacitados bit = null,
@intTiempoEspaciadoTramasReplicaImgsTCAM7000 int = null,
@intTiempoMaximoEnvioImgsTCAM7000 int = null,
@intCalidadHuella int = null,
@bitEsperarHuellaActualizar bit = null,
@bitGenerarContratoPDFyEnviar bit = null,
@intNumeroDiasActualizacionHuella int = null,
@bitNoValidarHuella bit = null,
@intTiempoReplicaHuellasTCAM7000 int = null,
@intTiempoPingContinuoTCAM7000 int = null,
@strTipoIdenTributaria varchar(50) = '',
@bitNo_Validar_Entrada_En_Salida bit = null,
@strIdentificadorUno varchar(50) = '',
@strIdentificadorDos varchar(50) = '',
@strPuertoCom varchar(50) = '',
@intVelocidadPuerto int = null,
@intTiempoPulso int = null,
@bitBloqueoCita bit = null,
@intDiasEntreCitaRiesgo int = null,
@intDiasEntreCitaNoRiesgo int = null,
@bitCambiarEstadoTiqueteClase bit = null,
@bitFirmarContratoAlEnrolar bit = null,
@intRangoHoraPrint int = null,
@timeGetWhiteList int = null,
@timeInsertEntries int = null,
@timeGetConfiguration int = null,
@timeGetClientMessages int = null,
@timeGetTerminals int = null,
@strPuertoComSalida varchar(50) = '',
@bitConsentimientoInformado bit = null,
@bitConsentimientoDatosBiometricos bit = null,
@bitDatosVirtualesUsuario bit = null,
@bitValidarHuellaMarcacionSDKAPI bit = null,
@Validación_de_huella_de_marcación_con_SDK_del_webserver bit = null,
@intTimeoutValidarHuellaMarcacionSDKAPI int = null,
@Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver int = null,
@bitValidarConfiguracionIngresoWeb bit = null,
@SMTPServer varchar(500) = '',
@user varchar(500) = '',
@password varchar(500) = '',
@port varchar(100) = '',
@timeGetPendingActions int = null,
@timeRemoveFingerprints int = null,
@timeInsertWhiteListOnTCAM int = null,
@allowWhiteListOnTCAM bit = null,
@timeTerminalConnections int = null,
@intTiempoEsperaRespuestaReplicaHuellasTCAM7000 int = null,
@timeWaitResponseReplicateUsers int = null,
@timeWaitResponseDeleteFingerprint int = null,
@timeWaitResponseDeleteUser int = null,
@timeResetDownloadEvents int = null,
@timeRemoveUsers int = null,
@timeDowndloadEvents int = null,
@timeHourSync int = null,
@quantityAppAccessControlSimultaneous int = null, 
@bitGenerarCodigoQRdeingresoparavalidarLocalmente  bit = null,
@bitLectorBiometricoSiempreEncendido  bit = null
as
begin
	if (@action = 'Insert')
	begin
		insert into tblConfiguration(bitSucActiva,intfkSucursal,strTipoIngreso,intMinutosDescontarTiquetes,
									 bitIngresoEmpSinPlan,blnPermitirIngresoPantalla,blnPermiteIngresosAdicionales,
									 intMinutosNoReingreso,blnLimpiarDescripcionAdicionales,blnEntradaCumpleConPlan,
									 blnEntradaCumpleSinPlan,intDiasGraciaClientes,bitBloqueoCitaNoCumplidaMSW,
									 bitBloqueoClienteNoApto,bitBloqueoNoDisentimento,bitBloqueoNoAutorizacionMenor,
									 bitConsultaInfoCita,bitImprimirHoraReserva,bitTiqueteClaseAsistido_alImprimir,
									 bitAccesoPorReservaWeb,bitValidarPlanYReservaWeb,bitValideContrato,
									 bitValideContratoPorFactura,intMinutosAntesReserva,intMinutosDespuesReserva,
									 intdiassincita_bloqueoing,intentradas_sincita_bloqueoing,bitIngresoTecladoHuella,
									 strClave,strNivelSeguridad,bitPermitirBorrarHuella,bitIngresoMiniTouch,
									 bitTrabajarConDBEnOtroEquipo,strRutaNombreLogo,intUbicacionTeclasTeclado,
									 bitBaseDatosSQLServer,intTiempoParaLimpiarPantalla,intTipoUbicacionTeclado,
									 strRutaNombreBanner,bitIngresoAbreDesdeTouch,intNivelSeguirdadLectorUSB,intTimeOutLector,
									 intTiempoDuermeHiloEnviaComandoSalida,bitMensajeCumpleanos,intTiempoActualizaIngresos,
									 intTiempoValidaSiAbrePuerta,strRutaArchivoGymsoftNet,bitComplices_CortxIngs,
									 bitComplices_DescuentoTiq,strTextoMensajeCumpleanos,bitSolo1HuellaxCliente,
									 bitMultiplesPlanesVig,intComplices_Plan_CortxIngs,strTextoMensajeCortxIngs,bitImagenSIBO,
									 strColorPpal,strRutaGuiaMarcacion,intPasoFinalAutoregistroWeb,bitAntipassbackEntrada,
									 bitReplicaImgsTCAM7000,intTiempoReplicaImgsTCAM7000,bitAccesoDiscapacitados,
									 intTiempoEspaciadoTramasReplicaImgsTCAM7000,intTiempoMaximoEnvioImgsTCAM7000,
									 intCalidadHuella,bitEsperarHuellaActualizar,bitGenerarContratoPDFyEnviar,
									 intNumeroDiasActualizacionHuella,bitNoValidarHuella,intTiempoReplicaHuellasTCAM7000,
									 intTiempoPingContinuoTCAM7000,strTipoIdenTributaria,bitNo_Validar_Entrada_En_Salida,
									 strIdentificadorUno,strIdentificadorDos,strPuertoCom,intVelocidadPuerto,intTiempoPulso,
									 bitBloqueoCita,intDiasEntreCitaRiesgo,intDiasEntreCitaNoRiesgo,bitCambiarEstadoTiqueteClase,
									 timeGetWhiteList,timeInsertEntries,bitFirmarContratoAlEnrolar,
									 intRangoHoraPrint,timeGetConfiguration,timeGetClientMessages,timeGetTerminals,strPuertoComSalida,
									 bitConsentimientoDatosBiometricos,bitConsentimientoInformado,bitDatosVirtualesUsuario,
									 bitValidarHuellaMarcacionSDKAPI,Validación_de_huella_de_marcación_con_SDK_del_webserver,
									 intTimeoutValidarHuellaMarcacionSDKAPI,Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver,
									 bitValidarConfiguracionIngresoWeb,
									 SMTPServer,strUser,strPassword,strPort,timeGetPendingActions,timeRemoveFingerprints,
									 timeInsertWhiteListOnTCAM,allowWhiteListOnTCAM,timeTerminalConnections,intTiempoEsperaRespuestaReplicaHuellasTCAM7000,
									 timeWaitResponseReplicateUsers,timeWaitResponseDeleteFingerprint,timeWaitResponseDeleteUser,
									 timeResetDownloadEvents,timeRemoveUsers,timeDowndloadEvents,timeHourSync,quantityAppAccessControlSimultaneous, bitGenerarCodigoQRdeingresoparavalidarLocalmente,
									 bitLectorBiometricoSiempreEncendido,intMinutosNoReingresoDia)
		values(@bitSucActiva,@intfkSucursal,@strTipoIngreso,@intMinutosDescontarTiquetes,
			   @bitIngresoEmpSinPlan,@blnPermitirIngresoPantalla,@blnPermiteIngresosAdicionales,
			   @intMinutosNoReingreso,@blnLimpiarDescripcionAdicionales,@blnEntradaCumpleConPlan,
			   @blnEntradaCumpleSinPlan,@intDiasGraciaClientes,@bitBloqueoCitaNoCumplidaMSW,
			   @bitBloqueoClienteNoApto,@bitBloqueoNoDisentimento,@bitBloqueoNoAutorizacionMenor,
			   @bitConsultaInfoCita,@bitImprimirHoraReserva,@bitTiqueteClaseAsistido_alImprimir,
			   @bitAccesoPorReservaWeb,@bitValidarPlanYReservaWeb,@bitValideContrato,
			   @bitValideContratoPorFactura,@intMinutosAntesReserva,@intMinutosDespuesReserva,
			   @intdiassincita_bloqueoing,@intentradas_sincita_bloqueoing,@bitIngresoTecladoHuella,
			   @strClave,@strNivelSeguridad,@bitPermitirBorrarHuella,@bitIngresoMiniTouch,
			   @bitTrabajarConDBEnOtroEquipo,@strRutaNombreLogo,@intUbicacionTeclasTeclado,
			   @bitBaseDatosSQLServer,@intTiempoParaLimpiarPantalla,@intTipoUbicacionTeclado,
			   @strRutaNombreBanner,@bitIngresoAbreDesdeTouch,@intNivelSeguirdadLectorUSB,
			   @intTimeOutLector,@intTiempoDuermeHiloEnviaComandoSalida,@bitMensajeCumpleanos,
			   @intTiempoActualizaIngresos,@intTiempoValidaSiAbrePuerta,@strRutaArchivoGymsoftNet,
			   @bitComplices_CortxIngs,@bitComplices_DescuentoTiq,@strTextoMensajeCumpleanos,
			   @bitSolo1HuellaxCliente,@bitMultiplesPlanesVig,@intComplices_Plan_CortxIngs,
			   @strTextoMensajeCortxIngs,@bitImagenSIBO,@strColorPpal,@strRutaGuiaMarcacion,
			   @intPasoFinalAutoregistroWeb,@bitAntipassbackEntrada,@bitReplicaImgsTCAM7000,
			   @intTiempoReplicaImgsTCAM7000,@bitAccesoDiscapacitados,@intTiempoEspaciadoTramasReplicaImgsTCAM7000,
			   @intTiempoMaximoEnvioImgsTCAM7000,@intCalidadHuella,@bitEsperarHuellaActualizar,
			   @bitGenerarContratoPDFyEnviar,@intNumeroDiasActualizacionHuella,@bitNoValidarHuella,
			   @intTiempoReplicaHuellasTCAM7000,@intTiempoPingContinuoTCAM7000,@strTipoIdenTributaria,
			   @bitNo_Validar_Entrada_En_Salida,@strIdentificadorUno,@strIdentificadorDos,@strPuertoCom,
			   @intVelocidadPuerto,@intTiempoPulso,@bitBloqueoCita,@intDiasEntreCitaRiesgo,
			   @intDiasEntreCitaNoRiesgo,@bitCambiarEstadoTiqueteClase,
			   @timeGetWhiteList,@timeInsertEntries,@bitFirmarContratoAlEnrolar,@intRangoHoraPrint,
			   @timeGetConfiguration,@timeGetClientMessages,@timeGetTerminals,@strPuertoComSalida,
			   @bitConsentimientoDatosBiometricos,@bitConsentimientoInformado,@bitDatosVirtualesUsuario,
			   @bitValidarHuellaMarcacionSDKAPI,@Validación_de_huella_de_marcación_con_SDK_del_webserver,
			   @intTimeoutValidarHuellaMarcacionSDKAPI,@Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver,
			   @bitValidarConfiguracionIngresoWeb,
			   @SMTPServer,@user,@password,@port,@timeGetPendingActions,@timeRemoveFingerprints,
			   @timeInsertWhiteListOnTCAM,@allowWhiteListOnTCAM,@timeTerminalConnections,@intTiempoEsperaRespuestaReplicaHuellasTCAM7000,
			   @timeWaitResponseReplicateUsers,@timeWaitResponseDeleteFingerprint,@timeWaitResponseDeleteUser,
			   @timeResetDownloadEvents,@timeRemoveUsers,@timeDowndloadEvents,@timeHourSync,@quantityAppAccessControlSimultaneous,
			   @bitGenerarCodigoQRdeingresoparavalidarLocalmente, @bitLectorBiometricoSiempreEncendido,@intMinutosNoReingresoDia)
	end

	if (@action = 'GetConfiguration')
	begin
		select *
		from tblConfiguration
	end

	if (@action = 'Update')
	begin
		update tblConfiguration
		set strTipoIngreso = @strTipoIngreso,intMinutosDescontarTiquetes = @intMinutosDescontarTiquetes,
			bitIngresoEmpSinPlan = @bitIngresoEmpSinPlan,blnPermitirIngresoPantalla = @blnPermitirIngresoPantalla,blnPermiteIngresosAdicionales = @blnPermiteIngresosAdicionales,
			intMinutosNoReingreso = @intMinutosNoReingreso,blnLimpiarDescripcionAdicionales = @blnLimpiarDescripcionAdicionales,
			blnEntradaCumpleConPlan = @blnEntradaCumpleConPlan,blnEntradaCumpleSinPlan = @blnEntradaCumpleSinPlan,intDiasGraciaClientes = @intDiasGraciaClientes,
			bitBloqueoCitaNoCumplidaMSW = @bitBloqueoCitaNoCumplidaMSW,bitBloqueoClienteNoApto = @bitBloqueoClienteNoApto,
			bitBloqueoNoDisentimento = @bitBloqueoNoDisentimento,bitBloqueoNoAutorizacionMenor = @bitBloqueoNoAutorizacionMenor,
			bitConsultaInfoCita = @bitConsultaInfoCita,bitImprimirHoraReserva = @bitImprimirHoraReserva,bitTiqueteClaseAsistido_alImprimir = @bitTiqueteClaseAsistido_alImprimir,
			bitAccesoPorReservaWeb = @bitAccesoPorReservaWeb,bitValidarPlanYReservaWeb = @bitValidarPlanYReservaWeb,bitValideContrato = @bitValideContrato,
			bitValideContratoPorFactura = @bitValideContratoPorFactura,intMinutosAntesReserva = @intMinutosAntesReserva,intMinutosDespuesReserva = @intMinutosDespuesReserva,
			intdiassincita_bloqueoing = @intdiassincita_bloqueoing,intentradas_sincita_bloqueoing = @intentradas_sincita_bloqueoing,
			bitIngresoTecladoHuella = @bitIngresoTecladoHuella,strClave = @strClave,strNivelSeguridad = @strNivelSeguridad,bitPermitirBorrarHuella = @bitPermitirBorrarHuella,
			bitIngresoMiniTouch = @bitIngresoMiniTouch,bitTrabajarConDBEnOtroEquipo = @bitTrabajarConDBEnOtroEquipo,strRutaNombreLogo = @strRutaNombreLogo,
			intUbicacionTeclasTeclado = @intUbicacionTeclasTeclado,bitBaseDatosSQLServer = @bitBaseDatosSQLServer,intTiempoParaLimpiarPantalla = @intTiempoParaLimpiarPantalla,
			intTipoUbicacionTeclado = @intTipoUbicacionTeclado,strRutaNombreBanner = @strRutaNombreBanner,bitIngresoAbreDesdeTouch = @bitIngresoAbreDesdeTouch,
			intNivelSeguirdadLectorUSB = @intNivelSeguirdadLectorUSB,intTimeOutLector = @intTimeOutLector,
			intTiempoDuermeHiloEnviaComandoSalida = @intTiempoDuermeHiloEnviaComandoSalida,bitMensajeCumpleanos = @bitMensajeCumpleanos,
			intTiempoActualizaIngresos = @intTiempoActualizaIngresos,intTiempoValidaSiAbrePuerta = @intTiempoValidaSiAbrePuerta,
			strRutaArchivoGymsoftNet = @strRutaArchivoGymsoftNet,bitComplices_CortxIngs = @bitComplices_CortxIngs,bitComplices_DescuentoTiq = @bitComplices_DescuentoTiq,
			strTextoMensajeCumpleanos = @strTextoMensajeCumpleanos,bitSolo1HuellaxCliente = @bitSolo1HuellaxCliente,
			bitMultiplesPlanesVig = @bitMultiplesPlanesVig,intComplices_Plan_CortxIngs = @intComplices_Plan_CortxIngs,strTextoMensajeCortxIngs = @strTextoMensajeCortxIngs,
			bitImagenSIBO = @bitImagenSIBO,strColorPpal = @strColorPpal,strRutaGuiaMarcacion = @strRutaGuiaMarcacion,intPasoFinalAutoregistroWeb = @intPasoFinalAutoregistroWeb,
			bitAntipassbackEntrada = @bitAntipassbackEntrada,bitReplicaImgsTCAM7000 = @bitReplicaImgsTCAM7000,intTiempoReplicaImgsTCAM7000 = @intTiempoReplicaImgsTCAM7000,
			bitAccesoDiscapacitados = @bitAccesoDiscapacitados, intTiempoEspaciadoTramasReplicaImgsTCAM7000 = @intTiempoEspaciadoTramasReplicaImgsTCAM7000,
			intTiempoMaximoEnvioImgsTCAM7000 = @intTiempoMaximoEnvioImgsTCAM7000,intCalidadHuella = @intCalidadHuella,bitEsperarHuellaActualizar = @bitEsperarHuellaActualizar,
			bitGenerarContratoPDFyEnviar = @bitGenerarContratoPDFyEnviar,intNumeroDiasActualizacionHuella = @intNumeroDiasActualizacionHuella,
			bitNoValidarHuella = @bitNoValidarHuella,intTiempoReplicaHuellasTCAM7000 = @intTiempoReplicaHuellasTCAM7000,
			intTiempoPingContinuoTCAM7000 = @intTiempoPingContinuoTCAM7000,strTipoIdenTributaria = @strTipoIdenTributaria,
			bitNo_Validar_Entrada_En_Salida = @bitNo_Validar_Entrada_En_Salida,strIdentificadorUno = @strIdentificadorUno,strIdentificadorDos = @strIdentificadorDos,
			strPuertoCom = @strPuertoCom,intVelocidadPuerto = @intVelocidadPuerto,intTiempoPulso = @intTiempoPulso,bitBloqueoCita = @bitBloqueoCita,
			intDiasEntreCitaRiesgo = @intDiasEntreCitaRiesgo,intDiasEntreCitaNoRiesgo = @intDiasEntreCitaNoRiesgo,bitCambiarEstadoTiqueteClase = @bitCambiarEstadoTiqueteClase,
			timeGetWhiteList = @timeGetWhiteList,timeInsertEntries = @timeInsertEntries,
			bitFirmarContratoAlEnrolar = @bitFirmarContratoAlEnrolar, intRangoHoraPrint = @intRangoHoraPrint,
			timeGetConfiguration = @timeGetConfiguration, timeGetClientMessages = @timeGetClientMessages,
			timeGetTerminals = @timeGetTerminals, strPuertoComSalida = @strPuertoComSalida,
			bitConsentimientoDatosBiometricos = @bitConsentimientoDatosBiometricos,bitConsentimientoInformado = @bitConsentimientoInformado,
			bitDatosVirtualesUsuario = @bitDatosVirtualesUsuario, bitValidarHuellaMarcacionSDKAPI = @bitValidarHuellaMarcacionSDKAPI,
			Validación_de_huella_de_marcación_con_SDK_del_webserver = @Validación_de_huella_de_marcación_con_SDK_del_webserver,
			Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver = @Timeout_Validación_de_huella_de_marcación_con_SDK_del_webserver,
			intTimeoutValidarHuellaMarcacionSDKAPI = @intTimeoutValidarHuellaMarcacionSDKAPI,bitValidarConfiguracionIngresoWeb = @bitValidarConfiguracionIngresoWeb,
			SMTPServer = @SMTPServer,strUser = @user,strPassword = @password,strPort = @port,
			timeGetPendingActions = @timeGetPendingActions, timeRemoveFingerprints = @timeRemoveFingerprints,
			timeInsertWhiteListOnTCAM = @timeInsertWhiteListOnTCAM, allowWhiteListOnTCAM = @allowWhiteListOnTCAM,
			timeTerminalConnections = @timeTerminalConnections,intTiempoEsperaRespuestaReplicaHuellasTCAM7000 = @intTiempoEsperaRespuestaReplicaHuellasTCAM7000,
			timeWaitResponseReplicateUsers = @timeWaitResponseReplicateUsers,timeWaitResponseDeleteFingerprint = @timeWaitResponseDeleteFingerprint,
			timeWaitResponseDeleteUser = @timeWaitResponseDeleteUser, timeResetDownloadEvents = @timeResetDownloadEvents,timeRemoveUsers = @timeRemoveUsers,
			timeDowndloadEvents = @timeDowndloadEvents,timeHourSync = @timeHourSync, quantityAppAccessControlSimultaneous = @quantityAppAccessControlSimultaneous,
			bitGenerarCodigoQRdeingresoparavalidarLocalmente = @bitGenerarCodigoQRdeingresoparavalidarLocalmente,
			bitLectorBiometricoSiempreEncendido = @bitLectorBiometricoSiempreEncendido,
			intMinutosNoReingresoDia = @intMinutosNoReingresoDia
	end
end
go

if not exists (select * from sys.objects where name = 'tblEvent' and type in (N'U'))
begin
	create table tblEvent(
		intPkId int not null identity (1,1) primary key,
		clientId varchar(15) not null,
		clientName varchar(max) not null,
		entryDate datetime null,
		entryHour datetime null,
		outDate datetime null,
		outHour datetime null,
		planId int not null,
		planName varchar(max) null,
		invoiceId int not null,
		documentType varchar(50) null,
		discountTicket bit null,
		visitId int null,
		updated bit not null,
		strFirstMessage varchar(max) not null,
		strSecondMessage varchar(max) not null,
		thirdMessage varchar(max) null,
		expirationDate datetime null,
		dateLastEntry datetime null,
		successEntry bit null,
		eventType varchar(100) not null,
		ipAddress varchar(100) null,
		modifiedDate datetime null
	)
end
go

--Od 1938 Criterio 5 - Mtoro
if not exists (select * from sys.columns where name = 'qrCode' and object_id = OBJECT_ID('tblEvent'))
begin
    alter table tblEvent add qrCode varchar (15)
end
go

if not exists (select * from sys.columns where name = 'temperature' and object_id = OBJECT_ID('tblEvent'))
begin
    alter table tblEvent add temperature varchar (15)
end
go

--FIN Od 1938 Criterio 5 - Mtoro

if not exists (select * from sys.columns where name = 'strUsuarioLogin' and object_id = OBJECT_ID('tblEvent'))
begin
    alter table tblEvent add strUsuarioLogin varchar (max)
end
go

if not exists (select * from sys.columns where name = 'strEmpresaIngresoAdicional' and object_id = OBJECT_ID('tblEvent'))
begin
    alter table tblEvent add strEmpresaIngresoAdicional varchar (max)
end
go

if not exists (select * from sys.objects where name like '%type_tblEvent%' and type in (N'TT'))
begin
	CREATE TYPE type_tblEvent AS TABLE(
		clientId varchar(15) not null,
		clientName varchar(max) not null,
		entryDate datetime null,
		entryHour datetime null,
		outDate datetime null,
		outHour datetime null,
		planId int not null,
		planName varchar(max) null,
		invoiceId int not null,
		documentType varchar(50) null,
		discountTicket bit null,
		visitId int null,
		updated bit not null,
		strFirstMessage varchar(max) not null,
		strSecondMessage varchar(max) not null,
		thirdMessage varchar(max) null,
		expirationDate datetime null,
		dateLastEntry datetime null,
		successEntry bit null,
		eventType varchar(100) not null,
		ipAddress varchar(100) null
	)
end
go

if not exists (select * from sys.objects where name like '%type_tblTerminalEvents%' and type in (N'TT'))
begin
	CREATE TYPE type_tblTerminalEvents AS TABLE(
		ipAddress varchar(100),
		date datetime,
		userId varchar(15),
		isExit bit
	)
end
go

if exists (select * from sys.objects where name = 'spEvent' and type in (N'P'))
begin
	drop procedure spEvent
end
go

create procedure spEvent
@action varchar(50) = '',
@intPkId int = null,
@clientId varchar(15) = '',
@clientName varchar(max) = '',
@entryDate datetime = null,
@entryHour datetime = null,
@outDate datetime = null,
@outHour datetime = null,
@planId int = null,
@planName varchar(max) = '',
@invoiceId int = null,
@documentType varchar(50) = '',
@discountTicket bit = null,
@visitId int = null,
@updated bit = null,
@strFirstMessage varchar(max) = '',
@strSecondMessage varchar(max) = '',
@thirdMessage varchar(max) = '',
@expirationDate datetime = null,
@dateLastEntry datetime = null,
@successEntry bit = null,
@ipAddress varchar(100) = '',
@modifiedDate datetime = null,
@idList varchar(max) = '',
@qrCode varchar(15) = '',
@temperature varchar(15) = '',
@eventType varchar(100) = '',
@strEmpresaIngresoAdicional varchar(max) = '',
@usuarioLoginIngresoAdicional varchar(max) = '',
@tblEvent type_tblEvent readonly,
@tblTerminalEvents type_tblTerminalEvents readonly
as
begin
	declare @strSql varchar(max)

	if (@action = 'Insert')
	begin
		insert into tblEvent(clientId,clientName,entryDate,entryHour,outDate,outHour,planId,planName,invoiceId,documentType,
							  discountTicket,visitId,updated,strFirstMessage,strSecondMessage,thirdMessage,expirationDate,
							  dateLastEntry,successEntry,eventType,ipAddress,modifiedDate,qrCode,temperature,strUsuarioLogin,strEmpresaIngresoAdicional)
		values (@clientId,@clientName,@entryDate,@entryHour,@outDate,@outHour,@planId,@planName,@invoiceId,@documentType,
				@discountTicket,@visitId,@updated,@strFirstMessage,@strSecondMessage,@thirdMessage,@expirationDate,
				@dateLastEntry,@successEntry,@eventType,@ipAddress,@modifiedDate,@qrCode,@temperature,@usuarioLoginIngresoAdicional ,@strEmpresaIngresoAdicional)
	end

	if (@action = 'InsertTable')
	begin
		insert into tblEvent(clientId,clientName,entryDate,entryHour,outDate,outHour,planId,planName,invoiceId,documentType,
							 discountTicket,visitId,updated,strFirstMessage,strSecondMessage,thirdMessage,expirationDate,
							 dateLastEntry,successEntry,eventType,ipAddress,modifiedDate)
		select clientId,clientName,entryDate,entryHour,outDate,outHour,planId,planName,invoiceId,documentType,
			   discountTicket,visitId,updated,strFirstMessage,strSecondMessage,thirdMessage,expirationDate,dateLastEntry,
			   successEntry,eventType,ipAddress,getdate()
		from @tblEvent
	end

	if (@action = 'InsertTerminalEvents')
	begin
		declare @quantity int = (select count(*)
								from @tblTerminalEvents),
			@aux int = 0,
			@ipAddresE varchar(100),
			@dateE datetime,
			@userIdE varchar(15),
			@isExitE bit,
			@existWL bit

		declare cursor_events cursor for
		select *
		from @tblTerminalEvents
		order by date asc
		open cursor_events

		while (@aux < @quantity)
		begin
		fetch next from cursor_events
		into @ipAddresE, @dateE, @userIdE, @isExitE
		set @existWL = isnull((select top 1 1 from tblWhiteList where id = @userIdE),0)

		if (@existWL = 1)
		begin
			if (@isExitE = 0)
			begin
				set @intPkId = isnull((select top 1 intPkId 
										from tblEvent 
										where eventType = 'Entry' 
												and successEntry = 1
												and entryDate is null 
												and entryHour is null
												and clientId = @userIdE
												and convert(varchar(10),outDate,111) = convert(varchar(10),@dateE,111)),0)

				if (@intPkId <= 0)
				begin
					set @discountTicket = 0
					declare @planType varchar(10) = ''
					select top 1 @planType = planType, @planId = planId from tblWhiteList where id = @userIdE
					declare @timeDiscountTicket int = isnull((select c.intMinutosDescontarTiquetes from tblConfiguration c),0)

					if (@planType = 'T')
					begin
						set @discountTicket = 1
						
						if (@timeDiscountTicket > 0)
						begin
							select top 1 @discountTicket = case when (datediff(minute,entryDate,@dateE) >= @timeDiscountTicket) then cast(1 as bit) else cast(0 as bit) end
							from tblEvent
							where clientId = @userIdE and discountTicket = 1 and planId = @planId
							order by entryDate desc
						end

						if (@discountTicket = 1)
						begin
							update tblWhiteList
							set availableEntries = availableEntries - 1
							where id = @userIdE and planType = 'T' and planId = @planId
						end
					end

					insert into tblEvent(clientId,clientName,entryDate,entryHour,planId,planName,
											invoiceId,documentType,discountTicket,visitId,updated,strFirstMessage,
											strSecondMessage,thirdMessage,expirationDate,successEntry,eventType,
											ipAddress,modifiedDate)
					select distinct @userIdE, wl.name, @dateE, @dateE, wl.planId, wl.planName, 
							case when wl.documentType = 'Factura' then wl.invoiceId 
								when wl.documentType = 'Cortesía' then wl.courtesy else cast ('' as varchar) end, 
							wl.documentType, @discountTicket,
							cast(0 as int), cast(0 as bit), cast('' as varchar(max)), cast('' as varchar(max)),
							cast('' as varchar(max)), wl.expirationDate, cast(1 as bit), cast('Entry' as varchar(max)), 
							@ipAddresE,getdate()
					from tblWhiteList wl
					where id = @userIdE
				end
				else
				begin
					update tblEvent
					set entryDate = @dateE, entryHour = @dateE
					where intPkId = @intPkId
				end
			end
			else
			begin
				set @intPkId = isnull((select top 1 intPkId 
										from tblEvent 
										where eventType = 'Entry' 
												and successEntry = 1
												and outDate is null 
												and outHour is null
												and convert(varchar(10),entryDate,111) = convert(varchar(10),@dateE,111)
												and clientId = @userIdE),0)
				if (@intPkId <= 0)
				begin
					insert into tblEvent(clientId,clientName,outDate,outHour,planId,planName,
										 invoiceId,documentType,discountTicket,visitId,updated,strFirstMessage,
										 strSecondMessage,thirdMessage,expirationDate,successEntry,eventType,
										 ipAddress,modifiedDate)
					select distinct @userIdE, wl.name, @dateE, @dateE, wl.planId, wl.planName, 
							case when wl.documentType = 'Factura' then wl.invoiceId 
								when wl.documentType = 'Cortesía' then wl.courtesy else cast ('' as varchar) end, 
							wl.documentType, case when wl.planType = 'T' then cast(1 as bit) else cast(0 as bit) end,
							cast(0 as int), cast(0 as bit), cast('' as varchar(max)), cast('' as varchar(max)),
							cast('' as varchar(max)), wl.expirationDate, cast(1 as bit), cast('Entry' as varchar(max)), 
							@ipAddresE,getdate()
					from tblWhiteList wl
					where id = @userIdE
				end
				else
				begin
					update tblEvent
					set outDate = @dateE, outHour = @dateE
					where intPkId = @intPkId
				end
			end
		end

		set @aux = @aux + 1
		end

		close cursor_events
		deallocate cursor_events
	end

	if (@action = 'GetEntries')
	begin
		select *
		from tblEvent
		where updated = 0 and eventType = 'Entry' and (successEntry = 1 or thirdMessage = 'HightTemp')
		and strFirstMessage <> 'Ingreso adicional'
	end

	if (@action = 'GetEntriesAdicional')
	begin
		select intPkId,clientId,clientName,entryDate,entryHour,outDate,outHour,planId,planName,invoiceId,documentType,
			discountTicket,visitId,updated,strFirstMessage,strSecondMessage,thirdMessage,expirationDate,dateLastEntry,successEntry,
			eventType,terminal.name as ipAddress,modifiedDate,qrCode,temperature,evento.strUsuarioLogin,evento.strEmpresaIngresoAdicional
		from tblEvent evento
		inner join tblTerminal terminal on evento.ipAddress = terminal.ipAddress
		where updated = 0 and eventType = 'Entry' and successEntry = 1 
		and strFirstMessage = 'Ingreso adicional'
	end

	if (@action = 'GetEntriesToShow')
	begin
		select clientId as 'Identificación', clientName as 'Nombre',
			   convert(varchar(10),entryDate,111) + ' ' + convert(varchar(10),entryDate,108) as 'Fecha', planName as 'Plan', 
			   convert(varchar(10),expirationDate,111) + ' ' + convert(varchar(10),expirationDate,108) as 'Fecha de vencimiento',
			   strFirstMessage as 'Mensaje de ingreso', strSecondMessage as 'Mensaje de restricción',
			   cast('NO' as varchar(10)) as 'Salida', 
			   case when (successEntry is null or successEntry = 0) then cast('NO' as varchar(10)) else cast('SI' as varchar(10)) end as 'Entrada exitosa',
			   @ipAddress as 'Terminal', cast('Conectada' as varchar(100)) as 'Estado', eventType as 'Tipo de evento' , qrCode as 'Código qr', temperature as 'Temperatura'
		from tblEvent
		where convert(varchar(10),modifiedDate,111) >= convert(varchar(10),getdate(),111) and ipAddress = @ipAddress
		order by modifiedDate desc
	end

	if (@action = 'GetExitToShow')
	begin
		select clientId as 'Identificación', clientName as 'Nombre',
				convert(varchar(10),outDate,111) + ' ' + convert(varchar(10),outDate,108) as 'Fecha', planName as 'Plan', 
				convert(varchar(10),expirationDate,111) + ' ' + convert(varchar(10),expirationDate,108) as 'Fecha de vencimiento',
				cast('Puede salir' as varchar(200)) as 'Mensaje de ingreso', cast('Vuelva pronto' as varchar(200)) as 'Mensaje de restricción',
				cast('SI' as varchar(10)) as 'Salida', 
				case when (successEntry is null or successEntry = 0) then cast('NO' as varchar(10)) else cast('SI' as varchar(10)) end as 'Entrada exitosa',
				cast('IngresoTouch' as varchar(50)) as 'Terminal', cast('Conectado' as varchar(100)) as 'Estado', cast('Exit' as varchar(100)) as 'Tipo de evento', qrCode as 'Código qr', temperature as 'Temperatura'
		from tblEvent
		where convert(varchar(10),modifiedDate,111) >= convert(varchar(10),getdate(),111) 
			  and outDate is not null and outDate >= entryDate and ipAddress = @ipAddress
		order by modifiedDate desc
	end

	if (@action = 'GetEntriesToShowMonitor')
	begin
		select clientId as 'Identificación', clientName as 'Nombre',
			   convert(varchar(10),entryDate,111) + ' ' + convert(varchar(10),entryDate,108) as 'Fecha', planName as 'Plan', 
			   convert(varchar(10),expirationDate,111) + ' ' + convert(varchar(10),expirationDate,108) as 'Fecha de vencimiento',
			   strFirstMessage as 'Mensaje de ingreso', strSecondMessage as 'Mensaje de restricción',
			   cast('NO' as varchar(10)) as 'Salida', 
			   case when (successEntry is null or successEntry = 0) then cast('NO' as varchar(10)) else cast('SI' as varchar(10)) end as 'Entrada exitosa',
			   cast('IngresoTouch' as varchar(50)) as 'Terminal', cast('Conectado' as varchar(100)) as 'Estado', eventType as 'Tipo de evento', qrCode as 'Código qr', temperature as 'Temperatura'
		from tblEvent
		where convert(varchar(10),modifiedDate,111) >= convert(varchar(10),getdate(),111)
		order by modifiedDate desc
	end

	if (@action = 'GetExitToShowMonitor')
	begin
		select clientId as 'Identificación', clientName as 'Nombre',
				convert(varchar(10),outDate,111) + ' ' + convert(varchar(10),outDate,108) as 'Fecha', planName as 'Plan', 
				convert(varchar(10),expirationDate,111) + ' ' + convert(varchar(10),expirationDate,108) as 'Fecha de vencimiento',
				cast('Puede salir' as varchar(200)) as 'Mensaje de ingreso', cast('Vuelva pronto' as varchar(200)) as 'Mensaje de restricción',
				cast('SI' as varchar(10)) as 'Salida', 
				case when (successEntry is null or successEntry = 0) then cast('NO' as varchar(10)) else cast('SI' as varchar(10)) end as 'Entrada exitosa',
				cast('IngresoTouch' as varchar(50)) as 'Terminal', cast('Conectado' as varchar(100)) as 'Estado', cast('Exit' as varchar(100)) as 'Tipo de evento', qrCode as 'Código qr', temperature as 'Temperatura'
		from tblEvent
		where convert(varchar(10),modifiedDate,111) >= convert(varchar(10),getdate(),111) 
			  and outDate is not null and outDate >= entryDate
		order by modifiedDate desc
	end

	if (@action = 'GetBothEvents')
	begin
		select clientId as 'Identificación', clientName as 'Nombre',
			   convert(varchar(10),entryDate,111) + ' ' + convert(varchar(10),entryDate,108) as 'Fecha', planName as 'Plan', 
			   convert(varchar(10),expirationDate,111) + ' ' + convert(varchar(10),expirationDate,108) as 'Fecha de vencimiento',
			   strFirstMessage as 'Mensaje de ingreso', strSecondMessage as 'Mensaje de restricción',
			   cast('NO' as varchar(10)) as 'Salida', 
			   case when (successEntry is null or successEntry = 0) then cast('NO' as varchar(10)) else cast('SI' as varchar(10)) end as 'Entrada exitosa',
			   cast('IngresoTouch' as varchar(50)) as 'Terminal', cast('Conectado' as varchar(100)) as 'Estado', eventType as 'Tipo de evento', qrCode as 'Código qr', temperature as 'Temperatura',
			   modifiedDate, ipAddress
		into ##tmpEventsToShow
		from tblEvent
		where convert(varchar(10),modifiedDate,111) >= convert(varchar(10),getdate(),111)

		insert into ##tmpEventsToShow
		select clientId as 'Identificación', clientName as 'Nombre',
				convert(varchar(10),outDate,111) + ' ' + convert(varchar(10),outDate,108) as 'Fecha', planName as 'Plan', 
				convert(varchar(10),expirationDate,111) + ' ' + convert(varchar(10),expirationDate,108) as 'Fecha de vencimiento',
				cast('Puede salir' as varchar(200)) as 'Mensaje de ingreso', cast('Vuelva pronto' as varchar(200)) as 'Mensaje de restricción',
				cast('SI' as varchar(10)) as 'Salida', 
				case when (successEntry is null or successEntry = 0) then cast('NO' as varchar(10)) else cast('SI' as varchar(10)) end as 'Entrada exitosa',
				cast('IngresoTouch' as varchar(50)) as 'Terminal', cast('Conectado' as varchar(100)) as 'Estado', cast('Exit' as varchar(100)) as 'Tipo de evento', qrCode as 'Código qr', temperature as 'Temperatura',
				modifiedDate, ipAddress
		from tblEvent
		where convert(varchar(10),modifiedDate,111) >= convert(varchar(10),getdate(),111) 
			  and outDate is not null and outDate >= entryDate

		if (@ipAddress != '' and @ipAddress is not null)
		begin
			select [Identificación], [Nombre], [Fecha], [Plan], [Fecha de vencimiento],
				   [Mensaje de ingreso], [Mensaje de restricción], [Salida], [Entrada exitosa],
				   [Terminal], [Estado], [Tipo de evento]
			from ##tmpEventsToShow
			where [ipAddress] = @ipAddress
			order by modifiedDate desc
		end
		else
		begin
			select [Identificación], [Nombre], [Fecha], [Plan], [Fecha de vencimiento],
				   [Mensaje de ingreso], [Mensaje de restricción], [Salida], [Entrada exitosa],
				   [Terminal], [Estado], [Tipo de evento]
			from ##tmpEventsToShow
			order by modifiedDate desc
		end

		drop table ##tmpEventsToShow
	end

	if (@action = 'Update')
	begin
		update tblEvent
		set outDate = @outDate,outHour = @outHour,modifiedDate=getdate(), thirdMessage = @thirdMessage
		where intPkId = (select top 1 intPkId
						 from tblEvent
						 where clientId = @clientId and planId = @planId and invoiceId = @invoiceId
							   and convert(varchar(10),entryDate,111) = convert(varchar(10),getdate(),111)
							   and outDate is null and successEntry = 1
						 order by entryDate desc)
	end

	if (@action = 'UpdateState')
	begin
		update tblEvent
		set updated = @updated,modifiedDate=@modifiedDate
		where intPkId = @intPkId
	end

	if (@action = 'GetEntriesByIdFamilyGroup')
	begin
		set @strSql = 'select *
					   from tblEvent
					   where successEntry = 1 
							 and convert(varchar(10),entryDate,111) = convert(varchar(10),getdate(),111)
							 and clientId in (''' + @idList + ''')'
		exec (@strSql)
	end

	if (@action = 'GetEntryByUserWithoutOutput')
	begin
		select top 1 *
		from tblEvent
		where clientId = @clientId and planId = @planId and invoiceId = @invoiceId
			  and convert(varchar(10),entryDate,111) = convert(varchar(10),getdate(),111)
			  and eventType = 'Entry' and outDate is null
		order by entryHour desc
	end

	if (@action = 'GetLastEntryByUserId')
	begin
		select top (1) *
		from tblEvent
		where clientId = @clientId
		order by modifiedDate desc
	end

	if (@action = 'GetLastEntryByUserIdAndInvoiceIdAndPlanId')
	begin
		select top (1) *
		from tblEvent
		where clientId = @clientId and invoiceId = @invoiceId and planId = @planId
			  and entryDate is not null and discountTicket = 1
		order by entryDate desc
	end

	IF(@action = 'IngresosVisitantes')
	BEGIN

		SELECT		tblEvent.*
		FROM		tblEvent
		WHERE		clientId = @clientId
		AND			planId = 0
		AND			eventType = 'Entry'
		ORDER BY	modifiedDate DESC

	END

END
GO

if not exists (select * from sys.objects where name = 'tblClientMessages' and type in (N'U'))
begin
	create table tblClientMessages(
		messageId int not null primary key,
		messageType varchar(50) not null,
		messageText varchar(max) not null,
		messageDurationTime int null,
		messageInitialDate datetime null,
		messageInitialHour datetime null,
		messageFinalDate datetime null,
		messageFinalHour datetime null,
		messageState bit not null,
		messageImgOrder varchar(2) null,
		messageImage image null
	)
end
go

if exists (select * from sys.objects where name = 'spClientMessages' and type in (N'P'))
begin
	drop procedure spClientMessages
end
go

create procedure spClientMessages
@action varchar(50) = '',
@messageId int = null,
@messageType varchar(50) = '',
@messageText varchar(max) = '',
@messageDurationTime int = null,
@messageInitialDate datetime = null,
@messageInitialHour datetime = null,
@messageFinalDate datetime = null,
@messageFinalHour datetime = null,
@messageState bit = null,
@messageImgOrder varchar(2) = '',
@messageImage image = null
as
begin
	if (@action = 'GetClientMessages')
	begin
		select *
		from tblClientMessages
		where messageState = 1
	end

	if (@action = 'Insert')
	begin
		insert into tblClientMessages (messageId,messageType,messageText,messageDurationTime,messageInitialDate,messageInitialHour,
									   messageFinalDate,messageFinalHour,messageState,messageImgOrder,messageImage)
		values (@messageId,@messageType,@messageText,@messageDurationTime,@messageInitialDate,@messageInitialHour,
				@messageFinalDate,@messageFinalHour,@messageState,@messageImgOrder,@messageImage)
	end

	if (@action = 'Update')
	begin
		update tblClientMessages
		set messageType = @messageType,messageText = @messageText,messageDurationTime = @messageDurationTime,messageInitialDate = @messageInitialDate,
			messageInitialHour = @messageInitialHour,messageFinalDate = @messageFinalDate,messageFinalHour = @messageFinalHour,messageState = @messageState,
			messageImgOrder = @messageImgOrder,messageImage = @messageImage
		where messageId = @messageId
	end

	if (@action = 'GetClientMessage')
	begin
		select *
		from tblClientMessages
		where messageId = @messageId
	end
end
go

if not exists (select * from sys.objects where name = 'tblProcessLog' and type in (N'U'))
begin
	create table tblProcessLog(
		intPkId int not null identity(1,1) primary key,
		dateLog datetime not null,
		process varchar(max) not null
	)
end
go

if not exists (select * from sys.objects where name = 'tblErrorsLog' and type in (N'U'))
begin
	create table tblErrorsLog(
		intPkId int not null identity(1,1) primary key,
		dateLog datetime not null,
		error varchar(max) not null
	)
end
go

if exists (select * from sys.objects where name = 'spLog' and type in (N'P'))
begin
	drop procedure spLog
end
go

create procedure spLog
@action varchar(50) = '',
@dateLog datetime = null,
@msg varchar(max) = ''
as
begin
	if (@action = 'InsertProcess')
	begin
		insert into tblProcessLog (dateLog,process)
		values (@dateLog,@msg)
	end

	if (@action = 'InsertError')
	begin
		insert into tblErrorsLog(dateLog,error)
		values (@dateLog,@msg)
	end
end
go

if not exists (select * from sys.objects where name = 'tblTerminal' and type in (N'U'))
begin
	create table tblTerminal(
		terminalId int not null primary key,
		ipAddress varchar(100) not null,
		timeWaitAnswerReplicate int not null,
		bitServesToOutput bit not null,
		bitServesToInputAndOutput bit not null,
		bitCardMode bit not null,
		TCAM7000 bit not null,
		LectorZK bit not null,
		ICAM7000 bit not null,
		port varchar(50) null,
		speedPort int null,
		withWhiteList bit null default(0),
		name varchar(200) null,
		state bit null,
		terminalTypeId int null,
		terminalTypeName varchar(100) null
	)
end
else
begin
	if not exists (select top 1 1 from sys.columns where name = 'name' and object_id = OBJECT_ID('tblTerminal'))
	begin
		alter table tblTerminal add name varchar(200) null
	end

	if not exists (select top 1 1 from sys.columns where name = 'state' and object_id = OBJECT_ID('tblTerminal'))
	begin
		alter table tblTerminal add state bit null
	end

	if not exists (select top 1 1 from sys.columns where name = 'terminalTypeId' and object_id = OBJECT_ID('tblTerminal'))
	begin
		alter table tblTerminal add terminalTypeId int null
	end

	if not exists (select top 1 1 from sys.columns where name = 'terminalTypeName' and object_id = OBJECT_ID('tblTerminal'))
	begin
		alter table tblTerminal add terminalTypeName varchar(100) null
	end
end
go
if not exists (select * from sys.columns where name = 'snTerminal' and object_id = OBJECT_ID('tblTerminal'))
begin  
    ALTER TABLE tblTerminal add snTerminal varchar(max) default('');
end
go
if not exists (select * from sys.columns where name = 'Zonas' and object_id = OBJECT_ID('tblTerminal'))
begin  
	ALTER TABLE tblTerminal add Zonas varchar(max) default('');
end
go

if not exists (select * from sys.objects where name = 'tbl_Maestro_Zonas' and type in (N'U'))
begin
	CREATE TABLE [dbo].[tbl_Maestro_Zonas](
	[id] [int] NOT NULL,
	[codigo_string] [varchar](max) NULL,
	[cdgimnasio] [int] NOT NULL,
	[descripcion] [varchar](max) NULL,
	[nombre_zona] [varchar](max) NULL,
	[intSucursal] [int] NOT NULL,
	[Terminales] [varchar](max) NULL,
	[bitEstado] [bit] NULL,
	[fechacreacion] [datetime] NULL
)
end
go

if exists (select * from sys.objects where name = 'spTerminal' and type in (N'P'))
begin
	drop procedure spTerminal
end
go

CREATE procedure [dbo].[spTerminal]
@action varchar(50) = '',
@terminalId int = null,
@ipAddress varchar(100) = '',
@timeWaitAnswerReplicate int = null,
@bitServesToOutput bit = null,
@bitServesToInputAndOutput bit = null,
@bitCardMode bit = null,
@TCAM7000 bit = null,
@LectorZK bit = null,
@ICAM7000 bit = null,
@port varchar(50) = '',
@speedPort int = null,
@withWhiteList bit = null,
@name varchar(200) = '',
@state bit = null,
@terminalTypeId int = null,
@terminalTypeName varchar(100) = '',
@snTerminal varchar(max) = '',
@Zonas varchar(max) = ''
as
begin
	if (@action = 'GetTerminalById')
	begin
		select *
		from tblTerminal
		where terminalId = @terminalId
	end

	if (@action = 'GetTerminalByIp')
	begin
		select *
		from tblTerminal
		where ipAddress = @ipAddress
	end

	if (@action = 'GetTerminals')
	begin
		select *
		from tblTerminal
		where terminalTypeId <> '7'
		and state = 'true'
	end
	
	if (@action = 'GetTerminalsType')
	begin
		select *
		from tblTerminal
		where terminalTypeId = @terminalTypeId
		and state = 'true'
	end

		if (@action = 'GetTerminalsBioLite')
	begin
		select *
		from tblTerminal
		where terminalTypeId = @terminalTypeId
		and state = 'true'
	end

	if (@action = 'Insert')
	begin
		insert into tblTerminal (terminalId,ipAddress,timeWaitAnswerReplicate,
								 bitServesToOutput,bitServesToInputAndOutput,bitCardMode,
								 TCAM7000,LectorZK,ICAM7000,port,speedPort,withWhiteList,
								 name,state,terminalTypeId,terminalTypeName,snTerminal,Zonas)
		values (@terminalId,@ipAddress,@timeWaitAnswerReplicate,
				@bitServesToOutput,@bitServesToInputAndOutput,@bitCardMode,
				@TCAM7000,@LectorZK,@ICAM7000,@port,@speedPort,@withWhiteList,
				@name,@state,@terminalTypeId,@terminalTypeName,@snTerminal,@Zonas)
	end

	if (@action = 'Update')
	begin
	
		update tblTerminal
		set ipAddress = @ipAddress,timeWaitAnswerReplicate = @timeWaitAnswerReplicate,
			TCAM7000 = @TCAM7000,LectorZK = @LectorZK,ICAM7000 = @ICAM7000,
			port = @port,speedPort = @speedPort,
			bitServesToOutput = @bitServesToOutput,bitServesToInputAndOutput=@bitServesToInputAndOutput,
			bitCardMode=@bitCardMode, withWhiteList = @withWhiteList,
			name=@name,state=@state,terminalTypeId=@terminalTypeId,terminalTypeName=@terminalTypeName
			,snTerminal = @snTerminal , Zonas = @Zonas
		where terminalId = @terminalId
	end
	if (@action = 'GetTerminalsID')
	begin
		select *
		from tblTerminal
		where terminalId =@terminalId
	end
end
go

if not exists (select * from sys.objects where name = 'tblAction' and type in (N'U'))
begin
	create table tblAction(
		id int not null primary key identity(1,1),
		ipAddress varchar(100) not null,
		strAction varchar(100) not null,
		actionDate datetime not null,
		used bit not null,
		actionState bit not null,
		modifiedDate datetime null
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

--if not exists (select * from sys.objects where name = 'tblMessage' and type in (N'U'))
--begin
--	create table tblMessage(
--		id int not null primary key identity(1,1),
--		ipAddress varchar(100) not null,
--		strAction varchar(100) not null,
--		strMessage varchar(max) not null,
--		dateMessage datetime not null,
--		typeMessage varchar(100) not null,
--		used int not null
--	)
--end
--go

if exists (select * from sys.objects where name = 'spAction' and type in (N'P'))
begin
	drop procedure spAction
end
go
CREATE PROCEDURE [dbo].[spAction] 
@action varchar(50) = '',
@id int = NULL,
@ipAddress varchar(100) = '',
@strAction varchar(100) = '',
@actionDate datetime = NULL,
@userId varchar(50) = '',
@used bit = NULL,
@actionState bit = NULL,
@modifiedDate datetime = NULL,
@snTerminar varchar(50) = '',
@personId varchar(50) = '',
@bitInsertFingert bit = NULL,
@fingerprint binary(2000) = NULL
AS
BEGIN
  DECLARE @responseValue int

  IF (@action = 'Insert')
  BEGIN
    INSERT INTO tblAction (ipAddress, strAction, actionDate, used, actionState)
      VALUES (@ipAddress, @strAction, @actionDate, @used, @actionState)

    SET @responseValue = @@IDENTITY
    SELECT
      @responseValue
  END

  IF (@action = 'GetPendingActionsByTerminal')
  BEGIN
    SELECT
      *
    FROM tblAction
    WHERE used = 0
    AND ipAddress = @ipAddress
    ORDER BY actionDate ASC
  END

  IF (@action = 'Update')
  BEGIN
    UPDATE tblAction
    SET used = 1,
        actionState = @actionState,
        modifiedDate = GETDATE()
    WHERE id = @id
    AND used = 0
  END
  IF (@action = 'InsertAccionHuellaBioLite')
  BEGIN

    INSERT INTO tblAccionesReplicaHuellas (strip,
    snTerminar,
    personId,
    bitInsertFingert,
    fingerprint)
      VALUES (@ipAddress, @snTerminar, @personId, @bitInsertFingert, NULL)
  END
  IF (@action = 'ConsultarAccionPendiente')
  BEGIN

    SELECT  *
    FROM tblAccionesReplicaHuellas
    WHERE strip = @ipAddress
    AND bitInsertFingert = '0'


  END

  if(@action = 'ActualizarrAccionPendiente')
  begin
  update tblAccionesReplicaHuellas
  set fingerprint = @fingerprint , bitInsertFingert = 'true'
  where intID = @id
  end 

    if(@action = 'EliminarAccionPendiente')
  begin
  
  delete from  tblAccionesReplicaHuellas
   where intID = @id
  end 

END

go

if exists (select * from sys.objects where name = 'spActionParameters' and type in (N'P'))
begin
	drop procedure spActionParameters
end
go

create procedure spActionParameters
@action varchar(50) = '',
@id int = null,
@actionId int = null,
@parameterName varchar(100) = '',
@parameterValue varchar(max) = '',
@dateParameter datetime = null
as
begin
	if (@action = 'Insert')
	begin
		insert into tblActionParameters (actionId,parameterName,parameterValue,dateParameter)
		values (@actionId,@parameterName,@parameterValue,@dateParameter)
	end

	if (@action = 'GetParameters')
	begin
		select *
		from tblActionParameters
		where actionId = @actionId
	end
end
go

if not exists (select * from sys.objects where name = 'tblAdditionalEntry' and type in (N'U'))
begin
	create table tblAdditionalEntry(
		id int not null primary key identity(1,1),
		reason varchar(max) not null,
		entryDate datetime not null,
		updated bit not null,
		entryType varchar(100) not null
	)
end
go

if exists (select * from sys.objects where name = 'spAdditionalEntry' and type in (N'P'))
begin
	drop procedure spAdditionalEntry
end
go

create procedure spAdditionalEntry
@action varchar(50) = '',
@id int = null,
@reason varchar(max) = '',
@entryDate datetime = null,
@updated bit = null,
@entryType varchar(100) = ''
as 
begin
	if (@action = 'Insert')
	begin
		insert into tblAdditionalEntry (reason,entryDate,updated,entryType)
		values (@reason,@entryDate,@updated,@entryType)
	end

	if (@action = 'GetAdditionalEntries')
	begin
		select *
		from tblAdditionalEntry
		where updated = 0
	end

	if (@action = 'UpdateState')
	begin
		update tblAdditionalEntry
		set updated = @updated
		where id = @id
	end
end
go

if not exists (select * from sys.objects where name = 'tblHoliday' and type in (N'U'))
begin
	create table tblHoliday(
		id int not null primary key,
		dateHoliday datetime not null
	)
end
go

if exists (select * from sys.objects where name = 'spHoliday' and type in (N'P'))
begin
	drop procedure spHoliday
end
go

create procedure spHoliday
@action varchar(50) = '',
@id int = null,
@dateholiday datetime = null,
@initialDate datetime = null,
@finalDate datetime = null
as
begin
	if (@action = 'Insert')
	begin
		insert into tblHoliday (id, dateHoliday)
		values (@id, @dateholiday)
	end

	if (@action = 'Update')
	begin
		update tblHoliday
		set dateHoliday = @dateholiday
		where id = @id
	end

	if (@action = 'GetHolidays')
	begin
		select *
		from tblHoliday
	end

	if (@action = 'GetHolidayById')
	begin
		select *
		from tblHoliday
		where id = @id
	end

	if (@action = 'GetHolidays')
	begin
		select *
		from tblHoliday
		where convert(varchar(10),dateHoliday,111) between convert(varchar(10),@initialDate,111) and convert(varchar(10),@finalDate,111)
	end
end
go

if not exists (select * from sys.objects where name = 'tblReplicatedFingerprint' and type in (N'U'))
begin
	create table tblReplicatedFingerprint(
		intPkId int not null identity(1,1) primary key,
		fingerprintId int not null,
		userId varchar(50) not null,
		ipAddress varchar(100) not null,
		replicationDate datetime not null
	)
end
go

if not exists(select * from dbo.syscolumns AS sc INNER JOIN sysobjects AS so ON
sc.id=so.id where sc.name = 'bitDelete' and so.name = 'tblReplicatedFingerprint')
alter table tblReplicatedFingerprint add bitDelete bit default 0
GO


if exists (select * from sys.objects where name = 'spReplicatedFingerprint' and type in (N'P'))
begin
	drop procedure spReplicatedFingerprint
end
go

create procedure spReplicatedFingerprint
@action varchar(50) = '',
@id int = null,
@fingerprintId int = null,
@userId varchar(50) = '',
@ipAddress varchar(100) = '',
@replicationDate datetime = null,
@fingerprints varchar(max) = ''
as
begin
	declare @strSql varchar(max)

	if (@action = 'Insert')
	begin
		insert into tblReplicatedFingerprint (fingerprintId,userId,ipAddress,replicationDate)
		values (@fingerprintId,@userId,@ipAddress,GETDATE())
	end

	if (@action = 'DeleteFingerprintReplicated')
	begin
		delete from tblReplicatedFingerprint
		where fingerprintId = @fingerprintId and userId = @userId
	end

	if (@action = 'DeleteFingerprints')
	begin
		set @strSql = 'delete 
					   from tblReplicatedFingerprint
					   where ipAddress = ''' + @ipAddress + ''' and fingerprintId in (' + @fingerprints + ')'

		exec (@strSql)
	end
end
go

if not exists (select * from sys.objects where name = 'tblSchedules' and type in (N'U'))
begin
	create table tblSchedules(
		id int not null primary key identity(1,1),
		executionHour datetime not null,
		ipAddress varchar(100) null,
		type varchar(200) not null
	)
end
go

if exists (select * from sys.objects where name = 'spSchedules' and type in (N'P'))
begin
	drop procedure spSchedules
end
go

create procedure spSchedules
@action varchar(50) = '',
@terminalId int = null,
@ipAddress varchar(100) = ''
as
begin
	if (@action = 'GetFingerprintSchedules')
	begin
		select *
		from tblSchedules
		where type = 'RemoveFingerprints'
	end

	if (@action = 'GetUserSchedules')
	begin
		select *
		from tblSchedules
		where type = 'RemoveUsers'
	end

	if (@action = 'GetEventSchedule')
	begin
		select *
		from tblSchedules
		where type = 'DownloadEvents'
	end

	if (@action = 'GetReplicateUserSchedules')
	begin
		select *
		from tblSchedules
		where type = 'ReplicateUsers'
	end
end
go

if not exists (select * from sys.objects where name = 'tblReplicatedMessages' and type in (N'U'))
begin
	create table tblReplicatedMessages(
		intPkId int not null identity(1,1) primary key,
		messageId int not null,
		ipAddress varchar(100) not null,
		replicationDate datetime not null
	)
end
go

if exists (select * from sys.objects where name = 'spReplicatedMessages' and type in (N'P'))
begin
	drop procedure spReplicatedMessages
end
go

create procedure spReplicatedMessages
@action varchar(50) = '',
@id int = null,
@messageId int = null,
@ipAddress varchar(100) = '',
@replicationDate datetime = null
as
begin
	if (@action = 'Insert')
	begin
		insert into tblReplicatedMessages (messageId,ipAddress,replicationDate)
		values (@messageId,@ipAddress,@replicationDate)
	end

	if (@action = 'DeleteFingerprintReplicated')
	begin
		delete from tblReplicatedMessages
		where messageId = @messageId
	end

	if (@action = 'GetMessagesWithoutReplicate')
	begin
		select *
		from tblClientMessages
		where messageId not in (select rm.messageId
								from tblReplicatedMessages rm
								where rm.ipAddress = @ipAddress)
			  and messageImage is not null
	end
end
go

if not exists (select * from sys.objects where name = 'tblReplicatedUsers' and type in (N'U'))
begin
	create table tblReplicatedUsers(
		intPkId int not null identity(1,1) primary key,
		userId varchar(50) not null,
		fingerprintId int not null,
		ipAddress varchar(100) not null,
		replicationDate datetime not null
	)
end
go

if not exists (select * from sys.objects where name like '%type_tblReplicatedUsers%' and type in (N'TT'))
begin
	CREATE TYPE type_tblReplicatedUsers AS TABLE(
		userId varchar(50),
		fingerprintId int,
		ipAddress varchar(100)
	)
end
go

if exists (select * from sys.objects where name = 'spReplicatedUser' and type in (N'P'))
begin
	drop procedure spReplicatedUser
end
go

create procedure spReplicatedUser
@action varchar(50) = '',
@id int = null,
@userId varchar(50) = '',
@fingerprintId int = null,
@ipAddress varchar(100) = '',
@replicationDate datetime = null,
@users varchar(max) = '',
@tblReplicatedUsers type_tblReplicatedUsers readonly
as
begin
	declare @strSql varchar(max)

	if (@action = 'Insert')
	begin
		insert into tblReplicatedUsers (userId,fingerprintId,ipAddress,replicationDate)
		values (@userId,@fingerprintId,@ipAddress,@replicationDate)
	end

	if (@action = 'InsertTable')
	begin
		insert into tblReplicatedUsers (userId,fingerprintId,ipAddress,replicationDate)
		select userId,fingerprintId,ipAddress,getdate()
		from @tblReplicatedUsers
	end

	if (@action = 'DeleteUserReplicated')
	begin
		delete from tblReplicatedUsers
		where userId = @userId and fingerprintId = @fingerprintId
	end

	if (@action = 'DeleteUsers')
	begin
		delete
		from tblReplicatedUsers
		where ipAddress = @ipAddress and userId in (SELECT value FROM STRING_SPLIT(@users, ','))
	end
end
go

if not exists (select * from sys.objects where name = 'tblUsers' and type in (N'U'))
begin
	create table tblUsers(
		PK_userId int not null identity(1,1) primary key,
		userId varchar(50) not null,
		userName varchar(20) not null,
		fingerprintId int not null,
		withoutFingerprint bit not null,
		bitInsert bit not null,
		bitDelete bit not null,
		bitUsed bit not null,
		registerDate datetime not null,
		modifiedDate datetime not null
	)
end
go

if not exists (select * from sys.objects where name like '%type_tblUsers%' and type in (N'TT'))
begin
	CREATE TYPE type_tblUsers AS TABLE(
		userId varchar(50),
		userName varchar(20),
		fingerprintId int,
		withoutFingerprint bit,
		bitInsert bit,
		bitDelete bit
	)
end
go

if exists (select * from sys.objects where name = 'spUser' and type in (N'P'))
begin
	drop procedure spUser
end
go

create procedure spUser
@action varchar(50) = '',
@userId varchar(50) = '',
@userName varchar(20) = '',
@fingerprintId int = null,
@ipAddress varchar(100) = '',
@withoutFingerprint bit = null,
@bitInsert bit = null,
@bitDelete bit = null,
@registerDate datetime = null,
@bitUsed bit = null,
@ids varchar(max) = '',
@tblUsers type_tblUsers readonly,
@exist bit = 0
as
begin
	declare @strSql varchar(max)

	if (@action = 'InsertTable')
	begin
		insert into tblUsers(userId,userName,fingerprintId,withoutFingerprint,bitInsert,bitDelete,registerDate,bitUsed,modifiedDate)
		select userId,userName,fingerprintId,withoutFingerprint,bitInsert,bitDelete,getdate(),0,getdate()
		from @tblUsers
		where ((fingerprintId is not null and fingerprintId > 0) or ((fingerprintId = 0 or fingerprintId is null) and withoutFingerprint = 1))
	end

	if (@action = 'Insert')
	begin
		if ((@fingerprintId is not null and @fingerprintId > 0) or ((@fingerprintId = 0 or @fingerprintId is null) and @withoutFingerprint = 1))
		begin
			insert into tblUsers(userId,userName,fingerprintId,withoutFingerprint,bitInsert,bitDelete,registerDate,bitUsed,modifiedDate)
			values (@userId,@userName,@fingerprintId,@withoutFingerprint,@bitInsert,@bitDelete,getdate(),0,getdate())
		end
	end

	if (@action = 'InsertUser')
	begin
		if ((@fingerprintId is not null and @fingerprintId > 0) or ((@fingerprintId = 0 or @fingerprintId is null) and @withoutFingerprint = 1))
		begin
			set @exist = isnull((select top 1 1 from tblUsers where userId = @userId),0)

			if (@exist = 1)
			begin
				update tblUsers
				set bitDelete = @bitDelete, withoutFingerprint = @withoutFingerprint, bitInsert = @bitInsert, bitUsed = 0, 
					modifiedDate = getdate(), fingerprintId = @fingerprintId
				where userId = @userId
			end
			else
			begin
				insert into tblUsers(userId,userName,fingerprintId,withoutFingerprint,bitInsert,bitDelete,registerDate,bitUsed,modifiedDate)
				values (@userId,@userName,@fingerprintId,@withoutFingerprint,1,0,getdate(),0,getdate())
			end

			if exists (select top 1 1 from tblReplicatedUsers where userId = @userId)
			begin
				delete from tblReplicatedUsers where userId = @userId
			end
		end
	end

	if (@action = 'GetUsersToReplicate')
	begin
		select *
		from tblUsers us
		where us.userId not in (select ru.userId
								from tblReplicatedUsers ru
								where ru.ipAddress = @ipAddress)
			  and us.bitInsert = 1 --and us.bitUsed = 0
	end

	if (@action = 'UpdateUsedUsers')
	begin
		set @strSql = 'update tblUsers
					   set bitUsed = 1, modifiedDate = getdate()
					   where PK_userId in (' + @ids + ')'
		exec (@strSql)
	end

	if (@action = 'Update')
	begin
		update tblUsers
		set bitDelete = @bitDelete, withoutFingerprint = @withoutFingerprint, bitInsert = @bitInsert, bitUsed = 0, 
			modifiedDate = getdate(), fingerprintId = @fingerprintId
		where userId = @userId

		if (@bitInsert = 1)
		begin
			if exists (select top 1 1 from tblReplicatedUsers where userId = @userId)
			begin
				delete from tblReplicatedUsers where userId = @userId
			end
		end
	end

	if (@action = 'UpdateFingerprintId')
	begin
		update tblUsers
		set fingerprintId = @fingerprintId, bitUsed = 0
		where userId = @userId
	end

	if (@action = 'GetUsersToDelete')
	begin
		select *
		from tblUsers
		where bitDelete = 1 and userId in (select userId from tblReplicatedUsers where ipAddress = @ipAddress)
	end

	if (@action = 'GetUser')
	begin
		select top 1 *
		from tblUsers
		where userId = @userId
	end

	if (@action = 'InsertOrUpdateUsers')
	begin
		declare @quantity int = (select count(*)
							 from @tblUsers),
				@aux int = 0

		declare cursor_users cursor for
			select *
			from @tblUsers
		open cursor_users

		while (@aux < @quantity)
		begin
			fetch next from cursor_users
			into @userId, @userName, @fingerprintId, @withoutFingerprint, @bitInsert, @bitDelete
			set @exist = isnull((select top 1 1 from tblUsers where userId = @userId),0)

			if (@exist = 1)
			begin
				--ACTUALIZAMOS EL USUARIO PARA QUE SEA ACTUALIZADO EN LAS TERMINALES
				update tblUsers
				set bitDelete = @bitDelete, bitInsert = @bitInsert, bitUsed = 0, 
					fingerprintId = @fingerprintId, withoutFingerprint = @withoutFingerprint, modifiedDate = getdate()
				where userId = @userId

				if (@bitInsert = 1)
				begin
					--BORRAMOS EL USUARIO REPLICADO ANTERIORMENTE
					delete
					from tblReplicatedUsers
					where userId = @userId
				end
			end
			else
			begin
				if (@bitInsert = 1)
				begin
					if ((@fingerprintId is not null and @fingerprintId > 0) or ((@fingerprintId = 0 or @fingerprintId is null) and @withoutFingerprint = 1))
					begin
						insert into tblUsers(userId,userName,fingerprintId,withoutFingerprint,bitInsert,bitDelete,bitUsed,registerDate,modifiedDate)
						values(@userId,@userName,@fingerprintId,@withoutFingerprint,@bitInsert,@bitDelete,0,getdate(),getdate())
					end
				end
			end

			set @aux = @aux + 1
		end

		close cursor_users
		deallocate cursor_users
	end
end
go

if not exists (select * from sys.objects where name like '%type_tblClientCards%' and type in (N'TT'))
begin
	CREATE TYPE type_tblClientCards AS TABLE(
		clientCardId int,
		clientId varchar(15),
		cardCode varchar(20),
		state bit
	)
end
go

if exists (select * from sys.objects where name = 'spClientCard' and type in (N'P'))
begin
	drop procedure spClientCard
end
go

create procedure spClientCard
@action varchar(50) = '',
@id int = null,
@clientId varchar(15) = '',
@cardCode varchar(20) = '',
@cardId int = null,
@state bit = null,
@tblClientCards type_tblClientCards readonly,
@clients varchar(max) = ''
as
begin
	if (@action = 'InsertTable')
	begin
		insert tblClientCards (id,clientId,cardCode,state)
		select *
		from @tblClientCards
	end

	if (@action = 'DeleteAll')
	begin
		delete from tblClientCards
	end

	if (@action = 'DeleteByClients')
	begin
		delete from tblClientCards where clientId in (SELECT value FROM STRING_SPLIT(@clients, ','))		
	end

	if (@action = 'GetCardById')
	begin
		select Count(*) from tblClientCards where id = @cardId
	end
end
go
if exists (select * from sys.objects where name = 'spZonas' and type in (N'P'))
begin
	drop procedure spZonas
end
go
CREATE PROCEDURE [dbo].[spZonas] 
@action varchar(50) = '',
@id int = NULL,
@codigo_string varchar(50) = '',
@cdgimnasio int = NULL,
@descripcion varchar(50) = '',
@nombre_zona varchar(50) = '',
@intSucursal int = NULL,
@Terminales varchar(50) = '',
@bitEstado bit = NULL,
@fechacreacion datetime = NULL

AS
BEGIN
  IF (@action = 'GetZonasById')
  BEGIN
    SELECT
      *
    FROM tbl_Maestro_Zonas
    WHERE id = @id
  END

  IF (@action = 'Update')
  BEGIN
    UPDATE tbl_Maestro_Zonas
    SET codigo_string = @codigo_string,
        cdgimnasio = @cdgimnasio,
        descripcion = @descripcion,
        nombre_zona = @nombre_zona,
        intSucursal = @intSucursal,
        Terminales = @Terminales,
        bitEstado = @bitEstado,
        fechacreacion = @fechacreacion
    WHERE id = @id
  END
  IF (@action = 'Insert')
  BEGIN
    INSERT INTO tbl_Maestro_Zonas (id,
    codigo_string,
    cdgimnasio,
    descripcion,
    nombre_zona,
    intSucursal,
    Terminales,
    bitEstado,
    fechacreacion)
      VALUES (@id, @codigo_string, @cdgimnasio, @descripcion, @nombre_zona, @intSucursal, @Terminales, @bitEstado, @fechacreacion)
  END

END
go