﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Urho.Samples.WinForms
{
	public partial class SamplesForm : Form
	{
		Application currentApplication;
		SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

		public SamplesForm()
		{
			InitializeComponent();
			UrhoEngine.Init(pathToAssets: @"../../Assets");
			var sampleTypes = typeof(Sample).Assembly.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Application)) && t != typeof(Sample))
				.ToArray();
			samplesListbox.DisplayMember = "Name";
			samplesListbox.Items.AddRange(sampleTypes);
			samplesListbox.SelectedIndex = 19; //Water by default
		}

		async void samplesListbox_SelectedIndexChanged(object sender, EventArgs e)
		{
			currentApplication?.Engine.Exit();
			currentApplication = null;
			await semaphoreSlim.WaitAsync();
			var type = (Type) samplesListbox.SelectedItem;
			if (type == null) return;
			urhoSurfacePlaceholder.Controls.Clear(); //urho will destroy previous control so we have to create a new one
			var urhoSurface = new Panel { Dock = DockStyle.Fill };
			urhoSurfacePlaceholder.Controls.Add(urhoSurface);
			await Task.Delay(100);//give some time for GC to cleanup everything
			currentApplication = Application.CreateInstance(type, new ApplicationOptions("Data") { ExternalWindow = urhoSurface.Handle });
			urhoSurface.Focus();
			currentApplication.Run();
			semaphoreSlim.Release();
		}
	}
}