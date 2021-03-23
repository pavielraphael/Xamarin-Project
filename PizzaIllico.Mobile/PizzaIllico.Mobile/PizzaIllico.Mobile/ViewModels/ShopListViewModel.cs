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
using Plugin.Settings;

namespace PizzaIllico.Mobile.ViewModels
{
    public class ShopListViewModel : ViewModelBase
    {
	    private ObservableCollection<ShopItem> _shops;
		private string _token;
		private double userLatitude;
		private double userLongitude;
		private List<long> _commandIds;
		private bool _running;

		public bool Running
		{
			get => _running;
			set => SetProperty(ref _running, value);
		}
		public ObservableCollection<ShopItem> Shops
	    {
		    get => _shops;
		    set => SetProperty(ref _shops, value);
	    }

		public List<long> CommandIds
		{
			get => _commandIds;
			set => SetProperty(ref _commandIds, value);
		}

		public ICommand SelectedCommand { get; }

		public ICommand GetUser { get; }

		public ICommand Disconnect { get; }

		public ICommand prevCommands { get; }

		public ICommand GoMap { get; }
	    public ShopListViewModel()
	    {
		    SelectedCommand = new Command<ShopItem>(SelectedAction);
			GetUser = new Command(getActualUser);
			Disconnect = new Command(deconnection);
			prevCommands = new Command(PreviousCommands);
			GoMap = new Command(AllerCarte);
	    }

		public async void AllerCarte()
        {
			Running = true;
			await NavigationService.PushAsync<Pages.MapPage>(
				new Dictionary<string, object>()
                {
					{ "Shops", Shops },
					{ "UserLatitude", userLatitude },
					{ "UserLongitude", userLongitude },
					{ "Token", Token }
                }
			);
			Running = false;
		}

		public async void PreviousCommands()
        {
			Running = true;
			await NavigationService.PushAsync<Pages.PreviousCommandsPage>(
					new Dictionary<string, object>()
					{
						{ "Token", Token }
					}
				);
			Running = false;
		}
		public async void deconnection()
        {
			Running = true;
			CrossSettings.Current.Clear();
			await NavigationService.PushAsync<Pages.ConnexionPage>();
			Running = false;
		}

	    private async void SelectedAction(ShopItem obj)
	    {
			Running = true;
			CommandIds = new List<long>();
			await NavigationService.PushAsync<Pages.PizzaListPage>(
					new Dictionary<string, object>()
					{
						{ "Shop", obj },
						{ "CommandIds", CommandIds },
						{ "Token", Token }
					}
				);
			Running = false;
		}

		public async void getActualUser()
        {
			Running = true;
			IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

			Response<UserProfileResponse> response = await service.GetUser(Token);
			
			if (response.IsSuccess)
            {
				await NavigationService.PushAsync<Pages.EditUserPage>(
					new Dictionary<string, object>()
					{
						{ "TokenU", Token }
					}
					);
			}
			Running = false;
		}

	    public override async Task OnResume()
        {
	        await base.OnResume();
			Running = true;
			await GetCurrentLocation();

			IPizzaApiService service = DependencyService.Get<IPizzaApiService>();

	        Response<List<ShopItem>> response = await service.ListShops();

	        if (response.IsSuccess)
	        {
				Shops = new ObservableCollection<ShopItem>(response.Data);
				for(int i = 0; i<Shops.Count; i++)
                {
					Shops[i].Distance = GetDistance(userLongitude, userLatitude, Shops[i].Longitude, Shops[i].Latitude);
					
				}
				sortList();
			}
			Running = false;
        }

		public void sortList()
        {
			ObservableCollection<ShopItem> d1 = Shops;
			d1 = new ObservableCollection<ShopItem>(d1.OrderBy(i => i.Distance));
			ObservableCollection<ShopItem> d2 = new ObservableCollection<ShopItem>();
			for (int i = 0; i < d1.Count; i++)
            {
				if(! d2.Any(p => p.Name == d1[i].Name))
                {
					d2.Add(d1[i]);
                }
            }
			Shops = d2;
		}

		CancellationTokenSource cts;

		async Task GetCurrentLocation()
		{
				var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
				cts = new CancellationTokenSource();
				var location = await Geolocation.GetLocationAsync(request, cts.Token);

				if (location != null)
				{
					userLatitude = location.Latitude;
					userLongitude = location.Longitude;
				}
		}

		public double GetDistance(double longitude, double latitude, double otherLongitude, double otherLatitude)
		{
			var d1 = latitude * (Math.PI / 180.0);
			var num1 = longitude * (Math.PI / 180.0);
			var d2 = otherLatitude * (Math.PI / 180.0);
			var num2 = otherLongitude * (Math.PI / 180.0) - num1;
			var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

			return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
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
			Token = GetNavigationParameter<string>("Token");
		}
	}
}