using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	static SoundManager _instance = null;

	public AudioClip StartClip;
	public AudioClip GameClip;
	public AudioClip EndClip;

	public static AudioSource PlayOneShot2d(AudioClip clip, float volume = 1, float pitch = 1, float pan = 0)
	{
		var newASource = _instance.gameObject.AddComponent<AudioSource>();

		newASource.clip = clip;
		newASource.volume = volume;
		newASource.spatialBlend = 0;
		newASource.pitch = pitch;
		newASource.panStereo = pan;

		newASource.Play();

		Destroy(newASource, clip.length + .1f);

		return newASource;
	}

	void Awake () {
		if (_instance != null && _instance != this) {
			Destroy (gameObject);
			print ("Duplicate music player self-destructing!");
		} else {
			_instance = this;
			GameObject.DontDestroyOnLoad(gameObject);

			_instance.GetComponent<AudioSource>().clip = StartClip;
			_instance.GetComponent<AudioSource>().volume = .4f;
			_instance.GetComponent<AudioSource>().loop = true;
			_instance.GetComponent<AudioSource>().Play();
		}
	}

	void OnLevelWasLoaded(int lvl)
	{
		_instance.GetComponent<AudioSource>().Stop();

		switch (lvl)
		{
			case 0:
				_instance.GetComponent<AudioSource>().clip = StartClip;
				_instance.GetComponent<AudioSource>().volume = .4f;
				break;
			case 1:
				_instance.GetComponent<AudioSource>().clip = GameClip;
				_instance.GetComponent<AudioSource>().volume = .2f;
				break;
			case 2:
				_instance.GetComponent<AudioSource>().clip = EndClip;
				_instance.GetComponent<AudioSource>().volume = .4f;
				break;
		}

		_instance.GetComponent<AudioSource>().Play();
	}
}
