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
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
            PersonShow model = new PersonShow(entities.PERSONS.Where(p => p.ID == id).FirstOrDefault());
            return View(model);
        }

        [HttpGet]
        public ActionResult Find()
        {
            List<PersonsListItem> model = new List<PersonsListItem>();
            return View(model);
        }

        [HttpPost]
        public ActionResult Find(string keyWord)
        {
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
            List<PERSONS> tempPersonsHolder = entities.PERSONS.ToList();
            List<PersonsListItem> model = new List<PersonsListItem>();

            foreach (var person in tempPersonsHolder)
            {
                if (person.NAME.ToLower().IndexOf(keyWord.ToLower()) > -1)
                    model.Add(new PersonsListItem(person));
            }

            return View(model);
        }

        public ActionResult AddComment(string commentText, int personId)
        {
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
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

    }
}
