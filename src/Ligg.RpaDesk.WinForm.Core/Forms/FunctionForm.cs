//using Ligg.Infrastructure.Handlers;
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.WinForm.Controls;
using Ligg.RpaDesk.WinForm.Controls.ShadowPanel;
using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.RpaDesk.WinForm.Helpers;
using Ligg.RpaDesk.WinForm.Resources;
using Ligg.WinFormBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Ligg.RpaDesk.WinForm.Forms
{
    //*start *begin
    public partial class FunctionForm : FrameForm
    {
        public bool BoolOutput = false;
        public string TextDataOutput;

        private FormInitParamSet _formInitParamSet;
        public FormInitParamSet FormInitParamSet
        {
            protected get => _formInitParamSet;
            set => _formInitParamSet = value;
        }

        private string _basicInfoForException;
        private string _additionalExceptionInfo;
        private string _formLocation;

        private string _startupDir;
        private string _cfgDir;
        private string _glbCfgDir;
        private string _glbSharedDir;
        private string _glbUiDir;

        private string _appCfgDir;
        private string _appSharedDir;
        private string _appUiDir;
        private string _portalsDir;
        private FormType _formType;
        private string _viewsDir;
        public string _zonesDir;
        private string _formCfgDir;
        private string _functionCode;

        private string _dataDir;
        private string _glbDataDir;
        private string _appDataDir;
        private string _libDir;
        private string _glbLibDir;
        private string _appLibDir;
        private string _appOutputDir;

        private string _sysDataDir;
        private string _sysAppDataDir;
        private string _myDataDir;
        private string _myAppDataDir;

        protected List<Procedure> _procedures = new List<Procedure>();
        protected List<Annex> _annexes = new List<Annex>();
        protected List<LayoutElement> _layoutElements = new List<LayoutElement>();
        protected List<ViewFeature> _viewFeatures = new List<ViewFeature>();
        protected List<MenuFeature> _menuFeatures = new List<MenuFeature>();
        protected List<ZoneItem> _zonesItems = new List<ZoneItem>();
        protected int _currentNestedMenuId { get; set; }
        protected string _currentViewName { get; set; }
        protected List<string> _renderedViewNames = new List<string>();

        private FormWindowState _ordinaryWindowStatus = FormWindowState.Normal;
        private readonly ToolTip _pictureBoxToolTip = new ToolTip();

        protected FunctionForm()
        {
            InitializeComponent();
            ToolBarSectionPublicRegionToolStrip.Enabled = true;
        }

        //*proc
        //*load
        //#FunctionForm_Load
        private void FunctionForm_Load(object sender, EventArgs e)
        {
            try
            {
                _startupDir = Directory.GetCurrentDirectory();
                _cfgDir = _startupDir + "\\Conf";
                _glbCfgDir = _cfgDir + "\\Global";
                _glbSharedDir = _glbCfgDir + "\\Shared";
                _glbUiDir = _glbCfgDir + "\\Ui\\WinForm";

                _appCfgDir = _cfgDir + "\\Apps\\" + _formInitParamSet.ApplicationCode;
                _appSharedDir = _appCfgDir + "\\Shared";
                _appUiDir = _appCfgDir + "\\Ui\\WinForm";
                _portalsDir = _appUiDir + "\\Portals";
                _viewsDir = _appUiDir + "\\views";
                _zonesDir = _appUiDir + "\\Zones";

                _formType = _formInitParamSet.FormType;
                _formCfgDir = _portalsDir + "\\" + _formInitParamSet.FormRelativeLocation; //for mvi
                _formLocation = "Portals" + "\\" + _formInitParamSet.FormRelativeLocation;
                _functionCode = _formInitParamSet.FormRelativeLocation;
                if (_formInitParamSet.FormType == FormType.Szi)
                {
                    _formCfgDir = FileHelper.GetPath(_formInitParamSet.FormRelativeLocation, _zonesDir, true);
                    _formLocation = "Zones" + "\\" + _formInitParamSet.FormRelativeLocation;
                    _functionCode = FileHelper.GetFileDetailByOption(_formCfgDir, FilePathComposition.FileTitle);

                }
                if (_formInitParamSet.FormType == FormType.Svi)
                {
                    _formCfgDir = FileHelper.GetPath(_formInitParamSet.FormRelativeLocation, _viewsDir, true);
                    _formLocation = "Views" + "\\" + _formInitParamSet.FormRelativeLocation;
                    _functionCode = FileHelper.GetFileDetailByOption(_formCfgDir, FilePathComposition.FileTitle);
                }

                _dataDir = _startupDir + "\\Data";
                _glbDataDir = _dataDir + "\\Global";
                _appDataDir = _formInitParamSet.ApplicationDataDir;
                _libDir = _startupDir + "\\Lib";
                _glbLibDir = _libDir + "\\Global";
                _appLibDir = _formInitParamSet.ApplicationLibDir;
                _appOutputDir = _formInitParamSet.ApplicationOutputDir;

                _sysDataDir = DirectoryHelper.GetSpecialDir("commonapplicationdata") + "\\" + _formInitParamSet.ArchitectureCode;
                _sysAppDataDir = _sysDataDir + "\\Apps\\" + _formInitParamSet.ApplicationCode;

                _myDataDir = DirectoryHelper.GetSpecialDir("mydocuments") + "\\" + _formInitParamSet.ArchitectureCode;
                _myAppDataDir = _myDataDir + "\\Apps\\" + _formInitParamSet.ApplicationCode;

                _basicInfoForException = _formInitParamSet.ApplicationCode + "." + _functionCode;
                //var userName = GetCentralData("UserName");
                var userName = ResolveConstantsEx("CurrentUserName");
                _additionalExceptionInfo = "Architecture: " + _formInitParamSet.ArchitectureCode + "-" + _formInitParamSet.ArchitectureVersion + ", " +
                    "Application: " + _formInitParamSet.ApplicationCode + "-" + _formInitParamSet.ApplicationVersion + ", " +
                    (userName.IsNullOrEmpty() ? "" : ", UserName: " + userName) + "\n--" +
               "Please send this error information to HelpdeskEmail: " + _formInitParamSet.HelpdeskEmail + ",";

                SetFrameTextByCulture(true, _formInitParamSet.SupportMultiLanguages);

                LoadForm();

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ".Load Error: _formLocation=" + _formLocation, ex.Message, _additionalExceptionInfo);

                if (!this.Modal) Application.Exit(); else CloseForm();
            }
        }

        private void FunctionForm_Resize(object sender, EventArgs e)
        {
            try
            {
                ResizeFrameComponent();
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ".FunctionForm_Resize" + " Error: _formLocation=" + _formLocation, ex.Message, _additionalExceptionInfo);
            }
        }

        //*eventHandler
        //#ToolBarSectionPublicRegionToolStripSplitButtonLanguageItemClickHandler
        private void ToolBarSectionPublicRegionToolStripSplitButtonLanguageItemClickHandler(object sender, EventArgs e)
        {
            try
            {
                var ctrl = sender as ToolStripMenuItem;
                var langId = Convert.ToString(ctrl.Tag);
                if (langId != LanguageHelper.CurrentId)
                {
                    var lang = LanguageHelper.GetLanguageById(langId);
                    var languageChanged = LanguageHelper.CurrentLanguageCode != lang.LanguageCode;
                    var cultureChanged = LanguageHelper.CurrentCultureName != lang.CultureName;

                    var splitButtonLang = ToolBarSectionPublicRegionToolStrip.Items.Find("ToolBarSectionPublicRegionToolStripSplitButtonLanguage", true)[0];
                    LanguageHelper.SetCurrent(langId);
                    if (languageChanged)
                    {
                        AnnexHelper.CurrentLanguageCode = LanguageHelper.CurrentLanguageCode;
                        SetLayoutTextByLanguage();
                        DoEx("UpdateCurrentLanguageCode", new string[] { LanguageHelper.CurrentLanguageCode });
                    }

                    if (cultureChanged)
                    {
                        LanguageHelper.SetCulture(LanguageHelper.CurrentCultureName);
                        SetFrameTextByCulture(false, _formInitParamSet.SupportMultiLanguages);
                        splitButtonLang.ToolTipText = TextRes.ChooseLanguage;
                    }

                    splitButtonLang.Tag = langId;
                    splitButtonLang.Image = ControlBaseHelper.GetImage(LanguageHelper.CurrentImageUrl);
                    splitButtonLang.Text = LanguageHelper.CurrentLanguageName;
                }
            }
            catch (Exception ex)
            {
                var methodName = "ToolBarSectionPublicRegionToolStripSplitButtonLanguageItemClickHandler";
                MessageHelper.PopupError(_basicInfoForException + "." + methodName + " Error: _formLocation=" + _formLocation, ex.Message, _additionalExceptionInfo);
            }
        }

        //#MenuItemClickHandler
        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            try
            {
                var type = sender.GetType().ToString();
                if (type.EndsWith("PictureBox")) ctrlName = (sender as PictureBox).Name;
                else if (type.EndsWith("ToolStripButton")) ctrlName = (sender as ToolStripButton).Name;//nestedMenu
                else if (type.EndsWith("Button")) ctrlName = (sender as Button).Name;//nestedMenu under panel
                else if (type.EndsWith("ToolStripMenuItem")) ctrlName = (sender as ToolStripMenuItem).Name; //horMenu
                else if (type.EndsWith("TreeViewEx")) ctrlName = (sender as TreeViewEx).Name;//VerMenu
                else throw new ArgumentException("Control type: " + type + " didn't be considered!");

                var menuItem = new LayoutElement();
                if (type.EndsWith("TreeViewEx"))
                {
                    var val = (sender as TreeViewEx).Value;
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
                        TransactByElementName(menuItem.Name.AddUiIdentifer(), TransactFrom.FromMenu);
                    }
                }
                else if (menuFeature.MenuType == (int)MenuType.Nested)
                {
                    var isValidLastMenu = false;
                    if (menuItem.IsLastLevel)
                    {
                        if (!menuItem.View.IsNullOrEmpty())
                        {
                            if (_viewFeatures.Find(x => x.Name.Contains(menuItem.View)) != null)
                                isValidLastMenu = true;
                        }
                    }

                    if (!menuItem.IsLastLevel | isValidLastMenu)
                    {
                        if (!menuItem.IsChecked)
                        {
                            var menuArea = _layoutElements.Find(x =>
                                x.Name == menuItem.Container && x.Type == (int)LayoutElementType.MenuArea && x.LayoutType == (int)LayoutType.Menu);
                            var lastCheckedParallelMenuItem = _layoutElements.Find(x =>
                                x.Container == menuArea.Name && x.Type == (int)LayoutElementType.MenuItem && x.LayoutType == (int)LayoutType.Menu && x.IsChecked);
                            if (lastCheckedParallelMenuItem != null) HideNestedMenuAreas(lastCheckedParallelMenuItem.Id);
                            CheckNestedMenuItemAndUncheckParallelItems(menuItem.Id);
                            UpdateNestedMenu(menuItem.Id, menuItem.LayoutId);
                        }
                    }

                    if (!menuItem.Action.IsNullOrEmpty())
                    {
                        TransactByElementName(menuItem.Name.AddUiIdentifer(), TransactFrom.FromMenu);
                    }
                }
                else if (menuFeature.MenuType == (int)MenuType.ToolBar)
                {
                    if (!menuItem.Action.IsNullOrEmpty())
                    {
                        TransactByElementName(menuItem.Name.AddUiIdentifer(), TransactFrom.FromMenu);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ".MenuItemClickHandler" + " Error: _formLocation=" + _formLocation + ", ctrlName=" + ctrlName, ex.Message, _additionalExceptionInfo);
            }
        }

        //#SubMenuItemClickHandler
        private void SubMenuItemClickHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            var transaction = new Transaction();
            try
            {

                var type = sender.GetType().ToString();
                if (type.EndsWith(MenuControlType.ToolStripSplitButtonEx.ToString()))//--Nested or Tool Menu Control
                {
                    var cpnt = sender as ToolStripSplitButtonEx;
                    ctrlName = (cpnt).Name;
                    transaction = cpnt.CurrentTransaction;
                }
                else throw new ArgumentException("Control type didn't be considered!");
                TransactByTransaction(transaction, TransactFrom.FromSubMenu);
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ".SubMenuItemClickHandler" + " Error: _formLocation=" + _formLocation + ", ctrlName=" + ctrlName + "", ex.Message, _additionalExceptionInfo);
            }
        }

        //#ContextMenuItemClickHandler
        private void ContextMenuItemClickHandler(object sender, EventArgs e)
        {
            var ctrlName = "";
            var transaction = new Transaction();
            try
            {
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ".ContextMenuItemClickHandler" + " Error: _formLocation=" + _formLocation + ", ctrlName= " + ctrlName + "", ex.Message, _additionalExceptionInfo);
            }
        }

        //#ZoneEventHandler
        private void ZoneEventHandler(string zoneName, ZoneItemType eventHandlerType)
        {
            try
            {
                var eventHandlers = _zonesItems.FindAll(x => x.Name.StartsWith(zoneName + "_") && x.Type == (int)eventHandlerType);
                foreach (var eventHandler in eventHandlers)
                {
                    var eventHandlerDisplayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", eventHandler.Name, _annexes, eventHandler.DisplayName);
                    TransactByElementName(eventHandler.Name.AddUiIdentifer(), TransactFrom.FromZoneEvent);
                }
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ".ZoneEventHandler" + " Error: _formLocation=" + _formLocation + ", zoneName= " + zoneName + "'", ex.Message, _additionalExceptionInfo);
            }
        }

        //#ControlEventHandler
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
                        TransactByElementName(item.Name.AddUiIdentifer(), TransactFrom.FromZoneUi);
                }
                else if (ctrlName.GetQtyOfIncludedChar('_') == 2)//zone item
                {
                    var item = _zonesItems.Find(x => x.Name == ctrlName);
                    if (!string.IsNullOrEmpty(item.Action))
                        TransactByElementName(item.Name.AddUiIdentifer(), TransactFrom.FromZoneUi);
                }

            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + "." + "ControlEventHandler" + " Error: _formLocation=" + _formLocation + ", ctrlName= " + ctrlName, ex.Message, _additionalExceptionInfo);
            }
        }

        //#ControlEventHandler1
        private void ControlEventHandler1(object sender, EventArgs e)
        {
            var ctrlName = "";
            try
            {
                var ctrl = sender as Control;
                var type = sender.GetType().ToString();
                //to be improved
                if (type.ToLower().EndsWith(MenuControlType.ToolStripMenuItem.ToString())) ctrlName = (sender as ToolStripMenuItem).Name;
                else if (type.EndsWith(MenuControlType.ToolStripButton.ToString())) ctrlName = (sender as ToolStripButton).Name;
                else
                {
                    ctrlName = ctrl.Name;
                }
                if (ctrlName.GetQtyOfIncludedChar('_') == 2)//zone item
                {
                    var item = _zonesItems.Find(x => x.Name == ctrlName);
                    if (!string.IsNullOrEmpty(item.Action1))
                    {
                        TransactByElementName(item.Name.AddUiIdentifer(), TransactFrom.FromZoneUi, 1);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageHelper.PopupError(_basicInfoForException + ". ControlEventHandler Error: _formLocation=" + _formLocation + ", ctrlName= " + ctrlName, ex.Message, _additionalExceptionInfo);
            }
        }

        //#ControlHoverHandler
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
                MessageHelper.PopupError(_basicInfoForException + ".ControlHoverHandler Error: _formLocation=" + _formLocation + ", ctrlName= " + ctrlName, ex.Message, _additionalExceptionInfo);
            }
        }


        //*func
        //*form
        //##LoadForm
        protected void LoadForm()
        {
            try
            {
                _annexes.AddRange(CommonHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\SharedAnnexes", "Shared", false));

                var annexes = CommonHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\ApplicationAnnexes", "", false);
                CommonHelper.SetApplicationAnnexes(annexes, _formInitParamSet.ApplicationCode);
                _annexes.AddRange(annexes);

                var annexes1 = CommonHelper.GetAnnexesFromCfgFile(_formCfgDir + "\\FormTitleAnnexes", "", false);
                CommonHelper.SetFormTitleAnnexes(annexes1, _functionCode);
                _annexes.AddRange(annexes1);

                var threadPoolMaxNum = 1;

                if (_formType == FormType.Mvi)
                {
                    var formStyle = DataParserHelper.ConvertToGeneric<FormStyle>(_formCfgDir + "\\FormStyle", true, TxtDataType.Undefined) ?? new FormStyle();
                    formStyle.IconUrl = formStyle.IconUrl.IsNullOrEmpty() ? "" : ResolveConstants(formStyle.IconUrl);
                    InitLayout(_formInitParamSet.FormType, formStyle);
                    ResizeForm(formStyle.ResizeParamsText);
                    if (_formInitParamSet.SupportMultiLanguages) InitToolBarSectionPublicRegionComponents();

                    var viewFeatures = DataParserHelper.ConvertToGeneric<List<ViewFeature>>(FileHelper.GetPath("\\ViewFeatures", _formCfgDir, true), true, TxtDataType.Undefined) ?? new List<ViewFeature>();
                    foreach (var viewFeature in viewFeatures)
                    {
                        viewFeature.Invalid = ResolveConstants(viewFeature.Invalid);
                        if (viewFeature.Invalid.StartsWith("=")) viewFeature.Invalid = GetText(viewFeature.Invalid, GetTextType.OnlyResolveCed);
                        viewFeature.Invalid = viewFeature.Invalid.GetJudgementFlag();
                    }
                    viewFeatures = viewFeatures.FindAll(x => (x.Invalid.ToLower() == "false"));

                    UiHelper.CheckViewFeatures(viewFeatures);
                    _viewFeatures = viewFeatures;
                    foreach (var viewFeature in _viewFeatures)
                    {


                        viewFeature.Location = string.IsNullOrEmpty(viewFeature.Location) ? "" : viewFeature.Location;
                        viewFeature.Location = ResolveConstants(viewFeature.Location);
                        var loc = viewFeature.Location;
                        loc = !loc.IsNullOrEmpty() ? DirectoryHelper.GetPathByRelativePath(viewFeature.Location, _appUiDir + "\\Views") : "";
                        viewFeature.Location = loc;

                        if (viewFeature.ResizeParamsText.IsNullOrEmpty())
                            viewFeature.ResizeParamsText = formStyle.ResizeParamsText;
                    }

                    //render public view
                    var publicViewFeature = _viewFeatures.Find(x => x.IsPublic);
                    if (publicViewFeature == null) throw new ArgumentException("Can not find Public View!");
                    MergeViewItems(publicViewFeature.Name);
                    RenderView(publicViewFeature.Name);

                    //--Menu
                    //var menuFeatures = CommonHelper.GetGenericFromCfgFile<List<MenuFeature>>(FileHelper.GetPath("\\MenuFeatures", _formCfgDir,true), false) ?? new List<MenuFeature>();
                    var menuFeatures = DataParserHelper.ConvertToGeneric<List<MenuFeature>>(FileHelper.GetPath("\\MenuFeatures", _formCfgDir, true), false, TxtDataType.Undefined) ?? new List<MenuFeature>();
                    if (menuFeatures.Count > 0)
                    {
                        foreach (var menuFeature in menuFeatures)
                        {
                            menuFeature.Location = string.IsNullOrEmpty(menuFeature.Location) ? "" : menuFeature.Location;
                            menuFeature.Location = ResolveConstants(menuFeature.Location);
                            var loc = menuFeature.Location;
                            loc = !loc.IsNullOrEmpty() ? DirectoryHelper.GetPathByRelativePath(menuFeature.Location, _formCfgDir + "\\Menus") : "";
                            menuFeature.Location = loc;
                            menuFeature.DataSource = string.IsNullOrEmpty(menuFeature.DataSource) ? "" : menuFeature.DataSource;
                            menuFeature.DataSource = ResolveConstants(menuFeature.DataSource);
                            if (!menuFeature.DataSource.IsNullOrEmpty())
                            {
                            }

                            menuFeature.ImageUrls = string.IsNullOrEmpty(menuFeature.ImageUrls) ? "" : menuFeature.ImageUrls;
                            menuFeature.ImageUrls = ResolveConstants(menuFeature.ImageUrls);

                            menuFeature.MenuType = EnumHelper.GetIdByName<MenuType>(menuFeature.MenuTypeName);
                            if (menuFeature.MenuType == (int)MenuType.Nested | menuFeature.MenuType == (int)MenuType.ToolBar)
                            {
                                menuFeature.Container = string.Empty;
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
                                menuItem.ImageUrl = GetImageUrl(menuItem.ImageUrl, menuFeature.Location);
                            }

                            if (menuItems.Count != 0) _layoutElements.AddRange(menuItems);
                        }

                        //if RefeshUI, or refreshUI after logon , _currentViewName is not empty
                        var firstViewName = "";
                        if (!_currentViewName.IsNullOrEmpty())
                        {
                            firstViewName = _currentViewName;
                            _currentViewName = "";
                        }
                        else if (!_formInitParamSet.StartViewName.IsNullOrEmpty()) firstViewName = _formInitParamSet.StartViewName;

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
                else//--svi/szi start
                {
                    var formStyleCfgFile = _formCfgDir + "\\FormStyle";

                    var formStyle = DataParserHelper.ConvertToGeneric<FormStyle>(formStyleCfgFile, _formType == FormType.Svi ? true : false, TxtDataType.Undefined) ?? new FormStyle();
                    formStyle.IconUrl = formStyle.IconUrl.IsNullOrEmpty() ? "" : ResolveConstants(formStyle.IconUrl);


                    var zoneFeature = new ZoneFeature();
                    if (_formType == FormType.Szi)
                    {
                        zoneFeature = DataParserHelper.ConvertToGeneric<ZoneFeature>(_formCfgDir + "\\Feature", true, TxtDataType.Undefined) ?? new ZoneFeature();
                        formStyle.Width = formStyle.Width < 0 | formStyle.Width == 0 ? zoneFeature.Width : formStyle.Width;
                        formStyle.Height = formStyle.Height < 0 | formStyle.Height == 0 ? zoneFeature.Height : formStyle.Height;
                    }

                    formStyle.Width = formStyle.Width < 0 | formStyle.Width == 0 ? 800 : formStyle.Width;
                    formStyle.Height = formStyle.Height < 0 | formStyle.Height == 0 ? 600 : formStyle.Height;

                    formStyle.HasNoControlBoxes = formStyle.HasNoControlBoxes;
                    formStyle.HasNoMaximizeBox = true;
                    formStyle.DrawIcon = formStyle.DrawIcon;
                    formStyle.IconUrl = formStyle.IconUrl;

                    if (_formType == FormType.Szi)
                    {
                        formStyle.ResizeParamsText = "MainSectionMainDivision: 0,0| " + "TopNavSection:0| ToolBarSection: 0,0,0| MiddleNavSection: 0,0| DownNavSection: 0,0| " + "" +
                        "MainSectionLeftNavDivision: 0,0,0| MainSectionLeftNavDivision1: 0,0,0| MainSectionMainDivision: 0,0| MainSectionRightDivision: 0,0,0| " +

                        ("RunningMessageHeight:" + formStyle.RunningMessageHeight.ToString()) + "| " +
                        ("ShowRunningProgress:" + formStyle.ShowRunningProgress.ToString().ToString()) + "| " +
                        ("ShowRunningStatus:" + formStyle.ShowRunningStatus.ToString().ToString()) + "| " +
                        "HorResizableDivisionStatus: none|HorResizableDivision1Status: none|";
                    }

                    InitLayout(_formInitParamSet.FormType, formStyle);
                    ResizeForm(formStyle.ResizeParamsText);

                    //render public view
                    if (_formType == FormType.Svi)
                    {
                        var publicViewFeature = new ViewFeature();
                        publicViewFeature.Name = "PublicView";
                        publicViewFeature.IsPublic = true;
                        publicViewFeature.Location = _formCfgDir;
                        _viewFeatures.Add(publicViewFeature);
                        MergeViewItems(publicViewFeature.Name);
                        RenderView(publicViewFeature.Name);
                    }
                    else if (_formType == FormType.Szi)
                    {
                        var areaLayoutElement = new LayoutElement()
                        {
                            Id = 10,
                            LayoutType = (int)LayoutType.View,
                            View = "PublicView",
                            Name = "PublicView" + "_" + "PublicArea",
                            Type = (int)LayoutElementType.ContentArea,
                            Container = "MainSectionMainDivisionMiddleRegion",
                            DockType = (int)ControlDockType.Fill,
                            DockOrder = "10",
                            Width = -1,
                            Height = -1,
                        };
                        _layoutElements.Add(areaLayoutElement);

                        var zoneName = _functionCode;
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

                            DataSource = zoneFeature.DataSource,
                        };
                        _layoutElements.Add(zoneLayoutElement);
                        RenderView("PublicView");
                    }
                }//--svi/szi end

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".LoadForm Error: " + ex.Message);
            }
        }

        //*layout
        //*init
        //##InitLayout
        private void InitLayout(FormType formType, FormStyle formStyle)
        {
            var isFormModal = this.Modal;

            try
            {
                if (_formInitParamSet.FormTitle.IsNullOrEmpty())
                {
                    var text = "";
                    if (_formInitParamSet.SupportMultiLanguages)
                    {
                        text = AnnexHelper.GetText("FormTitle", _functionCode, _annexes, AnnexTextType.DisplayName, GetAnnexMode.StepByStep, _formInitParamSet.ApplicationCode + "-" + _functionCode);
                    }
                    else
                    {
                        text = AnnexHelper.GetText("FormTitle", _functionCode, _annexes, AnnexTextType.DisplayName, GetAnnexMode.CurrentOrFirst, _formInitParamSet.ApplicationCode + "-" + _functionCode);
                    }
                    Text = text;

                }
                else
                {
                    Text = _formInitParamSet.FormTitle;
                }
                TopMost = formStyle.TopMost;
                ShowInTaskbar = !formStyle.NotShowInTaskbar;
                if (formStyle.DrawIcon)
                {
                    var iconUrl = formStyle.IconUrl;
                    iconUrl = GetImageUrl(iconUrl, _formCfgDir + "\\Icos");
                    Icon = GetIcon(iconUrl);
                }

                WindowState = FormWindowState.Normal;
                if (formStyle.HasNoControlBoxes)
                {
                    ControlBox = false;
                }

                MaximizeBox = !formStyle.HasNoMaximizeBox;
                if (isFormModal)
                {
                    MaximizeBox = false;
                }
                MinimizeBox = !formStyle.HasNoMinimizeBox;
                if (isFormModal)
                {
                    MinimizeBox = false;
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

                var frmW = formStyle.Width > 0 ? formStyle.Width : 1024;
                var frmH = formStyle.Height > 0 ? formStyle.Height : 768;
                if (formType == FormType.Mvi)
                {
                    //this.ClientSize = new System.Drawing.Size(formStyle.Width, formStyle.Height);
                    //if (WindowState != FormWindowState.Maximized)
                    {
                        //sv++
                        Width = 12 + frmW + 4;
                        Height = 9 + frmH + 30 + 25;//ShowRunningStatus--25
                    }
                }
                else if (formType == FormType.Svi)
                {
                    //sv++
                    Width = 12 + frmW + 4;
                    Height = 9 + frmH + 30 + 25;//ShowRunningStatus--25
                }
                else
                {
                    //sv++
                    Width = 12 + frmW + 4;
                    Height = 9 + frmH + 30 + formStyle.RunningMessageHeight + (formStyle.ShowRunningProgress ? 30 : 0) + (formStyle.ShowRunningStatus ? 25 : 0);//35
                }
                if (isFormModal)
                {
                    MinimizeBox = false;
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

                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".InitLayout Error:" + ex.Message);
            }
        }

        //##SetLayoutTextByCulture
        private void SetLayoutTextByLanguage()
        {
            var elmentEx = "";
            try
            {
                //form title
                var text = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    text = AnnexHelper.GetText("FormTitle", _functionCode, _annexes, AnnexTextType.DisplayName, GetAnnexMode.CurrentOrDefault, _formInitParamSet.ApplicationCode + "-" + _functionCode);
                }
                else
                {
                    text = AnnexHelper.GetText("FormTitle", _functionCode, _annexes, AnnexTextType.DisplayName, GetAnnexMode.First, _formInitParamSet.ApplicationCode + "-" + _functionCode);
                }
                Text = text;

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
                            var area = _layoutElements.Find(x => (x.Type == (int)LayoutElementType.MenuArea) & x.Name.EndsWith(ctnName) & x.IsRendered);
                            if (area != null)
                            {
                                if (area.ControlTypeName == MenuControlType.ToolStrip.ToString())
                                {
                                    var areaControl = GetControl(area.Name);
                                    var areaToolStrip = areaControl as ToolStrip;
                                    var elmtControls = areaToolStrip.Items.Find(elmt.Name, true);

                                    if (elmtControls != null && elmtControls.Length > 0)
                                    {
                                        elmtControls[0].Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
                                    }

                                    if (!string.IsNullOrEmpty(elmt.DisplayName) & (elmt.ControlTypeName == MenuControlType.ToolButton.ToString() | elmt.ControlTypeName == MenuControlType.ImageToolButton.ToString() |
                                        elmt.ControlTypeName == MenuControlType.ImageTextToolButtonH.ToString() | elmt.ControlTypeName == MenuControlType.ImageTextToolButtonV.ToString()))
                                    {
                                        elmtControls[0].Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
                                    }

                                    if (elmt.ControlTypeName == MenuControlType.ImageToolSplitButton.ToString() | elmt.ControlTypeName == MenuControlType.ImageTextToolSplitButtonH.ToString() |
                                        elmt.ControlTypeName == MenuControlType.ImageTextToolSplitButtonV.ToString())  //--subMenu
                                    {
                                        var ctrl = elmtControls[0] as ToolStripSplitButtonEx;
                                        ctrl.SetTextByCulture();
                                    }
                                }
                                else//panel
                                {
                                    var elmtControl = GetControl(elmt.Name);
                                    elmtControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
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
                            var area = _layoutElements.Find(x => (x.Type == (int)LayoutElementType.MenuArea) & x.Name.EndsWith(ctnName) & x.IsRendered);
                            if (area != null)
                            {
                                if (area.ControlTypeName == "MenuStrip")
                                {
                                    var areaControl = GetControl(area.Name);
                                    var areaMenuStrip = areaControl as MenuStrip;
                                    var elmtControls = areaMenuStrip.Items.Find(elmt.Name, true);

                                    if (elmtControls != null && elmtControls.Length > 0)
                                    {
                                        elmtControls[0].Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", elmt.Name, _annexes, elmt.DisplayName);
                                    }
                                }
                            }
                        }
                    }
                    else if (menuFeature.MenuType == (int)MenuType.Vertical)
                    {
                        var area = _layoutElements.Find(x => x.Type == (int)LayoutElementType.MenuArea & x.LayoutId == menuFeature.Id);
                        var areaName = area.Name;
                        var menuName = areaName + "-verticalMenu";
                        var areaControl = GetControl(areaName);
                        var menuControl = areaControl.Controls.Find(menuName, true)[0];
                        var verticalMenu = menuControl as TreeViewEx;
                        verticalMenu.SetTextByCulture();

                    }
                }

                //zone
                var zoneItems = _zonesItems.FindAll(x => (x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.SubControl) & x.ControlTypeName != ZoneControlType.Row.ToString());
                foreach (var zoneItem in zoneItems)
                {
                    elmentEx = zoneItem.Name;
                    var zoneItemControl = GetControl(zoneItem.Name);
                    if (zoneItem.ControlTypeName == ZoneControlType.Label.ToString() | zoneItem.ControlTypeName == ZoneControlType.TitleLabel.ToString() | zoneItem.ControlTypeName == ZoneControlType.RadioButton.ToString() |
                        zoneItem.ControlTypeName == ZoneControlType.CheckBox.ToString() | zoneItem.ControlTypeName == ZoneControlType.StatusLight.ToString() | zoneItem.ControlTypeName == ZoneControlType.Button.ToString() |
                        zoneItem.ControlTypeName == ZoneControlType.TextButton.ToString() | zoneItem.ControlTypeName == ZoneControlType.ImageTextButtonH.ToString() | zoneItem.ControlTypeName == ZoneControlType.ImageTextButtonV.ToString()
                        )
                    {
                        if (zoneItem.DisplayName.StartsWith("="))
                        {
                            zoneItemControl.Text = GetText(zoneItem.DisplayName, GetTextType.UiItem);
                        }
                        else
                        {
                            zoneItemControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                        }
                    }
                    else if (zoneItem.ControlTypeName == ZoneControlType.TextBox.ToString() | zoneItem.ControlTypeName == ZoneControlType.RichTextBox.ToString())
                    {
                        if (!zoneItem.StyleText.GetLdictValue("DisplayByLang", true, true).JudgeJudgementFlag()) continue;
                        var val = GetText(zoneItem.Value, GetTextType.UiItem);
                        zoneItemControl.Text = val;
                    }
                    else if (zoneItem.ControlTypeName == ZoneControlType.PictureBox.ToString())
                    {
                        if (zoneItem.DisplayName.StartsWith("="))
                        {
                            zoneItemControl.Tag = GetText(zoneItem.DisplayName, GetTextType.UiItem);
                        }
                        else
                        {
                            zoneItemControl.Tag = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                        }
                        if (!zoneItem.StyleText.GetLdictValue("ShowImageByLang", true, true).JudgeJudgementFlag()) continue;

                        var dataSrc = "";
                        if (!zoneItem.DataSource.IsNullOrEmpty())
                        {
                            dataSrc = GetText(zoneItem.DataSource, GetTextType.UiItem);
                        }
                        var arry = zoneItem.Name.Split('_');
                        var zoneName = arry.Unwrap('_'.ToString(), 0, 2);
                        var zone = _layoutElements.Find(x => x.Name == zoneName);
                        var imagUrl = GetImageUrl(dataSrc, zone.Location);
                        ControlBaseHelper.SetControlBackgroundImage(zoneItemControl, imagUrl);

                    }
                    else
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".SetLayoutTextByLanguage Error: " + ex.Message + elmentEx); ;
            }
        }

        //*init
        //##InitPublicRegionComponent
        private void InitToolBarSectionPublicRegionComponents()
        {
            if (!_formInitParamSet.SupportMultiLanguages) return;
            if (LanguageHelper.Languages.Count == 0) return;
            try
            {
                var toolBarSectionPublicRegionSplitButtonLanguage = new System.Windows.Forms.ToolStripSplitButton();
                toolBarSectionPublicRegionSplitButtonLanguage.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
                toolBarSectionPublicRegionSplitButtonLanguage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                toolBarSectionPublicRegionSplitButtonLanguage.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
                toolBarSectionPublicRegionSplitButtonLanguage.ForeColor = System.Drawing.Color.Azure;
                toolBarSectionPublicRegionSplitButtonLanguage.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
                //--by liggin2019 at 211119
                //ok for env of  fx 4.72 &vs 2019-16.92 
                //toolBarSectionPublicRegionSplitButtonCulture.Image = (System.Drawing.Image)(Properties.Resources.ChooseLanguage);
                //--by liggin2019 at 200313
                //not work in env of  fx 4.72 &vs 2019-16.92 
                //var resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionForm));
                //toolBarSectionPublicRegionSplitButtonCulture.Image = ((System.Drawing.Image)(resources.GetObject("ChooseLanguage")));

                //toolBarSectionPublicRegionSplitButtonLanguage.Image = imageListCultures.Images["cultures.png"];
                toolBarSectionPublicRegionSplitButtonLanguage.Image = ControlBaseHelper.GetImage(LanguageHelper.CurrentImageUrl);

                toolBarSectionPublicRegionSplitButtonLanguage.ImageTransparentColor = System.Drawing.Color.Magenta;
                toolBarSectionPublicRegionSplitButtonLanguage.Name = "ToolBarSectionPublicRegionToolStripSplitButtonLanguage";
                toolBarSectionPublicRegionSplitButtonLanguage.Size = new System.Drawing.Size(97, 56);
                toolBarSectionPublicRegionSplitButtonLanguage.Text = LanguageHelper.CurrentLanguageName;
                toolBarSectionPublicRegionSplitButtonLanguage.ToolTipText = TextRes.ChooseLanguage;
                foreach (var lang in LanguageHelper.Languages)
                {
                    var toolBarSectionPublicRegionSplitButtonLanguageItem = new System.Windows.Forms.ToolStripMenuItem();
                    toolBarSectionPublicRegionSplitButtonLanguageItem.Image = ControlBaseHelper.GetImage(lang.ImageUrl);
                    toolBarSectionPublicRegionSplitButtonLanguageItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
                    toolBarSectionPublicRegionSplitButtonLanguageItem.Name = "ToolBarSectionPublicRegionToolStripSplitButtonLanguageItem" + "_" + lang.Id;
                    toolBarSectionPublicRegionSplitButtonLanguageItem.Size = new System.Drawing.Size(124, 22);
                    toolBarSectionPublicRegionSplitButtonLanguageItem.Tag = lang.Id;
                    toolBarSectionPublicRegionSplitButtonLanguageItem.Text = lang.LanguageName;
                    toolBarSectionPublicRegionSplitButtonLanguageItem.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                    toolBarSectionPublicRegionSplitButtonLanguageItem.Click += new System.EventHandler(ToolBarSectionPublicRegionToolStripSplitButtonLanguageItemClickHandler);
                    toolBarSectionPublicRegionSplitButtonLanguage.DropDownItems.Add(toolBarSectionPublicRegionSplitButtonLanguageItem);
                }
                this.ToolBarSectionPublicRegionToolStrip.Items.Add(toolBarSectionPublicRegionSplitButtonLanguage);

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".InitPublicRegionComponent Error:" + ex.Message);
            }
        }

        //*resize
        //##ResizeForm
        private void ResizeForm(string resizeStrs)
        {
            try
            {
                var tempInt = 0;
                var resizeStr = "";
                var resizeStrDef = "0,0,0";
                var dict = resizeStrs.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                resizeStr = dict.GetLdictValue("TopNavSection");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    TopNavSectionHeight = tempInt > 0 ? tempInt : 0;
                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        TopNavSectionLeftRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        TopNavSectionRightRegionWidth = tempInt = Convert.ToInt16(resizeStrArry[2]) > 0 ? tempInt : 0;
                    }
                }



                resizeStrDef = "0,0,0";
                resizeStr = dict.GetLdictValue("ToolBarSection");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    ToolBarSectionHeight = tempInt > 0 ? tempInt : 0;
                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = (Convert.ToInt16(resizeStrArry[1]));
                        ToolBarSectionLeftRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[2]);
                        ToolBarSectionRightRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                    //if (resizeStrArry.Length > 3)
                    //{
                    //    tempInt = Convert.ToInt16(resizeStrArry[3]);
                    //    ToolBarSectionPublicRegionWidth = tempInt > 0 ? tempInt : 0;
                    //}
                }

                resizeStrDef = "0,0,0";
                resizeStr = dict.GetLdictValue("MiddleNavSection");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    MiddleNavSectionHeight = tempInt > 0 ? tempInt : 0;

                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        MiddleNavSectionLeftRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[2]);
                        MiddleNavSectionRightRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                }

                resizeStrDef = "0,0,0";
                resizeStr = dict.GetLdictValue("DownNavSection");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    DownNavSectionHeight = tempInt > 0 ? tempInt : 0;

                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        DownNavSectionLeftRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[2]);
                        DownNavSectionRightRegionWidth = tempInt > 0 ? tempInt : 0;
                    }
                }


                this.MainSection.Padding = new Padding(0);
                resizeStrDef = "0,0,0";
                resizeStr = dict.GetLdictValue("MainSectionLeftNavDivision");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    MainSectionLeftNavDivisionWidth = tempInt > 0 ? tempInt : 0;
                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        MainSectionLeftNavDivisionUpRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[2]);
                        MainSectionLeftNavDivisionDownRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                }

                resizeStrDef = "0,0,0";
                resizeStr = dict.GetLdictValue("MainSectionLeftNavDivision1");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    MainSectionLeftNavDivision1Width = tempInt > 0 ? tempInt : 0;
                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        MainSectionLeftNavDivision1UpRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[2]);
                        MainSectionLeftNavDivision1DownRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                }

                resizeStrDef = "0,0";
                resizeStr = dict.GetLdictValue("MainSectionMainDivision");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    MainSectionMainDivisionUpRegionHeight = tempInt > 0 ? tempInt : 0;
                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        MainSectionMainDivisionDownRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                }

                resizeStrDef = "0,0,0";
                resizeStr = dict.GetLdictValue("MainSectionRightDivision");
                resizeStr = resizeStr.IsNullOrEmpty() ? resizeStrDef : resizeStr;
                {
                    var resizeStrArry = resizeStr.Split(',');
                    tempInt = Convert.ToInt16(resizeStrArry[0]);
                    MainSectionRightDivisionWidth = tempInt > 0 ? tempInt : 0;
                    if (resizeStrArry.Length > 1)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[1]);
                        MainSectionRightDivisionUpRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                    if (resizeStrArry.Length > 2)
                    {
                        tempInt = Convert.ToInt16(resizeStrArry[2]);
                        MainSectionRightDivisionDownRegionHeight = tempInt > 0 ? tempInt : 0;
                    }
                }

                RunningMessageSectionHeight = 0;
                resizeStr = dict.GetLdictValue("RunningMessageHeight");
                if (!resizeStr.IsNullOrEmpty())
                {
                    tempInt = Convert.ToInt16(resizeStr);
                    RunningMessageSectionHeight = tempInt > 0 ? tempInt : 0;
                }

                ShowRunningProgressSection = false;
                resizeStr = dict.GetLdictValue("ShowRunningProgress");
                if (!resizeStr.IsNullOrEmpty())
                {
                    ShowRunningProgressSection = resizeStr.JudgeJudgementFlag();
                }

                ShowRunningStatusSection = false;
                resizeStr = dict.GetLdictValue("ShowRunningStatus");
                if (!resizeStr.IsNullOrEmpty())
                {
                    ShowRunningStatusSection = resizeStr.JudgeJudgementFlag();
                }
                if (_formType == FormType.Mvi) ShowRunningStatusSection = true;
                //if (_supportMultiThreads) ShowThreadInfo = true;

                HorizontalResizableDivisionStatus = ResizableDivisionStatus.None;
                resizeStr = dict.GetLdictValue("HorResizableDivisionStatus");
                if (!resizeStr.IsNullOrEmpty())
                {
                    if (resizeStr.ToLower() == "show")
                    {
                        HorizontalResizableDivisionStatus = ResizableDivisionStatus.Shown;
                    }
                    else if (resizeStr.ToLower() == "hide")
                    {
                        HorizontalResizableDivisionStatus = ResizableDivisionStatus.Hidden;
                    }
                }

                HorizontalResizableDivision1Status = ResizableDivisionStatus.None;
                resizeStr = dict.GetLdictValue("HorResizableDivision1Status");
                if (!resizeStr.IsNullOrEmpty())
                {
                    if (resizeStr.ToLower() == "show")
                    {
                        HorizontalResizableDivision1Status = ResizableDivisionStatus.Shown;
                    }
                    else if (resizeStr.ToLower() == "hide")
                    {
                        HorizontalResizableDivision1Status = ResizableDivisionStatus.Hidden;
                    }
                }

                InitFrameHorizontalResizableDivisionStatus();
                ResizeFrameComponent();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResizeForm Error: " + ex.Message);
            }
        }
        private void ResizeAdditionalRegion(int width, int height)
        {
            var positionX = MainSection.Width - width;
            var positionY = MainSection.Location.Y + MainSection.Height - height;
            var cpnt = GetControl("AdditionalRegion");
            ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, (int)DockStyle.None, width, height, positionX, positionY);
        }

        //*menu
        //##GetMenuItems
        private List<LayoutElement> GetMenuItems(MenuFeature menuFeature)
        {
            try
            {
                var tmpMenuItems = new List<LayoutElement>();
                if (menuFeature.DataSource.IsNullOrEmpty())
                {
                    var cfgFile = menuFeature.Location + "\\Ui";
                    tmpMenuItems = DataParserHelper.ConvertToGeneric<List<LayoutElement>>(cfgFile, false, TxtDataType.Undefined) ?? new List<LayoutElement>();
                }

                if (tmpMenuItems.Count == 0) return tmpMenuItems;

                if (menuFeature.MenuType == (int)MenuType.Horizontal)
                {
                    tmpMenuItems = tmpMenuItems.FindAll(x => x.ControlTypeName == MenuControlType.ToolStripMenuItem.ToString()
                    | x.ControlTypeName == MenuControlType.ImageToolStripMenuItemH.ToString() | x.ControlTypeName == MenuControlType.ImageToolStripMenuItemV.ToString());
                }
                else if (menuFeature.MenuType == (int)MenuType.Nested | menuFeature.MenuType == (int)MenuType.ToolBar)
                {
                    tmpMenuItems = tmpMenuItems.FindAll(x => x.ControlTypeName == MenuControlType.ToolStrip.ToString() |
                    x.ControlTypeName == MenuControlType.ToolLabel.ToString() | x.ControlTypeName == MenuControlType.ToolButton.ToString() |
                    x.ControlTypeName == MenuControlType.ImageToolButton.ToString() | x.ControlTypeName == MenuControlType.ImageTextToolButtonH.ToString() | x.ControlTypeName == MenuControlType.ImageTextToolButtonV.ToString() |

                    x.ControlTypeName == MenuControlType.ToolSplitButton.ToString() | x.ControlTypeName == MenuControlType.ImageToolSplitButton.ToString() | x.ControlTypeName == MenuControlType.ImageTextToolSplitButtonH.ToString() | x.ControlTypeName == MenuControlType.ImageTextToolSplitButtonV.ToString() |
                    x.ControlTypeName == MenuControlType.Panel.ToString() |
                    x.ControlTypeName == MenuControlType.TextButton.ToString() | x.ControlTypeName == MenuControlType.ImageTextButtonH.ToString() |
                    x.ControlTypeName == MenuControlType.ImageTextButtonV.ToString() | x.ControlTypeName == MenuControlType.CommandLabel.ToString()
                   );
                }


                var unavailableServerViews = GetText(menuFeature.UnavailableServerViews, GetTextType.OnlyResolveCed);
                var unavailableServerViewsArr = unavailableServerViews.GetLarrayArray(true, true);
                if (unavailableServerViewsArr != null)
                {
                    var unavailableViewFeatures = _viewFeatures.FindAll(x => unavailableServerViewsArr.ToLower().Contains(x.Alias.ToLower()));
                    if (unavailableViewFeatures.Count > 0)
                    {
                        var unavailableViewNameArr = unavailableViewFeatures.Select(x => x.Name).ToArray();
                        if (unavailableViewNameArr != null)
                        {
                            tmpMenuItems = tmpMenuItems.FindAll(x => !unavailableViewNameArr.ToLower().Contains(x.View.ToLower()));
                        }
                    }
                }


                foreach (var elmt in tmpMenuItems)
                {
                    if (menuFeature.MenuType == (int)MenuType.Horizontal | menuFeature.MenuType == (int)MenuType.Vertical)
                        elmt.TypeName = "MenuItem";
                    elmt.Location = _formCfgDir + "\\Menus";

                    elmt.Invalid = ResolveConstants(elmt.Invalid);
                    if (elmt.Invalid.StartsWith("=")) elmt.Invalid = GetText(elmt.Invalid, GetTextType.OnlyResolveCed);
                    elmt.Invalid = elmt.Invalid.GetJudgementFlag();
                }

                var menuItems = tmpMenuItems.Where(x =>
                     (x.Invalid.ToLower() != "true") &
                     (x.TypeName == LayoutElementType.MenuArea.ToString() | x.TypeName == LayoutElementType.MenuItem.ToString())
                   )
                  .ToList();

                if (menuItems.Count == 0) return null;

                foreach (var elmt in menuItems)
                {
                    UiHelper.SetLayoutElementTypes(elmt);
                    if (!elmt.View.IsNullOrEmpty())
                    {
                        var views = _viewFeatures.Select(x => x.Name);
                        if (views != null)
                        {
                            if (views.Count() > 0)
                            {
                                if (!views.Contains(elmt.View))
                                {
                                    throw new ArgumentException("View= " + elmt.View + " does not exist; menuName= " + menuFeature.Name + ", menuType = " + menuFeature.MenuTypeName);
                                }
                            }
                        }

                    }

                }
                UiHelper.CheckMenuItems(menuFeature.MenuType, menuItems);

                var annexCfgFile = menuFeature.Location + "\\Annexes";
                var annexList = new List<Annex>();
                var tempAnnexes = new List<Annex>();

                if (_formInitParamSet.SupportMultiLanguages)
                {
                    annexList = CommonHelper.GetAnnexesFromCfgFile(annexCfgFile, "MenuItem", false);
                }

                foreach (var elmt in menuItems)
                {
                    elmt.LayoutType = (int)LayoutType.Menu;
                    elmt.LayoutId = menuFeature.Id;

                    elmt.Invisible = ResolveConstants(elmt.Invisible);
                    if (elmt.Invisible.StartsWith("=")) elmt.Invisible = GetText(elmt.Invisible, GetTextType.OnlyResolveCed);
                    elmt.Invisible = elmt.Invisible.GetJudgementFlag();

                    elmt.Disabled = ResolveConstants(elmt.Disabled);
                    if (elmt.Disabled.StartsWith("=")) elmt.Disabled = GetText(elmt.Disabled, GetTextType.OnlyResolveCed);
                    elmt.Disabled = elmt.Disabled.GetJudgementFlag();
                    elmt.DisplayName = string.IsNullOrEmpty(elmt.DisplayName) ? "" : elmt.DisplayName;
                    elmt.DisplayName = ResolveConstants(elmt.DisplayName);
                    elmt.StyleText = string.IsNullOrEmpty(elmt.StyleText) ? "" : elmt.StyleText;
                    elmt.StyleText = ResolveConstants(elmt.StyleText);
                    elmt.DataSource = string.IsNullOrEmpty(elmt.DataSource) ? "" : elmt.DataSource;
                    elmt.DataSource = ResolveConstants(elmt.DataSource);
                    elmt.ImageUrl = string.IsNullOrEmpty(elmt.ImageUrl) ? "" : elmt.ImageUrl;
                    elmt.ImageUrl = ResolveConstants(elmt.ImageUrl);


                    elmt.ShowRunningStatus = string.IsNullOrEmpty(elmt.ShowRunningStatus) ? "" : elmt.ShowRunningStatus;
                    elmt.ShowRunningStatus = ResolveConstants(elmt.ShowRunningStatus);

                    if (elmt.Type == (int)LayoutElementType.MenuItem)
                    {
                        if (menuFeature.MenuType == (int)MenuType.Horizontal | menuFeature.MenuType == (int)MenuType.Vertical)
                        {
                            elmt.Container = menuFeature.Container + "Menu" + menuFeature.Id + "Area";
                        }

                        if (elmt.Type == (int)LayoutElementType.MenuItem)
                        {
                            var elmtNameNew = elmt.Container + elmt.Name;
                            if (_formInitParamSet.SupportMultiLanguages)
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
                        if (!elmt.View.IsNullOrEmpty()) elmt.View = GetText(elmt.View, GetTextType.OnlyResolveCed);
                    }

                    elmt.IsRendered = false;
                    elmt.IsChecked = false;
                }
                return menuItems;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetMenuItems Error: " + ex.Message);
            }
        }

        //##RenderHorizonalMenuAreaAndItems
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
                //sv++
                menuAreaControl.BackColor = Color.Transparent;
                var regionControl = new Control();
                try
                {
                    regionControl = this.Controls.Find(menuFeature.Container, true)[0];
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("menu Container doesn't exist! ContainerRegionName={0}, menuFeatureature.Id={1}, menuFeatureature.Name={2}"
                        .FormatWith(menuFeature.Container, menuFeature.Id, menuFeature.Name));
                }
                menuAreaControl.Dock = DockStyle.Fill;
                menuAreaControl.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
                //menuAreaControl.ImageScalingSize = new Size(26, 26);
                menuAreaControl.AutoSize = true;

                menuAreaControl.Name = menuFeature.Container + "Menu" + menuFeature.Id + "Area";
                menuAreaControl.Tag = menuAreaControl.Name;
                regionControl.Controls.Add(menuAreaControl);

                var horizontalMenuArea = new LayoutElement();
                if (menuFeature.MenuType == (int)MenuType.Horizontal)
                {
                    horizontalMenuArea.Container = menuFeature.Container;
                    horizontalMenuArea.TypeName = LayoutElementType.MenuArea.ToString();
                    horizontalMenuArea.ControlTypeName = MenuControlType.MenuStrip.ToString();
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
                var procs = _procedures.FindAll(x => x.ShellId.IsNullOrEmpty());
                var isItemVisible = !menuItem.Invisible.JudgeJudgementFlag();
                var isItemEnabled = !menuItem.Disabled.JudgeJudgementFlag();
                var imageUrl = menuItem.ImageUrl;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    imageUrl = GetText(imageUrl, GetTextType.OnlyResolveCed);
                }


                if (menuItem.ControlTypeName == MenuControlType.ToolStripMenuItem.ToString() | menuItem.ControlTypeName == MenuControlType.ImageToolStripMenuItemH.ToString() | menuItem.ControlTypeName == MenuControlType.ImageToolStripMenuItemV.ToString())
                {
                    var menuItemControl = new ToolStripMenuItem();

                    //--image
                    if (menuItem.ControlTypeName.Contains("Image") & !string.IsNullOrEmpty(menuItem.ImageUrl))
                    {
                        menuItemControl.Image = ControlBaseHelper.GetImage(imageUrl);
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

                    //--name
                    menuItemControl.Name = menuItem.Name;
                    menuItemControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                    //--displayname, remark
                    menuItemControl.AutoToolTip = false;

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
                        parentAreaCtrl.Items.Add(menuItemControl);
                    }
                    else
                    {
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
                verMenuArea.TypeName = LayoutElementType.MenuArea.ToString();
                verMenuArea.ControlTypeName = MenuControlType.Panel.ToString();
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
                    var imgIndexArry = menuItem.ImageUrl.GetLarrayArray(true, true);
                    if (imgIndexArry.Length > 0)
                    {
                        var imageIndex = imgIndexArry[0].IsPlusInteger() ? Convert.ToInt16(imgIndexArry[0]) : 0;
                        treeItem.ImageIndex = imageIndex;
                    }
                    if (imgIndexArry.Length > 1)
                    {
                        var imageIndex = imgIndexArry[1].IsPlusInteger() ? Convert.ToInt16(imgIndexArry[1]) : 0;
                        treeItem.SelectedImageIndex = imageIndex;
                    }
                    treeDataList.Add(treeItem);
                }
                var menuControl = new TreeViewEx();
                var styleText = StyleSheet.TreeViewExClass_Menu;
                var imgUrls = "";
                if (!menuFeature.ImageUrls.IsNullOrEmpty())
                {
                    imgUrls = menuFeature.ImageUrls;
                }

                styleText = styleText.AddToLdict("ImageUrls", imgUrls, true, true);
                menuControl.StyleText = styleText;
                if (_formInitParamSet.SupportMultiLanguages) menuControl.AnnexList = _annexes.FindAll(x => x.ClassName == "MenuItem");
                menuControl.DataSourceObject = treeDataList;
                menuControl.Dock = DockStyle.Fill;
                menuControl.LeafClick += new System.EventHandler(MenuItemClickHandler);
                menuControl.Name = areaName + "-VerticalMenu";

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

                var subMenuArea = _layoutElements.Find(x => x.ParentId == menuItemId && x.Type == (int)LayoutElementType.MenuArea
                & x.LayoutId == menuFeatureId & x.LayoutType == (int)LayoutType.Menu);

                if (subMenuArea == null) //isLastLevel ,-is menuItem not subMenuArea,
                {
                    var menuItem = _layoutElements.Find(x => x.Id == menuItemId && x.Type == (int)LayoutElementType.MenuItem
                    & x.LayoutId == menuFeatureId & x.LayoutType == (int)LayoutType.Menu);
                    if (menuItem == null) return;
                    if (menuItem.View.IsNullOrEmpty()) return;
                    var viewFeature = _viewFeatures.Find(x => x.Name == menuItem.View);
                    if (viewFeature == null) return;
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
                var toolBarMenuArea = _layoutElements.Find(x => x.Type == (int)LayoutElementType.MenuArea && x.ParentId == 0 && x.LayoutId == menuFeatureId);
                if (toolBarMenuArea == null) return;
                var toolMenuItems = _layoutElements.FindAll(x => x.Container == toolBarMenuArea.Name &&
                (x.Type == (int)LayoutElementType.MenuItem)
                && x.LayoutId == menuFeatureId);

                if (toolMenuItems.Count == 0) return;
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
                if ((menuContainerArea.Type == (int)LayoutElementType.MenuArea)
                    && menuContainerArea.ControlTypeName == MenuControlType.ToolStrip.ToString())
                {
                    var menuContainerAreaControl = new ToolStrip();
                    //sv++
                    menuContainerAreaControl.BackColor = Color.Transparent;
                    menuContainerAreaControl.GripStyle = ToolStripGripStyle.Hidden;
                    menuContainerAreaControl.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
                    menuContainerAreaControl.Paint += new System.Windows.Forms.PaintEventHandler(toolStrip_Paint);

                    menuContainerAreaControl.Tag = "$";

                    ControlBaseHelper.SetControlBackColor(menuContainerAreaControl, menuContainerArea.StyleText);

                    var regionControl = new Control();
                    try
                    {
                        regionControl = this.Controls.Find(menuContainerArea.Container, true)[0];
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("viewMenuArea.Container doesn't exist! ctrlName=" + menuContainerArea.Container);
                    }

                    var menuContainerAreaWidth = menuContainerArea.Width < 0 ? regionControl.Width : menuContainerArea.Width;
                    var menuContainerAreaHeight = menuContainerArea.Height < 0 ? regionControl.Height : menuContainerArea.Height;
                    ControlBaseHelper.SetControlDockStyleAndLocationAndSize(menuContainerAreaControl, menuContainerArea.DockType, menuContainerAreaWidth, menuContainerAreaHeight, menuContainerArea.OffsetOrPositionX, menuContainerArea.OffsetOrPositionY);
                    menuContainerAreaControl.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
                    var dict = menuContainerArea.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var imageWidthStr = dict.GetLdictValue("ImageWidth");
                    var imageHeightStr = dict.GetLdictValue("ImageHeight");
                    var imageWidth = imageWidthStr.IsNullOrEmpty() ? 0 : Convert.ToInt16(imageWidthStr);
                    var imageHeight = imageWidthStr.IsNullOrEmpty() ? 0 : Convert.ToInt16(imageHeightStr);
                    menuContainerAreaControl.ImageScalingSize = new Size(imageWidth > 0 ? imageWidth : 0, imageHeight > 0 ? imageHeight : 0);
                    menuContainerAreaControl.AutoSize = false;
                    menuContainerAreaControl.Name = menuContainerArea.Name;
                    menuContainerAreaControl.Tag = menuContainerArea.ParentId == 0 ? "$" + menuContainerArea.Name : menuContainerArea.Name;
                    regionControl.Controls.Add(menuContainerAreaControl);
                    menuContainerArea.IsChecked = true;
                    menuContainerArea.IsRendered = true;

                    var menuItems = _layoutElements.FindAll(x => x.Container == menuContainerArea.Name &&
                    x.Type == (int)LayoutElementType.MenuItem & x.LayoutId == menuContainerArea.LayoutId).ToList();

                    //menuItems.Reverse();
                    menuItems = menuItems.OrderByDescending(x => x.DockOrder).ToList();

                    var defMenuIdStr = menuContainerArea.DefaultSubItemId;
                    if (string.IsNullOrEmpty(defMenuIdStr) && menuItems.Count > 0)
                    {
                        defMenuIdStr = menuItems.FirstOrDefault().Id.ToString();
                    }
                    foreach (var menuItem in menuItems)
                    {
                        itemNameForEx = menuItem.Name;

                        //--Visible, Enabled
                        bool isItemVisible = true;
                        var itemInvisible = !menuItem.Invisible.JudgeJudgementFlag();
                        var isItemEnabled = !menuItem.Disabled.JudgeJudgementFlag();

                        var imageUrl = menuItem.ImageUrl;
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = GetText(imageUrl, GetTextType.OnlyResolveCed);
                        }

                        if (menuItem.ControlTypeName == MenuControlType.ToolSplitButton.ToString() | menuItem.ControlTypeName == MenuControlType.ImageToolSplitButton.ToString() |
                            menuItem.ControlTypeName == MenuControlType.ImageTextToolSplitButtonH.ToString() | menuItem.ControlTypeName == MenuControlType.ImageTextToolSplitButtonV.ToString())
                        {
                            //submenuitem
                            var subMenuDir = menuItem.Location + "\\" + menuItem.DataSource;
                            var subMenuItems = GetSubMenuItems(menuItem.Location, menuItem.DataSource);
                            if (subMenuItems == null) break;
                            if (subMenuItems.Count == 0) break;

                            foreach (var subMenuItem in subMenuItems)
                            {
                                subMenuItem.ImageUrl = ResolveConstants(subMenuItem.ImageUrl);
                                subMenuItem.ImageUrl = GetImageUrl(subMenuItem.ImageUrl, subMenuDir);
                            }


                            var subMenuAnnexesCfgFile = subMenuDir + "\\Annexes";
                            var subMenuItemsAnnexes = CommonHelper.GetAnnexesFromCfgFile(subMenuAnnexesCfgFile, "", false);

                            var menuItemControl = new ToolStripSplitButtonEx(subMenuItems, subMenuItemsAnnexes);

                            if (menuItem.ControlTypeName == MenuControlType.ImageToolSplitButton.ToString())
                            {
                                menuItemControl.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            }
                            //--image
                            if (menuItem.ControlTypeName.Contains("Image") & !string.IsNullOrEmpty(menuItem.ImageUrl))
                            {
                                menuItemControl.Image = ControlBaseHelper.GetImage(imageUrl);
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
                            if (menuItem.Width > 0)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Width = menuItem.Width;
                            }
                            if (menuItem.Height > 0)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Height = menuItem.Height;
                            }
                            if (menuItem.OffsetOrPositionX > 0)
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
                            menuItemControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            menuItemControl.AutoToolTip = false;
                            if (menuItem.ControlTypeName == MenuControlType.ImageToolSplitButton.ToString())
                            {
                                menuItemControl.AutoToolTip = true;
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
                        else if (menuItem.ControlTypeName == MenuControlType.ToolButton.ToString() | menuItem.ControlTypeName == MenuControlType.ImageToolButton.ToString()
                            | menuItem.ControlTypeName == MenuControlType.ImageTextToolButtonH.ToString() | menuItem.ControlTypeName == MenuControlType.ImageTextToolButtonV.ToString())
                        {
                            var menuItemControl = new ToolStripButton();
                            if (menuItem.ControlTypeName.Contains("ImageToolButton"))
                            {
                                menuItemControl.DisplayStyle = ToolStripItemDisplayStyle.Image;
                            }
                            //--image
                            if (menuItem.ControlTypeName.Contains("Image") & !string.IsNullOrEmpty(menuItem.ImageUrl))
                            {
                                menuItemControl.Image = ControlBaseHelper.GetImage(imageUrl);
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
                            if (menuItem.Width > 0)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Width = menuItem.Width;
                            }
                            if (menuItem.Height > 0)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Height = menuItem.Height;
                            }
                            if (menuItem.OffsetOrPositionX > 0)
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
                            menuItemControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            //--displayname, remark
                            menuItemControl.AutoToolTip = false;
                            if (menuItem.ControlTypeName == MenuControlType.ImageToolButton.ToString())
                            {
                                menuItemControl.AutoToolTip = true;

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
                        else if (menuItem.ControlTypeName == MenuControlType.ToolLabel.ToString())
                        {
                            var menuItemControl = new ToolStripLabel();
                            //--dock, size,offset
                            if (menuItem.DockType == (int)ControlDockType.Right)
                            {
                                menuItemControl.Alignment = ToolStripItemAlignment.Right;
                            }
                            if (menuItem.Width > 0)
                            {
                                menuItemControl.AutoSize = false;
                                menuItemControl.Width = menuItem.Width;
                            }
                            if (menuItem.Height > 0)
                            {
                                menuItemControl.Height = menuItem.Height;
                            }
                            if (menuItem.OffsetOrPositionX > 0)
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
                            menuItemControl.Visible = !menuItem.Invisible.JudgeJudgementFlag();
                            menuItemControl.Enabled = !menuItem.Disabled.JudgeJudgementFlag();
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
                else if ((menuContainerArea.Type == (int)LayoutElementType.MenuArea)
                         && menuContainerArea.ControlTypeName == MenuControlType.Panel.ToString())
                {

                    //--area
                    var regionControl = this.GetControl(menuContainerArea.Container);
                    var areaWidth = menuContainerArea.Width > 0 ? regionControl.Width : menuContainerArea.Width;
                    var areaHeight = menuContainerArea.Height > 0 ? regionControl.Height : menuContainerArea.Height;
                    var areaControl = new ContainerPanel();
                    areaControl.AutoScroll = true;
                    //ControlHelper.SetContainerPanelStyleByClass(areaControl, menuContainerArea.StyleClass);
                    if (!menuContainerArea.StyleText.IsNullOrEmpty()) ControlHelper.SetPanelStyleByText(areaControl, menuContainerArea.StyleText);
                    ControlBaseHelper.SetControlDockStyleAndLocationAndSize(areaControl, menuContainerArea.DockType, areaWidth, areaHeight, menuContainerArea.OffsetOrPositionX, menuContainerArea.OffsetOrPositionY);

                    areaControl.Name = menuContainerArea.Name;
                    regionControl.Controls.Add(areaControl);
                    menuContainerArea.IsChecked = true;
                    menuContainerArea.IsRendered = true;

                    var defMenuIdStr = menuContainerArea.DefaultSubItemId;

                    var menuItems = _layoutElements.Where(x => x.Container == menuContainerArea.Name
                        && (x.Type == (int)LayoutElementType.MenuItem)
                        ).ToList();
                    menuItems.Reverse();
                    menuItems = menuItems.OrderBy(x => x.DockOrder).ToList();
                    if (string.IsNullOrEmpty(defMenuIdStr) && menuItems.Count > 0)
                    {
                        defMenuIdStr = menuItems.FirstOrDefault().Id.ToString();
                    }
                    //--items
                    foreach (var menuItem in menuItems)
                    {
                        itemNameForEx = menuItem.Name;
                        //Visible, Enabled
                        var isItemVisible = !menuItem.Invisible.JudgeJudgementFlag();
                        var isItemEnabled = !menuItem.Disabled.JudgeJudgementFlag();
                        var imageUrl = menuItem.ImageUrl;
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = GetText(imageUrl, GetTextType.OnlyResolveCed);
                        }

                        if (menuItem.ControlTypeName == MenuControlType.ImageTextButtonH.ToString() | menuItem.ControlTypeName == MenuControlType.ImageTextButtonV.ToString())
                        {
                            var itemControl = new ImageTextButton();
                            itemControl.Name = menuItem.Name;
                            itemControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            //--location, size
                            ControlBaseHelper.SetControlDockStyleAndLocationAndSize(itemControl, menuItem.DockType, menuItem.Width, menuItem.Height, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);

                            //--image
                            //var imgUrl = GetImageUrl(menuItem.ImageUrl, menuItem.Location);
                            itemControl.Image = ControlBaseHelper.GetImage(imageUrl);
                            if (menuItem.ControlTypeName.EndsWith("V"))
                            {
                                itemControl.TextImageRelation = TextImageRelation.ImageAboveText;
                                ControlHelper.SetImageTextButtonStyleByClass(itemControl, "MenuItem");

                            }
                            else if (menuItem.ControlTypeName.EndsWith("H"))
                            {
                                itemControl.TextImageRelation = TextImageRelation.ImageBeforeText;
                                itemControl.TextAlign = ContentAlignment.MiddleLeft;
                                ControlHelper.SetImageTextButtonStyleByClass(itemControl, "MenuItem");
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
                            if (menuItem.DockType > 0 && menuItem.DockType < 5)
                            {
                                var dockStyle = menuItem.DockType;
                                if (
                                    ((dockStyle == (int)ControlDockType.Top | dockStyle == (int)ControlDockType.Bottom) & menuItem.OffsetOrPositionY > 0) |
                                    ((dockStyle == (int)ControlDockType.Left | dockStyle == (int)ControlDockType.Right) & menuItem.OffsetOrPositionX > 0)
                                    )
                                {
                                    var offsetCrtl = new Label();
                                    ControlBaseHelper.SetControlOffsetByDockStyle(offsetCrtl, menuItem.DockType, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                                    areaControl.Controls.Add(offsetCrtl);
                                }
                            }
                            else
                            {
                                itemControl.Location = new Point(menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                            }

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
                                //itemControl.CheckType =ControlCheckType.None;
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
                        else if (menuItem.ControlTypeName == MenuControlType.TextButton.ToString())
                        {
                            var itemControl = new TextButton();
                            itemControl.Name = menuItem.Name;
                            itemControl.Text = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", menuItem.Name, _annexes, menuItem.DisplayName);
                            ControlHelper.SetTextButtonStyleByClass(itemControl, "MenuItem");
                            ControlHelper.SetTextButtonStyleByText(itemControl, menuItem.StyleText);
                            //--location, size
                            ControlBaseHelper.SetControlDockStyleAndLocationAndSize(itemControl, menuItem.DockType, menuItem.Width, menuItem.Height, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);

                            areaControl.Controls.Add(itemControl);
                            if (menuItem.DockType > 0 && menuItem.DockType < 5)
                            {
                                var dockStyle = menuItem.DockType;
                                if (
                                    ((dockStyle == (int)ControlDockType.Top | dockStyle == (int)ControlDockType.Bottom) & menuItem.OffsetOrPositionY > 0) |
                                    ((dockStyle == (int)ControlDockType.Left | dockStyle == (int)ControlDockType.Right) & menuItem.OffsetOrPositionX > 0)
                                    )
                                {
                                    var offsetCrtl = new Label();
                                    ControlBaseHelper.SetControlOffsetByDockStyle(offsetCrtl, menuItem.DockType, menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                                    areaControl.Controls.Add(offsetCrtl);
                                }
                            }
                            else
                            {
                                itemControl.Location = new Point(menuItem.OffsetOrPositionX, menuItem.OffsetOrPositionY);
                            }

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

                                //itemControl.CheckType = ControlCheckType.None;
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
                    viewMenuArea.DefaultSubItemId = menuItem.Id.ToString();
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
                    viewMenuArea.DefaultSubItemId = menuItem.Id.ToString();
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
            var menuArea = _layoutElements.Find(x => x.Type == (int)LayoutElementType.MenuArea && x.ParentId == menuId);
            if (menuArea != null)
            {
                var areaControl = GetControl(menuArea.Name);
                ControlBaseHelper.HideControlByDockStyle(areaControl, menuArea.DockType);
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
                var viewMenuAreaControl = ControlBaseHelper.GetControlByNameByParent(viewMenuArea.Name, containerRegionControl);
                var viewMenuAreaWidth = viewMenuArea.Width > 0 ? containerRegionControl.Width : viewMenuArea.Width;
                var viewMenuAreaHeight = viewMenuArea.Height > 0 ? containerRegionControl.Height : viewMenuArea.Height;
                ControlBaseHelper.SetControlDockStyleAndLocationAndSize(viewMenuAreaControl, viewMenuArea.DockType, viewMenuAreaWidth, viewMenuAreaHeight, viewMenuArea.OffsetOrPositionX, viewMenuArea.OffsetOrPositionY);
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
                var menuArea = _layoutElements.Find(x => x.Name == menuItem.Container && x.Type == (int)LayoutElementType.MenuArea);
                var menuAreaControl = GetControl(menuArea.Name);
                //uncheck
                var lastCheckedParallelMenuItem = _layoutElements.Find(x => x.Container == menuArea.Name && x.Type == (int)LayoutElementType.MenuItem && x.IsChecked);
                if (lastCheckedParallelMenuItem != null)
                {
                    if (lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.ToolButton.ToString() | lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.ImageToolButton.ToString()
                        | lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.ImageTextToolButtonH.ToString() | lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.ImageTextToolButtonV.ToString())
                    {
                        var menuAreaCpnt = menuAreaControl as ToolStrip;
                        var cpnt = menuAreaCpnt.Items.Find(lastCheckedParallelMenuItem.Name, true)[0] as ToolStripButton;
                        cpnt.Checked = false;
                    }
                    else if (lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.ImageTextButtonH.ToString() | lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.ImageTextButtonV.ToString())
                    {
                        var menuItemControl = ControlBaseHelper.GetControlByNameByParent(lastCheckedParallelMenuItem.Name, menuAreaControl);
                        var cpnt = menuItemControl as ImageTextButton;
                        cpnt.Checked = false;
                    }
                    else if (lastCheckedParallelMenuItem.ControlTypeName == MenuControlType.TextButton.ToString())
                    {
                        var menuItemControl = ControlBaseHelper.GetControlByNameByParent(lastCheckedParallelMenuItem.Name, menuAreaControl);
                        var cpnt = menuItemControl as TextButton;
                        cpnt.Checked = false;
                    }
                    lastCheckedParallelMenuItem.IsChecked = false;
                }

                //--check
                if (menuItem.ControlTypeName == MenuControlType.ToolButton.ToString() | menuItem.ControlTypeName == MenuControlType.ImageToolButton.ToString() |
                    menuItem.ControlTypeName == MenuControlType.ImageTextToolButtonH.ToString() | menuItem.ControlTypeName == MenuControlType.ImageTextToolButtonV.ToString())
                {
                    var menuAreaCpnt = menuAreaControl as ToolStrip;
                    var cpnt = menuAreaCpnt.Items.Find(menuItem.Name, true)[0] as ToolStripButton;
                    cpnt.Checked = true;
                }
                else if (menuItem.ControlTypeName == MenuControlType.ImageTextButtonH.ToString() | menuItem.ControlTypeName == MenuControlType.ImageTextButtonV.ToString())
                {
                    var menuItemControl = ControlBaseHelper.GetControlByNameByParent(menuItem.Name, menuAreaControl);
                    var cpnt = menuItemControl as ImageTextButton;
                    cpnt.Checked = true;
                }
                else if (menuItem.ControlTypeName == MenuControlType.TextButton.ToString())
                {
                    var menuItemControl = ControlBaseHelper.GetControlByNameByParent(menuItem.Name, menuAreaControl);
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

        private List<SubMenuItem> GetSubMenuItems(string location, string dataSrc)
        {
            try
            {
                var subMenuItems = new List<LayoutElement>();
                if (!location.IsNullOrEmpty())
                {
                    var subMenuDir = location + "\\" + dataSrc;
                    var subMenuUiCfgFile = subMenuDir + "\\Ui";
                    subMenuItems = DataParserHelper.ConvertToGeneric<List<LayoutElement>>(subMenuUiCfgFile, false, TxtDataType.Undefined) ?? new List<LayoutElement>();
                }
                else
                {

                }
                if (subMenuItems == null) return null;
                if (subMenuItems.Count == 0) return null;

                foreach (var subMenuItem in subMenuItems)
                {
                    //check
                    CommonHelper.CheckName(subMenuItem.Name);
                    if (subMenuItem.Id < 1)
                    {
                        throw new ArgumentException("subMenuItem Id can't be less than 1! subMenuItem.Id=" + subMenuItem.Id + ", subMenuItem.Name=" + subMenuItem.Name);
                    }
                    if (subMenuItem.ParentId < 0)
                    {
                        throw new ArgumentException("subMenuItem ParentId can't be less than 0! subMenuItem.Id=" + subMenuItem.Id + ", subMenuItem.Name=" + subMenuItem.Name);
                    }

                    if (subMenuItems.FindAll(x => x.Id == subMenuItem.Id).Count > 1)
                    {
                        throw new ArgumentException("subMenuItem can't have duplicated Id! subMenuItem.Id=" + subMenuItem.Id + ", subMenuItem.Name=" + subMenuItem.Name);
                    }

                    if (subMenuItems.FindAll(x => x.Name == subMenuItem.Name).Count > 1)
                    {
                        throw new ArgumentException("menuItem can't have duplicated name! menuItem.Id=" + subMenuItem.Id + ", subMenuItem.Name=" + subMenuItem.Name);
                    }

                    //set
                    subMenuItem.Invalid = ResolveConstants(subMenuItem.Invalid);
                    if (subMenuItem.Invalid.StartsWith("=")) subMenuItem.Invalid = GetText(subMenuItem.Invalid, GetTextType.OnlyResolveCed);
                    subMenuItem.Invalid = subMenuItem.Invalid.GetJudgementFlag();
                    subMenuItem.Invisible = ResolveConstants(subMenuItem.Invisible);
                    if (subMenuItem.Invisible.StartsWith("=")) subMenuItem.Invisible = GetText(subMenuItem.Invisible, GetTextType.OnlyResolveCed);
                    subMenuItem.Invisible = subMenuItem.Invisible.GetJudgementFlag();
                    subMenuItem.Disabled = ResolveConstants(subMenuItem.Disabled);
                    if (subMenuItem.Disabled.StartsWith("=")) subMenuItem.Disabled = GetText(subMenuItem.Disabled, GetTextType.OnlyResolveCed);
                    subMenuItem.Disabled = subMenuItem.Disabled.GetJudgementFlag();
                    subMenuItem.ShowRunningStatus = subMenuItem.ShowRunningStatus.GetJudgementFlag();


                }

                var subMenuItems1 = subMenuItems.FindAll(x => x.Invalid.ToLower() == "false");
                var subMenuItems2 = new List<SubMenuItem>();
                foreach (var subMenuItem in subMenuItems1)
                {
                    var subMenuItem2 = new SubMenuItem();

                    subMenuItem2.Id = subMenuItem.Id.ToString();
                    subMenuItem2.ParentId = subMenuItem.ParentId.ToString();
                    subMenuItem2.Name = subMenuItem.Name ?? "";
                    subMenuItem2.DisplayName = subMenuItem.DisplayName ?? "";
                    subMenuItem2.Action = subMenuItem.Action ?? "";
                    subMenuItem2.ControlTypeName = subMenuItem.ControlTypeName ?? "";
                    subMenuItem2.ImageUrl = subMenuItem.ImageUrl.IsNullOrEmpty() ? "" : ResolveConstants(subMenuItem.ImageUrl);
                    subMenuItems2.Add(subMenuItem2);

                }
                return subMenuItems2;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".SetSubMenuItems Error: " + ex.Message);
            }
        }


        //*view
        //##MergeViewItems
        private void MergeViewItems(string viewName)
        {
            var viewNameEx = "";
            var viewCfgFileEx = "";
            try
            {
                viewNameEx = viewName;
                var viewCfgFile = "";
                viewCfgFileEx = viewCfgFile;
                if (_formInitParamSet.FormType == FormType.Mvi)
                {
                    var viewFeature = _viewFeatures.Find(x => x.Name == viewName);//Rd?
                    var viewLoc = viewFeature.Location;
                    viewNameEx = viewName;
                    if (!viewLoc.IsNullOrEmpty())
                    {
                        viewCfgFile = viewLoc + "\\Ui";
                    }
                }
                else //sviView
                {
                    viewCfgFile = FileHelper.GetPath(_formInitParamSet.FormRelativeLocation + "\\Ui", _appUiDir + "\\Views", true);
                }
                //var tmpViewItems = CommonHelper.GetGenericFromCfgFile<List<LayoutElement>>(viewCfgFile, true) ?? new List<LayoutElement>();
                var tmpViewItems = DataParserHelper.ConvertToGeneric<List<LayoutElement>>(viewCfgFile, true, TxtDataType.Undefined) ?? new List<LayoutElement>();

                foreach (var viewItem in tmpViewItems)
                {
                    viewItem.Invalid = ResolveConstants(viewItem.Invalid);
                    if (viewItem.Invalid.StartsWith("=")) viewItem.Invalid = GetText(viewItem.Invalid, GetTextType.OnlyResolveCed);
                    viewItem.Invalid = viewItem.Invalid.GetJudgementFlag();
                    viewItem.DataSource = string.IsNullOrEmpty(viewItem.DataSource) ? "" : viewItem.DataSource;
                    viewItem.DataSource = ResolveConstants(viewItem.DataSource);
                    viewItem.DisplayName = string.IsNullOrEmpty(viewItem.DisplayName) ? "" : viewItem.DisplayName;
                    viewItem.DisplayName = ResolveConstants(viewItem.DisplayName);
                    viewItem.Location = string.IsNullOrEmpty(viewItem.Location) ? "" : viewItem.Location;//zone loc
                    viewItem.Location = ResolveConstants(viewItem.Location);
                    viewItem.Action = string.IsNullOrEmpty(viewItem.Action) ? "" : viewItem.Action;
                    viewItem.Action = ResolveConstants(viewItem.Action);
                    viewItem.Trigger = string.IsNullOrEmpty(viewItem.Trigger) ? "" : viewItem.Trigger;
                    viewItem.ShowRunningStatus = viewItem.ShowRunningStatus.GetJudgementFlag();
                }

                var viewItems = tmpViewItems.Where(x =>
                    (x.Invalid != "true") &
                    (x.TypeName == LayoutElementType.ContentArea.ToString()
                    | x.TypeName == LayoutElementType.Zone.ToString()
                    | x.TypeName == LayoutElementType.ActionWatcher.ToString()
                    | x.TypeName == LayoutElementType.CedWatcher.ToString()
                    )
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
                        if (viewItem.Type == (int)LayoutElementType.ActionWatcher)
                        {
                            viewItem.Trigger = viewName + "_" + viewItem.Trigger.DeleteUiIdentifer();
                            viewItem.Action = AddViewIdentiferForViewUiElement(viewItem.Action, viewName, true);
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
                        var ctrl = ControlBaseHelper.GetControlByNameByParentForRecursionSubCall(item.Name, regionControl);
                        regionControl.Controls.SetChildIndex(ctrl, 0);
                    }
                    var regionCttAreas1 = regionCttAreas.FindAll(x => x.DockType == (int)ControlDockType.Right | x.DockType == (int)ControlDockType.Bottom);
                    regionCttAreas1.Reverse();
                    foreach (var item in regionCttAreas1)
                    {
                        var ctrl = ControlBaseHelper.GetControlByNameByParentForRecursionSubCall(item.Name, regionControl);
                        regionControl.Controls.SetChildIndex(ctrl, 0);
                    }
                }
                //ViewEventHandler(viewName, LayoutElementType.AfterViewRenderHandler);
            }
            catch (Exception ex)
            {
                var viewFT = _viewFeatures.Find(x => x.Name == viewName);
                var viewLoc = viewFT != null ? "View Config File: " + viewFT.Location + "; " : "";
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderView Error: ViewName= " + viewName + "; " + viewLoc + ex.Message);
            }
        }

        //##SwitchView
        private void SwitchView(string viewName)
        {
            try
            {
                var viewFeature = _viewFeatures.Find(x => x.Name == viewName);
                if (viewFeature == null) return;
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

                var areaControl = ControlBaseHelper.GetControlByNameByParent(area.Name, areaContainerControl);
                var areaWidth = area.Width > 0 ? areaContainerControl.Width : area.Width;
                var areaHeight = area.Height > 0 ? areaContainerControl.Height : area.Height;
                ControlBaseHelper.SetControlDockStyleAndLocationAndSize(areaControl, area.DockType, areaWidth, areaHeight, area.OffsetOrPositionX, area.OffsetOrPositionY);
            }

        }

        //##HideView
        private void HideView(string viewName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".HideLastCheckedView Error: ";
            var tmpParallelAreas = _layoutElements.FindAll(x => !x.View.IsNullOrEmpty());//21
            var parallelAreas = tmpParallelAreas.FindAll(x => x.View == viewName & x.Type == (int)LayoutElementType.ContentArea); //ok
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

                var parallelAreaControl = ControlBaseHelper.GetControlByNameByParent(parallelArea.Name, parallelAreaContainerControl);
                ControlBaseHelper.HideControlByDockStyle(parallelAreaControl, parallelArea.DockType);
            }
        }


        //##IsViewRendered
        private bool IsViewRendered(string viewName)
        {
            if (_renderedViewNames.Count == 0) return false;
            return _renderedViewNames.Contains(viewName);
        }


        //*area
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
                    var cttAreaWidth = cttArea.Width > 0 ? regionControl.Width : cttArea.Width;
                    var cttAreaHeight = cttArea.Height > 0 ? regionControl.Height : cttArea.Height;
                    var cttAreaControl = new ContainerPanel();
                    cttAreaControl.AutoScroll = true;
                    ControlHelper.SetContainerPanelStyleByClass(cttAreaControl, cttArea.StyleClass);
                    ControlHelper.SetContainerPanelStyleByText(cttAreaControl, cttArea.StyleText);
                    ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cttAreaControl, cttArea.DockType, cttAreaWidth, cttAreaHeight, cttArea.OffsetOrPositionX, cttArea.OffsetOrPositionY);

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
                    foreach (var item in zones.Where(x => true).OrderBy(x => x.DockOrder))
                    {
                        itemNameForEx = item.Name;
                        var ctrl = ControlBaseHelper.GetControlByNameByParentForRecursionSubCall(item.Name, cttAreaControl);
                        cttAreaControl.Controls.SetChildIndex(ctrl, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderContentAreaAndItems Error: areaName=" + areaNameForEx + "; zoneName=" + itemNameForEx + "; " + ex.Message);
            }
        }


        //*zone
        //##InitZone
        private void InitZone(LayoutElement zone, ContainerPanel cttAreaControl)
        {
            var zoneNameForEx = zone.Name;
            try
            {
                var zoneProcedures = GetZoneShell(zone);
                _procedures.AddRange(zoneProcedures);
                InitZoneShell(zoneProcedures);

                MergeZoneItems(zone);
                ZoneEventHandler(zone.Name, ZoneItemType.BeforeZoneRenderHandler);

                var location = zone.Location;

                var zoneFeature = DataParserHelper.ConvertToGeneric<ZoneFeature>(location + "\\Feature", true, TxtDataType.Undefined) ?? new ZoneFeature();
                if (zone.Width < 0) zone.Width = zoneFeature.Width;
                if (zone.Height < 0) zone.Height = zoneFeature.Height;

                if (zoneFeature.CpntArrangementType.IsNullOrEmpty()) zoneFeature.CpntArrangementType = ZoneCpntArrangementType.Positioning.ToString();
                var zoneCpntArrangementTypeInt = EnumHelper.GetIdByName<ZoneCpntArrangementType>(zoneFeature.CpntArrangementType);
                zone.ZoneCpntArrangementType = zoneCpntArrangementTypeInt;

                if (zone.StyleText.IsNullOrEmpty()) zone.StyleText = zoneFeature.StyleText;

                var zoneContainer = new ContainerPanel();
                zoneContainer.AutoScroll = true;
                zoneContainer.Name = zone.Name;
                ControlHelper.SetContainerPanelStyleByText(zoneContainer, zone.StyleText);
                var zoneWidth = zone.Width < 0 ? cttAreaControl.Width : zone.Width;
                var zoneHeight = zone.Height < 0 ? cttAreaControl.Height : zone.Height;
                {
                    ControlBaseHelper.SetControlDockStyleAndLocationAndSize(zoneContainer, zone.DockType, zoneWidth, zoneHeight, zone.OffsetOrPositionX, zone.OffsetOrPositionY);
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

        //##GetZoneShell
        private List<Procedure> GetZoneShell(LayoutElement zone)
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetZoneShell Error: ";

            var uiShellProcedures = new List<Procedure>();

            var args = zone.ShellArgs;
            if (!args.IsNullOrEmpty())
            {
                var argsArry = args.GetLarrayArray(true, false);
                for (var i = 0; i < argsArry.Length; i++)
                {
                    var tmp = argsArry[i];
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = ResolveConstants(tmp);
                        tmp = tmp.StartsWith("=") ? GetText(tmp, GetTextType.OnlyResolveCed) : tmp;
                    }
                    argsArry[i] = tmp;
                }

                var argsStr = TextDataHelper.UnwrapLarray(argsArry);
                var variableItem = new Procedure();
                variableItem.Name = zone.Name + "_" + "args";
                variableItem.Value = argsStr;
                variableItem.Type = (int)ProcedureType.Args;
                variableItem.TypeName = ProcedureType.Args.ToString();
                variableItem.DisplayName = "";
                variableItem.Expression = "";
                variableItem.StartValue = "";
                variableItem.SkipOnInit = "true";
                variableItem.ShowRunningStatus = "";
                uiShellProcedures.Add(variableItem);
            }

            var uiShellProceduresTmp = new List<Procedure>();
            var location = FileHelper.GetPath(zone.Location, _zonesDir);
            var cfgFile = location + "\\Shell";
            try
            {
                uiShellProceduresTmp = DataParserHelper.ConvertToGeneric<List<Procedure>>(cfgFile, false, TxtDataType.Undefined) ?? new List<Procedure>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            var uiShellProceduresTmp1 = new List<Procedure>();
            foreach (var proc in uiShellProceduresTmp)
            {
                proc.Invalid = proc.Invalid.GetJudgementFlag();
                if (proc.Invalid.ToLower() == "true")
                {
                    continue;
                }
                else
                {
                    uiShellProceduresTmp1.Add(proc);
                }
            }
            uiShellProceduresTmp1 = uiShellProceduresTmp1.FindAll(x => x.TypeName == ProcedureType.Variable.ToString()
                                              | x.TypeName == ProcedureType.Transaction.ToString() | x.TypeName == ProcedureType.SubTransaction.ToString());

            if (uiShellProceduresTmp1.Count == 0) return uiShellProcedures;

            var annexList = new List<Annex>();
            try
            {
                ShellHelper.CheckProcedures(uiShellProceduresTmp1, zone.Name);
                annexList = CommonHelper.GetAnnexesFromCfgFile(cfgFile + "Annexes", "Procedure", false);

                foreach (var proc in uiShellProceduresTmp1)
                {
                    ShellHelper.SetProcedureType(proc);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }
            foreach (var proc in uiShellProceduresTmp1)
            {
                proc.SkipOnInit = proc.SkipOnInit.GetJudgementFlag();
                proc.ShowRunningStatus = proc.ShowRunningStatus.GetJudgementFlag();
                proc.StartValue = string.IsNullOrEmpty(proc.StartValue) ? "" : proc.StartValue;
                proc.Value = proc.StartValue;
                proc.Expression = string.IsNullOrEmpty(proc.Expression) ? "" : proc.Expression;
                proc.Expression = ResolveConstants(proc.Expression);


                if (proc.GroupId < 0) proc.GroupId = 0;
                uiShellProcedures.Add(proc);


                var tempAnnexes = annexList.FindAll(x => x.MasterName == proc.Name);
                proc.Name = zone.Name + "_" + proc.Name;
                proc.ShellId = zone.Name;
                proc.Expression = ShellHelper.AddShellIdToRefsForProcedureElement(proc.Expression, zone.Name, (proc.Type == (int)ProcedureType.Transaction | proc.Type == (int)ProcedureType.SubTransaction) ? true : false);

                if (tempAnnexes.Count > 0)
                {
                    foreach (var annex in tempAnnexes)
                    {
                        annex.MasterName = proc.Name;
                        _annexes.Add(annex);
                    }
                }
            }
            //_procedures.AddRange(zoneShellProcedures);

            return uiShellProcedures;
        }

        private void InitZoneShell(List<Procedure> procedures)
        {
            var exInfo = "\n>> " + GetType().FullName + ".InitZoneShell Error: ";

            if (procedures == null) return;
            if (procedures.Count == 0) return;

            try
            {
                //var procListByGrp = procedures.FindAll(x => x.GroupId == 0 & x.SkipOnInitFlag.ToLower() != "true" &
                var procListByGrp = procedures.FindAll(x => x.GroupId == 0 & x.SkipOnInit.ToLower() != "true" &
                x.Type != (int)ProcedureType.SubTransaction && x.Type != (int)ProcedureType.Args);
                RefreshProcedures(procListByGrp);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }
        }

        //##MergeZoneItems
        private void MergeZoneItems(LayoutElement zone)
        {
            try
            {
                var location = FileHelper.GetPath(zone.Location, _zonesDir);
                zone.Location = location;//abs path

                var tmpZoneItems = DataParserHelper.ConvertToGeneric<List<ZoneItem>>(location + "\\ui", true, TxtDataType.Undefined) ?? new List<ZoneItem>();


                if (tmpZoneItems.Count == 0) return;

                tmpZoneItems = tmpZoneItems.FindAll(x => x.TypeName == ZoneItemType.ControlContainer.ToString() | x.TypeName == ZoneItemType.Control.ToString() | x.TypeName == ZoneItemType.SubControl.ToString() |
                x.TypeName == ZoneItemType.SubTransaction.ToString() | x.TypeName == ZoneItemType.BeforeZoneRenderHandler.ToString() | x.TypeName == ZoneItemType.AfterZoneRenderHandler.ToString());
                if (tmpZoneItems.Count == 0) return;

                var zoneItems = new List<ZoneItem>();

                foreach (var item in tmpZoneItems)
                {
                    item.Invalid = ResolveConstants(item.Invalid);
                    if (item.Invalid.StartsWith("=")) item.Invalid = GetText(item.Invalid, GetTextType.OnlyResolveCed);
                    item.Invalid = item.Invalid.GetJudgementFlag();
                    if (item.Invalid.ToLower() != "true")
                    {
                        zoneItems.Add(item);
                    }
                }

                if (zoneItems.Count == 0) return;

                foreach (var item in zoneItems)
                {
                    UiHelper.SetZoneItemType(item);
                }
                UiHelper.CheckZoneItems(zone.Name, zoneItems);
                var annexList = CommonHelper.GetAnnexesFromCfgFile(location + "\\Annexes", "ZoneItem", false);
                foreach (var item in zoneItems)
                {
                    item.Invisible = ResolveConstants(item.Invisible);
                    item.Disabled = ResolveConstants(item.Disabled);
                    item.DisplayName = string.IsNullOrEmpty(item.DisplayName) ? "" : item.DisplayName;
                    item.DisplayName = ResolveConstants(item.DisplayName);
                    item.Value = string.IsNullOrEmpty(item.Value) ? "" : item.Value;
                    item.Value = ResolveConstants(item.Value);

                    item.Action = string.IsNullOrEmpty(item.Action) ? "" : item.Action;
                    item.Action = ResolveConstants(item.Action);
                    item.Action1 = string.IsNullOrEmpty(item.Action1) ? "" : item.Action1;
                    item.Action1 = ResolveConstants(item.Action1);

                    item.StyleClass = string.IsNullOrEmpty(item.StyleClass) ? "" : item.StyleClass;
                    item.StyleText = string.IsNullOrEmpty(item.StyleText) ? "" : item.StyleText;
                    item.StyleText = ResolveConstants(item.StyleText);
                    item.Location = string.IsNullOrEmpty(item.Location) ? "" : item.Location;
                    item.Location = ResolveConstants(item.Location);
                    item.DataSource = string.IsNullOrEmpty(item.DataSource) ? "" : item.DataSource;
                    item.DataSource = ResolveConstants(item.DataSource);

                    item.ShowRunningStatus = item.ShowRunningStatus.GetJudgementFlag();
                    item.ShowRunningStatus = item.ShowRunningStatus.GetJudgementFlag();

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

            var zoneItems1 = zoneItems.Where(x => x.Type == (int)ZoneItemType.SubTransaction | x.Type == (int)ZoneItemType.BeforeZoneRenderHandler | x.Type == (int)ZoneItemType.AfterZoneRenderHandler);
            foreach (var zoneItem in zoneItems1)
            {
                if (!String.IsNullOrEmpty(zoneItem.Action))
                {
                    zoneItem.Action = AddZoneIdentiferForZoneUiElement(zoneItem.Action, zone.Name, true);
                }
            }

            try
            {
                var zoneItems2 = zoneItems.Where(x => x.Type == (int)ZoneItemType.Control | x.Type == (int)ZoneItemType.ControlContainer).ToList();
                RenderThenArrangeZoneItems(zone, zoneItems2, zonePanel, cpntArrangementType, false);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderZoneItems Error: zoneName=" + zone.Name + ex.Message);
            }
        }

        //##RenderThenArrangeZoneItems
        private void RenderThenArrangeZoneItems(LayoutElement zoneOrCtrlCtn, List<ZoneItem> zoneItems, Control container, ZoneCpntArrangementType cpntArrangementType, bool isCtrlContainer)
        {
            var itemNameEx = "";
            try
            {
                var zoneItems1 = zoneItems.FindAll(x => x.ControlTypeName != ZoneControlType.Row.ToString());
                foreach (var zoneItem in zoneItems1)
                {
                    itemNameEx = zoneItem.Name;
                    RenderZoneItem(zoneOrCtrlCtn, zoneItem, container as Panel, cpntArrangementType);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderThenArrangeZoneItems-RenderZoneItems Error: zoneOrCtrlContainer=" + zoneOrCtrlCtn.Name + ", itemName=" + itemNameEx + " " + ex.Message);
            }

            try
            {
                if (cpntArrangementType == ZoneCpntArrangementType.RowLining)
                {
                    int rowDefaultHeight = 28;
                    var ctrlDefWidth = 0;
                    var ctrlDefHeight = 0;
                    var rows = zoneItems.FindAll(x => x.ControlTypeName == ZoneControlType.Row.ToString());

                    int rowPosY = 0;
                    foreach (var row in rows)
                    {
                        var rowItems = new List<ZoneItem>();
                        //rowItems = zoneItems.Where(x => (x.Type == (int)ZoneItemType.SubControl | x.Type == (int)ZoneItemType.Control) && x.Row == row.Name).ToList();
                        if (isCtrlContainer)
                            rowItems = zoneItems.Where(x => (x.Type == (int)ZoneItemType.SubControl) && x.Row == row.Name).ToList();
                        else
                            rowItems = zoneItems.Where(x => (x.Type == (int)ZoneItemType.ControlContainer | x.Type == (int)ZoneItemType.Control) && x.Row == row.Name).ToList();
                        int posX = 0;
                        int lastItemW = 0;
                        rowPosY = rowPosY + row.OffsetOrPositionY;
                        foreach (var rowItem in rowItems)
                        {
                            itemNameEx = rowItem.Name;
                            //posX = posX + lastItemW + (rowItem.OffsetOrPositionX == -1 ? itemDefaultOffsetX : rowItem.OffsetOrPositionX);
                            posX = posX + lastItemW + rowItem.OffsetOrPositionX;

                            var posY = rowPosY;
                            posY = rowPosY + rowItem.OffsetOrPositionY;

                            //--localize and set size for item
                            var ctrl = ControlBaseHelper.GetControlByNameByParentForRecursionSubCall(rowItem.Name, container);
                            if (ctrl == null) throw new ArgumentException("Control does not exists; Control.Name =" + rowItem.Name);

                            ctrl.Location = new Point(posX, posY);
                            ctrl.Width = rowItem.Width < 0 ? ctrlDefWidth : rowItem.Width;
                            ctrl.Height = rowItem.Height < 0 ? ctrlDefHeight : rowItem.Height;

                            lastItemW = ctrl.Width;
                        }
                        rowPosY = rowPosY + (row.Height < 0 ? rowDefaultHeight : row.Height);
                    }
                }
                else //ZoneArrangementType.Positioning, reset dock sequence
                {
                    var zoneItems1 = zoneItems.FindAll(x => x.ControlTypeName != ZoneControlType.Row.ToString());
                    var zoneItems2 = zoneItems1.OrderBy(x => x.DockOrder);
                    foreach (var item in zoneItems2)
                    {
                        itemNameEx = item.Name;
                        var ctrl = ControlBaseHelper.GetControlByNameByParentForRecursionSubCall(item.Name, container);
                        if (ctrl == null) throw new ArgumentException("Control does not exists; Control.Name =" + item.Name);
                        container.Controls.SetChildIndex(ctrl, 0);
                    }


                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderThenArrangeZoneItems-ArrangeZoneItems Error: zoneOrCtrlContainer=" + zoneOrCtrlCtn.Name + ", itemName=" + itemNameEx + " " + ex.Message);
            }
        }

        //*zone
        //*renderzoneitem
        //##RenderZoneItem
        private void RenderZoneItem(LayoutElement zone, ZoneItem zoneItem, Panel container, ZoneCpntArrangementType cpntArrangementType)
        {
            var zoneItemNameEx = zoneItem.Name;
            try
            {
                var location = DirectoryHelper.GetPath(zoneItem.Location, zone.Location);

                //--visible
                zoneItem.Invisible = AddZoneIdentiferForZoneUiElement(zoneItem.Invisible, zone.Name, false);
                var invisibleFlag = GetText(zoneItem.Invisible, GetTextType.UiItem);
                var isCpntVisible = !invisibleFlag.JudgeJudgementFlag();

                //--enabled
                zoneItem.Disabled = AddZoneIdentiferForZoneUiElement(zoneItem.Disabled, zone.Name, false);
                var disabledFlag = GetText(zoneItem.Disabled, GetTextType.UiItem);
                var isCpntEnabled = !disabledFlag.JudgeJudgementFlag();

                //--dataSrc
                //var dataSrc = "";
                if (!String.IsNullOrEmpty(zoneItem.DataSource))
                {
                    zoneItem.DataSource = AddZoneIdentiferForZoneUiElement(zoneItem.DataSource, zone.Name, false);
                }
                //--value
                if (!String.IsNullOrEmpty(zoneItem.Value))
                {
                    zoneItem.Value = AddZoneIdentiferForZoneUiElement(zoneItem.Value, zone.Name, false);
                }

                //--diplayName
                var displayName = "";
                if (!String.IsNullOrEmpty(zoneItem.DisplayName))
                {
                    zoneItem.DisplayName = AddZoneIdentiferForZoneUiElement(zoneItem.DisplayName, zone.Name, false);
                    if (zoneItem.DisplayName.StartsWith("="))
                    {
                        displayName = GetText(zoneItem.DisplayName, GetTextType.UiItem);
                    }
                    else
                    {
                        displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", zoneItem.Name, _annexes, zoneItem.DisplayName);
                    }
                }
                else
                {
                    displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", zoneItem.Name, _annexes, "");
                }

                //--action
                if (!String.IsNullOrEmpty(zoneItem.Action))
                {
                    zoneItem.Action = AddZoneIdentiferForZoneUiElement(zoneItem.Action, zone.Name, true);
                }

                //--action1
                if (!String.IsNullOrEmpty(zoneItem.Action1))
                {
                    zoneItem.Action1 = AddZoneIdentiferForZoneUiElement(zoneItem.Action1, zone.Name, true);
                }


                //--styleClass
                var styleClass = "";
                if (!String.IsNullOrEmpty(zoneItem.StyleClass))
                {
                    //zoneItem.StyleClass = ResolveConstants(zoneItem.StyleClass);
                    zoneItem.StyleClass = AddZoneIdentiferForZoneUiElement(zoneItem.StyleClass, zone.Name, false);
                    styleClass = GetText(zoneItem.StyleClass, GetTextType.UiItem);
                }

                //--styleText
                var styleText = "";
                if (!String.IsNullOrEmpty(zoneItem.StyleText))
                {
                    zoneItem.StyleText = AddZoneIdentiferForZoneUiElement(zoneItem.StyleText, zone.Name, false);
                    styleText = GetText(zoneItem.StyleText, GetTextType.UiItem);
                }

                //-render item
                if (zoneItem.ControlTypeName == ZoneControlType.SplitRectangle.ToString())
                {
                    var cpnt = new SplitRectangle();
                    cpnt.Name = zoneItem.Name;

                    ControlHelper.SetSplitRectangleStyleByClass(cpnt, styleClass);
                    ControlHelper.SetSplitRectangleStyleByText(cpnt, styleText);
                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                //--container
                else if (zoneItem.ControlTypeName == ZoneControlType.Panel.ToString())
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                    if (zoneItem.Type == (int)ZoneItemType.ControlContainer)
                    {
                        var cpntArrangeType = ZoneCpntArrangementType.Positioning;
                        var cpntArrangeTypeStyleText = zoneItem.StyleText.GetLdictValue("CpntArrangeType", true, true);
                        cpntArrangeType = EnumHelper.GetByName(cpntArrangeTypeStyleText, cpntArrangeType);
                        var subZoneItems = _zonesItems.FindAll(x => x.Container == zoneItem.Name && x.Type == (int)ZoneItemType.SubControl);
                        if (subZoneItems.Count > 0) RenderThenArrangeZoneItems(zone, subZoneItems, cpnt, cpntArrangeType, true);
                    }
                }
                else if (zoneItem.ControlTypeName == ZoneControlType.ContainerPanel.ToString())
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                    if (zoneItem.Type == (int)ZoneItemType.ControlContainer)
                    {
                        var cpntArrangeType = ZoneCpntArrangementType.Positioning;
                        var cpntArrangeTypeStyleText = zoneItem.StyleText.GetLdictValue("CpntArrangeType", true, true);
                        cpntArrangeType = EnumHelper.GetByName(cpntArrangeTypeStyleText, cpntArrangeType);
                        var subZoneItems = _zonesItems.FindAll(x => x.Container == zoneItem.Name && x.Type == (int)ZoneItemType.SubControl);
                        if (subZoneItems.Count > 0) RenderThenArrangeZoneItems(zone, subZoneItems, cpnt, cpntArrangeType, true);
                    }
                }
                else if (zoneItem.ControlTypeName == ZoneControlType.ShadowPanel.ToString())
                {
                    var cpnt = new ShadowPanel();
                    cpnt.Name = zoneItem.Name;

                    ControlHelper.SetShadowPanelStyleByClass(cpnt, styleClass);
                    ControlHelper.SetShadowPanelStyleByText(cpnt, styleText);
                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                    if (zoneItem.Type == (int)ZoneItemType.ControlContainer)
                    {
                        var cpntArrangeType = ZoneCpntArrangementType.Positioning;
                        var cpntArrangeTypeStyleText = zoneItem.StyleText.GetLdictValue("CpntArrangeType", true, true);
                        cpntArrangeType = EnumHelper.GetByName(cpntArrangeTypeStyleText, cpntArrangeType);
                        var subZoneItems = _zonesItems.FindAll(x => x.Container == zoneItem.Name && x.Type == (int)ZoneItemType.SubControl);
                        if (subZoneItems.Count > 0) RenderThenArrangeZoneItems(zone, subZoneItems, cpnt, cpntArrangeType, true);
                    }
                }
                else if (zoneItem.ControlTypeName == ZoneControlType.GroupBox.ToString())
                {
                    var cpnt = new GroupBox();
                    cpnt.Name = zoneItem.Name;

                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                //--label
                else if (zoneItem.ControlTypeName == ZoneControlType.Label.ToString())
                {
                    var cpnt = new Label();
                    cpnt.TextAlign = ContentAlignment.MiddleLeft;
                    cpnt.Name = zoneItem.Name;
                    cpnt.Text = displayName;
                    ControlHelper.SetLabelStyleByClass(cpnt, styleClass);
                    ControlHelper.SetLabelStyleByText(cpnt, styleText);
                    cpnt.AutoSize = false;

                    if (!zoneItem.Action.IsNullOrEmpty())
                    {
                        cpnt.Click += new System.EventHandler(ControlEventHandler);
                    }

                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.TitleLabel.ToString())
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                //--button
                else if (zoneItem.ControlTypeName == ZoneControlType.Button.ToString())
                {
                    var cpnt = new Button();
                    cpnt.Name = zoneItem.Name;
                    cpnt.Text = displayName;
                    ControlHelper.SetButtonStyleByClass(cpnt, styleClass);
                    ControlHelper.SetButtonStyleByText(cpnt, styleText);

                    //--event
                    if (!zoneItem.Action.IsNullOrEmpty())
                    {
                        cpnt.Click += new System.EventHandler(ControlEventHandler);
                    }

                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.PictureBox.ToString())
                {
                    var cpnt = new PictureBox();
                    cpnt.Name = zoneItem.Name;

                    var dataSrc = "";
                    if (!zoneItem.DataSource.IsNullOrEmpty())
                    {
                        dataSrc = GetText(zoneItem.DataSource, GetTextType.UiItem);
                    }
                    //var dataSrcPath = FileHelper.GetPath(dataSrc, location);
                    var imagUrl = GetImageUrl(dataSrc, location);
                    if (!imagUrl.IsNullOrEmpty())
                        ControlBaseHelper.SetControlBackgroundImage(cpnt, imagUrl);

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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }

                //--textbox
                else if (zoneItem.ControlTypeName == ZoneControlType.TextBox.ToString())
                {
                    var cpnt = new TextBox();
                    cpnt.Name = zoneItem.Name;
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var type = dict.GetLdictValue("Type");
                    if (type.ToLower() == "password".ToLower())
                    {
                        cpnt.PasswordChar = '*';
                    }

                    var hasNoBorder = dict.GetLdictValue("HasNoBorder").JudgeJudgementFlag();
                    if (hasNoBorder) cpnt.BorderStyle = BorderStyle.None;

                    var notInitValue = dict.GetLdictValue("NotInitValue").JudgeJudgementFlag();
                    if (!notInitValue)
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem);
                        if (!val.IsNullOrEmpty())
                            cpnt.Text = val;
                    }

                    ControlHelper.SetTextBoxStyleByClass(cpnt, styleClass);
                    ControlHelper.SetTextBoxStyleByText(cpnt, styleText);


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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.RichTextBox.ToString())
                {
                    var cpnt = new RichTextBox();
                    cpnt.Name = zoneItem.Name;
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var hasNoBorder = dict.GetLdictValue("HasNoBorder").JudgeJudgementFlag();
                    if (hasNoBorder) cpnt.BorderStyle = BorderStyle.None;

                    var notInitValue = dict.GetLdictValue("NotInitValue").JudgeJudgementFlag();
                    if (!notInitValue)
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem);
                        if (!val.IsNullOrEmpty())
                            cpnt.Text = val;
                    }

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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.DateTimePickerEx.ToString())
                {
                    var cpnt = new DateTimePickerEx();

                    cpnt.Name = zoneItem.Name;
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict, null, true, true);

                    var notInitValue = dict.GetLdictValue("NotInitValue").JudgeJudgementFlag();
                    if (!notInitValue)
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem);
                        if (!val.IsNullOrEmpty())
                            cpnt.Value = val;
                    }

                    ControlHelper.SetDateTimePickerExStyleByClass(cpnt, styleClass);
                    ControlHelper.SetDateTimePickerExStyleByText(cpnt, styleText);
                    //--event
                    if (!zoneItem.Action.IsNullOrEmpty())
                    {
                        cpnt.ValueChanged += new System.EventHandler(ControlEventHandler);
                    }

                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                //--option ctrls
                else if (zoneItem.ControlTypeName == ZoneControlType.RadioButton.ToString())
                {
                    var cpnt = new RadioButton();
                    cpnt.Name = zoneItem.Name;
                    cpnt.Text = displayName;
                    if (!zoneItem.Value.IsNullOrEmpty())
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem).JudgeJudgementFlag();
                        cpnt.Checked = val;
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.CheckBox.ToString())
                {
                    var cpnt = new CheckBox();

                    cpnt.Name = zoneItem.Name;
                    cpnt.Text = displayName;
                    if (!zoneItem.Value.IsNullOrEmpty())
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem).JudgeJudgementFlag();
                        cpnt.Checked = val;
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.ComboBox.ToString())//ComboBox
                {
                    var cpnt = new ComboBox();
                    cpnt.Name = zoneItem.Name;
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var style = dict.GetLdictValue("Style");
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
                    var keyValues = new List<KeyValue>();
                    if (!zoneItem.DataSource.IsNullOrEmpty())
                    {

                        var dataSrc = GetText(zoneItem.DataSource, GetTextType.UiItem);
                        var dataSourceTypeStr = dict.GetLdictValue("DataSourceType");
                        var dataSourceType = dataSourceTypeStr.GetTextDataType();
                        var dataSrcText = dataSrc;
                        var dataSrcPath = "";
                        if (dataSourceTypeStr == "DataFile" | dataSourceTypeStr == "ConfigFile" | dataSourceTypeStr == "TextDataFile")
                        {
                            dataSrcPath = FileHelper.GetPath(dataSrc, location);
                            dataSrcText = dataSrcPath;
                            keyValues = dataSrcText.ConvertToGeneric<List<KeyValue>>(true, dataSourceType);
                        }
                        keyValues = dataSrcText.ConvertToGeneric<List<KeyValue>>(true, dataSourceType);
                        if (keyValues != null)
                        {
                            if (keyValues.Count > 0)
                            {
                                var displayByLang = dict.GetLdictValue("DisplayByLang").JudgeJudgementFlag();
                                if (displayByLang && _formInitParamSet.SupportMultiLanguages)
                                {
                                    var annexesList = CommonHelper.GetAnnexesFromCfgFile(location + "\\DataSourceAnnexes", "", false);
                                    if (annexesList != null)
                                    {
                                        if (annexesList.Count > 0)
                                        {
                                            foreach (var keyValue in keyValues)
                                            {
                                                var displayName1 = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "", keyValue.Key, annexesList, keyValue.Key);
                                                if (!displayName1.IsNullOrEmpty()) keyValue.Value = displayName1;
                                            }

                                        }
                                    }

                                }
                            }

                        }

                    }
                    cpnt.DataSource = keyValues;
                    cpnt.ValueMember = "Key";
                    cpnt.DisplayMember = "Value";


                    container.Controls.Add(cpnt); //for ComboBox, this sentence must be before setting default value! else the selected value will be first one
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                    //--DefaultValue
                    var val = GetText(zoneItem.Value, GetTextType.UiItem);
                    if (style.ToLower() == "DropDown".ToLower())
                    {
                        if (!val.IsNullOrEmpty())
                        {
                            cpnt.SelectedValue = val;
                        }
                        else
                            cpnt.SelectedIndex = -1;
                    }
                    else if (!val.IsNullOrEmpty())
                    {
                        cpnt.SelectedValue = val;
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

                //--status ctrls
                else if (zoneItem.ControlTypeName == ZoneControlType.ProgressBar.ToString())
                {
                    var cpnt = new ProgressBar();
                    cpnt.Name = zoneItem.Name;
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var initValue = dict.GetLdictValue("StartValue").JudgeJudgementFlag();
                    if (initValue)
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem);
                        cpnt.Value = Convert.ToInt16(val);
                    }

                    ControlHelper.SetProgressBarStyleByClass(cpnt, styleClass);
                    ControlHelper.SetProgressBarStyleByText(cpnt, styleText);
                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = false;

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);
                }
                else if (zoneItem.ControlTypeName == ZoneControlType.StatusLight.ToString())
                {
                    var cpnt = new StatusLight();
                    cpnt.Name = zoneItem.Name;
                    cpnt.Text = displayName;
                    ControlHelper.SetStatusLightStyleByClass(cpnt, styleClass);
                    ControlHelper.SetStatusLightStyleByText(cpnt, styleText);
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var notInitDataSource = dict.GetLdictValue("NotInitDataSource").JudgeJudgementFlag();
                    if (!notInitDataSource)
                    {
                        var dataSrc = GetText(zoneItem.DataSource, GetTextType.UiItem);
                        cpnt.DataSourceText = dataSrc;
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);
                }
                //--data ctrls

                else if (zoneItem.ControlTypeName == ZoneControlType.DataGridViewEx.ToString())
                {
                    var cpnt = new DataGridViewEx();
                    cpnt.Name = zoneItem.Name;

                    ControlHelper.SetDataGridViewExStyleByClass(cpnt, styleClass);
                    ControlHelper.SetDataGridViewExStyleByText(cpnt, styleText);

                    cpnt.DataFilePath = location;
                    var dict = zoneItem.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var displayByLang = dict.GetLdictValue("DisplayByLang").ToLower() == "true";
                    if (displayByLang & _formInitParamSet.SupportMultiLanguages)
                    {
                        var annexesCfgFile = location + "\\HeaderAnnexes";
                        var annexes = CommonHelper.GetAnnexesFromCfgFile(annexesCfgFile, "", false);
                        cpnt.AnnexList = annexes;
                    }

                    var notInitDataSource = dict.GetLdictValue("NotInitDataSource").JudgeJudgementFlag();
                    if (!notInitDataSource)
                    {
                        var val = GetText(zoneItem.Value, GetTextType.UiItem);
                        cpnt.Value = val;
                        var dataSrc = GetText(zoneItem.DataSource, GetTextType.UiItem);
                        cpnt.DataSourceText = dataSrc;
                    }

                    if (!isCpntVisible) cpnt.Visible = false;
                    if (!isCpntEnabled) cpnt.Enabled = true;

                    //--event
                    if (!zoneItem.Action.IsNullOrEmpty())
                    {
                    }
                    if (!zoneItem.Action1.IsNullOrEmpty())
                    {
                    }

                    container.Controls.Add(cpnt);
                    if (cpntArrangementType == ZoneCpntArrangementType.Positioning)
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);
                }


                else if (zoneItem.ControlTypeName == ZoneControlType.TextButton.ToString())
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.ImageTextButtonH.ToString() | zoneItem.ControlTypeName == MenuControlType.ImageTextButtonV.ToString())
                {
                    var cpnt = new ImageTextButton();
                    cpnt.Name = zoneItem.Name;
                    cpnt.Text = displayName;

                    //--image
                    var imgDir = zone.Location;

                    if (!zoneItem.DataSource.IsNullOrEmpty())
                    {
                        var dataSrc = GetText(zoneItem.DataSource, GetTextType.UiItem);
                        var imagUrl = GetImageUrl(dataSrc, zone.Location);
                        if (!imagUrl.IsNullOrEmpty())
                            cpnt.Image = ControlBaseHelper.GetImage(imagUrl);
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }
                else if (zoneItem.ControlTypeName == ZoneControlType.CommandLabel.ToString())
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
                        ControlBaseHelper.SetControlDockStyleAndLocationAndSize(cpnt, zoneItem.DockType, zoneItem.Width, zoneItem.Height, zoneItem.OffsetOrPositionX, zoneItem.OffsetOrPositionY);

                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RenderZoneItem Error: zoneItemName=" + zoneItemNameEx + " " + ex.Message);
            }

        }


        //*subfunc
        //*gettext
        //##GetText
        protected string GetText(string str, GetTextType getTextTp)
        {
            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }
            try
            {
                if (!str.StartsWith("="))
                {
                    if (getTextTp == GetTextType.ShellItem)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                        else if (str.IsShellElementDataFormat())
                            str = ResolveStringByRefShellVariables(str);
                    }
                    else if (getTextTp == GetTextType.UiItem)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                        else if (str.IsShellElementDataFormat())
                            str = ResolveStringByRefShellVariables(str);
                        else if (str.IsUiElementDataFormat())
                            str = ResolveStringByRefControls(str);
                    }
                    else if (getTextTp == GetTextType.OnlyResolveCed)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                    }
                    else { }//noresolve

                    return str;
                }


                var getNameAndParamsStr = str.Substring(1, str.Length - 1).Trim();
                var getNameAndParamsArray = getNameAndParamsStr.GetFormularParamArray(false, false);
                var funcName = getNameAndParamsArray[0];
                var funcParamArray = new string[getNameAndParamsArray.Length - 1];
                for (int i = 0; i < getNameAndParamsArray.Length - 1; i++)
                {
                    funcParamArray[i] = getNameAndParamsArray[i + 1];
                }

                for (int i = 0; i < funcParamArray.Length; i++)
                {

                    var str1 = funcParamArray[i];

                    if (getTextTp == GetTextType.ShellItem)
                    {
                        if (str1.IsCenterExchangeDataFormat())
                            funcParamArray[i] = ResolveCentralExchangeData(str1);
                        else if (str1.IsShellElementDataFormat())
                            funcParamArray[i] = ResolveStringByRefShellVariables(str1);
                    }
                    else if (getTextTp == GetTextType.UiItem)
                    {
                        if (str1.IsCenterExchangeDataFormat())
                            funcParamArray[i] = ResolveCentralExchangeData(str1);
                        else if (str1.IsShellElementDataFormat())
                            funcParamArray[i] = ResolveStringByRefShellVariables(str1);
                        else if (str1.IsUiElementDataFormat())
                            funcParamArray[i] = ResolveStringByRefControls(str1);
                    }
                    else if (getTextTp == GetTextType.OnlyResolveCed)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                    }
                    else { }

                    var str2 = funcParamArray[i];
                    if (str2.StartsWith("="))
                    {
                        throw new ArgumentException("Nesting of formulas is not supported");
                    };
                }

                var returnText = Get(funcName, funcParamArray);
                return returnText;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetText Error: params=" + str + "; " + ex.Message);
            }

        }

        //*transact
        //##TransactByElementName
        private void TransactByElementName(string elementName, TransactFrom transFr)
        {
            TransactByElementName(elementName, transFr, 0);
        }

        private void TransactByElementName(string elementName, TransactFrom transFr, int id)
        {
            var action = "";
            var displayName = "";
            var followingActions = new List<LayoutElement>();
            var showRunningStatus = false;
            var writeIntoLog = false;
            ExecMode execMode = ExecMode.Sync;

            if (transFr == TransactFrom.FromShell)
            {
                var elementName1 = IdentifierHelper.DeleteShellIdentifer(elementName);
                var item = _procedures.Find(x => x.Name == elementName1);
                if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByElementName Error: ProcessElementName={0} des not exist!", elementName));
                if (item.Expression.IsNullOrEmpty()) return;
                showRunningStatus = item.ShowRunningStatus.ToLower() == "true";
                displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "Procedure", item.Name, _annexes, item.DisplayName);
                if (displayName.IsNullOrEmpty()) displayName = "#" + item.Name + "#";
                if (item.Expression.IsNullOrEmpty()) return;
                action = item.Expression;
                action = (action.IsCenterExchangeDataFormat() | action.IsShellElementDataFormat()) ? GetText(action, GetTextType.ShellItem) : action;
            }

            else if (transFr == TransactFrom.FromMenu | transFr == TransactFrom.FromViewEvent | transFr == TransactFrom.FromZoneEvent | transFr == TransactFrom.FromZoneUi)
            {
                var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);//ByMenu
                if (elementName1.GetQtyOfIncludedChar('_') == 0) //menuitem==0(Only for menuitem, Following transaction in view 不会到此处)
                {
                    var item = _layoutElements.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByElementName Error: UiElementName={0} des not exist!", elementName));
                    if (item.Action.IsNullOrEmpty()) return;
                    showRunningStatus = item.ShowRunningStatus.GetJudgementFlag() == "true";
                    if (elementName1.GetQtyOfIncludedChar('_') == 0)
                    {
                        displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "MenuItem", item.Name, _annexes, item.DisplayName);
                        if (displayName.IsNullOrEmpty()) displayName = "$" + item.Name + "$";
                    }

                    action = item.Action;
                    action = (action.IsCenterExchangeDataFormat()) ? GetText(action, GetTextType.OnlyResolveCed) : action;

                }
                else if (elementName1.GetQtyOfIncludedChar('_') == 2)//zone item
                {
                    var item = _zonesItems.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByElementName Error: UiElementName={0} des not exist!", elementName));
                    if (id == 0)
                    {
                        if (item.Action.IsNullOrEmpty()) return;
                        action = item.Action;
                        showRunningStatus = item.ShowRunningStatus.GetJudgementFlag() == "true";
                    }
                    else
                    {
                        if (item.Action1.IsNullOrEmpty()) return;
                        action = item.Action1;
                        showRunningStatus = item.ShowRunningStatus.GetJudgementFlag() == "true";
                    }
                    displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", item.Name, _annexes, item.DisplayName);
                    if (displayName.IsNullOrEmpty()) displayName = "$" + item.Name + "$";

                    action = (action.IsCenterExchangeDataFormat() | action.IsShellElementDataFormat() | action.IsUiElementDataFormat()) ? GetText(action, GetTextType.UiItem) : action;

                    var followingActions1 = _layoutElements.FindAll(x => x.Trigger == elementName1);
                    var followingActions2 = _layoutElements.FindAll(x => x.Type == (int)LayoutElementType.ActionWatcher);
                    followingActions = _layoutElements.FindAll(x => x.Trigger == elementName1 & x.Type == (int)LayoutElementType.ActionWatcher);
                }
                else
                {
                    return;
                }

            }
            else return;

            if (!action.IsNullOrEmpty() | action != "DoNothing")
            {
                var transParams = ParseAction(action, transFr);
                var transactionDetail = new TransactionDetail()
                {
                    Name = transParams.Name,
                    Params = transParams.Params,
                    DisplayName = displayName,
                    ShowRunningStatus = showRunningStatus,
                    ExecMode = execMode,
                    WriteIntoLog = writeIntoLog
                };
                try
                {
                    Transact(transactionDetail);
                }
                catch (Exception ex)
                {
                    var exInfo = "\n>> " + GetType().FullName + ".TransactByElementName Error: ";
                    throw new ArgumentException(exInfo + string.Format("elementName={0}; action={1} " + ex.Message, elementName, action));
                }
            }
            var exInfo1 = "\n>> " + GetType().FullName + ".TransactByElementName-FollowingAction Error: ";
            foreach (var followingAction in followingActions)
            {
                if (followingAction.Action.IsNullOrEmpty()) break;
                if (followingAction.Action == "DoNothing") break;
                var displayName1 = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ViewItem", followingAction.Name, _annexes, followingAction.DisplayName);
                if (displayName1.IsNullOrEmpty()) displayName = "$" + followingAction.Name + "$";
                var showRunningStatus1 = followingAction.ShowRunningStatus.GetJudgementFlag() == "true";
                var writeIntoLog1 = false;
                var execMode1 = ExecMode.Sync;
                var followingActionStr = followingAction.Action;
                followingActionStr = (followingActionStr.IsCenterExchangeDataFormat() | followingActionStr.IsUiElementDataFormat()) ? GetText(followingActionStr, GetTextType.UiItem) : followingActionStr;
                var followingTransParams1 = ParseAction(followingActionStr, transFr);
                var followingTransactionDetail = new TransactionDetail()
                {
                    Name = followingTransParams1.Name,
                    Params = followingTransParams1.Params,
                    DisplayName = displayName1,
                    ShowRunningStatus = showRunningStatus1
                    ,
                    ExecMode = execMode1,
                    WriteIntoLog = writeIntoLog1
                };
                try
                {
                    Transact(followingTransactionDetail);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo1 + string.Format("elementName={0}; action={1} ; followingAction.Name={2} ; followingAction.Action={3} " + ex.Message, elementName, action, followingAction.Name, followingAction.Action));
                }
            }

        }

        private void TransactByTransaction(Transaction transaction, TransactFrom transFr)  //for subMenuItem/contentMenuItem/startTransactions
        {
            var action = "";

            var showRunningStatus = false;
            var displayName = "";
            ExecMode execMode = ExecMode.Sync;
            var writeIntoLog = false;
            var controlName = "";
            try
            {
                if (transaction != null)
                {
                    action = transaction.Action;
                    showRunningStatus = transaction.ShowRunningStatus.GetJudgementFlag() == "true";
                    displayName = transaction.DisplayName.IsNullOrEmpty() ? "" : transaction.DisplayName;
                    controlName = transaction.UiItemName.IsNullOrEmpty() ? "" : transaction.UiItemName;
                }
                if (action.IsNullOrEmpty()) return;
                if (action == "DoNothing") return;
                action = (action.IsCenterExchangeDataFormat()) ? GetText(action, GetTextType.OnlyResolveCed) : action;

                var transParams = ParseAction(action, transFr);
                var transactionDetail = new TransactionDetail()
                {
                    Name = transParams.Name,
                    Params = transParams.Params,
                    DisplayName = displayName,
                    ShowRunningStatus = showRunningStatus,
                };
                Transact(transactionDetail);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByTransaction Error: controlName={0}; action={1} " + ex.Message, controlName, action));
            }
        }

        private TransactionParams ParseAction(string action, TransactFrom transFr)
        {
            var actionNameAndParamsArray = action.GetFormularParamArray(false, false);
            var actionName = actionNameAndParamsArray[0];
            var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
            for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
            {
                actionParamArray[i] = actionNameAndParamsArray[i + 1];
            }
            var transName = ParseActionName(actionName, transFr);
            if (transName == "Xrun")
            {
                var transParams = new TransactionParams();
                transParams.Name = transName;
                transParams.Params = actionParamArray;
                return transParams;
            }

            if (transName == "Crun")
            {
                var con = actionParamArray[0];
                con = ParseActionName(con, transFr);
                var action1 = "";
                if (con.JudgeJudgementFlag())
                {
                    action1 = actionParamArray[1];
                }
                else
                {
                    if (actionParamArray.Length > 2)
                        action1 = actionParamArray[2];
                    else action1 = "DoNothing";
                }
                return ParseAction(action1, transFr);
            }
            else
            {
                for (int i = 0; i < actionParamArray.Length; i++)
                {
                    actionParamArray[i] = actionNameAndParamsArray[i + 1];
                }
                var actionParamArray1 = ParseActionParams(actionParamArray, transFr);
                var transParams = new TransactionParams();
                transParams.Name = transName;
                transParams.Params = actionParamArray1;
                return transParams;
            }
        }
        private string ParseActionName(string funcName, TransactFrom transFr)
        {
            //if (transFr == TransactFrom.FromMenu)
            //    funcName = GetText(funcName, GetTextType.OnlyResolveCed);
            //else if (transFr == TransactFrom.FromViewEvent)
            //    funcName = GetText(funcName, GetTextType.OnlyResolveCed);
            if (transFr == TransactFrom.FromZoneEvent | transFr == TransactFrom.FromZoneUi)
                funcName = GetText(funcName, GetTextType.UiItem);
            else if (transFr == TransactFrom.FromShell)
                funcName = GetText(funcName, GetTextType.ShellItem);
            else funcName = GetText(funcName, GetTextType.OnlyResolveCed);
            return funcName;
        }

        private string[] ParseActionParams(string[] actionParamArray, TransactFrom transFr)
        {

            for (int i = 0; i < actionParamArray.Length; i++)
            {
                //if (transFr == TransactFrom.FromMenu | transFr == TransactFrom.FromViewEvent)
                //    funcParamArray[i] = GetText(funcParamArray[i], GetTextType.OnlyResolveCed);
                //else if (transFr == TransactFrom.FromMenu | transFr == TransactFrom.FromSubMenu)
                //    funcParamArray[i] = GetText(funcParamArray[i], GetTextType.OnlyResolveCed);
                if (transFr == TransactFrom.FromZoneEvent | transFr == TransactFrom.FromZoneUi)
                    actionParamArray[i] = GetText(actionParamArray[i], GetTextType.UiItem);
                else if (transFr == TransactFrom.FromShell)
                    actionParamArray[i] = GetText(actionParamArray[i].Trim(), GetTextType.ShellItem);
                else actionParamArray[i] = GetText(actionParamArray[i].Trim(), GetTextType.OnlyResolveCed);

            }
            return actionParamArray;
        }


        ///////////////////////////////By Liggin2019 on 20200308////////////////////////////////////////
        //--Please optimize all try/catch before Release in front of this point, after full testing!!!//
        ////////////////////////////////////////////////////////////////////////////////////////////////
        //*common
        //*transact
        //###Transact
        private void Transact(TransactionDetail transactionDetail)
        {
            var displayName = transactionDetail.DisplayName;
            var funcName = transactionDetail.Name;
            var funcParamArray = transactionDetail.Params;
            var showRunningStatus = transactionDetail.ShowRunningStatus;
            var execMode = transactionDetail.ExecMode;
            var writeIntoLog = transactionDetail.WriteIntoLog;
            if (showRunningStatus)
            {
                RefreshRunningStatusMessage(displayName);
            }
            try
            {
                if (execMode == ExecMode.Sync)
                {
                    Do(funcName, funcParamArray);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Transact Error: funcName={0}; " + ex.Message, funcName));
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

        //###DoInThreadPool
        protected object DoInThreadPool(Object objParams)
        {
            var transactionParams = objParams as TransactionParams;
            Do(transactionParams.Name, transactionParams.Params);
            return null;
        }
        //*view
        //###AddViewIdentiferForViewUiElement
        private string AddViewIdentiferForViewUiElement(string str, string viewName, bool isForAction)
        {
            if (!isForAction &
                !(str.StartsWith(IdentifierHelper.UiIdentifer) & str.EndsWith(IdentifierHelper.UiIdentifer)) & !str.StartsWith("=")
                )
                return str;

            if (str.IsNullOrEmpty()) return string.Empty;
            if (str.Contains("$"))
            {
                var strArray = str.Split('$');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '$' no. in " + str + " is not a even! ");//*old
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

        //*shell
        //###RefreshProcedures
        private void RefreshProcedures(List<Procedure> procList)
        {
            foreach (var proc in procList)
            {
                var toDo = true;
                if (toDo)
                {
                    if (proc.Type == (int)ProcedureType.Variable)
                    {
                        if (!proc.Expression.IsNullOrEmpty())
                            if (!string.IsNullOrEmpty(proc.Expression))
                            {
                                proc.Value = GetText(proc.Expression, GetTextType.ShellItem);
                            }
                    }
                    else if (proc.Type == (int)ProcedureType.Transaction)
                    {
                        if (!string.IsNullOrEmpty(proc.Expression))
                        {
                            TransactByElementName("#" + proc.Name + "#", TransactFrom.FromShell);
                        }
                    }
                }
            }
        }

        //###RefreshProcedure
        private void RefreshProcedure(string procName)
        {

            var proc = _procedures.Find(x => x.Name == procName);
            if (proc == null)
            {
                return;
            }
            var toDo = true;
            if (toDo)
            {
                if (proc.Type == (int)ProcedureType.Variable)
                {
                    if (!proc.Expression.IsNullOrEmpty())
                        if (!string.IsNullOrEmpty(proc.Expression))
                        {
                            proc.Value = GetText(proc.Expression, GetTextType.ShellItem);
                        }
                }
                else if (proc.Type == (int)ProcedureType.Transaction)
                {
                    if (!string.IsNullOrEmpty(proc.Expression))
                    {
                        TransactByElementName("#" + proc.Name + "#", TransactFrom.FromShell);
                    }
                }
            }
        }

        //*zone
        //###AddZoneIdentifer
        private string AddZoneIdentiferForZoneUiElement(string str, string zoneName, bool isForAction)
        {
            if (!isForAction &
                !(str.StartsWith(IdentifierHelper.UiIdentifer) & str.EndsWith(IdentifierHelper.UiIdentifer))
                & !(str.StartsWith(IdentifierHelper.ShellIdentifer) & str.EndsWith(IdentifierHelper.ShellIdentifer))
                & !str.StartsWith("=")
                )
                return str;


            if (str.IsNullOrEmpty()) return string.Empty;
            var str1 = str;
            if (str.Contains("#"))
            {
                var strArray = str.Split('#');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '#' no. in " + str + " is not a even! ");//*old
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
                    throw new ArgumentException(" '$' no. in " + str + " is not a even! ");//old
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)//even
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


        //*ctrl
        //*control
        //###RefreshControlValue
        private void RefreshControlValue(string ctrlName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".RefreshControlValue Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //defVal
            var val = "";
            if (!String.IsNullOrEmpty(item.Value))
            {
                val = GetText(item.Value, GetTextType.UiItem);
            }

            SetControlValue(ctrlName, val);
        }

        //###SetControlValue
        private void SetControlValue(string ctrlName, string val)
        {
            var exInfo = "\n>> " + GetType().FullName + ".SetControlValue Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "Control doesn't exist! ctrlName=" + ctrlName);

            //--refresh item
            var ctrl = this.GetControl(item.Name);

            if (item.ControlTypeName == ZoneControlType.RadioButton.ToString())
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
            else if (item.ControlTypeName == ZoneControlType.CheckBox.ToString())
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
            else if (item.ControlTypeName == ZoneControlType.TextBox.ToString())
            {
                var cpnt = ctrl as TextBox;
                cpnt.Text = val;
            }

            else if (item.ControlTypeName == ZoneControlType.RichTextBox.ToString())
            {
                var cpnt = ctrl as RichTextBox;
                cpnt.Text = val;
            }
            else if (item.ControlTypeName == ZoneControlType.DateTimePickerEx.ToString())
            {
                var cpnt = ctrl as DateTimePickerEx;
                cpnt.Value = val;
            }
            else if (item.ControlTypeName == ZoneControlType.ComboBox.ToString())
            {
                var cpnt = ctrl as ComboBox;
                cpnt.SelectedValue = val;
            }
            else if (item.ControlTypeName == ZoneControlType.ProgressBar.ToString())
            {
                var cpnt = ctrl as ProgressBar;
                cpnt.Value = Convert.ToInt32(val);
            }

        }

        //###RefreshControlText
        private void RefreshControlText(string ctrlName)
        {
            var exInfo = "\n>> " + GetType().FullName + ".RefreshControlText Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);
            var text = "";
            if (!item.DisplayName.IsNullOrEmpty())
            {
                text = GetText(item.DisplayName, GetTextType.UiItem);
            }

            if (item.ControlTypeName == ZoneControlType.PictureBox.ToString())
            {
                SetZoneControlText(item, text);
            }

        }

        //###SetControlText
        private void SetControlText(string ctrlName, string text)
        {
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);
            SetZoneControlText(item, text);
        }

        //##SetControlText
        private void SetZoneControlText(ZoneItem item, string text)
        {
            var ctrl = this.GetControl(item.Name);
            if (item.ControlTypeName.Contains("Label"))
            {
                ctrl.Text = text;
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


        //###GetControlAttributeValue
        private string GetControlAttributeValue(string expression)
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetControlAttributeValue Error: ";

            expression = expression.Trim();
            var ctrlName = "";
            if (expression.ToLower().EndsWith(".v")
                | expression.ToLower().EndsWith(".t")
                //| expression.ToLower().EndsWith(".m") | expression.ToLower().EndsWith(".msg") //for statuslight, scorelight
                //| expression.ToLower().EndsWith(".vt") | expression.ToLower().EndsWith(".valtxt")//for listview and ValTxtPicker
                //| expression.ToLower().EndsWith(".vs") | expression.ToLower().EndsWith(".vals") //for listview and ValTxtPicker
                //| expression.ToLower().EndsWith(".ts") | expression.ToLower().EndsWith(".txts")//for listview and ValTxtPicker
                //| expression.ToLower().EndsWith(".id") | expression.ToLower().EndsWith(".idtxt") || expression.ToLower().EndsWith(".idtxts")  //for listview?
                //| expression.ToLower().EndsWith(".pi") //?
                //| expression.ToLower().EndsWith(".ps") //?
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

            if (typeName.Contains(ZoneControlType.Label.ToString()))
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
            else if (typeName == ZoneControlType.TextBox.ToString())
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
            else if (typeName == ZoneControlType.RichTextBox.ToString())
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
            else if (typeName == ZoneControlType.DateTimePickerEx.ToString())
            {
                var cpnt = ctrl as DateTimePickerEx;
                if (expression.ToLower().EndsWith(".v"))
                {
                    retStr = cpnt.Value;
                }
                else
                {
                    throw new ArgumentException(exInfo + "Control attribute value expression error: " + expression);
                }
            }

            else if (typeName == ZoneControlType.RadioButton.ToString())
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
            else if (typeName == ZoneControlType.CheckBox.ToString())
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
            else if (typeName == ZoneControlType.ComboBox.ToString())
            {
                var cpnt = ctrl as ComboBox;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".t"))
                    {
                        retStr = Convert.ToString(cpnt.Text);
                    }
                    else if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = Convert.ToString(cpnt.SelectedValue);
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "ComboBox attribute value expression error: " + expression);
                    }
                }
            }

            else if (typeName == ZoneControlType.ProgressBar.ToString())
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
            else if (typeName == ZoneControlType.StatusLight.ToString())
            {
                var cpnt = ctrl as StatusLight;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = Convert.ToString(cpnt.Value).ToLower();
                    }
                    else if (expression.ToLower().EndsWith(".t"))
                    {
                        retStr = Convert.ToString(cpnt.Message);
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "StatusLight attribute value expression error: " + expression);
                    }

                }
            }

            else if (typeName == ZoneControlType.DataGridViewEx.ToString())
            {
                var cpnt = ctrl as DataGridViewEx;
                if (cpnt != null)
                {
                    if (expression.ToLower().EndsWith(".v"))
                    {
                        retStr = cpnt.Value;
                    }
                    else if (expression.ToLower().EndsWith(".t"))
                    {
                        retStr = Convert.ToString(cpnt.Text);
                    }
                    else
                    {
                        throw new ArgumentException(exInfo + "DataGridViewEx attribute value expression error: " + expression);
                    }
                }
            }

            else
            {
                throw new ArgumentException(exInfo + "Control type doesn't exist, type name:" + typeName);
            }
            return retStr;

        }

        //###RefreshControlDataSource
        private void RefreshControlDataSource(string ctrlName)
        {

            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //refresh item
            var ctrl = GetControl(ctrlName);

            if (item.ControlTypeName == ZoneControlType.PictureBox.ToString())
            {
                if (!String.IsNullOrEmpty(item.DataSource))
                {
                    var cpnt = ctrl as PictureBox;
                    var dataSrc = GetText(item.DataSource, GetTextType.UiItem);
                    var arry = ctrlName.Split('_');
                    var zoneName = arry.Unwrap('_'.ToString(), 0, 2);
                    var zone = _layoutElements.Find(x => x.Name == zoneName);
                    var imagUrl = GetImageUrl(dataSrc, zone.Location);
                    ControlBaseHelper.SetControlBackgroundImage(cpnt, imagUrl);
                }
            }
            else if (item.ControlTypeName == ZoneControlType.ComboBox.ToString())
            {
                if (item.DataSource.IsNullOrEmpty()) return;

                var cpnt = ctrl as ComboBox;
                var keyValues = new List<KeyValue>();
                var dict = item.StyleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                var dataSourceTypeStr = dict.GetLdictValue("DataSourceTypeStr");
                var dataSrc = GetText(item.DataSource, GetTextType.UiItem);


                var arry = item.Name.Split('_');
                var zoneName = arry.Unwrap('_'.ToString(), 0, 2);
                var zone = _layoutElements.Find(x => x.Name == zoneName);
                var location = DirectoryHelper.GetPath(item.Location, zone.Location);

                var dataSourceType = dataSourceTypeStr.GetTextDataType();
                var dataSrcText = dataSrc;
                var dataSrcPath = "";

                if (dataSourceTypeStr == "DataFile" | dataSourceTypeStr == "ConfigFile" | dataSourceTypeStr == "TextDataFile")
                {
                    dataSrcPath = FileHelper.GetPath(dataSrc, location);
                    dataSrcText = dataSrcPath;
                    keyValues = dataSrcText.ConvertToGeneric<List<KeyValue>>(true, dataSourceType);
                }
                keyValues = dataSrcText.ConvertToGeneric<List<KeyValue>>(true, dataSourceType);

                var displayByLang = dict.GetLdictValue("DisplayByLang").JudgeJudgementFlag();
                if (displayByLang && _formInitParamSet.SupportMultiLanguages)
                {
                    if (keyValues != null)
                    {
                        if (keyValues.Count > 0)
                        {
                            var annexesList = CommonHelper.GetAnnexesFromCfgFile(location + "\\DataSourceAnnexes", "", false);
                            if (annexesList != null)
                            {
                                if (annexesList.Count > 0)
                                {
                                    foreach (var keyValue in keyValues)
                                    {
                                        var displayName1 = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "", keyValue.Key, annexesList, keyValue.Key);
                                        if (!displayName1.IsNullOrEmpty()) keyValue.Value = displayName1;
                                    }

                                }
                            }

                        }
                    }

                }

                cpnt.DataSource = keyValues;
                cpnt.ValueMember = "Key";
                cpnt.DisplayMember = "Value";
                //cpnt.SelectedIndex = selectedIndex;
            }
            else if (item.ControlTypeName == ZoneControlType.StatusLight.ToString())
            {
                if (!String.IsNullOrEmpty(item.DataSource))
                {
                    var cpnt = ctrl as StatusLight;
                    var dataSrc = GetText(item.DataSource, GetTextType.UiItem);
                    cpnt.DataSourceText = dataSrc;
                    cpnt.RefreshDataSource();
                }
            }

            else if (item.ControlTypeName == ZoneControlType.DataGridViewEx.ToString())
            {
                if (!String.IsNullOrEmpty(item.DataSource))
                {
                    var cpnt = ctrl as DataGridViewEx;
                    var dataSrc = GetText(item.DataSource, GetTextType.UiItem);
                    cpnt.DataSourceText = dataSrc;
                    cpnt.RefreshDataSource();
                }
            }
        }

        //###RefreshControlInvisible
        private void RefreshControlInvisible(string ctrlName)
        {
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException("ZoneItem doesn't exist! ZoneItem=" + ctrlName);
            //Visible
            var invisibleFlag = GetText(item.Invisible, GetTextType.UiItem);
            var isCpntVisible = !invisibleFlag.JudgeJudgementFlag();

            //refresh item
            var ctrl = GetControl(item.Name);
            if (!isCpntVisible) ctrl.Visible = false; else ctrl.Visible = true;

        }

        //###SetControlVisible
        private void SetControlVisible(string ctrlName, bool isVisible)
        {
            var exInfo = "\n>> " + GetType().FullName + ".SetControlVisible Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //set item
            var ctrl = GetControl(item.Name);
            if (!isVisible) ctrl.Visible = false; else ctrl.Visible = true;
        }

        //###RefreshControlDisabled
        private void RefreshControlDisabled(string ctrlName)
        {
            var exInfo = "\n >> " + GetType().FullName + ".RefreshControlDisabled Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            //Enabled
            var disabledFlag = GetText(item.Disabled, GetTextType.UiItem);
            var isCpntEnabled = !disabledFlag.JudgeJudgementFlag();

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

        //###SetControlEnabled
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

        //###RefreshControl
        private void RefreshControl(string ctrlName)
        {
            var exInfo = "\n >> " + GetType().FullName + ".RefreshControl Error: ";
            var item = _zonesItems.Find(x => x.Name == ctrlName);
            if (item == null) throw new ArgumentException(exInfo + "ZoneItem doesn't exist! ZoneItem=" + ctrlName);

            var ctrl = this.GetControl(item.Name);

            if (item.ControlTypeName == ZoneControlType.RadioButton.ToString())
            {
                var cpnt = ctrl as RadioButton;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == ZoneControlType.CheckBox.ToString())
            {
                var cpnt = ctrl as CheckBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == ZoneControlType.TextBox.ToString())
            {
                var cpnt = ctrl as TextBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == ZoneControlType.RichTextBox.ToString())
            {
                var cpnt = ctrl as RichTextBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == ZoneControlType.DateTimePickerEx.ToString())
            {
                var cpnt = ctrl as DateTimePickerEx;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == ZoneControlType.ComboBox.ToString())
            {
                var cpnt = ctrl as ComboBox;
                cpnt.Refresh();
            }
            else if (item.ControlTypeName == ZoneControlType.ProgressBar.ToString())
            {
                var cpnt = ctrl as ProgressBar;
                cpnt.Refresh();
            }
            if (item.ControlTypeName == ZoneControlType.StatusLight.ToString())
            {
                var cpnt = ctrl as StatusLight;
                cpnt.Refresh();
            }

        }

        //###GetControl
        protected Control GetControl(string ctrlName)
        {
            var ctrls = this.Controls.Find(ctrlName, true);
            if (ctrls.Length == 0) throw new ArgumentException("\n>> " + "Control doesn't exisit! ctrlName = " + ctrlName);
            var ctrl = ctrls[0];
            return ctrl;
        }

        //*subcommon
        //*get
        //###Get
        protected string Get(string funcName, string[] funcParamArray)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Get Error: ";
            if (funcName == "Equal")
            {
                return funcParamArray[0];
            }
            else if (funcName == "GetAnnexText")
            {

                var className = funcParamArray[0];
                var masterName = funcParamArray[1];
                masterName = masterName.IsShellElementDataFormat() ? masterName.DeleteShellIdentifer() : masterName;
                masterName = masterName.IsUiElementDataFormat() ? masterName.DeleteUiIdentifer() : masterName;
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 2)
                {
                    var annexTypeStr = funcParamArray[2];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var deftext = funcParamArray.Length > 3 ? funcParamArray[3] : "";
                var tempStr = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.StepByStep, deftext);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.CurrentOrFirst, deftext);
                }
                return tempStr;
            }
            else if (funcName == "GetSharedAnnexText")
            {
                var className = "Shared";
                var masterName = funcParamArray[0];
                var annexType = AnnexTextType.DisplayName;

                var tempStr = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.Current, masterName);
                }
                else
                {
                    tempStr = masterName;
                }
                return tempStr;
            }

            else if (funcName == "GetApplicationAnnexText")
            {
                var className = "Application";
                var masterName = _formInitParamSet.ApplicationCode;
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 0)
                {
                    var annexTypeStr = funcParamArray[0];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var deftext = funcParamArray.Length > 1 ? funcParamArray[1] : "";

                var tempStr = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.StepByStep, deftext);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.CurrentOrFirst, deftext);
                }
                return tempStr;
            }
            else if (funcName == "GetCurrentViewName")
            {
                return _currentViewName;
            }
            else if (funcName == "GetContentFromChosenTextFile" | funcName == "GetCttFrChsTextFile")
            {
                var filePath = Get("ChooseFile", funcParamArray);
                if (!filePath.IsNullOrEmpty())
                    return FileHelper.GetContentFromTextFile(filePath);
                return string.Empty;
            }
            else if (funcName == "ChooseFile")
            {
                var dlg = new OpenFileDialog();
                dlg.Title = TextRes.ChooseFile;
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
                return string.Empty;
            }
            else if (funcName == "ChooseDirectory")
            {

                var dlg = new FolderBrowserDialog();
                dlg.Description = TextRes.ChooseDir;
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
            else if (funcName == "GetClipboardText")
            {
                return Clipboard.GetText();
            }
            else if (funcName == "Judge")
            {
                if (funcParamArray[0] == "IsFormModal")
                {
                    return (this.Modal).ToString();
                }
                else
                {
                    return Getter.Get("Judge", funcParamArray);
                }
            }
            else if (funcName == "ValidateInput")
            {
                var dataFormat = funcParamArray.Length > 1 ? funcParamArray[1] : "";
                var ctrlArray = funcParamArray[0].GetLarrayArray(true, true);
                var len = ctrlArray.Length;
                foreach (var ctrlName in ctrlArray)
                {
                    var ctrlName1 = ctrlName.Trim().DeleteUiIdentifer();

                    var displayName = "";
                    if (len > 1)
                    {
                        var defDisplayName = _zonesItems.Find(x => x.Name == ctrlName1).DisplayName;
                        defDisplayName = defDisplayName.IsNullOrEmpty() ? ctrlName1.GetLastSeparatedString('_') : defDisplayName;
                        displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "ZoneItem", ctrlName1, _annexes, defDisplayName);
                    }

                    var ctrlValue = GetControlAttributeValue(ctrlName1 + ".v");
                    var ctrlValidateRules = _zonesItems.Find(x => x.Name == ctrlName1).ValidationRules;
                    if (ctrlValidateRules.IsNullOrEmpty())
                    {
                        continue;
                    }
                    else
                    {
                        var ruleArry = ctrlValidateRules.GetLarrayArray(true, true);
                        foreach (var rule in ruleArry)
                        {
                            var getParamArray1 = new string[] { ctrlValue, rule };
                            var rstTxt = Getter.Get("Validate", getParamArray1);

                            if (rstTxt == "LRDUNDEFINED")
                            {
                                rstTxt = ValidateEx(ctrlValue, rule);
                            }

                            var rst = rstTxt.ConvertToGeneric<UniversalResult>(true, TxtDataType.Json);
                            if (rst.Success) continue;
                            else
                            {
                                rst.Message = displayName + rst.Message;
                                return GenericHelper.ConvertToJson(rst);
                            }
                        }
                    }
                }
                return GenericHelper.ConvertToJson(new UniversalResult(true, ""));
            }
            else
            {
                var returnText = Getter.Get(funcName, funcParamArray);
                if (returnText == "LRDUNDEFINED")
                {
                    returnText = GetEx(funcName, funcParamArray);
                }
                return returnText;
            }

        }

        //*get
        private string GetImageUrl(string url, string defLocation)
        {
            if (url.IsNullOrEmpty()) return string.Empty;
            var url1 = FileHelper.GetPath(url, defLocation);
            return url1;
        }

        private Icon GetIcon(string url)
        {
            if (!url.IsNullOrEmpty() && System.IO.File.Exists(url))
            {
                var strm = File.Open(url, FileMode.Open, FileAccess.Read, FileShare.Read);
                return new Icon(strm);
            }
            return null;
        }

        //*do
        //###Do
        protected void Do(string funcName, string[] funcParamArray)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Do Error: ";
            if (funcName.IsNullOrEmpty()) return;
            //if (funcName == "DoNothing") return;
            if (funcName == "ThrowException") throw new ArgumentException(exInfo + ".Do Error: " + funcParamArray[0]);
            var returnStr = "";
            if (funcName == "Xrun")
            {
                var actionParams = IdentifierHelper.UnwrapFormularParamArray(funcParamArray);
                Xrun(actionParams);
            }
            else if (funcName == "Lrun")
            {
                var actionParams = IdentifierHelper.UnwrapFormularParamArray(funcParamArray);
                Lrun(actionParams);
            }
            //--form
            else if (funcName == "PopupMsg")
            {
                var title = funcParamArray[0];
                var content = funcParamArray[1];
                var format = PopupMessageFormFormat.Common;
                if (funcParamArray.Length > 2)
                {
                    var p3 = funcParamArray[2];
                    if (p3 == "MessageViewer")
                    {
                        format = PopupMessageFormFormat.MessageViewer;
                    }
                    else if (p3 == "RichTextViewer")
                    {
                        format = PopupMessageFormFormat.RichTextViewer;
                    }
                }
                var width = 0;
                if (funcParamArray.Length > 3)
                {
                    width = Convert.ToInt16(funcParamArray[3]);
                }
                content = GetHelper.FormatRichText(content);
                MessageHelper.Popup(title, content, format, width);
            }

            else if (funcName == "MaximizeForm")
            {
                MaximizeForm();
            }
            else if (funcName == "MinimizeForm")
            {
                MinimizeForm();
            }
            else if (funcName == "ExitApplication")
            {
                ExitApplication();
            }
            else if (funcName == "CloseForm")
            {
                CloseForm();
            }

            else if (funcName == "ReturnFalse")
            {
                BoolOutput = false;
            }
            else if (funcName == "ReturnTrue")
            {
                BoolOutput = true;
            }
            else if (funcName == "ReturnFalseAndClose")
            {
                BoolOutput = false;
                CloseForm();
            }
            else if (funcName == "ReturnTrueAndClose")
            {
                BoolOutput = true;
                CloseForm();
            }

            else if (funcName == "FadeIn")
            {
                var duration = 1000;
                if (funcParamArray.Length > 0) duration = Convert.ToInt16(funcParamArray[0]);
                FadeIn(duration);
            }
            else if (funcName == "FadeOut")
            {
                var duration = 1000;
                if (funcParamArray.Length > 0) duration = Convert.ToInt16(funcParamArray[0]);
                FadeOut(duration);
            }
            else if (funcName == "SetFormTitle")
            {
                Text = funcParamArray[0];
            }
            else if (funcName == "AppendRunningMessage")
            {
                var msg = funcParamArray[0];
                bool newLine = funcParamArray.Length > 1 ? funcParamArray[1] == "NewLine" ? true : false : false;

                RunningMessageSectionRichTextBox.Text = RunningMessageSectionRichTextBox.Text.IsNullOrEmpty() ? msg : msg + (newLine ? "\r\n" : "") + RunningMessageSectionRichTextBox.Text;
            }
            else if (funcName == "UpdateRunningMessage")
            {
                var msg = funcParamArray[0];
                RunningMessageSectionRichTextBox.Text = msg;
            }
            else if (funcName == "ClearRunningMessage")
            {
                var msg = "";
                RunningMessageSectionRichTextBox.Text = msg;
            }
            else if (funcName == "RefressProgress")
            {
                var score = Convert.ToSingle(funcParamArray[0]);
                var scoreFormat = ScoreFormat.Default;
                if (funcParamArray.Length > 1)
                {
                    scoreFormat = EnumHelper.GetByName(funcParamArray[1], scoreFormat);
                    score = score.ConvertScoreToDefaultFormat(scoreFormat);
                }
                RunningProgressSectionProgressBar.Value = Convert.ToInt32(score * 100);
            }
            else if (funcName == "ClearProgress")
            {
                RunningProgressSectionProgressBar.Value = 0;
            }
            else if (funcName == "ShowDialog")
            {
                DoEx(funcName, funcParamArray);
            }
            //--view
            else if (funcName == "SwitchView")
            {
                SwitchView(funcParamArray[0]);
            }

            //--ctrl
            else if (funcName == "ClearControls")
            {
                var ctrlNameArry = funcParamArray[0].GetLarrayArray(true, true);
                foreach (var ctrlName in ctrlNameArry)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    SetControlValue(ctrlName1, "");
                }
            }
            else if (funcName == "ClearControl")
            {
                var ctrlName1 = funcParamArray[0].DeleteUiIdentifer();
                SetControlValue(ctrlName1, "");
            }



            else if (funcName == "RefreshControlsValues")
            {
                foreach (var ctrlName in funcParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlValue(ctrlName1);
                }
            }
            else if (funcName == "RefreshControlValue")
            {
                var ctrlName1 = funcParamArray[0].DeleteUiIdentifer();
                RefreshControlValue(ctrlName1);
            }


            else if (funcName == "SetControlValue")
            {
                var ctrlName = funcParamArray[0].DeleteUiIdentifer();
                var ctrlValue = funcParamArray[1];
                SetControlValue(ctrlName, ctrlValue);
            }

            //--ctrl text
            else if (funcName == "RefreshControlsTexts")
            {
                var ctrlNameArry = funcParamArray[0].GetLarrayArray(true, true);
                foreach (var ctrlName in ctrlNameArry)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlText(ctrlName1);
                }
            }
            else if (funcName == "RefreshControlText")
            {
                var ctrlName1 = funcParamArray[0].DeleteUiIdentifer();
                RefreshControlText(ctrlName1);
            }

            else if (funcName == "SetControlText")
            {
                var ctrlName = funcParamArray[0].DeleteUiIdentifer();
                var ctrlValue = funcParamArray[1].Trim();
                SetControlText(ctrlName.Trim(), ctrlValue);
            }

            //--ctrl data
            else if (funcName == "RefreshControlsDataSources")
            {
                var ctrlNameArry = funcParamArray[0].GetLarrayArray(true, true);
                foreach (var ctrlName in ctrlNameArry)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlDataSource(ctrlName1);
                }
            }
            else if (funcName == "RefreshControlDataSource")
            {
                var ctrlName1 = funcParamArray[0].DeleteUiIdentifer();
                RefreshControlDataSource(ctrlName1);
            }
            else if (funcName == "RefreshControlsInvisibles")
            {
                var ctrlNameArry = funcParamArray[0].GetLarrayArray(true, true);
                foreach (var ctrlName in ctrlNameArry)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControlInvisible(ctrlName1);
                }
            }
            else if (funcName == "RefreshControlInvisible")
            {
                var ctrlName1 = funcParamArray[0].DeleteUiIdentifer();
                RefreshControlInvisible(ctrlName1);
            }

            else if (funcName == "SetControlVisible")
            {
                var ctrlName = funcParamArray[0].DeleteUiIdentifer();
                SetControlVisible(ctrlName, Convert.ToBoolean(funcParamArray[1]));
            }
            else if (funcName == "RefreshControlsDisableds")
            {
                var ctrlNameArry = funcParamArray[0].GetLarrayArray(true, true);
                foreach (var ctrlName in ctrlNameArry)
                {
                    RefreshControlDisabled(ctrlName.DeleteUiIdentifer());
                }
            }
            else if (funcName == "RefreshControlDisabled")
            {
                RefreshControlDisabled(funcParamArray[0].DeleteUiIdentifer());
            }
            else if (funcName == "SetControlEnabled")
            {
                var ctrlName = funcParamArray[0].DeleteUiIdentifer();
                SetControlEnabled(ctrlName, Convert.ToBoolean(funcParamArray[1].Trim()));
            }
            else if (funcName == "SetControlPadding")
            {

                var ctrlName = funcParamArray[0].DeleteUiIdentifer();
                var ctrl = GetControl(ctrlName);
                ControlBaseHelper.SetControlPadding(ctrl, "Padding:" + funcParamArray[1].Trim());
            }

            //--shell
            else if (funcName == "RefreshProcedures")
            {
                var procs = funcParamArray[0];
                var procArr = procs.GetLarrayArray(true, true);
                foreach (var proc in procArr)
                {
                    var procName = proc.DeleteShellIdentifer();
                    RefreshProcedure(procName);
                }
            }
            else if (funcName == "SetVariableValue")
            {
                var varName = funcParamArray[0].DeleteShellIdentifer();
                var varValue = funcParamArray[1].Trim();
                var procList = _procedures.FindAll(x => x.Type == (int)ProcedureType.Variable);
                ShellHelper.SetVariableValue(varName, varValue, _procedures);
            }


            //--common
            else if (funcName == "RefreshRunningStatus")
            {
                RefreshRunningStatusMessage(funcParamArray[0]);
            }
            else if (funcName == "RefreshControls") //no use
            {
                foreach (var ctrlName in funcParamArray)
                {
                    var ctrlName1 = ctrlName.DeleteUiIdentifer();
                    RefreshControl(ctrlName1);
                }
            }
            else if (funcName == "Sleep")
            {
                var duration = 1000;
                if (funcParamArray.Length > 0) duration = Convert.ToInt16(funcParamArray[0]);
                //this.Refresh(); // for asyn, asyn waited, managed thread will popup error
                Thread.Sleep(duration);
                //this.Refresh(); 
            }
            else if (funcName == "UpdateCed")
            {
                DoEx(funcName, funcParamArray);
            }
            else//*to do by doer
            {

                returnStr = Doer.Do(funcName, funcParamArray);
                if (returnStr == "LRDUNDEFINED")
                {
                    //if (!_formInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + exInfo + "Undefined; funcName=" + funcName);
                    DoEx(funcName, funcParamArray);
                }
            }

            if (funcName == "ExecCmds" | funcName == "ExecCmd" | funcName == "RunBat" | funcName == "RunPython")
            {
                var interceptOutput = funcParamArray.Length > 3 ? funcParamArray[3].JudgeJudgementFlag() : false;
                var saveOutput = interceptOutput;
                if (saveOutput)
                {
                    var title = funcName;
                    for (int i = 0; i < funcParamArray.Length; i++)
                    {
                        title = title + "-" + funcParamArray[i];
                    }
                    title = title + "-";
                    if (title.Length > 64) title = title.Substring(0, 64);
                    title = title.ToLegalFileName();
                    var content = GetHelper.FormatRichText(returnStr);
                    var outputDir = _appOutputDir + "\\ProcessLog\\Winform";
                    //var outputDir = "\\\\127.0.0.1" + "\\" + "Share\\Cmds\\Winform"; //ok

                    DirectoryHelper.CreateDirectory(outputDir);
                    var outputPath = outputDir + "\\" + title.ToUniqueStringByNow("-") + ".txt";
                    File.WriteAllText(outputPath, content);
                }
            }

        }

        //*xrun
        //###Xrun
        private void Xrun(string actParams)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Xrun Error: ";
            var elementNameArr = actParams.GetLarrayArray(true, true);
            foreach (var elementName in elementNameArr)
            {

                var action = "";
                if (IdentifierHelper.IsShellElementDataFormat(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteShellIdentifer(elementName);
                    var item = _procedures.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "ShellElementName={0} des not exist!", elementName));
                    if (item.Expression.IsNullOrEmpty()) continue;
                    action = item.Expression;
                }
                else if (IdentifierHelper.IsUiElementDataFormat(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);
                    var item = _zonesItems.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "UiElementName={0} des not exist!", elementName));
                    if (item.Action.IsNullOrEmpty()) continue;
                    action = item.Action;
                }

                var actionNameAndParamsArray = action.GetFormularParamArray(true, false);
                var funcName = actionNameAndParamsArray[0].Trim();
                var funcParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    funcParamArray[i] = GetText(actionNameAndParamsArray[i + 1].Trim(), GetTextType.UiItem);//max possible
                }

                if (funcName == "Crun")
                {
                    var con = funcParamArray[0];
                    con = GetText(con, GetTextType.UiItem); //max possible
                    var action1 = "";
                    if (con.JudgeJudgementFlag())
                    {
                        action1 = funcParamArray[1];
                    }
                    else
                    {
                        if (funcParamArray.Length > 2)
                            action1 = funcParamArray[2];
                        else action1 = "DoNothing";
                    }
                    var transParams = ParseAction(action1, TransactFrom.FromZoneUi); ////max possible
                    funcName = transParams.Name;
                    funcParamArray = transParams.Params;
                }

                try
                {
                    Do(funcName, funcParamArray);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo + "elementName =" + elementName + "; " + " action = " + action + "; " + ex.Message);
                }

            }
        }

        //###Lrun
        private void Lrun(string funcParams)
        {
            if (!IdentifierHelper.ContainsLrunIdentifer(funcParams)) return;
            var strArray = funcParams.GetFormularParamArray(true, false);
            var funcName = strArray[0];
            var funcParamsAndReplaceVals = strArray[1];
            var funcParamsAndReplaceValsArry = funcParamsAndReplaceVals.GetFormularParamArray(true, false);
            var transParams = funcParamsAndReplaceValsArry[0];
            var transParamsArry = transParams.GetFormularParamArray(true, false);
            var ReplaceVals = funcParamsAndReplaceValsArry[1];
            var ReplaceValsArry = ReplaceVals.GetLarrayArray(true, false);

            foreach (var lrunParam in ReplaceValsArry)
            {
                foreach (var param in transParamsArry)
                {
                    if (IdentifierHelper.ContainsLrunIdentifer(param))
                        param.Replace(IdentifierHelper.LrunIdentifer, lrunParam);
                }
                Do(funcName, transParamsArry);
            }
        }

        //###MinimizeForm
        private void MinimizeForm()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //###MaximizeForm
        private void MaximizeForm()
        {
            this.WindowState = FormWindowState.Maximized;
        }

        //###CloseForm
        private void CloseForm()
        {
            Close();
        }

        //####ExitApplication
        protected void ExitApplication()
        {
            Close();
            Application.Exit();
        }

        //###FadeIn
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

        //###FadeOut
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

            //Close();
        }

        //*resolve
        //###ResolveConstants
        protected string ResolveConstants(string text)
        {
            if (text.IsNullOrEmpty()) return string.Empty;
            if (!text.Contains("%")) return text;

            var toBeRplStr = "";
            toBeRplStr = "%ArchCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ArchitectureCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ArchitectureName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ArchitectureVersion;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%OrgCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.OrganizationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgShortName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.OrganizationShortName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.OrganizationName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ApplicationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ApplicationVersion;
                return Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FuncCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FormCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //-dir
            toBeRplStr = "%RootDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _startupDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%CfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _cfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            //--golbal dir
            toBeRplStr = "%GlbCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbSharedDir%".ToLower(); //tobecancelled
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbSharedDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbUiDir%".ToLower(); //tobecancelled
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //--app dir
            toBeRplStr = "%AppCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppSharedDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appSharedDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppUiDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%PortalsDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _portalsDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%ViewsDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _viewsDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%ZonesDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _zonesDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //-data dir
            toBeRplStr = "%DataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _dataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%sysDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _sysDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%sysAppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _sysAppDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%myDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _myDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%myAppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _myAppDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            //-lib dir
            toBeRplStr = "%LibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _libDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbLibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbLibDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppLibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appLibDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //-tmp dir
            toBeRplStr = "%AppOutputDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appOutputDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            if (!text.Contains("%"))
            {
                return text;
            }
            else
            {
                var retStr = Resolver.ResolveConstants(text);
                if (!retStr.Contains("%"))
                {
                    return retStr;
                }
                else
                {
                    var retStr1 = ResolveConstantsEx(text);
                    if (!retStr1.Contains("%")) return retStr1;
                    else
                        throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveConstants Error: There are constants not defined! str= " + retStr1);

                }
            }
        }

        //###ResolveStringByRefProcessVariables
        private string ResolveStringByRefShellVariables(string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return "";
                if (str.IsShellElementDataFormat())
                {
                    var procedures = _procedures.Where(x => x.Type == (int)ProcedureType.Args | x.Type == (int)ProcedureType.Variable).ToList();
                    var annexes = _annexes.Where(x => x.ClassName == "Procedure").ToList();
                    str = ShellHelper.ResolveStringByRefVariables(str, _formInitParamSet.SupportMultiLanguages, procedures, annexes);
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefShellVariables Error: " + ex.Message);
            }
        }

        private string ResolveStringByRefControls(string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return "";
                if (str.IsUiElementDataFormat())
                {
                    str = ResolveStringByRefControls(str, _zonesItems);
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefControls Error: " + ex.Message);
            }
        }

        //###ResolveStringByRefControls
        private string ResolveStringByRefControls(string str, List<ZoneItem> zoneItems)
        {
            try
            {
                str.CheckUiElementDataFormat();
                var strArray = str.Split(IdentifierHelper.UiIdentifer.ToChar());
                var i = 1;
                var txt = strArray[i];
                if (txt.ToLower().EndsWith(".v")// | txt.ToLower().EndsWith(".val")
                    | txt.ToLower().EndsWith(".t") //| txt.ToLower().EndsWith(".txt")
                    )
                    strArray[i] = GetControlAttributeValue(txt);
                else
                    strArray[i] = IdentifierHelper.UiIdentifer + strArray[i] + IdentifierHelper.UiIdentifer;
                return strArray[i];
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefControls Error: str='" + str + "'; " + ex.Message);
            }
        }

        private string ResolveCentralExchangeData(string str)
        {
            try
            {
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveCenteExchangeData Error: str='" + str + "'; " + ex.Message);
            }
        }


        //*virtual
        //####ResolveConstantsEx
        protected virtual string ResolveConstantsEx(string text)
        {
            return text;
        }

        protected virtual string ValidateEx(string code, string rule)
        {
            return string.Empty;
        }

        protected virtual string GetEx(string funName, string[] paramArray)
        {
            return string.Empty;
        }
        //####DoEx
        protected virtual void DoEx(string funcName, string[] funcParamArray)
        {
        }


        //*end

    }
}
