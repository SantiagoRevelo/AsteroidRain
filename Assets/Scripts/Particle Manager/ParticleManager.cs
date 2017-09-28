using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    PARTICLE_DUST
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public int particlesCount = 5;
    public ParticleSystem[] effects;
    ParticlePool particlePool;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            instance = null;
        }

        particlePool = new ParticlePool(effects[0], particlesCount);
    }

    public void playParticle(ParticleType particleType, Vector3 particlePos)
    {
        ParticleSystem particleToPlay = particlePool.getAvailabeParticle((int)particleType);

        if (particleToPlay != null)
        {
            if (particleToPlay.isPlaying)
                particleToPlay.Stop();

            particleToPlay.transform.position = particlePos;
            particleToPlay.Play();
        }

    }
}
