<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Compass
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.picGPS = New System.Windows.Forms.PictureBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblCourse = New System.Windows.Forms.Label()
        Me.lblSpeed = New System.Windows.Forms.Label()
        Me.lblFix = New System.Windows.Forms.Label()
        Me.lblNumSat = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtCurY = New System.Windows.Forms.TextBox()
        Me.txtCurX = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lblTurnIn = New System.Windows.Forms.Label()
        Me.lblArriveIn = New System.Windows.Forms.Label()
        Me.lblArriveInHeader = New System.Windows.Forms.Label()
        Me.lblBearing = New System.Windows.Forms.Label()
        Me.lblBearingHeader = New System.Windows.Forms.Label()
        Me.lblDistance = New System.Windows.Forms.Label()
        Me.lblDistHeader = New System.Windows.Forms.Label()
        Me.lblNavHeader = New System.Windows.Forms.Label()
        Me.lblCurrLocHeader = New System.Windows.Forms.Label()
        Me.txtLongY = New System.Windows.Forms.TextBox()
        Me.txtLongX = New System.Windows.Forms.TextBox()
        Me.lblDestHeader = New System.Windows.Forms.Label()
        Me.gpBoxControls = New System.Windows.Forms.GroupBox()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.picGPS, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.gpBoxControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.picGPS)
        Me.SplitContainer1.Panel1MinSize = 50
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.GroupBox1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.gpBoxControls)
        Me.SplitContainer1.Size = New System.Drawing.Size(600, 656)
        Me.SplitContainer1.SplitterDistance = 250
        Me.SplitContainer1.TabIndex = 0
        '
        'picGPS
        '
        Me.picGPS.BackgroundImage = Global.MobileControls.My.Resources.Resources.Compass
        Me.picGPS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.picGPS.Dock = System.Windows.Forms.DockStyle.Fill
        Me.picGPS.Location = New System.Drawing.Point(0, 0)
        Me.picGPS.Name = "picGPS"
        Me.picGPS.Size = New System.Drawing.Size(600, 250)
        Me.picGPS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picGPS.TabIndex = 0
        Me.picGPS.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblCourse)
        Me.GroupBox1.Controls.Add(Me.lblSpeed)
        Me.GroupBox1.Controls.Add(Me.lblFix)
        Me.GroupBox1.Controls.Add(Me.lblNumSat)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.txtCurY)
        Me.GroupBox1.Controls.Add(Me.txtCurX)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.lblTurnIn)
        Me.GroupBox1.Controls.Add(Me.lblArriveIn)
        Me.GroupBox1.Controls.Add(Me.lblArriveInHeader)
        Me.GroupBox1.Controls.Add(Me.lblBearing)
        Me.GroupBox1.Controls.Add(Me.lblBearingHeader)
        Me.GroupBox1.Controls.Add(Me.lblDistance)
        Me.GroupBox1.Controls.Add(Me.lblDistHeader)
        Me.GroupBox1.Controls.Add(Me.lblNavHeader)
        Me.GroupBox1.Controls.Add(Me.lblCurrLocHeader)
        Me.GroupBox1.Controls.Add(Me.txtLongY)
        Me.GroupBox1.Controls.Add(Me.txtLongX)
        Me.GroupBox1.Controls.Add(Me.lblDestHeader)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(600, 343)
        Me.GroupBox1.TabIndex = 27
        Me.GroupBox1.TabStop = False
        '
        'lblCourse
        '
        Me.lblCourse.AutoSize = True
        Me.lblCourse.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCourse.Location = New System.Drawing.Point(206, 75)
        Me.lblCourse.Name = "lblCourse"
        Me.lblCourse.Size = New System.Drawing.Size(31, 16)
        Me.lblCourse.TabIndex = 38
        Me.lblCourse.Text = "N/A"
        '
        'lblSpeed
        '
        Me.lblSpeed.AutoSize = True
        Me.lblSpeed.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSpeed.Location = New System.Drawing.Point(142, 75)
        Me.lblSpeed.Name = "lblSpeed"
        Me.lblSpeed.Size = New System.Drawing.Size(31, 16)
        Me.lblSpeed.TabIndex = 37
        Me.lblSpeed.Text = "N/A"
        '
        'lblFix
        '
        Me.lblFix.AutoSize = True
        Me.lblFix.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFix.Location = New System.Drawing.Point(73, 75)
        Me.lblFix.Name = "lblFix"
        Me.lblFix.Size = New System.Drawing.Size(31, 16)
        Me.lblFix.TabIndex = 36
        Me.lblFix.Text = "N/A"
        '
        'lblNumSat
        '
        Me.lblNumSat.AutoSize = True
        Me.lblNumSat.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNumSat.Location = New System.Drawing.Point(18, 75)
        Me.lblNumSat.Name = "lblNumSat"
        Me.lblNumSat.Size = New System.Drawing.Size(31, 16)
        Me.lblNumSat.TabIndex = 35
        Me.lblNumSat.Text = "N/A"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(204, 55)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(51, 16)
        Me.Label4.TabIndex = 34
        Me.Label4.Text = "Course"
        '
        'txtCurY
        '
        Me.txtCurY.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCurY.Location = New System.Drawing.Point(130, 30)
        Me.txtCurY.Name = "txtCurY"
        Me.txtCurY.ReadOnly = True
        Me.txtCurY.Size = New System.Drawing.Size(103, 22)
        Me.txtCurY.TabIndex = 33
        Me.txtCurY.Text = "N/A"
        '
        'txtCurX
        '
        Me.txtCurX.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCurX.Location = New System.Drawing.Point(16, 30)
        Me.txtCurX.Name = "txtCurX"
        Me.txtCurX.ReadOnly = True
        Me.txtCurX.Size = New System.Drawing.Size(103, 22)
        Me.txtCurX.TabIndex = 32
        Me.txtCurX.Text = "N/A"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(140, 54)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 16)
        Me.Label1.TabIndex = 31
        Me.Label1.Text = "Speed"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(70, 54)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(60, 16)
        Me.Label2.TabIndex = 30
        Me.Label2.Text = "Fix Type"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(13, 54)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(45, 16)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "# Sats"
        '
        'lblTurnIn
        '
        Me.lblTurnIn.AutoSize = True
        Me.lblTurnIn.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblTurnIn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTurnIn.ForeColor = System.Drawing.Color.Red
        Me.lblTurnIn.Location = New System.Drawing.Point(15, 208)
        Me.lblTurnIn.Name = "lblTurnIn"
        Me.lblTurnIn.Padding = New System.Windows.Forms.Padding(0, 0, 0, 5)
        Me.lblTurnIn.Size = New System.Drawing.Size(35, 25)
        Me.lblTurnIn.TabIndex = 28
        Me.lblTurnIn.Text = "N/A"
        Me.lblTurnIn.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblArriveIn
        '
        Me.lblArriveIn.AutoSize = True
        Me.lblArriveIn.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblArriveIn.Location = New System.Drawing.Point(182, 181)
        Me.lblArriveIn.Name = "lblArriveIn"
        Me.lblArriveIn.Size = New System.Drawing.Size(31, 16)
        Me.lblArriveIn.TabIndex = 27
        Me.lblArriveIn.Text = "N/A"
        '
        'lblArriveInHeader
        '
        Me.lblArriveInHeader.AutoSize = True
        Me.lblArriveInHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblArriveInHeader.Location = New System.Drawing.Point(179, 163)
        Me.lblArriveInHeader.Name = "lblArriveInHeader"
        Me.lblArriveInHeader.Size = New System.Drawing.Size(56, 16)
        Me.lblArriveInHeader.TabIndex = 26
        Me.lblArriveInHeader.Text = "Arrive in"
        '
        'lblBearing
        '
        Me.lblBearing.AutoSize = True
        Me.lblBearing.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBearing.Location = New System.Drawing.Point(107, 181)
        Me.lblBearing.Name = "lblBearing"
        Me.lblBearing.Size = New System.Drawing.Size(31, 16)
        Me.lblBearing.TabIndex = 25
        Me.lblBearing.Text = "N/A"
        '
        'lblBearingHeader
        '
        Me.lblBearingHeader.AutoSize = True
        Me.lblBearingHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBearingHeader.Location = New System.Drawing.Point(104, 163)
        Me.lblBearingHeader.Name = "lblBearingHeader"
        Me.lblBearingHeader.Size = New System.Drawing.Size(55, 16)
        Me.lblBearingHeader.TabIndex = 24
        Me.lblBearingHeader.Text = "Bearing"
        '
        'lblDistance
        '
        Me.lblDistance.AutoSize = True
        Me.lblDistance.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDistance.Location = New System.Drawing.Point(16, 181)
        Me.lblDistance.Name = "lblDistance"
        Me.lblDistance.Size = New System.Drawing.Size(31, 16)
        Me.lblDistance.TabIndex = 23
        Me.lblDistance.Text = "N/A"
        '
        'lblDistHeader
        '
        Me.lblDistHeader.AutoSize = True
        Me.lblDistHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDistHeader.Location = New System.Drawing.Point(13, 163)
        Me.lblDistHeader.Name = "lblDistHeader"
        Me.lblDistHeader.Size = New System.Drawing.Size(61, 16)
        Me.lblDistHeader.TabIndex = 22
        Me.lblDistHeader.Text = "Distance"
        '
        'lblNavHeader
        '
        Me.lblNavHeader.AutoSize = True
        Me.lblNavHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNavHeader.Location = New System.Drawing.Point(7, 145)
        Me.lblNavHeader.Name = "lblNavHeader"
        Me.lblNavHeader.Size = New System.Drawing.Size(83, 16)
        Me.lblNavHeader.TabIndex = 21
        Me.lblNavHeader.Text = "Navigation"
        '
        'lblCurrLocHeader
        '
        Me.lblCurrLocHeader.AutoSize = True
        Me.lblCurrLocHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrLocHeader.Location = New System.Drawing.Point(4, 14)
        Me.lblCurrLocHeader.Name = "lblCurrLocHeader"
        Me.lblCurrLocHeader.Size = New System.Drawing.Size(123, 16)
        Me.lblCurrLocHeader.TabIndex = 19
        Me.lblCurrLocHeader.Text = "GPS Information:"
        '
        'txtLongY
        '
        Me.txtLongY.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLongY.Location = New System.Drawing.Point(127, 114)
        Me.txtLongY.Name = "txtLongY"
        Me.txtLongY.ReadOnly = True
        Me.txtLongY.Size = New System.Drawing.Size(103, 22)
        Me.txtLongY.TabIndex = 18
        Me.txtLongY.Text = "N/A"
        '
        'txtLongX
        '
        Me.txtLongX.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtLongX.Location = New System.Drawing.Point(13, 114)
        Me.txtLongX.Name = "txtLongX"
        Me.txtLongX.ReadOnly = True
        Me.txtLongX.Size = New System.Drawing.Size(103, 22)
        Me.txtLongX.TabIndex = 17
        Me.txtLongX.Text = "N/A"
        '
        'lblDestHeader
        '
        Me.lblDestHeader.AutoSize = True
        Me.lblDestHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDestHeader.Location = New System.Drawing.Point(7, 96)
        Me.lblDestHeader.Name = "lblDestHeader"
        Me.lblDestHeader.Size = New System.Drawing.Size(90, 16)
        Me.lblDestHeader.TabIndex = 16
        Me.lblDestHeader.Text = "Destination:"
        '
        'gpBoxControls
        '
        Me.gpBoxControls.Controls.Add(Me.btnClear)
        Me.gpBoxControls.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.gpBoxControls.Location = New System.Drawing.Point(0, 343)
        Me.gpBoxControls.Name = "gpBoxControls"
        Me.gpBoxControls.Size = New System.Drawing.Size(600, 59)
        Me.gpBoxControls.TabIndex = 28
        Me.gpBoxControls.TabStop = False
        '
        'btnClear
        '
        Me.btnClear.BackgroundImage = Global.MobileControls.My.Resources.Resources.ClearGreen
        Me.btnClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnClear.FlatAppearance.BorderSize = 0
        Me.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClear.Location = New System.Drawing.Point(11, 13)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(40, 40)
        Me.btnClear.TabIndex = 28
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'Compass
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "Compass"
        Me.Size = New System.Drawing.Size(600, 656)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.picGPS, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.gpBoxControls.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents picGPS As System.Windows.Forms.PictureBox
    Friend WithEvents gpBoxControls As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblTurnIn As System.Windows.Forms.Label
    Friend WithEvents lblArriveIn As System.Windows.Forms.Label
    Friend WithEvents lblArriveInHeader As System.Windows.Forms.Label
    Friend WithEvents lblBearing As System.Windows.Forms.Label
    Friend WithEvents lblBearingHeader As System.Windows.Forms.Label
    Friend WithEvents lblDistance As System.Windows.Forms.Label
    Friend WithEvents lblDistHeader As System.Windows.Forms.Label
    Friend WithEvents lblNavHeader As System.Windows.Forms.Label
    Friend WithEvents lblCurrLocHeader As System.Windows.Forms.Label
    Friend WithEvents txtLongY As System.Windows.Forms.TextBox
    Friend WithEvents txtLongX As System.Windows.Forms.TextBox
    Friend WithEvents lblDestHeader As System.Windows.Forms.Label
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents txtCurY As System.Windows.Forms.TextBox
    Friend WithEvents txtCurX As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblCourse As System.Windows.Forms.Label
    Friend WithEvents lblSpeed As System.Windows.Forms.Label
    Friend WithEvents lblFix As System.Windows.Forms.Label
    Friend WithEvents lblNumSat As System.Windows.Forms.Label

End Class
