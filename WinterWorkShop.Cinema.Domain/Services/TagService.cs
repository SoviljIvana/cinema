using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<TagDomainModel> GetAllTags()
        {
            var data = await _tagRepository.GetAll();
            if (data == null)
            {
                return null;
            }

            TagDomainModel listOfTags = new TagDomainModel()
            {
                actores = new List<TagForMovieDomainModel>(),
                awords = new List<TagForMovieDomainModel>(),
                genres = new List<TagForMovieDomainModel>(),
                languages = new List<TagForMovieDomainModel>(),
                states = new List<TagForMovieDomainModel>(),
                directors =  new List<TagForMovieDomainModel>()
            };

            foreach (var item in data)
            {
                if (item.Type == "actor")
                {
                    listOfTags.actores.Add(new TagForMovieDomainModel()
                    {
                        Name = item.Name
                    }); 
                }
                if (item.Type == "aword")
                {
                    listOfTags.awords.Add(new TagForMovieDomainModel()
                    {
                        Name = item.Name
                    });
                }
               
                if (item.Type == "genre")
                {
                    listOfTags.genres.Add(new TagForMovieDomainModel() 
                    { Name = item.Name 
                    });
                }
                if (item.Type == "language")
                {
                    listOfTags.languages.Add(new TagForMovieDomainModel()
                    {
                        Name = item.Name
                    });
                }
                if (item.Type == "state")
                {
                    listOfTags.states.Add(new TagForMovieDomainModel()
                    {
                        Name = item.Name
                    });
                }
                if(item.Type == "director")
                {
                    listOfTags.directors.Add(new TagForMovieDomainModel()
                    {
                        Name = item.Name
                    }
                    );
                }
            }

            return listOfTags;
        }
    }
}
