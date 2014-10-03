Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports Esri.ArcGIS.Mobile.WebServices
Imports Esri.ArcGIS.Mobile.DataProducts


Imports System.Windows.Forms
Imports System.Windows.Forms.Control
Imports System.Xml
Imports System.Diagnostics.Process
Imports System.IO
Imports System.Threading
Imports System.Resources
Imports System.Drawing

Imports Esri.ArcGISTemplates

Public Class mobileIdentifyMapAction
    Inherits Esri.ArcGIS.Mobile.WinForms.PanMapAction
    Public raiseHyperEvent As Boolean = False
    Private m_SetGeoOnly As Boolean = False

    Private m_GPSEditing As Boolean = False
    Public Event HyperClick(ByVal PathToFile As String)

    Public Event RouteTo(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String)
    Public Event Waypoint(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String)

    Public Event LocationIdentified(ByVal fdr As FeatureDataRow)
    Public Event CheckGPS()
    Private m_GPSVal As GPSLocationDetails = Nothing
    Public Event RaiseMessage(ByVal Message As String)
    Public Event RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean)

    Private m_LongString As String = " "

    Private m_DownCur As System.Windows.Forms.Cursor
    Private m_NormCur As System.Windows.Forms.Cursor
    Private m_RouteToOption As Boolean
    Private m_WayPoint As Boolean
    Private m_LastMouseLoc As Point = New Point

    Private WithEvents m_AttFrm As MobileControls.MobileAttributes
    Private WithEvents m_EditFrm As MobileControls.EditControl

    Private m_Layer As String
    'Private m_contr As Control
    Private m_btn As Button
    Private WithEvents m_layCBO As ComboBox
    Private m_Fnt As System.Drawing.Font '= New System.Drawing.Font("Microsoft Sans Serif", 10.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
    Private m_FntSize As Single = 12
    Private m_AttLengthFld As Boolean
    Private m_AttFormatString As String
    Private m_AttLengthUnit As String

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map


    Private m_layersToID As ArrayList


    Private m_LastScale As Double = 0.0
    Private m_BufferDivideValue As Double


    Public Sub New()
        MyBase.New()
        '  MyBase.
        Dim s As System.IO.Stream = New System.IO.MemoryStream(My.Resources.PanCur)
        m_DownCur = New System.Windows.Forms.Cursor(s)
        s = New System.IO.MemoryStream(My.Resources.Finger)
        m_NormCur = New System.Windows.Forms.Cursor(s)
        initModule()



    End Sub

#Region "Defualt Overrides"


    Public Overrides Function ToString() As String
        Return MyBase.ToString()
    End Function

    Protected Overrides Sub OnMapPaint(ByVal e As Esri.ArcGIS.Mobile.WinForms.MapPaintEventArgs)
        MyBase.OnMapPaint(e)
    End Sub
    Protected Overrides Sub OnMapMouseWheel(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseWheel(e)
    End Sub

    Protected Overrides Sub OnMapMouseMove(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        MyBase.OnMapMouseMove(e)
    End Sub

    Protected Overrides Sub OnMapKeyUp(ByVal e As System.Windows.Forms.KeyEventArgs)
        MyBase.OnMapKeyUp(e)
    End Sub
    Protected Overrides Sub OnMapKeyPress(ByVal e As System.Windows.Forms.KeyPressEventArgs)
        MyBase.OnMapKeyPress(e)
    End Sub
    Protected Overrides Sub OnMapKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
        MyBase.OnMapKeyDown(e)
    End Sub

    Protected Overrides Sub OnActiveChanging(ByVal active As Boolean)
        Try

            'Me.Activating
            MyBase.OnActiveChanging(active)
        Catch ex As Exception

        End Try
    End Sub

    Public Overrides Function GetHashCode() As Integer
        Return MyBase.GetHashCode()
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        If m_btn IsNot Nothing Then
            If m_btn.Parent IsNot Nothing Then
                m_btn.Parent.Controls.Remove(m_btn)
            End If

            m_btn.Dispose()

        End If


        m_btn = Nothing
        If m_AttFrm IsNot Nothing Then
            m_AttFrm.Dispose()
        End If

        m_AttFrm = Nothing

        If m_EditFrm IsNot Nothing Then
            m_EditFrm.Dispose()
        End If

        m_EditFrm = Nothing

        If m_Fnt IsNot Nothing Then
            m_Fnt.Dispose()
        End If
        m_Fnt = Nothing
        If m_layCBO IsNot Nothing Then
            If m_layCBO.Parent IsNot Nothing Then
                m_layCBO.Parent.Controls.Remove(m_layCBO)
            End If
            m_layCBO.Dispose()
        End If
        m_layCBO = Nothing
        If m_Map IsNot Nothing Then
            m_Map.Dispose()
        End If

        m_Map = Nothing

    End Sub
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Return MyBase.Equals(obj)

    End Function
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    Public Overrides Sub Cancel()
        MyBase.Cancel()
    End Sub
    Protected Overrides Sub OnActiveChanged(ByVal active As Boolean)
        MyBase.OnActiveChanged(active)
    End Sub


#End Region
    Private Sub FixZoom(ByVal bOut As Boolean, ByVal xCent As Double, ByVal yCent As Double)
        'Make sure the map is valid
        If m_Map Is Nothing Then Return
        If m_Map.IsValid = False Then Return
        'Determine which direction to zoom
        If (bOut) Then
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = m_Map.Extent()
            'Resize it
            pExt.Resize(1.5)
            'Center it
            pExt.CenterAt(xCent, yCent)

            'Set the map extent
            m_Map.Extent = pExt
            'cleanup
            pExt = Nothing
        Else
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Envelope
            'Get the map extent
            pExt = m_Map.Extent()
            'Resize it
            pExt.Resize(0.5)
            'Center it
            pExt.CenterAt(xCent, yCent)

            'Set the map extent
            m_Map.Extent = pExt
            'cleanup
            pExt = Nothing
        End If


    End Sub
#Region "Public Functions"

    Public Sub IDRow(ByVal Feature As Esri.ArcGIS.Mobile.FeatureCaching.FeatureDataRow)

        IdentifyLayer = Feature.FeatureSource.Name
        RemoveHandler m_layCBO.SelectedIndexChanged, AddressOf m_layCBO_SelectedIndexChanged
        If m_AttFrm.ActiveGroup IsNot Nothing Then
            m_layCBO.SelectedItem = m_AttFrm.ActiveGroup.GroupName
        End If


        AddHandler m_layCBO.SelectedIndexChanged, AddressOf m_layCBO_SelectedIndexChanged

        m_AttFrm.currentRecord(Feature)


    End Sub
    Public Function addIDButton(Optional ByVal TopX As Integer = -1, Optional ByVal TopY As Integer = -1) As Boolean
        Try
            'Adds the ID button to the map display
            If m_btn Is Nothing Then

                'Create a new button
                m_btn = New Button

                With m_btn
                    'Set the button defaults

                    .BackColor = System.Drawing.SystemColors.Info
                    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
                    .FlatAppearance.BorderSize = 0
                    .FlatStyle = System.Windows.Forms.FlatStyle.Flat

                    .BackgroundImage = Global.MobileControls.My.Resources.Resources.Info


                    .Name = "btnID"
                    .Size = New System.Drawing.Size(50, 50)

                    .UseVisualStyleBackColor = False
                    'Add a handler for the click event
                    AddHandler .MouseClick, AddressOf IDBtnClick
                    'Set the id button on the top right
                    TopX = m_Map.Width - 80
                    TopY = 25

                    .Location = New System.Drawing.Point(TopX, TopY)
                    'Add a handler to relocate the button a map resize
                    AddHandler m_Map.Resize, AddressOf resize


                End With
                'Add the button to the map
                m_Map.Controls.Add(m_btn)
            End If
            'Create the layer combo 
            If m_layCBO Is Nothing Then
                m_layCBO = New ComboBox
                AddHandler m_layCBO.MeasureItem, AddressOf cbo_MeasureItem
                AddHandler m_layCBO.DrawItem, AddressOf cbo_DrawItem


                m_layCBO.Visible = False
                m_layCBO.ForeColor = Drawing.Color.White
                m_layCBO.BackColor = Drawing.Color.DarkBlue
                '  m_layCBO.DefaultBackColor = Drawing.Color.DarkBlue
                '   m_layCBO.DefaultForeColor = Drawing.Color.White
                'm_layCBO.
                ' m_layCBO.DropDownHeight = 300
                ' m_layCBO.DrawMode = DrawMode.OwnerDrawVariable
                m_layCBO.DropDownStyle = ComboBoxStyle.DropDownList
                'Load the layers 
                LoadLayers()
                'Set the selected layer to the current ID layer
                m_layCBO.SelectedItem = m_Layer
                m_layCBO.Font = m_Fnt
                m_layCBO.Width = 200
                m_layCBO.Left = m_btn.Left - m_layCBO.Width - 10
                m_layCBO.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_layCBO.Height / 2))
                'Add the combo to the map next to the button
                m_Map.Controls.Add(m_layCBO)
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True

    End Function
    Public Property ManualSetMap As Esri.ArcGIS.Mobile.WinForms.Map
        Get
            Return Map
        End Get
        Set(value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value


        End Set
    End Property


    Protected Overrides Sub OnSetMap(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        MyBase.OnSetMap(map)
        'm_Map = map

    End Sub
    Public Sub InitIDForm(ByVal container As Control, Optional ByVal LayerName As String = "", Optional ByVal RouteToOption As Boolean = False)


        loadIDLayers()

        getAttOptions()
        m_RouteToOption = RouteToOption

        'Init the attribute panel
        If m_AttFrm Is Nothing Then
            m_Layer = LayerName
            m_AttFrm = New MobileControls.MobileAttributes(m_Map, LayerName, m_RouteToOption, m_FntSize, m_AttLengthFld, m_AttFormatString, m_AttLengthUnit)

            m_AttFrm.Width = container.Width
            m_AttFrm.Height = container.Height
            m_AttFrm.Dock = DockStyle.Fill
            container.Controls.Add(m_AttFrm)
            m_AttFrm.m_BufferDivideValue = m_BufferDivideValue
        End If

        If m_EditFrm Is Nothing Then
            m_Layer = LayerName
            m_EditFrm = New MobileControls.EditControl(m_Map, Nothing)
            m_EditFrm.DrawGeo = True
            m_EditFrm.NewRecordAfterSave = False

            m_EditFrm.Width = container.Width
            m_EditFrm.Height = container.Height
            m_EditFrm.Dock = DockStyle.Fill
            container.Controls.Add(m_EditFrm)
            'm_EditFrm.m_BufferDivideValue = m_BufferDivideValue
            m_EditFrm.Visible = False

        End If


    End Sub
    'Public Sub showIDFormLayerBox(ByVal show As Boolean)
    '    'Toggles the layer box on the attribute panel
    '    m_Att.ShowHideLayerBox(show)

    'End Sub
    Public Sub showMapLayerBox(ByVal show As Boolean)
        'Toggles the layer box on the map display
        m_layCBO.Visible = show
        RemoveHandler m_layCBO.SelectedIndexChanged, AddressOf m_layCBO_SelectedIndexChanged
        m_layCBO.Text = m_Layer

        AddHandler m_layCBO.SelectedIndexChanged, AddressOf m_layCBO_SelectedIndexChanged
    End Sub
    Public Sub reload()
        'Call the reload method on the attribute panel
        m_AttFrm.reload()

    End Sub
    Public Sub refreshLayers()
        LoadLayers()

    End Sub
#End Region
#Region "Events"

    Private Sub m_Att_CheckGPS() Handles m_AttFrm.CheckGPS
        m_GPSEditing = False
        RaiseEvent CheckGPS()

        'm_Att.GPSLocation = m_GPSVal

    End Sub

    Private Sub m_Att_HyperClick(ByVal PathToFile As String) Handles m_AttFrm.HyperClick

        Try


            If raiseHyperEvent Then
                RaiseEvent HyperClick(PathToFile)
            Else
                System.Diagnostics.Process.Start(PathToFile)
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub
    Private Sub m_Att_RouteTo(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String) Handles m_AttFrm.RouteTo

        RaiseEvent RouteTo(location, LocationName)


    End Sub
    Private Sub m_Att_Waypoint(ByVal location As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal LocationName As String) Handles m_AttFrm.Waypoint

        RaiseEvent Waypoint(location, LocationName)


    End Sub

    Private Sub m_Att_LocationIdentified(ByVal fdr As FeatureDataRow) Handles m_AttFrm.LocationIdentified
        m_AttFrm.hideEditButton()
        Dim bEditFnd As Boolean = False
        For Each editLay In GlobalsFunctions.appConfig.EditControlOptions.Layers.Layer
            If editLay.Name = fdr.FeatureSource.Name Then
                If editLay.EditExisting.ToUpper() = "TRUE" Then

                    If editLay.EditGeo.ToUpper() = "TRUE" Then
                        m_EditFrm.MoveGeoButtonVisible = True
                    Else
                        m_EditFrm.MoveGeoButtonVisible = False


                    End If
                    If editLay.DeleteFeature.ToUpper() = "TRUE" Then
                        m_EditFrm.DeleteButtonVisible = True

                    Else
                        m_EditFrm.DeleteButtonVisible = False


                    End If
                    ' m_AttFrm.Visible = False
                    'm_EditFrm.Visible = True
                    m_AttFrm.showEditButton()
                    m_EditFrm.setCurrentRecord(fdr, editLay)

                    bEditFnd = True
                ElseIf editLay.EditExisting.ToUpper() = "OWN" Then

                    If fdr.FeatureSource.Columns(editLay.EditOwnerField) IsNot Nothing Then

                        If fdr(editLay.EditOwnerField).ToString() = Environment.UserName Then

                            If editLay.EditGeo.ToUpper() = "TRUE" Then
                                m_EditFrm.MoveGeoButtonVisible = True
                            Else
                                m_EditFrm.MoveGeoButtonVisible = False


                            End If
                            If editLay.DeleteFeature.ToUpper() = "TRUE" Then
                                m_EditFrm.DeleteButtonVisible = True
                            Else
                                m_EditFrm.DeleteButtonVisible = False


                            End If
                            ' m_AttFrm.Visible = False
                            'm_EditFrm.Visible = True
                            m_AttFrm.showEditButton()
                            m_EditFrm.setCurrentRecord(fdr, editLay)
                            bEditFnd = True
                        ElseIf fdr(editLay.EditOwnerField).ToString() = Environment.UserDomainName & "\\" & Environment.UserName Then

                            If editLay.EditGeo.ToUpper() = "TRUE" Then
                                m_EditFrm.MoveGeoButtonVisible = True
                            Else
                                m_EditFrm.MoveGeoButtonVisible = False


                            End If
                            If editLay.DeleteFeature.ToUpper() = "TRUE" Then
                                m_EditFrm.DeleteButtonVisible = True
                            Else
                                m_EditFrm.DeleteButtonVisible = False


                            End If
                            ' m_AttFrm.Visible = False
                            'm_EditFrm.Visible = True
                            m_AttFrm.showEditButton()
                            m_EditFrm.setCurrentRecord(fdr, editLay)
                            bEditFnd = True
                        ElseIf fdr(editLay.EditOwnerField).ToString() = Environment.UserDomainName & "\" & Environment.UserName Then

                            If editLay.EditGeo.ToUpper() = "TRUE" Then
                                m_EditFrm.MoveGeoButtonVisible = True
                            Else
                                m_EditFrm.MoveGeoButtonVisible = False


                            End If
                            If editLay.DeleteFeature.ToUpper() = "TRUE" Then
                                m_EditFrm.DeleteButtonVisible = True
                            Else
                                m_EditFrm.DeleteButtonVisible = False


                            End If
                            ' m_AttFrm.Visible = False
                            'm_EditFrm.Visible = True
                            m_AttFrm.showEditButton()
                            m_EditFrm.setCurrentRecord(fdr, editLay)
                            bEditFnd = True
                        End If
                    Else

                        If editLay.EditGeo.ToUpper() = "TRUE" Then
                            m_EditFrm.MoveGeoButtonVisible = True
                        Else
                            m_EditFrm.MoveGeoButtonVisible = False


                        End If
                        If editLay.DeleteFeature.ToUpper() = "TRUE" Then
                            m_EditFrm.DeleteButtonVisible = True
                        Else
                            m_EditFrm.DeleteButtonVisible = False


                        End If
                        ' m_AttFrm.Visible = False
                        'm_EditFrm.Visible = True
                        m_AttFrm.showEditButton()

                        m_EditFrm.setCurrentRecord(fdr, editLay)
                        bEditFnd = True
                    End If

                End If
            End If
        Next

        m_AttFrm.Visible = True
        m_EditFrm.Visible = False

        'RaiseEvent LocationIdentified(fdr)
    End Sub

    Private Sub m_EditFrm_CheckGPS() Handles m_EditFrm.CheckGPS
        m_GPSEditing = True
        RaiseEvent CheckGPS()

    End Sub

    Private Sub m_EditFrm_MoveGeo(ByVal status As Boolean) Handles m_EditFrm.MoveGeo
        m_SetGeoOnly = status

    End Sub

    Private Sub m_EditFrm_RaiseMessage(ByVal Message As String) Handles m_EditFrm.RaiseMessage
        RaiseEvent RaiseMessage(Message)
    End Sub

    Private Sub m_EditFrm_RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean) Handles m_EditFrm.RaisePermMessage
        RaiseEvent RaisePermMessage(Message, Hide)
    End Sub
    Private Sub m_EditFrm_RecordClear() Handles m_EditFrm.RecordClear
        m_AttFrm.Visible = True
        m_EditFrm.Visible = False
        m_AttFrm.currentRecord(Nothing)

    End Sub

    Private Sub m_EditFrm_RecordDelete(LayerName As String) Handles m_EditFrm.RecordDelete
        m_AttFrm.Visible = True
        m_EditFrm.Visible = False
        m_AttFrm.currentRecord(Nothing)

    End Sub

    Private Sub m_EditFrm_RecordSaved(ByVal LayerName As String, ByVal pGeo As Esri.ArcGIS.Mobile.Geometries.Geometry, ByVal OID As Integer) Handles m_EditFrm.RecordSaved
        'm_AttFrm.Visible = True
        ' m_EditFrm.Visible = False
        ' m_AttFrm.currentRecord(Nothing)
        RaiseEvent RaiseMessage(GlobalsFunctions.appConfig.EditControlOptions.UIComponents.SavedMessage)

    End Sub

    Private Sub m_map_ControlAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.ControlEventArgs) Handles m_Map.ControlAdded

    End Sub
    Private Sub m_map_ExtentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.ExtentChanged
        If m_LastScale = m_Map.Scale Then Return

        LoadLayers()

        m_LastScale = m_Map.Scale

    End Sub
    'Private Sub mobileIdentifyMapAction_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
    '    'destroy and remove the controls
    '    If m_btn IsNot Nothing Then
    '        If m_btn.Parent IsNot Nothing Then
    '            m_btn.Parent.Controls.Remove(m_btn)
    '        End If

    '        m_btn.Dispose()

    '    End If


    '    m_btn = Nothing
    '    If m_AttFrm IsNot Nothing Then
    '        m_AttFrm.Dispose()
    '    End If

    '    m_AttFrm = Nothing

    '    If m_EditFrm IsNot Nothing Then
    '        m_EditFrm.Dispose()
    '    End If

    '    m_EditFrm = Nothing

    '    If m_Fnt IsNot Nothing Then
    '        m_Fnt.Dispose()
    '    End If
    '    m_Fnt = Nothing
    '    If m_layCBO IsNot Nothing Then
    '        If m_layCBO.Parent IsNot Nothing Then
    '            m_layCBO.Parent.Controls.Remove(m_layCBO)
    '        End If
    '        m_layCBO.Dispose()
    '    End If
    '    m_layCBO = Nothing
    '    If m_map IsNot Nothing Then
    '        m_map.Dispose()
    '    End If

    '    m_map = Nothing

    'End Sub

    Private Sub mobileIdentifyMapAction_StatusChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged

        'Monitors the status of this map action
        If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
            'Change the button image to display the action is activated
            If m_btn IsNot Nothing Then

                m_btn.BackgroundImage = Global.MobileControls.My.Resources.Resources.InfoDown
            End If
            'Show the layer combo box
            showMapLayerBox(True)
            If m_AttFrm IsNot Nothing Then
                m_AttFrm.resizeLabels()
            End If


            If m_EditFrm.EditingGeo Then
                m_EditFrm.btnMoveStatusChanged()
            End If

            m_Map.Cursor = m_NormCur
        ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
            'Change the button image to display the action is deactivated

            If m_btn IsNot Nothing Then
                m_btn.BackgroundImage = Global.MobileControls.My.Resources.Resources.Info
            End If
            'Hide the layer combo box
            showMapLayerBox(False)
            If m_EditFrm.Visible Then
                If m_EditFrm.EditingGeo Then
                    m_EditFrm.hideMoveMessage()
                End If
            End If
        End If

    End Sub
    Private Sub IDBtnClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Activates/Deactivate the mapaction and changes the button 
        Dim pBtn As Button = CType(sender, Button)

        If m_Map.MapAction Is Me Then

            m_Map.MapAction = Nothing
            pBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.Info
        Else
            m_Map.MapAction = Me
            pBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.InfoDown


        End If

    End Sub
    Private Sub m_layCBO_DropDown(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_layCBO.DropDown
        'If m_layCBO.Width > 200 Then
        '    m_layCBO.Font = m_Fnt
        '    m_layCBO.Width = 200
        '    'm_cboInspectionTypes.Height
        '    m_layCBO.Left = m_btn.Left - m_layCBO.Width - 10
        '    m_layCBO.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_layCBO.Height / 2))

        'Else
        Dim g As Graphics = m_layCBO.CreateGraphics
        For i = 0 To 4
            ' m_layCBO.Width = m_layCBO.Width + 40
            '    m_cboInspectionTypes.Height = m_cboInspectionTypes.Height + 5

            Dim pFnt As System.Drawing.Font = New System.Drawing.Font(m_layCBO.Font.Name, m_layCBO.Font.Size + i, System.Drawing.FontStyle.Bold)

            m_layCBO.Font = pFnt

            m_layCBO.Width = CInt(g.MeasureString(m_LongString, pFnt).Width + 25)


            m_layCBO.Left = m_btn.Left - m_layCBO.Width - 10
            m_layCBO.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_layCBO.Height / 2))

        Next
        g.Dispose()
        g = Nothing
        ' End If

    End Sub
    Private Sub m_layCBO_DropDownClosed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_layCBO.DropDownClosed
        'If m_layCBO.Width > 200 Then
        '    m_layCBO.Font = m_Fnt
        '    m_layCBO.Width = 200
        '    'm_cboInspectionTypes.Height
        '    m_layCBO.Left = m_btn.Left - m_layCBO.Width - 10
        '    m_layCBO.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_layCBO.Height / 2))
        'End If
        Dim g As Graphics = m_layCBO.CreateGraphics
        '  For i = 0 To 5
        ' m_layCBO.Width = m_layCBO.Width + 40
        '    m_cboInspectionTypes.Height = m_cboInspectionTypes.Height + 5

        ' Dim pFnt As System.Drawing.Font = New System.Drawing.Font(m_layCBO.Font.Name, m_layCBO.Font.Size + i, System.Drawing.FontStyle.Bold)
        m_layCBO.Width = CInt(g.MeasureString(m_layCBO.Text, m_Fnt).Width + 50)


        m_layCBO.Font = m_Fnt

        m_layCBO.Left = m_btn.Left - m_layCBO.Width - 10
        m_layCBO.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_layCBO.Height / 2))
        g.Dispose()
        g = Nothing
        '   Next
    End Sub
    Private Sub m_layCBO_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_layCBO.SelectedIndexChanged
        'Handles a layer change in the map layer box
        m_Layer = m_layCBO.Text
        If m_AttFrm IsNot Nothing Then
            If m_layersToID IsNot Nothing Then
                For Each stL As MobileAttributes.LayersWithGroup In m_layersToID
                    If stL.GroupName = m_Layer Then
                        m_AttFrm.ActiveGroup = stL
                        Exit For

                    End If
                Next

            Else
                m_AttFrm.ActiveGroup = New MobileAttributes.LayersWithGroup(m_Layer, m_Layer)
            End If
        End If

        If m_layCBO.Text = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText Then
            m_AttFrm.toggleCombo = False
        Else
            m_AttFrm.toggleCombo = True

        End If
        If m_AttFrm.Visible = False Then
            m_AttFrm.Visible = True
            m_EditFrm.Visible = False
        End If
    End Sub
    Private Sub resize()
        'For Each item As String In m_layCBO.Items

        'Next
        'relocates the button and combo on a map resize
        If m_btn IsNot Nothing And m_Map IsNot Nothing Then
            m_btn.Location = New System.Drawing.Point(m_Map.Width - 80, 25)
            If m_layCBO IsNot Nothing Then

                m_layCBO.Width = 200
                m_layCBO.Left = m_btn.Left - m_layCBO.Width - 10
                m_layCBO.Top = CInt(m_btn.Top + (m_btn.Height / 2) - (m_layCBO.Height / 2))
            End If

        End If
    End Sub
#End Region
#Region "PrivateFunctions"
    ' If you set the Draw property to DrawMode.OwnerDrawVariable, 
    ' you must handle the MeasureItem event. This event handler 
    ' will set the height and width of each item before it is drawn. 
    Private Sub cbo_MeasureItem(ByVal sender As Object, _
       ByVal e As System.Windows.Forms.MeasureItemEventArgs)

        Dim cboBox As ComboBox = CType(sender, ComboBox)

        Select Case e.Index
            Case 0
                e.ItemHeight = cboBox.Height
            Case 1
                e.ItemHeight = cboBox.Height
            Case 2
                e.ItemHeight = cboBox.Height
        End Select
        e.ItemWidth = cboBox.Width

    End Sub

    ' You must handle the DrawItem event for owner-drawn combo boxes.  
    ' This event handler changes the color, size and font of an 
    ' item based on its position in the array.
    Private Sub cbo_DrawItem(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.DrawItemEventArgs)


        '  Dim size As Single
        Dim cboBox As ComboBox = CType(sender, ComboBox)

        ' Draw the background of the item.
        e.DrawBackground()

        ' Create a square filled with the animals color. Vary the size
        ' of the rectangle based on the length of the animals name.
        'Dim rectangle As Rectangle = New Rectangle(2, e.Bounds.Top + 2, _
        '    e.Bounds.Height, e.Bounds.Height - 4)
        ''e.Graphics.FillRectangle(New SolidBrush(Color.Beige), rectangle)

        ' Draw each string in the array, using a different size, color,
        ' and font for each item.

        'e.Graphics.DrawString(cboBox.Items(e.Index), cboBox.Font, System.Drawing.Brushes.White, _
        '    New RectangleF(e.Bounds.X + Rectangle.Width, e.Bounds.Y, _
        '    e.Bounds.Width, e.Bounds.Height))
        If cboBox.Items.Count > 0 Then
            e.Graphics.DrawString(cboBox.Items(e.Index).ToString, cboBox.Font, System.Drawing.Brushes.White, e.Bounds.X, e.Bounds.Y)
            ' Draw the focus rectangle if the mouse hovers over an item.
        End If

        e.DrawFocusRectangle()
    End Sub

    Public Sub loadIDLayers()
        Try
            Dim pLyGrp As MobileAttributes.LayersWithGroup


            If GlobalsFunctions.appConfig.IDPanel.IDGroups.IDGroup.Count > 0 Then
                m_layersToID = New ArrayList

                For Each idGroup As MobileConfigClass.MobileConfigMobileMapConfigIDPanelIDGroupsIDGroup In GlobalsFunctions.appConfig.IDPanel.IDGroups.IDGroup

                    pLyGrp = New MobileAttributes.LayersWithGroup(idGroup.DisplayName, idGroup.LayersToList)

                    m_layersToID.Add(pLyGrp)



                Next
            Else
                m_layersToID = New ArrayList
                Dim pList As List(Of String) = New List(Of String)

                Dim playersCount As Integer = m_Map.MapLayers.Count
                'Current map layer
                Dim pMapLayer As Esri.ArcGIS.Mobile.MapLayer

                'map layer's name
                Dim pMapLayerName As String = ""
                'Create a new list to hold layer names
                'Get all layers
                'Loops through current map layers collection
                For i As Integer = 0 To playersCount - 1

                    'Gets the map layer for that index
                    pMapLayer = m_Map.MapLayers(i)
                    If pMapLayer IsNot Nothing Then


                        'Gets the map layer's name
                        pMapLayerName = pMapLayer.Name


                        If TypeOf pMapLayer Is MobileCacheMapLayer Then
                            pList.Add(pMapLayerName)

                        End If
                    End If
                Next i

                pLyGrp = New MobileAttributes.LayersWithGroup(GlobalsFunctions.appConfig.IDPanel.IDGroups.AllLayersText, pList)

                m_layersToID.Add(pLyGrp)

            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try
    End Sub

    Private Sub initModule()


        Try

            Dim strVal As String = GlobalsFunctions.appConfig.IDPanel.SearchTolerence
            If Not IsNumeric(strVal) Then
                m_BufferDivideValue = 15
            Else
                m_BufferDivideValue = CDbl(strVal)
            End If
            strVal = GlobalsFunctions.appConfig.ApplicationSettings.FontSize
            If Not IsNumeric(strVal) Then
                m_FntSize = 0
            Else



                m_FntSize = CInt(strVal)

            End If
            If m_FntSize = 0 Then
                m_FntSize = 10
            End If
            m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", m_FntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)






        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        Finally
            ''myutils = Nothing
        End Try
    End Sub
    Private Sub getAttOptions()

        ''Dim myutils As ConfigUtils

        Try
            'get the url from the app.config file
            ' myutils = New ConfigUtils()


            Dim strVal As String = GlobalsFunctions.appConfig.IDPanel.ShowLengthFieldInID
            If strVal Is Nothing Then
                m_AttLengthFld = False
            ElseIf strVal.ToUpper = "TRUE" Then

                m_AttLengthFld = True
            Else
                m_AttLengthFld = False

            End If
            strVal = GlobalsFunctions.appConfig.IDPanel.LengthFormat
            If strVal Is Nothing Then
                m_AttFormatString = "{0:0.00}"
            ElseIf strVal = "" Then
                m_AttFormatString = "{0:0.00}"
            Else
                m_AttFormatString = strVal

            End If

            strVal = GlobalsFunctions.appConfig.IDPanel.LengthUnit 'GlobalsFunctions.FindConfigKey("LengthUnit")
            If strVal Is Nothing Then
                m_AttLengthUnit = "MAP"
            ElseIf strVal = "" Then
                m_AttLengthUnit = "MAP"
            Else
                m_AttLengthUnit = strVal

            End If



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        Finally
            ''myutils = Nothing
        End Try
    End Sub



    Private Sub LoadLayers()
        Try
            If m_Map Is Nothing Then Return


            ' Total number of MapLayers in the current map
            Dim playersCount As Integer = m_Map.MapLayers.Count


            'Current map layer
            Dim pMapLayer As Esri.ArcGIS.Mobile.MapLayer

            'map layer's name
            Dim pMapLayerName As String = ""
            'Create a new list to hold layer names
            Dim pList As IList = New List(Of String)


            'Get all layers
            'Loops through current map layers collection
            For i As Integer = 0 To playersCount - 1

                'Gets the map layer for that index
                pMapLayer = m_Map.MapLayers(i)
                If pMapLayer IsNot Nothing Then


                    'Gets the map layer's name
                    pMapLayerName = pMapLayer.Name

                    If TypeOf pMapLayer Is MobileCacheMapLayer Then
                        For k As Integer = 0 To CType(pMapLayer, MobileCacheMapLayer).MobileCache.FeatureSources.Count - 1

                            Dim pFS As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pMapLayer, MobileCacheMapLayer).MobileCache.FeatureSources(k)
                            Dim pLayDef As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayerDefinition = CType(pMapLayer, MobileCacheMapLayer).LayerDefinitions(k)

                            If pFS.MinScale = 0 And pFS.MaxScale = 0 And pLayDef.Visibility = True Then

                                If m_layersToID IsNot Nothing Then
                                    For Each idLy As MobileAttributes.LayersWithGroup In m_layersToID
                                        If pList.Contains(idLy.GroupName) Then
                                            Continue For
                                        End If
                                        If idLy.LayersInGroup.Contains(pFS.Name) Then
                                            pList.Add(idLy.GroupName)
                                            If (idLy.GroupName.Length > m_LongString.Length) Then
                                                m_LongString = idLy.GroupName
                                            End If

                                            Continue For
                                        End If
                                    Next

                                Else

                                    pList.Add(pLayDef.Name)
                                    If (pLayDef.Name.Length > m_LongString.Length) Then
                                        m_LongString = pLayDef.Name
                                    End If

                                End If
                            ElseIf pFS.MinScale = 0 And m_Map.Scale > pFS.MaxScale And pLayDef.Visibility = True Then

                                If m_layersToID IsNot Nothing Then
                                    For Each idLy As MobileAttributes.LayersWithGroup In m_layersToID
                                        If pList.Contains(idLy.GroupName) Then
                                            Continue For
                                        End If
                                        If idLy.LayersInGroup.Contains(pFS.Name) Then
                                            pList.Add(idLy.GroupName)
                                            If (idLy.GroupName.Length > m_LongString.Length) Then
                                                m_LongString = idLy.GroupName
                                            End If
                                            Continue For
                                        End If
                                    Next

                                Else

                                    pList.Add(pLayDef.Name)
                                    If (pLayDef.Name.Length > m_LongString.Length) Then
                                        m_LongString = pLayDef.Name
                                    End If
                                End If
                            ElseIf pFS.MaxScale = 0 And m_Map.Scale < pFS.MinScale And pLayDef.Visibility = True Then

                                If m_layersToID IsNot Nothing Then
                                    For Each idLy As MobileAttributes.LayersWithGroup In m_layersToID
                                        If pList.Contains(idLy.GroupName) Then
                                            Continue For
                                        End If
                                        If idLy.LayersInGroup.Contains(pFS.Name) Then
                                            pList.Add(idLy.GroupName)
                                            If (idLy.GroupName.Length > m_LongString.Length) Then
                                                m_LongString = idLy.GroupName
                                            End If
                                            Continue For
                                        End If
                                    Next

                                Else

                                    pList.Add(pLayDef.Name)
                                    If (pLayDef.Name.Length > m_LongString.Length) Then
                                        m_LongString = pLayDef.Name
                                    End If
                                End If

                            ElseIf m_Map.Scale < pFS.MinScale And m_Map.Scale > pFS.MaxScale And pLayDef.Visibility = True Then

                                If m_layersToID IsNot Nothing Then
                                    For Each idLy As MobileAttributes.LayersWithGroup In m_layersToID
                                        If pList.Contains(idLy.GroupName) Then
                                            Continue For
                                        End If
                                        If idLy.LayersInGroup.Contains(pFS.Name) Then
                                            pList.Add(idLy.GroupName)
                                            If (idLy.GroupName.Length > m_LongString.Length) Then
                                                m_LongString = idLy.GroupName
                                            End If
                                            Continue For
                                        End If
                                    Next

                                Else

                                    pList.Add(pLayDef.Name)
                                End If
                            End If
                            '  Next

                        Next k





                    End If
                End If

            Next i
            If pList.Count > 0 Then
                If Not pList.Contains(GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText) Then
                    pList.Insert(0, GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText)
                    If (GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText.Length > m_LongString.Length) Then
                        m_LongString = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText
                    End If
                    'If m_Att IsNot Nothing Then
                    '    m_Att.toggleCombo = False
                    'End If

                End If
            End If


            RemoveHandler m_layCBO.SelectedIndexChanged, AddressOf m_layCBO_SelectedIndexChanged
            Dim pTxt As String = m_layCBO.Text
            'Set layer list to the search drop down
            m_layCBO.DataSource = pList
            If pList.Count = 1 Then
                m_layCBO.Text = "<No visible identifiable layers>"

            ElseIf pList.Count > 1 Then

                m_Layer = pList.Item(0).ToString

                If pList.Contains(pTxt) Then
                    m_layCBO.Text = pTxt
                Else
                    If m_layersToID IsNot Nothing Then
                        For Each stL As MobileAttributes.LayersWithGroup In m_layersToID
                            If stL.GroupName = m_Layer Then
                                m_AttFrm.ActiveGroup = stL
                                Exit For

                            End If
                        Next

                    Else
                        m_AttFrm.ActiveGroup = New MobileAttributes.LayersWithGroup(m_Layer, m_Layer)
                    End If

                    'Call m_layCBO_SelectedIndexChanged(Nothing, Nothing)

                End If
            End If
            AddHandler m_layCBO.SelectedIndexChanged, AddressOf m_layCBO_SelectedIndexChanged

            pMapLayer = Nothing
            '     pMblService = Nothing
            m_AttFrm.resetIDLayers()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

#End Region
#Region "Properties"
    Private m_GPSStatus As String

    Public Property GPSStatus As String
        Get
            Return m_GPSStatus
        End Get
        Set(ByVal value As String)

            m_GPSStatus = value
            If m_AttFrm IsNot Nothing Then
                m_AttFrm.GPSStatus = value

            End If
            If m_EditFrm IsNot Nothing Then
                m_EditFrm.GPSStatus = value

            End If
        End Set
    End Property
    Public Property GPSLocation() As GPSLocationDetails
        Get
            Return m_GPSVal
        End Get
        Set(ByVal value As GPSLocationDetails)
            m_GPSVal = value
            'If m_GPSVal IsNot Nothing Then


            '    IDAtLocation(m_map.SpatialReference.FromGps(m_GPSVal.Coordinate))

            'End If
            If m_GPSVal IsNot Nothing Then

                If m_GPSEditing = False Then
                    IDAtLocation(m_Map.SpatialReference.FromGps(m_GPSVal.Coordinate))
                Else

                    If m_EditFrm.Geometry IsNot Nothing Then
                        If m_EditFrm.Geometry.GeometryType = Geometries.GeometryType.Point Then
                            m_EditFrm.EnableGPS()
                            Dim newCoord As Geometries.Coordinate
                            If m_GPSVal.SpatialReference Is Nothing Then
                                newCoord = Map.SpatialReference.FromGps(m_GPSVal.Coordinate)

                            ElseIf m_GPSVal.SpatialReference.FactoryCode <> Map.SpatialReference.FactoryCode Then

                                newCoord = Map.SpatialReference.FromGps(m_GPSVal.Coordinate)

                                'Me.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(m_Map.SpatialReference.FromDegreeMinuteSecond(m_GPSVal.LongitudeToDegreeMinutesSeconds), m_Map.SpatialReference.FromDegreeMinuteSecond(m_GPSVal.LatitudeToDegreeMinutesSeconds))

                                'Me.Geometry = m_GPSVal()
                            Else
                                newCoord = m_GPSVal.Coordinate




                            End If
                            m_EditFrm.Geometry = New Geometries.Point(newCoord)
                        End If
                    End If


                    Map.Invalidate()
                End If
            End If
        End Set
    End Property

    'returns the active layer
    Public Property IdentifyLayer() As String
        Get
            Return m_Layer
        End Get
        Set(ByVal value As String)
            m_Layer = value
            If m_AttFrm IsNot Nothing Then
                If m_layersToID IsNot Nothing Then
                    For Each stL As MobileAttributes.LayersWithGroup In m_layersToID
                        If stL.GroupName = m_Layer Then
                            m_AttFrm.ActiveGroup = stL
                            Exit For

                        Else
                            For Each lay As String In stL.LayersInGroup
                                If lay = value Then
                                    m_AttFrm.ActiveGroup = stL
                                    m_AttFrm.ActiveLayer = value
                                    m_Layer = value
                                    Exit For
                                End If

                            Next
                        End If
                    Next

                Else
                    m_AttFrm.ActiveGroup = New MobileAttributes.LayersWithGroup(m_Layer, m_Layer)
                End If
            End If



        End Set
    End Property
#End Region
#Region "Overrides"

    Protected Overrides Sub OnMapMouseDown(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)


        m_LastMouseLoc = e.Location

        'm_map.SuspendLayout()
        'm_map.SuspendOnPaint = True
        'MyBase.OnMapMouseDown(e)
        m_Map.Cursor = m_DownCur
        MyBase.OnMapMouseDown(e)
    End Sub
    Protected Overrides Sub OnMapMouseUp(ByVal e As Esri.ArcGIS.Mobile.MapMouseEventArgs)
        If Me.Active = False Then Return
        If e.Clicks > 1 Then Return
        Try

            'If m_EditFrm.Visible = True And m_SetGeoOnly = False Then Return

            If m_LastMouseLoc.Equals(e.Location) Then
                IDAtLocation(e.MapCoordinate)


            End If

        Catch ex As Exception
        Finally
            MyBase.OnMapMouseUp(e)
            m_Map.Cursor = m_NormCur
        End Try


    End Sub
    Public Sub IDAtLocation(ByVal coord As Esri.ArcGIS.Mobile.Geometries.Coordinate, Optional ByVal LayerName As String = "")
        ' If e.Clicks > 1 Then Return

        If m_Map.IsValid = False Then Return
        If m_AttFrm Is Nothing Then Return
        If m_Layer Is Nothing Then Return

        If m_AttFrm.mapControl Is Nothing Then
            'MsgBox("Please add the ID MapAction to the map's map action collection before setting the layer")
            Return
        End If

        If m_SetGeoOnly Then
            Select Case m_EditFrm.CurrentRecord.FeatureSource.GeometryType
                Case Geometries.GeometryType.Point

                    'Set the edit panels geoemtry
                    'pFDRInspection.Geometry = New ESRI.ArcGIS.Mobile.Geometries.Point(coord)
                    m_EditFrm.Geometry = New Esri.ArcGIS.Mobile.Geometries.Point(coord)
                Case Geometries.GeometryType.Polyline
                    'If m_EditFrm.Geometry Is Nothing Then
                    '    m_EditFrm.Geometry = New Esri.ArcGIS.Mobile.Geometries.Polyline()

                    'End If
                    'm_EditFrm.AddCoordToGeo(coord)
                    'm_EditPanel.Geometry.AddCoordinate(coord)
                Case Geometries.GeometryType.Polygon

            End Select
            ' m_SetGeoOnly = False
        Else


            If LayerName <> "" Then
                m_layCBO.Text = LayerName

            End If
            If m_layCBO.Text = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText Then
                For Each strVal As String In m_layCBO.DataSource
                    If strVal = GlobalsFunctions.appConfig.IDPanel.UIComponents.TopMostIDText Then
                        Continue For

                    Else
                        If m_layersToID Is Nothing Then

                        Else
                            For Each stL As MobileAttributes.LayersWithGroup In m_layersToID
                                If stL.GroupName = strVal Then
                                    m_AttFrm.ActiveGroup = stL
                                    If m_AttFrm.IdentifyLocation(coord) Then
                                        Return


                                    End If
                                End If
                            Next

                        End If


                    End If

                Next
                m_AttFrm.resetIDLayers()

            Else
                If m_AttFrm.IdentifyLocation(coord) Then
                    Return

                End If
            End If
        End If


    End Sub


#End Region


    Private Sub m_EditFrm_VisibleChanged(sender As Object, e As System.EventArgs) Handles m_EditFrm.VisibleChanged
        m_EditFrm.DrawGeo = m_EditFrm.Visible

    End Sub

    Private Sub m_AttFrm_showEditor() Handles m_AttFrm.showEditor
        m_AttFrm.Visible = False
        m_EditFrm.Visible = True
    End Sub
End Class
