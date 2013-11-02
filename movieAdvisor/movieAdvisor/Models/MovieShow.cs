using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movieAdvisor.Models
{
    public class MovieShow
    {
        public MOVIES movie { get; set; }
        public List<PICTURES> picturesList { get; set; }
        public List<Person> actorsList { get; set; }
        public List<Person> directorsList { get; set; }
        public string poster { get; set; }
        public string altPoster { get; set; }
        public List<MOVIES_COMMENTS> comments { get; set; }

        public MovieShow(MOVIES movie)
        {
            this.movie = movie;

            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();

            List<PICTURES_MOVIES> tempPicId=entities.PICTURES_MOVIES.Where(pm=>pm.MOVIE_ID==movie.ID).ToList();
            picturesList = new List<PICTURES>();

            foreach (var pic in tempPicId)
            {
                if (!pic.IS_POSTER)
                    picturesList.Add(entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First());
                else
                {
                    poster = entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First().PATH;
                    altPoster = entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First().TITLE;
                }
            }
            List<MOVIES_PERSONS_ROLES> tempPerId = entities.MOVIES_PERSONS_ROLES.Where(pmr => pmr.MOVIE_ID == movie.ID).ToList();

            actorsList = new List<Person>();
            directorsList = new List<Person>();

            foreach (var per in tempPerId)
            {
                if (per.ROLE_ID == 1)
                    actorsList.Add(new Person(entities.PERSONS.Where(p => p.ID == per.PERSON_ID).First()));
                if (per.ROLE_ID == 2)
                    directorsList.Add(new Person(entities.PERSONS.Where(p => p.ID == per.PERSON_ID).First()));
            }

            comments = entities.MOVIES_COMMENTS.Where(mc => mc.MOVIE_ID == movie.ID).ToList();
        }
    }
}