#region using
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Syndication;
using TechnicalInterview_FeedReader.Models;
using System.Xml;
#endregion

namespace TechnicalInterview_FeedReader.Controllers
{
    public class UserFeedsController : Controller
    {
        #region instance variables
        private FeedReaderConnectionstring db = new FeedReaderConnectionstring();
        #endregion
        
        #region UserFeeds Index

        ///<summary>
        /// Use to view the list of Subscribed Feeds for the User
        ///</summary>
        //
        // GET: /UserFeeds/ HttpGet Method
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                var userFeeds = from q in db.tbl_UserFeeds where q.username == User.Identity.Name select q;
                return View(userFeeds.ToList());
            }
            catch
            {
                throw;
            }

        }

        #endregion

        #region UserFeeds Subscribe Rss Feed

        ///<summary>
        /// Use to Subscribe for new Rss Feed
        ///</summary>
        //
        // GET: /UserFeeds/Create HttpGet Method Create()
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }        
        
        //
        // POST: /UserFeeds/Create HttpPost Method Create()
        [HttpPost]
        public ActionResult Create(tbl_UserFeeds tbl_userfeeds)
        {
            // Condition to Check if the entered RSS Feed is Valid or Not
            if (IsValidFeedUrl(tbl_userfeeds.feedname.ToString()) == true)
            {                
                if (ModelState.IsValid)
                {
                    // Condition to check if the entered RSS Feed have already subscribed by logged User or Not
                    var userFeeds = from q in db.tbl_UserFeeds where q.username == User.Identity.Name select q;
                    var feedObj = userFeeds.FirstOrDefault(a => a.feedname == tbl_userfeeds.feedname);
                    if (feedObj == null)
                    {
                        db.tbl_UserFeeds.Add(tbl_userfeeds);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Exist = "You have already subscribed for to the feed " + tbl_userfeeds.feedname + ". Please enter another one";
                    }
                }
            }
            else
            {
                return RedirectToAction("InvalidFeed", "UserFeeds");
            }

            return View(tbl_userfeeds);
        }
        #endregion

        #region UserFeeds Unsubscribe Rss Feed
        ///<summary>
        /// Use to Unsubscribe for Rss Feed
        ///</summary>
        //
        // GET: /UserFeeds/Delete/5 HttpGet Method
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            tbl_UserFeeds tbl_userfeeds = db.tbl_UserFeeds.Find(id);
            if (tbl_userfeeds == null)
            {
                return HttpNotFound();
            }
            return View(tbl_userfeeds);
        }

        //
        // POST: /UserFeeds/Delete/5 HttpPost Method

        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_UserFeeds tbl_userfeeds = db.tbl_UserFeeds.Find(id);
            db.tbl_UserFeeds.Remove(tbl_userfeeds);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region overides methods
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        #region Check for Valid Rss Feed
        /// <summary>
        /// To check if the entered RSS Feed is valid Feed, Returns bool value true = Valid Feed and false = Invalid Feed
        /// </summary>
        /// <param name="url"></param>
        /// <returns>boolean</returns>
        private bool IsValidFeedUrl(string url)
        {
            bool isValid = true;
            try
            {
                using (XmlReader reader = XmlReader.Create(url))
                {
                    Rss20FeedFormatter formatter = new Rss20FeedFormatter();
                    formatter.ReadFrom(reader);
                }
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }
        #endregion

        #region Display Feed
        /// <summary>
        ///  Use to display the feed items of selected RSS Feed
        /// </summary>
        /// <param name="feedLink"></param>
        /// <returns>Model View</returns>
        [Authorize]
        public ActionResult DisplayFeed(string feedLink)
        {
            var model = new Models.tbl_UserFeeds();
            using (XmlReader reader = XmlReader.Create(feedLink))
            {
                SyndicationFeed rssData = SyndicationFeed.Load(reader);
                model.Feed = rssData;
                return View(model);
            }
        }
        #endregion

        #region Invalid Rss Feed
        /// <summary>
        /// Use to Show the Error message of Invalid RSS Feed
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult InvalidFeed()
        {
            return View();
        }
        #endregion
    }
}