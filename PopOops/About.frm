VERSION 5.00
Begin VB.Form frmAbout 
   BorderStyle     =   3  'Fixed Dialog
   Caption         =   "About"
   ClientHeight    =   2415
   ClientLeft      =   45
   ClientTop       =   330
   ClientWidth     =   3855
   Icon            =   "About.frx":0000
   LinkTopic       =   "Form1"
   LockControls    =   -1  'True
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   2415
   ScaleWidth      =   3855
   ShowInTaskbar   =   0   'False
   Visible         =   0   'False
   Begin VB.CommandButton cmdUpdate 
      Caption         =   "Update"
      Height          =   375
      Left            =   2790
      TabIndex        =   11
      Top             =   1080
      Width           =   930
   End
   Begin PopOops.XPFrame xpfAbout 
      Height          =   2295
      Left            =   120
      TabIndex        =   2
      Top             =   0
      Width           =   2535
      _ExtentX        =   4471
      _ExtentY        =   4048
      Caption         =   ""
      Begin VB.Label lblVisit 
         AutoSize        =   -1  'True
         BackStyle       =   0  'Transparent
         Caption         =   "www.gasanov.net"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   177
            Weight          =   400
            Underline       =   -1  'True
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H00C00000&
         Height          =   195
         Left            =   720
         MouseIcon       =   "About.frx":058A
         MousePointer    =   99  'Custom
         TabIndex        =   8
         Top             =   1680
         Width           =   1290
      End
      Begin VB.Image imgEnabled 
         Height          =   480
         Left            =   120
         Picture         =   "About.frx":06DC
         Top             =   240
         Width           =   480
      End
      Begin VB.Label lblTitle 
         AutoSize        =   -1  'True
         BackStyle       =   0  'Transparent
         Caption         =   "Title"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   177
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   195
         Left            =   720
         TabIndex        =   3
         Top             =   240
         Width           =   390
      End
      Begin VB.Label lblComment 
         AutoSize        =   -1  'True
         BackStyle       =   0  'Transparent
         Caption         =   "Comment"
         Height          =   195
         Left            =   720
         TabIndex        =   4
         Top             =   480
         Width           =   660
      End
      Begin VB.Line Line1 
         X1              =   120
         X2              =   2400
         Y1              =   1560
         Y2              =   1560
      End
      Begin VB.Line Line2 
         X1              =   120
         X2              =   2400
         Y1              =   840
         Y2              =   840
      End
      Begin VB.Label lblSend 
         AutoSize        =   -1  'True
         BackStyle       =   0  'Transparent
         Caption         =   "shahin@gasanov.net"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   177
            Weight          =   400
            Underline       =   -1  'True
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         ForeColor       =   &H00C00000&
         Height          =   195
         Left            =   720
         MouseIcon       =   "About.frx":0FA6
         MousePointer    =   99  'Custom
         TabIndex        =   10
         Top             =   1920
         Width           =   1515
      End
      Begin VB.Label lblCopyright 
         AutoSize        =   -1  'True
         Caption         =   "Copyright"
         Height          =   195
         Left            =   120
         TabIndex        =   6
         Top             =   1200
         Width           =   660
      End
      Begin VB.Label lblCompany 
         AutoSize        =   -1  'True
         BackStyle       =   0  'Transparent
         Caption         =   "Company"
         BeginProperty Font 
            Name            =   "MS Sans Serif"
            Size            =   8.25
            Charset         =   177
            Weight          =   700
            Underline       =   0   'False
            Italic          =   0   'False
            Strikethrough   =   0   'False
         EndProperty
         Height          =   195
         Left            =   120
         TabIndex        =   5
         Top             =   960
         Width           =   780
      End
      Begin VB.Label lblEmail 
         AutoSize        =   -1  'True
         Caption         =   "E-mail:"
         Height          =   195
         Left            =   120
         TabIndex        =   9
         Top             =   1920
         Width           =   465
      End
      Begin VB.Label lblWeb 
         AutoSize        =   -1  'True
         Caption         =   "Web:"
         Height          =   195
         Left            =   120
         TabIndex        =   7
         Top             =   1680
         Width           =   390
      End
   End
   Begin VB.CommandButton cmdHelp 
      Caption         =   "Help"
      Height          =   375
      Left            =   2790
      TabIndex        =   1
      Top             =   585
      Width           =   930
   End
   Begin VB.CommandButton cmdOK 
      Caption         =   "OK"
      Default         =   -1  'True
      Height          =   375
      Left            =   2790
      TabIndex        =   0
      Top             =   90
      Width           =   930
   End
   Begin VB.Image imgDonate 
      Height          =   465
      Left            =   2790
      MouseIcon       =   "About.frx":10F8
      MousePointer    =   99  'Custom
      Picture         =   "About.frx":124A
      Top             =   1830
      Width           =   930
   End
End
Attribute VB_Name = "frmAbout"
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

Private rctTray As RECT, rctForm As RECT

Private Sub Form_Load()
    With App
        lblTitle.Caption = .Title & " " & .Major & "." & .Minor & "." & .Revision
        lblComment.Caption = .Comments
        lblCompany.Caption = .CompanyName
        lblCopyright.Caption = .LegalCopyright
    End With
    
    Me.Move (Screen.Width - Me.Width) / 2, (Screen.Height - Me.Height) / 2
    
    CalculateTrayRect
    GetWindowRect Me.hWnd, rctForm
    DrawAnimatedRects Me.hWnd, IDANI_OPEN Or IDANI_CAPTION, rctTray, rctForm

    InitCommonControls
End Sub

Private Sub Form_Unload(Cancel As Integer)
    CalculateTrayRect
    GetWindowRect Me.hWnd, rctForm
    DrawAnimatedRects Me.hWnd, IDANI_CLOSE Or IDANI_CAPTION, rctForm, rctTray
End Sub

Private Sub lblVisit_Click()
    ShellExecute Me.hWnd, vbNullString, "http://" & lblVisit.Caption & "/" & App.Title & ".htm", vbNullString, vbNullString, SW_SHOWNORMAL
End Sub

Private Sub lblSend_Click()
    ShellExecute Me.hWnd, vbNullString, "mailto:" & lblSend.Caption & "?subject=" & lblTitle.Caption, vbNullString, vbNullString, SW_SHOWNORMAL
End Sub

Private Sub cmdOK_Click()
    Unload Me
End Sub

Private Sub cmdHelp_Click()
    Dim hWnd As Long
    
    hWnd = FindWindow("HH Parent", App.Title & " Help")
    
    If hWnd > 0 Then
        If IsIconic(hWnd) Then
            ShowWindow hWnd, SW_SHOWNORMAL
        End If
        SetForegroundWindow hWnd
    Else
        ShellExecute Me.hWnd, vbNullString, App.HelpFile, vbNullString, vbNullString, SW_SHOWNORMAL
    End If
End Sub

Private Sub cmdUpdate_Click()
    On Error GoTo ErrHand
    
    Dim xmlDoc As New DOMDocument
    Dim xmlRoot As IXMLDOMElement, xmlNode As IXMLDOMElement
    Dim sVersion As String, sDownload As String
    Dim vTokens As Variant
    
    cmdUpdate.Enabled = False
    
    xmlDoc.async = False
    
    If xmlDoc.Load("http://" & lblVisit.Caption & "/Update.xml") Then
        Set xmlRoot = xmlDoc.documentElement
        Set xmlNode = xmlDoc.getElementsByTagName(App.Title).Item(0)
        
        If xmlNode Is Nothing Then
            Err.Raise 0
        Else
            sVersion = xmlNode.getAttribute("Version")
        End If
        
        vTokens = Split(sVersion, ".")
        If App.Major = vTokens(0) And App.Minor = vTokens(1) And App.Revision = vTokens(2) Then
            MsgBox "You already have the latest version.", vbInformation
        Else
            If MsgBox("Version " & sVersion & " is available for download now." & vbCrLf & "Do you want to visit " & App.Title & " Web page?", vbQuestion Or vbYesNo) = vbYes Then
                ShellExecute Me.hWnd, vbNullString, "http://" & lblVisit.Caption & "/" & App.Title & ".htm", vbNullString, vbNullString, SW_SHOWNORMAL
            End If
        End If
    End If
    
    cmdUpdate.Enabled = True
    
    Set xmlNode = Nothing
    Set xmlRoot = Nothing
    Set xmlDoc = Nothing
    
    Exit Sub
ErrHand:
    Err.Clear
    MsgBox "Sorry, an error occured during update!", vbExclamation
    cmdUpdate.Enabled = True
End Sub

Private Sub imgDonate_Click()
    ShellExecute Me.hWnd, vbNullString, "http://" & lblVisit.Caption & "/Donation.htm", vbNullString, vbNullString, SW_SHOWNORMAL
End Sub

Private Sub CalculateTrayRect()
    With rctTray
        .Left = pntTray.X
        .Top = pntTray.Y
        .Right = pntTray.X
        .Bottom = pntTray.Y
    End With
End Sub
