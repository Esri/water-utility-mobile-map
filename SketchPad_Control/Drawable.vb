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


'*******************
'** Thanks to http://www.vb-helper.com/howto_net_drawing_framework.html
'** for the orignal class design and the basis of this tool
'***************
Imports System.Xml.Serialization
Imports System.Drawing

<Serializable()> _
Public MustInherit Class Drawable
    ' Drawing characteristics.
    <XmlIgnore()> Public ForeColor As Color
    <XmlIgnore()> Public FillColor As Color
    <XmlAttributeAttribute()> Public ParentContainerWidth As Integer = 0
    <XmlAttributeAttribute()> Public ParentContainerHeight As Integer = 0
    <XmlAttributeAttribute()> Public LineWidth As Integer = 0
    <XmlAttributeAttribute()> Public Label As Boolean = True
    <XmlAttributeAttribute()> Public BlockSize As Integer
    <XmlAttributeAttribute()> Public FeetPerBlock As Integer
    <XmlAttributeAttribute()> Public X1 As Integer
    <XmlAttributeAttribute()> Public Y1 As Integer
    <XmlAttributeAttribute()> Public X2 As Integer
    <XmlAttributeAttribute()> Public Y2 As Integer

    ' Indicates whether we should draw as selected.
    <XmlIgnore()> Public IsSelected As Boolean = False
    <XmlIgnore()> Public IsDraw As Boolean = True

    ' Constructors.
    Public Sub New()
        ForeColor = Color.Black
        FillColor = Color.White
    End Sub
    Public Sub New(ByVal fore_color As Color, ByVal fill_color As Color, Optional ByVal line_width As Integer = 0, Optional ByVal label_distance As Boolean = True, Optional ByVal Block_Size As Integer = 25, Optional ByVal Feet_Per_Block As Integer = 10)
        LineWidth = line_width
        ForeColor = fore_color
        FillColor = fill_color
        Label = label_distance
        BlockSize = Block_Size
        FeetPerBlock = Feet_Per_Block

    End Sub

    ' Property procedures to serialize and
    ' deserialize ForeColor and FillColor.
    <XmlAttributeAttribute("ForeColor")> _
    Public Property ForeColorArgb() As Integer
        Get
            Return ForeColor.ToArgb()
        End Get
        Set(ByVal Value As Integer)
            ForeColor = Color.FromArgb(Value)
        End Set
    End Property
    <XmlAttributeAttribute("BackColor")> _
    Public Property FillColorArgb() As Integer
        Get
            Return FillColor.ToArgb()
        End Get
        Set(ByVal Value As Integer)
            FillColor = Color.FromArgb(Value)
        End Set
    End Property

#Region "Methods to override"
    ' Draw the object on this Graphics surface.
    Public MustOverride Sub Draw(ByVal gr As Graphics)

    ' Return the object's bounding rectangle.
    Public MustOverride Function GetBounds() As Rectangle

    ' Return True if this point is on the object.
    Public MustOverride Function IsAt(ByVal x As Integer, ByVal y As Integer) As Boolean

    ' The user is moving one of the object's points.
    Public MustOverride Sub NewPoint(ByVal x As Integer, ByVal y As Integer)

    ' Return True if the object is empty (e.g. a zero-length line).
    Public MustOverride Function IsEmpty() As Boolean
#End Region

End Class
