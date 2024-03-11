<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtID = New System.Windows.Forms.TextBox()
        Me.txtFN = New System.Windows.Forms.TextBox()
        Me.chkManager = New System.Windows.Forms.CheckBox()
        Me.cmdEnroll = New System.Windows.Forms.Button()
        Me.cmdVerify = New System.Windows.Forms.Button()
        Me.cmdVerifyID = New System.Windows.Forms.Button()
        Me.cmdIdentify = New System.Windows.Forms.Button()
        Me.cmdIdentify2 = New System.Windows.Forms.Button()
        Me.cmdFpCancel = New System.Windows.Forms.Button()
        Me.cmdSearchFinger = New System.Windows.Forms.Button()
        Me.cmdSearchID = New System.Windows.Forms.Button()
        Me.cmdCheckFinger = New System.Windows.Forms.Button()
        Me.cmdCheckID = New System.Windows.Forms.Button()
        Me.cmdReadTemplate = New System.Windows.Forms.Button()
        Me.cmdRemoveAll = New System.Windows.Forms.Button()
        Me.cmdRemove = New System.Windows.Forms.Button()
        Me.cmdEnrollCount = New System.Windows.Forms.Button()
        Me.cmdClose = New System.Windows.Forms.Button()
        Me.dlgSave = New System.Windows.Forms.SaveFileDialog()
        Me.cmdWriteTemplate = New System.Windows.Forms.Button()
        Me.dlgOpen = New System.Windows.Forms.OpenFileDialog()
        Me.btnGrabar = New System.Windows.Forms.Button()
        Me.ptrImagenHuella = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnSincronizarHuellas = New System.Windows.Forms.Button()
        Me.lblMensajeSincronizacion = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.SFEPCtrl = New AxACTIVEXSPLO15Lib.AxActiveXSPLO15()
        CType(Me.ptrImagenHuella, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SFEPCtrl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblStatus
        '
        Me.lblStatus.BackColor = System.Drawing.Color.White
        Me.lblStatus.Font = New System.Drawing.Font("Arial", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatus.ForeColor = System.Drawing.Color.Red
        Me.lblStatus.Location = New System.Drawing.Point(130, 45)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(459, 61)
        Me.lblStatus.TabIndex = 10
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(131, 457)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 13)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "UserID"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(256, 457)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 13)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "FingerNumber"
        '
        'txtID
        '
        Me.txtID.Location = New System.Drawing.Point(177, 454)
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(60, 20)
        Me.txtID.TabIndex = 13
        '
        'txtFN
        '
        Me.txtFN.Location = New System.Drawing.Point(335, 454)
        Me.txtFN.Name = "txtFN"
        Me.txtFN.Size = New System.Drawing.Size(58, 20)
        Me.txtFN.TabIndex = 14
        '
        'chkManager
        '
        Me.chkManager.AutoSize = True
        Me.chkManager.Location = New System.Drawing.Point(426, 459)
        Me.chkManager.Name = "chkManager"
        Me.chkManager.Size = New System.Drawing.Size(68, 17)
        Me.chkManager.TabIndex = 15
        Me.chkManager.Text = "Manager"
        Me.chkManager.UseVisualStyleBackColor = True
        '
        'cmdEnroll
        '
        Me.cmdEnroll.Location = New System.Drawing.Point(21, 495)
        Me.cmdEnroll.Name = "cmdEnroll"
        Me.cmdEnroll.Size = New System.Drawing.Size(114, 27)
        Me.cmdEnroll.TabIndex = 16
        Me.cmdEnroll.Text = "Enroll"
        Me.cmdEnroll.UseVisualStyleBackColor = True
        '
        'cmdVerify
        '
        Me.cmdVerify.Location = New System.Drawing.Point(134, 495)
        Me.cmdVerify.Name = "cmdVerify"
        Me.cmdVerify.Size = New System.Drawing.Size(114, 27)
        Me.cmdVerify.TabIndex = 17
        Me.cmdVerify.Text = "Verify"
        Me.cmdVerify.UseVisualStyleBackColor = True
        '
        'cmdVerifyID
        '
        Me.cmdVerifyID.Location = New System.Drawing.Point(247, 495)
        Me.cmdVerifyID.Name = "cmdVerifyID"
        Me.cmdVerifyID.Size = New System.Drawing.Size(114, 27)
        Me.cmdVerifyID.TabIndex = 18
        Me.cmdVerifyID.Text = "VerifyID"
        Me.cmdVerifyID.UseVisualStyleBackColor = True
        '
        'cmdIdentify
        '
        Me.cmdIdentify.Location = New System.Drawing.Point(360, 495)
        Me.cmdIdentify.Name = "cmdIdentify"
        Me.cmdIdentify.Size = New System.Drawing.Size(120, 27)
        Me.cmdIdentify.TabIndex = 19
        Me.cmdIdentify.Text = "Identify"
        Me.cmdIdentify.UseVisualStyleBackColor = True
        '
        'cmdIdentify2
        '
        Me.cmdIdentify2.Location = New System.Drawing.Point(480, 495)
        Me.cmdIdentify2.Name = "cmdIdentify2"
        Me.cmdIdentify2.Size = New System.Drawing.Size(111, 27)
        Me.cmdIdentify2.TabIndex = 20
        Me.cmdIdentify2.Text = "IdentifyFree"
        Me.cmdIdentify2.UseVisualStyleBackColor = True
        '
        'cmdFpCancel
        '
        Me.cmdFpCancel.Location = New System.Drawing.Point(480, 528)
        Me.cmdFpCancel.Name = "cmdFpCancel"
        Me.cmdFpCancel.Size = New System.Drawing.Size(111, 27)
        Me.cmdFpCancel.TabIndex = 25
        Me.cmdFpCancel.Text = "FPCancel"
        Me.cmdFpCancel.UseVisualStyleBackColor = True
        '
        'cmdSearchFinger
        '
        Me.cmdSearchFinger.Location = New System.Drawing.Point(360, 528)
        Me.cmdSearchFinger.Name = "cmdSearchFinger"
        Me.cmdSearchFinger.Size = New System.Drawing.Size(120, 27)
        Me.cmdSearchFinger.TabIndex = 24
        Me.cmdSearchFinger.Text = "Search FingerNumber"
        Me.cmdSearchFinger.UseVisualStyleBackColor = True
        '
        'cmdSearchID
        '
        Me.cmdSearchID.Location = New System.Drawing.Point(247, 528)
        Me.cmdSearchID.Name = "cmdSearchID"
        Me.cmdSearchID.Size = New System.Drawing.Size(114, 27)
        Me.cmdSearchID.TabIndex = 23
        Me.cmdSearchID.Text = "Search ID"
        Me.cmdSearchID.UseVisualStyleBackColor = True
        '
        'cmdCheckFinger
        '
        Me.cmdCheckFinger.Location = New System.Drawing.Point(133, 528)
        Me.cmdCheckFinger.Name = "cmdCheckFinger"
        Me.cmdCheckFinger.Size = New System.Drawing.Size(115, 27)
        Me.cmdCheckFinger.TabIndex = 22
        Me.cmdCheckFinger.Text = "Check FingerNumber"
        Me.cmdCheckFinger.UseVisualStyleBackColor = True
        '
        'cmdCheckID
        '
        Me.cmdCheckID.Location = New System.Drawing.Point(20, 528)
        Me.cmdCheckID.Name = "cmdCheckID"
        Me.cmdCheckID.Size = New System.Drawing.Size(114, 27)
        Me.cmdCheckID.TabIndex = 21
        Me.cmdCheckID.Text = "Check ID"
        Me.cmdCheckID.UseVisualStyleBackColor = True
        '
        'cmdReadTemplate
        '
        Me.cmdReadTemplate.Location = New System.Drawing.Point(249, 561)
        Me.cmdReadTemplate.Name = "cmdReadTemplate"
        Me.cmdReadTemplate.Size = New System.Drawing.Size(112, 27)
        Me.cmdReadTemplate.TabIndex = 29
        Me.cmdReadTemplate.Text = "Read Template"
        Me.cmdReadTemplate.UseVisualStyleBackColor = True
        '
        'cmdRemoveAll
        '
        Me.cmdRemoveAll.Location = New System.Drawing.Point(532, 457)
        Me.cmdRemoveAll.Name = "cmdRemoveAll"
        Me.cmdRemoveAll.Size = New System.Drawing.Size(148, 27)
        Me.cmdRemoveAll.TabIndex = 28
        Me.cmdRemoveAll.Text = "Borrar huellas del dispositivo"
        Me.cmdRemoveAll.UseVisualStyleBackColor = True
        '
        'cmdRemove
        '
        Me.cmdRemove.Location = New System.Drawing.Point(134, 561)
        Me.cmdRemove.Name = "cmdRemove"
        Me.cmdRemove.Size = New System.Drawing.Size(114, 27)
        Me.cmdRemove.TabIndex = 27
        Me.cmdRemove.Text = "Remove Template"
        Me.cmdRemove.UseVisualStyleBackColor = True
        '
        'cmdEnrollCount
        '
        Me.cmdEnrollCount.Location = New System.Drawing.Point(21, 561)
        Me.cmdEnrollCount.Name = "cmdEnrollCount"
        Me.cmdEnrollCount.Size = New System.Drawing.Size(114, 27)
        Me.cmdEnrollCount.TabIndex = 26
        Me.cmdEnrollCount.Text = "Get Enroll Count"
        Me.cmdEnrollCount.UseVisualStyleBackColor = True
        '
        'cmdClose
        '
        Me.cmdClose.Location = New System.Drawing.Point(595, 45)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(111, 27)
        Me.cmdClose.TabIndex = 31
        Me.cmdClose.Text = "Cancelar"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'dlgSave
        '
        Me.dlgSave.Filter = "bmp(*.bmp)|*.bmp"
        '
        'cmdWriteTemplate
        '
        Me.cmdWriteTemplate.Location = New System.Drawing.Point(360, 561)
        Me.cmdWriteTemplate.Name = "cmdWriteTemplate"
        Me.cmdWriteTemplate.Size = New System.Drawing.Size(120, 27)
        Me.cmdWriteTemplate.TabIndex = 33
        Me.cmdWriteTemplate.Text = "Write Template"
        Me.cmdWriteTemplate.UseVisualStyleBackColor = True
        '
        'dlgOpen
        '
        Me.dlgOpen.Filter = "fpt(*.fpt)|*.fpt"
        '
        'btnGrabar
        '
        Me.btnGrabar.Location = New System.Drawing.Point(180, 134)
        Me.btnGrabar.Name = "btnGrabar"
        Me.btnGrabar.Size = New System.Drawing.Size(75, 23)
        Me.btnGrabar.TabIndex = 64
        Me.btnGrabar.Tag = "1"
        Me.btnGrabar.Text = "Grabar"
        '
        'ptrImagenHuella
        '
        Me.ptrImagenHuella.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ptrImagenHuella.Location = New System.Drawing.Point(156, 163)
        Me.ptrImagenHuella.Name = "ptrImagenHuella"
        Me.ptrImagenHuella.Size = New System.Drawing.Size(120, 136)
        Me.ptrImagenHuella.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.ptrImagenHuella.TabIndex = 63
        Me.ptrImagenHuella.TabStop = False
        Me.ptrImagenHuella.Tag = "1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 433)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 13)
        Me.Label1.TabIndex = 67
        Me.Label1.Text = "Label1"
        '
        'btnSincronizarHuellas
        '
        Me.btnSincronizarHuellas.Location = New System.Drawing.Point(156, 388)
        Me.btnSincronizarHuellas.Name = "btnSincronizarHuellas"
        Me.btnSincronizarHuellas.Size = New System.Drawing.Size(111, 27)
        Me.btnSincronizarHuellas.TabIndex = 68
        Me.btnSincronizarHuellas.Text = "Sincronizar huellas"
        Me.btnSincronizarHuellas.UseVisualStyleBackColor = True
        '
        'lblMensajeSincronizacion
        '
        Me.lblMensajeSincronizacion.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.lblMensajeSincronizacion.Location = New System.Drawing.Point(55, 328)
        Me.lblMensajeSincronizacion.Name = "lblMensajeSincronizacion"
        Me.lblMensajeSincronizacion.Size = New System.Drawing.Size(323, 49)
        Me.lblMensajeSincronizacion.TabIndex = 69
        Me.lblMensajeSincronizacion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 101)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(81, 25)
        Me.Button1.TabIndex = 70
        Me.Button1.Text = "Prueba"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(-2, 1)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(723, 38)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 34
        Me.PictureBox1.TabStop = False
        '
        'SFEPCtrl
        '
        Me.SFEPCtrl.Enabled = True
        Me.SFEPCtrl.Location = New System.Drawing.Point(419, 138)
        Me.SFEPCtrl.Name = "SFEPCtrl"
        Me.SFEPCtrl.OcxState = CType(resources.GetObject("SFEPCtrl.OcxState"), System.Windows.Forms.AxHost.State)
        Me.SFEPCtrl.Size = New System.Drawing.Size(324, 337)
        Me.SFEPCtrl.TabIndex = 32
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(717, 456)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.lblMensajeSincronizacion)
        Me.Controls.Add(Me.btnSincronizarHuellas)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnGrabar)
        Me.Controls.Add(Me.ptrImagenHuella)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.cmdWriteTemplate)
        Me.Controls.Add(Me.SFEPCtrl)
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.cmdReadTemplate)
        Me.Controls.Add(Me.cmdRemoveAll)
        Me.Controls.Add(Me.cmdRemove)
        Me.Controls.Add(Me.cmdEnrollCount)
        Me.Controls.Add(Me.cmdFpCancel)
        Me.Controls.Add(Me.cmdSearchFinger)
        Me.Controls.Add(Me.cmdSearchID)
        Me.Controls.Add(Me.cmdCheckFinger)
        Me.Controls.Add(Me.cmdCheckID)
        Me.Controls.Add(Me.cmdIdentify2)
        Me.Controls.Add(Me.cmdIdentify)
        Me.Controls.Add(Me.cmdVerifyID)
        Me.Controls.Add(Me.cmdVerify)
        Me.Controls.Add(Me.cmdEnroll)
        Me.Controls.Add(Me.chkManager)
        Me.Controls.Add(Me.txtFN)
        Me.Controls.Add(Me.txtID)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblStatus)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Grabar huella CAMA2000"
        CType(Me.ptrImagenHuella, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SFEPCtrl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtID As System.Windows.Forms.TextBox
    Friend WithEvents txtFN As System.Windows.Forms.TextBox
    Friend WithEvents chkManager As System.Windows.Forms.CheckBox
    Friend WithEvents cmdEnroll As System.Windows.Forms.Button
    Friend WithEvents cmdVerify As System.Windows.Forms.Button
    Friend WithEvents cmdVerifyID As System.Windows.Forms.Button
    Friend WithEvents cmdIdentify As System.Windows.Forms.Button
    Friend WithEvents cmdIdentify2 As System.Windows.Forms.Button
    Friend WithEvents cmdFpCancel As System.Windows.Forms.Button
    Friend WithEvents cmdSearchFinger As System.Windows.Forms.Button
    Friend WithEvents cmdSearchID As System.Windows.Forms.Button
    Friend WithEvents cmdCheckFinger As System.Windows.Forms.Button
    Friend WithEvents cmdCheckID As System.Windows.Forms.Button
    Friend WithEvents cmdReadTemplate As System.Windows.Forms.Button
    Friend WithEvents cmdRemoveAll As System.Windows.Forms.Button
    Friend WithEvents cmdRemove As System.Windows.Forms.Button
    Friend WithEvents cmdEnrollCount As System.Windows.Forms.Button
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents dlgSave As System.Windows.Forms.SaveFileDialog
    Friend WithEvents SFEPCtrl As AxACTIVEXSPLO15Lib.AxActiveXSPLO15
    Friend WithEvents cmdWriteTemplate As System.Windows.Forms.Button
    Friend WithEvents dlgOpen As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnGrabar As Button
    Friend WithEvents ptrImagenHuella As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnSincronizarHuellas As Button
    Friend WithEvents lblMensajeSincronizacion As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents PictureBox1 As PictureBox
End Class
