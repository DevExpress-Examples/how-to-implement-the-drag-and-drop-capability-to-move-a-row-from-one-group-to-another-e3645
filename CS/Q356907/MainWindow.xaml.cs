using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Xpf.Grid;

namespace Q356907 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        BindingList<TestData> list;
        bool dragStarted;

        public MainWindow() {
            list = TestData.CreateTestData();
            InitializeComponent();
            grid.ItemsSource = list;

            view.PreviewMouseDown += new MouseButtonEventHandler(View_PreviewMouseDown);
            view.PreviewMouseMove += new MouseEventHandler(View_PreviewMouseMove);
            view.DragOver += new DragEventHandler(View_DragOver);
            view.Drop += new DragEventHandler(View_Drop);
        }

        void View_Drop(object sender, DragEventArgs e) {
            int rowHandle = (int)e.Data.GetData(typeof(int));
            TestData obj = (TestData)view.GetNodeByRowHandle(rowHandle).Content;
            int dropRowHandle = view.GetRowHandleByTreeElement(e.OriginalSource as DependencyObject);
            TestData dropObj = (TestData)view.GetNodeByRowHandle(dropRowHandle).Content;

            if(IsCopyEffect(e)) {
                TestData newData =
                    new TestData() {
                        Text = obj.Text + " (Copy)",
                        Number = list.Count + 2,
                        ParentNumber = dropObj.ParentNumber,
                    };
                list.Add(newData);
            } else {
                obj.ParentNumber = dropObj.ParentNumber;
            }
            grid.RefreshData();
        }

        void View_DragOver(object sender, DragEventArgs e) {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            int rowHandle = view.GetRowHandleByTreeElement(e.OriginalSource as DependencyObject);
            TreeListNode n = view.GetNodeByRowHandle(rowHandle);
            if(n == null) return;
            if(!(n.Content is TestData)) return;
            if(((TestData)n.Content).ParentNumber == 0)
                e.Effects = DragDropEffects.None;
            else e.Effects = IsCopyEffect(e) ? DragDropEffects.Copy : DragDropEffects.Move;
        }

        bool IsCopyEffect(DragEventArgs e) {
            return (e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey;
        }

        void View_PreviewMouseMove(object sender, MouseEventArgs e) {
            if(dragStarted) {
                int rowHandle = view.GetRowHandleByMouseEventArgs(e);
                dragStarted = false;
                DataObject data = CreateDataObject(rowHandle);
                DependencyObject dragSource = view.GetRowElementByMouseEventArgs(e);
                if(dragSource == null) return;
                DragDrop.DoDragDrop(dragSource, data, DragDropEffects.Move | DragDropEffects.Copy);
                e.Handled = true;
            }
        }

        void View_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if(dragStarted) return;
            int rowHandle = view.GetRowHandleByMouseEventArgs(e);
            TreeListNode n = view.GetNodeByRowHandle(rowHandle);
            if(n == null || !(n.Content is TestData)) return;
            if(((TestData)n.Content).ParentNumber != 0) {
                dragStarted = true;
                e.Handled = true;
            }
        }

        private DataObject CreateDataObject(int rowHandle) {
            DataObject data = new DataObject();
            data.SetData(typeof(int), rowHandle);
            return data;
        }
    }

    #region TestData class
    public class TestData : INotifyPropertyChanged {
        public static BindingList<TestData> CreateTestData() {
            BindingList<TestData> list;
            list = new BindingList<TestData>();
            int num = 6;
            for(int i = 1; i < 5; i++) {
                list.Add(new TestData() {
                    Text = "group " + i,
                    Group = "group " + i,
                    ParentNumber = 0,
                    Number = i,
                });
                for(int j = 1; j < 10; j++) {
                    list.Add(new TestData() {
                        Text = "row " + (num - 6).ToString(),
                        ParentNumber = i,
                        Number = num,
                    });
                    num++;
                }
            }
            return list;
        }
        string text;
        int number;
        string group;

        public string Text {
            get { return text; }
            set {
                if(Text == value)
                    return;
                text = value;
                OnPorpertyChanged("Text");
            }
        }
        public int Number {
            get { return number; }
            set {
                if(Number == value)
                    return;
                number = value;
                OnPorpertyChanged("Number");
            }

        }
        public int ParentNumber { get; set; }
        public string Group {
            get { return group; }
            set {
                if(Group == value)
                    return;
                group = value;
                OnPorpertyChanged("Group");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPorpertyChanged(string propertyName) {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}
