<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MobileTOC
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
        Me.tvToc = New System.Windows.Forms.TreeView
        Me.SuspendLayout()
        '
        'tvToc
        '
        Me.tvToc.CheckBoxes = True
        Me.tvToc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvToc.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tvToc.Location = New System.Drawing.Point(0, 0)
        Me.tvToc.Name = "tvToc"
        Me.tvToc.Size = New System.Drawing.Size(268, 328)
        Me.tvToc.TabIndex = 0
        '
        'MobileTOC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.tvToc)
        Me.Name = "MobileTOC"
        Me.Size = New System.Drawing.Size(268, 328)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tvToc As System.Windows.Forms.TreeView

   
End Class
