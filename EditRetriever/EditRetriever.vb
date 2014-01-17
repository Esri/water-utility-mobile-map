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


Imports Esri.ArcGISTemplates
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.Geometries

Imports Esri.ArcGIS.Mobile.CatalogServices
Imports Esri.ArcGIS.Mobile.FeatureCaching

Imports System.Threading
Imports System.IO

Imports System.Threading.Thread

Public Class EditRetriever

    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private WithEvents m_MobileCache As Esri.ArcGIS.Mobile.FeatureCaching.MobileCache
    Private WithEvents m_MobileConnect As Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
    'Private WithEvents m_MobileSyncAgent As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheSyncAgent
    'Private WithEvents m_MobileSyncResults As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheSyncResults
    Private strInd As String = "    "
    Private m_StreamWriter As System.IO.StreamWriter
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Form1_Load(sender As Object, e As System.EventArgs) Handles Me.Load



        writeError("Starting process: " & DateTime.Now())

        GlobalsFunctions.loadMapConfig()
        writeError("Attempting to load config: " & DateTime.Now.ToLongTimeString())

        If GlobalsFunctions.appConfig IsNot Nothing Then
            writeError("Config Loaded: " & DateTime.Now.ToLongTimeString())
            writeError("Attempting to load mobile cache: " & DateTime.Now.ToLongTimeString())
            m_Map = New Esri.ArcGIS.Mobile.WinForms.Map

            If loadMobileCache() Then
                writeError("Cache Loaded: " & DateTime.Now.ToLongTimeString())


                writeError("Data Posted: " & DateTime.Now.ToLongTimeString())

            Else
                writeError("Could not load cache: " & DateTime.Now.ToLongTimeString())

            End If
        Else
            writeError("Error: Config not found or cannot be loaded: " & DateTime.Now.ToLongTimeString())


        End If


        ' m_MobileSyncAgent = Nothing
        'm_MobileSyncResults = Nothing
        'writeError( "Process Ended: " & DateTime.Now())
        'writeError("*****************************************************************************")
        ' m_StreamWriter.Flush()
        'm_StreamWriter.Close()
        'm_StreamWriter = Nothing
    End Sub
    Public Function loadMobileCache() As Boolean

        If GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService.Count > 1 Then
            writeError("Two map services are not support: " & DateTime.Now.ToLongTimeString())

            Return False

        End If

        For Each strURL In GlobalsFunctions.appConfig.LayerOptions.MobileServices.MobileService
            If strURL.StorageLocation = "" Then
                m_MobileCache = New MobileCache(GlobalsFunctions.generateCachePath)
            Else
                m_MobileCache = New MobileCache(strURL.StorageLocation)
            End If
            writeError("Looking for cache at: " & m_MobileCache.StoragePath & ": " & DateTime.Now.ToLongTimeString())

            m_MobileConnect = New Esri.ArcGIS.Mobile.FeatureCaching.Synchronization.MobileServiceConnection
            m_Map.MapLayers.Add(m_MobileCache)
            If Not m_MobileCache.IsOpen Then


                'If the service is valid
                If m_MobileCache.IsValid Then

                    Try


                        m_MobileCache.Open()
                        loadLayers()
                    Catch ex As Exception
                        writeError("Cache is not valid or is open: " & DateTime.Now.ToLongTimeString())

                    End Try

                Else
                    writeError("Cache is not valid or is open: " & DateTime.Now.ToLongTimeString())

                End If
            Else
                writeError("Cache is not valid or is open: " & DateTime.Now.ToLongTimeString())


            End If
        Next


        Return True

    End Function
    Private Sub writeError(ByVal message As String)
        lstMsg.Items.Add(message)

    End Sub
    Private Sub loadLayers()
        Dim intTotalCnt As Integer = 0

        For Each pFS As FeatureSource In m_MobileCache.FeatureSources


            If pFS.HasEdits Then
                chkLstLayers.Items.Add(pFS.Name & ":" & pFS.EditsCount, True)
                intTotalCnt = intTotalCnt + pFS.EditsCount
            End If
            


        Next
        lblTotCnt.text = intTotalCnt
    End Sub
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click

        Dim pFL As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
        Dim pQFilt As QueryFilter
        Dim pDR As FeatureDataReader

        For Each chk In chkLstLayers.CheckedItems
            pFL = GlobalsFunctions.GetFeatureSource(chk.ToString().Split(":")(0), m_Map).FeatureSource
            If pFL IsNot Nothing Then
                CreateEditFile(TextBox1.Text, pFL.Name)

                Dim strEditVal As String = ""
                For Each col In pFL.Columns
                    Dim strColName As String = ""

                    If col.ColumnName = pFL.GeometryColumnName Then
                        ' Dim pCoord As Coordinate = GlobalsFunctions.GetGeometryCenter(pDR.Item(col.ColumnName))
                        strColName = "X|Y"
                    Else
                        strColName = col.ColumnName
                    End If
                    If strEditVal = "" Then
                        strEditVal = strColName

                    Else
                        strEditVal = strEditVal & "|" & strColName

                    End If

                Next
                strEditVal = strEditVal & "|EditState"

                writeEdit(strEditVal)
                For a As Integer = 0 To 2
                    Dim strType As String
                    Select Case a
                        Case 0
                            pDR = pFL.GetDataReader(Nothing, EditState.Added)
                            strType = "Added"
                        Case 1
                            pDR = pFL.GetDataReader(Nothing, EditState.Deleted)
                            strType = "Deleted"
                        Case 2
                            pDR = pFL.GetDataReader(Nothing, EditState.Modified)
                            strType = "Modified"
                    End Select
                    'Create a new datatable for hold all intersecting streets
                    Dim pDT As DataTable = Nothing
                    'Init DataTable Tools
                    'Dim pDTTools As New MobileControls.DataTableTools
                    'Loop through each street
                    strEditVal = ""

                    While pDR.Read()
                        'Geometry of primary street 
                        Dim pGeo As Geometry
                        pGeo = CType(pDR.Item(pFL.GeometryColumnName), Geometry)

                        For Each col In pFL.Columns
                            Dim strVal As String ' = pDR.Item(col.ColumnName).ToString()

                            If col.ColumnName = pFL.GeometryColumnName Then
                                Dim pCoord As Coordinate = GlobalsFunctions.GetGeometryCenter(pDR.Item(col.ColumnName))
                                strVal = pCoord.X & "|" & pCoord.Y
                            Else
                                strVal = pDR.Item(col.ColumnName).ToString()
                            End If
                            If strVal = "" Then
                                strVal = " "
                            End If
                            If strEditVal = "" Then
                                strEditVal = strVal


                            Else
                                strEditVal = strEditVal & "|" & strVal


                            End If

                        Next
                        strEditVal = strEditVal & "|" & strType
                        writeEdit(strEditVal)
                        strEditVal = ""


                    End While


                Next
                m_StreamWriter.Flush()
                m_StreamWriter.Close()
            End If
        Next

    End Sub
    Private Sub writeEdit(ByVal message As String)
        m_StreamWriter.WriteLine(message)

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text = "" Then
            Button1.Enabled = False
        Else
            If System.IO.Directory.Exists(TextBox1.Text) Then
                Button1.Enabled = True
            Else
                Button1.Enabled = False

            End If
        End If
    End Sub
    Private Sub CreateEditFile(path As String, name As String)
        Try

            Dim editPath As String = System.IO.Path.Combine(path, name & ".csv")
            If Not System.IO.File.Exists(editPath) Then

                File.Create(editPath).Close()


            End If
            m_StreamWriter = New StreamWriter(editPath, True)

        Catch ex As Exception
        End Try
    End Sub
End Class
