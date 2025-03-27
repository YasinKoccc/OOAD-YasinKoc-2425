using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ExcelDataReader;

namespace WpfPlaylist
{
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private List<Song> songs = new List<Song>();
        private List<Artist> artists = new List<Artist>();
        private Song currentSong = null;
        private bool isPlaying = false;

        public MainWindow()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            LoadDataFromExcel();
            PopulatePlaylist();
            btnStop.IsEnabled = false;

            lstPlaylist.SelectionChanged += lstPlaylist_SelectionChanged;
            mediaPlayer.MediaEnded += mediaPlayer_MediaEnded;
        }

        private void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                txtNowPlaying.Text = "Playback completed";
                btnPlay.IsEnabled = true;
                btnStop.IsEnabled = false;
                isPlaying = false;
            });
        }

        private void LoadDataFromExcel()
        {
            try
            {
                string excelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WpfPlaylistData.xlsx");

                if (!File.Exists(excelPath))
                {
                    MessageBox.Show($"Excel bestand niet gevonden op: {excelPath}");
                    return;
                }

                using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });

                        // Artiesten inlezen
                        DataTable artistsTable = result.Tables[0];
                        artists = artistsTable.AsEnumerable().Select(row => new Artist
                        {
                            Name = row.Field<string>("Name"),
                            BirthDate = row.Field<DateTime>("Birthdate"),
                            Biography = row.Field<string>("Bio"),
                            ImagePath = Path.Combine("foto's", row.Field<string>("Photo"))
                        }).Distinct().ToList();

                        // Liedjes inlezen
                        DataTable songsTable = result.Tables[1];
                        songs = songsTable.AsEnumerable().Select(row =>
                        {
                            string artistName = row.Field<string>("Artist").ToLower();
                            var artist = artists.FirstOrDefault(a => a.Name.ToLower().Contains(artistName));

                            return new Song
                            {
                                Name = row.Field<string>("Name"),
                                Artist = artist,
                                Year = Convert.ToInt32(row["Year"]),
                                Duration = ParseDuration(row["Duration"]),
                                FilePath = Path.Combine("muziek", row.Field<string>("Mp3"))
                            };
                        }).GroupBy(s => s.Name).Select(g => g.First()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden Excel: {ex.Message}");
            }
        }

        private TimeSpan ParseDuration(object duration)
        {
            return TimeSpan.FromMinutes(1);
        }

        private void PopulatePlaylist()
        {
            lstPlaylist.ItemsSource = null;
            lstPlaylist.ItemsSource = songs;
        }

        private void lstPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPlaylist.SelectedItem is Song selectedSong)
            {
                currentSong = selectedSong;
                UpdateArtistInfo(selectedSong.Artist);
                btnPlay.IsEnabled = true;
                btnStop.IsEnabled = isPlaying;
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (currentSong == null) return;

            try
            {
                mediaPlayer.Stop();
                string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, currentSong.FilePath);

                if (!File.Exists(musicPath))
                {
                    MessageBox.Show($"Bestand niet gevonden: {musicPath}\n\nZorg dat de map 'muziek' met MP3-bestanden in de uitvoermap staat.");
                    return;
                }

                mediaPlayer.Open(new Uri(musicPath));
                mediaPlayer.Play();
                isPlaying = true;
                txtNowPlaying.Text = $"Nu speelt: {currentSong.Name} - {currentSong.Artist.Name}";
                btnPlay.IsEnabled = false;
                btnStop.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Afspelen mislukt: {ex.Message}\n\nDetails: {ex.ToString()}");
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            isPlaying = false;
            txtNowPlaying.Text = "Gestopt";
            btnPlay.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            songs = songs.OrderBy(x => rnd.Next()).ToList();
            PopulatePlaylist();
            txtNowPlaying.Text = "Playlist is shuffled";
        }

        private void UpdateArtistInfo(Artist artist)
        {
            try
            {
                if (artist == null) return;

                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, artist.ImagePath);
                if (File.Exists(imagePath))
                {
                    imgArtist.Source = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    MessageBox.Show($"Afbeelding niet gevonden: {imagePath}");
                }

                txtArtistName.Text = artist.Name;
                txtArtistBio.Text = $"{artist.Name} (*{artist.BirthDate:dd/MM/yyyy})\n\n{artist.Biography}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij laden artiestinfo: {ex.Message}");
            }
        }
    }
}