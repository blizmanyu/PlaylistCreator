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

		public Playlist(PType type, string name=null)
		{
			if (String.IsNullOrWhiteSpace(name))
				name = type.ToString();

			Type = type;
			Name = name;
		}

		public void Create()
		{
			switch (Type) {
				#region JPopSpringSummer
				case PType.JPopSpringSummer:
					// Step 1: Get all songs //

					// Step 2: Create "New" list //

					// Step 3: Create "Good" list //

					// Step 4: Create New + Good list //

					// Step 5: Create playlist //

					// Step 6: Write playlist //

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