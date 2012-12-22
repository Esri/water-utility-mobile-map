Public Class DataTableTools
    Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal ParamArray FieldNames() As String) As DataTable
        '**Function borrowed from Erik Porter
        '**http://weblogs.asp.net/eporter/archive/2005/02/10/370548.aspx

        Dim lastValues() As Object
        Dim newTable As DataTable

        If FieldNames Is Nothing OrElse FieldNames.Length = 0 Then
            Throw New ArgumentNullException("FieldNames")
        End If

        lastValues = New Object(FieldNames.Length - 1) {}
        newTable = New DataTable

        For Each field As String In FieldNames
            newTable.Columns.Add(field, SourceTable.Columns(field).DataType)
        Next
        Dim pDR() As DataRow = SourceTable.Select("")


        For Each Row As DataRow In SourceTable.Select("", String.Join(", ", FieldNames))
            If Not fieldValuesAreEqual(lastValues, Row, FieldNames) Then
                newTable.Rows.Add(createRowClone(Row, newTable.NewRow(), FieldNames))

                setLastValues(lastValues, Row, FieldNames)
            End If
        Next

        Return newTable
    End Function
    Private Function fieldValuesAreEqual(ByVal lastValues() As Object, ByVal currentRow As DataRow, ByVal fieldNames() As String) As Boolean
        '**Function borrowed from Erik Porter
        '**http://weblogs.asp.net/eporter/archive/2005/02/10/370548.aspx

        Dim areEqual As Boolean = True

        For i As Integer = 0 To fieldNames.Length - 1
            If lastValues(i) Is Nothing OrElse Not lastValues(i).Equals(currentRow(fieldNames(i))) Then
                areEqual = False
                Exit For
            End If
        Next

        Return areEqual
    End Function
    Private Function createRowClone(ByVal sourceRow As DataRow, ByVal newRow As DataRow, ByVal fieldNames() As String) As DataRow
        '**Function borrowed from Erik Porter
        '**http://weblogs.asp.net/eporter/archive/2005/02/10/370548.aspx

        For Each field As String In fieldNames
            newRow(field) = sourceRow(field)
        Next

        Return newRow
    End Function
    Private Sub setLastValues(ByVal lastValues() As Object, ByVal sourceRow As DataRow, ByVal fieldNames() As String)
        '**Function borrowed from Erik Porter
        '**http://weblogs.asp.net/eporter/archive/2005/02/10/370548.aspx

        For i As Integer = 0 To fieldNames.Length - 1
            lastValues(i) = sourceRow(fieldNames(i))
        Next
    End Sub
    Public Function copySchema(ByRef pSourceDT As DataTable) As DataTable
        '      Dim filename As String = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArcGISTemplates\MobileTemplates\SchemaDoc.xml")


        ' Create a FileStream object with the file path and name.
        'Dim stream As New System.IO.FileStream _
        '   (filename, System.IO.FileMode.Create)

        ' Create a new XmlTextWriter object with the FileStream.
        'Dim writer As New System.Xml.XmlTextWriter _
        '   (stream, System.Text.Encoding.Unicode)

        ' Write the schema into the DataSet and close the reader.


        '  pSourceDT.WriteXmlSchema(writer)
        ' writer.Close()

        ' Create a FileStream object with the file path and name.
        ' stream = New System.IO.FileStream _
        '   (filename, System.IO.FileMode.Open)
        Dim stream As System.IO.MemoryStream = New System.IO.MemoryStream()
        pSourceDT.WriteXmlSchema(stream)
        'Dim xDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()
        'stream.Position = 0

        'xDocument.Load(stream)

        ' Create a new XmlTextReader object with the FileStream.
        'Dim xmlReader As New System.Xml.XmlTextReader(stream)

        ' Read the schema into the DataSet and close the reader.
        Dim pDT As New DataTable
        pDT.Namespace = pSourceDT.Namespace
        pDT.TableName = pSourceDT.TableName
        stream.Position = 0
        pDT.ReadXml(stream)
        ' xmlReader.Close()
        '   writer = Nothing
        ' xmlReader = Nothing
        stream = Nothing
        Try
            '   System.IO.File.Delete(filename)

        Catch ex As Exception

        End Try
        Return pDT
    End Function
End Class
