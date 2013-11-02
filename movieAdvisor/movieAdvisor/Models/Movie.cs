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

        public Movie(MOVIES movie)
        {
            this.movie = movie;

            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();

            List<PICTURES_MOVIES> tempPicturesHolder = entities.PICTURES_MOVIES.Where(pm => pm.MOVIE_ID == movie.ID && pm.IS_POSTER == true).ToList();

            if (tempPicturesHolder.Count > 0)
            {
                int tempPicId=tempPicturesHolder.First().PICTURES_ID;
                picture = entities.PICTURES.Where(p => p.ID == tempPicId).First();
            }
            else
                picture = entities.PICTURES.Where(p => p.ID == 9).First();
        }
    }
}