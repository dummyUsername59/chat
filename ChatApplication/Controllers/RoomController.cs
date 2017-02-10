using ChatApplication.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatApplication.Controllers
{
    public class RoomController : Controller
    {
        // GET: Rooom
        
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            var room = CacheProvider.Instance.Get(id);
            if(room == null)
            {
                Random rd = new Random(DateTime.Now.Millisecond);
                var roomRandomIndex = rd.Next(20, 300);
                CacheProvider.Instance.Set(id, new KeyValuePair<string, DateTime>("Room"+roomRandomIndex,DateTime.Now));
            }
            return View((object)id);
        }
        [HttpPost]
        public ActionResult UpdateRoomAccessDate(string id)
        {
            
            if (!string.IsNullOrEmpty(id))
            {
                CacheProvider.Instance.UpdateLastUsed(id);
                return Json("OK");
            }
            return Json("Failed");
        }
    }
}