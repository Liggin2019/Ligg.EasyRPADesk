using System;
using System.Linq;
using System.Collections.Generic;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Helpers
{
    public static class UiHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

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
                throw new ArgumentException("\n>> " + _typeFullName + ".SetLayoutElementTypess Error: " + ex.Message);
            }
        }

        //*menu
        internal static void CheckMenuFeatures(List<MenuFeature> menuFeatures)
        {
            var exInfo = "\n >> " + _typeFullName + ".CheckMenuFeatures Error: ";

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
            var exInfo = "\n >> " + _typeFullName + ".CheckMenuItems Error: ";
            var menuControlNames = EnumHelper.GetNames<MenuControlType>();
            foreach (var menuItem in menuItems)
            {
                CommonHelper.CheckName(menuItem.Name);

                if (menuItem.Type == (int)LayoutElementType.MenuArea)
                {
                    if ((menuType == (int)MenuType.Nested | menuType == (int)MenuType.ToolBar) & (menuItem.ControlTypeName != "ToolStrip" && menuItem.ControlTypeName != "Panel"))
                    {
                        throw new ArgumentException(exInfo + "For Nested or ToolBar menu, the ControlTypeName of MenuArea only can be \"ToolStrip\" or \"Panel\"! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                    }

                    if ((menuType == (int)MenuType.Horizontal) & (menuItem.ControlTypeName != "MenuStrip"))
                    {
                        throw new ArgumentException(exInfo + "For Horizontal menu, the ControlTypeName of MenuArea only can be \"MenuStrip\"! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
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
                    if (menuItem.Type != (int)LayoutElementType.MenuItem & menuItem.Type != (int)LayoutElementType.MenuArea)
                    {
                        if (menuItem.Container.IsNullOrEmpty())
                        {
                            throw new ArgumentException(exInfo + "For Nested or ToolBar menu, only MenuItem or MenuArea are permitted! menuItem.Id=" + menuItem.Id + ", menuItem.Name=" + menuItem.Name);
                        }
                    }
                    if (menuItem.Type == (int)LayoutElementType.MenuArea)
                    {
                        if (!menuItem.View.IsNullOrEmpty())
                            throw new ArgumentException(exInfo + "For Nested or ToolBar menu, MenuArea should not have View! MenuArea.Id=" + menuItem.Id + ", MenuArea.Name=" + menuItem.Name);
                        var subMenuItems = menuItems.FindAll(x => x.Container == menuItem.Name);
                        if (subMenuItems.Count == 0)
                            throw new ArgumentException(exInfo + "For Nested or ToolBar menu, MenuArea should contain subMenuItems! MenuArea.Id=" + menuItem.Id + ", MenuArea.Name=" + menuItem.Name);
                        foreach (var subMenuItem in subMenuItems)
                        {
                            if (subMenuItem.Type != (int)LayoutElementType.MenuItem)
                            {
                                throw new ArgumentException(exInfo + "For Nested or ToolBar menu, only MenuItem type can be as child element for MenuArea ! MenuArea.Id=" + menuItem.Id + ", MenuArea.Name=" + menuItem.Name
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
                if (menuType != (int)MenuType.Vertical)
                {
                    var isLegalControlTypeName = menuControlNames.Contains(menuItem.ControlTypeName);
                    if (!isLegalControlTypeName)
                        throw new ArgumentException(exInfo + "Control's ControlType is not valid ! ControlTypeName= " + menuItem.ControlTypeName + ", item.Name=" + menuItem.Name + ", ControlTypeName should be in " + menuControlNames.Unwrap(", "));

                }

            }
        }

        //*view
        public static void CheckViewFeatures(List<ViewFeature> viewFeatures)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckViewFeatures Error: ";

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
            var exInfo = "\n>> " + _typeFullName + ".CheckViewFeaturesDefaultView Error: ";

            if (viewFeatures.FindAll(x => x.IsDefault).Count > 1)
            {
                throw new ArgumentException(exInfo + "Default View qty should not more than 1 " + "!");
            }

            if (viewFeatures.FindAll(x => x.IsDefault).Count == 0)
            {
                throw new ArgumentException(exInfo + "Only 1 default view is permited " + "!");
            }

            if (viewFeatures.FindAll(x => x.IsDefault & x.IsPublic).Count > 0)
            {
                throw new ArgumentException(exInfo + "Public view can't  be set as default view " + "!");
            }

        }

        public static void CheckViewItems(string viewName, List<LayoutElement> viewItems)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckViewItems Error: ";

            foreach (var viewItem in viewItems)
            {
                CommonHelper.CheckName(viewItem.Name);
                if (viewItem.Type == (int)LayoutElementType.Zone)
                {
                    if (viewItem.Location.IsLegalAbsolutePath()) throw new ArgumentException("ZoneLocation can't be an absolute path! ");
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


                if (viewItems.FindAll(x => x.Name == viewItem.Name && ((viewItem.Type == (int)LayoutElementType.ActionWatcher | viewItem.Type == (int)LayoutElementType.ContentArea))).Count > 1)
                {
                    throw new ArgumentException(exInfo + "View Item can't have duplicated name! viewName=" + viewName + ", viewItem.Name=" + viewItem.Name);
                }

                if (viewItem.Type == (int)LayoutElementType.Zone)
                {
                    if (viewItem.Location.IsLegalAbsolutePath()) throw new ArgumentException("ZoneLocation can't be an absolute path! ");
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
                throw new ArgumentException("\n>> " + _typeFullName + ".SetZoneItemType Error: " + ex.Message);
            }
        }
        public static void CheckZoneItems(string zoneName, List<ZoneItem> zoneItems)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckZoneItems Error: ";
            var zoneControlNames = EnumHelper.GetNames<ZoneControlType>();
            foreach (var item in zoneItems)
            {
                CommonHelper.CheckName(item.Name);
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
                    if (item.ControlTypeName != ZoneControlType.Panel.ToString() & item.ControlTypeName != MenuControlType.ContainerPanel.ToString() & item.ControlTypeName != ZoneControlType.ShadowPanel.ToString())
                        throw new ArgumentException(exInfo + "ControlContainer's ControlType Error ! zoneName=" + zoneName + ", item.Name=" + item.Name +
                            "; ControlType shoulb be in " + ZoneControlType.Panel.ToString() + ", " + MenuControlType.ContainerPanel.ToString() + ", " + ZoneControlType.ShadowPanel.ToString());
                }

                if (item.Type == (int)ZoneItemType.SubControl)
                {
                    if (item.Container.IsNullOrEmpty())
                        throw new ArgumentException(exInfo + "SubControl's Container can't be empty! zoneName=" + zoneName + ", item.Name=" + item.Name);

                    if (zoneItems.FindAll(x => x.Name == item.Container).Count == 0)
                        throw new ArgumentException(exInfo + "SubControl's Container doesn't exist! zoneName=" + zoneName + ", item.Name=" + item.Name);
                }

                if (item.Type == (int)ZoneItemType.ControlContainer | item.Type == (int)ZoneItemType.Control | item.Type == (int)ZoneItemType.SubControl)
                {
                    var isLegalControlTypeName = zoneControlNames.Contains(item.ControlTypeName);
                    if (!isLegalControlTypeName)
                        throw new ArgumentException(exInfo + "Control's ControlType is not valid ! ControlTypeName= " + item.ControlTypeName + ", item.Name=" + item.Name + ", ControlTypeName should be in " + zoneControlNames.Unwrap(", "));

                }
            }
        }


        //*end




    }

}