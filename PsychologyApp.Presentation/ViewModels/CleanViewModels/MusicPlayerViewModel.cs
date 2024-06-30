using PsychologyApp.Presentation.Models;
using PsychologyApp.Presentation.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.Maui.Audio;
using System.Windows.Input;

namespace MobileHelper.ViewModels.CleanViewModels
{
    public class MusicPlayerViewModel : BaseViewModel
    {
        private readonly AudioManager audioManager;
        public ObservableCollection<Audio> AllItems { get; set; }
        public ObservableCollection<Audio> SelectedItems { get; set; }
        private string search_text { get; set; }

        public MusicPlayerViewModel()
        {
            this.Title = "Очиститель";

            SetCreated();

            this.AllItems = new ObservableCollection<Audio>();

            this.SelectedItems = new ObservableCollection<Audio>();

            this.Start = new Command(async () =>
            {
                SetInit();

                await Task.Run(async () => await InitAsync());
            });

            this.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;

            this.SearchText = string.Empty;

            this.audioManager = new AudioManager();
        }

        private void SelectedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.AllItems.Any()) 
            {
                SetDone();
            }
        }

        private async Task InitAsync()
        {
            SetInit();

            ObservableCollection<Audio> collection = new()
            {
                new Audio
                {
                    Name = "Молитва",
                    Description = "Андрей Смирнов",
                    File = "001_Molitva.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("001_Molitva.mp3")),
                    ClickCommand = new Command(() => ToExecute("001_Molitva.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 1",
                    Description = " Псалмы 1-8",
                    File = "002_Kafizma_1.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("002_Kafizma_1.mp3")),
                    ClickCommand = new Command(() => ToExecute("002_Kafizma_1.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 2",
                    Description = "Псалмы 9-16",
                    File = "003_Kafizma_2.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("003_Kafizma_2.mp3")),
                    ClickCommand = new Command(() => ToExecute("003_Kafizma_2.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 3",
                    Description = "Псалмы 17-23",
                    File = "004_Kafizma_3.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("004_Kafizma_3.mp3")),
                    ClickCommand = new Command(() => ToExecute("004_Kafizma_3.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 4",
                    Description = "Псалмы 24-31",
                    File = "005_Kafizma_4.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("005_Kafizma_4.mp3")),
                    ClickCommand = new Command(() => ToExecute("005_Kafizma_4.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 5",
                    Description = "Псалмы 32-36",
                    File = "006_Kafizma_5.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("006_Kafizma_5.mp3")),
                    ClickCommand = new Command(() => ToExecute("006_Kafizma_5.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 6",
                    Description = " Псалмы 37-45",
                    File = "007_Kafizma_6.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("007_Kafizma_6.mp3")),
                    ClickCommand = new Command(() => ToExecute("007_Kafizma_6.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 7",
                    Description = "Псалмы 46-54",
                    File = "008_Kafizma_7.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("008_Kafizma_7.mp3")),
                    ClickCommand = new Command(() => ToExecute("008_Kafizma_7.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 8",
                    Description = "Псалмы 55-63",
                    File = "009_Kafizma_8.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("009_Kafizma_8.mp3")),
                    ClickCommand = new Command(() => ToExecute("009_Kafizma_8.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 9",
                    Description = "Псалмы 64-69",
                    File = "010_Kafizma_9.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("010_Kafizma_9.mp3")),
                    ClickCommand = new Command(() => ToExecute("010_Kafizma_9.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 10",
                    Description = "Псалмы 70-76",
                    File = "011_Kafizma_10.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("011_Kafizma_10.mp3")),
                    ClickCommand = new Command(() => ToExecute("011_Kafizma_10.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 11",
                    Description = " Псалмы 77-84",
                    File = "012_Kafizma_11.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("012_Kafizma_11.mp3")),
                    ClickCommand = new Command(() => ToExecute("012_Kafizma_11.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 12",
                    Description = "Псалмы 85-90",
                    File = "013_Kafizma_12.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("013_Kafizma_12.mp3")),
                    ClickCommand = new Command(() => ToExecute("013_Kafizma_12.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 13",
                    Description = "Псалмы 91-100",
                    File = "014_Kafizma_13.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("014_Kafizma_13.mp3")),
                    ClickCommand = new Command(() => ToExecute("014_Kafizma_13.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 14",
                    Description = "Псалмы 101-104",
                    File = "015_Kafizma_14.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("015_Kafizma_14.mp3")),
                    ClickCommand = new Command(() => ToExecute("015_Kafizma_14.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 15",
                    Description = "Псалмы 105-108",
                    File = "016_Kafizma_15.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("016_Kafizma_15.mp3")),
                    ClickCommand = new Command(() => ToExecute("016_Kafizma_15.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 16",
                    Description = "Псалмы 109-117",
                    File = "017_Kafizma_16.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("017_Kafizma_16.mp3")),
                    ClickCommand = new Command(() => ToExecute("017_Kafizma_16.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 17",
                    Description = "Псалом 118",
                    File = "018_Kafizma_17.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("018_Kafizma_17.mp3")),
                    ClickCommand = new Command(() => ToExecute("018_Kafizma_17.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 18",
                    Description = "Псалмы 119-133",
                    File = "019_Kafizma_18.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("019_Kafizma_18.mp3")),
                    ClickCommand = new Command(() => ToExecute("019_Kafizma_18.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 19",
                    Description = "Псалмы 134-142",
                    File = "020_Kafizma_19.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("020_Kafizma_19.mp3")),
                    ClickCommand = new Command(() => ToExecute("020_Kafizma_19.mp3"))
                },

                new Audio
                {
                    Name = "Кафизма 20",
                    Description = "Псалмы 143-151",
                    File = "021_Kafizma_20.mp3",
                    AudioPlayer = this.audioManager.CreatePlayer(
                        await FileSystem.OpenAppPackageFileAsync("021_Kafizma_20.mp3")),
                    ClickCommand = new Command(() => ToExecute("021_Kafizma_20.mp3"))
                },
            };

            InitItems(collection);
        }

        private void InitItems(ObservableCollection<Audio> collection)
        {
            foreach (Audio item in collection)
            {
                this.SelectedItems.Add(item);

                this.AllItems.Add(item);
            }
        }

        private void HideAll()
        {
            for (int i = 0; i < this.SelectedItems.Count; i++)
            {
                Audio element = this.SelectedItems.ElementAt(i);

                element.AudioPlayer?.Stop();

                this.SelectedItems[i] = element;
            }
        }

        public void ToExecute(string file)
        {
            HideAll();

            Audio item = this.SelectedItems.FirstOrDefault(x => x.File == file);

            int index = this.SelectedItems.IndexOf(item);

            if (!item.IsPlaying)
            {
                item.AudioPlayer.Play();
            }

            item.IsPlaying = !item.IsPlaying;
        }

        public string SearchText
        {
            get => this.search_text;
            set
            {
                if (this.search_text != value)
                {
                    this.search_text = value;
                    OnPropertyChanged(nameof(this.SearchText));
                }
            }
        }
    }
}
