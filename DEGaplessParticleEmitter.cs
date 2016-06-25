namespace DestructionEffects
{
    using UnityEngine;

    public class DeGaplessParticleEmitter : MonoBehaviour
    {
        public bool Emit = false;

        public float MaxDistance = 1.1f;

        public Part Part = null;

        public KSPParticleEmitter PEmitter;

        public Rigidbody Rb;

        private void Start()
        {
            this.PEmitter = this.gameObject.GetComponent<KSPParticleEmitter>();
            this.PEmitter.emit = false;

            if (this.Part != null)
            {
                Debug.Log("Part " + this.Part.partName + "'s explosionPotential: " + this.Part.explosionPotential);
            }

            this.MaxDistance = this.PEmitter.minSize / 3;
        }

        private void FixedUpdate()
        {
            if (!this.Emit) return;


            var velocity = this.Part?.GetComponent<Rigidbody>().velocity ?? this.Rb.velocity;
            var originalLocalPosition = this.gameObject.transform.localPosition;
            var originalPosition = this.gameObject.transform.position;
            var startPosition = this.gameObject.transform.position + velocity * Time.fixedDeltaTime;
            var originalGapDistance = Vector3.Distance(originalPosition, startPosition);
            var intermediateSteps = originalGapDistance / this.MaxDistance;

            this.PEmitter.EmitParticle();
            this.gameObject.transform.position = Vector3.MoveTowards(
                this.gameObject.transform.position,
                startPosition,
                this.MaxDistance);
            for (var i = 1; i < intermediateSteps; i++)
            {
                this.PEmitter.EmitParticle();
                this.gameObject.transform.position = Vector3.MoveTowards(
                    this.gameObject.transform.position,
                    startPosition,
                    this.MaxDistance);
            }
            this.gameObject.transform.localPosition = originalLocalPosition;
        }

        public void EmitParticles()
        {
            var velocity = this.Part?.GetComponent<Rigidbody>().velocity ?? this.Rb.velocity;
            var originalLocalPosition = this.gameObject.transform.localPosition;
            var originalPosition = this.gameObject.transform.position;
            var startPosition = this.gameObject.transform.position + velocity * Time.fixedDeltaTime;
            var originalGapDistance = Vector3.Distance(originalPosition, startPosition);
            var intermediateSteps = originalGapDistance / this.MaxDistance;

            //gameObject.transform.position = startPosition;
            for (var i = 0; i < intermediateSteps; i++)
            {
                this.PEmitter.EmitParticle();
                this.gameObject.transform.position = Vector3.MoveTowards(
                    this.gameObject.transform.position,
                    startPosition,
                    this.MaxDistance);
            }
            this.gameObject.transform.localPosition = originalLocalPosition;
        }
    }
}