using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movieAdvisor.Models
{
    public class Movie
    {
        public MOVIES movie { get; set; }
        public PICTURES picture { get; set; }
        public List<ROLES> roles { get; set; }

        public Movie(MOVIES movie)
        {
            this.movie = movie;

            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();

            List<PICTURES_MOVIES> tempPicturesHolder = entities.PICTURES_MOVIES.Where(pm => pm.MOVIE_ID == movie.ID && pm.IS_POSTER == true).ToList();

            if (tempPicturesHolder.Count > 0)
            {
                int tempPicId=tempPicturesHolder.First().PICTURES_ID;
                picture = entities.PICTURES.Where(p => p.ID == tempPicId).First();
            }
            else
                picture = entities.PICTURES.Where(p => p.ID == 9).First();
        }

        public void SetRole(PERSONS person)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            roles = new List<ROLES>();
            List<MOVIES_PERSONS_ROLES> tempRoles = entities.MOVIES_PERSONS_ROLES.
                Where(mpr=>mpr.MOVIE_ID==movie.ID && mpr.PERSON_ID==person.ID).ToList();
            foreach(var role in tempRoles)
            {
                roles.Add(entities.ROLES.Where(r=>r.ID==role.ROLE_ID).First());
            }
        }

        public Movie()
        {
        }
    }
}