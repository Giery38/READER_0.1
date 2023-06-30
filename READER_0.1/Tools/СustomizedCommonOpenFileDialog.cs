using DevExpress.DirectX.Common;
using DevExpress.Utils.CommonDialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Xml;
using static Microsoft.WindowsAPICodePack.Shell.ShellNativeMethods;

namespace READER_0._1.Tools
{

    //https://github.com/mono/WindowsAPICodePack/blob/46dfc119e5f2208716b0b279ba5d205b794081c7/Shell/Interop/Dialogs/DialogsCOMInterfaces.cs //PopulateWithFileNames вернет файлы до выхода
    public class СustomizedCommonOpenFileDialog
    {

        private CommonOpenFileDialog CommonOpenFileDialogBase;
        private Reflector reflector;
        private object nativeDialog;
        private object customize;
        private IntPtr parentWindow;
        private IntPtr window = IntPtr.Zero;


        public СustomizedCommonOpenFileDialog()
        {
            CommonOpenFileDialogBase = new CommonOpenFileDialog();
            Start();
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PtInRect(RECT lprc, POINT pt);
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr CreateWindowEx(int exstyle, string lpClassName, string lpWindowName, int dwStyle,
  int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool enable);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int zIndex, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("DUIListView.dll")]
        private static extern int GetItemCount(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("shell32.dll")]
        static extern int SHGetFolderPath(
            IntPtr hwndOwner,
            int nFolder,
            IntPtr hToken,
            uint dwFlags,
            [Out] StringBuilder pszPath
            );

        public static string GetLocalizedPath(Environment.SpecialFolder folder)
        {            
            var builder = new StringBuilder();
            SHGetFolderPath(IntPtr.Zero, (int)folder, IntPtr.Zero, 0x0000, builder);
            return builder.ToString();
        }
        private static bool FindWindowByTitle(IntPtr hWnd, IntPtr lParam)
        {
            const int bufferLength = 1024;
            StringBuilder sb = new StringBuilder(bufferLength);
            if (GetWindowText(hWnd, sb, bufferLength) > 0)
            {
                ((List<IntPtr>)GCHandle.FromIntPtr(lParam).Target).Add(hWnd);
            }
            return true;
        }
        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder classNameBuilder = new StringBuilder(256);
            int length = GetClassName(hWnd, classNameBuilder, classNameBuilder.Capacity);
            if (length == 0)
            {
                return null;
            }
            return classNameBuilder.ToString();
        }
        private List<IntPtr> GetAllChildWindows(IntPtr parentWindow)
        {
            List<IntPtr> childWindows = new List<IntPtr>();

            EnumChildProc childProc = (hWnd, lParam) =>
            {
                childWindows.Add(hWnd);
                return true;
            };
            EnumChildWindows(parentWindow, childProc, IntPtr.Zero);
            return childWindows;
        }
        public List<IntPtr> FindWindowsByTitle(string title)
        {
            List<IntPtr> allWindow = new List<IntPtr>();
            List<IntPtr> result = new List<IntPtr>();
            GCHandle handle = GCHandle.Alloc(allWindow);
            try
            {
                EnumWindows(new EnumWindowsProc(FindWindowByTitle), GCHandle.ToIntPtr(handle));
                foreach (IntPtr windowHWnd in allWindow)
                {
                    const int bufferLength = 1024;
                    StringBuilder sb = new StringBuilder(bufferLength);
                    if (GetWindowText(windowHWnd, sb, bufferLength) > 0 && sb.ToString() == title)
                    {
                        result.Add(windowHWnd);
                    }
                }
            }
            finally
            {
                handle.Free();
            }
            return result;
        }
        private void Start()
        {
            reflector = new Reflector("Microsoft.WindowsAPICodePack", "Microsoft.WindowsAPICodePack");
            Type typeCommonFileDialog = typeof(CommonFileDialog);
            nativeDialog = reflector.GetField(typeCommonFileDialog, CommonOpenFileDialogBase, "_nativeDialog");
            if (nativeDialog == null)
            {
                reflector.Call(typeCommonFileDialog, CommonOpenFileDialogBase, "InitializeNativeFileDialog", new object[] { });
                nativeDialog = reflector.Call(typeCommonFileDialog, CommonOpenFileDialogBase, "GetNativeFileDialog", new object[] { });
            }
            reflector.Call(typeCommonFileDialog, CommonOpenFileDialogBase, "GetCustomizedFileDialog", new object[] { });
            customize = reflector.GetField(typeCommonFileDialog, CommonOpenFileDialogBase, "_customize");
            CommonOpenFileDialogBase.FileOk += CommonOpenFileDialog_FileOk;
            CommonOpenFileDialogBase.SelectionChanged += CommonOpenFileDialog_SelectionChanged;
            CommonOpenFileDialogBase.DialogOpening += CommonOpenFileDialog_DialogOpening;
            CommonOpenFileDialogBase.FolderChanging += CommonOpenFileDialog_FolderChanging;
        }

        private void SetWindow()
        {
            if (Title == null)
            {
                Title = "Выбрать";
            }
            List<IntPtr> allWindow = FindWindowsByTitle(Title);
            this.window = allWindow.FirstOrDefault(w => GetParent(w) == this.parentWindow);           
            if (OkButtonLabel == null)
            {
                InitializationOkButtonLabel();
            }
            if (FileNameLabel == null)
            {
                FileNameLabel = "Открыть";
            }
        }

        private string GetFolder()
        {
            Type typeIFileDialog2 = reflector.GetType("Dialogs.IFileOpenDialog");
            object[] args = new object[1];
            reflector.Call(typeIFileDialog2, nativeDialog, "GetFolder", args);
            if (args[0] == null)
            {
                return null;
            }
            string path = IShellItemToString(args[0] as IShellItem);
            return path;
        }
        private IShellItem GetCurrentSelection()
        {
            Type typeIFileDialog = reflector.GetType("Dialogs.IFileOpenDialog");
            object[] args = new object[1];
            reflector.Call(typeIFileDialog, nativeDialog, "GetCurrentSelection", args);
            if (args[0] == null)
            {
                return null;
            }
            return args[0] as IShellItem;
        }             
        private string IShellItemToString(IShellItem shellItem)
        {
            shellItem.GetDisplayName(ShellItemDesignNameOptions.FileSystemPath, out IntPtr pszName);
            string path = Marshal.PtrToStringUni(pszName);

            shellItem.GetDisplayName(ShellItemDesignNameOptions.DesktopAbsoluteEditing, out IntPtr pszName2);
            string path2 = Marshal.PtrToStringUni(pszName2);           

            return path;
        }
        private void SizeRepeating(IntPtr windowParant, IntPtr windowChild)
        {
            GetWindowRect(windowParant, out RECT startSize);
            while (IsWindow(window) == true)
            {
                GetWindowRect(windowParant, out RECT newSize);
                if (newSize.Width != startSize.Width)
                {
                    MoveWindow(windowChild, 0, 0, newSize.Width, newSize.Height, true);
                }
                startSize = newSize;
            }
        }
        private void CopySize(IntPtr windowParant, IntPtr windowChild)
        {
            GetWindowRect(windowParant, out RECT parantSize);
            MoveWindow(windowChild, 0, 0, parantSize.Width, parantSize.Height, true);
        }
        private void OnCustomFolderPicker(object sender, EventArgs e)
        {
            if (IsOKButtonClick() == true)
            {
                Close();
                return;
            }
            CopySize(comboBox, customEdit);
            List<string> selectedFoldersPath = GetSelectedFoldersPath();            
            List<string> fildersName = new List<string>();
            foreach (string folderPath in selectedFoldersPath)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                ShellObject folder = ShellObject.FromParsingName(directoryInfo.FullName);
                string displayedFilderName = folder.GetDisplayName(DisplayNameType.Default);
                if (displayedFilderName != null)
                {
                    fildersName.Add(displayedFilderName);
                }
                else
                {
                    fildersName.Add(directoryInfo.Name);
                }
                  
            }
            FilePaths = selectedFoldersPath;
            string result = string.Join(",", fildersName.Select(name => $"\"{name}\""));
            if (fildersName.Count > 1)
            {
                int lastCommaIndex = result.LastIndexOf(",");
                result = result.Substring(0, lastCommaIndex) + "," + result.Substring(lastCommaIndex + 1);
            }
            if (fildersName.Count == 0)
            {
                result = GetFolder();
                FilePaths = new List<string>() { result };
                result = Path.GetFileName(result);
                result = $"\"{result}\"";
            }
            Type typeFileDialogCustomize = reflector.GetType("Dialogs.IFileDialogCustomize");
            reflector.Call(typeFileDialogCustomize, customize, "SetEditBoxText", new object[] { customEditId, result });
        }

        private void Close()
        {
            CommonFileDialogResultCustom = CommonFileDialogResult.Ok;
            SendMessage(window, 0x0010, 0, IntPtr.Zero);
        }       
        private List<string> GetSelectedFoldersPath()
        {
            List<string> allSelctedItemNames = GetAllSelctedItemNames();
            List<string> selectedFoldersPath = new List<string>();
            string folderPath = GetFolder();
            string filePath = null;           

            GetLocalizedPath(Environment.SpecialFolder.Desktop);

            foreach (string selctedItemName in allSelctedItemNames)
            {
                
                filePath = Path.Combine(folderPath, selctedItemName);
                
                if (Directory.Exists(filePath) == true)
                {                    
                    selectedFoldersPath.Add(filePath);
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                    foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories())
                    {
                        ShellObject folder = ShellObject.FromParsingName(childDirectoryInfo.FullName);
                        if (folder.GetDisplayName(DisplayNameType.Default) == selctedItemName)
                        {                           
                            selectedFoldersPath.Add(childDirectoryInfo.FullName);
                        }
                    }
                }
            }
            return selectedFoldersPath;
        }
        private IntPtr customEdit = IntPtr.Zero;
        private IntPtr comboBox = IntPtr.Zero;
        private int customEditId = 1654;
        private void CreateCustomEdit()
        {
            List<IntPtr> allChildWindows = GetAllChildWindows(window);
            customEdit = allChildWindows.FirstOrDefault(w => GetClassName(w) == "Edit" && AutomationElement.FromHandle(w).Current.Name == "CustomEdit");
            IntPtr oldEdit = allChildWindows.FirstOrDefault(w => GetClassName(w) == "Edit" && AutomationElement.FromHandle(w).Current.Name == FileNameLabel);
            DestroyWindow(oldEdit);
            comboBox = allChildWindows.FirstOrDefault(w => GetClassName(w) == "ComboBoxEx32" && AutomationElement.FromHandle(w).Current.Name == FileNameLabel);
            EnableWindow(comboBox, false);
            SetParent(customEdit, comboBox);
            GetWindowRect(comboBox, out RECT comboBoxRect);
            MoveWindow(customEdit, 0, 0, comboBoxRect.Width, comboBoxRect.Height, true);
            Type typeFileDialogCustomize = reflector.GetType("Dialogs.IFileDialogCustomize");
            reflector.Call(typeFileDialogCustomize, customize, "SetEditBoxText", new object[] { customEditId, "" });
        }
        private void InitializationOkButtonLabel()
        {
            switch (FolderPicker)
            {
                case FolderPickerOption.Standard:
                    OkButtonLabel = "Выбор папки";
                    break;
                case FolderPickerOption.Custom:
                    OkButtonLabel = "Выбор папки";
                    break;
                case FolderPickerOption.None:
                    OkButtonLabel = "Выбор файла";
                    break;
                default:
                    break;
            }
        }
        private bool IsOKButtonClick()
        {
            IntPtr okButton = FindWindowEx(window, IntPtr.Zero, "Button", OkButtonLabel);
            GetWindowRect(okButton, out RECT okButtonRECT);
            GetCursorPos(out POINT mousePosition);
            return PtInRect(okButtonRECT, mousePosition);

        }
        private List<string> GetAllSelctedItemNames() //первая проблема секторы элементов по дате изменения это отдельные объекты, вторая некоторые папки имеют не отображаемое имя
        {
            List<string> allSelctedItemNames = new List<string>();
            List<IntPtr> allChildWindows = GetAllChildWindows(window);
                     
            IntPtr filesList = allChildWindows.FirstOrDefault(w => GetClassName(w) == "SHELLDLL_DefView");
            AutomationElement element = AutomationElement.FromHandle(filesList);           
            AutomationElement listViewContainer = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.List));
            if (listViewContainer != null)
            {
                List<AutomationElement> listViewItems = listViewContainer.FindAll(
                    TreeScope.Children,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ListItem)
                ).Cast<AutomationElement>().ToList();
                if (listViewItems.Count == 0)
                {
                    AutomationElementCollection allChildElements = listViewContainer.FindAll(TreeScope.Children, Condition.TrueCondition);
                    foreach (AutomationElement child in allChildElements)
                    {
                        if (child.Current.ClassName == "UIGroupItem")
                        {
                            AutomationElementCollection groupItems = child.FindAll(TreeScope.Children, Condition.TrueCondition);
                            foreach (AutomationElement item in groupItems)
                            {
                                listViewItems.Add(item);
                            }
                        }                                               
                    }
                }
                foreach (AutomationElement listViewItem in listViewItems)
                {
                    listViewItem.TryGetCurrentPattern(SelectionItemPattern.Pattern, out object tempObject);
                    SelectionItemPattern selectPattern = tempObject as SelectionItemPattern;
                    if (selectPattern == null)
                    {
                        continue;
                    }
                    if (selectPattern.Current.IsSelected == true)
                    {                       
                        allSelctedItemNames.Add(listViewItem.Current.Name);                       
                    }
                }
            }
            return allSelctedItemNames;
        }       
        private void CreateCustomElements()
        {
            CreateCustomEdit();
            new Thread(() => SizeRepeating(comboBox, customEdit)).Start();
            AutomationElement automationElement = AutomationElement.FromHandle(FindWindowEx(window, IntPtr.Zero, "Button", OkButtonLabel));
            var invokePattern = automationElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent, automationElement, TreeScope.Element, (sender, e) =>
            {
                if (filePaths.Count == 0)
                {
                    filePaths.Add(GetFolder());
                }
                Close();
            });
        }
        //Event
        public EventHandler<CancelEventArgs> FileOk;
        private void CommonOpenFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileOk?.Invoke(sender, e);
        }
        public EventHandler SelectionChanged;
        private void CommonOpenFileDialog_SelectionChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }
        public EventHandler DialogOpening;
        private void CommonOpenFileDialog_DialogOpening(object sender, EventArgs e)
        {
            SetWindow();
            if (FolderPicker == FolderPickerOption.Custom)
            {
                CreateCustomElements();
            }
            DialogOpening?.Invoke(sender, e);
        }
        public EventHandler FolderChanging;
        private void CommonOpenFileDialog_FolderChanging(object sender, EventArgs e)
        {
            FolderChanging?.Invoke(sender, e);
        }
        // Properties

        private FolderPickerOption folderPicker = FolderPickerOption.None;
        public FolderPickerOption FolderPicker
        {
            get
            {
                return folderPicker;
            }
            set
            {
                if (value == FolderPickerOption.Standard)
                {
                    SelectionChanged -= OnCustomFolderPicker;
                }
                if (value == FolderPickerOption.Custom)
                {
                    Type typeFileDialogCustomize = reflector.GetType("Dialogs.IFileDialogCustomize");
                    reflector.Call(typeFileDialogCustomize, customize, "AddEditBox", new object[] { customEditId, "CustomEdit" });
                    AddToMostRecentlyUsedList = false;
                    SelectionChanged += OnCustomFolderPicker;
                    FolderChanging += OnCustomFolderPicker;
                    CommonOpenFileDialogBase.FileOk += (sender, e) =>
                    {
                        e.Cancel = true;
                    };
                }
                folderPicker = value;

            }
        }
        private string okButtonLabel;
        public string OkButtonLabel
        {
            get
            {
                return okButtonLabel;
            }
            set
            {
                if (value != okButtonLabel)
                {
                    Type typeIFileDialog = reflector.GetType("Dialogs.IFileDialog");
                    reflector.Call(typeIFileDialog, nativeDialog, "SetOkButtonLabel", new object[] { value });
                    okButtonLabel = value;
                }
            }
        }
        private string fileNameLabel;
        public string FileNameLabel
        {
            get
            {
                return fileNameLabel;
            }
            set
            {
                if (value != fileNameLabel)
                {
                    Type typeIFileDialog = reflector.GetType("Dialogs.IFileDialog");
                    reflector.Call(typeIFileDialog, nativeDialog, "SetFileNameLabel", new object[] { value });
                    fileNameLabel = value;
                }
            }
        }
        public string Title
        {
            get { return CommonOpenFileDialogBase.Title; }
            set { CommonOpenFileDialogBase.Title = value; }
        }
        public bool AddToMostRecentlyUsedList
        {
            get { return CommonOpenFileDialogBase.AddToMostRecentlyUsedList; }
            set { CommonOpenFileDialogBase.AddToMostRecentlyUsedList = value; }
        }

        public bool AllowNonFileSystemItems
        {
            get { return CommonOpenFileDialogBase.AllowNonFileSystemItems; }
            set { CommonOpenFileDialogBase.AllowNonFileSystemItems = value; }
        }

        public bool AllowPropertyEditing
        {
            get { return CommonOpenFileDialogBase.AllowPropertyEditing; }
            set { CommonOpenFileDialogBase.AllowPropertyEditing = value; }
        }

        public string DefaultDirectory
        {
            get { return CommonOpenFileDialogBase.DefaultDirectory; }
            set { CommonOpenFileDialogBase.DefaultDirectory = value; }
        }

        public string DefaultExtension
        {
            get { return CommonOpenFileDialogBase.DefaultExtension; }
            set { CommonOpenFileDialogBase.DefaultExtension = value; }
        }

        public bool EnsureFileExists
        {
            get { return CommonOpenFileDialogBase.EnsureFileExists; }
            set { CommonOpenFileDialogBase.EnsureFileExists = value; }
        }

        public bool EnsurePathExists
        {
            get { return CommonOpenFileDialogBase.EnsurePathExists; }
            set { CommonOpenFileDialogBase.EnsurePathExists = value; }
        }

        public string FilePath
        {
            get
            {
                if (FilePaths.Count > 0)
                {
                    return FilePaths[0];
                }
                return null;
            }
        }
        private List<string> filePaths;
        public List<string> FilePaths
        {
            get
            {
                if (filePaths?.Count > 0 && CommonFileDialogResultCustom == CommonFileDialogResult.Ok)
                {
                    return filePaths;
                }
                else if (commonFileDialogBaseResult == CommonFileDialogResult.Ok && CommonFileDialogResultCustom != CommonFileDialogResult.Ok)
                {
                    return CommonOpenFileDialogBase.FileNames.ToList();
                }
                return new List<string>();
            }
            private set
            {
                filePaths = value;
            }
        }

        public CommonFileDialogResult CommonFileDialogResultCustom { get; private set; } = CommonFileDialogResult.None;
        private CommonFileDialogResult commonFileDialogBaseResult = CommonFileDialogResult.None;

        public CommonFileDialogFilterCollection Filters
        {
            get { return CommonOpenFileDialogBase.Filters; }
        }

        public bool Multiselect
        {
            get { return CommonOpenFileDialogBase.Multiselect; }
            set { CommonOpenFileDialogBase.Multiselect = value; }
        }

        // Methods

        public void SetTitle(string title)
        {
            CommonOpenFileDialogBase.Title = title;
        }
        public void Dispose()
        {
            CommonOpenFileDialogBase.Dispose();
        }
        public CommonFileDialogResult ShowDialog()
        {            
            WindowInteropHelper helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            IntPtr  hWnd = helper.Handle;
            return ShowDialog(hWnd);

        }
        public CommonFileDialogResult ShowDialog(IntPtr hwndOwner)
        {
            SetElementsLabel();
            parentWindow = hwndOwner;
            commonFileDialogBaseResult = CommonOpenFileDialogBase.ShowDialog(hwndOwner);
            if (CommonFileDialogResultCustom != CommonFileDialogResult.None)
            {
                return CommonFileDialogResultCustom;
            }
            return commonFileDialogBaseResult;
        }

        private void SetElementsLabel()
        {
            if (Title == null)
            {
                Title = "Выбрать";
            }
            if (OkButtonLabel == null)
            {
                InitializationOkButtonLabel();
            }
            if (FileNameLabel == null)
            {
                FileNameLabel = "Открыть";
            }
        }      
    }
    public enum FolderPickerOption
    {
        Standard,
        Custom,
        None
    }

}
