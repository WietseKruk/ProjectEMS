using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeConnect
{
	public interface IAudio
	{
		void PlayAudioFile(string fileName);
		void StopAudio();
	}
}
