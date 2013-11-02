using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movieAdvisor.Models
{
    public class MoviesListItem
    {
        public MOVIES movie { get; set; }
        public List<GENRES> genres { get; set; }
        public string poster { get; set; }
        public string alt { get; set; }

        public MoviesListItem(MOVIES movie)
        {
            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();
            int tempPosterId=entities.PICTURES_MOVIES.Where(pm=>pm.MOVIE_ID==movie.ID && pm.IS_POSTER==true).FirstOrDefault().PICTURES_ID;
            if (tempPosterId > 0)
            {
                poster = entities.PICTURES.Where(p => p.ID == tempPosterId).First().PATH;
                alt=entities.PICTURES.Where(p => p.ID == tempPosterId).First().TITLE;
            }
            else
            {
                poster = entities.PICTURES.Where(p => p.ID == 9).First().PATH;
                alt = entities.PICTURES.Where(p => p.ID == 9).First().TITLE;
            }

            this.movie = movie;
        }
    }
}