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
Imports Esri.ArcGIS.Mobile.FeatureCaching
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Reflection.Assembly
Imports Esri.ArcGISTemplates

Public Class MobileTOC
    Private WithEvents m_Map As Esri.ArcGIS.Mobile.WinForms.Map
    Private imgList As ImageList

    'used to send a message to remove checkboxes from a treeview
    Public Const TVIF_STATE As Integer = &H8

    Public Const TVIS_STATEIMAGEMASK As Integer = &HF000

    Public Const TV_FIRST As Integer = &H1100

    Public Const TVM_SETITEM As Integer = TV_FIRST + 63
    Public Structure TVITEM
        Public mask As Integer

        Public hItem As IntPtr

        Public state As Integer

        Public stateMask As Integer

        <MarshalAs(UnmanagedType.LPTStr)> _
        Public lpszText As String
        Public cchTextMax As Integer
        Public iImage As Integer
        Public iSelectedImage As Integer
        Public cChildren As Integer
        Public lParam As IntPtr
    End Structure
    <DllImport("User32.dll")> _
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal wMsg As Integer, _
                         ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

#Region "Public Functions"
    Public Sub New()
        Try

            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            'init the image list for the legend
            imgList = New ImageList
            Dim bmp1 As New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

            imgList.Images.Add("0", bmp1)
            bmp1 = Nothing
            tvToc.ImageList = imgList

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing


        End Try
    End Sub
    Public Sub initTOC()
        Try

            loadLayers()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Public Sub refreshTOC()
        Try

            setLayersProps()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
#End Region
#Region "Properties"
    Public WriteOnly Property Map() As Esri.ArcGIS.Mobile.WinForms.Map
        Set(ByVal value As Esri.ArcGIS.Mobile.WinForms.Map)
            m_Map = value

        End Set
    End Property
#End Region
#Region "Private Functions"
    'returns an embedded resource image by name
    'Public Function EmbeddedImage(ByVal Name As String) As System.Drawing.Bitmap
    '    Dim oStream As System.IO.Stream
    '    Dim oAssembly As System.Reflection.Assembly
    '    Dim oBitmap As Bitmap

    '    'open the executing assembly
    '    oAssembly = System.Reflection.Assembly.LoadFrom(Application.ExecutablePath)

    '    'create stream for image (icon) in assembly
    '    oStream = oAssembly.GetManifestResourceStream(Name)

    '    'create new bitmap from stream
    '    oBitmap = CType(Image.FromStream(oStream), Bitmap)

    '    Return oBitmap
    'End Function
    'Populates the treeview with feature layers available in the map
    'allowing users to select a feature layer for selection and for
    ' editing as well as setting the snap agents behavior
    Private Sub removeCheckBoxes(ByVal tvNode As TreeNode)
        Try

            'Used to remove the treeview nodes
            'code borrowed from http://www.eggheadcafe.com/community/aspnet/14/31566/hiding-checkboxes-in--net.aspx

            Dim tvi As New TVITEM()

            tvi.hItem = tvNode.Handle

            tvi.mask = TVIF_STATE

            tvi.stateMask = TVIS_STATEIMAGEMASK

            tvi.state = 0

            Dim lparam As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(tvi))

            Marshal.StructureToPtr(tvi, lparam, False)
            SendMessage(tvToc.Handle, TVM_SETITEM, CInt(IntPtr.Zero), CInt(lparam))
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub loadLayers()
        Try
            If m_Map Is Nothing Then Return

            ' Total number of MapLayers in the current map
            Dim playersCount As Integer = m_Map.MapLayers.Count


            'Current map layer
            Dim pMapLayer As Esri.ArcGIS.Mobile.MapLayer

            'map layer's name
            Dim pMapLayerName As String = ""

            'Map layer's index
            Dim pLayerIndex As Integer = 0

            'Map Layer's node
            Dim pNode As TreeNode = Nothing
            Dim pLayNode As TreeNode = Nothing

            'Snapping layer's node
            Dim pChildNode As TreeNode = Nothing

            'Treeview nodes count
            Dim pNodeCount As Integer = 0

            'Image Index
            Dim imageIndex As Integer = 1
            'Loops through current map layers collection
            For i As Integer = 0 To playersCount - 1

                'Gets the map layer for that index
                pMapLayer = m_Map.MapLayers(i)

                'Gets the map layer's name
                pMapLayerName = pMapLayer.Name


                ' nodes count
                pNodeCount = tvToc.Nodes.Count

                'Checks if the map layers already exist in the treeview
                For j As Integer = 0 To pNodeCount - 1

                    If tvToc.Nodes(j).Text = pMapLayerName Then
                        pNode = tvToc.Nodes(j)
                        Exit For

                    End If

                Next j


                'If map layer does not exist in treeview then add an entry
                If pNode Is Nothing Then

                    'Creates a map layer parent node
                    pNode = New TreeNode(pMapLayerName, 0, 0)

                    'Inserts the layer in the correct position in the treeview
                    'tvToc.Nodes.Insert(pLayerIndex, pNode)
                    tvToc.Nodes.Add(pNode)

                End If

                If TypeOf pMapLayer Is MobileCacheMapLayer Then

                    'Current map layer
                    Dim pMobileCacheLayer As MobileCacheMapLayer
                    Dim pFLayDef As MobileCacheMapLayerDefinition

                    pMobileCacheLayer = TryCast(pMapLayer, MobileCacheMapLayer)
                    Dim pFS As FeatureSource
                    For l As Integer = 0 To pMobileCacheLayer.MobileCache.FeatureSources.Count - 1

                        pFS = pMobileCacheLayer.MobileCache.FeatureSources(l)
                        pFLayDef = pMobileCacheLayer.LayerDefinitions(l)

                        'Creates a map layer parent node
                        pLayNode = New TreeNode(pFLayDef.Name, 0, 0)

                        'Inserts the layer in the correct position in the treeview
                        pNode.Nodes.Insert(l, pLayNode)


                        If (pFLayDef.Visibility) Then
                            pLayNode.Checked = True
                        Else
                            pLayNode.Checked = False


                        End If
                        If pFLayDef.MinScale = 0 And pFLayDef.MaxScale = 0 Then
                            pLayNode.ForeColor = System.Drawing.Color.Black
                        ElseIf pFLayDef.MinScale = 0 And m_Map.Scale > pFLayDef.MaxScale Then
                            pLayNode.ForeColor = System.Drawing.Color.Black
                        ElseIf m_Map.Scale < pFLayDef.MinScale And pFLayDef.MaxScale = 0 Then
                            pLayNode.ForeColor = System.Drawing.Color.Black
                        ElseIf m_Map.Scale < pFLayDef.MinScale And m_Map.Scale > pFLayDef.MaxScale Then
                            pLayNode.ForeColor = System.Drawing.Color.Black
                        Else
                            pLayNode.ForeColor = System.Drawing.Color.DarkGray
                        End If

                        ' Contains the map layer legend swatches and labels 
                        Dim legendSwatchesList As IList(Of LegendItem)

                        legendSwatchesList = pFS.GetLegendItems(tvToc.BackColor, New Drawing.Size(tvToc.ItemHeight, tvToc.ItemHeight), True)
                        If legendSwatchesList IsNot Nothing Then


                            For k As Integer = legendSwatchesList.Count - 1 To 0 Step -1
                                ' Adds the swatch image to the imagelist 
                                imgList.Images.Add(legendSwatchesList(k).Swatch)
                                Dim pLbl As String = legendSwatchesList(k).Symbol.Label
                                If Trim(pLbl) = "" Then
                                    pLbl = "Default"
                                Else
                                    pLbl = Trim(pLbl)
                                End If
                                ' Creates a new child node using the legend's swatch label 
                                Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                                ' Adds the layer node to the treeview 
                                pLayNode.Nodes.Add(childNode)
                                removeCheckBoxes(childNode)
                                imageIndex += 1
                            Next k
                        End If

                    Next


                ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer Then

                    ''Current map layer
                    ''Dim pTileLayer As ESRI.ArcGIS.Mobile.DataProducts.RasterData.TileMapLayer
                    '' Adds the swatch image to the imagelist 
                    'Dim bmp1 As System.Drawing.Bitmap = My.Resources.cache 'EmbeddedImage("MobileTOCControls.cache.png") 'New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    'bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

                    'imgList.Images.Add(bmp1)

                    'Dim pLbl As String = "Basemap"

                    '' Creates a new child node using the legend's swatch label 
                    'Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                    '' Adds the layer node to the treeview 
                    'pNode.Nodes.Add(childNode)
                    'removeCheckBoxes(childNode)

                    'imageIndex += 1
                ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.DataProducts.StreetMapData.StreetMapLayer Then
                    ' Adds the swatch image to the imagelist 
                    Dim bmp1 As System.Drawing.Bitmap = My.Resources.cache 'My.Resources.cacheEmbeddedImage("cache.png") 'New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

                    imgList.Images.Add(bmp1)

                    Dim pLbl As String = "Basemap"

                    ' Creates a new child node using the legend's swatch label 
                    Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                    ' Adds the layer node to the treeview 
                    pNode.Nodes.Add(childNode)
                    removeCheckBoxes(childNode)

                    imageIndex += 1
                ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer Then
                    ' Adds the swatch image to the imagelist 
                    'Dim bmp1 As System.Drawing.Bitmap = My.Resources.cache 'EmbeddedImage("cache.png") 'New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    'bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

                    'imgList.Images.Add(bmp1)

                    'Dim pLbl As String = "Basemap"

                    '' Creates a new child node using the legend's swatch label 
                    'Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                    '' Adds the layer node to the treeview 
                    'pNode.Nodes.Add(childNode)
                    'removeCheckBoxes(childNode)

                    'imageIndex += 1

                Else



                End If


                pNode = Nothing





            Next i
            pChildNode = Nothing
            pMapLayer = Nothing
            '   pMblService = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub setLayersProps()
        Try
            If m_Map Is Nothing Then Return



            For Each pParNode As TreeNode In tvToc.Nodes


                Dim pMapLay As MapLayer = GlobalsFunctions.GetMapLayer(pParNode.Text, m_Map)

                If pMapLay IsNot Nothing Then

                    ' If node is not null and is in range then turn it black

                    If pMapLay.MinScale = 0 And pMapLay.MaxScale = 0 Then
                        pParNode.ForeColor = System.Drawing.Color.Black
                    ElseIf pMapLay.MinScale = 0 And m_Map.Scale > pMapLay.MaxScale Then
                        pParNode.ForeColor = System.Drawing.Color.Black
                    ElseIf m_Map.Scale < pMapLay.MinScale And pMapLay.MaxScale = 0 Then
                        pParNode.ForeColor = System.Drawing.Color.Black
                    ElseIf m_Map.Scale < pMapLay.MinScale And m_Map.Scale > pMapLay.MaxScale Then
                        pParNode.ForeColor = System.Drawing.Color.Black
                    Else
                        pParNode.ForeColor = System.Drawing.Color.DarkGray
                    End If
                    If (pMapLay.Visible) Then

                        pParNode.Checked = True
                    Else
                        pParNode.Checked = False
                    End If
                    Dim pMobileCacheLayer As MobileCacheMapLayer
                    Dim pFLayDef As MobileCacheMapLayerDefinition

                    Dim pFS As FeatureSource


                    For Each pCNode As TreeNode In pParNode.Nodes
                        'Checks if the map layers already exist in the treeview


                        If TypeOf pMapLay Is MobileCacheMapLayer Then

                            pMobileCacheLayer = TryCast(pMapLay, MobileCacheMapLayer)
                            pFS = pMobileCacheLayer.MobileCache.FeatureSources(pCNode.Text)
                            If pFS IsNot Nothing Then


                                pFLayDef = pMobileCacheLayer.LayerDefinitions(pMobileCacheLayer.MobileCache.FeatureSources.IndexOf(pFS))
                                If pFLayDef.MinScale = 0 And pFLayDef.MaxScale = 0 Then
                                    pCNode.ForeColor = System.Drawing.Color.Black
                                ElseIf pFLayDef.MinScale = 0 And m_Map.Scale > pFLayDef.MaxScale Then
                                    pCNode.ForeColor = System.Drawing.Color.Black
                                ElseIf m_Map.Scale < pFLayDef.MinScale And pFLayDef.MaxScale = 0 Then
                                    pCNode.ForeColor = System.Drawing.Color.Black
                                ElseIf m_Map.Scale < pFLayDef.MinScale And m_Map.Scale > pFLayDef.MaxScale Then
                                    pCNode.ForeColor = System.Drawing.Color.Black
                                Else
                                    pCNode.ForeColor = System.Drawing.Color.DarkGray
                                End If
                                If (pFLayDef.Visibility) Then

                                    pCNode.Checked = True
                                Else
                                    pCNode.Checked = False
                                End If
                            End If








                        Else


                        End If
                    Next
                End If

            Next


        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub loadSetLayers_old()
        Try
            If m_Map Is Nothing Then Return

            ' Total number of MapLayers in the current map
            Dim playersCount As Integer = m_Map.MapLayers.Count


            'Current map layer
            Dim pMapLayer As Esri.ArcGIS.Mobile.MapLayer

            'map layer's name
            Dim pMapLayerName As String = ""

            'Map layer's index
            Dim pLayerIndex As Integer = 0

            'Map Layer's node
            Dim pNode As TreeNode = Nothing
            Dim pLayNode As TreeNode = Nothing

            'Snapping layer's node
            Dim pChildNode As TreeNode = Nothing

            'Treeview nodes count
            Dim pNodeCount As Integer = 0

            'Image Index
            Dim imageIndex As Integer = 1
            'Loops through current map layers collection
            For i As Integer = 0 To playersCount - 1

                'Gets the map layer for that index
                pMapLayer = m_Map.MapLayers(i)
                'If TypeOf (pMapLayer) Is MobileCacheMapLayer Then
                '    CType(pMapLayer, MobileCacheMapLayer).MobileCache.pa()

                'End If
                'pMobileCacheLayer = CType(m_Map.MapLayers(i), MobileCacheMapLayer)

                'Checks if layer is not visible, or not within scale range
                'if (!mapLayer.Visible || !mapLayer.InScaleRange(map1.Scale))
                '      continue

                'Gets the map layer's name
                pMapLayerName = pMapLayer.Name


                ' nodes count
                pNodeCount = tvToc.Nodes.Count

                'Checks if the map layers already exist in the treeview
                For j As Integer = 0 To pNodeCount - 1

                    If tvToc.Nodes(j).Text = pMapLayerName Then
                        pNode = tvToc.Nodes(j)
                        Exit For

                    End If

                Next j


                'If map layer does not exist in treeview then add an entry
                If pNode Is Nothing Then

                    'Creates a map layer parent node
                    pNode = New TreeNode(pMapLayerName, 0, 0)

                    'Inserts the layer in the correct position in the treeview
                    'tvToc.Nodes.Insert(pLayerIndex, pNode)
                    tvToc.Nodes.Add(pNode)

                    If (pMapLayer.Visible) Then
                        pNode.Checked = True
                    Else
                        pNode.Checked = False


                    End If
                    If pMapLayer.MinScale = 0 And pMapLayer.MaxScale = 0 Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    ElseIf pMapLayer.MinScale = 0 And m_Map.Scale > pMapLayer.MaxScale Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    ElseIf m_Map.Scale < pMapLayer.MinScale And pMapLayer.MaxScale = 0 Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    ElseIf m_Map.Scale < pMapLayer.MinScale And m_Map.Scale > pMapLayer.MaxScale Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    Else
                        pNode.ForeColor = System.Drawing.Color.DarkGray
                    End If
                    'If Not pMapLayer.InScaleRange(m_Map.Scale) Then
                    '    pNode.ForeColor = System.Drawing.Color.DarkGray
                    'Else
                    '    pNode.ForeColor = System.Drawing.Color.Black
                    'End If
                    If TypeOf pMapLayer Is MobileCacheMapLayer Then

                        'Current map layer
                        Dim pMobileCacheLayer As MobileCacheMapLayer
                        Dim pFLayDef As MobileCacheMapLayerDefinition

                        pMobileCacheLayer = TryCast(pMapLayer, MobileCacheMapLayer)
                        Dim pFS As FeatureSource
                        For l As Integer = 0 To pMobileCacheLayer.MobileCache.FeatureSources.Count - 1

                            pFS = pMobileCacheLayer.MobileCache.FeatureSources(l)
                            pFLayDef = pMobileCacheLayer.LayerDefinitions(l)

                            'Creates a map layer parent node
                            pLayNode = New TreeNode(pFLayDef.Name, 0, 0)

                            'Inserts the layer in the correct position in the treeview
                            pNode.Nodes.Insert(l, pLayNode)


                            If (pFLayDef.Visibility) Then
                                pLayNode.Checked = True
                            Else
                                pLayNode.Checked = False


                            End If
                            If pFLayDef.MinScale = 0 And pFLayDef.MaxScale = 0 Then
                                pLayNode.ForeColor = System.Drawing.Color.Black
                            ElseIf pFLayDef.MinScale = 0 And m_Map.Scale > pFLayDef.MaxScale Then
                                pLayNode.ForeColor = System.Drawing.Color.Black
                            ElseIf m_Map.Scale < pFLayDef.MinScale And pFLayDef.MaxScale = 0 Then
                                pLayNode.ForeColor = System.Drawing.Color.Black
                            ElseIf m_Map.Scale < pFLayDef.MinScale And m_Map.Scale > pFLayDef.MaxScale Then
                                pLayNode.ForeColor = System.Drawing.Color.Black
                            Else
                                pLayNode.ForeColor = System.Drawing.Color.DarkGray
                            End If

                            ' Contains the map layer legend swatches and labels 
                            Dim legendSwatchesList As IList(Of LegendItem)

                            legendSwatchesList = pFS.GetLegendItems(tvToc.BackColor, New Drawing.Size(tvToc.ItemHeight, tvToc.ItemHeight), True)
                            If legendSwatchesList IsNot Nothing Then


                                For k As Integer = legendSwatchesList.Count - 1 To 0 Step -1
                                    ' Adds the swatch image to the imagelist 
                                    imgList.Images.Add(legendSwatchesList(k).Swatch)
                                    Dim pLbl As String = legendSwatchesList(k).Symbol.Label
                                    If Trim(pLbl) = "" Then
                                        pLbl = "Default"
                                    Else
                                        pLbl = Trim(pLbl)
                                    End If
                                    ' Creates a new child node using the legend's swatch label 
                                    Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                                    ' Adds the layer node to the treeview 
                                    pLayNode.Nodes.Add(childNode)
                                    removeCheckBoxes(childNode)
                                    imageIndex += 1
                                Next k
                            End If

                        Next


                    ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.DataProducts.RasterData.TileCacheMapLayer Then

                        'Current map layer
                        'Dim pTileLayer As ESRI.ArcGIS.Mobile.DataProducts.RasterData.TileMapLayer
                        ' Adds the swatch image to the imagelist 
                        Dim bmp1 As System.Drawing.Bitmap = My.Resources.cache 'EmbeddedImage("MobileTOCControls.cache.png") 'New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                        bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

                        imgList.Images.Add(bmp1)

                        Dim pLbl As String = "Basemap"

                        ' Creates a new child node using the legend's swatch label 
                        Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                        ' Adds the layer node to the treeview 
                        pNode.Nodes.Add(childNode)
                        removeCheckBoxes(childNode)

                        imageIndex += 1
                    ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.DataProducts.StreetMapData.StreetMapLayer Then
                        ' Adds the swatch image to the imagelist 
                        Dim bmp1 As System.Drawing.Bitmap = My.Resources.cache 'My.Resources.cacheEmbeddedImage("cache.png") 'New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                        bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

                        imgList.Images.Add(bmp1)

                        Dim pLbl As String = "Basemap"

                        ' Creates a new child node using the legend's swatch label 
                        Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                        ' Adds the layer node to the treeview 
                        pNode.Nodes.Add(childNode)
                        removeCheckBoxes(childNode)

                        imageIndex += 1
                    ElseIf TypeOf pMapLayer Is Esri.ArcGIS.Mobile.WebServices.ArcGISServices.MapServices.TileServiceMapLayer Then
                        ' Adds the swatch image to the imagelist 
                        Dim bmp1 As System.Drawing.Bitmap = My.Resources.cache 'EmbeddedImage("cache.png") 'New System.Drawing.Bitmap(tvToc.ItemHeight, tvToc.ItemHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                        bmp1.MakeTransparent(bmp1.GetPixel(1, 1))

                        imgList.Images.Add(bmp1)

                        Dim pLbl As String = "Basemap"

                        ' Creates a new child node using the legend's swatch label 
                        Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                        ' Adds the layer node to the treeview 
                        pNode.Nodes.Add(childNode)
                        removeCheckBoxes(childNode)

                        imageIndex += 1

                    Else



                    End If
                Else
                    ' If node is not null and is in range then turn it black

                    If pMapLayer.MinScale = 0 And pMapLayer.MaxScale = 0 Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    ElseIf pMapLayer.MinScale = 0 And m_Map.Scale > pMapLayer.MaxScale Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    ElseIf m_Map.Scale < pMapLayer.MinScale And pMapLayer.MaxScale = 0 Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    ElseIf m_Map.Scale < pMapLayer.MinScale And m_Map.Scale > pMapLayer.MaxScale Then
                        pNode.ForeColor = System.Drawing.Color.Black
                    Else
                        pNode.ForeColor = System.Drawing.Color.DarkGray
                    End If
                    If (pMapLayer.Visible) Then

                        pNode.Checked = True
                    Else
                        pNode.Checked = False
                    End If

                    If TypeOf pMapLayer Is MobileCacheMapLayer Then

                        'Current map layer
                        Dim pMobileCacheLayer As MobileCacheMapLayer
                        Dim pFLayDef As MobileCacheMapLayerDefinition

                        pMobileCacheLayer = TryCast(pMapLayer, MobileCacheMapLayer)
                        Dim pFS As FeatureSource
                        'Checks if the map layers already exist in the treeview


                        For l As Integer = 0 To pMobileCacheLayer.MobileCache.FeatureSources.Count - 1
                            pFS = pMobileCacheLayer.MobileCache.FeatureSources(l)
                            pFLayDef = pMobileCacheLayer.LayerDefinitions(l)

                            If pNode.Nodes.Find(pFS.Name, True).Length = 0 Then

                                'Creates a map layer parent node
                                pLayNode = New TreeNode(pFLayDef.Name, 0, 0)

                                'Inserts the layer in the correct position in the treeview
                                pNode.Nodes.Insert(l, pLayNode)


                                If (pFLayDef.Visibility) Then
                                    pLayNode.Checked = True
                                Else
                                    pLayNode.Checked = False


                                End If
                                If pFLayDef.MinScale = 0 And pFLayDef.MaxScale = 0 Then
                                    pLayNode.ForeColor = System.Drawing.Color.Black
                                ElseIf pFLayDef.MinScale = 0 And m_Map.Scale > pFLayDef.MaxScale Then
                                    pLayNode.ForeColor = System.Drawing.Color.Black
                                ElseIf m_Map.Scale < pFLayDef.MinScale And pFLayDef.MaxScale = 0 Then
                                    pLayNode.ForeColor = System.Drawing.Color.Black
                                ElseIf m_Map.Scale < pFLayDef.MinScale And m_Map.Scale > pFLayDef.MaxScale Then
                                    pLayNode.ForeColor = System.Drawing.Color.Black
                                Else
                                    pLayNode.ForeColor = System.Drawing.Color.DarkGray
                                End If

                                ' Contains the map layer legend swatches and labels 
                                Dim legendSwatchesList As IList(Of LegendItem)

                                legendSwatchesList = pFS.GetLegendItems(tvToc.BackColor, New Drawing.Size(tvToc.ItemHeight, tvToc.ItemHeight), True)
                                If legendSwatchesList IsNot Nothing Then


                                    For k As Integer = legendSwatchesList.Count - 1 To 0 Step -1
                                        ' Adds the swatch image to the imagelist 
                                        imgList.Images.Add(legendSwatchesList(k).Swatch)
                                        Dim pLbl As String = legendSwatchesList(k).Symbol.Label
                                        If Trim(pLbl) = "" Then
                                            pLbl = "Default"
                                        Else
                                            pLbl = Trim(pLbl)
                                        End If
                                        ' Creates a new child node using the legend's swatch label 
                                        Dim childNode As New TreeNode(pLbl, imageIndex, imageIndex)


                                        ' Adds the layer node to the treeview 
                                        pLayNode.Nodes.Add(childNode)
                                        removeCheckBoxes(childNode)
                                        imageIndex += 1
                                    Next k
                                End If


                            Else


                            End If

                        Next

                        For p As Integer = 0 To pNode.Nodes.Count - 1

                            pFS = pMobileCacheLayer.MobileCache.FeatureSources(pNode.Nodes(p).Text)
                            If pFS IsNot Nothing Then


                                pFLayDef = pMobileCacheLayer.LayerDefinitions(pMobileCacheLayer.MobileCache.FeatureSources.IndexOf(pFS))
                                If pFLayDef.MinScale = 0 And pFLayDef.MaxScale = 0 Then
                                    pNode.Nodes(p).ForeColor = System.Drawing.Color.Black
                                ElseIf pFLayDef.MinScale = 0 And m_Map.Scale > pFLayDef.MaxScale Then
                                    pNode.Nodes(p).ForeColor = System.Drawing.Color.Black
                                ElseIf m_Map.Scale < pFLayDef.MinScale And pFLayDef.MaxScale = 0 Then
                                    pNode.Nodes(p).ForeColor = System.Drawing.Color.Black
                                ElseIf m_Map.Scale < pFLayDef.MinScale And m_Map.Scale > pFLayDef.MaxScale Then
                                    pNode.Nodes(p).ForeColor = System.Drawing.Color.Black
                                Else
                                    pNode.Nodes(p).ForeColor = System.Drawing.Color.DarkGray
                                End If
                                If (pFLayDef.Visibility) Then

                                    pNode.Nodes(p).Checked = True
                                Else
                                    pNode.Nodes(p).Checked = False
                                End If
                            End If

                        Next p



                    End If
                    pLayerIndex = pLayerIndex + 1


                End If

                pNode = Nothing





            Next i
            pChildNode = Nothing
            pMapLayer = Nothing
            '   pMblService = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
#End Region
#Region "Events"
    Private Sub m_Map_ExtentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Map.ExtentChanged
        Try


            'Gray out or activate layers that are visible
            setLayersProps()
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub tvToc_AfterCheck(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvToc.AfterCheck
        Try


            'Turn on or off a layer that was checked
            Dim pFSwD As FeatureSourceWithDef
            Dim pMapLayer As MapLayer
            Dim pNode As TreeNode = e.Node
            If e.Node.Level = 0 Then
                pMapLayer = GlobalsFunctions.GetMapLayer(pNode.Text, m_Map)
                If pMapLayer Is Nothing Then Return
                'Change the layers visible state


                pMapLayer.Visible = pNode.Checked
            Else
                'Get the layer clicked
                pFSwD = GlobalsFunctions.GetFeatureSource(pNode.Text, m_Map)
                'Verify the layer was found
                If pFSwD.FeatureSource Is Nothing Then Return
                'Change the layers visible state


                CType(m_Map.MapLayers(pFSwD.MapLayerIndex), MobileCacheMapLayer).LayerDefinitions(pFSwD.LayerIndex).Visibility = pNode.Checked
                'Cleanup
            End If

            pFSwD = Nothing
            pNode = Nothing
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub tvToc_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tvToc.ParentChanged
        Try


            'Change the color of the treeview to match the parent
            tvToc.BackColor = CType(sender, Control).Parent.BackColor
        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
    Private Sub MobileTOC_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Try


            If m_Map IsNot Nothing Then
                m_Map.Dispose()
            End If
            m_Map = Nothing
            If imgList IsNot Nothing Then
                imgList.Dispose()

            End If
            imgList = Nothing

        Catch ex As Exception
            Dim st As New StackTrace
            MsgBox(st.GetFrame(0).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Name & ":" & st.GetFrame(1).GetMethod.Module.Name & vbCrLf & ex.Message)
            st = Nothing

        End Try
    End Sub
#End Region


End Class
