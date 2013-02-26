<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MobileMapConsole
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.spContMain = New System.Windows.Forms.SplitContainer()
        Me.spContControls = New System.Windows.Forms.SplitContainer()
        Me.tblLayoutWidgets = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlGeonetTrace = New System.Windows.Forms.Panel()
        Me.pnlRouting = New System.Windows.Forms.Panel()
        Me.pnlInspect = New System.Windows.Forms.Panel()
        Me.pnlCreateFeature = New System.Windows.Forms.Panel()
        Me.pnlSketch = New System.Windows.Forms.Panel()
        Me.pnlWO = New System.Windows.Forms.Panel()
        Me.pnlID = New System.Windows.Forms.Panel()
        Me.pnlTrace = New System.Windows.Forms.Panel()
        Me.pnlSearch = New System.Windows.Forms.Panel()
        Me.pnlSync = New System.Windows.Forms.Panel()
        Me.pnlToc = New System.Windows.Forms.Panel()
        Me.pnlWeb = New System.Windows.Forms.Panel()
        Me.btnTakeImage = New System.Windows.Forms.Button()
        Me.btnOpenClosePanel = New System.Windows.Forms.Button()
        Me.Map1 = New Esri.ArcGIS.Mobile.WinForms.Map()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.statLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.statTool = New System.Windows.Forms.ToolStripStatusLabel()
        Me.gpBxSideBar = New System.Windows.Forms.GroupBox()
        Me.btnToggleScreens = New System.Windows.Forms.Button()
        Me.btnToggleDisplay = New System.Windows.Forms.Button()
        Me.pnlWaypoint = New System.Windows.Forms.Panel()
        Me.spContMain.Panel1.SuspendLayout()
        Me.spContMain.Panel2.SuspendLayout()
        Me.spContMain.SuspendLayout()
        Me.spContControls.Panel1.SuspendLayout()
        Me.spContControls.Panel2.SuspendLayout()
        Me.spContControls.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.gpBxSideBar.SuspendLayout()
        Me.SuspendLayout()
        '
        'spContMain
        '
        Me.spContMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spContMain.IsSplitterFixed = True
        Me.spContMain.Location = New System.Drawing.Point(41, 0)
        Me.spContMain.Name = "spContMain"
        '
        'spContMain.Panel1
        '
        Me.spContMain.Panel1.Controls.Add(Me.spContControls)
        Me.spContMain.Panel1.Margin = New System.Windows.Forms.Padding(3)
        Me.spContMain.Panel1.Padding = New System.Windows.Forms.Padding(5)
        '
        'spContMain.Panel2
        '
        Me.spContMain.Panel2.Controls.Add(Me.btnTakeImage)
        Me.spContMain.Panel2.Controls.Add(Me.btnOpenClosePanel)
        Me.spContMain.Panel2.Controls.Add(Me.Map1)
        Me.spContMain.Size = New System.Drawing.Size(881, 712)
        Me.spContMain.SplitterDistance = 299
        Me.spContMain.TabIndex = 0
        '
        'spContControls
        '
        Me.spContControls.Dock = System.Windows.Forms.DockStyle.Fill
        Me.spContControls.IsSplitterFixed = True
        Me.spContControls.Location = New System.Drawing.Point(5, 5)
        Me.spContControls.Name = "spContControls"
        Me.spContControls.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'spContControls.Panel1
        '
        Me.spContControls.Panel1.Controls.Add(Me.tblLayoutWidgets)
        Me.spContControls.Panel1MinSize = 75
        '
        'spContControls.Panel2
        '
        Me.spContControls.Panel2.Controls.Add(Me.pnlWaypoint)
        Me.spContControls.Panel2.Controls.Add(Me.pnlGeonetTrace)
        Me.spContControls.Panel2.Controls.Add(Me.pnlRouting)
        Me.spContControls.Panel2.Controls.Add(Me.pnlInspect)
        Me.spContControls.Panel2.Controls.Add(Me.pnlCreateFeature)
        Me.spContControls.Panel2.Controls.Add(Me.pnlSketch)
        Me.spContControls.Panel2.Controls.Add(Me.pnlWO)
        Me.spContControls.Panel2.Controls.Add(Me.pnlID)
        Me.spContControls.Panel2.Controls.Add(Me.pnlTrace)
        Me.spContControls.Panel2.Controls.Add(Me.pnlSearch)
        Me.spContControls.Panel2.Controls.Add(Me.pnlSync)
        Me.spContControls.Panel2.Controls.Add(Me.pnlToc)
        Me.spContControls.Panel2.Controls.Add(Me.pnlWeb)
        Me.spContControls.Size = New System.Drawing.Size(289, 702)
        Me.spContControls.SplitterDistance = 157
        Me.spContControls.TabIndex = 0
        '
        'tblLayoutWidgets
        '
        Me.tblLayoutWidgets.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetPartial
        Me.tblLayoutWidgets.ColumnCount = 3
        Me.tblLayoutWidgets.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.0!))
        Me.tblLayoutWidgets.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.tblLayoutWidgets.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.tblLayoutWidgets.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tblLayoutWidgets.Location = New System.Drawing.Point(0, 0)
        Me.tblLayoutWidgets.Name = "tblLayoutWidgets"
        Me.tblLayoutWidgets.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.tblLayoutWidgets.RowCount = 1
        Me.tblLayoutWidgets.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.0!))
        Me.tblLayoutWidgets.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.tblLayoutWidgets.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.tblLayoutWidgets.Size = New System.Drawing.Size(289, 157)
        Me.tblLayoutWidgets.TabIndex = 1
        '
        'pnlGeonetTrace
        '
        Me.pnlGeonetTrace.Location = New System.Drawing.Point(202, 114)
        Me.pnlGeonetTrace.Name = "pnlGeonetTrace"
        Me.pnlGeonetTrace.Size = New System.Drawing.Size(86, 64)
        Me.pnlGeonetTrace.TabIndex = 5
        '
        'pnlRouting
        '
        Me.pnlRouting.Location = New System.Drawing.Point(130, 181)
        Me.pnlRouting.Name = "pnlRouting"
        Me.pnlRouting.Size = New System.Drawing.Size(62, 49)
        Me.pnlRouting.TabIndex = 9
        '
        'pnlInspect
        '
        Me.pnlInspect.Location = New System.Drawing.Point(178, 356)
        Me.pnlInspect.Name = "pnlInspect"
        Me.pnlInspect.Size = New System.Drawing.Size(98, 35)
        Me.pnlInspect.TabIndex = 8
        '
        'pnlCreateFeature
        '
        Me.pnlCreateFeature.Location = New System.Drawing.Point(217, 222)
        Me.pnlCreateFeature.Name = "pnlCreateFeature"
        Me.pnlCreateFeature.Size = New System.Drawing.Size(59, 100)
        Me.pnlCreateFeature.TabIndex = 7
        '
        'pnlSketch
        '
        Me.pnlSketch.Location = New System.Drawing.Point(73, 247)
        Me.pnlSketch.Name = "pnlSketch"
        Me.pnlSketch.Size = New System.Drawing.Size(86, 76)
        Me.pnlSketch.TabIndex = 6
        Me.pnlSketch.Visible = False
        '
        'pnlWO
        '
        Me.pnlWO.Location = New System.Drawing.Point(118, 198)
        Me.pnlWO.Name = "pnlWO"
        Me.pnlWO.Size = New System.Drawing.Size(86, 64)
        Me.pnlWO.TabIndex = 5
        Me.pnlWO.Visible = False
        '
        'pnlID
        '
        Me.pnlID.Location = New System.Drawing.Point(94, 91)
        Me.pnlID.Name = "pnlID"
        Me.pnlID.Size = New System.Drawing.Size(86, 64)
        Me.pnlID.TabIndex = 4
        '
        'pnlTrace
        '
        Me.pnlTrace.Location = New System.Drawing.Point(24, 91)
        Me.pnlTrace.Name = "pnlTrace"
        Me.pnlTrace.Size = New System.Drawing.Size(64, 54)
        Me.pnlTrace.TabIndex = 3
        '
        'pnlSearch
        '
        Me.pnlSearch.Location = New System.Drawing.Point(202, 13)
        Me.pnlSearch.Name = "pnlSearch"
        Me.pnlSearch.Size = New System.Drawing.Size(59, 64)
        Me.pnlSearch.TabIndex = 2
        '
        'pnlSync
        '
        Me.pnlSync.Location = New System.Drawing.Point(109, 13)
        Me.pnlSync.Name = "pnlSync"
        Me.pnlSync.Size = New System.Drawing.Size(50, 49)
        Me.pnlSync.TabIndex = 1
        '
        'pnlToc
        '
        Me.pnlToc.Location = New System.Drawing.Point(12, 13)
        Me.pnlToc.Name = "pnlToc"
        Me.pnlToc.Size = New System.Drawing.Size(62, 49)
        Me.pnlToc.TabIndex = 0
        '
        'pnlWeb
        '
        Me.pnlWeb.Location = New System.Drawing.Point(12, 13)
        Me.pnlWeb.Name = "pnlWeb"
        Me.pnlWeb.Size = New System.Drawing.Size(62, 49)
        Me.pnlWeb.TabIndex = 0
        '
        'btnTakeImage
        '
        Me.btnTakeImage.BackgroundImage = Global.Esri.ArcGISTemplates.My.Resources.Resources.BluePrint
        Me.btnTakeImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnTakeImage.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz
        Me.btnTakeImage.FlatAppearance.BorderSize = 0
        Me.btnTakeImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnTakeImage.Location = New System.Drawing.Point(37, 364)
        Me.btnTakeImage.Name = "btnTakeImage"
        Me.btnTakeImage.Size = New System.Drawing.Size(50, 50)
        Me.btnTakeImage.TabIndex = 3
        Me.btnTakeImage.UseVisualStyleBackColor = True
        '
        'btnOpenClosePanel
        '
        Me.btnOpenClosePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnOpenClosePanel.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz
        Me.btnOpenClosePanel.FlatAppearance.BorderSize = 0
        Me.btnOpenClosePanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOpenClosePanel.Location = New System.Drawing.Point(50, 252)
        Me.btnOpenClosePanel.Name = "btnOpenClosePanel"
        Me.btnOpenClosePanel.Size = New System.Drawing.Size(50, 50)
        Me.btnOpenClosePanel.TabIndex = 1
        Me.btnOpenClosePanel.UseVisualStyleBackColor = True
        '
        'Map1
        '
        Me.Map1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Map1.Location = New System.Drawing.Point(0, 0)
        Me.Map1.Name = "Map1"
        Me.Map1.ScaleBar.DisplayPosition = Esri.ArcGIS.Mobile.WinForms.ScaleBarDisplayPosition.None
        Me.Map1.Size = New System.Drawing.Size(578, 712)
        Me.Map1.TabIndex = 0
        Me.Map1.Text = "Map1"
        Me.Map1.UseBuiltInNavigation = False
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.statLabel, Me.statTool})
        Me.StatusStrip1.Location = New System.Drawing.Point(41, 712)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(881, 22)
        Me.StatusStrip1.TabIndex = 10
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'statLabel
        '
        Me.statLabel.BackColor = System.Drawing.SystemColors.Control
        Me.statLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.statLabel.ForeColor = System.Drawing.Color.Red
        Me.statLabel.Name = "statLabel"
        Me.statLabel.Size = New System.Drawing.Size(162, 17)
        Me.statLabel.Text = "ToolStripStatusLabel1"
        '
        'statTool
        '
        Me.statTool.BackColor = System.Drawing.SystemColors.Control
        Me.statTool.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.statTool.ForeColor = System.Drawing.Color.Red
        Me.statTool.Name = "statTool"
        Me.statTool.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.statTool.Size = New System.Drawing.Size(704, 17)
        Me.statTool.Spring = True
        Me.statTool.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'gpBxSideBar
        '
        Me.gpBxSideBar.Controls.Add(Me.btnToggleScreens)
        Me.gpBxSideBar.Controls.Add(Me.btnToggleDisplay)
        Me.gpBxSideBar.Dock = System.Windows.Forms.DockStyle.Left
        Me.gpBxSideBar.Location = New System.Drawing.Point(0, 0)
        Me.gpBxSideBar.Name = "gpBxSideBar"
        Me.gpBxSideBar.Size = New System.Drawing.Size(41, 734)
        Me.gpBxSideBar.TabIndex = 11
        Me.gpBxSideBar.TabStop = False
        '
        'btnToggleScreens
        '
        Me.btnToggleScreens.FlatAppearance.BorderSize = 0
        Me.btnToggleScreens.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnToggleScreens.Location = New System.Drawing.Point(3, 12)
        Me.btnToggleScreens.Name = "btnToggleScreens"
        Me.btnToggleScreens.Size = New System.Drawing.Size(35, 100)
        Me.btnToggleScreens.TabIndex = 1
        Me.btnToggleScreens.UseVisualStyleBackColor = True
        '
        'btnToggleDisplay
        '
        Me.btnToggleDisplay.FlatAppearance.BorderColor = System.Drawing.Color.White
        Me.btnToggleDisplay.FlatAppearance.BorderSize = 0
        Me.btnToggleDisplay.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnToggleDisplay.Location = New System.Drawing.Point(3, 118)
        Me.btnToggleDisplay.Name = "btnToggleDisplay"
        Me.btnToggleDisplay.Size = New System.Drawing.Size(35, 100)
        Me.btnToggleDisplay.TabIndex = 0
        Me.btnToggleDisplay.Tag = "PanelVert"
        Me.btnToggleDisplay.UseVisualStyleBackColor = True

        'pnlWaypoint
        '
        Me.pnlWaypoint.Location = New System.Drawing.Point(73, 329)
        Me.pnlWaypoint.Name = "pnlWaypoint"
        Me.pnlWaypoint.Size = New System.Drawing.Size(86, 76)
        Me.pnlWaypoint.TabIndex = 7
        Me.pnlWaypoint.Visible = False
        '

        '
        'MobileMapConsole
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(922, 734)
        Me.Controls.Add(Me.spContMain)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.gpBxSideBar)
        Me.MinimumSize = New System.Drawing.Size(900, 570)
        Me.Name = "MobileMapConsole"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.spContMain.Panel1.ResumeLayout(False)
        Me.spContMain.Panel2.ResumeLayout(False)
        Me.spContMain.ResumeLayout(False)
        Me.spContControls.Panel1.ResumeLayout(False)
        Me.spContControls.Panel2.ResumeLayout(False)
        Me.spContControls.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.gpBxSideBar.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents spContMain As System.Windows.Forms.SplitContainer
    Friend WithEvents spContControls As System.Windows.Forms.SplitContainer
    Friend WithEvents pnlID As System.Windows.Forms.Panel
    Friend WithEvents pnlTrace As System.Windows.Forms.Panel
    Friend WithEvents pnlSearch As System.Windows.Forms.Panel
    Friend WithEvents pnlSync As System.Windows.Forms.Panel
    Friend WithEvents pnlToc As System.Windows.Forms.Panel
    Friend WithEvents pnlWeb As System.Windows.Forms.Panel
    Friend WithEvents btnOpenClosePanel As System.Windows.Forms.Button
    Friend WithEvents tblLayoutWidgets As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlWO As System.Windows.Forms.Panel
    Friend WithEvents pnlSketch As System.Windows.Forms.Panel
    Friend WithEvents pnlCreateFeature As System.Windows.Forms.Panel
    Friend WithEvents pnlInspect As System.Windows.Forms.Panel
    Friend WithEvents pnlRouting As System.Windows.Forms.Panel
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents statLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents statTool As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents pnlGeonetTrace As System.Windows.Forms.Panel
    Private WithEvents Map1 As Esri.ArcGIS.Mobile.WinForms.Map
    Friend WithEvents gpBxSideBar As System.Windows.Forms.GroupBox
    Friend WithEvents btnToggleScreens As System.Windows.Forms.Button
    Friend WithEvents btnToggleDisplay As System.Windows.Forms.Button
    Friend WithEvents btnTakeImage As System.Windows.Forms.Button
    Friend WithEvents pnlWaypoint As System.Windows.Forms.Panel
End Class
