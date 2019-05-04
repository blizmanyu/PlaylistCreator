using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yutaka.IO;

namespace PlaylistCreator
{
	public class Playlist
	{
		public enum PType { English, JPopSpringSummer, JPopFallWinter };
		public PType Type;
		public string Name;
		public List<SongFileInfo> GoodEnglishSongs = new List<SongFileInfo> {
			new SongFileInfo("artist", "title"),
		};
		public List<SongFileInfo> GoodJPopSpringSummerSongs = new List<SongFileInfo> {
			new SongFileInfo("artist", "title"),
		};
		public List<SongFileInfo> GoodJPopFallWinterSongs = new List<SongFileInfo> {
			new SongFileInfo("artist", "title"),
		};
		public DateTime NewSongThreshold;
		public string SourceFolder; // where the music files are //
		public string DestinationFolder; // where you want the playlist to go //

		protected FileUtil _fileUtil;
		protected HashSet<string> FolderExclusions;
		protected List<SongFileInfo> AllSongs = new List<SongFileInfo>();
		protected List<SongFileInfo> GoodList = new List<SongFileInfo>();
		protected List<SongFileInfo> NewList = new List<SongFileInfo>();
		protected List<SongFileInfo> NewPlusGoodList = new List<SongFileInfo>();
		protected List<SongFileInfo> ThePlaylist = new List<SongFileInfo>();

		public Playlist(PType type, string name=null, bool createPlaylistNow=false)
		{
			if (String.IsNullOrWhiteSpace(name))
				name = type.ToString();

			Type = type;
			Name = name;
			NewSongThreshold = DateTime.Now.AddYears(-2);
			SourceFolder = @"C:\Music\00 Genres\";
			DestinationFolder = @"C:\Music\01 Playlists\";
			_fileUtil = new FileUtil();

			if (createPlaylistNow)
				Create();
		}

		public void Create()
		{
			List<string> files;
			switch (Type) {
				#region English
				case PType.English:
					FolderExclusions = new HashSet<string>() { @"\Album", @"\Classical", @"\J-Pop", @"\J-Rap", @"\Spanish", };

					// Step 1: Get all songs //
					files = _fileUtil.GetAllAudioFiles(SourceFolder, FolderExclusions.ToArray());
					for (int i = 0; i < files.Count; i++)
						AllSongs.Add(new SongFileInfo(files[i]));

					// Step 2: Create GoodList //

					// Step 3: Create NewList //

					// Step 4: Create NewPlusGoodList //

					// Step 5: Create ThePlaylist //

					break;
				#endregion English
				#region JPopSpringSummer
				case PType.JPopSpringSummer:
					FolderExclusions = new HashSet<string>() { @"\Album", @"\Classical", @"\J-Pop", @"\J-Rap", @"\Spanish", };

					// Step 1: Get all songs //
					files = _fileUtil.GetAllAudioFiles(SourceFolder, FolderExclusions.ToArray());
					for (int i = 0; i < files.Count; i++)
						AllSongs.Add(new SongFileInfo(files[i]));

					// Step 2: Create GoodList //

					// Step 3: Create NewList //

					// Step 4: Create NewPlusGoodList //

					// Step 5: Create ThePlaylist //

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

		public void WriteForWinamp()
		{

		}
	}
}