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
                actores = new List<string>(),
                awords = new List<string>(),
                creators = new List<string>(),
                genres = new List<string>(),
                languages = new List<string>(),
                states = new List<string>()
            };

            foreach (var item in data)
            {
                if (item.Type == "actor")
                {
                    listOfTags.actores.Add(item.Name);
                }
                if (item.Type == "aword")
                {
                    listOfTags.awords.Add(item.Name);
                }
                if (item.Type == "director")
                {
                    listOfTags.creators.Add(item.Name);
                }
                if (item.Type == "genre")
                {
                    listOfTags.genres.Add(item.Name);
                }
                if (item.Type == "language")
                {
                    listOfTags.languages.Add(item.Name);
                }
                if (item.Type == "state")
                {
                    listOfTags.states.Add(item.Name);
                }
            }

            return listOfTags;
        }

        


    }
}
