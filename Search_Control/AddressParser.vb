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


'***
'Thanks to Andress Chapkowski at ESRI for this parser class
'**
Public Class AddressParser



    Private Shared arrStateNames As String() = {"washington", "montana", "maine", "north dakota", "south dakota", "wyoming", _
    "wisconsin", "idaho", "vermont", "minnesota", "oregon", "new hampshire", _
    "iowa", "massachusetts", "nebraska", "new york", "pennsylvania", "connecticut", _
    "rhode island", "new jersey", "indiana", "nevada", "utah", "california", _
    "ohio", "illinois", "district of columbia", "delaware", "west virginia", "maryland", _
    "colorado", "kentucky", "kansas", "virginia", "missouri", "arizona", _
    "oklahoma", "north carolina", "tennessee", "texas", "new mexico", "alabama", _
    "mississippi", "georgia", "south carolina", "arkansas", "louisiana", "florida", _
    "michigan", "hawaii", "alaska", "washington, dc", "washington dc"}

    Private Shared arrStateAbbrevs As String() = {"WA", "MT", "ME", "ND", "SD", "WY", _
    "WI", "ID", "VT", "MN", "OR", "NH", _
    "IA", "MA", "NE", "NY", "PA", "CT", _
    "RI", "NJ", "IN", "NV", "UT", "CA", _
    "OH", "IL", "DC", "DE", "WV", "MD", _
    "CO", "KY", "KS", "VA", "MO", "AZ", _
    "OK", "NC", "TN", "TX", "NM", "AL", _
    "MS", "GA", "SC", "AR", "LA", "FL", _
    "MI", "HI", "AK", "DC", "DC"}

    '* 
    ' * Put most frequenly used names upfront as we are using brute force to search for the name !!! 
    ' 

    Private Shared streetTypes As String() = {"RD", "ST", "STREET", "ROAD", "AV", "AVE", _
    "AVENUE", "DR", "DRIVE", "RT", "RTE", "ALY", _
    "ANX", "ARC", "BCH", "BG", "BLF", "BLFS", _
    "BLVD", "BND", "BOULEVARD", "BR", "BRG", "BRK", _
    "BRKS", "BTM", "BYP", "BYU", "CIR", "CIRCLE", _
    "CIRS", "CLB", "CLF", "CLFS", "CMN", "CMNS", _
    "COR", "CORS", "COURT", "CP", "CPE", "CRES", _
    "CRK", "CRSE", "CRST", "CSWY", "CT", "CTR", _
    "CTS", "CURV", "CV", "CVS", "CYN", "DL", _
    "DM", "DRS", "DV", "EST", "ESTS", "EXPY", _
    "EXT", "FALL", "FLD", "FLDS", "FLS", "FLT", _
    "FLTS", "FRD", "FRG", "FRK", "FRKS", "FRST", _
    "FRY", "FT", "FWY", "GDN", "GDNS", "GLN", _
    "GLNS", "GRN", "GRNS", "GRV", "GRVS", "GTWY", _
    "HBR", "HL", "HLS", "HOLW", "HTS", "HVN", _
    "HWY", "INLT", "IS", "ISLE", "ISS", "JCT", _
    "KNL", "KNLS", "KY", "KYS", "LAND", "LCK", _
    "LCKS", "LDG", "LF", "LGT", "LGTS", "LK", _
    "LKS", "LN", "LNDG", "LOOP", "MALL", "MDW", _
    "MDWS", "MEWS", "ML", "MLS", "MNR", "MNRS", _
    "MSN", "MT", "MTN", "MTNS", "MTWY", "NCK", _
    "OPAS", "ORCH", "OVAL", "OVLK", "PARK", "PASS", _
    "PATH", "PIKE", "PKWY", "PL", "PLN", "PLNS", _
    "PLZ", "PNE", "PNES", "PR", "PRT", "PSGE", _
    "PT", "PTS", "RADL", "RAMP", "RDG", "RDGS", _
    "RDS", "RIV", "RNCH", "ROW", "RPDS", "RST", _
    "RUE", "RUN", "SHL", "SHLS", "SHR", "SHRS", _
    "SKWY", "SMT", "SPG", "SPGS", "SPUR", "SQ", _
    "STA", "STRA", "STRM", "STS", "TER", "TPKE", _
    "TRAK", "TRCE", "TRFY", "TRL", "TRLR", "TRWY", _
    "TUNL", "UN", "UPAS", "VIA", "VIS", "VL", _
    "VLG", "VLY", "VW", "VWS", "WALK", "WALL", _
    "WAY", "WAYS", "WL", "WLS", "XING", "XRD", _
    "XRDS"}

    Private Shared streetPrefix As String() = {"E", "N", "NE", "NW", "S", "SE", _
    "SW", "W"}

    Private Shared streetSuffix As String() = {"E", "N", "NE", "NW", "S", "SE", _
    "SW", "W"}
    Private streetNumber As String = ""
    Private Prefix As String = ""
    Private streetName As String = ""
    Private streetType As String = ""
    Private Suffix As String = ""
    Private city As String = ""
    Private zip As String = ""
    Private state As String = ""
    Private stateAbbreviation As String = ""
    Private stateInput As String = ""


    '* 
    ' * Parse the place string name. Find address, zip, city, and state. 
    ' 

    Public Sub New(ByVal text As String)

        If text Is Nothing OrElse text.Length() = 0 Then
            Return
        End If
        'Parse the Address for each piece
        Dim tmpParts As String() = parseForDelimiters(text.Trim(), " ")

        streetName = "" 'text.Trim()
        For i As Integer = 0 To tmpParts.Length - 1
            If isNumber(tmpParts(i)) Then
                streetNumber = tmpParts(i)
            ElseIf isStreetPrefix(tmpParts(i)) Then
                Prefix = tmpParts(i)
            ElseIf isStreetSuffix(tmpParts(i)) Then
                Suffix = tmpParts(i)
            ElseIf isStreetType(tmpParts(i)) Then
                streetType = tmpParts(i)
            ElseIf isZip(tmpParts(i)) Then
                zip = tmpParts(i)

            Else

                If streetName <> "" Then

                    streetName = streetName & " " & tmpParts(i)
                ElseIf streetName = "" Then
                    streetName = tmpParts(i)
                End If

            End If
        Next


    End Sub

    '* 
    ' * Parse string into ArrayList. Divide by delimiter. 
    ' 

    Private Function parseForDelimiters(ByVal name As String, ByVal delimiter As String) As String()

        Dim st As String = name
        Dim parts As String() = st.Split(CChar(delimiter))

        Dim i As Integer = 0

        For i = 0 To parts.Length - 1

            parts(i) = CStr(parts(i).Trim())
        Next

        Return parts

    End Function
    '* 
    ' * Is the text a zip code? 
    ' 

    Private Function isZip(ByVal text As String) As Boolean

        ' is it 5 characters in length? 
        If text.Length() = 5 Then
            ' is it numeric 
            Try
                Integer.Parse(text) '.[Integer]()
                Return True
            Catch e As Exception
                Return False
            End Try
        ElseIf text.Length() = 10 AndAlso text.IndexOf("-") = 5 Then
            ' is it 10 characters in length 
            Dim first As String = text.Substring(0, 5)
            Dim last As String = text.Substring(6, 10)
            ' is it numeric 
            Try
                Integer.Parse(first) ').[Integer]()
                Try
                    Integer.Parse(last) '.[Integer]()
                    Return True
                Catch e As Exception
                    Return False
                End Try
            Catch e As Exception
                Return False
            End Try
        Else
            Return False
        End If
    End Function

    '* 
    ' * Is the text a state ? 
    ' 

    Private Function isState(ByVal part As String(), ByVal count As Integer) As Boolean
        Dim found As Boolean = False
        Dim text As String = ""
        For k As Integer = 0 To 2
            If part.Length > count + k Then
                If text.Length() > 0 Then
                    text += " "
                End If
                text += part(count + k)
                For i As Integer = 0 To arrStateNames.Length - 1
                    If text.ToLower.Equals(DirectCast(arrStateNames(i), String)) Then
                        stateAbbreviation = arrStateAbbrevs(i)
                        stateInput = text
                        found = True
                        Exit For
                    End If
                Next
                If Not found Then
                    For i As Integer = 0 To arrStateAbbrevs.Length - 1
                        If text.ToUpper.Equals(DirectCast(arrStateAbbrevs(i), String)) Then
                            stateAbbreviation = arrStateAbbrevs(i)
                            stateInput = text
                            found = True
                            Exit For
                        End If
                    Next
                End If
            Else
                Exit For
            End If
        Next
        Return found
    End Function

    '* 
    ' * Is the text a street prefix ? 
    ' 

    Private Function isStreetPrefix(ByVal text As String) As Boolean
        For i As Integer = 0 To streetPrefix.Length - 1

            If text.ToUpper.Equals(DirectCast(streetPrefix(i), String)) Then
                Return True
            End If
        Next
        Return False
    End Function

    '* 
    ' * Is the text a street suffix ? 
    ' 

    Private Function isStreetSuffix(ByVal text As String) As Boolean
        For i As Integer = 0 To streetSuffix.Length - 1

            If text.ToUpper.Equals(DirectCast(streetSuffix(i), String)) Then
                Return True
            End If
        Next
        Return False
    End Function

    '* 
    ' * Is the text a street type ? 
    ' 

    Private Function isStreetType(ByVal text As String) As Boolean
        For i As Integer = 0 To streetTypes.Length - 1

            If text.ToUpper.Equals(DirectCast(streetTypes(i), String)) Then
                Return True
            End If
        Next
        Return False
    End Function

    '* 
    ' * Is the text a appartment number 
    ' 

    Private Function isAppartmentNumber(ByVal text As String) As Boolean

        If text.StartsWith("#") Then
            Return True
        End If
        Return False
    End Function

    '* 
    ' * Is the text a number? 
    ' 

    Private Function isNumber(ByVal text As String) As Boolean

        Try
            Integer.Parse(text) 'parseInt(text).[integer]()
            Return True
        Catch e As Exception
            Return False
        End Try
    End Function


    Public Function getStreetName() As String
        Return Me.streetName
    End Function
    Public Function getStreetNumber() As String
        Return Me.streetNumber
    End Function
    Public Function getStreetType() As String
        Return Me.streetType
    End Function

    Public Function getStreetSuffix() As String
        Return Me.Suffix
    End Function
    Public Function getStreetPrefix() As String
        Return Me.Prefix
    End Function
    '* 
    ' * Get the city. 
    ' 

    Public Function getCity() As String
        Return Me.city
    End Function

    '* 
    ' * Get the state. 
    ' 

    Public Function getState() As String
        Return Me.state
    End Function

    '* 
    ' * Get the zip. 
    ' 

    Public Function getZip() As String
        Return Me.zip
    End Function

End Class
