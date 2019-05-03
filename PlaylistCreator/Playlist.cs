using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			if (createPlaylistNow)
				Create();
		}

		public void Create()
		{
			switch (Type) {
				#region JPopSpringSummer
				case PType.JPopSpringSummer:
					// Step 1: Get all songs //

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