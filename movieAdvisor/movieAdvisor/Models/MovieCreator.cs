using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace movieAdvisor.Models
{
    public class MovieCreator
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

        public void RemoveMovie(MOVIES movie)
        {
            MOVIEADVISOREntities6 entities1 = new MOVIEADVISOREntities6();
            int tempId = movie.ID;

            foreach (var pm in entities1.PICTURES_MOVIES.Where(p => p.MOVIE_ID == movie.ID))
            {
                entities1.PICTURES_MOVIES.DeleteObject(pm);
            }
            entities1.SaveChanges();
            foreach (var mpr in entities1.MOVIES_PERSONS_ROLES.Where(p => p.MOVIE_ID == movie.ID))
            {
                entities1.MOVIES_PERSONS_ROLES.DeleteObject(mpr);
            }
            entities1.SaveChanges();
            foreach (var mpr in entities1.MOVIES_COMMENTS.Where(p => p.MOVIE_ID == movie.ID))
            {
                entities1.MOVIES_COMMENTS.DeleteObject(mpr);
            }
            entities1.SaveChanges();

            var tempGenres = entities1.MOVIES_GENRES.Where(mg => mg.MOVIE_ID == movie.ID);
            foreach (var gen in tempGenres)
            {
                entities1.MOVIES_GENRES.DeleteObject(gen);
            }

            entities1.SaveChanges();
            entities1.MOVIES.DeleteObject(entities1.MOVIES.Where(m => m.ID == tempId).First());
            entities1.SaveChanges();
        }

        public MovieShow update(MovieShow model, string action, HttpPostedFileBase foto, HttpPostedFileBase poster, HttpServerUtilityBase Server , string person, string role, string genre)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            MovieShow result = new MovieShow();

            result.movie.ID = model.movie.ID;
            result.movie = model.movie;
            result.picturesList = new List<PICTURES>();
            foreach (var pic in model.picturesList)
            {
                result.picturesList.Add(entities.PICTURES.Where(p => p.ID == pic.ID).First());
            }
            result.personsList = new List<Person>();
            foreach (var per in model.personsList)
            {
                result.personsList.Add(new Person(entities.PERSONS.Where(p => p.ID == per.person.ID).First()));
                result.personsList.Last().roles= new List<ROLES>();
                result.personsList.Last().roles.Add(per.roles[0]);
            }

            foreach (var gen in model.genres)
            {
                result.genres.Add(new GENRES());
                result.genres.Last().ID = entities.GENRES.Where(g => g.ID == gen.ID).First().ID;
                result.genres.Last().TITLE = entities.GENRES.Where(g => g.ID == gen.ID).First().TITLE;
            }

            result.poster = model.poster;
            result.posterId = model.posterId;
            result.altPoster = model.altPoster;

            result.comments = new List<MOVIES_COMMENTS>();

            if (action == "Добавить жанр")
            {
                int idHolder = Int32.Parse(genre);
                result.genres.Add(new GENRES());
                result.genres.Last().ID = idHolder;
                result.genres.Last().TITLE = entities.GENRES.Where(g => g.ID == idHolder).First().TITLE;
            }
            else
            if (action == "Добавить личность")
            {
                int idHolder=Int32.Parse(person);
                int roleHolder = Int32.Parse(role);
                result.personsList.Add(new Person(entities.PERSONS.Where(p => p.ID == idHolder).First()));
                result.personsList.Last().roles = new List<ROLES>();
                result.personsList.Last().roles.Add(entities.ROLES.Where(r => r.ID == roleHolder).First());
            }
            else
            if (action == "Загрузить фото")
            {          
                int id = uploadPic(foto, Server);
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
                MOVIES tempMovie = new MOVIES();
                tempMovie.DESCRIPTION = result.movie.DESCRIPTION;
                tempMovie.TITLE = result.movie.TITLE;
                tempMovie.YEAR = result.movie.YEAR;
                if (result.movie.ID > 0)
                {
                    tempMovie.ID = result.movie.ID;
                    //tempMovie.ID = entities.MOVIES.OrderByDescending(m => m.ID).First().ID + 1;
                    RemoveMovie(tempMovie);
                }
                else
                {
                    tempMovie.ID = entities.MOVIES.OrderByDescending(m => m.ID).First().ID + 1;
                }

                foreach (var per in result.personsList)
                {                    
                    MOVIES_PERSONS_ROLES tempMPR = new MOVIES_PERSONS_ROLES();
                    tempMPR.MOVIE_ID = result.movie.ID;
                    tempMPR.PERSON_ID = per.person.ID;
                    tempMPR.ROLE_ID = per.roles.First().ID;
                    tempMovie.MOVIES_PERSONS_ROLES.Add(tempMPR);
                }

                foreach (var photo in result.picturesList)
                {
                    PICTURES_MOVIES tempPM = new PICTURES_MOVIES();
                    tempPM.MOVIE_ID = result.movie.ID;
                    tempPM.PICTURES_ID = photo.ID;
                    tempPM.IS_POSTER = false;
                    tempMovie.PICTURES_MOVIES.Add(tempPM);
                }

                foreach (var gen in result.genres)
                {
                    MOVIES_GENRES tempGen = new MOVIES_GENRES();
                    tempGen.MOVIE_ID = result.movie.ID;
                    tempGen.GENRE_ID = gen.ID;
                    tempMovie.MOVIES_GENRES.Add(tempGen);
                }
                
                if (model.posterId > 0)
                {                    
                    PICTURES_MOVIES tempPoster = new PICTURES_MOVIES();
                    tempPoster.MOVIE_ID = model.movie.ID;
                    tempPoster.PICTURES_ID = model.posterId;
                    tempPoster.IS_POSTER = true;
                    tempMovie.PICTURES_MOVIES.Add(tempPoster);
                }

                entities.MOVIES.AddObject(tempMovie);
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
            if (action.IndexOf("УдалитьЖанр") > -1)
            {
                int tempId = Int32.Parse(action.Substring(11, action.Length - 11));
                result.genres.Remove(result.genres.Where(p => p.ID == tempId).First());
            }
            else
            if (action.IndexOf("Удалить") > -1)
            {
                int tempId = Int32.Parse(action.Substring(7, action.Length-7));
                result.personsList.Remove(result.personsList.Where(p => p.person.ID == tempId).First());
            }
            

            return result;
        }
    }
}