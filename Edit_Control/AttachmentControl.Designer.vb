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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.lstAtt = New System.Windows.Forms.ListBox()
        Me.pnlSketchPalette = New System.Windows.Forms.Panel()
        Me.pnlSketchButtons = New System.Windows.Forms.Panel()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnAttCam = New System.Windows.Forms.Button()
        Me.btnAddSketch = New System.Windows.Forms.Button()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.btnRemoveLast = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.splitMainAtt.Panel1.SuspendLayout()
        Me.splitMainAtt.Panel2.SuspendLayout()
        Me.splitMainAtt.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlSketchButtons.SuspendLayout()
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
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnAdd)
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnAttCam)
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnAddSketch)
        Me.splitMainAtt.Panel1.Controls.Add(Me.btnRemove)
        Me.splitMainAtt.Panel1MinSize = 50
        '
        'splitMainAtt.Panel2
        '
        Me.splitMainAtt.Panel2.Controls.Add(Me.SplitContainer1)
        Me.splitMainAtt.Panel2MinSize = 50
        Me.splitMainAtt.Size = New System.Drawing.Size(241, 694)
        Me.splitMainAtt.SplitterDistance = 65
        Me.splitMainAtt.TabIndex = 0
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lstAtt)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlSketchPalette)
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlSketchButtons)
        Me.SplitContainer1.Size = New System.Drawing.Size(241, 625)
        Me.SplitContainer1.SplitterDistance = 212
        Me.SplitContainer1.TabIndex = 0
        '
        'lstAtt
        '
        Me.lstAtt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstAtt.FormattingEnabled = True
        Me.lstAtt.Location = New System.Drawing.Point(0, 0)
        Me.lstAtt.Name = "lstAtt"
        Me.lstAtt.Size = New System.Drawing.Size(241, 212)
        Me.lstAtt.TabIndex = 0
        '
        'pnlSketchPalette
        '
        Me.pnlSketchPalette.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlSketchPalette.Location = New System.Drawing.Point(0, 0)
        Me.pnlSketchPalette.Name = "pnlSketchPalette"
        Me.pnlSketchPalette.Size = New System.Drawing.Size(188, 409)
        Me.pnlSketchPalette.TabIndex = 2
        '
        'pnlSketchButtons
        '
        Me.pnlSketchButtons.Controls.Add(Me.btnRemoveLast)
        Me.pnlSketchButtons.Controls.Add(Me.btnClear)
        Me.pnlSketchButtons.Controls.Add(Me.btnSave)
        Me.pnlSketchButtons.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlSketchButtons.Location = New System.Drawing.Point(188, 0)
        Me.pnlSketchButtons.Name = "pnlSketchButtons"
        Me.pnlSketchButtons.Size = New System.Drawing.Size(53, 409)
        Me.pnlSketchButtons.TabIndex = 1
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnAdd
        '
        Me.btnAdd.BackColor = System.Drawing.Color.Transparent
        Me.btnAdd.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddAtt
        Me.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnAdd.FlatAppearance.BorderSize = 0
        Me.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAdd.Location = New System.Drawing.Point(13, 3)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(45, 45)
        Me.btnAdd.TabIndex = 2
        Me.btnAdd.UseVisualStyleBackColor = False
        '
        'btnAttCam
        '
        Me.btnAttCam.BackColor = System.Drawing.Color.Transparent
        Me.btnAttCam.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddCameraAtta
        Me.btnAttCam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnAttCam.FlatAppearance.BorderSize = 0
        Me.btnAttCam.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAttCam.Location = New System.Drawing.Point(64, 3)
        Me.btnAttCam.Name = "btnAttCam"
        Me.btnAttCam.Size = New System.Drawing.Size(45, 45)
        Me.btnAttCam.TabIndex = 4
        Me.btnAttCam.UseVisualStyleBackColor = False
        '
        'btnAddSketch
        '
        Me.btnAddSketch.BackColor = System.Drawing.Color.Transparent
        Me.btnAddSketch.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddSketchAtt
        Me.btnAddSketch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnAddSketch.FlatAppearance.BorderSize = 0
        Me.btnAddSketch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddSketch.Location = New System.Drawing.Point(115, 4)
        Me.btnAddSketch.Name = "btnAddSketch"
        Me.btnAddSketch.Size = New System.Drawing.Size(45, 45)
        Me.btnAddSketch.TabIndex = 5
        Me.btnAddSketch.UseVisualStyleBackColor = False
        '
        'btnRemove
        '
        Me.btnRemove.BackColor = System.Drawing.Color.Transparent
        Me.btnRemove.BackgroundImage = Global.MobileControls.My.Resources.Resources.DelAttBlue
        Me.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnRemove.FlatAppearance.BorderSize = 0
        Me.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRemove.Location = New System.Drawing.Point(166, 3)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(45, 45)
        Me.btnRemove.TabIndex = 3
        Me.btnRemove.UseVisualStyleBackColor = False
        '
        'btnRemoveLast
        '
        Me.btnRemoveLast.BackColor = System.Drawing.Color.Transparent
        Me.btnRemoveLast.BackgroundImage = Global.MobileControls.My.Resources.Resources.RotateLeftGray
        Me.btnRemoveLast.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnRemoveLast.Enabled = False
        Me.btnRemoveLast.FlatAppearance.BorderSize = 0
        Me.btnRemoveLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRemoveLast.Location = New System.Drawing.Point(3, 102)
        Me.btnRemoveLast.Name = "btnRemoveLast"
        Me.btnRemoveLast.Size = New System.Drawing.Size(45, 45)
        Me.btnRemoveLast.TabIndex = 6
        Me.btnRemoveLast.UseVisualStyleBackColor = False
        '
        'btnClear
        '
        Me.btnClear.BackColor = System.Drawing.Color.Transparent
        Me.btnClear.BackgroundImage = Global.MobileControls.My.Resources.Resources.ClearSketchGray
        Me.btnClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnClear.Enabled = False
        Me.btnClear.FlatAppearance.BorderSize = 0
        Me.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClear.Location = New System.Drawing.Point(3, 3)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(45, 45)
        Me.btnClear.TabIndex = 5
        Me.btnClear.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.Transparent
        Me.btnSave.BackgroundImage = Global.MobileControls.My.Resources.Resources.SaveSketchGray
        Me.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnSave.Enabled = False
        Me.btnSave.FlatAppearance.BorderSize = 0
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.Location = New System.Drawing.Point(3, 51)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(45, 45)
        Me.btnSave.TabIndex = 4
        Me.btnSave.UseVisualStyleBackColor = False
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
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlSketchButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents splitMainAtt As System.Windows.Forms.SplitContainer
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents btnAttCam As System.Windows.Forms.Button
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents lstAtt As System.Windows.Forms.ListBox
    Friend WithEvents btnAddSketch As System.Windows.Forms.Button
    Friend WithEvents pnlSketchPalette As System.Windows.Forms.Panel
    Friend WithEvents pnlSketchButtons As System.Windows.Forms.Panel
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnRemoveLast As System.Windows.Forms.Button

End Class
