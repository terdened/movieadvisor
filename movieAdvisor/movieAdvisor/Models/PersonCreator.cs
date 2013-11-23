using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace movieAdvisor.Models
{
    public class PersonCreator
    {

        public int uploadPic(HttpPostedFileBase file, HttpServerUtilityBase Server)
        {
            int id = 0;

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("../Uploads"), fileName);
                try
                {
                    file.SaveAs(path);
                }
                catch
                {
                    path = Path.Combine(Server.MapPath("../../Uploads"), fileName);
                    file.SaveAs(path);
                }
                MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
                PICTURES tempPic = new PICTURES();
                tempPic.PATH = fileName;
                tempPic.TITLE = fileName;
                id = entities.PICTURES.OrderByDescending(p => p.ID).First().ID + 1;
                tempPic.ID = id;
                entities.PICTURES.AddObject(tempPic);
                entities.SaveChanges();
            }
            else
            {
                id = -1;
            }

            return id;
        }

        public void RemovePerson(PERSONS person)
        {
            MOVIEADVISOREntities6 entities1 = new MOVIEADVISOREntities6();
            int tempId = person.ID;

            foreach (var pm in entities1.PICTURES_PERSONS.Where(p => p.PERSON_ID == person.ID))
            {
                entities1.PICTURES_PERSONS.DeleteObject(pm);
            }

            foreach (var mpr in entities1.MOVIES_PERSONS_ROLES.Where(p => p.PERSON_ID == person.ID))
            {
                entities1.MOVIES_PERSONS_ROLES.DeleteObject(mpr);
            }

            foreach (var mpr in entities1.PERSONS_COMMENTS.Where(p => p.PERSON_ID == person.ID))
            {
                entities1.PERSONS_COMMENTS.DeleteObject(mpr);
            }

            entities1.PERSONS.DeleteObject(entities1.PERSONS.Where(m => m.ID == tempId).First());
            entities1.SaveChanges();
        }

        public PersonShow update(PersonShow model, string action, HttpPostedFileBase foto, HttpPostedFileBase poster, HttpServerUtilityBase Server, string movie, string role)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            PersonShow result = new PersonShow();

            result.person.ID = model.person.ID;
            result.person = model.person;
            result.picturesList = new List<PICTURES>();
            foreach (var pic in model.picturesList)
            {
                result.picturesList.Add(entities.PICTURES.Where(p => p.ID == pic.ID).First());
            }
            result.moviesList = new List<Movie>();
            foreach (var per in model.moviesList)
            {
                result.moviesList.Add(new Movie(entities.MOVIES.Where(p => p.ID == per.movie.ID).First()));
                result.moviesList.Last().roles = new List<ROLES>();
                result.moviesList.Last().roles.Add(per.roles[0]);
            }

            result.poster = model.poster;
            result.posterId = model.posterId;
            result.altPoster = model.altPoster;

            result.comments = new List<PERSONS_COMMENTS>();

            if (action == "Добавить фильм")
            {
                int idHolder = Int32.Parse(movie);
                int roleHolder = Int32.Parse(role);
                result.moviesList.Add(new Movie(entities.MOVIES.Where(p => p.ID == idHolder).First()));
                result.moviesList.Last().roles = new List<ROLES>();
                result.moviesList.Last().roles.Add(entities.ROLES.Where(r => r.ID == roleHolder).First());
            }
            else
            if (action == "Загрузить фото")
            {          
                int id = this.uploadPic(foto, Server);
                if (id > -1)
                {
                    result.picturesList.Add(entities.PICTURES.OrderByDescending(p => p.ID).First());
                }
            }
            else
            if (action == "Загрузить постер")
            {
                int id = uploadPic(poster, Server);
                if (id > -1)
                {
                    result.poster = entities.PICTURES.OrderByDescending(p => p.ID).First().PATH;
                    result.posterId = entities.PICTURES.OrderByDescending(p => p.ID).First().ID;
                }
            }
            else
            if (action == "Сохранить")
            {
                PERSONS tempPersons = new PERSONS();
                tempPersons.DESCRIPTION = result.person.DESCRIPTION;
                tempPersons.NAME = result.person.NAME;

                if (result.person.ID > 0)
                {
                    tempPersons.ID = result.person.ID;
                    RemovePerson(tempPersons);
                }
                else
                {
                    tempPersons.ID = entities.PERSONS.OrderByDescending(m => m.ID).First().ID + 1;
                }

                foreach (var per in result.moviesList)
                {                    
                    MOVIES_PERSONS_ROLES tempMPR = new MOVIES_PERSONS_ROLES();
                    tempMPR.MOVIE_ID = per.movie.ID;
                    tempMPR.PERSON_ID = result.person.ID;
                    tempMPR.ROLE_ID = per.roles.First().ID;
                    tempPersons.MOVIES_PERSONS_ROLES.Add(tempMPR);
                }

                foreach (var photo in result.picturesList)
                {
                    PICTURES_PERSONS tempPM = new PICTURES_PERSONS();
                    tempPM.PERSON_ID = result.person.ID;
                    tempPM.PICTURE_ID = photo.ID;
                    tempPM.IS_POSTER = false;
                    tempPersons.PICTURES_PERSONS.Add(tempPM);
                }
                
                if (model.posterId > 0)
                {                    
                    PICTURES_PERSONS tempPoster = new PICTURES_PERSONS();
                    tempPoster.PERSON_ID = model.person.ID;
                    tempPoster.PICTURE_ID = model.posterId;
                    tempPoster.IS_POSTER = true;
                    tempPersons.PICTURES_PERSONS.Add(tempPoster);
                }

                entities.PERSONS.AddObject(tempPersons);
                try
                {
                    entities.SaveChanges();
                }
                catch
                {
                }
            }
            else
            if (action.IndexOf("УдалитьФото") > -1)
            {
                int tempId = Int32.Parse(action.Substring(11, action.Length - 11));
                result.picturesList.Remove(result.picturesList.Where(p => p.ID == tempId).First());
            }
            else
            if (action.IndexOf("Удалить") > -1)
            {
                int tempId = Int32.Parse(action.Substring(7, action.Length-7));
                result.moviesList.Remove(result.moviesList.Where(p => p.movie.ID == tempId).First());
            }
            
            return result;
        }
    }
}