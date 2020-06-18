using System;
using Xamarin.Forms;
using CoffeeConnect.Droid;
using Android.Media;
using Android.Content.Res;
using System.Linq.Expressions;

[assembly: Dependency(typeof(AudioService))]
namespace CoffeeConnect.Droid
{
	public class AudioService : IAudio
	{
		MediaPlayer player = new MediaPlayer();
		public AudioService()
		{
		}

		public void PlayAudioFile(string fileName)
		{
			var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);

			player.Prepared += (s, e) =>
			{
				player.Start();
			};
			try
			{
				player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
			}
			catch
			{

			}
			player.Prepare();
		}

		public void StopAudio()
		{
			if(player.IsPlaying)
				player.Stop();
		}
	}
}