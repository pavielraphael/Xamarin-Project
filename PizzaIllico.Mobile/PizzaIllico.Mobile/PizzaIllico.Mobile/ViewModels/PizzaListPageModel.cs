using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;

namespace PizzaIllico.Mobile.ViewModels
{
    public class PizzaListPageModel : ViewModelBase
    {

        private ObservableCollection<PizzaItem> _pizzas;
        private ShopItem _shop;
        private string _token;
        private List<long> _commandIds;
        private ObservableCollection<PizzaItem> _pizzasCommand;
        private bool _running;

        public ObservableCollection<PizzaItem> Pizzas
        {
            get => _pizzas;
            set => SetProperty(ref _pizzas, value);
        }

        public bool Running
        {
            get => _running;
            set => SetProperty(ref _running, value);
        }
        public ICommand SelectedCommand { get; }
        public ICommand RetourMenu { get; }
        public ICommand GoPanier { get; }
        public PizzaListPageModel()
        {
            SelectedCommand = new Command<PizzaItem>(SelectedAction);
            RetourMenu = new Command(RetournerMenu);
            GoPanier = new Command(AllerAuPanier);
        }

        private async void AllerAuPanier()
        {
            Running = true;
            commandIdsToPizzas();
            
            await NavigationService.PushAsync<Pages.PanierPage>(
                new Dictionary<string, object>()
                {
                                { "Token", Token },
                                { "CommandIds", CommandIds },
                                { "CommandPizzas", _pizzasCommand },
                                { "Shop", Shop }
                }
            );
            Running = false;
        }
        private async void RetournerMenu()
        {
            Running = true;
            await NavigationService.PushAsync<Pages.ShopListPage>(
                new Dictionary<string, object>()
                {
                                { "Token", Token }
                }
            );
            Running = false;
        }
        private async void SelectedAction(PizzaItem obj)
        {
            Running = true;
            await NavigationService.PushAsync<Pages.PizzaDetailPage>(
                new Dictionary<string, object>()
                {
                                { "Shop", Shop },
                                { "Pizza", obj },
                                { "CommandIds", CommandIds },
                                { "Token", Token }
                }
            );
            Running = false;
        }

        

        private void commandIdsToPizzas()
        {
            for (int i = 0; i < CommandIds.Count; i++)
            {
                for (int j = 0; j < Pizzas.Count; j++)
                {
                    if (Pizzas[j].Id == CommandIds[i])
                    {
                        _pizzasCommand.Add(Pizzas[j]);
                        Console.WriteLine("Pizza : " + Pizzas[j].Name);

                    }
                }
            }
        }

        [NavigationParameter]
        public List<long> CommandIds
        {
            get { return _commandIds; }
            set { SetProperty<List<long>>(ref _commandIds, value); }
        }

        [NavigationParameter]
        public ShopItem Shop
        {
            get { return _shop; }
            set { SetProperty<ShopItem>(ref _shop, value); }
        }

        [NavigationParameter]
        public string Token
        {
            get { return _token; }
            set { SetProperty<string>(ref _token, value); }
        }


        public override async void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            Running = true;
            Shop = GetNavigationParameter<ShopItem>("Shop");
            CommandIds = GetNavigationParameter<List<long>>("CommandIds");
            Token = GetNavigationParameter<string>("Token");
            _pizzasCommand = new ObservableCollection<PizzaItem>();
            Console.WriteLine("Shop selectionné : " + Shop.Id + " " + Shop.Name);

            IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

            Response<List<PizzaItem>> response = await service.ListPizzas(Shop.Id);
            if (response.IsSuccess)
            {
                Pizzas = new ObservableCollection<PizzaItem>(response.Data);
            }
            Running = false;

        }

    }
}