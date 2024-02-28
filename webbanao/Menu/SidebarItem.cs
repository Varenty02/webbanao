using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace webbanao.Menu {

    public enum SidebarItemType
    {
        Divider,
        Heading,
        NavItem
    }


    public class SidebarItem
    {
        public string Title { get; set; }

        public bool IsActive { get; set; }

        public SidebarItemType Type { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Area { get; set; }

        public string AwesomeIcon { get; set; } // fas fa-fw fa-cog


        public List<SidebarItem> Items { get; set; }

        public string GetLink(IUrlHelper urlHelper)
        {
            return urlHelper.Action(Action, Controller, new { area = Area});
        }
        //menu-is-opening menu-open -->active
        public string RenderHtml(IUrlHelper urlHelper)
        {
            var html = new StringBuilder();

            if (Type == SidebarItemType.Divider)
            {
                html.Append("<hr class=\"sidebar-divider my-2\">");
            }
            else if (Type == SidebarItemType.Heading)
            {
                html.Append(@$"<div class=""sidebar-heading"">
                                {Title}
                               </div>");
            }
            else if (Type == SidebarItemType.NavItem)
            {
                if (Items == null)
                {
                    var url = GetLink(urlHelper);
                    var icon = (AwesomeIcon != null) ? 
                                $"<i class=\"{AwesomeIcon}\"></i>":
                                "";
                    var cssClass = "nav-item";
                    var cssClassActive = "";
                    if (IsActive) cssClassActive= " active";        


                    html.Append(@$"
                        <li class=""{cssClass}"">
                            <a class=""nav-link {cssClassActive}"" href=""{url}"">
                                {icon}
                                <p>{Title}</p></a>
                         </li>                    
                    ");

                }
                else
                {
                    // Items != null
                    var classLiShow = "";
                    var classItemAtive = "";
                    if (IsActive)
                    {
                        classLiShow = " menu-open";
                        classItemAtive = "active";
                    }
                    var icon = (AwesomeIcon != null) ? 
                                $"<i class=\"{AwesomeIcon}\"></i>":
                                ""; 

                    var itemMenu = "";

                    foreach (var item in Items) 
                    {
                        var urlItem = item.GetLink(urlHelper);
                        var cssItem = "nav-link";
                        if (item.IsActive) cssItem += " active";
                        itemMenu  += $"<li class=\"nav-item\"><a class=\"{cssItem}\" href=\"{urlItem}\"><p>{item.Title}</p></a></li>";
                    }
                    html.Append(@$"
                    
                        <li class=""nav-item {classLiShow}"">
                            <a class=""nav-link {classItemAtive}"" href=""#"">
                                {icon}
                                <p>{Title}<i class=""right fas fa-angle-left""></i></p>
                            </a>

                            <ul class=""nav nav-treeview"">
                                    {itemMenu}
                            </ul>

                        </li>                    
                    
                    ");

                }
            } 

            return html.ToString();
        }



    }
}