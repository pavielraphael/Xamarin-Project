using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;


namespace PizzaIllico.Mobile.ViewModels
{
    public class PizzaDetailPageModel : ViewModelBase
    {

        private long _id;
        private string _name;
        private string _description;
        private double _price;
        private bool _oos;
        private PizzaItem _pizza;
        private ShopItem _shop;
        private string _imageSource;
        private List<long> _commandIds;
        private string _token;
        private string _hs;
        private string _hsColor;
        private bool _running;
        public bool Running
        {
            get => _running;
            set => SetProperty(ref _running, value);
        }
        public long Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Hs
        {
            get => _hs;
            set => SetProperty(ref _hs, value);
        }

        public string HsColor
        {
            get => _hsColor;
            set => SetProperty(ref _hsColor, value);
        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        public double Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }
        public bool Oos
        {
            get => _oos;
            set => SetProperty(ref _oos, value);
        }
        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public ICommand AjouterPizza { get; }
        public PizzaDetailPageModel()
        {
            AjouterPizza = new Command(AjouterUnePizza);
        }

        public async void AjouterUnePizza()
        {
            Running = true;
            if (Oos == false)
            {
                CommandIds.Add(Id);
                await NavigationService.PushAsync<Pages.PizzaListPage>(
                    new Dictionary<string, object>()
                        {
                            { "Shop", Shop },
                            { "CommandIds", CommandIds },
                            { "Token", Token }
                        }
                    );
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Attention", "La pizza n'est pas en stock", "OK");

            }
            Running = false;
        }

        [NavigationParameter]
        public List<long> CommandIds
        {
            get { return _commandIds; }
            set { SetProperty<List<long>>(ref _commandIds, value); }
        }

        [NavigationParameter]
        public PizzaItem Pizza
        {
            get { return _pizza; }
            set { SetProperty<PizzaItem>(ref _pizza, value); }
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

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            Running = true;
            Pizza = GetNavigationParameter<PizzaItem>("Pizza");
            Shop = GetNavigationParameter<ShopItem>("Shop");
            CommandIds = GetNavigationParameter<List<long>>("CommandIds");
            Token = GetNavigationParameter<string>("Token");

            Id = Pizza.Id;
            Name = Pizza.Name;
            Price = Pizza.Price;
            Description = Pizza.Description;
            if(Pizza.OutOfStock == false)
            {
                Hs = "En stock !";
                HsColor = "Green";
            }
            else
            {
                Hs = "Hors de stock !";
                HsColor = "Red";
            }
            Oos = Pizza.OutOfStock;

            ImageSource = "https://pizza.julienmialon.ovh/" + Urls.GET_IMAGE.Replace("{shopId}", Shop.Id.ToString()).Replace("{pizzaId}", Id.ToString());
            Running = false;
        }

    }
}