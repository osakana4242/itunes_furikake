
namespace jp.osakana4242.itunes_furikake {
	public struct ProgressPair {
		public float value;
		public float total;

		public ProgressPair(float value, float total) {
			this.value = value;
			this.total = total;
		}
	}

	public static class ProgressPairExtention {
		public static float Normalized(this in ProgressPair self) => self.value / self.total;
		public static float Percentage(this in ProgressPair self) => self.value * 100f / self.total;
	}
}
