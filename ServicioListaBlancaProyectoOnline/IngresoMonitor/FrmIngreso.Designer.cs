﻿namespace Sibo.WhiteList.IngresoMonitor
{
    partial class FrmIngresos
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmIngresos));
            this.lblEntryHour = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblPlan = new System.Windows.Forms.Label();
            this.lblExpirationDate = new System.Windows.Forms.Label();
            this.lblAccessMessage = new System.Windows.Forms.Label();
            this.lblrestrictionMessage = new System.Windows.Forms.Label();
            this.dgvEntries = new System.Windows.Forms.DataGridView();
            this.cmsOpciones = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.descargarHuellaDeOtraSedeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemRecordFingerprint = new System.Windows.Forms.ToolStripMenuItem();
            this.itemRecordFingerprintUSB = new System.Windows.Forms.ToolStripMenuItem();
            this.itemDisabledEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.itemAbrirPuerta = new System.Windows.Forms.ToolStripMenuItem();
            this.itemEntryByID = new System.Windows.Forms.ToolStripMenuItem();
            this.itemRestartTerminal = new System.Windows.Forms.ToolStripMenuItem();
            this.itemDisabledExit = new System.Windows.Forms.ToolStripMenuItem();
            this.itemExitByID = new System.Windows.Forms.ToolStripMenuItem();
            this.visitantesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.picFoto = new System.Windows.Forms.PictureBox();
            this.piclogo1 = new System.Windows.Forms.PictureBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntries)).BeginInit();
            this.cmsOpciones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.piclogo1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblEntryHour
            // 
            this.lblEntryHour.AutoSize = true;
            this.lblEntryHour.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEntryHour.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblEntryHour.Location = new System.Drawing.Point(15, 131);
            this.lblEntryHour.Name = "lblEntryHour";
            this.lblEntryHour.Size = new System.Drawing.Size(176, 22);
            this.lblEntryHour.TabIndex = 2;
            this.lblEntryHour.Text = "Hora de Ingreso:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblName.Location = new System.Drawing.Point(15, 157);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(95, 22);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Nombre:";
            // 
            // lblPlan
            // 
            this.lblPlan.AutoSize = true;
            this.lblPlan.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlan.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblPlan.Location = new System.Drawing.Point(15, 183);
            this.lblPlan.Name = "lblPlan";
            this.lblPlan.Size = new System.Drawing.Size(59, 22);
            this.lblPlan.TabIndex = 4;
            this.lblPlan.Text = "Plan:";
            // 
            // lblExpirationDate
            // 
            this.lblExpirationDate.AutoSize = true;
            this.lblExpirationDate.Font = new System.Drawing.Font("Lucida Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpirationDate.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblExpirationDate.Location = new System.Drawing.Point(15, 208);
            this.lblExpirationDate.Name = "lblExpirationDate";
            this.lblExpirationDate.Size = new System.Drawing.Size(234, 22);
            this.lblExpirationDate.TabIndex = 5;
            this.lblExpirationDate.Text = "Fecha de Vencimiento:";
            // 
            // lblAccessMessage
            // 
            this.lblAccessMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAccessMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblAccessMessage.Font = new System.Drawing.Font("Lucida Sans", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccessMessage.Location = new System.Drawing.Point(-70, 267);
            this.lblAccessMessage.Name = "lblAccessMessage";
            this.lblAccessMessage.Size = new System.Drawing.Size(1126, 76);
            this.lblAccessMessage.TabIndex = 61;
            this.lblAccessMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblrestrictionMessage
            // 
            this.lblrestrictionMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblrestrictionMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblrestrictionMessage.Font = new System.Drawing.Font("Lucida Sans", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblrestrictionMessage.Location = new System.Drawing.Point(-69, 313);
            this.lblrestrictionMessage.Name = "lblrestrictionMessage";
            this.lblrestrictionMessage.Size = new System.Drawing.Size(1126, 110);
            this.lblrestrictionMessage.TabIndex = 62;
            this.lblrestrictionMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dgvEntries
            // 
            this.dgvEntries.AllowUserToAddRows = false;
            this.dgvEntries.AllowUserToDeleteRows = false;
            this.dgvEntries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntries.ContextMenuStrip = this.cmsOpciones;
            this.dgvEntries.Location = new System.Drawing.Point(12, 447);
            this.dgvEntries.Name = "dgvEntries";
            this.dgvEntries.ReadOnly = true;
            this.dgvEntries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvEntries.Size = new System.Drawing.Size(1327, 240);
            this.dgvEntries.TabIndex = 55;
            this.dgvEntries.CurrentCellChanged += new System.EventHandler(this.dgvEntries_CurrentCellChanged);
            // 
            // cmsOpciones
            // 
            this.cmsOpciones.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.cmsOpciones.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.descargarHuellaDeOtraSedeToolStripMenuItem,
            this.itemRecordFingerprint,
            this.itemRecordFingerprintUSB,
            this.itemDisabledEntry,
            this.itemAbrirPuerta,
            this.itemEntryByID,
            this.itemRestartTerminal,
            this.itemDisabledExit,
            this.itemExitByID,
            this.visitantesToolStripMenuItem,
            this.itemExit});
            this.cmsOpciones.Name = "cmsOpciones";
            this.cmsOpciones.Size = new System.Drawing.Size(237, 334);
            // 
            // descargarHuellaDeOtraSedeToolStripMenuItem
            // 
            this.descargarHuellaDeOtraSedeToolStripMenuItem.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Sincronizar;
            this.descargarHuellaDeOtraSedeToolStripMenuItem.Name = "descargarHuellaDeOtraSedeToolStripMenuItem";
            this.descargarHuellaDeOtraSedeToolStripMenuItem.Size = new System.Drawing.Size(236, 30);
            this.descargarHuellaDeOtraSedeToolStripMenuItem.Text = "Descargar huella de otra sede";
            this.descargarHuellaDeOtraSedeToolStripMenuItem.Click += new System.EventHandler(this.descargarHuellaDeOtraSedeToolStripMenuItem_Click);
            // 
            // itemRecordFingerprint
            // 
            this.itemRecordFingerprint.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Sincronizar;
            this.itemRecordFingerprint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemRecordFingerprint.Name = "itemRecordFingerprint";
            this.itemRecordFingerprint.Size = new System.Drawing.Size(236, 30);
            this.itemRecordFingerprint.Text = "Grabar huella en terminal";
            this.itemRecordFingerprint.Click += new System.EventHandler(this.itemRecordFingerprint_Click);
            // 
            // itemRecordFingerprintUSB
            // 
            this.itemRecordFingerprintUSB.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Sincronizar;
            this.itemRecordFingerprintUSB.Name = "itemRecordFingerprintUSB";
            this.itemRecordFingerprintUSB.Size = new System.Drawing.Size(236, 30);
            this.itemRecordFingerprintUSB.Text = "Grabar huella en USB";
            this.itemRecordFingerprintUSB.Click += new System.EventHandler(this.itemRecordFingerprintUSB_Click);
            // 
            // itemDisabledEntry
            // 
            this.itemDisabledEntry.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.AbrirPuerta;
            this.itemDisabledEntry.Name = "itemDisabledEntry";
            this.itemDisabledEntry.Size = new System.Drawing.Size(236, 30);
            this.itemDisabledEntry.Text = "Ingreso de discapacitados";
            this.itemDisabledEntry.Click += new System.EventHandler(this.itemDisabledEntry_Click);
            // 
            // itemAbrirPuerta
            // 
            this.itemAbrirPuerta.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.AbrirPuerta;
            this.itemAbrirPuerta.Name = "itemAbrirPuerta";
            this.itemAbrirPuerta.Size = new System.Drawing.Size(236, 30);
            this.itemAbrirPuerta.Text = "Ingreso adicional";
            this.itemAbrirPuerta.Click += new System.EventHandler(this.itemAbrirPuerta_Click);
            // 
            // itemEntryByID
            // 
            this.itemEntryByID.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.AbrirPuerta;
            this.itemEntryByID.Name = "itemEntryByID";
            this.itemEntryByID.Size = new System.Drawing.Size(236, 30);
            this.itemEntryByID.Text = "Ingreso por ID";
            this.itemEntryByID.Click += new System.EventHandler(this.itemEntryByID_Click);
            // 
            // itemRestartTerminal
            // 
            this.itemRestartTerminal.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Desbloquear;
            this.itemRestartTerminal.Name = "itemRestartTerminal";
            this.itemRestartTerminal.Size = new System.Drawing.Size(236, 30);
            this.itemRestartTerminal.Text = "Reiniciar terminal";
            this.itemRestartTerminal.Click += new System.EventHandler(this.itemRestartTerminal_Click);
            // 
            // itemDisabledExit
            // 
            this.itemDisabledExit.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.AbrirPuerta;
            this.itemDisabledExit.Name = "itemDisabledExit";
            this.itemDisabledExit.Size = new System.Drawing.Size(236, 30);
            this.itemDisabledExit.Text = "Salida de discapacitados";
            this.itemDisabledExit.Click += new System.EventHandler(this.itemDisabledExit_Click);
            // 
            // itemExitByID
            // 
            this.itemExitByID.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.AbrirPuerta;
            this.itemExitByID.Name = "itemExitByID";
            this.itemExitByID.Size = new System.Drawing.Size(236, 30);
            this.itemExitByID.Text = "Salida por ID";
            this.itemExitByID.Click += new System.EventHandler(this.itemExitByID_Click);
            // 
            // visitantesToolStripMenuItem
            // 
            this.visitantesToolStripMenuItem.Enabled = false;
            this.visitantesToolStripMenuItem.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.empleados;
            this.visitantesToolStripMenuItem.Name = "visitantesToolStripMenuItem";
            this.visitantesToolStripMenuItem.Size = new System.Drawing.Size(236, 30);
            this.visitantesToolStripMenuItem.Text = "Visitantes";
            this.visitantesToolStripMenuItem.Visible = false;
            this.visitantesToolStripMenuItem.Click += new System.EventHandler(this.visitantesToolStripMenuItem_Click);
            // 
            // itemExit
            // 
            this.itemExit.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Salir;
            this.itemExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemExit.Name = "itemExit";
            this.itemExit.Size = new System.Drawing.Size(236, 30);
            this.itemExit.Text = "Salir";
            this.itemExit.Click += new System.EventHandler(this.itemExit_Click);
            // 
            // lblMensaje
            // 
            this.lblMensaje.BackColor = System.Drawing.Color.White;
            this.lblMensaje.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblMensaje.Location = new System.Drawing.Point(13, 423);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(403, 19);
            this.lblMensaje.TabIndex = 65;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.sibo_avance;
            this.pictureBox1.Location = new System.Drawing.Point(975, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(125, 81);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 66;
            this.pictureBox1.TabStop = false;
            // 
            // picFoto
            // 
            this.picFoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picFoto.Location = new System.Drawing.Point(1124, 2);
            this.picFoto.Name = "picFoto";
            this.picFoto.Size = new System.Drawing.Size(215, 203);
            this.picFoto.TabIndex = 1;
            this.picFoto.TabStop = false;
            // 
            // piclogo1
            // 
            this.piclogo1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.piclogo1.BackgroundImage = global::Sibo.WhiteList.IngresoMonitor.Properties.Resources.Splash;
            this.piclogo1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.piclogo1.Location = new System.Drawing.Point(2, 0);
            this.piclogo1.Name = "piclogo1";
            this.piclogo1.Size = new System.Drawing.Size(1123, 108);
            this.piclogo1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.piclogo1.TabIndex = 0;
            this.piclogo1.TabStop = false;
            // 
            // FrmIngresos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(913, 487);
            this.ContextMenuStrip = this.cmsOpciones;
            this.Controls.Add(this.lblrestrictionMessage);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.dgvEntries);
            this.Controls.Add(this.lblAccessMessage);
            this.Controls.Add(this.lblExpirationDate);
            this.Controls.Add(this.lblPlan);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblEntryHour);
            this.Controls.Add(this.picFoto);
            this.Controls.Add(this.piclogo1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(915, 486);
            this.Name = "FrmIngresos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitor Gymsoft";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmIngresos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntries)).EndInit();
            this.cmsOpciones.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.piclogo1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox piclogo1;
        private System.Windows.Forms.PictureBox picFoto;
        private System.Windows.Forms.Label lblEntryHour;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblPlan;
        private System.Windows.Forms.Label lblExpirationDate;
        private System.Windows.Forms.Label lblAccessMessage;
        private System.Windows.Forms.Label lblrestrictionMessage;
        private System.Windows.Forms.ContextMenuStrip cmsOpciones;
        private System.Windows.Forms.ToolStripMenuItem itemRecordFingerprint;
        private System.Windows.Forms.ToolStripMenuItem itemExit;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.PictureBox pictureBox1;
        internal System.Windows.Forms.DataGridView dgvEntries;
        private System.Windows.Forms.ToolStripMenuItem itemRecordFingerprintUSB;
        private System.Windows.Forms.ToolStripMenuItem itemDisabledEntry;
        private System.Windows.Forms.ToolStripMenuItem itemDisabledExit;
        private System.Windows.Forms.ToolStripMenuItem itemEntryByID;
        private System.Windows.Forms.ToolStripMenuItem itemExitByID;
        private System.Windows.Forms.ToolStripMenuItem itemRestartTerminal;
        private System.Windows.Forms.ToolStripMenuItem descargarHuellaDeOtraSedeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visitantesToolStripMenuItem;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.ToolStripMenuItem itemAbrirPuerta;
    }
}

