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


Imports Esri.ArcGISTemplates
Imports Esri.ArcGIS.Mobile

Imports Esri.ArcGIS.Mobile.CatalogServices
Imports Esri.ArcGIS.Mobile.FeatureCaching

Imports System.Threading
Imports System.IO

Imports System.Threading.Thread

Module AutoSyncerMain

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private WithEvents m_MobileCache As Esri.ArcGIS.Mobile.FeatureCaching.MobileCache
    Private WithEvents m_MobileConnect As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
    Private WithEvents m_MobileSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncAgent
    Private WithEvents m_MobileSyncResults As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncResults
    Private strInd As String = "    "
    Private m_StreamWriter As System.IO.StreamWriter
    Sub Main()
        CreateLogFile()
        writeError("*****************************************************************************")

        writeError(strInd & "Starting process: " & DateTime.Now())

        GlobalsFunctions.loadMapConfig()
        writeError(strInd & "Attempting to load config: " & DateTime.Now.ToLongTimeString())

        If GlobalsFunctions.appConfig IsNot Nothing Then
            writeError(strInd & "Config Loaded: " & DateTime.Now.ToLongTimeString())
            writeError(strInd & "Attempting to load mobile cache: " & DateTime.Now.ToLongTimeString())
            m_Map = New Esri.ArcGIS.Mobile.WinForms.Map

            If loadMobileCache() Then
                writeError(strInd & "Cache Loaded: " & DateTime.Now.ToLongTimeString())

                PostData(Nothing)
                writeError(strInd & "Data Posted: " & DateTime.Now.ToLongTimeString())

            Else
                writeError(strInd & "Could not load cache: " & DateTime.Now.ToLongTimeString())

            End If
        Else
            writeError(strInd & "Error: Config not found or cannot be loaded: " & DateTime.Now.ToLongTimeString())


        End If
        If m_MobileConnect IsNot Nothing Then

            m_MobileConnect.Dispose()
        End If
        m_MobileConnect = Nothing

        If (m_MobileCache IsNot Nothing) Then
            If m_MobileCache.IsOpen Then
                m_MobileCache.Close()
            End If
            m_MobileCache.Dispose()

        End If
        If (m_Map IsNot Nothing) Then
            m_Map.Dispose()

        End If
        m_Map = Nothing



        m_MobileSyncAgent = Nothing
        m_MobileSyncResults = Nothing
        writeError(strInd & "Process Ended: " & DateTime.Now())
        writeError("*****************************************************************************")
        m_StreamWriter.Flush()
        m_StreamWriter.Close()
        m_StreamWriter = Nothing

    End Sub

    Public Function loadMobileCache() As Boolean

        If GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService.Count > 1 Then
            writeError(strInd & "Two map services are not support: " & DateTime.Now.ToLongTimeString())

            Return False

        End If

        For Each strURL In GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService
            If strURL.StorageLocation = "" Then
                m_MobileCache = New MobileCache(GlobalsFunctions.generateCachePath)
            Else
                m_MobileCache = New MobileCache(strURL.StorageLocation)
            End If
            writeError(strInd & "Looking for cache at: " & m_MobileCache.StoragePath & ": " & DateTime.Now.ToLongTimeString())

            m_MobileConnect = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
            If GlobalsFunctions.UrlIsValid(strURL.Value) Then
                writeError(strInd & "URL used for posting: " & strURL.Value & ": " & DateTime.Now.ToLongTimeString())

                m_MobileConnect.Url = strURL.Value
                m_MobileConnect.WebClientProtocolType = WebClientProtocolType.BinaryWebService
            Else
                writeError(strInd & "URL is not valid: " & strURL.Value & ": " & DateTime.Now.ToLongTimeString())

            End If

            m_Map.MapLayers.Add(m_MobileCache)
            If Not m_MobileCache.IsOpen Then


                'If the service is valid
                If m_MobileCache.IsValid Then

                    Try

                        m_MobileSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncAgent(m_MobileCache, m_MobileConnect)
                        m_MobileSyncResults = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncResults()
                        m_MobileCache.Open()
                    Catch ex As Exception
                        Try
                            m_MobileConnect.CreateCache(m_MobileCache)
                            m_MobileCache.Open()
                            'setCacheDateConfig("")
                        Catch exin As Exception

                            writeError(strInd & "Cache is not valid or is open: " & vbCrLf & exin.Message)

                            Return False
                        End Try

                    End Try


                End If
            Else
                writeError(strInd & "Cache is not valid or is open: " & DateTime.Now.ToLongTimeString())


            End If
        Next


        Return True

    End Function

    Public Sub refreshDataExtent(ByVal FullExtent As Boolean, Optional ByVal strLayName As String = "")
        Try
            'Create a array of layers to refresh
            Dim pLay As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = New List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer)

            Dim pLoopLay As Esri.ArcGIS.Mobile.MapLayer = Nothing
            
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Polygon
            If FullExtent Then
                pExt = m_Map.FullExtent.ToPolygon
            Else
                pExt = m_Map.Extent.ToPolygon
            End If

            postAllEditsByFeat(Synchronization.SyncDirection.Bidirectional, pLay, pExt)


            pExt = Nothing
            pLay = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub

    Public Sub PostData(Optional ByVal Layers As List(Of Esri.ArcGIS.Mobile.MapLayer) = Nothing)
        Try
            Dim FilterLayers As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = Nothing
            If Layers IsNot Nothing Then
                For Each mblSL As Esri.ArcGIS.Mobile.MapLayer In Layers
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


            If FilterLayers Is Nothing Then

                postAllEditsByFeat(Synchronization.SyncDirection.UploadOnly)

            Else
                postAllEditsByFeat(Synchronization.SyncDirection.UploadOnly, FilterLayers)
            End If
            FilterLayers = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try

    End Sub

    Private Function postAllEditsByFeat(ByVal syncDir As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection, _
                                        Optional ByVal LayersToPost As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = Nothing, _
                                        Optional ByVal extent As Esri.ArcGIS.Mobile.Geometries.Polygon = Nothing) As Integer
        Try

            'Get the edits for each layer
            If m_MobileCache.IsOpen = False Then Return -1

            Dim pLayerCnt As Integer = 0
            Dim featLToSync As ArrayList = New ArrayList
            Dim pFLSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent
            Dim pQf As Esri.ArcGIS.Mobile.FeatureCaching.QueryFilter
            'm_MobileSyncAgent.SyncAgents.Clear()
            If LayersToPost Is Nothing Then


                For Each pL As FeatureSource In m_MobileCache.FeatureSources
                    If TypeOf pL Is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then


                        Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(pL, Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)



                        If pFL.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                        Else
                            ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager
                            writeError(strInd & pFL.Name & " has " & pFL.EditsCount & " edit(s) to be posted " & DateTime.Now.ToLongTimeString())

                            pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
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
            ElseIf LayersToPost.Count = 0 Then


                For Each pL As MapLayer In m_Map.MapLayers
                    If TypeOf pL Is Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer Then


                        Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType(CType(pL, MobileCacheMapLayer).MobileCache.FeatureSources(pL.Name), Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)



                        If pFL.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                        Else
                            ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager
                            writeError(strInd & pFL.Name & " has " & pFL.EditsCount & " edit(s) to be posted " & DateTime.Now.ToLongTimeString())

                            pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
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
                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = GlobalsFunctions.GetFeatureSource(pConfigValue.Name, m_Map).FeatureSource
                    If pFL IsNot Nothing Then


                        If pFL.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                        Else
                            writeError(strInd & pFL.Name & " has " & pFL.EditsCount & " edit(s) to be posted " & DateTime.Now.ToLongTimeString())

                            pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
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
                                'pFLSyncAgent.UploadFilter = pQf

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
                    End If

                Next
            End If

            SyncLayerAsync(featLToSync)


            Return pLayerCnt
        Catch ex As Exception
            writeError("Error refreshing data" & vbCrLf & ex.Message)
            Return -1

        Finally


        End Try

    End Function
    'Private Function postAllEditsByFeat(ByVal syncDir As esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection, _
    '                                    Optional ByVal LayersToPost As List(Of MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer) = Nothing, _
    '                                    Optional ByVal extent As Esri.ArcGIS.Mobile.Geometries.Polygon = Nothing) As Integer
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
    '        ElseIf LayersToPost.Count = 0 Then


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

    '            For Each pConfigValue As MobileConfigClass.MobileConfigMobileMapConfigServicePanelRefreshListRefreshGroupRefreshLayer In LayersToPost
    '                Dim pL as Esri.ArcGIS.Mobile.MapLayer = GlobalsFunctions.GetMapLayer(pL.Name, m_Map)
    '                If TypeOf (CType(pL, MobileCacheMapLayer).Layer) is Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource Then
    '                    Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource = CType((CType(pL, MobileCacheMapLayer).Layer), Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

    '                    If pFL.HasEdits = False And syncDir = esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
    '                    Else

    '                        pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
    '                        pFLSyncAgent.SynchronizationDirection = syncDir

    '                        pFLSyncAgent.MapDocumentConnection = m_MobileConnect
    '                        If extent IsNot Nothing Then
    '                            pQf = New QueryFilter

    '                            If pConfigValue.Extent.ToUpper = "FULL" Then
    '                                pQf.Geometry = m_Map.GetFullExtent
    '                            Else
    '                                pQf.Geometry = extent.Extent
    '                            End If
    '                            If pConfigValue.WhereClause <> "" Then
    '                                pQf.WhereClause = pConfigValue.WhereClause

    '                            End If

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

            writeError(strInd & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessStarted, DateTime.Now.ToLongTimeString()))

            Dim hadErrors As Boolean = False
            For Each syncAg As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent In FeatureSyncAgent
                Dim strStart As String

                writeError(strInd & syncAg.FeatureSource.Name.ToString())
                strStart = strInd & strInd

                If syncAg.IsValid = False Then

                    writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.NotValidText))

                Else
                    writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.StartedText, DateTime.Now.ToLongTimeString()))


                    Dim pSyncRes As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncResults = syncAg.Synchronize()

                    If pSyncRes.Exception Is Nothing Then

                        writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.FinishedText, DateTime.Now.ToLongTimeString()))

                    Else
                        writeError(strStart & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ErrorText))
                        writeError(strStart & pSyncRes.Exception.Message)
                        hadErrors = True

                    End If
                End If
            Next
            If hadErrors Then
                writeError(strInd & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessCompleteWithError, DateTime.Now.ToLongTimeString()))


            Else
                writeError(strInd & String.Format(GlobalsFunctions.appConfig.ServicePanel.UIComponents.ProcessComplete, DateTime.Now.ToLongTimeString()))

            End If

        Catch ex As Exception
            If ex.Message.ToString.Contains("Thread was being") Then

            Else
                writeError(ex.Message)

            End If
        Finally


        End Try

    End Sub

    Private Sub writeError(ByVal message As String)
        m_StreamWriter.WriteLine(message)

    End Sub

    Private Sub CreateLogFile()
        Try

            Dim portfolioPath As String = My.Application.Info.DirectoryPath
            If Not System.IO.File.Exists("syncLog.txt") Then

                File.Create("syncLog.txt").Close()


            End If
            m_StreamWriter = New StreamWriter("syncLog.txt", True)

        Catch ex As Exception
        End Try
    End Sub

End Module
