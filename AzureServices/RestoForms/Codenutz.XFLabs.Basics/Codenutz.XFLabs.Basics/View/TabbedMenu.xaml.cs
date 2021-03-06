﻿using Codenutz.XFLabs.Basics.DAL;
using Codenutz.XFLabs.Basics.Model;
using Codenutz.XFLabs.Basics.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs;

namespace Codenutz.XFLabs.Basics.View
{
    public partial class TabbedMenu : TabbedPage
    {
        private MenuViewModel viewModel;
        private string _RestoTitle { get; set; }
        public ToolbarItem AboutUs { get; set; }
        public ToolbarItem Cart { get; set; }
        public ToolbarItem Reservation { get; set; }
        public int StoreId { get; set; }
        public string RestoTitle
        {
            get { return _RestoTitle;}
            set { _RestoTitle = value; }
        }

        public TabbedMenu(string restoName, int storeId)
        {
            this.Title = restoName;
            InitializeComponent();
            RestoTitle = restoName;
            StoreId = storeId;
            string platformName = Device.OS.ToString();
            
            AboutUs = new ToolbarItem
            {
                Name = "About Us",
                Order = ToolbarItemOrder.Primary,
                Icon = "ic_action_info.png",
                Priority = 2,
                Command = new Command(() => this.LoadStorePage(restoName)),
            };

            Cart = new ToolbarItem
            {
                Name = "Cart",
                Order = ToolbarItemOrder.Primary,
                Priority = 1,
                Icon = "ic_action_cart.png",
                Command = new Command( () =>  Navigation.PushAsync(new OrderDetails(RestoTitle,StoreId)))
            };

            Reservation = new ToolbarItem
            {
                Name = "ReserveTable",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
                Icon = "ic_action_calendar_day.png",
                Command = new Command(() => this.ReserveTable(restoName))
            };

            this.ToolbarItems.Add(AboutUs);
            this.ToolbarItems.Add(Cart);
            this.ToolbarItems.Add(Reservation);
			
            BindingContext = viewModel = new MenuViewModel(this, RestoTitle,StoreId);
		}

        public async void ReserveTable(string restoName)
        {
            await Navigation.PushAsync(new ReserveTable(restoName));
        }

        /// <summary>
        /// Call Reserve Table page with specific restaurant page name;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnReserveTableClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReserveTable(_RestoTitle));
        }

        public async void OnItemSelected(object sender, ItemTappedEventArgs args)
        {
           

            var menuItem = args.Item as MenuDAO;
            if (menuItem == null)
                return;
            try
            {
                ((ListView)sender).SelectedItem = null; //unselect the list item;

                Navigation.PushAsync(new MenuItemDetailPage(RestoTitle, menuItem));
            }
            catch (Exception ex)
            {
                DisplayAlert("Help", "Error loading tabbed view", "OK");
            }

            //this.list.SelectedItem = null;
        }

        public async void RemoveClicked(object sender, EventArgs args)
        {
            var item = (Button)sender;
            var parameter = item.CommandParameter.ToString();

        }

        public async void AddClicked(object sender, EventArgs args)
        {
            var item = (Button)sender;
            var parameter = item.CommandParameter.ToString();
        }

        #region dataset_StorePage
        private async Task<int> LoadStorePage(string storeName)
        {
            string d1_dessert_v1 = "https://lh3.googleusercontent.com/-Xh8aY2RRwd0/VeFCHSv2lzI/AAAAAAAAAOY/8SY3a6qk7WM/d1_icecream.png";

            var closeTime = "11pm";
            var openTime = "11am";
            var store = new Store()
            {
                Name = storeName,
                City = "Auckland",
                Country = "New Zealand",


                Latitude = -36.891932,
                LocationCode = "Av pres Rouque Saenz Pena 875",
                LocationHint = "Buenos Alres C1035AAD",
                Longitude = 174.736355,
                MondayClose = closeTime,
                TuesdayClose = closeTime,
                WednesdayClose = closeTime,
                ThursdayClose = closeTime,
                FridayClose = closeTime,
                SaturdayClose = closeTime,
                SundayClose = closeTime,

                MondayOpen = openTime,
                TuesdayOpen = openTime,
                WednesdayOpen = openTime,
                ThursdayOpen = openTime,
                FridayOpen = openTime,
                SaturdayOpen = openTime,
                SundayOpen = openTime,

                Version = "11",
                PhoneNumber = "0220778677",
                State = "Auckland",
                StreetAddress = "12 Sandringham",
                ZipCode = "1065",
                ImageUri = new Uri(d1_dessert_v1),
                Image = d1_dessert_v1

            };

            await Navigation.PushAsync(new StorePage(store));
            return 1;
        }
        #endregion

        public async void OnCheckBoxChanged(object sender, EventArgs<bool> eventArgs)
        {
            this.ToolbarItems.Remove(Cart);

            if (eventArgs.Value)
            {
                Cart = new ToolbarItem
                {
                    Name = "Cart",
                    Order = ToolbarItemOrder.Primary,
                    Priority = 1,
                    Icon = "cartfilled.png",
                    Command = new Command(() => this.LoadStorePage(RestoTitle))
                };
                this.ToolbarItems.Add(Cart);
            }
            else
            {
                Cart = new ToolbarItem
                {
                    Name = "Cart",
                    Order = ToolbarItemOrder.Primary,
                    Priority = 1,
                    Icon = "scart48.png",
                    Command = new Command(() => this.LoadStorePage(RestoTitle))
                };
                this.ToolbarItems.Add(Cart);
            }

            

        }

		

		protected override void OnAppearing()
		{
			base.OnAppearing();
			//Part I
			//if (viewModel.MenuCollection.Count > 0 || viewModel.IsBusy)
			//	return;

			//viewModel.GetMenuList.Execute(null);
			/** Part II **/
			var currentTabPage = this.CurrentPage;
			var selectedTab = this.SelectedItem;
			var loadOnDemand = false;
			if (viewModel.MenuCollection.Count > 0 || viewModel.IsBusy)
			{
				loadOnDemand = true;
			}

			if (loadOnDemand)
			{
				//Fire a background task;
				Task.Run(() => 
				{
					UpdatePageOnDemand(350); //specify no of milliseconds for referesh
				});
				return;
			}
			else
			{
				UpdatePageOnDemand(0);//specify no of milliseconds for referesh
			} 
		}

		/// <summary>
		/// Updates the page on popupasync load.
		/// This is in place because on PopUpAsync will not update the page straight away. Hence Has to delay for few seconds.
		/// </summary>
		/// <param name="seconds">wait number of milliseconds to referesh the page</param>
		public async void UpdatePageOnDemand(int seconds)
		{
			//keep the current selected tab reference;
			var selectedPage = this.CurrentPage; 
			await Task.Delay(seconds);
			viewModel.GetMenuList.Execute(null);

			if (selectedPage != null && this.CurrentPage.Id != selectedPage.Id)
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					//Make sure after page referesh, show the same selected tab page;
					this.CurrentPage = selectedPage;
				});
			}

		}

	}
}
