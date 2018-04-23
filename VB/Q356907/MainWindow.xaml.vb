Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.ComponentModel
Imports DevExpress.Data
Imports DevExpress.Xpf.Grid

Namespace Q356907
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Private list As BindingList(Of TestData)
		Private dragStarted As Boolean

		Public Sub New()
			list = TestData.CreateTestData()
			InitializeComponent()
			grid.ItemsSource = list

			AddHandler view.PreviewMouseDown, AddressOf View_PreviewMouseDown
			AddHandler view.PreviewMouseMove, AddressOf View_PreviewMouseMove
			AddHandler view.DragOver, AddressOf View_DragOver
			AddHandler view.Drop, AddressOf View_Drop
		End Sub

		Private Sub View_Drop(ByVal sender As Object, ByVal e As DragEventArgs)
			Dim rowHandle As Integer = CInt(Fix(e.Data.GetData(GetType(Integer))))
			Dim obj As TestData = CType(view.GetNodeByRowHandle(rowHandle).Content, TestData)
			Dim dropRowHandle As Integer = view.GetRowHandleByTreeElement(TryCast(e.OriginalSource, DependencyObject))
			Dim dropObj As TestData = CType(view.GetNodeByRowHandle(dropRowHandle).Content, TestData)

			If IsCopyEffect(e) Then
				Dim newData As New TestData() With {.Text = obj.Text & " (Copy)", .Number = list.Count + 2, .ParentNumber = dropObj.ParentNumber}
				list.Add(newData)
			Else
				obj.ParentNumber = dropObj.ParentNumber
			End If
			grid.RefreshData()
		End Sub

		Private Sub View_DragOver(ByVal sender As Object, ByVal e As DragEventArgs)
			e.Effects = DragDropEffects.None
			e.Handled = True
			Dim rowHandle As Integer = view.GetRowHandleByTreeElement(TryCast(e.OriginalSource, DependencyObject))
			Dim n As TreeListNode = view.GetNodeByRowHandle(rowHandle)
			If n Is Nothing Then
				Return
			End If
			If Not(TypeOf n.Content Is TestData) Then
				Return
			End If
			If (CType(n.Content, TestData)).ParentNumber = 0 Then
				e.Effects = DragDropEffects.None
			Else
				e.Effects = If(IsCopyEffect(e), DragDropEffects.Copy, DragDropEffects.Move)
			End If
		End Sub

		Private Function IsCopyEffect(ByVal e As DragEventArgs) As Boolean
			Return (e.KeyStates And DragDropKeyStates.ControlKey) = DragDropKeyStates.ControlKey
		End Function

		Private Sub View_PreviewMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
			If dragStarted Then
				Dim rowHandle As Integer = view.GetRowHandleByMouseEventArgs(e)
				dragStarted = False
				Dim data As DataObject = CreateDataObject(rowHandle)
				Dim dragSource As DependencyObject = view.GetRowElementByMouseEventArgs(e)
				If dragSource Is Nothing Then
					Return
				End If
				DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move Or DragDropEffects.Copy)
				e.Handled = True
			End If
		End Sub

		Private Sub View_PreviewMouseDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
			If dragStarted Then
				Return
			End If
			Dim rowHandle As Integer = view.GetRowHandleByMouseEventArgs(e)
			Dim n As TreeListNode = view.GetNodeByRowHandle(rowHandle)
			If n Is Nothing OrElse Not(TypeOf n.Content Is TestData) Then
				Return
			End If
			If (CType(n.Content, TestData)).ParentNumber <> 0 Then
				dragStarted = True
				e.Handled = True
			End If
		End Sub

		Private Function CreateDataObject(ByVal rowHandle As Integer) As DataObject
			Dim data As New DataObject()
			data.SetData(GetType(Integer), rowHandle)
			Return data
		End Function
	End Class

	#Region "TestData class"
	Public Class TestData
		Implements INotifyPropertyChanged
		Public Shared Function CreateTestData() As BindingList(Of TestData)
			Dim list As BindingList(Of TestData)
			list = New BindingList(Of TestData)()
			Dim num As Integer = 6
			For i As Integer = 1 To 4
				list.Add(New TestData() With {.Text = "group " & i, .Group = "group " & i, .ParentNumber = 0, .Number = i})
				For j As Integer = 1 To 9
					list.Add(New TestData() With {.Text = "row " & (num - 6).ToString(), .ParentNumber = i, .Number = num})
					num += 1
				Next j
			Next i
			Return list
		End Function
		Private text_Renamed As String
		Private number_Renamed As Integer
		Private group_Renamed As String

		Public Property Text() As String
			Get
				Return text_Renamed
			End Get
			Set(ByVal value As String)
				If Text = value Then
					Return
				End If
				text_Renamed = value
				OnPorpertyChanged("Text")
			End Set
		End Property
		Public Property Number() As Integer
			Get
				Return number_Renamed
			End Get
			Set(ByVal value As Integer)
				If Number = value Then
					Return
				End If
				number_Renamed = value
				OnPorpertyChanged("Number")
			End Set

		End Property
		Private privateParentNumber As Integer
		Public Property ParentNumber() As Integer
			Get
				Return privateParentNumber
			End Get
			Set(ByVal value As Integer)
				privateParentNumber = value
			End Set
		End Property
		Public Property Group() As String
			Get
				Return group_Renamed
			End Get
			Set(ByVal value As String)
				If Group = value Then
					Return
				End If
				group_Renamed = value
				OnPorpertyChanged("Group")
			End Set
		End Property

		Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
		Private Sub OnPorpertyChanged(ByVal propertyName As String)
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
		End Sub
	End Class
	#End Region
End Namespace
