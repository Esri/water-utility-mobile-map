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
Imports System.IO
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Net
Public Enum ConnectionState
    INTERNET_CONNECTION_MODEM = 1
    INTERNET_CONNECTION_MODEM_BUSY = 8
    INTERNET_CONNECTION_CONFIGURED = 64
    INTERNET_CONNECTION_OFFLINE = 16
    INTERNET_CONNECTION_LAN = 18
    INTERNET_CONNECTION_PROXY = 86
End Enum
Public Class CheckStatusClass
    Private WithEvents m_ChkServStat As checkServiceStatus
    Public Event connectionStateChanged(ByVal connectionStatus As ConnectionState)

    Public Event URLError(ByVal Message As String)
    Public Delegate Sub m_ExtractorDll_ConnectionStatusChangedDelegate(ByVal ConnectionStatus As ConnectionState)
    Public Sub dispose()
        If m_ChkServStat IsNot Nothing Then
            m_ChkServStat.dispose()
        End If
    End Sub
    Public Sub New()


    End Sub
    Public Sub startChecking(ByVal strURL As String, ByVal netCred As ICredentials, Optional ByVal checkInterval As Integer = 10000, Optional ByVal SecureUserName As String = "", Optional ByVal SecurePassword As String = "", Optional ByVal SecureDomain As String = "")
        m_ChkServStat = New checkServiceStatus(strURL, netCred, checkInterval, SecureUserName, SecurePassword, SecureDomain)
        m_ChkServStat.startChecking()

    End Sub
    Public Sub stopChecking()
        If m_ChkServStat Is Nothing Then Return

        m_ChkServStat.stopChecking()

    End Sub

    Public Sub m_ChkServStat_connectionStateChanged(ByVal connectionStatus As ConnectionState) Handles m_ChkServStat.connectionStateChanged

        RaiseEvent connectionStateChanged(connectionStatus)

    End Sub






    Private Class InetConnection

#Region "Internet Connection"
        '	private System.Threading.Timer timer;

        '    <DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)> _
        'Private Shared Function InternetGetConnectedStateEx(<MarshalAs(UnmanagedType.U4)> _
        '            ByRef lpdwFlags As System.UInt32, ByVal lpszConnectioName As String, <MarshalAs(UnmanagedType.U4)> _
        '            ByVal dwNameLen As Integer, <MarshalAs(UnmanagedType.U4)> _
        '            ByVal dwReserved As Integer) As Boolean
        '    End Function
        Private Declare Auto Function InternetGetConnectedStateEx Lib "wininet" (<MarshalAs(UnmanagedType.U4)> _
                ByVal lpdwFlags As System.UInt32, <MarshalAs(UnmanagedType.U4)> _
                ByVal lpszConnectioName As String, <MarshalAs(UnmanagedType.U4)> _
                ByVal dwNameLen As Integer, <MarshalAs(UnmanagedType.U4)> _
                ByVal dwReserved As Integer) As Boolean

#End Region

        Public Shared Sub InetConnectionState(ByRef Connection As MobileControls.ConnectionState, ByRef Message As String, ByVal url As String, Optional ByVal SecureServiceUserName As String = "", Optional ByVal SecureServicePassword As String = "", Optional ByVal SecureServiceDomain As String = "")

            Connection = 0

            Dim pMess As String = ""


            Dim pStrUserName As String = SecureServiceUserName
            Dim pStrPass As String = SecureServicePassword
            Dim pStrDom As String = SecureServiceDomain

            Dim req As System.Net.WebRequest = System.Net.WebRequest.Create(url + "?wsdl")
            If pStrUserName <> "" And pStrPass <> "" Then
                Dim myCred As System.Net.NetworkCredential = New System.Net.NetworkCredential(pStrUserName, pStrPass, pStrDom)
                req.Credentials = myCred

            End If

            Try


                Dim wr As System.Net.WebResponse = req.GetResponse()
                wr.Close()
                pMess = "LAN Connection\0"
                Connection = ConnectionState.INTERNET_CONNECTION_LAN
                'INTERNET_CONNECTION_MODEM = 1
                'INTERNET_CONNECTION_MODEM_BUSY = 8
                'INTERNET_CONNECTION_CONFIGURED = 64
                'INTERNET_CONNECTION_OFFLINE = 16
                'INTERNET_CONNECTION_LAN = 18
                'INTERNET_CONNECTION_PROXY = 86
            Catch


                pMess = "\0"
                Connection = ConnectionState.INTERNET_CONNECTION_OFFLINE
            End Try

            Message = pMess
        End Sub
        Public Shared Sub InetConnectionState(ByRef Connection As MobileControls.ConnectionState, ByRef Message As String, ByVal url As String, netCred As ICredentials)

            Connection = 0

            Dim pMess As String = ""





            Try
                Dim httpRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
                If netCred IsNot Nothing Then

                    httpRequest.Credentials = netCred

                End If
                httpRequest.Method = "GET"

                ' if the URI doesn't exist, an exception will be thrown here...
                Using httpResponse As HttpWebResponse = DirectCast(httpRequest.GetResponse(), HttpWebResponse)
                    Using responseStream As Stream = httpResponse.GetResponseStream()
                        Using readStream As StreamReader = New StreamReader(responseStream, System.Text.Encoding.UTF8)

                            'Console.WriteLine("Response stream received.")
                            'Console.WriteLine(readStream.ReadToEnd())
                            Dim strRes As String = readStream.ReadToEnd()
                            If strRes.Contains("Application Error") Then
                                pMess = "\0"
                                Connection = ConnectionState.INTERNET_CONNECTION_OFFLINE
                            Else

                                pMess = "LAN Connection\0"
                                Connection = ConnectionState.INTERNET_CONNECTION_LAN
                            End If
                            readStream.Close()
                        End Using
                    End Using
                End Using


                'Dim req As System.Net.WebRequest = System.Net.WebRequest.Create(url)
                'If netCred IsNot Nothing Then

                '    req.Credentials = netCred

                'End If

                'Dim wr As System.Net.WebResponse = req.GetResponse()

                '            wr.Close()

                '            pMess = "LAN Connection\0"
                '            Connection = ConnectionState.INTERNET_CONNECTION_LAN
                'INTERNET_CONNECTION_MODEM = 1
                'INTERNET_CONNECTION_MODEM_BUSY = 8
                'INTERNET_CONNECTION_CONFIGURED = 64
                'INTERNET_CONNECTION_OFFLINE = 16
                'INTERNET_CONNECTION_LAN = 18
                'INTERNET_CONNECTION_PROXY = 86
            Catch


                pMess = "\0"
                Connection = ConnectionState.INTERNET_CONNECTION_OFFLINE
            End Try

            Message = pMess
        End Sub
    End Class
    Private Class checkServiceStatus
        Private Const INTERNET_CONNECTION_MODEM As Integer = 1 ' 0x01 ' 1
        Private Const INTERNET_CONNECTION_MODEM_BUSY As Integer = 8 ' 0x08  '8 ' no longer used 
        Private Const INTERNET_CONNECTION_CONFIGURED As Integer = 64 ' 0x40;'64
        Private Const INTERNET_CONNECTION_OFFLINE As Integer = 16 '0x10;'16
        Private Const INTERNET_CONNECTION_LAN As Integer = 18 '0x12;'18
        Private Const INTERNET_CONNECTION_PROXY As Integer = 86 ' 0x56;'86
        Public Event URLError(ByVal Message As String)

        Private Const TOOL_INTERNET_CONNECTION_ONLINE As Integer = 0
        Private Const TOOL_INTERNET_CONNECTION_OFFLINE As Integer = 1
        Private m_Timer As System.Threading.Timer
        Private m_CheckInteval As Integer
        Private m_url As String
        Private m_firstTime As Boolean = True
        Private m_LastConnectState As Integer
        Private m_bConnected As Boolean
        Public Event connectionStateChanged(ByVal connectionStatus As ConnectionState)

        Private m_SecureUserName As String
        Private m_SecurePassword As String
        Private m_SecureDomain As String
        Private m_netCred As ICredentials = Nothing


        Public Sub New(ByVal URL As String, ByVal netCred As ICredentials, Optional ByVal checkInterval As Integer = 10000, Optional ByVal SecureUserName As String = "", Optional ByVal SecurePassword As String = "", Optional ByVal SecureDomain As String = "")
            m_url = URL
            m_netCred = netCred
            m_CheckInteval = checkInterval
            m_LastConnectState = 16
            m_bConnected = False
            m_SecureUserName = SecureUserName
            m_SecurePassword = SecurePassword
            m_SecureDomain = SecureDomain
        End Sub
        Public Sub dispose()

            If (m_Timer IsNot DBNull.Value) Then
                If (m_Timer IsNot Nothing) Then
                    'Remove the old timer.
                    m_Timer.Dispose()
                    m_Timer = Nothing
                End If

            End If

        End Sub
        Public Sub startChecking()
            m_Timer = New System.Threading.Timer(New TimerCallback(AddressOf OnTimer), Me, 0, m_CheckInteval)

        End Sub
        Public Sub stopChecking()
            If (m_Timer Is Nothing) Then
            ElseIf (m_Timer Is DBNull.Value) Then


            Else

                'Remove the old timer.
                m_Timer.Dispose()
                m_Timer = Nothing

            End If
        End Sub
        Private Sub OnTimer(ByVal state As Object)

            Try

                Dim connection As MobileControls.ConnectionState
                Dim message As String = ""
                InetConnection.InetConnectionState(connection, message, m_url, m_netCred)
                ' MsgBox(m_LastConnectState & " " & connection)
                If m_firstTime Then
                    m_firstTime = False
                    m_LastConnectState = connection
                    RaiseEvent connectionStateChanged(connection)
                End If
                If (m_LastConnectState = connection) Then Return



                Select Case connection

                    Case ConnectionState.INTERNET_CONNECTION_MODEM
                        RaiseEvent connectionStateChanged(ConnectionState.INTERNET_CONNECTION_MODEM)

                    Case ConnectionState.INTERNET_CONNECTION_LAN
                        RaiseEvent connectionStateChanged(ConnectionState.INTERNET_CONNECTION_LAN)

                    Case ConnectionState.INTERNET_CONNECTION_PROXY
                        RaiseEvent connectionStateChanged(ConnectionState.INTERNET_CONNECTION_PROXY)
                    Case ConnectionState.INTERNET_CONNECTION_MODEM_BUSY
                        RaiseEvent connectionStateChanged(ConnectionState.INTERNET_CONNECTION_MODEM_BUSY)

                    Case ConnectionState.INTERNET_CONNECTION_OFFLINE
                        RaiseEvent connectionStateChanged(ConnectionState.INTERNET_CONNECTION_OFFLINE)

                    Case ConnectionState.INTERNET_CONNECTION_CONFIGURED
                        RaiseEvent connectionStateChanged(ConnectionState.INTERNET_CONNECTION_CONFIGURED)
                    Case Else


                End Select

                m_LastConnectState = connection

            Catch ex As Exception
                ' MsgBox(ex.Message)
                RaiseEvent URLError(ex.Message)
            End Try

        End Sub

        Public Property timerInterval() As Integer
            Get
                Return m_CheckInteval
            End Get
            Set(ByVal value As Integer)
                m_CheckInteval = value

            End Set
        End Property
    End Class

    Private Sub m_ChkServStat_URLError(ByVal Message As String) Handles m_ChkServStat.URLError
        RaiseEvent URLError(Message)

    End Sub
End Class
