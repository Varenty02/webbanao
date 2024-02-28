using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace webbanao.Menu {
    public class AdminSidebarService
    {
        private readonly IUrlHelper UrlHelper;
        public List<SidebarItem> Items { get; set; } = new List<SidebarItem>();


        public AdminSidebarService(IUrlHelperFactory factory, IActionContextAccessor action)
        {
            
            UrlHelper = factory.GetUrlHelper(action.ActionContext);
            
        
            Items.Add(new SidebarItem() { 
                    Type = SidebarItemType.NavItem,
                    Controller = "DbManage",
                    Action = "Index", 
                    Area = "Database",
                    Title = "Quản lý Database",
                    AwesomeIcon = "fas fa-database"
                });
            Items.Add(new SidebarItem() { 
                    Type = SidebarItemType.NavItem,
                    Controller = "Contact",
                    Action = "Index", 
                    Area = "Contact",
                    Title = "Quản lý liên hệ",
                    AwesomeIcon = "far fa-address-card"
                });

            Items.Add(new SidebarItem() { 
                    Type = SidebarItemType.NavItem,
                    Title = "Phân quyền & thành viên",
                    AwesomeIcon = "far fa-folder",
                    Items = new List<SidebarItem>() {
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "Role",
                                Action = "Index", 
                                Area = "Identity",
                                Title = "Các vai trò (role)"                        
                        },
                         new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "Role",
                                Action = "Create", 
                                Area = "Identity",
                                Title = "Tạo vai trò mới"                        
                        },
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "User",
                                Action = "Index", 
                                Area = "Identity",
                                Title = "Danh sách thành viên"                        
                        },
                    },
                });
                
                Items.Add(new SidebarItem() { 
                    Type = SidebarItemType.NavItem,
                    Title = "Quản lý bài viết",
                    AwesomeIcon = "far fa-folder",
                    Items = new List<SidebarItem>() {
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "Category",
                                Action = "Index", 
                                Area = "Blog",
                                Title = "Các chuyên mục"                        
                        },
                         new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "Category",
                                Action = "Create", 
                                Area = "Blog",
                                Title = "Tạo chuyên mục"                        
                        },   
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "Posts",
                                Action = "Index", 
                                Area = "Blog",
                                Title = "Các bài viết"                        
                        }, 
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "Posts",
                                Action = "Create", 
                                Area = "Blog",
                                Title = "Tạo bài viết"                        
                        },                                   
                    },
                });
                Items.Add(new SidebarItem() { 
                    Type = SidebarItemType.NavItem,
                    Title = "Quản lý sản phẩm",
                    AwesomeIcon = "far fa-folder",
                    Items = new List<SidebarItem>() {
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "CategoryProducts",
                                Action = "Index", 
                                Area = "Product",
                                Title = "Các chuyên mục"                        
                        },
                         new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "CategoryProducts",
                                Action = "Create", 
                                Area = "Product",
                                Title = "Tạo chuyên mục"                        
                        },   
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "ProductModels",
                                Action = "Index", 
                                Area = "Product",
                                Title = "Các sản phẩm"                        
                        }, 
                        new SidebarItem() { 
                                Type = SidebarItemType.NavItem,
                                Controller = "ProductModels",
                                Action = "Create", 
                                Area = "Product",
                                Title = "Tạo sản phẩm"                        
                        },                                   
                    },
                });
            Items.Add(new SidebarItem()
            {
                Type = SidebarItemType.NavItem,
                Controller = "OrderModels",
                Action = "Index",
                Area = "Order",
                Title = "Quản lý đơn hàng",
                AwesomeIcon = "far fa-folder"
            });

        }


        public string renderHtml()
        {
            var html = new StringBuilder();

            foreach (var item in Items)
            {
                html.Append(item.RenderHtml(UrlHelper));
            }


            return html.ToString();
        }

        public void SetActive(string Controller, string Action, string Area)
        {
            foreach (var item in Items)
            {
                if (item.Controller == Controller && item.Action == Action && item.Area == Area)
                {
                    item.IsActive =  true;
                    return;
                }
                else
                {
                    if (item.Items != null)
                    {
                        foreach (var childItem in item.Items)
                        {
                            if (childItem.Controller == Controller && childItem.Action == Action && childItem.Area == Area)
                            {
                                childItem.IsActive = true;
                                item.IsActive = true;
                                return;

                            }
                        }
                    }
                }



            }
        }


    }
}