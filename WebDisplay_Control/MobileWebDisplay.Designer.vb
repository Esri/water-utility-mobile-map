<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MobileWebDisplay
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
        Me.lstSites = New System.Windows.Forms.ListView()
        Me.gpBxControls = New System.Windows.Forms.GroupBox()
        Me.btnToggleLinkDisplay = New System.Windows.Forms.Button()
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.gpBxControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'lstSites
        '
        Me.lstSites.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstSites.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstSites.ForeColor = System.Drawing.Color.RoyalBlue
        Me.lstSites.Location = New System.Drawing.Point(0, 0)
        Me.lstSites.MultiSelect = False
        Me.lstSites.Name = "lstSites"
        Me.lstSites.Size = New System.Drawing.Size(351, 269)
        Me.lstSites.TabIndex = 0
        Me.lstSites.UseCompatibleStateImageBehavior = False
        Me.lstSites.View = System.Windows.Forms.View.List
        '
        'gpBxControls
        '
        Me.gpBxControls.Controls.Add(Me.btnToggleLinkDisplay)
        Me.gpBxControls.Controls.Add(Me.btnOpen)
        Me.gpBxControls.Controls.Add(Me.btnDelete)
        Me.gpBxControls.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.gpBxControls.Location = New System.Drawing.Point(0, 269)
        Me.gpBxControls.Name = "gpBxControls"
        Me.gpBxControls.Size = New System.Drawing.Size(351, 64)
        Me.gpBxControls.TabIndex = 1
        Me.gpBxControls.TabStop = False
        '
        'btnToggleLinkDisplay
        '
        Me.btnToggleLinkDisplay.BackgroundImage = Global.MobileControls.My.Resources.Resources.File
        Me.btnToggleLinkDisplay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnToggleLinkDisplay.Location = New System.Drawing.Point(128, 6)
        Me.btnToggleLinkDisplay.Name = "btnToggleLinkDisplay"
        Me.btnToggleLinkDisplay.Size = New System.Drawing.Size(55, 55)
        Me.btnToggleLinkDisplay.TabIndex = 2
        Me.btnToggleLinkDisplay.UseVisualStyleBackColor = True
        '
        'btnOpen
        '
        Me.btnOpen.BackgroundImage = Global.MobileControls.My.Resources.Resources.greenright
        Me.btnOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnOpen.Location = New System.Drawing.Point(67, 6)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(55, 55)
        Me.btnOpen.TabIndex = 1
        Me.btnOpen.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.BackgroundImage = Global.MobileControls.My.Resources.Resources.Delete2
        Me.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnDelete.Location = New System.Drawing.Point(6, 6)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(55, 55)
        Me.btnDelete.TabIndex = 0
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'MobileWebDisplay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lstSites)
        Me.Controls.Add(Me.gpBxControls)
        Me.Name = "MobileWebDisplay"
        Me.Size = New System.Drawing.Size(351, 333)
        Me.gpBxControls.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lstSites As System.Windows.Forms.ListView
    Friend WithEvents gpBxControls As System.Windows.Forms.GroupBox
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents btnToggleLinkDisplay As System.Windows.Forms.Button

End Class
