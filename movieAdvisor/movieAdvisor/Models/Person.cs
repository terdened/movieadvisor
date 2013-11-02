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

        public Person(PERSONS person)
        {
            this.person = person;

            MOVIEADVISOREntities5 entities = new MOVIEADVISOREntities5();

            List<PICTURES_PERSONS> tempPicturesHolder = entities.PICTURES_PERSONS.Where(pp => pp.PERSON_ID == person.ID && pp.IS_POSTER==true).ToList();

            if (tempPicturesHolder.Count > 0)
            {
                int tempPicId=tempPicturesHolder.First().PICTURE_ID;
                picture = entities.PICTURES.Where(p => p.ID == tempPicId).First();
            }
            else
                picture = entities.PICTURES.Where(p => p.ID == 9).First();
        }
    }
}