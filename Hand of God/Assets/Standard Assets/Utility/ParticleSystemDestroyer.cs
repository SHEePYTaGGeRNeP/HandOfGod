namespace Assets.Standard_Assets.Utility
{
    using System.Collections;

    using UnityEngine;

    using Random = UnityEngine.Random;

    public class ParticleSystemDestroyer : MonoBehaviour
    {
        // allows a particle system to exist for a specified duration,
        // then shuts off emission, and waits for all particles to expire
        // before destroying the gameObject

        public float minDuration = 8;
        public float maxDuration = 10;

        private float m_MaxLifetime;
        private bool m_EarlyStop;


        private IEnumerator Start()
        {
            var systems = this.GetComponentsInChildren<ParticleSystem>();

            // find out the maximum lifetime of any particles in this effect
            foreach (var system in systems)
            {
                this.m_MaxLifetime = Mathf.Max(system.startLifetime, this.m_MaxLifetime);
            }

            // wait for random duration

            float stopTime = Time.time + Random.Range(this.minDuration, this.maxDuration);

            while (Time.time < stopTime || this.m_EarlyStop)
            {
                yield return null;
            }
            Debug.Log("stopping " + this.name);

            // turn off emission
            foreach (var system in systems)
            {
                system.enableEmission = false;
            }
            this.BroadcastMessage("Extinguish", SendMessageOptions.DontRequireReceiver);

            // wait for any remaining particles to expire
            yield return new WaitForSeconds(this.m_MaxLifetime);

            Destroy(this.gameObject);
        }


        public void Stop()
        {
            // stops the particle system early
            this.m_EarlyStop = true;
        }
    }
}
