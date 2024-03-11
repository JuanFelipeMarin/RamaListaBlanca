Imports System.Data.SqlClient
Imports System.Net
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json

Public Class HuellasDAL
    Public strRutaBase As String
    Public strRutaBasePrincipal As String
    Private objCx As SqlConnection

    Public Function solicitarDatos(ByVal strConsulta As String, ByVal strConnection As String) As DataSet
        Dim conConexion As SqlConnection = Nothing
        Dim cmmComando As SqlCommand = Nothing
        Dim dtsConsultado As New DataSet
        Dim dtaAdaptador As SqlDataAdapter
        Try
            ConexionAplicacion = strConnection

            cmmComando = New SqlCommand()
            conConexion = New SqlConnection(ConexionAplicacion)
            cmmComando.Connection = conConexion
            cmmComando.CommandText = strConsulta
            cmmComando.CommandType = CommandType.Text
            conConexion.Open()
            dtaAdaptador = New SqlDataAdapter(cmmComando)
            dtaAdaptador.Fill(dtsConsultado)
            Return dtsConsultado
        Catch ex As Exception
            conConexion.Close()
            Return Nothing
        Finally
            conConexion.Close()
            cmmComando.Dispose()
        End Try
    End Function

    Public Function ActualizarDatos(ByVal strConsulta As String, ByVal strConnection As String) As DataSet
        Dim conConexion As SqlConnection = Nothing
        Dim cmmComando As SqlCommand = Nothing
        Dim dtsConsultado As New DataSet
        Dim dtaAdaptador As SqlDataAdapter
        Try
            ConexionAplicacion = strConnection

            cmmComando = New SqlCommand()
            conConexion = New SqlConnection(ConexionAplicacion)
            cmmComando.Connection = conConexion
            cmmComando.CommandText = strConsulta
            cmmComando.CommandType = CommandType.Text
            conConexion.Open()
            dtaAdaptador = New SqlDataAdapter(cmmComando)
            dtaAdaptador.Fill(dtsConsultado)
            Return dtsConsultado
        Catch ex As Exception
            conConexion.Close()
            Return Nothing
        Finally
            conConexion.Close()
            cmmComando.Dispose()
        End Try
    End Function

    Public Function vericarExistenciaHuella(ByVal identificacion As String, ByVal strConnection As String) As Integer
        Dim dsHuellas As DataSet = solicitarDatos("select * from tblFingerprint where personId = " & identificacion, strConnection)
        If Not dsHuellas Is Nothing AndAlso dsHuellas.Tables.Count > 0 AndAlso dsHuellas.Tables(0).Rows.Count > 0 Then
            If dsHuellas.Tables.Count >= 1 Then
                intHuella = 2
                Return intHuella
            Else
                intHuella = 1
                Return intHuella
            End If
        Else
            intHuella = 1
            Return intHuella
        End If
    End Function

    Public Function ConsultarHuellas_Grabadas(ByVal identificacion As String, ByVal strConnection As String) As DataTable
        Dim dsHuellas As DataSet = solicitarDatos("select * from tblFingerprint where personId = " & identificacion, strConnection)
        If Not dsHuellas Is Nothing AndAlso dsHuellas.Tables.Count > 0 AndAlso dsHuellas.Tables(0).Rows.Count > 0 Then
            Return dsHuellas.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function ConsultarUltimoID_Huellas(ByVal strConnection As String) As DataTable
        Dim dsHuellas As DataSet = solicitarDatos("select ISNULL(MAX(CONVERT(int,[intIndiceHuellaActual])),0) + 1 as Ultimo_ID from tblFingerprint", strConnection)
        If Not dsHuellas Is Nothing AndAlso dsHuellas.Tables.Count > 0 AndAlso dsHuellas.Tables(0).Rows.Count > 0 Then
            Return dsHuellas.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function ConsultarUltimoID_HuellasTablaHuellas(ByVal strConnection As String) As Integer
        Dim inIndice As Integer = 1
        'NO SE MANEJA DEDO
        'Dim dsHuellas As DataSet = solicitarDatos("select ISNULL(MAX([intIndiceDedoUno]),0) as Ultimo_IDUno, ISNULL(MAX([intIndiceDedoDos]),0) as Ultimo_IDDos from tblFingerprint", strConnection)
        'If Not dsHuellas Is Nothing AndAlso dsHuellas.Tables.Count > 0 AndAlso dsHuellas.Tables(0).Rows.Count > 0 Then
        '    If dsHuellas.Tables(0).Rows(0).Item("Ultimo_IDUno") > dsHuellas.Tables(0).Rows(0).Item("Ultimo_IDDos") Then
        '        inIndice = dsHuellas.Tables(0).Rows(0).Item("Ultimo_IDUno") + 1
        '    Else
        '        inIndice = dsHuellas.Tables(0).Rows(0).Item("Ultimo_IDDos") + 1
        '    End If
        'End If
        Return inIndice
    End Function

    Public Function BorrarHuella(ByVal identificacion As String, ByVal strConnection As String) As DataTable
        'If numero_dedo = "1" Then
        Dim dsHuellas As DataSet = ActualizarDatos("update tblFingerprint set fingerprint = NULL where personId = '" & identificacion & "'", strConnection)
        'Else
        '    Dim dsHuellas As DataSet = ActualizarDatos("update tblFingerprint set hue_dato = NULL where personId = '" & identificacion & "'", strConnection)
        'End If
    End Function

    Public Function Consultar_ID_x_Identificacion_Dedo_Equipo(ByVal strpkIdentificacion As String, ByVal strConnection As String) As DataTable
        Dim ds As DataSet = solicitarDatos("select * from tblFingerprint where personId = " & strpkIdentificacion, strConnection)
        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function Consultar_Huellas_Pendientes_Por_Eliminar_Del_Dispositivo(ByVal strpkIdentificacion As String, ByVal strConnection As String) As DataTable
        Dim ds As DataSet = solicitarDatos("select * from tblFingerprint where personId = " & strpkIdentificacion, strConnection)
        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function Consultar_Huellas_Pendientes_Por_Replicar(ByVal strConnection As String) As DataTable
        Dim ds As DataSet = solicitarDatos("select top 3000 * from tblHuellasCAMA2000 where strNombreEquipo = '" & My.Computer.Name & "' and bitReplicado = 0", strConnection)
        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function BorrarHuellaDeHuellasCAMA2000(ByVal intpkCodigo As Integer, ByVal strConnection As String) As Boolean
        Try
            ActualizarDatos("delete from tblFingerprint where intPkId =" & intpkCodigo, strConnection)
            Return True
        Catch ex As Exception
            Return False
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Function

    Public Function BorrarReplicaHuellaDeHuellasCAMA2000(ByVal identificacion As String, ByVal strConnection As String)
        Try
            ActualizarDatos("delete from tblReplicatedFingerprint where userId =" & identificacion, strConnection)
        Catch ex As Exception
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Function

    Public Function ProgramarBorradoHuellasOtrosCAMA2000(ByVal strpkIdentificacion As String, ByVal strConnection As String) As Boolean
        Try
            ActualizarDatos("Delete From tblFingerprint where personId = " & strpkIdentificacion, strConnection)
            Return True
        Catch ex As Exception
            Return False
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Function

    Public Function BorradoHuellasOtrosCAMA2000(ByVal intID As Integer, ByVal strConnection As String) As Boolean
        Try
            ActualizarDatos("Delete From tblHuellasCAMA2000 where intpkCodigo = " & intID, strConnection)
            Return True
        Catch ex As Exception
            Return False
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Function

    Public Function BorrarTodasLasHuellaDeHuellasCAMA2000(ByVal strConnection As String) As Boolean
        Try
            ActualizarDatos("Update tblHuellasCAMA2000 Set bitReplicado = 0, dtmFechaReplicado = NULL where strNombreEquipo = '" & My.Computer.Name & "'", strConnection)
            Return True
        Catch ex As Exception
            Return False
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Function

    Public Function CambiarEstado_HuellasCAMA2000(ByVal intpkCodigo As Integer, ByVal strConnection As String) As Boolean
        Try
            Dim format As String = "yyyy/MM/dd HH:mm:ss"
            ActualizarDatos("SET DATEFORMAT ymd Update tblHuellasCAMA2000 set bitReplicado = 1, dtmFechaReplicado = '" & DateTime.Now.ToString(format) & "' where intpkCodigo =" & intpkCodigo, strConnection)
            Return True
        Catch ex As Exception
            Return False
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Function

    Public Function Actualizar_LectoresCAMA2000(ByVal strConnection As String) As DataTable
        Dim ds As DataSet = solicitarDatos("select * from tblLectoresCAMA2000 where UPPER(strNombreEquipo) = UPPER('" & My.Computer.Name & "')", strConnection)
        If ds Is Nothing Or ds.Tables.Count = 0 Or ds.Tables(0).Rows.Count < 1 Then
            Dim dsHuellas As DataSet = ActualizarDatos("Insert into tblLectoresCAMA2000 Values ('" & My.Computer.Name & "', 1)", strConnection)
        Else
            Return Nothing
        End If
    End Function

    Public Function Consultar_Replicas_Pendientes_x_Equipo(ByVal strConnection As String) As DataTable
        Dim ds As DataSet = solicitarDatos("Select 0 as SinReplicar, (select count(*) from tblFingerprint) as Replicado", strConnection)
        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function Consultar_LectoresCAMA2000(ByVal strConnection As String) As DataTable
        'CONSULTAR LOS DISPOSITIVOS POR FUERA DEL PROPIO
        Dim ds As DataSet = solicitarDatos("select * from tblLectoresCAMA2000 where bitActivo = 1 and strNombreEquipo <> '" & My.Computer.Name & "' ", strConnection)
        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            Return ds.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Function Consultar_Huellas(ByVal identificacion As String, ByVal strConnection As String) As DataTable
        Dim dsHuellas As DataSet = solicitarDatos("select * from tblFingerprint", strConnection)
        If Not dsHuellas Is Nothing AndAlso dsHuellas.Tables.Count > 0 AndAlso dsHuellas.Tables(0).Rows.Count > 0 Then
            Return dsHuellas.Tables(0)
        Else
            Return Nothing
        End If
    End Function

    Public Sub guardarHuella(ByVal gymId As Integer, ByVal branchId As Integer, ByVal identificacion As String, ByVal huella() As Byte, ByVal strConnection As String, ByVal intID As Integer)
        Try
            Dim indiceMayorH As Integer = ConsultarUltimoID_HuellasTablaHuellas(strConnection)
            objCx = New SqlConnection
            objCx.ConnectionString = strConnection
            Try
                objCx.Open()
            Catch ex As Exception
                Throw New Exception(ex.Message)
                Exit Sub
            End Try

            Dim dsTemporal2 As New DataSet
            Dim objDA2 As SqlDataAdapter
            objDA2 = New SqlDataAdapter("Select * From tblFingerprint Where personId = " & identificacion, objCx)
            Dim cmdBuilder2 As SqlCommandBuilder = New SqlCommandBuilder(objDA2)
            objDA2.MissingSchemaAction = MissingSchemaAction.AddWithKey
            objDA2.FillSchema(dsTemporal2, SchemaType.Source, "tblFingerprint")

            Dim dtUltimoIdgim_huellas As New DataTable
            dtUltimoIdgim_huellas = ConsultarUltimoID_Huellas(strConnection)

            Dim nuevaFilaUnica As DataRow = dsTemporal2.Tables(0).NewRow

            Dim url As String = System.Configuration.ConfigurationSettings.AppSettings("urlApi").ToString()
            Dim url_Final As String = url & "api/Fingerprint/ValidatePersonToSaveFingerprint/" & gymId & "/" & branchId & "/" & identificacion

            Dim origRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(url_Final), HttpWebRequest)
            origRequest.Headers.Add("Authentication", "Basic Z33JxZmtZXRhbDpHTzliZX25yazE3Kg==")
            origRequest.AllowAutoRedirect = False
            origRequest.Method = "GET"
            'System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
            Dim objResponse As HttpWebResponse
            objResponse = DirectCast(origRequest.GetResponse(), HttpWebResponse)
            Dim Stream As Stream = objResponse.GetResponseStream()
            Dim sr As New StreamReader(Stream, Encoding.GetEncoding("utf-8"))
            Dim str As String = sr.ReadToEnd()

            Dim results = JsonConvert.DeserializeObject(Of JsonResponse)(str)

            nuevaFilaUnica.Item("fingerprintId") = Convert.ToInt32(results.messageTwo) '0 'Convert.ToInt32(dtUltimoIdgim_huellas.Rows(0).Item("Ultimo_ID"))
            nuevaFilaUnica.Item("personId") = identificacion
            'nuevaFilaUnica.Item("hue_dedo") = intID
            nuevaFilaUnica.Item("fingerprint") = huella
            'nuevaFilaUnica.Item("hue_dedo") = intHuella
            nuevaFilaUnica.Item("bitInsert") = 1
            nuevaFilaUnica.Item("bitDelete") = 0
            nuevaFilaUnica.Item("registerDate") = DateTime.Now
            nuevaFilaUnica.Item("bitUsed") = 1
            'nuevaFilaUnica.Item("EstaReplicada") = 1
            nuevaFilaUnica.Item("intIndiceHuellaActual") = intID
            dsTemporal2.Tables(0).Rows.Add(nuevaFilaUnica)

            objDA2.Update(dsTemporal2, "tblFingerprint")
            objCx.Close()

            'Mandamos a actualizar el binario de la huella para que quede en GymSoft Web
            ValidateFingerprintToRecord(gymId, branchId, identificacion, Convert.ToInt32(results.messageTwo), huella)

            'Actualizamos el fingerprintId en tblWhiteList
            UpdateFingerPrintIdtblWhiteList(strConnection, gymId, branchId, identificacion, Convert.ToInt32(results.messageTwo), huella)
        Catch ex As Exception
            MsgBox(ex.Message)
            objCx.Close()
        End Try
    End Sub

    Public Sub UpdateFingerPrintIdtblWhiteList(ByVal strConnection As String, ByVal gymId As Integer, ByVal branchId As Integer, ByVal clientId As String, ByVal fingerId As Integer, ByVal huella As Byte())
        Dim rowsAffected As Integer

        Using con As New SqlConnection(strConnection)
            Using cmd As New SqlCommand("UPDATE tblWhiteList Set fingerprintId = " & fingerId & ", fingerprint = @fingerprint WHERE id = '" & clientId & "' and branchId = " & branchId & " and gymId = " & gymId, con)
                cmd.Parameters.Add("@fingerprint", SqlDbType.Binary).Value = huella

                con.Open()
                rowsAffected = cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Sub ValidateFingerprintToRecord(ByVal gymId As Integer, ByVal branchId As Integer, ByVal clientId As String, ByVal fingerId As Integer, ByVal fingerprintImage As Byte())
        Dim url As String = System.Configuration.ConfigurationSettings.AppSettings("urlApi").ToString()
        Dim url_Final As String = url & "api/Fingerprint/ValidateAndSaveFingerprint"

        Dim postData As New Dictionary(Of String, Object)
        postData.Add("finger", 1)
        postData.Add("fingerPrint", fingerprintImage)
        postData.Add("gymId", gymId)
        postData.Add("branchId", branchId)
        postData.Add("id", fingerId)
        postData.Add("personId", clientId)
        postData.Add("quality", 100)

        Dim json As String = JsonConvert.SerializeObject(postData)
        Dim byteArray As Byte() = System.Text.Encoding.UTF8.GetBytes(json)

        Dim origRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(url_Final), HttpWebRequest)
        origRequest.Headers.Add("Authentication", "Basic Z33JxZmtZXRhbDpHTzliZX25yazE3Kg==")
        origRequest.AllowAutoRedirect = False
        origRequest.Method = "POST"
        origRequest.ContentLength = byteArray.Length
        origRequest.ContentType = "application/json"

        Dim dataStream As Stream = origRequest.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()

        'System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        Dim objResponse As HttpWebResponse
        objResponse = DirectCast(origRequest.GetResponse(), HttpWebResponse)
        Dim Stream As Stream = objResponse.GetResponseStream()
        Dim sr As New StreamReader(Stream, Encoding.GetEncoding("utf-8"))
        Dim str As String = sr.ReadToEnd()

        'Dim results = JsonConvert.DeserializeObject(Of JsonResponse)(Str)
    End Sub
End Class

Public Class JsonResponse
    <JsonProperty("state")>
    Public Property State As Boolean

    <JsonProperty("message")>
    Public Property Message As String

    <JsonProperty("messageOne")>
    Public Property MessageOne As String

    <JsonProperty("messageTwo")>
    Public Property MessageTwo As String

    <JsonProperty("messageThree")>
    Public Property MessageThree As String
End Class