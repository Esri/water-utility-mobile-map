<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AttachmentControl
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
        Me.splitMainAtt = New System.Windows.Forms.SplitContainer()
        Me.lstAtt = New System.Windows.Forms.ListBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnAttCam = New System.Windows.Forms.Button()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.splitMainAtt.Panel1.SuspendLayout()
        Me.splitMainAtt.Panel2.SuspendLayout()
        Me.splitMainAtt.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitMainAtt
        '
        Me.splitMainAtt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitMainAtt.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitMainAtt.IsSplitterFixed = True
        Me.splitMainAtt.Location = New System.Drawing.Point(0, 0)
        Me.splitMainAtt.Name = "splitMainAtt"
        Me.splitMainAtt.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitMainAtt.Panel1
        '
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnAttCam)
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnRemove)
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnAdd)
        Me.splitMainAtt.Panel1MinSize = 50
        '
        'splitMainAtt.Panel2
        '
        Me.splitMainAtt.Panel2.Controls.Add(Me.lstAtt)
        Me.splitMainAtt.Panel2MinSize = 50
        Me.splitMainAtt.Size = New System.Drawing.Size(241, 694)
        Me.splitMainAtt.SplitterDistance = 65
        Me.splitMainAtt.TabIndex = 0
        '
        'lstAtt
        '
        Me.lstAtt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstAtt.FormattingEnabled = True
        Me.lstAtt.Location = New System.Drawing.Point(0, 0)
        Me.lstAtt.Name = "lstAtt"
        Me.lstAtt.Size = New System.Drawing.Size(241, 625)
        Me.lstAtt.TabIndex = 0
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnAttCam
        '
        Me.btnAttCam.BackColor = System.Drawing.Color.Transparent
        Me.btnAttCam.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddCameraAtta
        Me.btnAttCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnAttCam.FlatAppearance.BorderSize = 0
        Me.btnAttCam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAttCam.Location = New System.Drawing.Point(90, 3)
        Me.btnAttCam.Name = "btnAttCam"
        Me.btnAttCam.Size = New System.Drawing.Size(45, 45)
        Me.btnAttCam.TabIndex = 4
        Me.btnAttCam.UseVisualStyleBackColor = False
        '
        'btnRemove
        '
        Me.btnRemove.BackColor = System.Drawing.Color.Transparent
        Me.btnRemove.BackgroundImage = Global.MobileControls.My.Resources.Resources.DelAtt
        Me.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnRemove.FlatAppearance.BorderSize = 0
        Me.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRemove.Location = New System.Drawing.Point(150, 3)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(45, 45)
        Me.btnRemove.TabIndex = 3
        Me.btnRemove.UseVisualStyleBackColor = False
        '
        'btnAdd
        '
        Me.btnAdd.BackColor = System.Drawing.Color.Transparent
        Me.btnAdd.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddAtt
        Me.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnAdd.FlatAppearance.BorderSize = 0
        Me.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAdd.Location = New System.Drawing.Point(33, 3)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(45, 45)
        Me.btnAdd.TabIndex = 2
        Me.btnAdd.UseVisualStyleBackColor = False
        '
        'AttachmentControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.splitMainAtt)
        Me.Name = "AttachmentControl"
        Me.Size = New System.Drawing.Size(241, 694)
        Me.splitMainAtt.Panel1.ResumeLayout(False)
        Me.splitMainAtt.Panel2.ResumeLayout(False)
        Me.splitMainAtt.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents splitMainAtt As System.Windows.Forms.SplitContainer
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents lstAtt As System.Windows.Forms.ListBox
    Friend WithEvents btnAttCam As System.Windows.Forms.Button

End Class
