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
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            List<MoviesListItem> model = new List<MoviesListItem>();
            List<MOVIES> tempHolder = entities.MOVIES.ToList();

            if (tempHolder.Count > 20)
            {
                foreach (var movie in tempHolder.GetRange(tempHolder.Count - 20, 20))
                {
                    model.Add(new MoviesListItem(movie));
                    var tempRating = movie.MOVIES_COMMENTS.Where(mc => mc.MARK != null);
                    double raiting = 0;
                    foreach (var rai in tempRating)
                    {
                        raiting += (double)rai.MARK;
                        if (User.Identity.Name != "")
                        {
                            if (rai.USER_ID == entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().ID)
                            {
                                model.Last().isRaited = true;
                                model.Last().yourMark = (int)rai.MARK;
                            }
                        }
                    }
                    raiting /= tempRating.Count();

                    model.Last().raiting = raiting;
                }
            }
            else
            {
                foreach (var movie in tempHolder)
                {
                    model.Add(new MoviesListItem(movie));
                    var tempRating = movie.MOVIES_COMMENTS.Where(mc => mc.MARK != null);
                    double raiting = 0;
                    foreach (var rai in tempRating)
                    {
                        raiting += (double)rai.MARK;
                        if (User.Identity.Name != "")
                        {
                            if (rai.USER_ID == entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().ID)
                            {
                                model.Last().isRaited = true;
                                model.Last().yourMark = (int)rai.MARK;
                            }
                        }
                    }
                    raiting /= tempRating.Count();

                    model.Last().raiting = raiting;
                }
            }
            model=model.OrderByDescending(m => m.movie.ID).ToList();
            return View(model);
        }

        public ActionResult Show(int id)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            MovieShow model = new MovieShow(entities.MOVIES.Where(m => m.ID == id).FirstOrDefault());
            return View(model);
        }

        [HttpGet]
        public ActionResult Find(int genre = 0)
        {
            return RedirectToAction("FindPost","Movies", new { genre = genre });
        }

        public ActionResult FindPost(string keyWord="", int genre=0, int year=0)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            List<MOVIES> tempMoviesHolder = entities.MOVIES.OrderByDescending(m=>m.YEAR).ToList();
            List<MoviesListItem> model = new List<MoviesListItem>();
            List<SelectListItem> genres = new List<SelectListItem>();
            var tempGenresHolder = entities.GENRES;

            genres.Add(new SelectListItem() { Text = "Любой", Value = "0" });

            foreach (var gen in tempGenresHolder)
            {
                genres.Add(new SelectListItem() { Text = gen.TITLE, Value = gen.ID.ToString() });
            }
            ViewBag.genres = genres;
            foreach (var movie in tempMoviesHolder)
            {
                bool match = true;
                if (keyWord != "")
                {
                    if (movie.TITLE.ToLower().IndexOf(keyWord.ToLower()) > -1)
                        match = true;
                    else
                        match = false;
                }
                if (year != 0)
                {
                    if ((movie.YEAR == year)&&(match))
                        match = true;
                    else
                        match = false;
                }
                if (genre != 0)
                {
                    bool mathc1 = false;
                    foreach(var gen in movie.MOVIES_GENRES)
                    {
                        if (gen.GENRE_ID==genre)
                            mathc1 = true;
                    }

                    if ((mathc1)&&(match))
                        match = true;
                    else
                        match = false;
                }
                if ((match) && (model.Count < 20))
                {
                    model.Add(new MoviesListItem(movie));
                    var tempRating = movie.MOVIES_COMMENTS.Where(mc => mc.MARK != null);
                    double raiting = 0;
                    foreach(var rai in tempRating)
                    {
                        raiting += (double)rai.MARK;
                        if (User.Identity.Name != "")
                        {
                            if (rai.USER_ID == entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().ID)
                            {
                                model.Last().isRaited = true;
                                model.Last().yourMark = (int)rai.MARK;
                            }
                        }
                    }
                    raiting /= tempRating.Count();

                    model.Last().raiting = raiting;
                }
            }

            return View("Find",model);
        }

        public ActionResult AddComment(string commentText, int movieId)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
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

        [HttpGet]
        public ActionResult AddMovie(int id = 0)
        {
            MovieShow model = new MovieShow();
            List<SelectListItem> persons = new List<SelectListItem>();
            List<SelectListItem> roles = new List<SelectListItem>();
            List<SelectListItem> genres = new List<SelectListItem>();
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            List<PERSONS> tempPersonsHolder = entities.PERSONS.ToList();

            model.movie.TITLE = "Название";
            model.movie.DESCRIPTION = "Описание";

            if (id != 0)
            {
                model = new MovieShow(entities.MOVIES.Where(m => m.ID == id).First());
            }

            foreach (var per in tempPersonsHolder)
            {
                persons.Add(new SelectListItem() { Text=per.NAME, Value=per.ID.ToString()});
            }
            ViewBag.persons = persons.AsEnumerable();

            List<GENRES> tempGenresHolder = entities.GENRES.ToList();

            foreach (var gen in tempGenresHolder)
            {
                bool isFree = true;
                foreach (var gen1 in model.movie.MOVIES_GENRES)
                {
                    if (gen.ID == gen1.GENRE_ID)
                        isFree = false;
                }

                if (isFree)
                    genres.Add(new SelectListItem() { Text = gen.TITLE, Value = gen.ID.ToString() });
            }
            ViewBag.genres = genres.AsEnumerable();


            List<ROLES> tempRolesHolder = entities.ROLES.ToList();

            foreach (var role in tempRolesHolder)
            {
                roles.Add(new SelectListItem() { Text = role.TITLE, Value = role.ID.ToString() });
            }
            ViewBag.roles = roles.AsEnumerable();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddMovie(MovieShow model, string action, string person = null, string role = null, string genre = null, HttpPostedFileBase foto = null, HttpPostedFileBase poster = null)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            MovieCreator creator = new MovieCreator();
            model = creator.update(model, action, foto, poster, Server, person, role,genre);


            List<SelectListItem> persons = new List<SelectListItem>();
            List<SelectListItem> genres = new List<SelectListItem>();
            List<SelectListItem> roles = new List<SelectListItem>();
            List<PERSONS> tempPersonsHolder = entities.PERSONS.ToList();

            foreach (var per in tempPersonsHolder)
            {
                bool isFree = true;
                foreach (var per1 in model.personsList)
                {
                    if (per.ID == per1.person.ID)
                        isFree = false;
                }

                if (isFree)
                    persons.Add(new SelectListItem() { Text = per.NAME, Value = per.ID.ToString() });
            }
            ViewBag.persons = persons.AsEnumerable();

            List<GENRES> tempGenresHolder = entities.GENRES.ToList();

            foreach (var gen in tempGenresHolder)
            {
                bool isFree = true;
                foreach (var gen1 in model.genres)
                {
                    if (gen.ID == gen1.ID)
                        isFree = false;
                }

                if (isFree)
                    genres.Add(new SelectListItem() { Text = gen.TITLE, Value = gen.ID.ToString() });
            }
            ViewBag.genres = genres.AsEnumerable();


            List<ROLES> tempRolesHolder = entities.ROLES.ToList();

            foreach (var rol in tempRolesHolder)
            {
                roles.Add(new SelectListItem() { Text = rol.TITLE, Value = rol.ID.ToString() });
            }

            ViewBag.roles = roles.AsEnumerable();

            if (action == "Сохранить")
                return RedirectToAction("New");

            return View(model);
        }

        
        public ActionResult DelMovie(int id = 0)
        {

            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            MovieCreator creator = new MovieCreator();
            creator.RemoveMovie(entities.MOVIES.Where(m => m.ID == id).First());

            return RedirectToAction("New");
        }

        public ActionResult UpdateRatingFind(string value, string keyWord="", int genre=0, int year=0)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            MOVIES_COMMENTS tempComment = new MOVIES_COMMENTS();
            int rating = Int32.Parse(value.Split('/').Last());
            tempComment.ID = entities.MOVIES_COMMENTS.OrderByDescending(mc=>mc.ID).First().ID+1;
            tempComment.MARK = rating;
            tempComment.USER_ID = entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().ID;
            tempComment.MOVIE_ID = Int32.Parse(value.Split('/').First());
            entities.MOVIES_COMMENTS.AddObject(tempComment);
            entities.SaveChanges();

            return RedirectToAction("FindPost",new { keyWord = keyWord, genre = genre, year = year });
        }
    }
}
