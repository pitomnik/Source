VERSION 5.00
Begin VB.Form frmMain 
   BorderStyle     =   1  'Fixed Single
   ClientHeight    =   1335
   ClientLeft      =   45
   ClientTop       =   615
   ClientWidth     =   2655
   Icon            =   "Main.frx":0000
   LinkTopic       =   "Form1"
   LockControls    =   -1  'True
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   1335
   ScaleWidth      =   2655
   ShowInTaskbar   =   0   'False
   StartUpPosition =   2  'CenterScreen
   Visible         =   0   'False
   Begin VB.PictureBox picHook 
      Height          =   495
      Left            =   1245
      ScaleHeight     =   435
      ScaleWidth      =   1155
      TabIndex        =   0
      Top             =   120
      Visible         =   0   'False
      Width           =   1215
   End
   Begin VB.Timer tmrError 
      Enabled         =   0   'False
      Interval        =   1000
      Left            =   2040
      Top             =   720
   End
   Begin VB.Timer tmrClick 
      Enabled         =   0   'False
      Left            =   840
      Top             =   720
   End
   Begin VB.Timer tmrClass 
      Enabled         =   0   'False
      Interval        =   100
      Left            =   240
      Top             =   720
   End
   Begin VB.Timer tmrInfo 
      Enabled         =   0   'False
      Interval        =   2000
      Left            =   1440
      Top             =   720
   End
   Begin VB.Image imgDisabled 
      Height          =   480
      Left            =   720
      Picture         =   "Main.frx":08CA
      Top             =   120
      Visible         =   0   'False
      Width           =   480
   End
   Begin VB.Image imgEnabled 
      Height          =   480
      Left            =   240
      Picture         =   "Main.frx":1194
      Top             =   120
      Width           =   480
   End
   Begin VB.Menu mnuPopup 
      Caption         =   "Popup"
      Begin VB.Menu mnuAbout 
         Caption         =   "About"
      End
      Begin VB.Menu line1 
         Caption         =   "-"
      End
      Begin VB.Menu mnuSettings 
         Caption         =   "Settings"
         Begin VB.Menu mnuSound 
            Caption         =   "Play sound"
         End
         Begin VB.Menu mnuTooltip 
            Caption         =   "Show tooltip"
         End
         Begin VB.Menu mnuStartup 
            Caption         =   "Run at startup"
         End
      End
      Begin VB.Menu mnuSensitivity_ 
         Caption         =   "Sensitivity"
         Begin VB.Menu mnuSensitivity 
            Caption         =   "High"
            Index           =   0
            Tag             =   "0"
         End
         Begin VB.Menu mnuSensitivity 
            Caption         =   "Medium"
            Index           =   1
            Tag             =   "100"
         End
         Begin VB.Menu mnuSensitivity 
            Caption         =   "Low"
            Index           =   2
            Tag             =   "1000"
         End
      End
      Begin VB.Menu mnuEmergency 
         Caption         =   "Emergency"
         Begin VB.Menu mnuStopAll 
            Caption         =   "Stop all"
         End
         Begin VB.Menu mnuClearAll 
            Caption         =   "Clear all"
         End
         Begin VB.Menu mnuCloseAll 
            Caption         =   "Close all"
         End
      End
      Begin VB.Menu mnuEnabled 
         Caption         =   "Enabled"
      End
      Begin VB.Menu line2 
         Caption         =   "-"
      End
      Begin VB.Menu mnuExit 
         Caption         =   "Exit"
      End
   End
End
Attribute VB_Name = "frmMain"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
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

Private WithEvents stMain As AdvSysTray
Attribute stMain.VB_VarHelpID = -1
Private WithEvents ieMain As IECollection
Attribute ieMain.VB_VarHelpID = -1

Private hMenu As Long
Private sSound As String

Private Sub Form_Load()
    On Error GoTo ErrHand
    
    Dim lRet As Long
    Dim bSound() As Byte
    
    With App
        lRet = FindWindow("ThunderRT6FormDC", .Title)
        If lRet > 0 Then
            MsgBox .Title & " is already running.", vbInformation
            End
        End If
        
        Me.Caption = .Title
        .HelpFile = .Path & "\" & .HelpFile
        
        mnuSound.Checked = GetSetting(.Title, "Settings", "PlaySound", True)
        mnuTooltip.Checked = GetSetting(.Title, "Settings", "ShowTooltip", True)
        mnuStartup.Checked = GetSetting(.Title, "Settings", "RunAtStartup", True)
        mnuSensitivity_Click GetSetting(.Title, "Settings", "Sensitivity", 1)
        mnuEnabled.Checked = GetSetting(.Title, "Settings", "Enabled", True)
    End With
    
    Set stMain = New AdvSysTray
    
    mnuTooltip.Enabled = (stMain.ShellVersion >= 5)
    mnuTooltip.Checked = mnuTooltip.Checked And mnuTooltip.Enabled
    
    hMenu = GetMenu(Me.hWnd)
    hMenu = GetSubMenu(hMenu, 0)
    hMenu = GetSubMenu(hMenu, 3)
    
    bSound = LoadResData(101, "WAVE")
    sSound = String$(UBound(bSound) + 1, vbNullChar)
    CopyMemory ByVal sSound, bSound(0), Len(sSound)
    
    stMain.Create Me
    SetBlocking mnuEnabled.Checked
    
    InitCommonControls
    
    Exit Sub
ErrHand:
    With Err
        Select Case MsgBox("Error " & .Number & " - " & .Description, vbCritical Or vbAbortRetryIgnore, .Source)
            Case vbAbort
                ShutDown
            Case vbRetry
                Resume
            Case vbIgnore
                Resume Next
        End Select
    End With
End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
    On Error GoTo ErrHand
    
    Dim hKey As Long, lRet As Long
    Dim sPathLong As String, sPathShort As String
    
    With App
        SaveSetting .Title, "Settings", "PlaySound", mnuSound.Checked
        SaveSetting .Title, "Settings", "ShowTooltip", mnuTooltip.Checked
        SaveSetting .Title, "Settings", "RunAtStartup", mnuStartup.Checked
        SaveSetting .Title, "Settings", "Sensitivity", iSenseInd
        SaveSetting .Title, "Settings", "Enabled", mnuEnabled.Checked
        
        RegOpenKeyEx HKEY_LOCAL_MACHINE, "SOFTWARE\Microsoft\Windows\CurrentVersion\Run", 0, KEY_ALL_ACCESS, hKey
        If mnuStartup.Checked Then
            sPathLong = .Path & "\" & .EXEName & ".exe"
            sPathShort = String$(165, vbNullChar)
            lRet = GetShortPathName(sPathLong, sPathShort, 164)
            sPathShort = Left(sPathShort, lRet)
            RegSetValueEx hKey, .Title, 0, REG_SZ, ByVal sPathShort, lRet + 1
        Else
            RegDeleteValue hKey, .Title
        End If
        RegCloseKey hKey
    End With
    
    Unload frmAbout
    Set frmAbout = Nothing
    
    Set stMain = Nothing
    Set ieMain = Nothing
    
    Exit Sub
ErrHand:
    With Err
        Select Case MsgBox("Error " & .Number & " - " & .Description, vbCritical Or vbAbortRetryIgnore, .Source)
            Case vbAbort
                ShutDown
            Case vbRetry
                Resume
            Case vbIgnore
                Resume Next
        End Select
    End With
End Sub

Private Sub picHook_MouseUp(Button As Integer, Shift As Integer, X As Single, Y As Single)
    If X / Screen.TwipsPerPixelX = -1 Then
        hClickWin = GetForegroundWindow
        tmrClick.Enabled = True
    End If
End Sub

Private Sub tmrClass_Timer()
    On Error GoTo ErrHand
    
    tmrClass.Enabled = False
    Set ieMain = New IECollection
    
    Exit Sub
ErrHand:
    With Err
        Select Case MsgBox("Error " & .Number & " - " & .Description, vbCritical Or vbAbortRetryIgnore, .Source)
            Case vbAbort
                ShutDown
            Case vbRetry
                Resume
            Case vbIgnore
                Resume Next
        End Select
    End With
End Sub

Private Sub tmrClick_Timer()
    tmrClick.Enabled = False
    hClickWin = 0
End Sub

Private Sub tmrInfo_Timer()
    tmrInfo.Enabled = False
    stMain.HideBalloon
End Sub

Private Sub tmrError_Timer()
    Dim lRet As Long
    
    tmrError.Enabled = False
    
    lRet = FindWindow("#32770", "Error")
    If lRet > 0 Then
        If IsDebugWindow(lRet) Then
            PostMessage lRet, WM_COMMAND, IDNO, 0
        End If
    End If
    
    lRet = FindWindow("Internet Explorer_TridentDlgFrame", "Internet Explorer")
    If lRet > 0 Then
        If IsDebugWindow(lRet) Then
            PostMessage lRet, WM_CLOSE, 0, 0
        End If
    End If
End Sub

Private Sub mnuAbout_Click()
    frmAbout.Show
End Sub

Private Sub mnuSound_Click()
    mnuSound.Checked = Not mnuSound.Checked
End Sub

Private Sub mnuTooltip_Click()
    mnuTooltip.Checked = Not mnuTooltip.Checked
End Sub

Private Sub mnuStartup_Click()
    mnuStartup.Checked = Not mnuStartup.Checked
End Sub

Private Sub mnuSensitivity_Click(Index As Integer)
    iSenseInd = Index
    tmrClick.Interval = mnuSensitivity(Index).Tag
    
    If Not ieMain Is Nothing Then ieMain.Refresh
End Sub

Private Sub mnuStopAll_Click()
    Dim ie As IEMember

    For Each ie In ieMain
        ie.Abort
        DoEvents
    Next
    
    Set ie = Nothing
End Sub

Private Sub mnuClearAll_Click()
    Dim ie As IEMember

    For Each ie In ieMain
        ie.Clear
        DoEvents
    Next
    
    Set ie = Nothing
End Sub

Private Sub mnuCloseAll_Click()
    Dim ie As IEMember

    For Each ie In ieMain
        PostMessage ie.Handle, WM_CLOSE, 0, 0
        DoEvents
    Next
    
    Set ie = Nothing
End Sub

Private Sub mnuEnabled_Click()
    mnuEnabled.Checked = Not mnuEnabled.Checked
    SetBlocking mnuEnabled.Checked
End Sub

Private Sub mnuExit_Click()
    Unload Me
End Sub

Private Sub stMain_LButtonUp()
    EndMenu
    mnuEnabled_Click
End Sub

Private Sub stMain_RButtonUp()
    GetCursorPos pntTray
    CheckMenuRadioItem hMenu, 0, mnuSensitivity.UBound, iSenseInd, MF_BYPOSITION
    SetForegroundWindow Me.hWnd
    Me.PopupMenu mnuPopup
End Sub

Private Sub ieMain_PopupBlocked()
    If mnuSound.Checked Then
        sndPlaySound sSound, SND_ASYNC Or SND_MEMORY
    End If
            
    If mnuTooltip.Checked Then
        tmrInfo.Enabled = False
        stMain.HideBalloon
        stMain.ShowBalloon "Oops!"
        tmrInfo.Enabled = True
    End If
    
    tmrError.Enabled = True
End Sub

Private Sub SetBlocking(ByVal Flag As Boolean)
    If Flag Then
        stMain.Icon = imgEnabled
        stMain.Tooltip = App.Title & " (enabled)"
        tmrClass.Enabled = True
    Else
        stMain.Icon = imgDisabled
        stMain.Tooltip = App.Title & " (disabled)"
        Set ieMain = Nothing
    End If
End Sub

Private Function IsDebugWindow(ByVal Window As Long) As Boolean
    Dim hOwner As Long
    Dim ie As IEMember
    
    IsDebugWindow = False
    
    hOwner = GetWindow(Window, GW_OWNER)
    
    For Each ie In ieMain
        If hOwner = ie.Handle Then
            IsDebugWindow = True
            Exit For
        End If
    Next
    
    Set ie = Nothing
End Function

Private Sub ShutDown()
    Set ieMain = Nothing
    Set stMain = Nothing
    End
End Sub
