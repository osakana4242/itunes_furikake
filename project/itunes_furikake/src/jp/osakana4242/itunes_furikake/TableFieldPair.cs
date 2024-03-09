
namespace jp.osakana4242.itunes_furikake {
	/// <summary>名称とよみがなのペア</summary>
	public readonly struct TrackFieldPair {
		public readonly string field;
		public readonly string sortField;
		public TrackFieldPair(
			string field,
			string sortField
		) {
			this.field = field;
			this.sortField = sortField;
		}

		public TrackFieldPair SetField( string field ) {
			return new TrackFieldPair( field, sortField );
		}

		public TrackFieldPair SetSortField( string sortField ) {
			return new TrackFieldPair( field, sortField );
		}
	}
}
