using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movieAdvisor.Models
{
    public class PersonShow
    {
        public PERSONS person { get; set; }
        public List<PICTURES> picturesList { get; set; }
        public List<Movie> moviesList { get; set; }
        public string poster { get; set; }
        public int posterId { get; set; }
        public string altPoster { get; set; }
        public List<PERSONS_COMMENTS> comments { get; set; }

        public PersonShow(PERSONS person)
        {
            this.person = person;

            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();

            List<PICTURES_PERSONS> tempPicId = entities.PICTURES_PERSONS.Where(pp => pp.PERSON_ID == person.ID).ToList();
            picturesList = new List<PICTURES>();

            foreach (var pic in tempPicId)
            {
                if (!pic.IS_POSTER)
                    picturesList.Add(entities.PICTURES.Where(p => p.ID == pic.PICTURE_ID).First());
                else
                {
                    poster = entities.PICTURES.Where(p => p.ID == pic.PICTURE_ID).First().PATH;
                    posterId = entities.PICTURES.Where(p => p.ID == pic.PICTURE_ID).First().ID;
                    altPoster = entities.PICTURES.Where(p => p.ID == pic.PICTURE_ID).First().TITLE;
                }
            }
            List<MOVIES_PERSONS_ROLES> tempMovieId = entities.MOVIES_PERSONS_ROLES.Where(pmr => pmr.PERSON_ID == person.ID).ToList();
            moviesList = new List<Movie>();

            foreach (var movie in tempMovieId)
            {
                moviesList.Add(new Movie(entities.MOVIES.Where(m => m.ID == movie.MOVIE_ID).First()));
                moviesList.Last().SetRole(person);
            }

            comments = entities.PERSONS_COMMENTS.Where(pc => pc.PERSON_ID == person.ID).ToList();
        }

        public PersonShow()
        {
            person = new PERSONS();
            person.NAME = " ";
            picturesList = new List<PICTURES>();
            moviesList = new List<Movie>();
            poster = "";
            altPoster = "";
            posterId = -1;
            comments = new List<PERSONS_COMMENTS>();
        }
    }
}