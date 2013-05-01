<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEnterText
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
        Me.gpBoxTextText = New System.Windows.Forms.GroupBox()
        Me.lblMessageText = New System.Windows.Forms.Label()
        Me.gpBoxButtonsText = New System.Windows.Forms.GroupBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.btnAcceptText = New System.Windows.Forms.Button()
        Me.btnCloseText = New System.Windows.Forms.Button()
        Me.gpBoxTextText.SuspendLayout()
        Me.gpBoxButtonsText.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBoxTextText
        '
        Me.gpBoxTextText.Controls.Add(Me.lblMessageText)
        Me.gpBoxTextText.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxTextText.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxTextText.Name = "gpBoxTextText"
        Me.gpBoxTextText.Size = New System.Drawing.Size(571, 100)
        Me.gpBoxTextText.TabIndex = 0
        Me.gpBoxTextText.TabStop = False
        '
        'lblMessageText
        '
        Me.lblMessageText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMessageText.Font = New System.Drawing.Font("Tahoma", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMessageText.Location = New System.Drawing.Point(3, 16)
        Me.lblMessageText.Name = "lblMessageText"
        Me.lblMessageText.Size = New System.Drawing.Size(565, 81)
        Me.lblMessageText.TabIndex = 0
        Me.lblMessageText.Text = "This is sample text"
        Me.lblMessageText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gpBoxButtonsText
        '
        Me.gpBoxButtonsText.Controls.Add(Me.TextBox1)
        Me.gpBoxButtonsText.Controls.Add(Me.btnAcceptText)
        Me.gpBoxButtonsText.Controls.Add(Me.btnCloseText)
        Me.gpBoxButtonsText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBoxButtonsText.Location = New System.Drawing.Point(0, 100)
        Me.gpBoxButtonsText.Name = "gpBoxButtonsText"
        Me.gpBoxButtonsText.Size = New System.Drawing.Size(571, 177)
        Me.gpBoxButtonsText.TabIndex = 1
        Me.gpBoxButtonsText.TabStop = False
        '
        'TextBox1
        '
        Me.TextBox1.Font = New System.Drawing.Font("Tahoma", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBox1.Location = New System.Drawing.Point(8, 20)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(551, 33)
        Me.TextBox1.TabIndex = 0
        '
        'btnAcceptText
        '
        Me.btnAcceptText.BackColor = System.Drawing.Color.Transparent
        Me.btnAcceptText.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.CheckGreen
        Me.btnAcceptText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAcceptText.FlatAppearance.BorderSize = 0
        Me.btnAcceptText.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAcceptText.Location = New System.Drawing.Point(353, 72)
        Me.btnAcceptText.Name = "btnAcceptText"
        Me.btnAcceptText.Size = New System.Drawing.Size(100, 100)
        Me.btnAcceptText.TabIndex = 1
        Me.btnAcceptText.UseVisualStyleBackColor = False
        '
        'btnCloseText
        '
        Me.btnCloseText.BackColor = System.Drawing.Color.Transparent
        Me.btnCloseText.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.CancelRed
        Me.btnCloseText.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnCloseText.FlatAppearance.BorderSize = 0
        Me.btnCloseText.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCloseText.Location = New System.Drawing.Point(459, 72)
        Me.btnCloseText.Name = "btnCloseText"
        Me.btnCloseText.Size = New System.Drawing.Size(100, 100)
        Me.btnCloseText.TabIndex = 2
        Me.btnCloseText.UseVisualStyleBackColor = False
        '
        'frmEnterText
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(571, 277)
        Me.Controls.Add(Me.gpBoxButtonsText)
        Me.Controls.Add(Me.gpBoxTextText)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEnterText"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.TopMost = True
        Me.gpBoxTextText.ResumeLayout(False)
        Me.gpBoxButtonsText.ResumeLayout(False)
        Me.gpBoxButtonsText.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents gpBoxTextText As System.Windows.Forms.GroupBox
    Private WithEvents lblMessageText As System.Windows.Forms.Label
    Private WithEvents gpBoxButtonsText As System.Windows.Forms.GroupBox

    Private WithEvents btnCloseText As System.Windows.Forms.Button
    Private WithEvents btnAcceptText As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
End Class
