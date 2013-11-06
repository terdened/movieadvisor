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
        public List<Person> personsList { get; set; }
        public int posterId { get; set; }
        public string poster { get; set; }
        public string altPoster { get; set; }
        public List<MOVIES_COMMENTS> comments { get; set; }
        public List<GENRES> genres { get; set; }

        public MovieShow(MOVIES movie)
        {
            this.movie = movie;

            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();

            List<PICTURES_MOVIES> tempPicId=entities.PICTURES_MOVIES.Where(pm=>pm.MOVIE_ID==movie.ID).ToList();
            picturesList = new List<PICTURES>();

            foreach (var pic in tempPicId)
            {
                if (!pic.IS_POSTER)
                    picturesList.Add(entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First());
                else
                {
                    poster = entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First().PATH;
                    posterId = entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First().ID;
                    altPoster = entities.PICTURES.Where(p => p.ID == pic.PICTURES_ID).First().TITLE;
                }
            }

            List<MOVIES_PERSONS_ROLES> tempPerId = entities.MOVIES_PERSONS_ROLES.Where(pmr => pmr.MOVIE_ID == movie.ID).ToList();
            personsList = new List<Person>();

            foreach (var per in tempPerId)
            {
                personsList.Add(new Person(entities.PERSONS.Where(p => p.ID == per.PERSON_ID).First()));
                personsList.Last().SetRole(movie);
            }

            comments = entities.MOVIES_COMMENTS.Where(mc => mc.MOVIE_ID == movie.ID).ToList();
            genres = new List<GENRES>();
            foreach (var gen in this.movie.MOVIES_GENRES)
            {
                genres.Add(entities.GENRES.Where(g=>g.ID==gen.GENRE_ID).First());
            }
        }

        public MovieShow()
        {
            movie = new MOVIES();
            movie.TITLE = " ";
            picturesList = new List<PICTURES>();
            personsList = new List<Person>();
            poster = "";
            altPoster = "";
            posterId = -1;
            comments = new List<MOVIES_COMMENTS>();
            genres = new List<GENRES>();
        }
    }
}