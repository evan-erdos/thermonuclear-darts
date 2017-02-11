using UnityEngine;
using System.Collections;

public class ExplosionAdvanced : MonoBehaviour {

	bool wait, startingWait;
	[SerializeField] GameObject explosion;
	[SerializeField] ParticleSystem explosionParticleSystem;
	GameObject[] explosions;
	ParticleSystem.Particle[] particles;

	int Count { get { return explosionParticleSystem.particleCount; } }

	void Awake() {
		particles = new ParticleSystem.Particle[Count];
		explosions = new GameObject[Count];
		var n = explosionParticleSystem.GetParticles(particles);
		print(n);
	}

	IEnumerator Start() {
		yield return new WaitForSeconds(0.25f);
		var n = explosionParticleSystem.GetParticles(particles);
		print(n);
		for (var i=0; i<n; i++) {
			var instance = Instantiate<GameObject>(explosion);
			instance.transform.parent = transform;
			explosions[i] = instance; 
		}
		StartCoroutine(Starting());
		StartCoroutine(Fireballs());
		startingWait = true;
	}

	IEnumerator Fireballs() {
		while (true) {
			yield return null;
			var n = explosionParticleSystem.GetParticles(particles);
			for (var i=0; i<n; i++)
				if (i<explosions.Length)
					CalculateParticle(particles[i], explosions[i]);
		}
	}

	void CalculateParticle(ParticleSystem.Particle particle, GameObject explosion) {
		explosion.transform.localPosition = particle.position;
		explosion.transform.localRotation = Quaternion.Euler(particle.rotation3D);
		explosion.transform.localScale = particle.GetCurrentSize3D(explosionParticleSystem);
	}


	IEnumerator Starting() {
		var light = GetComponent<Light>();
		if (light) {
				
			while (light.intensity<7f) {
				yield return null;
				light.intensity += Time.deltaTime * 6;
			}
			yield return new WaitForSeconds(1);
			wait = true;
			while (light.intensity>float.Epsilon) {
				yield return null;
				light.intensity -= Time.deltaTime;

			}
		}

		Destroy(gameObject);

	}

	void Update() {
		if (!startingWait) return;
		var time = Time.time;
		var vector = explosions[0].GetComponent<Renderer>().sharedMaterial.GetVector("_ChannelFactor");
		var factor = Time.deltaTime;

		if (wait) {
			vector.x += factor;
			vector.y += factor;
			vector.z += factor;
			//factor = 1f;
			if (vector.z>2222)
				foreach (var explosion in explosions)
					Destroy(explosion.gameObject);
		} else vector = new Vector4(0f,0f,0f,0f);

		var r = Mathf.Sin((time) * (2 * Mathf.PI)) * 0.5f + 0.25f;
		var g = Mathf.Sin((time + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
		var b = Mathf.Sin((time + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;

		if (!wait) {
			var correction = 1f / (r + g + b);
			r *= correction;
			g *= correction;
			b *= correction;
		}
		
		vector.x += r;
		vector.y += g;
		vector.z += b;

		explosions[0].GetComponent<Renderer>().sharedMaterial.SetVector("_ChannelFactor", vector);
	}
}