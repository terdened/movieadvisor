using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using movieAdvisor.Models;

namespace movieAdvisor.Controllers
{
    public class PersonsController : Controller
    {
        //
        // GET: /Persons/

        public ActionResult Index()
        {
            return RedirectToAction("Find");
        }

        public ActionResult Show(int id)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            PersonShow model = new PersonShow(entities.PERSONS.Where(p => p.ID == id).FirstOrDefault());
            return View(model);
        }


        public ActionResult Find(string keyWord="")
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            List<PERSONS> tempPersonsHolder = entities.PERSONS.OrderByDescending(p=>p.MOVIES_PERSONS_ROLES.Count()).ToList();
            List<PersonsListItem> model = new List<PersonsListItem>();

            foreach (var person in tempPersonsHolder)
            {
                if ((person.NAME.ToLower().IndexOf(keyWord.ToLower()) > -1) && (model.Count<20))
                    model.Add(new PersonsListItem(person));
            }

            return View(model);
        }

        public ActionResult AddComment(string commentText, int personId)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            PERSONS_COMMENTS tempComment = new PERSONS_COMMENTS();

            tempComment.PERSON_ID = personId;
            tempComment.TEXT = commentText;
            tempComment.USER_ID = entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().ID;
            tempComment.MARK = 10;
            tempComment.ID = entities.PERSONS_COMMENTS.OrderByDescending(pc => pc.ID).First().ID + 1;
            entities.PERSONS_COMMENTS.AddObject(tempComment);
            entities.SaveChanges();

            return RedirectToAction("Show", new { id = personId });
        }

        [HttpGet]
        public ActionResult AddPerson(int id = 0)
        {
            PersonShow model = new PersonShow();
            List<SelectListItem> movies = new List<SelectListItem>();
            List<SelectListItem> roles = new List<SelectListItem>();
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            List<MOVIES> tempMoviesHolder = entities.MOVIES.ToList();

            model.person.NAME = "Имя";
            model.person.DESCRIPTION = "Описание";

            if (id != 0)
            {
                model = new PersonShow(entities.PERSONS.Where(m => m.ID == id).First());
            }

            foreach (var mov in tempMoviesHolder)
            {
                movies.Add(new SelectListItem() { Text = mov.TITLE, Value = mov.ID.ToString() });
            }

            ViewBag.movies = movies.AsEnumerable();
            List<ROLES> tempRolesHolder = entities.ROLES.ToList();

            foreach (var role in tempRolesHolder)
            {
                roles.Add(new SelectListItem() { Text = role.TITLE, Value = role.ID.ToString() });
            }
            ViewBag.roles = roles.AsEnumerable();

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPerson(PersonShow model, string action, string movie = null, string role = null, HttpPostedFileBase foto = null, HttpPostedFileBase poster = null)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            PersonCreator creator = new PersonCreator();
            model = creator.update(model, action, foto, poster, Server, movie, role);


            List<SelectListItem> movies = new List<SelectListItem>();
            List<SelectListItem> roles = new List<SelectListItem>();
            List<MOVIES> tempMoviesHolder = entities.MOVIES.ToList();

            foreach (var mov in tempMoviesHolder)
            {
                bool isFree = true;
                foreach (var mov1 in model.moviesList)
                {
                    if (mov.ID == mov1.movie.ID)
                        isFree = false;
                }

                if (isFree)
                    movies.Add(new SelectListItem() { Text = mov.TITLE, Value = mov.ID.ToString() });
            }

            ViewBag.movies = movies.AsEnumerable();

            List<ROLES> tempRolesHolder = entities.ROLES.ToList();

            foreach (var rol in tempRolesHolder)
            {
                roles.Add(new SelectListItem() { Text = rol.TITLE, Value = rol.ID.ToString() });
            }

            ViewBag.roles = roles.AsEnumerable();

            if (action == "Сохранить")
                return RedirectToAction("Index");

            return View(model);
        }


        public ActionResult DelPerson(int id = 0)
        {

            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            PersonCreator creator = new PersonCreator();
            creator.RemovePerson(entities.PERSONS.Where(m => m.ID == id).First());

            return RedirectToAction("Index");
        }

    }
}
