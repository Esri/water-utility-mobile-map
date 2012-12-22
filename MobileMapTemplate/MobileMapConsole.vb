' | Version 10.1.1
' | Copyright 2012 Esri
' |
' | Licensed under the Apache License, Version 2.0 (the "License");
' | you may not use this file except in compliance with the License.
' | You may obtain a copy of the License at
' |
' |    http://www.apache.org/licenses/LICENSE-2.0
' |
' | Unless required by applicable law or agreed to in writing, software
' | distributed under the License is distributed on an "AS IS" BASIS,
' | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' | See the License for the specific language governing permissions and
' | limitations under the License.


Imports MobileControls
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGISTemplates.GlobalsFunctions
Imports Esri.ArcGISTemplates

Public Class MobileMapConsole
    Private m_pLnk As New frmLink

    Private m_TxtTimer As System.Windows.Forms.Timer
    Private m_tickCount As Integer = 0
    Private m_tickMessage As String = ""
    Private m_PermMessage As String = ""

    Private m_msgFont As Font = New System.Drawing.Font("Tahoma", 14.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_msgFontSmall As Font = New System.Drawing.Font("Tahoma", 12.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)

    Private m_msgFontLarge As Font = New System.Drawing.Font("Tahoma", 16.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_offset As Integer = 1
    Private m_msgX As Integer
    Private m_msgY As Integer
    Private m_msgBrush As Brush = Brushes.Red
    Private m_msgBrushFront As Brush = Brushes.White

    Private m_msgPermX As Integer
    Private m_msgPermY As Integer
    Private m_msgBrushPerm As Brush = Brushes.Orange
    Private m_msgBrushPermFront As Brush = Brushes.Black


    Private m_StatMessage As String = ""
    Private m_msgStatX As Integer
    Private m_msgStatY As Integer
    Private m_msgBrushStat As Brush = Brushes.Yellow
    Private m_msgBrushStatFront As Brush = Brushes.Black

    Private WithEvents m_MCIDMA As MobileControls.mobileIdentifyMapAction
    Private WithEvents m_MCService As MobileControls.MobileServiceSync
    Private WithEvents m_MCSearch As MobileControls.MobileSearch
    Private WithEvents m_MCCreateFeatureMA As MobileControls.EditFeaturesMapAction
    Private WithEvents m_MCActivityControl As MobileControls.AssignedWorkControl
    'Private WithEvents m_MCInspectMA As MobileControls.InspectMapAction
    Private WithEvents m_MCSketch As MobileControls.MobileSkecthPad
    'Private WithEvents m_MCTrace As MobileControls.NetworkTraceMobile
    'Private WithEvents m_MCGeonetTraceMA As MobileControls.GeometricNetworkTraceMapAction
    'Private WithEvents m_MCRoutingMA As MobileControls.NetworkRoutingMapAction
    Private WithEvents m_MCToggleGroup As MobileControls.mobileGroupToggle
    Private WithEvents m_MCALNInt As MobileControls.ALNIntegration
    'Private WithEvents m_MCMeasureMA As MobileControls.MeasureMapAction


    Private WithEvents m_MCNav As MobileControls.mobileNavigation
    Private m_MCToc As MobileControls.MobileTOC
    Private WithEvents m_MCWeb As MobileControls.MobileWebDisplay
    Private m_bPostingForTrace As Boolean = False
    Private bReload As Boolean = True

    Private m_RefreshWO As String = ""
    Private m_InspectionSourceLayer As String
    Private m_CrewName As String
    Private m_WOID As String
    Private m_AssetID As String
    Private m_LastInspectID As Integer
    Private m_FSplash As MobileMapConsoleSplash
    'Picture box to display sync indicator
    Private m_SyncIndicator As PictureBox
    Private m_HoveringMessageBox As Label

    Private m_Fnt As System.Drawing.Font
    Private m_FntSize As Single

#Region "Public Functions"
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        GlobalsFunctions.loadMapConfig()
        If GlobalsFunctions.appConfig Is Nothing Then

            Me.Close()

            Application.Exit()
            Exit Sub


        End If

        Dim pSetting As String = appConfig.ApplicationSettings.SplashLabel
        If pSetting = "" Then
            pSetting = "Mobile Map"
        End If
        Me.Text = pSetting

        m_FSplash = New MobileMapConsoleSplash
        m_FSplash.title = pSetting
        m_FSplash.copyrightInfo = appConfig.ApplicationSettings.CopyrightText
        m_FSplash.versionInfo = appConfig.ApplicationSettings.VersionInfo
        pSetting = ""
        pSetting = appConfig.ApplicationSettings.SplashImage

        If pSetting <> "" Then
            If System.IO.File.Exists(Application.StartupPath & "\AppImages\" & pSetting) Then
                Try


                    m_FSplash.BackgroundImage = New Bitmap(Application.StartupPath & "\AppImages\" & pSetting)
                Catch ex As Exception

                End Try
            End If

        End If
        pSetting = ""
        pSetting = appConfig.ApplicationSettings.AppImage

        If pSetting <> "" Then
            If System.IO.File.Exists(Application.StartupPath & "\AppImages\" & pSetting) Then
                Try


                    Me.Icon = New Icon(Application.StartupPath & "\AppImages\" & pSetting)
                Catch ex As Exception

                End Try
            End If

        End If
        'm_FSplash.Parent = Me
        m_FSplash.Show(Me)
        m_FSplash.Update()




        AddHandler spContMain.Panel1.VisibleChanged, AddressOf LeftPanelExpandCollapse


        Try


            'Set the panels to hold the controls to fill the parent

            pnlSync.Dock = DockStyle.Fill
            'Set the sync panel to visible
            pnlSync.Visible = True
            'Change the sync panel to red
            'showService()
            showPanel(TemplatePanels.Service)



            pnlID.Dock = DockStyle.Fill
            pnlID.Visible = False

            pnlToc.Dock = DockStyle.Fill
            pnlToc.Visible = False

            pnlRouting.Dock = DockStyle.Fill
            pnlRouting.Visible = False

            pnlSearch.Dock = DockStyle.Fill
            pnlSearch.Visible = False
            pnlTrace.Dock = DockStyle.Fill
            pnlTrace.Visible = False

            pnlWO.Visible = False
            pnlWO.Dock = DockStyle.Fill


            pnlSketch.Visible = False
            pnlSketch.Dock = DockStyle.Fill

            pnlCreateFeature.Visible = False
            pnlCreateFeature.Dock = DockStyle.Fill

            pnlInspect.Visible = False
            pnlInspect.Dock = DockStyle.Fill
            'Initilize the tools
            InitTools()
            m_MCService.refreshDataExtentStartup(True)


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        Finally
            m_FSplash.Visible = False
            m_FSplash.Dispose()
            m_FSplash = Nothing
            Me.WindowState = FormWindowState.Maximized

        End Try

        RemoveHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Resize, AddressOf spContControls_Resize

        spContMain.SplitterDistance = 443
        AddHandler spContMain.Resize, AddressOf spContControls_Resize
        AddHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        AddHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
    End Sub
#End Region
#Region "Private Functions"
    Private Sub InitTools()
        Try
            Map1.UseThreadedDrawing = True


            Dim intCol, intRow As Integer
            intCol = 0
            intRow = 0
            tblLayoutWidgets.Controls.Clear()
            tblLayoutWidgets.RowStyles.Clear()
            'Clear all the controls and map actions
            Map1.Controls.Clear()

            pnlID.Controls.Clear()
            pnlInspect.Controls.Clear()
            pnlCreateFeature.Controls.Clear()
            pnlSync.Controls.Clear()
            pnlSearch.Controls.Clear()
            pnlSketch.Controls.Clear()
            pnlToc.Controls.Clear()
            pnlRouting.Controls.Clear()
            pnlWO.Controls.Clear()
            pnlTrace.Controls.Clear()
            pnlWeb.Controls.Clear()
            pnlGeonetTrace.Controls.Clear()
            '  pnlMeasure.Controls.Clear()

            'Collapse the control panel
            spContMain.Panel1Collapsed = True
            'Set the open/close buttons image
            btnOpenClosePanel.BackgroundImage = My.Resources.ArrowRight









            'm_FntSize = GlobalsFunctions.FindConfigKey("FontSize")
            'm_Fnt = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)



            Dim pBtn As Button = Nothing



            'Service tools

            Try


                'Initilize the sync control 
                m_MCService = New MobileControls.MobileServiceSync
                'Set the map
                m_MCService.map = Map1
                If m_MCService.LoadSettings() = False Then
                   
                    Return

                End If

                m_MCService.Dock = DockStyle.Fill
                'Turn off the cache update panel
              
                If appConfig.ServicePanel.Visible.ToUpper() <> ("False").ToUpper() Then
                    'Add the controls to the sync panel
                    pnlSync.Controls.Add(m_MCService)

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnService"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8
                    If appConfig.ServicePanel.DisplayText IsNot Nothing Then
                        pBtn.Text = appConfig.ServicePanel.DisplayText
                    Else
                        pBtn.Text = "Service"
                    End If

                    pBtn.UseVisualStyleBackColor = True

                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If
                    tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                    intCol = intCol + 1


                    AddHandler pBtn.Click, AddressOf PanelButtonClick
                End If
            Catch ex As Exception
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing
                m_FSplash.Update()

            End Try
            'Nav
            Try
                m_MCNav = New MobileControls.mobileNavigation(Map1, True, True, True)

                'Set the zoom type from the config file
                If appConfig.NavigationOptions.ZoomPan.FixZoom.ToUpper() = "False".ToUpper Then
                    m_MCNav.ToggleFixZoom()
                End If

                'Set the GPS type from the config file
                If appConfig.NavigationOptions.GPS.Visible.ToUpper() = "False".ToUpper() Then
                    m_MCNav.ToggleGPSDisplay()
                Else

                    If appConfig.NavigationOptions.GPS.COMFile.Contains(".txt") Then
                        'Set the file GPS up
                        If System.IO.File.Exists(Application.StartupPath & "\" & appConfig.NavigationOptions.GPS.COMFile) Then
                            m_MCNav.SetGPSType(True, Application.StartupPath & "\" & appConfig.NavigationOptions.GPS.COMFile)
                        Else


                            m_FSplash.Update()

                            m_MCNav.SetGPSType(False)
                        End If

                    Else
                        m_MCNav.SetGPSType(False, appConfig.NavigationOptions.GPS.COMFile, appConfig.NavigationOptions.GPS.GPSBaud)

                    End If
                End If
                statTool.Text = appConfig.NavigationOptions.ZoomPan.PanToolMessage '"Active Tool: Pan"
            Catch ex As Exception
                Dim st As New StackTrace
                MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                st = Nothing
            End Try

            'ALN tools
            If GlobalsFunctions.appConfig.NavigationOptions.ArcLogisticsNavigator.Installed.ToUpper <> "False".ToUpper Then
                Try


                    m_MCALNInt = New MobileControls.ALNIntegration(Map1)
                Catch ex As Exception
                    MsgBox(GlobalsFunctions.appConfig.NavigationOptions.UIComponents.ALNNotInstalled)


                End Try


            End If

            If GlobalsFunctions.appConfig.SearchPanel.Visible.ToUpper() <> "False".ToUpper() Then

                Try

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnSearch"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8
                    If GlobalsFunctions.appConfig.SearchPanel.DisplayText <> "" Then
                        pBtn.Text = GlobalsFunctions.appConfig.SearchPanel.DisplayText
                    Else
                        pBtn.Text = "Search"
                    End If

                    pBtn.UseVisualStyleBackColor = True


                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If
                    tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                    intCol = intCol + 1


                    AddHandler pBtn.Click, AddressOf PanelButtonClick


                    'Create a new search control
                    m_MCSearch = New MobileControls.MobileSearch(Not (m_MCALNInt Is Nothing), appConfig.SearchPanel.SearchDataLoadType)
                    'Set it to fill the container
                    m_MCSearch.Dock = DockStyle.Fill
                    'Add it to the container
                    pnlSearch.Controls.Add(m_MCSearch)
                    'Set the map
                    m_MCSearch.map = Map1

                    'Initilize the search and address lookup
                    m_MCSearch.InitSearch()
                    If m_MCSearch.InitAddressLookup() = False Then
                        m_MCSearch.ToggleAddressSearchButtons(False) 'turn off add search
                    Else

                        If appConfig.SearchPanel.AddressSearch.PrefilterFields.PrefilterField.Count <> 0 Then
                            m_MCSearch.CreatePrefilterControls()
                        End If


                    End If

                    If m_MCSearch.InitAddressPointLookup() = False Then
                        m_MCSearch.disableAddPnt()
                    End If


                    m_MCSearch.LoadData()
                    If GlobalsFunctions.appConfig.SearchPanel.SearchXYOneLine IsNot Nothing Then


                        If GlobalsFunctions.appConfig.SearchPanel.SearchXYOneLine.ToUpper() = "TRUE" Then
                            m_MCSearch.ChangeXYFormat()

                        End If
                    End If

                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try

            End If
            ''TOC

            If GlobalsFunctions.appConfig.TOCPanel.Visible.ToUpper <> UCase("False") Then

                Try

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnToc"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8

                    If GlobalsFunctions.appConfig.TOCPanel.DisplayText IsNot Nothing Then
                        pBtn.Text = GlobalsFunctions.appConfig.TOCPanel.DisplayText
                    Else
                        pBtn.Text = "TOC"
                    End If

                    pBtn.UseVisualStyleBackColor = True

                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If
                    tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                    intCol = intCol + 1


                    AddHandler pBtn.Click, AddressOf PanelButtonClick


                    'Initilize the TOC control   
                    m_MCToc = New MobileControls.MobileTOC
                    'Set the map control
                    m_MCToc.Map = Map1
                    'set it to fill control
                    m_MCToc.Dock = DockStyle.Fill
                    'initlize the control
                    m_MCToc.refreshTOC()
                    'add the control to the panel
                    pnlToc.Controls.Add(m_MCToc)
                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try
            End If



            'Sketch PAD
            If GlobalsFunctions.appConfig.SketchPanel.Visible.ToUpper() <> UCase("False") Then

                Try

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnSketch"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8
                    If GlobalsFunctions.appConfig.SketchPanel.DisplayText IsNot Nothing Then
                        pBtn.Text = GlobalsFunctions.appConfig.SketchPanel.DisplayText
                    Else
                        pBtn.Text = "SketchPad"
                    End If


                    pBtn.UseVisualStyleBackColor = True


                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If
                    tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                    intCol = intCol + 1


                    AddHandler pBtn.Click, AddressOf PanelButtonClick

                    'Create the new sketch pad, pass in the panel to host it
                    m_MCSketch = New MobileControls.MobileSkecthPad(pnlSketch)

                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try


            End If
            Dim redAdded As Boolean = False
            'Redline/Note
            If GlobalsFunctions.appConfig.CreateFeaturePanel.Visible.ToUpper <> UCase("False") Then

                Try

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnNote"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8
                    If GlobalsFunctions.appConfig.CreateFeaturePanel.DisplayText IsNot Nothing Then
                        pBtn.Text = GlobalsFunctions.appConfig.CreateFeaturePanel.DisplayText
                    Else
                        pBtn.Text = "Note"
                    End If


                    pBtn.UseVisualStyleBackColor = True


                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If

                    'Create the Redline Map action
                    m_MCCreateFeatureMA = New MobileControls.EditFeaturesMapAction
                    m_MCCreateFeatureMA.ManualSetMap = Map1

                    'Add it to the map's map action collection
                    'Initilize the redline attribute form
                    If m_MCCreateFeatureMA.InitControl(pnlCreateFeature) = False Then

                    Else

                        'Add the button to toggle the redline map action
                        m_MCCreateFeatureMA.addMapButton()
                        tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                        intCol = intCol + 1
                        AddHandler pBtn.Click, AddressOf PanelButtonClick
                        redAdded = True
                    End If


                Catch ex As Exception
                    'Capture any errors
                    'MsgBox("Please install the prerequisite Tablet PC Edition Software Development Kit 1.7 (available for download from Microsoft)")
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try

            End If

            'ID MA
            If GlobalsFunctions.appConfig.IDPanel.Visible.ToUpper <> UCase("False") Then

                Try

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnInfo"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8
                    If GlobalsFunctions.appConfig.IDPanel.DisplayText IsNot Nothing Then
                        pBtn.Text = GlobalsFunctions.appConfig.IDPanel.DisplayText
                    Else
                        pBtn.Text = "Info"
                    End If

                    pBtn.UseVisualStyleBackColor = True


                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If
                    tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                    intCol = intCol + 1

                    AddHandler pBtn.Click, AddressOf PanelButtonClick
                    'Initlize the id map action
                    m_MCIDMA = New MobileControls.mobileIdentifyMapAction()
                    m_MCIDMA.ManualSetMap = Map1

                    'Add the map action to the map
                    'Init the ID form
                    m_MCIDMA.InitIDForm(pnlID, "", Not (m_MCALNInt Is Nothing))
                    'Add the ID button
                    m_MCIDMA.addIDButton()
                    'Set the default ID layer
                    'm_MCIDMA.IdentifyLayer = "wHydrants"
                    'Hide the ID layer box
                    'm_MCIDMA.showIDFormLayerBox(False)

                    If GlobalsFunctions.appConfig.WebPanel.WebInterceptHyperlinks.ToUpper() = "TRUE" Then
                        If GlobalsFunctions.appConfig.WebPanel.Visible.ToUpper = "TRUE" Then
                            m_MCIDMA.raiseHyperEvent = True
                        Else
                            ' MsgBox("You have elected to show hyperlinks in the built in web browser, but you have not enabled this control")

                        End If
                    End If



                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try
            End If

            'If UCase(#MEH#("Directions")) <> UCase("False") Then


            '    Try

            '        pBtn = New Button

            '        pBtn.BackgroundImage = My.Resources.Square___Blue
            '        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            '        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
            '        pBtn.FlatAppearance.BorderSize = 0
            '        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            '        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            '        pBtn.ForeColor = System.Drawing.Color.White
            '        pBtn.Location = New System.Drawing.Point(4, 4)
            '        pBtn.Margin = New System.Windows.Forms.Padding(1)
            '        pBtn.Name = "btnDirections"
            '        pBtn.Size = New System.Drawing.Size(105, 33)
            '        pBtn.TabIndex = 8
            '        If #MEH#("DirectionsName") IsNot Nothing Then
            '            pBtn.Text = #MEH#("DirectionsName")
            '        Else
            '            pBtn.Text = "Directions"
            '        End If

            '        pBtn.UseVisualStyleBackColor = True

            '        If intCol > 2 Then
            '            intRow = intRow + 1
            '            intCol = 0
            '        End If
            '        tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
            '        intCol = intCol + 1

            '        AddHandler pBtn.Click, AddressOf PanelButtonClick

            '        'create the routing Map Action
            '        m_MCRoutingMA = New MobileControls.NetworkRoutingMapAction
            '        'Add the routeing Map action to the map
            '        Map1.MapActions.Add(m_MCRoutingMA)
            '        'Initilize the routing form
            '        m_MCRoutingMA.InitRouting(pnlRouting)
            '        'Add the button to add stops 
            '        m_MCRoutingMA.addStopButton()
            '        'Hide the stop type box
            '        m_MCRoutingMA.showStopTypeLayerBox(False)
            '    Catch ex As Exception
            '        MsgBox("Error loading routing Controls" & vbCrLf & ex.Message)
            '    End Try

            'End If

            'Workorders
            If UCase(appConfig.WorkorderPanel.Visible) <> UCase("False") Then

                Try

                    If (appConfig.WorkorderPanel.WorkOrderFilters.WorkOrderFilter.Count > 0) Then


                        pBtn = New Button

                        pBtn.BackgroundImage = My.Resources.Square___Blue
                        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                        pBtn.FlatAppearance.BorderSize = 0
                        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                        pBtn.ForeColor = System.Drawing.Color.White
                        pBtn.Location = New System.Drawing.Point(4, 4)
                        pBtn.Margin = New System.Windows.Forms.Padding(1)
                        pBtn.Name = "btnActivities"
                        pBtn.Size = New System.Drawing.Size(105, 33)
                        pBtn.TabIndex = 8
                        If appConfig.WorkorderPanel.DisplayText IsNot Nothing Then
                            pBtn.Text = appConfig.WorkorderPanel.DisplayText
                        Else
                            pBtn.Text = "Activities"
                        End If


                        pBtn.UseVisualStyleBackColor = True


                        If intCol > 2 Then
                            intRow = intRow + 1
                            intCol = 0
                        End If
                        tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                        intCol = intCol + 1


                        AddHandler pBtn.Click, AddressOf PanelButtonClick

                        'Create the new activity control
                        m_MCActivityControl = New AssignedWorkControl(Map1)
                        'Set to dock it full
                        m_MCActivityControl.Dock = DockStyle.Fill
                        'Add the control to the panel to house it
                        pnlWO.Controls.Add(m_MCActivityControl)
                        'Initilize it with the map
                        'm_MCActivityControl.InitActivity(Map1, Not (m_MCALNInt Is Nothing))
                        'Load the activitoes
                        '  m_MCActivityControl.PopulateActivitys()
                    End If

                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try

            End If


            'If UCase(#MEH#("Measure")) <> UCase("False") Then

            '    'Inspection
            '    Try

            '        pBtn = New Button

            '        pBtn.BackgroundImage = My.Resources.Square___Blue
            '        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            '        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
            '        pBtn.FlatAppearance.BorderSize = 0
            '        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            '        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            '        pBtn.ForeColor = System.Drawing.Color.White
            '        pBtn.Location = New System.Drawing.Point(4, 4)
            '        pBtn.Margin = New System.Windows.Forms.Padding(1)
            '        pBtn.Name = "btnMeasure"
            '        pBtn.Size = New System.Drawing.Size(105, 33)
            '        pBtn.TabIndex = 8
            '        If #MEH#("MeasureName") IsNot Nothing Then
            '            pBtn.Text = #MEH#("MeasureName")
            '        Else
            '            pBtn.Text = "Measure"
            '        End If

            '        pBtn.UseVisualStyleBackColor = True


            '        If intCol > 2 Then
            '            intRow = intRow + 1
            '            intCol = 0
            '        End If
            '        tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
            '        intCol = intCol + 1


            '        AddHandler pBtn.Click, AddressOf PanelButtonClick
            '        'Create the inspection map action
            '        m_MCMeasureMA = New MobileControls.MeasureMapAction
            '        m_MCMeasureMA.MeasureMethod = m_MCMeasureMA.EsriMeasureMethod.Freehand
            '        m_MCMeasureMA.Font = m_Fnt
            '        m_MCMeasureMA.FontColor = Color.Red
            '        m_MCMeasureMA.DisplayedUnit = Map1.SpatialReference.Unit
            '        m_MCMeasureMA.LineColor = Color.Red
            '        m_MCMeasureMA.LineWidth = 6
            '        m_MCMeasureMA.ShowSegmentMeasures = True
            '        m_MCMeasureMA.SignificantDigits = 2

            '        'Add to the map
            '        Map1.MapActions.Add(m_MCMeasureMA)
            '        'Initilize the measure form
            '        'm_MCMeasureMA.InitMeasureForm(pnlMeasure)
            '        'Add the button to toggle the measure map action
            '        m_MCMeasureMA.addMeasureButton()
            '    Catch ex As Exception
            '        MsgBox("Error in init of measures control")
            '    End Try

            'End If


            'If UCase(appConfig.InspectionPanel.Visible) <> UCase("False") Then

            '    'Inspection
            '    Try
            '        MsgBox("This control is obselete, please move this function to the EditControl function")
            '        If 1 = 0 Then


            '            pBtn = New Button

            '            pBtn.BackgroundImage = My.Resources.Square___Blue
            '            pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            '            pBtn.Dock = System.Windows.Forms.DockStyle.Fill
            '            pBtn.FlatAppearance.BorderSize = 0
            '            pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            '            pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            '            pBtn.ForeColor = System.Drawing.Color.White
            '            pBtn.Location = New System.Drawing.Point(4, 4)
            '            pBtn.Margin = New System.Windows.Forms.Padding(1)
            '            pBtn.Name = "btnInspect"
            '            pBtn.Size = New System.Drawing.Size(105, 33)
            '            pBtn.TabIndex = 8
            '            If appConfig.InspectionPanel.DisplayText IsNot Nothing Then
            '                pBtn.Text = appConfig.InspectionPanel.DisplayText
            '            Else
            '                pBtn.Text = "Inspect"
            '            End If

            '            pBtn.UseVisualStyleBackColor = True


            '            If intCol > 2 Then
            '                intRow = intRow + 1
            '                intCol = 0
            '            End If
            '            tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
            '            intCol = intCol + 1


            '            AddHandler pBtn.Click, AddressOf PanelButtonClick
            '            'Create the inspection map action
            '            m_MCInspectMA = New MobileControls.InspectMapAction
            '            'Add to the map
            '            Map1.MapActions.Add(m_MCInspectMA)
            '            'Initilize the edit form
            '            m_MCInspectMA.InitEditForm(pnlInspect, redAdded)
            '        End If

            '    Catch ex As Exception

            '        'MsgBox("Please install the prerequisite Tablet PC Edition Software Development Kit 1.7 (available for download from Microsoft)")
            '    End Try

            'End If


            'If UCase(#MEH#("Isolate")) <> UCase("False") Then
            '    Try

            '        pBtn = New Button

            '        pBtn.BackgroundImage = My.Resources.Square___Blue
            '        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            '        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
            '        pBtn.FlatAppearance.BorderSize = 0
            '        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            '        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            '        pBtn.ForeColor = System.Drawing.Color.White
            '        pBtn.Location = New System.Drawing.Point(4, 4)
            '        pBtn.Margin = New System.Windows.Forms.Padding(1)
            '        pBtn.Name = "btnIsolate"
            '        pBtn.Size = New System.Drawing.Size(105, 33)
            '        pBtn.TabIndex = 8
            '        If #MEH#("IsolateName") IsNot Nothing Then
            '            pBtn.Text = #MEH#("IsolateName")
            '        Else
            '            pBtn.Text = "Isolate"
            '        End If


            '        pBtn.UseVisualStyleBackColor = True


            '        If intCol > 2 Then
            '            intRow = intRow + 1
            '            intCol = 0
            '        End If
            '        tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
            '        intCol = intCol + 1

            '        AddHandler pBtn.Click, AddressOf PanelButtonClick

            '        m_MCTrace = New MobileControls.NetworkTraceMobile(Map1)

            '        m_MCTrace.initTraceResults(pnlTrace)
            '    Catch ex As Exception
            '        MsgBox("Error loading Trace Controls" & vbCrLf & ex.Message)
            '    End Try

            'End If

            'If UCase(#MEH#("GeonetTrace")) <> UCase("False") Then
            '    Try

            '        pBtn = New Button

            '        pBtn.BackgroundImage = My.Resources.Square___Blue
            '        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
            '        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
            '        pBtn.FlatAppearance.BorderSize = 0
            '        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
            '        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            '        pBtn.ForeColor = System.Drawing.Color.White
            '        pBtn.Location = New System.Drawing.Point(4, 4)
            '        pBtn.Margin = New System.Windows.Forms.Padding(1)
            '        pBtn.Name = "btnGeonetTrace"
            '        pBtn.Size = New System.Drawing.Size(105, 33)
            '        pBtn.TabIndex = 8
            '        If #MEH#("GeonetTraceName") IsNot Nothing Then
            '            pBtn.Text = #MEH#("GeonetTraceName")
            '        Else
            '            pBtn.Text = "Trace"
            '        End If


            '        pBtn.UseVisualStyleBackColor = True


            '        If intCol > 2 Then
            '            intRow = intRow + 1
            '            intCol = 0
            '        End If
            '        tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
            '        intCol = intCol + 1

            '        AddHandler pBtn.Click, AddressOf PanelButtonClick
            '        'Initlize the Trace map action
            '        m_MCGeonetTraceMA = New MobileControls.GeometricNetworkTraceMapAction()
            '        'Add the map action to the map
            '        Map1.MapActions.Add(m_MCGeonetTraceMA)
            '        'Add the Trace button
            '        m_MCGeonetTraceMA.addTraceButton()
            '        'Init the Trace form
            '        m_MCGeonetTraceMA.InitTraceForm(pnlGeonetTrace)


            '    Catch ex As Exception
            '        MsgBox("Error loading Trace Controls" & vbCrLf & ex.Message)
            '    End Try

            'End If
            If appConfig.WebPanel.Visible.ToUpper <> UCase("False") Then

                Try

                    pBtn = New Button

                    pBtn.BackgroundImage = My.Resources.Square___Blue
                    pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                    pBtn.FlatAppearance.BorderSize = 0
                    pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                    pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                    pBtn.ForeColor = System.Drawing.Color.White
                    pBtn.Location = New System.Drawing.Point(4, 4)
                    pBtn.Margin = New System.Windows.Forms.Padding(1)
                    pBtn.Name = "btnWeb"
                    pBtn.Size = New System.Drawing.Size(105, 33)
                    pBtn.TabIndex = 8
                    If appConfig.WebPanel.DisplayText IsNot Nothing Then
                        pBtn.Text = appConfig.WebPanel.DisplayText
                    Else
                        pBtn.Text = "Web"
                    End If


                    pBtn.UseVisualStyleBackColor = True

                    If intCol > 2 Then
                        intRow = intRow + 1
                        intCol = 0
                    End If
                    tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                    intCol = intCol + 1


                    AddHandler pBtn.Click, AddressOf PanelButtonClick


                    'Initilize the web control   
                    m_MCWeb = New MobileControls.MobileWebDisplay
                    m_MCWeb.Dock = DockStyle.Fill

                    pnlWeb.Controls.Add(m_MCWeb)
                    pnlWeb.Dock = DockStyle.Fill

                    'Set the map control
                    m_MCWeb.initControl(Map1)
                    Try


                        m_MCWeb.initSites()
                    Catch ex As Exception

                    End Try

                    'set it to fill control
                    m_MCWeb.Dock = DockStyle.Fill

                    pnlWeb.Controls.Add(m_MCWeb)
                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try
            End If
            'Group Layer Toggle tools
            If UCase(appConfig.LayerOptions.ShowToggle) = UCase("True") Then
                Try


                    m_MCToggleGroup = New mobileGroupToggle(Map1)
                    m_MCToggleGroup.AddButtons(True, True, True, Map1)

                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try
            End If


            If UCase(appConfig.ApplicationSettings.MapExport.visible) = UCase("TRUE") Then
                btnTakeImage.Visible = True
            Else
                btnTakeImage.Visible = False
            End If

            If appConfig.ApplicationSettings.Extent IsNot Nothing Then

                If appConfig.ApplicationSettings.Extent.InitialExtent IsNot Nothing Then

                    Try
                        If GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.InitialExtent.xmax) And _
                       GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.InitialExtent.xmin) And _
                       GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.InitialExtent.ymax) And _
                       GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.InitialExtent.ymin) Then

                            Map1.Extent = (New Envelope(New Coordinate(appConfig.ApplicationSettings.Extent.InitialExtent.xmin, appConfig.ApplicationSettings.Extent.InitialExtent.ymin), New Coordinate(appConfig.ApplicationSettings.Extent.InitialExtent.xmax, appConfig.ApplicationSettings.Extent.InitialExtent.ymax)))


                        End If
                    Catch ex As Exception

                    End Try
                End If
                If appConfig.ApplicationSettings.Extent.fullExtent IsNot Nothing Then

                    Try
                        If GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.fullExtent.xmax) And _
                       GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.fullExtent.xmin) And _
                       GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.fullExtent.ymax) And _
                       GlobalsFunctions.IsNumeric(appConfig.ApplicationSettings.Extent.fullExtent.ymin) Then
                            Dim pen As Envelope = New Envelope(New Coordinate(appConfig.ApplicationSettings.Extent.fullExtent.xmin, appConfig.ApplicationSettings.Extent.fullExtent.ymin), New Coordinate(appConfig.ApplicationSettings.Extent.fullExtent.xmax, appConfig.ApplicationSettings.Extent.fullExtent.ymax))


                            Map1.FullExtent = pen


                        End If
                    Catch ex As Exception

                    End Try

                End If

            End If
            If intCol < 3 Then
                Select Case intCol
                    Case 1
                        If pBtn IsNot Nothing Then
                            tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                        Else
                            pBtn = New Button

                            pBtn.BackgroundImage = My.Resources.Square___Blue
                            pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                            pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                            pBtn.FlatAppearance.BorderSize = 0
                            pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                            pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                            pBtn.ForeColor = System.Drawing.Color.White
                            pBtn.Location = New System.Drawing.Point(4, 4)
                            pBtn.Margin = New System.Windows.Forms.Padding(1)
                            pBtn.Name = ""
                            pBtn.Size = New System.Drawing.Size(105, 33)
                            pBtn.TabIndex = 8
                            pBtn.Text = ""
                            pBtn.UseVisualStyleBackColor = True
                            tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                        End If
                        pBtn = New Button

                        pBtn.BackgroundImage = My.Resources.Square___Blue
                        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                        pBtn.FlatAppearance.BorderSize = 0
                        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                        pBtn.ForeColor = System.Drawing.Color.White
                        pBtn.Location = New System.Drawing.Point(4, 4)
                        pBtn.Margin = New System.Windows.Forms.Padding(1)
                        pBtn.Name = ""
                        pBtn.Size = New System.Drawing.Size(105, 33)
                        pBtn.TabIndex = 8
                        pBtn.Text = ""
                        pBtn.UseVisualStyleBackColor = True



                        tblLayoutWidgets.Controls.Add(pBtn, 0, intRow)
                        pBtn = New Button

                        pBtn.BackgroundImage = My.Resources.Square___Blue
                        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                        pBtn.FlatAppearance.BorderSize = 0
                        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                        pBtn.ForeColor = System.Drawing.Color.White
                        pBtn.Location = New System.Drawing.Point(4, 4)
                        pBtn.Margin = New System.Windows.Forms.Padding(1)
                        pBtn.Name = ""
                        pBtn.Size = New System.Drawing.Size(105, 33)
                        pBtn.TabIndex = 8
                        pBtn.Text = ""
                        pBtn.UseVisualStyleBackColor = True



                        tblLayoutWidgets.Controls.Add(pBtn, 2, intRow)

                    Case 2
                        If pBtn IsNot Nothing Then
                            tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                        Else
                            pBtn = New Button

                            pBtn.BackgroundImage = My.Resources.Square___Blue
                            pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                            pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                            pBtn.FlatAppearance.BorderSize = 0
                            pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                            pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                            pBtn.ForeColor = System.Drawing.Color.White
                            pBtn.Location = New System.Drawing.Point(4, 4)
                            pBtn.Margin = New System.Windows.Forms.Padding(1)
                            pBtn.Name = ""
                            pBtn.Size = New System.Drawing.Size(105, 33)
                            pBtn.TabIndex = 8
                            pBtn.Text = ""
                            pBtn.UseVisualStyleBackColor = True
                            tblLayoutWidgets.Controls.Add(pBtn, intCol, intRow)
                        End If
                        pBtn = New Button

                        pBtn.BackgroundImage = My.Resources.Square___Blue
                        pBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                        pBtn.Dock = System.Windows.Forms.DockStyle.Fill
                        pBtn.FlatAppearance.BorderSize = 0
                        pBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
                        pBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
                        pBtn.ForeColor = System.Drawing.Color.White
                        pBtn.Location = New System.Drawing.Point(4, 4)
                        pBtn.Margin = New System.Windows.Forms.Padding(1)
                        pBtn.Name = ""
                        pBtn.Size = New System.Drawing.Size(105, 33)
                        pBtn.TabIndex = 8
                        pBtn.Text = ""
                        pBtn.UseVisualStyleBackColor = True



                        tblLayoutWidgets.Controls.Add(pBtn, 1, intRow)

                    Case Else

                End Select

            End If



            tblLayoutWidgets.RowCount = intRow + 1 'UCase(#MEH#("rowCount"))
            '   tblLayoutWidgets.Height = (intRow + 1) * 33
            For i = 0 To intRow

                tblLayoutWidgets.RowStyles.Add(New RowStyle(SizeType.Percent, CSng(100 / (intRow + 1))))


                '   tblLayoutWidgets.RowStyles(i).SizeType = SizeType.AutoSize
                '  tblLayoutWidgets.RowStyles(i).Height = 100 / (intRow + 1)

            Next

            spContControls.SplitterDistance = (tblLayoutWidgets.RowCount * 33) + (tblLayoutWidgets.RowCount * 9)
            'MsgBox(spContMain.SplitterDistance)

            adjustimages(tblLayoutWidgets.Controls(0).Name)

            If UCase(appConfig.ApplicationSettings.SideBarControl) = "TRUE" Then
                gpBxSideBar.Visible = True
            Else
                gpBxSideBar.Visible = False

            End If
            CreateIndicator()
            CreateHoverMessage()
            'm_TxtTimer = New Timer
            m_TxtTimer = New System.Windows.Forms.Timer
            m_TxtTimer.Interval = 50
            'Add the handler for each tick
            AddHandler m_TxtTimer.Tick, AddressOf m_TxtTimer_Tick

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try


    End Sub
    Private Sub m_TxtTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs)
        'Preform the zoom on the ick
        'FixZoom(m_bZoomOut)
        If m_tickCount > 60 Then
            m_tickCount = 0
            m_tickMessage = ""
            Map1.Invalidate()
            resizeHoverMessage()

            ' m_HoveringMessageBox.Visible = False

        Else
            m_tickCount = m_tickCount + 1
            resizeHoverMessage()

        End If
    End Sub
    Private Sub resizeHoverMessage()
        Try
            If m_tickMessage <> "" Then
                Dim g As Graphics = Map1.CreateGraphics
                Dim sz As SizeF = g.MeasureString(m_tickMessage, m_msgFont)

                m_msgX = CInt((Map1.Width / 2)) - CInt((sz.Width / 2)) '- (m_HoveringMessageBox.Width / 2))


                m_msgY = CInt(Map1.Height - 55 - sz.Height)




                g = Nothing
                sz = Nothing


            Else
                m_msgX = CInt((Map1.Width / 2)) '- (m_HoveringMessageBox.Width / 2))
                m_msgY = CInt(Map1.Height - Map1.Height / 6)

            End If
            If m_PermMessage <> "" Then
                Dim g As Graphics = Map1.CreateGraphics
                Dim sz As SizeF = g.MeasureString(m_PermMessage, m_msgFontLarge)

                m_msgPermX = CInt((Map1.Width / 2)) - (sz.Width / 2) '- (m_HoveringMessageBox.Width / 2))


                m_msgPermY = CInt(Map1.Height - 25 - sz.Height)




                g = Nothing
                sz = Nothing


            Else
                m_msgPermX = CInt((Map1.Width / 2)) '- (m_HoveringMessageBox.Width / 2))
                m_msgPermY = CInt(Map1.Height - 25)

            End If

            If m_StatMessage <> "" Then
                Dim g As Graphics = Map1.CreateGraphics
                Dim sz As SizeF = g.MeasureString(m_StatMessage, m_msgFont)

                m_msgStatX = CInt((130)) '- (m_HoveringMessageBox.Width / 2))


                m_msgStatY = CInt(Map1.Height - 20 - sz.Height)




                g = Nothing
                sz = Nothing


            Else
                m_msgStatX = CInt((130)) '- (m_HoveringMessageBox.Width / 2))
                m_msgStatY = CInt(Map1.Height - 20)

            End If
            Map1.Invalidate()

            'If m_HoveringMessageBox Is Nothing Then Return
            ''Reposition the indictor when the map resizes
            'm_HoveringMessageBox.Left = CInt((Map1.Width / 2) - (m_HoveringMessageBox.Width / 2))
            'm_HoveringMessageBox.Top = CInt(Map1.Height - Map1.Height / 6 - m_HoveringMessageBox.Height / 2)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub CreateHoverMessage()
        Try


            ''Creates the sync indicator
            'm_HoveringMessageBox = New Label
            'Dim Fnt As Font = New System.Drawing.Font("Microsoft Sans Serif", 20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)

            'm_HoveringMessageBox.Font = Fnt


            ''m_HoveringMessageBox.BackColor = System.Drawing.Color.Transparent


            ''m_HoveringMessageBox.Margin = New System.Windows.Forms.Padding(0)
            'm_HoveringMessageBox.Name = "pcMessage"
            ''m_SyncIndicator.Size = New System.Drawing.Size(My.Resources.animated.Width, My.Resources.animated.Height)

            'm_HoveringMessageBox.Visible = False
            'resizeHoverMessage()
            'Map1.Controls.Add(m_HoveringMessageBox)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub resizeIndicator()
        Try
            If m_SyncIndicator Is Nothing Then Return
            'Reposition the indictor when the map resizes
            m_SyncIndicator.Left = CInt((Map1.Width / 2) - (m_SyncIndicator.Width / 2))
            m_SyncIndicator.Top = CInt(Map1.Height / 2 - m_SyncIndicator.Height / 2)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub CreateIndicator()
        Try
            'Creates the sync indicator
            m_SyncIndicator = New PictureBox
            m_SyncIndicator.BackColor = System.Drawing.Color.Transparent
            m_SyncIndicator.Image = My.Resources.animated


            m_SyncIndicator.Margin = New System.Windows.Forms.Padding(0)
            m_SyncIndicator.Name = "pcSync"
            m_SyncIndicator.Size = New System.Drawing.Size(My.Resources.animated.Width, My.Resources.animated.Height)

            m_SyncIndicator.Visible = False
            resizeIndicator()
            Map1.Controls.Add(m_SyncIndicator)
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Const WM_DISPLAYCHANGE As Integer = 126

    Protected Overloads Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_DISPLAYCHANGE
                'Me.Size = Me.ClientSize
                'FormRawMaterial.Show()
                ' MessageBox.Show("Screen resolution has changed")
                Exit Select
        End Select
        MyBase.WndProc(m)
    End Sub

    Public Function ScreenResolution() As String

        Dim intX As Integer = Screen.PrimaryScreen.Bounds.Width
        Dim intY As Integer = Screen.PrimaryScreen.Bounds.Height
        Return intX & " X " & intY
    End Function
    Private Sub adjustimages(ByVal activeBtn As String)
        Try


            'Change the target buttons image and reset the others
            For Each cnt As Control In tblLayoutWidgets.Controls
                If TypeOf cnt Is Button Then
                    If cnt.Name = activeBtn Then
                        cnt.BackgroundImage = My.Resources.Square___Red
                    Else

                        cnt.BackgroundImage = My.Resources.Square___Blue
                    End If

                End If
            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub HideControlPanel()
        spContMain.Panel1Collapsed = True
    End Sub
    Private Enum TemplatePanels
        Trace
        GeoNetTrace
        Workorder
        Sketch
        ID
        Redline
        Inspect
        Search
        Service
        Route
        Web
        '      Measure
        TOC
    End Enum
    Private Sub showPanel(ByVal MobilePanel As TemplatePanels)
        pnlSync.Visible = False
        pnlID.Visible = False
        pnlToc.Visible = False
        pnlWeb.Visible = False
        If m_MCWeb IsNot Nothing Then
            m_MCWeb.toggleWebDisplay(False)
        End If
        pnlSearch.Visible = False
        pnlWO.Visible = False
        pnlTrace.Visible = False
        pnlGeonetTrace.Visible = False
        pnlSketch.Visible = False
        pnlCreateFeature.Visible = False
        pnlInspect.Visible = False
        pnlRouting.Visible = False
        ' pnlMeasure.Visible = False
        RemoveHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Resize, AddressOf spContControls_Resize
        spContMain.Panel1Collapsed = False
        spContMain.SplitterDistance = 345
        AddHandler spContMain.Resize, AddressOf spContControls_Resize
        AddHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        AddHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        Select Case MobilePanel
            'Case TemplatePanels.GeoNetTrace
            '    pnlGeonetTrace.Visible = True
            '    statLabel.Text = "Network Trace:  Runs a server side trace"
            '    adjustimages("btnGeonetTrace")


            Case TemplatePanels.ID
                Try


                    If bReload Then
                        m_MCIDMA.reload()

                        bReload = False

                    End If


                    adjustimages("btnInfo")

                    statLabel.Text = appConfig.IDPanel.StatusBarMessage '"Information Tool:  Select an asset from the on screen drop down and click it to see the details"
                    statTool.Text = appConfig.IDPanel.ToolMessage '"Active Tool: Information Tool"
                    pnlID.Visible = True
                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                    Return
                End Try
                'Case TemplatePanels.Inspect
                '    Try

                '        'Show the inspection panel
                '        adjustimages("btnInspect")

                '        statLabel.Text = appConfig.InspectionPanel.StatusBarMessage '"Inspection Tool:  Select an inspection type and click the location to create the inspection"
                '        statTool.Text = appConfig.InspectionPanel.ToolMessage '"Active Tool: Create Inspection Tool"
                '        pnlInspect.Visible = True
                '    Catch ex As Exception
                '        Dim st As New StackTrace
                '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                '        st = Nothing
                '        Return

                '    End Try

                '    'Case TemplatePanels.Measure
                '    '    'Show the redline panel
                '    '    adjustimages("btnMeasure")
                '    '    statLabel.Text = "Measure the map"
                '    '    statTool.Text = "Active Tool: Measure"
                '    '    pnlMeasure.Visible = True
            Case TemplatePanels.Redline
                Try
                    'Show the redline panel
                    adjustimages("btnNote")
                    statLabel.Text = appConfig.CreateFeaturePanel.StatusBarMessage ' "Create A Note Tool:  Sketch a note on the screen and fill out the details on the left, click save to commit the note"
                    statTool.Text = appConfig.CreateFeaturePanel.ToolMessage '"Active Tool: Draw Note"
                    pnlCreateFeature.Visible = True
                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                    Return

                End Try
                'Case TemplatePanels.Route
                '    'Show the panel for routing
                '    adjustimages("btnDirections")

                '    statLabel.Text = "Get Directions:  Input locations to get turn by turn directions"
                '    statTool.Text = "Active Tool: Create Route Location"
                '    pnlRouting.Visible = True
            Case TemplatePanels.Search

                adjustimages("btnSearch") '.BackgroundImage = My.Resources.Square___Red
                statLabel.Text = appConfig.SearchPanel.StatusBarMessage '"Searching:  Search for an asset or address location"
                pnlSearch.Visible = True
            Case TemplatePanels.Service
                adjustimages("btnService")
                statLabel.Text = appConfig.ServicePanel.StatusBarMessage '"Data Services:  Post or get new data changes from your office's server"
                pnlSync.Visible = True
            Case TemplatePanels.Sketch
                adjustimages("btnSketch")

                statLabel.Text = appConfig.SketchPanel.StatusBarMessage '"Sketch Pad:  The sketch pad allows you to create a sketch and save it to an image"
                pnlSketch.Visible = True
            Case TemplatePanels.TOC

                adjustimages("btnToc")
                statLabel.Text = appConfig.TOCPanel.StatusBarMessage '"Layer List:  List all layers in your map"
                pnlToc.Visible = True
                'Case TemplatePanels.Trace
                '    statLabel.Text = "Valve Search:  Runs a server side trace to find the valves that need to be turned to isolate this main"
                '    adjustimages("btnIsolate")
                '    pnlTrace.Visible = True

            Case TemplatePanels.Web
                adjustimages("btnWeb")
                statLabel.Text = appConfig.WebPanel.StatusBarMessage '"Interface with web applications"
                pnlWeb.Visible = True
            Case TemplatePanels.Workorder

                adjustimages("btnActivities")

                statLabel.Text = appConfig.WorkorderPanel.StatusBarMessage '"Activitiy List:  Shows a list of assigned activities, opening a activity sets up an inspection"
                pnlWO.Visible = True
        End Select
    End Sub

    'Private Function LoadInspectionRecord(ByVal geo As Geometry) As Boolean
    '    Try
    '        'Get the inspection traget layer - to store the new inspection
    '        Dim pFlInspect as Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = m_MCInspectMA.GetInspectionTable(m_InspectionSourceLayer) 'GetFeatureSource(m_MCInspectMA.GetInspectionTableName(m_InspectionSourceLayer), Map1)

    '        If pFlInspect Is Nothing Then
    '            ' MsgBox("The inspection layer was not found, please select your inspection type")
    '            m_InspectionSourceLayer = ""
    '        Else
    '            'Create a new row 
    '            Dim pDr As FeatureDataRow = pFlInspect.GetDataTable().NewRow

    '            If m_AssetID = "" Then
    '                'If there is not an asset if of the layer to inspect
    '                If m_WOID = "" Then
    '                    'if the activity has no id

    '                    'Set the inspection up to the new row
    '                    m_MCInspectMA.SetCurrentRow(pDr)
    '                Else
    '                    'Get the activity associated with this 
    '                    Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(appConfig.ActivitiesPanel.ActivityLayer, Map1)
    '                    Dim pQFilt As New QueryFilter
    '                    If pFl.Columns("WORKID") IsNot Nothing Then
    '                        'Query for the activity
    '                        pQFilt.WhereClause = "WORKID" & " = '" & m_WOID & "'"
    '                        Dim pDRead As FeatureDataReader = pFl.GetDataReader(pQFilt)
    '                        If pDRead.ReadFirst Then
    '                            'Get the first record and set up the defaults for the inspection
    '                            If pDr.Table.Columns("WORKKEY") IsNot Nothing Then
    '                                pDr("WORKKEY") = m_WOID
    '                            End If
    '                            If pDr.Table.Columns("INSPECTOR") IsNot Nothing Then

    '                                pDr("INSPECTOR") = m_CrewName
    '                            End If
    '                            If pDr.Table.Columns("MTCENAME") IsNot Nothing Then

    '                                pDr("MTCENAME") = m_CrewName
    '                            End If
    '                            If pDr.Table.Columns("ASSIGNEDTO") IsNot Nothing Then

    '                                pDr("ASSIGNEDTO") = m_CrewName
    '                            End If
    '                            'If pDr.Table.Columns("INSSTART") IsNot Nothing Then

    '                            '    pDr("INSSTART") = Now
    '                            'End If
    '                            'If pDr.Table.Columns("LEAKSTART") IsNot Nothing Then

    '                            '    pDr("LEAKSTART") = Now
    '                            'End If
    '                            'Set the geometry
    '                            If CType(pDRead(pDRead.GeometryColumnIndex), Geometry).GeometryType = GeometryType.Point Then
    '                                pDr.Geometry = CType(pDRead(pDRead.GeometryColumnIndex), Geometry)
    '                            ElseIf geo IsNot Nothing Then
    '                                pDr.Geometry = geo
    '                            End If
    '                            'Set the row
    '                            m_MCInspectMA.SetCurrentRow(pDr)

    '                        Else
    '                            'Set the row
    '                            m_MCInspectMA.SetCurrentRow(pDr)

    '                        End If
    '                        'Close the reader
    '                        pDRead.Close()
    '                        pDRead = Nothing

    '                    Else
    '                        'Set the row
    '                        m_MCInspectMA.SetCurrentRow(pDr)
    '                    End If
    '                    pFl = Nothing
    '                    pQFilt = Nothing

    '                End If

    '            Else
    '                'Get the layer to inspect

    '                Dim pFl As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(m_InspectionSourceLayer, Map1)
    '                If pFl Is Nothing Then Return False
    '                Dim pQFilt As New QueryFilter
    '                Dim strQuery As String = ""
    '                If pFl.Columns("WWW_ID") IsNot Nothing Then
    '                    If pFl.Columns("WWW_ID").DataType Is System.Type.GetType("System.String") Then
    '                        strQuery = "WWW_ID" & " = '" & m_AssetID & "'"
    '                    Else
    '                        strQuery = "WWW_ID" & " = " & m_AssetID
    '                    End If

    '                End If

    '                If pFl.Columns("FACILITYID") IsNot Nothing Then
    '                    If pFl.Columns("FACILITYID").DataType Is System.Type.GetType("System.String") Then
    '                        strQuery = "FACILITYID" & " = '" & m_AssetID & "'"
    '                    Else
    '                        strQuery = "FACILITYID" & " = " & m_AssetID
    '                    End If

    '                End If
    '                If strQuery = "" Then

    '                    If pFl.Columns("NAME") IsNot Nothing Then
    '                        If pFl.Columns("NAME").DataType Is System.Type.GetType("System.String") Then

    '                            If strQuery = "" Then
    '                                strQuery = "NAME" & " = '" & m_AssetID & "'"
    '                            Else
    '                                strQuery = strQuery & " OR " & "NAME" & " = '" & m_AssetID & "'"
    '                            End If
    '                        ElseIf IsNumeric(m_AssetID) Then

    '                            If strQuery = "" Then
    '                                strQuery = "NAME" & " = " & m_AssetID
    '                            Else
    '                                strQuery = strQuery & " OR " & "NAME" & " = " & m_AssetID
    '                            End If
    '                        End If

    '                    End If
    '                End If



    '                If strQuery = "" Then

    '                    If UCase(pFl.DisplayColumnName) <> "NAME" And UCase(pFl.DisplayColumnName) <> "FACILITYID" Then
    '                        If pFl.Columns(pFl.DisplayColumnName).DataType Is System.Type.GetType("System.String") Then

    '                            If strQuery = "" Then
    '                                strQuery = pFl.DisplayColumnName & " = '" & m_AssetID & "'"
    '                            Else
    '                                strQuery = strQuery & " OR " & pFl.DisplayColumnName & " = '" & m_AssetID & "'"
    '                            End If
    '                        ElseIf IsNumeric(m_AssetID) Then
    '                            If strQuery = "" Then
    '                                strQuery = pFl.DisplayColumnName & " = " & m_AssetID
    '                            Else
    '                                strQuery = strQuery & " OR " & pFl.DisplayColumnName & " = " & m_AssetID
    '                            End If
    '                        End If

    '                    End If
    '                End If

    '                'Query for feature that is being inspected
    '                If strQuery <> "" Then

    '                    pQFilt.WhereClause = strQuery
    '                    Dim pDRead As FeatureDataReader
    '                    If pFl.GetFeatureCount(pQFilt) = 0 Then


    '                        strQuery = pFl.FidColumnName & " = " & m_AssetID
    '                        pQFilt.WhereClause = strQuery
    '                        pDRead = pFl.GetDataReader(pQFilt)
    '                    Else
    '                        pDRead = pFl.GetDataReader(pQFilt)
    '                    End If
    '                    pDRead = pFl.GetDataReader(pQFilt)
    '                    If pDRead.ReadFirst Then
    '                        For Each pDCSource As DataColumn In pFl.Columns
    '                            Try

    '                                If pFlInspect.Columns(pDCSource.ColumnName) IsNot Nothing Then

    '                                    If pDCSource.ColumnName.ToUpper <> pFlInspect.GeometryColumnName.ToUpper And _
    '                                        pDCSource.ColumnName.ToUpper <> pFlInspect.FidColumnName.ToUpper And _
    '                                        pDCSource.ColumnName.ToUpper <> pFlInspect.GlobalIdColumnName.ToUpper And _
    '                                        pDCSource.ColumnName <> "Shape.Len" And _
    '                                        pDCSource.ColumnName <> "Shape_Len" And _
    '                                        pDCSource.ColumnName <> "Shape.Length" And _
    '                                        pDCSource.ColumnName <> "Shape_Length" And _
    '                                        pDCSource.ColumnName <> "Shape.Area" And _
    '                                        pDCSource.ColumnName <> "Shape_Area" Then
    '                                        pDr(pDCSource.ColumnName) = pDRead(pDCSource.ColumnName)
    '                                    Else
    '                                        '  MsgBox(pDCSource.ColumnName)
    '                                    End If

    '                                End If
    '                            Catch ex As Exception

    '                            End Try

    '                        Next
    '                        'Get the first record and set the deafults
    '                        If pDr.Table.Columns("FACILITYKEY") IsNot Nothing And pFl.Columns("FACILITYID") IsNot Nothing Then
    '                            pDr("FACILITYKEY") = pDRead("FACILITYID")
    '                        ElseIf pDr.Table.Columns("FACILITYKEY") IsNot Nothing And pFl.Columns("NAME") IsNot Nothing Then
    '                            pDr("FACILITYKEY") = pDRead("NAME")
    '                        ElseIf pDr.Table.Columns("FACILITYKEY") IsNot Nothing And pFl.Columns("WWW_ID") IsNot Nothing Then
    '                            pDr("FACILITYKEY") = pDRead("WWW_ID")
    '                        ElseIf pDr.Table.Columns("FACILITYKEY") IsNot Nothing Then
    '                            pDr("FACILITYKEY") = pDRead(pFl.DisplayColumnName)

    '                        End If

    '                        If pDr.Table.Columns("WORKKEY") IsNot Nothing Then
    '                            pDr("WORKKEY") = m_WOID
    '                        End If
    '                        If pDr.Table.Columns("WORKID") IsNot Nothing Then
    '                            pDr("WORKID") = m_WOID
    '                        End If
    '                        If pDr.Table.Columns("MTCENAME") IsNot Nothing Then

    '                            pDr("MTCENAME") = m_CrewName
    '                        End If


    '                        If pDr.Table.Columns("INSPECTOR") IsNot Nothing Then

    '                            pDr("INSPECTOR") = m_CrewName
    '                        End If
    '                        If pDr.Table.Columns("ASSIGNEDTO") IsNot Nothing Then

    '                            pDr("ASSIGNEDTO") = m_CrewName
    '                        End If

    '                        'If pDr.Table.Columns("INSSTART") IsNot Nothing Then

    '                        '    pDr("INSSTART") = Now
    '                        'End If
    '                        'If pDr.Table.Columns("LEAKSTART") IsNot Nothing Then

    '                        '    pDr("LEAKSTART") = Now
    '                        'End If
    '                        'Set the gep
    '                        If CType(pDRead(pDRead.GeometryColumnIndex), Geometry).GeometryType = GeometryType.Point Then
    '                            pDr.Geometry = CType(pDRead(pDRead.GeometryColumnIndex), Geometry)
    '                        ElseIf geo IsNot Nothing Then
    '                            pDr.Geometry = geo

    '                        End If

    '                        'Set the inspection row
    '                        m_MCInspectMA.SetCurrentRow(pDr)
    '                        pDRead.Close()
    '                        pDRead = Nothing
    '                    Else
    '                        pDRead.Close()
    '                        pDRead = Nothing
    '                        If m_WOID = "" Then
    '                            'Set the inspection row
    '                            m_MCInspectMA.SetCurrentRow(pDr)
    '                        Else
    '                            'Get the activity FeatureSource
    '                            pFl = GlobalsFunctions.GetFeatureSource(appConfig.ActivitiesPanel.ActivityLayer, Map1)
    '                            pQFilt = New QueryFilter
    '                            'Query for the record
    '                            pQFilt.WhereClause = "WORKID" & " = '" & m_WOID & "'"
    '                            pDRead = pFl.GetDataReader(pQFilt)
    '                            If pDRead.ReadFirst Then
    '                                'Get the first record
    '                                If pDr.Table.Columns("WORKKEY") IsNot Nothing Then
    '                                    pDr("WORKKEY") = m_WOID
    '                                End If
    '                                If pDr.Table.Columns("MTCENAME") IsNot Nothing Then

    '                                    pDr("MTCENAME") = m_CrewName
    '                                End If
    '                                If pDr.Table.Columns("ASSIGNEDTO") IsNot Nothing Then

    '                                    pDr("ASSIGNEDTO") = m_CrewName
    '                                End If
    '                                If pDr.Table.Columns("INSPECTOR") IsNot Nothing Then

    '                                    pDr("INSPECTOR") = m_CrewName
    '                                End If

    '                                'If pDr.Table.Columns("INSSTART") IsNot Nothing Then

    '                                '    pDr("INSSTART") = Now
    '                                'End If
    '                                'If pDr.Table.Columns("LEAKSTART") IsNot Nothing Then

    '                                '    pDr("LEAKSTART") = Now
    '                                'End If
    '                                'Set the geometry
    '                                If CType(pDRead(pDRead.GeometryColumnIndex), Geometry).GeometryType = GeometryType.Point Then
    '                                    pDr.Geometry = CType(pDRead(pDRead.GeometryColumnIndex), Geometry)
    '                                ElseIf geo IsNot Nothing Then
    '                                    pDr.Geometry = geo
    '                                End If
    '                                'Set the inspection row
    '                                m_MCInspectMA.SetCurrentRow(pDr)

    '                            Else
    '                                'Set the inspection row
    '                                m_MCInspectMA.SetCurrentRow(pDr)
    '                            End If
    '                            'Close the reader
    '                            pDRead.Close()
    '                            pDRead = Nothing
    '                        End If

    '                    End If
    '                Else
    '                    'Set the inspection row
    '                    m_MCInspectMA.SetCurrentRow(pDr)
    '                End If
    '                'Clean up
    '                pFl = Nothing
    '                pQFilt = Nothing

    '            End If

    '            pDr = Nothing
    '        End If
    '        'If Map1.MapAction IsNot m_MCInspectMA Then
    '        '    Map1.MapAction = Nothing

    '        '    'click the inspect button to hightlight the form
    '        '    Map1.MapAction = m_MCInspectMA

    '        'End If

    '        Map1.MapAction = Nothing

    '        '    'click the inspect button to hightlight the form
    '        Map1.MapAction = m_MCInspectMA

    '        pFlInspect = Nothing

    '    Catch ex As Exception
    '        Return False
    '    End Try


    '    Return True

    'End Function

#End Region
#Region "Events"

    Private Sub spContMain_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spContMain.Resize
        Try

            RemoveHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
            RemoveHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
            RemoveHandler spContMain.Resize, AddressOf spContControls_Resize

            spContMain.SplitterDistance = 345
            AddHandler spContMain.Resize, AddressOf spContControls_Resize
            AddHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
            AddHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
            'MsgBox(spContMain.SplitterDistance)

        Catch ex As Exception

        End Try
    End Sub

    Private Sub m_MCIDMA_HyperClick(ByVal PathToFile As String) Handles m_MCIDMA.HyperClick
        If m_MCWeb IsNot Nothing Then
            ' showWEB()
            showPanel(TemplatePanels.Web)

            m_MCWeb.AddSite(PathToFile, True)


        Else
            System.Diagnostics.Process.Start(PathToFile)

        End If
    End Sub

    Private Sub m_MCIDMA_LocationIdentified(ByVal fdr As Esri.ArcGIS.Mobile.FeatureCaching.FeatureDataRow) Handles m_MCIDMA.LocationIdentified


        'If m_MCInspectMA.IsInspectionFeature(fdr) = True Then
        '    Map1.SuspendOnPaint = True

        '    m_MCInspectMA.SetCurrentRow(fdr)

        '    Map1.MapAction = m_MCInspectMA
        '    '  Map1.Pan(1, 1)
        '    ' Map1.Refresh

        '    Map1.Parent.Refresh()
        '    Map1.SuspendOnPaint = False
        '    Map1.ResumeLayout(True)
        '    Map1.Refresh()


        '    'set the status lable and status tool to info as the call to showPanel() switches these values to identify
        '    'statLabel.Text = appConfig.IDPanel.StatusBarMessage '"Information Tool:  Select an asset from the on screen drop down and click it to see the details"
        '    'statTool.Text = appConfig.IDPanel.ToolMessage '"Active Tool: Information Tool"
        'End If


    End Sub

    'Private Sub m_MCIDMA_RouteTo(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String) Handles m_MCIDMA.RouteTo, m_MCSearch.RouteTo, m_MCActivityControl.RouteTo


    '    Try

    '        If m_MCALNInt IsNot Nothing Then


    '            Dim pGPSDetails As GPSLocationDetails = m_MCNav.getGPS

    '            m_MCALNInt.RouteToGeo(getGeomCenter(location), LocationName, LocationName, New Coordinate(pGPSDetails.Longitude, pGPSDetails.Latitude))
    '            pGPSDetails = Nothing

    '        End If
    '    Catch ex As Exception

    '    End Try

    'End Sub


    Private Sub m_MCIDMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_MCIDMA.StatusChanged
        Try


            If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
                'show the ID panel if the Map Action has been activated
                '                showIDPanel()
                showPanel(TemplatePanels.ID)

            ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then


            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub showMessage(ByVal message As String)
        'm_HoveringMessageBox.Text = message
        'm_HoveringMessageBox.Visible = True
        m_tickMessage = message
        resizeHoverMessage()

        m_tickCount = 1
        m_TxtTimer.Start()

    End Sub
    Private Sub showPermMessage(ByVal message As String, ByVal hide As Boolean)
        m_PermMessage = message
        resizeHoverMessage()


    End Sub
    Private Sub showStatMessage(ByVal message As String, ByVal hide As Boolean)
        m_StatMessage = message
        resizeHoverMessage()


    End Sub
    'Private Sub m_MCGeonetTraceMA_showIndicator(ByVal state As Boolean) Handles m_MCGeonetTraceMA.showIndicator
    '    m_SyncIndicator.Visible = state
    'End Sub
    'Private Sub m_MCGeonetTraceMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_MCGeonetTraceMA.StatusChanged
    '    Try


    '        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
    '            'show the Geonet trace panel if the Map Action has been activated
    '            'showGeonetTracePanel()
    '            showPanel(TemplatePanels.GeoNetTrace)

    '        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then


    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error in the Geonet trace MA status change" & vbCrLf & ex.Message)
    '    End Try
    'End Sub

    'Private Sub m_MCInspectMA_checkGPS() Handles m_MCInspectMA.checkGPS
    '    callingGPSMod = "Inspect"

    '    m_MCNav.getGPSAsync()



    'End Sub

    'Private Sub m_MCInspectMA_InspectionChanged(ByVal InspectionLayer As String, ByVal LayerInspecting As String) Handles m_MCInspectMA.InspectionChanged
    '    m_InspectionSourceLayer = LayerInspecting
    'End Sub
    'Private Sub m_MCInspectMA_RecordSaved(ByVal LayerName As String, ByVal pGeo As ESRI.ArcGIS.Mobile.Geometries.Geometry, ByVal OID As Integer) Handles m_MCInspectMA.RecordSaved
    '    Try
    '        m_LastInspectID = OID
    '        'If it is a leak inspection, prompt the user to run the geometric network trace
    '        'If LayerName = "Leaks" And UCase(#MEH#("Isolate")) = UCase("True") Then

    '        '    If (MsgBox("Do you want to Isolate this main?", MsgBoxStyle.YesNo)) = MsgBoxResult.Yes Then
    '        '        'Clear the last results
    '        '        m_MCTrace.ClearLastResult()
    '        '        'Set to trace and isolate
    '        '        m_MCTrace.m_restoreOnly = False
    '        '        'Run the trace
    '        '        m_MCTrace.RunAt(CType(pGeo, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate)

    '        '    Else
    '        '        Map1.Invalidate()
    '        '    End If

    '        'Else
    '        Map1.Invalidate()
    '        'End If
    '        'Load the inspection record
    '        If m_InspectionSourceLayer <> "" Then
    '            LoadInspectionRecord(Nothing)
    '        End If
    '        If m_WOID <> "" Then
    '            '   showWOPanel()
    '            showPanel(TemplatePanels.Workorder)

    '        End If
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing
    '    End Try
    'End Sub



    'Private Sub m_MCInspectMA_RestoreService(ByVal coordinate As Coordinate) Handles m_MCInspectMA.RestoreService
    '    Try
    '        If m_MCTrace Is Nothing Then
    '            MsgBox("The tracing function is not part of this application")
    '            Return

    '        End If
    '        'Clear the last results
    '        m_MCTrace.ClearLastResult()
    '        'set to restore an isolated main
    '        m_MCTrace.RestoreOnly = True
    '        'run the trace to get list of valves to restore
    '        m_MCTrace.RunAt(coordinate)
    '    Catch ex As Exception
    '        MsgBox("Error in the Inspect MA RestoreService event" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
    'Private Sub m_MCInspectMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_MCInspectMA.StatusChanged
    '    Try


    '        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
    '            'Show the inspection form
    '            '                showInspectForm()
    '            showPanel(TemplatePanels.Inspect)

    '        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then

    '        End If
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing
    '    End Try
    'End Sub

    'Private Sub m_MCRoutingMA_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_MCRoutingMA.StatusChanged
    '    Try


    '        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
    '            'Show the routing form
    '            'showRoutePanel()
    '            showPanel(TemplatePanels.Route)

    '        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then

    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error in the Routing MA status change event" & vbCrLf & ex.Message)
    '    End Try
    'End Sub


    Private Sub PanelButtonClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Select Case UCase(CType(sender, Button).Name)
            Case UCase("btnService")
                'showService()
                showPanel(TemplatePanels.Service)

            Case UCase("btnSearch")
                'showSearchFrom()
                showPanel(TemplatePanels.Search)

            Case UCase("btnInfo")
                Try


                    If Map1.MapAction Is m_MCIDMA Then
                        'Show the ID Panel
                        '       showIDPanel()
                        showPanel(TemplatePanels.ID)

                    Else
                        'Set the map action
                        Map1.MapAction = m_MCIDMA
                    End If

                Catch ex As Exception
                    Dim st As New StackTrace
                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                    st = Nothing
                End Try
                'Case UCase("btnMeasure")
                '    Try


                '        If Map1.MapAction Is m_MCMeasureMA Then
                '            'Show the ID Panel
                '            '       showIDPanel()
                '            showPanel(TemplatePanels.Measure)

                '        Else
                '            'Set the map action
                '            Map1.MapAction = m_MCIDMA
                '        End If

                '    Catch ex As Exception
                '        MsgBox("Error setting the ID Map Action" & vbCrLf & ex.Message)
                '    End Try

            Case UCase("btnToc")
                'showTOC()
                showPanel(TemplatePanels.TOC)

            Case UCase("btnWeb")
                'showWEB()
                showPanel(TemplatePanels.Web)

            Case UCase("btnActivities")
                'showWOPanel()
                showPanel(TemplatePanels.Workorder)

            Case UCase("btnNote")
                If Map1.MapAction Is m_MCCreateFeatureMA Then
                    'Show the ID Panel
                    'showRedlineForm()
                    showPanel(TemplatePanels.Redline)

                Else
                    'Set the map action
                    Map1.MapAction = m_MCCreateFeatureMA
                End If
            Case UCase("btnSketch")
                'showSketchPanel()
                showPanel(TemplatePanels.Sketch)

                'Case UCase("btnIsolate")
                '    'showTracePanel()
                '    showPanel(TemplatePanels.Trace)

                'Case UCase("btnGeonetTrace")
                '    'showGeonetTracePanel()
                '    showPanel(TemplatePanels.GeoNetTrace)
                '    Try


                '        If Map1.MapAction Is m_MCGeonetTraceMA Then
                '            'Show the ID Panel
                '            '       showIDPanel()
                '            showPanel(TemplatePanels.GeoNetTrace)

                '        Else
                '            'Set the map action
                '            Map1.MapAction = m_MCGeonetTraceMA
                '        End If

                '    Catch ex As Exception
                '        MsgBox("Error setting the m_MCGeonetTraceMA Map Action" & vbCrLf & ex.Message)
                '    End Try
                'Case UCase("btnInspect")

                '    If Map1.MapAction Is m_MCInspectMA Then
                '        'Show the ID Panel
                '        'showInspectForm()
                '        showPanel(TemplatePanels.Inspect)

                '    Else
                '        'Set the map action
                '        Map1.MapAction = m_MCInspectMA
                '    End If
                '    'Case UCase("btnDirections")
                '    '    'Assign the map action
                '    '    If Map1.MapAction IsNot m_MCRoutingMA Then
                '    '        Map1.MapAction = m_MCRoutingMA

                '    '    End If
                '    '    'Show the route panel
                '    '    'showRoutePanel()
                '    '    showPanel(TemplatePanels.Route)

            Case Else

        End Select

    End Sub


    Private Sub LeftPanelExpandCollapse(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Toggle the left panel
        If spContMain.Panel1Collapsed = True Then
            btnOpenClosePanel.BackgroundImage = My.Resources.ArrowRight
        Else
            btnOpenClosePanel.BackgroundImage = My.Resources.ArrowLeft
        End If

    End Sub
    Private Sub btnOpenClosePanel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenClosePanel.Click

    End Sub
    Private Sub btnOpenClosePanel_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles btnOpenClosePanel.MouseDown
        'RemoveHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        'RemoveHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        'RemoveHandler spContMain.Resize, AddressOf spContControls_Resize

        'spContMain.SplitterDistance = 345
        'AddHandler spContMain.Resize, AddressOf spContControls_Resize
        'AddHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        'AddHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        If e.Button = Windows.Forms.MouseButtons.Left Then
            spContMain.Panel1Collapsed = Not spContMain.Panel1Collapsed
            'Else
            '    spContMain.Panel2Collapsed = True
            '    spContMain.Panel1Collapsed = False


            'spContMain.SplitterDistance = 345
        End If
        RemoveHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Resize, AddressOf spContControls_Resize

        spContMain.SplitterDistance = 345
        AddHandler spContMain.Resize, AddressOf spContControls_Resize
        AddHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        AddHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        ''MsgBox(spContMain.SplitterDistance)

    End Sub

    Private Sub Map1_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Map1.Paint
        If m_tickCount > 0 And m_tickMessage <> "" Then

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset, m_msgY - m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset - m_offset, m_msgY - m_offset - m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset, m_msgY - m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset + m_offset, m_msgY - m_offset - m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset, m_msgY + m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX - m_offset - m_offset, m_msgY + m_offset + m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset + m_offset, m_msgY + m_offset + m_offset)
            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrush, m_msgX + m_offset, m_msgY + m_offset)

            e.Graphics.DrawString(m_tickMessage, m_msgFont, m_msgBrushFront, m_msgX, m_msgY)
        End If

        If m_PermMessage <> "" Then

            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX - m_offset, m_msgPermY - m_offset)
            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX - m_offset - m_offset, m_msgPermY - m_offset - m_offset)

            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX + m_offset, m_msgPermY - m_offset)
            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX + m_offset + m_offset, m_msgPermY - m_offset - m_offset)

            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX - m_offset, m_msgPermY + m_offset)
            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX - m_offset - m_offset, m_msgPermY + m_offset + m_offset)

            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX + m_offset + m_offset, m_msgPermY + m_offset + m_offset)
            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPerm, m_msgPermX + m_offset, m_msgPermY + m_offset)

            e.Graphics.DrawString(m_PermMessage, m_msgFontLarge, m_msgBrushPermFront, m_msgPermX, m_msgPermY)
        End If


        If m_StatMessage <> "" Then

            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX - m_offset, m_msgStatY - m_offset)
            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX - m_offset - m_offset, m_msgStatY - m_offset - m_offset)

            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX + m_offset, m_msgStatY - m_offset)
            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX + m_offset + m_offset, m_msgStatY - m_offset - m_offset)

            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX - m_offset, m_msgStatY + m_offset)
            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX - m_offset - m_offset, m_msgStatY + m_offset + m_offset)

            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX + m_offset + m_offset, m_msgStatY + m_offset + m_offset)
            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStat, m_msgStatX + m_offset, m_msgStatY + m_offset)

            e.Graphics.DrawString(m_StatMessage, m_msgFontSmall, m_msgBrushStatFront, m_msgStatX, m_msgStatY)
        End If
    End Sub
    Private Sub Map1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Map1.Resize
        Try
            btnOpenClosePanel.Left = 25
            btnOpenClosePanel.Top = CInt((Map1.Height / 2)) + 15 '+ (Map1.Height / 4) - (btnOpenClosePanel.Height / 2)

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
        Try

            btnTakeImage.Left = Map1.Width - 35 - (btnTakeImage.Width * 2) - 10
            btnTakeImage.Top = Map1.Height - 25 - btnTakeImage.Height

            ' btnTakeImage.Left = 25
            ' btnTakeImage.Top = CInt(Map1.Height - 55) - btnTakeImage.Height


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
        Try
            'relocate the indicator
            resizeIndicator()
            resizeHoverMessage()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub spContControls_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spContControls.Resize
        spContControls.SplitterDistance = (tblLayoutWidgets.RowCount * 33) + (tblLayoutWidgets.RowCount * 9)
        'MsgBox(spContMain.SplitterDistance)

    End Sub


    Private Sub m_MCSearch_IDResult(ByVal Feature As Esri.ArcGIS.Mobile.FeatureCaching.FeatureDataRow) Handles m_MCSearch.IDResult
        Try
            'Show the ID panel
            ' showIDPanel()
            showPanel(TemplatePanels.ID)

            'Set the Map Action
            Map1.MapAction = m_MCIDMA
            'Load the ID panel with the current row

            m_MCIDMA.IDRow(Feature)


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub m_MCSearch_IDLocation(ByVal geo As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal Layer As String, ByVal oid As Integer) Handles m_MCSearch.IDLocation
        Try
            'Show the ID panel
            'showIDPanel()
            showPanel(TemplatePanels.ID)

            'Set the Map Action
            Map1.MapAction = m_MCIDMA
            'Load the ID panel 
            If TypeOf geo Is Esri.ArcGIS.Mobile.Geometries.Point Then
                m_MCIDMA.IDAtLocation(CType(geo, Esri.ArcGIS.Mobile.Geometries.Point).Coordinate, Layer)
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    'Private Sub m_MCTrace_TraceNotComplete() Handles m_MCTrace.TraceNotComplete
    '    m_MCInspectMA.reInspect(m_LastInspectID)

    'End Sub
    'Private Sub m_MCTrace_AllValvesClosed() Handles m_MCTrace.AllValvesClosed
    '    Try

    '        'Prompt the user if they want to save the isolated features
    '        If MsgBox("The leak has been isolated.  " & vbCrLf & "Do you want to commit this service interruption?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then

    '            'Change the meters to be out of service
    '            m_MCTrace.ChangeMeterStatus(True)
    '            'Post the data
    '            m_MCService.PostData()

    '        Else
    '            'Restore the service, undo edits
    '            m_MCTrace.RestoreService()

    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error handling the vavle closed event" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
    'Private Sub m_MCTrace_ValveIsNonOperable() Handles m_MCTrace.ValveIsNonOperable
    '    Try
    '        'Store a flag to trace after a post
    '        m_bPostingForTrace = True
    '        'Post
    '        m_MCService.PostData()

    '    Catch ex As Exception
    '        MsgBox("Error in the VavlesNotOperable Event" & vbCrLf & ex.Message)
    '    End Try

    'End Sub

    'Private Sub m_MCService_CacheUpdated() Handles m_MCService.CacheUpdated
    '    'Restart the application after the cache has been unzipped 
    '    MsgBox("The cache has been updated, restarting application")
    '    Application.Restart()
    'End Sub


    Private Delegate Sub adjustSyncDelegate(ByVal state As Boolean)
    Private Sub m_MCSearch_showIndicator(ByVal state As Boolean) Handles m_MCSearch.showIndicator
        Try
            If m_MCSearch.InvokeRequired Then
                m_MCSearch.Invoke(New adjustSyncDelegate(AddressOf m_MCSearch_showIndicator), state)
                Return

            End If


            If m_SyncIndicator IsNot Nothing Then
                m_SyncIndicator.Visible = state
                Map1.Update()

            End If


            'If state Then
            '    m_MCService.updateStatus()
            'End If
        Catch
        End Try
    End Sub
    Private Sub adjustSync(ByVal state As Boolean) Handles m_MCService.showIndicator
        Try
            If m_MCService.InvokeRequired Then
                m_MCService.Invoke(New adjustSyncDelegate(AddressOf adjustSync), state)
                Return

            End If



            m_SyncIndicator.Visible = state
            If state Then
                m_MCService.updateStatus()
            End If
        Catch
        End Try


    End Sub

    'Private Sub m_MCService_SyncFinished(ByVal syncLayerColl As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncResults) Handles m_MCService.SyncFinished
    '    Try

    '        Raised when a post has finished
    '        If syncLayerColl IsNot Nothing Then
    '            MsgBox("The Sync to the office failed, please check your connection")
    '        End If

    '        If there is flag to refresh the activities
    '            If m_MCActivityControl IsNot Nothing Then
    '                For Each syncAg In syncLayerColl
    '                    If TypeOf (syncAg) Is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent Then
    '                        If CType(syncAg, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent).FeatureSource.Name = GlobalsFunctions.appConfig.WorkorderPanel.LayerInfo.LayerName Then
    '                            m_MCActivityControl.LoadWorkOrders()

    '                        End If
    '                    End If

    '                Next




    '            End If

    '        If the post for to make a valve as inoperable and retrace
    '                If m_bPostingForTrace Then
    '                    If Successful Then

    '                        'Rerun trace if the post was successful
    '                        'm_MCTrace.RunTraceAtLastCoord()


    '                    End If
    '                    m_bPostingForTrace = False
    '                End If
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing
    '    End Try
    'End Sub

    'Private Sub m_MCTrace_ServiceRestored(ByVal ServiceRestored As Boolean) Handles m_MCTrace.ServiceRestored
    '    'If the service was restore, post the changes
    '    If ServiceRestored = True Then
    '        m_MCService.PostData()
    '    Else

    '    End If

    'End Sub


    Private Sub m_MCSketch_GetMapImage(ByVal width As Integer, ByVal height As Integer) Handles m_MCSketch.GetMapImage
        'Dim pBitMap As Bitmap = New Bitmap(width, height) ', System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        '' Dim pRec As System.Drawing.Rectangle = New System.Drawing.Rectangle(Map1.Width / 2 - width / 2, Map1.Height / 2 + height / 2, Map1.Width / 2 + width / 2, Map1.Height / 2 + height / 2)
        ''Dim pBitMap As Bitmap = New Bitmap(Map1.Width - 25, Map1.Height - 25)
        'Dim pRec As System.Drawing.Rectangle = New System.Drawing.Rectangle(0, 0, Map1.Width, Map1.Height)

        '' Map1.DrawToBitmap(pBitMap, pRec)
        'Dim pMapImg as Esri.ArcGIS.Mobile.Winforms.MapImage = Map1.GetMapImage(pRec)
        ''pMapImg.
        'Dim pImg As Image = Image.FromHbitmap(pMapImg.Handle)
        'pImg.Save("C:\tempcach\test.bmp")

        'pMapImg. 
        'pMapImg.Handle
        'pBitMap.Save()
        ''5m_MCSketch.SetBackGroundImage = pBitMap

    End Sub
    Private callingGPSMod As String
    Private Sub m_MCNav_GPSComplete(gpsDet As GPSLocationDetails) Handles m_MCNav.GPSComplete
        Dim pCoord As GPSLocationDetails = gpsDet
        Select Case callingGPSMod
            Case "ID"
                m_MCIDMA.GPSLocation = pCoord

            Case "Create"
                m_MCCreateFeatureMA.GPSLocation = pCoord

                'Case "Inspect"
                '    m_MCInspectMA.GPSLocation = pCoord

        End Select


    End Sub
    Private Sub m_MCCreateFeatureMA_checkGPS() Handles m_MCCreateFeatureMA.checkGPS
        callingGPSMod = "Create"

        m_MCNav.getGPSAsync()


        ''Dim pCoord As GPSLocationDetails = m_MCNav.getGPS
        ''If pCoord Is Nothing Then
        ''    pCoord = m_MCNav.getGPS
        ''    If pCoord Is Nothing Then
        ''        pCoord = m_MCNav.getGPS
        ''        m_MCCreateFeatureMA.GPSLocation = pCoord
        ''    Else
        ''        m_MCCreateFeatureMA.GPSLocation = pCoord

        ''    End If
        ''Else

        ''    m_MCCreateFeatureMA.GPSLocation = pCoord

        ''End If
    End Sub
    Private Sub m_MCIDMA_checkGPS() Handles m_MCIDMA.CheckGPS
        callingGPSMod = "ID"

        m_MCNav.getGPSAsync()

    End Sub

    Private Sub m_MCCreateFeatureMA_GetWorkorder() Handles m_MCCreateFeatureMA.GetWorkorder
        If m_MCActivityControl IsNot Nothing Then
            m_MCCreateFeatureMA.currentWO = m_MCActivityControl.getWO
            m_MCCreateFeatureMA.currentCrew = m_MCActivityControl.getCrew
        End If
    End Sub
    'Private Sub m_MCCreateFeatureMA_RaiseMessage(ByVal Message As String) Handles m_MCCreateFeatureMA.RaiseMessage, m_MCInspectMA.RaiseMessage
    '    showMessage(Message)

    'End Sub

    'Private Sub m_MCInspectMA_RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean) Handles m_MCInspectMA.RaisePermMessage
    '    showPermMessage(Message, Hide)

    'End Sub



    Private Sub m_MCNavMA_RaiseStatMessage(ByVal Message As String, ByVal Hide As Boolean) Handles m_MCNav.RaiseStatMessage
        showStatMessage(Message, Hide)

    End Sub
    Private Sub m_RedlineControl_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles m_MCCreateFeatureMA.StatusChanged
        Try


            If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
                'showRedlineForm()
                showPanel(TemplatePanels.Redline)

            ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
                '     HideControlPanel()

            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub

    Private Sub m_MCNav_GPSStarted() Handles m_MCNav.GPSStarted
        If m_MCCreateFeatureMA IsNot Nothing Then
            m_MCCreateFeatureMA.GPSStatus = "On"
        End If
        If m_MCIDMA IsNot Nothing Then
            m_MCIDMA.GPSStatus = "On"
        End If

    End Sub

    Private Sub m_MCNav_GPSStopped() Handles m_MCNav.GPSStopped
        If m_MCCreateFeatureMA IsNot Nothing Then
            m_MCCreateFeatureMA.GPSStatus = "Off"
        End If
        If m_MCIDMA IsNot Nothing Then
            m_MCIDMA.GPSStatus = "Off"
        End If

    End Sub
    'Private Sub m_MCActivityControl_activityCanceled() Handles m_MCActivityControl.ActivityCanceled
    '    Try

    '        m_InspectionSourceLayer = ""
    '        m_WOID = ""
    '        m_CrewName = ""
    '        m_AssetID = ""
    '        m_MCInspectMA.NewRecordAfterSave = True
    '        m_MCInspectMA.NewRecord()

    '        m_MCInspectMA.ToggleInspectionTypeDisplay(True)
    '    Catch ex As Exception
    '        MsgBox("Error in the Activity control - Cancels" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
    'Private Sub activityClosed() Handles m_MCActivityControl.ActivityClosed
    '    Try

    '        m_InspectionSourceLayer = ""
    '        m_WOID = ""
    '        m_CrewName = ""
    '        m_AssetID = ""
    '        'Set the inspection panel to create a new record after a save
    '        m_MCInspectMA.NewRecordAfterSave = True
    '        'Create a new record
    '        m_MCInspectMA.NewRecord()
    '        'Show the inspection type dialog
    '        m_MCInspectMA.ToggleInspectionTypeDisplay(True)
    '    Catch ex As Exception
    '        MsgBox("Error handling the activity closed event" & vbCrLf & ex.Message)
    '    End Try

    'End Sub
    'Private Sub ActivityOpened(ByVal InspectionLayer As String, ByVal AssetID As String, ByVal WOID As String, ByVal CrewName As String, ByVal Geo As Geometry) Handles m_MCActivityControl.ActivityOpened
    '    Try
    '        'Exit if the inspection layer is null
    '        If InspectionLayer = "" Then Return

    '        'Store the inspection layer, the activity ID, the crew, and the asset
    '        m_InspectionSourceLayer = InspectionLayer
    '        m_WOID = WOID
    '        m_CrewName = CrewName
    '        m_AssetID = AssetID
    '        'Tell the inspeciton panel to not create a new record after a save
    '        m_MCInspectMA.NewRecordAfterSave = False
    '        'Load the inspection record and hide the inspection type box
    '        If LoadInspectionRecord(Geo) Then
    '            If m_InspectionSourceLayer = "" Then
    '                m_MCInspectMA.ToggleInspectionTypeDisplay(True)
    '            Else
    '                m_MCInspectMA.ToggleInspectionTypeDisplay(False)
    '            End If

    '        End If
    '        'Assign the inspection to the current map action
    '        Map1.MapAction = m_MCInspectMA
    '    Catch ex As Exception
    '        MsgBox("Error handling the vavle closed event" & vbCrLf & ex.Message)
    '    End Try
    'End Sub

    'Private Sub m_MCActivityControl_RefreshActivitys(ByVal refreshLayers As String) Handles m_MCActivityControl.RefreshActivity

    '    Try
    '        'the layer to refresh
    '        m_RefreshWO = refreshLayers
    '        'Post the workorder and the inspection records

    '        'MsgBox("Get Activity Layer, remvoed for 10")

    '        ' m_MCService.RefreshData(New String(0) {refreshLayers}, esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.Bidirectional)
    '        m_MCService.refreshDataExtent(True, refreshLayers)
    '    Catch ex As Exception
    '        MsgBox("Error refreshing Activities" & vbCrLf & ex.Message)
    '    End Try

    'End Sub

    'Private Sub m_MCService_RefreshFinished(ByVal Successful As Boolean) Handles m_MCService.SyncFinished
    '    Try
    '        'Event raised after a refresh


    '    Catch ex As Exception
    '        MsgBox("Error in the refresh finished event" & vbCrLf & ex.Message)
    '    End Try
    'End Sub

    'Private Sub m_MCTrace_IsolateComplete(ByVal Success As Boolean) Handles m_MCTrace.IsolateComplete
    '    If Success Then
    '        'showTracePanel()
    '        showPanel(TemplatePanels.Trace)

    '    Else
    '        Map1.Invalidate()

    '    End If
    'End Sub

    Private Sub m_MCNav_ToolChange(ByVal toolType As MobileControls.mobileNavigation.ToolType) Handles m_MCNav.ToolChange
        Select Case toolType
            Case mobileNavigation.ToolType.none
                statTool.Text = ""
            Case mobileNavigation.ToolType.Pan
                statTool.Text = appConfig.NavigationOptions.ZoomPan.PanToolMessage ' "Active Tool: Pan"
            Case mobileNavigation.ToolType.ZoomInOut
                statTool.Text = appConfig.NavigationOptions.ZoomPan.ZoomInOutToolMessage '"Active Tool: Zoom In or Out"
            Case mobileNavigation.ToolType.Measure
                statTool.Text = appConfig.MeasureOptions.ToolMessage 'Active Tool: Measure"
            Case mobileNavigation.ToolType.ZoomIn
                statTool.Text = appConfig.NavigationOptions.ZoomPan.ZoomInToolMessage '"Active Tool: Zoom In"
            Case mobileNavigation.ToolType.ZoomOut
                statTool.Text = appConfig.NavigationOptions.ZoomPan.ZoomOutToolMessage '"Active Tool: Zoom Out"
        End Select

    End Sub


    Private Sub IMConsole_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        'Dispose
        Try

            If m_SyncIndicator IsNot Nothing Then
                m_SyncIndicator.Dispose()
            End If
            m_SyncIndicator = Nothing
            If m_MCNav IsNot Nothing Then
                m_MCNav.Dispose()

            End If
            m_MCNav = Nothing

            If m_MCALNInt IsNot Nothing Then
                m_MCALNInt.Dispose()
            End If
            m_MCALNInt = Nothing

            'If m_MCInspectMA IsNot Nothing Then
            '    m_MCInspectMA.Dispose()
            'End If
            If m_MCIDMA IsNot Nothing Then
                m_MCIDMA.Dispose()
            End If
            m_MCIDMA = Nothing

            If m_MCService IsNot Nothing Then
                m_MCService.Dispose()
            End If
            m_MCService = Nothing

            If m_MCSearch IsNot Nothing Then
                m_MCSearch.Dispose()

            End If
            m_MCSearch = Nothing


            If m_MCToc IsNot Nothing Then
                m_MCToc.Dispose()

            End If
            m_MCToc = Nothing
            If m_MCSketch IsNot Nothing Then
                m_MCSketch.Dispose()

            End If
            m_MCSketch = Nothing
            If m_MCActivityControl IsNot Nothing Then
                m_MCActivityControl.Dispose()

            End If
            m_MCActivityControl = Nothing
            If m_MCCreateFeatureMA IsNot Nothing Then
                m_MCCreateFeatureMA.Dispose()

            End If
            m_MCCreateFeatureMA = Nothing
            'If m_MCTrace IsNot Nothing Then
            '    m_MCTrace.dispose()

            'End If
            'm_MCTrace = Nothing

            GlobalsFunctions.deleteTempPhoto()

        Catch ex As Exception

        End Try
    End Sub
#End Region





    Private Sub spContMain_Panel1_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles spContMain.Panel1.Resize
        RemoveHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
        RemoveHandler spContMain.Resize, AddressOf spContControls_Resize

        spContMain.SplitterDistance = 345
        AddHandler spContMain.Resize, AddressOf spContControls_Resize
        AddHandler spContMain.Panel1.Resize, AddressOf spContMain_Panel1_Resize
        AddHandler spContMain.Panel2.Resize, AddressOf spContMain_Panel1_Resize
    End Sub

    Private Sub pnlWO_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles pnlWO.VisibleChanged
        If m_MCActivityControl IsNot Nothing Then

            'm_MCActivityControl.ToggleGeo(CType(sender, Panel).Visible)
        End If

    End Sub

    Private Sub m_MCToggleGroup_LayersChange() Handles m_MCToggleGroup.LayersChange
        If m_MCIDMA IsNot Nothing Then
            m_MCIDMA.refreshLayers()
        End If
        If m_MCToc IsNot Nothing Then
            m_MCToc.refreshTOC()
        End If
    End Sub

    Private Sub m_MCToggleGroup_syncBaseMapLayer(ByVal BaseMapLayer As Esri.ArcGIS.Mobile.MapLayer) Handles m_MCToggleGroup.syncBaseMapLayer
        m_MCService.refreshBaseMapLayer(BaseMapLayer)

    End Sub

    Private Sub tblLayoutWidgets_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tblLayoutWidgets.MouseClick

        If e.Button = Windows.Forms.MouseButtons.Right Then
            spContMain.Panel1Collapsed = True
            spContMain.Panel2Collapsed = False
        End If

    End Sub


    Private Sub btnToggleScreens_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnToggleScreens.Click
        spContControls.Panel1Collapsed = Not spContControls.Panel1Collapsed
        If spContControls.Panel1Collapsed Then
            btnToggleScreens.BackgroundImage = My.Resources.ButtonsVert
        Else
            btnToggleScreens.BackgroundImage = My.Resources.ButtonsVertDown
        End If

    End Sub

    Private Sub btnToggleDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnToggleDisplay.Click
        If btnToggleDisplay.Tag = "PanelVert" Then
            btnToggleDisplay.BackgroundImage = My.Resources.MapVert
            spContMain.Panel1Collapsed = False
            spContMain.Panel2Collapsed = True
            btnToggleDisplay.Tag = "MapVert"
        Else
            btnToggleDisplay.Tag = "PanelVert"
            btnToggleDisplay.BackgroundImage = My.Resources.PanelVert
            spContMain.Panel1Collapsed = True
            spContMain.Panel2Collapsed = False
        End If

    End Sub


    Private Sub btnShowHideTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'If tblLayoutWidgets.Visible Then
        '    btnShowHideTable.Visible = True
        '    tblLayoutWidgets.Visible = False
        '    spContControls.SplitterDistance = 15 '(tblLayoutWidgets.RowCount * 33) + (tblLayoutWidgets.RowCount * 9)

        'Else
        '    tblLayoutWidgets.Visible = True
        '    btnShowHideTable.Visible = False
        '    spContControls.SplitterDistance = (tblLayoutWidgets.RowCount * 33) + (tblLayoutWidgets.RowCount * 9)

        'End If

    End Sub

    Private Sub tblLayoutWidgets_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tblLayoutWidgets.MouseDoubleClick
        'If tblLayoutWidgets.Visible Then
        '    btnShowHideTable.Visible = True
        '    tblLayoutWidgets.Visible = False
        '    spContControls.SplitterDistance = 15 '(tblLayoutWidgets.RowCount * 33) + (tblLayoutWidgets.RowCount * 9)

        'Else
        '    tblLayoutWidgets.Visible = True
        '    btnShowHideTable.Visible = False
        '    spContControls.SplitterDistance = (tblLayoutWidgets.RowCount * 33) + (tblLayoutWidgets.RowCount * 9)

        'End If
    End Sub


    Private Sub MobileMapConsole_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress


    End Sub

    Private Sub m_MCIDMA_RouteTo(ByVal location As ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String) Handles m_MCIDMA.RouteTo

    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub m_MCIDMA_RaiseMessage(ByVal Message As String) Handles m_MCIDMA.RaiseMessage
        showMessage(Message)
    End Sub

    Private Sub m_MCIDMA_RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean) Handles m_MCIDMA.RaisePermMessage
        showPermMessage(Message, Hide)
    End Sub

    Private Sub m_MCActivityControl_RaiseMessage(Message As String) Handles m_MCActivityControl.RaiseMessage
        showMessage(Message)
    End Sub

    Private Sub m_MCActivityControl_RaisePermMessage(Message As String, Hide As Boolean) Handles m_MCActivityControl.RaisePermMessage
        showPermMessage(Message, Hide)
    End Sub

    Private Sub btnTakeImage_Click(sender As System.Object, e As System.EventArgs) Handles btnTakeImage.Click
        Try


            Dim image As System.Drawing.Image = Map1.ToBitmap



            Dim strPath As String = GlobalsFunctions.getRandomFileNameInLocation("MapImage_{0}.png")

            image.Save(strPath, System.Drawing.Imaging.ImageFormat.Png)

            If m_pLnk.setPath(strPath) = False Then
                Return
            End If


            m_pLnk.ShowDialog()




        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub

    Private Sub MobileMapConsole_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

    End Sub

    Private Sub pnlCreateFeature_VisibleChanged(sender As System.Object, e As System.EventArgs) Handles pnlCreateFeature.VisibleChanged
        m_MCCreateFeatureMA.reorderEditBtns()
    End Sub

    Private Sub m_MCCreateFeatureMA_SyncLayer(layerName As String) Handles m_MCCreateFeatureMA.SyncLayer
        Dim pLst As New List(Of FeatureSource)
        pLst.Add(GlobalsFunctions.GetFeatureSource(layerName, Map1).FeatureSource)
        m_MCService.PostData(pLst)
    End Sub

    Private Sub m_MCWeb_WebPanelToggle(visible As Boolean) Handles m_MCWeb.WebPanelToggle
        If btnTakeImage IsNot Nothing Then
            If visible = False And UCase(appConfig.ApplicationSettings.MapExport.visible) = UCase("TRUE") Then
                btnTakeImage.Visible = True
            ElseIf UCase(appConfig.ApplicationSettings.MapExport.visible) = UCase("FALSE") Then
                btnTakeImage.Visible = False
            Else
                btnTakeImage.Visible = False
            End If

        End If

    End Sub
End Class