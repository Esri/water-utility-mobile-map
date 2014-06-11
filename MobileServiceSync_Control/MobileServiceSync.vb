' | Version 10.2
' | Copyright 2014 Esri
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


Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGISTemplates
Imports Esri.ArcGIS.Mobile.CatalogServices
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports System.Windows.Forms
Imports System.Threading
Imports System.IO
Imports System.Threading.Thread




Imports System.Web
Imports System.Web.Services
Imports System.Web.UI
Imports System.ComponentModel
Imports System.Net

'Imports Ionic.Zip
Public Class MobileServiceSync
    Private Class LayersToSync
        Public Sub New()
            m_Layers = New List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)
            m_RefreshGroupName = ""

        End Sub
        Public Sub New(ByVal RefreshGroupName As String, ByVal LayersInGroup As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer))
            m_Layers = LayersInGroup
            m_RefreshGroupName = RefreshGroupName

        End Sub

        Public Sub AddLayer(ByVal LayerName As String)
            Dim pTmp As New MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer()
            pTmp.Name = LayerName
            m_Layers.Add(pTmp)

        End Sub
        Private m_RefreshGroupName As String
        Public Property RefreshGroupName() As String
            Get
                Return m_RefreshGroupName
            End Get
            Set(ByVal value As String)
                m_RefreshGroupName = value
            End Set
        End Property

        Private m_Layers As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)
        Public Property LayersInGroup() As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)
            Get
                Return m_Layers
            End Get
            Set(ByVal value As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer))
                m_Layers = value
            End Set
        End Property


    End Class
    Private Class AsyncPostFeatureSourceClass
        Public FeatureSourceSyncAgent As ArrayList
        Public StartDelegate As SyncFeatureSourceDelegate

        Public Sub syncFeatureSource()
            StartDelegate(FeatureSourceSyncAgent)
        End Sub
    End Class
    Delegate Sub SyncFeatureSourceDelegate(ByVal FeatureSourceSyncAgent As ArrayList)
    ' Private WithEvents cancelWorker As New System.ComponentModel.BackgroundWorker
    Private WithEvents syncThread As Thread
    'Map and Service
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Public Event showIndicator(ByVal state As Boolean)

    Private WithEvents m_MobileCache As Esri.ArcGIS.Mobile.FeatureCaching.MobileCache
    Private WithEvents m_MobileConnect As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
    Private WithEvents m_MobileSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncAgent
    Private WithEvents m_MobileSyncResults As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncResults
    Friend Shared m_PostLayCount As Integer = 0
    '  Friend Shared m_AllLayersSyncing As Boolean = False
    'Determine if the service is available
    Private WithEvents m_ConnectionStatus As CheckStatusClass

    Private m_SyncToolsInit As Boolean = False

    'Variable to show indictor or not(At this release, it cannot be false)
    Private m_ShowSyncIndicator As Boolean = True

    'Private m_GetDataOnExtentChange As Boolean = False
    ' Private m_RequestLimit As Integer
    'Private m_LimitRequest As Boolean = False
    Private m_edits As Integer = 0

    ' Private m_AutoPostLimit As Integer
    'Private m_AutoPost As Boolean = False
    'Used to monitor edits 
    Private m_MonitorEdits As Boolean = True
    Private m_HandlerState As Boolean = False
    'Public Event PostFinished(ByVal Successful As Boolean)

    'Public Event RefreshFinished(ByVal Successful As Boolean)
    Public Event SyncFinished(ByVal syncAgentCollection As IList(Of Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent))

    'Mtom Chucked Transfer
    '    Public Shared m_WebService As MTOM_Library.MtomWebService.MTOMWse
    'Private m_TaskPanel As TaskPanel

    'Mtom Binary transfer
    'Public WithEvents m_BinaryTransfer As BinaryDataMTOMClient.BinaryDataMTOMClient.BinaryDataMTOMClient


    'Private m_cacheDate As String = ""
    'Private m_CacheName As String = "Cache.zip"

    Private m_bRequiretokens As Boolean
    Private m_Token As String
    'Public Event CacheUpdated()

    Private WithEvents bkGrnOpenLayers As New System.ComponentModel.BackgroundWorker
    Private m_layersToRefresh As ArrayList
    Private m_Fnt As System.Drawing.Font

#Region "Public Functions"
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        Try


            'Init the service monitor class
            m_ConnectionStatus = New CheckStatusClass
            Dim fntSize As Single
            Single.TryParse(GlobalsFunctions.appConfig.ApplicationSettings.FontSize, fntSize)
            If Not IsNumeric(fntSize) Then
                fntSize = 10

            End If

            m_Fnt = New System.Drawing.Font("Microsoft Sans Serif", fntSize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
            lblNumEdits.Font = m_Fnt


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Public Function refreshBaseMapLayer(ByVal layer As Esri.ArcGIS.Mobile.MapLayer) As Boolean
        Dim lay As Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer = layer

    End Function

    'Public Function refreshDataExtent(ByVal HonorScale As Boolean, ByVal Refresh As Boolean, ByVal FullExtent As Boolean, Optional ByVal strLayName As String = "") As Boolean
    Public Function refreshDataExtent(ByVal FullExtent As Boolean, Optional ByVal strLayName As String = "") As Boolean
        Try
            'Check the status of the control, make sure it is valid
            If checkToolStatus() = False Then Return False
            'Create a array of layers to refresh
            Dim pLay As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = New List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)

            Dim pLoopLay As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = Nothing
            'Refresh the layer passed in, if not, refresh them all
            If strLayName <> "" Then
                For Each strGroup As LayersToSync In m_layersToRefresh
                    If strGroup.RefreshGroupName = strLayName Then
                        For Each strLay In strGroup.LayersInGroup
                            pLoopLay = m_MobileCache.FeatureSources(strLay.Name)
                            If pLoopLay IsNot Nothing Then
                                pLay.Add(strLay)
                            End If

                        Next
                        Exit For
                    End If
                Next

                If pLay.Count = 0 Then Return False

            ElseIf m_layersToRefresh IsNot Nothing Then
                For Each strGroup As LayersToSync In m_layersToRefresh
                    For Each strLay In strGroup.LayersInGroup
                        pLoopLay = m_MobileCache.FeatureSources(strLay.Name)
                        If pLoopLay IsNot Nothing Then
                            pLay.Add(strLay)
                        End If

                    Next

                Next
                If pLay.Count = 0 Then Return False


            End If
            'Determine the extent to refresh
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Polygon
            If FullExtent Then
                pExt = m_Map.FullExtent.ToPolygon
            Else
                pExt = m_Map.Extent.ToPolygon
            End If
            'Call a method to refresh
            'Dim pDblScale As Double
            'If HonorScale Then
            '    pDblScale = m_Map.Scale
            'Else
            '    pDblScale = 0

            'End If
            postAllEditsByFeat(Synchronization.SyncDirection.Bidirectional, pLay, pExt)

            'getDataForLayersInExtent(pExt, pDblScale, Refresh, pLay)

            pExt = Nothing
            pLay = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Function
    Public Function refreshDataExtentStartup(ByVal FullExtent As Boolean) As Boolean
        Try
            If m_layersToRefresh Is Nothing Then Return True


            'Check the status of the control, make sure it is valid
            If checkToolStatus() = False Then Return False
            'Create a array of layers to refresh
            Dim pLay As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = New List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)

            Dim pLoopLay As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = Nothing
            'Refresh the layer passed in, if not, refresh them all

            For Each strGroup As LayersToSync In m_layersToRefresh

                For Each strLay In strGroup.LayersInGroup
                    If GlobalsFunctions.IsBoolean(strLay.SyncOnStartUp) Then
                        If Convert.ToBoolean(strLay.SyncOnStartUp) Then
                            pLoopLay = m_MobileCache.FeatureSources(strLay.Name)

                            If pLoopLay IsNot Nothing Then
                                pLay.Add(strLay)
                            End If
                        End If

                    End If

                Next


            Next

            If pLay.Count = 0 Then Return False


            'Determine the extent to refresh
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Polygon
            If FullExtent Then
                pExt = m_Map.FullExtent.ToPolygon
            Else
                pExt = m_Map.Extent.ToPolygon
            End If
            'Call a method to refresh
            'Dim pDblScale As Double
            'If HonorScale Then
            '    pDblScale = m_Map.Scale
            'Else
            '    pDblScale = 0

            'End If
            postAllEditsByFeat(Synchronization.SyncDirection.Bidirectional, pLay, pExt)

            'getDataForLayersInExtent(pExt, pDblScale, Refresh, pLay)

            pExt = Nothing
            pLay = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Function

    'Public Function refreshDataExtent(ByVal FullExtent As Boolean, Optional ByVal strLayName As String = "") As Boolean
    '    Try
    '        'Check the status of the control, make sure it is valid
    '        If checkToolStatus() = False Then Return False
    '        'Create a array of layers to refresh
    '        Dim pLay As List(Of Esri.ArcGIS.Mobile.MapLayer) = New List(Of Esri.ArcGIS.Mobile.MapLayer)

    '        Dim pLoopLay As Esri.ArcGIS.Mobile.MapLayer = Nothing
    '        'Refresh the layer passed in, if not, refresh them all
    '        If strLayName <> "" Then
    '            For Each strGroup As LayersToSync In m_layersToRefresh
    '                If strGroup.RefreshGroupName = strLayName Then
    '                    For Each strLay In strGroup.LayersInGroup
    '                        pLoopLay = m_MobileCache.Layers(strLay.Name)
    '                        If pLoopLay IsNot Nothing Then
    '                            pLay.Add(pLoopLay)
    '                        End If

    '                    Next
    '                    Exit For
    '                End If
    '            Next

    '            If pLay.Count = 0 Then Return False

    '        ElseIf m_layersToRefresh IsNot Nothing Then
    '            For Each strGroup As LayersToSync In m_layersToRefresh
    '                For Each strLay In strGroup.LayersInGroup
    '                    pLoopLay = m_MobileCache.Layers(strLay.Name)
    '                    If pLoopLay IsNot Nothing Then
    '                        pLay.Add(pLoopLay)
    '                    End If

    '                Next

    '            Next
    '            If pLay.Count = 0 Then Return False


    '        End If
    '        'Determine the extent to refresh
    '        Dim pExt As Esri.ArcGIS.Mobile.Geometries.Polygon
    '        If FullExtent Then
    '            pExt = m_Map.GetFullExtent.ToPolygon
    '        Else
    '            pExt = m_Map.GetViewExtent
    '        End If
    '        'Call a method to refresh
    '        'Dim pDblScale As Double
    '        'If HonorScale Then
    '        '    pDblScale = m_Map.Scale
    '        'Else
    '        '    pDblScale = 0

    '        'End If
    '        postAllEditsByFeat(Synchronization.SyncDirection..Bidirectional, pLay, pExt)

    '        'getDataForLayersInExtent(pExt, pDblScale, Refresh, pLay)

    '        pExt = Nothing
    '        pLay = Nothing

    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    End Try
    'End Function

    'Public Sub RefreshData(ByVal Layers() As String, ByVal refreshType As esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection)
    '    Try
    '        Dim FilterLayers As List(Of Esri.ArcGIS.Mobile.MapLayer) = Nothing
    '        If Layers IsNot Nothing Then
    '            For Each strLayName As String In Layers

    '                Dim mblSL As Esri.ArcGIS.Mobile.MapLayer = m_MobileCache.FeatureSources(strLayName)

    '                If mblSL IsNot Nothing Then
    '                    If FilterLayers Is Nothing Then
    '                        FilterLayers = New List(Of Esri.ArcGIS.Mobile.MapLayer)
    '                    End If

    '                    FilterLayers.Add(mblSL)
    '                End If
    '            Next

    '            If FilterLayers Is Nothing Then Return

    '            If FilterLayers.Count = 0 Then Return
    '        End If
    '        '  FilterLayers = Layers
    '        'Check each pending request and make sure data is not already posting
    '        'For Each pReq As Request In m_MobileService.GetPendingRequests()
    '        '    If pReq.AsyncState = "Posting" Then
    '        '        Return

    '        '    End If
    '        'Next
    '        'Post
    '        If FilterLayers Is Nothing Then

    '            '  postAllEdits()
    '            postAllEditsByFeat(refreshType)

    '        Else
    '            '    postAllEdits(FilterLayers)
    '            postAllEditsByFeat(refreshType, FilterLayers)
    '        End If
    '        FilterLayers = Nothing

    '    Catch ex As Exception
    '        MsgBox("Error in the sync PostData" & vbCrLf & ex.Message)
    '    End Try

    'End Sub

    Public Sub PostData(Optional ByVal Layers As List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource) = Nothing)
        Try
            Dim FilterLayers As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = Nothing
            If Layers IsNot Nothing Then
                For Each mblSL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource In Layers
                    If mblSL IsNot Nothing Then
                        If FilterLayers Is Nothing Then
                            FilterLayers = New List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)
                        End If

                        Dim pconVal As New MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer
                        pconVal.Name = mblSL.Name

                        FilterLayers.Add(pconVal)
                    End If
                Next

                If FilterLayers Is Nothing Then Return

                If FilterLayers.Count = 0 Then Return
            Else
                FilterLayers = Nothing

            End If

            'Check each pending request and make sure data is not already posting
            'For Each pReq As Request In m_MobileService.GetPendingRequests()
            '    If pReq.AsyncState = "Posting" Then
            '        Return

            '    End If
            'Next
            'Post
            If FilterLayers Is Nothing Then

                '  postAllEdits()
                postAllEditsByFeat(Synchronization.SyncDirection.UploadOnly)

            Else
                '    postAllEdits(FilterLayers)
                postAllEditsByFeat(Synchronization.SyncDirection.UploadOnly, FilterLayers)
            End If
            FilterLayers = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub
    'Public Sub ToggleCacheSyncDisplay()
    '    Try
    '        'Toggles the cache update tools display

    '        gpBxSettings.Visible = Not gpBxSettings.Visible
    '    Catch ex As Exception
    '        MsgBox("Error Toggleing Cache Display" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
#End Region
#Region "Properties"
    '********These properties can be exposed, but coded needs to be added to control the form when they change

    'Public Property ConcurrentServerRequestLimit() As Integer
    '    Get
    '        Return m_RequestLimit
    '    End Get
    '    Set(ByVal value As Integer)
    '        m_RequestLimit = value
    '        m_MobileService.NumberOfSimultaneousRequest = m_RequestLimit

    '    End Set
    'End Property
    'Public Property LimitRequest() As Boolean
    '    Get
    '        Return m_LimitRequest
    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_LimitRequest = value
    '    End Set
    'End Property
    'Public Property ShowSyncIndicator() As Boolean
    '    Get
    '        Return m_ShowSyncIndicator
    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_ShowSyncIndicator = value
    '    End Set
    'End Property
    'Public Property GetDataOnExtentChange() As Boolean
    '    Get
    '        Return m_GetDataOnExtentChange
    '    End Get
    '    Set(ByVal value As Boolean)
    '        m_GetDataOnExtentChange = value

    '    End Set
    'End Property
    Public WriteOnly Property map() As Esri.ArcGIS.Mobile.WinForms.Map
        'Set the map used by the sync controls
        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value


        End Set
    End Property


#End Region
#Region "Events"
    Private Sub layer_DataChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.DataChangedEventArgs)
        Try


            'Monitors data change events to a layer.  This handler is removed during sync as it is called when data is being created from the server

            'If called from a different thread, call the parent thread
            If InvokeRequired Then
                Try
                    Invoke(New EventHandler(Of DataChangedEventArgs)(AddressOf layer_DataChanged), sender, e)

                Catch ex As Exception

                End Try
                Return

            End If

            'Get the edit count and adjust the labels
            adjustEditLbl()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub m_MobileService_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_MobileCache.Closed
        Try


            'If the service closes, uninit the tools
            If m_MobileCache.IsOpen = False Then
                m_SyncToolsInit = False
                'Add the handlers to monitor status
                adjustDataChangeHandlers(False)
                'Update the display
                adjustEditLbl()
                MonitorServerConnection(False)

            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub m_MobileService_Opened(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_MobileCache.Opened
        Try


            'If the service opens, initilize the tools
            If m_MobileCache.IsOpen = True Then
                m_SyncToolsInit = True
                'Add the handlers to monitor status
                adjustDataChangeHandlers(True)
                'Update the display
                adjustEditLbl()
                MonitorServerConnection(True)

            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub


    Private Sub m_MobileSyncAgent_StateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_MobileSyncAgent.StateChanged
        If InvokeRequired Then
            Try
                Invoke(New EventHandler(AddressOf m_MobileSyncAgent_StateChanged), sender, e)

            Catch ex As Exception

            End Try
            Return

        End If

        For Each syAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent In m_MobileSyncAgent.FeatureSyncAgents
            If syAgent.State = Synchronization.SyncState.Synchronizing Then Return


        Next
        If m_MobileSyncAgent.State = Synchronization.SyncState.Ready Then
            'Hide the indicator
            RaiseEvent showIndicator(False)
            '  m_SyncIndicator.Visible = False
            'Adjust the edit label
            adjustEditLbl()
            'add the handler if monitoring edits
            If m_MonitorEdits Then
                adjustDataChangeHandlers(True)
            End If
        Else
            'Show the sync idictator
            If m_SyncToolsInit = False Then Return
            If m_ShowSyncIndicator Then
                RaiseEvent showIndicator(True)

            End If
        End If
    End Sub

    Private Sub m_Map_DataSynchronizationFinished(sender As Object, e As EventArgs) Handles m_Map.DataSynchronizationFinished

        If m_MobileSyncAgent.State = Synchronization.SyncState.Ready Then
            'Hide the indicator
            RaiseEvent showIndicator(False)
            '  m_SyncIndicator.Visible = False
        End If
    End Sub

    Private Sub m_Map_DataSynchronizationStarted(sender As Object, e As EventArgs) Handles m_Map.DataSynchronizationStarted

        If m_SyncToolsInit = False Then Return
        If m_ShowSyncIndicator Then
            RaiseEvent showIndicator(True)

        End If
    End Sub

    'Private Sub m_MobileSyncAgent_ProgressChanged(ByVal sender As Object, ByVal e As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheSyncAgentProgressEventArgs) Handles m_MobileSyncAgent.ProgressChanged

    '    Try
    '        'Call the parent thread if required
    '        If m_Map.InvokeRequired Then
    '            Try

    '                m_Map.Invoke(New EventHandler(AddressOf m_MobileSyncAgent_ProgressChanged), sender, e)


    '            Catch
    '            End Try

    '            Return
    '        End If
    '        Dim blErrorFound As Boolean = False

    '        If e.SyncResults.Exception IsNot Nothing Then
    '            If e.SyncResults.Exception.Message.Contains("Esri.ArcGIS.Mobile.FeatureCaching.RequestException") Then
    '                lstErrors.Items.Add(GlobalsFunctions.appConfig.ServicePanel.UIComponents.RefreshConnectionError)
    '            ElseIf e.SyncResults.Exception.Message.Contains("404") Then
    '                lstErrors.Items.Add(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessCompleteWithError)
    '            Else
    '                lstErrors.Items.Add(e.SyncResults.Exception.Message)
    '            End If
    '            blErrorFound = True

    '        Else

    '            If (e.SyncResults.Canceled = False) Then
    '                If TypeOf (e.SyncAgent) is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent Then
    '                    writeError(CType(e.SyncAgent, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent).FeatureSource.Name)
    '                    writeError("   Start: " & e.SyncResults.SyncStartTime.ToLongTimeString())
    '                    writeError("   Completed: " & e.SyncResults.SyncCompleteTime.ToLongTimeString())


    '                End If
    '            End If


    '            End If


    '            'For Each syAgent As SyncAgent In m_MobileSyncAgent.SyncAgents
    '            '    If syAgent.State = SyncState.Synchronizing Then Return


    '            'Next
    '            'RaiseEvent SyncFinished(Not blErrorFound)

    '            'If TypeOf e.SyncAgent is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent Then
    '            '    Dim pFlSyncAgent as Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource.Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent = CType(e.SyncAgent, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent)
    '            '    If pFlSyncAgent.SynchronizationDirection = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then

    '            '    End If


    '            'End If
    '            ''If e.SyncAgent.St = "Posting" Then
    '            '    Dim pLReq As List(Of Esri.ArcGIS.Mobile.FeatureCaching.Request) = m_MobileService.GetPendingRequests()
    '            '    'Loop through each request
    '            '    For Each pReq As Esri.ArcGIS.Mobile.FeatureCaching.Request In pLReq

    '            '        'If there is another post called
    '            '        If pReq.AsyncState = "Posting" Then
    '            '            Return

    '            '        End If
    '            '    Next
    '            '    If e.Error Is Nothing Then
    '            '        RaiseEvent PostFinished(True)
    '            '    Else
    '            '        RaiseEvent PostFinished(False)

    '            '    End If
    '            'ElseIf e.UserState = "RefreshData" Then
    '            '    Dim pLReq As List(Of Esri.ArcGIS.Mobile.FeatureCaching.Request) = m_MobileService.GetPendingRequests()
    '            '    'Loop through each request
    '            '    For Each pReq As Esri.ArcGIS.Mobile.FeatureCaching.Request In pLReq

    '            '        'If there is another post called
    '            '        If pReq.AsyncState = "RefreshData" Then
    '            '            Return

    '            '        End If
    '            '    Next
    '            '    If e.Error Is Nothing Then
    '            '        RaiseEvent RefreshFinished(True)
    '            '    Else
    '            '        RaiseEvent RefreshFinished(False)

    '            '    End If
    '            'End If

    '    Catch ex As Exception
    '        If ex IsNot Nothing Then
    '            MsgBox("Error in the MobileService RequestCompleted event" & vbCrLf & ex.Message)
    '        End If

    '    End Try
    '    'Not required, moved code to sync finish
    '    ''Determine the name or type of request
    '    'If e.UserState = "Extent Changed" Or e.UserState = "RefreshDataInExtent" Then
    '    '    'Get a list of pending request
    '    '    Dim pLReq As List(Of Esri.ArcGIS.Mobile.FeatureCaching.Request) = m_MobileService.GetPendingRequests()
    '    '    'If any request match the name of the request from this tool, return
    '    '    For Each pReq As Esri.ArcGIS.Mobile.FeatureCaching.Request In pLReq
    '    '        If pReq.AsyncState = "Extent Changed" Or pReq.AsyncState = "Posting" Or pReq.AsyncState = "RefreshDataInExtent" Then
    '    '            Return
    '    '        End If
    '    '    Next
    '    '    'If no pending request match ones from this tool, hide the box and add the edits event handler
    '    '    'Hide the indicator
    '    '    m_SyncIndicator.Visible = False
    '    '    'Add the edits monitor handler
    '    '    adjustDataChangeHandlers(True)
    '    '    'Cleanup
    '    '    pLReq = Nothing
    '    'ElseIf e.UserState = "Posting" Then
    '    '    'Get a list of pending request

    '    '    Dim pLReq As List(Of Esri.ArcGIS.Mobile.FeatureCaching.Request) = m_MobileService.GetPendingRequests()
    '    '    Dim bOtherSync As Boolean = False
    '    '    Dim bOtherPost As Boolean = False
    '    '    'Loop through each request
    '    '    For Each pReq As Esri.ArcGIS.Mobile.FeatureCaching.Request In pLReq
    '    '        'If there are a getDataCall
    '    '        If pReq.AsyncState = "Extent Changed" Or pReq.AsyncState = "RefreshDataInExtent" Then
    '    '            bOtherSync = True

    '    '            Exit For
    '    '        End If
    '    '        'If there is another post called
    '    '        If pReq.AsyncState = "Posting" Then
    '    '            bOtherPost = True
    '    '        End If


    '    '    Next
    '    '    'If no pendign request match ones from this tool, add the edit events handler and hide the indicator
    '    '    If bOtherPost = False And bOtherSync = False Then
    '    '        'Hide the indicatof
    '    '        m_SyncIndicator.Visible = False
    '    '        'Adjust the edit label
    '    '        adjustEditLbl()
    '    '        'add the handler
    '    '        adjustDataChangeHandlers(True)
    '    '    ElseIf bOtherPost = True And bOtherSync = False Then
    '    '        'Do nothing if there is a post request still pending
    '    '    ElseIf bOtherPost = False And bOtherSync = True Then

    '    '        'adjsut the labels
    '    '        adjustEditLbl()
    '    '    Else
    '    '        'Do nothing
    '    '    End If

    '    '    pLReq = Nothing

    '    'Else


    '    '    If m_MobileService.GetPendingRequests.Count = 0 Then
    '    '        'Hide the indicatof
    '    '        m_SyncIndicator.Visible = False
    '    '        'Adjust the edit label
    '    '        adjustEditLbl()
    '    '        'add the handler
    '    '        '       adjustDataChangeHandlers(True)
    '    '    End If

    '    'End If

    'End Sub

    'Private Sub m_MobileService_SynchronizationStarted(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_MobileService.SynchronizationStarted
    '    Try
    '        'Check if the map thread needs to be invoked
    '        If m_Map.InvokeRequired Then
    '            Try
    '                m_Map.Invoke(New EventHandler(AddressOf m_MobileService_SynchronizationStarted), sender, e)
    '            Catch
    '            End Try
    '            Return
    '        End If


    '    Catch ex As Exception
    '        MsgBox("Error in the MobileService SynchronizationStarted event" & vbCrLf & ex.Message)
    '    End Try
    'End Sub
    Private Sub m_Map_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.Disposed
        Try
            If m_MobileCache.IsOpen Then
                If m_MobileCache.IsValid Then
                    If m_MobileSyncAgent.IsValid Then
                        If m_MobileSyncAgent.State = Synchronization.SyncState.Synchronizing Then
                            m_MobileSyncAgent.Cancel()
                        End If

                    End If
                End If
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub m_Map_ExtentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.ExtentChanged
        Try
            ' IF the tools are not initilized, quit
            If m_SyncToolsInit = False Then Return
            'if get data on extent change is checked on




        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub m_Map_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles m_Map.KeyUp
        Try


            'If the escape key is pressed, cancel any request
            If e.KeyCode = Keys.Escape Then
                If syncThread IsNot Nothing Then
                    'cancelWorker.RunWorkerAsync()
                    syncThread.Abort()
                    hideSyncInc()
                    TogglePostRefresh(True)

                    RaiseEvent SyncFinished(m_MobileSyncAgent.FeatureSyncAgents)
                    writeError(String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.CanceledText, DateTime.Now.ToLongTimeString()))
                End If


            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub MobileServiceSync_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Try
            If m_Map IsNot Nothing Then
                m_Map.Dispose()
            End If
            If m_MobileCache IsNot Nothing Then
                '     m_MobileCache.Dispose()
            End If
            m_MobileCache = Nothing

            m_Map = Nothing



            If m_ConnectionStatus IsNot Nothing Then
                m_ConnectionStatus.dispose()
            End If
            m_ConnectionStatus = Nothing



            'If m_BinaryTransfer IsNot Nothing Then
            '    m_BinaryTransfer.dispose()
            'End If
            'm_BinaryTransfer = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try


    End Sub
    Private Sub MobileSync_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        Try
            'If the escape key is pressed, cancel any request
            If e.KeyCode = Keys.Escape Then
                m_MobileSyncAgent.Cancel()


            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub m_ConnectionStatus_connectionStateChanged(ByVal connectionStatus As ConnectionState) Handles m_ConnectionStatus.connectionStateChanged
        Try

            'Monitors the connection statues
            If InvokeRequired Then
                Try

                    Invoke(New CheckStatusClass.m_ExtractorDll_ConnectionStatusChangedDelegate(AddressOf m_ConnectionStatus_connectionStateChanged), connectionStatus)

                Catch
                End Try

                Return


            End If
            'If there is a connection
            If connectionStatus = ConnectionState.INTERNET_CONNECTION_LAN Then
                TogglePostRefresh(True)





            Else
                TogglePostRefresh(False)



            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub disableSync()
        MonitorServerConnection(False)
        btnRefresh.Enabled = False
        btnRefresh.BackgroundImage = My.Resources.DownloadGray

        btnPost.Enabled = False
        btnPost.BackgroundImage = My.Resources.UploadGray

    End Sub
    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        Try
            disableSync()
            lstErrors.Items.Clear()

            If cboRefreshOptions.Text = GlobalsFunctions.appConfig.ServicePanel.UIComponents.RefreshAllLayersText Then
                'refresh the data in the current extent against the server
                refreshDataExtent(False)

            Else
                'Refresh the entire layer at scale sepcified

                refreshDataExtent(False, cboRefreshOptions.Text)


            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub


    Private Sub btnPost_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPost.Click
        disableSync()

        Try


            lstErrors.Items.Clear()


            'Post the data
            PostData()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Sub ClearErrors() Handles gpBoxErrors.Click
        Try

            'Clears the error box
            lstErrors.Items.Clear()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub

    Private Sub MobileServiceSync_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize
        Try



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub lstErrors_MouseClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstErrors.MouseClick
        Try

            'Attempt to clear the error box
            If e.Button = Windows.Forms.MouseButtons.Right Then
                lstErrors.Text = ""
                lstErrors.Items.Clear()
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
    'Private Sub m_BinaryTransfer_DownloadCompleted(ByVal File As String) Handles m_BinaryTransfer.DownloadCompleted
    '    Try
    '        'Event raised once the transfer has completed

    '        If File = "Error" Then
    '            MsgBox("There was an error downloading the cache, try again later")
    '            'Reset the display
    '            DownloadComplete()
    '            Return

    '        End If
    '        'process the downloaded cache
    '        CacheDownloaded()
    '        btnDownload.Enabled = True
    '    Catch ex As Exception
    '        MsgBox("Error handling the BinaryTransfer DownloadCompleted" & vbCrLf & ex.Message)

    '    End Try
    'End Sub
    'Private Sub bkGrdUnZip_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bkGrdUnZip.DoWork
    '    Try
    '        'Unzip the cache in a background process
    '        deleteAllFilesInFolder(m_MobileCache.StoragePath)
    '        UnZipDotNetZip(m_MobileCache.StoragePath & "\CacheUpdate\" & m_CacheName, m_MobileCache.StoragePath)
    '    Catch ex As Exception
    '        MsgBox("Error in the background worker-  unzipping" & vbCrLf & ex.Message)

    '    End Try
    'End Sub
    'Private Sub bkGrdUnZip_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bkGrdUnZip.RunWorkerCompleted
    '    Try


    '        'Open the cache back up
    '        m_MobileCache.Open()


    '        Dim pFInfo As FileInfo = New FileInfo(m_MobileCache.StoragePath & "\CacheUpdate\" & m_CacheName)
    '        m_cacheDate = CStr(pFInfo.CreationTime)
    '        'get teh cache creation date and set it to the current cache(stored in the config file)
    '        setCacheDateConfig(m_cacheDate)
    '        pFInfo = Nothing
    '        'Reset the interface
    '        DownloadComplete()

    '        'Raise event  to tell the parent that cache has been updated
    '        RaiseEvent CacheUpdated()
    '    Catch ex As Exception
    '        MsgBox("Error in the completion of the unzip worker" & vbCrLf & ex.Message)

    '    End Try
    'End Sub

    'Private Sub m_BinaryTransfer_FileDate(ByVal FileDate As String) Handles m_BinaryTransfer.FileDate
    '    Try
    '        'Get the file date from the web service
    '        If FileDate = "Error" Then
    '            MsgBox("The server is unavailable")
    '            DownloadComplete()
    '            Return
    '        End If
    '        Dim d1 As DateTime
    '        Dim d2 As DateTime
    '        'Comare the date of the current cache to the one on the web server
    '        If IsDate(m_cacheDate) Then
    '            d1 = CDate(m_cacheDate)
    '            d2 = CDate(FileDate)
    '            'If d2 > d1 Then
    '            If DateTime.Compare(d2, d1) > 0 Then
    '                'if the web service is newer, download
    '                If MsgBox("An Update is available, do you want to download it?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then

    '                    btnDownload.Visible = False
    '                    lblDate.Visible = False
    '                    lblDownloadProgress.Visible = True
    '                    lblDownloadProgress.Text = "Downloading..."
    '                    picDownloading.Visible = True
    '                    'Delete the old cache package if it exist
    '                    If File.Exists(m_MobileCache.StoragePath & "\CacheUpdate\" & m_CacheName) Then
    '                        Try
    '                            File.Delete(m_MobileCache.StoragePath & "\CacheUpdate\" & m_CacheName)
    '                        Catch ex As Exception

    '                        End Try

    '                    End If
    '                    Try

    '                        'Get the file
    '                        m_BinaryTransfer.getFile(m_CacheName, m_MobileCache.StoragePath & "\CacheUpdate\")
    '                        Return
    '                    Catch ex As Exception

    '                    End Try

    '                End If
    '            Else
    '                MsgBox("An update was not available, you may update individual layers with the tools above")
    '            End If

    '        Else 'Handles no date in the config
    '            'Prompt to the user if they want to download the cache
    '            If MsgBox("An Update is available, do you want to download it?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then

    '                btnDownload.Visible = False
    '                lblDate.Visible = False
    '                lblDownloadProgress.Visible = True
    '                lblDownloadProgress.Text = "Downloading..."
    '                picDownloading.Visible = True
    '                'Delete the old cache package if it exist
    '                If File.Exists(m_MobileCache.StoragePath & "\CacheUpdate\" & m_CacheName) Then
    '                    Try
    '                        File.Delete(m_MobileCache.StoragePath & "\CacheUpdate\" & m_CacheName)
    '                    Catch ex As Exception

    '                    End Try

    '                End If
    '                Try

    '                    'get the cache file
    '                    m_BinaryTransfer.getFile(m_CacheName, m_MobileCache.StoragePath & "\CacheUpdate\")
    '                    Return
    '                Catch ex As Exception

    '                End Try

    '            End If
    '        End If

    '        btnDownload.Enabled = True

    '        btnDownload.Visible = True
    '        lblDate.Visible = True
    '        lblDownloadProgress.Visible = False
    '        lblDownloadProgress.Text = "Downloading..."
    '        picDownloading.Visible = False
    '    Catch ex As Exception
    '        MsgBox("Error handling the BinaryTransfer FileDate" & vbCrLf & ex.Message)

    '    End Try
    'End Sub
#End Region
#Region "Private Functions"

    Private Function loadDataSources() As Boolean
        Try


            If GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService.Count > 1 Then
                MsgBox("Two map services are not supported at this time")
                Return False

            End If

            For Each strURL In GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService
                If strURL.StorageLocation = "" Then
                    m_MobileCache = New MobileCache(GlobalsFunctions.generateCachePath)
                Else
                    m_MobileCache = New MobileCache(strURL.StorageLocation)
                End If



                If GlobalsFunctions.UrlIsValid(strURL.Value) Then
                    If GlobalsFunctions.URLIsMobileServer(strURL.Value) Then

                        m_MobileConnect = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
                        m_MobileConnect.Url = strURL.Value
                        If strURL.UserName <> "" Then
                            Try
                                GlobalsFunctions.OverrideCertificateValidation()


                                'Dim webRequest_401 As HttpWebRequest = Nothing

                                'webRequest_401 = HttpWebRequest.Create(New Uri(strURL.Value & "?wsdl"))

                                'webRequest_401.ContentType = "text/xml;charset=""utf-8"""

                                'webRequest_401.Method = "GET"
                                'webRequest_401.Accept = "text/xml"
                                ''  webRequest_401.Credentials = New NetworkCredential(strURL.UserName, strURL.Password, strURL.Domain)

                                'Dim webResponse_401 As HttpWebResponse
                                'Try

                                '    webResponse_401 = webRequest_401.GetResponse()
                                'Catch webex As System.Net.WebException
                                '    Dim webexResponse As HttpWebResponse = webex.Response
                                '    If webexResponse.StatusCode = HttpStatusCode.Unauthorized Then

                                '        If (webRequest_401.Credentials Is Nothing) Then

                                '            webRequest_401 = HttpWebRequest.Create(New Uri(strURL.Value & "?wsdl"))

                                '            webRequest_401.ContentType = "text/xml;charset=""utf-8"""

                                '            webRequest_401.Method = "GET"
                                '            webRequest_401.Accept = "text/xml"
                                '            webRequest_401.Credentials = New NetworkCredential(strURL.UserName, strURL.Password, strURL.Domain)
                                '            Try

                                '                webResponse_401 = webRequest_401.GetResponse()
                                '            Catch webex2 As System.Net.WebException
                                '                MsgBox(webex2.Message)
                                '            End Try

                                '            If (webResponse_401 IsNot Nothing) Then
                                '                webResponse_401.Close()
                                '            End If
                                '            '   m_MobileConnect.WebClientProtocol. = webRequest_401

                                '            m_MobileConnect.WebClientProtocol.Credentials = webRequest_401.Credentials


                                '        Else
                                '        End If


                                '    End If


                                'End Try


                                Dim myCache As CredentialCache = New CredentialCache()

                                myCache.Add(New Uri(strURL.Value), "Basic", New NetworkCredential(strURL.UserName, strURL.Password))
                                myCache.Add(New Uri(strURL.Value), "Digest", New NetworkCredential(strURL.UserName, strURL.Password, strURL.Domain))
                                myCache.Add(New Uri(strURL.Value), "Negotiate", New NetworkCredential(strURL.UserName, strURL.Password, strURL.Domain))


                                m_MobileConnect.WebClientProtocol.Credentials = myCache
                               



                            Catch exTm As Exception
                                MsgBox(GlobalsFunctions.appConfig.ServicePanel.UIComponents.CacheNotValid & vbNewLine & exTm.Message)

                                Return False
                            End Try

                            m_MobileConnect.WebClientProtocolType = WebClientProtocolType.BinaryWebService
                        End If
                    Else
                        Dim pFSRep As FeatureServiceReplicaManager = New FeatureServiceReplicaManager()
                        pFSRep.MobileCache = m_MobileCache
                        pFSRep.Url = strURL.Value

                        If strURL.UserName <> "" Then
                            Try
                                GlobalsFunctions.OverrideCertificateValidation()


                                Dim myCache As CredentialCache = New CredentialCache()

                                myCache.Add(New Uri(strURL.Value), "Basic", New NetworkCredential(strURL.UserName, strURL.Password))
                                myCache.Add(New Uri(strURL.Value), "Digest", New NetworkCredential(strURL.UserName, strURL.Password, strURL.Domain))
                                myCache.Add(New Uri(strURL.Value), "Negotiate", New NetworkCredential(strURL.UserName, strURL.Password, strURL.Domain))


                                pFSRep.Credentials = myCache

                            Catch exTm As Exception
                                MsgBox(GlobalsFunctions.appConfig.ServicePanel.UIComponents.CacheNotValid & vbNewLine & exTm.Message)

                                Return False
                            End Try
                        End If
                        pFSRep.Initialize()

                        ' pFSRep.Synchronize()
                        'm_MobileCache.Open()

                        pFSRep.CreateReplica()
                        

                        ' m_MobileCache.Open()


                    End If

                End If

                'Dim proxy As WebProxy = WebRequest.DefaultWebProxy
                'proxy.BypassList = New String() {strURL.Value}


                'MsgBox(proxy.IsBypassed(New Uri(strURL.Value)))
                If Not m_MobileCache.IsOpen Then


                    'If the service is valid
                    If m_MobileCache.IsValid Then

                        Try

                            m_MobileSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncAgent(m_MobileCache, m_MobileConnect)
                            m_MobileSyncResults = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncResults()
                            m_MobileCache.Open()
                            m_Map.MapLayers.Add(m_MobileCache)
                        Catch ex As Exception
                            Try
                                m_MobileConnect.CreateCache(m_MobileCache)
                                m_MobileCache.Open()
                                m_Map.MapLayers.Add(m_MobileCache)
                                'setCacheDateConfig("")
                            Catch exin As Exception

                                MsgBox(GlobalsFunctions.appConfig.ServicePanel.UIComponents.CacheNotValid & vbCrLf & exin.Message)

                                Return False

                                'setCacheDateConfig("")
                            End Try

                        End Try
                        m_SyncToolsInit = True
                        loadLayers()
                    End If
                Else
                    m_SyncToolsInit = True
                    loadLayers()
                End If
            Next

            If GlobalsFunctions.appConfig.LayerOptions.CachedMaps.CachedMap.Count > 0 Then


                For Each strLocalCache In GlobalsFunctions.appConfig.LayerOptions.CachedMaps.CachedMap

                    If strLocalCache.Value IsNot Nothing Then


                        If System.IO.Directory.Exists(strLocalCache.Value) Then


                            Dim pFlList As List(Of String) = New List(Of String)()
                            pFlList.AddRange(System.IO.Directory.GetFiles(strLocalCache.Value))


                            If pFlList.Contains(strLocalCache.Value & "\MapSchema.bin") Then
                                Dim pTempMobileCache As MobileCache = New MobileCache(strLocalCache.Value)

                                pTempMobileCache.Open()

                                Try


                                    m_Map.MapLayers.Insert(m_Map.MapLayers.Count, pTempMobileCache)
                                Catch ex As Exception
                                    Dim st As New StackTrace
                                    MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                                    st = Nothing

                                End Try

                            Else

                                Dim pTileMapLay As Esri.ArcGIS.Mobile.DataProducts.RasterData.TileMapLayer = New Esri.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer(strLocalCache.Value)
                                pTileMapLay.Open()

                                pTileMapLay.Name = strLocalCache.DisplayName
                                m_Map.MapLayers.Insert(m_Map.MapLayers.Count, pTileMapLay)
                            End If

                        Else

                            Dim strTempPath As String = Path.Combine(GlobalsFunctions.getEXEPath(), "LocalCacheMaps")
                            strTempPath = Path.Combine(strTempPath, strLocalCache.Value)

                            If System.IO.Directory.Exists(strTempPath) Then

                                Dim pFlList As List(Of String) = New List(Of String)()
                                pFlList.AddRange(System.IO.Directory.GetFiles(strTempPath))


                                If pFlList.Contains(strTempPath & "\MapSchema.bin") Then
                                    Dim pTempMobileCache As MobileCache = New MobileCache(strTempPath)

                                    pTempMobileCache.Open()
                                    Try


                                        m_Map.MapLayers.Insert(m_Map.MapLayers.Count, pTempMobileCache)
                                    Catch ex As Exception
                                        Dim st As New StackTrace
                                        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
                                        st = Nothing

                                    End Try
                                Else

                                    Dim pTileMapLay As Esri.ArcGIS.Mobile.DataProducts.RasterData.TileMapLayer = New Esri.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer(strTempPath)
                                    pTileMapLay.Open()

                                    pTileMapLay.Name = strLocalCache.DisplayName
                                    m_Map.MapLayers.Insert(m_Map.MapLayers.Count, pTileMapLay)
                                End If

                            End If


                        End If

                    End If
                Next
            End If


            If GlobalsFunctions.appConfig.LayerOptions.WebMaps.WebMap.Count > 0 Then
                Dim strPath As String = GlobalsFunctions.generateWebMapsCachePath()

                For Each strLocalCache In GlobalsFunctions.appConfig.LayerOptions.WebMaps.WebMap
                    If strLocalCache.Value IsNot Nothing Then




                        Dim strSplit() As String = strLocalCache.Value.Trim().Split(CChar("/"))
                        Dim pWbPath As String = Path.Combine(strPath, strSplit(strSplit.Length - 2))


                        If Directory.Exists(pWbPath) = False Then
                            Directory.CreateDirectory(pWbPath)
                        End If
                        Dim pTileMapLay As Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer = _
                             New Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer(strLocalCache.Value.Trim(), pWbPath)
                        pTileMapLay.Name = strLocalCache.DisplayName

                        '   pTileMapLay.Open()

                        'm_Map.MapLayers.Add(pTileMapLay)
                        m_Map.MapLayers.Insert(m_Map.MapLayers.Count, pTileMapLay)
                    End If


                Next
            End If
            bkGrnOpenLayers.WorkerReportsProgress = True

            bkGrnOpenLayers.RunWorkerAsync()
            Return True



        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False

        End Try
    End Function

    Public Function LoadSettings() As Boolean
        Try
            m_SyncToolsInit = False
            Return loadDataSources()


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

            Return False

        End Try
    End Function
    Private Sub GetServerToken()

        'Try

        '   Dim cs As CatalogService = New CatalogService()

        '    cs.Url = m_MobileConnect.Url


        '    m_bRequiretokens = cs.RequiresTokens()

        '    '' if token is required
        '    'If (m_bRequiretokens) Then

        '    '    ' step 2. get the url for token server
        '    '    Dim tokenserviceurl As String = cs.GetTokenServiceURL()

        '    '    'step 3. create a tokencredential
        '    '    Dim tokencredential As TokenCredential = New TokenCredential(getServiceUserName, getServicePassword)


        '    '    ' step 4. use a tokengenerator to get the token from token server
        '    '    Dim tokengenerator As TokenGenerator = New TokenGenerator()


        '    '    m_Token = tokengenerator.GenerateToken(tokenserviceurl, tokencredential)

        '    '    m_MobileService.ServiceConnection.TokenCredential = tokencredential

        '    'Else

        '    m_bRequiretokens = False
        '    m_Token = "NoTokenNeeded"
        '    'End If



        'Catch ex As Exception


        '    m_Token = "-99"

        'End Try

    End Sub
    'Private Sub deleteAllFilesInFolder(ByVal folderPath As String)

    '    Try
    '        Dim s As String
    '        For Each s In System.IO.Directory.GetFiles(folderPath)
    '            System.IO.File.Delete(s)
    '        Next s
    '    Catch ex As Exception
    '        MsgBox("Error deleting old cache" & vbCrLf & ex.Message)
    '    End Try

    'End Sub
    ''Private Sub UnZipDotNetZip(ByVal ZipFile As String, ByVal UnZipPath As String)
    '    Dim pUnZip As Ionic.Zip.ZipFile
    '    Try

    '        'Unzip the cache
    '        pUnZip = New Ionic.Zip.ZipFile(ZipFile)

    '        pUnZip.ExtractAll(UnZipPath, True)

    '        pUnZip.Dispose()
    '        pUnZip = Nothing

    '    Catch ex As Exception
    '        MsgBox("Error unzipping cache" & vbCrLf & ex.Message)
    '    End Try

    'End Sub

    'Private Function getURLFromConfig() As String
    '    Try
    '        If GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService.Count > 0 Then
    '            Return GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService(0).Value
    '        Else
    '            Return ""
    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error getting map service URL" & vbCrLf & ex.Message)
    '        Return Nothing
    '    End Try
    'End Function

    Private Sub adjustDataChangeHandlers(ByVal Add As Boolean)
        Try
            If m_MobileCache Is Nothing Then Return

            'Add or remove the event handler to monitor edits

            'If the handlers are already added, exit
            If m_HandlerState = Add Then Return
            'Update the handler state
            m_HandlerState = Add
            'Loop through all layers and add/remove the handlers
            For Each pFS As FeatureSource In m_MobileCache.FeatureSources
                'Only for feature layers


                If pFS.Name = GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogLayer Then
                    'pass
                Else

                    Select Case Add
                        Case True

                            AddHandler pFS.DataChanged, AddressOf layer_DataChanged

                        Case Else
                            RemoveHandler pFS.DataChanged, AddressOf layer_DataChanged

                    End Select
                End If
            Next
        Catch ex As Exception

            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub

    Private Sub loadLayers()
        Try
            If checkToolStatus() = False Then Return
            If GlobalsFunctions.appConfig.LayerOptions.MobileServices.LimitQuery <> "" Then
                GlobalsFunctions.SetDefQueryOnLayers(GlobalsFunctions.appConfig.LayerOptions.MobileServices.LimitQuery, m_Map)

            End If

            'Loop through all layers and add/remove the handlers
            loadSyncLayers()

            cboRefreshOptions.Items.Clear()
            If GlobalsFunctions.appConfig.ServicePanel.RefreshList.ShowAllLayers.ToUpper = "TRUE" Then
                cboRefreshOptions.Items.Add(GlobalsFunctions.appConfig.ServicePanel.UIComponents.RefreshAllLayersText)
            End If

            If (m_layersToRefresh IsNot Nothing) Then
                For Each lySync As LayersToSync In m_layersToRefresh

                    ' If m_MobileCache.Layers.Item(strLay) IsNot Nothing Then
                    cboRefreshOptions.Items.Add(lySync.RefreshGroupName)
                    'End If

                Next

            Else
                For Each pL As FeatureSource In m_MobileCache.FeatureSources
                    cboRefreshOptions.Items.Add(pL.Name)
                Next
            End If

            cboRefreshOptions.SelectedIndex = 0
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Sub loadSyncLayers()
        Try
            If GlobalsFunctions.appConfig.ServicePanel.RefreshList.RefreshGroup.Count <> 0 Then
                m_layersToRefresh = New ArrayList
                Dim pLyGrp As LayersToSync

                For Each strLayers In GlobalsFunctions.appConfig.ServicePanel.RefreshList.RefreshGroup








                    pLyGrp = New LayersToSync(strLayers.Name, strLayers.RefreshLayer)

                    m_layersToRefresh.Add(pLyGrp)




                Next
            End If
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub



    Delegate Sub ChangeTextControlDelegate(ByVal message As String)

    Private Sub writeError(ByVal message As String)
        If lstErrors.InvokeRequired Then
            lstErrors.Invoke(New ChangeTextControlDelegate(AddressOf writeError), New Object() {message})
            Return
        Else
            ' lstErrors.Items.Add(message)
            lstErrors.Items.Insert(0, message)
        End If

    End Sub
    Delegate Sub ChangeCheckControlDelegate(ByVal CheckState As Boolean)


    Delegate Sub checkProgressStatusDelegate()

    Private Sub checkProgressStatus()
        'If m_SyncIndicator.InvokeRequired Then
        '    m_SyncIndicator.Invoke(New checkProgressStatusDelegate(AddressOf checkProgressStatus))
        '    Return

        'End If
        If m_MobileSyncAgent.State = Synchronization.SyncState.Ready Then

            hideSyncInc()

            RaiseEvent SyncFinished(m_MobileSyncAgent.FeatureSyncAgents)

        Else
            showSyncInc()
        End If
    End Sub
    Private Sub hideSyncInc()
        'Hide the indicator
        RaiseEvent showIndicator(False)

        'm_SyncIndicator.Visible = False
        'Adjust the edit label
        adjustEditLbl()
        'add the handler if monitoring edits
        If m_MonitorEdits Then
            adjustDataChangeHandlers(True)
        End If
    End Sub
    Private Sub showSyncInc()
        If m_SyncToolsInit = False Then Return
        If m_ShowSyncIndicator Then
            RaiseEvent showIndicator(True)

            'Remove the edit monitor event handles

        End If
    End Sub
    Public Sub updateStatus(adjustState As Boolean)
        If m_MonitorEdits Then
            adjustDataChangeHandlers(adjustState)
        End If

    End Sub
    Private Function checkToolStatus() As Boolean
        Try
            If m_SyncToolsInit = False Then Return False
            If m_Map.IsValid = False Then Return False

            If m_MobileCache.IsValid = False Then Return False
            If m_MobileCache.IsOpen = False Then Return False
            Return True
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Function
    'Public Sub refreshDataExtent(ByVal useScale As Boolean, ByVal refresh As Boolean, ByVal Extent As Esri.ArcGIS.Mobile.Geometries.Polygon, Optional ByVal layers As List(Of Esri.ArcGIS.Mobile.MapLayer) = Nothing)

    '    Try
    '        'Remove the edit monitor event handles
    '        If m_MonitorEdits Then
    '            adjustDataChangeHandlers(False)
    '        End If
    '        'Get data and determine if to use the display scale or not
    '        'If useScale Then
    '        postAllEditsByFeat(Synchronization.SyncDirection..DownloadOnly, layers)

    '        'getDataForLayersInExtent(Extent, m_Map.Scale, refresh, layers)
    '        ' m_MobileService.GetDataAsync(Extent, m_Map.Scale, refresh, "RefreshData", layers)

    '        'Else
    '        'postAllEditsByFeat(layers, esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.DownloadOnly)

    '        ''m_MobileService.GetDataAsync(Extent, 0, refresh, "RefreshData", layers)
    '        ''getDataForLayersInExtent(Extent, 0, refresh, layers)
    '        'End If
    '    Catch ex As Exception
    '        MsgBox("Error refreshing data by extent" & vbCrLf & ex.Message)
    '    End Try
    'End Sub


    Delegate Sub adjustEditLblDelegate()


    Private Sub adjustEditLbl()
        Try
            If lblNumEdits.InvokeRequired Then
                lblNumEdits.Invoke(New adjustEditLblDelegate(AddressOf adjustEditLbl))
                Return

            End If
            'Get the number of edits
            m_edits = NumberOfEdits()
            If m_MonitorEdits = True Then
                'Update the label
                lblNumEdits.Text = String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.EditMonitorLabel, m_edits)
            End If

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Private Function NumberOfEdits() As Integer
        Try
            'Get the edits for each layer
            If m_MobileCache.IsOpen = False Then Return -1

            Dim pEditCnt As Integer = 0
            Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
            For Each pL As FeatureSource In m_MobileCache.FeatureSources
                If TypeOf pL Is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
                    If pL.Name <> GlobalsFunctions.appConfig.NavigationOptions.GPS.GPSLogLayer Then
                        pFL = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

                        pEditCnt = pEditCnt + pFL.EditsCount
                    End If
                End If
            Next
            Return pEditCnt
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Function
    Private Function isSyncing() As Boolean

        For Each syAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent In m_MobileSyncAgent.FeatureSyncAgents
            If syAgent.State = Synchronization.SyncState.Synchronizing Then Return True

        Next
        Return False

    End Function
    'Private Function getDataForLayersInExtent(ByVal extent As Esri.ArcGIS.Mobile.Geometries.Polygon, _
    '                                          ByVal scale As Double, ByVal Refresh As Boolean, _
    '                                          Optional ByVal LayersToSync As List(Of Esri.ArcGIS.Mobile.MapLayer) = Nothing) As Integer
    '    Try
    '        'Get the edits for each layer
    '        If m_MobileCache.IsOpen = False Then Return 0

    '        Dim pLayerCnt As Integer = 0
    '        m_MobileSyncAgent.SyncAgents.Clear()

    '        If LayersToSync Is Nothing Then
    '            LayersToSync = New List(Of Esri.ArcGIS.Mobile.MapLayer)

    '        End If
    '        If LayersToSync.Count = 0 Then

    '            For Each pL as MapLayer In m_MobileCache.Layers
    '                If TypeOf pL is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
    '                    Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL, m_MobileConnect)
    '                    pFLSyncAgent.SynchronizationDirection = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.DownloadOnly
    '                    If extent IsNot Nothing Then
    '                        Dim pQf As Esri.ArcGIS.Mobile.FeatureCaching.QueryFilter = New QueryFilter
    '                        pQf.Geometry = extent
    '                        pQf.GeometricRelationship = GeometricRelationshipType.Any


    '                        pFLSyncAgent.DownloadFilter = pQf

    '                    End If

    '                    pLayerCnt = pLayerCnt + 1

    '                    m_MobileSyncAgent.SyncAgents.Add(pFLSyncAgent)
    '                ElseIf TypeOf pL Is Esri.ArcGIS.Mobile.FeatureCaching.RasterLayer Then
    '                    Dim pRL As Esri.ArcGIS.Mobile.FeatureCaching.RasterLayer = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.RasterLayer)
    '                    Dim pRLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.RasterLayerSyncAgent = New RasterLayerSyncAgent(pRL, m_MobileConnect)
    '                    If extent IsNot Nothing Then

    '                        pRLSyncAgent.Extent = extent.Extent
    '                    End If

    '                    pRLSyncAgent.LowerScale = scale



    '                    pLayerCnt = pLayerCnt + 1

    '                    m_MobileSyncAgent.SyncAgents.Add(pRLSyncAgent)
    '                End If
    '            Next
    '        Else


    '            For Each pL as MapLayer In LayersToSync
    '                If TypeOf pL is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
    '                    Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL, m_MobileConnect)
    '                    pFLSyncAgent.SynchronizationDirection = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.DownloadOnly
    '                    If extent IsNot Nothing Then
    '                        Dim pQf As Esri.ArcGIS.Mobile.FeatureCaching.QueryFilter = New QueryFilter
    '                        pQf.Geometry = extent
    '                        pQf.GeometricRelationship = GeometricRelationshipType.Any


    '                        pFLSyncAgent.DownloadFilter = pQf
    '                    End If

    '                    pLayerCnt = pLayerCnt + 1

    '                    m_MobileSyncAgent.SyncAgents.Add(pFLSyncAgent)
    '                ElseIf TypeOf pL Is Esri.ArcGIS.Mobile.FeatureCaching.RasterLayer Then
    '                    Dim pRL As Esri.ArcGIS.Mobile.FeatureCaching.RasterLayer = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.RasterLayer)
    '                    Dim pRLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.RasterLayerSyncAgent = New RasterLayerSyncAgent(pRL, m_MobileConnect)
    '                    If extent IsNot Nothing Then

    '                        pRLSyncAgent.Extent = extent.Extent
    '                    End If

    '                    pRLSyncAgent.LowerScale = scale



    '                    pLayerCnt = pLayerCnt + 1

    '                    m_MobileSyncAgent.SyncAgents.Add(pRLSyncAgent)
    '                End If
    '            Next
    '        End If

    '        '   m_MobileSyncAgent.Synchronize()
    '        writeError("Download Started at " & DateTime.Now.ToLongTimeString())


    '        m_MobileSyncAgent.BeginDownloadExtent(extent.Extent, 0, scale, AddressOf downloadCallback, "Sync")

    '        Return pLayerCnt
    '    Catch ex As Exception
    '        MsgBox("Error posting all edits" & vbCrLf & ex.Message)
    '    End Try
    'End Function

    'Public Delegate Sub downloadCallbackDelegate( _
    ' ByVal ar As IAsyncResult _
    ')
    'Private Sub downloadCallback(ByVal result As IAsyncResult)

    '    If m_MobileSyncAgent.Canceled Then
    '        writeError("Canceled" & DateTime.Now.ToLongTimeString())
    '    Else
    '        writeError("Completed" & DateTime.Now.ToLongTimeString())
    '    End If
    '    ''If TypeOf (result) Is Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheSyncAgent.DownloadAsyncResult Then
    '    'writeError("Completed" & DateTime.Now.ToLongTimeString())
    '    'writeError(m_MobileSyncAgent.SyncAgents.Count.ToString)

    '    '' End If

    '    'writeError("Completed" & DateTime.Now.ToLongTimeString())
    '    'writeError("Download Completed at " & DateTime.Now.ToLongTimeString())

    'End Sub



    Private Function postAllEditsByFeat(ByVal syncDir As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection, _
                                        Optional ByVal LayersToPost As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = Nothing, _
                                        Optional ByVal extent As Esri.ArcGIS.Mobile.Geometries.Polygon = Nothing) As Integer
        Try
            showSyncInc()

            'Get the edits for each layer
            If m_MobileCache.IsOpen = False Then Return -1

            Dim pLayerCnt As Integer = 0
            Dim featLToSync As ArrayList = New ArrayList
            Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent
            Dim pQf As Esri.ArcGIS.Mobile.FeatureCaching.QueryFilter
            'm_MobileSyncAgent.SyncAgents.Clear()

            If LayersToPost Is Nothing Then


                For Each pL As MapLayer In m_Map.MapLayers
                    If TypeOf pL Is MobileCacheMapLayer Then

                        For Each pFS As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource In CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer).MobileCache.FeatureSources



                            If pFS.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                            Else
                                ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager

                                pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFS)
                                pFLSyncAgent.SynchronizationDirection = syncDir
                                pFLSyncAgent.SynchronizeAttachments = True

                                pFLSyncAgent.MapDocumentConnection = m_MobileConnect
                                If extent IsNot Nothing Then

                                    pQf = New QueryFilter

                                    pQf.Geometry = extent.Extent
                                    pQf.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect


                                    pFLSyncAgent.DownloadFilter = pQf
                                    'pFLSyncAgent.UploadFilter = pQf
                                End If

                                featLToSync.Add(pFLSyncAgent)

                                pLayerCnt = pLayerCnt + 1


                            End If

                        Next
                    End If
                Next
            ElseIf LayersToPost.Count = 0 Then


                For Each pFS As FeatureSource In m_MobileCache.FeatureSources
                    If TypeOf pFS Is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then





                        If pFS.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                        Else
                            ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager

                            pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFS)
                            pFLSyncAgent.SynchronizationDirection = syncDir
                            pFLSyncAgent.SynchronizeAttachments = True

                            pFLSyncAgent.MapDocumentConnection = m_MobileConnect
                            If extent IsNot Nothing Then

                                pQf = New QueryFilter

                                pQf.Geometry = extent.Extent
                                pQf.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect


                                pFLSyncAgent.DownloadFilter = pQf
                                'pFLSyncAgent.UploadFilter = pQf
                            End If

                            featLToSync.Add(pFLSyncAgent)

                            pLayerCnt = pLayerCnt + 1


                        End If
                    End If
                Next
            Else

                For Each pConfigValue As MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer In LayersToPost
                    Dim pFS As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(pConfigValue.Name, m_Map).FeatureSource




                    If pFS.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                    Else

                        pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFS)
                        pFLSyncAgent.SynchronizationDirection = syncDir
                        pFLSyncAgent.SynchronizeAttachments = True


                        pFLSyncAgent.MapDocumentConnection = m_MobileConnect
                        If extent IsNot Nothing Then
                            pQf = New QueryFilter

                            If pConfigValue.Extent.ToUpper = "FULL" Then
                                pQf.Geometry = m_Map.FullExtent
                            Else
                                pQf.Geometry = extent.Extent
                            End If
                            If pConfigValue.WhereClause <> "" Then
                                pQf.WhereClause = pConfigValue.WhereClause

                            End If

                            pQf.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect


                            pFLSyncAgent.DownloadFilter = pQf
                            ' pFLSyncAgent.UploadFilter = pQf

                        End If
                        featLToSync.Add(pFLSyncAgent)

                        'Dim ca As New AsyncPostFeatureSourceClass()
                        'ca.Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent = pFLSyncAgent
                        'ca.StartDelegate = AddressOf SyncLayerAsync
                        'Dim t As New Thread(AddressOf ca.syncFeatureSource)
                        'm_PostLayCount = m_PostLayCount + 1
                        't.IsBackground = True
                        't.Start()


                    End If

                Next
            End If

            Dim ca As New AsyncPostFeatureSourceClass()
            ca.FeatureSourceSyncAgent = featLToSync

            ca.StartDelegate = AddressOf SyncLayerAsync
            syncThread = New Thread(AddressOf ca.syncFeatureSource)
            m_PostLayCount = m_PostLayCount + 1
            syncThread.IsBackground = True
            syncThread.Start()


            Return pLayerCnt
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        Finally
            If m_PostLayCount < 1 Then
                hideSyncInc()
                TogglePostRefresh(True)
            End If

        End Try

    End Function

    'Private Function postAllEditsByFeat(ByVal syncDir As esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection, _
    '                                      Optional ByVal LayersToPost As List(Of Esri.ArcGIS.Mobile.MapLayer) = Nothing, _
    '                                      Optional ByVal extent As Esri.ArcGIS.Mobile.Geometries.Polygon = Nothing) As Integer
    '    Try
    '        showSyncInc()

    '        'Get the edits for each layer
    '        If m_MobileCache.IsOpen = False Then Return -1

    '        Dim pLayerCnt As Integer = 0
    '        Dim featLToSync As ArrayList = New ArrayList
    '        Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent
    '        Dim pQf As Esri.ArcGIS.Mobile.FeatureCaching.QueryFilter
    '        'm_MobileSyncAgent.SyncAgents.Clear()
    '        If LayersToPost Is Nothing Then


    '            For Each pL as MapLayer In m_MobileCache.Layers
    '                If TypeOf pL is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then


    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)



    '                    If pFL.HasEdits = False And syncDir = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
    '                    Else
    '                        ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager

    '                        pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
    '                        pFLSyncAgent.SynchronizationDirection = syncDir
    '                        pFLSyncAgent.MapDocumentConnection = m_MobileConnect
    '                        If extent IsNot Nothing Then

    '                            pQf = New QueryFilter

    '                            pQf.Geometry = extent.Extent
    '                            pQf.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect


    '                            pFLSyncAgent.DownloadFilter = pQf
    '                            'pFLSyncAgent.UploadFilter = pQf
    '                        End If

    '                        featLToSync.Add(pFLSyncAgent)

    '                        pLayerCnt = pLayerCnt + 1


    '                    End If
    '                End If
    '            Next
    '        Else

    '            For Each pL as MapLayer In LayersToPost
    '                If TypeOf pL is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                    If pFL.HasEdits = False And syncDir = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
    '                    Else

    '                        pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
    '                        pFLSyncAgent.SynchronizationDirection = syncDir

    '                        pFLSyncAgent.MapDocumentConnection = m_MobileConnect
    '                        If extent IsNot Nothing Then
    '                            pQf = New QueryFilter

    '                            pQf.Geometry = extent.Extent
    '                            pQf.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect


    '                            pFLSyncAgent.DownloadFilter = pQf
    '                            ' pFLSyncAgent.UploadFilter = pQf

    '                        End If
    '                        featLToSync.Add(pFLSyncAgent)

    '                        'Dim ca As New AsyncPostFeatureSourceClass()
    '                        'ca.Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent = pFLSyncAgent
    '                        'ca.StartDelegate = AddressOf SyncLayerAsync
    '                        'Dim t As New Thread(AddressOf ca.syncFeatureSource)
    '                        'm_PostLayCount = m_PostLayCount + 1
    '                        't.IsBackground = True
    '                        't.Start()


    '                    End If
    '                End If

    '            Next
    '        End If

    '        Dim ca As New AsyncPostFeatureSourceClass()
    '        ca.Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent = featLToSync

    '        ca.StartDelegate = AddressOf SyncLayerAsync
    '        syncThread = New Thread(AddressOf ca.syncFeatureSource)
    '        m_PostLayCount = m_PostLayCount + 1
    '        syncThread.IsBackground = True
    '        syncThread.Start()


    '        Return pLayerCnt
    '    Catch ex As Exception
    '        Dim st As New StackTrace
    '        MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
    '        st = Nothing

    '    Finally
    '        If m_PostLayCount < 1 Then
    '            hideSyncInc()
    '        End If

    '    End Try

    'End Function

    Private Sub SyncLayerAsync(ByVal FeatureSyncAgent As ArrayList)
        Try

            writeError(String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessStarted, DateTime.Now.ToLongTimeString()))

            Dim hadErrors As Boolean = False
            For Each syncAg As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent In FeatureSyncAgent
                Dim strStart As String = "  "
                writeError(strStart & syncAg.FeatureSource.Name)
                strStart = strStart & strStart

                If syncAg.IsValid = False Then

                    writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.NotValidText))

                Else
                    writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.StartedText, DateTime.Now.ToLongTimeString()))


                    Dim pSyncRes As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncResults = syncAg.Synchronize()

                    If pSyncRes.Exception Is Nothing Then

                        writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.FinishedText, DateTime.Now.ToLongTimeString()))

                    Else



                        writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ErrorText))


                        Dim strMain As String = pSyncRes.Exception.Message
                        Dim len As Integer = strMain.Length
                        Dim newString As String = ""
                        Dim myArray As New ArrayList()
                        Dim intSplitDis As Integer = 40
                        While len > intSplitDis
                            newString = strMain.Substring(0, intSplitDis)
                            strMain = strMain.Substring(intSplitDis)
                            writeError("     " & newString)
                            myArray.Add(newString)
                            len = strMain.Length
                        End While
                        writeError(strStart & strStart & strMain)
                        myArray.Add(strMain)

                        ' writeError(pSyncRes.Exception.Message)
                        hadErrors = True

                    End If
                End If
            Next
            If hadErrors Then
                writeError(String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessCompleteWithError, DateTime.Now.ToLongTimeString()))


            Else
                writeError(String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessComplete, DateTime.Now.ToLongTimeString()))

            End If

        Catch ex As Exception
            If ex.Message.ToString.Contains("Thread was being") Then

            Else
                writeError(ex.Message)

            End If
        Finally

            MonitorServerConnection(True)

        End Try
        checkProgressStatus()
        '  m_PostLayCount = m_PostLayCount - 1
        '   If m_PostLayCount < 1 Then

        '    End If
    End Sub
    'Private Function postAllEdits(Optional ByVal LayersToPost() As Esri.ArcGIS.Mobile.MapLayer = Nothing) As Integer
    '    Try
    '        'Get the edits for each layer
    '        If m_MobileCache.IsOpen = False Then Return -1

    '        Dim pLayerCnt As Integer = 0
    '        m_MobileSyncAgent.SyncAgents.Clear()
    '        If LayersToPost Is Nothing Then


    '            For Each pL as MapLayer In m_MobileCache.Layers
    '                If TypeOf pL is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then


    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
    '                    If pFL.HasEdits Then
    '                        ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager

    '                        Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
    '                        pFLSyncAgent.SynchronizationDirection = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly
    '                        pFLSyncAgent.MapDocumentConnection = m_MobileConnect

    '                        pLayerCnt = pLayerCnt + 1

    '                        m_MobileSyncAgent.SyncAgents.Add(pFLSyncAgent)
    '                    End If
    '                End If
    '            Next
    '        Else

    '            For Each pL as MapLayer In LayersToPost
    '                If TypeOf pL is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                    If pFL.HasEdits Then
    '                        Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSourceSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
    '                        pFLSyncAgent.SynchronizationDirection = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly
    '                        pFLSyncAgent.MapDocumentConnection = m_MobileConnect

    '                        pLayerCnt = pLayerCnt + 1

    '                        m_MobileSyncAgent.SyncAgents.Add(pFLSyncAgent)
    '                    End If
    '                End If

    '            Next
    '        End If
    '        If m_MobileSyncAgent.IsValid = False Then
    '            MsgBox("The Sync Agent is not valid at this time")
    '            Return -1


    '        End If
    '        If m_MobileSyncAgent.State = SyncState.Synchronizing Then
    '            m_MobileSyncAgent.Cancel()

    '        End If
    '        m_MobileSyncAgent.Cancel()
    '        Dim pSyncRes As SyncResults = m_MobileSyncAgent.Synchronize()
    '        If pSyncRes.Exception IsNot Nothing Then
    '            If pSyncRes.Exception.Message <> "" Then
    '                MsgBox(pSyncRes.Exception.Message)

    '            End If
    '        End If


    '        Return pLayerCnt
    '    Catch ex As Exception
    '        MsgBox("Error posting all edits" & vbCrLf & ex.Message)
    '    End Try

    'End Function

    Private Sub MonitorServerConnection(ByVal Start As Boolean)
        Try
            'Stop/Start the service monitoring
            Select Case Start
                Case True
                    If m_MobileConnect.Url IsNot Nothing Then
                        m_ConnectionStatus.startChecking(m_MobileConnect.Url & "?wsdl", m_MobileConnect.WebClientProtocol.Credentials)
                    End If

                Case Else
                    m_ConnectionStatus.stopChecking()
            End Select


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub


#End Region

#Region "CodeNotUsed"
    'Private Sub InitDownloadService()
    '    ThreadPool.SetMaxThreads(8, 8)

    '    If AuthenticateWebService() = False Then
    '        '     btnDownload.Enabled = False
    '    Else
    '        '      btnDownload.Enabled = True
    '    End If
    'End Sub
    'Private Sub Download()

    '    Try
    '        m_WebService.Ping()
    '    Catch ex As Exception
    '        MsgBox("The update server was not available, please check your connection.", MsgBoxStyle.ApplicationModal)
    '        Return

    '    End Try
    '    Try

    '        m_WebService.Timeout = 500
    '        Dim guid As String = System.Guid.NewGuid().ToString()
    '        m_TaskPanel.RemoveItemsWhenFinished = True
    '        m_TaskPanel.RemoveItemsOnError = True
    '        m_TaskPanel.AddOperation(New TaskPanelOperation(guid, String.Format("Downloading {0}", System.IO.Path.GetFileName(m_CacheName)), ProgressBarStyle.Blocks))
    '        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf DownloadFile), New Triplet(guid, m_CacheName, 0))
    '    Catch ex As Exception
    '        MsgBox("Download: Error setting up the download" & vbCrLf & ex.Message)
    '        DownloadComplete(Nothing, Nothing)
    '    End Try
    'End Sub


    'Private Sub DownloadFile(ByVal triplet As Object)
    '    Dim ftd As MTOM_Library.FileTransferDownload
    '    Try


    '        Dim guid As String = CType(triplet, Triplet).First.ToString()

    '        Dim path As String = CType(triplet, Triplet).Second.ToString()
    '        Dim offset As Long = Int64.Parse(CType(triplet, Triplet).Third.ToString())

    '        ftd = New MTOM_Library.FileTransferDownload
    '        ftd.MaxRetries = 4

    '        ftd.WebService.CookieContainer = m_WebService.CookieContainer '' copy the CookieContainer into the transfer object (for auth cookie, if relevant)
    '        ftd.Guid = guid
    '        ' set up the chunking options

    '        ftd.AutoSetChunkSize = True
    '        '     Else
    '        '{
    '        '	ftd.AutoSetChunkSize = false;
    '        '	ftd.ChunkSize = (int)this.dudChunkSize.Value * 1024;	// kb
    '        '}
    '        'set the remote file name and start the background worker
    '        ftd.RemoteFileName = System.IO.Path.GetFileName(path)

    '        If Directory.Exists(m_MobileService.CacheStoragePath & "\CacheUpdate") = False Then
    '            Directory.CreateDirectory(m_MobileService.CacheStoragePath & "\CacheUpdate")
    '        Else

    '        End If

    '        ftd.LocalSaveFolder = m_MobileService.CacheStoragePath & "\CacheUpdate"

    '        ftd.IncludeHashVerification = True
    '        AddHandler ftd.ProgressChanged, AddressOf ft_ProgressChanged
    '        AddHandler ftd.RunWorkerCompleted, AddressOf ft_RunWorkerCompleted
    '        ftd.RunWorkerSync(New DoWorkEventArgs(0))
    '    Catch ex As Exception
    '        MsgBox("DownloadFile: Error Downloading Cache" & vbCrLf & ex.Message)
    '        If InvokeRequired Then
    '            Try
    '                Invoke(New EventHandler(AddressOf DownloadComplete), Nothing, Nothing)

    '            Catch e2x As Exception

    '            End Try
    '            Return

    '        End If

    '    End Try



    'End Sub
    'Private Function AuthenticateWebService() As Boolean

    '    Try


    '        If m_WebService.CookieContainer IsNot Nothing Then
    '            If m_WebService.CookieContainer.Count > 0 Then
    '                Return True 'already authenticated
    '            End If

    '        End If

    '        Dim strUsername, strPassword As String

    '        strUsername = getUserName()
    '        strPassword = getPassword()
    '        Dim req As HttpWebRequest
    '        If strUsername = "" Then
    '            Return True
    '            '      req = HttpWebRequest.Create(m_WebService.Url)
    '        Else
    '            'end a HTTP web request to the login.aspx page, using the querystring to pass in username and password
    '            Dim postData As String = String.Format("?Username={0}&Password={1}", strUsername, strPassword)
    '            Dim url As String = m_WebService.Url.Replace("MTOM.asmx", "") + "Login.aspx" + postData ' get the path of the login page, assuming it is in the same folder as the web service
    '            req = HttpWebRequest.Create(url)

    '        End If

    '        req.CookieContainer = New CookieContainer()
    '        Dim response As HttpWebResponse = req.GetResponse()


    '        ' copy the cookie container to the web servicreqes
    '        m_WebService.CookieContainer = req.CookieContainer

    '        Return (response.Cookies.Count > 0) ' true if the server sent an auth cookie, i.e. authenticated successfully
    '    Catch ex As Exception
    '        MsgBox("AuthenticateWebService: Error Authenticating Cache" & vbCrLf & ex.Message)

    '    End Try

    'End Function
    'Private Sub ft_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
    '    Try


    '        'MsgBox(e.UserState.ToString())

    '        m_TaskPanel.ProgressChanged(sender.Guid, e.ProgressPercentage, e.UserState.ToString())
    '    Catch ex As Exception
    '        MsgBox("ProgressChanged:" & vbCrLf & ex.Message)

    '    End Try
    'End Sub
    'Private Sub ft_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
    '    Try


    '        Dim guid As String = CType(sender, FileTransferBase).Guid

    '        If (e.Error IsNot Nothing) Then

    '            m_TaskPanel.EndOperation(guid, e.Error)
    '            If InvokeRequired Then
    '                Try
    '                    Invoke(New EventHandler(AddressOf DownloadComplete), sender, e)
    '                    Return
    '                Catch ex As Exception

    '                End Try
    '            End If
    '            Return

    '        ElseIf (e.Cancelled) Then

    '            m_TaskPanel.EndOperation(guid, New Exception("Cancelled"))
    '            If InvokeRequired Then
    '                Try
    '                    Invoke(New EventHandler(AddressOf DownloadComplete), sender, e)
    '                    Return
    '                Catch ex As Exception

    '                End Try
    '            End If
    '            Return

    '        Else

    '            m_TaskPanel.EndOperation(guid, Nothing)

    '            If File.Exists(m_MobileService.CacheStoragePath & "\CacheUpdate\" & m_CacheName) Then

    '                Try
    '                    If MsgBox("The update has been downloaded, do you want to update you cache?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
    '                        If NumberOfEdits() > 0 Then
    '                            If MsgBox("There are edits in your data, if you update the cache, the edits will be deleted, Proceed?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
    '                                If InvokeRequired Then
    '                                    Try
    '                                        Invoke(New EventHandler(AddressOf updateCache), sender, e)
    '                                        Return
    '                                    Catch ex As Exception

    '                                    End Try
    '                                End If
    '                            End If
    '                        Else

    '                            If InvokeRequired Then
    '                                Try
    '                                    Invoke(New EventHandler(AddressOf updateCache), sender, e)
    '                                    Return
    '                                Catch ex As Exception

    '                                End Try
    '                            End If
    '                        End If

    '                    End If

    '                Catch ex1 As Exception
    '                    Console.Error.WriteLine("exception: {0}", ex1.ToString)
    '                End Try

    '            End If


    '        End If
    '        If InvokeRequired Then
    '            Try
    '                Invoke(New EventHandler(AddressOf DownloadComplete), sender, e)

    '            Catch ex As Exception

    '            End Try
    '            Return
    '        End If
    '    Catch ex As Exception
    '        MsgBox("Error finishing download" & vbCrLf & ex.Message)
    '    End Try
    'End Sub

    'Private Sub DownloadComplete(ByVal sender As Object, ByVal e As EventArgs)
    '    m_TaskPanel.Controls.Clear()
    '    m_TaskPanel.List.Clear()

    '    m_TaskPanel.Visible = False
    '    btnDownload.Visible = True
    '    lblDate.Visible = True

    '    lblDate.Text = "Cache Date: " & m_cacheDate
    '    If File.Exists(m_MobileService.CacheStoragePath & "\CacheUpdate\" & m_CacheName) Then
    '        Try
    '            File.Delete(m_MobileService.CacheStoragePath & "\CacheUpdate\" & m_CacheName)
    '        Catch ex As Exception

    '        End Try

    '    End If

    'End Sub
    'Private Sub updateCache(ByVal sender As Object, ByVal e As EventArgs)
    '    If File.Exists(m_MobileService.CacheStoragePath & "\CacheUpdate\" & m_CacheName) Then

    '        m_MobileService.Close()

    '        'Using  the ICSharpCode Libraries
    '        'Try
    '        '    Dim fz As ICSharpCode.SharpZipLib.Zip.FastZip = New ICSharpCode.SharpZipLib.Zip.FastZip()

    '        '    fz.ExtractZip(m_MobileService.CacheStoragePath & "\" & m_CacheName, m_MobileService.CacheStoragePath, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, Nothing, "", "", False)



    '        '    fz = Nothing

    '        'Catch ex As Exception
    '        '    MsgBox("Zip was corrupt, download the update again.")
    '        '    m_MobileService.Open()
    '        '    DownloadComplete(sender, e)
    '        '    Return
    '        'End Try

    '        'Using the Ionic Utils to unzip
    '        UnZipDotNetZip(m_MobileService.CacheStoragePath & "\CacheUpdate\" & m_CacheName, m_MobileService.CacheStoragePath)
    '        m_MobileService.Open()


    '        Dim pFInfo As FileInfo = New FileInfo(m_MobileService.CacheStoragePath & "\CacheUpdate\" & m_CacheName)
    '        m_cacheDate = CDate(pFInfo.CreationTime)
    '        setCacheDateConfig(m_cacheDate)
    '        pFInfo = Nothing
    '    Else
    '        MsgBox("Error finding download package, try again")
    '        DownloadComplete(sender, e)
    '        Return

    '    End If
    '    DownloadComplete(sender, e)
    '    MsgBox("The cache has been updated, restarting application")
    '    RaiseEvent CacheUpdated()
    'End Sub

#End Region


    Private Delegate Sub URLConnectionErrorDel()
    Private Sub URLConnectionError()
        TogglePostRefresh(False)



        lstErrors.Items.Insert(0, GlobalsFunctions.appConfig.ServicePanel.UIComponents.URLError)
    End Sub
    Private Sub TogglePostRefresh(ByVal enabled As Boolean)
        If enabled Then
            btnPost.BackgroundImage = My.Resources.UploadGreen
            btnRefresh.BackgroundImage = My.Resources.DownloadGreen
            btnPost.Enabled = True
            btnRefresh.Enabled = True

        Else


            btnPost.BackgroundImage = My.Resources.UploadRed
            btnRefresh.BackgroundImage = My.Resources.DownloadRed
            btnPost.Enabled = False
            btnRefresh.Enabled = False
        End If

    End Sub
    Private Sub m_ConnectionStatus_URLError(ByVal Message As String) Handles m_ConnectionStatus.URLError
        If Me.InvokeRequired Then
            Try
                Me.Invoke(New URLConnectionErrorDel(AddressOf URLConnectionError))

            Catch ex As Exception

            End Try

            'New EventHandler(AddressOf URLConnectionError ))
        Else
            URLConnectionError()
        End If

    End Sub



    Private Sub bkGrnOpenLayers_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bkGrnOpenLayers.DoWork
        Try


            For Each mapLay As Esri.ArcGIS.Mobile.MapLayer In m_Map.MapLayers
                If mapLay IsNot Nothing Then


                    If TypeOf mapLay Is Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer Then
                        Dim pTileMapLay As Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer = CType(mapLay, Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer)
                        If pTileMapLay.Visible Then
                            pTileMapLay.Open()

                        End If
                        If pTileMapLay.SpatialReference IsNot Nothing Then
                            If m_Map.SpatialReference.CoordinateSystemString <> pTileMapLay.SpatialReference.CoordinateSystemString Then
                                bkGrnOpenLayers.ReportProgress(0, String.Format(GlobalsFunctions.appConfig.LayerOptions.UIComponents.LayerSpatialReferenceDontMatch, pTileMapLay.Name))

                            Else

                            End If
                        End If

                    End If
                End If

            Next
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing
        End Try

    End Sub

    'Private Sub cancelWorker_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles cancelWorker.DoWork
    '    m_MobileSyncAgent.Cancel()

    'End Sub

    'Private Sub cancelWorker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles cancelWorker.RunWorkerCompleted
    '    RaiseEvent showIndicator(False)

    'End Sub

    Private Sub lstErrors_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstErrors.MouseDoubleClick
        lstErrors.Items.Clear()

    End Sub

    Private Sub gpBoxErrors_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles gpBoxErrors.Enter

    End Sub

    Private Sub bkGrnOpenLayers_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bkGrnOpenLayers.ProgressChanged
        'MsgBox(e.UserState)
        lstErrors.Items.Insert(0, e.UserState)

    End Sub
End Class
