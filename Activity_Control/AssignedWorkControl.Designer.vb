<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AssignedWorkControl
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
        Me.components = New System.ComponentModel.Container()
        Me.gpBoxButtons = New System.Windows.Forms.GroupBox()
        Me.btnCrew = New System.Windows.Forms.Button()
        Me.lblCurrentWO = New System.Windows.Forms.Label()
        Me.gpBoxWOList = New System.Windows.Forms.GroupBox()
        Me.gpBoxOptions = New System.Windows.Forms.GroupBox()
        Me.gpBoxWODetails = New System.Windows.Forms.GroupBox()
        Me.gpBoxWOClose = New System.Windows.Forms.GroupBox()
        Me.gpBoxWOCreate = New System.Windows.Forms.GroupBox()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.btnClear = New System.Windows.Forms.CheckBox()
        Me.btnCloseWork = New System.Windows.Forms.CheckBox()
        Me.btnViewWorkDetails = New System.Windows.Forms.CheckBox()
        Me.btnViewAllWork = New System.Windows.Forms.CheckBox()
        Me.btnCreateWork = New System.Windows.Forms.CheckBox()
        Me.gpBoxButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBoxButtons
        '
        Me.gpBoxButtons.Controls.Add(Me.btnCrew)
        Me.gpBoxButtons.Controls.Add(Me.btnClear)
        Me.gpBoxButtons.Controls.Add(Me.lblCurrentWO)
        Me.gpBoxButtons.Controls.Add(Me.btnCloseWork)
        Me.gpBoxButtons.Controls.Add(Me.btnViewWorkDetails)
        Me.gpBoxButtons.Controls.Add(Me.btnViewAllWork)
        Me.gpBoxButtons.Controls.Add(Me.btnCreateWork)
        Me.gpBoxButtons.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxButtons.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxButtons.Name = "gpBoxButtons"
        Me.gpBoxButtons.Size = New System.Drawing.Size(517, 113)
        Me.gpBoxButtons.TabIndex = 0
        Me.gpBoxButtons.TabStop = False
        '
        'btnCrew
        '
        Me.btnCrew.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnCrew.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCrew.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCrew.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.btnCrew.Location = New System.Drawing.Point(3, 16)
        Me.btnCrew.Name = "btnCrew"
        Me.btnCrew.Size = New System.Drawing.Size(511, 31)
        Me.btnCrew.TabIndex = 26
        Me.btnCrew.Text = "Button1"
        Me.btnCrew.UseVisualStyleBackColor = True
        '
        'lblCurrentWO
        '
        Me.lblCurrentWO.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblCurrentWO.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrentWO.Location = New System.Drawing.Point(3, 94)
        Me.lblCurrentWO.Name = "lblCurrentWO"
        Me.lblCurrentWO.Size = New System.Drawing.Size(511, 16)
        Me.lblCurrentWO.TabIndex = 23
        Me.lblCurrentWO.Text = "Current WO"
        Me.lblCurrentWO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'gpBoxWOList
        '
        Me.gpBoxWOList.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxWOList.Location = New System.Drawing.Point(0, 342)
        Me.gpBoxWOList.Name = "gpBoxWOList"
        Me.gpBoxWOList.Size = New System.Drawing.Size(517, 67)
        Me.gpBoxWOList.TabIndex = 1
        Me.gpBoxWOList.TabStop = False
        Me.gpBoxWOList.Text = "Workorder List"
        '
        'gpBoxOptions
        '
        Me.gpBoxOptions.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.gpBoxOptions.Location = New System.Drawing.Point(0, 715)
        Me.gpBoxOptions.Name = "gpBoxOptions"
        Me.gpBoxOptions.Size = New System.Drawing.Size(517, 100)
        Me.gpBoxOptions.TabIndex = 2
        Me.gpBoxOptions.TabStop = False
        Me.gpBoxOptions.Text = "Options"
        Me.gpBoxOptions.Visible = False
        '
        'gpBoxWODetails
        '
        Me.gpBoxWODetails.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxWODetails.Location = New System.Drawing.Point(0, 255)
        Me.gpBoxWODetails.Name = "gpBoxWODetails"
        Me.gpBoxWODetails.Size = New System.Drawing.Size(517, 87)
        Me.gpBoxWODetails.TabIndex = 3
        Me.gpBoxWODetails.TabStop = False
        Me.gpBoxWODetails.Text = "Workorder Details"
        '
        'gpBoxWOClose
        '
        Me.gpBoxWOClose.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxWOClose.Location = New System.Drawing.Point(0, 191)
        Me.gpBoxWOClose.Name = "gpBoxWOClose"
        Me.gpBoxWOClose.Size = New System.Drawing.Size(517, 64)
        Me.gpBoxWOClose.TabIndex = 4
        Me.gpBoxWOClose.TabStop = False
        Me.gpBoxWOClose.Text = "Close Workorders"
        '
        'gpBoxWOCreate
        '
        Me.gpBoxWOCreate.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxWOCreate.Location = New System.Drawing.Point(0, 113)
        Me.gpBoxWOCreate.Name = "gpBoxWOCreate"
        Me.gpBoxWOCreate.Size = New System.Drawing.Size(517, 78)
        Me.gpBoxWOCreate.TabIndex = 5
        Me.gpBoxWOCreate.TabStop = False
        Me.gpBoxWOCreate.Text = "Create Workorder"
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageList1.ImageSize = New System.Drawing.Size(4, 40)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'btnClear
        '
        Me.btnClear.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnClear.AutoCheck = False
        Me.btnClear.BackgroundImage = Global.MobileControls.My.Resources.Resources.ClearGreen
        Me.btnClear.FlatAppearance.BorderSize = 0
        Me.btnClear.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent
        Me.btnClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClear.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClear.Location = New System.Drawing.Point(238, 44)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(40, 40)
        Me.btnClear.TabIndex = 25
        Me.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnCloseWork
        '
        Me.btnCloseWork.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnCloseWork.AutoCheck = False
        Me.btnCloseWork.BackgroundImage = Global.MobileControls.My.Resources.Resources.CheckBlue
        Me.btnCloseWork.FlatAppearance.BorderSize = 0
        Me.btnCloseWork.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent
        Me.btnCloseWork.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.btnCloseWork.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.btnCloseWork.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCloseWork.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCloseWork.Location = New System.Drawing.Point(129, 43)
        Me.btnCloseWork.Name = "btnCloseWork"
        Me.btnCloseWork.Size = New System.Drawing.Size(40, 40)
        Me.btnCloseWork.TabIndex = 22
        Me.btnCloseWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnCloseWork.UseVisualStyleBackColor = True
        '
        'btnViewWorkDetails
        '
        Me.btnViewWorkDetails.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnViewWorkDetails.AutoCheck = False
        Me.btnViewWorkDetails.BackgroundImage = Global.MobileControls.My.Resources.Resources.DocBlue
        Me.btnViewWorkDetails.FlatAppearance.BorderSize = 0
        Me.btnViewWorkDetails.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent
        Me.btnViewWorkDetails.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.btnViewWorkDetails.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.btnViewWorkDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnViewWorkDetails.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnViewWorkDetails.Location = New System.Drawing.Point(71, 43)
        Me.btnViewWorkDetails.Name = "btnViewWorkDetails"
        Me.btnViewWorkDetails.Size = New System.Drawing.Size(40, 40)
        Me.btnViewWorkDetails.TabIndex = 21
        Me.btnViewWorkDetails.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnViewWorkDetails.UseVisualStyleBackColor = True
        '
        'btnViewAllWork
        '
        Me.btnViewAllWork.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnViewAllWork.AutoCheck = False
        Me.btnViewAllWork.BackColor = System.Drawing.Color.Transparent
        Me.btnViewAllWork.BackgroundImage = Global.MobileControls.My.Resources.Resources.ClipboardBlue
        Me.btnViewAllWork.Checked = True
        Me.btnViewAllWork.CheckState = System.Windows.Forms.CheckState.Checked
        Me.btnViewAllWork.FlatAppearance.BorderSize = 0
        Me.btnViewAllWork.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent
        Me.btnViewAllWork.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.btnViewAllWork.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.btnViewAllWork.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnViewAllWork.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnViewAllWork.Location = New System.Drawing.Point(15, 43)
        Me.btnViewAllWork.Name = "btnViewAllWork"
        Me.btnViewAllWork.Size = New System.Drawing.Size(40, 40)
        Me.btnViewAllWork.TabIndex = 20
        Me.btnViewAllWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnViewAllWork.UseVisualStyleBackColor = False
        '
        'btnCreateWork
        '
        Me.btnCreateWork.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnCreateWork.AutoCheck = False
        Me.btnCreateWork.BackgroundImage = Global.MobileControls.My.Resources.Resources.EmergBlue
        Me.btnCreateWork.FlatAppearance.BorderSize = 0
        Me.btnCreateWork.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent
        Me.btnCreateWork.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent
        Me.btnCreateWork.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent
        Me.btnCreateWork.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCreateWork.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateWork.Location = New System.Drawing.Point(188, 43)
        Me.btnCreateWork.Name = "btnCreateWork"
        Me.btnCreateWork.Size = New System.Drawing.Size(40, 40)
        Me.btnCreateWork.TabIndex = 19
        Me.btnCreateWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnCreateWork.UseVisualStyleBackColor = True
        Me.btnCreateWork.Visible = False
        '
        'AssignedWorkControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gpBoxWOList)
        Me.Controls.Add(Me.gpBoxOptions)
        Me.Controls.Add(Me.gpBoxWODetails)
        Me.Controls.Add(Me.gpBoxWOClose)
        Me.Controls.Add(Me.gpBoxWOCreate)
        Me.Controls.Add(Me.gpBoxButtons)
        Me.Name = "AssignedWorkControl"
        Me.Size = New System.Drawing.Size(517, 815)
        Me.gpBoxButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBoxButtons As System.Windows.Forms.GroupBox
    Friend WithEvents btnCloseWork As System.Windows.Forms.CheckBox
    Friend WithEvents btnViewWorkDetails As System.Windows.Forms.CheckBox
    Friend WithEvents btnViewAllWork As System.Windows.Forms.CheckBox
    Friend WithEvents btnCreateWork As System.Windows.Forms.CheckBox
    Friend WithEvents gpBoxWOList As System.Windows.Forms.GroupBox
    Friend WithEvents gpBoxOptions As System.Windows.Forms.GroupBox
    Friend WithEvents gpBoxWODetails As System.Windows.Forms.GroupBox
    Friend WithEvents gpBoxWOClose As System.Windows.Forms.GroupBox
    Friend WithEvents gpBoxWOCreate As System.Windows.Forms.GroupBox
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents btnClear As System.Windows.Forms.CheckBox
    Friend WithEvents btnCrew As System.Windows.Forms.Button
    Friend WithEvents lblCurrentWO As System.Windows.Forms.Label

End Class
