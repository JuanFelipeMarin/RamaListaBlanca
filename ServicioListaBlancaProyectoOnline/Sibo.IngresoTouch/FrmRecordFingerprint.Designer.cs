﻿namespace Sibo.WhiteList.IngresoTouch
{
    partial class FrmRecordFingerprint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRecordFingerprint));
            this.picPhoto = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblData = new System.Windows.Forms.Label();
            this.lblQualityFingerprintValue = new System.Windows.Forms.Label();
            this.lblAcceptableFingerprintQualityValue = new System.Windows.Forms.Label();
            this.lblQualityFingerprint = new System.Windows.Forms.Label();
            this.lblAcceptableFingerprintQuality = new System.Windows.Forms.Label();
            this.lblClientName = new System.Windows.Forms.Label();
            this.ctrlTecladoHuella = new Sibo.WhiteList.IngresoTouch.ctrlKeyboard();
            this.txtDocument = new System.Windows.Forms.TextBox();
            this.lblDocumentTitle = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // picPhoto
            // 
            this.picPhoto.Location = new System.Drawing.Point(443, 27);
            this.picPhoto.Name = "picPhoto";
            this.picPhoto.Size = new System.Drawing.Size(268, 292);
            this.picPhoto.TabIndex = 31;
            this.picPhoto.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = global::Sibo.WhiteList.IngresoTouch.Properties.Resources.Botonlimpio_O;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.Transparent;
            this.btnClose.Location = new System.Drawing.Point(251, 283);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(151, 36);
            this.btnClose.TabIndex = 30;
            this.btnClose.Text = "Cerrar";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblData
            // 
            this.lblData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblData.Location = new System.Drawing.Point(256, 144);
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(163, 77);
            this.lblData.TabIndex = 29;
            this.lblData.Text = "Recuerde:\r\n- Calidad de huella:\r\n   - 100 - Alta calidad\r\n   - 1 - Muy defectuosa" +
    "";
            // 
            // lblQualityFingerprintValue
            // 
            this.lblQualityFingerprintValue.AutoSize = true;
            this.lblQualityFingerprintValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQualityFingerprintValue.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblQualityFingerprintValue.Location = new System.Drawing.Point(413, 113);
            this.lblQualityFingerprintValue.Name = "lblQualityFingerprintValue";
            this.lblQualityFingerprintValue.Size = new System.Drawing.Size(16, 16);
            this.lblQualityFingerprintValue.TabIndex = 28;
            this.lblQualityFingerprintValue.Text = "0";
            // 
            // lblAcceptableFingerprintQualityValue
            // 
            this.lblAcceptableFingerprintQualityValue.AutoSize = true;
            this.lblAcceptableFingerprintQualityValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcceptableFingerprintQualityValue.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblAcceptableFingerprintQualityValue.Location = new System.Drawing.Point(413, 78);
            this.lblAcceptableFingerprintQualityValue.Name = "lblAcceptableFingerprintQualityValue";
            this.lblAcceptableFingerprintQualityValue.Size = new System.Drawing.Size(16, 16);
            this.lblAcceptableFingerprintQualityValue.TabIndex = 27;
            this.lblAcceptableFingerprintQualityValue.Text = "0";
            // 
            // lblQualityFingerprint
            // 
            this.lblQualityFingerprint.AutoSize = true;
            this.lblQualityFingerprint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQualityFingerprint.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblQualityFingerprint.Location = new System.Drawing.Point(256, 113);
            this.lblQualityFingerprint.Name = "lblQualityFingerprint";
            this.lblQualityFingerprint.Size = new System.Drawing.Size(151, 16);
            this.lblQualityFingerprint.TabIndex = 26;
            this.lblQualityFingerprint.Text = "Calidad de la huella:";
            // 
            // lblAcceptableFingerprintQuality
            // 
            this.lblAcceptableFingerprintQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcceptableFingerprintQuality.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblAcceptableFingerprintQuality.Location = new System.Drawing.Point(256, 68);
            this.lblAcceptableFingerprintQuality.Name = "lblAcceptableFingerprintQuality";
            this.lblAcceptableFingerprintQuality.Size = new System.Drawing.Size(128, 38);
            this.lblAcceptableFingerprintQuality.TabIndex = 25;
            this.lblAcceptableFingerprintQuality.Text = "Calidad de huella aceptable:";
            // 
            // lblClientName
            // 
            this.lblClientName.AutoSize = true;
            this.lblClientName.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientName.ForeColor = System.Drawing.Color.Red;
            this.lblClientName.Location = new System.Drawing.Point(142, 26);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(89, 12);
            this.lblClientName.TabIndex = 24;
            this.lblClientName.Text = "lblClientName";
            // 
            // ctrlTecladoHuella
            // 
            this.ctrlTecladoHuella.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ctrlTecladoHuella.BackColor = System.Drawing.Color.Transparent;
            this.ctrlTecladoHuella.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ctrlTecladoHuella.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctrlTecladoHuella.Location = new System.Drawing.Point(4, 44);
            this.ctrlTecladoHuella.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ctrlTecladoHuella.Name = "ctrlTecladoHuella";
            this.ctrlTecladoHuella.Quantity = "";
            this.ctrlTecladoHuella.Size = new System.Drawing.Size(236, 274);
            this.ctrlTecladoHuella.TabIndex = 14;
            this.ctrlTecladoHuella.ctrlOk += new Sibo.WhiteList.IngresoTouch.ctrlKeyboard.ctrlOkHandler(this.ctrlTecladoHuella_ctrlOk);
            // 
            // txtDocument
            // 
            this.txtDocument.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDocument.Location = new System.Drawing.Point(4, 21);
            this.txtDocument.Name = "txtDocument";
            this.txtDocument.Size = new System.Drawing.Size(128, 22);
            this.txtDocument.TabIndex = 23;
            this.txtDocument.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDocument_KeyPress);
            // 
            // lblDocumentTitle
            // 
            this.lblDocumentTitle.AutoSize = true;
            this.lblDocumentTitle.Font = new System.Drawing.Font("Lucida Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDocumentTitle.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblDocumentTitle.Location = new System.Drawing.Point(1, 7);
            this.lblDocumentTitle.Name = "lblDocumentTitle";
            this.lblDocumentTitle.Size = new System.Drawing.Size(144, 12);
            this.lblDocumentTitle.TabIndex = 22;
            this.lblDocumentTitle.Text = "Documento del Cliente";
            // 
            // lblText
            // 
            this.lblText.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblText.ForeColor = System.Drawing.Color.Red;
            this.lblText.Location = new System.Drawing.Point(0, 323);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(716, 43);
            this.lblText.TabIndex = 23;
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmRecordFingerprint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 366);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.picPhoto);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.lblQualityFingerprintValue);
            this.Controls.Add(this.lblAcceptableFingerprintQualityValue);
            this.Controls.Add(this.lblQualityFingerprint);
            this.Controls.Add(this.lblAcceptableFingerprintQuality);
            this.Controls.Add(this.lblClientName);
            this.Controls.Add(this.txtDocument);
            this.Controls.Add(this.lblDocumentTitle);
            this.Controls.Add(this.ctrlTecladoHuella);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRecordFingerprint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Grabar Huella";
            this.Activated += new System.EventHandler(this.FrmRecordFingerprint_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRecordFingerprint_FormClosing);
            this.Load += new System.EventHandler(this.FrmRecordFingerprint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox picPhoto;
        internal System.Windows.Forms.Button btnClose;
        internal System.Windows.Forms.Label lblData;
        internal System.Windows.Forms.Label lblQualityFingerprintValue;
        internal System.Windows.Forms.Label lblAcceptableFingerprintQualityValue;
        internal System.Windows.Forms.Label lblQualityFingerprint;
        internal System.Windows.Forms.Label lblAcceptableFingerprintQuality;
        internal System.Windows.Forms.Label lblClientName;
        internal ctrlKeyboard ctrlTecladoHuella;
        internal System.Windows.Forms.TextBox txtDocument;
        public System.Windows.Forms.Label lblDocumentTitle;
        private System.Windows.Forms.Label lblText;
    }
}