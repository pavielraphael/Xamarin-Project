using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Pizzas;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.Services
{
    public interface IPizzaApiService
    {
        Task<Response<List<ShopItem>>> ListShops();

        Task<Response<List<PizzaItem>>> ListPizzas(int shopId);

        Task<string> CreateUser(string myJson);

        Task<string> Login(string myJson);

        Task<Response<UserProfileResponse>> GetUser(string token);

        Task<Response<UserProfileResponse>> AccountPatch(string myJson, string token);

        Task<string> PasswordPatch(string myJson, string token);

        Task<Response<List<PizzaItem>>> ListPizzas(long idShop);

        Task<string> PostCommand(string token, long idShop, string myJson);

        Task<Response<List<OrderItem>>> ListOrders(string token);

    }
    
    public class PizzaApiService : IPizzaApiService
    {
        private readonly IApiService _apiService;

        public PizzaApiService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<Response<List<ShopItem>>> ListShops()
        {
            return await _apiService.Get<Response<List<ShopItem>>>(Urls.LIST_SHOPS);
        }

        public async Task<Response<List<PizzaItem>>> ListPizzas(long idShop)
        {
            return await _apiService.GetPizzas<Response<List<PizzaItem>>>(Urls.LIST_PIZZA, idShop);
        }

        public async Task<Response<List<OrderItem>>> ListOrders(string token)
        {
            return await _apiService.GetOrders<Response<List<OrderItem>>>(Urls.LIST_ORDERS, token);
        }

        public async Task<string> CreateUser(string myJson)
        {
            return await _apiService.Post<Response<string>>(Urls.CREATE_USER, myJson);
        }

        public async Task<string> Login(string myJson)
        {
            return await _apiService.PostLogin<Response<string>>(Urls.LOGIN_WITH_CREDENTIALS, myJson);
        }

        public async Task<string> PostCommand(string token, long idShop, string myJson)
        {
            return await _apiService.PostCommand<Response<string>>(Urls.DO_ORDER, token, idShop, myJson);
        }

        public async Task<Response<UserProfileResponse>> GetUser(string token)
        {
            return await _apiService.GetGetUser<Response<UserProfileResponse>>(Urls.USER_PROFILE, token);
        }

        public async Task<Response<UserProfileResponse>> AccountPatch(string myJson, string token)
        {
            return await _apiService.PatchAccount<Response<UserProfileResponse>>(Urls.SET_USER_PROFILE, myJson, token);
        }

        public async Task<string> PasswordPatch(string myJson, string token)
        {
            return await _apiService.PatchPwd<Response<string>>(Urls.SET_PASSWORD, myJson, token);
        }

        public Task<Response<List<PizzaItem>>> ListPizzas(int shopId)
        {
            throw new NotImplementedException();
        }
    }
}