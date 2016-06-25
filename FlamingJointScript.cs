namespace DestructionEffects
{
    using UnityEngine;

    public class FlamingJointScript : MonoBehaviour
    {
        private GameObject destroyer;

        private float destroyTimerStart;

        private float highestEnergy;

        private readonly float maxCombineDistance = 0.6f;

        private readonly float shrinkRateFlame = 0.35f;

        private readonly float shrinkRateSmoke = 1f;

        public void Start()
        {
            foreach (var otherFlame in FlamingJoints.FlameObjects)
            {
                if (
                    !((this.gameObject.transform.position - otherFlame.transform.position).sqrMagnitude
                      < this.maxCombineDistance * this.maxCombineDistance)) continue;
                Debug.Log("== Flame combined ==");
                Destroy(this.gameObject);
                return;
            }

            
            foreach (var pe in this.gameObject.GetComponentsInChildren<KSPParticleEmitter>())
            {
                var color = pe.material.color;
                color.a = color.a / 2;
                pe.material.SetColor("_TintColor", color);
                pe.force = -FlightGlobals.getGeeForceAtPosition(this.transform.position) / 3;
                if (!(pe.maxEnergy > this.highestEnergy)) continue;
                this.destroyer = pe.gameObject;
                this.highestEnergy = pe.maxEnergy;
            }
            FlamingJoints.FlameObjects.Add(this.gameObject);
        }

        public void FixedUpdate()
        {
            foreach (var pe in this.gameObject.GetComponentsInChildren<KSPParticleEmitter>())
            {
                var shrinkRate = pe.gameObject.name.Contains("smoke") ? this.shrinkRateSmoke : this.shrinkRateFlame;
                pe.maxSize = Mathf.MoveTowards(pe.maxSize, 0, shrinkRate * Time.fixedDeltaTime);
                pe.minSize = Mathf.MoveTowards(pe.minSize, 0, shrinkRate * Time.fixedDeltaTime);
                if (pe.maxSize < 0.1f && pe.gameObject == this.destroyer && this.destroyTimerStart == 0)
                {
                    this.destroyTimerStart = Time.time;
                }

                var lightComponent = pe.gameObject.GetComponent<Light>();

                if (lightComponent != null)
                {
                    lightComponent.intensity = Random.Range(0f, pe.maxSize / 6);
                }
            }

            if (this.destroyTimerStart != 0 && Time.time - this.destroyTimerStart > this.highestEnergy)
            {
                Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            if (FlamingJoints.FlameObjects.Contains(this.gameObject))
            {
                FlamingJoints.FlameObjects.Remove(this.gameObject);
            }
        }
    }
}