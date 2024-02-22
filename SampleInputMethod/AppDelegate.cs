using AppKit;
using Foundation;
using InputMethodKit;

namespace SampleInputMethod {
[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
  static readonly string ConnectionName = "org.atelierinmu.inputMethod.SampleInputMethod_Connection";
  static readonly string BundleIdentifier = "org.atelierinmu.inputMethod.SampleInputMethod";

  static IMKServer? server;

  public bool EstablishConnection() {
    server = new IMKServer(ConnectionName, BundleIdentifier);
    return server is not null;
  }

  public AppDelegate() {}

  public override void WillFinishLaunching(NSNotification notification) {
    if (!EstablishConnection()) {
      System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
  }

  public override void WillTerminate(NSNotification notification) {}
}
}
