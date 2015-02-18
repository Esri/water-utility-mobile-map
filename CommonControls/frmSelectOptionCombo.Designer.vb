<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectOptionCombo
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.gpBoxText = New System.Windows.Forms.GroupBox()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.gpBoxButtons = New System.Windows.Forms.GroupBox()
        Me.btnAccept = New System.Windows.Forms.Button()
        Me.chkPersist = New System.Windows.Forms.CheckBox()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.gpBoxText.SuspendLayout()
        Me.gpBoxButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBoxText
        '
        Me.gpBoxText.Controls.Add(Me.lblMessage)
        Me.gpBoxText.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxText.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxText.Name = "gpBoxText"
        Me.gpBoxText.Size = New System.Drawing.Size(571, 100)
        Me.gpBoxText.TabIndex = 0
        Me.gpBoxText.TabStop = False
        '
        'lblMessage
        '
        Me.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMessage.Font = New System.Drawing.Font("Tahoma", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMessage.Location = New System.Drawing.Point(3, 16)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(565, 81)
        Me.lblMessage.TabIndex = 0
        Me.lblMessage.Text = "This is sample text"
        Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gpBoxButtons
        '
        Me.gpBoxButtons.Controls.Add(Me.btnAccept)
        Me.gpBoxButtons.Controls.Add(Me.chkPersist)
        Me.gpBoxButtons.Controls.Add(Me.btnClose)
        Me.gpBoxButtons.Controls.Add(Me.ComboBox1)
        Me.gpBoxButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBoxButtons.Location = New System.Drawing.Point(0, 100)
        Me.gpBoxButtons.Name = "gpBoxButtons"
        Me.gpBoxButtons.Size = New System.Drawing.Size(571, 177)
        Me.gpBoxButtons.TabIndex = 1
        Me.gpBoxButtons.TabStop = False
        '
        'btnAccept
        '
        Me.btnAccept.BackColor = System.Drawing.Color.Transparent
        Me.btnAccept.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.CheckGreen
        Me.btnAccept.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAccept.FlatAppearance.BorderSize = 0
        Me.btnAccept.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAccept.Location = New System.Drawing.Point(353, 72)
        Me.btnAccept.Name = "btnAccept"
        Me.btnAccept.Size = New System.Drawing.Size(100, 100)
        Me.btnAccept.TabIndex = 17
        Me.btnAccept.UseVisualStyleBackColor = False
        '
        'chkPersist
        '
        Me.chkPersist.AutoSize = True
        Me.chkPersist.Font = New System.Drawing.Font("Tahoma", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkPersist.Location = New System.Drawing.Point(12, 105)
        Me.chkPersist.Name = "chkPersist"
        Me.chkPersist.Size = New System.Drawing.Size(132, 29)
        Me.chkPersist.TabIndex = 16
        Me.chkPersist.Text = "CheckBox1"
        Me.chkPersist.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.BackColor = System.Drawing.Color.Transparent
        Me.btnClose.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.CancelRed
        Me.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnClose.FlatAppearance.BorderSize = 0
        Me.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClose.Location = New System.Drawing.Point(459, 72)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 100)
        Me.btnClose.TabIndex = 15
        Me.btnClose.UseVisualStyleBackColor = False
        '
        'ComboBox1
        '
        Me.ComboBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 27.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(3, 16)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(565, 50)
        Me.ComboBox1.TabIndex = 0
        Me.ComboBox1.IntegralHeight = False

        '
        'frmSelectOptionCombo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(571, 277)
        Me.Controls.Add(Me.gpBoxButtons)
        Me.Controls.Add(Me.gpBoxText)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSelectOptionCombo"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.TopMost = True
        Me.gpBoxText.ResumeLayout(False)
        Me.gpBoxButtons.ResumeLayout(False)
        Me.gpBoxButtons.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBoxText As System.Windows.Forms.GroupBox
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents gpBoxButtons As System.Windows.Forms.GroupBox
    Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Friend WithEvents chkPersist As System.Windows.Forms.CheckBox
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnAccept As System.Windows.Forms.Button
End Class
