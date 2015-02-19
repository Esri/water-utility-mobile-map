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

Public Class activitySelectorMapAction
    Inherits Esri.ArcGIS.Mobile.WinForms.PanMapAction
    Public Event RaiseMessage(ByVal Message As String)
    Public Event RaisePermMessage(ByVal Message As String, ByVal Hide As Boolean)

    Private m_DownCur As System.Windows.Forms.Cursor
    Private m_NormCur As System.Windows.Forms.Cursor
    Private m_LastMouseLoc As Point = New Point

    Private m_btn As Button


    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map


    Private m_BufferDivideValue As Double

    Private WithEvents m_MCActivityControl As MobileControls.AssignedWorkControl


    Public Sub New()
        MyBase.New()
        '  MyBase.
        Dim s As System.IO.Stream = New System.IO.MemoryStream(My.Resources.PanCur)
        m_DownCur = New System.Windows.Forms.Cursor(s)
        s = New System.IO.MemoryStream(My.Resources.Finger)
        m_NormCur = New System.Windows.Forms.Cursor(s)
        initModule()



    End Sub

#Region "Default Overrides"


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
    Public Function getCrew() As String
        Return m_MCActivityControl.getCrew()

    End Function
    Public Function getWO() As String
        Return m_MCActivityControl.getWO()

    End Function
    Public Function getText() As String
        Return m_MCActivityControl.getText()

    End Function
    Public Function addActivityButton(Optional ByVal TopX As Integer = -1, Optional ByVal TopY As Integer = -1) As Boolean
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

                    .BackgroundImage = Global.MobileControls.My.Resources.Resources.ActivitySelector


                    .Name = "btnID"
                    .Size = New System.Drawing.Size(50, 50)

                    .UseVisualStyleBackColor = False
                    'Add a handler for the click event
                    AddHandler .MouseClick, AddressOf ActivityBtnClick
                    'Set the id button on the top right
                    TopX = m_Map.Width - 25
                    TopY = 145

                    .Location = New System.Drawing.Point(TopX, TopY)
                    'Add a handler to relocate the button a map resize
                    AddHandler m_Map.Resize, AddressOf resize
                End With
                'Add the button to the map
                m_Map.Controls.Add(m_btn)
            End If
            'Create the layer combo 

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
    Public Sub InitActivityForm(panel As Control)
        'Create the new activity control
        m_MCActivityControl = New AssignedWorkControl(m_Map)
        'Set to dock it full
        m_MCActivityControl.Dock = DockStyle.Fill
        'Add the control to the panel to house it
        panel.Controls.Add(m_MCActivityControl)

    End Sub

    Protected Overrides Sub OnSetMap(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        MyBase.OnSetMap(map)
        'm_Map = map

    End Sub

#End Region
#Region "Events"

    Private Sub ActivityBtnClick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Activates/Deactivate the mapaction and changes the button 
        Dim pBtn As Button = CType(sender, Button)

        If m_Map.MapAction Is Me Then

            m_Map.MapAction = Nothing
            pBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.ActivitySelector
        Else
            m_Map.MapAction = Me
            pBtn.BackgroundImage = Global.MobileControls.My.Resources.Resources.ActivitySelectorDown


        End If

    End Sub
    Private Sub resize()
        'For Each item As String In m_layCBO.Items

        'Next
        'relocates the button and combo on a map resize
        If m_btn IsNot Nothing And m_Map IsNot Nothing Then
            m_btn.Location = New System.Drawing.Point(m_Map.Width - 80, 145)


        End If
    End Sub
#End Region
#Region "PrivateFunctions"

    Private Sub initModule()


        Try

            Dim strVal As String = GlobalsFunctions.appConfig.IDPanel.SearchTolerence
            If Not IsNumeric(strVal) Then
                m_BufferDivideValue = 15
            Else
                m_BufferDivideValue = CDbl(strVal)
            End If
            strVal = GlobalsFunctions.appConfig.ApplicationSettings.FontSize





        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        Finally
            ''myutils = Nothing
        End Try
    End Sub


#End Region
#Region "Properties"

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
                CheckForActivityAtLocation(e.MapCoordinate)


            End If

        Catch ex As Exception
        Finally
            MyBase.OnMapMouseUp(e)
            m_Map.Cursor = m_NormCur
        End Try


    End Sub
    Public Sub CheckForActivityAtLocation(ByVal coord As Esri.ArcGIS.Mobile.Geometries.Coordinate, Optional ByVal LayerName As String = "")
        ' If e.Clicks > 1 Then Return

        If m_Map.IsValid = False Then Return

        m_MCActivityControl.selectWOatLocation(coord)
    End Sub


#End Region



    Private Sub m_MCActivityControl_RaiseMessage(Message As String) Handles m_MCActivityControl.RaiseMessage
        RaiseEvent RaiseMessage(Message)
    End Sub

    Private Sub m_MCActivityControl_RaisePermMessage(Message As String, Hide As Boolean) Handles m_MCActivityControl.RaisePermMessage
        RaiseEvent RaisePermMessage(Message, Hide)
    End Sub

    Private Sub activitySelectorMapAction_StatusChanged(sender As Object, e As WinForms.MapActionStatusChangedEventArgs) Handles Me.StatusChanged
        Try
            'Check the map actions status
            If e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Activated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.ActivitySelectorDown

                End If


            ElseIf e.StatusId = Esri.ArcGIS.Mobile.WinForms.MapAction.Deactivated Then
                'If the button has been created, update the buttons image
                If m_btn IsNot Nothing Then
                    m_btn.BackgroundImage = My.Resources.ActivitySelector

                End If


            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
            Return
        End Try
    End Sub
    Public Event WOChanged(WOOID As String, WOCrew As String, WODisplayText As String)
    Private Sub m_MCActivityControl_WOChanged(WOOID As String, WOCrew As String, WODisplayText As String) Handles m_MCActivityControl.WOChanged
        RaiseEvent WOChanged(WOOID, WOCrew, WODisplayText)
    End Sub
End Class
