
using System.Collections.Generic;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake {
	public sealed class TemporaryListPool<T> {
		public static readonly TemporaryListPool<T> instance_g = new TemporaryListPool<T>();
		object syncObject_ = new object();
		List<ListContainer> pool_ = new List<ListContainer>();

		public ListContainer Alloc(out List<T> outList) {
			ListContainer container;
			lock (syncObject_) {
				var index = pool_.Count - 1;
				if (index < 0) {
					container = new ListContainer(this);
				} else {
					container = pool_[index];
					pool_.RemoveAt(index);
				}
			}
			outList = container.List;
			return container;
		}


		public sealed class ListContainer : System.IDisposable {
			readonly TemporaryListPool<T> parent_;
			readonly List<T> list_;
			bool isActive_;

			public ListContainer(TemporaryListPool<T> parent) {
				parent_ = parent;
				list_ = new List<T>();
				isActive_ = true;
			}

			public void Dispose() {
				if (!isActive_) return;

				lock (parent_.syncObject_) {
					isActive_ = false;
					parent_.pool_.Add(this);
				}
			}

			public List<T> List => list_;
		}
	}
}
