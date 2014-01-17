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


Imports System.Drawing
Imports System.Drawing.Drawing2D
Public NotInheritable Class MobileMapConsoleSplash

    'TODO: This form can easily be set as the splash screen for the application by going to the "Application" tab
    '  of the Project Designer ("Properties" under the "Project" menu).

    Public title As String
    Public versionInfo As String
    Public copyrightInfo As String
    Private Sub fieldConsoleSplash_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Set up the dialog text at runtime according to the application's assembly information.  

        'TODO: Customize the application's assembly information in the "Application" pane of the project 
        ''  properties dialog (under the "Project" menu).

        ''Application title
        'If My.Application.Info.Title <> "" Then
        '    ApplicationTitle.Text = My.Application.Info.Title
        'Else
        '    'If the application title is missing, use the application name, without the extension
        '    ApplicationTitle.Text = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        'End If

        ''Format the version information using the text set into the Version control at design time as the
        ''  formatting string.  This allows for effective localization if desired.
        ''  Build and revision information could be included by using the following code and changing the 
        ''  Version control's designtime text to "Version {0}.{1:00}.{2}.{3}" or something similar.  See
        ''  String.Format() in Help for more information.
        ''
        ''    Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision)

        'Version.Text = System.String.Format(Version.Text, My.Application.Info.Version.Major, My.Application.Info.Version.Minor)

        ''Copyright info
        'Copyright.Text = My.Application.Info.Copyright


    End Sub
    Private Function GetStringPath(ByVal s As String, ByVal dpi As Single, ByVal rect As RectangleF, ByVal font As Font, ByVal format As StringFormat) As GraphicsPath
        Dim path As GraphicsPath = New GraphicsPath()
        Dim emSize As Single = dpi * font.SizeInPoints / 16
        path.AddString(s, font.FontFamily, CInt(font.Style), emSize, rect, format)
        Return path
    End Function
    Private Function GetStringPathSmall(ByVal s As String, ByVal dpi As Single, ByVal rect As RectangleF, ByVal font As Font, ByVal format As StringFormat) As GraphicsPath
        Dim path As GraphicsPath = New GraphicsPath()
        Dim emSize As Single = dpi * font.SizeInPoints / 66
        path.AddString(s, font.FontFamily, CInt(font.Style), emSize, rect, format)
        Return path
    End Function

    Private Sub ShadowFontsForm_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Label1.Paint
        Dim g As Graphics = e.Graphics
        Dim s As String
        s = title




        Dim size As SizeF = New SizeF(CType(sender, Control).Width - 4, CType(sender, Control).Height - 4)

        Dim rect As RectangleF = New RectangleF(0, 0, size.Width, size.Height)

        Dim pathShadow As GraphicsPath = GetStringPath(s, g.DpiY, New RectangleF(4, 4, size.Width, size.Height), Me.Font, StringFormat.GenericTypographic)
        Dim path As GraphicsPath = GetStringPath(s, g.DpiY, rect, Me.Font, StringFormat.GenericTypographic)
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.FillPath(Brushes.Black, pathShadow)
        g.FillPath(Brushes.WhiteSmoke, path)
        '  g.Dispose()
        g = Nothing

    End Sub


    Private Sub Label2_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Label2.Paint
        Dim g As Graphics = e.Graphics
        'Dim s As String
        's = System.String.Format("Version {0}.{1}", My.Application.Info.Version.Major, My.Application.Info.Version.Minor)



        Dim size As SizeF = New SizeF(CType(sender, Control).Width - 4, CType(sender, Control).Height - 4)

        Dim rect As RectangleF = New RectangleF(0, 0, size.Width, size.Height)

        Dim pathShadow As GraphicsPath = GetStringPathSmall(versionInfo, g.DpiY, New RectangleF(1, 1, size.Width, size.Height), Me.Font, StringFormat.GenericTypographic)
        Dim path As GraphicsPath = GetStringPathSmall(versionInfo, g.DpiY, rect, Me.Font, StringFormat.GenericTypographic)
        g.SmoothingMode = SmoothingMode.AntiAlias
        '  g.FillPath(Brushes.SlateGray, pathShadow)
        g.FillPath(Brushes.Black, path)
        '    g.Dispose()
        g = Nothing

    End Sub

    Private Sub Label3_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Label3.Paint

        Dim g As Graphics = e.Graphics
        'Dim s As String
        's = My.Application.Info.Copyright



        Dim size As SizeF = New SizeF(CType(sender, Control).Width - 4, CType(sender, Control).Height - 4)

        Dim rect As RectangleF = New RectangleF(0, 0, size.Width, size.Height)

        Dim pathShadow As GraphicsPath = GetStringPathSmall(copyrightInfo, g.DpiY, New RectangleF(1, 1, size.Width, size.Height), Me.Font, StringFormat.GenericTypographic)
        Dim path As GraphicsPath = GetStringPathSmall(copyrightInfo, g.DpiY, rect, Me.Font, StringFormat.GenericTypographic)
        g.SmoothingMode = SmoothingMode.AntiAlias
        '  g.FillPath(Brushes.LightSlateGray, pathShadow)
        g.FillPath(Brushes.Black, path)
        '    g.Dispose()
        g = Nothing

    End Sub


End Class
