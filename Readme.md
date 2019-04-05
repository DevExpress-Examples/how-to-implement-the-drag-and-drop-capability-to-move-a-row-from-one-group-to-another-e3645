<!-- default file list -->
*Files to look at*:

* [MainWindow.xaml](./CS/Q356907/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/Q356907/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/Q356907/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/Q356907/MainWindow.xaml.vb))
<!-- default file list end -->
# How to implement the drag-and-drop capability to move a row from one group to another


<p>The following example demonstrates how to implement the drag-and-drop capability, to enable end-users to drag any tree list row from one group and drop it to another group, thus changing it.</p><br />
<p>To accomplish this task, it is necessary to set the TreeListControl.AllowDrop property to True, and handle its PreviewMouseDown, PreviewMouseMove, DragOver and Drop events, as shown in this example.</p>

<br/>


