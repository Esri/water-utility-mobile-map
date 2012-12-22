<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WOList
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
        Me.trVwWO = New System.Windows.Forms.TreeView()
        Me.SuspendLayout()
        '
        'trVwWO
        '
        Me.trVwWO.Dock = System.Windows.Forms.DockStyle.Fill
        Me.trVwWO.Location = New System.Drawing.Point(0, 0)
        Me.trVwWO.Name = "trVwWO"
        Me.trVwWO.Size = New System.Drawing.Size(150, 150)
        Me.trVwWO.TabIndex = 0
        '
        'WOList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.trVwWO)
        Me.Name = "WOList"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents trVwWO As System.Windows.Forms.TreeView

End Class
