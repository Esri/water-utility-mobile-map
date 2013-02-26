<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MobileSearch
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.splContMainSearch = New System.Windows.Forms.SplitContainer()
        Me.pnlSearch = New System.Windows.Forms.GroupBox()
        Me.btnWaypointSearch = New System.Windows.Forms.Button()
        Me.btnBuffer = New System.Windows.Forms.CheckBox()
        Me.cboBufferVal = New System.Windows.Forms.ComboBox()
        Me.lblSearchGeo = New System.Windows.Forms.Label()
        Me.cboSearchValue = New System.Windows.Forms.ComboBox()
        Me.chkSearchField = New System.Windows.Forms.CheckedListBox()
        Me.picSearching = New System.Windows.Forms.PictureBox()
        Me.lblNumResults = New System.Windows.Forms.Label()
        Me.btnSearchFind = New System.Windows.Forms.Button()
        Me.chkSearchSimliar = New System.Windows.Forms.CheckBox()
        Me.txtSearchValue = New System.Windows.Forms.TextBox()
        Me.lblSearchValue = New System.Windows.Forms.Label()
        Me.cboSearchField = New System.Windows.Forms.ComboBox()
        Me.lblSearchField = New System.Windows.Forms.Label()
        Me.cboSearchLayer = New System.Windows.Forms.ComboBox()
        Me.lblSearchLayer = New System.Windows.Forms.Label()
        Me.pnlAddress = New System.Windows.Forms.GroupBox()
        Me.gpInterSearch = New System.Windows.Forms.GroupBox()
        Me.btnWaypoint = New System.Windows.Forms.Button()
        Me.cboStreetLayer2 = New System.Windows.Forms.ComboBox()
        Me.btnAddressReload = New System.Windows.Forms.Button()
        Me.lblAddressStreet1 = New System.Windows.Forms.Label()
        Me.btnAddressRouteTo = New System.Windows.Forms.Button()
        Me.cboStreetLayer1 = New System.Windows.Forms.ComboBox()
        Me.btnAddressZoomTo = New System.Windows.Forms.Button()
        Me.rdoAddressRange = New System.Windows.Forms.RadioButton()
        Me.rdoAddressStreet2 = New System.Windows.Forms.RadioButton()
        Me.cboStreetRange = New System.Windows.Forms.ComboBox()
        Me.gpBoxPreFiltInt = New System.Windows.Forms.GroupBox()
        Me.pnlAddressPoint = New System.Windows.Forms.GroupBox()
        Me.pnlAddPntButtons = New System.Windows.Forms.Panel()
        Me.btnWaypointDrillDown = New System.Windows.Forms.Button()
        Me.btnAddressPointZoomTo = New System.Windows.Forms.Button()
        Me.lblMatchingResults = New System.Windows.Forms.Label()
        Me.gpBxAddPntControls = New System.Windows.Forms.Panel()
        Me.gpBxAddPntControlsLayer = New System.Windows.Forms.Panel()
        Me.cboDrillDownLayer = New System.Windows.Forms.ComboBox()
        Me.pnlGeocode = New System.Windows.Forms.GroupBox()
        Me.pnlOnlineGC = New System.Windows.Forms.GroupBox()
        Me.btnWaypointOnline = New System.Windows.Forms.Button()
        Me.btnOnlineGC = New System.Windows.Forms.Button()
        Me.lblOnlineGCLabel = New System.Windows.Forms.Label()
        Me.txtbxOnlineGCAddress = New MobileControls.MobileSearch.MyTextBox()
        Me.pnlSDCGC = New System.Windows.Forms.Panel()
        Me.txtState = New System.Windows.Forms.TextBox()
        Me.txtCity = New System.Windows.Forms.TextBox()
        Me.txtStreet = New System.Windows.Forms.TextBox()
        Me.txtStreetNum = New System.Windows.Forms.TextBox()
        Me.btnLocateOnMap = New System.Windows.Forms.Button()
        Me.label3 = New System.Windows.Forms.Label()
        Me.State = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.pnlMyGC = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnWaypointGC = New System.Windows.Forms.Button()
        Me.lblGeocode = New System.Windows.Forms.Label()
        Me.btnRunGC = New System.Windows.Forms.Button()
        Me.txtGeocodeValue = New MobileControls.MobileSearch.MyTextBox()
        Me.lblGCNumResults = New System.Windows.Forms.Label()
        Me.gpBoxPreFiltSt = New System.Windows.Forms.GroupBox()
        Me.pnlSearchOptions = New System.Windows.Forms.GroupBox()
        Me.btnAddressPnt = New System.Windows.Forms.CheckBox()
        Me.gpBxGoToXY = New System.Windows.Forms.GroupBox()
        Me.chkLatLong = New System.Windows.Forms.CheckBox()
        Me.txtX = New MobileControls.MobileSearch.MyTextBox()
        Me.lblY = New System.Windows.Forms.Label()
        Me.txtY = New MobileControls.MobileSearch.MyTextBox()
        Me.lblX = New System.Windows.Forms.Label()
        Me.btnRunXY = New System.Windows.Forms.Button()
        Me.btnAddress = New System.Windows.Forms.CheckBox()
        Me.btnLookup = New System.Windows.Forms.CheckBox()
        Me.btnGeocode = New System.Windows.Forms.CheckBox()
        Me.pnResults = New System.Windows.Forms.GroupBox()
        Me.HScrollBar1 = New System.Windows.Forms.HScrollBar()
        Me.VScrollBar1 = New System.Windows.Forms.VScrollBar()
        Me.dgResults = New System.Windows.Forms.DataGridView()
        Me.pnlResultsTools = New System.Windows.Forms.GroupBox()
        Me.btnDGRouteTo = New System.Windows.Forms.Button()
        Me.btnDGFlash = New System.Windows.Forms.Button()
        Me.btnDGZoomTo = New System.Windows.Forms.Button()
        Me.splContMainSearch.Panel1.SuspendLayout()
        Me.splContMainSearch.Panel2.SuspendLayout()
        Me.splContMainSearch.SuspendLayout()
        Me.pnlSearch.SuspendLayout()
        CType(Me.picSearching, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlAddress.SuspendLayout()
        Me.gpInterSearch.SuspendLayout()
        Me.pnlAddressPoint.SuspendLayout()
        Me.pnlAddPntButtons.SuspendLayout()
        Me.gpBxAddPntControlsLayer.SuspendLayout()
        Me.pnlGeocode.SuspendLayout()
        Me.pnlOnlineGC.SuspendLayout()
        Me.pnlSDCGC.SuspendLayout()
        Me.pnlMyGC.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.pnlSearchOptions.SuspendLayout()
        Me.gpBxGoToXY.SuspendLayout()
        Me.pnResults.SuspendLayout()
        CType(Me.dgResults, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlResultsTools.SuspendLayout()
        Me.SuspendLayout()
        '
        'splContMainSearch
        '
        Me.splContMainSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splContMainSearch.IsSplitterFixed = True
        Me.splContMainSearch.Location = New System.Drawing.Point(0, 0)
        Me.splContMainSearch.Name = "splContMainSearch"
        Me.splContMainSearch.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splContMainSearch.Panel1
        '
        Me.splContMainSearch.Panel1.Controls.Add(Me.pnlSearch)
        Me.splContMainSearch.Panel1.Controls.Add(Me.pnlAddress)
        Me.splContMainSearch.Panel1.Controls.Add(Me.pnlAddressPoint)
        Me.splContMainSearch.Panel1.Controls.Add(Me.pnlGeocode)
        Me.splContMainSearch.Panel1.Controls.Add(Me.pnlSearchOptions)
        '
        'splContMainSearch.Panel2
        '
        Me.splContMainSearch.Panel2.Controls.Add(Me.pnResults)
        Me.splContMainSearch.Panel2.Controls.Add(Me.pnlResultsTools)
        Me.splContMainSearch.Size = New System.Drawing.Size(629, 1606)
        Me.splContMainSearch.SplitterDistance = 694
        Me.splContMainSearch.TabIndex = 6
        '
        'pnlSearch
        '
        Me.pnlSearch.Controls.Add(Me.btnWaypointSearch)
        Me.pnlSearch.Controls.Add(Me.btnBuffer)
        Me.pnlSearch.Controls.Add(Me.cboBufferVal)
        Me.pnlSearch.Controls.Add(Me.lblSearchGeo)
        Me.pnlSearch.Controls.Add(Me.cboSearchValue)
        Me.pnlSearch.Controls.Add(Me.chkSearchField)
        Me.pnlSearch.Controls.Add(Me.picSearching)
        Me.pnlSearch.Controls.Add(Me.lblNumResults)
        Me.pnlSearch.Controls.Add(Me.btnSearchFind)
        Me.pnlSearch.Controls.Add(Me.chkSearchSimliar)
        Me.pnlSearch.Controls.Add(Me.txtSearchValue)
        Me.pnlSearch.Controls.Add(Me.lblSearchValue)
        Me.pnlSearch.Controls.Add(Me.cboSearchField)
        Me.pnlSearch.Controls.Add(Me.lblSearchField)
        Me.pnlSearch.Controls.Add(Me.cboSearchLayer)
        Me.pnlSearch.Controls.Add(Me.lblSearchLayer)
        Me.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlSearch.Location = New System.Drawing.Point(0, 479)
        Me.pnlSearch.Name = "pnlSearch"
        Me.pnlSearch.Size = New System.Drawing.Size(629, 33)
        Me.pnlSearch.TabIndex = 7
        Me.pnlSearch.TabStop = False
        '
        'btnWaypointSearch
        '
        Me.btnWaypointSearch.BackgroundImage = Global.MobileControls.My.Resources.Resources.NavTooBlue
        Me.btnWaypointSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaypointSearch.FlatAppearance.BorderSize = 0
        Me.btnWaypointSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWaypointSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypointSearch.Location = New System.Drawing.Point(8, 251)
        Me.btnWaypointSearch.Name = "btnWaypointSearch"
        Me.btnWaypointSearch.Size = New System.Drawing.Size(50, 50)
        Me.btnWaypointSearch.TabIndex = 16
        Me.btnWaypointSearch.UseVisualStyleBackColor = True
        '
        'btnBuffer
        '
        Me.btnBuffer.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnBuffer.BackgroundImage = Global.MobileControls.My.Resources.Resources.CircleBuffer
        Me.btnBuffer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnBuffer.FlatAppearance.BorderSize = 0
        Me.btnBuffer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBuffer.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBuffer.Location = New System.Drawing.Point(153, 187)
        Me.btnBuffer.Name = "btnBuffer"
        Me.btnBuffer.Size = New System.Drawing.Size(40, 40)
        Me.btnBuffer.TabIndex = 15
        Me.btnBuffer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnBuffer.UseVisualStyleBackColor = True
        '
        'cboBufferVal
        '
        Me.cboBufferVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBufferVal.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboBufferVal.FormattingEnabled = True
        Me.cboBufferVal.Location = New System.Drawing.Point(11, 198)
        Me.cboBufferVal.Name = "cboBufferVal"
        Me.cboBufferVal.Size = New System.Drawing.Size(135, 28)
        Me.cboBufferVal.TabIndex = 14
        '
        'lblSearchGeo
        '
        Me.lblSearchGeo.AutoSize = True
        Me.lblSearchGeo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSearchGeo.Location = New System.Drawing.Point(10, 175)
        Me.lblSearchGeo.Name = "lblSearchGeo"
        Me.lblSearchGeo.Size = New System.Drawing.Size(117, 20)
        Me.lblSearchGeo.TabIndex = 13
        Me.lblSearchGeo.Text = "In this buffer:"
        '
        'cboSearchValue
        '
        Me.cboSearchValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSearchValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboSearchValue.FormattingEnabled = True
        Me.cboSearchValue.Location = New System.Drawing.Point(10, 144)
        Me.cboSearchValue.Name = "cboSearchValue"
        Me.cboSearchValue.Size = New System.Drawing.Size(184, 28)
        Me.cboSearchValue.TabIndex = 12
        '
        'chkSearchField
        '
        Me.chkSearchField.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSearchField.FormattingEnabled = True
        Me.chkSearchField.Location = New System.Drawing.Point(326, 83)
        Me.chkSearchField.Name = "chkSearchField"
        Me.chkSearchField.Size = New System.Drawing.Size(183, 46)
        Me.chkSearchField.TabIndex = 11
        Me.chkSearchField.Visible = False
        '
        'picSearching
        '
        Me.picSearching.Location = New System.Drawing.Point(123, 256)
        Me.picSearching.Name = "picSearching"
        Me.picSearching.Size = New System.Drawing.Size(92, 24)
        Me.picSearching.TabIndex = 10
        Me.picSearching.TabStop = False
        Me.picSearching.Visible = False
        '
        'lblNumResults
        '
        Me.lblNumResults.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNumResults.Location = New System.Drawing.Point(118, 256)
        Me.lblNumResults.Name = "lblNumResults"
        Me.lblNumResults.Size = New System.Drawing.Size(112, 24)
        Me.lblNumResults.TabIndex = 9
        Me.lblNumResults.Text = "Number of R"
        Me.lblNumResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSearchFind
        '
        Me.btnSearchFind.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomInOut
        Me.btnSearchFind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnSearchFind.Enabled = False
        Me.btnSearchFind.FlatAppearance.BorderSize = 0
        Me.btnSearchFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSearchFind.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSearchFind.Location = New System.Drawing.Point(66, 251)
        Me.btnSearchFind.Name = "btnSearchFind"
        Me.btnSearchFind.Size = New System.Drawing.Size(50, 50)
        Me.btnSearchFind.TabIndex = 8
        Me.btnSearchFind.UseVisualStyleBackColor = True
        '
        'chkSearchSimliar
        '
        Me.chkSearchSimliar.AutoSize = True
        Me.chkSearchSimliar.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSearchSimliar.Location = New System.Drawing.Point(40, 226)
        Me.chkSearchSimliar.Name = "chkSearchSimliar"
        Me.chkSearchSimliar.Size = New System.Drawing.Size(152, 24)
        Me.chkSearchSimliar.TabIndex = 7
        Me.chkSearchSimliar.Text = "Find all similar?"
        Me.chkSearchSimliar.UseVisualStyleBackColor = True
        '
        'txtSearchValue
        '
        Me.txtSearchValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSearchValue.Location = New System.Drawing.Point(8, 145)
        Me.txtSearchValue.Name = "txtSearchValue"
        Me.txtSearchValue.Size = New System.Drawing.Size(170, 26)
        Me.txtSearchValue.TabIndex = 6
        '
        'lblSearchValue
        '
        Me.lblSearchValue.AutoSize = True
        Me.lblSearchValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSearchValue.Location = New System.Drawing.Point(8, 121)
        Me.lblSearchValue.Name = "lblSearchValue"
        Me.lblSearchValue.Size = New System.Drawing.Size(122, 20)
        Me.lblSearchValue.TabIndex = 5
        Me.lblSearchValue.Text = "For this value:"
        '
        'cboSearchField
        '
        Me.cboSearchField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSearchField.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboSearchField.FormattingEnabled = True
        Me.cboSearchField.Location = New System.Drawing.Point(8, 89)
        Me.cboSearchField.Name = "cboSearchField"
        Me.cboSearchField.Size = New System.Drawing.Size(184, 28)
        Me.cboSearchField.TabIndex = 4
        '
        'lblSearchField
        '
        Me.lblSearchField.AutoSize = True
        Me.lblSearchField.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSearchField.Location = New System.Drawing.Point(8, 65)
        Me.lblSearchField.Name = "lblSearchField"
        Me.lblSearchField.Size = New System.Drawing.Size(138, 20)
        Me.lblSearchField.TabIndex = 3
        Me.lblSearchField.Text = "Using this Field:"
        '
        'cboSearchLayer
        '
        Me.cboSearchLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSearchLayer.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboSearchLayer.FormattingEnabled = True
        Me.cboSearchLayer.Location = New System.Drawing.Point(8, 33)
        Me.cboSearchLayer.Name = "cboSearchLayer"
        Me.cboSearchLayer.Size = New System.Drawing.Size(184, 28)
        Me.cboSearchLayer.TabIndex = 2
        '
        'lblSearchLayer
        '
        Me.lblSearchLayer.AutoSize = True
        Me.lblSearchLayer.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSearchLayer.Location = New System.Drawing.Point(8, 11)
        Me.lblSearchLayer.Name = "lblSearchLayer"
        Me.lblSearchLayer.Size = New System.Drawing.Size(141, 20)
        Me.lblSearchLayer.TabIndex = 0
        Me.lblSearchLayer.Text = "Search In Layer:"
        '
        'pnlAddress
        '
        Me.pnlAddress.Controls.Add(Me.gpInterSearch)
        Me.pnlAddress.Controls.Add(Me.gpBoxPreFiltInt)
        Me.pnlAddress.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlAddress.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlAddress.Location = New System.Drawing.Point(0, 439)
        Me.pnlAddress.Name = "pnlAddress"
        Me.pnlAddress.Size = New System.Drawing.Size(629, 40)
        Me.pnlAddress.TabIndex = 9
        Me.pnlAddress.TabStop = False
        '
        'gpInterSearch
        '
        Me.gpInterSearch.Controls.Add(Me.btnWaypoint)
        Me.gpInterSearch.Controls.Add(Me.cboStreetLayer2)
        Me.gpInterSearch.Controls.Add(Me.btnAddressReload)
        Me.gpInterSearch.Controls.Add(Me.lblAddressStreet1)
        Me.gpInterSearch.Controls.Add(Me.btnAddressRouteTo)
        Me.gpInterSearch.Controls.Add(Me.cboStreetLayer1)
        Me.gpInterSearch.Controls.Add(Me.btnAddressZoomTo)
        Me.gpInterSearch.Controls.Add(Me.rdoAddressRange)
        Me.gpInterSearch.Controls.Add(Me.rdoAddressStreet2)
        Me.gpInterSearch.Controls.Add(Me.cboStreetRange)
        Me.gpInterSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gpInterSearch.Location = New System.Drawing.Point(3, 181)
        Me.gpInterSearch.Name = "gpInterSearch"
        Me.gpInterSearch.Size = New System.Drawing.Size(623, 0)
        Me.gpInterSearch.TabIndex = 14
        Me.gpInterSearch.TabStop = False
        '
        'btnWaypoint
        '
        Me.btnWaypoint.BackgroundImage = Global.MobileControls.My.Resources.Resources.NavTooBlue
        Me.btnWaypoint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaypoint.FlatAppearance.BorderSize = 0
        Me.btnWaypoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWaypoint.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypoint.Location = New System.Drawing.Point(130, 191)
        Me.btnWaypoint.Name = "btnWaypoint"
        Me.btnWaypoint.Size = New System.Drawing.Size(50, 50)
        Me.btnWaypoint.TabIndex = 14
        Me.btnWaypoint.UseVisualStyleBackColor = True
        '
        'cboStreetLayer2
        '
        Me.cboStreetLayer2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cboStreetLayer2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboStreetLayer2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboStreetLayer2.FormattingEnabled = True
        Me.cboStreetLayer2.Location = New System.Drawing.Point(18, 93)
        Me.cboStreetLayer2.Name = "cboStreetLayer2"
        Me.cboStreetLayer2.Size = New System.Drawing.Size(184, 28)
        Me.cboStreetLayer2.Sorted = True
        Me.cboStreetLayer2.TabIndex = 6
        '
        'btnAddressReload
        '
        Me.btnAddressReload.BackColor = System.Drawing.Color.Transparent
        Me.btnAddressReload.BackgroundImage = Global.MobileControls.My.Resources.Resources.refresh
        Me.btnAddressReload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddressReload.FlatAppearance.BorderSize = 0
        Me.btnAddressReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddressReload.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddressReload.Location = New System.Drawing.Point(188, 191)
        Me.btnAddressReload.Name = "btnAddressReload"
        Me.btnAddressReload.Size = New System.Drawing.Size(50, 50)
        Me.btnAddressReload.TabIndex = 13
        Me.btnAddressReload.UseVisualStyleBackColor = False
        '
        'lblAddressStreet1
        '
        Me.lblAddressStreet1.AutoSize = True
        Me.lblAddressStreet1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAddressStreet1.Location = New System.Drawing.Point(18, 16)
        Me.lblAddressStreet1.Name = "lblAddressStreet1"
        Me.lblAddressStreet1.Size = New System.Drawing.Size(128, 20)
        Me.lblAddressStreet1.TabIndex = 3
        Me.lblAddressStreet1.Text = "Primary Street:"
        '
        'btnAddressRouteTo
        '
        Me.btnAddressRouteTo.BackgroundImage = Global.MobileControls.My.Resources.Resources.BlueTruckSmall
        Me.btnAddressRouteTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddressRouteTo.FlatAppearance.BorderSize = 0
        Me.btnAddressRouteTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddressRouteTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddressRouteTo.Location = New System.Drawing.Point(74, 190)
        Me.btnAddressRouteTo.Name = "btnAddressRouteTo"
        Me.btnAddressRouteTo.Size = New System.Drawing.Size(50, 50)
        Me.btnAddressRouteTo.TabIndex = 12
        Me.btnAddressRouteTo.UseVisualStyleBackColor = True
        '
        'cboStreetLayer1
        '
        Me.cboStreetLayer1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cboStreetLayer1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboStreetLayer1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboStreetLayer1.FormattingEnabled = True
        Me.cboStreetLayer1.Location = New System.Drawing.Point(18, 36)
        Me.cboStreetLayer1.Name = "cboStreetLayer1"
        Me.cboStreetLayer1.Size = New System.Drawing.Size(184, 28)
        Me.cboStreetLayer1.Sorted = True
        Me.cboStreetLayer1.TabIndex = 4
        '
        'btnAddressZoomTo
        '
        Me.btnAddressZoomTo.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomIn1
        Me.btnAddressZoomTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddressZoomTo.FlatAppearance.BorderSize = 0
        Me.btnAddressZoomTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddressZoomTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddressZoomTo.Location = New System.Drawing.Point(13, 191)
        Me.btnAddressZoomTo.Name = "btnAddressZoomTo"
        Me.btnAddressZoomTo.Size = New System.Drawing.Size(50, 50)
        Me.btnAddressZoomTo.TabIndex = 11
        Me.btnAddressZoomTo.UseVisualStyleBackColor = True
        '
        'rdoAddressRange
        '
        Me.rdoAddressRange.AutoSize = True
        Me.rdoAddressRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoAddressRange.Location = New System.Drawing.Point(18, 132)
        Me.rdoAddressRange.Name = "rdoAddressRange"
        Me.rdoAddressRange.Size = New System.Drawing.Size(204, 24)
        Me.rdoAddressRange.TabIndex = 10
        Me.rdoAddressRange.Text = "Primary Street Range:"
        Me.rdoAddressRange.UseVisualStyleBackColor = True
        '
        'rdoAddressStreet2
        '
        Me.rdoAddressStreet2.AutoSize = True
        Me.rdoAddressStreet2.Checked = True
        Me.rdoAddressStreet2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rdoAddressStreet2.Location = New System.Drawing.Point(18, 67)
        Me.rdoAddressStreet2.Name = "rdoAddressStreet2"
        Me.rdoAddressStreet2.Size = New System.Drawing.Size(183, 24)
        Me.rdoAddressStreet2.TabIndex = 9
        Me.rdoAddressStreet2.TabStop = True
        Me.rdoAddressStreet2.Text = "Intersecting Street:"
        Me.rdoAddressStreet2.UseVisualStyleBackColor = True
        '
        'cboStreetRange
        '
        Me.cboStreetRange.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.cboStreetRange.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cboStreetRange.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboStreetRange.FormattingEnabled = True
        Me.cboStreetRange.Location = New System.Drawing.Point(18, 157)
        Me.cboStreetRange.Name = "cboStreetRange"
        Me.cboStreetRange.Size = New System.Drawing.Size(184, 28)
        Me.cboStreetRange.Sorted = True
        Me.cboStreetRange.TabIndex = 8
        '
        'gpBoxPreFiltInt
        '
        Me.gpBoxPreFiltInt.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxPreFiltInt.Location = New System.Drawing.Point(3, 22)
        Me.gpBoxPreFiltInt.Name = "gpBoxPreFiltInt"
        Me.gpBoxPreFiltInt.Size = New System.Drawing.Size(623, 159)
        Me.gpBoxPreFiltInt.TabIndex = 15
        Me.gpBoxPreFiltInt.TabStop = False
        Me.gpBoxPreFiltInt.Text = "PreFilter"
        '
        'pnlAddressPoint
        '
        Me.pnlAddressPoint.Controls.Add(Me.pnlAddPntButtons)
        Me.pnlAddressPoint.Controls.Add(Me.gpBxAddPntControls)
        Me.pnlAddressPoint.Controls.Add(Me.gpBxAddPntControlsLayer)
        Me.pnlAddressPoint.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAddressPoint.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlAddressPoint.Location = New System.Drawing.Point(0, 439)
        Me.pnlAddressPoint.Name = "pnlAddressPoint"
        Me.pnlAddressPoint.Size = New System.Drawing.Size(629, 255)
        Me.pnlAddressPoint.TabIndex = 9
        Me.pnlAddressPoint.TabStop = False
        Me.pnlAddressPoint.Visible = False
        '
        'pnlAddPntButtons
        '
        Me.pnlAddPntButtons.Controls.Add(Me.btnWaypointDrillDown)
        Me.pnlAddPntButtons.Controls.Add(Me.btnAddressPointZoomTo)
        Me.pnlAddPntButtons.Controls.Add(Me.lblMatchingResults)
        Me.pnlAddPntButtons.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAddPntButtons.Location = New System.Drawing.Point(3, 134)
        Me.pnlAddPntButtons.Name = "pnlAddPntButtons"
        Me.pnlAddPntButtons.Size = New System.Drawing.Size(623, 118)
        Me.pnlAddPntButtons.TabIndex = 13
        '
        'btnWaypointDrillDown
        '
        Me.btnWaypointDrillDown.BackgroundImage = Global.MobileControls.My.Resources.Resources.NavTooBlue
        Me.btnWaypointDrillDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaypointDrillDown.FlatAppearance.BorderSize = 0
        Me.btnWaypointDrillDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWaypointDrillDown.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypointDrillDown.Location = New System.Drawing.Point(10, 3)
        Me.btnWaypointDrillDown.Name = "btnWaypointDrillDown"
        Me.btnWaypointDrillDown.Size = New System.Drawing.Size(50, 50)
        Me.btnWaypointDrillDown.TabIndex = 17
        Me.btnWaypointDrillDown.UseVisualStyleBackColor = True
        '
        'btnAddressPointZoomTo
        '
        Me.btnAddressPointZoomTo.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomInOut
        Me.btnAddressPointZoomTo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddressPointZoomTo.FlatAppearance.BorderSize = 0
        Me.btnAddressPointZoomTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddressPointZoomTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddressPointZoomTo.Location = New System.Drawing.Point(68, 4)
        Me.btnAddressPointZoomTo.Name = "btnAddressPointZoomTo"
        Me.btnAddressPointZoomTo.Size = New System.Drawing.Size(50, 50)
        Me.btnAddressPointZoomTo.TabIndex = 16
        Me.btnAddressPointZoomTo.UseVisualStyleBackColor = True
        '
        'lblMatchingResults
        '
        Me.lblMatchingResults.AutoSize = True
        Me.lblMatchingResults.Location = New System.Drawing.Point(124, 14)
        Me.lblMatchingResults.Name = "lblMatchingResults"
        Me.lblMatchingResults.Size = New System.Drawing.Size(158, 20)
        Me.lblMatchingResults.TabIndex = 15
        Me.lblMatchingResults.Text = "Matching Results: "
        '
        'gpBxAddPntControls
        '
        Me.gpBxAddPntControls.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBxAddPntControls.Location = New System.Drawing.Point(3, 68)
        Me.gpBxAddPntControls.Name = "gpBxAddPntControls"
        Me.gpBxAddPntControls.Size = New System.Drawing.Size(623, 66)
        Me.gpBxAddPntControls.TabIndex = 13
        '
        'gpBxAddPntControlsLayer
        '
        Me.gpBxAddPntControlsLayer.Controls.Add(Me.cboDrillDownLayer)
        Me.gpBxAddPntControlsLayer.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBxAddPntControlsLayer.Location = New System.Drawing.Point(3, 22)
        Me.gpBxAddPntControlsLayer.Name = "gpBxAddPntControlsLayer"
        Me.gpBxAddPntControlsLayer.Size = New System.Drawing.Size(623, 46)
        Me.gpBxAddPntControlsLayer.TabIndex = 13
        '
        'cboDrillDownLayer
        '
        Me.cboDrillDownLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDrillDownLayer.FormattingEnabled = True
        Me.cboDrillDownLayer.Location = New System.Drawing.Point(10, 8)
        Me.cboDrillDownLayer.Name = "cboDrillDownLayer"
        Me.cboDrillDownLayer.Size = New System.Drawing.Size(307, 28)
        Me.cboDrillDownLayer.Sorted = True
        Me.cboDrillDownLayer.TabIndex = 1
        '
        'pnlGeocode
        '
        Me.pnlGeocode.Controls.Add(Me.pnlOnlineGC)
        Me.pnlGeocode.Controls.Add(Me.pnlSDCGC)
        Me.pnlGeocode.Controls.Add(Me.pnlMyGC)
        Me.pnlGeocode.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlGeocode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlGeocode.Location = New System.Drawing.Point(0, 82)
        Me.pnlGeocode.Name = "pnlGeocode"
        Me.pnlGeocode.Size = New System.Drawing.Size(629, 357)
        Me.pnlGeocode.TabIndex = 8
        Me.pnlGeocode.TabStop = False
        '
        'pnlOnlineGC
        '
        Me.pnlOnlineGC.Controls.Add(Me.btnWaypointOnline)
        Me.pnlOnlineGC.Controls.Add(Me.btnOnlineGC)
        Me.pnlOnlineGC.Controls.Add(Me.lblOnlineGCLabel)
        Me.pnlOnlineGC.Controls.Add(Me.txtbxOnlineGCAddress)
        Me.pnlOnlineGC.Location = New System.Drawing.Point(326, 193)
        Me.pnlOnlineGC.Name = "pnlOnlineGC"
        Me.pnlOnlineGC.Size = New System.Drawing.Size(312, 151)
        Me.pnlOnlineGC.TabIndex = 13
        Me.pnlOnlineGC.TabStop = False
        Me.pnlOnlineGC.Text = "Online Services"
        '
        'btnWaypointOnline
        '
        Me.btnWaypointOnline.BackgroundImage = Global.MobileControls.My.Resources.Resources.NavTooBlue
        Me.btnWaypointOnline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaypointOnline.FlatAppearance.BorderSize = 0
        Me.btnWaypointOnline.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWaypointOnline.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypointOnline.Location = New System.Drawing.Point(9, 77)
        Me.btnWaypointOnline.Name = "btnWaypointOnline"
        Me.btnWaypointOnline.Size = New System.Drawing.Size(50, 50)
        Me.btnWaypointOnline.TabIndex = 14
        Me.btnWaypointOnline.UseVisualStyleBackColor = True
        '
        'btnOnlineGC
        '
        Me.btnOnlineGC.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomInOut
        Me.btnOnlineGC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnOnlineGC.FlatAppearance.BorderSize = 0
        Me.btnOnlineGC.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnOnlineGC.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnOnlineGC.Location = New System.Drawing.Point(65, 77)
        Me.btnOnlineGC.Name = "btnOnlineGC"
        Me.btnOnlineGC.Size = New System.Drawing.Size(50, 50)
        Me.btnOnlineGC.TabIndex = 13
        Me.btnOnlineGC.UseVisualStyleBackColor = True
        '
        'lblOnlineGCLabel
        '
        Me.lblOnlineGCLabel.AutoSize = True
        Me.lblOnlineGCLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblOnlineGCLabel.Location = New System.Drawing.Point(5, 22)
        Me.lblOnlineGCLabel.Name = "lblOnlineGCLabel"
        Me.lblOnlineGCLabel.Size = New System.Drawing.Size(265, 20)
        Me.lblOnlineGCLabel.TabIndex = 10
        Me.lblOnlineGCLabel.Text = "Address to locate (Ex: 10 Main):"
        '
        'txtbxOnlineGCAddress
        '
        Me.txtbxOnlineGCAddress.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtbxOnlineGCAddress.Location = New System.Drawing.Point(11, 45)
        Me.txtbxOnlineGCAddress.Name = "txtbxOnlineGCAddress"
        Me.txtbxOnlineGCAddress.Size = New System.Drawing.Size(170, 26)
        Me.txtbxOnlineGCAddress.TabIndex = 11
        '
        'pnlSDCGC
        '
        Me.pnlSDCGC.Controls.Add(Me.txtState)
        Me.pnlSDCGC.Controls.Add(Me.txtCity)
        Me.pnlSDCGC.Controls.Add(Me.txtStreet)
        Me.pnlSDCGC.Controls.Add(Me.txtStreetNum)
        Me.pnlSDCGC.Controls.Add(Me.btnLocateOnMap)
        Me.pnlSDCGC.Controls.Add(Me.label3)
        Me.pnlSDCGC.Controls.Add(Me.State)
        Me.pnlSDCGC.Controls.Add(Me.label2)
        Me.pnlSDCGC.Controls.Add(Me.label1)
        Me.pnlSDCGC.Location = New System.Drawing.Point(376, 21)
        Me.pnlSDCGC.Name = "pnlSDCGC"
        Me.pnlSDCGC.Size = New System.Drawing.Size(310, 159)
        Me.pnlSDCGC.TabIndex = 12
        '
        'txtState
        '
        Me.txtState.Location = New System.Drawing.Point(143, 88)
        Me.txtState.Name = "txtState"
        Me.txtState.Size = New System.Drawing.Size(148, 26)
        Me.txtState.TabIndex = 18
        Me.txtState.Text = "CA"
        '
        'txtCity
        '
        Me.txtCity.Location = New System.Drawing.Point(143, 62)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(148, 26)
        Me.txtCity.TabIndex = 17
        Me.txtCity.Text = "Redlands"
        '
        'txtStreet
        '
        Me.txtStreet.Location = New System.Drawing.Point(143, 36)
        Me.txtStreet.Name = "txtStreet"
        Me.txtStreet.Size = New System.Drawing.Size(148, 26)
        Me.txtStreet.TabIndex = 16
        Me.txtStreet.Text = "New York"
        '
        'txtStreetNum
        '
        Me.txtStreetNum.Location = New System.Drawing.Point(143, 10)
        Me.txtStreetNum.Name = "txtStreetNum"
        Me.txtStreetNum.Size = New System.Drawing.Size(148, 26)
        Me.txtStreetNum.TabIndex = 15
        Me.txtStreetNum.Text = "380"
        '
        'btnLocateOnMap
        '
        Me.btnLocateOnMap.Location = New System.Drawing.Point(65, 125)
        Me.btnLocateOnMap.Name = "btnLocateOnMap"
        Me.btnLocateOnMap.Size = New System.Drawing.Size(154, 31)
        Me.btnLocateOnMap.TabIndex = 10
        Me.btnLocateOnMap.Text = "Look up"
        Me.btnLocateOnMap.UseVisualStyleBackColor = True
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(11, 10)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(126, 20)
        Me.label3.TabIndex = 14
        Me.label3.Text = "Street Number"
        '
        'State
        '
        Me.State.AutoSize = True
        Me.State.Location = New System.Drawing.Point(11, 87)
        Me.State.Name = "State"
        Me.State.Size = New System.Drawing.Size(53, 20)
        Me.State.TabIndex = 11
        Me.State.Text = "State"
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(11, 37)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(59, 20)
        Me.label2.TabIndex = 13
        Me.label2.Text = "Street"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(11, 62)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(39, 20)
        Me.label1.TabIndex = 12
        Me.label1.Text = "City"
        '
        'pnlMyGC
        '
        Me.pnlMyGC.Controls.Add(Me.GroupBox1)
        Me.pnlMyGC.Controls.Add(Me.gpBoxPreFiltSt)
        Me.pnlMyGC.Location = New System.Drawing.Point(6, 16)
        Me.pnlMyGC.Name = "pnlMyGC"
        Me.pnlMyGC.Size = New System.Drawing.Size(314, 395)
        Me.pnlMyGC.TabIndex = 11
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnWaypointGC)
        Me.GroupBox1.Controls.Add(Me.lblGeocode)
        Me.GroupBox1.Controls.Add(Me.btnRunGC)
        Me.GroupBox1.Controls.Add(Me.txtGeocodeValue)
        Me.GroupBox1.Controls.Add(Me.lblGCNumResults)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 100)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(314, 295)
        Me.GroupBox1.TabIndex = 12
        Me.GroupBox1.TabStop = False
        '
        'btnWaypointGC
        '
        Me.btnWaypointGC.BackgroundImage = Global.MobileControls.My.Resources.Resources.NavTooBlue
        Me.btnWaypointGC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnWaypointGC.FlatAppearance.BorderSize = 0
        Me.btnWaypointGC.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnWaypointGC.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnWaypointGC.Location = New System.Drawing.Point(8, 77)
        Me.btnWaypointGC.Name = "btnWaypointGC"
        Me.btnWaypointGC.Size = New System.Drawing.Size(50, 50)
        Me.btnWaypointGC.TabIndex = 11
        Me.btnWaypointGC.UseVisualStyleBackColor = True
        '
        'lblGeocode
        '
        Me.lblGeocode.AutoSize = True
        Me.lblGeocode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGeocode.Location = New System.Drawing.Point(11, 22)
        Me.lblGeocode.Name = "lblGeocode"
        Me.lblGeocode.Size = New System.Drawing.Size(265, 20)
        Me.lblGeocode.TabIndex = 7
        Me.lblGeocode.Text = "Address to locate (Ex: 10 Main):"
        '
        'btnRunGC
        '
        Me.btnRunGC.BackgroundImage = Global.MobileControls.My.Resources.Resources.ZoomInOut
        Me.btnRunGC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnRunGC.FlatAppearance.BorderSize = 0
        Me.btnRunGC.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRunGC.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRunGC.Location = New System.Drawing.Point(63, 77)
        Me.btnRunGC.Name = "btnRunGC"
        Me.btnRunGC.Size = New System.Drawing.Size(50, 50)
        Me.btnRunGC.TabIndex = 9
        Me.btnRunGC.UseVisualStyleBackColor = True
        '
        'txtGeocodeValue
        '
        Me.txtGeocodeValue.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGeocodeValue.Location = New System.Drawing.Point(11, 45)
        Me.txtGeocodeValue.Name = "txtGeocodeValue"
        Me.txtGeocodeValue.Size = New System.Drawing.Size(170, 26)
        Me.txtGeocodeValue.TabIndex = 8
        '
        'lblGCNumResults
        '
        Me.lblGCNumResults.AutoSize = True
        Me.lblGCNumResults.Location = New System.Drawing.Point(117, 81)
        Me.lblGCNumResults.Name = "lblGCNumResults"
        Me.lblGCNumResults.Size = New System.Drawing.Size(110, 20)
        Me.lblGCNumResults.TabIndex = 10
        Me.lblGCNumResults.Text = "Number of R"
        '
        'gpBoxPreFiltSt
        '
        Me.gpBoxPreFiltSt.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpBoxPreFiltSt.Location = New System.Drawing.Point(0, 0)
        Me.gpBoxPreFiltSt.Name = "gpBoxPreFiltSt"
        Me.gpBoxPreFiltSt.Size = New System.Drawing.Size(314, 100)
        Me.gpBoxPreFiltSt.TabIndex = 11
        Me.gpBoxPreFiltSt.TabStop = False
        Me.gpBoxPreFiltSt.Text = "PreFilter"
        '
        'pnlSearchOptions
        '
        Me.pnlSearchOptions.Controls.Add(Me.btnAddressPnt)
        Me.pnlSearchOptions.Controls.Add(Me.gpBxGoToXY)
        Me.pnlSearchOptions.Controls.Add(Me.btnAddress)
        Me.pnlSearchOptions.Controls.Add(Me.btnLookup)
        Me.pnlSearchOptions.Controls.Add(Me.btnGeocode)
        Me.pnlSearchOptions.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlSearchOptions.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pnlSearchOptions.Location = New System.Drawing.Point(0, 0)
        Me.pnlSearchOptions.Name = "pnlSearchOptions"
        Me.pnlSearchOptions.Size = New System.Drawing.Size(629, 82)
        Me.pnlSearchOptions.TabIndex = 6
        Me.pnlSearchOptions.TabStop = False
        '
        'btnAddressPnt
        '
        Me.btnAddressPnt.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnAddressPnt.AutoCheck = False
        Me.btnAddressPnt.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddPoint
        Me.btnAddressPnt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddressPnt.FlatAppearance.BorderSize = 0
        Me.btnAddressPnt.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddressPnt.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddressPnt.Location = New System.Drawing.Point(242, 13)
        Me.btnAddressPnt.Name = "btnAddressPnt"
        Me.btnAddressPnt.Size = New System.Drawing.Size(40, 40)
        Me.btnAddressPnt.TabIndex = 18
        Me.btnAddressPnt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnAddressPnt.UseVisualStyleBackColor = True
        '
        'gpBxGoToXY
        '
        Me.gpBxGoToXY.Controls.Add(Me.chkLatLong)
        Me.gpBxGoToXY.Controls.Add(Me.txtX)
        Me.gpBxGoToXY.Controls.Add(Me.lblY)
        Me.gpBxGoToXY.Controls.Add(Me.txtY)
        Me.gpBxGoToXY.Controls.Add(Me.lblX)
        Me.gpBxGoToXY.Controls.Add(Me.btnRunXY)
        Me.gpBxGoToXY.Location = New System.Drawing.Point(12, 53)
        Me.gpBxGoToXY.Name = "gpBxGoToXY"
        Me.gpBxGoToXY.Size = New System.Drawing.Size(308, 30)
        Me.gpBxGoToXY.TabIndex = 17
        Me.gpBxGoToXY.TabStop = False
        Me.gpBxGoToXY.Text = "Go To XY"
        '
        'chkLatLong
        '
        Me.chkLatLong.AutoSize = True
        Me.chkLatLong.Location = New System.Drawing.Point(152, 88)
        Me.chkLatLong.Name = "chkLatLong"
        Me.chkLatLong.Size = New System.Drawing.Size(118, 24)
        Me.chkLatLong.TabIndex = 16
        Me.chkLatLong.Text = "CheckBox1"
        Me.chkLatLong.UseVisualStyleBackColor = True
        '
        'txtX
        '
        Me.txtX.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtX.Location = New System.Drawing.Point(59, 21)
        Me.txtX.Name = "txtX"
        Me.txtX.Size = New System.Drawing.Size(229, 26)
        Me.txtX.TabIndex = 13
        '
        'lblY
        '
        Me.lblY.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblY.Location = New System.Drawing.Point(5, 56)
        Me.lblY.Name = "lblY"
        Me.lblY.Size = New System.Drawing.Size(52, 20)
        Me.lblY.TabIndex = 15
        Me.lblY.Text = "Y:"
        Me.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtY
        '
        Me.txtY.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtY.Location = New System.Drawing.Point(59, 53)
        Me.txtY.Name = "txtY"
        Me.txtY.Size = New System.Drawing.Size(229, 26)
        Me.txtY.TabIndex = 14
        '
        'lblX
        '
        Me.lblX.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblX.Location = New System.Drawing.Point(5, 24)
        Me.lblX.Name = "lblX"
        Me.lblX.Size = New System.Drawing.Size(52, 20)
        Me.lblX.TabIndex = 14
        Me.lblX.Text = "X:"
        Me.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'btnRunXY
        '
        Me.btnRunXY.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRunXY.Location = New System.Drawing.Point(46, 85)
        Me.btnRunXY.Name = "btnRunXY"
        Me.btnRunXY.Size = New System.Drawing.Size(98, 29)
        Me.btnRunXY.TabIndex = 15
        Me.btnRunXY.Text = "Locate"
        Me.btnRunXY.UseVisualStyleBackColor = True
        '
        'btnAddress
        '
        Me.btnAddress.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnAddress.AutoCheck = False
        Me.btnAddress.BackgroundImage = Global.MobileControls.My.Resources.Resources.AddIntersect
        Me.btnAddress.FlatAppearance.BorderSize = 0
        Me.btnAddress.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnAddress.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAddress.Location = New System.Drawing.Point(79, 13)
        Me.btnAddress.Name = "btnAddress"
        Me.btnAddress.Size = New System.Drawing.Size(40, 40)
        Me.btnAddress.TabIndex = 11
        Me.btnAddress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnAddress.UseVisualStyleBackColor = True
        '
        'btnLookup
        '
        Me.btnLookup.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnLookup.AutoCheck = False
        Me.btnLookup.BackgroundImage = Global.MobileControls.My.Resources.Resources.Lookup
        Me.btnLookup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnLookup.Checked = True
        Me.btnLookup.CheckState = System.Windows.Forms.CheckState.Checked
        Me.btnLookup.FlatAppearance.BorderSize = 0
        Me.btnLookup.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLookup.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLookup.Location = New System.Drawing.Point(8, 13)
        Me.btnLookup.Name = "btnLookup"
        Me.btnLookup.Size = New System.Drawing.Size(40, 40)
        Me.btnLookup.TabIndex = 10
        Me.btnLookup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnLookup.UseVisualStyleBackColor = True
        '
        'btnGeocode
        '
        Me.btnGeocode.Appearance = System.Windows.Forms.Appearance.Button
        Me.btnGeocode.AutoCheck = False
        Me.btnGeocode.BackgroundImage = Global.MobileControls.My.Resources.Resources.Geocode
        Me.btnGeocode.FlatAppearance.BorderSize = 0
        Me.btnGeocode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnGeocode.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnGeocode.Location = New System.Drawing.Point(154, 13)
        Me.btnGeocode.Name = "btnGeocode"
        Me.btnGeocode.Size = New System.Drawing.Size(40, 40)
        Me.btnGeocode.TabIndex = 9
        Me.btnGeocode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.btnGeocode.UseVisualStyleBackColor = True
        '
        'pnResults
        '
        Me.pnResults.Controls.Add(Me.HScrollBar1)
        Me.pnResults.Controls.Add(Me.VScrollBar1)
        Me.pnResults.Controls.Add(Me.dgResults)
        Me.pnResults.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnResults.Location = New System.Drawing.Point(0, 52)
        Me.pnResults.Name = "pnResults"
        Me.pnResults.Size = New System.Drawing.Size(629, 856)
        Me.pnResults.TabIndex = 5
        Me.pnResults.TabStop = False
        '
        'HScrollBar1
        '
        Me.HScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.HScrollBar1.LargeChange = 2
        Me.HScrollBar1.Location = New System.Drawing.Point(3, 823)
        Me.HScrollBar1.Name = "HScrollBar1"
        Me.HScrollBar1.Size = New System.Drawing.Size(593, 30)
        Me.HScrollBar1.TabIndex = 2
        '
        'VScrollBar1
        '
        Me.VScrollBar1.Dock = System.Windows.Forms.DockStyle.Right
        Me.VScrollBar1.Location = New System.Drawing.Point(596, 16)
        Me.VScrollBar1.Name = "VScrollBar1"
        Me.VScrollBar1.Size = New System.Drawing.Size(30, 837)
        Me.VScrollBar1.TabIndex = 1
        '
        'dgResults
        '
        Me.dgResults.AllowUserToAddRows = False
        Me.dgResults.AllowUserToDeleteRows = False
        Me.dgResults.AllowUserToResizeRows = False
        Me.dgResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.dgResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgResults.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.dgResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgResults.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgResults.Location = New System.Drawing.Point(3, 16)
        Me.dgResults.MultiSelect = False
        Me.dgResults.Name = "dgResults"
        Me.dgResults.ReadOnly = True
        Me.dgResults.RowHeadersVisible = False
        Me.dgResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.White
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Black
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White
        Me.dgResults.RowsDefaultCellStyle = DataGridViewCellStyle2
        Me.dgResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgResults.Size = New System.Drawing.Size(623, 837)
        Me.dgResults.TabIndex = 0
        '
        'pnlResultsTools
        '
        Me.pnlResultsTools.Controls.Add(Me.btnDGRouteTo)
        Me.pnlResultsTools.Controls.Add(Me.btnDGFlash)
        Me.pnlResultsTools.Controls.Add(Me.btnDGZoomTo)
        Me.pnlResultsTools.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlResultsTools.Location = New System.Drawing.Point(0, 0)
        Me.pnlResultsTools.Name = "pnlResultsTools"
        Me.pnlResultsTools.Size = New System.Drawing.Size(629, 52)
        Me.pnlResultsTools.TabIndex = 4
        Me.pnlResultsTools.TabStop = False
        Me.pnlResultsTools.Visible = False
        '
        'btnDGRouteTo
        '
        Me.btnDGRouteTo.AutoSize = True
        Me.btnDGRouteTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDGRouteTo.Location = New System.Drawing.Point(135, 12)
        Me.btnDGRouteTo.Name = "btnDGRouteTo"
        Me.btnDGRouteTo.Size = New System.Drawing.Size(69, 34)
        Me.btnDGRouteTo.TabIndex = 3
        Me.btnDGRouteTo.Text = "Route"
        Me.btnDGRouteTo.UseVisualStyleBackColor = True
        '
        'btnDGFlash
        '
        Me.btnDGFlash.AutoSize = True
        Me.btnDGFlash.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDGFlash.Location = New System.Drawing.Point(284, 11)
        Me.btnDGFlash.Name = "btnDGFlash"
        Me.btnDGFlash.Size = New System.Drawing.Size(63, 34)
        Me.btnDGFlash.TabIndex = 1
        Me.btnDGFlash.Text = "Flash"
        Me.btnDGFlash.UseVisualStyleBackColor = True
        '
        'btnDGZoomTo
        '
        Me.btnDGZoomTo.AutoSize = True
        Me.btnDGZoomTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDGZoomTo.Location = New System.Drawing.Point(45, 12)
        Me.btnDGZoomTo.Name = "btnDGZoomTo"
        Me.btnDGZoomTo.Size = New System.Drawing.Size(70, 34)
        Me.btnDGZoomTo.TabIndex = 0
        Me.btnDGZoomTo.Text = "Show"
        Me.btnDGZoomTo.UseVisualStyleBackColor = True
        '
        'MobileSearch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.splContMainSearch)
        Me.Name = "MobileSearch"
        Me.Size = New System.Drawing.Size(629, 1606)
        Me.splContMainSearch.Panel1.ResumeLayout(False)
        Me.splContMainSearch.Panel2.ResumeLayout(False)
        Me.splContMainSearch.ResumeLayout(False)
        Me.pnlSearch.ResumeLayout(False)
        Me.pnlSearch.PerformLayout()
        CType(Me.picSearching, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlAddress.ResumeLayout(False)
        Me.gpInterSearch.ResumeLayout(False)
        Me.gpInterSearch.PerformLayout()
        Me.pnlAddressPoint.ResumeLayout(False)
        Me.pnlAddPntButtons.ResumeLayout(False)
        Me.pnlAddPntButtons.PerformLayout()
        Me.gpBxAddPntControlsLayer.ResumeLayout(False)
        Me.pnlGeocode.ResumeLayout(False)
        Me.pnlOnlineGC.ResumeLayout(False)
        Me.pnlOnlineGC.PerformLayout()
        Me.pnlSDCGC.ResumeLayout(False)
        Me.pnlSDCGC.PerformLayout()
        Me.pnlMyGC.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.pnlSearchOptions.ResumeLayout(False)
        Me.gpBxGoToXY.ResumeLayout(False)
        Me.gpBxGoToXY.PerformLayout()
        Me.pnResults.ResumeLayout(False)
        CType(Me.dgResults, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlResultsTools.ResumeLayout(False)
        Me.pnlResultsTools.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents splContMainSearch As System.Windows.Forms.SplitContainer
    Friend WithEvents pnlAddress As System.Windows.Forms.GroupBox
    Friend WithEvents pnlAddressPoint As System.Windows.Forms.GroupBox
    Friend WithEvents pnlGeocode As System.Windows.Forms.GroupBox
    Friend WithEvents pnlSearch As System.Windows.Forms.GroupBox
    Friend WithEvents chkSearchSimliar As System.Windows.Forms.CheckBox
    Friend WithEvents txtSearchValue As System.Windows.Forms.TextBox
    Friend WithEvents lblSearchValue As System.Windows.Forms.Label
    Friend WithEvents cboSearchField As System.Windows.Forms.ComboBox
    Friend WithEvents lblSearchField As System.Windows.Forms.Label
    Friend WithEvents cboSearchLayer As System.Windows.Forms.ComboBox
    Friend WithEvents lblSearchLayer As System.Windows.Forms.Label
    Friend WithEvents pnlSearchOptions As System.Windows.Forms.GroupBox
    Friend WithEvents pnResults As System.Windows.Forms.GroupBox
    Friend WithEvents dgResults As System.Windows.Forms.DataGridView
    Friend WithEvents pnlResultsTools As System.Windows.Forms.GroupBox
    Friend WithEvents btnDGFlash As System.Windows.Forms.Button
    Friend WithEvents btnDGZoomTo As System.Windows.Forms.Button
    Friend WithEvents cboStreetLayer1 As System.Windows.Forms.ComboBox
    Friend WithEvents lblAddressStreet1 As System.Windows.Forms.Label
    Friend WithEvents cboStreetRange As System.Windows.Forms.ComboBox
    Friend WithEvents cboStreetLayer2 As System.Windows.Forms.ComboBox
    Friend WithEvents rdoAddressStreet2 As System.Windows.Forms.RadioButton
    Friend WithEvents btnAddressRouteTo As System.Windows.Forms.Button
    Friend WithEvents btnAddressZoomTo As System.Windows.Forms.Button
    Friend WithEvents rdoAddressRange As System.Windows.Forms.RadioButton
    Friend WithEvents txtGeocodeValue As MyTextBox
    Friend WithEvents lblGeocode As System.Windows.Forms.Label
    Friend WithEvents btnRunGC As System.Windows.Forms.Button
    Friend WithEvents lblNumResults As System.Windows.Forms.Label
    Friend WithEvents btnAddress As System.Windows.Forms.CheckBox
    Friend WithEvents btnLookup As System.Windows.Forms.CheckBox
    Friend WithEvents btnGeocode As System.Windows.Forms.CheckBox
    Friend WithEvents btnAddressReload As System.Windows.Forms.Button
    Friend WithEvents VScrollBar1 As System.Windows.Forms.VScrollBar
    Friend WithEvents HScrollBar1 As System.Windows.Forms.HScrollBar
    Friend WithEvents btnDGRouteTo As System.Windows.Forms.Button
    Friend WithEvents pnlSDCGC As System.Windows.Forms.Panel
    Private WithEvents txtState As System.Windows.Forms.TextBox
    Private WithEvents txtCity As System.Windows.Forms.TextBox
    Private WithEvents txtStreet As System.Windows.Forms.TextBox
    Private WithEvents txtStreetNum As System.Windows.Forms.TextBox
    Private WithEvents btnLocateOnMap As System.Windows.Forms.Button
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents State As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
    Friend WithEvents pnlMyGC As System.Windows.Forms.Panel
    Friend WithEvents lblGCNumResults As System.Windows.Forms.Label
    Friend WithEvents gpBoxPreFiltInt As System.Windows.Forms.GroupBox
    Friend WithEvents gpInterSearch As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents gpBoxPreFiltSt As System.Windows.Forms.GroupBox
    Friend WithEvents gpBxGoToXY As System.Windows.Forms.GroupBox
    Friend txtX As MobileControls.MobileSearch.MyTextBox
    Friend WithEvents lblY As System.Windows.Forms.Label
    Friend txtY As MobileControls.MobileSearch.MyTextBox
    Friend WithEvents lblX As System.Windows.Forms.Label
    Friend WithEvents btnRunXY As System.Windows.Forms.Button
    Friend WithEvents btnAddressPnt As System.Windows.Forms.CheckBox
    Friend WithEvents gpBxAddPntControlsLayer As System.Windows.Forms.Panel
    Friend WithEvents gpBxAddPntControls As System.Windows.Forms.Panel

    Friend WithEvents pnlAddPntButtons As System.Windows.Forms.Panel
    Friend WithEvents lblMatchingResults As System.Windows.Forms.Label
    Friend WithEvents cboDrillDownLayer As System.Windows.Forms.ComboBox
    Friend WithEvents chkSearchField As System.Windows.Forms.CheckedListBox
    Friend WithEvents chkLatLong As System.Windows.Forms.CheckBox
    Friend WithEvents cboSearchValue As System.Windows.Forms.ComboBox
    Friend WithEvents picSearching As System.Windows.Forms.PictureBox
    Friend WithEvents btnSearchFind As System.Windows.Forms.Button
    Friend WithEvents cboBufferVal As System.Windows.Forms.ComboBox
    Friend WithEvents lblSearchGeo As System.Windows.Forms.Label
    Friend WithEvents btnBuffer As System.Windows.Forms.CheckBox
    Friend WithEvents pnlOnlineGC As System.Windows.Forms.GroupBox
    Friend WithEvents lblOnlineGCLabel As System.Windows.Forms.Label
    Friend txtbxOnlineGCAddress As MobileControls.MobileSearch.MyTextBox
    Friend WithEvents btnWaypoint As System.Windows.Forms.Button
    Friend WithEvents btnWaypointSearch As System.Windows.Forms.Button
    Friend WithEvents btnWaypointOnline As System.Windows.Forms.Button
    Friend WithEvents btnOnlineGC As System.Windows.Forms.Button
    Friend WithEvents btnWaypointGC As System.Windows.Forms.Button
    Friend WithEvents btnAddressPointZoomTo As System.Windows.Forms.Button
    Friend WithEvents btnWaypointDrillDown As System.Windows.Forms.Button

End Class
