<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BrowseDisplay
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
        Me.gpBoxControls = New System.Windows.Forms.GroupBox()
        Me.lblCurrentDoc = New System.Windows.Forms.Label()
        Me.pnlControls = New System.Windows.Forms.Panel()
        Me.btnZoomOut = New System.Windows.Forms.Button()
        Me.btnZoomIn = New System.Windows.Forms.Button()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.btnForward = New System.Windows.Forms.Button()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.webDisplay = New ZoomBrowser.MyBrowser()
        Me.gpBoxControls.SuspendLayout()
        Me.pnlControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'gpBoxControls
        '
        Me.gpBoxControls.Controls.Add(Me.lblCurrentDoc)
        Me.gpBoxControls.Controls.Add(Me.pnlControls)
        Me.gpBoxControls.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxControls.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxControls.Name = "gpBoxControls"
        Me.gpBoxControls.Size = New System.Drawing.Size(522, 68)
        Me.gpBoxControls.TabIndex = 0
        Me.gpBoxControls.TabStop = False
        '
        'lblCurrentDoc
        '
        Me.lblCurrentDoc.BackColor = System.Drawing.SystemColors.Control
        Me.lblCurrentDoc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCurrentDoc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblCurrentDoc.Font = New System.Drawing.Font("Verdana", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrentDoc.ForeColor = System.Drawing.Color.Crimson
        Me.lblCurrentDoc.Location = New System.Drawing.Point(3, 16)
        Me.lblCurrentDoc.Name = "lblCurrentDoc"
        Me.lblCurrentDoc.Size = New System.Drawing.Size(227, 49)
        Me.lblCurrentDoc.TabIndex = 3
        Me.lblCurrentDoc.Text = ""
        '
        'pnlControls
        '
        Me.pnlControls.Controls.Add(Me.btnZoomOut)
        Me.pnlControls.Controls.Add(Me.btnZoomIn)
        Me.pnlControls.Controls.Add(Me.btnRefresh)
        Me.pnlControls.Controls.Add(Me.btnForward)
        Me.pnlControls.Controls.Add(Me.btnBack)
        Me.pnlControls.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlControls.Location = New System.Drawing.Point(230, 16)
        Me.pnlControls.Name = "pnlControls"
        Me.pnlControls.Size = New System.Drawing.Size(289, 49)
        Me.pnlControls.TabIndex = 4
        '
        'btnZoomOut
        '
        Me.btnZoomOut.FlatAppearance.BorderSize = 0
        Me.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnZoomOut.Image = Global.MobileControls.My.Resources.Resources.ZoomOut
        Me.btnZoomOut.Location = New System.Drawing.Point(64, -1)
        Me.btnZoomOut.Name = "btnZoomOut"
        Me.btnZoomOut.Size = New System.Drawing.Size(50, 50)
        Me.btnZoomOut.TabIndex = 8
        Me.btnZoomOut.UseVisualStyleBackColor = True
        '
        'btnZoomIn
        '
        Me.btnZoomIn.FlatAppearance.BorderSize = 0
        Me.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnZoomIn.Image = Global.MobileControls.My.Resources.Resources.ZoomIn
        Me.btnZoomIn.Location = New System.Drawing.Point(176, 0)
        Me.btnZoomIn.Name = "btnZoomIn"
        Me.btnZoomIn.Size = New System.Drawing.Size(50, 50)
        Me.btnZoomIn.TabIndex = 7
        Me.btnZoomIn.UseVisualStyleBackColor = True
        '
        'btnRefresh
        '
        Me.btnRefresh.FlatAppearance.BorderSize = 0
        Me.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRefresh.Image = Global.MobileControls.My.Resources.Resources.refresh
        Me.btnRefresh.Location = New System.Drawing.Point(120, 0)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(50, 50)
        Me.btnRefresh.TabIndex = 5
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'btnForward
        '
        Me.btnForward.FlatAppearance.BorderSize = 0
        Me.btnForward.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnForward.Image = Global.MobileControls.My.Resources.Resources.rightArrow
        Me.btnForward.Location = New System.Drawing.Point(232, -1)
        Me.btnForward.Name = "btnForward"
        Me.btnForward.Size = New System.Drawing.Size(50, 50)
        Me.btnForward.TabIndex = 4
        Me.btnForward.UseVisualStyleBackColor = True
        '
        'btnBack
        '
        Me.btnBack.FlatAppearance.BorderSize = 0
        Me.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBack.Image = Global.MobileControls.My.Resources.Resources.LeftArrow
        Me.btnBack.Location = New System.Drawing.Point(8, -1)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(50, 50)
        Me.btnBack.TabIndex = 3
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.Highlight
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel1.Location = New System.Drawing.Point(0, 68)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(10, 418)
        Me.Panel1.TabIndex = 1
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.SystemColors.Highlight
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(10, 476)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(512, 10)
        Me.Panel2.TabIndex = 2
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.SystemColors.Highlight
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel3.Location = New System.Drawing.Point(512, 68)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(10, 408)
        Me.Panel3.TabIndex = 3
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.SystemColors.Highlight
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel4.Location = New System.Drawing.Point(10, 68)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(502, 10)
        Me.Panel4.TabIndex = 5
        '
        'webDisplay
        '
        Me.webDisplay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.webDisplay.Location = New System.Drawing.Point(10, 78)
        Me.webDisplay.MinimumSize = New System.Drawing.Size(20, 20)
        Me.webDisplay.Name = "webDisplay"
        Me.webDisplay.ScriptErrorsSuppressed = True
        Me.webDisplay.Size = New System.Drawing.Size(502, 398)
        Me.webDisplay.TabIndex = 6
        '
        'BrowseDisplay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.webDisplay)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.gpBoxControls)
        Me.Name = "BrowseDisplay"
        Me.Size = New System.Drawing.Size(522, 486)
        Me.gpBoxControls.ResumeLayout(False)
        Me.pnlControls.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents gpBoxControls As System.Windows.Forms.GroupBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents webDisplay As ZoomBrowser.MyBrowser
    Friend WithEvents lblCurrentDoc As System.Windows.Forms.Label
    Friend WithEvents pnlControls As System.Windows.Forms.Panel
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents btnForward As System.Windows.Forms.Button
    Friend WithEvents btnBack As System.Windows.Forms.Button
    Friend WithEvents btnZoomOut As System.Windows.Forms.Button
    Friend WithEvents btnZoomIn As System.Windows.Forms.Button

End Class
