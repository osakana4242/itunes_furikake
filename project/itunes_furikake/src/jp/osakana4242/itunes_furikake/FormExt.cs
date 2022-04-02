
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace jp.osakana4242.itunes_furikake {
	static class FormExt {
		public static IObservable<EventPattern<object>> OnCreatedAsObservableExt(this Form self)
			=> Observable.FromEventPattern(_h => self.HandleCreated += _h, _h => self.HandleCreated -= _h);
		public static IObservable<EventPattern<object>> OnDestroyedAsObservableExt(this Form self)
			=> Observable.FromEventPattern(_h => self.HandleDestroyed += _h, _h => self.HandleDestroyed -= _h);
	}

}
