
using iTunesLib;

namespace jp.osakana4242.itunes_furikake
{
	public struct TrackID
	{
		public readonly int highID;
		public readonly int lowID;
		public TrackID(int high, int low)
		{
			this.highID = high;
			this.lowID = low;
		}
	}

	public static class iTunesAppExt
	{
		public static TrackID GetTrackID_ext(this iTunesApp self, object obj)
		{
			int highID;
			int lowID;
			self.GetITObjectPersistentIDs(ref obj, out highID, out lowID);
			return new TrackID(highID, lowID);
		}
	}

	public static class IITTrackCollectionExt
	{

		public static IITTrack GetItemByTrackID_ext(this IITTrackCollection self, in TrackID id)
		{
			return self.ItemByPersistentID[id.highID, id.lowID];
		}
	}

}
