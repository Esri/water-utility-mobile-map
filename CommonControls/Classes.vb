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


Imports System
Imports System.Windows.Forms
Imports System.IO
Imports System.Xml
Imports System.Collections.Specialized
Imports System.Collections
Imports System.Text
Imports System.Xml.Serialization
Imports Esri.ArcGIS.Mobile
Imports Esri.ArcGIS.Mobile.Geometries
Imports Esri.ArcGIS.Mobile.SpatialReferences
Public Class attFiles
    Private m_filePath As String
    Private m_fileName As String

    Public Property fileName() As String
        Get
            Return m_fileName

        End Get

        Set(ByVal value As String)
            m_fileName = value

        End Set

    End Property
    Public Property filePath() As String
        Get
            Return m_filePath

        End Get

        Set(ByVal value As String)
            m_filePath = value

        End Set

    End Property
End Class

Public Class FeatureSourceWithDef
    Private m_FeatureSource As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
    Private m_MobileCacheMapLayer As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer
    Private m_LayerIndex As Integer
    Private m_MapLayerIndex As Integer
    Public Sub New(featureSource As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource, _
                   mobileCacheMapLayer As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer, _
                   layerIndex As Integer, _
                   mapLayerIndex As Integer)

        m_FeatureSource = featureSource
        m_MobileCacheMapLayer = mobileCacheMapLayer
        m_LayerIndex = layerIndex
        m_MapLayerIndex = mapLayerIndex
    End Sub
    Public Property FeatureSource As Esri.ArcGIS.Mobile.FeatureCaching.FeatureSource
        Get
            Return m_FeatureSource
        End Get
        Set(value As ESRI.ArcGIS.Mobile.FeatureCaching.FeatureSource)
            m_FeatureSource = value
        End Set
    End Property
    Public Property MobileCacheMapLayer As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer
        Get
            Return m_MobileCacheMapLayer
        End Get
        Set(value As Esri.ArcGIS.Mobile.FeatureCaching.MobileCacheMapLayer)
            m_MobileCacheMapLayer = value
        End Set
    End Property

    Public Property LayerIndex As Integer
        Get
            Return m_LayerIndex
        End Get
        Set(value As Integer)
            m_LayerIndex = value
        End Set
    End Property

    Public Property MapLayerIndex As Integer
        Get
            Return m_MapLayerIndex
        End Get
        Set(value As Integer)
            m_MapLayerIndex = value
        End Set
    End Property
End Class
Public Class spatialReference
    Dim wkidField As String
    Public Property wkid As String
        Get
            Return wkidField
        End Get
        Set(value As String)
            wkidField = value
        End Set
    End Property

End Class
Public Class location
    Private xField As String
    Public Property x As String
        Get
            Return xField
        End Get
        Set(value As String)
            xField = value
        End Set
    End Property

    Private yField As String
    Public Property y As String
        Get
            Return yField
        End Get
        Set(value As String)
            yField = value
        End Set
    End Property

End Class
Public Class candidates
    Private addressField As String
    Public Property address As String
        Get
            Return addressField
        End Get
        Set(value As String)
            addressField = value
        End Set
    End Property
    Private locationField As location
    Public Property location As location
        Get
            Return locationField
        End Get
        Set(value As location)
            locationField = value
        End Set
    End Property
    Private scoreField As String
    Public Property score As String
        Get
            Return scoreField
        End Get
        Set(value As String)
            scoreField = value
        End Set
    End Property

End Class
Public Class gcResponse

    Private spatialReferenceField As spatialReference
    Private candidatesField As candidates()
    Public Sub New()

    End Sub
    Public Property spatialReference As spatialReference
        Get
            Return spatialReferenceField
        End Get
        Set(value As spatialReference)
            spatialReferenceField = value
        End Set
    End Property
    Public Property candidates As candidates()
        Get
            Return candidatesField
        End Get
        Set(value As candidates())
            candidatesField = value
        End Set
    End Property
    '  "wkid" : 4326
    '},
    '"candidates" : [
    '  {
    '    "address" : "110 Fox Run Dr, Collegeville, PA, 19426",
    '    "location" : {
    '      "x" : -75.498251353816784,
    '      "y" : 40.179868101132115
    '    },
    '    "score" : 100,
    '    "attributes" : {

    '    }
    '  },
    '  {
    '    "address" : "111 Fox Run Dr, Collegeville, PA, 19426",
    '    "location" : {
    '      "x" : -75.498306582082932,
    '      "y" : 40.179742724847017
    '    },
    '    "score" : 79,
    '    "attributes" : {

    '    }
    '  }
    ']
End Class

Public Class cValue
    Private sDisplay As String
    Private sValue As Object
    Public Property Display() As String
        Get
            Return Me.sDisplay
        End Get
        Set(ByVal value As String)
            Me.sDisplay = value
        End Set
    End Property
    Public Property Value() As Object
        Get
            Return Me.sValue
        End Get
        Set(ByVal value As Object)
            Me.sValue = value
        End Set
    End Property

    Public Sub New(ByVal sDisplay As String, ByVal sValue As Object)
        Me.Display = sDisplay
        Me.Value = sValue
    End Sub

    Public Overrides Function ToString() As String
        Return Display
    End Function
End Class
Public Class GPSLocationDetails

    Private m_Altitude As Double
    Public Property Altitude As Double
        Get
            Return m_Altitude
        End Get
        Set(ByVal value As Double)
            m_Altitude = value
        End Set
    End Property
    Private m_Coord As Double
    Public ReadOnly Property Coordinate As Coordinate

        Get
            Return New Coordinate(m_Longitude, m_Latitude)
        End Get

    End Property

    Private m_Course As Double
    Public Property Course As Double
        Get
            Return m_Course
        End Get
        Set(ByVal value As Double)
            m_Course = value
        End Set
    End Property

    Private m_CourseMagnetic As Double
    Public Property CourseMagnetic As Double
        Get
            Return m_CourseMagnetic
        End Get
        Set(ByVal value As Double)
            m_CourseMagnetic = value
        End Set
    End Property

    Private m_DateTime As Date
    Public Property DateTime As Date
        Get
            Return m_DateTime
        End Get
        Set(ByVal value As Date)
            m_DateTime = value
        End Set
    End Property

    Private m_FixSatelliteCount As Integer
    Public Property FixSatelliteCount As Integer
        Get
            Return m_FixSatelliteCount
        End Get
        Set(ByVal value As Integer)
            m_FixSatelliteCount = value
        End Set
    End Property


    Private m_FixStatus As String
    Public Property FixStatus As String
        Get
            Return m_FixStatus
        End Get
        Set(ByVal value As String)
            m_FixStatus = value
            'Select Case value
            '    Case "0"
            '        m_FixStatus = "No Fix"

            '    Case "1"
            '        m_FixStatus = "Gps Fix"
            '    Case "2"
            '        m_FixStatus = "DGps Fix"
            '    Case "3"
            '        m_FixStatus = "PPS fix"
            '    Case "4"
            '        m_FixStatus = "Real Time Kinematic"
            '    Case "5"
            '        m_FixStatus = "Float RTK"
            '    Case "6"
            '        m_FixStatus = "estimated (dead reckoning)"
            '    Case "7"
            '        m_FixStatus = "Manual input mode"
            '    Case "8"
            '        m_FixStatus = "Simulation mode"
            '    Case Else

            '        m_FixStatus = value
            'End Select

        End Set
    End Property

    Private m_GeoidHeight As Double
    Public Property GeoidHeight As Double
        Get
            Return m_GeoidHeight
        End Get
        Set(ByVal value As Double)
            m_GeoidHeight = value
        End Set
    End Property

    Private m_HorizontalDilutionOfPrecision As Double
    Public Property HorizontalDilutionOfPrecision As Double
        Get
            Return m_HorizontalDilutionOfPrecision
        End Get
        Set(ByVal value As Double)
            m_HorizontalDilutionOfPrecision = value
        End Set
    End Property

    Private m_Latitude As Double
    Public Property Latitude As Double
        Get
            Return m_Latitude
        End Get
        Set(ByVal value As Double)
            m_Latitude = value
        End Set
    End Property


    'Private m_LatitudeToDegreeMinutesSeconds As String
    'Public Property LatitudeToDegreeMinutesSeconds As String
    '    Get
    '        Return m_LatitudeToDegreeMinutesSeconds
    '    End Get
    '    Set(ByVal value As String)
    '        m_LatitudeToDegreeMinutesSeconds = value
    '    End Set
    'End Property
    'Private m_LongitudeToDegreeMinutesSeconds As String
    'Public Property LongitudeToDegreeMinutesSeconds As String
    '    Get
    '        Return m_LongitudeToDegreeMinutesSeconds
    '    End Get
    '    Set(ByVal value As String)
    '        m_LongitudeToDegreeMinutesSeconds = value
    '    End Set
    'End Property

    Private m_Longitude As Double
    Public Property Longitude As Double
        Get
            Return m_Longitude
        End Get
        Set(ByVal value As Double)
            m_Longitude = value
        End Set
    End Property

  
    Private m_PositionDilutionOfPrecision As Double
    Public Property PositionDilutionOfPrecision As Double
        Get
            Return m_PositionDilutionOfPrecision
        End Get
        Set(ByVal value As Double)
            m_PositionDilutionOfPrecision = value
        End Set
    End Property

    Private m_SpatialReference As Esri.ArcGIS.Mobile.SpatialReferences.SpatialReference
    Public Property SpatialReference As Esri.ArcGIS.Mobile.SpatialReferences.SpatialReference
        Get
            Return m_SpatialReference
        End Get
        Set(ByVal value As Esri.ArcGIS.Mobile.SpatialReferences.SpatialReference)
            m_SpatialReference = value
        End Set
    End Property


    Private m_Speed As Double
    Public Property Speed As Double
        Get
            Return m_Speed
        End Get
        Set(ByVal value As Double)
            m_Speed = value
        End Set
    End Property


    Private m_VerticalDilutionOfPrecision As Double
    Public Property VerticalDilutionOfPrecision As Double
        Get
            Return m_VerticalDilutionOfPrecision
        End Get
        Set(ByVal value As Double)
            m_VerticalDilutionOfPrecision = value
        End Set
    End Property




End Class
Public Class SpatialIntersectDef

    Private m_LayerEdited As String
    Public Property LayerEdited As String
        Get
            Return m_LayerEdited
        End Get
        Set(ByVal value As String)
            m_LayerEdited = value
        End Set
    End Property
    Private m_LayerToIntersect As String

    Public Property LayerToIntersect As String
        Get
            Return m_LayerToIntersect
        End Get
        Set(ByVal value As String)
            m_LayerToIntersect = value
        End Set
    End Property
    Private m_FieldPairs As List(Of FieldPairs)
    Public Property FieldPairs As List(Of FieldPairs)
        Get
            Return m_FieldPairs
        End Get
        Set(ByVal value As List(Of FieldPairs))
            m_FieldPairs = value
        End Set
    End Property
End Class
<XmlRootAttribute(ElementName:="BookmarkEntries", IsNullable:=True)> _
Public Class Bookmarks


    'Private m_Bookmarks() As BookmarkDetails
    Private m_Bookmarks As List(Of BookmarkDetails)

    Public Sub New()
    End Sub

    <XmlArray("Bookmarks"), XmlArrayItem(ElementName:="Bookmark", Type:=GetType(BookmarkDetails))> _
    Public Property Bookmark As List(Of BookmarkDetails)
        Get
            Return m_Bookmarks
        End Get
        Set(ByVal value As List(Of BookmarkDetails))
            m_Bookmarks = value
        End Set
    End Property

    '<XmlArray("Bookmarks"), XmlArrayItem(ElementName:="Bookmark", Type:=GetType(BookmarkDetails))> _
    'Public Property Bookmark() As BookmarkDetails()
    '    Get
    '        Return m_Bookmarks
    '    End Get
    '    Set(ByVal value As BookmarkDetails())
    '        m_Bookmarks = value
    '    End Set
    'End Property


End Class
<XmlRootAttribute(ElementName:="Bookmark", IsNullable:=True)> _
Public Class BookmarkDetails


    Private m_Name As String
    Private m_XMax As String
    Private m_XMin As String
    Private m_YMax As String
    Private m_YMin As String


    Public Sub New()
    End Sub
    Public Sub New(ByVal Name As String, ByVal env As Geometries.Envelope)
        m_Name = Name
        m_XMax = env.XMax
        m_XMin = env.XMin
        m_YMax = env.YMax
        m_YMin = env.YMin


    End Sub
    Public Sub New(ByVal Name As String, ByVal XMax As String, ByVal XMin As String, ByVal YMax As String, ByVal YMin As String)
        m_Name = Name
        m_XMax = XMax
        m_XMin = XMin
        m_YMax = YMax
        m_YMin = YMin


    End Sub
    Public Function isEmpty() As Boolean
        If m_XMax = Nothing Or m_XMin = Nothing Or m_YMax = Nothing Or m_YMin = Nothing Then
            Return True
        Else
            If m_XMax = "" Or m_XMin = "" Or m_YMax = "" Or m_YMin = "" Then
                Return True
            Else
                Return False
            End If
        End If
    End Function
    <XmlAttribute("Name")> _
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property

    <XmlAttribute("XMax")> _
    Public Property XMax() As String
        Get
            Return m_XMax
        End Get
        Set(ByVal value As String)
            m_XMax = value
        End Set
    End Property
    <XmlAttribute("XMin")> _
    Public Property XMin() As String
        Get
            Return m_XMin
        End Get
        Set(ByVal value As String)
            m_XMin = value
        End Set
    End Property
    <XmlAttribute("YMax")> _
    Public Property YMax() As String
        Get
            Return m_YMax
        End Get
        Set(ByVal value As String)
            m_YMax = value
        End Set
    End Property
    <XmlAttribute("YMin")> _
    Public Property YMin() As String
        Get
            Return m_YMin
        End Get
        Set(ByVal value As String)
            m_YMin = value
        End Set
    End Property


End Class
Public Class FieldPairs
    Private m_FieldToCopy As String
    Private m_FieldToPopulate As String
    Private m_SetReadOnly As Boolean
    Public Sub New(ByVal FieldToPopulate As String, ByVal FieldToCopy As String, ByVal SetReadOnly As String)
        m_FieldToCopy = FieldToCopy
        m_FieldToPopulate = FieldToPopulate
        If SetReadOnly.Trim.ToUpper = "FALSE" Then
            m_SetReadOnly = False
        Else
            m_SetReadOnly = True
        End If

    End Sub
    Public Sub New(ByVal FieldToPopulate As String, ByVal FieldToCopy As String, ByVal SetReadOnly As Boolean)
        m_FieldToCopy = FieldToCopy
        m_FieldToPopulate = FieldToPopulate

        m_SetReadOnly = SetReadOnly

        

    End Sub
    Public Property SetReadOnly As Boolean
        Get
            Return m_SetReadOnly
        End Get
        Set(ByVal value As Boolean)
            m_SetReadOnly = value
        End Set
       
    End Property
    Public Property FieldToPopulate As String
        Get
            Return m_FieldToPopulate
        End Get
        Set(ByVal value As String)
            m_FieldToPopulate = value
        End Set
    End Property

    Public Property FieldToCopy As String
        Get
            Return m_FieldToCopy
        End Get
        Set(ByVal value As String)
            m_FieldToCopy = value
        End Set
    End Property
End Class
Public Class ConfigEntries
    Public Property FileName() As String
        Get
            Return m_FileName
        End Get
        Set(ByVal value As String)
            m_FileName = value
        End Set
    End Property
    Private m_FileName As String
    Public Property Path() As String
        Get
            Return m_Path
        End Get
        Set(ByVal value As String)
            m_Path = value
        End Set
    End Property
    Private m_Path As String
    Public Property FullName() As String
        Get
            Return m_FullName
        End Get
        Set(ByVal value As String)
            m_FullName = value
        End Set
    End Property
    Private m_FullName As String
    Public Property Loaded() As Boolean
        Get
            Return m_Loaded
        End Get
        Set(ByVal value As Boolean)
            m_Loaded = value
        End Set
    End Property
    Private m_Loaded As Boolean
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
End Class