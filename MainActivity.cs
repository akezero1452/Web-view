using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using webviewUpload;
using Java.Interop;
using Android.Provider;
using Android.Content.PM;

namespace MyWb
{
    [Activity(Label = "E-saan thai silk", Icon = "@drawable/picture", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MyWb : Activity  
	{
		int count = 1;

		IValueCallback mUploadMessage;
		private static int FILECHOOSER_RESULTCODE = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			// Get our button from the layout resource,
			// and attach an event to it
			
			

			var chrome = new FileChooserWebChromeClient ((uploadMsg, acceptType, capture) => {
				mUploadMessage = uploadMsg;
				var i = new Intent (Intent.ActionGetContent);
				i.AddCategory (Intent.CategoryOpenable);
				i.SetType ("image/*");
				StartActivityForResult (Intent.CreateChooser (i, "File Chooser"), FILECHOOSER_RESULTCODE);	
			});

			var webview = this.FindViewById<WebView> (Resource.Id.webView1);
            string fileUrl = "file:///android_asset/HTML5_Demo.html";
            webview.Settings.JavaScriptEnabled = true;
            webview.AddJavascriptInterface(new MyJSInterface(this), "wx");
            webview.SetWebViewClient (new WebViewClient ());
			webview.SetWebChromeClient (chrome);
			webview.Settings.JavaScriptEnabled = true;
			webview.LoadUrl ("https://hostcs.kku.ac.th/2561/583021143-5/esaan/index.php");
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent intent)
		{
			if (requestCode == FILECHOOSER_RESULTCODE) {
				if (null == mUploadMessage)
					return;
				Java.Lang.Object result = intent == null || resultCode != Result.Ok
					? null
					: intent.Data;
				mUploadMessage.OnReceiveValue (result);
				mUploadMessage = null;
			}
		}
	}

    class MyJSInterface : Java.Lang.Object
    {
        Context context;

        public MyJSInterface(Context context)
        {
            this.context = context;
        }
        [Export]
        [JavascriptInterface]
        public void CallCamera()
        {
            Toast.MakeText(context, "กล้องถ่ายรูป", ToastLength.Short).Show();
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            context.StartActivity(intent);
        }
    }
    partial class FileChooserWebChromeClient : WebChromeClient
	{
		Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback;

		public FileChooserWebChromeClient (Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback)
		{
			this.callback = callback;
		}

		//For Android 4.1
		[Java.Interop.Export]
		public void openFileChooser (IValueCallback uploadMsg, Java.Lang.String acceptType, Java.Lang.String capture)
		{
			callback (uploadMsg, acceptType, capture);
		}
	}
}