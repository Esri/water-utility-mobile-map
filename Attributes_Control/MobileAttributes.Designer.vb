<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MobileAttributes
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.gpBxLayer = New System.Windows.Forms.GroupBox()
        Me.gpBoxAttLay = New System.Windows.Forms.GroupBox()
        Me.SuspendLayout()
        '
        'gpBxLayer
        '
        Me.gpBxLayer.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBxLayer.Location = New System.Drawing.Point(0, 0)
        Me.gpBxLayer.Name = "gpBxLayer"
        Me.gpBxLayer.Size = New System.Drawing.Size(316, 52)
        Me.gpBxLayer.TabIndex = 0
        Me.gpBxLayer.TabStop = False
        '
        'gpBoxAttLay
        '
        Me.gpBoxAttLay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpBoxAttLay.Location = New System.Drawing.Point(0, 52)
        Me.gpBoxAttLay.Name = "gpBoxAttLay"
        Me.gpBoxAttLay.Size = New System.Drawing.Size(316, 583)
        Me.gpBoxAttLay.TabIndex = 1
        Me.gpBoxAttLay.TabStop = False
        '
        'MobileAttributes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gpBoxAttLay)
        Me.Controls.Add(Me.gpBxLayer)
        Me.Name = "MobileAttributes"
        Me.Size = New System.Drawing.Size(316, 635)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBxLayer As System.Windows.Forms.GroupBox
    Friend WithEvents gpBoxAttLay As System.Windows.Forms.GroupBox

End Class
