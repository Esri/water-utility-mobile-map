<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()

                'm_FDR.Delete()
                m_FDR = Nothing

                m_GPSVal = Nothing

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditControl))
        Me.spCntMain = New System.Windows.Forms.SplitContainer()
        Me.lstBoxError = New System.Windows.Forms.ListBox()
        Me.spltContEdit = New System.Windows.Forms.SplitContainer()
        Me.tbCntrlEdit = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.pnlEditBtns = New System.Windows.Forms.Panel()
        Me.btnGPSLoc = New System.Windows.Forms.Button()
        Me.btnMove = New System.Windows.Forms.CheckBox()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.spCntMain.Panel1.SuspendLayout()
        Me.spCntMain.Panel2.SuspendLayout()
        Me.spCntMain.SuspendLayout()
        Me.spltContEdit.Panel1.SuspendLayout()
        Me.spltContEdit.Panel2.SuspendLayout()
        Me.spltContEdit.SuspendLayout()
        Me.tbCntrlEdit.SuspendLayout()
        Me.pnlEditBtns.SuspendLayout()
        Me.SuspendLayout()
        '
        'spCntMain
        '
        Me.spCntMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spCntMain.Location = New System.Drawing.Point(0, 0)
        Me.spCntMain.Name = "spCntMain"
        Me.spCntMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'spCntMain.Panel1
        '
        Me.spCntMain.Panel1.Controls.Add(Me.lstBoxError)
        '
        'spCntMain.Panel2
        '
        Me.spCntMain.Panel2.Controls.Add(Me.spltContEdit)
        Me.spCntMain.Size = New System.Drawing.Size(349, 634)
        Me.spCntMain.SplitterDistance = 40
        Me.spCntMain.SplitterWidth = 1
        Me.spCntMain.TabIndex = 0
        '
        'lstBoxError
        '
        Me.lstBoxError.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstBoxError.FormattingEnabled = True
        Me.lstBoxError.HorizontalScrollbar = True
        Me.lstBoxError.Location = New System.Drawing.Point(0, 0)
        Me.lstBoxError.Name = "lstBoxError"
        Me.lstBoxError.Size = New System.Drawing.Size(349, 40)
        Me.lstBoxError.TabIndex = 0
        '
        'spltContEdit
        '
        Me.spltContEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spltContEdit.IsSplitterFixed = True
        Me.spltContEdit.Location = New System.Drawing.Point(0, 0)
        Me.spltContEdit.Name = "spltContEdit"
        Me.spltContEdit.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'spltContEdit.Panel1
        '
        Me.spltContEdit.Panel1.Controls.Add(Me.tbCntrlEdit)
        '
        'spltContEdit.Panel2
        '
        Me.spltContEdit.Panel2.Controls.Add(Me.pnlEditBtns)
        Me.spltContEdit.Size = New System.Drawing.Size(349, 593)
        Me.spltContEdit.SplitterDistance = 186
        Me.spltContEdit.SplitterWidth = 1
        Me.spltContEdit.TabIndex = 1
        '
        'tbCntrlEdit
        '
        Me.tbCntrlEdit.Controls.Add(Me.TabPage1)
        Me.tbCntrlEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbCntrlEdit.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbCntrlEdit.Location = New System.Drawing.Point(0, 0)
        Me.tbCntrlEdit.Name = "tbCntrlEdit"
        Me.tbCntrlEdit.SelectedIndex = 0
        Me.tbCntrlEdit.Size = New System.Drawing.Size(349, 186)
        Me.tbCntrlEdit.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(341, 157)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'pnlEditBtns
        '
        Me.pnlEditBtns.Controls.Add(Me.btnGPSLoc)
        Me.pnlEditBtns.Controls.Add(Me.btnMove)
        Me.pnlEditBtns.Controls.Add(Me.btnClear)
        Me.pnlEditBtns.Controls.Add(Me.btnSave)
        Me.pnlEditBtns.Controls.Add(Me.btnDelete)
        Me.pnlEditBtns.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlEditBtns.Location = New System.Drawing.Point(0, 0)
        Me.pnlEditBtns.Name = "pnlEditBtns"
        Me.pnlEditBtns.Size = New System.Drawing.Size(349, 58)
        Me.pnlEditBtns.TabIndex = 4
        '
        'btnGPSLoc
        '
        Me.btnGPSLoc.BackColor = System.Drawing.Color.Transparent
        Me.btnGPSLoc.BackgroundImage = Global.MobileControls.My.Resources.Resources.SatGreen
        Me.btnGPSLoc.FlatAppearance.BorderSize = 0
        Me.btnGPSLoc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.btnGPSLoc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.btnGPSLoc.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnGPSLoc.ForeColor = System.Drawing.Color.Transparent
        Me.btnGPSLoc.Location = New System.Drawing.Point(205, 3)
        Me.btnGPSLoc.Name = "btnGPSLoc"
        Me.btnGPSLoc.Size = New System.Drawing.Size(50, 50)
        Me.btnGPSLoc.TabIndex = 6
        Me.btnGPSLoc.UseVisualStyleBackColor = False
        Me.btnGPSLoc.Visible = False
        '
        'btnMove
        '
        Me.btnMove.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnMove.BackColor = System.Drawing.Color.Transparent
        Me.btnMove.BackgroundImage = Global.MobileControls.My.Resources.Resources.GrayMove
        Me.btnMove.Enabled = False
        Me.btnMove.FlatAppearance.BorderSize = 0
        Me.btnMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnMove.Location = New System.Drawing.Point(98, 3)
        Me.btnMove.Name = "btnMove"
        Me.btnMove.Size = New System.Drawing.Size(50, 50)
        Me.btnMove.TabIndex = 5
        Me.btnMove.UseVisualStyleBackColor = False
        '
        'btnClear
        '
        Me.btnClear.BackColor = System.Drawing.Color.Transparent
        Me.btnClear.BackgroundImage = Global.MobileControls.My.Resources.Resources.ClearGreen
        Me.btnClear.FlatAppearance.BorderSize = 0
        Me.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClear.Location = New System.Drawing.Point(269, 3)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(50, 50)
        Me.btnClear.TabIndex = 4
        Me.btnClear.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.BackColor = System.Drawing.Color.Transparent
        Me.btnSave.BackgroundImage = CType(resources.GetObject("btnSave.BackgroundImage"), System.Drawing.Image)
        Me.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnSave.FlatAppearance.BorderSize = 0
        Me.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSave.Location = New System.Drawing.Point(22, 3)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(50, 50)
        Me.btnSave.TabIndex = 3
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.Transparent
        Me.btnDelete.BackgroundImage = Global.MobileControls.My.Resources.Resources.DeleteGreen
        Me.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnDelete.FlatAppearance.BorderSize = 0
        Me.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDelete.Location = New System.Drawing.Point(149, 4)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(50, 50)
        Me.btnDelete.TabIndex = 7
        Me.btnDelete.UseVisualStyleBackColor = False
        Me.btnDelete.Visible = False
        '
        'EditControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.spCntMain)
        Me.Name = "EditControl"
        Me.Size = New System.Drawing.Size(349, 634)
        Me.spCntMain.Panel1.ResumeLayout(False)
        Me.spCntMain.Panel2.ResumeLayout(False)
        Me.spCntMain.ResumeLayout(False)
        Me.spltContEdit.Panel1.ResumeLayout(False)
        Me.spltContEdit.Panel2.ResumeLayout(False)
        Me.spltContEdit.ResumeLayout(False)
        Me.tbCntrlEdit.ResumeLayout(False)
        Me.pnlEditBtns.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents spCntMain As System.Windows.Forms.SplitContainer
    Friend WithEvents spltContEdit As System.Windows.Forms.SplitContainer
    Friend WithEvents tbCntrlEdit As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents pnlEditBtns As System.Windows.Forms.Panel
    Friend WithEvents btnMove As System.Windows.Forms.CheckBox
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Friend WithEvents btnGPSLoc As System.Windows.Forms.Button
    Friend WithEvents lstBoxError As System.Windows.Forms.ListBox
    Friend WithEvents btnDelete As System.Windows.Forms.Button
End Class
