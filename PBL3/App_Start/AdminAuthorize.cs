using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PBL3.App_Start
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        public string[] Role { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //1.Check session: đã đăng nhập -> cho thực hiện Filter
            //Ngược lại thi cho trở lại -> trang đăng nhập 
            User nvSession = (User)HttpContext.Current.Session["user"];
            if (nvSession != null)
            {
                if (nvSession.Role == 2)
                {
                    //là khách hàng thì chay ve userhome
                    var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    filterContext.Result = new RedirectToRouteResult
                     (
                        new RouteValueDictionary
                        (new
                        {
                            controller = "AdminHome",
                            action = "ChuyenVeUserHome",
                            //  area = "Admin",
                            returnUrl = returnUrl.ToString()
                        }));
                }
                else
                {
                    if (Role.Contains(nvSession.Role1.RoleName))
                    {
                        return;
                    }
                    else
                    {
                        var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                        filterContext.Result = new RedirectToRouteResult
                         (
                            new RouteValueDictionary
                            (new
                            {
                                controller = "AdminHome",
                                action = "Error",
                                //  area = "Admin",
                                returnUrl = returnUrl.ToString()
                            }));
                    }
                }
                return;
            }
            else
            {
                var returnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult
                 (
                    new RouteValueDictionary
                    (new
                    {
                        controller = "AdminHome",
                        action = "Login",
                        //  area = "Admin",
                        returnUrl = returnUrl.ToString()
                    }));
            }
        }


    }
}