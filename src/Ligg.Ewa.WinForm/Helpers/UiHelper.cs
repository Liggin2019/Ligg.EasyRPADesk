using System;
using System.Collections.Generic;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;
using Ligg.Infrastructure.Utility.FileWrap;
using Ligg.EasyWinApp.Parser.Helpers;

namespace Ligg.EasyWinApp.WinForm.Helpers
{
    public static class UiHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*layout
        public static void SetLayoutElementTypes(LayoutElement layoutElement)
        {
            try
            {
                layoutElement.Type = EnumHelper.GetIdByName<LayoutElementType>(layoutElement.TypeName);
                layoutElement.DockType = EnumHelper.GetIdByName<ControlDockType>(layoutElement.DockTypeName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".SetLayoutElementTypess Error: " + ex.Message);
            }
        }

        //*menu
        internal static void CheckMenuFeatures(List<MenuFeature> menuFeatures)
        {
            var exInfo = "\n >> " + TypeName + ".CheckMenuFeatures Error: ";

            //ContainerName can't be empty for hor/ver menu?
            if (menuFeatures.FindAll(x => x.MenuType == (int)MenuType.Horizontal).Count > 1
                | menuFeatures.FindAll(x => x.MenuType == (int)MenuType.Vertical).Count > 1
                | menuFeatures.FindAll(x => x.MenuType == (int)MenuType.Nested).Count > 1)
            {
                throw new ArgumentException(exInfo + "You can only set only one Horizontal or Vertical and Nested menu for each" + "!");
            }

            foreach (var menuFeature in menuFeatures)
            {
                if (menuFeature.MenuTypeName.IsNullOrEmpty())
                    throw new ArgumentException(exInfo + "the MenuTypeName can't be empty ! menuFeature.Id={0}".FormatWith(menuFeature.Id));

                if (menuFeature.MenuType == 0)
                    throw new ArgumentException(exInfo + "the MenuTypeName is not correct ! menuFeature.Id={0},menuFeature.MenuTypeName".FormatWith(menuFeature.Id, menuFeature.MenuTypeName));

                if (menuFeatures.FindAll(x => x.Location == menuFeature.Location).Count > 1)
                {
                    throw new ArgumentException(exInfo + "Can't load menu from same location for more than one time! menuFeature.Id={0}, Location={1} !".FormatWith(menuFeature.Id, menuFeature.Location));
                }

                if (menuFeatures.FindAll(x => x.Container == menuFeature.Container && !x.Container.IsNullOrEmpty()).Count > 1)
                {
                    throw new ArgumentException(exInfo + "Can't use same ContainerName for more than one time! menuFeature.Id={0}, Container={1}!".FormatWith(menuFeature.Id, menuFeature.Container));
                }

                if (menuFeature.MenuType == (int)MenuType.Nested)
                {
                    if (menuFeatures.FindAll(x => x.MenuType == (int)MenuType.Horizontal | x.MenuType == (int)MenuType.Vertical).Count > 0)
                        throw new ArgumentException(exInfo + "Nested menu can't be used  with Horizontal or Vertical menu simuitaneously! nestedMenuFeature.Id={0}!".FormatWith(menuFeature.Id));
                }

            }
        }

        internal static void CheckMenuItems(int menuType, List<LayoutElement> menuItems)
        {
            var exInfo = "\n >> " + TypeName + ".CheckMenuItems Error: ";
            foreach (var menuItem in menuItems)
            {
                UiElementHelper.CheckName(menuItem.Name);
                if (menuItem.Type == (int)LayoutElementType.MenuItemContainerArea)
                {
                    if ((menuType == (int)MenuType.Nested | menuType == (int)MenuType.ToolBar) & (menuItem.ControlTypeName != "ToolStrip" && menuItem.ControlTypeName != "Panel"))
                    {
                        throw new ArgumentException(exInfo + "For Nested or ToolBar menu, the ControlTypeName of MenuItemContainerArea only can be \"ToolStrip\" or \"Panel\"! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                    }

                    if ((menuType == (int)MenuType.Horizontal) & (menuItem.ControlTypeName != "MenuStrip"))
                    {
                        throw new ArgumentException(exInfo + "For Horizontal menu, the ControlTypeName of MenuItemContainerArea only can be \"MenuStrip\"! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                    }
                }


                if (menuItem.Id < 1)
                {
                    throw new ArgumentException(exInfo + "menuItem Id can't be less than 1! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                }

                if (menuItems.FindAll(x => x.Id == menuItem.Id).Count > 1)
                {
                    throw new ArgumentException(exInfo + "menuItem can't have duplicated Id! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                }

                if (menuItems.FindAll(x => x.Name == menuItem.Name).Count > 1)
                {
                    throw new ArgumentException(exInfo + "menuItem can't have duplicated name! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                }

                if (menuItem.DockType < 0 || menuItem.DockType > 5)
                {
                    throw new ArgumentException(exInfo + "menuItem's DockType can't be less than 0 or greater than 5! area.Id=" + menuItem.Id + ", area.Name=" + menuItem.Name);
                }

                if (menuType == (int)MenuType.Nested | menuType == (int)MenuType.ToolBar)
                {
                    if (menuItem.Type != (int)LayoutElementType.MenuItem & menuItem.Type != (int)LayoutElementType.MenuItemContainerArea)
                    {
                        if (menuItem.Container.IsNullOrEmpty())
                        {
                            throw new ArgumentException(exInfo + "For Nested or ToolBar menu, only MenuItem or MenuItemContainerArea are permitted! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                        }
                    }
                    if (menuItem.Type == (int)LayoutElementType.MenuItemContainerArea)
                    {
                        var subMenuItems = menuItems.FindAll(x => x.Container == menuItem.Name);
                        foreach (var subMenuItem in subMenuItems)
                        {
                            if (subMenuItem.Type != (int)LayoutElementType.MenuItem)
                            {
                                throw new ArgumentException(exInfo + "For Nested or ToolBar menu, only MenuItem type can be as child element for MenuItemContainerArea ! MenuItemContainerArea.Id=" + menuItem.Id + ", MenuItemContainerArea.Name=" + menuItem.Name
                                                            + ", subMenuItem.Id=" + subMenuItem.Id + ", subMenuItem.Name=" + subMenuItem.Name);
                            }
                        }
                    }
                }

                else if (menuType == (int)MenuType.Horizontal | menuType == (int)MenuType.Vertical)
                {

                    if (menuItem.Type != (int)LayoutElementType.MenuItem)
                    {
                        throw new ArgumentException(exInfo + "For Horizontal/Vertical menu type, only MenuItem,TransactionOnlyItem are permitted! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                    }
                }

            }
        }
        internal static List<SubMenuItem> GetSubMenuItems(string subMenuDir)
        {
            try
            {
                var subMenuUiCfgFile = subMenuDir + "\\Ui";
                if (!ConfigFileHelper.IsFileExisting(subMenuUiCfgFile)) return null;
                var cfgFileMgr = new ConfigFileManager(subMenuUiCfgFile);
                var subMenuItems = FunctionHelper.GetGenericFromCfgFile<List<LayoutElement>>(subMenuUiCfgFile, false) ?? new List<LayoutElement>();

                foreach (var subMenuItem in subMenuItems)
                {
                    //check
                    UiElementHelper.CheckName(subMenuItem.Name);
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
                    subMenuItem.InvalidFlag = subMenuItem.InvalidFlag.GetJudgementFlag();
                    subMenuItem.InvisibleFlag = subMenuItem.InvisibleFlag.GetJudgementFlag();
                    subMenuItem.DisabledFlag = subMenuItem.DisabledFlag.GetJudgementFlag();
                    subMenuItem.WriteIntoLogFlag = subMenuItem.WriteIntoLogFlag.GetJudgementFlag();
                    subMenuItem.ShowRunningStatusFlag = subMenuItem.ShowRunningStatusFlag.GetJudgementFlag();
                }

                var subMenuItems1 = subMenuItems.FindAll(x => x.InvalidFlag.ToLower() == "false");
                var subMenuItems2 = new List<SubMenuItem>();
                foreach (var subMenuItem in subMenuItems1)
                {
                    var subMenuItem2 = new SubMenuItem();
                    subMenuItem.ExecModeFlag = string.IsNullOrEmpty(subMenuItem.ExecModeFlag) ? "" : subMenuItem.ExecModeFlag;
                    subMenuItem2.Id = subMenuItem.Id.ToString();
                    subMenuItem2.ParentId = subMenuItem.ParentId.ToString();
                    subMenuItem2.Name = subMenuItem.Name ?? "";
                    subMenuItem2.DisplayName = subMenuItem.DisplayName ?? "";
                    subMenuItem2.Action = subMenuItem.Action ?? "";
                    subMenuItem2.ControlTypeName = subMenuItem.ControlTypeName ?? "";
                    subMenuItem2.ImageUrl = subMenuItem.ImageUrl ?? "";
                    subMenuItems2.Add(subMenuItem2);

                }
                return subMenuItems2;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".SetSubMenuItems Error: " + ex.Message);
            }
        }


        //*view
        public static void CheckViewFeatures(List<ViewFeature> viewFeatures)
        {
            var exInfo = "\n>> " + TypeName + ".CheckViewFeatures Error: ";

            if (viewFeatures.FindAll(x => x.IsPublic).Count > 1)
            {
                throw new ArgumentException(exInfo + "Public View qty should not more than 1 " + "!");
            }
            if (viewFeatures.FindAll(x => x.IsPublic).Count == 0)
            {
                throw new ArgumentException(exInfo + "only 1 public view is permited " + "!");
            }

            foreach (var viewFeature in viewFeatures)
            {

                if (viewFeatures.FindAll(x => x.Name == viewFeature.Name).Count > 1)
                {
                    throw new ArgumentException(exInfo + "View can't have duplicated name! viewName=" + viewFeature.Name + "!");
                }
            }

        }

        public static void CheckViewFeaturesDefaultView(List<ViewFeature> viewFeatures)
        {
            var exInfo = "\n>> " + TypeName + ".CheckViewFeaturesDefaultView Error: ";

            if (viewFeatures.FindAll(x => x.IsDefault).Count > 1)
            {
                throw new ArgumentException(exInfo + "Default View qty should not more than 1 " + "!");
            }

            if (viewFeatures.FindAll(x => x.IsDefault).Count == 0)
            {
                throw new ArgumentException(exInfo + "Only 1 default view is permited " + "!");
            }

            if (viewFeatures.FindAll(x => x.IsDefault & x.IsPublic).Count >0)
            {
                throw new ArgumentException(exInfo + "Public view can't  be set as default view " + "!");
            }

        }

        public static List<ViewFeature> SetViewFeatures(List<ViewFeature> viewFeatures)
        {
            foreach (var viewFeature in viewFeatures)
            {
                viewFeature.InvalidFlag = viewFeature.InvalidFlag.GetJudgementFlag();
            }
            var viewFeatures1 = viewFeatures.FindAll(x => (x.InvalidFlag.ToLower() == "false"));
            return viewFeatures1;

        }


        public static void CheckViewItems(string viewName, List<LayoutElement> viewItems)
        {
            var exInfo = "\n>> " + TypeName + ".CheckViewItems Error: ";

            foreach (var viewItem in viewItems)
            {
                UiElementHelper.CheckName(viewItem.Name);
                if (viewItem.Type == (int)LayoutElementType.Zone)
                {
                    if (FileHelper.IsAbsolutePath(viewItem.Location)) throw new ArgumentException("ZoneLocation can't be an absolute path! ");
                    if (viewItem.Location.StartsWith("~")) throw new ArgumentException(exInfo + "ZoneLocation can't starts with '~'! ");
                }

                if (viewItem.Type == (int)LayoutElementType.ContentArea | viewItem.Type == (int)LayoutElementType.Zone)
                {
                    if (viewItem.Container.IsNullOrEmpty()) throw new ArgumentException(exInfo + "ContentArea or Zone Item container can't be empty!  viewName=" + viewName + ", viewItem.Name=" + viewItem.Name);
                }

                if (viewItem.Type == (int)LayoutElementType.ContentArea)
                {
                    if (viewItem.DockType < 1 || viewItem.DockType > 5)
                    {
                        throw new ArgumentException(exInfo + "Content area's DockTypeName should be ‘Top’, ‘Right’, ‘Bottom’, ‘Left' or 'Fill’! viewName=" + viewName + ", area.Name=" + viewItem.Name);
                    }

                    var sameAreaItems = viewItems.FindAll(x => x.Container == viewItem.Name);
                    foreach (var subItem in sameAreaItems)
                    {
                        if (sameAreaItems.FindAll(x => x.Name == subItem.Name).Count > 1)
                        {
                            throw new ArgumentException(exInfo + "Content area can't have duplicated contained item name! viewName=" + viewName + ",area.Name=" + viewItem.Name + ", subItem.Name=" + subItem.Name);
                        }
                    }
                }


                if (viewItems.FindAll(x => x.Name == viewItem.Name && ((viewItem.Type == (int)LayoutElementType.FollowingTransaction | viewItem.Type == (int)LayoutElementType.ContentArea))).Count > 1)
                {
                    throw new ArgumentException(exInfo + "View Item can't have duplicated name! viewName=" + viewName + ", viewItem.Name=" + viewItem.Name);
                }

                if (viewItem.Type == (int)LayoutElementType.Zone)
                {
                    if (FileHelper.IsAbsolutePath(viewItem.Location)) throw new ArgumentException("ZoneLocation can't be an absolute path! ");
                    if (viewItem.Location.StartsWith("~")) throw new ArgumentException(exInfo + "ZoneLocation can't starts with '~'! ");
                }
            }
        }

        //*zone
        public static void SetZoneItemType(ZoneItem zoneItem)
        {
            try
            {
                zoneItem.Type = EnumHelper.GetIdByName<ZoneItemType>(zoneItem.TypeName);
                zoneItem.DockType = EnumHelper.GetIdByName<ControlDockType>(zoneItem.DockTypeName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".SetZoneItemType Error: " + ex.Message);
            }
        }
        public static void CheckZoneItems(string zoneName, List<ZoneItem> zoneItems)
        {
            var exInfo = "\n>> " + TypeName + ".CheckZoneItems Error: ";

            foreach (var item in zoneItems)
            {
                UiElementHelper.CheckName(item.Name);
                if (item.Name.IsNullOrEmpty())
                {
                    throw new ArgumentException(exInfo + "ZoneItem name can't be empty! zoneName=" + zoneName + ", item.Name=" + item.Name);
                }

                if (zoneItems.FindAll(x => x.Name == item.Name).Count > 1)
                {
                    throw new ArgumentException(exInfo + "ZoneItem can't have duplicated name! zoneName=" + zoneName + ", item.Name=" + item.Name);
                }

                if (item.Type == (int)ZoneItemType.ControlContainer)
                {
                    if (item.ControlTypeName != ControlType.Panel.ToString() & item.ControlTypeName != ControlType.ContainerPanel.ToString() & item.ControlTypeName != ControlType.ShadowPanel.ToString())
                        throw new ArgumentException(exInfo + "ControlContainer's ControlType Error ! zoneName=" + zoneName + ", item.Name=" + item.Name +
                            "; ControlType shoulb be in " + ControlType.Panel.ToString() + ", " + ControlType.ContainerPanel.ToString() + ", " + ControlType.ShadowPanel.ToString());
                }

                if (item.Type == (int)ZoneItemType.SubControl)
                {
                    if (item.Container.IsNullOrEmpty())
                        throw new ArgumentException(exInfo + "SubControl's Container can't be empty! zoneName=" + zoneName + ", item.Name=" + item.Name);

                    if (zoneItems.FindAll(x => x.Name == item.Container).Count == 0)
                        throw new ArgumentException(exInfo + "SubControl's Container doesn't exist! zoneName=" + zoneName + ", item.Name=" + item.Name);
                }


            }
        }


        //*end




    }

}