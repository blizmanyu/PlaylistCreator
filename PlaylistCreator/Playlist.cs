using System;
using System.Collections.Generic;
using System.Linq;
using Yutaka.IO;

namespace PlaylistCreator
{
	public class Playlist
	{
		public enum PType { English, JPopSpringSummer, JPopFallWinter };
		public PType Type;
		public string Name;
		public HashSet<string> GoodSongsEnglish = new HashSet<string> {
			"asdfasdf",
		};
		public HashSet<string> GoodSongsJPopFallWinter = new HashSet<string> {
			"asdfasdf",
		};
		public HashSet<string> GoodSongsJPopSpringSummer = new HashSet<string>() {
			"Floatin'", // Chemistry //
			"It Takes Two", // Chemistry //
			"Pieces of a Dream", // Chemistry //
			"Point of No Return", // Chemistry //
			"fukai mori", // Do As Infinity //
			"Oasis", // Do As Infinity //
			"Under the Moon", // Do As Infinity //
			"Yesterday & Today", // Do As Infinity //
			"grateful days", // Dragon Ash //
			"your eyes only", // Exile //
			"Be With You", // Glay //
			"Beloved", // Glay //
			"However", // Glay //
			"Kiseki no Hate", // Glay //
			"Pure Soul", // Glay //
			"Face", // Globe //
			"Faces Places", // Globe //
			"Perfume of Love", // Globe //
			"asu e no tobira", // I WiSH //
			"Everything (It's you)", // Mr.Children //
			"hana", // Mr.Children //
			"kuchibue", // Mr.Children //
			"kurumi", // Mr.Children //
			"mirai", // Mr.Children //
			"namonaki uta", // Mr.Children //
			"owarinaki tabi", // Mr.Children //
			"te no hira", // Mr.Children //
			"yasashii uta", // Mr.Children //
			"youthful days", // Mr.Children //
			"fly", // Smap //
			"Sekai ga Owaru Madewa", // Wands //
			"Sekaijuu no Dare Yori Kitto", // Wands //
		};
		public DateTime NewSongThreshold;
		public string SourceFolder; // where the music files are //

		private FileUtil _fileUtil;
		private List<string> IgnoreListFolders;
		private List<SongFileInfo> AllSongs = new List<SongFileInfo>();
		private List<SongFileInfo> GoodList = new List<SongFileInfo>();
		private List<SongFileInfo> NewList = new List<SongFileInfo>();
		private List<SongFileInfo> NewPlusGoodList = new List<SongFileInfo>();
		private List<SongFileInfo> ThePlaylist = new List<SongFileInfo>();
		private int AllSongsCount = 0;
		private int GoodListCount = 0;
		private int NewListCount = 0;
		private int NewPlusGoodListCount = 0;
		private int ThePlaylistCount = 0;

		public Playlist(PType type, string name=null, bool createPlaylistNow=false)
		{
			if (String.IsNullOrWhiteSpace(name))
				name = type.ToString();

			Type = type;
			Name = name;
			NewSongThreshold = DateTime.Now.AddYears(-2);
			SourceFolder = @"C:\Music\00 Genres\";
			_fileUtil = new FileUtil();

			if (createPlaylistNow)
				Create();
		}

		public void Create()
		{
			List<string> files;
			int goodInd;
			string folder;
			string[] exclusions;

			switch (Type) {
				#region JPopSpringSummer
				case PType.JPopSpringSummer:
					folder = @"C:\Music\00 Genres\J-Pop\";
					exclusions = new string[] { @"\_Album", @"_Christmas", @"_FallWinter" };

					// Step 1: Get all songs //
					files = _fileUtil.GetAllAudioFiles(folder, exclusions);
					for (int i = 0; i < files.Count; i++)
						AllSongs.Add(new SongFileInfo(files[i]));

					AllSongsCount = AllSongs.Count;

					// Step 2: Create GoodList //
					GoodList = AllSongs.Where(x => GoodSongsJPopSpringSummer.Contains(x.Title, StringComparer.OrdinalIgnoreCase)).OrderBy(x => x.Title).ThenBy(y => y.Artist).ToList();
					GoodListCount = GoodList.Count;

					// Step 3: Create NewList // Ignored for J-Pop lists //
					// Step 4: Create NewPlusGoodList // Ignored for J-Pop lists //

					// Step 5: Create ThePlaylist //
					AllSongs = AllSongs.Except(GoodList).ToList();
					AllSongs = AllSongs.OrderBy(x => x.Title).ThenBy(y => y.Artist).ToList();
					goodInd = 0;

					for (int i = 0; i < AllSongs.Count; i++) {
						if (goodInd == GoodListCount)
							goodInd = 0;

						ThePlaylist.Add(GoodList[goodInd++]);
						ThePlaylist.Add(AllSongs[i++]);

						try {
							ThePlaylist.Add(AllSongs[i++]);
						}
						catch (Exception) {
							break;
						}

						try {
							ThePlaylist.Add(AllSongs[i]);
						}
						catch (Exception) {
							break;
						}
					}
					break;
				#endregion JPopSpringSummer
				default:
					break;
			}
		}

		public void WriteForBrowser()
		{

		}

		public void WriteForITunes()
		{

		}

		public void WriteForWinamp(string destFolder)
		{
			var dest = String.Format("{0}{1}{2:yyyy MMdd HHmm ssff}.m3u", destFolder, Name, DateTime.Now);
			var content = "#EXTM3U";

			foreach (var song in ThePlaylist) {
				content += String.Format("\n#EXTINF:{0},{1} - {2}", song.Duration, song.Artist, song.Title);
				content += String.Format("\n{0}", song.Path);
			}

			content += "\n";
			_fileUtil.Write(content, dest);
		}
	}
}