using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

using Ligg.EasyWinApp.WinForm.Controls;
using Ligg.EasyWinApp.WinForm.Controls.ShadowPanel;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;
using Ligg.EasyWinApp.WinForm.Helpers;
using Ligg.EasyWinApp.WinForm.Resources;

using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser;
using Ligg.EasyWinApp.Parser.Helpers;

using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.Handlers;
using Ligg.Infrastructure.Utility.FileWrap;
using ContentAlignment = System.Drawing.ContentAlignment;


namespace Ligg.EasyWinApp.WinForm.Forms
{
    //*start
    public partial class FunctionForm : FrameForm
    {
        public bool BoolValue = true;

        private FormInitParamSet _functionInitParamSet;
        public FormInitParamSet FunctionInitParamSet
        {
            get => _functionInitParamSet;
            set => _functionInitParamSet = value;
        }

        private List<Procedure> _procedures = new List<Procedure>();
        private List<Annex> _annexes = new List<Annex>();
        private List<LayoutElement> _layoutElements = new List<LayoutElement>();
        private List<ZoneItem> _zonesItems = new List<ZoneItem>();

        private List<ViewFeature> _viewFeatures = new List<ViewFeature>();
        private List<MenuFeature> _menuFeatures = new List<MenuFeature>();
        private int _currentNestedMenuId { get; set; }
        private string _currentViewName { get; set; }
        private List<string> _renderedViewNames = new List<string>();

        private bool _hasTray;
        NotifyIcon _tray;
        ContextMenuStripEx _trayContextMenuStrip = null;

        private FormWindowState _ordinaryWindowStatus = FormWindowState.Normal;
        private readonly ToolTip _pictureBoxToolTip = new ToolTip();

        private ManagedThreadPool _threadPool;

        private string _basicInfoForException;
        private string _additionalInfoForException;

        private string _startupDir;
        private string _cfgDir;
        private string _cfgUiDir;
        private string _cfgStyleSheetsDir;

        private string _appCfgDir;
        private string _appCfgUiDir;
        private string _functionsDir;
        private string _functionDir;
        private string _zonesDir;

        private string _formCfgDir;


        private string _appLibDir;
        private string _appDataDir;
        private string _appTempDir;

        private string _sysDataDir;
        private string _sysAppDataDir;
        private string _myDataDir;
        private string _myAppDataDir;
        private string _myAppZoneDataDir;

        public FunctionForm()
        {
            InitializeComponent();
            ToolBarSectionPublicRegionToolStrip.Enabled = true;
        }

        //*proc
        //**load
        //##FunctionForm_Load
        private void FunctionForm_Load(object sender, EventArgs e)
        {
            try
            {
                _startupDir = Directory.GetCurrentDirectory();
                _cfgDir = _startupDir + "\\Conf";
                _cfgUiDir = _cfgDir + "\\UIs\\WinForm";
                _cfgStyleSheetsDir = _cfgUiDir + "\\StyleSheets";

                _appCfgDir = _cfgDir + "\\Apps\\" + _functionInitParamSet.ApplicationCode;
                _appCfgUiDir = _appCfgDir + "\\UIs\\WinForm";
                _functionsDir = _appCfgUiDir + "\\Functions";
                _functionDir = _functionsDir + "\\" + _functionInitParamSet.FunctionCode;
                _zonesDir = _appCfgUiDir + "\\Zones";

                _formCfgDir = _functionDir;
                if (_functionInitParamSet.FormType == FormType.SviOfZone)
                {
                    _formCfgDir = FileHelper.GetFilePath(_functionInitParamSet.StartSviZoneRelativeLocation, _zonesDir);
                    _functionInitParamSet.FunctionCode = FileHelper.GetFileDetailByOption(_formCfgDir, FilePathComposition.FileTitle);
                }
                if (_functionInitParamSet.FormType == FormType.SviOfView)
                {
                    var sviViewsDir = _appCfgUiDir + "\\Views";
                    _formCfgDir = FileHelper.GetFilePathByRelativeLocation(_functionInitParamSet.StartSviViewRelativeLocation, sviViewsDir);
                    _functionInitParamSet.FunctionCode = FileHelper.GetFileDetailByOption(_formCfgDir, FilePathComposition.FileTitle);
                }

                _appDataDir = _functionInitParamSet.ApplicationDataDir;
                _appLibDir = _functionInitParamSet.ApplicationLibDir;
                _appTempDir = _functionInitParamSet.ApplicationTempDir;
                _sysDataDir = DirectoryHelper.GetSpecialDir("commonapplicationdata") + "\\" + _functionInitParamSet.ArchitectureCode;
                _sysAppDataDir = _sysDataDir + "\\Apps\\" + _functionInitParamSet.ApplicationCode;

                _myDataDir = DirectoryHelper.GetSpecialDir("mydocuments") + "\\" + _functionInitParamSet.ArchitectureCode;
                _myAppDataDir = _myDataDir + "\\Apps\\" + _functionInitParamSet.ApplicationCode;
                _myAppZoneDataDir = _myAppDataDir + "Zones";

                var rltvLoc = _functionInitParamSet.FormType == FormType.Mvi ? _functionInitParamSet.FunctionCode :
                    (_functionInitParamSet.FormType == FormType.SviOfView ? "SviViews\\" + _functionInitParamSet.StartSviViewRelativeLocation : ("zones\\" + _functionInitParamSet.StartSviZoneRelativeLocation));
                _basicInfoForException = "Function: " + rltvLoc;

                var userCode = GetCenterData("UserCode");
                _additionalInfoForException = "Architecture: " + _functionInitParamSet.ArchitectureCode + "-" + _functionInitParamSet.ArchitectureVersion + "; " +
                    "Application: " + _functionInitParamSet.ApplicationCode + "-" + _functionInitParamSet.ApplicationVersion + "; " +
                    (userCode.IsNullOrEmpty() ? "" : "; UserCode: " + userCode) + "\n--" +
                    "Please send this error information to: " + _functionInitParamSet.HelpdeskEmail + "\n";

                SetFrameTextByCulture(true, _functionInitParamSet.SupportMultiCultures);
                LoadForm();

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + " Load Error", ex.Message, GetAdditionalInfoForException());

                if (!this.Modal) Application.Exit(); else CloseForm();
            }
        }

        private void FunctionForm_Resize(object sender, EventArgs e)
        {
            try
            {
                ResizeFrameComponent();
                if (_hasTray == true & WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + ".FunctionForm_Resize" + " Error", ex.Message, GetAdditionalInfoForException());
            }
        }

        private void tray_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    ShowForm();
                }
                else if (WindowState == _ordinaryWindowStatus)
                {
                    this.Visible = true;
                    this.Activate();
                }
            }
        }

        private void ToolBarSectionPublicRegionToolStripSplitButtonCultureItemClickHandler(object sender, EventArgs e)
        {
            try
            {
                var ctrl = sender as ToolStripMenuItem;
                var culName = Convert.ToString(ctrl.Tag);
                if (culName != CultureHelper.CurrentCultureName & !culName.IsNullOrEmpty())
                {
                    CultureHelper.SetCurrentCulture(culName);
                    var splitButtonCultures = ToolBarSectionPublicRegionToolStrip.Items.Find("ToolBarSectionPublicRegionToolStripSplitButtonCulture", true);
                    if (splitButtonCultures.Count() > 0)
                    {
                        var splitButtonCulture = splitButtonCultures[0];
                        splitButtonCulture.Text = CultureHelper.CurrentLanguageName;
                        splitButtonCulture.Tag = culName;
                        splitButtonCulture.ToolTipText = WinformRes.ChooseLanguage;
                    }

                    SetFrameTextByCulture(false, _functionInitParamSet.SupportMultiCultures);
                    SetLayoutTextByCulture();
                }
            }
            catch (Exception ex)
            {
                var methodName = "ToolBarSectionPublicRegionToolStripSplitButtonCultureItemClickHandler";
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + "." + methodName + " Error", ex.Message, GetAdditionalInfoForException());
            }
        }


        //*event
        //##MenuItemClickHandler
        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            try
            {
                var type = sender.GetType().ToString();
                if (type.ToLower().EndsWith("PictureBox".ToLower())) ctrlName = (sender as PictureBox).Name;
                else if (type.ToLower().EndsWith("ToolStripButton".ToLower())) ctrlName = (sender as ToolStripButton).Name;//nestedMenu
                else if (type.ToLower().EndsWith("Button".ToLower())) ctrlName = (sender as Button).Name;//nestedMenu under panel
                else if (type.ToLower().EndsWith("ToolStripMenuItem".ToLower())) ctrlName = (sender as ToolStripMenuItem).Name; //horMenu
                else if (type.ToLower().EndsWith("VerticalMenu".ToLower())) ctrlName = (sender as VerticalMenu).Name;//VerMenu
                else throw new ArgumentException("Control type: " + type + " didn't be considered!");

                var menuItem = new LayoutElement();
                if (type.ToLower().EndsWith("VerticalMenu".ToLower()))
                {
                    var val = (sender as VerticalMenu).Value;
                    menuItem = _layoutElements.Find(x => x.Name == val);
                }
                else
                {
                    menuItem = _layoutElements.Find(x => x.Name == ctrlName);
                }
                var menuFeature = _menuFeatures.Find(x => x.Id == menuItem.LayoutId);

                if (menuFeature.MenuType == (int)MenuType.Horizontal | menuFeature.MenuType == (int)MenuType.Vertical)
                {
                    if (!menuItem.View.IsNullOrEmpty())
                    {
                        SwitchView(menuItem.View);
                    }
                    if (!menuItem.Action.IsNullOrEmpty())
                    {
                        ActByUiElementAndProcedure(menuItem.Name.AddUiIdentifer());
                    }
                }
                else if (menuFeature.MenuType == (int)MenuType.Nested)
                {
                    if (!menuItem.View.IsNullOrEmpty() | !menuItem.IsLastLevel)
                    {
                        if (!menuItem.IsChecked)
                        {
                            var menuArea = _layoutElements.Find(x =>
                                x.Name == menuItem.Container && x.Type == (int)LayoutElementType.MenuItemContainerArea && x.LayoutType == (int)LayoutType.Menu);
                            var lastCheckedParallelMenuItem = _layoutElements.Find(x =>
                                x.Container == menuArea.Name && x.Type == (int)LayoutElementType.MenuItem && x.LayoutType == (int)LayoutType.Menu && x.IsChecked);
                            HideNestedMenuAreas(lastCheckedParallelMenuItem.Id);
                            CheckNestedMenuItemAndUncheckParallelItems(menuItem.Id);
                            UpdateNestedMenu(menuItem.Id, menuItem.LayoutId);
                        }
                    }

                    if (!menuItem.Action.IsNullOrEmpty())
                    {
                        ActByUiElementAndProcedure(menuItem.Name.AddUiIdentifer());
                    }
                }
                else if (menuFeature.MenuType == (int)MenuType.ToolBar)
                {
                    if (!menuItem.Action.IsNullOrEmpty())
                    {
                        ActByUiElementAndProcedure(menuItem.Name.AddUiIdentifer());
                    }
                }

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + ".MenuItemClickHandler" + " Error: ctrlName='" + ctrlName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }
        private void SubMenuItemClickHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            var transaction = new Transaction();
            try
            {
                var type = sender.GetType().ToString();
                if (type.ToLower().EndsWith("ToolStripSplitButtonEx".ToLower()))//--Nested or Tool Menu 
                {
                    var cpnt = sender as ToolStripSplitButtonEx;
                    ctrlName = (cpnt).Name;
                    transaction = cpnt.CurrentTransaction;
                }
                else if (type.ToLower().EndsWith("ContextMenuStripEx".ToLower()))//--tray
                {
                    var cpnt = sender as ContextMenuStripEx;
                    ctrlName = (cpnt).Name;
                    transaction = cpnt.CurrentTransaction;
                }
                else throw new ArgumentException("Control type didn't be considered!");
                ActByTransaction(transaction);
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + ".SubMenuItemClickHandler" + " Error: ctrlName='" + ctrlName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }

        //##ContextMenuItemClickHandler
        private void ContextMenuItemClickHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            var transaction = new Transaction();
            try
            {
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + "." + "ContextMenuItemClickHandler" + " Error: ctrlName='" + ctrlName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }

        //##ZoneEventHandler
        private void ZoneEventHandler(string zoneName, ZoneItemType eventHandlerType)
        {
            try
            {
                var eventHandlers = _zonesItems.FindAll(x => x.Name.StartsWith(zoneName + "_") && x.Type == (int)eventHandlerType);
                foreach (var eventHandler in eventHandlers)
                {
                    var eventHandlerDisplayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", eventHandler.Name, _annexes, eventHandler.DisplayName);
                    ActByUiElementAndProcedure(eventHandler.Name.AddUiIdentifer());
                }
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + ".ZoneEventHandler" + " Error: zoneName='" + zoneName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }

        //##ControlEventHandler
        private void ControlEventHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            try
            {
                var ctrl = sender as Control;
                var type = sender.GetType().ToString();
                ctrlName = ctrl.Name;
                if (ctrlName.GetQtyOfIncludedChar('_') < 2) //menuitem==0;viewItem=1
                {
                    var item = _layoutElements.Find(x => x.Name == ctrlName);
                    if (!string.IsNullOrEmpty(item.Action))
                        ActByUiElementAndProcedure(item.Name.AddUiIdentifer());
                }
                else if (ctrlName.GetQtyOfIncludedChar('_') == 2)//zone item
                {
                    var item = _zonesItems.Find(x => x.Name == ctrlName);
                    if (!string.IsNullOrEmpty(item.Action))
                        ActByUiElementAndProcedure(item.Name.AddUiIdentifer());
                }

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + "." + "ControlEventHandler" + " Error: ctrlName='" + ctrlName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }

        //##ControlEventHandler1
        private void ControlEventHandler1(object sender, EventArgs e)
        {
            var ctrlName = "";
            try
            {
                var ctrl = sender as Control;
                var type = sender.GetType().ToString();
                //to be improved
                if (type.ToLower().EndsWith("ToolStripMenuItem".ToLower())) ctrlName = (sender as ToolStripMenuItem).Name;
                else if (type.ToLower().EndsWith("ToolStripButton".ToLower())) ctrlName = (sender as ToolStripButton).Name;
                else
                {
                    ctrlName = ctrl.Name;
                }
                var action = "";
                if (ctrlName.GetQtyOfIncludedChar('_') == 2)//zone item
                {
                    var item = _zonesItems.Find(x => x.Name == ctrlName);
                    if (!string.IsNullOrEmpty(item.Action1))
                    {
                        ActByUiElementAndProcedure(item.Name.AddUiIdentifer(), 1);
                    }

                }



            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + ". ControlEventHandler Error: ctrlName='" + ctrlName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }

        //##ControlHoverHandler
        private void ControlHoverHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            try
            {
                var type = sender.GetType().ToString();
                if (type.ToLower().EndsWith("PictureBox".ToLower()))
                {
                    var cpnt = sender as PictureBox;
                    var ctrlTag = cpnt.Tag.ToString();
                    ctrlName = cpnt.Name;
                    _pictureBoxToolTip.SetToolTip(cpnt, ctrlTag);
                }
                else
                {
                    throw new ArgumentException("Control type didn't be considered to trigger ControlHoverHandler!");
                }
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ": " + GetType().FullName + ".ControlHoverHandler Error: ctrlName='" + ctrlName + "'", ex.Message, GetAdditionalInfoForException());
            }
        }


        //*func
        //**form
        //##LoadForm
        private void LoadForm()
        {
            try
            {
                _annexes.AddRange(FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\AbbrevAnnexes", "Abbrev", false));
                _annexes.AddRange(FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\PhraseAnnexes", "Phrase", false));

                var annexes = FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\ApplicationAnnexes", "", false);
                FunctionHelper.SetApplicationAnnexes(annexes, _functionInitParamSet.ApplicationCode);
                _annexes.AddRange(annexes);

                var annexes1 = FunctionHelper.GetAnnexesFromCfgFile(_formCfgDir + "\\FormTitleAnnexes", "", false);
                FunctionHelper.SetFormTitleAnnexes(annexes1, _functionInitParamSet.FunctionCode);
                _annexes.AddRange(annexes1);

                var supportsMultipleThreads = false;
                var threadPoolMaxNum = 0;
                if (_functionInitParamSet.FormType == FormType.Mvi)
                {
                    var formStyle = FunctionHelper.GetGenericFromCfgFile<FormStyle>(_functionDir + "\\FormStyle", true) ?? new FormStyle();
                    supportsMultipleThreads = formStyle.SupportsMultipleThreads;
                    threadPoolMaxNum = formStyle.ThreadPoolMaxNum;

                    InitLayout(_functionInitParamSet.FormType, formStyle);
                    ResizeForm(formStyle.ResizeParamsText);
                    if (_functionInitParamSet.SupportMultiCultures) InitToolBarSectionPublicRegionComponents();

                    var viewFeatures = FunctionHelper.GetGenericFromCfgFile<List<ViewFeature>>(FileHelper.GetFilePathByRelativeLocation("\\ViewFeatures", _functionDir), true) ?? new List<ViewFeature>();
                    UiHelper.CheckViewFeatures(viewFeatures);
                    _viewFeatures = viewFeatures;
                    foreach (var viewFeature in _viewFeatures)
                    {
                        viewFeature.InvalidFlag = string.IsNullOrEmpty(viewFeature.InvalidFlag) ? "" : viewFeature.InvalidFlag;
                        viewFeature.InvalidFlag = ResolveConstants(viewFeature.InvalidFlag);
                        viewFeature.Location = string.IsNullOrEmpty(viewFeature.Location) ? "" : viewFeature.Location;
                        viewFeature.Location = ResolveConstants(viewFeature.Location);

                        if (viewFeature.ResizeParamsText.IsNullOrEmpty())
                            viewFeature.ResizeParamsText = formStyle.ResizeParamsText;
                    }

                    //render public view
                    var publicViewFeature = _viewFeatures.Find(x => x.IsPublic);
                    if (publicViewFeature == null) throw new ArgumentException("Can not find Public View!");
                    MergeViewItems(publicViewFeature.Name);
                    RenderView(publicViewFeature.Name);

                    //--Menu
                    var menuFeatures = FunctionHelper.GetGenericFromCfgFile<List<MenuFeature>>(FileHelper.GetFilePathByRelativeLocation("\\MenuFeatures", _functionDir), false) ?? new List<MenuFeature>();
                    if (menuFeatures.Count > 0)
                    {
                        foreach (var menuFeature in menuFeatures)
                        {
                            menuFeature.Location = string.IsNullOrEmpty(menuFeature.Location) ? "" : menuFeature.Location;
                            menuFeature.Location = ResolveConstants(menuFeature.Location);
                            menuFeature.ImageUrl = string.IsNullOrEmpty(menuFeature.ImageUrl) ? "" : menuFeature.ImageUrl;
                            menuFeature.ImageUrl = ResolveConstants(menuFeature.ImageUrl);
                            menuFeature.InvalidFlag = string.IsNullOrEmpty(menuFeature.InvalidFlag) ? "" : menuFeature.InvalidFlag;
                            menuFeature.InvalidFlag = ResolveConstants(menuFeature.InvalidFlag);

                            menuFeature.MenuType = EnumHelper.GetIdByName<MenuType>(menuFeature.MenuTypeName);
                            if (menuFeature.MenuType == (int)MenuType.Nested | menuFeature.MenuType == (int)MenuType.ToolBar)
                            {
                                menuFeature.Container = string.Empty;
                            }

                            if (!menuFeature.InvalidFlag.IsNullOrEmpty())
                            {
                                if (menuFeature.InvalidFlag.ToLower() == "true") menuFeature.InvalidFlag = "true";
                            }
                            else
                            {
                                menuFeature.InvalidFlag = "false";
                            }
                        }
                        UiHelper.CheckMenuFeatures(menuFeatures);// to improve
                        _menuFeatures = menuFeatures;
                        var menuItems = new List<LayoutElement>();
                        foreach (var menuFeature in menuFeatures)
                        {
                            menuItems = GetMenuItems(menuFeature);
                            foreach (var menuItem in menuItems)
                            {
                                menuItem.ImageUrl = ResolveConstants(menuItem.ImageUrl);
                                menuItem.ImageUrl = FileHelper.GetFilePath(menuItem.ImageUrl, _functionDir + "\\menus");
                            }

                            if (menuItems != null) _layoutElements.AddRange(menuItems);
                        }

                        //if RefeshUI, or refreshUI after logon , _currentViewName is not empty
                        var firstViewName = "";
                        if (!_currentViewName.IsNullOrEmpty())
                        {
                            firstViewName = _currentViewName;
                            _currentViewName = "";
                        }
                        else if (!_functionInitParamSet.StartViewName.IsNullOrEmpty()) firstViewName = _functionInitParamSet.StartViewName;

                        var isNestedMenu = false;
                        foreach (var menuFeature in menuFeatures)
                        {
                            if (menuFeature.MenuType == (int)MenuType.Horizontal)
                            {
                                RenderHorizonalMenuAreaAndItems(menuFeature);

                            }
                            else if (menuFeature.MenuType == (int)MenuType.Vertical)
                            {
                                RenderVerticalMenuAreaAndItems(menuFeature);
                            }
                            else if (menuFeature.MenuType == (int)MenuType.Nested)
                            {
                                isNestedMenu = true;
                                if (!firstViewName.IsNullOrEmpty())
                                {
                                    ResetNestedMenuDefaultFlag(_layoutElements, firstViewName);
                                }
                                UpdateNestedMenu(0, menuFeature.Id);
                            }
                            else if (menuFeature.MenuType == (int)MenuType.ToolBar)
                            {
                                RenderToolBarMenuAreasAndItems(menuFeature.Id);
                            }
                        }

                        if (!isNestedMenu)
                        {
                            UiHelper.CheckViewFeaturesDefaultView(_viewFeatures);
                            if (firstViewName.IsNullOrEmpty())
                            {
                                var defView = _viewFeatures.Find(x => x.IsDefault);
                                var defViewName = (defView != null) ? defView.Name : "";
                                if (!defViewName.IsNullOrEmpty()) firstViewName = defViewName;
                            }
                            if (!firstViewName.IsNullOrEmpty())
                                SwitchView(firstViewName);
                        }
                    }//--menu end




                }//--mvi end
                else//--svi start
                {
                    var feartureCfgFile = _formCfgDir + "\\Feature";
                    if (!ConfigFileHelper.IsFileExisting(feartureCfgFile)) throw new ArgumentException(_functionInitParamSet.FormType.ToString() + " must have Fearture config file in " + _formCfgDir);

                    var formStyle = new FormStyle();
                    formStyle.CannotBeMaximized = true;
                    var zoneFeature = new ZoneFeature();
                    var sviViewFeature = new SviViewFeature();

                    var cfgFileMgr = new ConfigFileManager(feartureCfgFile);

                    if (_functionInitParamSet.FormType == FormType.SviOfView)
                    {
                        sviViewFeature = FunctionHelper.GetGenericFromCfgFile<SviViewFeature>(feartureCfgFile, true) ?? new SviViewFeature();
                        formStyle.Width = sviViewFeature.FormWidth == 0 ? 800 : sviViewFeature.FormWidth; ;
                        formStyle.Height = sviViewFeature.FormHeight == 0 ? 600 : sviViewFeature.FormHeight; ;

                        formStyle.WindowRadius = sviViewFeature.FormWindowRadius;
                        formStyle.CannotBeClosed = sviViewFeature.FormCannotBeClosed;
                        formStyle.HasNoControlBoxes = sviViewFeature.FormHasNoControlBoxes;

                        formStyle.DrawIcon = sviViewFeature.FormDrawIcon;
                        formStyle.IconUrl = sviViewFeature.FormIconUrl;
                        formStyle.HasTray = sviViewFeature.FormHasTray;
                        formStyle.TrayIconUrl = sviViewFeature.FormTrayIconUrl;
                        formStyle.TrayDataSource = sviViewFeature.FormTrayDataSource;
                        formStyle.ShowRunningStatus = sviViewFeature.FormShowRunningStatus;
                        var commonResizeParamsText = "MainSectionMainDivision: 0,0; " + "TopNavSection:0; ToolBarSection: 5,0,-1; MiddleNavSection: 0,0; DownNavSection: 0,0; " + "" +
                        "MainSectionLeftNavDivision: 0,0,0; MainSectionRightNavDivision: 0,0,0; MainSectionRightDivision: 0,0,0; " +
                        "RunningMessageSection: 0; " + (formStyle.ShowRunningStatus ? "RunningStatusSection: 26,0;" : "RunningStatusSection: 0,0;") +
                        "HorResizableDivisionStatus: none";
                        formStyle.ResizeParamsText = sviViewFeature.FormResizeParamsText.IsNullOrEmpty() ? commonResizeParamsText : sviViewFeature.FormResizeParamsText;

                        supportsMultipleThreads = sviViewFeature.SupportsMultipleThreads;
                        threadPoolMaxNum = sviViewFeature.ThreadPoolMaxNum;
                    }
                    else
                    {
                        zoneFeature = cfgFileMgr.ConvertToGeneric<ZoneFeature>();
                        formStyle.Width = zoneFeature.Width == 0 ? 800 : zoneFeature.Width;
                        formStyle.Height = zoneFeature.Height == 0 ? 600 : zoneFeature.Height;

                        formStyle.CannotBeClosed = zoneFeature.FormCannotBeClosed;
                        formStyle.HasNoControlBoxes = zoneFeature.FormHasNoControlBoxes;

                        formStyle.WindowRadius = zoneFeature.FormWindowRadius;
                        formStyle.DrawIcon = zoneFeature.FormDrawIcon;
                        formStyle.IconUrl = zoneFeature.FormIconUrl;
                        formStyle.HasTray = zoneFeature.FormHasTray;
                        formStyle.TrayIconUrl = zoneFeature.FormTrayIconUrl;
                        formStyle.TrayDataSource = zoneFeature.FormTrayDataSource;
                        formStyle.ShowRunningStatus = zoneFeature.FormShowRunningStatus;

                        formStyle.ResizeParamsText = "MainSectionMainDivision: 0,0; " + "TopNavSection:0; ToolBarSection: 5,0,-1; MiddleNavSection: 0,0; DownNavSection: 0,0; " + "" +
                        "MainSectionLeftNavDivision: 0,0,0; MainSectionRightNavDivision: 0,0,0; MainSectionRightDivision: 0,0,0; " +
                        "RunningMessageSection: 0; " + (formStyle.ShowRunningStatus ? "RunningStatusSection: 26,0;" : "RunningStatusSection: 0,0;") +
                        "HorResizableDivisionStatus: none";

                        supportsMultipleThreads = zoneFeature.SupportsMultipleThreads;
                        threadPoolMaxNum = zoneFeature.ThreadPoolMaxNum;
                    }


                    InitLayout(_functionInitParamSet.FormType, formStyle);
                    ResizeForm(formStyle.ResizeParamsText);

                    //render public view
                    if (_functionInitParamSet.FormType == FormType.SviOfView)
                    {
                        var publicViewFeature = new ViewFeature();
                        publicViewFeature.Name = "PublicView";
                        publicViewFeature.IsPublic = true;
                        publicViewFeature.Location = _formCfgDir;
                        _viewFeatures.Add(publicViewFeature);
                        MergeViewItems(publicViewFeature.Name);
                        RenderView(publicViewFeature.Name);
                    }
                    else if (_functionInitParamSet.FormType == FormType.SviOfZone)
                    {
                        var areaLayoutElement = new LayoutElement()
                        {
                            Id = 10,
                            LayoutType = (int)LayoutType.View,
                            View = "PublicView",
                            Name = "PublicView" + "_" + "PublicArea",
                            Type = (int)LayoutElementType.ContentArea,
                            Container = "MainSectionMainDivisionMidRegion",
                            DockType = (int)ControlDockType.Fill,
                            DockOrder = "10",
                            Width = -1,
                            Height = -1,
                        };
                        _layoutElements.Add(areaLayoutElement);

                        var zoneName = _functionInitParamSet.FunctionCode;
                        var zoneLayoutElement = new LayoutElement()
                        {
                            Id = 1010,

                            Name = "PublicView" + "_" + zoneName,
                            Type = (int)LayoutElementType.Zone,
                            Container = "PublicArea",
                            View = "PublicView",
                            Location = _formCfgDir,
                            LayoutType = (int)LayoutType.Zone,
                            DockType = (int)ControlDockType.Top,
                            DockOrder = "1010",
                            Width = -1,
                            Height = zoneFeature.Height,
                            StyleText = zoneFeature.StyleText,
                            InputVariables = _functionInitParamSet.StartZoneProcessParams,
                            //InputVariables = zoneFeature.InputParams,
                            DataSource = zoneFeature.DataSource,
                        };
                        _layoutElements.Add(zoneLayoutElement);
                        RenderView("PublicView");
                    }
                }//--svi end

                //_threadPool
                if (supportsMultipleThreads)
                    _threadPool = new ManagedThreadPool(threadPoolMaxNum);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".LoadForm Error: " + ex.Message);
            }
        }

        //**layout
        //*init
        //##InitLayout
        private void InitLayout(FormType formType, FormStyle formStyle)
        {
            try
            {
                if (_functionInitParamSet.FormTitle.IsNullOrEmpty())
                {
                    var text = "";
                    if (_functionInitParamSet.SupportMultiCultures)
                    {
                        text = AnnexHelper.GetText("Function", _functionInitParamSet.FunctionCode, _annexes, AnnexTextType.DisplayName, CultureHelper.CurrentLanguageCode, GetAnnexMode.OnlyByCurLang);
                    }
                    else
                    {
                        text = AnnexHelper.GetText("Function", _functionInitParamSet.FunctionCode, _annexes, AnnexTextType.DisplayName, CultureHelper.CurrentLanguageCode, GetAnnexMode.FirstAnnex);
                    }

                    text = !text.IsNullOrEmpty() ? text : _functionInitParamSet.ApplicationCode + "-" + _functionInitParamSet.FunctionCode;
                    Text = text;

                }
                else
                {
                    Text = _functionInitParamSet.FormTitle;
                }


                _hasTray = formStyle.HasTray;
                var isFormModal = this.Modal;
                if (isFormModal)
                {
                    _hasTray = false;
                }

                Resizable = false;
                DrawCationBackground = false;
                DrawIcon = false;
                WindowState = FormWindowState.Normal;

                if (formStyle.HasNoControlBoxes)
                {
                    ControlBox = false;
                }

                if (formStyle.CannotBeClosed)
                {
                    MinimizeBox = false;
                    TreatCloseBoxAsMinimizeBox = true;
                }
                if (isFormModal)
                {
                    MinimizeBox = false;
                    TreatCloseBoxAsMinimizeBox = false;
                }

                MaximizeBox = !formStyle.CannotBeMaximized;
                if (isFormModal)
                {
                    MaximizeBox = false;
                }

                var startWindowState = formStyle.StartWindowState;
                if (startWindowState == "maximized")
                {
                    WindowState = FormWindowState.Maximized;
                }
                else if (startWindowState == "minimized")
                {
                    WindowState = FormWindowState.Minimized;
                }
                else
                {
                    WindowState = FormWindowState.Normal;

                }
                var ordinaryWindowStatus = formStyle.OrdinaryWindowStatus;
                if (ordinaryWindowStatus.IsNullOrEmpty()) _ordinaryWindowStatus = WindowState;
                else _ordinaryWindowStatus = ordinaryWindowStatus == "maximized" ? FormWindowState.Maximized : FormWindowState.Normal;

                if (formType == FormType.Mvi)
                {
                    var w = formStyle.Width > -1 ? formStyle.Width : 1024;
                    var h = formStyle.Height > -1 ? formStyle.Height : 768;
                    this.ClientSize = new System.Drawing.Size(formStyle.Width, formStyle.Height);
                    if (WindowState != FormWindowState.Maximized)
                    {
                        Width = w;
                        Height = h;
                    }
                }
                else
                {
                    Width = formStyle.Width + 4;
                    Height = formStyle.Height + 35 + (formStyle.ShowRunningStatus ? 26 : 0);
                }


                DrawIcon = formStyle.DrawIcon;
                if (DrawIcon)
                {
                    var iconUrl = formStyle.IconUrl;
                    iconUrl = FileHelper.GetFilePath(iconUrl, _formCfgDir);
                    if (!iconUrl.IsNullOrEmpty() && System.IO.File.Exists(iconUrl))
                    {
                        var strm = File.Open(iconUrl, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Icon = new Icon(strm);
                    }
                }

                if (formStyle.WindowRadius != 0)
                {
                    Radius = formStyle.WindowRadius;
                    RoundStyle = RoundStyle.All;
                }

                if (_hasTray)
                {
                    ShowInTaskbar = false;
                    InitTray(_formCfgDir, formStyle);
                }

                if (isFormModal)
                {
                    if (Owner != null)
                    {
                        Left = Owner.Location.X + (Owner.Width / 2 - Width / 2);
                    }
                    else
                    {
                        var rect = new Rectangle();
                        rect = Screen.GetWorkingArea(this);
                        Left = rect.Width / 2 - Width / 2;
                        Top = rect.Height > Height ? (rect.Height / 2 - Height / 2) / 3 : 10;
                    }
                }
                else
                {
                    //var rect = new Rectangle();
                    //rect = Screen.GetWorkingArea(this);
                    //Left = rect.Width / 2 - Width / 2;
                    //Top = _formStyle.TopLocationY == -1 ? 20 : _formStyle.TopLocationY;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".InitLayout Error:" + ex.Message);
            }
        }

        //##SetLayoutTextByCulture
        private void SetLayoutTextByCulture()
        {
            var elmentEx = "";
            try
            {
                var text = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    text = AnnexHelper.GetText("Function", _functionInitParamSet.FunctionCode, _annexes, AnnexTextType.DisplayName, CultureHelper.CurrentLanguageCode, GetAnnexMode.OnlyByCurLang);
                }
                else
                {
                    text = AnnexHelper.GetText("Function", _functionInitParamSet.FunctionCode, _annexes, AnnexTextType.DisplayName, CultureHelper.CurrentLanguageCode, GetAnnexMode.FirstAnnex);
                }

                text = !text.IsNullOrEmpty() ? text : _functionInitParamSet.ApplicationCode + "-" + _functionInitParamSet.FunctionCode;
                Text = text;

                //tray
                if (_hasTray)
                {
                    _tray.Text = Text;
                    _trayContextMenuStrip.SetTextByCulture();
                }

                //menu
                foreach (var menuFeature in _menuFeatures)
                {
                    if (menuFeature.MenuType == (int)MenuType.Nested | menuFeature.MenuType == (int)MenuType.ToolBar)
                    {
                        var elmts = _layoutElements.FindAll(x => x.Type == (int)LayoutElementType.MenuItem & x.LayoutId == menuFeature.Id & x.LayoutType == (int)LayoutType.Menu);
                        foreach (var elmt in elmts)
                        {

                            var ctnName = elmt.Container;
                            elmentEx = elmt.Name;
                            var area = _layoutElements.Find(x => (x.Type == (int)LayoutElementType.MenuItemContainerArea) & x.Name.EndsWith(ctnName) & x.IsRendered);
                            if (area != null)
                            {
                                if (area.ControlTypeName == "ToolStrip")
                                {
                                    var areaControl = GetControl(area.Name);
                                    var areaToolStrip = areaControl as ToolStrip;
                                    var elmtControls = areaToolStrip.Items.Find(elmt.Name, true);

                                    if (elmtControls != null && elmtControls.Length > 0)
                                    {
                                        elmtControls[0].Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
                                    }

                                    if (!string.IsNullOrEmpty(elmt.Remark) & (elmt.ControlTypeName.ToLower().StartsWith("ImageToolButton".ToLower())))
                                    {
                                        elmtControls[0].Text = FunctionHelper.GetRemark(_functionInitParamSet.SupportMultiCultures, "MenuItem", elmt.Name, _annexes, elmt.Remark);
                                    }

                                    if ((elmt.ControlTypeName.ToLower().Contains("ToolSplitButton".ToLower())))
                                    {
                                        var ctrl = elmtControls[0] as ToolStripSplitButtonEx;
                                        ctrl.SetTextByCulture();
                                    }
                                }
                                else//panel
                                {
                                    var elmtControl = GetControl(elmt.Name);
                                    elmtControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
                                    if (!string.IsNullOrEmpty(elmt.Remark) & (elmt.ControlTypeName.ToLower().StartsWith("ImageButton".ToLower())))
                                    {
                                        elmtControl.Tag = FunctionHelper.GetRemark(_functionInitParamSet.SupportMultiCultures, "MenuItem", elmt.Name, _annexes, elmt.Remark);
                                    }
                                }
                            }
                        }
                    }
                    else if (menuFeature.MenuType == (int)MenuType.Horizontal)
                    {
                        var elmts = _layoutElements.FindAll(x => x.Type == (int)LayoutElementType.MenuItem & x.LayoutId == menuFeature.Id & x.LayoutType == (int)LayoutType.Menu);

                        foreach (var elmt in elmts)
                        {
                            var ctnName = elmt.Container;
                            var area = _layoutElements.Find(x => (x.Type == (int)LayoutElementType.MenuItemContainerArea) & x.Name.EndsWith(ctnName) & x.IsRendered);
                            if (area != null)
                            {
                                if (area.ControlTypeName == "MenuStrip")
                                {
                                    var areaControl = GetControl(area.Name);
                                    var areaMenuStrip = areaControl as MenuStrip;
                                    var elmtControls = areaMenuStrip.Items.Find(elmt.Name, true);

                                    if (elmtControls != null && elmtControls.Length > 0)
                                    {
                                        elmtControls[0].Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
                                    }
                                }
                            }
                        }
                    }
                    else if (menuFeature.MenuType == (int)MenuType.Vertical)
                    {
                        var area = _layoutElements.Find(x => x.Type == (int)LayoutElementType.MenuItemContainerArea & x.LayoutId == menuFeature.Id);
                        var areaName = area.Name;
                        var menuName = areaName + "-verticalMenu";
                        var areaControl = GetControl(areaName);
                        var menuControl = areaControl.Controls.Find(menuName, true)[0];
                        var verticalMenu = menuControl as VerticalMenu;
                        verticalMenu.SetTextByCulture();

                    }


                    //zone
                    var zoneItems = _zonesItems.FindAll(x => (x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.SubControl) & x.ControlTypeName != "Row");
                    foreach (var zoneItem in zoneItems)
                    {
                        elmentEx = zoneItem.Name;
                        var zoneItemControl = GetControl(zoneItem.Name);
                        if (zoneItem.ControlTypeName == "Radio" | zoneItem.ControlTypeName == "CheckBox" | zoneItem.ControlTypeName.Contains("Button")
                            | zoneItem.ControlTypeName == "Label" | zoneItem.ControlTypeName == "TitleLabel" | zoneItem.ControlTypeName == "CommandLabel")
                        {
                            if (string.IsNullOrEmpty(zoneItem.DisplayName))
                            {
                                zoneItemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                            }
                            else
                            {
                                var txt = ResolveStringByRefProcessVariablesAndControls(zoneItem.DisplayName);
                                if (zoneItem.DisplayName.StartsWith("="))
                                {
                                    zoneItemControl.Text = GetText(txt);
                                }
                                else
                                {
                                    zoneItemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                                }
                            }

                        }
                        else if (zoneItem.ControlTypeName == "PictureBox")
                        {
                            if (!string.IsNullOrEmpty(zoneItem.DisplayName))
                            {
                                var txt = ResolveStringByRefProcessVariablesAndControls(zoneItem.DisplayName);
                                if (zoneItem.DisplayName.StartsWith("="))
                                {
                                    zoneItemControl.Tag = GetText(txt);
                                }
                                else
                                {
                                    zoneItemControl.Tag = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                                }
                            }
                            else
                            {
                                zoneItemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                            }
                        }
                        else if (zoneItem.ControlTypeName == "StatusLight")
                        {
                            var zoneItemStatusLight = zoneItemControl as StatusLight;
                            if (string.IsNullOrEmpty(zoneItem.DisplayName))
                            {
                                zoneItemStatusLight.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                            }
                            else
                            {
                                var txt = ResolveStringByRefProcessVariablesAndControls(zoneItem.DisplayName);
                                if (zoneItem.DisplayName.StartsWith("="))
                                {
                                    zoneItemStatusLight.Text = GetText(txt);
                                }
                                else
                                {
                                    zoneItemStatusLight.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                                }
                            }

                        }
                        else if (zoneItem.ControlTypeName == "ScoreLight")
                        {
                            var zoneItemScoreLight = zoneItemControl as ScoreLight;
                            if (string.IsNullOrEmpty(zoneItem.DisplayName))
                            {
                                zoneItemScoreLight.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                            }
                            else
                            {
                                var txt = ResolveStringByRefProcessVariablesAndControls(zoneItem.DisplayName);
                                if (zoneItem.DisplayName.StartsWith("="))
                                {
                                    zoneItemScoreLight.Text = GetText(txt);
                                }
                                else
                                {
                                    zoneItemScoreLight.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                                }
                            }
                        }
                        else if (zoneItem.ControlTypeName == "ComboBox")
                        {
                            if (!String.IsNullOrEmpty(zoneItem.DataSource) & zoneItem.DataSource.ToLower().Contains("bylang"))
                            {
                                var zoneItemComboBox = zoneItemControl as ComboBox;
                                var selectedIndex = zoneItemComboBox.SelectedIndex;
                                var dataSrcStr = zoneItem.DataSource;
                                if (dataSrcStr.StartsWith("="))
                                {
                                    var txt = GetText(dataSrcStr);
                                    if (txt.Contains("{") & txt.Contains("}"))
                                    {
                                        var valTxts = JsonHelper.ConvertToGeneric<List<ValueText>>(txt);
                                        zoneItemComboBox.DataSource = valTxts;
                                        zoneItemComboBox.ValueMember = "Value";
                                        zoneItemComboBox.DisplayMember = "Text";
                                    }
                                }
                                zoneItemComboBox.SelectedIndex = selectedIndex;
                            }
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResetLayoutTextByCulture Error: " + ex.Message + elmentEx); ;
            }
        }

        //##InitTray
        private void InitTray(string _formCfgDir, FormStyle formStyle)
        {
            try
            {

                var trayDir = _formCfgDir + "\\Tray";
                var subMenuItems = UiHelper.GetSubMenuItems(trayDir);
                if (subMenuItems.Count == 0) return;
                foreach (var subMenuItem in subMenuItems)
                {
                    if (!subMenuItem.ImageUrl.IsNullOrEmpty())
                    {
                        subMenuItem.ImageUrl = ResolveConstants(subMenuItem.ImageUrl);
                        subMenuItem.ImageUrl = FileHelper.GetFilePath(subMenuItem.ImageUrl, trayDir);
                    }
                }
                var subMenuItemsAnnexes = FunctionHelper.GetAnnexesFromCfgFile(trayDir + "\\Annexes", "", false);


                _tray = new NotifyIcon();
                _trayContextMenuStrip = new ContextMenuStripEx(subMenuItems, subMenuItemsAnnexes);
                _trayContextMenuStrip.Name = "trayContextMenuStripEx";
                _trayContextMenuStrip.OnMenuItemClick += new System.EventHandler(SubMenuItemClickHandler);

                _tray.Text = Text;
                _tray.Visible = true;
                _tray.ContextMenuStrip = _trayContextMenuStrip;
                _tray.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tray_MouseUp);

                var trayIconUrl = "";
                var iconDir = _formCfgDir + "\\icons";
                trayIconUrl = FileHelper.GetFilePath(formStyle.TrayIconUrl, iconDir);

                if (trayIconUrl.IsNullOrEmpty() | !System.IO.File.Exists(trayIconUrl)) trayIconUrl = formStyle.IconUrl;
                if (System.IO.File.Exists(trayIconUrl))
                {
                    var strm = File.Open(trayIconUrl, FileMode.Open, FileAccess.Read, FileShare.Read);
                    _tray.Icon = new Icon(strm);
                }
                else if (DrawIcon)
                {
                    _tray.Icon = Icon;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".InitTray Error:" + ex.Message);
            }
        }

        //##InitPublicRegionComponent
        private void InitToolBarSectionPublicRegionComponents()
        {
            try
            {


                if (CultureHelper.Cultures.Count > 1)
                {
                    var toolBarSectionPublicRegionSplitButtonCulture = new System.Windows.Forms.ToolStripSplitButton();
                    toolBarSectionPublicRegionSplitButtonCulture.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
                    toolBarSectionPublicRegionSplitButtonCulture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                    toolBarSectionPublicRegionSplitButtonCulture.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
                    toolBarSectionPublicRegionSplitButtonCulture.ForeColor = System.Drawing.Color.Azure;
                    toolBarSectionPublicRegionSplitButtonCulture.Image = (System.Drawing.Image)(Properties.Resources.ChooseLanguage);
                    //not work in env of  fx 4.72 &vs 16.92 
                    //var resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionForm));
                    //toolBarSectionPublicRegionSplitButtonCulture.Image = ((System.Drawing.Image)(resources.GetObject("ChooseLanguage")));

                    toolBarSectionPublicRegionSplitButtonCulture.ImageTransparentColor = System.Drawing.Color.Magenta;
                    toolBarSectionPublicRegionSplitButtonCulture.Name = "ToolBarSectionPublicRegionToolStripSplitButtonCulture";
                    toolBarSectionPublicRegionSplitButtonCulture.Size = new System.Drawing.Size(97, 56);
                    toolBarSectionPublicRegionSplitButtonCulture.Text = CultureHelper.CurrentLanguageName;
                    toolBarSectionPublicRegionSplitButtonCulture.ToolTipText = WinformRes.ChooseLanguage;
                    foreach (var culture in CultureHelper.Cultures)
                    {
                        var toolBarSectionPublicRegionSplitButtonCultureItem = new System.Windows.Forms.ToolStripMenuItem();
                        var imgUrl = FileHelper.GetFilePath(culture.ImageUrl, _appCfgDir + "\\Cultures");
                        toolBarSectionPublicRegionSplitButtonCultureItem.Image = ControlHelper.GetImage(imgUrl);
                        toolBarSectionPublicRegionSplitButtonCultureItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
                        toolBarSectionPublicRegionSplitButtonCultureItem.Name = "ToolBarSectionPublicRegionToolStripSplitButtonCultureItem" + "_" + culture.Name;
                        toolBarSectionPublicRegionSplitButtonCultureItem.Size = new System.Drawing.Size(124, 22);
                        toolBarSectionPublicRegionSplitButtonCultureItem.Tag = culture.Name;
                        toolBarSectionPublicRegionSplitButtonCultureItem.Text = culture.LanguageName;
                        toolBarSectionPublicRegionSplitButtonCultureItem.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                        toolBarSectionPublicRegionSplitButtonCultureItem.Click += new System.EventHandler(ToolBarSectionPublicRegionToolStripSplitButtonCultureItemClickHandler);
                        toolBarSectionPublicRegionSplitButtonCulture.DropDownItems.Add(toolBarSectionPublicRegionSplitButtonCultureItem);
                    }
                    this.ToolBarSectionPublicRegionToolStrip.Items.Add(toolBarSectionPublicRegionSplitButtonCulture);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".InitPublicRegionComponent Error:" + ex.Message);
            }
        }

        //**resize
        //##ResizeForm
        private void ResizeForm(string resizeStrs)
        {
            try
            {
                if (!string.IsNullOrEmpty(resizeStrs))
                {
                    var resizeStr = resizeStrs.GetStyleValue("TopNavSection");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            TopNavSectionHeight = Convert.ToInt16(resizeStrArry[0]);
                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                TopNavSectionLeftRegionWidth = Convert.ToInt16(resizeStrArry[1]);
                        }
                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                TopNavSectionRightRegionWidth = Convert.ToInt16(resizeStrArry[2]);
                        }
                    }


                    resizeStr = resizeStrs.GetStyleValue("ToolBarSection");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            ToolBarSectionHeight = Convert.ToInt16(resizeStrArry[0]);

                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                ToolBarSectionLeftRegionWidth = Convert.ToInt16(resizeStrArry[1]);
                        }

                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                ToolBarSectionRightRegionWidth = Convert.ToInt16(resizeStrArry[2]);
                        }

                        if (resizeStrArry.Length > 3)
                        {
                            if (Convert.ToInt16(resizeStrArry[3]) > -1)
                                ToolBarSectionPublicRegionWidth = Convert.ToInt16(resizeStrArry[3]);
                        }
                    }

                    resizeStr = resizeStrs.GetStyleValue("MiddleNavSection");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            MiddleNavSectionHeight = Convert.ToInt16(resizeStrArry[0]);

                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                MiddleNavSectionLeftRegionWidth = Convert.ToInt16(resizeStrArry[1]);
                        }
                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                MiddleNavSectionRightRegionWidth = Convert.ToInt16(resizeStrArry[2]);
                        }
                    }


                    resizeStr = resizeStrs.GetStyleValue("DownNavSection");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (resizeStrArry[0] != "-1")
                            DownNavSectionHeight = Convert.ToInt16(resizeStrArry[0]);

                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                DownNavSectionLeftRegionWidth = Convert.ToInt16(resizeStrArry[1]);
                        }
                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                DownNavSectionRightRegionWidth = Convert.ToInt16(resizeStrArry[2]);
                        }
                    }


                    this.MainSection.Padding = new Padding(0);
                    resizeStr = resizeStrs.GetStyleValue("MainSectionLeftNavDivision");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            MainSectionLeftNavDivisionWidth = Convert.ToInt16(resizeStrArry[0]);
                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                MainSectionLeftNavDivisionUpRegionHeight = Convert.ToInt16(resizeStrArry[1]);
                        }
                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                MainSectionLeftNavDivisionDownRegionHeight = Convert.ToInt16(resizeStrArry[2]);
                        }
                    }

                    resizeStr = resizeStrs.GetStyleValue("MainSectionRightNavDivision");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            MainSectionRightNavDivisionWidth = Convert.ToInt16(resizeStrArry[0]);
                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                MainSectionRightNavDivisionUpRegionHeight = Convert.ToInt16(resizeStrArry[1]);
                        }
                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                MainSectionRightNavDivisionDownRegionHeight = Convert.ToInt16(resizeStrArry[2]);
                        }
                    }



                    resizeStr = resizeStrs.GetStyleValue("MainSectionMainDivision");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            MainSectionMainDivisionUpRegionHeight = Convert.ToInt16(resizeStrArry[0]);
                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                MainSectionMainDivisionDownRegionHeight = Convert.ToInt16(resizeStrArry[1]);
                        }
                    }


                    resizeStr = resizeStrs.GetStyleValue("MainSectionRightDivision");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        var resizeStrArry = resizeStr.Split(',');
                        if (Convert.ToInt16(resizeStrArry[0]) > -1)
                            MainSectionRightDivisionWidth = Convert.ToInt16(resizeStrArry[0]);
                        if (resizeStrArry.Length > 1)
                        {
                            if (Convert.ToInt16(resizeStrArry[1]) > -1)
                                MainSectionRightDivisionUpRegionHeight = Convert.ToInt16(resizeStrArry[1]);
                        }
                        if (resizeStrArry.Length > 2)
                        {
                            if (Convert.ToInt16(resizeStrArry[2]) > -1)
                                MainSectionRightDivisionDownRegionHeight = Convert.ToInt16(resizeStrArry[2]);
                        }
                    }

                    var runningMessageSectionHeight = 0;
                    resizeStr = resizeStrs.GetStyleValue("RunningMessageSection");
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        runningMessageSectionHeight = Convert.ToInt16(resizeStr);
                    }
                    RunningMessageSectionHeight = runningMessageSectionHeight;

                    resizeStr = resizeStrs.GetStyleValue("HasRunningProgress");
                    var runningProgressSectionHeight = 0;
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        if (resizeStr == "true") runningProgressSectionHeight = RunningProgressSectionHeight;
                    }
                    RunningProgressSectionHeight = runningProgressSectionHeight;


                    resizeStr = resizeStrs.GetStyleValue("HasRunningStatus");
                    var runningStatusSectionHeight = 0;
                    if (!resizeStr.IsNullOrEmpty())
                    {
                        if (resizeStr == "true") runningStatusSectionHeight = RunningStatusSectionHeight;
                    }
                    RunningStatusSectionHeight = runningStatusSectionHeight;


                    resizeStr = resizeStrs.GetStyleValue("HorResizableDivisionStatus");
                    if (resizeStr == "show")
                    {
                        HorizontalResizableDivisionStatus = ResizableDivisionStatus.Shown;
                    }
                    else if (resizeStr == "hide")
                    {
                        HorizontalResizableDivisionStatus = ResizableDivisionStatus.Hidden;
                    }
                    else
                    {
                        HorizontalResizableDivisionStatus = ResizableDivisionStatus.None;
                    }

                    InitFrameHorizontalResizableDivisionStatus();
                    ResizeFrameComponent();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResizeRegion Error: " + ex.Message);
            }
        }

        //**menu
        //##GetMenuItems
        private List<LayoutElement> GetMenuItems(MenuFeature menuFeature)
        {
            try
            {
                var cfgFile = FileHelper.GetFilePathByRelativeLocation(menuFeature.Location + "\\Ui", _functionDir + "\\Menus");
                var tmpMenuItems = FunctionHelper.GetGenericFromCfgFile<List<LayoutElement>>(cfgFile, false) ?? new List<LayoutElement>();
                foreach (var elmt in tmpMenuItems)
                {
                    if (menuFeature.MenuType == (int)MenuType.Horizontal | menuFeature.MenuType == (int)MenuType.Vertical)
                        elmt.TypeName = "MenuItem";
                    elmt.Location = _functionDir + "\\Menus";

                    elmt.InvalidFlag = ResolveConstants(elmt.InvalidFlag);
                    if (elmt.InvalidFlag.StartsWith("=")) elmt.InvalidFlag = GetText(elmt.InvalidFlag);
                    elmt.InvalidFlag = elmt.InvalidFlag.GetJudgementFlag();
                }

                var menuItems = tmpMenuItems.Where(x =>
                  (x.InvalidFlag.ToLower() != "true") & (
                    x.TypeName == LayoutElementType.MenuItemContainerArea.ToString()
                  | x.TypeName == LayoutElementType.MenuItem.ToString()
                    )
                    )
                  .ToList();


                if (menuItems.Count > 0)
                {
                    foreach (var elmt in menuItems)
                    {
                        UiHelper.SetLayoutElementTypes(elmt);
                    }
                    UiHelper.CheckMenuItems(menuFeature.MenuType, menuItems);

                    var annexCfgFile = FileHelper.GetFilePathByRelativeLocation(menuFeature.Location + "\\Annexes", _functionDir + "\\Menus");
                    var annexList = new List<Annex>();
                    var tempAnnexes = new List<Annex>();

                    if (_functionInitParamSet.SupportMultiCultures)
                    {
                        annexList = FunctionHelper.GetAnnexesFromCfgFile(annexCfgFile, "MenuItem", false);
                    }

                    foreach (var elmt in menuItems)
                    {
                        elmt.LayoutType = (int)LayoutType.Menu;
                        elmt.LayoutId = menuFeature.Id;
                        elmt.InvisibleFlag = string.IsNullOrEmpty(elmt.InvisibleFlag) ? "" : elmt.InvisibleFlag;
                        elmt.InvisibleFlag = ResolveConstants(elmt.InvisibleFlag);
                        elmt.DisabledFlag = string.IsNullOrEmpty(elmt.DisabledFlag) ? "" : elmt.DisabledFlag;
                        elmt.DisabledFlag = ResolveConstants(elmt.DisabledFlag);
                        elmt.WriteIntoLogFlag = string.IsNullOrEmpty(elmt.WriteIntoLogFlag) ? "" : elmt.WriteIntoLogFlag;
                        elmt.WriteIntoLogFlag = ResolveConstants(elmt.WriteIntoLogFlag);
                        elmt.ShowRunningStatusFlag = string.IsNullOrEmpty(elmt.ShowRunningStatusFlag) ? "" : elmt.ShowRunningStatusFlag;
                        elmt.ShowRunningStatusFlag = ResolveConstants(elmt.ShowRunningStatusFlag);

                        elmt.ExecModeFlag = string.IsNullOrEmpty(elmt.ExecModeFlag) ? "" : elmt.ExecModeFlag;
                        elmt.ExecModeFlag = ResolveConstants(elmt.ExecModeFlag);
                        elmt.DisplayName = string.IsNullOrEmpty(elmt.DisplayName) ? "" : elmt.DisplayName;
                        elmt.DisplayName = ResolveConstants(elmt.DisplayName);
                        elmt.ImageUrl = string.IsNullOrEmpty(elmt.ImageUrl) ? "" : elmt.ImageUrl;
                        elmt.ImageUrl = ResolveConstants(elmt.ImageUrl);
                        elmt.DataSource = string.IsNullOrEmpty(elmt.DataSource) ? "" : elmt.DataSource;
                        elmt.DataSource = ResolveConstants(elmt.DataSource);

                        if (elmt.Type == (int)LayoutElementType.MenuItem)
                        {
                            if (menuFeature.MenuType == (int)MenuType.Horizontal | menuFeature.MenuType == (int)MenuType.Vertical)
                            {
                                elmt.Container = menuFeature.Container + "Menu" + menuFeature.Id + "Area";
                            }

                            if (elmt.Type == (int)LayoutElementType.MenuItem)
                            {
                                var elmtNameNew = elmt.Container + elmt.Name;
                                if (_functionInitParamSet.SupportMultiCultures)
                                {
                                    tempAnnexes = annexList.FindAll(x => x.MasterName == elmt.Name);
                                    if (tempAnnexes.Count > 0)
                                    {
                                        foreach (var annex in tempAnnexes)
                                        {
                                            annex.MasterName = elmtNameNew;
                                            _annexes.Add(annex);
                                        }
                                    }
                                }
                                elmt.Name = elmtNameNew;
                            }
                            var subItems = menuItems.FindAll(x => x.ParentId == elmt.Id);
                            if (subItems.Count == 0)
                            {
                                elmt.IsLastLevel = true;
                            }
                            if (!elmt.View.IsNullOrEmpty()) elmt.View = GetText(elmt.View);
                        }

                        elmt.IsRendered = false;
                        elmt.IsChecked = false;
                    }
                    return menuItems;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetMenuItems Error: " + ex.Message);
            }
        }

        //#RenderHorizonalMenuAreaAndItems
        private void RenderHorizonalMenuAreaAndItems(MenuFeature menuFeature)
        {
            try
            {
                if (_layoutElements == null) return;

                //*top level menu items
                var topLevelMenuItems = _layoutElements.FindAll(x =>
                (x.Type == (int)LayoutElementType.MenuItem) && x.ParentId == 0 & x.LayoutId == menuFeature.Id & x.LayoutType == (int)LayoutType.Menu);
                if (topLevelMenuItems.Count < 1) return;

                var menuAreaControl = new MenuStrip();
                var regionControl = new Control();
                try
                {
                    regionControl = this.Controls.Find(menuFeature.Container, true)[0];
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("menu Container doesn't exist! ContainerRegionName={}, menuFeatureature.Id={}, menuFeatureature.Name={}"
                        .FormatWith(menuFeature.Container, menuFeature.Id, menuFeature.Name));
                }
                menuAreaControl.Dock = DockStyle.Fill;
                menuAreaControl.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
                menuAreaControl.ImageScalingSize = new Size(26, 26);
                menuAreaControl.AutoSize = false;

                menuAreaControl.Name = menuFeature.Container + "Menu" + menuFeature.Id + "Area";
                menuAreaControl.Tag = menuAreaControl.Name;
                regionControl.Controls.Add(menuAreaControl);

                var horizontalMenuArea = new LayoutElement();
                if (menuFeature.MenuType == (int)MenuType.Horizontal)
                {
                    horizontalMenuArea.Container = menuFeature.Container;
                    horizontalMenuArea.TypeName = "MenuItemContainerArea";
                    horizontalMenuArea.ControlTypeName = "MenuStrip";
                    horizontalMenuArea.Name = menuFeature.Container + "Menu" + menuFeature.Id + "Area";
                    horizontalMenuArea.Id = -1;

                    horizontalMenuArea.IsChecked = true;
                    horizontalMenuArea.IsRendered = true;
                    horizontalMenuArea.View = "";
                }
                UiHelper.SetLayoutElementTypes(horizontalMenuArea);

                _layoutElements.Add(horizontalMenuArea);


                foreach (var topLevelViewMenuItem in topLevelMenuItems)
                {
                    RenderHorizonalMenu(menuAreaControl, null, topLevelViewMenuItem);
                }


            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderHorizonalMenuAreaAndItems Error: " + ex.Message);
            }
        }

        //##RenderHorizonalMenu
        private void RenderHorizonalMenu(MenuStrip parentAreaCtrl, ToolStripMenuItem parentItemCtrl, LayoutElement menuItem)
        {
            var itemNameForEx = "";
            try
            {

                itemNameForEx = menuItem.Name;

                //-Visible, Enabled
                bool isItemVisible = true;
                var itemInvisibleFlag = menuItem.InvisibleFlag;
                var procs = _procedures.FindAll(x => x.ZoneName.IsNullOrEmpty());
                if (string.IsNullOrEmpty(itemInvisibleFlag)) itemInvisibleFlag = "false";
                else
                {
                    itemInvisibleFlag = GetText(menuItem.InvisibleFlag);
                }
                isItemVisible = (itemInvisibleFlag == "false") ? true : false;

                bool isItemEnabled = true;
                var itemDisabledFlag = menuItem.DisabledFlag;
                if (string.IsNullOrEmpty(itemDisabledFlag)) itemDisabledFlag = "false";
                else
                {
                    itemDisabledFlag = GetText(menuItem.DisabledFlag);

                }
                isItemEnabled = (itemDisabledFlag == "false") ? true : false;

                var imageUrl = menuItem.ImageUrl;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    imageUrl = GetText(imageUrl);
                }


                if (menuItem.ControlTypeName.ToLower().Contains("ToolStripMenuItem".ToLower()))
                {
                    var menuItemControl = new ToolStripMenuItem();

                    //--image
                    if (menuItem.ControlTypeName.ToLower().Contains("image") & !string.IsNullOrEmpty(menuItem.ImageUrl))
                    {
                        var imgUrl = FileHelper.GetFilePath(imageUrl, menuItem.Location);
                        menuItemControl.Image = ControlHelper.GetImage(imgUrl);
                    }

                    //--imagetext relation
                    if (menuItem.ControlTypeName.EndsWith("V")) menuItemControl.TextImageRelation = TextImageRelation.ImageAboveText;
                    else if (menuItem.ControlTypeName.EndsWith("H"))
                    {
                        menuItemControl.TextImageRelation = TextImageRelation.ImageBeforeText;
                    }
                    //--dock, size,offset
                    if (menuItem.DockType == (int)ControlDockType.Right)
                    {
                        menuItemControl.Alignment = ToolStripItemAlignment.Right;
                    }

                    menuItemControl.AutoSize = true;
                    /*
                    if (menuItem.Width != -1)
                    {
                        menuItemControl.AutoSize = false;
                        menuItemControl.Width = menuItem.Width;
                    }
                    if (menuItem.Height != -1)
                    {
                        menuItemControl.AutoSize = false;
                        menuItemControl.Height = menuItem.Height;
                    }
                    if (menuItem.ImageWidth != -1 && menuItem.ImageHeight != -1) menuItemControl.ImageScaling = ToolStripItemImageScaling.None;
                    */

                    //--name
                    menuItemControl.Name = menuItem.Name;
                    menuItemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                    //--displayname, remark
                    menuItemControl.AutoToolTip = false;
                    if (!string.IsNullOrEmpty(menuItem.Remark))
                    {
                        menuItemControl.AutoToolTip = true;
                        menuItemControl.Text = FunctionHelper.GetRemark(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.Remark);
                    }

                    //--font
                    if (!menuItem.StyleText.IsNullOrEmpty())
                    {
                        ControlHelper.SetToolStripMenuItemStyleByText(menuItemControl, menuItem.StyleText);
                    }

                    //--enable, visible
                    if (!isItemEnabled)
                    {
                        menuItemControl.Enabled = false;
                    }
                    if (!isItemVisible)
                    {
                        menuItemControl.Visible = false;
                    }

                    if (parentAreaCtrl != null)
                    {
                        //menuItemControl.AutoSize = true;
                        parentAreaCtrl.Items.Add(menuItemControl);
                    }
                    else
                    {
                        //parentItemCtrl.BackColor = Color.Yellow;
                        //menuItemControl.AutoSize = true;
                        parentItemCtrl.DropDownItems.Add(menuItemControl);
                    }

                    if (menuItem.Type == (int)LayoutElementType.MenuItem & menuItem.IsLastLevel)
                    {
                        menuItemControl.Click += new System.EventHandler(MenuItemClickHandler);
                    }

                    var subViewMenuItems = _layoutElements.FindAll(x => x.ParentId == menuItem.Id & x.LayoutId == menuItem.LayoutId & x.LayoutType == (int)LayoutType.Menu);
                    foreach (var subViewMenuItem in subViewMenuItems)
                    {
                        RenderHorizonalMenu(null, menuItemControl, subViewMenuItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderHorizonalMenu Error: itemName=" + itemNameForEx + "; " + ex.Message);
            }
        }

        //##RenderVerticalMenuAreaAndItems
        private void RenderVerticalMenuAreaAndItems(MenuFeature menuFeature)

        {
            try
            {
                if (_layoutElements == null) return;

                var menuItems = _layoutElements.FindAll(x =>
                x.Type == (int)LayoutElementType.MenuItem & x.LayoutId == menuFeature.Id & x.LayoutType == (int)LayoutType.Menu);
                if (menuItems.Count == 0) return;

                var regionControl = new Control();
                try
                {
                    regionControl = this.Controls.Find(menuFeature.Container, true)[0];
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("menuFeatureature ContainerName doesn't exist! ContainerName={0}, menuFeatureature.Id={1}, menuFeatureature.Name={2}"
                        .FormatWith(menuFeature.Container, menuFeature.Id, menuFeature.Name)); ;
                }

                var areaName = menuFeature.Container + "Menu" + menuFeature.Id + "Area";
                var menuAreaControl = new Panel();
                menuAreaControl.Name = areaName;
                menuAreaControl.Dock = DockStyle.Fill;
                regionControl.Controls.Add(menuAreaControl);

                var verMenuArea = new LayoutElement();
                verMenuArea.Container = menuFeature.Container;
                verMenuArea.TypeName = "MenuItemContainerArea";
                verMenuArea.ControlTypeName = "Panel";
                verMenuArea.LayoutId = menuFeature.Id;
                verMenuArea.LayoutType = (int)LayoutType.Menu;
                verMenuArea.Name = areaName;
                verMenuArea.Id = -1;
                verMenuArea.IsChecked = true;
                verMenuArea.IsRendered = true;
                verMenuArea.View = "";

                UiHelper.SetLayoutElementTypes(verMenuArea);
                _layoutElements.Add(verMenuArea);

                var treeDataList = new List<TreeItem>();
                foreach (var menuItem in menuItems)
                {
                    var treeItem = new TreeItem();
                    treeItem.Id = menuItem.Id.ToString();
                    treeItem.ParentId = menuItem.ParentId.ToString();
                    treeItem.Name = menuItem.Name;
                    treeItem.DisplayName = menuItem.DisplayName;
                    var imgUrlArry = menuItem.ImageUrl.GetSubParamArray(true, true);
                    treeItem.ImageKey = imgUrlArry.Length > 0 ? imgUrlArry[0] : "";
                    treeItem.SelectedImageKey = imgUrlArry.Length > 1 ? imgUrlArry[1] : "";
                    treeDataList.Add(treeItem);
                }
                var menuControl = new VerticalMenu();
                if (_functionInitParamSet.SupportMultiCultures) menuControl.AnnexList = _annexes.FindAll(x => x.ClassName == "MenuItem");
                menuControl.TopId = "0";
                menuControl.DataSource = treeDataList;
                if (!menuFeature.ImageUrl.IsNullOrEmpty())
                {
                    var imgUrlArry = menuFeature.ImageUrl.GetParamArray(true, false);
                    var dir = FileHelper.GetFilePath(imgUrlArry[0], _functionDir);
                    var fileArray = imgUrlArry[1].GetSubParamArray(true, true);
                    var filesStr = IdentifierHelper.UnwrapSubParamArray(fileArray);
                    var imgUrl = dir + menuFeature.ImageUrl.GetParamSeparator() + filesStr;
                    menuControl.ImageUrl = imgUrl;
                }

                menuControl.Dock = DockStyle.Fill;
                menuControl.OnLastLevelNodeClick += new System.EventHandler(MenuItemClickHandler);
                menuControl.Name = areaName + "-VerticalMenu";
                menuControl.SetNameAsValue = true;
                menuAreaControl.Controls.Add(menuControl);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderVerticalMenuAreaAndItems Error: " + ex.Message);
            }
        }

        //##UpdateNestedViewMenu
        private void UpdateNestedMenu(int menuItemId, int menuFeatureId)
        {
            try
            {
                if (_layoutElements == null) return;

                var subMenuArea = _layoutElements.Find(x => x.ParentId == menuItemId && x.Type == (int)LayoutElementType.MenuItemContainerArea
                & x.LayoutId == menuFeatureId & x.LayoutType == (int)LayoutType.Menu);

                if (subMenuArea == null) //isLastLevel ,-is menuItem not area,
                {
                    var menuItem = _layoutElements.Find(x => x.Id == menuItemId && x.Type == (int)LayoutElementType.MenuItem
                    & x.LayoutId == menuFeatureId & x.LayoutType == (int)LayoutType.Menu);
                    if (menuItem.View.IsNullOrEmpty()) return;
                    var viewFeature = _viewFeatures.Find(x => x.Name == menuItem.View);
                    if (viewFeature.IsPublic) return;

                    //ResizeForm
                    var lastCheckedMenuItem = _layoutElements.Find(x => x.Id == _currentNestedMenuId && x.Type == (int)LayoutElementType.MenuItem
                     & x.LayoutId == menuFeatureId & x.LayoutType == (int)LayoutType.Menu);

                    SwitchView(menuItem.View);
                    _currentNestedMenuId = menuItem.Id;
                }
                else //not last level-is menuItem not area, and has sub area
                {

                    var menuItemsInSubMenuArea = _layoutElements.FindAll(x => x.Container == subMenuArea.Name && x.Type == (int)LayoutElementType.MenuItem
                     & x.LayoutId == menuFeatureId & x.LayoutType == (int)LayoutType.Menu);

                    if (menuItemsInSubMenuArea.Count == 0) return;
                    if (subMenuArea.IsRendered)
                    {
                        if (subMenuArea.ParentId != 0)
                        {
                            ShowNestedMenuArea(subMenuArea);
                        }
                    }
                    else
                    {
                        RenderNestedOrToolBarMenuAreaAndItems(subMenuArea, menuFeatureId);
                    }

                    var checkedmenuItemInSubMenuArea = menuItemsInSubMenuArea.Find(x => x.IsChecked);
                    if (checkedmenuItemInSubMenuArea != null)
                    {
                        var checkedSubMenuItemId = checkedmenuItemInSubMenuArea.Id;
                        UpdateNestedMenu(checkedSubMenuItemId, menuFeatureId);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".UpdateNestedViewMenu Error: menuItemId=" + menuItemId + ", " + ex.Message);
            }
        }

        //##RenderToolBarMenuAreasAndItems
        private void RenderToolBarMenuAreasAndItems(int menuFeatureId)
        {
            try
            {
                var toolBarMenuArea = _layoutElements.Find(x => x.Type == (int)LayoutElementType.MenuItemContainerArea && x.ParentId == 0 && x.LayoutId == menuFeatureId);
                var toolMenuItems = _layoutElements.FindAll(x => x.Container == toolBarMenuArea.Name &&
                (x.Type == (int)LayoutElementType.MenuItem)
                && x.LayoutId == menuFeatureId);
                if (toolMenuItems.Count < 1) return;
                RenderNestedOrToolBarMenuAreaAndItems(toolBarMenuArea, menuFeatureId);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderToolMenuAreaAndItems Error: " + ex.Message);
            }
        }

        //##RenderNestedOrToolBarMenuAreaAndItems
        private void RenderNestedOrToolBarMenuAreaAndItems(LayoutElement menuContainerArea, int menuFeatureId)
        {
            var areaNameForEx = menuContainerArea.Name;
            var itemNameForEx = "";
            try
            {
                if ((menuContainerArea.Type == (int)LayoutElementType.MenuItemContainerArea)
                    && menuContainerArea.ControlTypeName.ToLower() == "ToolStrip".ToLower())
                {
                    var menuContainerAreaControl = new ToolStrip();
                    ControlHelper.SetControlBackColor(menuContainerAreaControl, menuContainerArea.StyleText);

                    var regionControl = new Control();
                    try
                    {
                        regionControl = this.Controls.Find(menuContainerArea.Container, true)[0];
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("viewMenuArea.Container doesn't exist! ctrlName=" + menuContainerArea.Container);
                    }

                    var menuContainerAreaWidth = menuContainerArea.Width == -1 ? regionControl.Width : menuContainerArea.Width;
                    var menuContainerAreaHeight = menuContainerArea.Height == -1 ? regionControl.Height : menuContainerArea.Height;
                    ControlHelper.SetControlDockStyleAndLocationAndSize(menuContainerAreaControl, menuContainerArea.DockType, menuContainerAreaWidth, menuContainerAreaHeight, menuContainerArea.OffsetOrPositionX, menuContainerArea.OffsetOrPositionY);
                    menuContainerAreaControl.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
                    if (menuContainerArea.ImageWidth != -1 && menuContainerArea.ImageHeight != -1)
                        menuContainerAreaControl.ImageScalingSize = new Size(menuContainerArea.ImageWidth, menuContainerArea.ImageHeight);
                    menuContainerAreaControl.AutoSize = false;
                    var autoSizeStr = menuContainerArea.StyleText.GetStyleValue("AutoSize");
                    if (!string.IsNullOrEmpty(autoSizeStr) && autoSizeStr.ToLower() == "true")
                    {
                        menuContainerAreaControl.AutoSize = true;
                    }

                    menuContainerAreaControl.Name = menuContainerArea.Name;
                    menuContainerAreaControl.Tag = menuContainerArea.ParentId == 0 ? "$" + menuContainerArea.Name : menuContainerArea.Name;
                    regionControl.Controls.Add(menuContainerAreaControl);
                    menuContainerArea.IsChecked = true;
                    menuContainerArea.IsRendered = true;

                    var menuItems = _layoutElements.FindAll(x => x.Container == menuContainerArea.Name &&
                    x.Type == (int)LayoutElementType.MenuItem & x.LayoutId == menuContainerArea.LayoutId).ToList();

                    //menuItems.Reverse();
                    menuItems = menuItems.OrderByDescending(x => x.DockOrder).ToList();

                    var defMenuIdStr = menuContainerArea.DefaultSubItemIdFlag;
                    if (string.IsNullOrEmpty(defMenuIdStr) && menuItems.Count > 0)
                    {
                        defMenuIdStr = menuItems.FirstOrDefault().Id.ToString();
                    }


                    foreach (var menuItem in menuItems)
                    {
                        itemNameForEx = menuItem.Name;

                        //--Visible, Enabled
                        bool isItemVisible = true;
                        var itemInvisibleFlag = menuItem.InvisibleFlag;
                        if (string.IsNullOrEmpty(itemInvisibleFlag)) itemInvisibleFlag = "false";
                        else
                        {
                            itemInvisibleFlag = GetText(menuItem.InvisibleFlag);
                        }
                        isItemVisible = (itemInvisibleFlag == "false") ? true : false;

                        bool isItemEnabled = true;
                        var itemDisabledFlag = menuItem.DisabledFlag;
                        if (string.IsNullOrEmpty(itemDisabledFlag)) itemDisabledFlag = "false";
                        else
                        {
                            itemDisabledFlag = GetText(menuItem.DisabledFlag);

                        }
                        isItemEnabled = (itemDisabledFlag == "false") ? true : false;

                        var imageUrl = menuItem.ImageUrl;
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = GetText(imageUrl);
                        }

                        if (menuItem.ControlTypeName.ToLower().Contains("ToolSplitButton".ToLower()))
                        {
                            //*submenuitem
                            var subMenuDir = menuItem.Location + "\\" + menuItem.DataSource;
                            var subMenuItems = UiHelper.GetSubMenuItems(subMenuDir);
                            foreach (var subMenuItem in subMenuItems)
                            {
                                subMenuItem.ImageUrl = ResolveConstants(subMenuItem.ImageUrl);
                                subMenuItem.ImageUrl = FileHelper.GetFilePath(subMenuItem.ImageUrl, subMenuDir);
                            }


                            var subMenuAnnexesCfgFile = subMenuDir + "\\Annexes";
                            var subMenuItemsAnnexes = FunctionHelper.GetAnnexesFromCfgFile(subMenuAnnexesCfgFile, "", false);

                            //_annexes.AddRange(subMenuItemAnnexes);
                            var menuItemControl = new ToolStripSplitButtonEx(subMenuItems, subMenuItemsAnnexes);

                            if (menuItem.ControlTypeName.ToLower() == "ImageToolSplitButton".ToLower())
                            {
                                menuItemControl.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            }
                            //--image
                            if (menuItem.ControlTypeName.ToLower().Contains("image") & !string.IsNullOrEmpty(menuItem.ImageUrl))
                            {
                                var imgUrl = FileHelper.GetFilePath(imageUrl, menuItem.Location);
                                menuItemControl.Image = ControlHelper.GetImage(imgUrl);
                            }
                            //--imagetext relation
                            if (menuItem.ControlTypeName.EndsWith("V")) menuItemControl.TextImageRelation = TextImageRelation.ImageAboveText;
                            else if (menuItem.ControlTypeName.EndsWith("H"))
                            {
                                menuItemControl.TextImageRelation = TextImageRelation.ImageBeforeText;
                            }
                            //--dock, size,offset
                            if (menuItem.DockType == (int)ControlDockType.Right)
                            {
                                menuItemControl.Alignment = ToolStripItemAlignment.Right;
                            }
                            if (menuItem.Width != -1)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Width = menuItem.Width;
                            }
                            if (menuItem.Height != -1)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Height = menuItem.Height;
                            }
                            if (menuItem.ImageWidth != -1 && menuItem.ImageHeight != -1) menuItemControl.ImageScaling = ToolStripItemImageScaling.None;
                            if (menuItem.OffsetOrPositionX != -1)
                            {
                                var subItemOffsetControl = new ToolStripLabel();
                                subItemOffsetControl.AutoSize = false;
                                subItemOffsetControl.Width = menuItem.OffsetOrPositionX;
                                if (menuItem.DockType == (int)ControlDockType.Right)
                                {
                                    menuItemControl.Alignment = ToolStripItemAlignment.Right;
                                    subItemOffsetControl.Alignment = ToolStripItemAlignment.Right;
                                }
                                menuContainerAreaControl.Items.AddRange(new ToolStripItem[] { subItemOffsetControl });
                            }

                            //--name
                            menuItemControl.Name = menuItem.Name;
                            //--displayname, remark
                            menuItemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            menuItemControl.AutoToolTip = false;
                            if (!string.IsNullOrEmpty(menuItem.Remark))
                            {
                                menuItemControl.AutoToolTip = true;
                                menuItemControl.Text = FunctionHelper.GetRemark(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.Remark);
                            }
                            //--font
                            if (menuContainerArea.ParentId == 0)
                            {
                                menuItemControl.ForeColor = StyleSheet.CaptionTextColor;
                                menuItemControl.Font = new Font("", 8, FontStyle.Bold);
                            }
                            //--enable, visible
                            if (!isItemEnabled)
                            {
                                menuItemControl.Enabled = false;
                            }
                            if (!isItemVisible)
                            {
                                menuItemControl.Visible = false;
                            }

                            menuContainerAreaControl.Items.AddRange(new ToolStripItem[] { menuItemControl });

                            //--event
                            menuItemControl.OnMenuItemClick += new System.EventHandler(SubMenuItemClickHandler);
                        }
                        else if (menuItem.ControlTypeName.ToLower().Contains("ToolButton".ToLower()))
                        {
                            var menuItemControl = new ToolStripButton();
                            if (menuItem.ControlTypeName.ToLower() == "ImageToolButton".ToLower())
                            {
                                menuItemControl.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            }
                            //--image
                            if (menuItem.ControlTypeName.ToLower().Contains("image") & !string.IsNullOrEmpty(menuItem.ImageUrl))
                            {
                                var imgUrl = FileHelper.GetFilePath(imageUrl, menuItem.Location);
                                menuItemControl.Image = ControlHelper.GetImage(imgUrl);
                            }
                            //--imagetext relation
                            if (menuItem.ControlTypeName.EndsWith("V")) menuItemControl.TextImageRelation = TextImageRelation.ImageAboveText;
                            else if (menuItem.ControlTypeName.EndsWith("H"))
                            {
                                menuItemControl.TextImageRelation = TextImageRelation.ImageBeforeText;
                            }
                            //--dock, size,offset
                            if (menuItem.DockType == (int)ControlDockType.Right)
                            {
                                menuItemControl.Alignment = ToolStripItemAlignment.Right;
                            }
                            if (menuItem.Width != -1)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Width = menuItem.Width;
                            }
                            if (menuItem.Height != -1)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Height = menuItem.Height;
                            }
                            if (menuItem.ImageWidth != -1 && menuItem.ImageHeight != -1) menuItemControl.ImageScaling = ToolStripItemImageScaling.None;
                            if (menuItem.OffsetOrPositionX != -1)
                            {
                                var subItemOffsetControl = new ToolStripLabel();
                                subItemOffsetControl.AutoSize = false;
                                subItemOffsetControl.Width = menuItem.OffsetOrPositionX;
                                if (menuItem.DockType == (int)ControlDockType.Right)
                                {
                                    menuItemControl.Alignment = ToolStripItemAlignment.Right;
                                    subItemOffsetControl.Alignment = ToolStripItemAlignment.Right;
                                }
                                menuContainerAreaControl.Items.AddRange(new ToolStripItem[] { subItemOffsetControl });
                            }

                            //--name
                            menuItemControl.Name = menuItem.Name;
                            menuItemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            //--displayname, remark
                            menuItemControl.AutoToolTip = false;
                            if (!string.IsNullOrEmpty(menuItem.Remark))
                            {
                                menuItemControl.AutoToolTip = true;
                                menuItemControl.Text = FunctionHelper.GetRemark(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.Remark);
                            }
                            //--font
                            if (menuContainerArea.ParentId == 0)
                            {
                                menuItemControl.ForeColor = StyleSheet.CaptionTextColor;
                                menuItemControl.Font = new Font("", 8, FontStyle.Bold);
                            }
                            //--enable, visible
                            if (!isItemEnabled)
                            {
                                menuItemControl.Enabled = false;
                            }
                            if (!isItemVisible)
                            {
                                menuItemControl.Visible = false;
                            }

                            menuContainerAreaControl.Items.AddRange(new ToolStripItem[] { menuItemControl });

                            var menuType = _menuFeatures.Find(x => x.Id == menuFeatureId).MenuType;
                            //--event 
                            if (menuType == (int)MenuType.ToolBar & menuItem.Type == (int)LayoutElementType.MenuItem)   //for toolbar menu
                            {
                                if (!menuItem.Action.IsNullOrEmpty())
                                    menuItemControl.Click += new System.EventHandler(MenuItemClickHandler);
                            }
                            else if (menuType == (int)MenuType.Nested & menuItem.Type == (int)LayoutElementType.MenuItem) //for nested menu
                            {

                                menuItemControl.Click += new System.EventHandler(MenuItemClickHandler);
                                if (menuItem.Id.ToString() == defMenuIdStr)
                                {
                                    //if (!menuItem.View.IsNullOrEmpty())
                                    {
                                        menuItem.IsChecked = true;
                                        menuItemControl.Checked = true;
                                    }
                                }

                            }
                        }
                        else if (menuItem.ControlTypeName.ToLower() == "ToolLabel".ToLower())
                        {
                            var menuItemControl = new ToolStripLabel();
                            //--dock, size,offset
                            if (menuItem.DockType == (int)ControlDockType.Right)
                            {
                                menuItemControl.Alignment = ToolStripItemAlignment.Right;
                            }
                            if (menuItem.Width != -1)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Width = menuItem.Width;
                            }
                            if (menuItem.Height != -1)
                            {
                                menuItemControl.Height = menuItem.Height;
                            }
                            if (menuItem.OffsetOrPositionX != -1)
                            {
                                var subItemOffsetControl = new ToolStripLabel();
                                subItemOffsetControl.AutoSize = false;
                                subItemOffsetControl.Width = menuItem.OffsetOrPositionX;
                                if (menuItem.DockType == (int)ControlDockType.Right)
                                {
                                    menuItemControl.Alignment = ToolStripItemAlignment.Right;
                                    subItemOffsetControl.Alignment = ToolStripItemAlignment.Right;
                                }
                                menuContainerAreaControl.Items.AddRange(new ToolStripItem[] { subItemOffsetControl });
                            }

                            menuItem.Name = menuContainerArea.Name;

                            //--displayname
                            if (!string.IsNullOrEmpty(menuItem.DisplayName))
                            {
                                menuItemControl.Text = menuItem.DisplayName;
                            }

                            //--visible, enabled
                            if (menuItem.InvisibleFlag == "true")
                            {
                                menuItemControl.Visible = false;
                            }
                            if (menuItem.DisabledFlag == "true")
                            {
                                menuItemControl.Enabled = false;
                            }
                            //--font
                            if (menuContainerArea.ParentId == 0)
                            {
                                menuItemControl.ForeColor = StyleSheet.CaptionTextColor;
                                menuItemControl.Font = new Font("", 8, FontStyle.Regular);
                            }
                            //--enable, visible
                            if (!isItemEnabled)
                            {
                                menuItemControl.Enabled = false;
                            }
                            if (!isItemVisible)
                            {
                                menuItemControl.Visible = false;
                            }

                            menuContainerAreaControl.Items.AddRange(new ToolStripItem[] { menuItemControl });
                        }

                    }
                }
                else if ((menuContainerArea.Type == (int)LayoutElementType.MenuItemContainerArea)
                         && menuContainerArea.ControlTypeName.ToLower() == "Panel".ToLower())
                {

                    //--area
                    var regionControl = this.GetControl(menuContainerArea.Container);

                    var areaWidth = menuContainerArea.Width == -1 ? regionControl.Width : menuContainerArea.Width;
                    var areaHeight = menuContainerArea.Height == -1 ? regionControl.Height : menuContainerArea.Height;
                    var areaControl = new ContainerPanel();
                    areaControl.AutoScroll = true;
                    ControlHelper.SetContainerPanelStyleByClass(areaControl, menuContainerArea.StyleClass);
                    ControlHelper.SetContainerPanelStyleByText(areaControl, menuContainerArea.StyleText);
                    ControlHelper.SetControlDockStyleAndLocationAndSize(areaControl, menuContainerArea.DockType, areaWidth, areaHeight, menuContainerArea.OffsetOrPositionX, menuContainerArea.OffsetOrPositionY);

                    areaControl.Name = menuContainerArea.Name;
                    //areaControl.Padding=new Padding(1,0,0,0);
                    regionControl.Controls.Add(areaControl);
                    menuContainerArea.IsChecked = true;
                    menuContainerArea.IsRendered = true;

                    var defMenuIdStr = menuContainerArea.DefaultSubItemIdFlag;

                    var menuItems = _layoutElements.Where(x => x.Container == menuContainerArea.Name
                        && (x.Type == (int)LayoutElementType.MenuItem)
                        ).ToList();
                    menuItems.Reverse();
                    menuItems = menuItems.OrderBy(x => x.DockOrder).ToList();
                    //menuItems = menuItems.OrderByDescending(x => x.DockOrder).ToList();
                    if (string.IsNullOrEmpty(defMenuIdStr) && menuItems.Count > 0)
                    {
                        defMenuIdStr = menuItems.FirstOrDefault().Id.ToString();
                    }
                    //--items
                    foreach (var menuItem in menuItems)
                    {
                        itemNameForEx = menuItem.Name;
                        //Visible, Enabled
                        bool isItemVisible = true;
                        var itemInvisibleFlag = menuItem.InvisibleFlag;
                        if (string.IsNullOrEmpty(itemInvisibleFlag)) itemInvisibleFlag = "false";
                        else
                        {
                            itemInvisibleFlag = GetText(menuItem.InvisibleFlag);
                        }
                        isItemVisible = (itemInvisibleFlag == "false") ? true : false;

                        bool isItemEnabled = true;
                        var itemDisabledFlag = menuItem.DisabledFlag;
                        if (string.IsNullOrEmpty(itemDisabledFlag)) itemDisabledFlag = "false";
                        else
                        {
                            itemDisabledFlag = GetText(menuItem.DisabledFlag);

                        }
                        isItemEnabled = (itemDisabledFlag == "false") ? true : false;

                        if (menuItem.ControlTypeName.ToLower().StartsWith("ImageTextButton".ToLower()))
                        {
                            var itemControl = new ImageTextButton();
                            itemControl.Name = menuItem.Name;
                            itemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            //--location, size
                            ControlHelper.SetControlDockStyleAndLocationAndSize(itemControl, menuItem.DockType, menuItem.Width, menuItem.Height, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                            if (menuItem.DockType > 0 && menuItem.DockType < 5)
                            {
                                if (menuItem.OffsetOrPositionX != -1 || menuItem.OffsetOrPositionY != -1)
                                {
                                    var offsetCrtl = new Label();
                                    ControlHelper.SetControlOffsetByDockStyle(offsetCrtl, menuItem.DockType, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                                    areaControl.Controls.Add(offsetCrtl);
                                }
                            }
                            else
                            {
                                itemControl.Location = new Point(menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                            }

                            //--image
                            itemControl.ImageWidth = menuItem.ImageWidth;
                            itemControl.ImageHeight = menuItem.ImageHeight;
                            //var imgDir = _functionCfgDir + "\\Images";
                            itemControl.Image = ControlHelper.GetImage(FileHelper.GetFilePath(menuItem.ImageUrl, menuItem.Location));
                            if (menuItem.ControlTypeName.EndsWith("V"))
                            {
                                itemControl.TextImageRelation = TextImageRelation.ImageAboveText;
                                ControlHelper.SetImageTextButtonStyleByClass(itemControl, "VerMenu");

                            }
                            else if (menuItem.ControlTypeName.EndsWith("H"))
                            {
                                itemControl.TextImageRelation = TextImageRelation.ImageBeforeText;
                                itemControl.TextAlign = ContentAlignment.MiddleLeft;
                                ControlHelper.SetImageTextButtonStyleByClass(itemControl, "HorMenu");
                            }

                            ControlHelper.SetImageTextButtonStyleByText(itemControl, menuItem.StyleText);

                            //--enable, visible
                            if (!isItemEnabled)
                            {
                                itemControl.Enabled = false;
                            }
                            if (!isItemVisible)
                            {
                                itemControl.Visible = false;
                            }

                            areaControl.Controls.Add(itemControl);

                            //--event
                            var menuType = _menuFeatures.Find(x => x.Id == menuFeatureId).MenuType;
                            //--event 
                            if (menuType == (int)MenuType.ToolBar & menuItem.Type == (int)LayoutElementType.MenuItem)   //for toolbar menu
                            {
                                if (!menuItem.Action.IsNullOrEmpty())
                                    itemControl.Click += new System.EventHandler(MenuItemClickHandler);
                            }
                            else if (menuType == (int)MenuType.Nested & menuItem.Type == (int)LayoutElementType.MenuItem) //for nested menu
                            {
                                itemControl.SensitiveType = ControlSensitiveType.Check;
                                itemControl.Click += new System.EventHandler(MenuItemClickHandler);
                                if (menuItem.Id.ToString() == defMenuIdStr)
                                {
                                    //if (!menuItem.View.IsNullOrEmpty())
                                    {
                                        menuItem.IsChecked = true;
                                        itemControl.Checked = true;
                                    }

                                }
                            }

                        }
                        else if (menuItem.ControlTypeName.ToLower() == "TextButton".ToLower())
                        {
                            var itemControl = new TextButton();
                            itemControl.Name = menuItem.Name;
                            itemControl.Text = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            ControlHelper.SetTextButtonStyleByClass(itemControl, "Menu");
                            ControlHelper.SetTextButtonStyleByText(itemControl, menuItem.StyleText);
                            //--location, size
                            ControlHelper.SetControlDockStyleAndLocationAndSize(itemControl, menuItem.DockType, menuItem.Width, menuItem.Height, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                            if (menuItem.DockType > 0 && menuItem.DockType < 5)
                            {
                                if (menuItem.OffsetOrPositionX != -1 || menuItem.OffsetOrPositionY != -1)
                                {
                                    var offsetCrtl = new Label();
                                    ControlHelper.SetControlOffsetByDockStyle(offsetCrtl, menuItem.DockType, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                                    areaControl.Controls.Add(offsetCrtl);
                                }
                            }
                            else
                            {
                                itemControl.Location = new Point(menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                            }

                            areaControl.Controls.Add(itemControl);
                            //--enable, visible
                            if (!isItemEnabled)
                            {
                                itemControl.Enabled = false;
                            }
                            if (!isItemVisible)
                            {
                                itemControl.Visible = false;
                            }

                            //--event
                            var menuType = _menuFeatures.Find(x => x.Id == menuFeatureId).MenuType;
                            if (menuType == (int)MenuType.ToolBar & menuItem.Type == (int)LayoutElementType.MenuItem)   //for toolbar menu
                            {
                                itemControl.Click += new System.EventHandler(MenuItemClickHandler);
                            }
                            else if (menuType == (int)MenuType.Nested & menuItem.Type == (int)LayoutElementType.MenuItem) //for nested menu
                            {

                                itemControl.SensitiveType = ControlSensitiveType.Check;
                                itemControl.Click += new System.EventHandler(MenuItemClickHandler);
                                if (menuItem.Id.ToString() == defMenuIdStr)
                                {
                                    //if (!menuItem.View.IsNullOrEmpty())
                                    {
                                        menuItem.IsChecked = true;
                                        itemControl.Checked = true;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderNestedOrToolBarMenuAreaAndItems Error: areaName=" + areaNameForEx + "; itemName=" + itemNameForEx + "; " + ex.Message);
            }
        }

        //##ResetNestedMenuDefaultFlag, only for Nested menu
        private void ResetNestedMenuDefaultFlag(List<LayoutElement> layoutElements, string startViewName)
        {
            var menuItem = layoutElements.Find(x => x.View == startViewName);
            if (menuItem != null)
            {
                var menuAreaName = menuItem.Container;
                var viewMenuArea = layoutElements.Find(x => x.Name == menuAreaName);
                if (viewMenuArea != null)
                {
                    viewMenuArea.DefaultSubItemIdFlag = menuItem.Id.ToString();
                }

                var parentViewMenuId = viewMenuArea.ParentId;
                if (parentViewMenuId != 0)
                {
                    ResetNestedMenuDefaultFlag(layoutElements, parentViewMenuId);
                }
            }
        }

        private void ResetNestedMenuDefaultFlag(List<LayoutElement> layoutElements, int menuId)
        {
            var menuItem = layoutElements.Find(x => x.Id == menuId);
            if (menuItem != null)
            {
                var menuAreaName = menuItem.Container;
                var viewMenuArea = layoutElements.Find(x => x.Name == menuAreaName);
                if (viewMenuArea != null)
                {
                    viewMenuArea.DefaultSubItemIdFlag = menuItem.Id.ToString();
                }

                var parentViewMenuId = viewMenuArea.ParentId;
                if (parentViewMenuId != 0)
                {
                    ResetNestedMenuDefaultFlag(layoutElements, parentViewMenuId);
                }
            }
        }

        //##HideNestedMenuAreas
        private void HideNestedMenuAreas(int menuId)
        {
            var menuArea = _layoutElements.Find(x => x.Type == (int)LayoutElementType.MenuItemContainerArea && x.ParentId == menuId);
            if (menuArea != null)
            {
                var areaControl = GetControl(menuArea.Name);
                ControlHelper.HideControlByDockStyle(areaControl, menuArea.DockType);
                menuArea.IsChecked = false;
                var checkedMenuItem = _layoutElements.Find(x => x.Container == menuArea.Name && x.Type == (int)LayoutElementType.MenuItem && x.IsChecked);
                if (checkedMenuItem != null)
                {
                    HideNestedMenuAreas(checkedMenuItem.Id);
                }
            }
        }

        //##ShowNestedMenuArea
        private void ShowNestedMenuArea(LayoutElement viewMenuArea)
        {
            try
            {
                var containerRegionControl = GetControl(viewMenuArea.Container);
                var viewMenuAreaControl = ControlHelper.GetControlByNameByParent(viewMenuArea.Name, containerRegionControl);
                var viewMenuAreaWidth = viewMenuArea.Width == -1 ? containerRegionControl.Width : viewMenuArea.Width;
                var viewMenuAreaHeight = viewMenuArea.Height == -1 ? containerRegionControl.Height : viewMenuArea.Height;
                ControlHelper.SetControlDockStyleAndLocationAndSize(viewMenuAreaControl, viewMenuArea.DockType, viewMenuAreaWidth, viewMenuAreaHeight, viewMenuArea.OffsetOrPositionX, viewMenuArea.OffsetOrPositionY);
                viewMenuArea.IsChecked = true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ShowNestedMenuArea Error: " + ex.Message);
            }
        }

        //##CheckNestedMenuItemAndUncheckParallelItems
        private void CheckNestedMenuItemAndUncheckParallelItems(int menuId)
        {
            try
            {
                var menuItem = _layoutElements.Find(x => x.Id == menuId && x.Type == (int)LayoutElementType.MenuItem);
                var menuArea = _layoutElements.Find(x => x.Name == menuItem.Container && x.Type == (int)LayoutElementType.MenuItemContainerArea);
                var menuAreaControl = GetControl(menuArea.Name);
                //uncheck
                var lastCheckedParallelMenuItem = _layoutElements.Find(x => x.Container == menuArea.Name && x.Type == (int)LayoutElementType.MenuItem && x.IsChecked);
                if (lastCheckedParallelMenuItem != null)
                {
                    if (lastCheckedParallelMenuItem.ControlTypeName.ToLower().Contains("ImageTextToolButton".ToLower()))
                    {
                        var menuAreaCpnt = menuAreaControl as ToolStrip;
                        var cpnt = menuAreaCpnt.Items.Find(lastCheckedParallelMenuItem.Name, true)[0] as ToolStripButton;
                        cpnt.Checked = false;
                    }
                    else if (lastCheckedParallelMenuItem.ControlTypeName.ToLower().Contains("ImageTextButton".ToLower()))
                    {
                        var menuItemControl = ControlHelper.GetControlByNameByParent(lastCheckedParallelMenuItem.Name, menuAreaControl);
                        var cpnt = menuItemControl as ImageTextButton;
                        cpnt.Checked = false;
                    }
                    else if (lastCheckedParallelMenuItem.ControlTypeName.ToLower().Contains("TextButton".ToLower()))
                    {
                        var menuItemControl = ControlHelper.GetControlByNameByParent(lastCheckedParallelMenuItem.Name, menuAreaControl);
                        var cpnt = menuItemControl as TextButton;
                        cpnt.Checked = false;
                    }
                    lastCheckedParallelMenuItem.IsChecked = false;
                }

                //--check
                if (menuItem.ControlTypeName.ToLower().Contains("ImageTextToolButton".ToLower()))
                {
                    var menuAreaCpnt = menuAreaControl as ToolStrip;
                    var cpnt = menuAreaCpnt.Items.Find(menuItem.Name, true)[0] as ToolStripButton;
                    cpnt.Checked = true;
                }
                else if (menuItem.ControlTypeName.ToLower().Contains("ImageTextButton".ToLower()))
                {
                    var menuItemControl = ControlHelper.GetControlByNameByParent(menuItem.Name, menuAreaControl);
                    var cpnt = menuItemControl as ImageTextButton;
                    cpnt.Checked = true;
                }
                else if (menuItem.ControlTypeName.ToLower().Contains("TextButton".ToLower()))
                {
                    var menuItemControl = ControlHelper.GetControlByNameByParent(menuItem.Name, menuAreaControl);
                    var cpnt = menuItemControl as TextButton;
                    cpnt.Checked = true;
                }
                menuItem.IsChecked = true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".CheckViewMenuItemAndUncheckedParallelItems Error: " + ex.Message);
            }
        }


        //**view
        //##MergeViewItems
        private void MergeViewItems(string viewName)
        {
            var viewNameEx = "";
            try
            {
                viewNameEx = viewName;

                var viewCfgFile = "";

                if (_functionInitParamSet.FormType == FormType.Mvi)
                {
                    var viewFeature = _viewFeatures.Find(x => x.Name == viewName);//Rd?
                    var viewLoc = viewFeature.Location;
                    viewNameEx = viewName;
                    if (!viewLoc.IsNullOrEmpty())
                    {
                        viewCfgFile = FileHelper.GetFilePathByRelativeLocation(viewLoc + "\\Ui", _appCfgUiDir + "\\Views");
                    }
                }
                else //sviView
                {
                    viewCfgFile = FileHelper.GetFilePathByRelativeLocation(_functionInitParamSet.StartSviViewRelativeLocation + "\\Ui", _appCfgUiDir + "\\Views");
                }
                var tmpViewItems = FunctionHelper.GetGenericFromCfgFile<List<LayoutElement>>(viewCfgFile, true) ?? new List<LayoutElement>();

                foreach (var viewItem in tmpViewItems)
                {
                    viewItem.InvalidFlag = ResolveConstants(viewItem.InvalidFlag);
                    if (viewItem.InvalidFlag.StartsWith("=")) viewItem.InvalidFlag = GetText(viewItem.InvalidFlag);
                    viewItem.InvalidFlag = viewItem.InvalidFlag.GetJudgementFlag();

                    viewItem.WriteIntoLogFlag = viewItem.ExecModeFlag.GetJudgementFlag();
                    viewItem.WriteIntoLogFlag = ResolveConstants(viewItem.WriteIntoLogFlag);
                    viewItem.ShowRunningStatusFlag = viewItem.ShowRunningStatusFlag.GetJudgementFlag();
                    viewItem.ShowRunningStatusFlag = ResolveConstants(viewItem.ShowRunningStatusFlag);
                    viewItem.ExecModeFlag = string.IsNullOrEmpty(viewItem.ExecModeFlag) ? "" : viewItem.ExecModeFlag;
                    viewItem.ExecModeFlag = ResolveConstants(viewItem.ExecModeFlag);
                    viewItem.DataSource = string.IsNullOrEmpty(viewItem.DataSource) ? "" : viewItem.DataSource;
                    viewItem.DataSource = ResolveConstants(viewItem.DataSource);
                    viewItem.DisplayName = string.IsNullOrEmpty(viewItem.DisplayName) ? "" : viewItem.DisplayName;
                    viewItem.DisplayName = ResolveConstants(viewItem.DisplayName);
                    viewItem.Location = string.IsNullOrEmpty(viewItem.Location) ? "" : viewItem.Location;
                    viewItem.Location = ResolveConstants(viewItem.Location);
                    viewItem.Action = string.IsNullOrEmpty(viewItem.Action) ? "" : viewItem.Action;
                    viewItem.Action = ResolveConstants(viewItem.Action);
                    viewItem.Trigger = string.IsNullOrEmpty(viewItem.Trigger) ? "" : viewItem.Trigger;
                }

                var viewItems = tmpViewItems.Where(x =>
                    (x.InvalidFlag != "true") &
                    (x.TypeName == LayoutElementType.ContentArea.ToString()
                    | x.TypeName == LayoutElementType.Zone.ToString())
                    | x.TypeName == LayoutElementType.FollowingTransaction.ToString()
                ).ToList();

                if (viewItems.Count > 0)
                {

                    UiHelper.CheckViewItems(viewName, viewItems);
                    foreach (var viewItem in viewItems)
                    {
                        UiHelper.SetLayoutElementTypes(viewItem);
                        viewItem.LayoutType = (int)LayoutType.View;
                        viewItem.IsRendered = false;
                        viewItem.IsChecked = false;
                        viewItem.View = viewName;//??
                        viewItem.Name = viewName + "_" + viewItem.Name;
                        if (viewItem.Type == (int)LayoutElementType.FollowingTransaction)
                        {
                            viewItem.Trigger = viewName + "_" + viewItem.Trigger;
                            viewItem.Action = AddViewIdentifer(viewItem.Action, viewName);
                        }

                        _layoutElements.Add(viewItem);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".MergeViewItems Error: ViewName=" + viewNameEx + "; " + ex.Message);
            }
        }

        //##RenderView
        private void RenderView(string viewName)
        {
            try
            {
                var cttAreas = _layoutElements.FindAll(x => x.View == viewName && (x.Type == (int)LayoutElementType.ContentArea));
                if (cttAreas.Count == 0) throw new ArgumentException("View: '" + viewName + "' has no ContentArea! ");
                var regions = cttAreas.Select(x => x.Container).Distinct();

                //ViewEventHandler(viewName, LayoutElementType.BeforeViewRenderHandler);
                foreach (var region in regions)
                {
                    var regionControl = this.GetControl(region);
                    var regionCttAreas = cttAreas.Where(x => x.Container == region).ToList();

                    foreach (var regionCttArea in regionCttAreas)
                    {
                        RenderContentAreaAndItems(regionCttArea, regionControl);
                    }


                    foreach (var item in regionCttAreas)
                    {
                        var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(item.Name, regionControl);
                        regionControl.Controls.SetChildIndex(ctrl, 0);
                    }
                    var regionCttAreas1 = regionCttAreas.FindAll(x => x.DockType == (int)ControlDockType.Right | x.DockType == (int)ControlDockType.Bottom);
                    regionCttAreas1.Reverse();
                    foreach (var item in regionCttAreas1)
                    {
                        var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(item.Name, regionControl);
                        regionControl.Controls.SetChildIndex(ctrl, 0);
                    }
                }
                //ViewEventHandler(viewName, LayoutElementType.AfterViewRenderHandler);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderView Error: viewName= " + viewName + "; " + ex.Message);
            }
        }

        //##SwitchView
        private void SwitchView(string viewName)
        {
            try
            {
                var viewFeature = _viewFeatures.Find(x => x.Name == viewName);
                if (viewFeature.IsPublic) return;

                if (_currentViewName.IsNullOrEmpty()) //first time
                {
                    var resizeParams = _viewFeatures.Find(x => x.Name == viewName).ResizeParamsText;
                    ResizeForm(resizeParams);

                    MergeViewItems(viewName);
                    RenderView(viewName);
                    _renderedViewNames.Add(viewName);
                    ShowView(viewName);

                    _currentViewName = viewName;

                }
                else
                {
                    if (_currentViewName == viewName) return;

                    var resizeParams = _viewFeatures.Find(x => x.Name == viewName).ResizeParamsText;
                    var lastResizeParams = _viewFeatures.Find(x => x.Name == _currentViewName).ResizeParamsText;
                    if (resizeParams != lastResizeParams)
                        ResizeForm(resizeParams);

                    if (IsViewRendered(viewName))
                    {
                        HideView(_currentViewName);
                        ShowView(viewName);
                    }
                    else
                    {
                        MergeViewItems(viewName);
                        RenderView(viewName);
                        _renderedViewNames.Add(viewName);
                        HideView(_currentViewName);
                        ShowView(viewName);
                    }
                    _currentViewName = viewName;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".SwitchView Error: " + ex.Message);
            }
        }

        //##ShowView
        private void ShowView(string viewName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".ShowView Error: ";

            //--show
            var areas = _layoutElements.FindAll(x => x.View == viewName && x.Type == (int)LayoutElementType.ContentArea);
            foreach (var area in areas)
            {
                var areaContainerControl = new Control();
                try
                {
                    areaContainerControl = this.Controls.Find(area.Container, true)[0];
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo + "area.Container doesn't exist! ctrlName=" + area.Container);
                }

                var areaControl = ControlHelper.GetControlByNameByParent(area.Name, areaContainerControl);
                var areaWidth = area.Width == -1 ? areaContainerControl.Width : area.Width;
                var areaHeight = area.Height == -1 ? areaContainerControl.Height : area.Height;
                ControlHelper.SetControlDockStyleAndLocationAndSize(areaControl, area.DockType, areaWidth, areaHeight, area.OffsetOrPositionX, area.OffsetOrPositionY);
            }

        }

        //##HideView
        private void HideView(string viewName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".HideLastCheckedView Error: ";
            var tmpParallelAreas = _layoutElements.FindAll(x => !x.View.IsNullOrEmpty());//21
            var parallelAreas = tmpParallelAreas.FindAll(x => x.View.ToLower() == viewName.ToLower() & x.Type == (int)LayoutElementType.ContentArea); //ok
            foreach (var parallelArea in parallelAreas)
            {
                var parallelAreaContainerControl = new Control();
                try
                {
                    parallelAreaContainerControl = this.Controls.Find(parallelArea.Container, true)[0];
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo + "parallelArea.Container doesn't exist! ctrlName=" + parallelArea.Container);
                }

                var parallelAreaControl = ControlHelper.GetControlByNameByParent(parallelArea.Name, parallelAreaContainerControl);
                ControlHelper.HideControlByDockStyle(parallelAreaControl, parallelArea.DockType);
            }
        }


        //##IsViewRendered
        private bool IsViewRendered(string viewName)
        {
            if (_renderedViewNames.Count == 0) return false;
            return _renderedViewNames.Contains(viewName);
        }


        //**area
        //##RenderContentAreaAndItems
        private void RenderContentAreaAndItems(LayoutElement cttArea, Control regionControl)
        {
            var areaNameForEx = cttArea.Name;
            var itemNameForEx = "";
            try
            {
                if (cttArea.Type == (int)LayoutElementType.ContentArea)
                {
                    //area
                    //var regionControl = this.GetControl(cttArea.Container);
                    var cttAreaWidth = cttArea.Width == -1 ? regionControl.Width : cttArea.Width;
                    var cttAreaHeight = cttArea.Height == -1 ? regionControl.Height : cttArea.Height;
                    var cttAreaControl = new ContainerPanel();
                    cttAreaControl.AutoScroll = true;
                    ControlHelper.SetContainerPanelStyleByClass(cttAreaControl, cttArea.StyleClass);
                    ControlHelper.SetContainerPanelStyleByText(cttAreaControl, cttArea.StyleText);
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cttAreaControl, cttArea.DockType, cttAreaWidth, cttAreaHeight, cttArea.OffsetOrPositionX, cttArea.OffsetOrPositionY);

                    cttAreaControl.Name = cttArea.Name;
                    regionControl.Controls.Add(cttAreaControl);
                    cttArea.IsChecked = true;
                    cttArea.IsRendered = true;
                    var zones = _layoutElements.Where(x => x.Type == (int)LayoutElementType.Zone && x.View == cttArea.View && x.Container == cttArea.Name.Split('_')[1]
                            ).ToList();

                    //items
                    foreach (var zone in zones)
                    {
                        itemNameForEx = zone.Name;
                        InitZone(zone, cttAreaControl);
                    }


                    foreach (var item in zones)
                    {
                        itemNameForEx = item.Name;
                        var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(item.Name, cttAreaControl);
                        cttAreaControl.Controls.SetChildIndex(ctrl, 0);
                    }
                    var zones1 = zones.FindAll(x => x.DockType == (int)ControlDockType.Right | x.DockType == (int)ControlDockType.Bottom);
                    zones1.Reverse();
                    foreach (var item in zones1)
                    {
                        var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(item.Name, cttAreaControl);
                        cttAreaControl.Controls.SetChildIndex(ctrl, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderContentAreaAndItems Error: areaName=" + areaNameForEx + "; zoneName=" + itemNameForEx + "; " + ex.Message);
            }
        }


        //**zone
        //##InitZone
        private void InitZone(LayoutElement zone, ContainerPanel cttAreaControl)
        {
            var zoneNameForEx = zone.Name;
            try
            {
                var zoneProcedures = GetAndMergeZoneProcedures(zone);
                InitZoneProcess(zoneProcedures);

                MergeZoneItems(zone);
                ZoneEventHandler(zone.Name, ZoneItemType.BeforeZoneRenderHandler);

                var location = zone.Location;

                var cfgFileMgr = new ConfigFileManager(location + "\\Feature");
                var zoneFeature = cfgFileMgr.ConvertToGeneric<ZoneFeature>();
                if (zone.Width < 0) zone.Width = zoneFeature.Width;
                if (zone.Height < 0) zone.Height = zoneFeature.Height;

                if (zoneFeature.CpntArrangementTypeFlag.IsNullOrEmpty()) zoneFeature.CpntArrangementTypeFlag = ZoneCpntArrangementType.Positioning.ToString();
                var zoneCpntArrangementTypeInt = EnumHelper.GetIdByName<ZoneCpntArrangementType>(zoneFeature.CpntArrangementTypeFlag);
                zone.ZoneCpntArrangementType = zoneCpntArrangementTypeInt;

                if (zone.StyleText.IsNullOrEmpty()) zone.StyleText = zoneFeature.StyleText;

                var zoneContainer = new ContainerPanel();
                zoneContainer.AutoScroll = true;
                zoneContainer.Name = zone.Name;
                //zoneContainer.Dock = DockStyle.Left;
                ControlHelper.SetContainerPanelStyleByClass(zoneContainer, zone.StyleClass);
                ControlHelper.SetContainerPanelStyleByText(zoneContainer, zone.StyleText);
                ControlHelper.SetPanelStyleByText(zoneContainer, zone.StyleText);
                var zoneWidth = zone.Width == -1 ? cttAreaControl.Width : zone.Width;
                var zoneHeight = zone.Height == -1 ? cttAreaControl.Height : zone.Height;

                var isPopup = zone.IsPopup;
                if (isPopup)
                {
                    zoneContainer.Location = new Point(3, 3);
                    zoneContainer.Size = new Size(zoneWidth, zoneHeight);
                    var popupContainer = new PopupContainer();
                    popupContainer.Name = zone.Name + "_" + "Container";
                    popupContainer.Width = zone.Width + 9;
                    popupContainer.Height = zone.Height + 9;
                    GroundPanel.Controls.Add(popupContainer);
                    var shadowPanelCtrl = ControlHelper.GetControlByNameByContainer(popupContainer, "shadowPanel");
                    shadowPanelCtrl.Controls.Add(zoneContainer);
                }
                else
                {
                    ControlHelper.SetControlDockStyleAndLocationAndSize(zoneContainer, zone.DockType, zoneWidth, zoneHeight, zone.OffsetOrPositionX, zone.OffsetOrPositionY);
                    cttAreaControl.Controls.Add(zoneContainer);
                }
                RenderZoneItems(zone, zoneContainer);
                //LoadZoneData(zone);

                ZoneEventHandler(zone.Name, ZoneItemType.AfterZoneRenderHandler);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".InitZone Error: zoneName=" + zoneNameForEx + "; " + ex.Message);
            }
        }

        //##GetAndMergeZoneProcedures
        private List<Procedure> GetAndMergeZoneProcedures(LayoutElement zone)
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetAndMergeZoneProcedures Error: ";

            var zoneProcedures = new List<Procedure>();

            var inputTxt = zone.InputVariables;
            if (!inputTxt.IsNullOrEmpty())
            {
                var inputTxtArry = inputTxt.GetSubParamArray(true, false);
                var zinputStr = "";
                for (var i = 0; i < inputTxtArry.Length; i++)
                {
                    var tmp = inputTxtArry[i];
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = ResolveConstants(tmp);
                        tmp = GetText(tmp);
                    }
                    inputTxtArry[i] = tmp;
                }

                zinputStr = IdentifierHelper.UnwrapSubParamArray(inputTxtArry);
                var variableItem = new Procedure();
                variableItem.Name = zone.Name + "_" + "zinput";
                variableItem.Value = zinputStr;
                variableItem.Type = (int)ProcedureType.Params;
                variableItem.DisplayName = "";
                variableItem.ExecModeFlag = "";
                variableItem.ExecOnInitFlag = "";
                variableItem.Formula = "";
                variableItem.ShowRunningStatusFlag = "";
                zoneProcedures.Add(variableItem);
            }

            var zoneProceduresTmp = new List<Procedure>();
            var location = FileHelper.GetFilePath(zone.Location, _zonesDir);
            var cfgFile = location + "\\Process";
            try
            {
                zoneProceduresTmp = FunctionHelper.GetGenericFromCfgFile<List<Procedure>>(cfgFile = location + "\\Process", false) ?? new List<Procedure>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            var zoneProceduresTmp1 = new List<Procedure>();
            foreach (var proc in zoneProceduresTmp)
            {
                proc.InvalidFlag = ResolveConstants(proc.InvalidFlag);
                if (proc.InvalidFlag.StartsWith("=")) proc.InvalidFlag = GetText(proc.InvalidFlag);
                proc.InvalidFlag = proc.InvalidFlag.GetJudgementFlag();
                if (proc.InvalidFlag.ToLower() == "true")
                {
                    continue;
                }
                else
                {
                    zoneProceduresTmp1.Add(proc);
                }
            }
            zoneProceduresTmp1 = zoneProceduresTmp1.FindAll(x => x.TypeName == ProcedureType.Variable.ToString()
                                              | x.TypeName == ProcedureType.Transaction.ToString() | x.TypeName == ProcedureType.SubTransaction.ToString()
                                              | x.TypeName == ProcedureType.Break.ToString() | x.TypeName == ProcedureType.Exit.ToString());
            var annexList = new List<Annex>();
            try
            {
                ProcessHelper.CheckProcedures(zoneProceduresTmp1, zone.Name);
                ProcessHelper.CheckProcedures(zoneProceduresTmp1, zone.Name);
                annexList = FunctionHelper.GetAnnexesFromCfgFile(cfgFile + "Annexes", "Procedure", false);

                foreach (var proc in zoneProceduresTmp1)
                {
                    ProcessHelper.SetProcedureType(proc);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }
            foreach (var proc in zoneProceduresTmp1)
            {
                proc.ExecOnInitFlag = string.IsNullOrEmpty(proc.ExecOnInitFlag) ? "" : proc.ExecOnInitFlag;
                proc.ExecOnInitFlag = ResolveConstants(proc.ExecOnInitFlag);
                proc.WriteIntoLogFlag = proc.WriteIntoLogFlag.GetJudgementFlag();
                proc.WriteIntoLogFlag = string.IsNullOrEmpty(proc.WriteIntoLogFlag) ? "" : proc.WriteIntoLogFlag;
                proc.WriteIntoLogFlag = ResolveConstants(proc.WriteIntoLogFlag);

                proc.ShowRunningStatusFlag = proc.ShowRunningStatusFlag.GetJudgementFlag();
                proc.ShowRunningStatusFlag = ResolveConstants(proc.ShowRunningStatusFlag);
                proc.ExecModeFlag = string.IsNullOrEmpty(proc.ExecModeFlag) ? "" : proc.ExecModeFlag;
                proc.ExecModeFlag = ResolveConstants(proc.ExecModeFlag);

                proc.Value = string.IsNullOrEmpty(proc.Value) ? "" : proc.Value;
                proc.Value = ResolveConstants(proc.Value);
                proc.Formula = string.IsNullOrEmpty(proc.Formula) ? "" : proc.Formula;
                proc.Formula = ResolveConstants(proc.Formula);
                proc.Condition = string.IsNullOrEmpty(proc.Condition) ? "" : proc.Condition;
                proc.Condition = ResolveConstants(proc.Condition);

                proc.WriteIntoLogFlag = proc.WriteIntoLogFlag.GetJudgementFlag();
                proc.ExecModeFlag = string.IsNullOrEmpty(proc.ExecModeFlag) ? "" : proc.ExecModeFlag;

                if (proc.GroupId < 0) proc.GroupId = 0;

                zoneProcedures.Add(proc);


                var tempAnnexes = annexList.FindAll(x => x.MasterName == proc.Name);
                proc.Name = zone.Name + "_" + proc.Name;
                proc.ZoneName = zone.Name;
                proc.Formula = AddZoneIdentifer(proc.Formula, zone.Name);
                proc.Condition = AddZoneIdentifer(proc.Condition, zone.Name);
                proc.WriteIntoLogFlag = AddZoneIdentifer(proc.WriteIntoLogFlag, zone.Name);
                proc.ExecModeFlag = AddZoneIdentifer(proc.ExecModeFlag, zone.Name);

                if (tempAnnexes.Count > 0)
                {
                    foreach (var annex in tempAnnexes)
                    {
                        annex.MasterName = proc.Name;
                        _annexes.Add(annex);
                    }
                }
            }
            var VALUESItem = new Procedure();
            VALUESItem.GroupId = -1;
            VALUESItem.Name = zone.Name + "_" + "VALUES";
            VALUESItem.Type = (int)ProcedureType.Variable;
            VALUESItem.Value = string.Empty;
            VALUESItem.DisplayName = "";
            VALUESItem.ExecModeFlag = "";
            VALUESItem.ExecOnInitFlag = "";
            VALUESItem.Formula = "";
            VALUESItem.ShowRunningStatusFlag = "";
            zoneProcedures.Add(VALUESItem);
            _procedures.AddRange(zoneProcedures);

            return zoneProcedures;
        }

        private void InitZoneProcess(List<Procedure> procedures)
        {
            var exInfo = "\n>> " + GetType().FullName + ".InitZoneProcess Error: ";

            if (procedures.Count == 0) return;

            try
            {
                var procListByGrp = procedures.FindAll(x => x.GroupId == 0 & x.ExecOnInitFlag.ToLower() == "true" &
                x.Type != (int)ProcedureType.SubTransaction && x.Type != (int)ProcedureType.Params && !x.Name.Contains("_VALUES"));
                RefreshProcedures(procListByGrp, procedures);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }
        }

        //#MergeZoneItems
        private void MergeZoneItems(LayoutElement zone)
        {
            try
            {
                var location = FileHelper.GetFilePath(zone.Location, _zonesDir);
                zone.Location = location;
                var cfgFileMgr = new ConfigFileManager(location + "\\ui");
                var tmpZoneItems = FunctionHelper.GetGenericFromCfgFile<List<ZoneItem>>(location + "\\ui", true) ?? new List<ZoneItem>();

                tmpZoneItems = tmpZoneItems.FindAll(x => x.TypeName == ZoneItemType.ControlContainer.ToString() | x.TypeName == ZoneItemType.Control.ToString() | x.TypeName == ZoneItemType.SubControl.ToString() |
                x.TypeName == ZoneItemType.Transaction.ToString() | x.TypeName == ZoneItemType.BeforeZoneRenderHandler.ToString() | x.TypeName == ZoneItemType.AfterZoneRenderHandler.ToString());
                var zoneItems = new List<ZoneItem>();

                foreach (var item in tmpZoneItems)
                {
                    item.InvalidFlag = ResolveConstants(item.InvalidFlag);
                    if (item.InvalidFlag.StartsWith("=")) item.InvalidFlag = GetText(item.InvalidFlag);
                    item.InvalidFlag = item.InvalidFlag.GetJudgementFlag();
                    if (item.InvalidFlag.ToLower() != "true")
                    {
                        zoneItems.Add(item);
                    }
                }

                if (zoneItems.Count > 0)
                {
                    foreach (var item in zoneItems)
                    {
                        UiHelper.SetZoneItemType(item);
                    }
                    UiHelper.CheckZoneItems(zone.Name, zoneItems);
                    var annexList = FunctionHelper.GetAnnexesFromCfgFile(location + "\\Annexes", "ZoneItem", false);

                    foreach (var item in zoneItems)
                    {

                        item.InvisibleFlag = string.IsNullOrEmpty(item.InvisibleFlag) ? "" : item.InvisibleFlag;
                        item.InvisibleFlag = ResolveConstants(item.InvisibleFlag);
                        item.DisabledFlag = string.IsNullOrEmpty(item.DisabledFlag) ? "" : item.DisabledFlag;
                        item.DisabledFlag = ResolveConstants(item.DisabledFlag);




                        item.DisplayName = string.IsNullOrEmpty(item.DisplayName) ? "" : item.DisplayName;
                        item.DisplayName = ResolveConstants(item.DisplayName);
                        item.Value = string.IsNullOrEmpty(item.Value) ? "" : item.Value;
                        item.Value = ResolveConstants(item.Value);
                        item.Action = string.IsNullOrEmpty(item.Action) ? "" : item.Action;
                        item.Action = ResolveConstants(item.Action);
                        item.Action1 = string.IsNullOrEmpty(item.Action1) ? "" : item.Action1;
                        item.Action1 = ResolveConstants(item.Action1);
                        item.DataSource = string.IsNullOrEmpty(item.DataSource) ? "" : item.DataSource;
                        item.DataSource = ResolveConstants(item.DataSource);
                        item.DataSource1 = string.IsNullOrEmpty(item.DataSource1) ? "" : item.DataSource1;
                        item.DataSource1 = ResolveConstants(item.DataSource1);
                        item.DataSource2 = string.IsNullOrEmpty(item.DataSource2) ? "" : item.DataSource2;
                        item.DataSource2 = ResolveConstants(item.DataSource2);
                        item.DataSource3 = string.IsNullOrEmpty(item.DataSource3) ? "" : item.DataSource3;
                        item.DataSource3 = ResolveConstants(item.DataSource3);

                        item.LoadValueOnInitFlag = string.IsNullOrEmpty(item.LoadValueOnInitFlag) ? "" : item.LoadValueOnInitFlag;

                        item.ShowRunningStatusFlag = string.IsNullOrEmpty(item.ShowRunningStatusFlag) ? "" : item.ShowRunningStatusFlag;
                        item.WriteIntoLogFlag = string.IsNullOrEmpty(item.WriteIntoLogFlag) ? "" : item.WriteIntoLogFlag;
                        item.ExecModeFlag = string.IsNullOrEmpty(item.ExecModeFlag) ? "" : item.ExecModeFlag;
                        item.ReplacedTabooIdentifiers = string.IsNullOrEmpty(item.ReplacedTabooIdentifiers) ? "" : item.ReplacedTabooIdentifiers;

                        item.ShowRunningStatusFlag1 = string.IsNullOrEmpty(item.ShowRunningStatusFlag1) ? "" : item.ShowRunningStatusFlag1;
                        item.WriteIntoLogFlag1 = string.IsNullOrEmpty(item.WriteIntoLogFlag1) ? "" : item.WriteIntoLogFlag1;
                        item.ExecModeFlag1 = string.IsNullOrEmpty(item.ExecModeFlag1) ? "" : item.ExecModeFlag1;
                        item.ReplacedTabooIdentifiers1 = string.IsNullOrEmpty(item.ReplacedTabooIdentifiers1) ? "" : item.ReplacedTabooIdentifiers1;


                        var tempAnnexes = annexList.FindAll(x => x.MasterName == item.Name);
                        item.Name = zone.Name + "_" + item.Name;

                        if (item.Type == (int)ZoneItemType.SubControl) item.Container = zone.Name + "_" + item.Container; ;

                        if (!item.Row.IsNullOrEmpty())
                            item.Row = zone.Name + "_" + item.Row;
                        _zonesItems.Add(item);
                        if (tempAnnexes.Count > 0)
                        {
                            foreach (var annex in tempAnnexes)
                            {
                                annex.ClassName = "ZoneItem";
                                annex.MasterName = item.Name;
                                _annexes.Add(annex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new ArgumentException("\n>> " + GetType().FullName + ".MergeZoneItems Error: ZoneName=" + zone.Name + " " + ex.Message);
            }
        }

        //##RenderZoneItems
        private void RenderZoneItems(LayoutElement zone, Control zoneContainer)
        {

            var zonePanel = new Panel();
            zonePanel = zoneContainer as Panel;
            var zoneItems = _zonesItems.FindAll(x => x.Name.StartsWith(zone.Name + "_"));
            var cpntArrangementType = EnumHelper.GetById(zone.ZoneCpntArrangementType, ZoneCpntArrangementType.Positioning);

            var zoneItems1 = zoneItems.Where(x => x.Type == (int)ZoneItemType.Transaction | x.Type == (int)ZoneItemType.BeforeZoneRenderHandler | x.Type == (int)ZoneItemType.AfterZoneRenderHandler);
            foreach (var zoneItem in zoneItems1)
            {
                if (!String.IsNullOrEmpty(zoneItem.Action))
                {
                    zoneItem.Action = AddZoneIdentifer(zoneItem.Action, zone.Name);
                }
            }

            try
            {
                var zoneItems2 = zoneItems.Where(x => x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.ControlContainer).ToList();
                RenderThenArrangeZoneItems(zone, zoneItems2, zonePanel, cpntArrangementType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderZoneItems Error: zoneName=" + zone.Name + ex.Message);
            }
        }

        //##RenderThenArrangeZoneItems
        private void RenderThenArrangeZoneItems(LayoutElement zone, List<ZoneItem> zoneItems, Control container, ZoneCpntArrangementType cpntArrangementType)
        {
            var zoneItemNameEx = "";
            try
            {
                var zoneItems1 = zoneItems.FindAll(x => x.ControlTypeName != ControlType.Row.ToString());
                foreach (var zoneItem in zoneItems1)
                {
                    zoneItemNameEx = zoneItem.Name;
                    RenderZoneItem(zone, zoneItem, container as Panel, cpntArrangementType);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderThenArrangeZoneItems-RenderZoneItems Error: zoneName=" + zone.Name + ", zoneItemName=" + zoneItemNameEx + " " + ex.Message);
            }

            try
            {
                if (cpntArrangementType == ZoneCpntArrangementType.RowLining)
                {
                    int itemDefaultOffsetX = StyleSheet.DefaultOffsetX;
                    int itemDefaultOffsetY = StyleSheet.DefaultOffsetY;
                    int rowDefaultHeight = 2 * StyleSheet.DefaultOffsetY + 20;
                    var rows = zoneItems.FindAll(x => x.ControlTypeName == ControlType.Row.ToString());

                    int rowPosY = 0;
                    foreach (var row in rows)
                    {
                        var rowItems = zoneItems.Where(x => (x.Type == (int)ZoneItemType.SubControl | x.Type == (int)ZoneItemType.Control
                            ) && x.Row == row.Name).ToList();
                        int posX = 0;
                        int lastItemW = 0;
                        rowPosY = rowPosY + row.OffsetOrPositionY;
                        foreach (var rowItem in rowItems)
                        {
                            zoneItemNameEx = rowItem.Name;
                            posX = posX + lastItemW + (rowItem.OffsetOrPositionX < 0 ? itemDefaultOffsetX : rowItem.OffsetOrPositionX);

                            var posY = rowPosY;
                            posY = rowPosY + (rowItem.OffsetOrPositionY < 0 ? 0 : rowItem.OffsetOrPositionY);

                            //--localize and set size for item
                            var ctrlDefWidth = 20;
                            var ctrlDefHeight = 20;
                            var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(rowItem.Name, container);
                            ctrl.Location = new Point(posX, posY);
                            ctrl.Width = rowItem.Width < 0 ? ctrlDefWidth : rowItem.Width;
                            ctrl.Height = rowItem.Height < 0 ? ctrlDefHeight : rowItem.Height;

                            lastItemW = ctrl.Width;
                        }
                        rowPosY = rowPosY + (row.Height < 0 ? rowDefaultHeight : row.Height);
                    }
                }
                else //ZoneArrangementType.Positioning
                {

                    var zoneItems1 = zoneItems.FindAll(x => x.ControlTypeName != ControlType.Row.ToString());
                    foreach (var item in zoneItems1)
                    {
                        zoneItemNameEx = item.Name;
                        var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(item.Name, container);
                        container.Controls.SetChildIndex(ctrl, 0);
                    }

                    var zoneItems2 = zoneItems1.FindAll(x => x.DockType == (int)ControlDockType.Right | x.DockType == (int)ControlDockType.Bottom);
                    zoneItems2.Reverse();
                    foreach (var item in zoneItems2)
                    {
                        zoneItemNameEx = item.Name;
                        var ctrl = ControlHelper.GetControlByNameByParentForRecursionSubCall(item.Name, container);
                        container.Controls.SetChildIndex(ctrl, 0);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderThenArrangeZoneItems-ArrangeZoneItems Error: zoneName=" + zone.Name + ", zoneItemName=" + zoneItemNameEx + " " + ex.Message);
            }
        }

        //*subfunc
        //**gettext
        //##GetText
        private string GetText(string str)
        {
            var funcName = "";
            var returnText = "";
            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }

            if (!str.StartsWith("="))
            {
                return str;
            }

            var repalceTabooIdentifers = false;
            var paramsStr = str.Substring(1, str.Length - 1).Trim();
            if (paramsStr.EndsWith("RTI") | paramsStr.ToLower().EndsWith("RepalceTabooIdentifers".ToLower()))
            {
                var tempParamArr = paramsStr.GetParamArray(true, false);
                var tempParamArr1 = new string[tempParamArr.Length-1];
                for (int i = 0; i < tempParamArr.Length - 1; i++)
                {
                    tempParamArr1[i] = tempParamArr[i];
                }
                paramsStr = IdentifierHelper.UnwrapParamArray(tempParamArr1);
                repalceTabooIdentifers = true;
            }

            var funNameAndParamsArray = paramsStr.GetParamArray(true, false);
            funcName = funNameAndParamsArray[0];
            var funcParamArray = new string[funNameAndParamsArray.Length - 1];
            for (int i = 0; i < funNameAndParamsArray.Length - 1; i++)
            {
                funcParamArray[i] = funNameAndParamsArray[i + 1];
            }

            if (funcName.ToLower() == "GetContentFromChosenTextFile".ToLower() | funcName.ToLower() == "ChooseFile".ToLower() | funcName.ToLower() == "ChooseDirectory".ToLower())
            {
                try
                {
                    returnText = GetText(funcName, funcParamArray);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + ".GetText Error: params=" + str + "; " + ex.Message);
                }
            }
            else
            {
                try
                {
                    Task task = new Task(() =>
                    {
                        returnText = GetText(funcName, funcParamArray);
                    });
                    task.Start();
                    task.Wait();
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + ".GetText Error: params=" + str + "; " + ex.Message);
                }
            }
            if (returnText == "OutOfScope")
            {
                returnText = GetTextEx(funcName, funcParamArray);
            }
            return repalceTabooIdentifers ? returnText.RepalceTabooIdentifers() : returnText;
        }

        //**act
        private void ActByUiElementAndProcedure(string elementName)
        {
            ActByUiElementAndProcedure(elementName, 0);
        }

        private void ActByUiElementAndProcedure(string elementName, int id)
        {
            var action = "";
            var writeIntoLog = false;
            var showRunningStatus = false;
            var displayName = "";
            var replacedIdentifiers = "";
            TransactionExecMode execMode = TransactionExecMode.Sync;
            var followingActions = new List<LayoutElement>();

            try
            {
                if (IdentifierHelper.IsProcessElement(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteProcessIdentifer(elementName);
                    var item = _procedures.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Act Error: ProcessElementName={0} des not exist!", elementName));
                    if (item.Formula.IsNullOrEmpty()) return;
                    action = item.Formula;
                    replacedIdentifiers = item.ReplacedTabooIdentifiers.IsNullOrEmpty() ? "" : item.ReplacedTabooIdentifiers;
                    if (!item.WriteIntoLogFlag.IsNullOrEmpty())
                    {
                        writeIntoLog = item.WriteIntoLogFlag.ToLower() == "true";
                    }
                    if (!item.ShowRunningStatusFlag.IsNullOrEmpty())
                    {
                        showRunningStatus = item.ShowRunningStatusFlag.ToLower() == "true";
                    }
                    if (!item.ExecModeFlag.IsNullOrEmpty())
                    {
                        execMode = EnumHelper.GetByName<TransactionExecMode>(item.ExecModeFlag, execMode);
                    }
                    displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "Procedure", item.Name, _annexes, item.DisplayName);
                    if (displayName.IsNullOrEmpty()) displayName = "#" + item.Name + "#";
                    if (item.Value.IsNullOrEmpty()) return;
                    action = item.Value;
                }
                else if (IdentifierHelper.IsUiElement(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);

                    if (elementName1.GetQtyOfIncludedChar('_') == 0) //menuitem==0 or viewitem==1 (Only for menuitem, Following transaction in view 不会到此处)
                    {
                        var item = _layoutElements.Find(x => x.Name == elementName1);
                        if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Act Error: UiElementName={0} des not exist!", elementName));
                        if (item.Action.IsNullOrEmpty()) return;
                        action = item.Action;
                        replacedIdentifiers = item.ReplacedIdentifiers.IsNullOrEmpty() ? "" : item.ReplacedIdentifiers;
                        writeIntoLog = item.WriteIntoLogFlag.GetJudgementFlag() == "true";
                        showRunningStatus = item.ShowRunningStatusFlag.GetJudgementFlag() == "true";
                        if (!item.ExecModeFlag.IsNullOrEmpty())
                        {
                            execMode = EnumHelper.GetByName<TransactionExecMode>(item.ExecModeFlag, execMode);
                        }

                        if (elementName1.GetQtyOfIncludedChar('_') == 0)
                        {
                            displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "MenuItem", item.Name, _annexes, item.DisplayName);
                            if (displayName.IsNullOrEmpty()) displayName = "$" + item.Name + "$";
                        }

                    }
                    else if (elementName1.GetQtyOfIncludedChar('_') == 2)//zone item
                    {
                        var item = _zonesItems.Find(x => x.Name == elementName1);
                        if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Act Error: UiElementName={0} des not exist!", elementName));
                        if (id == 0)
                        {
                            if (item.Action.IsNullOrEmpty()) return;
                            action = item.Action;
                            replacedIdentifiers = item.ReplacedTabooIdentifiers.IsNullOrEmpty() ? "" : item.ReplacedTabooIdentifiers;
                            writeIntoLog = item.WriteIntoLogFlag.GetJudgementFlag() == "true";
                            showRunningStatus = item.ShowRunningStatusFlag.GetJudgementFlag() == "true";
                            execMode = EnumHelper.GetByName<TransactionExecMode>(item.ExecModeFlag, execMode);
                        }
                        else
                        {
                            if (item.Action1.IsNullOrEmpty()) return;
                            action = item.Action1;
                            replacedIdentifiers = item.ReplacedTabooIdentifiers1;
                            writeIntoLog = item.WriteIntoLogFlag1.GetJudgementFlag() == "true";
                            showRunningStatus = item.ShowRunningStatusFlag1.GetJudgementFlag() == "true";
                            execMode = EnumHelper.GetByName<TransactionExecMode>(item.ExecModeFlag1, execMode);
                        }

                        displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", item.Name, _annexes, item.DisplayName);
                        if (displayName.IsNullOrEmpty()) displayName = "$" + item.Name + "$";
                        followingActions = _layoutElements.FindAll(x => x.Trigger == elementName1 & x.Type == (int)LayoutElementType.FollowingTransaction);
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    return;
                }

                if (action.IsNullOrEmpty()) return;
                if (action.ToLower() == "DoNothing".ToLower()) return;
                var actionNameAndParamsArray = action.GetParamArray(true, false);
                var actionName = actionNameAndParamsArray[0];
                actionName = ResolveStringByRefProcessVariablesAndControls(actionName);
                actionName = GetText(actionName);
                var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    actionParamArray[i] = actionNameAndParamsArray[i + 1].Trim();
                }
                actionParamArray = ParseActionParams(actionName, actionParamArray, replacedIdentifiers);
                var transactionDetail = new TransactionDetail() { ActionName = actionName, ActionParams = actionParamArray, DisplayName = displayName, ExecMode = execMode, ShowRunningStatus = showRunningStatus, WriteIntoLog = writeIntoLog };
                Transact(transactionDetail);
                foreach (var followingAction in followingActions)
                {
                    if (followingAction.Action.IsNullOrEmpty()) break;
                    var actionNameAndParamsArray1 = followingAction.Action.GetParamArray(true, false);
                    var actionName1 = actionNameAndParamsArray1[0].ToLower();
                    actionName1 = ResolveStringByRefProcessVariablesAndControls(actionName1);
                    actionName1 = GetText(actionName1);
                    var actionParamArray1 = new string[actionNameAndParamsArray1.Length - 1];
                    for (int i = 0; i < actionNameAndParamsArray1.Length - 1; i++)
                    {
                        actionParamArray1[i] = actionNameAndParamsArray1[i + 1].Trim();
                    }
                    actionParamArray1 = ParseActionParams(actionName1, actionParamArray1, "");
                    var displayName1 = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ViewItem", followingAction.Name, _annexes, followingAction.DisplayName);
                    if (displayName1.IsNullOrEmpty()) displayName = "$" + followingAction.Name + "$";

                    var writeIntoLog1 = followingAction.WriteIntoLogFlag.GetJudgementFlag() == "true";
                    var showRunningStatus1 = followingAction.ShowRunningStatusFlag.GetJudgementFlag() == "true";
                    var execMode1 = TransactionExecMode.Async;
                    if (!followingAction.ExecModeFlag.IsNullOrEmpty())
                    {
                        execMode1 = EnumHelper.GetByName<TransactionExecMode>(followingAction.ExecModeFlag, execMode1);
                    }

                    var transactionDetail1 = new TransactionDetail() { ActionName = actionName1, ActionParams = actionParamArray1, DisplayName = displayName1, ExecMode = execMode1, ShowRunningStatus = showRunningStatus1, WriteIntoLog = writeIntoLog1 };
                    Transact(transactionDetail1);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".ActByUiElementAndProcedure Error: elementName={0}; action={1} " + ex.Message, elementName, action));
            }
        }


        //--for subMenuItem/contentMenuItem/alienProcess
        private void ActByTransaction(Transaction transaction)
        {
            var action = "";
            var writeIntoLog = false;
            var showRunningStatus = false;
            var displayName = "";
            TransactionExecMode execMode = TransactionExecMode.Sync;
            var controlName = "";
            try
            {
                if (transaction != null)
                {
                    action = transaction.Action;
                    writeIntoLog = transaction.WriteIntoLogFlag.GetJudgementFlag() == "true";
                    showRunningStatus = transaction.ShowRunningStatusFlag.GetJudgementFlag() == "true";
                    if (!transaction.ExecModeFlag.IsNullOrEmpty())
                    {
                        execMode = EnumHelper.GetByName<TransactionExecMode>(transaction.ExecModeFlag, execMode);
                    }
                    displayName = transaction.DisplayName.IsNullOrEmpty() ? "" : transaction.DisplayName;
                    controlName = transaction.ControlName.IsNullOrEmpty() ? "" : transaction.ControlName;
                }
                if (action.IsNullOrEmpty()) return;
                if (action.ToLower() == "DoNothing".ToLower()) return;

                var actionNameAndParamsArray = action.GetParamArray(true, false);
                var actionName = actionNameAndParamsArray[0];
                actionName = ResolveStringByRefProcessVariablesAndControls(actionName);
                actionName = GetText(actionName);
                var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    actionParamArray[i] = actionNameAndParamsArray[i + 1].Trim();
                }


                actionParamArray = ParseActionParams(actionName, actionParamArray, "");
                var transactionDetail = new TransactionDetail() { ActionName = actionName, ActionParams = actionParamArray, DisplayName = displayName, ExecMode = execMode, ShowRunningStatus = showRunningStatus, WriteIntoLog = writeIntoLog };
                Transact(transactionDetail);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".ActByTransaction Error: controlName={0}; action={1} " + ex.Message, controlName, action));
            }
        }

        private string[] ParseActionParams(string actionName, string[] actionParamArray, string replacedIdentifiers)
        {
            try
            {
                if (actionName.ToLower() != "Xrun".ToLower())
                {
                    if (actionName.ToLower() == "Crun".ToLower())
                    {
                        actionParamArray[0] = GetText(ResolveStringByRefProcessVariablesAndControls(actionParamArray[0].Trim()));
                        actionParamArray[0] = actionParamArray[0].RecoverTabooIdentifers(replacedIdentifiers);
                    }
                    else
                    {
                        for (int i = 0; i < actionParamArray.Length; i++)
                        {
                            actionParamArray[i] = GetText(ResolveStringByRefProcessVariablesAndControls(actionParamArray[i].Trim()));
                            if (!replacedIdentifiers.IsNullOrEmpty())
                                actionParamArray[i] = actionParamArray[i].RecoverTabooIdentifers(replacedIdentifiers);
                        }
                    }

                }
                return actionParamArray;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".ParseActionParams Error: controlName={0}; " + ex.Message, actionName));
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //--Please delete all try/catch before Release in front of this point, after full testing!!!//
        //////////////////////////////////////////////////////////////////////////////////////////////

        //**transact
        //##Transact
        private void Transact(TransactionDetail transactionDetail)
        {
            var displayName = transactionDetail.DisplayName;
            var actionName = transactionDetail.ActionName;
            var actionParamArray = transactionDetail.ActionParams;
            var showRunningStatus = transactionDetail.ShowRunningStatus;
            var writeIntoLog = transactionDetail.WriteIntoLog;
            var execMode = transactionDetail.ExecMode;
            if (showRunningStatus)
            {
                RefreshRunningStatusMessage(displayName);
            }
            try
            {
                if (execMode == TransactionExecMode.Sync)
                {
                    Act(actionName, actionParamArray);
                }
                else if (execMode == TransactionExecMode.Async | execMode == TransactionExecMode.AsyncAwaited)
                {
                    Task task = new Task(() =>
                    {
                        Act(actionName, actionParamArray);
                    });
                    task.Start();
                    if (execMode == TransactionExecMode.AsyncAwaited) task.Wait();
                }
                else//managed thread
                {
                    if (_threadPool == null) throw new ArgumentException("Please set \"SupportsMultipleThreads and ThreadPoolMaxNum\" in form config file! ");
                    var transactionParams = new TransactionParams() { ActionName = transactionDetail.ActionName, ActionParams = transactionDetail.ActionParams };
                    _threadPool.Join(ActInThreadPool, transactionDetail.DisplayName, transactionParams);
                }
            }
            catch (Exception ex)
            {
                //--write into error log
            }

            if (showRunningStatus)
            {
                InitRunningStatusMessageComponent();
            }

            if (writeIntoLog)
            {
                //--write into transaction log
            }
        }

        //##ActInThreadPool
        private object ActInThreadPool(Object objParams)
        {
            var transactionParams = objParams as TransactionParams;
            Act(transactionParams.ActionName, transactionParams.ActionParams);
            return null;
        }

        //*common
        //**view
        private string AddViewIdentifer(string str, string viewName)
        {

            if (str.IsNullOrEmpty()) return string.Empty;
            if (str.Contains("$"))
            {
                var strArray = str.Split('$');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '$' no. in " + str + " is not a even! ");
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (strArray[i].IsNullOrEmpty())
                            {
                                strArray[i] = viewName;
                            }
                            else
                                strArray[i] = viewName + "_" + strArray[i];
                        }
                    }
                    str = string.Join("$", strArray);
                }
            }
            return str;
        }

        //**process
        //##RefreshZoneProcedures
        private void RefreshZoneProcess(string zoneName)
        {
            RefreshZoneProcessByGroup(zoneName, 0);
        }

        //##RefreshZoneProceduresByGroup
        private void RefreshZoneProcessByGroup(string zoneName, int grpId)
        {
            if (string.IsNullOrEmpty(zoneName))
            {
                return;
            }

            var procList = _procedures.FindAll(x => x.ZoneName == zoneName);

            if (procList.Count == 0)
            {
                return;
            }

            var procListByGrp = procList.FindAll(x => x.GroupId == grpId);
            if (procList.Count != 0)
            {
                RefreshProcedures(procListByGrp, procList);
            }
        }

        private void RefreshProcedures(List<Procedure> procListByGrp, List<Procedure> procListAll)
        {
            foreach (var proc in procListByGrp)
            {
                var conTxt = ProcessHelper.ResolveStringByRefProcessVariables(proc.Condition, _procedures);
                var con = conTxt.IsNullOrEmpty() ? string.Empty : GetText(conTxt);
                bool toDo = string.IsNullOrEmpty(con) || con.ToLower() == "true";

                if (toDo)
                {
                    if (proc.Type == (int)ProcedureType.Variable)
                    {
                        if (!proc.Formula.IsNullOrEmpty())
                            if (!string.IsNullOrEmpty(proc.Formula))
                            {
                                var formularTxt = ProcessHelper.ResolveStringByRefProcessVariables(proc.Formula, _procedures);
                                proc.Value = GetText(formularTxt);
                            }
                    }
                    else if (proc.Type == (int)ProcedureType.Transaction)
                    {
                        if (!string.IsNullOrEmpty(proc.Formula))
                        {
                            ActByUiElementAndProcedure("#" + proc.Name + "#");
                        }
                    }
                    else if (proc.Type == (int)ProcedureType.Break) break;
                    else if (proc.Type == (int)ProcedureType.Exit) return;
                }
            }
        }

        //##RefreshProcedureVariable
        private void RefreshProcedureVariable(string varName)
        {

            var var = _procedures.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
            if (var == null)
            {
                return;
            }

            var vars = _procedures.FindAll(x => x.ZoneName == var.ZoneName);
            var conTxt = ProcessHelper.ResolveStringByRefProcessVariables(var.Condition, vars);
            var con = GetText(conTxt);
            if (string.IsNullOrEmpty(con) || con.ToLower() == "true")
            {
                var formularTxt = ResolveConstants(var.Formula);
                formularTxt = ProcessHelper.ResolveStringByRefProcessVariables(formularTxt, vars);
                var.Value = GetText(formularTxt);
            }

        }

        //**zone
        //##RenderZoneItem
        private void RenderZoneItem(LayoutElement zone, ZoneItem zoneItem, Panel container, ZoneCpntArrangementType cpntArrangementType)
        {
            //--Visible
            bool isCpntVisible = true;
            var invisibleFlag = zoneItem.InvisibleFlag;
            if (string.IsNullOrEmpty(invisibleFlag)) invisibleFlag = "false";
            else
            {
                zoneItem.InvisibleFlag = AddZoneIdentifer(zoneItem.InvisibleFlag, zone.Name);
                invisibleFlag = ResolveStringByRefProcessVariablesAndControls(zoneItem.InvisibleFlag);
                invisibleFlag = GetText(invisibleFlag);
            }
            isCpntVisible = (invisibleFlag.ToLower() == "false") ? true : false;

            //--Enabled
            bool isCpntEnabled = true;
            var disabledFlag = zoneItem.DisabledFlag;
            if (string.IsNullOrEmpty(disabledFlag)) disabledFlag = "false";
            else
            {
                zoneItem.DisabledFlag = ResolveConstants(zoneItem.DisabledFlag);
                zoneItem.DisabledFlag = AddZoneIdentifer(zoneItem.DisabledFlag, zone.Name);
                disabledFlag = ResolveStringByRefProcessVariablesAndControls(zoneItem.DisabledFlag);
                disabledFlag = GetText(disabledFlag);
            }
            isCpntEnabled = (disabledFlag.ToLower() == "false") ? true : false;

            //--dataSrc
            var dataSrc = "";
            if (!String.IsNullOrEmpty(zoneItem.DataSource))
            {
                zoneItem.DataSource = AddZoneIdentifer(zoneItem.DataSource, zone.Name);
                dataSrc = ResolveStringByRefProcessVariablesAndControls(zoneItem.DataSource);
            }
            var dataSrc1 = "";
            if (!String.IsNullOrEmpty(zoneItem.DataSource1))
            {
                zoneItem.DataSource1 = AddZoneIdentifer(zoneItem.DataSource1, zone.Name);
                dataSrc1 = ResolveStringByRefProcessVariablesAndControls(zoneItem.DataSource1);
            }
            var dataSrc2 = "";
            if (!String.IsNullOrEmpty(zoneItem.DataSource2))
            {
                zoneItem.DataSource2 = AddZoneIdentifer(zoneItem.DataSource2, zone.Name);
                dataSrc2 = ResolveStringByRefProcessVariablesAndControls(zoneItem.DataSource2);
            }
            var dataSrc3 = "";
            if (!String.IsNullOrEmpty(zoneItem.DataSource3))
            {
                zoneItem.DataSource3 = AddZoneIdentifer(zoneItem.DataSource, zone.Name);
                dataSrc3 = ResolveStringByRefProcessVariablesAndControls(zoneItem.DataSource3);
            }

            //--defVal
            var defVal = "";
            if (!String.IsNullOrEmpty(zoneItem.Value))
            {
                zoneItem.Value = AddZoneIdentifer(zoneItem.Value, zone.Name);
                var lodaValueOnInit = false;
                if (!zoneItem.LoadValueOnInitFlag.IsNullOrEmpty())
                {
                    if (zoneItem.LoadValueOnInitFlag.ToLower() == "true")
                        lodaValueOnInit = true;
                }
                if (lodaValueOnInit)
                {
                    defVal = ResolveStringByRefProcessVariablesAndControls(zoneItem.Value);
                    if (defVal.StartsWith("="))
                        defVal = GetText(defVal);
                }

            }

            //--diplayName
            var displayName = "";
            if (!String.IsNullOrEmpty(zoneItem.DisplayName))
            {
                zoneItem.DisplayName = ResolveConstants(zoneItem.DisplayName);
                zoneItem.DisplayName = AddZoneIdentifer(zoneItem.DisplayName, zone.Name);
                zoneItem.DisplayName = ResolveStringByRefProcessVariablesAndControls(zoneItem.DisplayName);
                if (zoneItem.DisplayName.StartsWith("="))
                {
                    displayName = GetText(zoneItem.DisplayName);
                }
                else
                {
                    displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                }
            }
            else
            {
                displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", zoneItem.Name, _annexes, "");
            }

            //--action
            if (!String.IsNullOrEmpty(zoneItem.Action))
            {
                zoneItem.Action = AddZoneIdentifer(zoneItem.Action, zone.Name);
            }

            //--action1
            if (!String.IsNullOrEmpty(zoneItem.Action1))
            {
                zoneItem.Action1 = AddZoneIdentifer(zoneItem.Action1, zone.Name);
            }


            //--styleClass
            var styleClass = "";
            if (!String.IsNullOrEmpty(zoneItem.StyleClass))
            {
                zoneItem.StyleClass = ResolveConstants(zoneItem.StyleClass);
                zoneItem.StyleClass = AddZoneIdentifer(zoneItem.StyleClass, zone.Name);
                styleClass = ResolveStringByRefProcessVariablesAndControls(zoneItem.StyleClass);
                styleClass = GetText(styleClass);
            }

            //--styleText
            var styleText = "";
            if (!String.IsNullOrEmpty(zoneItem.StyleText))
            {
                zoneItem.StyleText = ResolveConstants(zoneItem.StyleText);
                zoneItem.StyleText = AddZoneIdentifer(zoneItem.StyleText, zone.Name);
                styleText = ResolveStringByRefProcessVariablesAndControls(zoneItem.StyleText);
                styleText = GetText(styleText);
            }

            //--render item
            //--container
            if (zoneItem.ControlTypeName == ControlType.Panel.ToString())
            {
                var cpnt = new Panel();
                cpnt.Name = zoneItem.Name;
                cpnt.AutoScroll = false;
                ControlHelper.SetPanelStyleByClass(cpnt, styleClass);
                ControlHelper.SetPanelStyleByText(cpnt, styleText);
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                if (zoneItem.Type == (int)ZoneItemType.ControlContainer)
                {
                    var cpntArrangeType = ZoneCpntArrangementType.Positioning;
                    var cpntArrangeTypeStyleText = zoneItem.StyleText.GetStyleValue("CpntArrangeType");
                    cpntArrangeType = EnumHelper.GetByName(cpntArrangeTypeStyleText, cpntArrangeType);
                    var subZoneItems = _zonesItems.FindAll(x => x.Container == zoneItem.Name && x.Type == (int)ZoneItemType.SubControl);
                    if (subZoneItems.Count > 0) RenderThenArrangeZoneItems(zone, subZoneItems, cpnt, cpntArrangeType);
                }
            }
            else if (zoneItem.ControlTypeName == ControlType.ContainerPanel.ToString())
            {
                var cpnt = new ContainerPanel();
                cpnt.Name = zoneItem.Name;
                cpnt.AutoScroll = false;
                ControlHelper.SetContainerPanelStyleByClass(cpnt, styleClass);
                ControlHelper.SetContainerPanelStyleByText(cpnt, styleText);
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                if (zoneItem.Type == (int)ZoneItemType.ControlContainer)
                {
                    var cpntArrangeType = ZoneCpntArrangementType.Positioning;
                    var cpntArrangeTypeStyleText = zoneItem.StyleText.GetStyleValue("CpntArrangeType");
                    cpntArrangeType = EnumHelper.GetByName(cpntArrangeTypeStyleText, cpntArrangeType);
                    var subZoneItems = _zonesItems.FindAll(x => x.Container == zoneItem.Name && x.Type == (int)ZoneItemType.SubControl);
                    if (subZoneItems.Count > 0) RenderThenArrangeZoneItems(zone, subZoneItems, cpnt, cpntArrangeType);
                }
            }
            else if (zoneItem.ControlTypeName == ControlType.ShadowPanel.ToString())
            {
                var cpnt = new ShadowPanel();
                cpnt.Name = zoneItem.Name;

                ControlHelper.SetShadowPanelStyleByClass(cpnt, styleClass);
                ControlHelper.SetShadowPanelStyleByText(cpnt, styleText);
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                if (zoneItem.Type == (int)ZoneItemType.ControlContainer)
                {
                    var cpntArrangeType = ZoneCpntArrangementType.Positioning;
                    var cpntArrangeTypeStyleText = zoneItem.StyleText.GetStyleValue("CpntArrangeType");
                    cpntArrangeType = EnumHelper.GetByName(cpntArrangeTypeStyleText, cpntArrangeType);
                    var subZoneItems = _zonesItems.FindAll(x => x.Container == zoneItem.Name && x.Type == (int)ZoneItemType.SubControl);
                    if (subZoneItems.Count > 0) RenderThenArrangeZoneItems(zone, subZoneItems, cpnt, cpntArrangeType);
                }
            }
            //--container ends

            else if (zoneItem.ControlTypeName == ControlType.SplitRectangle.ToString())
            {
                var cpnt = new SplitRectangle();
                cpnt.Name = zoneItem.Name;

                ControlHelper.SetSplitRectangleStyleByClass(cpnt, styleClass);
                ControlHelper.SetSplitRectangleStyleByText(cpnt, styleText);
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.GroupBox.ToString())
            {
                var cpnt = new GroupBox();
                cpnt.Name = zoneItem.Name;

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.Label.ToString())
            {
                var cpnt = new Label();
                cpnt.TextAlign = ContentAlignment.MiddleLeft;
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                ControlHelper.SetLabelStyleByClass(cpnt, styleClass);
                ControlHelper.SetLabelStyleByText(cpnt, styleText);
                if (zoneItem.Width != -1) cpnt.AutoSize = false;
                else
                {
                    cpnt.AutoSize = true;
                    cpnt.Width = 1;
                }

                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.Click += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.TitleLabel.ToString())
            {
                var cpnt = new TitleLabel();
                cpnt.TextAlign = ContentAlignment.MiddleLeft;
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                cpnt.TextAlign = ContentAlignment.MiddleLeft;
                ControlHelper.SetTitleLabelStyleByClass(cpnt, styleClass);
                ControlHelper.SetTitleLabelStyleByText(cpnt, styleText);

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.CommandLabel.ToString())
            {
                var cpnt = new CommandLabel();
                cpnt.TextAlign = ContentAlignment.MiddleLeft;
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                cpnt.TextAlign = ContentAlignment.MiddleLeft;
                ControlHelper.SetCommandLabelStyleByClass(cpnt, styleClass);
                ControlHelper.SetCommandLabelStyleByText(cpnt, styleText);

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.Click += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.StatusLight.ToString())
            {
                var cpnt = new StatusLight();
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                ControlHelper.SetStatusLightStyleByClass(cpnt, styleClass);
                ControlHelper.SetStatusLightStyleByText(cpnt, styleText);

                if (!defVal.IsNullOrEmpty())
                {
                    cpnt.Value = Convert.ToInt16(defVal);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = true;

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.OnLightClick += new System.EventHandler(ControlEventHandler);
                }

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);
            }
            else if (zoneItem.ControlTypeName == ControlType.ScoreLight.ToString())
            {
                var cpnt = new ScoreLight();
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                ControlHelper.SetScoreLightStyleByClass(cpnt, styleClass);
                ControlHelper.SetScoreLightStyleByText(cpnt, styleText);

                if (!defVal.IsNullOrEmpty())
                {
                    cpnt.Value = Convert.ToSingle(defVal);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = true;

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.OnLightClick += new System.EventHandler(ControlEventHandler);
                }

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.PictureBox.ToString())
            {
                var cpnt = new PictureBox();
                cpnt.Name = zoneItem.Name;
                var location = zone.Location;
                var imagUrl = FileHelper.GetFilePath(GetText(dataSrc), location);

                ControlHelper.SetControlBackgroundImage(cpnt, imagUrl);

                //--event
                if (!String.IsNullOrEmpty(zoneItem.DisplayName))
                {
                    cpnt.Tag = displayName;
                    cpnt.MouseHover += new System.EventHandler(ControlHoverHandler);
                }
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.Click += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.TextButton.ToString())
            {
                var cpnt = new TextButton();
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                ControlHelper.SetTextButtonStyleByClass(cpnt, styleClass);
                ControlHelper.SetTextButtonStyleByText(cpnt, styleText);

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.Click += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.ImageTextButtonH.ToString() | zoneItem.ControlTypeName == ControlType.ImageTextButtonV.ToString())
            {
                var cpnt = new ImageTextButton();
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;

                //--image
                var imgDir = zone.Location;
                cpnt.ImageWidth = zoneItem.Width;
                cpnt.ImageHeight = zoneItem.Height;

                if (!dataSrc.IsNullOrEmpty())
                {
                    if (dataSrc.StartsWith("="))
                    {
                        dataSrc = GetText(dataSrc);
                    }
                    var imgPath = FileHelper.GetFilePath(GetText(dataSrc), imgDir);
                    cpnt.Image = ControlHelper.GetImage(imgPath);
                }

                if (zoneItem.ControlTypeName.EndsWith("V"))
                {
                    cpnt.TextImageRelation = TextImageRelation.ImageAboveText;
                    cpnt.TextAlign = ContentAlignment.MiddleCenter;
                }
                else if (zoneItem.ControlTypeName.EndsWith("H"))
                {
                    cpnt.TextImageRelation = TextImageRelation.ImageBeforeText;
                    cpnt.TextAlign = ContentAlignment.MiddleLeft;
                }
                ControlHelper.SetImageTextButtonStyleByClass(cpnt, styleClass);
                ControlHelper.SetImageTextButtonStyleByText(cpnt, styleText);

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.Click += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.RadioButton.ToString())
            {
                var cpnt = new RadioButton();
                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                if (!defVal.IsNullOrEmpty())
                {
                    if (defVal.ToLower() == "true") cpnt.Checked = true;
                    else cpnt.Checked = false;
                }
                ControlHelper.SetRadioButtonStyleByClass(cpnt, styleClass);
                ControlHelper.SetRadioButtonStyleByText(cpnt, styleText);
                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.Click += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.CheckBox.ToString())
            {
                var cpnt = new CheckBox();

                cpnt.Name = zoneItem.Name;
                cpnt.Text = displayName;
                if (!defVal.IsNullOrEmpty())
                {
                    if (defVal.ToLower() == "true") cpnt.Checked = true;
                    else cpnt.Checked = false;
                }
                ControlHelper.SetCheckBoxStyleByClass(cpnt, styleClass);
                ControlHelper.SetCheckBoxStyleByText(cpnt, styleText);
                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.CheckedChanged += new System.EventHandler(ControlEventHandler);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.TextBox.ToString())
            {
                var cpnt = new TextBox();
                cpnt.Name = zoneItem.Name;
                var type = zoneItem.StyleText.GetStyleValue("Type");
                if (type.ToLower() == "password".ToLower())
                {
                    cpnt.PasswordChar = '*';
                }
                if (!defVal.IsNullOrEmpty()) cpnt.Text = defVal;

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.TextChanged += new System.EventHandler(ControlEventHandler);
                }
                if (!zoneItem.Action1.IsNullOrEmpty())
                {
                    cpnt.DoubleClick += new System.EventHandler(ControlEventHandler1);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.ReadOnly = true;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.RichTextBox.ToString())
            {
                var cpnt = new RichTextBox();
                cpnt.Name = zoneItem.Name;
                if (!defVal.IsNullOrEmpty()) cpnt.Text = defVal;

                ControlHelper.SetRichTextBoxStyleByClass(cpnt, styleClass);
                ControlHelper.SetRichTextBoxStyleByText(cpnt, styleText);
                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.TextChanged += new System.EventHandler(ControlEventHandler);
                }
                if (!zoneItem.Action1.IsNullOrEmpty())
                {
                    cpnt.DoubleClick += new System.EventHandler(ControlEventHandler1);
                }

                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.ReadOnly = true;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }
            else if (zoneItem.ControlTypeName == ControlType.ProgressBar.ToString())
            {
                var cpnt = new ProgressBar();
                cpnt.Name = zoneItem.Name;
                if (!defVal.IsNullOrEmpty()) cpnt.Value = Convert.ToInt16(defVal);

                ControlHelper.SetProgressBarStyleByClass(cpnt, styleClass);
                ControlHelper.SetProgressBarStyleByText(cpnt, styleText);
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

            }

            else if (zoneItem.ControlTypeName == ControlType.ComboBox.ToString())//ComboBox
            {
                var cpnt = new ComboBox();
                cpnt.Name = zoneItem.Name;
                var style = zoneItem.StyleText.GetStyleValue("Style");
                if (style.ToLower() == "DropDown".ToLower())
                {
                    cpnt.DropDownStyle = ComboBoxStyle.DropDown;
                    cpnt.SelectionLength = 0;
                }
                else
                {
                    cpnt.DropDownStyle = ComboBoxStyle.DropDownList;
                }

                //--DataSource
                if (!dataSrc.IsNullOrEmpty())
                {
                    var valTxts = new List<ValueText>();
                    if (dataSrc.StartsWith("="))
                    {
                        //*ov-
                        dataSrc = GetText(dataSrc);
                        if (dataSrc.IsListJson())
                        {
                            valTxts = JsonHelper.ConvertToGeneric<List<ValueText>>(dataSrc);
                        }
                    }
                    else if (dataSrc.ContainsSubParamSeparator())
                    {
                        var strArray = dataSrc.GetSubParamArray(true, false);
                        if (dataSrc.Contains("`"))//xml.to generic not support & <
                        {
                            var i = 0;
                            foreach (var v in strArray)
                            {
                                var arry = v.Split('`');
                                var valTxt = new ValueText();
                                valTxt.Value = arry[0];
                                valTxt.Text = arry[1];
                                valTxts.Add(valTxt);
                                i++;
                            }
                        }
                        else
                        {
                            var i = 0;
                            foreach (var v in strArray)
                            {
                                var valTxt = new ValueText();
                                valTxt.Value = i.ToString();
                                valTxt.Text = v;
                                valTxts.Add(valTxt);
                                i++;
                            }
                        }
                    }

                    else
                    {
                        var cfgFile = FileHelper.GetFilePath(dataSrc, zone.Location);
                        valTxts = new ConfigFileManager(cfgFile).ConvertToGeneric<List<ValueText>>();
                    }

                    cpnt.DataSource = valTxts;
                    cpnt.ValueMember = "Value";
                    cpnt.DisplayMember = "Text";
                }

                container.Controls.Add(cpnt); //for ComboBox, this sentence must be before setting default value! else the selected value will be first one
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);


                //--DefaultValue
                if (style.ToLower() == "DropDown".ToLower())
                {
                    if (!defVal.IsNullOrEmpty())
                    {
                        cpnt.SelectedValue = defVal;
                    }
                    else
                        cpnt.SelectedIndex = -1;
                }
                else if (!defVal.IsNullOrEmpty())
                {
                    cpnt.SelectedValue = defVal;
                }

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.SelectedIndexChanged += new System.EventHandler(ControlEventHandler);
                }
                //--Visible
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;
            }

            //--entctrls
            else if (zoneItem.ControlTypeName == ControlType.TransactionBox.ToString())
            {
                var cpnt = new CommandTextBox();
                cpnt.Name = zoneItem.Name;
                if (!isCpntVisible) cpnt.Visible = false;
                if (!isCpntEnabled) cpnt.Enabled = false;
                ControlHelper.SetCommandTextBoxStyleByClass(cpnt, styleClass);
                ControlHelper.SetCommandTextBoxStyleByText(cpnt, styleText);

                if (!defVal.IsNullOrEmpty())
                {
                    cpnt.Text = defVal;
                }

                container.Controls.Add(cpnt);
                if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                    ControlHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                //--event
                if (!zoneItem.Action.IsNullOrEmpty())
                {
                    cpnt.OnEnterKeyDown += new System.EventHandler(ControlEventHandler);
                }
            }

        }

        private void SetZoneCpntDockStyleOrPositionOnZoneArrangementTypeIsPositioning(Panel zonePanel, Control ctrl, int dockType, int width, int height, int offsetOrPositionX, int offsetOrPositionY)
        {
            //if (zoneArrangementType == ZoneCpntArrangementType.Positioning)
            {
                ControlHelper.SetControlDockStyleAndLocationAndSize(ctrl, dockType, width, height, offsetOrPositionX, offsetOrPositionY);
                //if (dockType > 0 && dockType < 5)
                //{
                //    if (offsetOrPositionX > 0 || offsetOrPositionY > 0)
                //    {
                //        var offsetCrtl = new Label();
                //        ControlHelper.SetControlOffsetByDockStyle(offsetCrtl, dockType, offsetOrPositionX, offsetOrPositionY);
                //        zonePanel.Controls.Add(offsetCrtl);
                //    }
                //}
            }

        }


        //##AddZoneIdentifer
        private string AddZoneIdentifer(string str, string zoneName)
        {

            if (str.IsNullOrEmpty()) return string.Empty;
            var str1 = str;
            if (str.Contains("#"))
            {
                var strArray = str.Split('#');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '#' no. in " + str + " is not a even! ");
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (strArray[i].IsNullOrEmpty() | strArray[i] == ".")
                            {
                                strArray[i] = zoneName;
                            }
                            else
                                strArray[i] = zoneName + "_" + strArray[i];
                        }
                    }
                    str1 = string.Join("#", strArray);
                }
            }

            var str2 = str1;
            if (str2.Contains("$"))
            {
                var strArray = str2.Split('$');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '$' no. in " + str + " is not a even! ");
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (strArray[i].IsNullOrEmpty())
                            {
                                strArray[i] = zoneName;
                            }
                            else
                                strArray[i] = zoneName + "_" + strArray[i];
                        }
                    }
                    str2 = string.Join("$", strArray);
                }
            }
            return str2;
        }


        //**ctrl
        //**control

        //##RefreshZoneControlsValues
        private void RefreshZoneControlsValues(string zoneName)
        {
            var zoneItems = _zonesItems.FindAll(x => x.Name.StartsWith(zoneName + "_"));
            var items = zoneItems.Where(x => x.ControlTypeName.ToLower() != "row"
                                             & (x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.SubControl)
            ).ToList();
            foreach (var item in items)
            {
                RefreshControlValue(item.Name);
            }
        }

        //##RefreshZoneControlsInvisibles
        private void RefreshZoneControlsInvisibles(string zoneName)
        {
            var zoneItems = _zonesItems.FindAll(x => x.Name.StartsWith(zoneName + "_"));
            var items = zoneItems.Where(x => x.ControlTypeName.ToLower() != "row"
                                             & (x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.SubControl)
            ).ToList();
            foreach (var item in items)
            {
                RefreshControlInvisible(item.Name);
            }
        }

        //##RefreshZoneControlsDisableds
        private void RefreshZoneControlsDisableds(string zoneName)
        {
            //zoneName = ReplaceAbbreviatedCurrentZoneName(zoneName);
            var zoneItems = _zonesItems.FindAll(x => x.Name.StartsWith(zoneName + "_"));
            var items = zoneItems.Where(x => x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.SubControl
            ).ToList();
            foreach (var item in items)
            {
                RefreshControlDisabled(item.Name);
            }
        }

        //##RefreshControlValue
        private void RefreshControlValue(string ctrlName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".RefreshControlValue Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //defVal
            var defVal = "";
            if (!String.IsNullOrEmpty(item.Value))
            {
                var txt = ResolveStringByRefProcessVariablesAndControls(item.Value);
                defVal = GetText(txt);
            }

            SetControlValue(ctrlName, defVal);
        }

        //##SetControlValue
        private void SetControlValue(string ctrlName, string val)
        {
            var exInfo = "\n>> " + GetType().FullName + ".SetControlValue Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //--refresh item
            var ctrl = this.GetControl(item.Name);

            if (item.ControlTypeName == ControlType.RadioButton.ToString())
            {
                var cpnt = ctrl as RadioButton;
                if (val.ToLower() == "true")
                {
                    cpnt.Checked = true;
                }
                else
                {
                    cpnt.Checked = false;
                }
            }
            else if (item.ControlTypeName == ControlType.CheckBox.ToString())
            {
                var cpnt = ctrl as CheckBox;
                if (val.ToLower() == "true")
                {
                    cpnt.Checked = true;
                }
                else
                {
                    cpnt.Checked = false;
                }
            }
            else if (item.ControlTypeName == ControlType.TextBox.ToString())
            {
                var cpnt = ctrl as TextBox;
                cpnt.Text = val;
            }

            else if (item.ControlTypeName == ControlType.RichTextBox.ToString())
            {
                var cpnt = ctrl as RichTextBox;
                cpnt.Text = val;
            }
            else if (item.ControlTypeName == ControlType.ComboBox.ToString())
            {
                var cpnt = ctrl as ComboBox;
                cpnt.SelectedValue = val;
            }
            else if (item.ControlTypeName == ControlType.StatusLight.ToString())
            {
                var cpnt = ctrl as StatusLight;
                cpnt.Value = Convert.ToInt16(val);
            }
            else if (item.ControlTypeName == ControlType.ScoreLight.ToString())
            {
                var cpnt = ctrl as ScoreLight;
                cpnt.Value = Convert.ToSingle(val, CultureInfo.InvariantCulture);
            }
            else if (item.ControlTypeName == ControlType.ProgressBar.ToString())
            {
                var cpnt = ctrl as ProgressBar;
                cpnt.Value = Convert.ToInt32(val);
            }

            else if (item.ControlTypeName == ControlType.TransactionBox.ToString())
            {
                var cpnt = ctrl as CommandTextBox;
                cpnt.Text = val;
            }


        }

        //##SetControlValue
        private string GetControlValue(string ctrlName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetControlValue Error: ";

            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //--refresh item
            var ctrl = this.GetControl(item.Name);

            var retStr = "";

            if (item.ControlTypeName == ControlType.RadioButton.ToString())
            {
                var cpnt = ctrl as RadioButton;
                retStr = (cpnt.Checked) ? "true" : "false";
            }
            else if (item.ControlTypeName == ControlType.CheckBox.ToString())
            {
                var cpnt = ctrl as CheckBox;
                retStr = (cpnt.Checked) ? "true" : "false";
            }
            else if (item.ControlTypeName == ControlType.TextBox.ToString())
            {
                var cpnt = ctrl as TextBox;
                retStr = cpnt.Text;
            }

            else if (item.ControlTypeName == ControlType.RichTextBox.ToString())
            {
                var cpnt = ctrl as RichTextBox;
                retStr = cpnt.Text;
            }
            else if (item.ControlTypeName == ControlType.ComboBox.ToString())
            {
                var cpnt = ctrl as ComboBox;
                retStr = cpnt.SelectedValue.ToString();
            }
            else if (item.ControlTypeName == ControlType.StatusLight.ToString())
            {
                var cpnt = ctrl as StatusLight;
                retStr = cpnt.Value.ToString();
            }
            else if (item.ControlTypeName == ControlType.ScoreLight.ToString())
            {
                var cpnt = ctrl as ScoreLight;
                retStr = Convert.ToString(cpnt.Value, CultureInfo.InvariantCulture);
            }
            else if (item.ControlTypeName == ControlType.ProgressBar.ToString())
            {
                var cpnt = ctrl as ProgressBar;
                retStr = cpnt.Value.ToString();
            }

            else if (item.ControlTypeName == ControlType.TransactionBox.ToString())
            {
                var cpnt = ctrl as CommandTextBox;
                retStr = cpnt.Text;
            }


            return retStr;
        }

        //##RefreshControlText
        private void RefreshControlText(string ctrlName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".RefreshControlText Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);
            var text = "";
            if (!item.DisplayName.IsNullOrEmpty())
            {
                var txt = ResolveStringByRefProcessVariablesAndControls(item.DisplayName);
                text = GetText(txt);
            }

            if (item.ControlTypeName == ControlType.PictureBox.ToString())
            {
                if (!item.DisplayName.IsNullOrEmpty())
                {
                    if (item.DisplayName.StartsWith("="))
                    {
                        var txt = ResolveStringByRefProcessVariablesAndControls(item.DisplayName);
                        text = GetText(txt);
                    }
                }
            }
            SetControlText(item, text);
        }

        //##SetControlText
        private void SetControlText(string ctrlName, string text)
        {
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);
            SetControlText(item, text);
        }

        //##SetControlText
        private void SetControlText(ZoneItem item, string text)
        {
            var ctrl = this.GetControl(item.Name);
            if (item.ControlTypeName.ToLower().Contains("label"))
            {
                ctrl.Text = text;
            }
            else if (item.ControlTypeName == ControlType.StatusLight.ToString())
            {
                var cpnt = ctrl as StatusLight;
                cpnt.Text = text;
            }
            else if (item.ControlTypeName == "ScoreLight")
            {
                var cpnt = ctrl as ScoreLight;
                cpnt.Text = text;
            }
            else if (item.ControlTypeName == "RadioButton")
            {
                ctrl.Text = text;
            }
            else if (item.ControlTypeName == "CheckBox")
            {
                ctrl.Text = text;
            }
            else if (item.ControlTypeName == "ComboBox")
            {
                var cpnt = ctrl as ComboBox;
                cpnt.SelectedText = text;
            }
            else if (item.ControlTypeName == "PictureBox")
            {
                ctrl.Tag = text;
            }
        }

        //#ToggleControlCheckStatus
        private void ToggleControlCheckStatus(string ctrlName)
        {
            //var ctrl = this.GetControl(ctrlName);
            //if (ctrl != null)
            //    if (ctrl.Name.ToLower().Contains("pagedlistview"))
            //    {
            //        var cpnt = ctrl as PagedListView;
            //        cpnt.ToggleCheckStatus();
            //    }

        }

        //##GetControlAttributeValue
        private string GetControlAttributeValue(string expression)
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetControlAttributeValue Error: ";

            expression = expression.Trim();
            var ctrlName = "";
            if (expression.ToLower().EndsWith(".v") | expression.ToLower().EndsWith(".t") | expression.ToLower().EndsWith(".vt")
                | expression.ToLower().EndsWith(".ts") | expression.ToLower().EndsWith(".vs")
                | expression.ToLower().EndsWith(".pi") | expression.ToLower().EndsWith(".ps")
                )
            {
                ctrlName = expression.Split('.')[0].Trim();
            }
            else
            {
                throw new ArgumentException(exInfo + "Control attribute value expression error: " + expression);
            }

            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "Zone item doesn't exist! Item=" + ctrlName);

            var typeName = item.ControlTypeName;
            var ctrl = this.GetControl(item.Name);
            var retStr = "";

            if (typeName.Contains(ControlType.Label.ToString()))
            {
                if (expression.ToLower().EndsWith(".t"))
                {
                    retStr = ctrl.Text;
                }
                else
                {
                    throw new ArgumentException(exInfo + "Label attribute value expression error: " + expression);
                }

            }
            else if (typeName == ControlType.TextBox.ToString() | typeName == ControlType.RichTextBox.ToString())
            {
                if (expression.ToLower().EndsWith(".v"))
                {
                    retStr = ctrl.Text;
                }
                else
                {
                    throw new ArgumentException(exInfo + "Control attribute value expression error: " + expression);
                }
            }

            else if (typeName == ControlType.RadioButton.ToString())
            {
                var cpnt = ctrl as RadioButton;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = (cpnt.Checked) ? "true" : "false";
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "RadioButton attribute value expression error: " + expression);
                    }
                }
            }
            else if (typeName == ControlType.CheckBox.ToString())
            {
                var cpnt = ctrl as CheckBox;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = (cpnt.Checked) ? "true" : "false";
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "CheckBox attribute value expression error: " + expression);
                    }
                }
            }
            else if (typeName == ControlType.ComboBox.ToString())
            {
                var cpnt = ctrl as ComboBox;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".t"))
                    {
                        retStr = Convert.ToString(cpnt.Text);
                    }
                    else
                    {
                        retStr = Convert.ToString(cpnt.SelectedValue);
                    }
                }
            }
            else if (typeName == ControlType.StatusLight.ToString())
            {
                var cpnt = ctrl as StatusLight;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = Convert.ToString(cpnt.Value);
                    }
                    else if (expression.ToLower().EndsWith(".t"))
                    {
                        retStr = Convert.ToString(cpnt.Text);
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "StatusLight attribute value expression error: " + expression);
                    }

                }
            }
            else if (typeName == ControlType.ScoreLight.ToString())
            {
                var cpnt = ctrl as ScoreLight;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = Convert.ToString(cpnt.Value, CultureInfo.InvariantCulture);
                    }
                    else if (expression.ToLower().EndsWith(".t"))
                    {
                        retStr = Convert.ToString(cpnt.Text);
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "ScoreLight attribute value expression error: " + expression);
                    }

                }
            }
            else if (typeName == ControlType.ProgressBar.ToString())
            {
                var cpnt = ctrl as ProgressBar;
                if (expression.ToLower().EndsWith(".v"))
                {
                    retStr = Convert.ToString(cpnt.Value);
                }
                else
                {
                    throw new ArgumentException(exInfo + "ProgressBar attribute value expression error: " + expression);
                }
            }

            else if (typeName == ControlType.TransactionBox.ToString())
            {
                var cpnt = ctrl as CommandTextBox;
                if (expression.ToLower().EndsWith(".v"))
                {
                    retStr = Convert.ToString(cpnt.Text);
                }
                else
                {
                    throw new ArgumentException(exInfo + "TransactionBox attribute value expression error: " + expression);
                }
            }

            else
            {
                throw new ArgumentException(exInfo + "Control type doesn't exist, type name:" + typeName);
            }
            return retStr;

        }

        //##RefreshControlDataSource
        private void RefreshControlDataSource(string ctrlName)
        {

            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //refresh item
            var ctrl = GetControl(ctrlName);

            if (item.ControlTypeName == ControlType.PictureBox.ToString())
            {
                if (!String.IsNullOrEmpty(item.DataSource))
                {
                    var cpnt = ctrl as PictureBox;
                    var dataSrc = item.DataSource;
                    dataSrc = ResolveConstants(dataSrc);
                    dataSrc = ResolveStringByRefProcessVariablesAndControls(dataSrc);
                    dataSrc = GetText(dataSrc);
                    ControlHelper.SetControlBackgroundImage(cpnt, dataSrc);
                }
            }

            else if (item.ControlTypeName == "ComboBox")
            {
                var cpnt = ctrl as ComboBox;
                if (!String.IsNullOrEmpty(item.DataSource))
                {
                    var dataSrc = item.DataSource;
                    dataSrc = ResolveConstants(dataSrc);
                    dataSrc = ResolveStringByRefProcessVariablesAndControls(dataSrc);

                    var valTxts = new List<ValueText>();
                    if (dataSrc.StartsWith("="))
                    {
                        dataSrc = GetText(dataSrc);
                        if (dataSrc.Contains("{") & dataSrc.Contains("}"))
                        {
                            valTxts = JsonHelper.ConvertToGeneric<List<ValueText>>(dataSrc);
                        }
                    }
                    else
                    {
                        var strArray = dataSrc.GetSubParamArray(true, true);
                        if (dataSrc.Contains("|"))
                        {
                            foreach (var v in strArray)
                            {
                                var arry = v.Split('|');
                                var valTxt = new ValueText();
                                valTxt.Value = arry[0];
                                valTxt.Text = arry[1];
                                valTxts.Add(valTxt);
                            }
                        }
                        else
                        {
                            var i = 0;
                            foreach (var v in strArray)
                            {
                                var valTxt = new ValueText();
                                valTxt.Value = i.ToString();
                                valTxt.Text = v;
                                valTxts.Add(valTxt);
                                i++;
                            }
                        }
                    }

                    cpnt.DataSource = valTxts;
                    cpnt.ValueMember = "Value";
                    cpnt.DisplayMember = "Text";
                    //cpnt.SelectedIndex = selectedIndex;

                }
            }

        }

        //##ResetControlData
        private void ResetControlData(string ctrlName, string data)
        {

            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //refresh item
            var ctrl = GetControl(ctrlName);

            if (item.ControlTypeName == ControlType.PictureBox.ToString())
            {
                if (!String.IsNullOrEmpty(data))
                {
                    var cpnt = ctrl as PictureBox;
                    ControlHelper.SetControlBackgroundImage(cpnt, data);
                }
            }

            else if (item.ControlTypeName == "ComboBox")
            {
                var cpnt = ctrl as ComboBox;
                if (!String.IsNullOrEmpty(data))
                {
                    var valTxts = new List<ValueText>();
                    if (data.Contains("{") & data.Contains("}"))
                    {
                        valTxts = JsonHelper.ConvertToGeneric<List<ValueText>>(data);
                    }
                    else
                    {
                        var strArray = data.GetSubParamArray(true, true);
                        if (data.Contains("|"))
                        {
                            foreach (var v in strArray)
                            {
                                var arry = v.Split('|');
                                var valTxt = new ValueText();
                                valTxt.Value = arry[0];
                                valTxt.Text = arry[1];
                                valTxts.Add(valTxt);
                            }
                        }
                        else
                        {
                            var i = 0;
                            foreach (var v in strArray)
                            {
                                var valTxt = new ValueText();
                                valTxt.Value = i.ToString();
                                valTxt.Text = v;
                                valTxts.Add(valTxt);
                                i++;
                            }
                        }
                    }

                    cpnt.DataSource = valTxts;
                    cpnt.ValueMember = "Value";
                    cpnt.DisplayMember = "Text";
                    //cpnt.SelectedIndex = selectedIndex;

                }
            }

        }

        //##AppendControlData
        private void AppendControlData(string ctrlName, string data)
        {
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //refresh item
            var ctrl = GetControl(ctrlName);

        }

        //##RefreshControlInvisible
        private void RefreshControlInvisible(string ctrlName)
        {
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);
            //Visible
            bool isCpntVisible = true;
            var inVisibleFlag = item.InvisibleFlag;
            if (string.IsNullOrEmpty(inVisibleFlag)) inVisibleFlag = "false";
            else
            {
                var txt = ResolveStringByRefProcessVariablesAndControls(item.InvisibleFlag);
                inVisibleFlag = GetText(txt);
            }
            isCpntVisible = (inVisibleFlag.ToLower() == "false") ? true : false;

            //refresh item
            var ctrl = GetControl(item.Name);
            if (!isCpntVisible) ctrl.Visible = false; else ctrl.Visible = true;

        }

        //##SetControlVisible
        private void SetControlVisible(string ctrlName, bool isVisible)
        {
            var exInfo = "\n>> " + GetType().FullName + ".SetControlVisible Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //set item
            var ctrl = GetControl(item.Name);
            if (!isVisible) ctrl.Visible = false; else ctrl.Visible = true;
        }

        //##RefreshControlDisabled
        private void RefreshControlDisabled(string ctrlName)
        {
            var exInfo = "\n >> " + GetType().FullName + ".RefreshControlDisabled Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //Enabled
            bool isCpntEnabled = true;
            var disabledFlag = item.DisabledFlag;
            if (string.IsNullOrEmpty(disabledFlag)) disabledFlag = "false";
            else
            {
                var txt = ResolveStringByRefProcessVariablesAndControls(item.DisabledFlag);
                disabledFlag = GetText(txt);
            }
            isCpntEnabled = (disabledFlag.ToLower() == "false") ? true : false;

            var ctrl = GetControl(item.Name);
            if (item.ControlTypeName == "TextBox" | item.ControlTypeName == "RichTextBox")
            {
                var cpnt = ctrl as TextBox;
                if (!isCpntEnabled) cpnt.ReadOnly = true; else cpnt.ReadOnly = false;
            }
            else
            {
                if (!isCpntEnabled) ctrl.Enabled = false; else ctrl.Enabled = true;
            }
        }

        //##SetControlEnabled
        private void SetControlEnabled(string ctrlName, bool isEnabled)
        {
            var exInfo = "\n >> " + GetType().FullName + ".SetControlEnabled Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //set item
            var ctrl = GetControl(item.Name);
            if (item.ControlTypeName == "TextBox" | item.ControlTypeName == "RichTextBox")
            {
                var cpnt = ctrl as TextBox;
                if (!isEnabled) cpnt.ReadOnly = true; else cpnt.ReadOnly = false;
            }
            else
            {
                if (!isEnabled) ctrl.Enabled = false; else ctrl.Enabled = true;
            }
        }

        //##RefreshControl
        private void RefreshControl(string ctrlName)
        {
            var exInfo = "\n >> " + GetType().FullName + ".RefreshControl Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            var ctrl = this.GetControl(item.Name);

            if (item.ControlTypeName == "StatusLight")
            {
                var cpnt = ctrl as StatusLight;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "ScoreLight")
            {
                var cpnt = ctrl as ScoreLight;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "RadioButton")
            {
                var cpnt = ctrl as RadioButton;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "CheckBox")
            {
                var cpnt = ctrl as CheckBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "TextBox")
            {
                var cpnt = ctrl as TextBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "RichTextBox")
            {
                var cpnt = ctrl as RichTextBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "ComboBox")
            {
                var cpnt = ctrl as ComboBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == "ProgressBar")
            {
                var cpnt = ctrl as ProgressBar;
                cpnt.Refresh();
            }

        }

        //##GetControl
        private Control GetControl(string ctrlName)
        {
            var ctrl = this.Controls.Find(ctrlName, true)[0];
            return ctrl;
        }

        //*subcommon
        //**gettext
        //##GetText
        private string GetText(string funcName, string[] funcParamArray)
        {
            var returnText = "";

            if (funcName.ToLower() == "Equal".ToLower())
            {
                return funcParamArray[0];
            }
            else if (funcName.ToLower() == "GetCenterData".ToLower())
            {
                var dataName = funcParamArray[0];
                returnText = GetCenterData(dataName);
            }
            else if (funcName.ToLower() == "GetCommonParams".ToLower())
            {
                var id = "";
                if (funcParamArray.Length > 0)
                    id = funcParamArray[0];
                returnText = GetCommonParams(id);
            }

            else if (funcName.ToLower() == "GetAnnexText".ToLower())
            {
                var className = funcParamArray[0];
                var masterName = funcParamArray[1];
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 2)
                {
                    var annexTypeStr = funcParamArray[2];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var tempStr = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, CultureHelper.CurrentLanguageCode, GetAnnexMode.StepByStep);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, "", GetAnnexMode.FirstAnnex);
                }
                returnText = tempStr.IsNullOrEmpty() ? masterName : tempStr;
            }
            else if (funcName.ToLower() == "GetAbbrevAnnexText".ToLower())
            {
                var className = "Abbrev";
                var masterName = funcParamArray[0];
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 1)
                {
                    var annexTypeStr = funcParamArray[1];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var tempStr = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, CultureHelper.CurrentLanguageCode, GetAnnexMode.StepByStep);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, "", GetAnnexMode.FirstAnnex);
                }
                returnText = tempStr.IsNullOrEmpty() ? masterName : tempStr;
            }
            else if (funcName.ToLower() == "GetPhraseAnnexText".ToLower())
            {
                var className = "Phrase";
                var masterName = funcParamArray[0];
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 1)
                {
                    var annexTypeStr = funcParamArray[1];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var tempStr = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, CultureHelper.CurrentLanguageCode, GetAnnexMode.OnlyByCurLang);
                    if (tempStr.IsNullOrEmpty()) tempStr = masterName;
                }
                else
                {
                    tempStr = masterName;
                }
                returnText = tempStr.IsNullOrEmpty() ? masterName : tempStr;
            }
            else if (funcName.ToLower() == "GetThreadPoolInfo".ToLower())
            {
                if (_threadPool != null)
                    return GenericHelper.ConvertListToRichText<ThreadTaskInfo>(_threadPool.GetThreadPoolInfo(), false);
                else return "";
            }
            else if (funcName.ToLower() == "GetCurrentViewName".ToLower())
            {
                return _currentViewName;
            }


            else if (funcName.ToLower() == "GetContentFromChosenTextFile".ToLower() | funcName.ToLower() == "GetCttFrChsTextFile".ToLower())
            {
                var filePath = GetText("ChooseFile", funcParamArray);
                if (!filePath.IsNullOrEmpty())
                    return FileHelper.GetContentFromTextFile(filePath);
                return string.Empty;
            }
            else if (funcName.ToLower() == "ChooseFile".ToLower())
            {
                var dlg = new OpenFileDialog();
                dlg.Title = WinformRes.ChooseFile;
                //for example: "图片|*.jpg;*.png;*.gif"  "标签1|*.jpg|标签2|.png|标签3|.gif"
                if (funcParamArray.Length > 0)
                {
                    dlg.Filter = funcParamArray[0];
                }

                dlg.InitialDirectory = PathHelper.GetLastDriveName() + "\\";
                if (funcParamArray.Length > 1)
                {
                    dlg.InitialDirectory = funcParamArray[1];
                }
                if (funcParamArray.Length > 2)
                {
                    if (!funcParamArray[2].IsNullOrEmpty())
                        dlg.Title = funcParamArray[2];
                }

                dlg.Multiselect = false;
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.FileNames.Count() > 0)
                    {
                        var firstFilePath = dlg.FileNames[0];
                        return firstFilePath;
                    }
                }
            }
            else if (funcName.ToLower() == "ChooseDirectory".ToLower())
            {

                var dlg = new FolderBrowserDialog();
                dlg.Description = WinformRes.ChooseDir;
                if (funcParamArray.Length > 0)
                {
                    dlg.SelectedPath = funcParamArray[0];
                }
                else dlg.SelectedPath = PathHelper.GetLastDriveName() + "\\";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.SelectedPath;
                }
                return string.Empty;

            }
            else if (funcName.ToLower() == "GetClipboardText".ToLower())
            {
                return Clipboard.GetText();
            }
            else if (funcName.ToLower() == "GetBool".ToLower())
            {
                if (funcParamArray[0] == "IsFormModal")
                {
                    returnText = (this.Modal).ToString();
                }
                else if (funcParamArray[0] == "IsUserCodeNotEmpty")
                {
                    returnText = (!GetCenterData("UserCode").IsNullOrEmpty()).ToString();
                }
                else
                {
                    returnText = Getter.GetText("GetBool", funcParamArray);
                }
            }
            else if (funcName.ToLower() == "ValidateInput".ToLower())
            {
                var ctrlArray = funcParamArray[0].GetSubParamArray(true, true);
                foreach (var ctrlName in ctrlArray)
                {
                    var ctrlName1 = ctrlName.Trim().DeleteUiIdentifer();

                    var defDisplayName = _zonesItems.Find(x => x.Name == ctrlName1).DisplayName;
                    defDisplayName = defDisplayName.IsNullOrEmpty() ? ctrlName1.GetLastSeparatedString('_') : defDisplayName;
                    var displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "ZoneItem", ctrlName1, _annexes, defDisplayName);

                    var ctrlValue = GetControlAttributeValue(ctrlName1 + ".v");
                    var ctrlValidateRules = _zonesItems.Find(x => x.Name == ctrlName1).ValidationRules;
                    if (ctrlValidateRules.IsNullOrEmpty())
                    {
                        return "true";
                    }
                    else
                    {
                        var ruleArry = ctrlValidateRules.GetSubParamArray(true, true);
                        foreach (var rule in ruleArry)
                        {
                            var funcParamArray1 = new string[] { ctrlValue, rule };
                            var validationResult = Getter.GetText("Validate", funcParamArray1);

                            if (validationResult == "OutOfScope") //false
                            {
                                if (!_functionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".GetText Error: OutOfScope");
                                validationResult = GetTextEx("Validate", funcParamArray1);
                                //if (validationResult != "true") return displayName + ": " + validationResult;
                            }

                            if (validationResult != "true")
                            {
                                return displayName + ": " + validationResult;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                returnText = "true";
            }
            else
            {
                returnText = Getter.GetText(funcName, funcParamArray);
            }

            return returnText;
        }

        //**act
        //##Act
        private void Act(string actionName, string[] actionParamArray)
        {

            if (actionName.IsNullOrEmpty()) return;

            var returnStr = "";
            if (actionName.ToLower() == "Xrun".ToLower())
            {
                var actionParams = IdentifierHelper.UnwrapParamArray(actionParamArray);
                Xrun(actionParams);
            }
            else if (actionName.ToLower() == "Lrun".ToLower())
            {
                var actionParams = IdentifierHelper.UnwrapParamArray(actionParamArray);
                Lrun(actionParams);
            }
            else if (actionName.ToLower() == "Crun".ToLower())
            {
                var conVal = actionParamArray[0];
                if (conVal.ToLower() == "true")
                {
                    Xrun(actionParamArray[1]);
                }
                else
                {
                    if (actionParamArray.Length > 2)
                    {
                        Xrun(actionParamArray[2]);
                    }
                }
            }
            //--form
            else if (actionName.ToLower() == "PopupMsg".ToLower())
            {
                var title = actionParamArray[0];
                var content = actionParamArray[1];
                var format = PopupMessageFormFormat.Common;
                if (actionParamArray.Length > 2)
                {
                    var p3 = actionParamArray[2];
                    if (p3.ToLower() == "MessageViewer".ToLower())
                    {
                        format = PopupMessageFormFormat.MessageViewer;
                    }
                    else if (p3.ToLower() == "RichTextViewer".ToLower())
                    {
                        format = PopupMessageFormFormat.RichTextViewer;
                    }
                }
                var width = 0;
                if (actionParamArray.Length > 3)
                {
                    width = Convert.ToInt16(actionParamArray[3]);
                }
                content = GetHelper.FormatRichText(content);
                MessageHelper.Popup(title, content, format, width);
            }
            else if (actionName.ToLower() == "MaximizeForm".ToLower())
            {
                MaximizeForm();
            }
            else if (actionName.ToLower() == "MinimizeForm".ToLower())
            {
                MinimizeForm();
            }
            else if (actionName.ToLower() == "ShowForm".ToLower())
            {
                ShowForm();
            }
            else if (actionName.ToLower() == "HideForm".ToLower())
            {
                HideForm();
            }
            else if (actionName.ToLower() == "CloseForm".ToLower())
            {
                CloseForm();
            }

            else if (actionName.ToLower() == "FadeIn".ToLower())
            {
                var duration = 1000;
                if (actionParamArray.Length > 0) duration = Convert.ToInt16(actionParamArray[0]);
                FadeIn(duration);
            }
            else if (actionName.ToLower() == "FadeOut".ToLower())
            {
                var duration = 1000;
                if (actionParamArray.Length > 0) duration = Convert.ToInt16(actionParamArray[0]);
                FadeOut(duration);
            }
            else if (actionName.ToLower() == "ReturnFalse".ToLower())
            {
                BoolValue = false;
            }
            else if (actionName.ToLower() == "ReturnFalseAndClose".ToLower())
            {
                BoolValue = false;
                CloseForm();
            }
            else if (actionName.ToLower() == "ExitApp".ToLower())
            {
                ExitApplication();
            }

            else if (actionName.ToLower() == "ExecBootStrapperTasks".ToLower())
            {
                if (_functionInitParamSet.RunBootStrapperTasksAtStart)
                    ExecBootStrapperTasksEx();
            }



            else if (actionName.ToLower() == "RefreshUi".ToLower())
            {
                RefreshUi();
            }
            else if (actionName.ToLower() == "RefreshForm".ToLower())
            {
                Act("NewForm", new string[] { "true" });
                ExitApplication();
            }

            else if (actionName.ToLower() == "NewForm".ToLower())
            {

                bool copyCurrentForm = true; ;
                bool closeCurrentForm = false;
                string appCode = "", formTitle = "", commonParams = "", startPassword = "", usrCode = "", usrToken = "", formTypeFlag = "",
                    startFuncAndViewOrSviRelativeLoc = "", startZoneParams = "";
                if (actionParamArray.Length == 0 | actionParamArray.Length == 1)//new same form
                {
                    if (actionParamArray.Length == 1) // refresh or new same form
                    {
                        if (actionParamArray[0] == "true") closeCurrentForm = true; //refresh 
                    }
                    appCode = _functionInitParamSet.ApplicationCode;
                    formTitle = _functionInitParamSet.FormTitle;
                    commonParams = GetCommonParams("");
                    startPassword = _functionInitParamSet.StartPassword;
                    usrCode = GetCenterData("UserCode");
                    usrToken = GetCenterData("UserToken");

                    formTypeFlag = _functionInitParamSet.FormType.ToString();
                    startFuncAndViewOrSviRelativeLoc = _functionInitParamSet.FormType == FormType.Mvi ? _functionInitParamSet.FunctionCode + "|" + _currentViewName : _formCfgDir + "\\" + _functionInitParamSet.FunctionCode;
                    startZoneParams = _functionInitParamSet.StartZoneProcessParams;
                }
                else//New diff form
                {
                    if (actionParamArray[0] == "true") closeCurrentForm = true; //fresh 
                    copyCurrentForm = false;
                    var subParams = actionParamArray[1];
                    var subParamArry = subParams.Split(' ');
                    var appParamsArry = subParamArry[0].GetParamArray(true, false);

                    appCode = appParamsArry[0];
                    var isSameApp = appCode == _functionInitParamSet.ApplicationCode ? true : false;
                    formTitle = appParamsArry.Length > 1 ? appParamsArry[1] : "";
                    commonParams = appParamsArry.Length > 2 ? appParamsArry[2] : "";
                    startPassword = isSameApp ? _functionInitParamSet.StartPassword : (appParamsArry.Length > 3 ? appParamsArry[3] : "");
                    usrCode = isSameApp ? GetCenterData("UserCode") : (appParamsArry.Length > 4 ? appParamsArry[4] : "");
                    usrToken = isSameApp ? GetCenterData("UserToken") : (appParamsArry.Length > 5 ? appParamsArry[5] : "");
                    var winformParams = subParamArry[1].GetParamArray(true, false);
                    formTypeFlag = winformParams[0];
                    startFuncAndViewOrSviRelativeLoc = winformParams[1];
                    if (winformParams.Length > 2) startZoneParams = winformParams[1];

                }

                var appParams = appCode + "^" + formTitle + "^" + commonParams + "^" + startPassword + "^" + usrCode + "^" + "userPassword" + "^" + usrToken + "^" + "true";
                var formParams = formTypeFlag + "^" + startFuncAndViewOrSviRelativeLoc + "^" + startZoneParams;

                var passedCultureName = "";
                if (_functionInitParamSet.SupportMultiCultures)
                    passedCultureName = CultureHelper.CurrentCultureName;
                if (actionParamArray.Length > 1) passedCultureName = actionParamArray[2];

                var arguments = appParams + " " + formParams + " " + passedCultureName;
                var path = Application.ExecutablePath;
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = path;
                if (!string.IsNullOrEmpty(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }
                process.Start();


                if (copyCurrentForm)
                {
                    if (Control.ModifierKeys == Keys.Control) closeCurrentForm = true;
                }
                if (closeCurrentForm) ExitApplication();
            }

            else if (actionName.ToLower() == "PopupViewDialog".ToLower())
            {
                var passedArgs = actionParamArray.Length == 0 ? "" : actionParamArray[0];
                var passedArgArry = passedArgs.GetParamArray(true, false);

                string startSviViewRelativeLocation = "";
                string startFormTitle = "";
                string startActions = "";
                string startFormProcessParams = "";

                startSviViewRelativeLocation = passedArgArry[0];
                if (passedArgArry.Length > 1) startFormTitle = passedArgArry[1];
                if (passedArgArry.Length > 2) startActions = passedArgArry[2];
                if (passedArgArry.Length > 3) startFormProcessParams = passedArgArry[3];

                var functionInitParamSet = new FormInitParamSet();
                functionInitParamSet.FormType = FormType.SviOfView;

                functionInitParamSet.ArchitectureCode = _functionInitParamSet.ArchitectureCode;
                functionInitParamSet.ArchitectureName = _functionInitParamSet.ArchitectureName;
                functionInitParamSet.ArchitectureVersion = _functionInitParamSet.ArchitectureVersion;
                functionInitParamSet.OrganizationCode = _functionInitParamSet.OrganizationCode;
                functionInitParamSet.OrganizationShortName = _functionInitParamSet.OrganizationShortName;
                functionInitParamSet.OrganizationName = _functionInitParamSet.OrganizationName;
                functionInitParamSet.ApplicationCode = _functionInitParamSet.ApplicationCode;
                functionInitParamSet.ApplicationVersion = _functionInitParamSet.ApplicationVersion;
                functionInitParamSet.HelpdeskEmail = _functionInitParamSet.HelpdeskEmail;
                functionInitParamSet.SupportMultiCultures = _functionInitParamSet.SupportMultiCultures;

                functionInitParamSet.StartSviViewRelativeLocation = startSviViewRelativeLocation;
                functionInitParamSet.FormTitle = startFormTitle;

                PopupSviDialogEx(functionInitParamSet);
            }
            else if (actionName.ToLower() == "PopupZoneDialog".ToLower())
            {
                var passedArgs = actionParamArray.Length == 0 ? "" : actionParamArray[0];
                var passedArgArry = passedArgs.GetParamArray(true, false);

                string startSviZoneRelativeLocation = "";

                string startFormTitle = "";
                string startActions = "";
                string startZoneProcessParams = "";

                startSviZoneRelativeLocation = passedArgArry[0];

                if (passedArgArry.Length > 1) startFormTitle = passedArgArry[1];
                if (passedArgArry.Length > 2) startActions = passedArgArry[2];
                if (passedArgArry.Length > 4) startZoneProcessParams = passedArgArry[3];

                var functionInitParamSet = new FormInitParamSet();
                functionInitParamSet.FormType = FormType.SviOfZone;

                functionInitParamSet.ArchitectureCode = _functionInitParamSet.ArchitectureCode;
                functionInitParamSet.ArchitectureName = _functionInitParamSet.ArchitectureName;
                functionInitParamSet.ArchitectureVersion = _functionInitParamSet.ArchitectureVersion;
                functionInitParamSet.OrganizationCode = _functionInitParamSet.OrganizationCode;
                functionInitParamSet.OrganizationShortName = _functionInitParamSet.OrganizationShortName;
                functionInitParamSet.OrganizationName = _functionInitParamSet.OrganizationName;
                functionInitParamSet.ApplicationCode = _functionInitParamSet.ApplicationCode;
                functionInitParamSet.ApplicationVersion = _functionInitParamSet.ApplicationVersion;
                functionInitParamSet.HelpdeskEmail = _functionInitParamSet.HelpdeskEmail;
                functionInitParamSet.SupportMultiCultures = _functionInitParamSet.SupportMultiCultures;

                functionInitParamSet.StartSviZoneRelativeLocation = startSviZoneRelativeLocation;
                functionInitParamSet.FormTitle = startFormTitle;
                functionInitParamSet.StartZoneProcessParams = startZoneProcessParams;

                PopupSviDialogEx(functionInitParamSet);
            }

            else if (actionName.ToLower() == "PopupZone".ToLower())
            {
                var popupZoneName = actionParamArray[0].DeleteUiIdentifer();
                var popupItem = _layoutElements.Find(x => x.Name == popupZoneName);
                if (popupItem != null)
                {
                    var popupCtnCtrl = GetControl(popupZoneName + "_Container");
                    var posArray = actionParamArray[1].Split(',');
                    var baseCtrlName = posArray[0].DeleteUiIdentifer();
                    var baseCtrl = GetControl(baseCtrlName);

                    var alignType = Convert.ToInt16(posArray[1]);
                    var offSetX = Convert.ToInt16(posArray[2]);
                    var offSetY = Convert.ToInt16(posArray[3]);
                    var p = baseCtrl.PointToClient(new Point(0, 0));
                    var p1 = GroundPanel.PointToClient(new Point(0, 0));
                    Point pos;
                    if (alignType == (int)ControlAlignPointType.LeftTop)
                    {
                        pos = new Point(p1.X - p.X, p1.Y - p.Y);
                    }
                    else if (alignType == (int)ControlAlignPointType.RightTop)
                    {
                        pos = new Point(p1.X - p.X - popupCtnCtrl.Width, p1.Y - p.Y);
                    }
                    else if (alignType == (int)ControlAlignPointType.LeftBottom)
                    {
                        pos = new Point(p1.X - p.X, p1.Y - p.Y - popupCtnCtrl.Height);
                    }
                    else if (alignType == (int)ControlAlignPointType.RightBottom)
                    {
                        pos = new Point(p1.X - p.X - popupCtnCtrl.Width, p1.Y - p.Y - popupCtnCtrl.Height);
                    }
                    else
                    {
                        pos = new Point(p1.X - p.X, p1.Y - p.Y);
                    }
                    pos.X = pos.X + offSetX;
                    pos.Y = pos.Y + offSetY;

                    popupCtnCtrl.Location = pos;
                    popupCtnCtrl.Visible = true;
                    popupCtnCtrl.BringToFront();
                }
            }

            //--process
            else if (actionName.ToLower() == "RefreshZonesProcesses".ToLower())
            {
                var zoneNameArry = actionParamArray[0].GetSubParamArray(true, true);
                foreach (var zoneName in zoneNameArry)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    RefreshZoneProcess(zoneName1);
                }
            }
            else if (actionName.ToLower() == "RefreshZoneProcess".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshZoneProcess(zoneName1);
            }

            else if (actionName.ToLower() == "RefreshZonesProcessByGroup".ToLower())
            {
                var zoneNameArry = actionParamArray[0].GetSubParamArray(true, true);
                foreach (var zoneName in zoneNameArry)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    RefreshZoneProcessByGroup(zoneName1, Convert.ToInt32(actionParamArray[1]));
                }
            }
            else if (actionName.ToLower() == "RefreshZoneProcessByGroup".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshZoneProcessByGroup(zoneName1, Convert.ToInt32(actionParamArray[1]));
            }

            else if (actionName.ToLower() == "ClearZonesProcessesVariables".ToLower())
            {
                var zoneNameArry = actionParamArray[0].GetSubParamArray(true, true);
                foreach (var zoneName in zoneNameArry)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    var procList = _procedures.FindAll(x => x.ZoneName == zoneName1 & x.Type == (int)ProcedureType.Variable);
                    ProcessHelper.ClearProcessVariablesByGroup(Convert.ToInt32(actionParamArray[1].Trim()), procList);
                }
            }
            else if (actionName.ToLower() == "ClearZonesProcessesVariablesByGroup".ToLower())
            {
                var zoneNameArry = actionParamArray[0].GetSubParamArray(true, true);
                foreach (var zoneName in zoneNameArry)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    var procList = _procedures.FindAll(x => x.ZoneName == zoneName1 & x.Type == (int)ProcedureType.Variable);
                    ProcessHelper.ClearProcessVariablesByGroup(Convert.ToInt32(actionParamArray[1]), procList);
                }
            }
            else if (actionName.ToLower() == "ClearZoneProcessVariables".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                var procList = _procedures.FindAll(x => x.ZoneName == zoneName1 & x.Type == (int)ProcedureType.Variable);
                ProcessHelper.ClearProcessVariablesByGroup(Convert.ToInt32(actionParamArray[1]), procList);
            }


            else if (actionName.ToLower() == "ClearZoneProcessVariablesByGroup".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                var procList = _procedures.FindAll(x => x.ZoneName == zoneName1 & x.Type == (int)ProcedureType.Variable);
                ProcessHelper.ClearProcessVariablesByGroup(Convert.ToInt32(actionParamArray[1]), procList);
            }

            else if (actionName.ToLower() == "SetProcessVariableValue".ToLower())
            {
                var varName = IdentifierHelper.DeleteProcessIdentifer(actionParamArray[0]);
                var varValue = actionParamArray[1].Trim();
                var procList = _procedures.FindAll(x => x.Type == (int)ProcedureType.Variable);
                ProcessHelper.SetProcessVariableValue(varValue, "", _procedures);
            }

            //--view
            else if (actionName.ToLower() == "SwitchView".ToLower())
            {
                SwitchView(actionParamArray[0]);
            }
            //--zones, for following trans in view
            else if (actionName.ToLower() == "RefreshZonesControlsValues".ToLower())
            {
                foreach (var zoneName in actionParamArray)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    RefreshZoneControlsValues(zoneName1);
                }
            }
            else if (actionName.ToLower() == "RefreshZonesControlsInvisibles".ToLower())
            {
                foreach (var zoneName in actionParamArray)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    RefreshZoneControlsInvisibles(zoneName1);
                }
            }
            else if (actionName.ToLower() == "RefreshZonesControlsDisableds".ToLower())
            {
                foreach (var zoneName in actionParamArray)
                {
                    var zoneName1 = zoneName.DeleteUiIdentifer();
                    RefreshZoneControlsDisableds(zoneName1);
                }
            }
            //--zone, for following trans in view
            else if (actionName.ToLower() == "RefreshZoneControlsValues".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshZoneControlsValues(zoneName1);
            }
            else if (actionName.ToLower() == "RefreshZoneControlsInvisibles".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshZoneControlsInvisibles(zoneName1);
            }
            else if (actionName.ToLower() == "RefreshZoneControlsDisableds".ToLower())
            {
                var zoneName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshZoneControlsDisableds(zoneName1);
            }


            //--ctrl
            else if (actionName.ToLower() == "RefreshControlsValues".ToLower())
            {
                foreach (var ctrlName in actionParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlValue(ctrlName1);
                }
            }
            else if (actionName.ToLower() == "RefreshControlValue".ToLower())
            {
                var ctrlName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshControlValue(ctrlName1);
            }
            else if (actionName.ToLower() == "ClearControlsValues".ToLower())
            {
                var ctrlNameArry = actionParamArray[0].GetSubParamArray(true, true);
                foreach (var ctrlName in ctrlNameArry)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    SetControlValue(ctrlName1, "");
                }
            }
            else if (actionName.ToLower() == "ClearControlValue".ToLower())
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                SetControlValue(ctrlName, "");
            }

            else if (actionName.ToLower() == "SetControlValue".ToLower())
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                var ctrlValue = actionParamArray[1];
                SetControlValue(ctrlName, ctrlValue);
            }

            //--ctrl text
            else if (actionName.ToLower() == "RefreshControlsTexts".ToLower())
            {
                foreach (var ctrlName in actionParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlText(ctrlName1);
                }
            }
            else if (actionName.ToLower() == "RefreshControlText".ToLower())
            {
                var ctrlName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshControlText(ctrlName1);
            }

            else if (actionName.ToLower() == "SetControlText".ToLower())
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                var ctrlValue = actionParamArray[1].Trim();
                SetControlText(ctrlName.Trim(), ctrlValue);
            }

            //--ctrl data
            else if (actionName.ToLower() == "RefreshControlsDataSources".ToLower())
            {
                foreach (var ctrlName in actionParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlDataSource(ctrlName1);
                }
            }
            else if (actionName.ToLower() == "RefreshControlDataSource".ToLower())
            {
                var ctrlName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshControlDataSource(ctrlName1);
            }
            else if (actionName.ToLower() == "AppendControlData".ToLower()) //only for valueTextBox, listviewex, treeex, combo
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                var data = actionParamArray[1];
                AppendControlData(ctrlName, data);
            }

            else if (actionName.ToLower() == "ResetControlData".ToLower()) //only for valueTextBox, TimerExRecurringRun, listView, richtextBoxComboBox
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                var data = actionParamArray[1];
                ResetControlData(ctrlName, data);
            }

            else if (actionName.ToLower() == "RefreshControlsInvisibles".ToLower())
            {
                foreach (var ctrlName in actionParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlInvisible(ctrlName1);
                }
            }
            else if (actionName.ToLower() == "RefreshControlInvisible".ToLower())
            {
                var ctrlName1 = actionParamArray[0].DeleteUiIdentifer();
                RefreshControlInvisible(ctrlName1);
            }

            else if (actionName.ToLower() == "SetControlVisible".ToLower())
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                SetControlVisible(ctrlName, Convert.ToBoolean(actionParamArray[1]));
            }
            else if (actionName.ToLower() == "RefreshControlsDisableds".ToLower())
            {
                foreach (var ctrlName in actionParamArray)
                {
                    RefreshControlDisabled(ctrlName.DeleteUiIdentifer());
                }
            }
            else if (actionName.ToLower() == "RefreshControlDisabled".ToLower())
            {
                RefreshControlDisabled(actionParamArray[0].DeleteUiIdentifer());
            }

            else if (actionName.ToLower() == "SetControlEnabled".ToLower())
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                SetControlEnabled(ctrlName, Convert.ToBoolean(actionParamArray[1].Trim()));
            }

            else if (actionName.ToLower() == "SetControlPadding".ToLower())
            {

                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                var ctrl = GetControl(ctrlName);
                ControlHelper.SetControlPadding(ctrl, "Padding:" + actionParamArray[1].Trim());
            }
            else if (actionName.ToLower() == "ToggleControlCheckStatus".ToLower()) //--only for listview now
            {
                var ctrlName = actionParamArray[0].DeleteUiIdentifer();
                ToggleControlCheckStatus(ctrlName);
            }

            //--common
            else if (actionName.ToLower() == "RefreshRunningStatus".ToLower())
            {
                RefreshRunningStatusMessage(actionParamArray[0]);
            }
            else if (actionName.ToLower() == "RefreshControls".ToLower()) //no use
            {
                foreach (var ctrlName in actionParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControl(ctrlName1);
                }
            }
            else if (actionName.ToLower() == "Sleep".ToLower())
            {
                var duration = 1000;
                if (actionParamArray.Length > 0) duration = Convert.ToInt16(actionParamArray[0]);
                //this.Refresh(); // for asyn, asyn waited, managed thread will popup error
                Thread.Sleep(duration);
                //this.Refresh(); 
            }

            else//to do by dispacher
            {
                if (actionName.ToLower() == "run" | actionName.ToLower().StartsWith("runasadmin") |
                actionName.ToLower() == "runcmd" | actionName.ToLower().StartsWith("runcmdasadmin"))
                {
                    var defLoc = _appLibDir;
                    var path = actionParamArray[0];
                    path = FileHelper.GetFilePath(path, defLoc);
                    actionParamArray[0] = path;
                }
                else if (actionName.ToLower() == "openfile" | actionName.ToLower().StartsWith("openfileasadmin") |
                    actionName.ToLower() == "editfile" | actionName.ToLower().StartsWith("editfileasadmin") |
                    actionName.ToLower() == "runPy" | actionName.ToLower().StartsWith("runpyasadmin")
                    )
                {
                    var defLoc = _appDataDir;
                    var path = actionParamArray[0];
                    path = FileHelper.GetFilePath(path, defLoc);
                    actionParamArray[0] = path;
                }

                returnStr = Dispatcher.Act(actionName, actionParamArray);
                if (returnStr == "OutOfScope")
                {
                    if (!_functionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".Act Error: OutOfScope");
                    returnStr = ActEx(actionName, actionParamArray);
                }
            }

            if (actionName.ToLower() == "ExecCmd".ToLower() | actionName.ToLower() == "ExecCmdAsAdmin".ToLower() |
                actionName.ToLower() == "ExecPython".ToLower() | actionName.ToLower() == "ExecPythonAsAdmin".ToLower())
            {
                var lastParam = actionParamArray[actionParamArray.Length - 1];
                var execCmdWindowOptionStr = "";
                if (actionName.ToLower() == "ExecCmd".ToLower() | actionName.ToLower() == "ExecCmdAsAdmin".ToLower())
                {
                    if (actionParamArray.Length > 1) execCmdWindowOptionStr = actionParamArray[1];
                }
                else
                {
                    if (actionParamArray.Length > 2) execCmdWindowOptionStr = actionParamArray[2];
                }
                var execCmdWindowOption = ExecCmdWindowOption.ShowWindow;
                execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, execCmdWindowOption);
                if (execCmdWindowOption == ExecCmdWindowOption.ShowWindow) return;
                if (lastParam.ToLower() == "pop")
                {
                    var title = actionName + " Executing Result";
                    var content = GetHelper.FormatRichText(returnStr);
                    MessageHelper.Popup(title, content, PopupMessageFormFormat.RichTextViewer, 0);
                }

                if (lastParam.ToLower() == "save")
                {
                    var title = actionName;
                    foreach (var actParam in actionParamArray)
                    {
                        title = title + "-" + actParam.ToLegalFileName();
                    }

                    title = title + "-Executing-Result";
                    var content = GetHelper.FormatRichText(returnStr);
                    var dir = _appTempDir + "\\" + "Cmds";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    var path = dir + "\\" + title.ToUniqueStringByNow("") + ".txt";
                    File.WriteAllText(path, content);
                }
            }

        }

        //**xrun
        //##xrun
        private void Xrun(string actParams)
        {
            //#
            var exInfo = "\n>> " + GetType().FullName + ".Xrun Error: ";
            var elementNameArr = actParams.GetSubParamArray(true, true);
            foreach (var elementName in elementNameArr)
            {

                var action = "";
                if (IdentifierHelper.IsProcessElement(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteProcessIdentifer(elementName);
                    var item = _procedures.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "ProcessElementName={0} des not exist!", elementName));
                    if (item.Formula.IsNullOrEmpty()) continue;
                    action = item.Formula;
                }
                else if (IdentifierHelper.IsUiElement(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);
                    var item = _zonesItems.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "UiElementName={0} des not exist!", elementName));
                    if (item.Action.IsNullOrEmpty()) continue;
                    action = item.Action;
                }

                var actionNameAndParamsArray = action.GetParamArray(true, false);
                var actionName = actionNameAndParamsArray[0].Trim().ToLower();
                var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    actionParamArray[i] = GetText(ResolveStringByRefProcessVariablesAndControls(actionNameAndParamsArray[i + 1].Trim()));
                }
                try
                {
                    Act(actionName, actionParamArray);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + ".Xrun Error: elementName=" + elementName + "; " + " action = " + action + "; " + ex.Message);
                }

            }
        }

        private void Lrun(string actParams)
        {
            if (!IdentifierHelper.ContainsLrunIdentifer(actParams)) return;
            var strArray = actParams.GetParamArray(true, false);
            var actionName = strArray[0];
            var actionParamsAndReplaceVals = strArray[1];
            var actionParamsAndReplaceValsArry = actionParamsAndReplaceVals.GetParamArray(true, false);
            var actionParams = actionParamsAndReplaceValsArry[0];
            var actionParamsArry = actionParams.GetParamArray(true, false);
            var ReplaceVals = actionParamsAndReplaceValsArry[1];
            var ReplaceValsArry = ReplaceVals.GetSubParamArray(true, false);



            foreach (var lrunParam in ReplaceValsArry)
            {
                foreach (var actionParam in actionParamsArry)
                {
                    if (IdentifierHelper.ContainsLrunIdentifer(actionParam))
                        actionParam.Replace(IdentifierHelper.LrunIdentifer, lrunParam);
                }

                Act(actionName, actionParamsArry);
            }

        }


        //**resolve
        //###ResolveConstants
        private string ResolveConstants(string text)
        {
            if (text.IsNullOrEmpty()) return string.Empty;
            if (!text.Contains("%")) return text;

            var toBeRplStr = "";

            toBeRplStr = "%ArchCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ArchitectureCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ArchitectureName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ArchitectureVersion;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%OrgCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.OrganizationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgShortName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.OrganizationShortName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.OrganizationName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ApplicationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ApplicationVersion;
                return Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FuncCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.FunctionCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            //golbal config
            toBeRplStr = "%CfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _cfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%CfgUiDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _cfgUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            //appCfg
            toBeRplStr = "%AppCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppUiDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appCfgUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FormCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppLibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appLibDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppTempDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appTempDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%sysAppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _sysAppDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%myAppZoneDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _myAppZoneDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            if (!text.Contains("%"))
            {
                return text;
            }
            else
            {
                var retStr = Ligg.EasyWinApp.Parser.Helpers.GetHelper.ResolveConstants(text);
                if (!retStr.Contains("%"))
                {
                    return retStr;
                }
                else
                {
                    return ResolveConstantsEx(text);
                }
            }
        }

        //##ResolveStringByRefProcessVariablesAndControls
        private string ResolveStringByRefProcessVariablesAndControls(string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return "";
                if (str.Contains("#"))
                {
                    str = ProcessHelper.ResolveStringByRefProcessVariables(str, _procedures.Where(x => x.Type == (int)ProcedureType.Params | x.Type == (int)ProcedureType.Variable).ToList());
                }
                if (str.Contains("$"))
                {
                    str = ResolveStringByRefControls(str, _zonesItems);
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefProcessVariablesAndControls Error: " + ex.Message);
            }
        }

        //##ResolveStringByRefControls
        private string ResolveStringByRefControls(string str, List<ZoneItem> zoneItems)
        {
            try
            {
                var strArray = str.Split('$');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '$' no. in " + str + " is not an even! ");
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)
                        {
                            var txt = strArray[i];
                            if (txt.ToLower().EndsWith(".v") | txt.ToLower().EndsWith(".t") | txt.ToLower().EndsWith(".vt") | txt.ToLower().EndsWith(".vs") | txt.ToLower().EndsWith(".ts")
                                | txt.ToLower().EndsWith(".pi") | txt.ToLower().EndsWith(".ps"))
                                strArray[i] = GetControlAttributeValue(txt);
                            else strArray[i] = "$" + strArray[i] + "$";
                        }
                    }
                    return string.Join("", strArray);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefControls Error: str='" + str + "'; " + ex.Message);
            }
        }


        //##ShowForm
        private void ShowForm()
        {
            this.Visible = true;
            this.WindowState = _ordinaryWindowStatus;
            this.Show();
        }

        private void HideForm()
        {
            this.Visible = false;
            //this.WindowState = _ordinaryWindowStatus;
            this.Hide();
        }

        //##MinimizeForm
        private void MinimizeForm()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //##MaximizeForm
        private void MaximizeForm()
        {
            this.WindowState = FormWindowState.Maximized;
        }

        //##CloseForm
        private void CloseForm()
        {
            Close();
        }

        //##ExitApplication
        private void ExitApplication()
        {
            if (_hasTray)
                _tray.Visible = false;
            IsRealClosing = true;
            Close();
            Application.Exit();
        }

        //##FadeIn
        private void FadeIn(int duration) //ms
        {
            int interval = duration / 100;
            this.Opacity = 0;
            do
            {
                this.Opacity += 0.01;
                this.Refresh();
                Thread.Sleep(interval);
            } while (this.Opacity < 1);

        }

        //##FadeOut
        private void FadeOut(int duration) //ms
        {
            int interval = duration / 100;
            this.Opacity = 1;
            do
            {
                this.Opacity -= 0.01;
                this.Refresh();
                Thread.Sleep(interval);
            } while (this.Opacity > 0);

            Close();
        }

        //##RefreshUi
        //###testing UI, refresh after logon
        private void RefreshUi()
        {
            try
            {
                foreach (var elemt in _layoutElements)
                {
                    var isPopup = elemt.IsPopup;
                    if (isPopup)
                    {
                        var ctrl = new Control();

                        try
                        {
                            ctrl = GroundPanel.Controls.Find(elemt.Name + "_" + "Container", true)[0];
                            GroundPanel.Controls.Remove(ctrl);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                //_formStyle = null;
                _procedures.Clear();
                _layoutElements.Clear();
                _annexes.Clear();
                _zonesItems.Clear();
                _renderedViewNames.Clear();
                _viewFeatures.Clear();
                _menuFeatures.Clear();
                _currentNestedMenuId = 0;
                _hasTray = false;
                //keep _currentViewName to pass strat view
                //_currentViewName="";


                TopNavSectionLeftRegion.Controls.Clear();
                TopNavSectionCenterRegion.Controls.Clear();
                TopNavSectionRightRegion.Controls.Clear();

                ToolBarSectionLeftRegion.Controls.Clear();
                ToolBarSectionCenterRegion.Controls.Clear();
                ToolBarSectionRightRegion.Controls.Clear();
                ToolBarSectionPublicRegionToolStrip.Items.Clear();
                //ToolBarSectionPublicRegionToolStrip.Controls.Clear(); //error:集合为只读。

                MiddleNavSectionLeftRegion.Controls.Clear();
                MiddleNavSectionCenterRegion.Controls.Clear();
                MiddleNavSectionRightRegion.Controls.Clear();

                DownNavSectionLeftRegion.Controls.Clear();
                DownNavSectionCenterRegion.Controls.Clear();
                DownNavSectionRightRegion.Controls.Clear();

                MainSectionLeftNavDivisionUpRegion.Controls.Clear();
                MainSectionLeftNavDivisionMidRegion.Controls.Clear();
                MainSectionLeftNavDivisionDownRegion.Controls.Clear();

                MainSectionRightNavDivisionUpRegion.Controls.Clear();
                MainSectionRightNavDivisionMidRegion.Controls.Clear();
                MainSectionRightNavDivisionDownRegion.Controls.Clear();

                MainSectionMainDivisionUpRegion.Controls.Clear();
                MainSectionMainDivisionMidRegion.Controls.Clear();
                MainSectionMainDivisionDownRegion.Controls.Clear();

                MainSectionRightDivisionUpRegion.Controls.Clear();
                MainSectionRightDivisionMidRegion.Controls.Clear();
                MainSectionRightDivisionDownRegion.Controls.Clear();

                LoadForm();
                this.Refresh();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RefreshForm Error: " + ex.Message);
            }
        }

        //##GetAdditionalInfoForException
        private string GetAdditionalInfoForException()
        {
            return _additionalInfoForException + GetCenterData("UserCode");
        }

        //**virtual
        //##ResolveConstantsEx
        protected virtual string ResolveConstantsEx(string text)
        {
            return text;
        }

        //##GetTextEx
        protected virtual string GetTextEx(string funName, string[] paramArray)
        {
            return string.Empty;
        }

        //##ActEx
        protected virtual string ActEx(string funcName, string[] funcParamArray)
        {
            return string.Empty;
        }

        //##PopupZoneDialogEx
        protected virtual void PopupSviDialogEx(FormInitParamSet functionInitParamSet)
        {
        }
        //##ExecBootStrapperTasksEx
        protected virtual void ExecBootStrapperTasksEx()
        {
        }


        protected virtual string GetCenterData(string dataName)
        {
            return string.Empty;
        }

        protected virtual string GetCommonParams(string id)
        {
            return string.Empty;
        }





        //##Implement
        protected virtual void Implement(string ctrlName, string funcName, string controlsNames, string values)
        {
        }

        //*end

    }
}
