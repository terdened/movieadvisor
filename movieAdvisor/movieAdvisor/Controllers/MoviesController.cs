using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using movieAdvisor.Models;

namespace movieAdvisor.Controllers
{
    public class MoviesController : Controller
    {
        //
        // GET: /Movies/

        public ActionResult Index()
        {
            return RedirectToAction("New");
        }

        public ActionResult New()
        {
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
            List<MoviesListItem> model = new List<MoviesListItem>();
            List<MOVIES> tempHolder = entities.MOVIES.ToList();

            if (tempHolder.Count > 20)
            {
                foreach (var movie in tempHolder.GetRange(tempHolder.Count - 20, 20))
                {

                    model.Add(new MoviesListItem(movie));
                }
            }
            else
            {
                foreach (var movie in tempHolder)
                {

                    model.Add(new MoviesListItem(movie));
                }
            }
            return View(model);
        }

        public ActionResult Show(int id)
        {
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
            MovieShow model = new MovieShow(entities.MOVIES.Where(m => m.ID == id).FirstOrDefault());
            return View(model);
        }

        public ActionResult AddComment(string commentText, int movieId)
        {
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
            MOVIES_COMMENTS tempComment = new MOVIES_COMMENTS();

            tempComment.MOVIE_ID = movieId;
            tempComment.TEXT = commentText;
            tempComment.USER_ID = entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().ID;
            tempComment.MARK = 10;
            tempComment.ID = entities.MOVIES_COMMENTS.OrderByDescending(mc=>mc.ID).First().ID+1;
            entities.MOVIES_COMMENTS.AddObject(tempComment);
            entities.SaveChanges();

            return RedirectToAction("Show", new { id = movieId });
        }

    }
}
