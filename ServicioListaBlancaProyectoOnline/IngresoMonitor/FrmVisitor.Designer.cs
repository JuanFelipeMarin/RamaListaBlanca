namespace Sibo.WhiteList.IngresoMonitor
{
    partial class FrmVisitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmVisitor));
            this.gbPersonalData = new System.Windows.Forms.GroupBox();
            this.lblEntryFingerprint = new System.Windows.Forms.Label();
            this.chkEntryFingerprint = new System.Windows.Forms.CheckBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtSecondLastName = new System.Windows.Forms.TextBox();
            this.txtFirstLastName = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.dtpBornDate = new System.Windows.Forms.DateTimePicker();
            this.cmbGenre = new System.Windows.Forms.ComboBox();
            this.cmbEPS = new System.Windows.Forms.ComboBox();
            this.cmbIdType = new System.Windows.Forms.ComboBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblGenre = new System.Windows.Forms.Label();
            this.lblEPS = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.lblBornDate = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblSecondLastName = new System.Windows.Forms.Label();
            this.lblFirstLastName = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblIdType = new System.Windows.Forms.Label();
            this.gbVisitData = new System.Windows.Forms.GroupBox();
            this.txtElements = new System.Windows.Forms.TextBox();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.numVisitTime = new System.Windows.Forms.NumericUpDown();
            this.cmbWhoAuthorized = new System.Windows.Forms.ComboBox();
            this.cmbVisitedPerson = new System.Windows.Forms.ComboBox();
            this.lblElements = new System.Windows.Forms.Label();
            this.lblWhoAuthorized = new System.Windows.Forms.Label();
            this.lblReason = new System.Windows.Forms.Label();
            this.lblVisitTime = new System.Windows.Forms.Label();
            this.lblVisitedPerson = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.gbPersonalData.SuspendLayout();
            this.gbVisitData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVisitTime)).BeginInit();
            this.SuspendLayout();
            // 
            // gbPersonalData
            // 
            this.gbPersonalData.Controls.Add(this.lblEntryFingerprint);
            this.gbPersonalData.Controls.Add(this.chkEntryFingerprint);
            this.gbPersonalData.Controls.Add(this.txtEmail);
            this.gbPersonalData.Controls.Add(this.txtAddress);
            this.gbPersonalData.Controls.Add(this.txtPhone);
            this.gbPersonalData.Controls.Add(this.txtSecondLastName);
            this.gbPersonalData.Controls.Add(this.txtFirstLastName);
            this.gbPersonalData.Controls.Add(this.txtName);
            this.gbPersonalData.Controls.Add(this.txtId);
            this.gbPersonalData.Controls.Add(this.dtpBornDate);
            this.gbPersonalData.Controls.Add(this.cmbGenre);
            this.gbPersonalData.Controls.Add(this.cmbEPS);
            this.gbPersonalData.Controls.Add(this.cmbIdType);
            this.gbPersonalData.Controls.Add(this.lblEmail);
            this.gbPersonalData.Controls.Add(this.lblAddress);
            this.gbPersonalData.Controls.Add(this.lblGenre);
            this.gbPersonalData.Controls.Add(this.lblEPS);
            this.gbPersonalData.Controls.Add(this.lblId);
            this.gbPersonalData.Controls.Add(this.lblBornDate);
            this.gbPersonalData.Controls.Add(this.lblPhone);
            this.gbPersonalData.Controls.Add(this.lblSecondLastName);
            this.gbPersonalData.Controls.Add(this.lblFirstLastName);
            this.gbPersonalData.Controls.Add(this.lblName);
            this.gbPersonalData.Controls.Add(this.lblIdType);
            this.gbPersonalData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPersonalData.Location = new System.Drawing.Point(9, 12);
            this.gbPersonalData.Name = "gbPersonalData";
            this.gbPersonalData.Size = new System.Drawing.Size(782, 213);
            this.gbPersonalData.TabIndex = 0;
            this.gbPersonalData.TabStop = false;
            this.gbPersonalData.Text = "Datos personales";
            // 
            // lblEntryFingerprint
            // 
            this.lblEntryFingerprint.AutoSize = true;
            this.lblEntryFingerprint.ForeColor = System.Drawing.Color.Black;
            this.lblEntryFingerprint.Location = new System.Drawing.Point(409, 178);
            this.lblEntryFingerprint.Name = "lblEntryFingerprint";
            this.lblEntryFingerprint.Size = new System.Drawing.Size(116, 13);
            this.lblEntryFingerprint.TabIndex = 24;
            this.lblEntryFingerprint.Text = "Ingreso con huella:";
            // 
            // chkEntryFingerprint
            // 
            this.chkEntryFingerprint.AutoSize = true;
            this.chkEntryFingerprint.ForeColor = System.Drawing.Color.Black;
            this.chkEntryFingerprint.Location = new System.Drawing.Point(531, 177);
            this.chkEntryFingerprint.Name = "chkEntryFingerprint";
            this.chkEntryFingerprint.Size = new System.Drawing.Size(15, 14);
            this.chkEntryFingerprint.TabIndex = 23;
            this.chkEntryFingerprint.UseVisualStyleBackColor = true;
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(531, 141);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(242, 20);
            this.txtEmail.TabIndex = 22;
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Location = new System.Drawing.Point(531, 115);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(242, 20);
            this.txtAddress.TabIndex = 21;
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhone.Location = new System.Drawing.Point(531, 57);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(242, 20);
            this.txtPhone.TabIndex = 20;
            // 
            // txtSecondLastName
            // 
            this.txtSecondLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSecondLastName.Location = new System.Drawing.Point(148, 145);
            this.txtSecondLastName.Name = "txtSecondLastName";
            this.txtSecondLastName.Size = new System.Drawing.Size(242, 20);
            this.txtSecondLastName.TabIndex = 19;
            // 
            // txtFirstLastName
            // 
            this.txtFirstLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstLastName.Location = new System.Drawing.Point(148, 115);
            this.txtFirstLastName.Name = "txtFirstLastName";
            this.txtFirstLastName.Size = new System.Drawing.Size(242, 20);
            this.txtFirstLastName.TabIndex = 18;
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(148, 86);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(242, 20);
            this.txtName.TabIndex = 17;
            // 
            // txtId
            // 
            this.txtId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtId.Location = new System.Drawing.Point(148, 26);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(242, 20);
            this.txtId.TabIndex = 16;
            this.txtId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtId_KeyPress);
            // 
            // dtpBornDate
            // 
            this.dtpBornDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpBornDate.Location = new System.Drawing.Point(148, 172);
            this.dtpBornDate.Name = "dtpBornDate";
            this.dtpBornDate.Size = new System.Drawing.Size(242, 20);
            this.dtpBornDate.TabIndex = 15;
            // 
            // cmbGenre
            // 
            this.cmbGenre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGenre.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbGenre.FormattingEnabled = true;
            this.cmbGenre.Location = new System.Drawing.Point(531, 29);
            this.cmbGenre.Name = "cmbGenre";
            this.cmbGenre.Size = new System.Drawing.Size(242, 21);
            this.cmbGenre.TabIndex = 14;
            // 
            // cmbEPS
            // 
            this.cmbEPS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEPS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEPS.FormattingEnabled = true;
            this.cmbEPS.Location = new System.Drawing.Point(531, 86);
            this.cmbEPS.Name = "cmbEPS";
            this.cmbEPS.Size = new System.Drawing.Size(242, 21);
            this.cmbEPS.TabIndex = 13;
            // 
            // cmbIdType
            // 
            this.cmbIdType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbIdType.FormattingEnabled = true;
            this.cmbIdType.Location = new System.Drawing.Point(148, 54);
            this.cmbIdType.Name = "cmbIdType";
            this.cmbIdType.Size = new System.Drawing.Size(242, 21);
            this.cmbIdType.TabIndex = 12;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.ForeColor = System.Drawing.Color.Black;
            this.lblEmail.Location = new System.Drawing.Point(409, 148);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(41, 13);
            this.lblEmail.TabIndex = 10;
            this.lblEmail.Text = "Email:";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.ForeColor = System.Drawing.Color.Black;
            this.lblAddress.Location = new System.Drawing.Point(409, 118);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(65, 13);
            this.lblAddress.TabIndex = 9;
            this.lblAddress.Text = "Dirección:";
            // 
            // lblGenre
            // 
            this.lblGenre.AutoSize = true;
            this.lblGenre.ForeColor = System.Drawing.Color.Black;
            this.lblGenre.Location = new System.Drawing.Point(409, 32);
            this.lblGenre.Name = "lblGenre";
            this.lblGenre.Size = new System.Drawing.Size(52, 13);
            this.lblGenre.TabIndex = 8;
            this.lblGenre.Text = "Género:";
            // 
            // lblEPS
            // 
            this.lblEPS.AutoSize = true;
            this.lblEPS.ForeColor = System.Drawing.Color.Black;
            this.lblEPS.Location = new System.Drawing.Point(409, 89);
            this.lblEPS.Name = "lblEPS";
            this.lblEPS.Size = new System.Drawing.Size(35, 13);
            this.lblEPS.TabIndex = 7;
            this.lblEPS.Text = "EPS:";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.ForeColor = System.Drawing.Color.Black;
            this.lblId.Location = new System.Drawing.Point(8, 29);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(88, 13);
            this.lblId.TabIndex = 6;
            this.lblId.Text = "Identificación:";
            // 
            // lblBornDate
            // 
            this.lblBornDate.AutoSize = true;
            this.lblBornDate.ForeColor = System.Drawing.Color.Black;
            this.lblBornDate.Location = new System.Drawing.Point(8, 178);
            this.lblBornDate.Name = "lblBornDate";
            this.lblBornDate.Size = new System.Drawing.Size(129, 13);
            this.lblBornDate.TabIndex = 5;
            this.lblBornDate.Text = "Fecha de nacimiento:";
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.ForeColor = System.Drawing.Color.Black;
            this.lblPhone.Location = new System.Drawing.Point(409, 60);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(61, 13);
            this.lblPhone.TabIndex = 4;
            this.lblPhone.Text = "Teléfono:";
            // 
            // lblSecondLastName
            // 
            this.lblSecondLastName.AutoSize = true;
            this.lblSecondLastName.ForeColor = System.Drawing.Color.Black;
            this.lblSecondLastName.Location = new System.Drawing.Point(8, 148);
            this.lblSecondLastName.Name = "lblSecondLastName";
            this.lblSecondLastName.Size = new System.Drawing.Size(109, 13);
            this.lblSecondLastName.TabIndex = 3;
            this.lblSecondLastName.Text = "Segundo apellido:";
            // 
            // lblFirstLastName
            // 
            this.lblFirstLastName.AutoSize = true;
            this.lblFirstLastName.ForeColor = System.Drawing.Color.Black;
            this.lblFirstLastName.Location = new System.Drawing.Point(8, 118);
            this.lblFirstLastName.Name = "lblFirstLastName";
            this.lblFirstLastName.Size = new System.Drawing.Size(94, 13);
            this.lblFirstLastName.TabIndex = 2;
            this.lblFirstLastName.Text = "Primer apellido:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.ForeColor = System.Drawing.Color.Black;
            this.lblName.Location = new System.Drawing.Point(8, 89);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(60, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Nombres:";
            // 
            // lblIdType
            // 
            this.lblIdType.AutoSize = true;
            this.lblIdType.ForeColor = System.Drawing.Color.Black;
            this.lblIdType.Location = new System.Drawing.Point(8, 57);
            this.lblIdType.Name = "lblIdType";
            this.lblIdType.Size = new System.Drawing.Size(134, 13);
            this.lblIdType.TabIndex = 0;
            this.lblIdType.Text = "Tipo de identificación:";
            // 
            // gbVisitData
            // 
            this.gbVisitData.Controls.Add(this.txtElements);
            this.gbVisitData.Controls.Add(this.txtReason);
            this.gbVisitData.Controls.Add(this.numVisitTime);
            this.gbVisitData.Controls.Add(this.cmbWhoAuthorized);
            this.gbVisitData.Controls.Add(this.cmbVisitedPerson);
            this.gbVisitData.Controls.Add(this.lblElements);
            this.gbVisitData.Controls.Add(this.lblWhoAuthorized);
            this.gbVisitData.Controls.Add(this.lblReason);
            this.gbVisitData.Controls.Add(this.lblVisitTime);
            this.gbVisitData.Controls.Add(this.lblVisitedPerson);
            this.gbVisitData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbVisitData.Location = new System.Drawing.Point(9, 231);
            this.gbVisitData.Name = "gbVisitData";
            this.gbVisitData.Size = new System.Drawing.Size(782, 150);
            this.gbVisitData.TabIndex = 1;
            this.gbVisitData.TabStop = false;
            this.gbVisitData.Text = "Datos de la visita";
            // 
            // txtElements
            // 
            this.txtElements.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtElements.Location = new System.Drawing.Point(319, 79);
            this.txtElements.Multiline = true;
            this.txtElements.Name = "txtElements";
            this.txtElements.Size = new System.Drawing.Size(301, 65);
            this.txtElements.TabIndex = 24;
            // 
            // txtReason
            // 
            this.txtReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReason.Location = new System.Drawing.Point(9, 79);
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(300, 65);
            this.txtReason.TabIndex = 23;
            // 
            // numVisitTime
            // 
            this.numVisitTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numVisitTime.Location = new System.Drawing.Point(665, 34);
            this.numVisitTime.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numVisitTime.Name = "numVisitTime";
            this.numVisitTime.Size = new System.Drawing.Size(64, 20);
            this.numVisitTime.TabIndex = 16;
            this.numVisitTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmbWhoAuthorized
            // 
            this.cmbWhoAuthorized.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhoAuthorized.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWhoAuthorized.FormattingEnabled = true;
            this.cmbWhoAuthorized.Location = new System.Drawing.Point(319, 33);
            this.cmbWhoAuthorized.Name = "cmbWhoAuthorized";
            this.cmbWhoAuthorized.Size = new System.Drawing.Size(300, 21);
            this.cmbWhoAuthorized.TabIndex = 15;
            // 
            // cmbVisitedPerson
            // 
            this.cmbVisitedPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVisitedPerson.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbVisitedPerson.FormattingEnabled = true;
            this.cmbVisitedPerson.Location = new System.Drawing.Point(9, 32);
            this.cmbVisitedPerson.Name = "cmbVisitedPerson";
            this.cmbVisitedPerson.Size = new System.Drawing.Size(300, 21);
            this.cmbVisitedPerson.TabIndex = 14;
            // 
            // lblElements
            // 
            this.lblElements.AutoSize = true;
            this.lblElements.ForeColor = System.Drawing.Color.Black;
            this.lblElements.Location = new System.Drawing.Point(316, 63);
            this.lblElements.Name = "lblElements";
            this.lblElements.Size = new System.Drawing.Size(180, 13);
            this.lblElements.TabIndex = 10;
            this.lblElements.Text = "Elementos con los que ingresa";
            // 
            // lblWhoAuthorized
            // 
            this.lblWhoAuthorized.AutoSize = true;
            this.lblWhoAuthorized.ForeColor = System.Drawing.Color.Black;
            this.lblWhoAuthorized.Location = new System.Drawing.Point(316, 16);
            this.lblWhoAuthorized.Name = "lblWhoAuthorized";
            this.lblWhoAuthorized.Size = new System.Drawing.Size(96, 13);
            this.lblWhoAuthorized.TabIndex = 9;
            this.lblWhoAuthorized.Text = "Quien autoriza?";
            // 
            // lblReason
            // 
            this.lblReason.AutoSize = true;
            this.lblReason.ForeColor = System.Drawing.Color.Black;
            this.lblReason.Location = new System.Drawing.Point(8, 63);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(115, 13);
            this.lblReason.TabIndex = 8;
            this.lblReason.Text = "Motivo de la visita:";
            // 
            // lblVisitTime
            // 
            this.lblVisitTime.AutoSize = true;
            this.lblVisitTime.ForeColor = System.Drawing.Color.Black;
            this.lblVisitTime.Location = new System.Drawing.Point(632, 16);
            this.lblVisitTime.Name = "lblVisitTime";
            this.lblVisitTime.Size = new System.Drawing.Size(135, 13);
            this.lblVisitTime.TabIndex = 7;
            this.lblVisitTime.Text = "Tiempo de visita (min.)";
            // 
            // lblVisitedPerson
            // 
            this.lblVisitedPerson.AutoSize = true;
            this.lblVisitedPerson.ForeColor = System.Drawing.Color.Black;
            this.lblVisitedPerson.Location = new System.Drawing.Point(6, 16);
            this.lblVisitedPerson.Name = "lblVisitedPerson";
            this.lblVisitedPerson.Size = new System.Drawing.Size(105, 13);
            this.lblVisitedPerson.TabIndex = 6;
            this.lblVisitedPerson.Text = "Persona visitada:";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BackgroundImage = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Botonlimpio_O;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(421, 387);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(131, 47);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImage = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Botonlimpio_O;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(267, 387);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(131, 47);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Guardar";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmVisitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(803, 454);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gbVisitData);
            this.Controls.Add(this.gbPersonalData);
            this.Font = new System.Drawing.Font("Lucida Sans", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmVisitor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Visitantes";
            this.Load += new System.EventHandler(this.FrmVisitor_Load);
            this.gbPersonalData.ResumeLayout(false);
            this.gbPersonalData.PerformLayout();
            this.gbVisitData.ResumeLayout(false);
            this.gbVisitData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVisitTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPersonalData;
        private System.Windows.Forms.GroupBox gbVisitData;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblGenre;
        private System.Windows.Forms.Label lblEPS;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblBornDate;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.Label lblSecondLastName;
        private System.Windows.Forms.Label lblFirstLastName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblIdType;
        private System.Windows.Forms.Label lblEntryFingerprint;
        private System.Windows.Forms.CheckBox chkEntryFingerprint;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtSecondLastName;
        private System.Windows.Forms.TextBox txtFirstLastName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.DateTimePicker dtpBornDate;
        private System.Windows.Forms.ComboBox cmbGenre;
        private System.Windows.Forms.ComboBox cmbEPS;
        private System.Windows.Forms.ComboBox cmbIdType;
        private System.Windows.Forms.TextBox txtElements;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.NumericUpDown numVisitTime;
        private System.Windows.Forms.ComboBox cmbWhoAuthorized;
        private System.Windows.Forms.ComboBox cmbVisitedPerson;
        private System.Windows.Forms.Label lblElements;
        private System.Windows.Forms.Label lblWhoAuthorized;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.Label lblVisitTime;
        private System.Windows.Forms.Label lblVisitedPerson;
    }
}