Imports System.IO
Imports System.Runtime.InteropServices
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Net
Imports Newtonsoft.Json


Public Class Form1
    Dim objAccesoDatos As New HuellasDAL
    Dim g_bCancel As Boolean
    Dim g_nIdentifyMode As Byte
    Dim separadas As String()
    Dim FingerNumber As Integer
    Dim conex As String
    Dim persona As String
    Dim Administrador As Boolean
    Dim BorradoMasivoAlSincronizar As Boolean
    Dim gymID As Integer
    Dim SucursalID As Integer
    Dim Opcion As Integer
    Dim GrabandoHuella As Boolean
    Dim Contratos As Boolean
    'Dim intID_DedoUno As Integer
    'Dim intID_DedoDos As Integer
    Dim s As String

    Dim cantidadActualHuellas As Integer

    Public Sub New()
        InitializeComponent()
        Try
            If Environment.GetCommandLineArgs().Length > 1 Then
                For i = 1 To Environment.GetCommandLineArgs().Length - 1
                    If i = 1 Then
                        s = s + Environment.GetCommandLineArgs(i).Replace("_", " ")
                        s = s + " "
                    Else
                        s = s + Environment.GetCommandLineArgs(i) + " "
                    End If
                Next
            End If
        Catch ex As Exception
            s = Nothing
        End Try
    End Sub

    Public Function VarPtr1(ByVal e As Object) As Integer
        Dim GC As GCHandle = GCHandle.Alloc(e, GCHandleType.Pinned)
        Dim GC2 As Integer = GC.AddrOfPinnedObject.ToInt32
        GC.Free()
        Return GC2
    End Function

    Public Function VarPtr(ByVal o As Object) As Integer
        Dim GC As System.Runtime.InteropServices.GCHandle = System.Runtime.InteropServices.GCHandle.Alloc(o, System.Runtime.InteropServices.GCHandleType.Pinned)
        Dim ret As Integer = GC.AddrOfPinnedObject.ToInt32()

        GC.Free()
        Return ret
    End Function

    Sub ControlStatus(ByVal bEnable As Boolean)
        cmdEnroll.Enabled = bEnable
        txtID.Enabled = bEnable
        txtFN.Enabled = bEnable
        chkManager.Enabled = bEnable
        Label2.Enabled = bEnable
        Label3.Enabled = bEnable
        cmdIdentify.Enabled = bEnable
        cmdIdentify2.Enabled = bEnable
        cmdVerify.Enabled = bEnable
        cmdVerifyID.Enabled = bEnable
        cmdEnrollCount.Enabled = bEnable
        cmdCheckID.Enabled = bEnable
        cmdCheckFinger.Enabled = bEnable
        cmdSearchID.Enabled = bEnable
        cmdSearchFinger.Enabled = bEnable
        cmdFpCancel.Enabled = bEnable
        cmdRemove.Enabled = bEnable
        cmdRemoveAll.Enabled = bEnable
        cmdReadTemplate.Enabled = bEnable
        cmdWriteTemplate.Enabled = bEnable
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ControlStatus(False)

        Try
            If (s <> Nothing) Then
                Label1.Text = s
            Else
                s = "4449808 | Password=sa;Persist Security Info=True;User ID=sa;Initial Catalog=dbWhitelist;Data Source=SB-RYZENN2\DRESTREPO | 0 | 229 | 1 | False"
            End If

            separadas = Split(s, "|")
            conex = separadas(1)
            persona = separadas(0)
            'ActualizarLectores() ' Si no  existe el lector (Nombre del equipo) en tblLectoresCAMA2000 lo crea
            BorradoMasivoAlSincronizar = Convert.ToInt32(separadas(2))
            gymID = Convert.ToInt32(separadas(3))
            SucursalID = Convert.ToInt32(separadas(4))
            Contratos = Convert.ToBoolean(separadas(5))

            Me.Size = New Size(733, 471)
        Catch ex As Exception
            lblStatus.Text = MsgBox("Error en la cadena de conexión: " + ex.Message)
            ControlStatus(False)
            Return
        End Try

        Dim nRet As Integer

        SFEPCtrl.SFEPSetDatabasePath(Application.StartupPath)

        nRet = 0
        nRet = SFEPCtrl.SFEPInitialize()
        If nRet <> RES_OK Then
            If nRet = ERR_NOT_USBDEV Then
                lblStatus.Text = "No se pudo detectar el lector USB."
            Else
                lblStatus.Text = "Verifique que el sistema se esta ejecutando como administrador (Fail!!" + vbCrLf + "nERROR CODE = " + Str(nRet) + ")"
            End If
        Else
            'ConsultarHuellasPendientesPorEliminarDelHuellero()
            ConsultarReplicasPendientes_x_equipo()
            ConsultarHuellas()
            'btnSincronizarHuellas_Click(Nothing, Nothing)
            ControlStatus(True)
            lblStatus.Text = "Seleccione la huella a grabar."
            txtID.Text = "1"
            txtFN.Text = "1"
        End If
    End Sub

    Function ActualizarLectores()
        Dim dt As DataTable = objAccesoDatos.Actualizar_LectoresCAMA2000(conex)
    End Function

    Function ConsultarReplicasPendientes_x_equipo()
        Dim dt As DataTable = objAccesoDatos.Consultar_Replicas_Pendientes_x_Equipo(conex)
        If Not dt Is Nothing Then
            lblMensajeSincronizacion.Text = "Huellero CAMA2000 conectado al equipo: " & My.Computer.Name & vbCrLf & "Huellas pendientes por grabar en este huellero: " & dt.Rows(0).Item("SinReplicar") & vbCrLf & "Huellas grabadas en este huellero: " & dt.Rows(0).Item("Replicado")
            cantidadActualHuellas = dt.Rows(0).Item("Replicado")
        Else
            lblMensajeSincronizacion.Text = "Sin huellas creadas"
            cantidadActualHuellas = 0
        End If
    End Function

    Function BorrarHuellasEquipoMasivamente()
        Dim nRet As Long
        If objAccesoDatos.BorrarTodasLasHuellaDeHuellasCAMA2000(conex) Then
            nRet = SFEPCtrl.SFEPRemoveAll()
            ConsultarReplicasPendientes_x_equipo()
            ConsultarHuellas()
        End If
    End Function

    Function ConsultarHuellas()
        Dim Path As String = Application.StartupPath + "\\Fingers\\finger.fpt"
        Dim dt As DataTable = objAccesoDatos.ConsultarHuellas_Grabadas(Convert.ToString(persona), conex)

        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                If dt.Rows.Count >= 1 AndAlso Not IsDBNull(dt.Rows(0).Item("fingerprint")) Then
                    ptrImagenHuella.ImageLocation = Application.StartupPath + "\\huellapng.png"
                Else
                    ptrImagenHuella.ImageLocation = Nothing
                End If
            End If
        Else
            ptrImagenHuella.ImageLocation = Nothing
        End If
    End Function

    Function ConsultarHuellasPendientesPorEliminarDelHuellero()
        Dim dt As DataTable = objAccesoDatos.Consultar_Huellas_Pendientes_Por_Eliminar_Del_Dispositivo(Convert.ToString(persona), conex)
        If Not dt Is Nothing Then
            For index = 0 To dt.Rows.Count - 1
                Dim nRet As Long
                Dim dwID As Long, bFN As Byte
                txtID.Text = dt.Rows(index).Item("fingerprintId").ToString().Trim()
                txtFN.Text = dt.Rows(index).Item("intNumeroDedo").ToString().Trim()
                cmdRemove_Click(Nothing, Nothing)
                If lblStatus.Text = "Huella borrada correctamente" Then
                    objAccesoDatos.BorradoHuellasOtrosCAMA2000(Convert.ToInt32(txtID.Text), conex)
                End If
            Next
        End If
    End Function

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        SFEPCtrl.SFEPFpCancel()
        lblStatus.Text = ""
        g_bCancel = False
        SFEPCtrl.SFEPFpCancel()
        SFEPCtrl.SFEPUninitialize()
        ControlStatus(False)
        End
    End Sub

    Private Sub frmMain_UnLoad(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SFEPCtrl.SFEPFpCancel()
        lblStatus.Text = ""
        g_bCancel = False
        SFEPCtrl.SFEPFpCancel()
        SFEPCtrl.SFEPUninitialize()
        ControlStatus(False)
        End
    End Sub

    Private Sub cmdUnInit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub cmdFpCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdFpCancel.Click
        SFEPCtrl.SFEPFpCancel()
    End Sub

    Private Sub cmdEnroll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEnroll.Click
        If (Opcion = 1) Then ' Si es insertar
            Dim w_nRet
            ControlStatus(False)
            cmdFpCancel.Enabled = True
            w_nRet = SFEPCtrl.SFEPEnroll(CLng(txtID.Text), CByte(txtFN.Text), chkManager.Checked)
        End If

        If (Opcion = 2) Then '  Si es modificar
            Dim w_nRet
            ControlStatus(False)
            cmdFpCancel.Enabled = True
            BorrarHuella()
            ConsultarHuellas()
        End If
    End Sub

    Function BorrarHuella()
        Dim dt As DataTable = objAccesoDatos.BorrarHuella(Convert.ToString(persona), conex)
    End Function

    Private Sub cmdVerify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdVerify.Click
        Dim nRet As Long
        Dim dwID As Long, bFingerNumber As Byte

        dwID = CLng(txtID.Text)
        bFingerNumber = CByte(txtFN.Text)

        nRet = SFEPCtrl.SFEPCheckFingerNum(dwID, bFingerNumber)

        If nRet <> ERR_ENROLLED_FINGER Then
            lblStatus.Text = GetErrMsg(nRet)
            Exit Sub
        End If
        lblStatus.Text = "Presione su dedo"

        ControlStatus(False)
        cmdFpCancel.Enabled = True
        g_nIdentifyMode = VERIFY_MODE
        SFEPCtrl.SFEPIdentify()
    End Sub

    Private Sub cmdVerifyID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdVerifyID.Click
        lblStatus.Text = "Presione su dedo"
        ControlStatus(False)
        cmdFpCancel.Enabled = True
        g_nIdentifyMode = VERIFYID_MODE
        SFEPCtrl.SFEPIdentify()
    End Sub

    Private Sub cmdIdentify_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdIdentify.Click
        lblStatus.Text = "Presione su dedo"
        ControlStatus(False)
        cmdFpCancel.Enabled = True
        g_nIdentifyMode = IDENTIFY_MODE
        SFEPCtrl.SFEPIdentify()
    End Sub

    Private Sub cmdIdentify2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdIdentify2.Click
        lblStatus.Text = "Presione su dedo"
        ControlStatus(False)
        cmdFpCancel.Enabled = True
        SFEPCtrl.SFEPIdentifyFree()
    End Sub

    Private Sub cmdCheckID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCheckID.Click
        Dim nRet As Long
        Dim dwID As Long

        dwID = CLng(txtID.Text)
        nRet = SFEPCtrl.SFEPCheckID(dwID)
        lblStatus.Text = GetErrMsg(nRet)
    End Sub

    Private Sub cmdCheckFinger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCheckFinger.Click
        Dim nRet As Long
        Dim dwID As Long, bFingerNumber As Byte

        dwID = CLng(txtID.Text)
        bFingerNumber = CByte(txtFN.Text)
        nRet = SFEPCtrl.SFEPCheckFingerNum(dwID, bFingerNumber)
        lblStatus.Text = GetErrMsg(nRet)
    End Sub

    Private Sub cmdSearchID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSearchID.Click
        Dim nRet As Long

        txtID.Text = "0"
        txtFN.Text = "0"
        nRet = SFEPCtrl.SFEPSearchID()

        If nRet < 0 Then
            lblStatus.Text = GetErrMsg(nRet)
        Else
            lblStatus.Text = "Success."
            txtID.Text = Str(nRet)
        End If
    End Sub

    Private Sub cmdSearchFinger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSearchFinger.Click
        Dim nRet As Long
        Dim dwID As Long

        dwID = CLng(txtID.Text)
        txtFN.Text = "0"
        nRet = SFEPCtrl.SFEPSearchFingerNumber(dwID)

        If nRet < 0 Then
            lblStatus.Text = GetErrMsg(nRet)
        Else
            lblStatus.Text = "Success."
            txtFN.Text = Str(nRet)
        End If
    End Sub

    Private Sub cmdEnrollCount_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEnrollCount.Click
        Dim nRet As Integer

        nRet = SFEPCtrl.SFEPGetEnrollCount()
        lblStatus.Text = "Count = " + Str(nRet)
    End Sub

    Private Sub cmdRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRemove.Click
        Dim nRet As Long
        Dim dwID As Long, bFN As Byte

        dwID = CLng(txtID.Text)
        bFN = CByte(txtFN.Text)
        nRet = SFEPCtrl.SFEPRemoveTemplate(dwID, bFN)
        lblStatus.Text = "Huella borrada correctamente"
    End Sub

    Function Borrar_ID_del_Huellero() As String
        Dim dt As DataTable = objAccesoDatos.Consultar_ID_x_Identificacion_Dedo_Equipo(persona, conex)
        If Not dt Is Nothing Then
            If objAccesoDatos.BorrarHuellaDeHuellasCAMA2000(dt.Rows(0).Item("intPkId"), conex) Then
                'Se elimina de la tabla de huellas replicadas 
                objAccesoDatos.BorrarReplicaHuellaDeHuellasCAMA2000(persona, conex)

                Return dt.Rows(0).Item("intIndiceHuellaActual").ToString().Trim()
            Else
                Return "No"
            End If
        Else
            Return "No"
        End If
    End Function

    Private Sub cmdRemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRemoveAll.Click
        Dim dr As DialogResult
        dr = MessageBox.Show("¿Esta seguro de eliminar todas las huellas grabadas en este dispositivo conectado al equipo " & My.Computer.Name & "?", "Alerta ", MessageBoxButtons.YesNo)
        If (dr = DialogResult.Yes) Then
            Dim nRet As Long
            If objAccesoDatos.BorrarTodasLasHuellaDeHuellasCAMA2000(conex) Then
                nRet = SFEPCtrl.SFEPRemoveAll()
                ConsultarReplicasPendientes_x_equipo()
                ConsultarHuellas()
            End If
        End If
    End Sub

    Private Sub cmdReadTemplate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReadTemplate.Click
        Dim nRet As Long, i As Integer
        Dim dwID As Long, bFN As Byte

        i = 0
        dwID = CLng(txtID.Text)
        bFN = CByte(txtFN.Text)
        nRet = SFEPCtrl.SFEPReadTemplate(dwID, bFN, Application.StartupPath + "\\Fingers\\finger.fpt")
        Select Case (nRet)
            Case ERR_INVALID_ID
                lblStatus.Text = "Invalid ID number"
            Case ERR_INVALID_FINGERNUM
                lblStatus.Text = "Invalid Finger number"
            Case ERR_NOT_ENROLLED
                lblStatus.Text = "Huella no enrolada."
            Case RES_OK
                '   lblStatus.Text = "Success read template." + vbCrLf + "Saved in 'c:/finger.fpt'"
        End Select
    End Sub

    Function GetErrMsg(ByVal nErrCode As Long)
        Dim str As String

        str = " "

        Select Case nErrCode
            Case ERR_MEMORY
                str = "Error en memoria"
            Case ERR_INVALID_ID
                str = "Invalid ID"
            Case ERR_INVALID_TEMPLATE
                str = "Huella invalida"
            Case ERR_NOT_EMPTY
                str = ""
            Case ERR_DISK_SIZE
                str = "Error Disk Size"
            Case ERR_NOT_USBDEV
                str = "No se puede detectar el lector USB"
            Case ERR_INVALID_PARAMETER
                str = "Invalid parameter."
            Case ERR_FP_TIMEOUT
                str = "Tiempo de espera superado"
            Case ERR_BAD_QUALITY
                str = "Bad Quality"
            Case ERR_FAIL_GEN
                str = "Failed in Generating"
            Case ERR_FAIL_FILE_IO
                str = "Huella no enrolada."
            Case ERR_ENROLLED_ID
                str = "This ID number is used"
            Case ERR_ENROLLED_FINGER
                str = "Esta numero de dedo ya esta en uso"
            Case ERR_INVALID_FINGERNUM
                str = "Invalid Finger number"
            Case ERR_FAIL_MATCH
                str = "Match Failed"
            Case ERR_NOT_ENROLLED
                str = "Huella no enrolada."
            Case ERR_FAIL_SET
                str = "Error Fail Set"
            Case ERR_FAIL_GET
                str = "Error Fail Get"
            Case ERR_DUPLICATED
                str = "Huella duplicada!"
            Case ERR_CANCELED
                str = "Fp Cancel"
            Case ERR_FAST_RELEASE
                str = "Retiro el dedo muy rapido"
            Case ERR_CREATE_PROC_THREAD
                str = "Huella no enrolada."
            Case ERR_PROCESS_NOT_DONE
                str = "Process has been not done yet."
            Case RES_OK
                str = "Grabado correctamente"
            Case IS_EMPTY_ID
                str = "This ID is Empty."
            Case IS_EMPTY_FINGER
                str = "This Finger Number is Empty."
            Case GD_ENROLL_SUCCESS
                str = "Enroll Success"
            Case GD_IDENTIFY_SUCCESS
                str = "Identificacion exitosa"
            Case GD_NEED_PRESS_FINGER
                str = "Presiona el dedo"
            Case GD_NEED_RELEASE_FINGER
                str = "Levanta el dedo"
        End Select

        GetErrMsg = str
    End Function

    Private Sub SFEPCtrl_OnSFEPEvent(ByVal sender As Object, ByVal e As AxACTIVEXSPLO15Lib._DActiveXSPLO15Events_OnSFEPEventEvent) Handles SFEPCtrl.OnSFEPEvent
        Dim m_dwID, m_bFingerNum

        m_dwID = CLng(txtID.Text)
        m_bFingerNum = CByte(txtFN.Text)

        If e.p_Param1 = GD_PROCESS_DONE Then
            ControlStatus(True)
            cmdReadTemplate_Click(Nothing, Nothing)
            If (lblStatus.Text <> "Huella no enrolada." AndAlso lblStatus.Text <> "Este numero de dedo ya esta en uso" AndAlso lblStatus.Text <> "Identificacion exitosa") Then
                If GrabandoHuella Then
                    If (SaveFinger()) Then
                        ConsultarHuellas()
                        ConsultarReplicasPendientes_x_equipo()
                        Return
                    End If
                End If
            End If
            Exit Sub
        End If

        Select Case e.p_nEventID
            Case CMD_ENROLL_CODE
                lblStatus.Text = GetErrMsg(e.p_Param1)
            Case CMD_IDENTIFY_CODE
                If e.p_Param1 <> RES_OK Then
                    lblStatus.Text = GetErrMsg(e.p_Param1)
                    Exit Sub
                End If

                Select Case g_nIdentifyMode
                    Case VERIFY_MODE
                        If (m_dwID = e.p_Param2) And (m_bFingerNum = e.p_Param3) Then
                            lblStatus.Text = GetErrMsg(GD_IDENTIFY_SUCCESS)
                        Else
                            lblStatus.Text = GetErrMsg(ERR_FAIL_MATCH)
                        End If
                    Case VERIFYID_MODE
                        If m_dwID = e.p_Param2 Then
                            txtFN.Text = Str(e.p_Param3)
                            chkManager.Checked = e.p_Param4
                            lblStatus.Text = GetErrMsg(GD_IDENTIFY_SUCCESS)
                        Else
                            lblStatus.Text = GetErrMsg(ERR_FAIL_MATCH)
                        End If
                    Case IDENTIFY_MODE
                        txtID.Text = Str(e.p_Param2)
                        txtFN.Text = Str(e.p_Param3)
                        chkManager.Checked = e.p_Param4
                        lblStatus.Text = GetErrMsg(GD_IDENTIFY_SUCCESS)
                End Select
            Case CMD_IDENTIFY_FREE_CODE
                If e.p_Param1 = RES_OK Then
                    txtID.Text = Str(e.p_Param2)
                    txtFN.Text = Str(e.p_Param3)
                    chkManager.Checked = e.p_Param4
                End If
                lblStatus.Text = GetErrMsg(e.p_Param1)
        End Select
    End Sub

    Function SaveFinger()
        Dim Path As String = Application.StartupPath + "\\Fingers\\finger.fpt"

        If File.Exists(Path) Then
            Dim HuellaConTamañoCompleto(498) As Byte
            Dim template() As Byte = File.ReadAllBytes(Path)
            File.WriteAllBytes(Path, template)

            Dim numerodedo As Integer = Convert.ToInt32(txtFN.Text)

            Array.Copy(template, HuellaConTamañoCompleto, 498)

            Dim numero_huella_final As Integer = objAccesoDatos.vericarExistenciaHuella(persona, conex)

            HuellaBytes = HuellaConTamañoCompleto

            Dim huellastring As String = Convert.ToBase64String(HuellaBytes)
            ptrImagenHuella.ImageLocation = Application.StartupPath + "\\huellapng.png"
            objAccesoDatos.guardarHuella(gymID, SucursalID, persona, HuellaBytes, conex, Convert.ToInt32(txtID.Text))

            If Contratos = True Then
                Dim url As String = System.Configuration.ConfigurationSettings.AppSettings("urlApi").ToString()
                Dim url_Final As String = url & "api/Fingerprint/SaveSignedContract"

                Dim postData As New Dictionary(Of String, Object)
                postData.Add("gymId", gymID)
                postData.Add("branchId", SucursalID)
                postData.Add("userId", persona)
                postData.Add("fingerprintImage", HuellaBytes)

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
            End If
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub SFEPCtrl_SFEPDoEvent_1(ByVal sender As System.Object, ByVal e As AxACTIVEXSPLO15Lib._DActiveXSPLO15Events_SFEPDoEventEvent) Handles SFEPCtrl.SFEPDoEvent
        Application.DoEvents()
    End Sub

    Private Sub cmdWriteTemplate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdWriteTemplate.Click
        Dim nRet As Integer
        Dim dwID As Integer, bFN As Byte, bManager As Byte

        dwID = CLng(txtID.Text)
        bFN = CByte(txtFN.Text)
        bManager = chkManager.Checked

        nRet = SFEPCtrl.SFEPCheckFingerNum(dwID, bFN)

        If nRet = ERR_INVALID_ID Then
            lblStatus.Text = "Invalid ID."
            Exit Sub
        End If

        If nRet = ERR_INVALID_FINGERNUM Then
            lblStatus.Text = "Invalid Finger Number"
            Exit Sub
        End If

        If dlgOpen.ShowDialog() = Windows.Forms.DialogResult.OK Then
            nRet = SFEPCtrl.SFEPWriteTemplate(dwID, bFN, bManager, dlgOpen.FileName)
            lblStatus.Text = GetErrMsg(nRet)
        End If
    End Sub

    Private Sub btnGrabar3_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        GrabandoHuella = True
        If (Not ptrImagenHuella.ImageLocation = Nothing) Then
            Dim dr As DialogResult
            dr = MessageBox.Show("¿Desea Eliminar esta huella?", "Alerta ", MessageBoxButtons.YesNo)
            If (dr = DialogResult.Yes) Then
                Opcion = 2
                txtFN.Text = 1
                Dim borrarID = Borrar_ID_del_Huellero()
                If borrarID <> "No" Then
                    txtID.Text = borrarID
                    txtFN.Text = Convert.ToInt32(txtFN.Text)
                    cmdRemove_Click(Nothing, Nothing)
                    ConsultarHuellas()
                End If
                ConsultarReplicasPendientes_x_equipo()
            End If
        ElseIf (cantidadActualHuellas < 3000) Then
            Opcion = 1
            Dim dt As DataTable = objAccesoDatos.ConsultarUltimoID_Huellas(conex)
            If Not dt Is Nothing Then
                txtID.Text = dt.Rows(0).Item("Ultimo_ID")
                intID_DedoUno = dt.Rows(0).Item("Ultimo_ID")
                txtFN.Text = "1"
                cmdEnroll_Click(Nothing, Nothing)
            End If
        Else
            MessageBox.Show("No es posible realizar la acción por falta de capacidad de almacenamiento de huellas en la terminal.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnSincronizarHuellas_Click(sender As Object, e As EventArgs) Handles btnSincronizarHuellas.Click
        If BorradoMasivoAlSincronizar Then
            BorrarHuellasEquipoMasivamente()
        End If

        Dim dt As DataTable = objAccesoDatos.Consultar_Huellas_Pendientes_Por_Replicar(conex)
        Dim fingersPath As String = Application.StartupPath + "\Fingers"
        If Not System.IO.Directory.Exists(fingersPath) Then
            My.Computer.FileSystem.CreateDirectory(fingersPath)
        Else
            My.Computer.FileSystem.DeleteDirectory(fingersPath, FileIO.DeleteDirectoryOption.DeleteAllContents)
            My.Computer.FileSystem.CreateDirectory(fingersPath)
        End If

        Dim HuellaConTamañoCompleto(497) As Byte
        If Not dt Is Nothing Then
            Dim pathFingerfpt As String
            Dim intdwID, intbFN As Integer
            For i = 0 To dt.Rows.Count - 1
                Dim template() As Byte = dt.Rows(i).Item("binDatoHuella")
                Array.Copy(template, HuellaConTamañoCompleto, 498)
                pathFingerfpt = fingersPath & "\\Fingers\\finger" & dt.Rows(i).Item("intpkCodigo") & ".fpt"
                File.WriteAllBytes(pathFingerfpt, HuellaConTamañoCompleto)
                intdwID = dt.Rows(i).Item("intpkCodigo")
                intbFN = dt.Rows(i).Item("intNumeroDedo")
                nRet = SFEPCtrl.SFEPWriteTemplate(intdwID, intbFN, 1, pathFingerfpt)
                If GetErrMsg(nRet) = "Grabado correctamente" Then
                    objAccesoDatos.CambiarEstado_HuellasCAMA2000(dt.Rows(i).Item("intpkCodigo"), conex)
                End If
            Next
            ConsultarReplicasPendientes_x_equipo()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim fingersPath As String = Application.StartupPath + "\3000 templates CAMA"

        For i = 1 To 3001
            Dim nombre As String = ""

            If i <= 9 Then
                nombre = "000"
            End If

            If i <= 99 And i > 9 Then
                nombre = "00"
            End If

            If i <= 999 And i > 99 Then
                nombre = "0"
            End If

            Dim pathFingerfpt As String = fingersPath & "\" & nombre + i.ToString() & ".fpt"

            nRet = SFEPCtrl.SFEPWriteTemplate(i, 1, 1, pathFingerfpt)
            If GetErrMsg(nRet) = "Grabado correctamente" Then

            Else
                MessageBox.Show("Error en la huella: " & fingersPath & "\" & nombre + i.ToString() & ".fpt", "Alerta ", MessageBoxButtons.YesNo)
            End If

            If i = 3001 Then
                MsgBox("terminado")
            End If
        Next
    End Sub

    'Public Class JsonResponse
    '    <JsonProperty("userId")>
    '    Public Property UserId As String

    '    <JsonProperty("userEmail")>
    '    Public Property UserEmail As String

    '    <JsonProperty("userName")>
    '    Public Property UserName As String

    '    <JsonProperty("contractId")>
    '    Public Property ContractId As Integer

    '    <JsonProperty("SignDate")>
    '    Public Property SignDate As DateTime

    '    <JsonProperty("invoiceId")>
    '    Public Property InvoiceId As Integer

    '    <JsonProperty("documentType")>
    '    Public Property DocumentType As String

    '    <JsonProperty("contractText")>
    '    Public Property ContractText As String

    '    <JsonProperty("responsibleSignature")>
    '    Public Property ResponsibleSignature As Byte

    '    <JsonProperty("contractType")>
    '    Public Property ContractType As String

    '    <JsonProperty("gymNit")>
    '    Public Property GymNit As String

    '    <JsonProperty("gymName")>
    '    Public Property GymName As String

    '    <JsonProperty("gymAddress")>
    '    Public Property GymAddress As String

    '    <JsonProperty("gymPhone")>
    '    Public Property GymPhone As String

    '    <JsonProperty("gymLogo")>
    '    Public Property GymLogo As Byte

    '    <JsonProperty("userFingerprint")>
    '    Public Property UserFingerprint As Byte

    '    <JsonProperty("branchName")>
    '    Public Property BranchName As String
    'End Class
End Class