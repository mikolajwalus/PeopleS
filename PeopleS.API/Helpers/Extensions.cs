using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PeopleS.API.Dtos;
using PeopleS.API.Models;

namespace PeopleS.API.Helpers
{
    public static class Extensions
    {
        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static PagedList<UserForSearchDto> ChangeToSearchDto(this PagedList<User> source, int friendId)
        {

            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserForSearchDto>().ReverseMap());
            var mapper = config.CreateMapper();

            var dtoList = mapper.Map<List<UserForSearchDto>>(source);

            var mappedList = new PagedList<UserForSearchDto>(dtoList, source.Count, source.CurrentPage);
            
            for(int i = 0; i < source.Count; i++)
            {

                if( source[i].FriendsRecieved.Where(x => x.RequestorId == friendId).FirstOrDefault() != null )
                {
                    mappedList[i].FriendshipStatus = source[i].FriendsRecieved.Where(x => x.RequestorId == friendId).FirstOrDefault().Status;
                    continue;
                }

                if( source[i].FriendsRequested.Where(x => x.RecieverId == friendId).FirstOrDefault() != null ) 
                {
                    mappedList[i].FriendshipStatus = source[i].FriendsRequested.Where(x => x.RecieverId == friendId).FirstOrDefault().Status;
                    continue;
                }

                else mappedList[i].FriendshipStatus = 3;
            }

            return mappedList;
        }
    }
}