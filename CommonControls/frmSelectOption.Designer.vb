<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectOption
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
        Me.tblLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.btnOpt2 = New System.Windows.Forms.Button()
        Me.btnOpt1 = New System.Windows.Forms.Button()
        Me.gpBoxText.SuspendLayout()
        Me.gpBoxButtons.SuspendLayout()
        Me.tblLayout.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBoxText
        '
        Me.gpBoxText.Controls.Add(Me.lblMessage)
        Me.gpBoxText.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxText.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxText.Name = "gpBoxText"
        Me.gpBoxText.Size = New System.Drawing.Size(563, 100)
        Me.gpBoxText.TabIndex = 0
        Me.gpBoxText.TabStop = False
        '
        'lblMessage
        '
        Me.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMessage.Font = New System.Drawing.Font("Tahoma", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblMessage.Location = New System.Drawing.Point(3, 16)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(557, 81)
        Me.lblMessage.TabIndex = 0
        Me.lblMessage.Text = "This is sample text"
        Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gpBoxButtons
        '
        Me.gpBoxButtons.Controls.Add(Me.tblLayout)
        Me.gpBoxButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBoxButtons.Location = New System.Drawing.Point(0, 100)
        Me.gpBoxButtons.Name = "gpBoxButtons"
        Me.gpBoxButtons.Size = New System.Drawing.Size(563, 145)
        Me.gpBoxButtons.TabIndex = 1
        Me.gpBoxButtons.TabStop = False
        '
        'tblLayout
        '
        Me.tblLayout.ColumnCount = 2
        Me.tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.tblLayout.Controls.Add(Me.btnOpt2, 1, 0)
        Me.tblLayout.Controls.Add(Me.btnOpt1, 0, 0)
        Me.tblLayout.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tblLayout.Location = New System.Drawing.Point(3, 16)
        Me.tblLayout.Name = "tblLayout"
        Me.tblLayout.RowCount = 1
        Me.tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tblLayout.Size = New System.Drawing.Size(557, 126)
        Me.tblLayout.TabIndex = 0
        '
        'btnOpt2
        '
        Me.btnOpt2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpt2.AutoEllipsis = True
        Me.btnOpt2.Font = New System.Drawing.Font("Tahoma", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOpt2.Location = New System.Drawing.Point(281, 3)
        Me.btnOpt2.Name = "btnOpt2"
        Me.btnOpt2.Size = New System.Drawing.Size(273, 120)
        Me.btnOpt2.TabIndex = 1
        Me.btnOpt2.Text = "Button2"
        Me.btnOpt2.UseVisualStyleBackColor = True
        '
        'btnOpt1
        '
        Me.btnOpt1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpt1.Font = New System.Drawing.Font("Tahoma", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOpt1.Location = New System.Drawing.Point(3, 3)
        Me.btnOpt1.Name = "btnOpt1"
        Me.btnOpt1.Size = New System.Drawing.Size(272, 120)
        Me.btnOpt1.TabIndex = 0
        Me.btnOpt1.Text = "Button1"
        Me.btnOpt1.UseVisualStyleBackColor = True
        '
        'frmSelectOption
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(563, 245)
        Me.Controls.Add(Me.gpBoxButtons)
        Me.Controls.Add(Me.gpBoxText)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSelectOption"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.TopMost = True
        Me.gpBoxText.ResumeLayout(False)
        Me.gpBoxButtons.ResumeLayout(False)
        Me.tblLayout.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBoxText As System.Windows.Forms.GroupBox
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents gpBoxButtons As System.Windows.Forms.GroupBox
    Friend WithEvents tblLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnOpt2 As System.Windows.Forms.Button
    Friend WithEvents btnOpt1 As System.Windows.Forms.Button
End Class
