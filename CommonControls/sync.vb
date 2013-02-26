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


Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGISTemplates
Imports Esri.ArcGIS.Mobile.CatalogServices
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports System.Windows.Forms
Imports System.Threading
Imports System.IO
Imports System.Threading.Thread


Public Class sync
    Public Delegate Sub SyncFeatureSourceDelegate(ByVal FeatureSyncAgent As ArrayList)
    Private WithEvents syncThread As Thread
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Public Event showIndicator(ByVal state As Boolean)

    Private WithEvents m_MobileCache As Esri.ArcGIS.Mobile.FeatureCaching.MobileCache
    Private WithEvents m_MobileConnect As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
    Private WithEvents m_MobileSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncAgent
    Private WithEvents m_MobileSyncResults As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileCacheSyncResults
    Friend Shared m_PostLayCount As Integer = 0

    Private m_HandlerState As Boolean = False
    Public Event SyncFinished(ByVal Successful As Boolean)


    Public Sub New(ByVal map As Esri.ArcGIS.Mobile.WinForms.Map)
        m_Map = map

    End Sub
    Private Class AsyncPostFeatureSourceClass
        Public FeatureSyncAgent As ArrayList
        Public StartDelegate As SyncFeatureSourceDelegate

        Public Sub syncFeatureSource()
            StartDelegate(FeatureSyncAgent)
        End Sub
    End Class
    Public Function refreshDataExtent(ByVal FullExtent As Boolean, Optional ByVal strLayName As String = "") As Boolean
        Try
            'Create a array of layers to refresh
            Dim pLay As List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource) = New List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)

            Dim pLoopLay As Esri.ArcGIS.Mobile.MapLayer = Nothing
            'Refresh the layer passed in, if not, refresh them all
            'If strLayName <> "" Then
            '    For Each strGroup As LayersToSync In m_layersToRefresh
            '        If strGroup.RefreshGroupName = strLayName Then
            '            For Each strLay In strGroup.LayersInGroup
            '                pLoopLay = m_MobileCache.Layers(strLay.Name)
            '                If pLoopLay IsNot Nothing Then
            '                    pLay.Add(pLoopLay)
            '                End If

            '            Next
            '            Exit For
            '        End If
            '    Next

            '    If pLay.Count = 0 Then Return False

            'ElseIf m_layersToRefresh IsNot Nothing Then
            '    For Each strGroup As LayersToSync In m_layersToRefresh
            '        For Each strLay In strGroup.LayersInGroup
            '            pLoopLay = m_MobileCache.Layers(strLay.Name)
            '            If pLoopLay IsNot Nothing Then
            '                pLay.Add(pLoopLay)
            '            End If

            '        Next

            '    Next
            '    If pLay.Count = 0 Then Return False


            'End If
            'Determine the extent to refresh
            Dim pExt As Esri.ArcGIS.Mobile.Geometries.Polygon
            If FullExtent Then
                pExt = m_Map.FullExtent.ToPolygon
            Else
                pExt = m_Map.Extent.ToPolygon
            End If

            postAllEditsByFeat(Synchronization.SyncDirection.DownloadOnly, pLay, pExt)


            pExt = Nothing
            pLay = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Function

    Public Sub PostData(Optional ByVal Layers As List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource) = Nothing)
        Try
            Dim FilterLayers As List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource) = Nothing
            If Layers IsNot Nothing Then
                For Each mblSL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource In Layers
                    If mblSL IsNot Nothing Then
                        If FilterLayers Is Nothing Then
                            FilterLayers = New List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource)
                        End If

                        FilterLayers.Add(mblSL)
                    End If
                Next

                If FilterLayers Is Nothing Then Return

                If FilterLayers.Count = 0 Then Return
            End If
            FilterLayers = Layers
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

    Public Function loadMobileCache() As Boolean

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

            m_MobileConnect = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
            If GlobalsFunctions.UrlIsValid(strURL.Value) Then
                m_MobileConnect.Url = strURL.Value
                m_MobileConnect.WebClientProtocolType = WebClientProtocolType.BinaryWebService
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
                            MsgBox(GlobalsFunctions.appConfig.ServicePanel.UIComponents.CacheNotValid)

                            Return False
                        End Try

                    End Try


                End If
            Else


            End If
        Next



    End Function

    Private Function postAllEditsByFeat(ByVal syncDir As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection, _
                                        Optional ByVal LayersToPost As List(Of Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource) = Nothing, _
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


                For Each pFL As FeatureSource In m_MobileCache.FeatureSources





                    If pFL.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                    Else
                        ' Dim pFeatAtt As Esri.ArcGIS.Mobile.FeatureCaching.FeatureAttachmentManager = pFL.AttachmentManager

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
                Next
            Else

                For Each pFL As FeatureSource In LayersToPost
                   
                        If pFL.HasEdits = False And syncDir = Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.SyncDirection.UploadOnly Then
                        Else

                            pFLSyncAgent = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.FeatureSyncAgent(pFL)
                        pFLSyncAgent.SynchronizationDirection = syncDir
                        pFLSyncAgent.SynchronizeAttachments = True


                            pFLSyncAgent.MapDocumentConnection = m_MobileConnect
                            If extent IsNot Nothing Then
                                pQf = New QueryFilter

                                pQf.Geometry = extent.Extent
                                pQf.GeometricRelationship = Geometries.GeometricRelationshipType.Intersect


                                pFLSyncAgent.DownloadFilter = pQf


                            End If
                            featLToSync.Add(pFLSyncAgent)


                        End If


                Next
            End If

            Dim ca As New AsyncPostFeatureSourceClass()
            ca.FeatureSyncAgent = featLToSync

            ca.StartDelegate = AddressOf SyncLayerAsync
            syncThread = New Thread(AddressOf ca.syncFeatureSource)
            m_PostLayCount = m_PostLayCount + 1
            syncThread.IsBackground = True
            syncThread.Start()


            Return pLayerCnt
        Catch ex As Exception
            writeError("Error refreshing data" & vbCrLf & ex.Message)
        Finally


        End Try

    End Function

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


        End Try

    End Sub

    Private Sub writeError(ByVal message As String)

    End Sub


End Class
