using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PizzaIllico.Mobile.Dtos;
using PizzaIllico.Mobile.Dtos.Accounts;
using PizzaIllico.Mobile.Dtos.Pizzas;
using PizzaIllico.Mobile.Services;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;
using System.Linq;
using Xamarin.Forms.Maps;

namespace PizzaIllico.Mobile.ViewModels
{
    public class MapPageModel : ViewModelBase
    {

		private ObservableCollection<ShopItem> _shops;
		private double userLatitude;
		private double userLongitude;
		private string _token;
		private List<long> _commandIds;

		public Xamarin.Forms.Maps.Map Map { get; private set; }

		public List<long> CommandIds
		{
			get => _commandIds;
			set => SetProperty(ref _commandIds, value);
		}
		public ICommand RetourMenu { get; }
		public MapPageModel()
        {
			Map = new Xamarin.Forms.Maps.Map();
			RetourMenu = new Command(RetournerMenu);
		}

		private async void RetournerMenu()
		{
			await NavigationService.PopAsync();
		}



		[NavigationParameter]
		public ObservableCollection<ShopItem> Shops
		{
			get { return _shops; }
			set { SetProperty<ObservableCollection<ShopItem>>(ref _shops, value); }
		}

		[NavigationParameter]
		public double UserLatitude
		{
			get { return userLatitude; }
			set { SetProperty<double>(ref userLatitude, value); }
		}

		[NavigationParameter]
		public double UserLongitude
		{
			get { return userLongitude; }
			set { SetProperty<double>(ref userLongitude, value); }
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
			Shops = GetNavigationParameter<ObservableCollection<ShopItem>>("Shops");
			UserLatitude = GetNavigationParameter<double>("UserLatitude");
			UserLongitude = GetNavigationParameter<double>("UserLongitude");
			Token = GetNavigationParameter<string>("Token");

			Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(UserLatitude, UserLongitude),
											 Distance.FromMiles(1)));
			var userPin = new Pin()
			{
				Position = new Position(UserLatitude, UserLongitude),
				Label = "Votre position"
			};
			Map.Pins.Add(userPin);

			for (int i = 0; i < Shops.Count; i++)
            {
				var shopPin = new Pin()
				{
					Position = new Position(Shops[i].Latitude, Shops[i].Longitude),
					Label = Shops[i].Name,
					Type = PinType.Place
				};
				ShopItem shopI = Shops[i];
				shopPin.MarkerClicked += async (s, args) =>
				{
					args.HideInfoWindow = true;
					string pinName = ((Pin)s).Label;
					CommandIds = new List<long>();
					Console.WriteLine("Click " + shopI.Name + " " + Token);

					await NavigationService.PushAsync<Pages.PizzaListPage>(
							new Dictionary<string, object>()
							{
								{ "Shop", shopI },
								{ "CommandIds", CommandIds },
								{ "Token", Token }
							}
						);
				};

				Map.Pins.Add(shopPin);
			}
		}
	}
}