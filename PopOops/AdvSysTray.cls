VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "AdvSysTray"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
' PopOops is pop-up blocking solution for Internet Explorer
' Copyright (C) 2002-2004 Shahin Gasanov
'
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

Option Explicit

Private Const NIM_ADD = &H0
Private Const NIM_MODIFY = &H1
Private Const NIM_DELETE = &H2

Private Const NIF_MESSAGE = &H1
Private Const NIF_ICON = &H2
Private Const NIF_TIP = &H4
Private Const NIF_STATE = &H8
Private Const NIF_INFO = &H10

Private Const NIF_ALL = NIF_MESSAGE Or NIF_ICON Or NIF_TIP

Private Const WM_MOUSEMOVE = &H200
Private Const WM_LBUTTONDOWN = &H201
Private Const WM_LBUTTONUP = &H202
Private Const WM_LBUTTONDBLCLK = &H203
Private Const WM_RBUTTONDOWN = &H204
Private Const WM_RBUTTONUP = &H205
Private Const WM_RBUTTONDBLCLK = &H206

Private Const NOTIFYICONDATA_V1_SIZE = 88

Public Enum NIIF
    NIIF_NONE = &H0
    NIIF_INFO = &H1
    NIIF_WARNING = &H2
    NIIF_ERROR = &H3
    NIIF_NOSOUND = &H10
End Enum

Private Type DLLVERSIONINFO
    cbSize As Long
    dwMajorVersion As Long
    dwMinorVersion As Long
    dwBuildNumber As Long
    dwPlatformID As Long
End Type

Private Type NOTIFYICONDATA
    cbSize As Long
    hWnd As Long
    uID As Long
    uFlags As Long
    uCallbackMessage As Long
    hIcon As Long
    szTip As String * 128
    dwState As Long
    dwStateMask As Long
    szInfo As String * 256
    uTimeoutVersion As Long
    szInfoTitle As String * 64
    dwInfoFlags As Long
End Type

Private lVersion As Long
Private iTipSize As Integer
Private fCreated As Boolean
Private WithEvents frmOwner As Form
Attribute frmOwner.VB_VarHelpID = -1
Private nid As NOTIFYICONDATA

Private Declare Function DllGetVersion Lib "shell32" (dwVersion As DLLVERSIONINFO) As Long
Private Declare Function Shell_NotifyIcon Lib "shell32" Alias "Shell_NotifyIconA" (ByVal dwMessage As Long, lpData As NOTIFYICONDATA) As Long

Public Event LButtonDown()
Public Event LButtonUp()
Public Event LButtonDblClk()
Public Event RButtonDown()
Public Event RButtonUp()
Public Event RButtonDblClk()

Private Sub Class_Initialize()
    Dim dvi As DLLVERSIONINFO
    
    fCreated = False
    
    dvi.cbSize = Len(dvi)
    On Error Resume Next
    DllGetVersion dvi
    On Error GoTo 0
    
    lVersion = dvi.dwMajorVersion
    
    iTipSize = IIf(lVersion < 5, 63, 127)
End Sub

Private Sub Class_Terminate()
    Destroy
End Sub

Public Property Get ShellVersion() As Long
    ShellVersion = lVersion
End Property

Public Property Let Icon(ByVal Icon As StdPicture)
    If Not fCreated Then Exit Property
    
    nid.hIcon = Icon
    Shell_NotifyIcon NIM_MODIFY, nid
End Property

Public Property Get Icon() As StdPicture
    If Not fCreated Then Exit Property
    
    Icon = nid.hIcon
End Property

Public Property Let Tooltip(ByVal Tooltip As String)
    If Not fCreated Then Exit Property
    
    nid.szTip = Left$(Tooltip, iTipSize) & vbNullChar
    Shell_NotifyIcon NIM_MODIFY, nid
End Property

Public Property Get Tooltip() As String
    If Not fCreated Then Exit Property
    
    Tooltip = Left$(nid.szTip, InStr(nid.szTip, vbNullChar) - 1)
End Property

Public Sub Create(ByVal Owner As Form)
    If fCreated Then Exit Sub
    
    Set frmOwner = Owner
    
    With nid
        If lVersion >= 5 Then
            .cbSize = Len(nid)
            .uFlags = NIF_ALL Or NIF_INFO
        Else
            .cbSize = NOTIFYICONDATA_V1_SIZE
            .uFlags = NIF_ALL
        End If
        .hWnd = frmOwner.hWnd
        .uID = 1
        .uCallbackMessage = WM_MOUSEMOVE
        .hIcon = frmOwner.Icon
        .szTip = frmOwner.Caption & vbNullChar
    End With
    
    fCreated = CBool(Shell_NotifyIcon(NIM_ADD, nid))
End Sub

Public Sub Destroy()
    If Not fCreated Then Exit Sub
    
    fCreated = Not CBool(Shell_NotifyIcon(NIM_DELETE, nid))
    Set frmOwner = Nothing
End Sub

Public Sub ShowBalloon(ByVal text As String, Optional ByVal Title As String = "", Optional ByVal Flags As NIIF = NIIF_NONE Or NIIF_NOSOUND)
    If Not (fCreated And lVersion >= 5) Then Exit Sub
    
    With nid
        .szInfo = Left$(text, 255) & vbNullChar
        .szInfoTitle = Left$(Title, 63) & vbNullChar
        .dwInfoFlags = Flags
    End With
    
    Shell_NotifyIcon NIM_MODIFY, nid
End Sub

Public Sub HideBalloon()
    If Not (fCreated And lVersion >= 5) Then Exit Sub
    
    nid.szInfo = vbNullChar
    Shell_NotifyIcon NIM_MODIFY, nid
End Sub

Private Sub frmOwner_MouseMove(Button As Integer, Shift As Integer, X As Single, Y As Single)
    Select Case X / Screen.TwipsPerPixelX
        Case WM_LBUTTONDOWN:
            RaiseEvent LButtonDown
        Case WM_LBUTTONUP:
            RaiseEvent LButtonUp
        Case WM_LBUTTONDBLCLK:
            RaiseEvent LButtonDblClk
        Case WM_RBUTTONDOWN:
            RaiseEvent RButtonDown
        Case WM_RBUTTONUP:
            RaiseEvent RButtonUp
        Case WM_RBUTTONDBLCLK:
            RaiseEvent RButtonDblClk
    End Select
End Sub
