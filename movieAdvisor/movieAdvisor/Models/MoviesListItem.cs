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
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            var tempPosterId=entities.PICTURES_MOVIES.Where(pm=>pm.MOVIE_ID==movie.ID && pm.IS_POSTER==true).ToList();
            if (tempPosterId.Count > 0)
            {
                int tempId=tempPosterId[0].PICTURES_ID;
                poster = entities.PICTURES.Where(p => p.ID == tempId).First().PATH;
                alt = entities.PICTURES.Where(p => p.ID == tempId).First().TITLE;
            }
            else
            {
                poster = entities.PICTURES.Where(p => p.ID == 9).First().PATH;
                alt = entities.PICTURES.Where(p => p.ID == 9).First().TITLE;
            }

            this.movie = movie;
            genres = new List<GENRES>();
            foreach (var genre in movie.MOVIES_GENRES)
            {
                genres.Add(entities.GENRES.Where(g=>g.ID==genre.GENRE_ID).First());
            }
        }
    }
}