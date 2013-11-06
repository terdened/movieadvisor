using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movieAdvisor.Models
{
    public class PersonsListItem
    {
        public PERSONS person { get; set; }
        public List<ROLES> roles { get; set; }
        public string poster { get; set; }
        public string alt { get; set; }

        public PersonsListItem(PERSONS person)
        {
            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();
            int tempPosterId=entities.PICTURES_PERSONS.Where(pp=>pp.PERSON_ID==person.ID && pp.IS_POSTER==true).FirstOrDefault().PICTURE_ID;
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

            this.person = person;
        }
    }
}