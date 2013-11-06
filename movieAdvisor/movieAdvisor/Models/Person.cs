using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace movieAdvisor.Models
{
    public class Person
    {
        public PERSONS person { get; set; }
        public PICTURES picture { get; set; }
        public List<ROLES> roles { get; set; }

        public Person(PERSONS person)
        {
            this.person = person;

            MOVIEADVISOREntities6 entities = new MOVIEADVISOREntities6();

            List<PICTURES_PERSONS> tempPicturesHolder = entities.PICTURES_PERSONS.Where(pp => pp.PERSON_ID == person.ID && pp.IS_POSTER==true).ToList();

            if (tempPicturesHolder.Count > 0)
            {
                int tempPicId=tempPicturesHolder.First().PICTURE_ID;
                picture = entities.PICTURES.Where(p => p.ID == tempPicId).First();
            }
            else
                picture = entities.PICTURES.Where(p => p.ID == 9).First();
        }

        public void SetRole(MOVIES movie)
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

        public Person()
        {
        }
    }
}