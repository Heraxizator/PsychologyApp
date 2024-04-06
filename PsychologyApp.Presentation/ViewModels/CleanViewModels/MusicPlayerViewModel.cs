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

            this.Start = new Command(() =>
            {
                SetInit();

                Task.Run(async () => await InitAsync());
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

        private void ToSearch(string input)
        {
            if (this.SelectedItems == null)
            {
                return;
            }

            this.SelectedItems.Clear();

            string target = input.ToUpper();

            foreach (Audio item in this.AllItems)
            {
                string? name = item.Name?.ToUpper();

                string? describtion = item.Description?.ToUpper();

                if (name.Contains(target) || describtion.Contains(target))
                {
                    this.SelectedItems.Add(item);
                }
            }
        }

        private void ToExecute(string file)
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
                    ToSearch(this.search_text);
                    OnPropertyChanged(nameof(this.SearchText));
                }
            }
        }
    }
}
