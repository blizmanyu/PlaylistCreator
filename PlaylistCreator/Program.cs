﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PlaylistCreator
{
	class Program
	{
		// Config/Settings //
		const string srcFolder = @"C:\Music\00 Genres\";
		const string playlistFolder = @"C:\Music\01 Playlists\";
		private static bool consoleOut = false; // default = false
		private static DateTime newSongThreshold = DateTime.Now.AddYears(-4);
		private static HashSet<string> supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mp3", ".m4a", ".wma" };

		#region Fields
		const string EN_US = @"M/d/yyyy h:mmtt";
		private static List<SongFileInfo> playlist = new List<SongFileInfo>();
		private static List<SongFileInfo> allSongs = new List<SongFileInfo>();
		private static List<SongFileInfo> goodList = new List<SongFileInfo>();
		private static List<SongFileInfo> newList = new List<SongFileInfo>();
		private static List<SongFileInfo> newPlusGoodList = new List<SongFileInfo>();
		private static DateTime startTime = DateTime.Now;

		#region Good Songs
		private static HashSet<string> goodSongs = new HashSet<string>() {
			"All The Shine",
			"All Of Me",
			"Blank Space",
			"Breakeven",
			"Clarity",
			"Different",
			"Everybody Knows",
			"Feels Like Home",
			"first day of my life", // gnash //
			"Give A Little More",
			"Hands Down (Acoustic)",
			"Heartbeat",
			"i hate you, i love you (PBH & Jack Shizzle Remix)",
			"It Might Be You",
			"Like A Stone",
			"Lost & Found",
			"Love Lost",
			"Must Get Out",
			"Never Gonna Leave This Bed (Acoustic)",
			"Never Too Late",
			"nothing lasts forever",
			"One Last Time",
			"Ordinary People",
			"Our Youth",
			"Panic Switch",
			"Piece Of Me",
			"Say You Love Me",
			"Style",
			"Style (Cover)",
			"Sunday Morning",
			"Sweetest Goodbye",
			"The Sun",
			"Thinkin Bout You",
			"Through With You",
			"Who You Are",
			"Wildest Dreams",
			"Wildest Moments",
			"With Or Without You",
			"won't go home without you",
		};
		#endregion

		#region Folder Exclusions
		private static HashSet<string> folderExclusions = new HashSet<string>() {
			"Album",
			"Classical",
			"J-Pop",
			"J-Rap",
			"Spanish",
		};
		#endregion

		#region Exclude Artists
		private static HashSet<string> badArtists = new HashSet<string>() {
			"Britney Spears",
			"Felix Mendelssohn",
			"Justin Bieber",
			"Ke$ha",
			"Miley Cyrus",
			"Sergey Rachmaninov",
			"Wolfgang Amadeus Mozart",
		};
		#endregion
		#endregion Fields

		static void Main(string[] args)
		{
			StartProgram(args);
			CheckFolders();
			GetAllSongs();
			CreateGoodList();
			RemoveExclusionArtists();
			CreateNewList();
			CreateNewPlusGoodList();
			var writeGoodList = true;
			#region Sort & Create Good List
			if (writeGoodList) {
				goodList = goodList.OrderBy(x => x.Title).ThenBy(y => y.AlbumArtist).ToList();
				WritePlaylistM3U(goodList, "Good");
				WritePlaylistITunes(goodList, "Good");
			}
			#endregion
			CreatePlaylist();
			WritePlaylistM3U(playlist, "All");
			WritePlaylistITunes(playlist, "All");
			WriteHtmlFile(playlist, "All");
			Process.Start("explorer.exe", playlistFolder);
			EndProgram();
		}

		#region Methods
		private static void CheckFolders()
		{
			string[] folders = { @"C:\Music\", @"C:\Music\01 Playlists" };

			for (int i=0; i<folders.Length; i++)
				if (!Directory.Exists(folders[i]))
					Directory.CreateDirectory(folders[i]);
		}

		private static void GetAllSongs()
		{
			//var srcFolder = @"Y:\Music\00 Genres\Dance & House\"; // TEST only //
			var dInfo = new DirectoryInfo(srcFolder);
			var files = dInfo.EnumerateFiles("*", SearchOption.AllDirectories).Where(x => supportedExtensions.Contains(x.Extension, StringComparer.OrdinalIgnoreCase) && !folderExclusions.Contains(x.Directory.Name, StringComparer.OrdinalIgnoreCase) && !folderExclusions.Contains(x.Directory.Parent.Name, StringComparer.OrdinalIgnoreCase)).ToList();

			for (int i = 0; i < files.Count; i++)
				allSongs.Add(new SongFileInfo(files[i]));
		}

		private static void RemoveExclusionArtists()
		{
			allSongs = allSongs.Except(allSongs.Where(x => badArtists.Contains(x.Artist, StringComparer.OrdinalIgnoreCase)).ToList()).ToList();
		}

		private static void CreateGoodList()
		{
			goodList = allSongs.Where(x => goodSongs.Contains(x.Title, StringComparer.OrdinalIgnoreCase)).ToList();
			allSongs = allSongs.Except(goodList).ToList();
		}

		private static void CreateNewList()
		{
			newList = allSongs.Where(x => newSongThreshold < x.Date).ToList();
			allSongs = allSongs.Except(newList).ToList();

			#region Logging
			if (consoleOut) {
				Console.Write("\n{0}: {1} songs", newSongThreshold.ToShortDateString(), newList.Count);
				Console.Write("\n");
				Console.Write("\n NewList.Count: {0}", newList.Count);
				Console.Write("\nAllSongs.Count: {0}", allSongs.Count);
			}
			#endregion
		}

		private static void CreateNewPlusGoodList()
		{
			newPlusGoodList.AddRange(newList);
			newPlusGoodList.AddRange(goodList);
			newPlusGoodList = newPlusGoodList.OrderBy(x => x.Title).ThenBy(y => y.Artist).ToList();

			if (consoleOut) {
				Console.Write("\n");
				for (int i = 0; i < newPlusGoodList.Count; i++)
					Console.Write("\n{0}) {1} - {2}", i + 1, newPlusGoodList[i].Artist, newPlusGoodList[i].Title);

				Console.Write("\n");
				Console.Write("\nNewPlusGoodList.Count: {0}", newPlusGoodList.Count);
				Console.Write("\n       AllSongs.Count: {0}", allSongs.Count);
			}
		}

		private static void CreatePlaylist()
		{
			var newPlusGoodListCount = newPlusGoodList.Count;
			if (consoleOut) {
				Console.Write("\n\n\n\n\n\n\n");
				Console.Write("\nnewPlusGoodListCount: {0}", newPlusGoodListCount);
			}

			allSongs = allSongs.OrderBy(x => x.Title).ThenBy(y => y.Artist).ToList();

			for (int i = 0; i < allSongs.Count - 2; i++) {
				playlist.Add(newPlusGoodList[i % newPlusGoodListCount]);
				playlist.Add(allSongs[i]);
				playlist.Add(allSongs[i + 1]);
				playlist.Add(allSongs[i + 2]);
				i = i + 2;
			}

			if (consoleOut) {
				Console.Write("\n");
				var temp = newPlusGoodListCount * 2;
				for (int i = 0; i < playlist.Count; i++) {
					if (i % temp == 0 && i / temp > 0)
						Console.Write("\n\n============================\n");
					Console.Write("\n{0}) {1} - {2}", i + 1, playlist[i].Artist, playlist[i].Title);
				}
			}
		}

		private static void WritePlaylistITunes(List<SongFileInfo> songs, string filename, bool timestamp = true)
		{
			if (timestamp)
				filename += DateTime.Now.ToString("yyyy MMdd HHmm ssff");
			filename += ".txt";
			var dest = playlistFolder + filename;
			var fileHeader = "Name\tArtist\tComposer\tAlbum\tGrouping\tGenre\tSize\tTime\tDisc Number\tDisc Count\tTrack Number\tTrack Count\tYear\tDate Modified\tDate Added\tBit Rate\tSample Rate\tVolume Adjustment\tKind\tEqualizer\tComments\tPlays\tLast Played\tSkips\tLast Skipped\tMy Rating\tLocation";
			File.WriteAllText(dest, fileHeader, Encoding.Default);

			foreach (var song in songs)
				File.AppendAllText(dest, "\n" + song.Title + "\t" + song.Artist + "\t\t" + song.Album + "\t\t" + song.Genre + "\t\t" + song.Duration + "\t" + song.DiscNum + "\t1\t" + song.TrackNum + "\t\t" + song.Year + "\t\t\t\t\t\t\t\t\t0\t\t\t\t\t" + song.Path, Encoding.Default);

			File.AppendAllText(dest, "\n", Encoding.Default);
		}

		private static void WritePlaylistM3U(List<SongFileInfo> songs, string filename, bool timestamp = true)
		{
			if (timestamp)
				filename += DateTime.Now.ToString("yyyy MMdd HHmm ssff");
			filename += ".m3u";
			var dest = playlistFolder + filename;
			var fileHeader = "#EXTM3U";
			File.WriteAllText(dest, fileHeader, Encoding.Default);

			foreach (var song in songs) {
				File.AppendAllText(dest, "\n#EXTINF:" + song.Duration + "," + song.Artist + " - " + song.Title, Encoding.Default);
				File.AppendAllText(dest, "\n" + song.Path, Encoding.Default);
			}

			File.AppendAllText(dest, "\n", Encoding.Default);
		}

		private static void WriteHtmlFile(List<SongFileInfo> songs, string filename, bool timestamp = true)
		{
			//var url = "https://musicbrainz.org/ws/2/release?query=";
			var googleUrl = "https://www.google.com/search?q=";
			songs = songs.OrderBy(x => x.Title).ThenBy(y => y.Artist).ToList();

			if (timestamp)
				filename += DateTime.Now.ToString("yyyy MMdd HHmm ssff");
			filename += ".html";
			var dest = playlistFolder + filename;
			var fileHeader = "";
			File.WriteAllText(dest, fileHeader, Encoding.Default);

			foreach (var song in songs) {
				var comment = song.Comment ?? "";
				if (!comment.Contains("Release")) {
					File.AppendAllText(dest, String.Format("{0} - {1}<br/>", song.Artist, song.Title), Encoding.Default);
					File.AppendAllText(dest, String.Format("<a href='{0}{1} {2} song wiki' target=\"_blank\">Google</a><br/>", googleUrl, WebUtility.UrlEncode(song.Title), WebUtility.UrlEncode(song.Artist)), Encoding.Default);
					//File.AppendAllText(dest, String.Format("<a href='{0}\"{1}\" AND artist:\"{2}\" AND primarytype:single' target=\"_blank\">MusicBrainz</a><br/>", url, song.Title.Replace("'", "%27").Replace("&", "%26"), song.Artist.Replace("&", "%26").Replace("'", "%27")), Encoding.Default);
					//File.AppendAllText(dest, String.Format("<a href='{0}\"{1}\" AND artistname:\"{2}\" AND primarytype:single' target=\"_blank\">MusicBrainz</a><br/><br/>", url, song.Title.Replace("&", "%26").Replace("'", "%27"), song.Artist.Replace("&", "%26").Replace("'", "%27")), Encoding.Default);
				}
			}
		}

		#region StartProgram & EndProgram
		private static void StartProgram(string[] args)
		{
			if (args != null && args.Length > 0)
				if (args.Contains("-cons", StringComparer.OrdinalIgnoreCase))
					consoleOut = true;

			if (consoleOut) {
				Console.Clear();
				Console.Write("Program started at: {0}\n", startTime.ToString(EN_US).ToLower());
				Console.Write("        consoleOut: {0}\n", consoleOut.ToString().ToUpper());
			}
		}

		private static void EndProgram()
		{
			var endTime = DateTime.Now;
			var ts = endTime - startTime;

			if (consoleOut) {
				Console.Write("\n");
				Console.Write("\nProgram ended at: {0}", endTime.ToString(EN_US).ToLower());
				Console.Write("\nIt took: ");
				if (ts.TotalMinutes >= 60)
					Console.Write("{0}hr ", ts.Hours);
				if (ts.TotalSeconds >= 60)
					Console.Write("{0}min ", ts.Minutes);
				Console.Write("{0}sec to complete", ts.Seconds);
				Console.Write("\n");
				Console.Write("\n... Press any key to exit ...");
				Console.ReadKey(true);
			}
		}
		#endregion StartProgram & EndProgram
		#endregion
	}
}