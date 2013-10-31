using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Threading;

namespace TeaTimer
{
	public partial class TeaTimerWindowController : MonoMac.AppKit.NSWindowController
	{ 
		ITeaList<Tea> teaOptions;
		Thread countDownThread = null;
		ICounter cd = null;

		#region Constructors

		// Called when created from unmanaged code
		public TeaTimerWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TeaTimerWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Call to load from the XIB/NIB file
		public TeaTimerWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{ 
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{ 
			teaOptions = new TeaList();
			teaOptions.DefineTeaVarieties ();

			InitComboBox ();

			//Set the Start Button behaviour
			StartButton.Activated += (object sender, EventArgs e) => {
				StartCountDown ();
			};
		}

		/// <summary>
		/// Init the combo box that holdes the tea varieties
		/// and selects the first item
		/// </summary>
		/// <author>Alexandra Marin</author>
		void InitComboBox ()
		{
			TeaChoicesCombo.UsesDataSource = true;
			TeaChoicesCombo.DataSource = new TeaVarieties (teaOptions.GetTeaNameList());
			TeaChoicesCombo.SelectItem (0);
			TeaChoicesCombo.Editable = false;
		}

		/// <summary>
		/// Starts the count down.
		/// If another countdown is running, it stops it and starts a new one
		/// </summary>
		/// <author>Alexandra Marin</author>
		private void StartCountDown ()
		{  
			InfoLabel.StringValue = "";

			//Get the time 
			string varietyName = TeaChoicesCombo.DataSource.ObjectValueForItem (TeaChoicesCombo, TeaChoicesCombo.SelectedIndex).ToString(); //"Green tea"
			TimeSpan time = teaOptions.GetDurationForTea(varietyName);

			//Close any running threads
			if (countDownThread != null && countDownThread.IsAlive) {
				cd.RequestStop ();
			}

			//Init a new counter
			cd = new CountDown (time, CountdownLabel, InfoLabel);

			//Start the countdown
			countDownThread = new Thread( new ThreadStart (cd.Start) );
			countDownThread.Start ();
		}
	}
}

